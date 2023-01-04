
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


            //When loaded see what is the state that the villager needs to be at
            switch ((VillagerState)GetBedZDO().GetInt("state", (int)VillagerState.GuardingBed))
            {
                case VillagerState.GuardingBed:
                    GuardBed();
                    break;
                case VillagerState.GuardingDefensePost:
                    DefendPost();
                    break;
                case VillagerState.Journeying:
                    GuardBed();
                    break;
            }


        }



        private void OnDestroy()
        {
            Global.followers.Remove(this);
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


        private void FixedUpdate()
        {
            if (!KukusVillagerMod.isMapDataLoaded) return;

            //Wait for the bed's ID which spawned this villagers to be saved in the zdo of this villager
            if (!isBedAssigned()) return;

            //We reach here when the bed has been assigned

            //Check if the bed assigned is valid. Or else destroy
            if (!GetBedZDO().IsValid())
            {
                ZNetScene.instance.Destroy(this.gameObject);
            }


            if (followingTarget != null && followingTarget.GetComponent<Player>() != null)
            {
                //TP if distance is greater or player is teleporting
                if (Vector3.Distance(followingTarget.transform.position, transform.position) > 70 || followingTarget.GetComponent<Player>().IsTeleporting() || ai.FindPath(followingTarget.transform.position) == false)
                {
                    transform.position = followingTarget.transform.position;
                }
            }
            else if (followingTarget == null && keepMoving)
            {
                //Move to command was given and we will stop moving if we reach destination or if we follow someone
                if (ai.MoveTo(ai.GetWorldTimeDelta(), moveToTarget, 5f, true))
                {
                    keepMoving = false;
                }
            }

        }

        private bool isBedAssigned()
        {

            ZDOID zdoid = this.znv.GetZDO().GetZDOID("spawner_id");

            //if zdoid is not null and it exists then we can say that the bed has been assigned for this villager after it spawned
            if (!zdoid.IsNone() && ZDOMan.instance.GetZDO(zdoid) != null)
            {
                return true;
            }
            KLog.warning("bed not assigned yet");
            return false;
        }

        public GameObject GetBed()
        {
            ZDOID zdoid = this.znv.GetZDO().GetZDOID("spawner_id");
            return ZNetScene.instance.FindInstance(zdoid);
        }

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

        Vector3 moveToTarget;
        bool keepMoving = false;

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

        }


        public bool GuardBed()
        {
            var bed = GetBed();

            FollowTarget(bed, GetBedZDO().GetPosition()); //if bed is not within loaded range then teleport there
            if (talk != null)
                talk.Say("Going to my bed and guard that area", "Bed");

            //Update state in ZDO of bed
            if (bed != null)
            {
                bed.GetComponent<BedVillagerProcessor>().UpdateVilState(VillagerState.GuardingBed);
            }
            else
            {
                GetBedZDO().Set("state", (int)VillagerState.GuardingBed);
            }
            return true;


        }



        public bool FollowPlayer(Player p)
        {

            if (ZNetScene.instance.IsAreaReady(transform.position) == false)
            {
                return false;
            }

            FollowTarget(p.gameObject, null);
            if (Global.followers.Contains(this) == false)
            {
                Global.followers.Add(this);
            }
            var bed = GetBed();
            if (bed != null)
            {
                bed.GetComponent<BedVillagerProcessor>().UpdateVilState(VillagerState.GuardingBed);
            }
            else
            {
                GetBedZDO().Set("state", (int)VillagerState.Journeying);
            }
            if (talk != null)
                talk.Say($"Following you {Player.m_localPlayer.GetHoverName()}", "Following");

            return true;
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
                FollowTarget(defense, ZDOMan.instance.GetZDO(defenseID).GetPosition());
                if (talk != null)
                    talk.Say($"Defeinding Post {defenseID.id}", "Defend");

                var bed = GetBed();
                if (bed != null)
                {
                    bed.GetComponent<BedVillagerProcessor>().UpdateVilState(VillagerState.GuardingDefensePost);
                }
                else
                {
                    GetBedZDO().Set("state", (int)VillagerState.GuardingBed);
                }

                return true;
            }


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






