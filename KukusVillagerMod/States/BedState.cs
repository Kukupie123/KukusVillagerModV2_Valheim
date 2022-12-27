using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace KukusVillagerMod.States
{
    class BedState : MonoBehaviour
    {
        public string villagerID; //The id of the villager that it will spawn
        Piece piece; //Piece component to determine if placed or not
        public VillagerState villagerState; //The Villager who spawned/Belongs to this bed
        public string uid = null; //UID of the bed. Will be generated or loaded

        //Other vars
        bool placed = false; //To know if the object is placed or not
        int respawnTimer = 2000; //Used to respawn after a certain delay
        bool respawnTimerActive = false; //Respawn system
        bool firstRespawnCountdown = true; //Respawn System
        private void Awake()
        {
            piece = GetComponent<Piece>();
        }

        private void FixedUpdate()
        {
            if (!piece)
            {
                KLog.warning("Piece is not valid yet");
                return;
            }

            //Else should trigger only once.
            if (piece.IsPlacedByPlayer())
            {
                if (placed)
                {
                    if (Global.bedStates.Contains(this) == false)
                        Global.bedStates.Add(this);

                    if (villagerState) //If villagerState is valid then we can't spawn
                    {
                        // maybe something in future
                    }
                    else
                    {
                        //Check if respawn counter is active, if not then start it 
                        if (!respawnTimerActive)
                        {
                            startRespawnCountdown();
                        }

                    }
                }

                else
                {
                    placed = true;
                    KLog.info("Bed placed. Trying to load UID NOW!");
                    loadUID();
                }


            }


        }

        private void OnDestroy()
        {
            Global.bedStates.Remove(this);
        }


        async void startRespawnCountdown()
        {
            //if uid valid then only try to spawn
            if (uid == null) return;

            KLog.info("Respawn Timer started");
            if (firstRespawnCountdown) respawnTimer = 5000; //To let the game load the monsters initially. TODO: Make it event based
            else respawnTimer = 10000;
            firstRespawnCountdown = false;
            respawnTimerActive = true;
            await Task.Delay(respawnTimer);
            SpawnVillager();
            respawnTimerActive = false;
            KLog.info("Respawn Timer Ended");
        }



        private void loadUID()
        {
            GetComponentInParent<ZNetView>().SetPersistent(true); // Makes sure that the ZDO persists after game is closed

            //Try to load the UID
            uid = GetComponentInParent<ZNetView>().GetZDO().GetString(Util.bedID);

            //failed to load create new one
            if (uid == null || uid.Trim().Length == 0)
            {
                string guid = System.Guid.NewGuid().ToString();
                GetComponentInParent<ZNetView>().GetZDO().Set(Util.bedID, guid);
                uid = GetComponentInParent<ZNetView>().GetZDO().GetString(Util.bedID);
                KLog.warning($"Failed to load UID for Bed, Created and saved new bedID : {guid}");
            }
            else
            {
                KLog.warning($"Found bed ID  : {uid}");

            }
        }

        private void PostVillagerSet()
        {
            if (villagerState == null) return;
            //Remove comps that we do not need
            DestroyImmediate(villagerState.GetComponent<NpcTalk>());
            DestroyImmediate(villagerState.GetComponent<CharacterDrop>());
            DestroyImmediate(villagerState.GetComponentInParent<CharacterDrop>()); // remove item drops

            //After spawning we need to set the bedID
            villagerState.SetBed(this);

            //Get taming component and tame
            villagerState.GetComponentInParent<Tameable>().Tame();
        }

        /// <summary>
        /// tries to load villager, if failed will spawn a new one
        /// </summary>
        private void SpawnVillager()
        {
            if (uid == null || villagerState != null || villagerID == null)
            {
                KLog.warning("uid missing or villagerState already found or villagerID invalid");
            }


            FindVillager();
            if (villagerState != null)
            {
                KLog.warning($"BED : Found villager with id : {villagerState.uid}");
                return;
            }

            GameObject villagerCreaturePrefab = CreatureManager.Instance.GetCreaturePrefab(villagerID);
            var villagerCreature = SpawnSystem.Instantiate(villagerCreaturePrefab, transform.position, transform.rotation);

            //Store the reference of villagerState
            villagerState = villagerCreature.GetComponent<VillagerState>();

            PostVillagerSet();

            //Save the uid of villager in zdo of bed for persistence
            GetComponentInParent<ZNetView>().GetZDO().Set(Util.villagerID, villagerState.uid);
            KLog.info($"BED : Spawned new villager and saved its id in zdo {villagerState.uid}");

        }

        //Find villager with key {bedID : uid}
        private void FindVillager()
        {
            //Tries to load villager based on data saved in zdo

            var vv = FindObjectsOfType<VillagerState>();

            if (vv == null) return;

            foreach (var v in vv) //nice naming 
            {
                try
                {

                    string localBedID = v.GetComponent<ZNetView>().GetZDO().GetString(Util.bedID);

                    if (localBedID == null || localBedID.Trim().Length == 0) continue;

                    if (localBedID.Equals(uid))
                    {
                        this.villagerState = v;
                        PostVillagerSet();
                        return;
                    }
                }
                catch (UnityException e)
                {
                    KLog.warning($"ERROR at find villager in bedstate : {e.Message}");
                }

            }
        }


    }
}
