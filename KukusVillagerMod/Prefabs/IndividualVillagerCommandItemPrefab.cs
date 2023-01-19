using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            List<RequirementConfig> requirements = new List<RequirementConfig> { new RequirementConfig("ArmorRagsChest", VillagerModConfigurations.ArmorRagSetReq, 0, false), new RequirementConfig("ArmorRagsLegs", VillagerModConfigurations.ArmorRagSetReq, 0, false) };
            CreateItem("KukuVillager_Rag_Set", "ArmorRagsChest", "Use it on Villager to upgrade their Health stats by 20% of their effeciency.", requirements);

            requirements = new List<RequirementConfig> { new RequirementConfig("ArmorTrollLeatherChest", VillagerModConfigurations.ArmorTrollSetReq, 0, false), new RequirementConfig("ArmorTrollLeatherLegs", VillagerModConfigurations.ArmorTrollSetReq, 0, false) };
            CreateItem("KukuVillager_Troll_Set", "ArmorTrollLeatherChest", "Use it on Villager to upgrade their Health stats by 40% of their effeciency.", requirements);

            requirements = new List<RequirementConfig> { new RequirementConfig("ArmorBronzeChest", VillagerModConfigurations.ArmorBronzeSetReq, 0, false), new RequirementConfig("ArmorBronzeLegs", VillagerModConfigurations.ArmorBronzeSetReq, 0, false), };
            CreateItem("KukuVillager_Bronze_Set", "ArmorBronzeChest", "Use it on Villager to upgrade their Health stats by 60% of their effeciency.", requirements);

            requirements = new List<RequirementConfig> { new RequirementConfig("ArmorIronChest", VillagerModConfigurations.ArmorIronSetReq, 0, false), new RequirementConfig("ArmorIronLegs", VillagerModConfigurations.ArmorIronSetReq, 0, false), };
            CreateItem("KukuVillager_Iron_Set", "ArmorIronChest", "Use it on Villager to upgrade their Health stats by 100% of their effeciency.", requirements);





            requirements = new List<RequirementConfig> { new RequirementConfig("AxeStone", VillagerModConfigurations.CombatStoneSetReq, 0, false), new RequirementConfig("PickaxeStone", VillagerModConfigurations.CombatStoneSetReq, 0, false), };
            CreateItem("KukuVillager_Stone_Warlord_Set", "AxeStone", "Use it on Villager to upgrade their Combat stats by 10% of their effeciency.", requirements);

            requirements = new List<RequirementConfig> { new RequirementConfig("PickaxeBronze", VillagerModConfigurations.CombatBronzeSetReq, 0, false), new RequirementConfig("AxeBronze", VillagerModConfigurations.CombatBronzeSetReq, 0, false), };
            CreateItem("KukuVillager_Bronze_Warlod_Set", "PickaxeBronze", "Use it on Villager to upgrade their Combat stats by 40% of their effeciency.", requirements);

            requirements = new List<RequirementConfig> { new RequirementConfig("PickaxeIron", VillagerModConfigurations.CombatIronSetReq, 0, false), new RequirementConfig("AxeIron", VillagerModConfigurations.CombatIronSetReq, 0, false), };
            CreateItem("KukuVillager_Iron_Warlord_Set", "PickaxeIron", "Use it on Villager to upgrade their Combat stats by 60% of their effeciency.", requirements);

            requirements = new List<RequirementConfig> { new RequirementConfig("PickaxeBlackMetal", VillagerModConfigurations.CombatBmSetReq, 0, false), new RequirementConfig("AtgeirBlackmetal", VillagerModConfigurations.CombatBmSetReq, 0, false), };
            CreateItem("KukuVillager_BM_Warlord_Set", "PickaxeBlackMetal", "Use it on Villager to upgrade their Combat stats by 120% of their effeciency.", requirements);



        }



        private void CreateItem(string name, string cloneName, string desc, List<RequirementConfig> reqs)
        {
            ItemConfig commanderConfig = new ItemConfig();
            commanderConfig.Name = name;
            commanderConfig.Description = desc;
            commanderConfig.CraftingStation = null;
            foreach (var r in reqs)
            {
                commanderConfig.AddRequirement(r);
            }
            var commander = new CustomItem(name, cloneName, commanderConfig);
            ItemManager.Instance.AddItem(commander);
        }
    }
}
