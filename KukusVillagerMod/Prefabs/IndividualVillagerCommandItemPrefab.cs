using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KukusVillagerMod.Prefabs
{
    /*
     * Items that will be used to command villagers to do stuff will be created here
     * They can be used on both villagers and beds
     * 1. Guard Bed
     * 2. Defend Post
     * 3. Work
     */

    class IndividualVillagerCommandItemPrefab
    {
        public IndividualVillagerCommandItemPrefab()
        {
            List<RequirementConfig> requirements = new List<RequirementConfig> { new RequirementConfig("Wood", 1, 1, false), };
            CreateItem("GuardianFruit", "BreadDough", "Use it on bed or villager to make them guard their bed", requirements);


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
