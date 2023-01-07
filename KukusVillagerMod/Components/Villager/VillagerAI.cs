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

        private void Awake()
        {
            this.villagerGeneral = GetComponent<VillagerGeneral>();
            ai = GetComponent<MonsterAI>();
        }


        bool updateRanOnce = false;
        private void FixedUpdate()
        {
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
                    default:
                        GuardBed();
                        break;
                }
            }
            else
            {
                //If following a player and player is valid
                if (villagerGeneral.GetVillagerState() == VillagerState.Following && followingPlayerZDOID.IsNone() == false)
                {
                    var playerPos = ZDOMan.instance.GetZDO(followingPlayerZDOID).GetPosition();
                    var distance = Vector3.Distance(transform.position, ZDOMan.instance.GetZDO(followingPlayerZDOID).GetPosition());

                    if (distance > 60)
                    {
                        villagerGeneral.ZNV.GetZDO().SetPosition(playerPos);
                        KLog.info($"Teleported Villager {villagerGeneral.ZNV.GetZDO().m_uid.id} to Player {followingPlayerZDOID.id}");
                    }
                }
                else //FUTURE : Change logic
                {
                    MovePerUpdateIfDesired(); //Will move the villager to a location if it needs to.
                }
            }

        }

        //Moves the villager to a location if it needs to, eg: Move command by player, Move to nearest tree to cut it, etc
        bool keepMoving = false; //used to determine if villager should keep moving or stop
        float acceptableDistance = 2f;
        Vector3 movePos; //the location to move to
        private void MovePerUpdateIfDesired()
        {
            if (keepMoving)
            {
                keepMoving = ai.MoveAndAvoid(ai.GetWorldTimeDelta(), movePos, acceptableDistance, true);
            }
        }


        private void StopMoving()
        {
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
            ai.SetFollowTarget(target);

            if (ai.FindPath(target.transform.position) == false || ai.HavePath(target.transform.position) == false || ZNetScene.instance.IsAreaReady(transform.position) == false || ZNetScene.instance.IsAreaReady(target.transform.position) == false)
            {
                TPToLoc(target.transform.position);
            }
            return true;
        }

        /// <summary>
        /// Teleports the villager to the given location, doesn't matter if it's loaded or not.
        /// </summary>
        /// <param name="pos"></param>
        private bool TPToLoc(Vector3 pos)
        {
            ai.SetFollowTarget(null);
            villagerGeneral.ZNV.GetZDO().SetPosition(pos);
            transform.position = pos;
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
            ZNetView playerZNV = p.GetComponent<ZNetView>();
            if (playerZNV == null)
            {
                playerZNV = p.GetComponentInParent<ZNetView>();
                if (playerZNV == null)
                {
                    talk.Say("Can't follow", nameof(FollowPlayer));
                    return false;
                }
            }
            followingPlayerZDOID = playerZNV.GetZDO().m_uid;

            //Get instance of the player
            var playerInstance = ZNetScene.instance.FindInstance(ZDOMan.instance.GetZDO(followingPlayerZDOID));

            //if instance of the player is not valid we do not follow
            if (playerInstance == null || !playerInstance.IsValid())
                return false;

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

            //FUTURE
            if (!keepFollower)
            {
                RemoveVillagersFollower();
                villagerGeneral.SetVillagerState(VillagerState.Moving);
            }
            talk.Say($"Moving to {pos.ToString()}", "Moving");
            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();
            keepMoving = true;
            return true;
        }

        public bool GuardBed()
        {
            var bed = villagerGeneral.GetBedInstance();

            RemoveVillagersFollower(); //Remove the villager from follower in case it was following.
            StopMoving(); //Sets the keepMoving boolean to false so that the character stops moving
            FollowGameObject(bed, villagerGeneral.GetBedZDO().GetPosition()); //if bed is not within loaded range then teleport there

            if (talk != null)
                talk.Say("Going to my bed and guard that area", "Bed");

            //Update state in ZDO of bed
            villagerGeneral.SetVillagerState(VillagerState.Guarding_Bed);
            return true;
        }

        public bool DefendPost()
        {
            var dp = villagerGeneral.GetDefensePostID();

            if (dp == null || dp.IsNone())
            {
                talk.Say("No Defense Post assigned", "Defense");
                return false;
            }

            var dpi = villagerGeneral.GetDefensePostInstance();

            RemoveVillagersFollower(); //Remove the villager from follower in case it was following.
            StopMoving(); //Sets the keepMoving boolean to false so that the character stops moving
            FollowGameObject(dpi, villagerGeneral.GetDefenseZDO().GetPosition()); //if bed is not within loaded range then teleport there

            talk.Say($"Going to defend my post {villagerGeneral.GetDefensePostID().id}", "Defense");

            //Update state in ZDO of bed
            villagerGeneral.SetVillagerState(VillagerState.Defending_Post);
            return true;
        }
    }
}
