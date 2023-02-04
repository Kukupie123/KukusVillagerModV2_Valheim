using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Configuration;
using System.Collections.Generic;

namespace KukusVillagerMod.Prefabs
{
    /*
     * Items that will be used to command villagers 
     * 1. Upgrade combat experience,
     * 2. Upgrade health experience,
     */

    class IndividualVillagerCommandItemPrefab
    {
        public IndividualVillagerCommandItemPrefab()
        {
            List<RequirementConfig> requirements = new List<RequirementConfig> { new RequirementConfig("ArmorRagsChest", VillagerModConfigurations.ArmorRagSetReq), new RequirementConfig("ArmorRagsLegs", VillagerModConfigurations.ArmorRagSetReq) };
            CreateItem("KukuVillager_Rag_Set", "ArmorRagsChest", "Use it on Villager to upgrade their Health stats by 20% of their efficiency.", requirements);

            requirements = new List<RequirementConfig> { new RequirementConfig("ArmorTrollLeatherChest", VillagerModConfigurations.ArmorTrollSetReq), new RequirementConfig("ArmorTrollLeatherLegs", VillagerModConfigurations.ArmorTrollSetReq) };
            CreateItem("KukuVillager_Troll_Set", "ArmorTrollLeatherChest", "Use it on Villager to upgrade their Health stats by 40% of their efficiency.", requirements);

            requirements = new List<RequirementConfig> { new RequirementConfig("ArmorBronzeChest", VillagerModConfigurations.ArmorBronzeSetReq), new RequirementConfig("ArmorBronzeLegs", VillagerModConfigurations.ArmorBronzeSetReq), };
            CreateItem("KukuVillager_Bronze_Set", "ArmorBronzeChest", "Use it on Villager to upgrade their Health stats by 60% of their efficiency.", requirements);

            requirements = new List<RequirementConfig> { new RequirementConfig("ArmorIronChest", VillagerModConfigurations.ArmorIronSetReq), new RequirementConfig("ArmorIronLegs", VillagerModConfigurations.ArmorIronSetReq), };
            CreateItem("KukuVillager_Iron_Set", "ArmorIronChest", "Use it on Villager to upgrade their Health stats by 100% of their efficiency.", requirements);





            requirements = new List<RequirementConfig> { new RequirementConfig("AxeStone", VillagerModConfigurations.CombatStoneSetReq), new RequirementConfig("PickaxeStone", VillagerModConfigurations.CombatStoneSetReq), };
            CreateItem("KukuVillager_Stone_Warlord_Set", "AxeStone", "Use it on Villager to upgrade their Combat stats by 10% of their efficiency.", requirements);

            requirements = new List<RequirementConfig> { new RequirementConfig("PickaxeBronze", VillagerModConfigurations.CombatBronzeSetReq), new RequirementConfig("AxeBronze", VillagerModConfigurations.CombatBronzeSetReq), };
            CreateItem("KukuVillager_Bronze_Warlord_Set", "PickaxeBronze", "Use it on Villager to upgrade their Combat stats by 40% of their efficiency.", requirements);

            requirements = new List<RequirementConfig> { new RequirementConfig("PickaxeIron", VillagerModConfigurations.CombatIronSetReq), new RequirementConfig("AxeIron", VillagerModConfigurations.CombatIronSetReq), };
            CreateItem("KukuVillager_Iron_Warlord_Set", "PickaxeIron", "Use it on Villager to upgrade their Combat stats by 60% of their efficiency.", requirements);

            requirements = new List<RequirementConfig> { new RequirementConfig("PickaxeBlackMetal", VillagerModConfigurations.CombatBmSetReq), new RequirementConfig("AtgeirBlackmetal", VillagerModConfigurations.CombatBmSetReq), };
            CreateItem("KukuVillager_BM_Warlord_Set", "PickaxeBlackMetal", "Use it on Villager to upgrade their Combat stats by 120% of their efficiency.", requirements);



        }



        private void CreateItem(string name, string cloneName, string desc, List<RequirementConfig> reqs)
        {
            ItemConfig commanderConfig = new ItemConfig
            {
                Name = name,
                Description = desc,
                CraftingStation = null
            };
            foreach (var r in reqs)
            {
                commanderConfig.AddRequirement(r);
            }
            var commander = new CustomItem(name, cloneName, commanderConfig);
            ItemManager.Instance.AddItem(commander);
        }
    }
}
