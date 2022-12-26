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

        private void Awake()
        {
            PrefabManager.OnVanillaPrefabsAvailable += LoadBedPrefab;
            CreatureManager.OnVanillaCreaturesAvailable += LoadVillagerPrefab;
        }

        private void LoadBedPrefab()
        {
            new BedPrefab();
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


        public static HashSet<BedState> bedStates = new HashSet<BedState>(); //Keep track of all the beds in memory
        public static HashSet<VillagerState> villagerStates = new HashSet<VillagerState>(); //Keep track of all villager in memory
        public static HashSet<VillagerState> followingVillagers = new HashSet<VillagerState>(); //Keep track of all villager in memory who are following
        public static HashSet<DefensePostState> defences = new HashSet<DefensePostState>(); //keep track of all defense post in memory

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
