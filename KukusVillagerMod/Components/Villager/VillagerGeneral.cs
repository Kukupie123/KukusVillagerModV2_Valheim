
using KukusVillagerMod.enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KukusVillagerMod.Components.Villager
{
    /*
     * Stats of villager. Stored in ZDO
     * 1. Name
     * 2. Health (Upgradable, upgrades as they fight)
     * 3. Special Damage (None, Fire, Frost, Poison)
     * 4. Efficiency% (For upgrading)
     * 5. Random base damage stats
     * 6. Mining Level : (Simple, Bronze, Iron, Silver, BM)
     * 
     * Upgrades:
     * Health and damage can be upgraded : (current * efficiency rate ) : Eg 36 + 36 * 0.2. You will be using items to upgrade them. Different trophy created item will provide different multiplier for efficiency.
     * Passive slow upgrade for working and fighting
     * Chop & pickaxe will be capped and will be uncapped by feeding them bronze to unlock bronze chop&pickaxe and so on
     */
    class VillagerGeneral : MonoBehaviour
    {
        public static ZDOID SELECTED_VILLAGER_ID = ZDOID.None;

        public static bool TameVillager(ZDOID villagerZDOID)
        {
            if (!Util.ValidateZDOID(villagerZDOID)) return false;
            ZDO villagerZDO = ZDOMan.instance.GetZDO(villagerZDOID);
            if (!Util.ValidateZDO(villagerZDO)) return false;
            //Get ZNV of the villager
            ZNetView villagerZNV = ZNetScene.instance.FindInstance(villagerZDO);
            if (!Util.ValidateZNetView(villagerZNV)) return false;
            villagerZNV.SetPersistent(true);
            villagerZDO.Set("tamed", true);
            return true;
        }
        public bool TameVillager()
        {
            tameable.Tame();
            return TameVillager(ZNV.GetZDO().m_uid);
        }
        public static bool IsVillagerTamed(ZDOID villagerZDOID)
        {
            if (!Util.ValidateZDOID(villagerZDOID)) return false;
            ZDO villagerZDO = ZDOMan.instance.GetZDO(villagerZDOID);
            if (!Util.ValidateZDO(villagerZDO)) return false;
            return villagerZDO.GetBool("tamed", false);
        }
        public bool IsVillagerTamed()
        {
            return IsVillagerTamed(ZNV.GetZDO().m_uid);
        }
        public static void SetRandomStats(ZNetView ZNV)
        {

            //Basics
            var n = Util.RandomName();
            ZNV.GetZDO().Set("name", n);
            ZNV.GetZDO().Set("health", UnityEngine.Random.Range(50f, 150f));
            ZNV.GetZDO().Set("efficiency", UnityEngine.Random.Range(0.1f, 1.0f)); //Percentage stuff

            KLog.warning($"Villager {ZNV.GetZDO().m_uid.id} has name {n} ");

            //Farming stuff
            ZNV.GetZDO().Set("pickaxe", 15.0f);
            ZNV.GetZDO().Set("chop", 20.0f);
            ZNV.GetZDO().Set("mining", 0);
            //15 should be base pickaxe, 20 chop

            //Damage
            /*
        *  weapon.m_shared.m_damages = new HitData.DamageTypes();
               weapon.m_shared.m_damages.m_damage = 0f;
               weapon.m_shared.m_damages.m_slash = 0f;
               weapon.m_shared.m_damages.m_blunt = 0f;
               weapon.m_shared.m_damages.m_fire = 0f;
               weapon.m_shared.m_damages.m_frost = 0f;
               weapon.m_shared.m_damages.m_lightning = 0f;
               weapon.m_shared.m_damages.m_pierce = 0f;
               weapon.m_shared.m_damages.m_poison = 0f;
               weapon.m_shared.m_damages.m_spirit = 0f;
        */
            ZNV.GetZDO().Set("damage", UnityEngine.Random.Range(0f, 20f));
            ZNV.GetZDO().Set("slash", UnityEngine.Random.Range(0f, 20f));
            ZNV.GetZDO().Set("blunt", UnityEngine.Random.Range(0f, 20f));
            ZNV.GetZDO().Set("fire", 0f); //special
            ZNV.GetZDO().Set("frost", 0f); //special
            ZNV.GetZDO().Set("lightning", 0f); //special
            ZNV.GetZDO().Set("pierce", UnityEngine.Random.Range(0f, 20f));
            ZNV.GetZDO().Set("poison", 0f); //special
            ZNV.GetZDO().Set("spirit", 0f); //special

            if (UnityEngine.Random.Range(0, 3) == 2)
            {

                ZNV.GetZDO().Set("special", true);
                //Speciality
                switch (UnityEngine.Random.Range(0, 4))
                {
                    case 0:
                        ZNV.GetZDO().Set("fire", UnityEngine.Random.Range(0f, 10f));
                        break;
                    case 1:
                        ZNV.GetZDO().Set("frost", UnityEngine.Random.Range(0f, 10f));
                        break;
                    case 2:
                        ZNV.GetZDO().Set("lightning", UnityEngine.Random.Range(0f, 10f));
                        break;
                    case 3:
                        ZNV.GetZDO().Set("poison", UnityEngine.Random.Range(0f, 10f));
                        break;
                    case 4:
                        ZNV.GetZDO().Set("spirit", UnityEngine.Random.Range(0f, 10f));
                        break;
                }
            }



        }
        private void SetRandomStats()
        {
            if (humanoid.IsTamed()) KLog.warning("Villager is tamed, aborting setting up random values");
            else
            {
                SetRandomStats(this.ZNV);

            }
        }

        private void LoadStatsFromZDO()
        {
            if (IsVillagerTamed())
                humanoid.m_name = "Villager " + GetName();
            else humanoid.m_name = "wanderer " + GetName();
            //Set up health
            humanoid.SetMaxHealth(GetHealth());
            //If not recruited then set current hp to max
            if (!IsVillagerTamed())
            {
                humanoid.SetHealth(GetHealth());
            }
        }

        public static float GetDamage(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetFloat("damage", 0f);
        }
        public float GetDamage()
        {
            return GetDamage(this.ZNV.GetZDO().m_uid);
        }
        public static float GetSlash(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetFloat("slash", 0f);
        }
        public float GetSlash()
        {
            return GetSlash(this.ZNV.GetZDO().m_uid);
        }
        public static float GetBlunt(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetFloat("blunt", 0f);
        }
        public float GetBlunt()
        {
            return GetBlunt(this.ZNV.GetZDO().m_uid);
        }
        public static float GetFire(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetFloat("fire", 0f);
        }
        public float GetFire()
        {
            return GetFire(this.ZNV.GetZDO().m_uid);
        }
        public static float GetFrost(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetFloat("frost", 0f);
        }
        public float GetFrost()
        {
            return GetFrost(this.ZNV.GetZDO().m_uid);
        }
        public static float Getlightning(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetFloat("lightning", 0f);
        }
        public float Getlightning()
        {
            return Getlightning(this.ZNV.GetZDO().m_uid);
        }
        public static float GetPierce(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetFloat("pierce", 0f);
        }
        public float GetPierce()
        {
            return GetPierce(this.ZNV.GetZDO().m_uid);
        }
        public static float GetPoison(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetFloat("GetPoison", 0f);
        }
        public float GetPoison()
        {
            return GetPoison(this.ZNV.GetZDO().m_uid);
        }
        public static float GetSpirit(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetFloat("spirit", 0f);
        }
        public float GetSpirit()
        {
            return GetSpirit(this.ZNV.GetZDO().m_uid);
        }
        public static float GetChop(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetFloat("chop", 0f);
        }
        public float GetChop()
        {
            return GetChop(this.ZNV.GetZDO().m_uid);
        }
        public static float GetPickaxe(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetFloat("pickaxe", 0f);
        }
        public float GetPickaxe()
        {
            return GetPickaxe(this.ZNV.GetZDO().m_uid);
        }
        public static string GetName(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return null;
            return zdo.GetString("name");
        }
        public string GetName()
        {
            return GetName(this.ZNV.GetZDO().m_uid);
        }
        public static float GetHealth(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetFloat("health");
        }
        public float GetHealth(bool fromAI = false)
        {
            if (fromAI) return humanoid.GetHealth();
            return GetHealth(this.ZNV.GetZDO().m_uid);
        }
        public static int GetMiningLevel(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetInt("mining");
        }
        public int GetMiningLevel()
        {
            return GetMiningLevel(this.ZNV.GetZDO().m_uid);
        }

        public static float GetEfficiency(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetFloat("efficiency");
        }
        public float GetEfficiency()
        {
            return GetEfficiency(this.ZNV.GetZDO().m_uid);
        }

        public static Tuple<HitData.DamageType, float> GetSpecialSkill(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return null;

            var dmg = 0f;
            dmg = GetFrost(villagerZDOID);
            if (dmg != 0)
            {
                return new Tuple<HitData.DamageType, float>(HitData.DamageType.Frost, dmg);
            }
            dmg = Getlightning(villagerZDOID);
            if (dmg != 0)
            {
                return new Tuple<HitData.DamageType, float>(HitData.DamageType.Lightning, dmg);
            }
            dmg = GetPoison(villagerZDOID);
            if (dmg != 0)
            {
                return new Tuple<HitData.DamageType, float>(HitData.DamageType.Poison, dmg);
            }
            dmg = GetSpirit(villagerZDOID);
            if (dmg != 0)
            {
                return new Tuple<HitData.DamageType, float>(HitData.DamageType.Spirit, dmg);
            }
            return null;
            //frost, lightning,poison,spirit
        }
        public Tuple<HitData.DamageType, float> GetSpecialSkill()
        {
            return GetSpecialSkill(ZNV.GetZDO().m_uid);
        }


        //Object methods and members-------------------------------

        public ZNetView ZNV;
        private MonsterAI ai;
        private Humanoid humanoid;
        private Tameable tameable;

        private void Awake()
        {
            //Get necessary components
            ZNV = GetComponent<ZNetView>();
            ai = GetComponent<MonsterAI>();
            humanoid = GetComponent<Humanoid>();
            tameable = GetComponent<Tameable>();
            ai.m_attackPlayerObjects = false;

            //Generate and load stats
            if (!IsVillagerTamed())
            {
                SetRandomStats();
            }
            LoadStatsFromZDO();

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


        //Stats section--------------


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






