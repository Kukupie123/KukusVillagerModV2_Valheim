// KukusVillagerMod
// a Valheim mod skeleton using Jötunn
// 
// File:    KukusVillagerMod.cs
// Project: KukusVillagerMod

using BepInEx;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.itemPrefab;
using KukusVillagerMod.Prefabs;
using KukusVillagerMod.States;
using System.Collections.Generic;

namespace KukusVillagerMod
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class KukusVillagerMod : BaseUnityPlugin
    {
        public const string PluginGUID = "com.jotunn.KukusVillagerMod";
        public const string PluginName = "KukusVillagerMod";
        public const string PluginVersion = "1.0.0";
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();
        private VillagerCommander vc;

        private void Awake()
        {
            PrefabManager.OnVanillaPrefabsAvailable += LoadBedPrefab;
            CreatureManager.OnVanillaCreaturesAvailable += LoadVillagerPrefab;
        }

        private void FixedUpdate()
        {
            if (vc != null)
            {
                vc.HandleInputs();
            }
            if (MessageHud.instance != null)
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Villagers : {Global.villagerStates.Count} Followers : {Global.followingVillagers.Count} Beds : {Global.bedStates.Count} DP : {Global.defences.Count}");


            //Clean lists
            foreach (var i in Global.bedStates)
            {
                if (i == null)
                {
                    Global.bedStates.Remove(i);
                }
                else if (i.gameObject == null)
                {
                    Global.bedStates.Remove(i);
                }
            }

            foreach (var i in Global.villagerStates)
            {
                if (i == null)
                {
                    Global.villagerStates.Remove(i);
                }
                else if (i.gameObject == null)
                {
                    Global.villagerStates.Remove(i);
                }
            }
            foreach (var i in Global.followingVillagers)
            {
                if (i == null)
                {
                    Global.followingVillagers.Remove(i);
                }
                else if (i.gameObject == null)
                {
                    Global.followingVillagers.Remove(i);
                }
            }
            foreach (var i in Global.defences)
            {
                if (i == null)
                {
                    Global.defences.Remove(i);
                }
                else if (i.gameObject == null)
                {
                    Global.defences.Remove(i);
                }
            }
        }



        private void LoadBedPrefab()
        {
            new BedPrefab();
            new DefensivePostPrefab();
            vc = new VillagerCommander();
            PrefabManager.OnVanillaPrefabsAvailable -= LoadBedPrefab;
        }
        private void LoadVillagerPrefab()
        {
            new VillagerPrefab();
            CreatureManager.OnVanillaCreaturesAvailable -= LoadVillagerPrefab;
        }
    }


    class Global
    {


        public static List<BedState> bedStates = new List<BedState>(); //Keep track of all the beds in memory
        public static List<VillagerState> villagerStates = new List<VillagerState>(); //Keep track of all villager in memory
        public static List<VillagerState> followingVillagers = new List<VillagerState>(); //Keep track of all villager in memory who are following
        public static List<DefensePostState> defences = new List<DefensePostState>(); //keep track of all defense post in memory

    }

    class KLog
    {
        public static void info(string msg)
        {
            Jotunn.Logger.LogInfo(msg);
        }
        public static void warning(string msg)
        {
            Jotunn.Logger.LogWarning(msg);
        }
    }

    class Util
    {
        private Util()
        {
        }

        public const string villagerID = "villagerID";
        public const string bedID = "bedID";


    }
}
/*
 * NOTES:
 * ZDO data are saved in parent component from state components such as VillagerState saves its data as GetComponentInParent, Same goes for BedState
 * 
 * How it works :
 * We use ZDO to save link between villager and beds
 * 
 * Bed's Map {
 * uid : "uid of bed",
 * villagerID : "uid of villager who spawned using this bed"
 * }
 * Villager's Map {
 * uid : "uid of villager",
 * bedID : "uid of the bed which spawned this villager"
 * }
 * 
 * When we create a bed. It needs to setup it's uid.
 * Then it needs to spawn a villager. The villager will then setup it's uid
 * Once done the bed will set it's villagerID to uid of Villager
 * The Villager will set its bedID to the uid of the bed 
 * 
 * This can be heavily simplified if we can have a global ZNet
 */


//PROBLEM : HASHLIST FUCKED