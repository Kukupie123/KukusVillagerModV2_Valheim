using KukusVillagerMod.Components;
using KukusVillagerMod.enums;
using System;
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
        float acceptableDistance = 2f;
        Vector3 movePos; //the location to move to
        private void MovePerUpdateIfDesired()
        {
            if (keepMoving == true)
            {
                KLog.info($"Moving to {movePos}");
                if (ai.HavePath(movePos) == false)
                {
                    TPToLoc(movePos);
                    keepMoving = false;

                }
                else if (ai.MoveAndAvoid(ai.GetWorldTimeDelta(), movePos, acceptableDistance, true))
                {
                    keepMoving = false;
                }
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
            keepMoving = false;
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
        public bool MoveVillagerToLoc(Vector3 pos, bool keepFollower = true)
        {

            movePos = pos; //update the movePos
            acceptableDistance = 2f;
            keepMoving = true;

            //FUTURE
            if (!keepFollower)
            {
                RemoveVillagersFollower();
                villagerGeneral.SetVillagerState(VillagerState.Moving);
            }
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
    }
}
