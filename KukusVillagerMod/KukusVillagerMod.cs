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

namespace KukusVillagerMod
{
    [BepInDependency(Jotunn.Main.ModGuid)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    //[BepInDependency("horemvore.Companions",BepInDependency.DependencyFlags.HardDependency)]
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class KukusVillagerMod : BaseUnityPlugin
    {
        public static bool isModded = true;
        public const string PluginGUID = "com.kukukodes.KukuVillagers";
        public const string PluginName = "KukusVillagerMod";
        public const string PluginVersion = "3.1.0";
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();
        private VillagerCommander vc;
        public static bool isMapDataLoaded = false;

        private readonly Harmony harmony = new Harmony("kukuvillager");

        public static AssetBundle NPCBundle;

        private void Awake()
        {
            LoadBundle();
            LoadAssets();
            AddNamedNPC();
            AddNamedMageNPC();
            VillagerModConfigurations.LoadConfig(Config);
            PrefabManager.OnVanillaPrefabsAvailable += LoadPiecesPrefab;
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

        //Provided by Horem, I take no credit
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
                var spawnercheck = PrefabManager.Instance.GetPrefab("Spawner_NPCCamp_DoD");
                if (spawnercheck != null)
                {
                    Logger.LogMessage("Spawners already added");
                }
                else
                {
                    Logger.LogMessage("Adding Spawners");
                    GameObject monsterspawner1 = NPCBundle.LoadAsset<GameObject>("Spawner_NPCCamp_DoD");
                    if (monsterspawner1 != null)
                    {
                        CustomPrefab customSpawner1 = new CustomPrefab(monsterspawner1, true);
                        PrefabManager.Instance.AddPrefab(customSpawner1);
                    }
                    else
                    {
                        Logger.LogWarning("Spawners not found");
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

        private void AddNamedNPC()
        {
            try
            {
                //Debug.Log("Companions: NPC Bob");
                GameObject NPCBob = NPCBundle.LoadAsset<GameObject>("HumanNPCBob_DoD");
                if (NPCBob != null)
                {
                    var bobMob = new CustomCreature(NPCBob, true,
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
                    CreatureManager.Instance.AddCreature(bobMob);
                    //Debug.Log("Companions: NPC Fred");
                    GameObject NPCFred = NPCBundle.LoadAsset<GameObject>("HumanNPCFred_DoD");
                    var fredMob = new CustomCreature(NPCFred, true,
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
                    CreatureManager.Instance.AddCreature(fredMob);
                    //Debug.Log("Companions: NPC Barry");
                    GameObject NPCBarry = NPCBundle.LoadAsset<GameObject>("HumanNPCBarry_DoD");
                    var barryMob = new CustomCreature(NPCBarry, true,
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
                    CreatureManager.Instance.AddCreature(barryMob);
                    //Debug.Log("Companions: NPC Bobby");
                    GameObject NPCBobby = NPCBundle.LoadAsset<GameObject>("HumanNPCBobby_DoD");
                    var bobbyMob = new CustomCreature(NPCBobby, true,
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
                    CreatureManager.Instance.AddCreature(bobbyMob);
                    //Debug.Log("Companions: NPC Jeff");
                    GameObject NPCJeff = NPCBundle.LoadAsset<GameObject>("HumanNPCJeff_DoD");
                    var jeffMob = new CustomCreature(NPCJeff, true,
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
                    CreatureManager.Instance.AddCreature(jeffMob);
                    //Debug.Log("Companions: NPC Mandy");
                    GameObject NPCMandy = NPCBundle.LoadAsset<GameObject>("HumanNPCMandy_DoD");
                    var mandyMob = new CustomCreature(NPCMandy, true,
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
                    CreatureManager.Instance.AddCreature(mandyMob);
                    //Debug.Log("Companions: NPC Barbara");
                    GameObject NPCBarbara = NPCBundle.LoadAsset<GameObject>("HumanNPCBarbara_DoD");
                    var barbaraMob = new CustomCreature(NPCBarbara, true,
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
                    CreatureManager.Instance.AddCreature(barbaraMob);
                    //Debug.Log("Companions: NPC Sandra");
                    GameObject NPCSandra = NPCBundle.LoadAsset<GameObject>("HumanNPCSandra_DoD");
                    var sandraMob = new CustomCreature(NPCSandra, true,
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
                    CreatureManager.Instance.AddCreature(sandraMob);
                    //Debug.Log("Companions: NPC Daisy");
                    GameObject NPCDaisy = NPCBundle.LoadAsset<GameObject>("HumanNPCDaisy_DoD");
                    var daisyMob = new CustomCreature(NPCDaisy, true,
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
                    CreatureManager.Instance.AddCreature(daisyMob);
                    //Debug.Log("Companions: NPC Cathrine");
                    GameObject NPCCathrine = NPCBundle.LoadAsset<GameObject>("HumanNPCCathrine_DoD");
                    var cathrineMob = new CustomCreature(NPCCathrine, true,
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
                    CreatureManager.Instance.AddCreature(cathrineMob);
                    //Debug.Log("Companions: NPC Karen");
                    GameObject NPCKaren = NPCBundle.LoadAsset<GameObject>("HumanNPCKaren_DoD");
                    var karenMob = new CustomCreature(NPCKaren, true,
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
                    CreatureManager.Instance.AddCreature(karenMob);
                    //Debug.Log("Companions: NPC Fletch");
                    GameObject NPCFletch = NPCBundle.LoadAsset<GameObject>("HumanNPCFletch_DoD");
                    var fletchMob = new CustomCreature(NPCFletch, true,
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
                    CreatureManager.Instance.AddCreature(fletchMob);
                }
                else
                {
                    Logger.LogWarning("Companions not Found");
                }

            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Exception caught while adding Named Companions: {ex}");
            }
        }
        private void AddNamedMageNPC()
        {
            try
            {
                //Debug.Log("Companions: NPC Gary");
                GameObject NPCGary = NPCBundle.LoadAsset<GameObject>("HumanNPCGary_DoD");
                if (NPCGary != null)
                {
                    var garyMob = new CustomCreature(NPCGary, true,
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
                    //Debug.Log("Companions: NPC Tania");
                    GameObject NPCTania = NPCBundle.LoadAsset<GameObject>("HumanNPCTania_DoD");
                    if (NPCTania != null)
                    {
                        var taniaMob = new CustomCreature(NPCTania, true,
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
                        CreatureManager.Instance.AddCreature(taniaMob);
                        //Debug.Log("Companions: NPC Tina");
                        GameObject NPCTina = NPCBundle.LoadAsset<GameObject>("HumanNPCTina_DoD");
                        if (NPCTina != null)
                        {
                            var tinaMob = new CustomCreature(NPCTina, true,
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
                            CreatureManager.Instance.AddCreature(tinaMob);
                        }
                    }
                }
                else
                {
                    Logger.LogWarning("Companions not Found");
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Exception caught while adding Mage Companions: {ex}");
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
 * 
 * TODO:
 * Ability to change villager's name ~ DONE
 * Comand GUI for villager commander ~ DONE
 * UPGRADE WORKING : ~ FUTURE.
 * Upgrade items for dmage and working : ~ DONE
 * NOT WORKING VILLAGERS FIX ~ DONE
 * PASSIVE UPGRADE villager
 * CONFIG FOR SPAWNS    and multipliers done ~
 * ADD SEARCH RADIUS FOR WORK SUCH AS SMELT AND PICKUP
 * Experiment with follow target for work
 * Different villager for different biome
 * 
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