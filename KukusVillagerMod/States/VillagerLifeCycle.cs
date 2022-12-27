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
        //Components
        MonsterAI ai;
        Humanoid humanoid;
        ZNetView znv;
        public BedCycle bed;

        GameObject following; //The target it is following

        public int villagerType;
        public int villagerLevel;
        public string uid;
        bool searchedBed = false; //Bed needs to be searched only once so we use this variable to search only once
        private void Awake()
        {
            znv = GetComponentInParent<ZNetView>();
            znv.SetPersistent(true);
            /*
             * 1. Load UID of the villager, if not found then create one and save it persistently
             * 2. Load the components
             * 3. Set ZNV as persistent
             */
            LoadUID();

            ai = GetComponent<MonsterAI>();
            humanoid = GetComponent<Humanoid>();
            humanoid.SetLevel(this.villagerLevel);
        }

        private void OnDestroy()
        {
            //Remove the item from global list
            KLog.info($"Destroying Villager {uid} ");
            Global.villagerData.Remove(this);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // ignore collision with player action and bed

            Character character = collision.gameObject.GetComponent<Character>();
            if ((character != null
                && character.m_faction == Character.Faction.Players))
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
                return;
            }
        }


        private void FixedUpdate()
        {
            /*
             * 1. Check if map data has been loaded. If yes then try to load bed, if failed then destroy this villager as a villager MUST have a bed at spawn. Its okay for a villager to not have bed after some time but initially at spawn every villager needs a bed or else they are considered ill villagers and will polute the game
             * 
             */
            znv.SetPersistent(true);
            if (!KukusVillagerMod.isMapDataLoaded) return;
            if (!humanoid || !ai) return;

            if (!Global.villagerData.Contains(this))
                Global.villagerData.Add(this);

            if (bed)
            {

            }
            else
            {
                //A bed is null on only two conditions
                //1. When the villager is loaded for the first time
                //2. When villager goes far away with player and bed depsawns. This is acceptable and we will deal with it easily

                //1. See if we searched bed already. if yes then destroy if bed not found as it is not acceptable
                if (!searchedBed)
                {
                    //Before we search for bed please wait for a short amount of time as the creatures spawned by bed will have no bedID assigned and this will fuck up so we add a little delay for the bed to setup ZDO of the creatures before we start searching
                    searchedBed = true;
                    LittleWaitAndSearch();
                }
            }

            PlayerFollowingDistanceCheck();
        }

        async void LittleWaitAndSearch()
        {
            await Task.Delay(1000);
            FindBed();
            //If no bed found then this villager doesn't belong to be alive
            if (bed == null)
            {
                LittleWaitNDestroy();
            }
        }

        async void LittleWaitNDestroy()
        {
            await Task.Delay(5000);
            Destroy(this.gameObject);
        }

        void LoadUID()
        {
            uid = znv.GetZDO().GetString(Util.villagerID);

            if (uid == null || uid.Trim().Length == 0)
            {
                string guid = System.Guid.NewGuid().ToString();
                znv.GetZDO().Set(Util.villagerID, guid);
                uid = znv.GetZDO().GetString(Util.villagerID);
                KLog.warning($"Created uid of villager  {uid}");
            }
            else
            {
                KLog.info($"Loaded uid of villager  {uid}");
            }
        }

        //Search for a bed. If failed delete it.
        void FindBed()
        {
            int c = 0;
            KLog.warning($"Finding bed for villager : {uid}");
            foreach (var b in FindObjectsOfType<BedCycle>())
            {
                c++;

                if (b == null) continue; //Very unlikely but safety first
                var bznv = b.GetComponentInParent<ZNetView>();
                var zdo = bznv.GetZDO();
                string vilIDOfBed = zdo.GetString(Util.villagerID);
                KLog.warning($"{c} BED FOUND WITH VILLAGER : {vilIDOfBed}");

                if (vilIDOfBed == null || vilIDOfBed.Trim().Length == 0) continue;
                if (vilIDOfBed.Equals(uid))
                {
                    KLog.warning($"FOUND bed for villager : {uid}");
                    znv.GetZDO().Set(Util.bedID, b.uid);
                    this.bed = b;
                    return;
                }
            }
            KLog.warning($"FAILED TO FIND bed for villager : {uid}");
        }

        //If following a player TP to them if distance exceeds certain amount or if they are teleporting
        void PlayerFollowingDistanceCheck()
        {
            if (following)
            {
                if (following.GetComponent<Player>() != null)
                {
                    if (following.GetComponent<Player>().IsTeleporting())
                    {
                        transform.position = following.transform.position;
                        return;
                    }

                    //TP to player if distance is too much
                    if (Vector3.Distance(transform.position, following.transform.position) > 50 || !ai.FindPath(following.transform.position))
                    {
                        transform.position = following.transform.position;
                    }
                }
            }
        }

        private void startFollowing(GameObject following, bool havePathCheck = false)
        {
            if (following == null) return;
            this.following = following;

            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();
            ai.SetFollowTarget(following);
            ai.SetPatrolPoint(following.transform.position);
            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();




            if (havePathCheck)
            {
                if (!ai.HavePath(following.transform.position) || !ai.FindPath(following.transform.position))
                {
                    transform.position = following.transform.position;
                }
            }
            else
            {
                if (!ai.FindPath(following.transform.position))
                    transform.position = following.transform.position;
            }

        }

        public bool GuardBed()
        {
            /*
             * 1. Check if bed is valid, if not try to load it, if not found then return false
             * 2. if bed valid then make it follow it
             * 3. remove from defenders and followers list
             */

            if (!ai) return false;
            //Remove from following list

            if (bed == null)
            {
                FindBed();
            }

            if (bed == null)
            {
                return false;
            }

            startFollowing(bed.gameObject);

            RemoveFromFollower();
            RemoveFromDefensePost();

            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Guarding Bed");

            return true;
        }

        public bool FollowPlayer(Player player)
        {
            /*
             * 1. Check if player is valid, if not simply do nothing
             */
            if (!ai) return false;
            if (player == null) return false;
            //Add villager to following list

            startFollowing(player.gameObject, true);

            RemoveFromFollower();
            RemoveFromDefensePost();

            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Following {player.GetHoverName()}");
            return true;
        }

        public bool DefendPost()
        {
            /*
             * 1. Iterate all defensive posts 
             * 2. Check if it's occupied or not, if not then use it and update value of defensive post
             * 3. Remove from follower
             */
            if (!ai) return false;


            foreach (var d in Global.defences)
            {
                if (d.defenseType == villagerType)
                {
                    //Check if occupied by a villager
                    if (d.villager == null)
                    {
                        //Not occupied

                        //Set villagerState as this to signify that it's occupied
                        d.villager = this;

                        RemoveFromFollower();

                        startFollowing(d.gameObject, true);
                        return true;

                    }
                    else continue;
                }
            }
            return false;
        }

        private void RemoveFromDefensePost()
        {
            foreach (var d in Global.defences)
            {
                if (d == null) continue;
                if (d.villager == null) continue;
                if (d.villager == this)
                {
                    d.villager = null;
                    return;
                }
            }
        }
        private void RemoveFromFollower()
        {
            Global.followingVillagers.Remove(this);
        }


    }
}
