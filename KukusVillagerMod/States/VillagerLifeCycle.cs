
using KukusVillagerMod.enums;
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

        bool fixedUpdateRanOnce = false;

        public GameObject followingTarget;

        MonsterAI ai;
        public Humanoid humanoid;


        private void Awake()
        {
            znv = GetComponentInParent<ZNetView>();
            znv.SetPersistent(true);
            ai = GetComponent<MonsterAI>();
            //ai.m_alertRange = 500f; TEST THIS RANGE OUT
            humanoid = GetComponent<Humanoid>();
            humanoid.SetLevel(villagerLevel);
            humanoid.SetMaxHealth(health);
            humanoid.SetHealth(health);
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

            if (!ZNet.instance.IsServer()) return;

            if (Player.m_localPlayer == null) return;


            if (KukusVillagerMod.isMapDataLoaded == false || ZNetScene.instance.IsAreaReady(transform.position) == false)
            {
                return;
            }


            if (fixedUpdateRanOnce == false)
            {


                fixedUpdateRanOnce = true;
            }
            else
            {
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
        }


        //COMMANDS----------------------
        private void FollowTarget(GameObject target)
        {
            if (ZNetScene.instance.IsAreaReady(transform.position) == false || ZNetScene.instance.IsAreaReady(target.transform.position) == false)
            {
                return;
            }

            if (target == null) return;
            this.followingTarget = target;
            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();
            ai.SetFollowTarget(target);

            if (ai.FindPath(target.transform.position) == false || ai.HavePath(target.transform.position) == false)
            {
                transform.position = target.transform.position;
            }

        }

        Vector3 moveToTarget;
        bool keepMoving = false;

        public void MoveTo(Vector3 target)
        {
            if (ZNetScene.instance.IsAreaReady(transform.position) == false || ZNetScene.instance.IsAreaReady(target) == false)
            {
                return;
            }


            this.followingTarget = null;
            ai.SetFollowTarget(null);
            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();
            RemoveVillagerFromDefending();

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
            return false;


            /*if (bed == null)
            {
                FindBed();
                if (bed == null || ZNetScene.instance.IsAreaReady(bed.transform.position) == false) ZNetScene.instance.Destroy(this.gameObject);
            }
            else if (ZNetScene.instance.IsAreaReady(bed.transform.position) == false) return false;
            RemoveVillagerFromFollower();
            RemoveVillagerFromDefending();
            FollowTarget(bed.gameObject);
            if (bed != null)
            {
                bed.UpdateVillagerState(enums.VillagerState.GuardingBed);
            }
            return true;
            */

        }

        public bool FollowPlayer(Player p)
        {

            if (ZNetScene.instance.IsAreaReady(transform.position) == false)
            {
                return false;
            }

            RemoveVillagerFromDefending();
            FollowTarget(p.gameObject);
            if (Global.followers.Contains(this) == false)
            {
                Global.followers.Add(this);
            }
            return true;
        }


        public bool DefendPost()
        {

            foreach (var d in FindObjectsOfType<DefensePostState>())
            {
                if (ZNetScene.instance.IsAreaReady(d.transform.position) == false || ZNetScene.instance.IsAreaReady(transform.position) == false) continue;
                if (d.defenseType == villagerType)
                {
                    if (d.villager == null)
                    {
                        d.villager = this;
                        RemoveVillagerFromFollower();
                        FollowTarget(d.gameObject);
                        return true;
                    }
                }
            }
            return false;
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

        private void RemoveVillagerFromFollower()
        {
            this.followingTarget = null;
            Global.followers.Remove(this);
        }

        private void RemoveVillagerFromDefending()
        {
            foreach (var d in UnityEngine.GameObject.FindObjectsOfType<DefensePostState>())
            {
                if (d.villager == this)
                {
                    d.villager = null;
                }
            }
        }

    }

}






