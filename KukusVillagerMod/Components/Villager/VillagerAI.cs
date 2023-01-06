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

                    if (distance > 75)
                    {
                        transform.position = playerPos;
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
                ai.MoveAndAvoid(ai.GetWorldTimeDelta(), movePos, acceptableDistance, true);
            }
        }

        private bool FollowGameObject(GameObject target)
        {
            if (target == null) return false;
            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();
            ai.SetFollowTarget(target);
            return true;
        }

        /// <summary>
        /// Teleports the villager to the given location, doesn't matter if it's loaded or not.
        /// </summary>
        /// <param name="pos"></param>
        private bool TPToLoc(Vector3 pos)
        {
            villagerGeneral.ZNV.GetZDO().SetPosition(pos);
            return true;
        }

        /// <summary>
        /// Removes the follower from the villager
        /// </summary>
        private void RemoveVillagersFollower()
        {
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
            if (FollowGameObject(p.gameObject))
            {
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
            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();
            keepMoving = true;
            return true;
        }

        public bool GuardBed()
        {
            //Get the bed's instance
            var bed = villagerGeneral.GetBed();
            //if bed is valid we are going to make villager follow it
            if (bed != null && bed.gameObject != null)
            {
                RemoveVillagersFollower();
                return FollowGameObject(bed);
            }
            //Bed instance not valid. Get the bed's ZDO and use it's position to move villager
            else
            {
                var bedPos = villagerGeneral.GetBedZDO().GetPosition();
                RemoveVillagersFollower();
                if (TPToLoc(bedPos))
                {
                    villagerGeneral.SetVillagerState(VillagerState.Guarding_Bed);
                    return true;
                }
                return false;
                //Update villager's State
            }
        }

        public bool DefendPost()
        {
            //Get bed's ZDO and then defense's ZDOID stored
            var defensePostID = villagerGeneral.GetDefensePostID();
            if (defensePostID.IsNone())
            {
                return false;
            }
            //Get the instance of the defense
            var defenseInstance = villagerGeneral.GetDefensePostInstance();

            //if instance is valid we make villager follow it
            if (defenseInstance)
            {
                RemoveVillagersFollower();
                if (FollowGameObject(defenseInstance))
                {
                    villagerGeneral.SetVillagerState(VillagerState.Defending_Post);
                    return true;
                }
            }
            else
            {
                RemoveVillagersFollower();
                if (TPToLoc(villagerGeneral.GetDefenseZDO().GetPosition()))
                {
                    villagerGeneral.SetVillagerState(VillagerState.Defending_Post);
                    return true;
                }
            }
            return false;
        }
    }
}
