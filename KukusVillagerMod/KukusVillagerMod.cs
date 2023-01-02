// KukusVillagerMod
// a Valheim mod skeleton using Jötunn
// 
// File:    KukusVillagerMod.cs
// Project: KukusVillagerMod

using BepInEx;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Configuration;
using KukusVillagerMod.itemPrefab;
using KukusVillagerMod.Prefabs;

namespace KukusVillagerMod
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class KukusVillagerMod : BaseUnityPlugin
    {
        
        public const string PluginGUID = "com.kukukodes.KukuVillagers";
        public const string PluginName = "KukusVillagerMod";
        public const string PluginVersion = "1.0.0";
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();
        private VillagerCommander vc;

        public static bool isMapDataLoaded = false;

        private void Awake()
        {
            VillagerModConfigurations.LoadConfig(Config);
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
        public const string uid = "UID";
        public const string bedID = "bedUID";
        public const string villagerID = "villagerUID";


    }
}

/*
 * Notes : 
 * To delete scene objects we need to use ZNetScene.instance.delete
 * We use ZDO to save persistent data, ZNetView of the component has to have persistent set to true
 * MapDataLoaded is an event triggered at the last moment after all world data is loaded. We need to be searching for objects in world only after this is true
 * ZNetScene's isAreaReady used to fix infinte spawn and respawn
 */

/*
 * Bed and Villager Linking process :
 * 
 * 
 * TODO:
 * 1. More commands, specially for following character such as move there. And patrol command
 * 2. Remove collision from villagers and players
 */

/*
 * Linking bed and villagers
 * 
 * VillagerCycle will be disabled by default and only bed can set it to enabled by a bed.
 * 
 * Benifits:
 * You can set bed of villager before activating it and then OnEnabled can do some stuff
 * 
 * 
 * Working:
 * When bed is initiated. It checks if it has a UID. if not then it creates one.
 * Then it tries to find villagers nearby and fails to find any villager with the same UID
 * It spawns a new one (villagers are disabled by default)
 * Then it saves the UID on the bed too.
 * Then activates the villager
 * 
 * The villager now has a UID saved and tries to find a bed and it finds it.
 * and saves the reference to the bed
 */