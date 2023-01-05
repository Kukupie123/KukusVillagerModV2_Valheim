
using KukusVillagerMod.enums;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace KukusVillagerMod.States
{
    class VillagerLifeCycle : MonoBehaviour
    {
        public ZNetView znv;


        public int villagerLevel; //The level of the villager. Has to be set during creature prefab setup
        public int villagerType; //The type of villager. Servers no purpose anymore, will remove soon
        public int health; //The health of the villager. Has to be set during the creature prefab setup.


        public GameObject followingTarget; //Used to store the game object the villager is following
        public long followingPlayerID; //Used to figure out which player the villagers are following

        MonsterAI ai;
        public Humanoid humanoid;
        NpcTalk talk;

        Vector3 moveToTarget; //used in FixedUpdate to make the ai go to a location
        bool keepMoving = false; //Used in FixedUpdate to make ai go to a location

        private void Awake()
        {
            znv = GetComponentInParent<ZNetView>();
            znv.SetPersistent(true); //ZNV has to be persistent
            ai = GetComponent<MonsterAI>();
            talk = GetComponent<NpcTalk>();
            //ai.m_alertRange = 500f; TEST THIS RANGE OUT

            //Setting the values set during prefab setup 
            humanoid = GetComponent<Humanoid>();
            humanoid.SetLevel(villagerLevel);
            humanoid.SetMaxHealth(health);
            humanoid.SetHealth(health);

        }



        private void OnDestroy()
        {
        }


        //Ignore collision with player
        private void OnCollisionEnter(Collision collision)
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (character != null
                && character.m_faction == Character.Faction.Players
                && character.GetComponent<VillagerLifeCycle>() == null) // allow collision between minions
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
                return;
            }


        }
        //updateRanOnce is used to determine if FixedUpdate function has been run once after certain criterias were met. It will perform operations that needs to be done once but needs certain conditions to be met so we can't do them in Awake Method
        bool updateRanOnce = false;
        DateTime? startingTimeForBedNotFound;
        private void FixedUpdate()
        {
            if (!KukusVillagerMod.isMapDataLoaded) return;
            //Wait for the bed's ID which spawned this villagers to be saved in the zdo of this villager. The threshold is 10 sec. If we fail to find bed in 10 sec then we are going to assume that this villager was spawned without a bed and needs to be destroyed
            if (!isBedAssigned())
            {
                //Set starting time. Will execute only once
                if (startingTimeForBedNotFound == null)
                {
                    KLog.warning("SET STARTING TIME FOR BED NOT ASSIGNED");
                    startingTimeForBedNotFound = ZNet.instance.GetTime();
                }

                DateTime currentTime = DateTime.Now;

                var timeElasped = currentTime - startingTimeForBedNotFound.Value;

                if (timeElasped.TotalSeconds > 10)
                {
                    //if we crossed 10 sec of waiting we are destroying thezdo
                    var zdo = base.GetComponent<ZNetView>().GetZDO();
                    KLog.warning("10 sec passed since the villager has not found a bed. Destroying");
                    ZDOMan.instance.DestroyZDO(zdo);
                }
                return;
            }

            //Runs only once per creature load
            if (!updateRanOnce)
            {
                if (talk == null) talk = GetComponentInParent<NpcTalk>();
                //When loaded see what is the state that the villager needs to be at
                switch ((VillagerState)GetBedZDO().GetInt("state", (int)VillagerState.Guarding_Bed))
                {
                    case VillagerState.Guarding_Bed:
                        GuardBed();
                        break;
                    case VillagerState.Defending_Post:
                        DefendPost();
                        break;
                    case VillagerState.Following:
                        //Too much work to make them follow player again
                        GuardBed();
                        break;
                }
                updateRanOnce = true;
            }




            //Check if the bed assigned is valid. Or else destroy
            if (!GetBedZDO().IsValid())
            {
                ZNetScene.instance.Destroy(this.gameObject);
            }

            //TP to player if distance > certain distance, following and the following target is a valid player
            if (followingTarget != null && followingTarget.GetComponent<Player>() != null && (VillagerState)GetBedZDO().GetInt("state", (int)VillagerState.Guarding_Bed) == VillagerState.Following)
            {
                //TP if distance is greater or player is teleporting
                if (Vector3.Distance(followingTarget.transform.position, transform.position) > 70 || followingTarget.GetComponent<Player>().IsTeleporting() || ai.FindPath(followingTarget.transform.position) == false)
                {
                    transform.position = followingTarget.transform.position;
                }
            }

            //Move to designated location if keepMoving was set to true
            else if (keepMoving)
            {
                //Move to the moveToTarget and disable keepMoving when villager reaches that location
                if (ai.MoveTo(ai.GetWorldTimeDelta(), moveToTarget, 2.5f, true))
                {
                    keepMoving = false;
                }
            }

        }

        //Returns true if a bed was assigned to this villager after it spawned
        private bool isBedAssigned()
        {

            ZDOID zdoid = this.znv.GetZDO().GetZDOID("spawner_id");

            //if zdoid is not null and it exists then we can say that the bed has been assigned for this villager after it spawned
            if (!zdoid.IsNone() && ZDOMan.instance.GetZDO(zdoid) != null)
            {
                return true;
            }
            return false;
        }

        //Returns GO based on the ZDOID of the bed saved in the ZDO of this creature, will return null if not loaded in memory
        public GameObject GetBed()
        {
            ZDOID zdoid = this.znv.GetZDO().GetZDOID("spawner_id");
            return ZNetScene.instance.FindInstance(zdoid);
        }

        //Returns the ZDO of the bed that spawned this creature
        public ZDO GetBedZDO()
        {
            var id = this.znv.GetZDO().GetZDOID("spawner_id");
            return ZDOMan.instance.GetZDO(id);
        }




        //COMMANDS----------------------

        /// <summary>
        /// Follow the Go, if Go is null, TP the villager to the Vector3 position
        /// </summary>
        /// <param name="target">The Go to follow</param>
        /// <param name="position">The location to TP to if target is null</param>
        private void FollowTarget(GameObject target, Vector3? position)
        {
            if (target == null)
            {
                if (position != null)
                {
                    ai.ResetPatrolPoint();
                    ai.ResetRandomMovement();
                    transform.position = position.Value;
                    return;
                }
                else
                {
                    KLog.warning("Following target & position are null ");
                    return;
                }

            }
            this.followingTarget = target;
            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();
            ai.SetFollowTarget(target);

            if (ai.FindPath(target.transform.position) == false || ai.HavePath(target.transform.position) == false || ZNetScene.instance.IsAreaReady(transform.position) == false || ZNetScene.instance.IsAreaReady(target.transform.position) == false)
            {
                transform.position = target.transform.position;
            }

        }




        /// <summary>
        /// Guards the bed it was assigned. Updates the state of the bed to "Guarding Bed" Will teleport to the bed if no path available. Or if GO not loaded in memory.
        /// </summary>
        /// <returns>true if successfully executed</returns>
        public bool GuardBed()
        {
            var bed = GetBed();

            removeFromFollower(); //Remove the villager from follower in case it was following.
            StopMoving(); //Sets the keepMoving boolean to false so that the character stops moving
            FollowTarget(bed, GetBedZDO().GetPosition()); //if bed is not within loaded range then teleport there

            if (talk != null)
                talk.Say("Going to my bed and guard that area", "Bed");

            //Update state in ZDO of bed
            GetBedZDO().Set("state", (int)VillagerState.Guarding_Bed);
            return true;
        }


        /// <summary>
        /// Follows the player and updates state of the bed to "Following"
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool FollowPlayer(Player p)
        {
            //If player's area is not ready we are aborting follow command
            if (ZNetScene.instance.IsAreaReady(transform.position) == false)
            {
                return false;
            }
            StopMoving();

            FollowTarget(p.gameObject, null);

            //Update the state in bed's ZDO
            GetBedZDO().Set("state", (int)VillagerState.Following);

            if (talk != null)
                talk.Say($"Following you {Player.m_localPlayer.GetHoverName()}", "Following");
            this.followingPlayerID = p.GetPlayerID(); //Save the playerID of the player it is following.

            return true;
        }

        /// <summary>
        /// Moves the villager to the target 
        /// </summary>
        /// <param name="target"></param>
        public void MoveTo(Vector3 target)
        {

            this.followingTarget = null;
            ai.SetFollowTarget(null);
            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();

            if (ai.FindPath(target) == false || ai.HavePath(target) == false)
            {
                transform.position = target;
            }
            else
            {
                moveToTarget = target;
                keepMoving = true;
            }

            if (talk != null)
                talk.Say("Moving to position", "Moving");

        }


        /// <summary>
        /// Guards the defense post it was assigned. If no defense post assigned it will do nothing.
        /// </summary>
        /// <returns></returns>
        public bool DefendPost()
        {
            var defenseID = GetBedZDO().GetZDOID("defense");
            if (defenseID.IsNone())
            {
                if (talk != null)
                    talk.Say("You have not given me any defense post to defend", "Defend");
                return false;
            }
            else
            {
                var defense = ZNetScene.instance.FindInstance(defenseID);

                StopMoving();

                removeFromFollower();
                FollowTarget(defense, ZDOMan.instance.GetZDO(defenseID).GetPosition());
                if (talk != null)
                    talk.Say($"Defending Post {defenseID.id}", "Defend");


                //Update the state in bed's ZDO
                GetBedZDO().Set("state", (int)VillagerState.Defending_Post);
                return true;
            }
        }

        //Sets the follow target of the AI to null and invalidates the followingPlayerID. DOES NOT UPDATE STATE OF THE BED
        private void removeFromFollower()
        {
            ai.SetFollowTarget(null);
            followingPlayerID = -1;
            this.followingTarget = null;
        }

        //Sets the keep moving variable to false. DOES NOT UPDATE STATE.
        private void StopMoving()
        {
            keepMoving = false;
        }

        //FUTURE
        public void CutTree()
        {
            var colliders = Physics.OverlapSphere(transform.position, 5000f);

            foreach (var c in colliders)
            {
                var tree = c?.gameObject?.GetComponentInParent<TreeBase>();
                var log = c?.gameObject?.GetComponentInParent<TreeLog>();
                var destructible = c?.gameObject?.GetComponentInParent<Destructible>();

                if (tree != null)
                {
                    ai.LookAt(tree.transform.position);
                    ai.DoAttack(null, false);
                }
                else if (log != null)
                {
                    ai.LookAt(log.transform.position);
                    ai.DoAttack(null, false);
                }
                else if (destructible != null)
                {
                    if (destructible.name.ToLower().Contains("stub"))
                    {
                        ai.LookAt(destructible.transform.position);
                        ai.DoAttack(null, false);
                    }
                }


            }
        }
    }

}






