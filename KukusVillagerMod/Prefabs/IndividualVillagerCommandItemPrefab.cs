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
            CreateItem("GuardianFruit", "BreadDough", "Use it on bed or villager to make them guard their bed");
            CreateItem("WatcherFruit", "FirCone", "Use it on bed or villager to make them Defend their post");
            CreateItem("LabourerFruit", "Acorn", "Use it on bed or villager to make them Work");

        }

        private void CreateItem(string name, string cloneName, string desc)
        {
            ItemConfig commanderConfig = new ItemConfig();
            commanderConfig.Name = name;
            commanderConfig.Description = desc;
            commanderConfig.CraftingStation = null;
            commanderConfig.AddRequirement(new RequirementConfig("Wood", 1, 0, false));
            var commander = new CustomItem(name, cloneName, commanderConfig);
            ItemManager.Instance.AddItem(commander);
        }
    }
}
