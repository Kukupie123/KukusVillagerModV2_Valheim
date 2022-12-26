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





        Piece piece;
        VillagerState villagerState;


        public string uid = null; //UID of the bed

        //Other vars
        bool placed = false; //To know if the object is placed or not
        int respawnTimer = 2000; //Used to respawn after a certain delay
        bool respawnTimerActive = false;
        bool canSpawn = true;
        bool firstRespawnCountdown = true;
        private void Awake()
        {
            piece = GetComponent<Piece>();
        }

        private void FixedUpdate()
        {
            if (!piece) return;


            //Should only trigger ONCE. Which is when placed or loaded first time in game
            if (piece.IsPlacedByPlayer())
            {
                if (placed) return;
                placed = true;
                loadUID();

            }
            if (!placed) return;

            if (!Global.bedStates.Contains(this))
                Global.bedStates.Add(this);

            if (villagerState) //If villagerState is valid then we can't spawn
            {
                canSpawn = false;
            }
            else
            {
                //Check if respawn counter is active
                if (respawnTimerActive)
                {
                    //if respawnTimer is active then check for canSpawn bool to be true, as respawn timer is complete canRespawn will be true
                    if (canSpawn)
                    {
                        SpawnVillager();
                        respawnTimerActive = false;
                    }
                }
                else
                {
                    //Respawn timer is not active so we need to start countdown
                    respawnTimerActive = true;
                    startRespawnCountdown();
                }
            }
        }

        private void OnDestroyed()
        {
            Global.bedStates.Remove(this);
        }


        async void startRespawnCountdown()
        {
            if (firstRespawnCountdown) respawnTimer = 2000; //To let the game load the monsters initially. TODO: Make it event based
            else respawnTimer = 10000;
            firstRespawnCountdown = false;
            respawnTimerActive = true;
            canSpawn = false;
            await Task.Delay(respawnTimer);
            canSpawn = true;
            respawnTimerActive = false;
        }



        private void loadUID()
        {
            GetComponentInParent<ZNetView>().SetPersistent(true); // Makes sure that the ZDO persists after game is closed

            //Try to load the UID
            uid = GetComponentInParent<ZNetView>().GetZDO().GetString(Util.bedID);

            //failed to load create new one
            if (uid == null)
            {
                string guid = System.Guid.NewGuid().ToString();
                GetComponentInParent<ZNetView>().GetZDO().Set(Util.bedID, guid);
                uid = GetComponentInParent<ZNetView>().GetZDO().GetString(Util.bedID);
                KLog.warning($"Failed to load UID for Bed, Created and saved new bedID : {guid}");
            }
        }

        /// <summary>
        /// tries to load villager, if failed will spawn a new one
        /// </summary>
        private void SpawnVillager()
        {
            if (uid == null) return;
            if (villagerState != null) return;
            if (villagerID == null) return;

            LoadVillager();
            if (villagerState != null)
            {
                KLog.info($"Found villager with id : {villagerState.uid}");
                return;
            }

            GameObject villagerCreaturePrefab = CreatureManager.Instance.GetCreaturePrefab(villagerID);
            var villagerCreature = SpawnSystem.Instantiate(villagerCreaturePrefab, transform.position, transform.rotation);

            //Remove comps that we do not need
            DestroyImmediate(villagerCreature.GetComponent<NpcTalk>());
            DestroyImmediate(villagerCreature.GetComponent<CharacterDrop>());
            DestroyImmediate(villagerCreature.GetComponentInParent<CharacterDrop>()); // remove item drops


            //Store the reference of villagerState
            villagerState = villagerCreature.GetComponent<VillagerState>();

            //Save the uid of villager in zdo for persistence
            GetComponentInParent<ZNetView>().GetZDO().Set(Util.villagerID, villagerState.uid);
            KLog.info($"Spawned new villager and saved its id in zdo {villagerState.uid}");

        }

        private void LoadVillager()
        {
            //Tries to load villager based on data saved in zdo

            var vv = FindObjectsOfType<VillagerState>();

            if (vv == null) return;

            foreach (var v in vv) //nice naming bro
            {
                string localBedID = v.GetComponentInParent<ZNetView>().GetZDO().GetString(Util.bedID);

                if (localBedID == null) continue;

                if (localBedID.Equals(uid))
                {
                    this.villagerState = v;
                    return;
                }
            }
        }


    }
}
