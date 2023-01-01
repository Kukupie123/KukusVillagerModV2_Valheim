using BepInEx.Configuration;
using Jotunn.Configs;
using UnityEngine;

namespace KukusVillagerMod.Configuration
{
    class VillagerModConfigurations
    {
       
        private static ConfigFile config= new ConfigFile("kukuVillagerConfig.cfg",true);
        public static string weak_villager_melee_prefab;
        public static string weak_villager_ranged_prefab;
        public static string bronze_villager_melee_prefab;
        public static string bronze_villager_ranged_prefab;
        public static string iron_villager_melee_prefab;
        public static string iron_villager_ranged_prefab;
        public static string silver_villager_melee_prefab;
        public static string silver_villager_ranged_prefab;
        public static string bm_villager_melee_prefab;
        public static string bm_villager_ranged_prefab;
        public static int weak_villager_level;
        public static int weak_villager_ranged_level;
        public static int bronze_villager_level;
        public static int bronze_villager_ranged_level;
        public static int iron_villager_level;
        public static int iron_villager_ranged_level;
        public static int silver_villager_level;
        public static int silver_villager_ranged_level;
        public static int bm_villager_level;
        public static int bm_villager_ranged_level;
        public static string bed_weak_melee_prefab;
        public static string bed_weak_ranged_prefab;
        public static string bed_bronze_melee_prefab;
        public static string bed_bronze_ranged_prefab;
        public static string bed_iron_melee_prefab;
        public static string bed_iron_ranged_prefab;
        public static string bed_silver_melee_prefab;
        public static string bed_silver_ranged_prefab;
        public static string bed_bm_melee_prefab;
        public static string bed_bm_ranged_prefab;
        public static int weak_bed_respawn;
        public static int bronze_bed_respawn;
        public static int iron_bed_respawn;
        public static int silver_bed_respawn;
        public static int bm_bed_respawn;
        public static string guardBedKey;
        public static string followPlayerKey;
        public static string defendPostKey;
        public static string deletePostKey;
        public static string deleteVillagerKey;
        public static string deleteBedsKey;
        public static string showStatKey;

        public VillagerModConfigurations()
        {
            config = new ConfigFile("/kukuvillagerconfig.cfg", true);

        }

        public static void LoadConfig()
        {
            config.SaveOnConfigSet = true;


            //Villager prefab
            weak_villager_melee_prefab = (string)config.Bind("Villager Prefab", "Weak_Villager_Melee", "Skeleton",
       new ConfigDescription("This decides what your models are going to be. You can visit here for full list https://valheim-modding.github.io/Jotunn/data/prefabs/character-list.html",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            weak_villager_ranged_prefab = (string)config.Bind("Villager Prefab", "Weak_Villager_Ranged", "Dverger",
      new ConfigDescription("",
      null,
      new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bronze_villager_melee_prefab = (string)config.Bind("Villager Prefab", "Bronze_Villager_Melee", "Dverger",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bronze_villager_ranged_prefab = (string)config.Bind("Villager Prefab", "Bronze_Villager_Ranged", "Dverger",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            iron_villager_melee_prefab = (string)config.Bind("Villager Prefab", "Iron_Villager_Melee", "Dverger",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            iron_villager_ranged_prefab = (string)config.Bind("Villager Prefab", "Iron_Villager_Ranged", "Dverger",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            silver_villager_melee_prefab = (string)config.Bind("Villager Prefab", "Silver_Villager_Melee", "Dverger",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            silver_villager_ranged_prefab = (string)config.Bind("Villager Prefab", "Silver_Villager_Ranged", "Dverger",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bm_villager_melee_prefab = (string)config.Bind("Villager Prefab", "BM_Villager_Melee", "Dverger",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bm_villager_ranged_prefab = (string)config.Bind("Villager Prefab", "BM_Villager_Ranged", "Dverger",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;



            //Villager level

            weak_villager_level = (int)config.Bind("Villagers Level", "Weak_Villager_Level", 0,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            weak_villager_ranged_level = (int)config.Bind("Villagers Level", "Weak_Villager_Ranged_Level", 0,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bronze_villager_level = (int)config.Bind("Villagers Level", "Bronze_Villager_Level", 0,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bronze_villager_ranged_level = (int)config.Bind("Villagers Level", "Bronze_Villager_Ranged_Level", 0,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            iron_villager_level = (int)config.Bind("Villagers Level", "Iron_Villager_Level", 0,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            iron_villager_ranged_level = (int)config.Bind("Villagers Level", "Iron_Villager_Ranged_Level", 0,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            silver_villager_level = (int)config.Bind("Villagers Level", "Silver_Villager_Level", 0,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            silver_villager_ranged_level = (int)config.Bind("Villagers Level", "Silver_Villager_Ranged_Level", 0,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bm_villager_level = (int)config.Bind("Villagers Level", "BM_Villager_Level", 0,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bm_villager_ranged_level = (int)config.Bind("Villagers Level", "BM_Villager_Ranged_Level", 0,
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            //Bed model

            bed_weak_melee_prefab = (string)config.Bind("Bed prefab", "Weak_Bed_Melee", "bed",
       new ConfigDescription("The model used by your bed. Not all piece prefabs are compatible",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bed_weak_ranged_prefab = (string)config.Bind("Bed prefab", "Weak_Bed_Ranged_Melee", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bed_bronze_melee_prefab = (string)config.Bind("Bed prefab", "Bronze_Bed_Melee", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bed_bronze_ranged_prefab = (string)config.Bind("Bed prefab", "Bronze_Bed_Ranged_Melee", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bed_iron_melee_prefab = (string)config.Bind("Bed prefab", "Iron_Bed_Melee", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bed_iron_ranged_prefab = (string)config.Bind("Bed prefab", "Iron_Bed_Ranged_Melee", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bed_silver_melee_prefab = (string)config.Bind("Bed prefab", "Silver_Bed_Melee", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bed_silver_ranged_prefab = (string)config.Bind("Bed prefab", "Silver_Bed_Ranged_Melee", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;

            bed_bm_melee_prefab = (string)config.Bind("Bed prefab", "BM_Bed_Melee", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bed_bm_ranged_prefab = (string)config.Bind("Bed prefab", "BM_Bed_Ranged_Melee", "bed",
       new ConfigDescription("",
       null,
       new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;


            //Bed Respawn timer
            weak_bed_respawn = (int)config.Bind("Bed respawn timer", "Weak_Bed_Melee_RT", 6000, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bronze_bed_respawn = (int)config.Bind("Bed respawn timer", "Bronze_Bed_Melee_RT", 6000, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            iron_bed_respawn = (int)config.Bind("Bed respawn timer", "Iron_Bed_Melee_RT", 6000, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            silver_bed_respawn = (int)config.Bind("Bed respawn timer", "Silver_Bed_Melee_RT", 6000, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;
            bm_bed_respawn = (int)config.Bind("Bed respawn timer", "BM_Bed_Melee_RT", 6000, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdminOnly = true })).BoxedValue;



            //Villager Commander Club keys
            guardBedKey = (string)config.Bind("Commander Club keybinds", "Guard Bed Key", "Keypad0").BoxedValue;
            followPlayerKey = (string)config.Bind("Commander Club keybinds", "Follow Player Key", "Keypad1").BoxedValue;
            defendPostKey = (string)config.Bind("Commander Club keybinds", "Defend Posts Key", "Keypad2").BoxedValue;
            deletePostKey = (string)config.Bind("Commander Club keybinds", "Delete Defend Posts", "Keypad4").BoxedValue;
            deleteVillagerKey = (string)config.Bind("Commander Club keybinds", "Delete Villagers Key", "Keypad5").BoxedValue;
            deleteBedsKey = (string)config.Bind("Commander Club keybinds", "Delete Beds Key", "Keypad6").BoxedValue;
            showStatKey = (string)config.Bind("Commander Club keybinds", "Show stats Key", "Keypad7").BoxedValue;

        }



    }




}