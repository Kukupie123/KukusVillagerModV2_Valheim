// KukusVillagerMod
// a Valheim mod skeleton using Jötunn
// 
// File:    KukusVillagerMod.cs
// Project: KukusVillagerMod

using BepInEx;
using Jotunn.Entities;
using Jotunn.Managers;
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

        // Use this class to add your own localization to the game
        // https://valheim-modding.github.io/Jotunn/tutorials/localization.html
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private void Awake()
        {
        }
    }


    class Global
    {
        public static List<VillagerState> villagerStates = new List<VillagerState>(); //Components of villagers prefab. Essentially keeps track of villagers spawned
        public static List<BedState> bedStates = new List<BedState>(); //Components of bed prefabs. Essentially keeps track of our beds spawned
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
 */
