using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace KukusVillagerMod.States
{
    class VillagerLifeCycle : MonoBehaviour
    {
        /*
         * When a villager is spawned it is going to firstly load it's UID
         * Then it is going to look for a bed, if found we will set the the bed as this villager's bed
         * Or else we will destroy this
         */

        public ZNetView znv;

        public string UID; //VillagerID
        public int villagerType; //Set during prefab creation
        public int villagerLevel; //Set during prefab creation

        MonsterAI ai;
        Humanoid humanoid;

        public BedCycle bed;

        private void Awake()
        {
            znv = GetComponentInParent<ZNetView>();
            znv.SetPersistent(true);

            //Load/Create UID
            LoadUID();
            ai = GetComponent<MonsterAI>();
            humanoid = GetComponent<Humanoid>();

            humanoid.SetLevel(villagerLevel);
        }

        void OnDestroy()
        {
            KLog.warning($"Destroying Villager {UID}");
        }



        //In fixed update we need to look for villager once mapData has been loaded. If it fails to do so we delete

        bool updatedOnce = false;
        public GameObject followingTarget;

        private void FixedUpdate()
        {



            //First If condition for bed check. We also check if the area is readyw
            if (Player.m_localPlayer != null && !Player.m_localPlayer.IsTeleporting() && !ZNetScene.instance.InLoadingScreen() && KukusVillagerMod.isMapDataLoaded && ZNetScene.instance.IsAreaReady(transform.position))
            {
                //Only search for bed if bed is null
                if (!bed)
                {


                    //This block will get executed once per spawn 
                    if (updatedOnce == false && (followingTarget == null || followingTarget.GetComponent<Player>() == null))
                    {
                        updatedOnce = true;
                        FindBed();
                        if (bed == null)
                        {
                            ZNet.instance.m_zdoMan.DestroyZDO(znv.GetZDO());
                            ZNetScene.instance.Destroy(this.gameObject);
                        }
                        else
                        {
                            //They found bed and are in an area
                            ai.SetFollowTarget(this.gameObject);  //Make the Villager stay where they were
                        }
                    }


                }

            }

            //Second if condition for teleporting followers and stuff
            else if (Player.m_localPlayer != null && followingTarget != null && followingTarget.GetComponent<Player>() != null)
            {


                if (Vector3.Distance(followingTarget.transform.position, transform.position) > 50 || followingTarget.GetComponent<Player>().IsTeleporting()) transform.position = followingTarget.transform.position;


            }





        }

        private void LoadUID()
        {
            if (!znv)
            {
                KLog.warning("Villager !!! ZNV Value is null");
                return;
            }

            UID = znv.GetZDO().GetString(Util.villagerID, null);

            //If failed to load UID Create a new one. It fails When new villager is created
            if (UID == null || UID.Trim().Length == 0)
            {
                string uid = System.Guid.NewGuid().ToString();
                znv.GetZDO().Set(Util.villagerID, uid);
                UID = znv.GetZDO().GetString(Util.villagerID);
                KLog.warning($"Villager Created UID ${UID}");
            }
            else
            {
                KLog.warning($"Villager Found UID ${UID}");
            }

        }


        //Recursive function. Uses recursion because when a villager is spawned by the bed, the bed needs time to save villager's ID to its zdo so we keep recursing and recursing and wait for the bed to set values
        private void FindBed(BedCycle[] useThis = null)
        {
            var list = FindObjectsOfType<BedCycle>();

            if (useThis != null)
            {
                list = useThis;
            }

            List<BedCycle> NonReadyBeds = new List<BedCycle>();

            foreach (var b in FindObjectsOfType<BedCycle>())
            {
                if (b == null || ZNetScene.instance.IsAreaReady(b.transform.position) == false) continue;

                //if bed is not ready we are going to add this to nonReadybeds for recursion, znv to determine if they are placed yet or not
                if (b.znv == null || !b.znv.GetZDO().GetBool(Util.villagerSet, false))
                {
                    NonReadyBeds.Add(b);
                    continue;
                }


                //Bed has villagerSet so we can compare villagers IID as well as bedid
                string vilID = b.znv.GetZDO().GetString(Util.villagerID);
                KLog.warning($"SEACHING BED FOR VILLAGER {UID} FOUND : {b.znv.GetZDO().GetString(Util.bedID)} with villager {vilID}");
                if (vilID.Equals(UID))
                {
                    if (b.znv.GetZDO().GetString(Util.villagerID).Equals(UID))
                    {
                        KLog.warning($"Villager {UID} has found BED {b.UID}");
                        znv.GetZDO().Set(Util.bedID, b.UID);
                        this.bed = b;
                        return;
                    }
                    else
                    {
                        KLog.warning($"Villager {UID} has found BED BUT BED HAS DIFFERENT VILLAGER ID :(");

                    }

                }
            }

            //After loop we are going to recursively call 
            if (NonReadyBeds.Count == 0)
            {
                KLog.warning($"Villager {UID} failed to find bed");
            }
            else
            {
                FindBed(NonReadyBeds.ToArray());
            }
        }


        private void FollowTarget(GameObject target)
        {
            try
            {

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
            catch (Exception e)
            {
                ai.ResetPatrolPoint();
                ai.ResetRandomMovement();
                ai.SetFollowTarget(null);
                this.followingTarget = null;
                KLog.warning($"Exception in follow target {e.Message}");
            }

        }

        public void GuardBed()
        {
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
            RemoveVillagerFromDefending();
            FollowTarget(p.gameObject);
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
                        if (ZNetScene.instance.IsAreaReady(d.transform.position) == false) return;
                        RemoveVillagerFromFollower();
                        d.villager = this;
                        FollowTarget(d.gameObject);
                        return;
                    }
                }
            }
        }

        private void RemoveVillagerFromFollower()
        {
            this.followingTarget = null;
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



