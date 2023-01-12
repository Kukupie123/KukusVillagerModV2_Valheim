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
                    case VillagerState.Roaming:
                        RoamAround();
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
                WorkLoop();
            }

        }



        //Functions that execute every frame---------------------------

        //If we are following a target, this function checks how long have we been following the target for and if it crosses a threshold we tp the villager to that location if acceptable distance has not been reached
        double? startedFollowingTime = null;
        float AcceptedFollowDistance = VillagerModConfigurations.AcceptedFollowDistance;
        int FollowTimeLimit = VillagerModConfigurations.FollowTimeLimit;
        private void FollowCheckPerUpdate()
        {
            //Check if followingObjZDOID is valid and that we are not in following state. following state has it's own function to take care of it
            if (villagerGeneral.GetVillagerState() != VillagerState.Following && ZDOMan.instance.GetZDO(followingObjZDOID) != null && ZDOMan.instance.GetZDO(followingObjZDOID).IsValid())
            {
                float distance = Vector3.Distance(transform.position, ZDOMan.instance.GetZDO(followingObjZDOID).GetPosition());

                //If distance is acceptable then we do not proceed
                if (distance < AcceptedFollowDistance)
                {
                    startedFollowingTime = null;
                    return;
                }

                //Check if we started counting already
                if (startedFollowingTime == null)
                {
                    startedFollowingTime = ZNet.instance.GetTimeSeconds();
                }

                //Calculate time elasped and distance between villager and target
                double timeElapsedSec = ZNet.instance.GetTimeSeconds() - startedFollowingTime.Value;

                if (timeElapsedSec > FollowTimeLimit) //Time limit reached, TP the villager and reset Timer
                {
                    KLog.warning($"Teleporting villager {villagerGeneral.ZNV.GetZDO().m_uid.id} Because he didn't reach following target within the time limit");
                    TPToLoc(ZDOMan.instance.GetZDO(this.followingObjZDOID).GetPosition(), false); //We want the villager to still follow it's target after tping so we set false as 2nd parameter
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
        int timeLimitForMove = VillagerModConfigurations.MoveTimeLimit;
        private void MovePerUpdateIfDesired()
        {
            //Only continue if we are moving
            if (keepMoving == true)
            {
                //Reset AI
                ai.ResetPatrolPoint();

                //Check if we have set starting timer
                if (keepMovingStartTime == null)
                {
                    keepMovingStartTime = ZNet.instance.GetTime();
                }

                //Check if villager has been trying to reach target for too long
                double timeDiff = (ZNet.instance.GetTime() - keepMovingStartTime.Value).TotalSeconds;

                if (timeDiff > timeLimitForMove && Vector3.Distance(transform.position, movePos) > acceptableDistance) //60 sec passed and still hasn't reached path so we tp
                {
                    KLog.warning($"TPing Villager {villagerGeneral.ZNV.GetZDO().m_uid.id} Because time limit reached for moving");
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

        async private void WorkLoop()
        {
            ZDOID wp = villagerGeneral.GetWorkPostID();

            if (wp == null || wp.IsNone())
            {
                return;
            }

            if (villagerGeneral.GetWorkZDO() == null || villagerGeneral.GetWorkZDO().IsValid() == false)
            {
                return;
            }

            ZDOID containerID = villagerGeneral.GetContainerID();

            if (containerID == null || containerID.IsNone())
            {
                return;
            }

            ZDO containerZDO = villagerGeneral.GetContainerZDO();
            if (containerZDO == null || containerZDO.IsValid() == false)
            {
                return;
            }

            if (villagerGeneral.GetVillagerState() == VillagerState.Working)
            {
                if (villagerGeneral.GetWorkSkill_CanPickUp())
                {
                    await PickupAndStoreWork();
                }
                if (villagerGeneral.GetWorkSkill_CanSmelt())
                {
                    await RefillWork();
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
            ai.SetFollowTarget(target);
            return true;
        }

        private bool TPToLoc(Vector3 pos, bool RemoveFollowTarget = true)
        {
            if (RemoveFollowTarget)
                ai.SetFollowTarget(null);
            transform.position = pos; //THIS WORKS. BUT IDK HOW IT'S GOING TO BE IN SERVERS. FUTURE:: CHANGE THIS WITH SOME SERVER CALLS
            villagerGeneral.ZNV.GetZDO().SetPosition(pos); //THIS IS NOT RELIABLE IDK WHY
            return true;
        }

        public bool FollowPlayer(ZDOID playerID)
        {

            if (playerID == null || playerID.IsNone())
            {
                talk.Say("Can't follow, invalid ZDOID", "Follow");
                return false;
            }

            //Stop moving incase the villager was moving earlier
            keepMoving = false;
            //Update state
            villagerGeneral.SetVillagerState(VillagerState.Following);

            GameObject playerGO = ZNetScene.instance.FindInstance(playerID);
            if (playerGO == null)
            {
                talk.Say("Failed to find the player, Going to guard bed", "Follow");
                GuardBed();
                return false;
            }

            FollowGameObject(playerGO);

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

        public bool RoamAround(bool shouldTalk = true)
        {

            //Stop moving
            keepMoving = false;
            //Remove from follower
            ai.SetFollowTarget(null);
            this.followingObjZDOID = new ZDOID { m_hash = -1, m_userID = -1 };

            //Update the state of the villager
            villagerGeneral.SetVillagerState(VillagerState.Roaming);

            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();
            if (shouldTalk)
                talk.Say("Roaming Around", "Roam");
            ai.RandomMovement(ai.GetWorldTimeDelta(), transform.position);
            return true;
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
                talk.Say("Defending Post", "Defend");
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
            alreadyPickingUp = false;
            alreadyRefilling = false;
            ZDOID wp = villagerGeneral.GetWorkPostID();

            if (wp == null || wp.IsNone())
            {
                talk.Say("No Work Post assigned", "Work");
                RoamAround(false);
                return false;
            }

            if (villagerGeneral.GetWorkZDO() == null || villagerGeneral.GetWorkZDO().IsValid() == false)
            {
                talk.Say("My Work post was destroyed", "Work");
                RoamAround(false);
                return false;
            }

            ZDOID containerID = villagerGeneral.GetContainerID();

            if (containerID == null || containerID.IsNone())
            {
                talk.Say("Container not assigned", "Work");
                RoamAround(false);
                return false;
            }

            ZDO containerZDO = villagerGeneral.GetContainerZDO();
            if (containerZDO == null || containerZDO.IsValid() == false)
            {
                talk.Say("Container was destroyed", "Work");
                RoamAround(false);
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

        int minRandomTime = VillagerModConfigurations.MinWaitTimeWork;
        int maxRandomTime = VillagerModConfigurations.MaxWaitTimeWork;
        bool workTalk = VillagerModConfigurations.TalkWhileWorking;
        bool workRun = VillagerModConfigurations.workRun;
        bool alreadyPickingUp = false;
        async private Task PickupAndStoreWork()
        {
            try
            {


                if (alreadyPickingUp || alreadyRefilling) return;
                alreadyPickingUp = true;

                ZDO WorkPostZDO = villagerGeneral.GetWorkZDO();
                Vector3 workPosLoc = WorkPostZDO.GetPosition();

                if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                {
                    alreadyPickingUp = false;
                    return;
                }



                //Move to workpost
                MoveVillagerToLoc(workPosLoc, 3f, false, false, workRun);
                while (keepMoving)
                {
                    ai.ResetPatrolPoint();
                    ai.LookAt(workPosLoc);
                    movePos = workPosLoc;
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
                    alreadyPickingUp = false;
                    return;
                }

                //Search for pickable item
                ItemDrop pickable = FindClosestValidPickup(workPosLoc, 250f);

                //ItemDrop found.
                if (pickable != null)
                {
                    KLog.warning($"PICKING UP {pickable.m_itemData.m_shared.m_name}");

                    if (workTalk)
                        talk.Say($"Going to Pickup {pickable.m_itemData.m_shared.m_name}", "Work");

                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        alreadyPickingUp = false;
                        return;
                    }



                    //Move to the pickable item 
                    MoveVillagerToLoc(pickable.transform.position, 1f, false, false, workRun);

                    while (keepMoving)
                    {
                        ai.ResetPatrolPoint();
                        ai.LookAt(workPosLoc);
                        movePos = pickable.transform.position;
                        await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                        if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                        {
                            break;
                        }
                    }


                    await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        alreadyPickingUp = false;
                        return;
                    }

                    //Fake pickup by storing the prefab, and deleting the GO from world if only 1 stack or else reduce stack by one
                    string prefabName = pickable.m_itemData.m_dropPrefab.name;
                    int stackCount = pickable.m_itemData.m_stack;
                    var prefab = PrefabManager.Instance.GetPrefab(prefabName);

                    if (prefab == null)
                    {
                        alreadyPickingUp = false;
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
                    if (workTalk)
                        talk.Say($"Going to Put {prefabName} in container", "Work");

                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        alreadyPickingUp = false;
                        return;
                    }



                    MoveVillagerToLoc(containerLoc, 3f, false, false, workRun);

                    while (keepMoving)
                    {
                        ai.ResetPatrolPoint();
                        ai.LookAt(workPosLoc);
                        movePos = containerLoc;
                        await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                        if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                        {
                            break;
                        }
                    }




                    await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        alreadyPickingUp = false;
                        return;
                    }

                    if (villagerGeneral.GetContainerInstance().GetComponent<Container>().GetInventory().AddItem(prefab, 1))
                    {
                        if (workTalk)
                            talk.Say($"Added {prefabName} to container", "Work");
                    }
                    else
                    {
                        if (workTalk)
                            talk.Say($"Failed to add {prefabName} to container", "Work");
                    }
                }
                else
                {
                    if (workTalk)
                        talk.Say("Found nothing nearby that can be put in container", "Work");

                    KLog.info("Found nothing to pickup");
                }

                alreadyPickingUp = false;


            }
            catch (Exception)
            {
                alreadyPickingUp = false;
            }



        }

        bool alreadyRefilling = false;
        async private Task RefillWork()
        {
            try
            {
                if (alreadyRefilling || alreadyPickingUp) return;
                alreadyRefilling = true;
                ZDO WorkPostZDO = villagerGeneral.GetWorkZDO();
                Vector3 workPosLoc = WorkPostZDO.GetPosition();
                //Move to workpost

                if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                {
                    alreadyRefilling = false;
                    return;
                }



                MoveVillagerToLoc(workPosLoc, 3f, false, false, workRun);
                while (keepMoving)
                {
                    ai.ResetPatrolPoint();
                    ai.LookAt(workPosLoc);
                    movePos = workPosLoc;
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
                    alreadyRefilling = false;
                    return;
                }

                //Find smelter
                Smelter smelter = FindValidSmelter(workPosLoc, 500f, true);
                if (smelter != null)
                {
                    //Go to container and remove the fuel or cookable or both

                    if (workTalk)
                        talk.Say("Going to container to get items for smelting", "Work");

                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        alreadyRefilling = false;
                        return;
                    }


                    //Move to container
                    MoveVillagerToLoc(villagerGeneral.GetContainerZDO().GetPosition(), 2f, false, false, workRun);

                    while (keepMoving)
                    {
                        ai.ResetPatrolPoint();
                        ai.LookAt(workPosLoc);
                        movePos = villagerGeneral.GetContainerZDO().GetPosition();
                        await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                        if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                        {
                            break;
                        }
                    }




                    await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        alreadyRefilling = false;
                        return;
                    }


                    bool tookFuel = false;
                    bool tookCookable = false;
                    //Check and remove fuel/cookable
                    var inventory = villagerGeneral.GetContainerInstance().GetComponent<Container>().GetInventory();


                    foreach (var i in inventory.GetAllItems())
                    {
                        if (i.m_shared.m_name.Equals(smelter.m_fuelItem.m_itemData.m_shared.m_name))
                        {
                            tookFuel = true;
                            inventory.RemoveItem(smelter.m_fuelItem.m_itemData.m_shared.m_name, 1);
                            break;
                        }
                    }



                    ItemDrop.ItemData cookableItem = smelter.FindCookableItem(inventory);
                    if (cookableItem != null)
                        foreach (var i in inventory.GetAllItems())
                        {

                            if (i.m_shared.m_name.Equals(cookableItem.m_shared.m_name))
                            {
                                tookCookable = true;
                                inventory.RemoveItem(cookableItem.m_shared.m_name, 1);
                                break;
                            }
                        }


                    if (tookFuel == false && tookCookable == false)
                    {
                        if (workTalk)
                            talk.Say("No processable or fuel in my container or the smelter I was going to fill is already full now", "");
                        alreadyRefilling = false;
                        return;
                    }

                    if (workTalk)
                        talk.Say("Moving to Smelter to fill it.", "Work");

                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        alreadyRefilling = false;
                        return;
                    }

                    //Go to smelter
                    MoveVillagerToLoc(smelter.transform.position, 4f, false, false, workRun);

                    while (keepMoving)
                    {
                        ai.ResetPatrolPoint();
                        ai.LookAt(workPosLoc);
                        movePos = smelter.transform.position;
                        await Task.Delay(UnityEngine.Random.Range(minRandomTime, maxRandomTime));
                        if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                        {
                            break;
                        }
                    }




                    await Task.Delay(500);
                    //Add fuel to the smelter

                    int fuelCapacity = smelter.m_maxFuel;
                    float currentFuel = (int)smelter.GetFuel();
                    int cookableCap = smelter.m_maxOre;
                    int currentCookableSize = smelter.GetQueueSize();

                    if (tookFuel)
                    {
                        //Can we add fuel?
                        if (currentFuel < fuelCapacity)
                        {
                            smelter.GetComponentInParent<ZNetView>().InvokeRPC("AddFuel");
                        }
                        else
                        {
                            inventory.AddItem(smelter.m_fuelItem.m_itemData);
                        }
                    }
                    if (tookCookable)
                    {
                        if (currentCookableSize < cookableCap)
                        {
                            smelter.GetComponentInParent<ZNetView>().InvokeRPC("AddOre", cookableItem.m_dropPrefab.name);

                        }
                        else
                        {
                            var cookable = PrefabManager.Instance.GetPrefab(cookableItem.m_dropPrefab.name);
                            inventory.AddItem(cookable.GetComponent<ItemDrop>().m_itemData);

                        }
                    }



                }
                else
                {
                    if (workTalk)
                        talk.Say("Found no smelter nearby that needs to be filled", "Work");
                }
                alreadyRefilling = false;
            }
            catch (Exception)
            {
                alreadyRefilling = false;
            }




        }

        private ItemDrop FindClosestValidPickup(Vector3 center, float radius)
        {
            //Scan for objects that we can pickup and add it in list
            Collider[] colliders = Physics.OverlapSphere(center, radius);

            string PickupPrefabNames = VillagerModConfigurations.PickableObjects.Trim() + ",randomstuff";
            List<string> pickUpNameList = new List<string>();
            string p = "";
            for (int i = 0; i < PickupPrefabNames.Length; i++)
            {
                char c = PickupPrefabNames[i];
                if (c.Equals(' ')) continue;
                if (c.Equals(','))
                {
                    if (p.Length == 0) continue;
                    pickUpNameList.Add(p);
                    KLog.info($"Pickup item {i + 1} = {p}");
                    p = "";
                    continue;
                }
                p = p + c;
            }


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

                //validate container and inventory
                var containerGO = villagerGeneral.GetContainerInstance();
                if (containerGO == null) continue;
                var container = containerGO.GetComponent<Container>();
                if (container == null) continue;
                var inventory = container.GetInventory();
                if (inventory == null) continue;
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

                int fuelCapacity = d.m_maxFuel;
                float currentFuel = (int)d.GetFuel();
                int cookableCap = d.m_maxOre;
                int currentCookable = d.GetQueueSize();



                //Check if contanier has the fuel
                var inventory = container.GetComponent<Container>().GetInventory();
                ItemDrop.ItemData fuel = d.m_fuelItem.m_itemData;
                var cookable = d.FindCookableItem(inventory);
                bool fuelPresent = false;
                bool cookablePresent = false;

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

            if (getRandom && validSmelters.Count > 0)
            {
                smelter = validSmelters[UnityEngine.Random.Range(0, validSmelters.Count - 1)];
            }
            return smelter;
        }



    }
}
