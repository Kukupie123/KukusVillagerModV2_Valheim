using Jotunn.Entities;
using Jotunn.Managers;
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
                    if (KukusVillagerMod.isMapDataLoaded == false || ZNetScene.instance.IsAreaReady(transform.position) == false) return;

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
                if (ZNetScene.instance.IsAreaReady(v.transform.position) == false || v == null || v.GetUID() == null) continue;

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
            KLog.warning($"creating villager for bed {GetUID()}");

            var prefab = CreatureManager.Instance.GetCreaturePrefab(villagerName);
            var villager = SpawnSystem.Instantiate(prefab, transform.position, transform.rotation);
            this.villager = villager.GetComponent<VillagerLifeCycle>();
            this.villager.bed = this;
            SaveVillager(this.villager); //Save the villager's ID before activating the villager
            villager.GetComponent<Tameable>().Tame();
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
