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
        public static string WorkKey;
        public static string RoamKey;

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



            //Villager Commander Club keys
            OpenMenuKey = (string)Config.Bind("Commander Club keybinds", "Open Menu Key", "Keypad1", new ConfigDescription("Key list : https://docs.unity3d.com/ScriptReference/KeyCode.html")).BoxedValue;
            defendPostKey = (string)Config.Bind("Commander Club keybinds", "Defend Posts Key", "Keypad2").BoxedValue;
            WorkKey = (string)Config.Bind("Commander Club keybinds", "Start Working Key", "Keypad3").BoxedValue;
            RoamKey = (string)Config.Bind("Commander Club keybinds", "Start Roaming Key", "Keypad4").BoxedValue;
            CallFollowers = (string)Config.Bind("Commander Club keybinds", "Call Back Followers", "Keypad5").BoxedValue;
            moveToKey = (string)Config.Bind("Commander Club keybinds", "Move to Key", "Keypad6").BoxedValue;

            VillagerMeleePrefabName = (string)Config.Bind("Villager Prefab", "BM_Villager_Melee", "Goblin",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            VillagerRangedPrefabName = (string)Config.Bind("Villager Prefab", "BM_Villager_Ranged", "GoblinArcher",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;






            //Bed model

            VillagerBedPrefab = (string)Config.Bind("Bed prefab", "Weak_Bed_Melee", "bed",
       new ConfigDescription("The model used by your bed. Not all piece prefabs are compatible",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
        }
    }
}