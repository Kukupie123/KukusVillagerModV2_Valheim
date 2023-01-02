
using UnityEngine;

namespace KukusVillagerMod.States
{
    class VillagerLifeCycle : MonoBehaviour
    {
        public ZNetView znv;

        public BedCycle bed;

        public int villagerLevel;
        public int villagerType;
        public int health;

        bool fixedUpdateRanOnce = false;

        public GameObject followingTarget;

        MonsterAI ai;
        Humanoid humanoid;


        private void Awake()
        {
            znv = GetComponentInParent<ZNetView>();
            znv.SetPersistent(true);
            ai = GetComponent<MonsterAI>();
            humanoid = GetComponent<Humanoid>();
            humanoid.SetLevel(villagerLevel);
            humanoid.SetMaxHealth(health);
            humanoid.SetHealth(health);
            loadOrCreateUID();
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

            if (GetIsSet() == false)
            {
                KLog.warning($"Villager {GetUID()} has no bed YET!");
                return; //After spawning we have to wait for the bed to be linked with this villager. Once done this will be true persistently.
            }

            if (fixedUpdateRanOnce == false)
            {
                if (bed == null)
                {
                    fixedUpdateRanOnce = true;
                    FindBed();
                    if (bed == null)
                    {
                        KLog.info($"destroying villager");
                        ZNetScene.instance.Destroy(this.gameObject);
                    }
                }
                else
                {
                    //This should never get executed. At first loop the bed should always be null
                    fixedUpdateRanOnce = true;
                    //ZNetScene.instance.Destroy(this.gameObject);
                }
            }
            else
            {
                if (followingTarget != null && followingTarget.GetComponent<Player>() != null)
                {
                    //TP if distance is greater or player is teleporting
                    if (Vector3.Distance(followingTarget.transform.position, transform.position) > 70 || followingTarget.GetComponent<Player>().IsTeleporting())
                    {
                        transform.position = followingTarget.transform.position;
                    }
                }
            }
        }

        void loadOrCreateUID()
        {
            var uid = znv.GetZDO().GetString(Util.uid, null);
            if (uid == null)
            {
                //Create new UID
                uid = System.Guid.NewGuid().ToString();

                znv.GetZDO().Set(Util.uid, uid);
                KLog.warning($"Created UID for Villager {GetUID()}");
            }
            {
                KLog.warning($"Found UID for Villager {uid}");
            }

        }

        bool GetIsSet()
        {
            if (znv == null) return false;
            return znv.GetZDO().GetBool("set", false);
        }

        void markAsActivePersistent()
        {
            znv.GetZDO().Set("set", true);
        }

        public void SaveBed(BedCycle bed)
        {
            znv.GetZDO().Set(Util.bedID, bed.GetUID());
            KLog.warning($"Villager {GetUID()}  saved beds UID {GetLinkedBedID()}");
            markAsActivePersistent();
        }

        public string GetUID()
        {
            if (znv == null) return null;
            return znv.GetZDO().GetString(Util.uid, null);
        }

        public string GetLinkedBedID()
        {
            if (znv == null) return null;
            return znv.GetZDO().GetString(Util.bedID, null);
        }


        private void FindBed()
        {
            string bedID = znv.GetZDO().GetString(Util.bedID, "");
            var beds = FindObjectsOfType<BedCycle>();
            KLog.warning($"Seaching bed for villager {GetUID()}");
            foreach (var bed in beds)
            {
                if (bed == null || bed.znv == null || bed.GetUID() == null || ZNetScene.instance.IsAreaReady(transform.position) == false || ZNetScene.instance.IsAreaReady(bed.transform.position) == false) continue;
                if (bed.GetUID().Equals(GetLinkedBedID()) && GetUID().Equals(bed.GetLinkedVillagerID()))
                {
                    this.bed = bed;
                    SaveBed(bed);
                    KLog.warning($"Found bed for villager {GetUID()} , Bed : {GetLinkedBedID()}");

                    return;
                }
            }
            KLog.warning($"Seaching bed FAILED for villager {GetUID()}");

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

        public void GoToPosition(Vector3 target)
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
            ai.MoveTo(1f, target, 1f, true);

            if (ai.FindPath(target) == false || ai.HavePath(target) == false)
            {
                transform.position = target;
            }

        }

        public void GuardBed()
        {
            if (ZNetScene.instance.IsAreaReady(transform.position) == false)
            {
                return;
            }

            if (bed == null)
            {
                FindBed();
                if (bed == null || ZNetScene.instance.IsAreaReady(bed.transform.position) == false) ZNetScene.instance.Destroy(this.gameObject);
            }
            else if (ZNetScene.instance.IsAreaReady(bed.transform.position) == false) return;
            RemoveVillagerFromFollower();
            RemoveVillagerFromDefending();
            FollowTarget(bed.gameObject);

        }

        public void FollowPlayer(Player p)
        {
            if (ZNetScene.instance.IsAreaReady(transform.position) == false)
            {
                return;
            }

            RemoveVillagerFromDefending();
            FollowTarget(p.gameObject);
            if (Global.followers.Contains(this) == false)
            {
                Global.followers.Add(this);
            }
        }


        public void DefendPost()
        {


            foreach (var d in FindObjectsOfType<DefensePostState>())
            {
                if (ZNetScene.instance.IsAreaReady(d.transform.position) == false) continue;
                if (d.defenseType == villagerType)
                {
                    if (d.villager == null)
                    {
                        d.villager = this;
                        FollowTarget(d.gameObject);
                        RemoveVillagerFromFollower();

                        return;
                    }
                }
            }
        }

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






