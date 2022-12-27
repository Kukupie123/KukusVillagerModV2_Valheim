using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace KukusVillagerMod.States
{
    class VillagerState : MonoBehaviour
    {
        public string uid = null;

        public BedState bedState;

        private MonsterAI ai;
        private Humanoid humanoid;

        public GameObject following;
        public int villagerType = -1; // 1 = melee, 2 = ranged
        public int villagerLevel;

        private void Awake()
        {
            try
            {
                UnityEngine.GameObject.DestroyImmediate(GetComponent<CharacterDrop>()); //Destroy player controller
                UnityEngine.GameObject.DestroyImmediate(GetComponent<PlayerController>()); //Destroy player controller
                UnityEngine.GameObject.DestroyImmediate(GetComponent<Talker>()); //destroy talking comp
                UnityEngine.GameObject.DestroyImmediate(GetComponent<Skills>()); //Disable skils
                UnityEngine.GameObject.DestroyImmediate(GetComponent<Player>()); //Disable skils
            }
            catch (UnityException e)
            {

            }
            //Destroy comps


            GetComponentInParent<ZNetView>().SetPersistent(true);

            ai = GetComponent<MonsterAI>();

            humanoid = GetComponent<Humanoid>();
            humanoid.SetLevel(villagerLevel);

            //Try to laod the uid
            LoadUID();

            //if this was spawned without a bed it will get destroyed
            //onSpawn();

        }

        private void FixedUpdate()
        {
            //Since it's set no duplicates will be present
            Global.villagerStates.Add(this);


            //If following player tp to him when stuck
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
                    if (Vector3.Distance(transform.position, following.transform.position) > 50 || !ai.HavePath(following.transform.position))
                    {
                        transform.position = following.transform.position;
                    }
                }
            }

        }

        private void OnDestroy()
        {
            Global.villagerStates.Remove(this);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // ignore collision with player action and bed

            Character character = collision.gameObject.GetComponent<Character>();
            if ((character != null
                && character.m_faction == Character.Faction.Players) || collision.gameObject.GetComponent<BedState>() != null
               )
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
                return;
            }
        }

        //Save the bed to zdo of the villager. To be used by the bed after spawning this villager
        public void SetBed(BedState bed)
        {
            GetComponentInParent<ZNetView>().GetZDO().Set(Util.bedID, bed.uid);
            this.bedState = bed;
        }

        //When you spawn you are assigned a bed. if you dont have a bed it generally means you are a leftover. A situation would be where you died with your villagers very far away from their bed. And when you go back to their location after dying. They are reinitiated with no bed. They need to be destroyed
        async void onSpawn()
        {
            await Task.Delay(500);
            if (bedState == null)
            {
                DestroyImmediate(this.gameObject);
                return;
            }
            KLog.info("BED FOUND ONSPAWN()");
        }

        private void LoadUID()
        {
            uid = GetComponentInParent<ZNetView>().GetZDO().GetString(Util.villagerID);

            //Failed to load. Create a new uid
            if (uid == null || uid.Trim().Length == 0)
            {
                string guid = System.Guid.NewGuid().ToString();
                GetComponentInParent<ZNetView>().GetZDO().Set(Util.villagerID, guid);
                uid = GetComponentInParent<ZNetView>().GetZDO().GetString(Util.villagerID);
                KLog.warning($"Failed to load ID for villager, Saved new {uid}");
            }
            else
            {
                KLog.warning($"Loadedvillager, ID : {uid}");

            }
        }

        //Find bed which has key {villagerID : uid}


        //Start following the object.
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

        private void FindBed()
        {
            var beds = FindObjectsOfType<BedState>();
            if (beds == null) return;

            foreach (var b in beds)
            {
                if (b == null) continue;
                var znv = b.GetComponentInParent<ZNetView>();
                if (znv == null) continue;
                var zdo = znv.GetZDO();
                if (zdo == null) continue;
                string vilID = zdo.GetString(Util.villagerID);

                if (vilID == null || vilID.Trim().Length == 0) continue;
                if (vilID.Equals(uid))
                {
                    bedState = b;
                    return;
                }
            }
        }




        public bool GuardBed()
        {
            if (!ai) return false;
            //Remove from following list

            //Find bed
            FindBed();

            //if bed not valid then try to find bed and destroy if still not found
            if (bedState == null)
            {
                //DestroyImmediate(this.gameObject);
                return false;
            }
            removeFromFollower();
            removeFromDefensePost();

            startFollowing(bedState.gameObject);
            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Guarding Bed");
            return true;
        }

        //When villager follows someone we need to set bed to null. So that if villager dies when bed isnt' in memory and player is near bed a new villager is spawned, and when player goes  near this villager we will see that bed is null and delete the villager
        public bool FollowPlayer(Player player)
        {
            if (!ai) return false;
            if (player == null) return false;
            //Add villager to following list
            removeFromFollower();
            removeFromDefensePost();
            bedState = null;
            startFollowing(player.gameObject, true);
            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Following {player.GetHoverName()}");
            return true;
        }

        public bool DefendPost()
        {
            if (!ai) return false;
            if (villagerType == -1) return false;


            foreach (var d in Global.defences)
            {
                if (d.defenseType == villagerType)
                {
                    //Check if occupied by a villager
                    if (d.villagerState == null)
                    {
                        //Not occupied

                        //Set villagerState as this to signify that it's occupied
                        d.villagerState = this;
                        removeFromFollower();
                        this.following = d.gameObject;
                        startFollowing(d.gameObject, true);
                        return true;

                    }
                    else continue;
                }
            }
            return false;
        }

        private void removeFromDefensePost()
        {
            foreach (var d in Global.defences)
            {
                if (d == null) continue;
                if (d.villagerState == null) continue;
                if (d.villagerState == this)
                {
                    d.villagerState = null;
                    return;
                }
            }
        }
        private void removeFromFollower()
        {
            Global.followingVillagers.Remove(this);
        }


    }
}
