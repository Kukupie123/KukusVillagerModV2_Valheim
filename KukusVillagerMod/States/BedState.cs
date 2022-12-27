using Jotunn.Managers;
using KukusVillagerMod.Datas;
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

        private void PostVillagerSet(GameObject villager)
        {
            if (villager == null) return;
            //Remove comps that we do not need
            DestroyImmediate(villager.GetComponent<NpcTalk>());
            DestroyImmediate(villager.GetComponent<CharacterDrop>());
            DestroyImmediate(villager.GetComponentInParent<CharacterDrop>()); // remove item drops
            DestroyImmediate(villager.GetComponent<CharacterDrop>()); //Destroy player controller
            DestroyImmediate(villager.GetComponent<PlayerController>()); //Destroy player controller
            DestroyImmediate(villager.GetComponent<Talker>()); //destroy talking comp
            DestroyImmediate(villager.GetComponent<Skills>()); //Disable skils
            DestroyImmediate(villager.GetComponent<Player>()); //Disable skils

            //TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! WE MIGHT NOT NEED TO ADD THIS CHECK LATER

            foreach (var v in villager.GetComponents<VillagerState>())
            {
                DestroyImmediate(v);
            }
            foreach(var v in villager.GetComponentsInParent<VillagerState>())
            {
                DestroyImmediate(v);
            }

            //Get villager data and set its bed
            var vd = villager.GetComponent<VillagerData>();
            vd.SetBed(this); //Set the bed for the villager, this will also save in zdo of the villager

            //Add a villager state component which will take info from villagerData and since it's set already we should be fine
            villager.AddComponent<VillagerState>();
            //Store reference of the character
            villagerState = villager.GetComponent<VillagerState>();

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
                KLog.warning($"BED : Found villager with id : {villagerState.villagerData.uid}");
                return;
            }

            //Get Villager Prefab and Spawn it
            GameObject villagerCreaturePrefab = CreatureManager.Instance.GetCreaturePrefab(villagerID);
            var villagerCreature = SpawnSystem.Instantiate(villagerCreaturePrefab, transform.position, transform.rotation);

            //Call PostVillagerSet method that is going to do some cruicial work
            PostVillagerSet(villagerCreature);

            //Save the uid of villager in zdo of this bed for persistence
            GetComponentInParent<ZNetView>().GetZDO().Set(Util.villagerID, villagerState.villagerData.uid);
            KLog.info($"BED : Spawned new villager and saved its id in zdo {villagerState.villagerData.uid}");

        }

        //Find villager with key {bedID : uid}
        private void FindVillager()
        {
            //Tries to load villager based on data saved in zdo

            var vv = FindObjectsOfType<VillagerData>();

            if (vv == null) return;

            foreach (var v in vv) //nice naming 
            {
                try
                {

                    string localBedID = v.GetComponentInParent<ZNetView>().GetZDO().GetString(Util.bedID);

                    if (localBedID == null || localBedID.Trim().Length == 0) continue;

                    if (localBedID.Equals(uid))
                    {
                        //MIGHT NOT WORK TEST IT OUT
                        PostVillagerSet(v.gameObject);
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
