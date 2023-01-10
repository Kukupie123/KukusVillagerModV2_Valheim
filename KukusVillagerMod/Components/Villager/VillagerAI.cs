using Jotunn.Managers;
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
        public ZDOID followingPlayerZDOID;
        bool updateRanOnce = false;

        private void Awake()
        {
            this.villagerGeneral = GetComponent<VillagerGeneral>();
            ai = GetComponent<MonsterAI>();
            updateRanOnce = false;
        }


        private void FixedUpdate()
        {
            if (!KukusVillagerMod.isMapDataLoaded || villagerGeneral.isBedAssigned() == false) return;


            if (talk == null) talk = GetComponent<NpcTalk>();



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
            }

        }

        //Moves the villager to a location if it needs to, eg: Move command by player, Move to nearest tree to cut it, etc
        bool keepMoving = false; //used to determine if villager should keep moving or stop
        bool shouldRun = true;

        private void SetKeepMoving(bool keepMoving, bool shouldRun)
        {
            this.keepMoving = keepMoving;
            this.shouldRun = shouldRun;
            if (keepMoving == false)
            {
                keepMovingStartTime = null;
            }
        }

        float acceptableDistance = 2f;
        Vector3 movePos; //the location to move to

        DateTime? keepMovingStartTime = null;
        private void MovePerUpdateIfDesired()
        {
            if (keepMoving == true)
            {
                //Check if we have set starting timer
                if (keepMovingStartTime == null)
                {
                    keepMovingStartTime = ZNet.instance.GetTime();
                }

                //Check if villager has been trying to reach target for too long
                double timeDiff = (ZNet.instance.GetTime() - keepMovingStartTime.Value).TotalSeconds;

                if (timeDiff > 60) //60 sec passed and still hasn't reached path so we tp
                {
                    TPToLoc(movePos);
                    SetKeepMoving(false, shouldRun);
                    return;
                }
                if (ai.MoveAndAvoid(ai.GetWorldTimeDelta(), movePos, acceptableDistance, shouldRun))
                {
                    SetKeepMoving(false, shouldRun);
                    return;
                }
            }
            else
            {
                keepMovingStartTime = null;
            }

        }

        private void TPVillagerToFollowerIfNeeded()
        {
            //If following a valid player with valid zdo
            if (villagerGeneral.GetVillagerState() == VillagerState.Following)
            {
                if (followingPlayerZDOID != null && followingPlayerZDOID.IsNone() == false && ZDOMan.instance.GetZDO(followingPlayerZDOID) != null && ZDOMan.instance.GetZDO(followingPlayerZDOID).IsValid() && ai.GetFollowTarget() != null && ai.GetFollowTarget().GetComponent<Player>() != null)
                {
                    Vector3 playerPos = ZDOMan.instance.GetZDO(followingPlayerZDOID).GetPosition();
                    float distance = Vector3.Distance(transform.position, ZDOMan.instance.GetZDO(followingPlayerZDOID).GetPosition());

                    if (distance > 60)
                    {
                        TPToLoc(playerPos);
                        KLog.info($"Teleported Villager {villagerGeneral.ZNV.GetZDO().m_uid.id} to Player {followingPlayerZDOID.id}");
                    }
                }
            }
        }


        private void StopMoving()
        {
            KLog.info($"{villagerGeneral.ZNV.GetZDO().m_uid.id} Stopped moving");
            SetKeepMoving(false, shouldRun);
        }

        /// <summary>
        /// Follow the given target, if invalid then TP to the backup pos, if backupPos is invalid return false
        /// </summary>
        /// <param name="target"></param>
        /// <param name="BackupPos"></param>
        /// <returns></returns>
        private bool FollowGameObject(GameObject target, Vector3? BackupPos)
        {

            if (target == null)
            {
                if (BackupPos != null)
                {
                    ai.ResetPatrolPoint();
                    ai.ResetRandomMovement();
                    TPToLoc(BackupPos.Value);
                    KLog.warning($"Teleporting villager {villagerGeneral.ZNV.GetZDO().m_uid.id} to pos {BackupPos.Value} for FollowObject method");
                    return true;
                }
                else
                {
                    KLog.warning("Following target & position are null ");
                    return false;
                }

            }
            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();
            if (ai.HavePath(target.transform.position) == false || Vector3.Distance(transform.position, target.transform.position) > 80 || ai.FindPath(target.transform.position) == false || ZNetScene.instance.IsAreaReady(target.transform.position) == false)
            {
                TPToLoc(target.transform.position);
            }
            ai.SetFollowTarget(target);


            return true;
        }

        /// <summary>
        /// Teleports the villager to the given location, doesn't matter if it's loaded or not.
        /// </summary>
        /// <param name="pos"></param>
        private bool TPToLoc(Vector3 pos)
        {
            ai.SetFollowTarget(null);
            transform.position = pos; //THIS WORKS. BUT IDK HOW IT'S GOING TO BE IN SERVERS. FUTURE:: CHANGE THIS WITH SOME SERVER CALLS
            villagerGeneral.ZNV.GetZDO().SetPosition(pos); //THIS IS NOT RELIABLE IDK WHY
            return true;
        }

        /// <summary>
        /// Removes the follower from the villager
        /// </summary>
        private void RemoveVillagersFollower()
        {
            ai.SetFollowTarget(null);
            followingPlayerZDOID = new ZDOID { m_hash = -1, m_userID = -1 };
        }


        public bool FollowPlayer(Player p)
        {
            if (p == null)
            {
                talk.Say("Can't follow", nameof(FollowPlayer));
            }

            //Save player's ZDO
            followingPlayerZDOID = p.GetZDOID();

            //Get instance of the player
            GameObject playerInstance = ZNetScene.instance.FindInstance(followingPlayerZDOID);

            //if instance of the player is not valid we do not follow
            if (playerInstance == null)
            {
                talk.Say("Can't follow", nameof(FollowPlayer));
                return false;
            }

            //Follow the target and update villager state value stored in bed
            if (FollowGameObject(p.gameObject, ZDOMan.instance.GetZDO(p.GetZDOID()).GetPosition()))
            {
                talk.Say($"Following {p.GetHoverText()}", nameof(FollowPlayer));
                villagerGeneral.SetVillagerState(VillagerState.Following);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Moves the villager to a location
        /// </summary>
        /// <param name="pos">The position to move to</param>
        /// <param name="keepFollower">If true, then will keep the villager as follower of the player. if false, will remove the villager as the follower of the player</param>
        /// <returns></returns>
        public bool MoveVillagerToLoc(Vector3 pos, float radius, bool keepFollower = true, bool shouldTalk = true, bool shouldRun = true)
        {

            movePos = pos; //update the movePos
            acceptableDistance = radius;

            //check if area is loaded, if not we TP to location
            if (ZNetScene.instance.IsAreaReady(pos) == false)
            {

                if (shouldTalk)
                {
                    talk.Say("Area is not loaded", "move");

                }

                SetKeepMoving(false, shouldRun);
                if (!keepFollower)
                {
                    RemoveVillagersFollower();
                }
                return false;
            }

            //Check if already within range
            if (Vector3.Distance(transform.position, pos) < radius)
            {
                if (shouldTalk)
                {
                    talk.Say("Already near the move location", "move");

                }

                SetKeepMoving(false, shouldRun);

                if (!keepFollower)
                {
                    RemoveVillagersFollower();
                }
                return false;
            }
            SetKeepMoving(true, shouldRun);

            //FUTURE
            if (!keepFollower)
            {
                RemoveVillagersFollower();
            }
            if (shouldTalk)
                talk.Say($"Moving to {pos.ToString()}", "Moving");
            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();
            ai.SetFollowTarget(null);
            return true;
        }

        public bool GuardBed()
        {

            ZDO bedZDO = villagerGeneral.GetBedZDO();


            if (bedZDO == null || bedZDO.IsValid() == false)
            {
                talk.Say("Couldn't find my bed", "Bed");
                return false;
            }
            villagerGeneral.SetVillagerState(VillagerState.Guarding_Bed);
            GameObject bed = villagerGeneral.GetBedInstance();
            RemoveVillagersFollower();
            StopMoving();
            FollowGameObject(bed, villagerGeneral.GetBedZDO().GetPosition());
            talk.Say("Guarding Bed", "Bed");
            return true;
        }

        public bool DefendPost()
        {
            ZDOID dp = villagerGeneral.GetDefensePostID();

            if (dp == null || dp.IsNone())
            {
                talk.Say("No Defense Post assigned", "Defense");
                return false;
            }

            if (villagerGeneral.GetDefenseZDO() == null || villagerGeneral.GetDefenseZDO().IsValid() == false)
            {
                talk.Say("My Defense post was destroyed", "Defense");
                return false;
            }
            //Update state in ZDO of bed
            villagerGeneral.SetVillagerState(VillagerState.Defending_Post);
            GameObject dpi = villagerGeneral.GetDefensePostInstance();
            RemoveVillagersFollower(); //Remove the villager from follower in case it was following.
            StopMoving(); //Sets the keepMoving boolean to false so that the character stops moving
            FollowGameObject(dpi, villagerGeneral.GetDefenseZDO().GetPosition()); //if bed is not within loaded range then teleport there
            talk.Say($"Defending post {villagerGeneral.GetDefensePostID().id}", "Defense");


            return true;
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
            villagerGeneral.SetVillagerState(VillagerState.Working);
            GameObject wpi = villagerGeneral.GetWorkInstance();
            RemoveVillagersFollower();

            //If wpi is invalid we are going to tp the villager to it's work post
            if (wpi == null)
            {
                TPToLoc(villagerGeneral.GetWorkZDO().GetPosition());
            }

            talk.Say("Working", "Work");
            return true;


        }





        //Worker AI
        /*
         * Need to figure out things hmmm
         * Things AI needs to be able to do
         * 1. Pickup valuables and store them in container,
         * 2. Take coals/ores from container and smelt them
         * 
         * Farming:
         * 1. Cut trees/mine rocks nearby. 
         * 2. Pickup and store
         */

        /// <summary>
        /// Runs once and never stops until villager is destroyed.
        /// Used to Handle Actions that need a lot if waiting and ordered execution Eg : Going to Work post and waiting to reach -> Then going to Pickup location and waiting to reach
        /// </summary>
        async private void WorkAsync()
        {
            while (true)
            {

                if (villagerGeneral.GetVillagerState() != VillagerState.Working || villagerGeneral.GetContainerZDO().IsValid() == false || villagerGeneral.GetWorkZDO().IsValid() == false)
                {
                    await Task.Delay(500);
                    continue;
                }

                //await PickupAndStoreWork();
                await RefillWork();
                await Task.Delay(500);




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
                await Task.Delay(500);

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
                //Move to the pickable item 
                MoveVillagerToLoc(pickable.transform.position, 1f, false, false, false);

                while (keepMoving)
                {
                    await Task.Delay(500);
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        break;
                    }
                }

                await Task.Delay(500);
                if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                {
                    return;
                }

                //Fake pickup by storing the prefab, and deleting the GO from world.
                string prefabName = GetPrefabNameFromHoverName4ItemDrop(pickable.GetHoverName());
                var prefab = PrefabManager.Cache.GetPrefab<GameObject>(prefabName);
                ZDOMan.instance.DestroyZDO(pickable.GetComponentInParent<ZNetView>().GetZDO());

                //Find the container location and move there
                Vector3 containerLoc = villagerGeneral.GetContainerZDO().GetPosition();
                MoveVillagerToLoc(containerLoc, 3f, false, false, false);

                while (keepMoving)
                {
                    await Task.Delay(500);
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        break;
                    }
                }

                await Task.Delay(1000);
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
            talk.Say("Moving to Work Post", "Work");
            //Move to workpost
            MoveVillagerToLoc(workPosLoc, 3f, false, false, false);
            while (keepMoving)
            {
                await Task.Delay(500);

                if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                {
                    break;
                }

                if (!keepMoving) break;
            }


            //Reached Work post, Check if still working
            await Task.Delay(500);
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
                    await Task.Delay(500);
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                    {
                        break;
                    }
                }

                await Task.Delay(500);
                if (villagerGeneral.GetVillagerState() != VillagerState.Working)
                {
                    return;
                }


                bool tookFuel = false;
                bool tookCookable = false;
                //Check and remove fuel/cookable
                var inventory = villagerGeneral.GetContainerInstance().GetComponent<Container>().GetInventory();

                if (inventory.HaveItem(smelter.m_fuelItem.m_itemData.m_shared.m_name))
                {
                    talk.Say("Found Fuel in container", "");
                    tookFuel = true;
                    inventory.RemoveItem(smelter.m_fuelItem.m_itemData.m_shared.m_name, 1);
                }
                talk.Say("REACHED!!!!", "Work");

                var cookableItem = smelter.FindCookableItem(inventory);
                if (cookableItem != null)
                {
                    talk.Say("Found Processable Item in container", "");
                    tookCookable = true;
                    inventory.RemoveItem(cookableItem, 1);
                }


                if (tookFuel == false && tookCookable == false)
                {
                    talk.Say("No processable or fuel in my container", "");
                    return;
                }


                talk.Say("Moving to Smelter to fill it.", "Work");

                //Go to smelter
                MoveVillagerToLoc(smelter.transform.position, 4f, false, false, false);

                while (keepMoving)
                {
                    await Task.Delay(500);
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
            else
            {
                talk.Say("No smelter found in sight", "Work");
                await Task.Delay(500);
            }

            return;


        }

        private string GetPrefabNameFromHoverName4ItemDrop(string hoverName)
        {
            return "Bronze";
        }

        private ItemDrop FindClosestPickup(Vector3 center, float radius)
        {
            //Scan for objects that we can pickup and add it in list
            Collider[] colliders = Physics.OverlapSphere(center, radius);

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

                string hoverName = d.GetHoverName();
                if (hoverName.Equals("$item_bronze"))
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
