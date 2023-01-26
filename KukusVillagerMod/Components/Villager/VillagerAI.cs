using Jotunn.Managers;
using KukusVillagerMod.Components;
using KukusVillagerMod.Configuration;
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
        private float workScanRange = VillagerModConfigurations.WorkScanRange;
        CustomAI customAI;
        private void Awake()
        {
            updateRanOnce = false;
        }


        private void FixedUpdate()
        {
            //Keeping them in Awake doesn't always guarentee so we do it here instead.

            if (talk == null) talk = GetComponent<NpcTalk>();

            if (villagerGeneral == null)
            {
                villagerGeneral = GetComponent<VillagerGeneral>();

            }

            if (ai == null) ai = GetComponent<MonsterAI>();

            if (customAI == null) customAI = new CustomAI(ai);


            if (!villagerGeneral.IsVillagerTamed()) return;

            //Runs only once
            if (!updateRanOnce)
            {
                updateRanOnce = true;

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
                    case VillagerState.Roaming:
                        RoamAround();
                        break;
                    default:
                        GuardBed();
                        break;
                }
            }
            else //Per Tick Functions
            {
                TPVillagerToFollowerIfNeeded();
                MovePerUpdateIfDesired();
                FollowCheckPerUpdate();
                RegenHPIfGuarding();
                WorkLoop();
            }

        }



        //Functions that execute every frame---------------------------

        //If we are following a target, this function checks how long have we been following the target for and if it crosses a threshold we tp the villager to that location if acceptable distance has not been reached
        double? startedFollowingTime = null;
        float AcceptedFollowDistance = VillagerModConfigurations.AcceptedFollowDistance;
        int FollowTimeLimit = VillagerModConfigurations.FollowTimeLimit;
        bool closeToFollowTarget = false;
        private void FollowCheckPerUpdate()
        {
            //Check if followingObjZDOID is valid and that we are not in following state. following state has it's own function to take care of it
            if (villagerGeneral.GetVillagerState() != VillagerState.Following && ZDOMan.instance.GetZDO(followingObjZDOID) != null && ZDOMan.instance.GetZDO(followingObjZDOID).IsValid())
            {
                //try to use game object's location, if failed use zdo's saved location
                float distance;
                var go = ZNetScene.instance.FindInstance(followingObjZDOID);
                if (go) distance = Vector3.Distance(transform.position, go.transform.position);
                else
                    distance = Vector3.Distance(transform.position, ZDOMan.instance.GetZDO(followingObjZDOID).GetPosition());


                //If distance is acceptable then we do not proceed
                if (distance < AcceptedFollowDistance)
                {
                    startedFollowingTime = null;
                    closeToFollowTarget = true;
                    return;
                }

                //Check if we started counting already
                if (startedFollowingTime == null)
                {
                    startedFollowingTime = ZNet.instance.GetTimeSeconds();
                    closeToFollowTarget = false;
                }

                //Calculate time elasped and distance between villager and target
                double timeElapsedSec = ZNet.instance.GetTimeSeconds() - startedFollowingTime.Value;

                if (timeElapsedSec > FollowTimeLimit) //Time limit reached, TP the villager and reset Timer
                {
                    KLog.warning($"Teleporting villager {villagerGeneral.ZNV.GetZDO().m_uid.id} Because he didn't reach following target within the time limit");
                    TPToLoc(ZDOMan.instance.GetZDO(this.followingObjZDOID).GetPosition(), false); //We want the villager to still follow it's target after tping so we set false as 2nd parameter
                    startedFollowingTime = null;
                    closeToFollowTarget = true;
                }
                else
                {
                    closeToFollowTarget = false;
                }
            }
            else
            {
                startedFollowingTime = null;
                closeToFollowTarget = false;
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
        double? startStuckCheckTime = null;
        int timeLimitToStuck = 4;
        int timeLimitForMove = VillagerModConfigurations.MoveTimeLimit;
        Vector3? stuckCheckPos;
        private void MovePerUpdateIfDesired()
        {
            //Only continue if we are moving
            if (keepMoving == true)
            {

                //Check if we have set starting timer for move
                if (keepMovingStartTime == null)
                {
                    keepMovingStartTime = ZNet.instance.GetTime();
                }

                if (startStuckCheckTime == null) //Setup start stuck check and save position of villager when start stuck check 
                {
                    startStuckCheckTime = ZNet.instance.GetTimeSeconds();
                    stuckCheckPos = transform.position;
                }

                //Check if villager has been trying to reach target for too long
                double timeDiff = (ZNet.instance.GetTime() - keepMovingStartTime.Value).TotalSeconds;
                double stuckTimeDiff = (ZNet.instance.GetTimeSeconds() - startStuckCheckTime.Value);


                if (timeDiff > timeLimitForMove && Vector3.Distance(transform.position, movePos) > acceptableDistance) //60 sec passed and still hasn't reached path so we tp
                {
                    KLog.warning($"TPing Villager {villagerGeneral.ZNV.GetZDO().m_uid.id} Because time limit reached for moving");
                    startStuckCheckTime = null;
                    keepMovingStartTime = null;
                    TPToLoc(movePos);
                    keepMoving = false;
                    return;
                }
                if (stuckTimeDiff > timeLimitToStuck) //4 sec passed, check if position of villager is different
                {
                    //Reset the startStuck timer first
                    startStuckCheckTime = null;


                    float distance = Vector3.Distance(transform.position, stuckCheckPos.Value);
                    if (distance <= 1f)
                    {
                        //KLog.warning("Villager is stuck");
                    }
                }

                customAI.MoveTo(ai.GetWorldTimeDelta(), movePos, acceptableDistance, shouldRun, transform);

                if (Vector3.Distance(transform.position, movePos) <= acceptableDistance)
                {

                    //Necessary
                    ai.StopMoving();
                    ai.SetPatrolPoint();

                    //KLog.info($"Villager {villagerGeneral.ZNV.GetZDO().m_uid.id} reached destination {movePos}");
                    keepMoving = false;
                    startStuckCheckTime = null;
                    keepMovingStartTime = null;
                    return;
                }
            }
            else
            {
                //Not moving so reset timer

                keepMovingStartTime = null;
                startStuckCheckTime = null;
            }

        }

        /*
         * This function is going to execute only when it's following a player.
         * Makes sure to TP the villager to the player if it's Teleporting or If it's too far
         */
        int playerFarDistance = VillagerModConfigurations.FollowerMaxDistance;
        private void TPVillagerToFollowerIfNeeded()
        {
            //Check if villager is following
            if (villagerGeneral.GetVillagerState() != VillagerState.Following) return;
            if (this.followingObjZDOID == null || this.followingObjZDOID.IsNone()) return;
            GameObject playerGO = ZNetScene.instance.FindInstance(this.followingObjZDOID);
            if (playerGO == null) return;
            Player player = playerGO.GetComponent<Player>();
            if (player == null) return;

            //Following a player
            float distance = Vector3.Distance(transform.position, playerGO.transform.position);
            if (distance > playerFarDistance || player.IsTeleporting())
            {
                TPToLoc(playerGO.transform.position, false); //We want the villager to still keep following the player so we set removefollower to false which is true by default
            }

        }

        /*
         * Handles villager's actions when working so that it is executed step by step such as go to pickup -> pickup -> go to container -> place inside container
         */

        bool alreadyWorking = false;
        async private void WorkLoop()
        {
            if (alreadyWorking) return;
            alreadyWorking = true;
            try
            {
                ZDOID wp = villagerGeneral.GetWorkPostZDOID();

                if (wp == null || wp.IsNone())
                {
                    alreadyWorking = false;
                    return;
                }

                if (villagerGeneral.GetWorkPostZDO() == null || villagerGeneral.GetWorkPostZDO().IsValid() == false)
                {
                    alreadyWorking = false;
                    return;
                }

                ZDOID containerID = villagerGeneral.GetContainerZDOID();

                if (containerID == null || containerID.IsNone())
                {
                    alreadyWorking = false;
                    return;
                }

                ZDO containerZDO = villagerGeneral.GetContainerZDO();
                if (containerZDO == null || containerZDO.IsValid() == false)
                {
                    alreadyWorking = false;
                    return;
                }

                if (villagerGeneral.GetVillagerState() == VillagerState.Working)
                {
                    WorkSkill skill = villagerGeneral.GetWorkSkill();
                    if (skill == WorkSkill.Pickup)
                    {
                        unchoppableTrees.Clear();
                        await PickupAndStoreWork(VillagerModConfigurations.UseMoveForWork);
                        alreadyWorking = false;
                        return;
                    }
                    if (skill == WorkSkill.Fill_Smelt)
                    {
                        unchoppableTrees.Clear();
                        await FillSmelt(VillagerModConfigurations.UseMoveForWork);
                        alreadyWorking = false;
                        return;
                    }
                    if (skill == WorkSkill.Chop_Wood)
                    {
                        await ChopWood(VillagerModConfigurations.UseMoveForWork);
                        alreadyWorking = false;
                        return;
                    }
                }
                unchoppableTrees.Clear();
                alreadyWorking = false;
                return;
            }
            catch (Exception e)
            {
                KLog.info($"Exception in work loop {e.Message}\n{e.StackTrace}");
                unchoppableTrees.Clear();
                alreadyWorking = false;
                return;
            }

        }



        //Recover certain % of hp if guarding bed
        DateTime? healTimeStart = null;
        private void RegenHPIfGuarding()
        {
            if (Util.ValidateZDOID(villagerGeneral.GetBedZDOID()) == false) return;
            if (villagerGeneral.GetVillagerState() == VillagerState.Guarding_Bed)
            {
                float distance = Vector3.Distance(transform.position, villagerGeneral.GetBedZDO().GetPosition());
                if (distance > 5) return;

                if (healTimeStart == null)
                {
                    healTimeStart = ZNet.instance.GetTime();
                }

                double timeElasped = (ZNet.instance.GetTime() - healTimeStart.Value).Seconds;

                if (timeElasped > 3)
                {
                    healTimeStart = null;
                    //heal 5% of hp, configure in future
                    float currentHP = villagerGeneral.GetAIHP();
                    float toAdd = currentHP * 0.05f;
                    float newHP = currentHP + toAdd;
                    if (newHP > villagerGeneral.GetStatHealth()) newHP = villagerGeneral.GetStatHealth();
                    villagerGeneral.SetAIHP(newHP);
                }
            }
            else
            {
                healTimeStart = null;
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------


        //Makes villager follow the targer, used internally to make villager follow it's bed, defense post, player, etc
        private bool FollowGameObject(GameObject target)
        {
            //Temporarily stop the followPerUpdate Check function by setting these variables
            RemoveFollower();

            if (target == null)
            {
                return false;
            }



            //Get ZDOID and validate
            ZDOID targetZDOID = target.GetComponentInParent<ZNetView>().GetZDO().m_uid;

            if (targetZDOID == null || targetZDOID.IsNone())
            {
                KLog.warning($"FollowGameObject(): Target has invalid ZDOID");
                return false;
            }

            //Save ZDOID reference as followingZDOID
            this.followingObjZDOID = targetZDOID;

            //Reset AI and set follower target
            ai.ResetPatrolPoint();
            ai.SetFollowTarget(target);
            return true;
        }

        /// <summary>
        /// Invalidates the followingObjZDOID thus making the followCheckPerUpdate function "stop"
        /// </summary>
        private void RemoveFollower()
        {
            closeToFollowTarget = false;
            followingObjZDOID = ZDOID.None;
            ai.SetFollowTarget(null);
            AcceptedFollowDistance = VillagerModConfigurations.AcceptedFollowDistance;
        }

        /// <summary>
        /// Teleports the villager to the given location, optional parameter to remove the villager's follow target or keep the follow target
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="RemoveFollowTarget"></param>
        /// <returns></returns>
        private bool TPToLoc(Vector3 pos, bool RemoveFollowTarget = true)
        {
            //Remove the target villager is following?
            if (RemoveFollowTarget)
                ai.SetFollowTarget(null);

            transform.position = pos; //THIS WORKS. BUT IDK HOW IT'S GOING TO BE IN SERVERS. FUTURE:: CHANGE THIS WITH SOME SERVER CALLS
            villagerGeneral.ZNV.GetZDO().SetPosition(pos); //THIS IS NOT RELIABLE IDK WHY
            return true;
        }

        //Main command that makes villager follow a player
        public bool FollowPlayer(ZDOID playerID)
        {
            AcceptedFollowDistance = VillagerModConfigurations.AcceptedFollowDistance;

            if (playerID == null || playerID.IsNone())
            {
                talk.Say("Can't follow, invalid ZDOID", "Follow");
                return false;
            }

            //Stop moving incase the villager was moving earlier
            keepMoving = false;
            //Update state FIRST. IMPORTANT
            villagerGeneral.SetVillagerState(VillagerState.Following);

            //Validate if the following player is valid and loaded in memory
            GameObject playerGO = ZNetScene.instance.FindInstance(playerID);
            if (playerGO == null)
            {
                talk.Say("Failed to find the player, Going to guard bed", "Follow");
                GuardBed();
                return false;
            }
            talk.Say("Following", "");
            //Follow the player instance
            FollowGameObject(playerGO);

            return true;
        }

        /// <summary>
        /// Moves the villager to the given location
        /// </summary>
        /// <param name="pos"> The position to move to</param>
        /// <param name="radius">Villager will stop moving once it reaches this radius of the pos</param>
        /// <param name="resetFollower">Remove the following ZDOID stored</param>
        /// <param name="shouldTalk">should villager announce the position it's moving to</param>
        /// <param name="shouldRun">if villager should run or walk</param>
        /// <returns></returns>
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
                RemoveFollower();
            }

            if (shouldTalk)
                talk.Say($"Moving to {pos.ToString()}", "Moving");
            ai.ResetPatrolPoint();
            ai.SetFollowTarget(null);
            keepMoving = true;
            return true;
        }

        /// <summary>
        /// Main command to make the villager guard bed
        /// </summary>
        /// <returns></returns>
        public bool GuardBed()
        {

            //1. Validate if bed zdo is valid
            ZDO bedZDO = villagerGeneral.GetBedZDO();

            if (bedZDO == null || bedZDO.IsValid() == false)
            {
                talk.Say("Couldn't find my bed", "Bed");
                RoamAround(false);
                return false;
            }

            //Stop moving
            keepMoving = false;
            //Remove from follower
            RemoveFollower();

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

        /// <summary>
        /// Main function to make the villager roam around it's current point
        /// </summary>
        /// <param name="shouldTalk"></param>
        /// <returns></returns>
        public bool RoamAround(bool shouldTalk = true)
        {

            //Stop moving
            keepMoving = false;
            //Remove from follower
            ai.SetFollowTarget(null);
            RemoveFollower();

            //Update the state of the villager
            villagerGeneral.SetVillagerState(VillagerState.Roaming);

            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();
            if (shouldTalk)
                talk.Say("Roaming Around", "Roam");
            ai.RandomMovement(ai.GetWorldTimeDelta(), transform.position);
            return true;
        }

        /// <summary>
        /// Main function to make villager defend the post it was assigned
        /// </summary>
        /// <returns></returns>
        public bool DefendPost()
        {
            //Validate defenseZDO
            ZDO defenseZDO = villagerGeneral.GetDefenseZDO();

            if (defenseZDO == null || defenseZDO.IsValid() == false)
            {
                talk.Say("Defend Post not assigned or destroyed", "Defend");
                RoamAround(false);
                return false;
            }

            //Stop moving
            keepMoving = false;
            //Remove from follower
            RemoveFollower();

            //Update villager's state
            villagerGeneral.SetVillagerState(VillagerState.Defending_Post);

            //Get instance of defensePost and validate
            GameObject defenseInstance = villagerGeneral.GetDefenseInstance();

            //Follow defense post
            if (defenseInstance != null && defenseInstance.gameObject != null)
            {
                talk.Say("Defending Post", "Defend");
                FollowGameObject(defenseInstance);
                return true;
            }
            else //TP to defense post as instance not found
            {
                TPToLoc(defenseZDO.GetPosition());
                return true;
            }

        }

        /// <summary>
        /// Main function to make villagers start working, the step by step operations are handles inside FixedUpdate method in "WorkLoop" function
        /// </summary>
        /// <returns></returns>
        public bool StartWork()
        {
            //Set all work actions as false

            alreadyWorking = false;
            //Validate work post
            ZDOID wp = villagerGeneral.GetWorkPostZDOID();

            if (wp == null || wp.IsNone())
            {
                RoamAround(false);
                talk.Say("No Work Post assigned", "Work");
                RoamAround(false);
                return false;
            }

            if (villagerGeneral.GetWorkPostZDO() == null || villagerGeneral.GetWorkPostZDO().IsValid() == false)
            {
                RoamAround(false);
                talk.Say("My Work post was destroyed", "Work");
                RoamAround(false);
                return false;
            }

            //Validate container
            ZDOID containerID = villagerGeneral.GetContainerZDOID();

            if (containerID == null || containerID.IsNone())
            {
                RoamAround(false);
                talk.Say("Container not assigned", "Work");
                RoamAround(false);
                return false;
            }

            ZDO containerZDO = villagerGeneral.GetContainerZDO();
            if (containerZDO == null || containerZDO.IsValid() == false)
            {
                RoamAround(false);
                talk.Say("Container was destroyed", "Work");
                RoamAround(false);
                return false;
            }

            //Stop moving
            keepMoving = false;
            //Remove from follower
            RemoveFollower();
            //Update villager state
            villagerGeneral.SetVillagerState(VillagerState.Working);


            //Get Work post's instance
            GameObject wpi = villagerGeneral.GetWorkPostInstance();

            //If wpi is invalid we are going to tp the villager to it's work post. It its valid then villagers will automatically move to it cuz of "WorkLoop" function
            if (wpi == null)
            {
                TPToLoc(villagerGeneral.GetWorkPostZDO().GetPosition());
            }

            talk.Say("Working....", "Work");
            var rot = Quaternion.FromToRotation(transform.position, wpi.transform.position);
            transform.rotation = rot;
            return true;


        }

        int minRandomTime = VillagerModConfigurations.MinWaitTimeWork; //min wait time for work
        int maxRandomTime = VillagerModConfigurations.MaxWaitTimeWork; //max wait time for work
        bool workTalk = VillagerModConfigurations.TalkWhileWorking; //should talk while working
        bool workRun = VillagerModConfigurations.workRun; //should villager run while working

        /// <summary>
        /// We can either move to a location or follow a target, if we pass in followTarget it will start following, following is recommended if you want the character to stay close
        /// </summary>
        /// <param name="location"></param>
        /// <param name="acceptableDistance"></param>
        /// <param name="followTarget"></param>
        /// <returns></returns>
        async private Task GoToLocationAwaitWork(Vector3 location, float acceptableDistance = 3f)
        {

            //Move to workpost
            MoveVillagerToLoc(location, acceptableDistance, false, false, workRun);
            while (keepMoving)
            {
                ai.LookAt(location);
                //movePos = location;
                await Task.Delay(1);
                if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                {
                    break;
                }
            }
        }

        async private Task FollowTargetAwaitWork(GameObject target, float acceptableRadius = 3f)
        {
            if (target == null) return;
            //RemoveFollower();
            while (!closeToFollowTarget && target != null)
            {
                //ai.LookAt(target.transform.position);
                //FollowGameObject(target);
                //AcceptedFollowDistance = acceptableRadius;
                await Task.Delay(5);
                if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                {
                    break;
                }
            }
        }

        async private Task PickupAndStoreWork(bool useMoveTo)
        {
            KLog.info("Picking up " + villagerGeneral.ZNV.GetZDO().m_uid.id);
            try
            {

                ZDO WorkPostZDO = villagerGeneral.GetWorkPostZDO();
                Vector3 workPosLoc = WorkPostZDO.GetPosition();

                await Task.Delay(500);

                //Go to work post
                GameObject workPostInstance = villagerGeneral.GetWorkPostInstance();
                if (workPostInstance)
                {
                    if (useMoveTo)
                        await GoToLocationAwaitWork(workPostInstance.transform.position);
                    else
                        await FollowTargetAwaitWork(workPostInstance);
                }
                else
                {
                    TPToLoc(workPosLoc);
                }

                //Reached Work post, Check if still working
                await Task.Delay(500);
                if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                {
                    return;
                }

                //Search for pickable item
                ItemDrop pickable = FindClosestValidPickup(workPosLoc, workScanRange * 1.3f, true);

                //ItemDrop found.
                if (pickable != null)
                {

                    if (workTalk)
                        talk.Say($"Going to Pickup {pickable.m_itemData.m_shared.m_name}", "Work");
                    await Task.Delay(500);

                    if (pickable == null) return;
                    //Go to item
                    if (useMoveTo)
                        await GoToLocationAwaitWork(pickable.transform.position);
                    else
                        await FollowTargetAwaitWork(pickable.gameObject);

                    //Reached item, check if still working
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        return;
                    }

                    if (pickable == null) return;

                    //Fake pickup by storing the prefab, and deleting the GO from world if only 1 stack or else reduce stack by one
                    string prefabName = pickable.m_itemData.m_dropPrefab.name; //Get prefab name
                    int stackCount = pickable.m_itemData.m_stack; //Get stack count
                    var prefab = PrefabManager.Instance.GetPrefab(prefabName); //Get prefab using prefabname

                    if (prefab == null) //if prefab not found we exit pickup
                    {
                        return;
                    }
                    if (stackCount == 1) //if only 1 stack then we destroy the pickup item
                        ZDOMan.instance.DestroyZDO(pickable.GetComponentInParent<ZNetView>().GetZDO());
                    else //If more than 1 stack available we decrement by 1
                    {
                        pickable.SetStack(stackCount - 1);
                    }

                    //Get inventory ZDO of the container
                    Inventory inv = villagerGeneral.GetContainerInventory();
                    bool added = inv.AddItem(prefab, 1); //Try to add item and store result boolean
                    villagerGeneral.SaveContainerInventory(inv); //Save the inventory to container's ZDO

                    //Get instance of container
                    GameObject containerInstance = villagerGeneral.GetContainerInstance();
                    if (containerInstance != null) //Pretend to go to container and put item
                    {

                        Vector3 containerLoc = villagerGeneral.GetContainerZDO().GetPosition(); //Get location of container

                        if (workTalk)
                            talk.Say($"Going to Put {prefabName} in container", "Work");
                        await Task.Delay(1000);
                        //Go to container
                        if (useMoveTo)
                            await GoToLocationAwaitWork(containerInstance.transform.position);
                        else
                            await FollowTargetAwaitWork(containerInstance);

                        if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                        {
                            return;
                        }

                        if (containerInstance == null) return;

                        if (added)
                        {
                            if (workTalk)
                                talk.Say($"Added {prefabName} to container", "Work");
                        }

                        else
                        {
                            if (workTalk)
                                talk.Say($"Failed to add {prefabName} to container", "Work");
                        }
                        await Task.Delay(2000);
                    }
                    else //Container instance not valid so we will not move villager
                    {
                        if (workTalk)
                            talk.Say($"Transferred {prefabName} to container", "Work");
                    }

                }
                else
                {
                    if (workTalk)
                        talk.Say("Found nothing nearby that can be put in container or container is full", "Work");

                }



            }
            catch (Exception)
            {
            }



        }
        private ItemDrop FindClosestValidPickup(Vector3 center, float radius, bool random)
        {
            //Scan for objects that we can pickup and add it in list
            Collider[] colliders = Physics.OverlapSphere(center, radius);
            var inventory = villagerGeneral.GetContainerInventory();
            string PickupPrefabNames = VillagerModConfigurations.PickableObjects.Trim() + ",randomstuff";
            List<string> pickUpNameList = new List<string>();
            string p = "";
            string itemPickups = "";
            for (int i = 0; i < PickupPrefabNames.Length; i++)
            {

                char c = PickupPrefabNames[i];
                if (c.Equals(' ')) continue;
                if (c.Equals(','))
                {
                    if (p.Length == 0) continue;
                    pickUpNameList.Add(p);
                    itemPickups = itemPickups + ", " + p;
                    p = "";
                    continue;
                }
                p = p + c;
            }
            KLog.info($"Items pickup list : {itemPickups}");


            ItemDrop pickable = null;

            float distance = -1;
            List<ItemDrop> randomItemList = new List<ItemDrop>();
            foreach (var c in colliders)
            {
                try
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

                    if (inventory == null)
                    {
                        KLog.warning("Inventory is null!");
                        continue;
                    }
                    //Check if we can store this by adding it temporarily and removing it again

                    GameObject itemPrefab = PrefabManager.Instance.GetPrefab(prefabName);
                    if (itemPrefab == null) continue;
                    ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
                    if (itemDrop == null) continue;

                    //Worst nest ever but i am too tired to see which one is the right way now.
                    if (!inventory.CanAddItem(d.gameObject))
                    {
                        if (!inventory.CanAddItem(itemPrefab))
                        {
                            if (!inventory.CanAddItem(d.m_itemData))
                            {
                                if (!inventory.CanAddItem(itemPrefab.GetComponent<ItemDrop>().m_itemData))
                                {
                                    continue;
                                }
                            }
                        }
                    }


                    if (pickUpNameList.Contains(prefabName))
                    {

                        if (random)
                        {
                            randomItemList.Add(d);
                        }

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
                catch (Exception e)
                {
                    KLog.warning($"{e.Message}\n{e.StackTrace}");
                }

            }

            if (random)
            {
                pickable = randomItemList[UnityEngine.Random.Range(0, randomItemList.Count - 1)];
            }
            return pickable;
        }

        async private Task FillSmelt(bool useMoveTo)
        {
            KLog.info("Filling up smelt" + villagerGeneral.ZNV.GetZDO().m_uid.id);
            try
            {

                ZDO WorkPostZDO = villagerGeneral.GetWorkPostZDO();
                Vector3 workPosLoc = WorkPostZDO.GetPosition();
                await Task.Delay(500);

                //Go to work post
                if (villagerGeneral.GetWorkPostInstance())
                {
                    if (useMoveTo)
                        await GoToLocationAwaitWork(WorkPostZDO.GetPosition());
                    else
                        await FollowTargetAwaitWork(villagerGeneral.GetWorkPostInstance());

                }
                else
                {
                    TPToLoc(workPosLoc);
                }


                //Reached Work post, Check if still working
                await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                {

                    return;
                }

                //Find smelter that can be filled
                Smelter smelter = FindValidSmelter(workPosLoc, workScanRange, true);
                if (smelter != null)
                {

                    //Check if container instance is valid, if not we will remove items needed from it virtually using zdo
                    GameObject container = villagerGeneral.GetContainerInstance();
                    Inventory inv = villagerGeneral.GetContainerInventory();

                    if (container == null) //if instance not VALID we will skip moving to the container
                    {
                        if (workTalk)
                            talk.Say("Transferring items from container to me", "Work");


                    }
                    else //if instance valid we will move to the container
                    {
                        if (workTalk)
                            talk.Say("Going to container to get items for smelting", "Work");


                        await Task.Delay(500);

                        //Move to container
                        GameObject containerInstance = villagerGeneral.GetContainerInstance();
                        if (containerInstance)
                        {
                            if (useMoveTo)
                                await GoToLocationAwaitWork(containerInstance.transform.position);
                            else
                                await FollowTargetAwaitWork(containerInstance);

                        }
                        else
                        {
                            TPToLoc(villagerGeneral.GetContainerZDO().GetPosition());

                        }
                    }

                    if (smelter == null) return; //if smelter was destroyed then exit

                    await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {

                        return;
                    }


                    bool tookFuel = false;
                    bool tookCookable = false;
                    foreach (var i in inv.GetAllItems())
                    {
                        if (i.m_shared.m_name.Equals(smelter.m_fuelItem.m_itemData.m_shared.m_name))
                        {
                            tookFuel = true;
                            inv.RemoveItem(smelter.m_fuelItem.m_itemData.m_shared.m_name, 1);
                            villagerGeneral.SaveContainerInventory(inv);
                            break;
                        }
                    }

                    //Find item that can be processed by the smelter and store it
                    ItemDrop.ItemData cookableItem = smelter.FindCookableItem(inv);
                    if (cookableItem != null) //if processable item found remove it from inventory
                        foreach (var i in inv.GetAllItems())
                        {

                            if (i.m_shared.m_name.Equals(cookableItem.m_shared.m_name))
                            {
                                tookCookable = true;
                                inv.RemoveItem(cookableItem.m_shared.m_name, 1);
                                villagerGeneral.SaveContainerInventory(inv);
                                break;
                            }
                        }


                    if (tookFuel == false && tookCookable == false)
                    {
                        if (workTalk)
                            talk.Say("No processable or fuel in my container or the smelter I was going to fill is already full now", "");
                        return;
                    }

                    if (workTalk)
                        talk.Say("Moving to Smelter to fill it.", "Work");

                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        return;
                    }

                    await Task.Delay(500);

                    //Go to smelter
                    if (useMoveTo)
                        await GoToLocationAwaitWork(smelter.gameObject.transform.position);
                    else
                        await FollowTargetAwaitWork(smelter.gameObject);

                    await Task.Delay(500);
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        return;
                    }

                    if (smelter == null) return;
                    //Add fuel to the smelter
                    int fuelCapacity = smelter.m_maxFuel;
                    float currentFuel = (int)smelter.GetFuel();
                    int cookableCap = smelter.m_maxOre;
                    int currentCookableSize = smelter.GetQueueSize();

                    bool addedFuel = false;
                    bool addedCookable = false;
                    if (tookFuel)
                    {
                        //Can we add fuel?
                        if (currentFuel < fuelCapacity)
                        {
                            addedFuel = true;
                            smelter.GetComponentInParent<ZNetView>().InvokeRPC("AddFuel");
                        }
                        else
                        {
                            inv.AddItem(smelter.m_fuelItem.m_itemData);
                            villagerGeneral.SaveContainerInventory(inv);
                        }
                    }
                    if (tookCookable)
                    {
                        if (currentCookableSize < cookableCap)
                        {
                            addedCookable = true;
                            smelter.GetComponentInParent<ZNetView>().InvokeRPC("AddOre", cookableItem.m_dropPrefab.name);

                        }
                        else
                        {
                            var cookable = PrefabManager.Instance.GetPrefab(cookableItem.m_dropPrefab.name);
                            inv.AddItem(cookable.GetComponent<ItemDrop>().m_itemData);
                            villagerGeneral.SaveContainerInventory(inv);

                        }
                    }


                    if (workTalk)
                    {
                        string talkString = "";
                        if (tookFuel)
                        {
                            talkString = talkString + "Fuel ";
                            if (addedFuel) talkString = talkString + "Added. ";
                            else talkString = talkString + "Not added. ";
                        }
                        if (tookCookable)
                        {
                            talkString = talkString + "Processable Item ";
                            if (addedCookable) talkString = talkString + "Added";
                            else talkString = talkString + "Not added.";
                        }

                        talk.Say(talkString, "Work");
                    }

                }
                else
                {
                    if (workTalk)
                        talk.Say("Found no smelter nearby that can be filled based on items in container", "Work");
                }

            }
            catch (Exception e)
            {
                KLog.warning($"Exception for villager : {villagerGeneral.ZNV.GetZDO().m_uid.id}\n{e.Message}\n{e.StackTrace}");
            }




        }
        private Smelter FindValidSmelter(Vector3 center, float radius, bool getRandom)
        {
            Collider[] colliders = Physics.OverlapSphere(center, radius);
            List<Smelter> validSmelters = new List<Smelter>();
            Inventory inventory = villagerGeneral.GetContainerInventory();
            Smelter smelter = null;
            float distance = -1;
            foreach (Collider c in colliders)
            {
                try
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

                    ItemDrop fuelItem = d.m_fuelItem;
                    if (fuelItem == null) continue;

                    ItemDrop.ItemData fuelItemData = fuelItem.m_itemData;
                    if (fuelItemData == null) continue;

                    string fuelName = fuelItemData.m_shared.m_name; //Get the type of fuel it uses

                    int fuelCapacity = d.m_maxFuel;
                    float currentFuel = (int)d.GetFuel();
                    int cookableCap = d.m_maxOre;
                    int currentCookable = d.GetQueueSize();



                    //Check if contanier has the fuel
                    ItemDrop.ItemData fuel = d.m_fuelItem.m_itemData;
                    var cookable = d.FindCookableItem(inventory);
                    bool fuelPresent = false;
                    bool cookablePresent = false;
                    KLog.info(inventory.GetAllItems().Count.ToString());

                    //Check if it has fuel or cookable
                    foreach (var i in inventory.GetAllItems())
                    {
                        if (i.m_shared.m_name.Equals(fuel.m_shared.m_name) && fuelCapacity > currentFuel)
                        {
                            fuelPresent = true;
                        }
                        if (cookable != null && i.m_shared.m_name.Equals(cookable.m_shared.m_name) && cookableCap > currentCookable)
                        {
                            cookablePresent = true;
                        }
                    }

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
                catch (Exception e)
                {
                    KLog.warning(e.Message + "   " + e.StackTrace);
                }


            }

            if (getRandom && validSmelters.Count > 0)
            {
                smelter = validSmelters[UnityEngine.Random.Range(0, validSmelters.Count - 1)];
            }
            return smelter;
        }






        //IGNORE BIRCH TREE. TOO HARD
        async private Task ChopWood(bool useMoveTo)
        {
            KLog.info("Chopping wood " + villagerGeneral.ZNV.GetZDO().m_uid.id);
            ZDO WorkPostZDO = villagerGeneral.GetWorkPostZDO();
            Vector3 workPosLoc = WorkPostZDO.GetPosition();
            //Go to work post
            if (villagerGeneral.GetWorkPostInstance())
            {
                if (useMoveTo)
                    await GoToLocationAwaitWork(villagerGeneral.GetWorkPostInstance().transform.position);
                else
                    await FollowTargetAwaitWork(villagerGeneral.GetWorkPostInstance());

            }
            else
            {
                TPToLoc(workPosLoc);
            }


            //Reached Work post, Check if still working
            await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
            if (villagerGeneral.GetVillagerState() != VillagerState.Working)
            {
                return;
            }

            //Find wood to chop
            var obj = FindValidTree2Chop(workPosLoc, true); //Get random tree
            if (obj != null)
            {
                if (workTalk) talk.Say($"Going to Chop {obj.name}", "Work");
                int count = 0;
                int limit = 200;
                while (obj != null && count < limit)
                {
                    if (obj == null) break;

                    //Get close to the tree
                    if (useMoveTo)
                        await GoToLocationAwaitWork(obj.transform.position, 2);
                    else
                        await FollowTargetAwaitWork(obj.gameObject);

                    if (obj == null) break;

                    ai.LookAt(obj.transform.position);

                    if (obj == null) return; //if tree was destroyed by the time we reached we exit

                    ai.DoAttack(null, false);

                    await Task.Delay(1);
                    count++;
                }
                if (obj != null)
                {
                    //Apply damage and break it by brute force
                    var treeLog = obj.GetComponentInParent<TreeLog>();
                    if (treeLog)
                    {
                        treeLog.Destroy();
                        KLog.info($"Destroyed {obj.name} using Treelog component for villager{villagerGeneral.ZNV.GetZDO().m_uid.id} chopping tree");
                        if (treeLog != null)
                        {
                            if (!unchoppableTrees.Contains(obj.gameObject))
                            {

                                unchoppableTrees.Add(obj.gameObject);

                                KLog.info($"Ignoring {obj.gameObject.name} for wood chop");

                            }
                        }
                        return;
                    }
                    var treebase = obj.GetComponentInParent<TreeBase>();
                    if (treebase)
                    {
                        HitData hd = new HitData();
                        hd.m_damage.m_damage = 5000f;
                        hd.m_damage.m_chop = 5000f;
                        hd.m_damage.m_pierce = 5000f;
                        hd.m_damage.m_slash = 5000f;
                        hd.m_damage.m_pickaxe = 5000f;
                        hd.m_damage.m_blunt = 5000f;
                        hd.m_damage.m_pierce = 5000f;
                        treebase.Damage(hd);
                        KLog.info($"Destroyed {obj.name} using Treebase component for villager{villagerGeneral.ZNV.GetZDO().m_uid.id} chopping tree");
                        if (treebase != null)
                        {
                            if (!unchoppableTrees.Contains(obj.gameObject))
                            {

                                unchoppableTrees.Add(obj.gameObject);

                                KLog.info($"Ignoring {obj.gameObject.name} for wood chop");

                            }
                        }
                        return;
                    }
                    var desc = obj.GetComponentInParent<Destructible>();
                    if (desc)
                    {
                        desc.DestroyNow();
                        KLog.info($"Destroyed {obj.name} using destructible component for villager{villagerGeneral.ZNV.GetZDO().m_uid.id} chopping tree");
                        if (desc != null)
                        {
                            if (!unchoppableTrees.Contains(obj.gameObject))
                            {

                                unchoppableTrees.Add(obj.gameObject);

                                KLog.info($"Ignoring {obj.gameObject.name} for wood chop");

                            }
                        }
                        return;
                    }
                    else
                    {
                        KLog.info($"Failed to destroy {obj.name} for villager{villagerGeneral.ZNV.GetZDO().m_uid.id} chopping tree. Adding it to unchoppable list");
                    }
                }
            }
            else
            {
                talk.Say("Nothing to chop nearby", "WORK");
            }

        }


        List<GameObject> unchoppableTrees = new List<GameObject>();

        private GameObject FindValidTree2Chop(Vector3 pos, bool random)
        {
            Vector3 scanLocation = pos;
            Collider[] colliders = Physics.OverlapSphere(scanLocation, workScanRange);
            GameObject item = null;
            float distance = -1;

            //priorities -> treelog -> treebase -> destructible

            List<TreeLog> logs = new List<TreeLog>();
            List<TreeBase> trees = new List<TreeBase>();
            List<Destructible> destructibles = new List<Destructible>();


            foreach (Collider c in colliders)
            {
                if (unchoppableTrees.Contains(c.gameObject)) continue;
                TreeBase tree = c?.gameObject?.GetComponentInParent<TreeBase>();
                TreeLog log = c?.gameObject?.GetComponentInParent<TreeLog>();
                Destructible destructible = c?.gameObject?.GetComponentInParent<Destructible>();

                if (tree != null)
                {
                    if (unchoppableTrees.Contains(tree.gameObject)) continue;
                    trees.Add(tree);
                }
                else if (log != null)
                {
                    if (unchoppableTrees.Contains(log.gameObject)) continue;
                    logs.Add(log);
                }
                else if (destructible != null)
                {
                    if (unchoppableTrees.Contains(destructible.gameObject)) continue;
                    if (destructible.name.ToLower().Contains("stub"))
                    {
                        destructibles.Add(destructible);
                    }
                    else if (destructible.name.ToLower().Contains("beech"))
                    {
                        destructibles.Add(destructible);
                    }

                }
            }

            if (logs.Count > 0 && random)
            {
                return logs[UnityEngine.Random.Range(0, logs.Count - 1)].gameObject;
            }

            foreach (var v in logs)
            {

                if (item == null || Vector3.Distance(scanLocation, v.transform.position) < distance)
                {
                    distance = Vector3.Distance(scanLocation, v.transform.position);
                    item = v.gameObject;
                }
            }
            if (item != null) return item;


            if (trees.Count > 0 && random)
            {
                return trees[UnityEngine.Random.Range(0, trees.Count - 1)].gameObject;
            }
            foreach (var v in trees)
            {

                if (item == null || Vector3.Distance(scanLocation, v.transform.position) < distance)
                {
                    distance = Vector3.Distance(scanLocation, v.transform.position);
                    item = v.gameObject;
                }
            }

            if (item != null) return item;

            if (destructibles.Count > 0 && random)
            {
                return destructibles[UnityEngine.Random.Range(0, destructibles.Count - 1)].gameObject;
            }
            foreach (var v in destructibles)
            {

                if (item == null || Vector3.Distance(scanLocation, v.transform.position) < distance)
                {
                    distance = Vector3.Distance(scanLocation, v.transform.position);
                    item = v.gameObject;
                }
            }

            return item;
        }

    }


    class CustomAI
    {
        public CustomAI(BaseAI ai)
        {
            this.ai = ai;
        }
        BaseAI ai;
        public void MoveTo(float dt, Vector3 point, float dist, bool run, Transform villagerTransform)
        {
            if (ai.m_character.m_flying)
            {
                dist = Mathf.Max(dist, 1f);
                float height;
                if (ai.GetSolidHeight(point, 0f, ai.m_flyAltitudeMin * 2f, out height))
                {
                    point.y = Mathf.Max(point.y, height + ai.m_flyAltitudeMin);
                }
                MoveAndAvoid(dt, point, dist, run, villagerTransform); //originally we return this
                return;
            }

            float num = (run ? 1f : 0.5f);
            if (Utils.DistanceXZ(point, villagerTransform.position) < Mathf.Max(dist, num))
            {
                //ai.StopMoving();
                return;
            }
            if (!ai.FindPath(point))
            {
                //ai.StopMoving();
                return;
            }
            if (ai.m_path.Count == 0)
            {
                //ai.StopMoving();
                return;
            }
            Vector3 vector = ai.m_path[0];
            if (Utils.DistanceXZ(vector, villagerTransform.position) < num)
            {
                ai.m_path.RemoveAt(0);
                if (ai.m_path.Count == 0)
                {
                    //ai.StopMoving();
                    return;
                }
            }
            else
            {
                Vector3 normalized2 = (vector - villagerTransform.position).normalized;
                MoveTowards(normalized2, run);
            }
            return;
        }


        public void MoveTowards(Vector3 dir, bool run)
        {
            dir = dir.normalized;
            ai.LookTowards(dir);

            if (ai.IsLookingTowards(dir, ai.m_moveMinAngle))
            {
                ai.m_character.SetMoveDir(dir);
                ai.m_character.SetRun(run);
                if (ai.m_jumpInterval > 0f && ai.m_jumpTimer >= ai.m_jumpInterval)
                {
                    ai.m_jumpTimer = 0f;
                    ai.m_character.Jump();
                }
            }
            else
            {
                ai.StopMoving();
            }
        }

        protected bool MoveAndAvoid(float dt, Vector3 point, float dist, bool run, Transform villagerTransform)
        {
            Vector3 vector = point - villagerTransform.position;
            if (ai.m_character.IsFlying())
            {
                if (vector.magnitude < dist)
                {
                    ai.StopMoving();
                    return true;
                }
            }
            else
            {
                vector.y = 0f;
                if (vector.magnitude < dist)
                {
                    ai.StopMoving();
                    return true;
                }
            }
            vector.Normalize();
            float radius = ai.m_character.GetRadius();
            float num = radius + 1f;
            if (!ai.m_character.InAttack())
            {
                ai.m_getOutOfCornerTimer -= dt;
                if (ai.m_getOutOfCornerTimer > 0f)
                {
                    Vector3 dir = Quaternion.Euler(0f, ai.m_getOutOfCornerAngle, 0f) * -vector;
                    MoveTowards(dir, run);
                    return false;
                }
                ai.m_stuckTimer += Time.fixedDeltaTime;
                if (ai.m_stuckTimer > 1.5f)
                {
                    if (Vector3.Distance(villagerTransform.position, ai.m_lastPosition) < 0.2f)
                    {
                        ai.m_getOutOfCornerTimer = 4f;
                        ai.m_getOutOfCornerAngle = UnityEngine.Random.Range(-20f, 20f);
                        ai.m_stuckTimer = 0f;
                        return false;
                    }
                    ai.m_stuckTimer = 0f;
                    ai.m_lastPosition = villagerTransform.position;
                }
            }
            if (ai.CanMove(vector, radius, num))
            {
                MoveTowards(vector, run);
            }
            else
            {
                Vector3 forward = villagerTransform.position;
                if (ai.m_character.IsFlying())
                {
                    forward.y = 0.2f;
                    forward.Normalize();
                }
                Vector3 vector2 = villagerTransform.right * radius * 0.75f;
                float num2 = num * 1.5f;
                Vector3 centerPoint = ai.m_character.GetCenterPoint();
                float num3 = ai.Raycast(centerPoint - vector2, forward, num2, 0.1f);
                float num4 = ai.Raycast(centerPoint + vector2, forward, num2, 0.1f);
                if (num3 >= num2 && num4 >= num2)
                {
                    MoveTowards(forward, run);
                }
                else
                {
                    Vector3 dir2 = Quaternion.Euler(0f, -20f, 0f) * forward;
                    Vector3 dir3 = Quaternion.Euler(0f, 20f, 0f) * forward;
                    if (num3 > num4)
                    {
                        MoveTowards(dir2, run);
                    }
                    else
                    {
                        MoveTowards(dir3, run);
                    }
                }
            }
            return false;
        }



    }













}
