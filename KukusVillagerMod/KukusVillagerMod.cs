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
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Villagers : {Global.villagers.Count}, Beds : {Global.beds.Count}, DP : {Global.defences.Count},");
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


        public static List<BedCycle> beds = new List<BedCycle>(); //Keep track of all the beds in memory
        public static List<VillagerLifeCycle> villagers = new List<VillagerLifeCycle>(); //Keep track of all villager in memory
        public static List<VillagerLifeCycle> followingVillagers = new List<VillagerLifeCycle>(); //Keep track of all villager in memory who are following
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
        public const string villagerSet = "villagerSet";


    }
}

/*
 * Notes : 
 * To delete scene objects we need to use ZNetScene.instance.delete
 * We use ZDO to save persistent data, ZNetView of the component has to have persistent set to true
 * MapDataLoaded is an event triggered at the last moment after all world data is loaded. We need to be searching for objects in world only after this is true
 * 
 * 
 * 
 * How we persist and Link Beds and villagers
 * Beds save BedID, villagerID and VillagerSet(boolean) in it's zdo
 * Villagers save VillagerID and BedID
 * 
 * Bed and Villagers:
 * 1. When a bed is crafted, We generate UID for the bed and save it as BedID in it's ZDO
 * 2. Next we try to find villagers in world whose ZDo has BedID saved and matches the bed's BedID
 * 3. Ofcourse we are not going to find any villager as this bed was just created
 * 4. We then spawn a Villager
 * 5. The Villager when spawned will generate UID and save it as it's VillagerID
 * 6. The bed will save the Villager's villagerID in it's ZDO
 * 7. The Bed will then save VillagerSet as true in it's ZDO. This is going to be used by Villager as we seen later down the line
 * 8. Lets talk about the villager now.
 * 9. When the villager was spawned it generated UID and saved it as VillagerID in it's ZDO like mentioned in step 5
 * 10. The villager will then Look for it's bed
 * 11. The Looking for bed is going to be recursive as it takes time for the Bed to save the Villager's VillagerID in it's ZDO after it spawns, and after saving the villagerID the bed makes sure to save VillagerSet true value
 * 12. The Villager's find bed function will get all bed in world currently and check if it has VillagerSet as true
 * 13. If it does then it is going to Check the bed's villagerID and the villager's VillagerID. If it matches we know that we found the bed that spawned this villager
 * 14. If the bed doesn't have villagerSet true then it is going to be added to the "villagerLessBed" list and it is going to search that list again.
 * 15. This process continues until we end up with the bed found or end up with an empty "villagerLessBed" list which means that the villager was spawned without a bed. (Maybe bed got destroyed and you closed the game and reopened it or you spawned it using cheats)
 * 16. When no bed is found the villager is going to be deleted to prevent redundant villager. Every villager needs a home
 */