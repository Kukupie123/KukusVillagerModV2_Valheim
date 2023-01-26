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
using System.Reflection;
using UnityEngine;
using Jotunn.Utils;
using System;
using Jotunn.Configs;
using System.Collections.Generic;

namespace KukusVillagerMod
{
    [BepInDependency(Jotunn.Main.ModGuid)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    internal class KukusVillagerMod : BaseUnityPlugin
    {
        public static bool isModded = true;
        public const string PluginGUID = "com.kukukodes.KukuVillagers";
        public const string PluginName = "KukusVillagerMod";
        public const string PluginVersion = "3.1.3";
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();
        private VillagerCommander vc;
        public static bool isMapDataLoaded = false;

        private readonly Harmony harmony = new Harmony("kukuvillager");

        public static AssetBundle NPCBundle;

        private void Awake()
        {
            VillagerModConfigurations.LoadConfig(Config);
            LoadBundle();
            LoadAssets();
            AddNamedNPC("HumanNPCBob_DoD");
            AddNamedNPC("HumanNPCFred_DoD");
            AddNamedNPC("HumanNPCBarry_DoD");
            AddNamedNPC("HumanNPCBobby_DoD");
            AddNamedNPC("HumanNPCJeff_DoD");
            AddNamedNPC("HumanNPCMandy_DoD");
            AddNamedNPC("HumanNPCBarbara_DoD");
            AddNamedNPC("HumanNPCSandra_DoD");
            AddNamedNPC("HumanNPCDaisy_DoD");
            AddNamedNPC("HumanNPCCathrine_DoD");
            AddNamedNPC("HumanNPCKaren_DoD");
            AddNamedNPC("HumanNPCFletch_DoD");
            //Doesn't work with our villager
            AddNamedMageNPC("HumanNPCGary_DoD");
            AddNamedMageNPC("HumanNPCTania_DoD");
            AddNamedMageNPC("HumanNPCTina_DoD");
            //Create villager prefab using horem's NPC
            new VillagerPrefab();
            //CreatureManager.OnVanillaCreaturesAvailable += CloneSpawnPoint;
            //CloneSpawnPoint(); //Clones and modifies each spawn point as needed
            PrefabManager.OnVanillaPrefabsAvailable += LoadPiecesPrefab; //load original prefabs cloned from original game
            //ZoneManager.OnVanillaLocationsAvailable += AddAllSpawnPointForVillager; //add spawn points to the game
            harmony.PatchAll();

        }
        private void FixedUpdate()
        {
            if (vc != null)
            {
                vc.HandleInputs();
            }
        }

        private void LoadBundle()
        {
            try
            {
                Logger.LogMessage("Loading bundle, Thanks to Horem for providing the assets.");
                NPCBundle = AssetUtils.LoadAssetBundleFromResources("companions", Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Exception caught while loading Companions asset bundle: {ex}");
            }
        }
        private void CloneSpawner()
        {
            KLog.info("Cloning Horem's Original Spawner for villager");
            //Villagers prefab
            var Villager_Meadow1 = PrefabManager.Instance.GetPrefab("Villager_Meadow1");
            var Villager_Meadow2 = PrefabManager.Instance.GetPrefab("Villager_Meadow2");
            var Villager_BF1 = PrefabManager.Instance.GetPrefab("Villager_BF1");
            var Villager_BF2 = PrefabManager.Instance.GetPrefab("Villager_BF2");
            var Villager_Mountain1 = PrefabManager.Instance.GetPrefab("Villager_Mountain1");
            var Villager_Mountain2 = PrefabManager.Instance.GetPrefab("Villager_Mountain2");
            var Villager_Plains1 = PrefabManager.Instance.GetPrefab("Villager_Plains1");
            var Villager_Plains2 = PrefabManager.Instance.GetPrefab("Villager_Plains2");
            var Villager_Plains3 = PrefabManager.Instance.GetPrefab("Villager_Plains3");
            var Villager_Mist1 = PrefabManager.Instance.GetPrefab("Villager_Mist1");
            var Villager_Mist2 = PrefabManager.Instance.GetPrefab("Villager_Mist2");
            var Villager_Mist3 = PrefabManager.Instance.GetPrefab("Villager_Mist3");

            var originalSpawnPoint = PrefabManager.Instance.GetPrefab("Spawner_NPCCamp_DoD");
            if (originalSpawnPoint == null)
            {
                KLog.warning($"Original spawner by horem failed to load");
                return;
            }

            var cloningSpawnPoint = PrefabManager.Instance.CreateClonedPrefab("KukuVillager_Spawner_Meadow", originalSpawnPoint);
            var spawnArea = cloningSpawnPoint.GetComponentInChildren<SpawnArea>();
            var hoverText = cloningSpawnPoint.GetComponentInChildren<HoverText>();
            hoverText.m_text = "Meadow Residence";
            spawnArea.m_spawnIntervalSec = VillagerModConfigurations.SpawnPoint_SpawnIntervalSec;
            spawnArea.m_maxNear = VillagerModConfigurations.SpawnPoint_MaxNear;
            spawnArea.m_maxTotal = VillagerModConfigurations.SpawnPoint_MaxTotal;
            List<SpawnArea.SpawnData> spawnCreatures = new List<SpawnArea.SpawnData>();
            foreach (var g in new List<GameObject> { Villager_Meadow1, Villager_Meadow2 })
            {
                var spawnData = new SpawnArea.SpawnData
                {
                    m_prefab = g,
                    m_maxLevel = 1,
                    m_minLevel = 1,
                    m_weight = 0.5f
                };

                spawnCreatures.Add(spawnData);
            }
            spawnArea.m_prefabs = spawnCreatures;
            PrefabManager.Instance.AddPrefab(cloningSpawnPoint);

            cloningSpawnPoint = PrefabManager.Instance.CreateClonedPrefab("KukuVillager_Spawner_BF", originalSpawnPoint);
            hoverText = cloningSpawnPoint.GetComponentInChildren<HoverText>();
            hoverText.m_text = "Blackforest Residence";
            spawnArea = cloningSpawnPoint.GetComponentInChildren<SpawnArea>();
            spawnArea.m_spawnIntervalSec = VillagerModConfigurations.SpawnPoint_SpawnIntervalSec;
            spawnArea.m_maxNear = VillagerModConfigurations.SpawnPoint_MaxNear;
            spawnArea.m_maxTotal = VillagerModConfigurations.SpawnPoint_MaxTotal;
            spawnCreatures = new List<SpawnArea.SpawnData>();
            foreach (var g in new List<GameObject> { Villager_BF1, Villager_BF2 })
            {
                var spawnData = new SpawnArea.SpawnData
                {
                    m_prefab = g,
                    m_maxLevel = 1,
                    m_minLevel = 1,
                    m_weight = 0.5f
                };

                spawnCreatures.Add(spawnData);
            }
            spawnArea.m_prefabs = spawnCreatures;
            PrefabManager.Instance.AddPrefab(cloningSpawnPoint);

            cloningSpawnPoint = PrefabManager.Instance.CreateClonedPrefab("KukuVillager_Spawner_Mountain", originalSpawnPoint);
            hoverText = cloningSpawnPoint.GetComponentInChildren<HoverText>();
            hoverText.m_text = "Mountain Base Point";
            spawnArea = cloningSpawnPoint.GetComponentInChildren<SpawnArea>();
            spawnArea.m_spawnIntervalSec = VillagerModConfigurations.SpawnPoint_SpawnIntervalSec;
            spawnArea.m_maxNear = VillagerModConfigurations.SpawnPoint_MaxNear;
            spawnArea.m_maxTotal = VillagerModConfigurations.SpawnPoint_MaxTotal;
            spawnCreatures = new List<SpawnArea.SpawnData>();
            foreach (var g in new List<GameObject> { Villager_Mountain1, Villager_Mountain2 })
            {
                var spawnData = new SpawnArea.SpawnData
                {
                    m_prefab = g,
                    m_maxLevel = 1,
                    m_minLevel = 1,
                    m_weight = 0.5f
                };

                spawnCreatures.Add(spawnData);
            }
            spawnArea.m_prefabs = spawnCreatures;
            PrefabManager.Instance.AddPrefab(cloningSpawnPoint);

            cloningSpawnPoint = PrefabManager.Instance.CreateClonedPrefab("KukuVillager_Spawner_Plains", originalSpawnPoint);
            hoverText = cloningSpawnPoint.GetComponentInChildren<HoverText>();
            hoverText.m_text = "Plains Survivor Spot";
            spawnArea = cloningSpawnPoint.GetComponentInChildren<SpawnArea>();
            spawnArea.m_spawnIntervalSec = VillagerModConfigurations.SpawnPoint_SpawnIntervalSec;
            spawnArea.m_maxNear = VillagerModConfigurations.SpawnPoint_MaxNear;
            spawnArea.m_maxTotal = VillagerModConfigurations.SpawnPoint_MaxTotal;
            spawnCreatures = new List<SpawnArea.SpawnData>();
            foreach (var g in new List<GameObject> { Villager_Plains1, Villager_Plains2, Villager_Plains3 })
            {
                var spawnData = new SpawnArea.SpawnData
                {
                    m_prefab = g,
                    m_maxLevel = 1,
                    m_minLevel = 1,
                    m_weight = 0.5f
                };

                spawnCreatures.Add(spawnData);
            }
            spawnArea.m_prefabs = spawnCreatures;
            PrefabManager.Instance.AddPrefab(cloningSpawnPoint);

            cloningSpawnPoint = PrefabManager.Instance.CreateClonedPrefab("KukuVillager_Spawner_MistLand", originalSpawnPoint);
            hoverText = cloningSpawnPoint.GetComponentInChildren<HoverText>();
            hoverText.m_text = "Mistland Warrior Den";
            spawnArea = cloningSpawnPoint.GetComponentInChildren<SpawnArea>();
            spawnArea.m_spawnIntervalSec = VillagerModConfigurations.SpawnPoint_SpawnIntervalSec;
            spawnArea.m_maxNear = VillagerModConfigurations.SpawnPoint_MaxNear;
            spawnArea.m_maxTotal = VillagerModConfigurations.SpawnPoint_MaxTotal;
            spawnCreatures = new List<SpawnArea.SpawnData>();
            foreach (var g in new List<GameObject> { Villager_Mist1, Villager_Mist2, Villager_Mist3 })
            {
                var spawnData = new SpawnArea.SpawnData
                {
                    m_prefab = g,
                    m_maxLevel = 1,
                    m_minLevel = 1,
                    m_weight = 0.5f
                };

                spawnCreatures.Add(spawnData);
            }
            spawnArea.m_prefabs = spawnCreatures;
            PrefabManager.Instance.AddPrefab(cloningSpawnPoint);
        }

        private void CloneSpawnPoint()
        {

            KLog.info("Cloning Spawn point from Horem for villager's HUT");
            //Villagers prefab
            var Villager_Meadow1 = CreatureManager.Instance.GetCreaturePrefab("Villager_Meadow1");
            var Villager_Meadow2 = CreatureManager.Instance.GetCreaturePrefab("Villager_Meadow2");
            var Villager_BF1 = CreatureManager.Instance.GetCreaturePrefab("Villager_BF1");
            var Villager_BF2 = CreatureManager.Instance.GetCreaturePrefab("Villager_BF2");
            var Villager_Mountain1 = CreatureManager.Instance.GetCreaturePrefab("Villager_Mountain1");
            var Villager_Mountain2 = CreatureManager.Instance.GetCreaturePrefab("Villager_Mountain2");
            var Villager_Plains1 = CreatureManager.Instance.GetCreaturePrefab("Villager_Plains1");
            var Villager_Plains2 = CreatureManager.Instance.GetCreaturePrefab("Villager_Plains2");
            var Villager_Plains3 = CreatureManager.Instance.GetCreaturePrefab("Villager_Plains3");
            var Villager_Mist1 = CreatureManager.Instance.GetCreaturePrefab("Villager_Mist1");
            var Villager_Mist2 = CreatureManager.Instance.GetCreaturePrefab("Villager_Mist2");
            var Villager_Mist3 = CreatureManager.Instance.GetCreaturePrefab("Villager_Mist3");
            if (Villager_Mist3 == null)
            {
                KLog.warning("Villagers are not yet registered and loaded aborting clone spawn point");
                return;
            }

            //Original SpawnPoint
            var originalSpawnPoint = PrefabManager.Instance.GetPrefab("Loc_NPCCamp_DoD");
            if (originalSpawnPoint == null)
            {
                KLog.warning($"Original spawn point by horem is null");
                return;
            }

            //Clone original 
            var clonedSpawnPointMeadow = PrefabManager.Instance.CreateClonedPrefab("KukuVillager_SpawnPoint_M", originalSpawnPoint);

            //Get SpawnerArea of cloned Spawn Point
            SpawnArea spawnerMeadow = clonedSpawnPointMeadow.GetComponentInChildren<SpawnArea>();
            HoverText hoverMeadow = clonedSpawnPointMeadow.GetComponentInChildren<HoverText>();
            hoverMeadow.m_text = "Meadow Residence";
            if (spawnerMeadow == null)
            {
                KLog.warning("Spawner is null for cloned spawn point");
                return;
            }

            //Modify spawnArea
            spawnerMeadow.m_spawnIntervalSec = VillagerModConfigurations.SpawnPoint_SpawnIntervalSec;
            spawnerMeadow.m_maxNear = VillagerModConfigurations.SpawnPoint_MaxNear;
            spawnerMeadow.m_maxTotal = VillagerModConfigurations.SpawnPoint_MaxTotal;
            List<SpawnArea.SpawnData> spawnerCreaturesMeadow = new List<SpawnArea.SpawnData>();
            foreach (var c in new GameObject[] { Villager_Meadow1, Villager_Meadow2 })
            {
                var spawnData = new SpawnArea.SpawnData
                {
                    m_prefab = c,
                    m_maxLevel = 1,
                    m_minLevel = 1,
                    m_weight = 0.5f
                };
                spawnerCreaturesMeadow.Add(spawnData);
            }
            spawnerMeadow.m_prefabs = spawnerCreaturesMeadow;
            PrefabManager.Instance.AddPrefab(clonedSpawnPointMeadow);



            //Clone original 
            var clonedSpawnPointBF = PrefabManager.Instance.CreateClonedPrefab("KukuVillager_SpawnPoint_BLFR", originalSpawnPoint);

            //Get SpawnerArea of cloned Spawn Point
            SpawnArea spawnerBF = clonedSpawnPointBF.GetComponentInChildren<SpawnArea>();
            HoverText hoverBF = clonedSpawnPointBF.GetComponentInChildren<HoverText>();
            hoverBF.m_text = "BlackForest Residence";
            if (spawnerBF == null)
            {
                KLog.warning("Spawner is null for cloned spawn point BF");
                return;
            }

            //Modify spawnArea
            spawnerBF.m_spawnIntervalSec = VillagerModConfigurations.SpawnPoint_SpawnIntervalSec;
            spawnerBF.m_maxNear = VillagerModConfigurations.SpawnPoint_MaxNear;
            spawnerBF.m_maxTotal = VillagerModConfigurations.SpawnPoint_MaxTotal;
            List<SpawnArea.SpawnData> spawnerCreaturesBF = new List<SpawnArea.SpawnData>();
            foreach (var c in new GameObject[] { Villager_BF1, Villager_BF2 })
            {
                var spawnData = new SpawnArea.SpawnData
                {
                    m_prefab = c,
                    m_maxLevel = 1,
                    m_minLevel = 1,
                    m_weight = 0.5f
                };
                spawnerCreaturesBF.Add(spawnData);
            }
            spawnerBF.m_prefabs = spawnerCreaturesBF;
            PrefabManager.Instance.AddPrefab(clonedSpawnPointBF);





            //Clone original 
            var clonedSpawnPointMountain = PrefabManager.Instance.CreateClonedPrefab("KukuVillager_SpawnPoint_Mtn", originalSpawnPoint);

            //Get SpawnerArea of cloned Spawn Point
            SpawnArea spawnerMountain = clonedSpawnPointMountain.GetComponentInChildren<SpawnArea>();
            HoverText hoverMountain = clonedSpawnPointMountain.GetComponentInChildren<HoverText>();
            hoverMountain.m_text = "Mountain Defender Base";
            if (spawnerMountain == null)
            {
                KLog.warning("Spawner is null for cloned spawn point mountain");
                return;
            }

            //Modify spawnArea
            spawnerMountain.m_spawnIntervalSec = VillagerModConfigurations.SpawnPoint_SpawnIntervalSec;
            spawnerMountain.m_maxNear = VillagerModConfigurations.SpawnPoint_MaxNear;
            spawnerMountain.m_maxTotal = VillagerModConfigurations.SpawnPoint_MaxTotal;
            List<SpawnArea.SpawnData> spawnerCreaturesMountain = new List<SpawnArea.SpawnData>();
            foreach (var c in new GameObject[] { Villager_Mountain1, Villager_Mountain2 })
            {
                var spawnData = new SpawnArea.SpawnData
                {
                    m_prefab = c,
                    m_maxLevel = 1,
                    m_minLevel = 1,
                    m_weight = 0.5f
                };
                spawnerCreaturesMountain.Add(spawnData);
            }
            spawnerMountain.m_prefabs = spawnerCreaturesMountain;
            PrefabManager.Instance.AddPrefab(clonedSpawnPointMountain);



            //Clone original 
            var clonedSpawnPointPlains = PrefabManager.Instance.CreateClonedPrefab("KukuVillager_SpawnPoint_Pln", originalSpawnPoint);

            //Get SpawnerArea of cloned Spawn Point
            SpawnArea spawnerPlains = clonedSpawnPointPlains.GetComponentInChildren<SpawnArea>();
            HoverText hoverPlains = clonedSpawnPointPlains.GetComponentInChildren<HoverText>();
            hoverPlains.m_text = "Plains Survivor Den";
            if (spawnerPlains == null)
            {
                KLog.warning("Spawner is null for cloned spawn point Plains");
                return;
            }

            //Modify spawnArea
            spawnerPlains.m_spawnIntervalSec = VillagerModConfigurations.SpawnPoint_SpawnIntervalSec;
            spawnerPlains.m_maxNear = VillagerModConfigurations.SpawnPoint_MaxNear;
            spawnerPlains.m_maxTotal = VillagerModConfigurations.SpawnPoint_MaxTotal;
            List<SpawnArea.SpawnData> spawnerCreaturesPlains = new List<SpawnArea.SpawnData>();
            foreach (var c in new GameObject[] { Villager_Plains1, Villager_Plains2, Villager_Plains3 })
            {
                var spawnData = new SpawnArea.SpawnData
                {
                    m_prefab = c,
                    m_maxLevel = 1,
                    m_minLevel = 1,
                    m_weight = 0.5f
                };
                spawnerCreaturesPlains.Add(spawnData);
            }
            spawnerPlains.m_prefabs = spawnerCreaturesPlains;
            PrefabManager.Instance.AddPrefab(clonedSpawnPointPlains);





            //Clone original 
            var clonedSpawnPointML = PrefabManager.Instance.CreateClonedPrefab("KukuVillager_SpawnPoint_MLL", originalSpawnPoint);

            //Get SpawnerArea of cloned Spawn Point
            SpawnArea spawnerML = clonedSpawnPointML.GetComponentInChildren<SpawnArea>();
            HoverText hoverML = clonedSpawnPointML.GetComponentInChildren<HoverText>();
            hoverML.m_text = "Mistland Fighter Den";
            if (spawnerML == null)
            {
                KLog.warning("Spawner is null for cloned spawn point ML");
                return;
            }

            //Modify spawnArea
            spawnerML.m_spawnIntervalSec = VillagerModConfigurations.SpawnPoint_SpawnIntervalSec;
            spawnerML.m_maxNear = VillagerModConfigurations.SpawnPoint_MaxNear;
            spawnerML.m_maxTotal = VillagerModConfigurations.SpawnPoint_MaxTotal;
            List<SpawnArea.SpawnData> spawnerCreaturesML = new List<SpawnArea.SpawnData>();
            foreach (var c in new GameObject[] { Villager_Mist1, Villager_Mist2, Villager_Mist3 })
            {
                var spawnData = new SpawnArea.SpawnData
                {
                    m_prefab = c,
                    m_maxLevel = 1,
                    m_minLevel = 1,
                    m_weight = 0.5f
                };
                spawnerCreaturesML.Add(spawnData);
            }
            spawnerML.m_prefabs = spawnerCreaturesML;
            PrefabManager.Instance.AddPrefab(clonedSpawnPointML);

            KLog.warning("Cloned Villager Spawn point successfully");
            CreatureManager.OnVanillaCreaturesAvailable -= CloneSpawnPoint;

        }
        private void AddAllSpawnPointForVillager()
        {
            AddSpawnPointToWorld("KukuVillager_SpawnPoint_M", Heightmap.Biome.Meadows);
            AddSpawnPointToWorld("KukuVillager_SpawnPoint_BLFR", Heightmap.Biome.BlackForest);
            AddSpawnPointToWorld("KukuVillager_SpawnPoint_Mtn", Heightmap.Biome.Mountain);
            AddSpawnPointToWorld("KukuVillager_SpawnPoint_Pln", Heightmap.Biome.Plains);
            AddSpawnPointToWorld("KukuVillager_SpawnPoint_MLL", Heightmap.Biome.Mistlands);
            ZoneManager.OnVanillaLocationsAvailable -= AddAllSpawnPointForVillager;
        }
        private void AddSpawnPointToWorld(string spawnPointName, Heightmap.Biome biome)
        {
            try
            {
                var spawnPoint = PrefabManager.Instance.GetPrefab(spawnPointName);
                if (spawnPoint == null)
                {
                    KLog.warning($"Failed to find spawn point {spawnPointName} Prefab");
                    return;
                }
                var camp = ZoneManager.Instance.CreateLocationContainer(spawnPoint);
                if (camp == null)
                {
                    KLog.warning($"Failed to create spawn point {spawnPointName} container");
                    return;
                }
                //camp.GetComponentInChildren<AudioSource>().outputAudioMixerGroup = AudioMan.instance.m_ambientMixer;
                if (ZoneManager.Instance.AddCustomLocation(new CustomLocation(camp, true, new LocationConfig
                {
                    Biome = biome,
                    Quantity = VillagerModConfigurations.HutSpawnQuantity,
                    Priotized = true,
                    ExteriorRadius = 32f,
                    ClearArea = true,
                    MinDistanceFromSimilar = 1000f
                })))
                    Logger.LogMessage($"Villager hut Spawn point added {spawnPointName}");
                else Logger.LogMessage($"Villager hut Spawn point NOT added {spawnPointName}");
            }
            catch (Exception e)
            {
                KLog.warning("Exception when placing Spawn Point for villager \n" + e.Message + e.StackTrace);
            }
        }

        private void LoadAssets()
        {
            try
            {
                // Status Effects
                SE_Stats so1 = NPCBundle.LoadAsset<SE_Stats>("SE_NPC_GreaterHeal_DoD");
                if (so1 != null)
                {
                    Logger.LogMessage("Adding Status Effects");
                    CustomStatusEffect customSE1 = new CustomStatusEffect(so1, true);
                    ItemManager.Instance.AddStatusEffect(customSE1);

                    SE_Stats so2 = NPCBundle.LoadAsset<SE_Stats>("SE_NPC_Heal_DoD");
                    CustomStatusEffect customSE2 = new CustomStatusEffect(so2, true);
                    ItemManager.Instance.AddStatusEffect(customSE2);

                    SE_Stats so3 = NPCBundle.LoadAsset<SE_Stats>("SE_NPC_LesserHeal_DoD");
                    CustomStatusEffect customSE3 = new CustomStatusEffect(so3, true);
                    ItemManager.Instance.AddStatusEffect(customSE3);

                    SE_Shield so4 = NPCBundle.LoadAsset<SE_Shield>("SE_NPC_Shield_DoD");
                    CustomStatusEffect customSE4 = new CustomStatusEffect(so4, true);
                    ItemManager.Instance.AddStatusEffect(customSE4);

                    SE_Shield so5 = NPCBundle.LoadAsset<SE_Shield>("SE_NPC_ShieldGreater_DoD");
                    CustomStatusEffect customSE5 = new CustomStatusEffect(so5, true);
                    ItemManager.Instance.AddStatusEffect(customSE5);

                    SE_Shield so6 = NPCBundle.LoadAsset<SE_Shield>("SE_NPC_ShieldLesser_DoD");
                    CustomStatusEffect customSE6 = new CustomStatusEffect(so6, true);
                    ItemManager.Instance.AddStatusEffect(customSE6);
                }
                // Shields
                var shieldcheck = PrefabManager.Instance.GetPrefab("NPC_Shield_DoD");
                if (shieldcheck != null)
                {
                    Logger.LogMessage("Shield Spells already added");
                }
                else
                {
                    Logger.LogMessage("Adding Shields");
                    GameObject shielditem1 = NPCBundle.LoadAsset<GameObject>("NPC_Shield_DoD");
                    if (shielditem1 != null)
                    {
                        CustomItem customItem1 = new CustomItem(shielditem1, true);
                        ItemManager.Instance.AddItem(customItem1);

                        GameObject shielditem2 = NPCBundle.LoadAsset<GameObject>("NPC_ShieldLesser_DoD");
                        CustomItem customItem2 = new CustomItem(shielditem2, true);
                        ItemManager.Instance.AddItem(customItem2);

                        GameObject shielditem3 = NPCBundle.LoadAsset<GameObject>("NPC_ShieldGreater_DoD");
                        CustomItem customItem3 = new CustomItem(shielditem3, true);
                        ItemManager.Instance.AddItem(customItem3);
                    }
                }
                // Heals
                var healcheck = PrefabManager.Instance.GetPrefab("NPC_SelfHeal_DoD");
                if (healcheck != null)
                {
                    Logger.LogMessage("Heal Spells already added");
                }
                else
                {
                    Logger.LogMessage("Adding Heals");
                    GameObject healitem1 = NPCBundle.LoadAsset<GameObject>("NPC_SelfHeal_DoD");
                    if (healitem1 != null)
                    {
                        CustomItem customItem1 = new CustomItem(healitem1, true);
                        ItemManager.Instance.AddItem(customItem1);

                        GameObject healitem2 = NPCBundle.LoadAsset<GameObject>("NPC_SelfHealLesser_DoD");
                        CustomItem customItem2 = new CustomItem(healitem2, true);
                        ItemManager.Instance.AddItem(customItem2);

                        GameObject healitem3 = NPCBundle.LoadAsset<GameObject>("NPC_SelfHealGreater_DoD");
                        CustomItem customItem3 = new CustomItem(healitem3, true);
                        ItemManager.Instance.AddItem(customItem3);
                    }
                }
                // AoE
                var aoecheck = PrefabManager.Instance.GetPrefab("NPC_Heal_AoE_DoD");
                if (aoecheck != null)
                {
                    Logger.LogMessage("AoE Spells already added");
                }
                else
                {
                    Logger.LogMessage("Adding AoE");
                    GameObject aoeitem1 = NPCBundle.LoadAsset<GameObject>("NPC_Heal_AoE_DoD");
                    if (aoeitem1 != null)
                    {
                        CustomPrefab customPrefab1 = new CustomPrefab(aoeitem1, true);
                        PrefabManager.Instance.AddPrefab(customPrefab1);

                        GameObject aoeitem2 = NPCBundle.LoadAsset<GameObject>("NPC_HealLesser_AoE_DoD");
                        CustomPrefab customPrefab2 = new CustomPrefab(aoeitem2, true);
                        PrefabManager.Instance.AddPrefab(customPrefab2);

                        GameObject aoeitem3 = NPCBundle.LoadAsset<GameObject>("NPC_HealGreater_AoE_DoD");
                        CustomPrefab customPrefab3 = new CustomPrefab(aoeitem3, true);
                        PrefabManager.Instance.AddPrefab(customPrefab3);

                        GameObject aoeitem4 = NPCBundle.LoadAsset<GameObject>("NPC_Shield_AoE_DoD");
                        CustomPrefab customPrefab4 = new CustomPrefab(aoeitem4, true);
                        PrefabManager.Instance.AddPrefab(customPrefab4);
                    }
                }
                // Attacks
                var itemcheck = PrefabManager.Instance.GetPrefab("NPC_KickAttack_L_DoD");
                if (itemcheck != null)
                {
                    Logger.LogMessage("Abilities already added");
                }
                else
                {
                    Logger.LogMessage("Adding Abilities");
                    GameObject monsteritem1 = NPCBundle.LoadAsset<GameObject>("NPC_KickAttack_L_DoD");
                    if (monsteritem1 != null)
                    {
                        CustomItem customItem1 = new CustomItem(monsteritem1, true);
                        ItemManager.Instance.AddItem(customItem1);

                        GameObject monsteritem2 = NPCBundle.LoadAsset<GameObject>("NPC_KickAttack_R_DoD");
                        CustomItem customItem2 = new CustomItem(monsteritem2, true);
                        ItemManager.Instance.AddItem(customItem2);


                        GameObject monsteritem3 = NPCBundle.LoadAsset<GameObject>("NPC_KoshAttack_L_DoD");
                        CustomItem customItem3 = new CustomItem(monsteritem3, true);
                        ItemManager.Instance.AddItem(customItem3);



                        GameObject monsteritem4 = NPCBundle.LoadAsset<GameObject>("NPC_KoshAttack_R_DoD");
                        CustomItem customItem4 = new CustomItem(monsteritem4, true);
                        ItemManager.Instance.AddItem(customItem4);


                        GameObject monsteritem5 = NPCBundle.LoadAsset<GameObject>("NPC_KnifeAttack_L_DoD");
                        CustomItem customItem5 = new CustomItem(monsteritem5, true);
                        ItemManager.Instance.AddItem(customItem5);

                        GameObject monsteritem6 = NPCBundle.LoadAsset<GameObject>("NPC_KnifeAttack_R_DoD");
                        CustomItem customItem6 = new CustomItem(monsteritem6, true);
                        ItemManager.Instance.AddItem(customItem6);

                        GameObject monsteritem7 = NPCBundle.LoadAsset<GameObject>("NPC_DaggerAttack_L_DoD");
                        CustomItem customItem7 = new CustomItem(monsteritem7, true);
                        ItemManager.Instance.AddItem(customItem7);

                        GameObject monsteritem8 = NPCBundle.LoadAsset<GameObject>("NPC_GladiatorSwordAttack_R_DoD");
                        CustomItem customItem8 = new CustomItem(monsteritem8, true);
                        ItemManager.Instance.AddItem(customItem8);

                        GameObject monsteritem9 = NPCBundle.LoadAsset<GameObject>("NPC_RedAxeAttack_R_DoD");
                        CustomItem customItem9 = new CustomItem(monsteritem9, true);
                        ItemManager.Instance.AddItem(customItem9);

                        GameObject monsteritem10 = NPCBundle.LoadAsset<GameObject>("NPC_VikingAxeAttack_L_DoD");
                        CustomItem customItem10 = new CustomItem(monsteritem10, true);
                        ItemManager.Instance.AddItem(customItem10);

                        GameObject monsteritem11 = NPCBundle.LoadAsset<GameObject>("NPC_VikingAxeAttack_R_DoD");
                        CustomItem customItem11 = new CustomItem(monsteritem11, true);
                        ItemManager.Instance.AddItem(customItem11);

                        GameObject monsteritem12 = NPCBundle.LoadAsset<GameObject>("NPC_WoodAxeAttack_L_DoD");
                        CustomItem customItem12 = new CustomItem(monsteritem12, true);
                        ItemManager.Instance.AddItem(customItem12);

                        GameObject monsteritem13 = NPCBundle.LoadAsset<GameObject>("NPC_DaggerAttack_R_DoD");
                        CustomItem customItem13 = new CustomItem(monsteritem13, true);
                        ItemManager.Instance.AddItem(customItem13);

                        GameObject monsteritem14 = NPCBundle.LoadAsset<GameObject>("NPC_GladiatorSwordAttack_L_DoD");
                        CustomItem customItem14 = new CustomItem(monsteritem14, true);
                        ItemManager.Instance.AddItem(customItem14);

                        GameObject monsteritem15 = NPCBundle.LoadAsset<GameObject>("NPC_GoldAxeAttack_L_DoD");
                        CustomItem customItem15 = new CustomItem(monsteritem15, true);
                        ItemManager.Instance.AddItem(customItem15);

                        GameObject monsteritem16 = NPCBundle.LoadAsset<GameObject>("NPC_GoldAxeAttack_R_DoD");
                        CustomItem customItem16 = new CustomItem(monsteritem16, true);
                        ItemManager.Instance.AddItem(customItem16);

                        GameObject monsteritem17 = NPCBundle.LoadAsset<GameObject>("NPC_BowAttack_DoD");
                        CustomItem customItem17 = new CustomItem(monsteritem17, true);
                        ItemManager.Instance.AddItem(customItem17);

                        GameObject monsteritem18 = NPCBundle.LoadAsset<GameObject>("NPC_MageDaggerAttack_L_DoD");
                        CustomItem customItem18 = new CustomItem(monsteritem18, true);
                        ItemManager.Instance.AddItem(customItem18);

                        GameObject monsteritem19 = NPCBundle.LoadAsset<GameObject>("NPC_StaffSledgeAttack_R_DoD");
                        CustomItem customItem19 = new CustomItem(monsteritem19, true);
                        ItemManager.Instance.AddItem(customItem19);
                    }
                    else
                    {
                        Logger.LogWarning("Abilities not found");
                    }
                }
                // Spawners
                var spawnercheck = PrefabManager.Instance.GetPrefab("Spawner_NPCCamp_DoDkuku");
                if (spawnercheck != null)
                {
                    //Logger.LogMessage("Spawners already added");
                }
                else
                {
                    Logger.LogMessage("Adding Spawners");
                    GameObject monsterspawner1 = NPCBundle.LoadAsset<GameObject>("Spawner_NPCCamp_DoDkuku");
                    if (monsterspawner1 != null)
                    {
                        CustomPrefab customSpawner1 = new CustomPrefab(monsterspawner1, true);
                        PrefabManager.Instance.AddPrefab(customSpawner1);
                    }
                    else
                    {
                        //Logger.LogWarning("Spawners not found");
                    }
                }

                var spawnPoint = PrefabManager.Instance.GetPrefab("Loc_NPCCamp_DoDkuku");
                if (spawnPoint != null)
                {
                    //Logger.LogMessage("Spawn Point is already added");
                }
                else
                {
                    Logger.LogMessage("Adding spawn point");
                    GameObject spawner = NPCBundle.LoadAsset<GameObject>("Loc_NPCCamp_DoD");
                    if (spawner != null)
                    {
                        CustomPrefab customSpawner = new CustomPrefab(spawner, true);
                        PrefabManager.Instance.AddPrefab(customSpawner);
                    }
                    else
                    {
                        //Logger.LogWarning("Spawn point asset not found");
                    }
                }
                // Ragdolls
                var rdcheck = PrefabManager.Instance.GetPrefab("HumanNPCBarry_RD_DoD");
                if (rdcheck != null)
                {
                    Logger.LogMessage("Ragdolls already added");
                }
                else
                {
                    Logger.LogMessage("Adding Ragdolls");
                    GameObject monsterrd1 = NPCBundle.LoadAsset<GameObject>("HumanNPCBarry_RD_DoD");
                    if (monsterrd1 != null)
                    {
                        CustomPrefab customRD1 = new CustomPrefab(monsterrd1, true);
                        PrefabManager.Instance.AddPrefab(customRD1);

                        GameObject monsterrd2 = NPCBundle.LoadAsset<GameObject>("HumanNPCBob_RD_DoD");
                        CustomPrefab customRD2 = new CustomPrefab(monsterrd2, true);
                        PrefabManager.Instance.AddPrefab(customRD2);

                        GameObject monsterrd3 = NPCBundle.LoadAsset<GameObject>("HumanNPCBobby_RD_DoD");
                        CustomPrefab customRD3 = new CustomPrefab(monsterrd3, true);
                        PrefabManager.Instance.AddPrefab(customRD3);

                        GameObject monsterrd4 = NPCBundle.LoadAsset<GameObject>("HumanNPCFred_RD_DoD");
                        CustomPrefab customRD4 = new CustomPrefab(monsterrd4, true);
                        PrefabManager.Instance.AddPrefab(customRD4);

                        GameObject monsterrd5 = NPCBundle.LoadAsset<GameObject>("HumanNPCJeff_RD_DoD");
                        CustomPrefab customRD5 = new CustomPrefab(monsterrd5, true);
                        PrefabManager.Instance.AddPrefab(customRD5);

                        GameObject monsterrd6 = NPCBundle.LoadAsset<GameObject>("HumanNPCFletch_RD_DoD");
                        CustomPrefab customRD6 = new CustomPrefab(monsterrd6, true);
                        PrefabManager.Instance.AddPrefab(customRD6);

                        GameObject monsterrd7 = NPCBundle.LoadAsset<GameObject>("HumanNPCBarbara_RD_DoD");
                        CustomPrefab customRD7 = new CustomPrefab(monsterrd7, true);
                        PrefabManager.Instance.AddPrefab(customRD7);

                        GameObject monsterrd8 = NPCBundle.LoadAsset<GameObject>("HumanNPCCathrine_RD_DoD");
                        CustomPrefab customRD8 = new CustomPrefab(monsterrd8, true);
                        PrefabManager.Instance.AddPrefab(customRD8);

                        GameObject monsterrd9 = NPCBundle.LoadAsset<GameObject>("HumanNPCDaisy_RD_DoD");
                        CustomPrefab customRD9 = new CustomPrefab(monsterrd9, true);
                        PrefabManager.Instance.AddPrefab(customRD9);

                        GameObject monsterrd10 = NPCBundle.LoadAsset<GameObject>("HumanNPCKaren_RD_DoD");
                        CustomPrefab customRD10 = new CustomPrefab(monsterrd10, true);
                        PrefabManager.Instance.AddPrefab(customRD10);

                        GameObject monsterrd11 = NPCBundle.LoadAsset<GameObject>("HumanNPCMandy_RD_DoD");
                        CustomPrefab customRD11 = new CustomPrefab(monsterrd11, true);
                        PrefabManager.Instance.AddPrefab(customRD11);

                        GameObject monsterrd12 = NPCBundle.LoadAsset<GameObject>("HumanNPCSandra_RD_DoD");
                        CustomPrefab customRD12 = new CustomPrefab(monsterrd12, true);
                        PrefabManager.Instance.AddPrefab(customRD12);

                        GameObject monsterrd13 = NPCBundle.LoadAsset<GameObject>("HumanNPCGary_RD_DoD");
                        CustomPrefab customRD13 = new CustomPrefab(monsterrd13, true);
                        PrefabManager.Instance.AddPrefab(customRD13);

                        GameObject monsterrd14 = NPCBundle.LoadAsset<GameObject>("HumanNPCTania_RD_DoD");
                        CustomPrefab customRD14 = new CustomPrefab(monsterrd14, true);
                        PrefabManager.Instance.AddPrefab(customRD14);

                        GameObject monsterrd15 = NPCBundle.LoadAsset<GameObject>("HumanNPCTina_RD_DoD");
                        CustomPrefab customRD15 = new CustomPrefab(monsterrd15, true);
                        PrefabManager.Instance.AddPrefab(customRD15);
                    }
                    else
                    {
                        Logger.LogWarning("Ragdolls not found");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Exception caught while adding assets for Companions: {ex}");
            }
        }
        private void AddNamedNPC(string name)
        {
            GameObject NPC = NPCBundle.LoadAsset<GameObject>(name);
            if (NPC != null)
            {
                var humanCompanion = new CustomCreature(NPC, true,
                    new CreatureConfig
                    {
                        DropConfigs = new[]
                        {
                                new DropConfig
                                {
                                    Item = "Coins",
                                    Chance = 100,
                                    MinAmount = 5,
                                    MaxAmount = 10,
                                    OnePerPlayer = false,
                                    LevelMultiplier = false
                                }
                        }
                    });

                CreatureManager.Instance.AddCreature(humanCompanion);
            }
            else
            {
                KLog.warning("Failed to load " + name);
            }
        }

        private void AddNamedMageNPC(string name)
        {
            GameObject NPC = NPCBundle.LoadAsset<GameObject>(name);
            if (NPC != null)
            {
                var garyMob = new CustomCreature(NPC, true,
                    new CreatureConfig
                    {
                        DropConfigs = new[]
                        {
                                new DropConfig
                                {
                                    Item = "Coins",
                                    Chance = 100,
                                    MinAmount = 5,
                                    MaxAmount = 10,
                                    OnePerPlayer = false,
                                    LevelMultiplier = false
                                }
                        }
                    });

                CreatureManager.Instance.AddCreature(garyMob);
            }
            else
            {
                KLog.warning($"Failed to load Mage NPC {name}");
            }

        }
        private void LoadPiecesPrefab()
        {
            //Register Piece and Item
            new BedPrefab();
            new DefensePostPrefab();
            new WorkPostPrefab();
            new IndividualVillagerCommandItemPrefab();
            vc = new VillagerCommander();
            PrefabManager.OnVanillaPrefabsAvailable -= LoadPiecesPrefab;
        }

        private void OnVanillaCreaturesAvailable()
        {
            new VillagerPrefab();
            //CloneSpawner(); //We need to wait for villagers to laod in first
            //CloneSpawnPoint(); //Clone spawnpoints with custom villagers
            CreatureManager.OnVanillaCreaturesAvailable -= OnVanillaCreaturesAvailable;
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
            var firstNames = new[] { "Kuku", "Hoezae", "Jeff", "Klint", "Druftes", "Fver", "Qanzop", "Lufty", "Khaku", "Dodo", "DisNuts", "Horneice" };
            var lastNames = new[] { "Deb", "Barma", "Hoster", "Ward", "Bap", "Cergate", "Jamatiya", "Astovert", "JoeMama", "Lund", "AyAyRon" };
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
 * Major features list todo:
 * Recruit Villagers using Gold
 * Villagers help with fueling
 * Find better way to move villagers. they try to go directly to path (Dynamically decreasing await time only when staring at player or not moving, trying to use moveTo, moveAvoid, follow etc. or see follow me and try to write your own version of it)
 * Fixing base
 * Mining rock
 * using default prefab for villagers
 * modifier for upgrades in config
 * Mainataining villagers eg rest and energy
 */