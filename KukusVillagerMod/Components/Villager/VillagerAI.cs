﻿using Jotunn.Managers;
using KukusVillagerMod.Components;
using KukusVillagerMod.enums;
using KukusVillagerMod.enums.Work_Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KukusVillagerMod.Components.Villager
{
    class VillagerAI : MonoBehaviour
    {
        private VillagerGeneral villagerGeneral;
        private NpcTalk talk;
        private MonsterAI ai;
        bool updateRanOnce = false;
        private ZDOID followingObjZDOID;

        private void Awake()
        {

            updateRanOnce = false;
        }


        private void FixedUpdate()
        {
            //Keeping them in Awake doesn't always guarentee so we do it here instead.
            if (talk == null) talk = GetComponent<NpcTalk>();

            if (villagerGeneral == null) villagerGeneral = GetComponent<VillagerGeneral>();

            if (ai == null) ai = GetComponent<MonsterAI>();



            if (!KukusVillagerMod.isMapDataLoaded || villagerGeneral.isBedAssigned() == false) return;

            if (!updateRanOnce)
            {
                updateRanOnce = true;
                WorkAsync();
                {
                    if (talk == null) talk = GetComponent<NpcTalk>();
                }

                //Get the state of the villager using bed zdo to determine which command to execute
                VillagerState villagerState = villagerGeneral.GetVillagerState();
                switch (villagerState)
                {
                    case VillagerState.Guarding_Bed:
                        GuardBed();
                        break;
                    case VillagerState.Defending_Post:
                        DefendPost();
                        break;
                    case VillagerState.Working:
                        StartWork();
                        break;
                    default:
                        GuardBed();
                        break;
                }
            }
            else
            {
                TPVillagerToFollowerIfNeeded();
                MovePerUpdateIfDesired();
                FollowCheckPerUpdate();
            }

        }



        //Functions that execute every frame---------------------------

        //If we are following a target, this function checks how long have we been following the target for and if it crosses a threshold we tp the villager to that location if acceptable distance has not been reached
        double? startedFollowingTime = null;
        float followTargetAcceptableDistance = 2f;
        private void FollowCheckPerUpdate()
        {
            //Check if followingObjZDOID is valid and that we are not in following state. following state has it's own function to take care of it
            if (this.followingObjZDOID != null && this.followingObjZDOID.IsNone() == false && villagerGeneral.GetVillagerState() != VillagerState.Following && ZDOMan.instance.GetZDO(followingObjZDOID) != null && ZDOMan.instance.GetZDO(followingObjZDOID).IsValid())
            {
                float distance = Vector3.Distance(transform.position, ZDOMan.instance.GetZDO(followingObjZDOID).GetPosition());

                //If distance is acceptable then we do not proceed
                if (distance < followTargetAcceptableDistance) return;

                //Check if we started counting already
                if (startedFollowingTime == null)
                {
                    startedFollowingTime = ZNet.instance.GetTimeSeconds();
                }

                //Calculate time elasped
                double timeElapsedSec = ZNet.instance.GetTimeSeconds() - startedFollowingTime.Value;

                if (timeElapsedSec > 10) //Time limit reached, TP the villager and reset Timer
                {
                    KLog.warning($"Teleporting villager {villagerGeneral.ZNV.GetZDO().m_uid.id} Because he didn't reach following target within the time limit");
                    TPToLoc(ZDOMan.instance.GetZDO(this.followingObjZDOID).GetPosition());
                    startedFollowingTime = null;
                }
            }
            else
            {
                startedFollowingTime = null;
            }
        }


        /*
         * The function below is going to make sure that if we command a villager to move to a location, it reaches that position.
         * If certain time elasped and it didn't reach then it will TP the villager to the location.
         * The variables below needs to be modifed by a function when needed
         */
        Vector3 movePos; //The position to move to
        bool keepMoving = false; //VERY IMPORTANT: Determines if the villager needs to keep moving or not
        bool shouldRun = true; //Should the villager run
        float acceptableDistance = 2f; //Acceptable distance to stop
        DateTime? keepMovingStartTime = null; //Keeps track of how much time has elasped
        private void MovePerUpdateIfDesired()
        {
            //Only continue if we are moving
            if (keepMoving == true)
            {
                //Reset AI
                ai.ResetPatrolPoint();
                ai.ResetRandomMovement();

                //Check if we have set starting timer
                if (keepMovingStartTime == null)
                {
                    keepMovingStartTime = ZNet.instance.GetTime();
                }

                //Check if villager has been trying to reach target for too long
                double timeDiff = (ZNet.instance.GetTime() - keepMovingStartTime.Value).TotalSeconds;

                if (timeDiff > 10) //60 sec passed and still hasn't reached path so we tp
                {
                    keepMoving = false;
                    TPToLoc(movePos);
                    return;
                }
                if (ai.MoveAndAvoid(ai.GetWorldTimeDelta(), movePos, acceptableDistance, shouldRun)) //If time limit threshold not hit keep moving normally. MoveAndAvoid function returns true when it reaches acceptable distance
                {
                    keepMoving = false;
                    return;
                }
            }
            else
            {
                //Not moving so reset timer
                keepMovingStartTime = null;
            }

        }





        /*
         * This function is going to execute only when it's following a player.
         * Makes sure to TP the villager to the player if it's Teleporting or If it's too far
         */
        private void TPVillagerToFollowerIfNeeded()
        {
            //Check if villager is following
            if (villagerGeneral.GetVillagerState() == VillagerState.Following)
            {

                //Check if ZDOID is valid and is following this player
                if (followingObjZDOID != null && followingObjZDOID.IsNone() == false && Player.m_localPlayer.GetZDOID() == followingObjZDOID)
                {
                    Vector3 playerPOS = ZDOMan.instance.GetZDO(followingObjZDOID).GetPosition();
                    float distance = Vector3.Distance(transform.position, playerPOS);
                    if (distance > 50 || Player.m_localPlayer.IsTeleporting())
                    {
                        TPToLoc(playerPOS);
                    }
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------


        private bool FollowGameObject(GameObject target)
        {

            if (target == null)
            {
                return false;
            }

            ZDOID targetZDOID = target.GetComponentInParent<ZNetView>().GetZDO().m_uid;

            if (targetZDOID == null || targetZDOID.IsNone())
            {
                KLog.warning($"FollowGameObject(): Target has invalid ZDOID");
                return false;
            }

            this.followingObjZDOID = targetZDOID;
            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();
            ai.SetFollowTarget(target);
            return true;
        }

        private bool TPToLoc(Vector3 pos)
        {
            ai.SetFollowTarget(null);
            transform.position = pos; //THIS WORKS. BUT IDK HOW IT'S GOING TO BE IN SERVERS. FUTURE:: CHANGE THIS WITH SOME SERVER CALLS
            villagerGeneral.ZNV.GetZDO().SetPosition(pos); //THIS IS NOT RELIABLE IDK WHY
            return true;
        }

        public bool FollowPlayer(Player p)
        {
            /*
             * Validate if player is valid.
             * StopMoving
             * Save player's ZDOID
             * Update state
             * Follow target
             */

            if (p == null)
            {
                talk.Say("Can't follow", "Follow");
                return false;
            }

            //Stop moving incase the villager was moving earlier
            keepMoving = false;

            //Save player's ZDO
            followingObjZDOID = p.GetZDOID();

            //Update state
            villagerGeneral.SetVillagerState(VillagerState.Following);

            //Follow the target and update villager state value stored in bed
            FollowGameObject(p.gameObject);

            return true;
        }



        public bool MoveVillagerToLoc(Vector3 pos, float radius, bool resetFollower, bool shouldTalk, bool shouldRun)
        {
            /*
             * Update variabes as needed
             * If keepFollowers is false then remove follower data
             */
            movePos = pos;
            acceptableDistance = radius;
            this.shouldRun = shouldRun;

            if (resetFollower)
            {
                this.followingObjZDOID = new ZDOID { m_hash = -1, m_userID = -1 };
            }

            if (shouldTalk)
                talk.Say($"Moving to {pos.ToString()}", "Moving");
            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();
            ai.SetFollowTarget(null);
            keepMoving = true;
            return true;
        }

        public bool GuardBed()
        {
            /*
             * Validate if bedZDO is valid, then set state as guarding bed
             * Check if it has a bed instance.
             * Make villager move to bed by setting it as follow target. The followTarget Update function will make sure that if villager doesn't reach after certain amount of time. It will TP the villager to the Bed
             * If bed instance is not valid then we TP the villager to the bed
             */

            //1. Validate if bed zdo is valid
            ZDO bedZDO = villagerGeneral.GetBedZDO();

            if (bedZDO == null || bedZDO.IsValid() == false)
            {
                talk.Say("Couldn't find my bed", "Bed");
                return false;
            }

            //Stop moving
            keepMoving = false;
            //Remove from follower
            this.followingObjZDOID = new ZDOID { m_hash = -1, m_userID = -1 };

            //Update the state of the villager
            villagerGeneral.SetVillagerState(VillagerState.Guarding_Bed);

            //2. Check if bed has an instance
            GameObject bedInstance = villagerGeneral.GetBedInstance();

            if (bedInstance != null && bedInstance.gameObject != null)
            {
                FollowGameObject(bedInstance);
                talk.Say("Guarding Bed", "Bed");
                return true;
            }
            else
            {
                //3. Bed instance not valid. TP the villager to bed
                TPToLoc(bedZDO.GetPosition());
                return true;
            }
        }

        public bool DefendPost()
        {
            /*
             * Validate if DefenseZDO is valid. If valid update state to defending post
             * Check if defensePost has an instance, if yes then set it as followTarget
             * If instance not valid then TP to defenseZDO's position
             */

            ZDO defenseZDO = villagerGeneral.GetDefenseZDO();

            if (defenseZDO == null || defenseZDO.IsValid() == false)
            {
                talk.Say("Defend Post not assigned or destroyed", "Defend");

                return false;
            }

            //Stop moving
            keepMoving = false;
            //Remove from follower
            this.followingObjZDOID = new ZDOID { m_hash = -1, m_userID = -1 };

            //Update villager's state
            villagerGeneral.SetVillagerState(VillagerState.Defending_Post);

            //Get instance of defensePost and validate
            GameObject defenseInstance = villagerGeneral.GetDefensePostInstance();

            //Follow defense post
            if (defenseInstance != null && defenseInstance.gameObject != null)
            {
                FollowGameObject(defenseInstance);
                return true;
            }
            else //TP to defense post
            {
                TPToLoc(defenseZDO.GetPosition());
                return true;
            }

        }

        public bool StartWork()
        {
            ZDOID wp = villagerGeneral.GetWorkPostID();

            if (wp == null || wp.IsNone())
            {
                talk.Say("No Work Post assigned", "Work");
                return false;
            }

            if (villagerGeneral.GetWorkZDO() == null || villagerGeneral.GetWorkZDO().IsValid() == false)
            {
                talk.Say("My Defense post was destroyed", "Defense");
                return false;
            }

            //Stop moving
            keepMoving = false;
            //Remove from follower
            this.followingObjZDOID = new ZDOID { m_hash = -1, m_userID = -1 };


            villagerGeneral.SetVillagerState(VillagerState.Working);

            GameObject wpi = villagerGeneral.GetWorkInstance();

            //If wpi is invalid we are going to tp the villager to it's work post
            if (wpi == null)
            {
                TPToLoc(villagerGeneral.GetWorkZDO().GetPosition());
            }

            talk.Say("Working", "Work");
            return true;


        }





        int minRandomTime = 200;
        int maxRandomTime = 2000;


        /// <summary>
        /// Runs once and never stops until villager is destroyed.
        /// Used to Handle Actions that need a lot if waiting and ordered execution Eg : Going to Work post and waiting to reach -> Then going to Pickup location and waiting to reach
        /// </summary>
        async private void WorkAsync()
        {
            while (true)
            {
                await Task.Delay(500);
                try
                {
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working || villagerGeneral.GetContainerZDO().IsValid() == false || villagerGeneral.GetWorkZDO().IsValid() == false)
                    {
                        await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                        continue;
                    }

                    await PickupAndStoreWork();
                    await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                    await RefillWork();
                }
                catch (NullReferenceException e)
                {
                    await Task.Delay(500);
                    KLog.info($"Ai->WorkAsync() {e.Message}\nStack: {e.StackTrace}");
                }
            }
        }

        async private Task PickupAndStoreWork()
        {

            ZDO WorkPostZDO = villagerGeneral.GetWorkZDO();
            Vector3 workPosLoc = WorkPostZDO.GetPosition();

            //Move to workpost
            MoveVillagerToLoc(workPosLoc, 3f, false, false, false);
            while (keepMoving)
            {
                await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));

                if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                {
                    break;
                }
            }

            //Reached Work post, Check if still working
            await Task.Delay(500);
            if (villagerGeneral.GetVillagerState() != VillagerState.Working)
            {
                return;
            }

            //Search for pickable item
            ItemDrop pickable = FindClosestPickup(workPosLoc, 250f);

            //ItemDrop found.
            if (pickable != null)
            {
                talk.Say($"Going to Pickup {pickable.m_itemData.m_shared.m_name}", "Work");

                //Move to the pickable item 
                MoveVillagerToLoc(pickable.transform.position, 1f, false, false, false);

                while (keepMoving)
                {
                    await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        break;
                    }
                }

                await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                {
                    return;
                }

                //Fake pickup by storing the prefab, and deleting the GO from world if only 1 stack or else reduce stack by one
                string prefabName = pickable.m_itemData.m_dropPrefab.name;
                int stackCount = pickable.m_itemData.m_stack;
                var prefab = PrefabManager.Instance.GetPrefab(prefabName);
                if (prefab == null)
                {
                    return;
                }
                if (stackCount == 1)
                    ZDOMan.instance.DestroyZDO(pickable.GetComponentInParent<ZNetView>().GetZDO());
                else
                {
                    pickable.SetStack(stackCount - 1);
                    //TODO: TEST IN MP
                }

                Vector3 containerLoc = villagerGeneral.GetContainerZDO().GetPosition();
                talk.Say($"Going to Put {prefabName} in container", "Work");
                MoveVillagerToLoc(containerLoc, 3f, false, false, false);

                while (keepMoving)
                {
                    await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        break;
                    }
                }

                await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                {
                    return;
                }

                villagerGeneral.GetContainerInstance().GetComponent<Container>().GetInventory().AddItem(prefab, 1);
            }



        }

        async private Task RefillWork()
        {

            ZDO WorkPostZDO = villagerGeneral.GetWorkZDO();
            Vector3 workPosLoc = WorkPostZDO.GetPosition();
            //Move to workpost
            MoveVillagerToLoc(workPosLoc, 3f, false, false, false);
            while (keepMoving)
            {
                await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));

                if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                {
                    break;
                }

                if (!keepMoving) break;
            }


            //Reached Work post, Check if still working
            await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
            if (villagerGeneral.GetVillagerState() != VillagerState.Working)
            {
                return;
            }



            //Find smelter
            Smelter smelter = FindValidSmelter(workPosLoc, 500f, true);
            if (smelter != null)
            {
                //Go to container and remove the fuel or cookable or both
                talk.Say("Going to container to get items for smelting", "Work");

                //Move to container
                MoveVillagerToLoc(villagerGeneral.GetContainerZDO().GetPosition(), 2f, false, false, false);

                while (keepMoving)
                {
                    await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        break;
                    }
                }

                await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                {
                    return;
                }


                bool tookFuel = false;
                bool tookCookable = false;
                //Check and remove fuel/cookable
                var inventory = villagerGeneral.GetContainerInstance().GetComponent<Container>().GetInventory();

                if (inventory == null)
                {
                    talk.Say("Inventory not found for container", "Work");
                    return;
                }

                if (inventory.HaveItem(smelter.m_fuelItem.m_itemData.m_shared.m_name))
                {
                    tookFuel = true;
                    inventory.RemoveItem(smelter.m_fuelItem.m_itemData.m_shared.m_name, 1);
                }
                ItemDrop.ItemData cookableItem = smelter.FindCookableItem(inventory);
                if (cookableItem != null && inventory.HaveItem(cookableItem.m_shared.m_name))
                {
                    tookCookable = true;
                    inventory.RemoveItem(cookableItem.m_shared.m_name, 1);
                }


                if (tookFuel == false && tookCookable == false)
                {
                    KLog.info("Woops no processable or fuel in contianer");
                    talk.Say("No processable or fuel in my container", "");
                    return;
                }


                talk.Say("Moving to Smelter to fill it.", "Work");

                //Go to smelter
                MoveVillagerToLoc(smelter.transform.position, 4f, false, false, false);

                while (keepMoving)
                {
                    await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        break;
                    }
                }

                await Task.Delay(500);
                //Add fuel to the smelter
                if (tookFuel)
                {
                    smelter.GetComponentInParent<ZNetView>().InvokeRPC("AddFuel");
                }
                if (tookCookable)
                {
                    smelter.GetComponentInParent<ZNetView>().InvokeRPC("AddOre", cookableItem.m_dropPrefab.name);
                    KLog.warning($"drop name : {cookableItem.m_dropPrefab.name}");
                }

            }

            return;


        }

        private ItemDrop FindClosestPickup(Vector3 center, float radius)
        {
            //Scan for objects that we can pickup and add it in list
            Collider[] colliders = Physics.OverlapSphere(center, radius);

            string[] validPickupPrefabNames = new[] { "Bronze", "Iron", "Silver", "IronScrap", "BronzeScrap", "SilverScrap", "Coal" };

            ItemDrop pickable = null;

            float distance = -1;

            foreach (var c in colliders)
            {
                var d = c?.gameObject?.GetComponent<ItemDrop>();

                if (d == null)
                {
                    d = c?.gameObject?.GetComponentInChildren<ItemDrop>();
                }
                if (d == null)
                {
                    d = c?.gameObject?.GetComponentInParent<ItemDrop>();
                }
                if (d == null)
                {

                    continue;
                }

                string prefabName = d.m_itemData.m_dropPrefab.name; //We need the dropn not shared name
                if (validPickupPrefabNames.Contains(prefabName))
                {
                    if (pickable == null) //No pickable item selected so we select this as first
                    {
                        pickable = d;
                        distance = Vector3.Distance(center, d.transform.position);
                    }
                    else //Pickable object exist we check for distance
                    {
                        float thisDistance = Vector3.Distance(center, d.transform.position);
                        if (thisDistance < distance)
                        {
                            pickable = d;
                            distance = thisDistance;
                        }
                    }
                }
            }
            return pickable;
        }

        private Smelter FindValidSmelter(Vector3 center, float radius, bool getRandom)
        {
            GameObject container = villagerGeneral.GetContainerInstance();
            Collider[] colliders = Physics.OverlapSphere(center, radius);
            List<Smelter> validSmelters = new List<Smelter>();
            Smelter smelter = null;
            float distance = -1;
            foreach (Collider c in colliders)
            {
                var d = c.gameObject.GetComponent<Smelter>();
                if (d == null)
                {
                    d = c.gameObject.GetComponentInParent<Smelter>();
                }
                if (d == null)
                {
                    d = c.gameObject.GetComponentInChildren<Smelter>();
                }
                if (d == null)
                {
                    continue;
                }

                string fuelName = d.m_fuelItem.m_itemData.m_shared.m_name; //Get the type of fuel it uses

                //Check if contanier has the fuel
                var inventory = container.GetComponent<Container>().GetInventory();
                var cookable = d.FindCookableItem(inventory);

                bool fuelPresent = inventory.HaveItem(fuelName);
                bool cookablePresent = d.FindCookableItem(inventory) != null;

                if (fuelPresent || cookablePresent)
                {
                    if (getRandom) //If getRandom is true we add this smelter to the vaildSmelters list and then send one randomly at the end
                    {
                        validSmelters.Add(d);
                        continue;
                    }

                    if (smelter == null)
                    {
                        distance = Vector3.Distance(d.transform.position, center);
                        smelter = d;
                    }
                    else
                    {
                        if (Vector3.Distance(d.transform.position, center) < distance)
                        {
                            smelter = d;
                        }
                    }
                }


            }

            if (getRandom && validSmelters.Count > 0)
            {
                smelter = validSmelters[UnityEngine.Random.Range(0, validSmelters.Count - 1)];
            }
            return smelter;
        }



    }
}
