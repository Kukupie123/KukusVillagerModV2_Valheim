using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Datas;
using KukusVillagerMod.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KukusVillagerMod.Prefabs
{
    class VillagerPrefab
    {

        public VillagerPrefab()
        {
            //Weak
            createCreature2("Weak_Villager_Ranged", "Dverger", 2, 1);
            createCreature2("Weak_Villager", "Skeleton_NoArcher", 1, 3);

            //Bronze
            createCreature2("Bronze_Villager_Ranged", "Dverger", 2, 2);
            createCreature2("Bronze_Villager", "Skeleton_NoArcher", 1, 5);

            //Iron
            createCreature2("Iron_Villager_Ranged", "Dverger", 2, 3);
            createCreature2("Iron_Villager", "Skeleton_NoArcher", 1, 7);

            //Silver
            createCreature2("Silver_Villager_Ranged", "Dverger", 2, 4);
            createCreature2("Silver_Villager", "Skeleton_NoArcher", 1, 9);

            //BM
            createCreature2("BlackMetal_Villager_Ranged", "Dverger", 2, 5);
            createCreature2("BlackMetal_Villager", "Skeleton_NoArcher", 1, 10);


        }
        void createCreature2(string villagerName, string prefabCloneName, int villagerType, int level)
        {
            CreatureConfig villagerConfig = new CreatureConfig();
            villagerConfig.Name = villagerName.Replace("_", " "); //Replace the "_" with " " Eg: Weak_Mage becomes Weak Mage
            villagerConfig.Faction = Character.Faction.Players;
            villagerConfig.Group = "Player";

            CustomCreature villager = new CustomCreature(villagerName, prefabCloneName, villagerConfig);

           


            //Add custom comps
            villager.Prefab.GetOrAddComponent<Tameable>(); //Add taming component so that it can be tamed if needed
            //Add villager data comp to store values that will not change
            villager.Prefab.AddComponent<VillagerData>();
            villager.Prefab.GetComponent<VillagerData>().villagerType = villagerType;
            villager.Prefab.GetComponent<VillagerData>().villagerLevel = level;
            //ADD VILLAGER STATE AFTER SPAWNINIG


            //YOU CAN'T CONFIGURE SOME VALUES UNTIL CREATURE HAS BEEN INSTANTIATED SUCH AS HP

            CreatureManager.Instance.AddCreature(villager);

            KLog.info($"Created Creature with Name : {villagerName}");

        }
    }
}
