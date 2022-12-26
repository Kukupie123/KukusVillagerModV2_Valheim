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
        public int villagerLevel; //The level of the creature spawned
        public float villagerHealth; //The health of the villager spawned
        public int villagerType; //1 = melee, 2 = Ranged
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

            //Should only trigger ONCE. Which is when placed or loaded first time in game or when it respawns after going too far
            if (piece.IsPlacedByPlayer())
            {
                if (placed) return;
                placed = true;
                KLog.info("Bed placed. Trying to load UID NOW!");
                loadUID();

            }
            if (!placed) return;

            //Since it's a set we do not have to worry about it being added over and over
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

        private void OnDestroy()
        {
            Global.bedStates.Remove(this);
        }


        async void startRespawnCountdown()
        {
            //if uid valid then only try to spawn
            if (uid == null) return;

            KLog.info("Respawn Timer started");
            if (firstRespawnCountdown) respawnTimer = 3000; //To let the game load the monsters initially. TODO: Make it event based
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
                KLog.warning($"Found villager with id : {villagerState.uid}");
                return;
            }

            /*
             * When you spawn a villager make sure to set it's bedID 
             */
            GameObject villagerCreaturePrefab = CreatureManager.Instance.GetCreaturePrefab(villagerID);
            var villagerCreature = SpawnSystem.Instantiate(villagerCreaturePrefab, transform.position, transform.rotation);

            //Remove comps that we do not need
            DestroyImmediate(villagerCreature.GetComponent<NpcTalk>());
            DestroyImmediate(villagerCreature.GetComponent<CharacterDrop>());
            DestroyImmediate(villagerCreature.GetComponentInParent<CharacterDrop>()); // remove item drops


            //Store the reference of villagerState
            villagerState = villagerCreature.GetComponent<VillagerState>();
            //After spawning we need to set the bedID
            villagerState.SetBed(this);

            //Save the uid of villager in zdo of bed for persistence
            GetComponentInParent<ZNetView>().GetZDO().Set(Util.villagerID, villagerState.uid);
            KLog.info($"Spawned new villager and saved its id in zdo {villagerState.uid}");

        }

        //Find villager with key {bedID : uid}
        private void FindVillager()
        {
            //Tries to load villager based on data saved in zdo

            var vv = FindObjectsOfType<VillagerState>();

            if (vv == null) return;

            foreach (var v in vv) //nice naming bro
            {
                string localBedID = v.GetComponentInParent<ZNetView>().GetZDO().GetString(Util.bedID);

                if (localBedID == null || localBedID.Trim().Length == 0) continue;

                if (localBedID.Equals(uid))
                {
                    this.villagerState = v;
                    return;
                }
            }
        }


    }
}
