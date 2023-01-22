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

        public static string VillagerMeleePrefabName;
        public static string VillagerRangedPrefabName;

        public static string OpenMenuKey;
        public static string CallFollowers;
        public static string moveToKey;

        //Villager configuration
        public static int FollowerMaxDistance;
        public static int AcceptedFollowDistance;
        public static int FollowTimeLimit;
        public static int MoveTimeLimit;
        public static int MinWaitTimeWork;
        public static int MaxWaitTimeWork;
        public static bool TalkWhileWorking;
        public static string PickableObjects;
        public static bool workRun;
        public static float WorkScanRange;
        public static bool UseMoveForWork;


        //Spawner config
        public static float GroupRadius;
        public static int MaxGroupSize;
        public static int MaxSpawned;
        public static int MinGroupSize;
        public static float SpawnChance;
        public static float SpawnDistance;
        public static int NormalVillagerChance;


        public static float MinHealth;
        public static float MaxHealth;
        public static float MinEfficiency;
        public static float MaxEfficiency;
        public static float MinDmg;
        public static float MaxDmg;
        public static float MinSlash;
        public static float MaxSlash;
        public static float MinBlunt;
        public static float MaxBlunt;
        public static float MaxPierce;
        public static float MinPierce;
        public static float MinSpecial;
        public static float MaxSpecial;
        public static float UpgradeStrengthMultiplier;
        public static int ArmorRagSetReq;
        public static int ArmorTrollSetReq;
        public static int ArmorBronzeSetReq;
        public static int ArmorIronSetReq;
        public static int CombatStoneSetReq;
        public static int CombatBronzeSetReq;
        public static int CombatIronSetReq;
        public static int CombatBmSetReq;
        public static float MeadowRandomStatsMultiplier;
        public static float BlackForestRandomStatsMultiplier;
        public static float PlainsRandomStatsMultiplier;
        public static float MountainRandomStatsMultiplier;
        public static float MistlandRandomStatsMultiplier;

        public static float SpawnPoint_SpawnIntervalSec;

        public static int SpawnPoint_MaxNear;

        public static int SpawnPoint_MaxTotal;

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

            workRun = (bool)Config.Bind("Villager AI Configuration", "Run When Working", true,
               new ConfigDescription("Should villagers run when they are working?", null, new ConfigurationManagerAttributes { IsAdminOnly = true })
               ).BoxedValue;

            PickableObjects = (string)Config.Bind("Villager AI Configuration", "Pickable Objects", "Bronze, Iron, Silver, IronScrap, BronzeScrap, SilverScrap, Coal, Wood, Stone, Resin, Flint",
                new ConfigDescription("List of items that the worker villager with pickup skill can pickup. Each entries must be separated using ','\nTo add items paste the prefab name from here : https://valheim-modding.github.io/Jotunn/data/prefabs/prefab-list.html", null, new ConfigurationManagerAttributes { IsAdminOnly = true })
                ).BoxedValue;
            WorkScanRange = (int)Config.Bind("Villager AI Configuration", "Work Scan Range", 50,
                new ConfigDescription("The radius to scan for items when working from the work post.", null, new ConfigurationManagerAttributes { IsAdminOnly = true })
                ).BoxedValue;

            UseMoveForWork = (bool)Config.Bind("Villager AI Configuration", "Use Move instead of follow for work", true,
                new ConfigDescription("There's two ways of going from one place to another, it's preferred to keep this as true but in case you want to use follow method set this as false", null, new ConfigurationManagerAttributes { IsAdminOnly = true })
                ).BoxedValue;

            //Villager spawn configuration
            GroupRadius = (float)Config.Bind("Villager Spawner WILDERNESS", "Wilderness Spawn Group radius", 3f
                ).BoxedValue;
            MaxGroupSize = (int)Config.Bind("Villager Spawner WILDERNESS", "Wilderness Spawn Max Group Size", 1
                ).BoxedValue;
            MinGroupSize = (int)Config.Bind("Villager Spawner WILDERNESS", "Wilderness Spawn Min Group Size", 1
                ).BoxedValue;
            MaxSpawned = (int)Config.Bind("Villager Spawner WILDERNESS", "Wilderness Spawn Max Spawned", 1
                ).BoxedValue;
            SpawnChance = (float)Config.Bind("Villager Spawner WILDERNESS", "Wilderness Spawn Spawn Chance", 0.01f
                ).BoxedValue;
            SpawnDistance = (float)Config.Bind("Villager Spawner WILDERNESS", "Wilderness Spawn Spawn Distance", 100f
                ).BoxedValue;

            //Villager spawner point
            SpawnPoint_SpawnIntervalSec = (float)Config.Bind("Villager Spawner HUT", "Spawn Point Spawn Time interval", 15f, new ConfigDescription("Spawn time interval for villagers spawning in hut")
                ).BoxedValue;
            SpawnPoint_MaxNear = (int)Config.Bind("Villager Spawner HUT", "Spawn Point Max Spawn near", 3, new ConfigDescription("How many villagers will spawn when player is nearby hut")
                ).BoxedValue;
            SpawnPoint_MaxTotal = (int)Config.Bind("Villager Spawner HUT", "Spawn Point Max Spawn Total", 8, new ConfigDescription("How many villagers will spawn when player is far from hut")
               ).BoxedValue;

            //Villager stats configuration
            MinHealth = (float)Config.Bind("Villager Stats", "Minimum Health", 50f
                ).BoxedValue;
            MaxHealth = (float)Config.Bind("Villager Stats", "Maxmimum Health", 100f
                ).BoxedValue;
            MinEfficiency = (float)Config.Bind("Villager Stats", "Min Efficiency", 0.1f
               ).BoxedValue;
            MaxEfficiency = (float)Config.Bind("Villager Stats", "Max Efficiency", 2f
                ).BoxedValue;
            MinDmg = (float)Config.Bind("Villager Stats", "Min Damage", 0.1f
              ).BoxedValue;
            MaxDmg = (float)Config.Bind("Villager Stats", "Max Damage", 5f
                ).BoxedValue;
            MinSlash = (float)Config.Bind("Villager Stats", "Min Slash", 0.1f
              ).BoxedValue;
            MaxSlash = (float)Config.Bind("Villager Stats", "Max Slash", 2f
                ).BoxedValue;
            MinBlunt = (float)Config.Bind("Villager Stats", "Min Blunt", 0.1f
            ).BoxedValue;
            MaxBlunt = (float)Config.Bind("Villager Stats", "Max Blunt", 2f
                ).BoxedValue;
            MinPierce = (float)Config.Bind("Villager Stats", "Min Pierce", 0.1f
           ).BoxedValue;
            MaxPierce = (float)Config.Bind("Villager Stats", "Max Pierce", 2f
                ).BoxedValue;
            MinSpecial = (float)Config.Bind("Villager Stats", "Min Special_Fire,Frost,Ice etc_RARE QUALITY", 0.1f, new ConfigDescription("It's a rare quality and not every villager you find in the wild will have it.")
           ).BoxedValue;
            MaxSpecial = (float)Config.Bind("Villager Stats", "Max Special", 0.5f
                ).BoxedValue;
            UpgradeStrengthMultiplier = (float)Config.Bind("Villager Stats", "Upgrade strength Multiplier", 1.0f, new ConfigDescription("If default upgrade strength is too weak for you, increase this. Lowest value is 0 which will completely disable upgrades, going negative will downgrade the villager")
                ).BoxedValue;

            MeadowRandomStatsMultiplier = (float)Config.Bind("Villager Stats", "Meadow Wandering Stats Multiplier", 1f, new ConfigDescription("Determines how much the stats needs to scale for villagers spawned in the wilderness")
                ).BoxedValue;
            BlackForestRandomStatsMultiplier = (float)Config.Bind("Villager Stats", "BlackForest Wandering Stats Multiplier", 1.5f, new ConfigDescription("Determines how much the stats needs to scale for villagers spawned in the wilderness")
               ).BoxedValue;
            MountainRandomStatsMultiplier = (float)Config.Bind("Villager Stats", "Mountain Wandering Stats Multiplier", 2.5f, new ConfigDescription("Determines how much the stats needs to scale for villagers spawned in the wilderness")
               ).BoxedValue;
            PlainsRandomStatsMultiplier = (float)Config.Bind("Villager Stats", "Plains Wandering Stats Multiplier", 3f, new ConfigDescription("Determines how much the stats needs to scale for villagers spawned in the wilderness")
               ).BoxedValue;
            MistlandRandomStatsMultiplier = (float)Config.Bind("Villager Stats", "Mistland Wandering Stats Multiplier", 3.5f, new ConfigDescription("Determines how much the stats needs to scale for villagers spawned in the wilderness")
              ).BoxedValue;
            NormalVillagerChance = (int)Config.Bind("Villager Stats", "Normal Wild Villager Chance", 5, new ConfigDescription("Determines the chance of villagers having special combat skills (forst,ice,poison,etc). 0 = Every Wild villager will have special skill, increase it by 1 will reduce the chance of villager having special skill. It's counterintuiive I know :P but it is what it is")
              ).BoxedValue;

            //Upgrade items
            ArmorRagSetReq = (int)Config.Bind("Villager Upgrade", "Armor Rag Set Requirements", 3, new ConfigDescription("The amount of resources required to craft upgrading items")
                ).BoxedValue;
            ArmorTrollSetReq = (int)Config.Bind("Villager Upgrade", "Armor Troll Set Requirements", 4
                ).BoxedValue;

            ArmorBronzeSetReq = (int)Config.Bind("Villager Upgrade", "Armor Bronze Set Requirements", 5
                ).BoxedValue;
            ArmorIronSetReq = (int)Config.Bind("Villager Upgrade", "Armor Bronze Set Requirements", 6
                ).BoxedValue;

            CombatStoneSetReq = (int)Config.Bind("Villager Upgrade", "Combat Stone Set Requirements", 1
                ).BoxedValue;
            CombatBronzeSetReq = (int)Config.Bind("Villager Upgrade", "Combat Bronze Set Requirements", 2
                ).BoxedValue;
            CombatIronSetReq = (int)Config.Bind("Villager Upgrade", "Combat Iron Set Requirements", 3
                ).BoxedValue;
            CombatBmSetReq = (int)Config.Bind("Villager Upgrade", "Combat Black Metal Set Requirements", 4
                ).BoxedValue;


            //Villager Commander Club keys
            OpenMenuKey = (string)Config.Bind("Commander Club keybinds", "Open Menu Key", "Keypad1", new ConfigDescription("Key list : https://docs.unity3d.com/ScriptReference/KeyCode.html")).BoxedValue;
            CallFollowers = (string)Config.Bind("Commander Club keybinds", "Call Back Followers", "Keypad3").BoxedValue;
            moveToKey = (string)Config.Bind("Commander Club keybinds", "Move to Key", "Keypad2").BoxedValue;

            VillagerMeleePrefabName = (string)Config.Bind("Villager Prefab", "Villager Melee", "Skeleton",
       new ConfigDescription("Prefab for Melee based villagers, if failed to load it will default to Skeleton",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            VillagerRangedPrefabName = (string)Config.Bind("Villager Prefab", "Villager Ranged", "Dverger",
       new ConfigDescription("Prefab for Melee based villagers, if failed to load it will default to Dverger",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
        }

    }
}