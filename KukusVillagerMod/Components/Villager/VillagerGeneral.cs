
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
        //Methods
        public static GameObject GetVillagerInstance(ZDOID villagerZDOID)
        {
            return ZNetScene.instance.FindInstance(villagerZDOID);
        }
        public static ZDOID SELECTED_VILLAGER_ID = ZDOID.None;
        //Taming
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

        //Stats
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
        //Bed
        public static ZDOID GetBedZDOID(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return ZDOID.None;
            return zdo.GetZDOID("bed");
        }
        public ZDOID GetBedZDOID()
        {
            return GetBedZDOID(ZNV.GetZDO().m_uid);
        }
        public static ZDO GetBedZDO(ZDOID villagerZDOID)
        {
            return ZDOMan.instance.GetZDO(GetBedZDOID(villagerZDOID));
        }
        public ZDO GetBedZDO()
        {
            return GetBedZDO(ZNV.GetZDO().m_uid);
        }
        public static bool IsBedAssigned(ZDOID villagerZDOID)
        {
            return Util.ValidateZDOID(GetBedZDOID(villagerZDOID)) && Util.ValidateZDO(GetBedZDO(villagerZDOID));
        }
        public bool IsBedAssigned()
        {
            return IsBedAssigned(ZNV.GetZDO().m_uid);
        }
        public static GameObject GetBedInstance(ZDOID villagerZDOID)
        {
            return ZNetScene.instance.FindInstance(GetBedZDOID(villagerZDOID));
        }
        public GameObject GetBedInstance()
        {
            return GetBedInstance(ZNV.GetZDO().m_uid);
        }
        public static void AssignBed(ZDOID villagerZDOID, ZDOID bedZDOID)
        {
            Util.GetZDO(villagerZDOID).Set("bed", bedZDOID);
        }
        public void AssignBed(ZDOID bedZDOID)
        {
            AssignBed(ZNV.GetZDO().m_uid, bedZDOID);
        }
        //Defense post
        public static ZDOID GetDefenseZDOID(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return ZDOID.None;
            return zdo.GetZDOID("defense");
        }
        public ZDOID GetDefenseZDOID()
        {
            return GetDefenseZDOID(ZNV.GetZDO().m_uid);
        }
        public static ZDO GetDefenseZDO(ZDOID villagerZDOID)
        {
            return ZDOMan.instance.GetZDO(GetDefenseZDOID(villagerZDOID));
        }
        public ZDO GetDefenseZDO()
        {
            return GetDefenseZDO(ZNV.GetZDO().m_uid);
        }
        public static bool IsDefenseAssigned(ZDOID villagerZDOID)
        {
            return Util.ValidateZDOID(GetDefenseZDOID(villagerZDOID)) && Util.ValidateZDO(GetDefenseZDO(villagerZDOID));
        }
        public bool IsDefenseAssigned()
        {
            return IsDefenseAssigned(ZNV.GetZDO().m_uid);
        }
        public static GameObject GetDefenseInstance(ZDOID villagerZDOID)
        {
            return ZNetScene.instance.FindInstance(GetDefenseZDOID(villagerZDOID));
        }
        public GameObject GetDefenseInstance()
        {
            return GetDefenseInstance(ZNV.GetZDO().m_uid);
        }
        public static void AssignDefense(ZDOID villagerZDOID, ZDOID defenseZDOID)
        {
            Util.GetZDO(villagerZDOID).Set("defense", defenseZDOID);
        }
        public void AssignDefense(ZDOID defenseZDOID)
        {
            AssignDefense(ZNV.GetZDO().m_uid, defenseZDOID);
        }
        //Container
        public static ZDOID GetContainerZDOID(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return ZDOID.None;
            return zdo.GetZDOID("container");
        }
        public ZDOID GetContainerZDOID()
        {
            return GetContainerZDOID(ZNV.GetZDO().m_uid);
        }
        public static ZDO GetContainerZDO(ZDOID villagerZDOID)
        {
            return ZDOMan.instance.GetZDO(GetContainerZDOID(villagerZDOID));
        }
        public ZDO GetContainerZDO()
        {
            return GetContainerZDO(ZNV.GetZDO().m_uid);
        }
        public static bool IsContainerAssigned(ZDOID villagerZDOID)
        {
            return Util.ValidateZDOID(GetContainerZDOID(villagerZDOID)) && Util.ValidateZDO(GetContainerZDO(villagerZDOID));
        }
        public bool IsContainerAssigned()
        {
            return IsContainerAssigned(ZNV.GetZDO().m_uid);
        }
        public static GameObject GetContainerInstance(ZDOID villagerZDOID)
        {
            return ZNetScene.instance.FindInstance(GetContainerZDOID(villagerZDOID));
        }
        public GameObject GetContainerInstance()
        {
            return GetContainerInstance(ZNV.GetZDO().m_uid);
        }
        public static void SetContainer(ZDOID villagerZDOID, ZDOID containerZDOID)
        {
            Util.GetZDO(villagerZDOID).Set("container", containerZDOID);
        }
        public void SetContainer(ZDOID containerZDOID)
        {
            SetContainer(ZNV.GetZDO().m_uid, containerZDOID);
        }
        public static Inventory GetContainerInventory(ZDOID villagerZDOID)
        {
            //Container stores inventory items in "items" key
            ZDO containerZDO = GetContainerZDO(villagerZDOID);
            string m_name = containerZDO.GetString("m_name", "");
            int w = containerZDO.GetInt("width", 0);
            int h = containerZDO.GetInt("height", 0);
            string items = containerZDO.GetString("items", ""); //Get items from container ZDO
            var pkg = new ZPackage(items); //Load items into zpackage
            var dummySprite = Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero);
            Inventory inv = new Inventory(m_name, dummySprite, w, h); //create new inventory
            if (!string.IsNullOrEmpty(items))
                inv.Load(pkg);
            return inv;
        }
        public Inventory GetContainerInventory()
        {
            return GetContainerInventory(ZNV.GetZDO().m_uid);
        }

        public static void SaveContainerInventory(ZDOID villagerZDOID, Inventory inv)
        {
            ZPackage pkg = new ZPackage();
            inv.Save(pkg);
            string encodedItem = pkg.GetBase64();
            GetContainerZDO(villagerZDOID).Set("items", encodedItem);
        }
        public void SaveContainerInventory(Inventory inv)
        {
            SaveContainerInventory(ZNV.GetZDO().m_uid, inv);
        }
        //Work post
        public static ZDOID GetWorkPostZDOID(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return ZDOID.None;
            return zdo.GetZDOID("workpost");
        }
        public ZDOID GetWorkPostZDOID()
        {
            return GetWorkPostZDOID(ZNV.GetZDO().m_uid);
        }
        public static ZDO GetWorkPostZDO(ZDOID villagerZDOID)
        {
            return ZDOMan.instance.GetZDO(GetWorkPostZDOID(villagerZDOID));
        }
        public ZDO GetWorkPostZDO()
        {
            return GetWorkPostZDO(ZNV.GetZDO().m_uid);
        }
        public static bool IsWorkPostAssigned(ZDOID villagerZDOID)
        {
            return Util.ValidateZDOID(GetWorkPostZDOID(villagerZDOID)) && Util.ValidateZDO(GetWorkPostZDO(villagerZDOID));
        }
        public bool IsWorkPostAssigned()
        {
            return IsWorkPostAssigned(ZNV.GetZDO().m_uid);
        }
        public static GameObject GetWorkPostInstance(ZDOID villagerZDOID)
        {
            return ZNetScene.instance.FindInstance(GetWorkPostZDOID(villagerZDOID));
        }
        public GameObject GetWorkPostInstance()
        {
            return GetWorkPostInstance(ZNV.GetZDO().m_uid);
        }
        public static void AssignWorkPost(ZDOID villagerZDOID, ZDOID defenseZDOID)
        {
            Util.GetZDO(villagerZDOID).Set("workpost", defenseZDOID);
        }
        public void AssignWorkPost(ZDOID defenseZDOID)
        {
            AssignWorkPost(ZNV.GetZDO().m_uid, defenseZDOID);
        }
        //Villager state
        public static VillagerState GetVillagerState(ZDOID villagerZDOID)
        {
            return (VillagerState)Util.GetZDO(villagerZDOID).GetInt("work", (int)VillagerState.Roaming);
        }
        public VillagerState GetVillagerState()
        {
            return GetVillagerState(ZNV.GetZDO().m_uid);
        }
        public static void SetVillagerState(ZDOID villagerZDOID, VillagerState state)
        {
            Util.GetZDO(villagerZDOID).Set("work", (int)state);
        }
        public void SetVillagerState(VillagerState newState)
        {
            SetVillagerState(ZNV.GetZDO().m_uid, newState);
        }

        //Work skills
        public static bool GetWorkSkill_Pickup(ZDOID villagerZDOID)
        {
            return Util.GetZDO(villagerZDOID).GetBool("work_skill_pickup", false);
        }
        public bool GetWorkSkill_Pickup()
        {
            return GetWorkSkill_Pickup(ZNV.GetZDO().m_uid);
        }
        public static void SetWorkSkill_Pickup(ZDOID villagerZDOID, bool canPickup)
        {
            Util.GetZDO(villagerZDOID).Set("work_skill_pickup", canPickup);
        }
        public void SetWorkSkill_Pickup(bool canPickup)
        {
            SetWorkSkill_Pickup(ZNV.GetZDO().m_uid, canPickup);
        }
        public static bool GetWorkSkill_Smelter(ZDOID villagerZDOID)
        {
            return Util.GetZDO(villagerZDOID).GetBool("work_skill_smelt", false);
        }
        public bool GetWorkSkill_Smelter()
        {
            return GetWorkSkill_Smelter(ZNV.GetZDO().m_uid);
        }
        public static void SetWorkSkill_Smelter(ZDOID villagerZDOID, bool canSmelt)
        {
            Util.GetZDO(villagerZDOID).Set("work_skill_smelt", canSmelt);
        }
        public void SetWorkSkill_Smelter(bool canSmelt)
        {
            SetWorkSkill_Smelter(ZNV.GetZDO().m_uid, canSmelt);
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
            else //In case it was tamed when it was not in memory
            {
                tameable.Tame();
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






