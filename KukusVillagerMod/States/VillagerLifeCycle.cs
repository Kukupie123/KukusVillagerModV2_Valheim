
using KukusVillagerMod.enums;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace KukusVillagerMod.States
{
    class VillagerLifeCycle : MonoBehaviour
    {
        public ZNetView znv;


        public int villagerLevel;
        public int villagerType;
        public int health;


        public GameObject followingTarget;
        public long followingPlayerID; //Used to figure out which player the villagers are following

        MonsterAI ai;
        public Humanoid humanoid;
        NpcTalk talk;


        private void Awake()
        {
            znv = GetComponentInParent<ZNetView>();
            znv.SetPersistent(true);
            ai = GetComponent<MonsterAI>();
            talk = GetComponent<NpcTalk>();
            //ai.m_alertRange = 500f; TEST THIS RANGE OUT
            humanoid = GetComponent<Humanoid>();
            humanoid.SetLevel(villagerLevel);
            humanoid.SetMaxHealth(health);
            humanoid.SetHealth(health);





        }



        private void OnDestroy()
        {
        }

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

        bool updateRanOnce = false;
        DateTime? startingTimeForBedNotFound;
        private void FixedUpdate()
        {
            if (!KukusVillagerMod.isMapDataLoaded) return;
            //Wait for the bed's ID which spawned this villagers to be saved in the zdo of this villager
            if (!isBedAssigned())
            {
                if (startingTimeForBedNotFound == null)
                {
                    KLog.warning("SET STARTING TIME FOR BED NOT ASSIGNED");
                    startingTimeForBedNotFound = DateTime.Now;
                }


                DateTime currentTime = DateTime.Now;

                var a = currentTime - startingTimeForBedNotFound.Value;
                if (a.TotalSeconds > 10)
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

            //TP to player under certain conditions
            if (followingTarget != null && followingTarget.GetComponent<Player>() != null)
            {
                //TP if distance is greater or player is teleporting
                if (Vector3.Distance(followingTarget.transform.position, transform.position) > 70 || followingTarget.GetComponent<Player>().IsTeleporting() || ai.FindPath(followingTarget.transform.position) == false)
                {
                    transform.position = followingTarget.transform.position;
                }
            }

            //Move to designated location
            else if (followingTarget == null && keepMoving)
            {
                //Move to command was given and we will stop moving if we reach destination or if we follow someone
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
                    KLog.warning("Following target is null");
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





        public bool GuardBed()
        {
            var bed = GetBed();

            removeFromFollower();

            FollowTarget(bed, GetBedZDO().GetPosition()); //if bed is not within loaded range then teleport there

            if (talk != null)
                talk.Say("Going to my bed and guard that area", "Bed");

            //Update state in ZDO of bed
            GetBedZDO().Set("state", (int)VillagerState.Guarding_Bed);
            return true;
        }



        public bool FollowPlayer(Player p)
        {
            //If player's area is not ready we are aborting follow command
            if (ZNetScene.instance.IsAreaReady(transform.position) == false)
            {
                return false;
            }

            FollowTarget(p.gameObject, null);

            //Update the state in bed's ZDO
            GetBedZDO().Set("state", (int)VillagerState.Following);

            if (talk != null)
                talk.Say($"Following you {Player.m_localPlayer.GetHoverName()}", "Following");
            this.followingPlayerID = p.GetPlayerID();

            return true;
        }

        Vector3 moveToTarget; //used in FixedUpdate
        bool keepMoving = false; //Used in FixedUpdate
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
                removeFromFollower();
                FollowTarget(defense, ZDOMan.instance.GetZDO(defenseID).GetPosition());
                if (talk != null)
                    talk.Say($"Defending Post {defenseID.id}", "Defend");


                //Update the state in bed's ZDO
                GetBedZDO().Set("state", (int)VillagerState.Defending_Post);
                return true;
            }
        }

        private void removeFromFollower()
        {
            ai.SetFollowTarget(null);
            followingPlayerID = -1;
            this.followingTarget = null;
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






