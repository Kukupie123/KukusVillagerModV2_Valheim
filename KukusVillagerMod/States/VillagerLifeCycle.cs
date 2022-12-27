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
        public BedCycle bed;

        GameObject following; //The target it is following

        public int villagerType;
        public int villagerLevel;
        public string uid;
        bool searchedBed = false; //Bed needs to be searched only once so we use this variable to search only once
        private void Awake()
        {
            GetComponentInParent<ZNetView>().SetPersistent(true);
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

            if (!KukusVillagerMod.isMapDataLoaded) return;
            if (!humanoid || !ai) return;


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
                    FindBed(true); //will destroy villager if bed is not found
                    searchedBed = true;
                }
            }

            PlayerFollowingDistanceCheck();
        }

        void LoadUID()
        {
            uid = GetComponentInParent<ZNetView>().GetZDO().GetString(Util.villagerID);

            if (uid == null || uid.Trim().Length == 0)
            {
                string guid = System.Guid.NewGuid().ToString();
                GetComponentInParent<ZNetView>().GetZDO().Set(Util.villagerID, guid);
                uid = GetComponentInParent<ZNetView>().GetZDO().GetString(Util.villagerID);
            }
        }

        //Search for a bed. If failed delete it.
        void FindBed(bool destroyIfNotFound = false)
        {
            foreach (var b in FindObjectsOfType<BedCycle>())
            {
                if (b == null) continue; //Very unlikely but safety first
                string vilIDOfBed = b.GetComponentInParent<ZNetView>().GetZDO().GetString(Util.villagerID);
                if (vilIDOfBed == null || vilIDOfBed.Trim().Length == 0) continue;
                if (vilIDOfBed.Equals(uid))
                {
                    this.bed = b;
                    return;
                }
            }
            if (destroyIfNotFound)
            {
                Destroy(this.gameObject);
            }
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
