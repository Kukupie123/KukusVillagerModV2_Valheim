
using KukusVillagerMod.enums;
using System;
using System.Collections.Generic;
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
            if (ZNV == null || ZNV.IsValid() == false)
            {
                ZNV = GetComponentInParent<ZNetView>();
                ZNV.SetPersistent(true); //ZNV has to be persistent
                return;
            }
            if (humanoid == null)
            {
                humanoid = GetComponent<Humanoid>();
                humanoid.SetLevel(villagerLevel);
                return;
            }
            if (ai == null)
            {
                ai = GetComponent<MonsterAI>();
                return;
            }


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
                    startingTimeForBedNotFound = null;
                    ZDO zdo = base.GetComponent<ZNetView>().GetZDO();
                    KLog.warning("The villager has not found a bed. Destroying");
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


        //Villager state related functions--------------------------------------------------
        public VillagerState GetVillagerState()
        {
            return (VillagerState)GetBedZDO().GetInt("state", (int)VillagerState.Guarding_Bed);
        }

        public void SetVillagerState(VillagerState newState)
        {
            GetBedZDO().Set("state", (int)newState);
        }


        //Bed related functions-------------------------------------------------------------

        //Returns true if a bed was assigned to this villager after it spawned
        public bool isBedAssigned()
        {
            try
            {
                ZDOID zdoid = this.ZNV.GetZDO().GetZDOID("spawner_id");

                //if zdoid is not null and it exists then we can say that the bed has been assigned for this villager after it spawned
                if (!zdoid.IsNone() && ZDOMan.instance.GetZDO(zdoid) != null)
                {
                    return true;
                }

                //Check if the bed ZDO also has spawn_id of this zdo
                if (ZDOMan.instance.GetZDO(zdoid).GetZDOID("spawn_id") != null && ZDOMan.instance.GetZDO(zdoid).GetZDOID("spawn_id").IsNone() == false && ZDOMan.instance.GetZDO(zdoid).GetZDOID("spawn_id").id == this.ZNV.GetZDO().m_uid.id)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

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

        //Defense post related functions-------------------------------------------------

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


        //Work post related functions----------------------------------------------
        public ZDOID GetWorkPostID()
        {
            return GetBedZDO().GetZDOID("work");
        }


        public bool isWorkPostAssigned()
        {
            return !GetWorkPostID().IsNone();
        }

        public ZDO GetWorkZDO()
        {
            return ZDOMan.instance.GetZDO(GetWorkPostID());
        }

        public GameObject GetWorkInstance()
        {
            return ZNetScene.instance.FindInstance(GetWorkPostID());
        }

        //Work skill related function
        public bool GetWorkSkill_CanPickUp()
        {
            return GetBedZDO().GetBool("CanPickup", false);
        }

        public void SetWorkSkill_Pickup(bool canPickup)
        {
            GetBedZDO().Set("CanPickup", canPickup);
        }

        public bool GetWorkSkill_CanSmelt()
        {
            return GetBedZDO().GetBool("CanSmelt", false);
        }

        public void SetWorkSkill_Smelt(bool canSmelt)
        {
            GetBedZDO().Set("CanSmelt", canSmelt);
        }


        //Container related functions----------------------------------------------------
        public ZDOID GetContainerID()
        {
            return GetBedZDO().GetZDOID("container");
        }

        public bool IsContainerAssigned()
        {
            return !GetContainerID().IsNone();
        }

        public ZDO GetContainerZDO()
        {
            return ZDOMan.instance.GetZDO(GetContainerID());
        }

        public GameObject GetContainerInstance()
        {
            return ZNetScene.instance.FindInstance(GetContainerID());
        }

        public Inventory GetContainerInventory()
        {
            //Container stores inventory items in "items" key
            ZDO containerZDO = GetContainerZDO();
            string m_name = containerZDO.GetString("m_name", "");
            int w = containerZDO.GetInt("width", 0);
            int h = containerZDO.GetInt("height", 0);
            string items = GetContainerZDO().GetString("items", ""); //Get items from container ZDO
            var pkg = new ZPackage(items); //Load items into zpackage
            var dummySprite = Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero);
            Inventory inv = new Inventory(m_name, dummySprite, w, h); //create new inventory
            if (!string.IsNullOrEmpty(items))
                inv.Load(pkg);
            return inv;



        }

        public void SaveContainerInventory(Inventory inv)
        {
            ZPackage pkg = new ZPackage();
            inv.Save(pkg);
            string encodedItem = pkg.GetBase64();
            GetContainerZDO().Set("items", encodedItem);
        }


        //FUTURE WIP
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






