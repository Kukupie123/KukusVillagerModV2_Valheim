﻿using Jotunn.Managers;
using KukusVillagerMod.enums;
using System;
using UnityEngine;
namespace KukusVillagerMod.States
{
    //Based off of CreatureSpawner of valheim
    class BedVillagerProcessor : MonoBehaviour, Hoverable, Interactable
    {
        private ZNetView znv;
        public float respawnTimeInMinute = 1f; //The respawn time for the villager. Has to be set during prefab creation
        public string VillagerPrefabName; //Name of the villager it has to spawn, has to be set during prefab creation
        private Piece piece;

        /*
         * When assigning posts and containers for a bed we need to keep track of the bed we interacted with. We store that bed's ZDO in this static variable
         */
        public static ZDOID? SELECTED_BED_ID;


        private void Awake()
        {
            piece = base.GetComponent<Piece>();
        }

        
        bool fixedUpdateRanOnce = false; //Used to determine if we have ran the fixed update atleast once, we need to perform few actions during the first update call and then never perform them. We use this boolean to determine it.
        private void FixedUpdate()
        {
            if (!piece || KukusVillagerMod.isMapDataLoaded == false) return; //Map data has to be loaded before we can proceed

            if (piece.IsPlacedByPlayer())
            {
                if (fixedUpdateRanOnce == false)

                {
                    //Piece needs to be placed before ZNetView is Valid so we have to check if it has been placed every frame and run the codes below once
                    this.znv = base.GetComponent<ZNetView>();
                    this.znv.SetPersistent(true);

                    //Load/Create Villager's state (Guarding, Defending etc)

                    if (znv.GetZDO() == null)
                    {
                        fixedUpdateRanOnce = true;
                        return;
                    }
                    base.InvokeRepeating("UpdateSpawner", 2f, 5f);

                    fixedUpdateRanOnce = true;
                }

            }
        }

        /// <summary>
        /// Determines if we need to spawn a new villager or not.
        /// </summary>
        private void UpdateSpawner()
        {
            if (!this.znv.IsOwner())
            {
                return;
            }

            ZDOID villagerZDOID = this.znv.GetZDO().GetZDOID("spawn_id");

            //If respawn timer is less than 0 and villager is valid we simply return. IDK why tho, this is from the game's official CreatureSpawner class
            if (this.respawnTimeInMinute <= 0f && !villagerZDOID.IsNone())
            {
                return;
            }

            //If villager is valid and the ZDO of the villager we got from villagerZDOID is valid we update the alive time variable. Pretty sure we do not need this. Part of official code
            if (!villagerZDOID.IsNone() && ZDOMan.instance.GetZDO(villagerZDOID) != null)
            {
                this.znv.GetZDO().Set("alive_time", ZNet.instance.GetTime().Ticks);
                return;
            }


            //If respawnTimerInMin is greater than 0 we check the alive_time count. And if alive time - currentTime is invalid then we can assume that the villager is dead and spawn if enough time has passed after the villager died
            if (this.respawnTimeInMinute > 0f)
            {
                DateTime time = ZNet.instance.GetTime();
                DateTime d = new DateTime(this.znv.GetZDO().GetLong("alive_time", 0L));
                if ((time - d).TotalMinutes < (double)this.respawnTimeInMinute)
                {
                    return;
                }
            }


            this.Spawn();

        }


        /// <summary>
        /// Checks if this bed has a villager spawned
        /// </summary>
        /// <returns>true if it has a villager </returns>
        private bool HasSpawned()
        {
            return !(this.znv == null) && this.znv.GetZDO() != null && !this.znv.GetZDO().GetZDOID("spawn_id").IsNone();
        }


        /// <summary>
        /// Spawns a villager and saves essential data
        /// </summary>
        /// <returns></returns>
        private ZNetView Spawn()
        {
            KLog.warning("Spawning CREATURE!");
            Vector3 position = base.transform.position;
            float y;

            //Finding the best spawn location
            if (ZoneSystem.instance.FindFloor(position, out y))
            {
                position.y = y;
            }

            
            var villagerPrefab = CreatureManager.Instance.GetCreaturePrefab(this.VillagerPrefabName); //Getting prefab of the villager
            Quaternion rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f); //Random rotation along the YAW
            GameObject villager = UnityEngine.Object.Instantiate<GameObject>(villagerPrefab, position, rotation); //spawning a villager


            ZNetView component = villager.GetComponent<ZNetView>();
            BaseAI component2 = villager.GetComponent<BaseAI>();
            Tameable tameable = villager.GetComponent<Tameable>();
            tameable.Tame(); //Taming the spawned villager

            component.GetZDO().Set("spawner_id", this.znv.GetZDO().m_uid); //Save the bed's ID in the villager's ZDO
            component.GetZDO().SetPGWVersion(this.znv.GetZDO().GetPGWVersion()); //not sure that this is for
            this.znv.GetZDO().Set("spawn_id", component.GetZDO().m_uid); //Save the villager's ID in this bed's ZDO
            this.znv.GetZDO().Set("alive_time", ZNet.instance.GetTime().Ticks); //Save alive time in this bed's ZDO


            //Based on the state stored in zdo we are going to make the villager do stuff
            switch (GetVilState())
            {
                case VillagerState.Guarding_Bed:
                    villager.GetComponent<VillagerLifeCycle>().GuardBed();
                    break;
                case VillagerState.Defending_Post:
                    villager.GetComponent<VillagerLifeCycle>().DefendPost();
                    break;
                case VillagerState.Following:
                    villager.GetComponent<VillagerLifeCycle>().GuardBed(); //TODO: Save follower's ID to follow after respawn
                    break;
            }
            return component;
        }


        /// <summary>
        /// Returns an enum which is the state the villager should be at. If none found it will create a new one, save it in the zdo and return it back
        /// </summary>
        /// <returns>State of the villager</returns>
        public VillagerState GetVilState()
        {
            return (VillagerState)this.znv.GetZDO().GetInt("state", (int)VillagerState.Guarding_Bed);
        }
        public void UpdateVilState(VillagerState state)
        {
            this.znv.GetZDO().Set("state", (int)state);
        }

        //Interface

        public string GetHoverText()
        {
            string bedID = this.znv.GetZDO().m_uid.id.ToString();

            var villagerID = this.znv.GetZDO().GetZDOID("spawn_id");
            string villager = "None";
            if (!villagerID.IsNone())
            {
                villager = villagerID.id.ToString();
            }

            string containerID = "None";

            var defenseID = this.znv.GetZDO().GetZDOID("defense");
            string defense = "None";
            if (!defenseID.IsNone()) defense = defenseID.id.ToString();

            return $"Bed ID : {bedID}\nVillager ID {villager}\nContainer ID : {containerID}\nDefense Post ID :{defense}";
        }

        public string GetHoverName()
        {
            return name;
        }

        /// <summary>
        /// Interacting with the bed will save the bed's ZDO to the static variable SELECTED_BED_ZDO and will be used for setting defenses and containers assigned to the bed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="hold"></param>
        /// <param name="alt"></param>
        /// <returns></returns>
        public bool Interact(Humanoid user, bool hold, bool alt)
        {
            if (!hold) //Save bed in ZDO of user temporarily, when interacted with defense post, we will make use of this bed
            {
                SELECTED_BED_ID = this.znv.GetZDO().m_uid;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Bed {SELECTED_BED_ID.Value.id} selected. Interact with a Defense to let the villager know where to defend");
                return true;
            }
            return false;
        }

        //Does nothing as of now
        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return true;
        }
    }
}
