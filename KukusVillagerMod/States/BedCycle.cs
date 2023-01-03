using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KukusVillagerMod.States
{
    class BedCycle : MonoBehaviour
    {
        /*
         * When a bed is loaded it will search for villagers near it's area. 
         * If it finds one it will save its reference.
         */
        public string villagerName; //The name of the villager it will spawn
        public int respawnDuration;

        public ZNetView znv;

        Piece piece;
        VillagerLifeCycle villager;

        private bool respawnTimerActive = false;

        private void Awake()
        {
            piece = GetComponent<Piece>();
        }


        private void OnDestroy()
        {
        }



        bool fixedUpdateRan = false;


        private void FixedUpdate()
        {
            if (!ZNet.instance.IsServer()) return;

            if (Player.m_localPlayer == null) return;
            if (piece.IsPlacedByPlayer())
            {
                if (fixedUpdateRan == false)
                {
                    if (KukusVillagerMod.isMapDataLoaded == false || ZNetScene.instance.IsAreaReady(transform.position) == false || Player.m_localPlayer == null) return;

                    fixedUpdateRan = true;
                    znv = GetComponentInParent<ZNetView>();
                    znv.SetPersistent(true);
                    loadOrCreateUID();
                    FindVillager();
                    if (villager == null)
                    {
                        CreateVillager();
                    }
                }
                else
                {
                    //If villager reference gets invalid we are going to start respawning timer
                    if (villager == null)
                    {
                        StartRespawn();
                    }
                }
            }
        }

        void loadOrCreateUID()
        {

            var uid = znv.GetZDO().GetString(Util.uid, null);
            if (uid == null)
            {
                //Create new UID
                uid = System.Guid.NewGuid().ToString();

                znv.GetZDO().Set(Util.uid, uid);
                KLog.warning($"Created UID for bed {GetUID()}");
            }
            {
                KLog.warning($"Found UID for bed {uid}");
            }

        }

        public string GetUID()
        {
            if (znv == null) return null;
            return znv.GetZDO().GetString(Util.uid, null);
        }

        public VillagerState GetVillagerState()
        {
            if (znv == null) return VillagerState.GuardingBed;
            return (VillagerState)znv.GetZDO().GetInt(Util.villagerState, (int)VillagerState.GuardingBed);
        }

        public void UpdateVillagerState(VillagerState villagerState)
        {
            znv.GetZDO().Set(Util.villagerState, (int)villagerState);
        }

        public string GetLinkedVillagerID()
        {
            if (znv == null) return null;
            return znv.GetZDO().GetString(Util.villagerID, null);
        }

        private void SaveVillager(VillagerLifeCycle villager)
        {
            villager.SaveBed(this); //Save this bed in villager's ZDO
            znv.GetZDO().Set(Util.villagerID, villager.GetUID());
        }

        void FindVillager()
        {

            KLog.warning($"Searchig villager STARTED for bed {GetUID()}");

            if (GetLinkedVillagerID() == null) return;

            var villagers = FindObjectsOfType<VillagerLifeCycle>();

            foreach (var v in villagers)
            {
                //Check if area is available or if znv and UID is valid
                if (ZNetScene.instance.IsAreaReady(v.transform.position) == false || v == null || v.GetUID() == null || ZNetScene.instance.IsAreaReady(transform.position) == false) continue;

                if (v.GetUID().Equals(GetLinkedVillagerID()) && v.GetLinkedBedID().Equals(GetUID()))
                {
                    this.villager = v;
                    SaveVillager(v);
                    KLog.warning($"Searchig villager SUCCESS for bed {GetUID()}, FOUND {GetLinkedVillagerID()}");
                    return;
                }
            }
            KLog.warning($"Searchig villager FAILED for bed {GetUID()}");

        }

        void CreateVillager()
        {

            var prefab = CreatureManager.Instance.GetCreaturePrefab(villagerName);
            var villager = SpawnSystem.Instantiate(prefab, transform.position, transform.rotation);
            this.villager = villager.GetComponent<VillagerLifeCycle>();
            this.villager.bed = this;
            SaveVillager(this.villager); //Save the villager's ID before activating the villager
            villager.GetComponent<Tameable>().Tame();
            //Check villagerState enum saved in ZDO. If found we use it to make villager perform it's last action.
            switch (GetVillagerState())
            {
                case VillagerState.GuardingBed:
                    KLog.warning($"Created new villager {this.villager.GetUID()} who is guarding bed {GetUID()}");
                    this.villager.GuardBed();
                    break;
                case VillagerState.GuardingDefensePost:
                    //Defending post can fail sometime

                    //If defending post failed
                    if (!this.villager.DefendPost())
                    {
                        KLog.warning($"Created new villager {this.villager.GetUID()} who is guarding bed {GetUID()}");
                        this.villager.GuardBed();
                    }
                    else
                    {
                        KLog.warning($"Created new Villager {this.villager.GetUID()} who is defending Post for bed {GetUID()}");
                    }
                    break;
            }
        }

        async void StartRespawn()
        {
            if (respawnTimerActive) return;
            respawnTimerActive = true;
            await Task.Delay(respawnDuration);
            FindVillager();
            if (villager == null)
            {
                CreateVillager();
            }
            respawnTimerActive = false;
        }







    }

}
