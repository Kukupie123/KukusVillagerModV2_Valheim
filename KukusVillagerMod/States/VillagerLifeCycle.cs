
using KukusVillagerMod.enums;
using System.Threading.Tasks;
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

                    switch (this.bed.GetVillagerState())
                    {
                        case VillagerState.GuardingBed:
                            GuardBed();
                            KLog.warning($"Found bed for villager {GetUID()} , Bed : {GetLinkedBedID()}, GUARDING BED NOW");
                            break;
                        case VillagerState.GuardingDefensePost:
                            DefendPost();
                            KLog.warning($"Found bed for villager {GetUID()} , Bed : {GetLinkedBedID()}, DEFENDING POST NOW");
                            break;
                    }
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

            if (bed == null)
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
            if (bed != null)
            {
                bed.UpdateVillagerState(enums.VillagerState.Journeying);
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
                        if (bed != null)
                        {
                            bed.UpdateVillagerState(enums.VillagerState.GuardingDefensePost);
                        }
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






