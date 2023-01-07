
using KukusVillagerMod.enums;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace KukusVillagerMod.Components.Villager
{
    class VillagerGeneral : MonoBehaviour
    {
        public ZNetView ZNV; //ZNetView of the Villager Creature


        public int villagerLevel; //The level of the villager. Has to be set during creature prefab setup
        public int villagerType; //The type of villager. Servers no purpose anymore, will remove soon
        public int health; //The health of the villager. Has to be set during the creature prefab setup.



        private MonsterAI ai;
        private Humanoid humanoid;

        private void Awake()
        {
            ZNV = GetComponentInParent<ZNetView>();
            ZNV.SetPersistent(true); //ZNV has to be persistent
            ai = GetComponent<MonsterAI>();

            //Setting the values set during prefab setup 
            humanoid = GetComponent<Humanoid>();
            humanoid.SetLevel(villagerLevel);
            humanoid.SetMaxHealth(health);
            humanoid.SetHealth(health);

        }


        //Ignore collision with player
        private void OnCollisionEnter(Collision collision)
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (character != null
                && character.m_faction == Character.Faction.Players
                && character.GetComponent<VillagerGeneral>() == null) // allow collision between minions
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
                return;
            }


        }

        DateTime? startingTimeForBedNotFound;
        private void FixedUpdate()
        {
            if (!KukusVillagerMod.isMapDataLoaded) return;
            //Wait for the bed's ID which spawned this villagers to be saved in the zdo of this villager. The threshold is 10 sec. If we fail to find bed in 10 sec then we are going to assume that this villager was spawned without a bed and needs to be destroyed
            if (!isBedAssigned())
            {
                //Set starting time. Will execute only once
                if (startingTimeForBedNotFound == null)
                {
                    KLog.warning("SET STARTING TIME FOR BED NOT ASSIGNED");
                    startingTimeForBedNotFound = ZNet.instance.GetTime();
                }

                DateTime currentTime = DateTime.Now;

                TimeSpan timeElasped = currentTime - startingTimeForBedNotFound.Value;

                if (timeElasped.TotalSeconds > 10)
                {
                    //if we crossed 10 sec of waiting we are destroying thezdo
                    ZDO zdo = base.GetComponent<ZNetView>().GetZDO();
                    KLog.warning("10 sec passed since the villager has not found a bed. Destroying");
                    ZDOMan.instance.DestroyZDO(zdo);
                }
                return;
            }

            //Check if the bed assigned is valid, if not valid destroy
            if (!GetBedZDO().IsValid())
            {
                ZDOMan.instance.DestroyZDO(ZNV.GetZDO());
            }

        }



        public VillagerState GetVillagerState()
        {
            return (VillagerState)GetBedZDO().GetInt("state", (int)VillagerState.Guarding_Bed);
        }

        public void SetVillagerState(VillagerState newState)
        {
            GetBedZDO().Set("state", (int)newState);
        }


        //Returns true if a bed was assigned to this villager after it spawned
        public bool isBedAssigned()
        {

            ZDOID zdoid = this.ZNV.GetZDO().GetZDOID("spawner_id");

            //if zdoid is not null and it exists then we can say that the bed has been assigned for this villager after it spawned
            if (!zdoid.IsNone() && ZDOMan.instance.GetZDO(zdoid) != null)
            {
                return true;
            }
            return false;
        }

        public ZDOID GetBedZDOID()
        {
            return this.ZNV.GetZDO().GetZDOID("spawner_id");
        }

        //Returns GO based on the ZDOID of the bed saved in the ZDO of this creature, will return null if not loaded in memory
        public GameObject GetBedInstance()
        {
            ZDOID zdoid = GetBedZDOID();
            return ZNetScene.instance.FindInstance(zdoid);
        }

        //Returns the ZDO of the bed that spawned this creature
        public ZDO GetBedZDO()
        {
            ZDOID id = GetBedZDOID();
            return ZDOMan.instance.GetZDO(id);
        }

        public ZDOID GetDefensePostID()
        {
            return GetBedZDO().GetZDOID("defense");
        }

        public bool isDefensePostAssigned()
        {
            ZDOID defensePostID = GetDefensePostID();
            return !defensePostID.IsNone();
        }

        public GameObject GetDefensePostInstance()
        {
            ZDOID defensePostID = GetDefensePostID();
            return ZNetScene.instance.FindInstance(defensePostID);
        }

        public ZDO GetDefenseZDO()
        {
            ZDOID defensePostID = GetDefensePostID();
            return ZDOMan.instance.GetZDO(defensePostID);
        }

        //FUTURE
        public void CutTree()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5000f);

            foreach (Collider c in colliders)
            {
                TreeBase tree = c?.gameObject?.GetComponentInParent<TreeBase>();
                TreeLog log = c?.gameObject?.GetComponentInParent<TreeLog>();
                Destructible destructible = c?.gameObject?.GetComponentInParent<Destructible>();

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






