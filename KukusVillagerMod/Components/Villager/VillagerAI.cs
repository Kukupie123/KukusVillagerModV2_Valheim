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

        private void SetKeepMoving(bool keepMoving)
        {
            this.keepMoving = keepMoving;
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

                if (timeDiff > 15) //15 sec passed and still hasn't reached path so we tp
                {
                    TPToLoc(movePos);
                    SetKeepMoving(false);
                    return;
                }
                if (ai.MoveAndAvoid(ai.GetWorldTimeDelta(), movePos, acceptableDistance, true))
                {
                    SetKeepMoving(false);
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
            SetKeepMoving(false);
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
        public bool MoveVillagerToLoc(Vector3 pos, float radius, bool keepFollower = true, bool shouldTalk = true)
        {

            movePos = pos; //update the movePos
            acceptableDistance = radius;

            //Check if already within range
            if (Vector3.Distance(transform.position, pos) < radius)
            {
                if (shouldTalk)
                {
                    talk.Say("Already near the move location", "move");



                }

                SetKeepMoving(false);
                if (!keepFollower)
                {
                    RemoveVillagersFollower();
                }
                return false;
            }
            SetKeepMoving(true);

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


        async private void WorkAsync()
        {
            while (true)
            {
                try
                {
                    if (villagerGeneral.GetVillagerState() != VillagerState.Working || villagerGeneral.GetContainerZDO().IsValid() == false || villagerGeneral.GetWorkZDO().IsValid() == false)
                    {
                        await Task.Delay(500);
                        continue;
                    }

                    //Go to work post
                    ZDO WorkPostZDO = villagerGeneral.GetWorkZDO();
                    Vector3 workPosLoc = WorkPostZDO.GetPosition();

                    MoveVillagerToLoc(workPosLoc, 3f, false, false);
                    while (keepMoving)
                    {
                        talk.Say("Going to Work Post", "work");
                        await Task.Delay(500);
                    }
                    await Task.Delay(500);
                    talk.Say("Reached Work Post", "work");

                    //Now search for pickable item
                    ItemDrop pickable = FindClosestPickup(workPosLoc, 250f);

                    if (pickable != null) //Go to the pickable item and "pick it up"
                    {
                        //Move to the pickable item 
                        MoveVillagerToLoc(pickable.transform.position, 1f, false, false);

                        while (keepMoving)
                        {
                            talk.Say($"Going to pickup {pickable.GetHoverName()}", "work");
                            await Task.Delay(500);
                        }
                        talk.Say("Can pickup item now", "work");
                        await Task.Delay(1000);
                        //Fake pickup
                        string prefabName = GetPrefabNameFromHoverName4ItemDrop(pickable.GetHoverName());
                        var prefab = PrefabManager.Cache.GetPrefab<GameObject>(prefabName);
                        KLog.info(prefab.name);
                        ZDOMan.instance.DestroyZDO(pickable.GetComponentInParent<ZNetView>().GetZDO());

                        //Find the container location and move there
                        Vector3 containerLoc = villagerGeneral.GetContainerZDO().GetPosition();
                        MoveVillagerToLoc(containerLoc, 3f, false, false);

                        while (keepMoving)
                        {
                            talk.Say($"Going to container to keep {prefab.name}", "work");
                            await Task.Delay(500);
                        }
                        talk.Say("Putting Item in storage", "work");
                        await Task.Delay(1000);
                        villagerGeneral.GetContainerInstance().GetComponent<Container>().GetInventory().AddItem(prefab, 1);
                    }
                }
                catch (Exception e)
                {
                    await Task.Delay(500);
                    //KLog.warning($"AI UPDATE ERROR IN VILLAGER AI {e.Message}");
                }


            }
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

        public void StartWork()
        {
            villagerGeneral.SetVillagerState(VillagerState.Working);
            return;


        }

    }
}
