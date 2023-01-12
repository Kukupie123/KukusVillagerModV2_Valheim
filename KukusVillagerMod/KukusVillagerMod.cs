// KukusVillagerMod
// a Valheim mod skeleton using Jötunn
// 
// File:    KukusVillagerMod.cs
// Project: KukusVillagerMod

using BepInEx;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Configuration;
using KukusVillagerMod.itemPrefab;
using KukusVillagerMod.Prefabs;
using KukusVillagerMod.Components;
using System.Collections.Generic;

namespace KukusVillagerMod
{
    [BepInDependency(Jotunn.Main.ModGuid)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class KukusVillagerMod : BaseUnityPlugin
    {
        public static bool isModded = true;
        public const string PluginGUID = "com.kukukodes.KukuVillagers";
        public const string PluginName = "KukusVillagerMod";
        public const string PluginVersion = "2.0.0";
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();
        private VillagerCommander vc;

        public static bool isMapDataLoaded = false;

        private readonly Harmony harmony = new Harmony("kukuvillager");
        private void Awake()
        {
            VillagerModConfigurations.LoadConfig(Config);
            PrefabManager.OnVanillaPrefabsAvailable += LoadBedPrefab;
            CreatureManager.OnVanillaCreaturesAvailable += LoadVillagerPrefab;
            MinimapManager.OnVanillaMapDataLoaded += OnMapDataLoaded;

            harmony.PatchAll();

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
            //Register Piece and Item
            new BedPrefab();
            new DefensePostPrefab();
            new WorkPostPrefab();
            new IndividualVillagerCommandItemPrefab();
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
        private Global() { }

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
}

/*
 * Notes : 
 * To delete scene objects we need to use ZNetScene.instance.delete
 * We use ZDO to save persistent data, ZNetView of the component has to have persistent set to true
 * MapDataLoaded is an event triggered at the last moment after all world data is loaded. We need to be searching for objects in world only after this is true
 * ZNetScene's isAreaReady used to fix infinte spawn and respawn
 * We can get the game object by doind znetscene.instance.Getinstance(zdoid). So save ZDOid of important data
 */

/*
 * TODO:
 * 1. INDIVIDUAL BED/Villager Commands
 * 1. AI Tweaks such as attack rate etc
 * 2. Troll worker villager.
 * 3. When near bed they regen HP. 
 * 4. Show commands in commander desc
 * 5. Show how to control bed in bed desc
 * 6. Map marker
 * 8. Updating contents of containers, smelters etc using ZDO
 * 9. Config for AI worker such as wait time, what to pickup etc~DONE
 * 11. Huge thing but, virtual refuling and picking using ZDO? Eg for pickup we can get distance of zdo and decide then put those item inside container using zdo serialize deserialize. same concept for fueling and all
 * 12. Enable bed interaction for commanding villagers.~DONE
 * 
 * How should we go about it hmmm.
 * 
 * They can be commanded just like normal villagers.
 * 
 */

/*
 * BUG:
 * 1. FUEL OVER EXCEED FIX
 * 2. CHECK CONTAINER FOR THIS ISSUE TOO
 */

/*
 * A bed is going to spawn a villager and save it's ZDOID, the villager spawned in is going to save the beds ZDO ID
 * Thats how they can know about each other.
 * Even if the areas are not loaded the ZDOID is always going to be valid and informations like positions etc etc are always going to bevalid
 * 
 * the only time ZDOID is going to be invalid is if it doesn't exist anymore.
 * 
 * 
 * We are also going to be storing defense post ID and container ID in bed.
 * We Can then use this to send a villager to the defense post the bed has saved the id of.
 * Same goes for container.
 * 
 * We save the state of the villager in bed too such as guarding bed, defending post, following player etc
 */