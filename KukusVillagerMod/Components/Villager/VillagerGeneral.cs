using Jotunn.Managers;
using KukusVillagerMod.Components.VillagerBed;
using KukusVillagerMod.Configuration;
using KukusVillagerMod.enums;
using KukusVillagerMod.enums.Work_Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KukusVillagerMod.Components.Villager
{
    class VillagerGeneral : MonoBehaviour
    {
        //Methods

        public int goldToRecruit;


        public static string GetVillagerFaction(ZDOID villagerZDOID)
        {
            return Util.GetZDO(villagerZDOID).GetString("faction", "NONE");
        }

        public string GetVillagerFaction()
        {
            return GetVillagerFaction(ZNV.GetZDO().m_uid);
        }

        public static void SetVillagerFaction(ZDOID villagerZDOID, string faction)
        {
            Util.GetZDO(villagerZDOID).Set("faction", faction.Trim().ToUpper().Replace(" ", "").Replace("_", ""));
        }

        public void SetVillagerFaction(string faction)
        {
            SetVillagerFaction(ZNV.GetZDO().m_uid, faction);
        }

        public static void SetVillagerSpawnRegion(ZDOID villagerZDOID, Heightmap.Biome biome)
        {
            Util.GetZDO(villagerZDOID).Set("spawn", (int)biome);
        }

        public void SetVillagerSpawnRegion(Heightmap.Biome biome)
        {
            SetVillagerSpawnRegion(ZNV.GetZDO().m_uid, biome);
        }

        public static Heightmap.Biome GetVillagerSpawnRegion(ZDOID villagerZDOID)
        {
            return (Heightmap.Biome)Util.GetZDO(villagerZDOID).GetInt("spawn", (int)Heightmap.Biome.Meadows);
        }

        public Heightmap.Biome GetVillagerSpawnRegion()
        {
            return GetVillagerSpawnRegion(ZNV.GetZDO().m_uid);
        }

        public static GameObject GetVillagerInstance(ZDOID villagerZDOID)
        {
            return ZNetScene.instance.FindInstance(villagerZDOID);
        }

        public static ZDOID SELECTED_VILLAGER_ID = ZDOID.None;
        public static List<ZDOID> SELECTED_VILLAGERS_ID = null;

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
            LoadStatsFromZDO();
            return TameVillager(ZNV.GetZDO().m_uid);
        }

        public static bool IsVillagerTamed(ZDOID villagerZDOID)
        {
            if (!Util.ValidateZDOID(villagerZDOID)) return false;
            ZDO villagerZDO = ZDOMan.instance.GetZDO(villagerZDOID);
            if (!Util.ValidateZDO(villagerZDO)) return false;
            return villagerZDO.GetBool("tamed");
        }

        public bool IsVillagerTamed()
        {
            if (ZNV == null) return false;
            return IsVillagerTamed(ZNV.GetZDO().m_uid);
        }

        //Stats
        public static void SetRandomStats(ZNetView ZNV, float modifier = 1)
        {
            //Basics
            var n = Util.RandomName();
            ZNV.GetZDO().Set("name", n);
            float minVal = VillagerModConfigurations.MinHealth;
            float maxVal = VillagerModConfigurations.MaxHealth;
            ZNV.GetZDO().Set("stathealth", UnityEngine.Random.Range(minVal, maxVal) * modifier);
            minVal = VillagerModConfigurations.MinEfficiency;
            maxVal = VillagerModConfigurations.MaxEfficiency;
            ZNV.GetZDO().Set("efficiency", UnityEngine.Random.Range(minVal, maxVal) * modifier); //Percentage stuff

            KLog.warning($"Villager {ZNV.GetZDO().m_uid.id} has name {n} ");

            //Farming stuff FOR FUTURE WHEN I FIGURE OUT CONSISTENT METHOD
            ZNV.GetZDO().Set("pickaxe", 15.0f); //15 is base, 18 is antler
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
            minVal = VillagerModConfigurations.MinDmg;
            maxVal = VillagerModConfigurations.MaxDmg;
            ZNV.GetZDO().Set("damage", UnityEngine.Random.Range(minVal, maxVal) * modifier);
            minVal = VillagerModConfigurations.MinSlash;
            maxVal = VillagerModConfigurations.MaxSlash;
            ZNV.GetZDO().Set("slash", UnityEngine.Random.Range(minVal, maxVal) * modifier);
            minVal = VillagerModConfigurations.MinBlunt;
            maxVal = VillagerModConfigurations.MaxBlunt;
            ZNV.GetZDO().Set("blunt", UnityEngine.Random.Range(minVal, maxVal) * modifier);
            ZNV.GetZDO().Set("fire", 0f); //special
            ZNV.GetZDO().Set("frost", 0f); //special
            ZNV.GetZDO().Set("lightning", 0f); //special
            minVal = VillagerModConfigurations.MinPierce;
            maxVal = VillagerModConfigurations.MaxPierce;
            ZNV.GetZDO().Set("pierce", UnityEngine.Random.Range(minVal, maxVal) * modifier);
            ZNV.GetZDO().Set("poison", 0f); //special
            ZNV.GetZDO().Set("spirit", 0f); //special

            if (UnityEngine.Random.Range(0, VillagerModConfigurations.NormalVillagerChance) == 0)
            {
                minVal = VillagerModConfigurations.MinSpecial;
                maxVal = VillagerModConfigurations.MaxSpecial;

                ZNV.GetZDO().Set("special", true);
                //Speciality
                switch (UnityEngine.Random.Range(0, 4))
                {
                    case 0:
                        ZNV.GetZDO().Set("fire", UnityEngine.Random.Range(minVal, maxVal) * modifier);
                        break;
                    case 1:
                        ZNV.GetZDO().Set("frost", UnityEngine.Random.Range(minVal, maxVal) * modifier);
                        break;
                    case 2:
                        ZNV.GetZDO().Set("lightning", UnityEngine.Random.Range(minVal, maxVal) * modifier);
                        break;
                    case 3:
                        ZNV.GetZDO().Set("poison", UnityEngine.Random.Range(minVal, maxVal) * modifier);
                        break;
                    case 4:
                        ZNV.GetZDO().Set("spirit", UnityEngine.Random.Range(minVal, maxVal) * modifier);
                        break;
                }
            }
        }

        private void SetRandomStats(float modifier = 1)
        {
            if (humanoid.IsTamed()) KLog.warning("Villager is tamed, aborting setting up random values");
            else
            {
                SetRandomStats(this.ZNV, modifier);
            }
        }

        private void LoadStatsFromZDO()
        {
            if (IsVillagerTamed())
                humanoid.m_name = "Villager " + GetName();
            else humanoid.m_name = "Wanderer " + GetName();

            //Add special title if they are special 
            if (GetSpecialSkill() != null)
            {
                humanoid.m_name = "* " + humanoid.m_name;
            }

            //Set up health
            humanoid.SetMaxHealth(GetStatHealth());
            //If not recruited then set current hp to max
            //if (!IsVillagerTamed())
            //{
            humanoid.SetHealth(GetStatHealth()); //Had to do this. The villagers were getting their health reset
            //}
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
            return zdo.GetFloat("poison", 0f);
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

        public static void SetName(ZDOID villagerZDOID, string name)
        {
            Util.GetZDO(villagerZDOID).Set("name", name);
        }

        public void SetName(string name)
        {
            SetName(ZNV.GetZDO().m_uid, name);
            LoadStatsFromZDO();
        }

        public static float GetStatHealth(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetFloat("stathealth");
        }

        public float GetStatHealth()
        {
            return GetStatHealth(this.ZNV.GetZDO().m_uid);
        }

        public float GetAIHP()
        {
            return humanoid.GetHealth();
        }

        public void SetAIHP(float hp)
        {
            this.humanoid.SetHealth(hp);
        }

        public static int GetWorkLevel(ZDOID villagerZDOID)
        {
            var zdo = Util.GetZDO(villagerZDOID);
            if (Util.ValidateZDO(zdo) == false) return 0;
            return zdo.GetInt("mining");
        }

        public int GetWorkLevel()
        {
            return GetWorkLevel(this.ZNV.GetZDO().m_uid);
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

        public static void UpgradeVillagerDamage(ZDOID villagerZDOID, float multiplier = 1)
        {
            /*
             * weapon.m_shared.m_damages.m_damage = 0f;
               weapon.m_shared.m_damages.m_slash = 0f;
               weapon.m_shared.m_damages.m_blunt = 0f;
               weapon.m_shared.m_damages.m_fire = 0f;
               weapon.m_shared.m_damages.m_frost = 0f;
               weapon.m_shared.m_damages.m_lightning = 0f;
               weapon.m_shared.m_damages.m_pierce = 0f;
               weapon.m_shared.m_damages.m_poison = 0f;
               weapon.m_shared.m_damages.m_spirit = 0f;
            */

            //Damages
            var zdo = Util.GetZDO(villagerZDOID);
            var dmg = GetDamage(villagerZDOID);
            if (dmg != 0f)
            {
                zdo.Set("damage",
                    dmg + (GetEfficiency(villagerZDOID) * multiplier) *
                    VillagerModConfigurations.UpgradeStrengthMultiplier);
                KLog.warning($"New Damage : {dmg}");
            }

            dmg = GetSlash(villagerZDOID);
            if (dmg != 0f)
            {
                zdo.Set("slash",
                    dmg + (GetEfficiency(villagerZDOID) * multiplier) *
                    VillagerModConfigurations.UpgradeStrengthMultiplier);
                KLog.warning($"New slash : {dmg}");
            }

            dmg = GetPierce(villagerZDOID);
            if (dmg != 0f)
            {
                zdo.Set("pierce",
                    dmg + (GetEfficiency(villagerZDOID) * multiplier) *
                    VillagerModConfigurations.UpgradeStrengthMultiplier);
                KLog.warning($"New Pierce : {dmg}");
            }

            dmg = GetBlunt(villagerZDOID);
            if (dmg != 0f)
            {
                zdo.Set("blunt",
                    dmg + (GetEfficiency(villagerZDOID) * multiplier) *
                    VillagerModConfigurations.UpgradeStrengthMultiplier);
                KLog.warning($"New Blunt : {dmg}");
            }

            //special damages
            dmg = GetFire(villagerZDOID);
            if (dmg != 0f)
            {
                zdo.Set("fire",
                    dmg + (GetEfficiency(villagerZDOID) * multiplier) *
                    VillagerModConfigurations.UpgradeStrengthMultiplier);
                KLog.warning($"New Fire : {dmg}");
            }

            dmg = GetFrost(villagerZDOID);
            if (dmg != 0f)
            {
                zdo.Set("frost",
                    dmg + (GetEfficiency(villagerZDOID) * multiplier) *
                    VillagerModConfigurations.UpgradeStrengthMultiplier);
                KLog.warning($"New Frost : {dmg}");
            }

            dmg = Getlightning(villagerZDOID);
            if (dmg != 0f)
            {
                zdo.Set("lightning",
                    dmg + (GetEfficiency(villagerZDOID) * multiplier) *
                    VillagerModConfigurations.UpgradeStrengthMultiplier);
                KLog.warning($"New Lightning : {dmg}");
            }

            dmg = GetPoison(villagerZDOID);
            if (dmg != 0f)
            {
                zdo.Set("poison",
                    dmg + (GetEfficiency(villagerZDOID) * multiplier) *
                    VillagerModConfigurations.UpgradeStrengthMultiplier);
                KLog.warning($"New Poison : {dmg}");
            }

            dmg = GetSpirit(villagerZDOID);
            if (dmg != 0f)
            {
                zdo.Set("spirit",
                    dmg + (GetEfficiency(villagerZDOID) * multiplier) *
                    VillagerModConfigurations.UpgradeStrengthMultiplier);
                KLog.warning($"New Spirit : {dmg}");
            }
        }

        public void UpgradeVillagerDamage(float multiplier = 1)
        {
            UpgradeVillagerDamage(ZNV.GetZDO().m_uid, multiplier);
            LoadStatsFromZDO();
        }

        public static void UpgradeVillagerHealth(ZDOID villagerZDOID, float multiplier = 1)
        {
            var health = GetStatHealth(villagerZDOID) + (GetEfficiency(villagerZDOID) * multiplier *
                                                         VillagerModConfigurations.UpgradeStrengthMultiplier);
            KLog.warning($"New Health = {health}");
            Util.GetZDO(villagerZDOID).Set("stathealth", health);
        }

        public void UpgradeVillagerHealth(float multiplier)
        {
            UpgradeVillagerHealth(ZNV.GetZDO().m_uid, multiplier);
            LoadStatsFromZDO();
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
            return Util.GetZDO(GetBedZDOID(villagerZDOID));
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
            if (!Util.ValidateZDOID(bedZDOID))
            {
                KLog.warning("BedZDOID invalid for assignBed()");
                return;
            }

            //Check if bed already had a villager
            if (BedState.IsVillagerAssigned(bedZDOID))
            {
                //Remove bed from the villager
                RemoveBedForVillager(BedState.GetVillagerZDOID(bedZDOID));
            }

            Util.GetZDO(villagerZDOID).Set("bed", bedZDOID);
            Util.GetZDO(bedZDOID).Set("villager", villagerZDOID);
        }

        public static void RemoveBedForVillager(ZDOID villagerZDOID)
        {
            //Remove villager from bed
            GetBedZDO(villagerZDOID).Set("villager", ZDOID.None);
            //Remove bed from villager
            Util.GetZDO(villagerZDOID).Set("bed", ZDOID.None);
        }

        public void RemoveBedForVillager()
        {
            RemoveBedForVillager(ZNV.GetZDO().m_uid);
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
            return Util.ValidateZDOID(GetContainerZDOID(villagerZDOID)) &&
                   Util.ValidateZDO(GetContainerZDO(villagerZDOID));
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

        public static void AssignContainer(ZDOID villagerZDOID, ZDOID containerZDOID)
        {
            Util.GetZDO(villagerZDOID).Set("container", containerZDOID);
        }

        public void AssignContainer(ZDOID containerZDOID)
        {
            AssignContainer(ZNV.GetZDO().m_uid, containerZDOID);
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
            return Util.ValidateZDOID(GetWorkPostZDOID(villagerZDOID)) &&
                   Util.ValidateZDO(GetWorkPostZDO(villagerZDOID));
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
            switch (state)
            {
                case VillagerState.Guarding_Bed:
                    if (IsBedAssigned(villagerZDOID))
                    {
                        Util.GetZDO(villagerZDOID).Set("work", (int)VillagerState.Guarding_Bed);
                        Util.GetZDO(villagerZDOID).SetPosition(GetBedZDO(villagerZDOID).GetPosition());
                    }
                    else
                    {
                        Util.GetZDO(villagerZDOID).Set("work", (int)VillagerState.Roaming);
                    }

                    break;
                case VillagerState.Defending_Post:
                    if (IsDefenseAssigned(villagerZDOID))
                    {
                        Util.GetZDO(villagerZDOID).Set("work", (int)VillagerState.Defending_Post);
                        Util.GetZDO(villagerZDOID).SetPosition(GetDefenseZDO(villagerZDOID).GetPosition());
                    }
                    else
                    {
                        Util.GetZDO(villagerZDOID).Set("work", (int)VillagerState.Roaming);
                    }

                    break;
                case VillagerState.Roaming:
                    Util.GetZDO(villagerZDOID).Set("work", (int)VillagerState.Roaming);
                    break;
                case VillagerState.Working:
                    if (IsWorkPostAssigned(villagerZDOID) && IsContainerAssigned(villagerZDOID))
                    {
                        Util.GetZDO(villagerZDOID).Set("work", (int)VillagerState.Working);
                        Util.GetZDO(villagerZDOID).SetPosition(GetWorkPostZDO(villagerZDOID).GetPosition());
                    }
                    else
                    {
                        Util.GetZDO(villagerZDOID).Set("work", (int)VillagerState.Roaming);
                    }

                    break;
                case VillagerState.Following:
                    Util.GetZDO(villagerZDOID).Set("work", (int)VillagerState.Following);
                    break;
            }
        }

        public void SetVillagerState(VillagerState newState)
        {
            SetVillagerState(ZNV.GetZDO().m_uid, newState);
        }

        //Work skills
        public static WorkSkill GetWorkSkill(ZDOID villagerZDOID)

        {
            return (WorkSkill)Util.GetZDO(villagerZDOID).GetInt("workskill", (int)WorkSkill.Pickup);
        }

        public WorkSkill GetWorkSkill()
        {
            return GetWorkSkill(ZNV.GetZDO().m_uid);
        }

        public static void SetWorkSkill(ZDOID villagerZDOID, WorkSkill skill)
        {
            Util.GetZDO(villagerZDOID).Set("workskill", (int)skill);
        }

        public void SetWorkSkill(WorkSkill skill)
        {
            SetWorkSkill(ZNV.GetZDO().m_uid, skill);
        }

        //Object methods and members-------------------------------

        public ZNetView ZNV;
        private MonsterAI ai;
        public Humanoid humanoid;
        private Tameable tameable;
        public Heightmap.Biome assignedBiome;


        //Ignore collision with player
        private void OnCollisionEnter(Collision collision)
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (character != null
                && character.m_faction == Character.Faction.Players
                && character.GetComponent<VillagerGeneral>() == null) // allow collision between minions
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
            }
        }

        bool done = false;
        public CharacterDrop charDrop;

        private void FixedUpdate()
        {
            if (tameable == null || ZNV == null || ai == null || humanoid == null || charDrop == null)
            {
                if (ZNV == null)
                {
                    ZNV = GetComponent<ZNetView>();
                    if (ZNV == null)
                    {
                        ZNV = base.GetComponent<ZNetView>();
                    }

                    if (ZNV == null)
                    {
                        ZNV = GetComponentInParent<ZNetView>();
                    }

                    if (ZNV == null)
                    {
                        ZNV = GetComponentInChildren<ZNetView>();
                    }
                }

                if (ZNV == null)
                {
                    KLog.warning("FAILED TO FIND ZNetView for villager");
                }

                if (tameable == null)
                {
                    tameable = GetComponent<Tameable>();
                    if (tameable == null)
                    {
                        tameable = GetComponentInParent<Tameable>();
                        if (tameable)
                        {
                            KLog.warning($"Found tameable for");
                        }
                    }

                    if (tameable == null)
                    {
                        tameable = GetComponentInChildren<Tameable>();
                    }

                    if (tameable == null)
                    {
                        tameable = base.GetComponent<Tameable>();
                    }
                }

                if (tameable == null)
                {
                    KLog.warning("Tameable component not Found!");
                }

                if (ai == null)
                {
                    ai = GetComponent<MonsterAI>();
                    if (ai == null)
                    {
                        ai = GetComponentInParent<MonsterAI>();
                    }

                    if (ai == null)
                    {
                        ai = GetComponentInChildren<MonsterAI>();
                    }

                    if (ai == null)
                    {
                        ai = base.GetComponent<MonsterAI>();
                    }
                }

                if (ai == null)
                {
                    KLog.warning("AI component not Found!");
                }

                if (humanoid == null)
                {
                    humanoid = GetComponent<Humanoid>();
                    if (humanoid == null)
                    {
                        humanoid = GetComponentInParent<Humanoid>();
                    }

                    if (humanoid == null)
                    {
                        humanoid = GetComponentInChildren<Humanoid>();
                    }

                    if (humanoid == null)
                    {
                        humanoid = base.GetComponent<Humanoid>();
                    }
                }

                if (humanoid == null)
                {
                    KLog.warning("humanoid component not Found!");
                }


                if (charDrop == null)
                {
                    charDrop = GetComponent<CharacterDrop>();
                    if (charDrop == null)
                    {
                        charDrop = GetComponentInParent<CharacterDrop>();
                    }

                    if (charDrop == null)
                    {
                        charDrop = GetComponentInChildren<CharacterDrop>();
                    }

                    if (charDrop == null)
                    {
                        charDrop = base.GetComponent<CharacterDrop>();
                    }
                }

                if (charDrop == null)
                {
                    KLog.warning("chardrop component not Found!");
                }
            }
            else
            {
                if (done == false)
                {
                    done = true;
                    ai.m_attackPlayerObjects = false;
                    ai.m_avoidFire = true;
                    ai.SetHuntPlayer(false);
                    ai.m_sleepDelay = 0.01f;


                    humanoid.m_faction = Character.Faction.Players;
                    humanoid.m_group = "Player";

                    //humanoid.m_speed = 100;
                    //humanoid.m_runSpeed = 100;
                    //Generate and load stats
                    if (!IsVillagerTamed())
                    {
                        //Check biome and scale damage as needed

                        var biome = Heightmap.FindBiome(transform.position);
                        float statsMultiplier = 1f;
                        switch (assignedBiome)
                        {
                            case Heightmap.Biome.Meadows:
                                KLog.info("Spawned Wild villager in meadows");
                                statsMultiplier = VillagerModConfigurations.MeadowRandomStatsMultiplier;
                                break;
                            case Heightmap.Biome.BlackForest:
                                KLog.info("Spawned Wild villager in Black forest");
                                statsMultiplier = VillagerModConfigurations.BlackForestRandomStatsMultiplier;
                                break;
                            case Heightmap.Biome.Plains:
                                KLog.info("Spawned Wild villager in Plains");
                                statsMultiplier = VillagerModConfigurations.PlainsRandomStatsMultiplier;
                                break;
                            case Heightmap.Biome.Mountain:
                                KLog.info("Spawned Wild villager in Mountain");
                                statsMultiplier = VillagerModConfigurations.MountainRandomStatsMultiplier;
                                break;
                            case Heightmap.Biome.Mistlands:
                                KLog.info("Spawned Wild villager in MistLands");
                                statsMultiplier = VillagerModConfigurations.MistlandRandomStatsMultiplier;
                                break;
                        }

                        SetVillagerSpawnRegion(assignedBiome);
                        SetRandomStats(statsMultiplier);
                    }
                    else //In case it was tamed when it was not in memory
                    {
                        tameable.Tame();
                    }

                    //Sometime villagers will throw error so we do this to fix it, related to patching getCurrentWeapon function of humanoid
                    UpgradeVillagerDamage(0);
                    UpgradeVillagerHealth(0);
                    LoadStatsFromZDO();
                }
            }
        }
    }
}