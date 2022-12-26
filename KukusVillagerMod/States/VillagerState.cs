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
        private void Awake()
        {
            GetComponentInParent<ZNetView>().SetPersistent(true);

            ai = GetComponent<MonsterAI>();
            humanoid = GetComponent<Humanoid>();

            //Try to laod the uid
            LoadUID();

        }

        private void FixedUpdate()
        {
            //Since it's set no duplicates will be present
            Global.villagerStates.Add(this);

        }

        private void OnDestroy()
        {
            Global.villagerStates.Remove(this);
        }

        //Save the bed to zdo of the villager. To be used by the bed after spawning this villager
        public void SetBed(BedState bed)
        {
            GetComponentInParent<ZNetView>().GetZDO().Set(Util.bedID, bed.uid);
            this.bedState = bed;
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
        private void FindBed()
        {
            var beds = FindObjectsOfType<BedState>();

            if (beds == null || uid.Trim().Length == 0)
            {
                //no beds found at all in world
                return;
            }

            foreach (var b in beds)
            {
                string vilID = b.GetComponentInParent<ZNetView>().GetZDO().GetString(Util.villagerID);
                if (vilID == null || vilID.Trim().Length == 0)
                {
                    continue;
                }
                if (vilID.Equals(uid))
                {
                    if (b.villagerState != null)
                    {
                        KLog.warning("!!!!!!!!! DUPLICATE VILLAGER");
                        continue;
                    }

                    bedState = b;
                    b.villagerState = this;
                    return;
                }
            }
        }

        //Start following the object.
        private void startFollowing(GameObject following)
        {
            if (!ai.FindPath(following.transform.position) || !ai.HavePath(following.transform.position)) //if no path available then tp villager
                transform.position = following.transform.position;

            this.following = following;
            ai.ResetPatrolPoint();
            ai.ResetRandomMovement();
            ai.SetFollowTarget(following);
            ai.SetPatrolPoint(following.transform.position);
        }


        public void GuardBed()
        {
            if (!ai) return;
            //Remove from following list
            Global.followingVillagers.Remove(this);

            //if bed not valid then try to find bed and destroy if still not found
            if (bedState == null)
            {
                FindBed();
                if (bedState == null)
                {
                    DestroyImmediate(this);
                }
            }
            startFollowing(bedState.gameObject);
            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Guarding Bed");
        }

        public void FollowPlayer(Player player)
        {
            if (!ai) return;
            if (player == null) return;
            //Add villager to following list
            Global.followingVillagers.Add(this);

            startFollowing(player.gameObject);
            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Following {player.GetHoverName()}");
        }

        public void DefendPost()
        {
            if (!ai) return;
            if (villagerType == -1) return;

            Global.followingVillagers.Remove(this);

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
                        this.following = d.gameObject;
                        startFollowing(d.gameObject);
                        return;

                    }
                    else continue;
                }
            }
        }
    }
}
