using KukusVillagerMod.Datas;
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

        private MonsterAI ai;
        private Humanoid humanoid;

        public VillagerData villagerData;

        public GameObject following;


        private void Awake()
        {
            //Make ZDO Persist
            GetComponentInParent<ZNetView>().SetPersistent(true);

            //Get villager data
            villagerData = GetComponentInParent<VillagerData>();

            if (villagerData == null)
            {
                KLog.warning("VILLAGER DATA IS NOT VALID!!!!!!");
                DestroyImmediate(this.gameObject);
            }
            if (villagerData.GetBed() == null)
            {
                //When spawned and no bed found we need to remove it.
                KLog.warning("Bedless Villager spawned in game world");
                DestroyImmediate(this.gameObject);
            }
            else
            {
                KLog.warning("Bedfull Villager spawned in game world");

            }

            ai = GetComponent<MonsterAI>();

            humanoid = GetComponent<Humanoid>();

            humanoid.SetLevel(villagerData.villagerLevel);



        }

        private void FixedUpdate()
        {

            try
            {
                if (Global.villagerData.Contains(villagerData) == false)
                    Global.villagerData.Add(villagerData);


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
                        if (Vector3.Distance(transform.position, following.transform.position) > 50 || !ai.FindPath(following.transform.position))
                        {
                            transform.position = following.transform.position;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }



        }

        private void OnDestroy()
        {
            KLog.warning($"Destroying Villager with id {villagerData.uid}");
            Global.villagerData.Remove(villagerData);
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
                if (vilID.Equals(villagerData.uid))
                {
                    villagerData.SetBed(b);
                    return;
                }
            }
        }




        public bool GuardBed()
        {
            if (!ai) return false;
            //Remove from following list

            //Find bed in world.
            FindBed();

            //if bed not valid then try to find bed and destroy if still not found
            if (villagerData.GetBed() == null)
            {
                //NEEDS TO BE DESTROYED
                return false;
            }

            removeFromFollower();
            removeFromDefensePost();

            startFollowing(villagerData.GetBed().gameObject);
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
            startFollowing(player.gameObject, true);
            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Following {player.GetHoverName()}");
            return true;
        }

        public bool DefendPost()
        {
            if (!ai) return false;
            if (villagerData.villagerType == -1) return false;


            foreach (var d in Global.defences)
            {
                if (d.defenseType == villagerData.villagerType)
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
            Global.followingVillagers.Remove(villagerData);
        }


    }
}
