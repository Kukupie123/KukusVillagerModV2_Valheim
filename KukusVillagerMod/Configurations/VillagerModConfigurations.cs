using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KukusVillagerMod.Configuration
{
    class VillagerModConfigurations
    {

        public static string weak_villager_melee_prefab;
        public static string weak_villager_ranged_prefab;
        public static string bronze_villager_melee_prefab;
        public static string bronze_villager_ranged_prefab;
        public static string iron_villager_melee_prefab;
        public static string iron_villager_ranged_prefab;
        public static string silver_villager_melee_prefab;
        public static string silver_villager_ranged_prefab;
        public static string bm_villager_melee_prefab;
        public static string bm_villager_ranged_prefab;
        public static int weak_villager_level;
        public static int weak_villager_ranged_level;
        public static int bronze_villager_level;
        public static int bronze_villager_ranged_level;
        public static int iron_villager_level;
        public static int iron_villager_ranged_level;
        public static int silver_villager_level;
        public static int silver_villager_ranged_level;
        public static int bm_villager_level;
        public static int bm_villager_ranged_level;
        public static int weak_villager_health;
        public static int weak_villager_ranged_health;
        public static int bronze_villager_health;
        public static int bronze_villager_ranged_health;
        public static int iron_villager_health;
        public static int iron_villager_ranged_health;
        public static int silver_villager_health;
        public static int silver_villager_ranged_health;
        public static int bm_villager_health;
        public static int bm_villager_ranged_health;
        public static string bed_weak_melee_prefab;
        public static string bed_weak_ranged_prefab;
        public static string bed_bronze_melee_prefab;
        public static string bed_bronze_ranged_prefab;
        public static string bed_iron_melee_prefab;
        public static string bed_iron_ranged_prefab;
        public static string bed_silver_melee_prefab;
        public static string bed_silver_ranged_prefab;
        public static string bed_bm_melee_prefab;
        public static string bed_bm_ranged_prefab;
        public static int weak_bed_respawn;
        public static int bronze_bed_respawn;
        public static int iron_bed_respawn;
        public static int silver_bed_respawn;
        public static int bm_bed_respawn;
        public static string guardBedKey;
        public static string followPlayerKey;
        public static string defendPostKey;
        public static string deletePostKey;
        public static string deleteVillagerKey;
        public static string deleteBedsKey;
        public static string moveToKey;
        public static string showStatKey;

        public static void LoadConfig(ConfigFile Config)
        {
            Config.SaveOnConfigSet = true;

            //Villager prefab
            weak_villager_melee_prefab = (string)Config.Bind("Villager Prefab", "Weak_Villager_Melee", "Skeleton",
       new ConfigDescription("This decides what your models are going to be. You can visit here for full list https://valheim-modding.github.io/Jotunn/data/prefabs/character-list.html",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            weak_villager_ranged_prefab = (string)Config.Bind("Villager Prefab", "Weak_Villager_Ranged", "Dverger",
      new ConfigDescription("",
      null,
      new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bronze_villager_melee_prefab = (string)Config.Bind("Villager Prefab", "Bronze_Villager_Melee", "Skeleton",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bronze_villager_ranged_prefab = (string)Config.Bind("Villager Prefab", "Bronze_Villager_Ranged", "Dverger",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            iron_villager_melee_prefab = (string)Config.Bind("Villager Prefab", "Iron_Villager_Melee", "Skeleton",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            iron_villager_ranged_prefab = (string)Config.Bind("Villager Prefab", "Iron_Villager_Ranged", "Dverger",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            silver_villager_melee_prefab = (string)Config.Bind("Villager Prefab", "Silver_Villager_Melee", "Skeleton",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            silver_villager_ranged_prefab = (string)Config.Bind("Villager Prefab", "Silver_Villager_Ranged", "Dverger",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bm_villager_melee_prefab = (string)Config.Bind("Villager Prefab", "BM_Villager_Melee", "Skeleton",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bm_villager_ranged_prefab = (string)Config.Bind("Villager Prefab", "BM_Villager_Ranged", "Dverger",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;



            //Villager level

            weak_villager_level = (int)Config.Bind("Villagers Level", "Weak_Villager_Level", 0,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            weak_villager_ranged_level = (int)Config.Bind("Villagers Level", "Weak_Villager_Ranged_Level", 0,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bronze_villager_level = (int)Config.Bind("Villagers Level", "Bronze_Villager_Level", 0,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bronze_villager_ranged_level = (int)Config.Bind("Villagers Level", "Bronze_Villager_Ranged_Level", 0,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            iron_villager_level = (int)Config.Bind("Villagers Level", "Iron_Villager_Level", 1,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            iron_villager_ranged_level = (int)Config.Bind("Villagers Level", "Iron_Villager_Ranged_Level", 1,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            silver_villager_level = (int)Config.Bind("Villagers Level", "Silver_Villager_Level", 1,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            silver_villager_ranged_level = (int)Config.Bind("Villagers Level", "Silver_Villager_Ranged_Level", 1,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bm_villager_level = (int)Config.Bind("Villagers Level", "BM_Villager_Level", 2,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bm_villager_ranged_level = (int)Config.Bind("Villagers Level", "BM_Villager_Ranged_Level", 2,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            //Villagers health
            weak_villager_health = (int)Config.Bind("Villagers Health", "Weak_Villager_Health", 400, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            weak_villager_ranged_health = (int)Config.Bind("Villagers Health", "Weak_Villager_Ranged_Health", 200, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bronze_villager_health = (int)Config.Bind("Villagers Health", "Bronze_Villager_Health", 600, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bronze_villager_ranged_health = (int)Config.Bind("Villagers Health", "Bronze_Villager_Ranged_Health", 300, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            iron_villager_health = (int)Config.Bind("Villagers Health", "Iron_Villager_Health", 700, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            iron_villager_ranged_health = (int)Config.Bind("Villagers Health", "Iron_Villager_Ranged_Health", 300, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            silver_villager_health = (int)Config.Bind("Villagers Health", "Silver_Villager_Health", 900, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            silver_villager_ranged_health = (int)Config.Bind("Villagers Health", "Silver_Villager_Ranged_Health", 400, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bm_villager_health = (int)Config.Bind("Villagers Health", "BM_Villager_Health", 1000, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bm_villager_ranged_health = (int)Config.Bind("Villagers Health", "BM_Villager_Ranged_Health", 500, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;


            //Bed model

            bed_weak_melee_prefab = (string)Config.Bind("Bed prefab", "Weak_Bed_Melee", "bed",
       new ConfigDescription("The model used by your bed. Not all piece prefabs are compatible",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bed_weak_ranged_prefab = (string)Config.Bind("Bed prefab", "Weak_Bed_Ranged", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bed_bronze_melee_prefab = (string)Config.Bind("Bed prefab", "Bronze_Bed_Melee", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bed_bronze_ranged_prefab = (string)Config.Bind("Bed prefab", "Bronze_Bed_Ranged", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bed_iron_melee_prefab = (string)Config.Bind("Bed prefab", "Iron_Bed_Melee", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bed_iron_ranged_prefab = (string)Config.Bind("Bed prefab", "Iron_Bed_Ranged", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bed_silver_melee_prefab = (string)Config.Bind("Bed prefab", "Silver_Bed_Melee", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bed_silver_ranged_prefab = (string)Config.Bind("Bed prefab", "Silver_Bed_Ranged", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bed_bm_melee_prefab = (string)Config.Bind("Bed prefab", "BM_Bed_Melee", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bed_bm_ranged_prefab = (string)Config.Bind("Bed prefab", "BM_Bed_Ranged", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;


            //Bed Respawn timer
            weak_bed_respawn = (int)Config.Bind("Bed respawn timer", "Weak_Bed_Melee_RT", 40000, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bronze_bed_respawn = (int)Config.Bind("Bed respawn timer", "Bronze_Bed_Melee_RT", 120000, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            iron_bed_respawn = (int)Config.Bind("Bed respawn timer", "Iron_Bed_Melee_RT", 240000, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            silver_bed_respawn = (int)Config.Bind("Bed respawn timer", "Silver_Bed_Melee_RT", 360000, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bm_bed_respawn = (int)Config.Bind("Bed respawn timer", "BM_Bed_Melee_RT", 480000, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;



            //Villager Commander Club keys
            guardBedKey = (string)Config.Bind("Commander Club keybinds", "Guard Bed Key", "Keypad1").BoxedValue;
            followPlayerKey = (string)Config.Bind("Commander Club keybinds", "Follow Player Key", "Keypad2").BoxedValue;
            defendPostKey = (string)Config.Bind("Commander Club keybinds", "Defend Posts Key", "Keypad3").BoxedValue;
            deletePostKey = (string)Config.Bind("Commander Club keybinds", "Delete Defend Posts", "Keypad4").BoxedValue;
            deleteVillagerKey = (string)Config.Bind("Commander Club keybinds", "Delete Villagers Key", "Keypad5").BoxedValue;
            deleteBedsKey = (string)Config.Bind("Commander Club keybinds", "Delete Beds Key", "Keypad6").BoxedValue;
            moveToKey = (string)Config.Bind("Commander Club keybinds", "Show stats Key", "Keypad7").BoxedValue;
            showStatKey = (string)Config.Bind("Commander Club keybinds", "Show stats Key", "Keypad8").BoxedValue;

        }



    }




}