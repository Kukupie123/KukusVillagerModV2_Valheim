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
using KukusVillagerMod.VirtualWorker;

namespace KukusVillagerMod
{
    [BepInDependency("com.alexanderstrada.rrrnpcs")]
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
        private VirtualWork vw;
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
            if (vw != null)
            {
                vw.vw();
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
            vw = new VirtualWork();
            PrefabManager.OnVanillaPrefabsAvailable -= LoadBedPrefab;
        }


        private void LoadVillagerPrefab()
        {
            new VillagerPrefab();
            CreatureManager.OnVanillaCreaturesAvailable -= LoadVillagerPrefab;
        }

    }

    class Util
    {
        private Util() { }
        public static bool ValidateZDOID(ZDOID zdoid)
        {
            if (zdoid == null || zdoid.IsNone()) return false;
            return true;
        }
        public static bool ValidateZDO(ZDO zdo)
        {
            if (zdo == null || !zdo.IsValid()) return false;
            return true;
        }
        public static bool ValidateZNetView(ZNetView ZNV)
        {
            if (ZNV == null || ZNV.IsValid() == false || ZNV.GetZDO() == null || ZNV.GetZDO().IsValid() == false) return false;
            return true;
        }

        public static ZDO GetZDO(ZDOID zdoid)
        {
            if (ValidateZDOID(zdoid) == false)
            {
                return null;
            }
            return ZDOMan.instance.GetZDO(zdoid);
        }

        public static string RandomName()
        {
            var firstNames = new[] { "Kuku", "Hoezae", "Jeff", "Klint", "Druftes", "Fver", "Qanzop", "Lufty", "Khaku", "Dodo" };
            var lastNames = new[] { "Deb", "Barma", "Hoster", "Ward", "Bap", "Cergate","Jamatiya", "Astovert", "JoeMama","Lund" };
            string name = firstNames[UnityEngine.Random.Range(0, firstNames.Length - 1)] + " " + lastNames[UnityEngine.Random.Range(0, lastNames.Length - 1)];
            return name;
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
}

/*
 * 
 * TODO:
 * Ability to change villager's name ~ DONE
 * Comand GUI for villager commander ~ DONE
 * UPGRADE WORKING :
 * Upgrade items for dmage and working :
 * CONFIG FOR SPAWNS    and multipliers of upgrade etc etc
 * NOT WORKING VILLAGERS FIX
 * 
 * 
 * Huge overhaul incomming
 * Villagers are now going to be personal. They will have traits which is going to get better as you level them up. And if they die, they will no longer respawn.
 * Beds no longer spawn villagers instead you will have to find villagers in the wild and bring them with you and assign them a bed.
 * Villagers will have these traits which are going to be divided into section:
 * 
 * Wild villagers can be found all over valheim. Their ZDO will not be persistent.
 * And each villager whose ZDO is not persistent will have random stats.
 * When tamed, they will have persistent ZDO and their stats will be saved in ZDO. They will also be marked in map.
 * They will have no beds assigned by default. Which is valid.
 * You will be using FollowerFruit to make them follow you. 
 * You can then bring them to your base and craft them a bed. 
 * Interact with the bed first and then with the villager to assign them the bed.
 * Protect the bed at any cost as it's where every crucial data is going to be stored.
 * 
 * 
 * 
 * 
 * 1st section (General)
 * 1. Name (Can be changed)
 * 2. Health/MaxHealth (can be upgraded till cap) health updrage = (currenthealth + efficiency/2)
 * 4. Advancement : Weak, Bronze, Iron, Silver, Black metal. Decides the cap for farming. Damages should not be capped but is super slow to upgrade
 * 3. Efficiency (scale 1 to 10 ) (NOT UPGRADABLE) Decides how much a villager should get better at damage and farming
 * 
 * 2nd section (Strength)
 * 1. Listing all damage it does
 * slash = 0f;
 * blunt = 0f;
 * chop = 0f;
 * fire = 0f;
 * frost = 0f;
 * lightning = 0f;
 * pickaxe = 0f;
 * pierce = 0f;
 * poison = 0f;
 * slash = 0f;
 * 
 * The modifiers are age, efficiency
 * Upgrading them is going to be = (currentDmg + efficiency ) * age
 * These damages are to be normalised and scaled based on efficiency. Initially villagers in wilderness needs to have random stats. Good stats but less efficiency & Vice versa
 * RARELY YOU WILL FIND ELITE Villagers with good efficiency and stats both
 * 
 * Leveling up:
 * Villagers can improve till a cap. This cap is going to be huge. 
 * Players can manually upgrade the villagers or by villagers can upgrade in 25% of manual upgrade when working and fighting.
 * Working will improve their Chop & Pickaxe skill BUT will be capped until you give them items to progress to the next age
 * 
 * Special damage:
 * Villagers can have special dmg such as fire, poison, lightning
 */

/*
 * Technical overhaul.
 * 1. Make common functions static to avoid conflicts such as GetContainer() GetBed() GetWorkpost()
 * 2. Normalise damages so that we can apply modifiers nicely
 */

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