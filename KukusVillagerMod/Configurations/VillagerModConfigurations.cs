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


        //Spawner config
        public static float GroupRadius;
        public static int MaxGroupSize;
        public static int MaxSpawned;
        public static int MinGroupSize;
        public static float SpawnChance;
        public static float SpawnDistance;
        public static string biomeToSpawn;

        public static float MinHealth { get; internal set; }
        public static float MaxHealth { get; internal set; }
        public static float MinEfficiency { get; internal set; }
        public static float MaxEfficiency { get; internal set; }
        public static float MinDmg { get; internal set; }
        public static float MaxDmg { get; internal set; }
        public static float MinSlash { get; internal set; }
        public static float MaxSlash { get; internal set; }
        public static float MinBlunt { get; internal set; }
        public static float MaxBlunt { get; internal set; }
        public static float MaxPierce { get; internal set; }
        public static float MinPierce { get; internal set; }
        public static float MinSpecial { get; internal set; }
        public static float MaxSpecial { get; internal set; }

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

            PickableObjects = (string)Config.Bind("Villager AI Configuration", "Pickable Objects", "Bronze, Iron, Silver, IronScrap, BronzeScrap, SilverScrap, Coal",
                new ConfigDescription("List of items that the worker villager with pickup skill can pickup. Each entries must be separated using ','\nTo add items paste the prefab name from here : https://valheim-modding.github.io/Jotunn/data/prefabs/prefab-list.html", null, new ConfigurationManagerAttributes { IsAdminOnly = true })
                ).BoxedValue;

            //Villager spawn configuration
            GroupRadius = (float)Config.Bind("Villager Spawner", "Group radius", 3f
                ).BoxedValue;
            MaxGroupSize = (int)Config.Bind("Villager Spawner", "Max Group Size", 5
                ).BoxedValue;
            MinGroupSize = (int)Config.Bind("Villager Spawner", "Min Group Size", 2
                ).BoxedValue;
            MaxSpawned = (int)Config.Bind("Villager Spawner", "Max Spawned", 10
                ).BoxedValue;
            SpawnChance = (float)Config.Bind("Villager Spawner", "Spawn Chance", 55f
                ).BoxedValue;
            SpawnDistance = (float)Config.Bind("Villager Spawner", "Spawn Distance", 20f
                ).BoxedValue;

            biomeToSpawn = (string)Config.Bind("Villager Spawner", "Spawn Area", "blackforest,meadows,plains,mountains"
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
            MinSpecial = (float)Config.Bind("Villager Stats", "Min Special_Fire,Frost,Ice etc_RARE QUALITY", 0.1f
           ).BoxedValue;
            MaxSpecial = (float)Config.Bind("Villager Stats", "Max Special", 0.5f
                ).BoxedValue;

            //Villager Commander Club keys
            OpenMenuKey = (string)Config.Bind("Commander Club keybinds", "Open Menu Key", "Keypad1", new ConfigDescription("Key list : https://docs.unity3d.com/ScriptReference/KeyCode.html")).BoxedValue;
            CallFollowers = (string)Config.Bind("Commander Club keybinds", "Call Back Followers", "Keypad3").BoxedValue;
            moveToKey = (string)Config.Bind("Commander Club keybinds", "Move to Key", "Keypad2").BoxedValue;

            VillagerMeleePrefabName = (string)Config.Bind("Villager Prefab", "BM_Villager_Melee", "Goblin",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            VillagerRangedPrefabName = (string)Config.Bind("Villager Prefab", "BM_Villager_Ranged", "GoblinArcher",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
        }

    }
}