﻿using BepInEx;
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
        public static float weak_bed_respawn;
        public static float bronze_bed_respawn;
        public static float iron_bed_respawn;
        public static float silver_bed_respawn;
        public static float bm_bed_respawn;
        public static string guardBedKey;
        public static string CallFollowers;
        public static string defendPostKey;
        public static string deletePostKey;
        public static string deleteVillagerKey;
        public static string deleteBedsKey;
        public static string moveToKey;
        public static string WorkKey;

        //Villager configuration
        public static int FollowerMaxDistance;
        public static int AcceptedFollowDistance;
        public static int FollowTimeLimit;
        public static int MoveTimeLimit;
        public static int MinWaitTimeWork;
        public static int MaxWaitTimeWork;
        public static bool TalkWhileWorking;


        public static void LoadConfig(ConfigFile Config)
        {
            Config.SaveOnConfigSet = true;
            //Villager AI Configuration
            FollowerMaxDistance = (int)Config.Bind("Villager AI Configuration", "Follower Max Distance", 60,
                new ConfigDescription("If the distance between the player and the villagers following the player exteed this value. They are teleported to the Player.", null, new ConfigurationManagerAttributes { IsAdminOnly = true })
                ).BoxedValue;

            AcceptedFollowDistance = (int)Config.Bind("Villager AI Configuration", "Accepted Follow Distance", 5,
                new ConfigDescription("When following a object (Bed, Defense Post, Work Post), The villagers will stop once they are within this distance", null, new ConfigurationManagerAttributes { IsAdminOnly = true })
                ).BoxedValue;

            FollowTimeLimit = (int)Config.Bind("Villager AI Configuration", "Follow TP Time Limit (Seconds)", 30,
                new ConfigDescription("When Following objects(Bed, Defense Post, Work Post), If the villager does not reach the Accepted Follow Distance of the object wthin this time limit, the villager will be Teleported to the location. This is to prevent AI getting stuck", null, new ConfigurationManagerAttributes { IsAdminOnly = true })
                ).BoxedValue;

            MoveTimeLimit = (int)Config.Bind("Villager AI Configuration", "Move TP Time Limit (Seconds)", 30,
                new ConfigDescription("When Moving to a location (Pickup item, Going to smelter for smelting), If the villager does not reach acceptable range of the target wthin this time limit, the villager will be Teleported to the location. This is to prevent AI getting stuck", null, new ConfigurationManagerAttributes { IsAdminOnly = true })
                ).BoxedValue;
            MinWaitTimeWork = (int)Config.Bind("Villager AI Configuration", "Min Work Wait Time (Milliseconds)", 250,
                new ConfigDescription("Minimum time the villager should wait after one task is complete", null, new ConfigurationManagerAttributes { IsAdminOnly = true })
                ).BoxedValue;
            MaxWaitTimeWork = (int)Config.Bind("Villager AI Configuration", "Max Work Wait Time (Milliseconds)", 3000,
                new ConfigDescription("Maximum time the villager should wait after one task is complete", null, new ConfigurationManagerAttributes { IsAdminOnly = true })
                ).BoxedValue;

            TalkWhileWorking = (bool)Config.Bind("Villager AI Configuration", "Talk while Working", true,
                new ConfigDescription("Villagers will say what their next move is when working", null, new ConfigurationManagerAttributes { IsAdminOnly = true })
                ).BoxedValue;



            //Villagers health

            weak_villager_health = (int)Config.Bind("Villagers Health", "Weak_Villager_Health", 400, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            weak_villager_ranged_health = (int)Config.Bind("Villagers Health", "Weak_Villager_Ranged_Health", 200, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bronze_villager_health = (int)Config.Bind("Villagers Health", "Bronze_Villager_Health", 600, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bronze_villager_ranged_health = (int)Config.Bind("Villagers Health", "Bronze_Villager_Ranged_Health", 300, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            iron_villager_health = (int)Config.Bind("Villagers Health", "Iron_Villager_Health", 800, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            iron_villager_ranged_health = (int)Config.Bind("Villagers Health", "Iron_Villager_Ranged_Health", 400, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            silver_villager_health = (int)Config.Bind("Villagers Health", "Silver_Villager_Health", 1000, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            silver_villager_ranged_health = (int)Config.Bind("Villagers Health", "Silver_Villager_Ranged_Health", 500, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bm_villager_health = (int)Config.Bind("Villagers Health", "BM_Villager_Health", 1200, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bm_villager_ranged_health = (int)Config.Bind("Villagers Health", "BM_Villager_Ranged_Health", 600, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;


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


            //Bed Respawn timer
            weak_bed_respawn = (float)Config.Bind("Bed respawn timer", "Weak_Bed_Melee_RT (Minute)", 1f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bronze_bed_respawn = (float)Config.Bind("Bed respawn timer", "Bronze_Bed_Melee_RT (Minute)", 3f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            iron_bed_respawn = (float)Config.Bind("Bed respawn timer", "Iron_Bed_Melee_RT (Minute)", 5f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            silver_bed_respawn = (float)Config.Bind("Bed respawn timer", "Silver_Bed_Melee_RT (Minute)", 7f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bm_bed_respawn = (float)Config.Bind("Bed respawn timer", "BM_Bed_Melee_RT (Minute)", 9f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            //Villager Commander Club keys
            guardBedKey = (string)Config.Bind("Commander Club keybinds", "Guard Bed Key", "Keypad1").BoxedValue;
            CallFollowers = (string)Config.Bind("Commander Club keybinds", "Call Back Followers", "Keypad2").BoxedValue;
            defendPostKey = (string)Config.Bind("Commander Club keybinds", "Defend Posts Key", "Keypad3").BoxedValue;
            moveToKey = (string)Config.Bind("Commander Club keybinds", "Move to Key", "Keypad4").BoxedValue;
            WorkKey = (string)Config.Bind("Commander Club keybinds", "Start Working Key", "Keypad5").BoxedValue;



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
        }
    }
}