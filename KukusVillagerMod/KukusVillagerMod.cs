// KukusVillagerMod
// a Valheim mod skeleton using J�tunn
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
        public const string PluginGUID = "com.jotunn.KukuVillagers";
        public const string PluginName = "KukusVillagerMod";
        public const string PluginVersion = "2.0.0";
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();
        private VillagerCommander vc;

        public static bool isMapDataLoaded = false;

        private void Awake()
        {
            PrefabManager.OnVanillaPrefabsAvailable += LoadBedPrefab;
            CreatureManager.OnVanillaCreaturesAvailable += LoadVillagerPrefab;
            MinimapManager.OnVanillaMapDataLoaded += OnMapDataLoaded;
        }

        private void OnMapDataLoaded()
        {
            isMapDataLoaded = true;
        }

        private void FixedUpdate()
        {
            if (vc != null)
            {
                vc.HandleInputs();
            }
            if (MessageHud.instance != null)
            {
                //MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Villagers : {Global.villagers.Count}, Beds : {Global.beds.Count}, DP : {Global.defences.Count},");
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
 * Notes : 
 * To delete scene objects we need to use ZNetScene.instance.delete
 * We use ZDO to save persistent data, ZNetView of the component has to have persistent set to true
 * MapDataLoaded is an event triggered at the last moment after all world data is loaded. We need to be searching for objects in world only after this is true
 * ZNetScene's isAreaReady used to fix infinte spawn and respawn
 */