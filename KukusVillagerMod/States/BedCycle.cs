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
        Piece piece;
        VillagerLifeCycle villager;
        private ZNetView znv;

        public string villagerName;
        public string uid;
        bool doneOnce = false;
        bool respawnTimerActive = false;


        private void Awake()
        {
            piece = GetComponent<Piece>();
        }

        private void OnDestroy()
        {
            Global.bedStates.Remove(this);
        }

        private void FixedUpdate()
        {
            if (!piece) return;
            if (!KukusVillagerMod.isMapDataLoaded) return;
            /*
             * 1. Check if the piece is placed and is placed by player
             * 2. If placed then do important operations ONCE such as loading uid and setting zdo as persistent and finding/loading villager
             */
            if (piece.IsPlacedByPlayer())
            {
                if (!Global.bedStates.Contains(this))
                    Global.bedStates.Add(this);
                if (doneOnce)
                {
                    //Keep checking if villager is dead or not, if dead we respawn a new one
                    if (villager)
                    {

                    }
                    else
                    {
                        StartRespawnTimerAndRespawn();
                    }
                }
                else
                {
                    //Not done yet so lets do our thing and change doneOnce to true
                    znv = GetComponentInParent<ZNetView>();
                    znv.GetComponentInChildren<ZNetView>().SetPersistent(true);
                    LoadUID();
                    FindOrSpawnVillager();
                    doneOnce = true;
                }
            }

        }

        async void StartRespawnTimerAndRespawn()
        {
            if (respawnTimerActive) return;
            respawnTimerActive = true;
            await Task.Delay(10000);
            FindOrSpawnVillager();
            respawnTimerActive = false;
        }

        private void LoadUID()
        {
            uid = znv.GetComponentInChildren<ZNetView>().GetZDO().GetString(Util.bedID);

            if (uid == null || uid.Trim().Length == 0)
            {
                //Create a new UID and save it
                string guid = System.Guid.NewGuid().ToString();
                znv.GetComponentInChildren<ZNetView>().GetZDO().Set(Util.bedID, guid);
                uid = znv.GetComponentInChildren<ZNetView>().GetZDO().GetString(Util.bedID);
            }
        }
        private void FindOrSpawnVillager()
        {
            FindVillager();
            if (villager == null)
                SpawnVillager();
        }

        private void FindVillager()
        {
            KLog.warning($"Finding Villager for bed {uid}");
            foreach (var v in FindObjectsOfType<VillagerLifeCycle>())
            {
                if (v == null) continue; //Very unlikely but safety first
                string bedIDofVil = v.GetComponentInParent<ZNetView>().GetZDO().GetString(Util.bedID);
                if (bedIDofVil == null || bedIDofVil.Trim().Length == 0) continue;
                if (bedIDofVil.Equals(uid))
                {
                    KLog.warning($"Bed {uid} has found villager {v.uid}");
                    PostVillagerTasks(v.gameObject);
                    return;
                }
            }
        }

        private void SpawnVillager()
        {
            GameObject villagerPrefab = CreatureManager.Instance.GetCreaturePrefab(villagerName);
            var villagerCreature = SpawnSystem.Instantiate(villagerPrefab, transform.position, transform.rotation);
            KLog.warning($"Bed {uid} has SPAWNED villager {villagerCreature.GetComponent<VillagerLifeCycle>().uid}");
            PostVillagerTasks(villagerCreature);
        }

        private void PostVillagerTasks(GameObject v)
        {
            //Tame
            var tame = v.GetComponent<Tameable>();
            if (tame == null)
            {
                tame = v.GetComponentInParent<Tameable>();

                if (tame)
                {
                    tame.Tame();
                }
                else
                {
                    KLog.warning("TAMING COMPONENT NOT FOUND!!!!!!!!!!!!!!!!!!!");
                }
            }
            else
            {
                tame.Tame();
            }


            //Set reference of villager
            this.villager = v.GetComponent<VillagerLifeCycle>();

            //Save the villagerID in the bed
            znv.GetZDO().Set(Util.villagerID, this.villager.uid);
            KLog.warning($"Bed {uid} saved villager {znv.GetZDO().GetString(Util.villagerID)}");

        }
    }
}
