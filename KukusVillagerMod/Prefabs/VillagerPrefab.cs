using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
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
            createCreature2("Weak_Villager_Ranged", "Dverger");
        }
        void createCreature2(string villagerName, string prefabCloneName)
        {
            CreatureConfig villagerConfig = new CreatureConfig();
            villagerConfig.Name = villagerName.Replace("_", " "); //Replace the "_" with " " Eg: Weak_Mage becomes Weak Mage
            villagerConfig.Faction = Character.Faction.Players;
            villagerConfig.Group = "Player";

            CustomCreature villager = new CustomCreature(villagerName, prefabCloneName, villagerConfig);

            //Destroy comps
            UnityEngine.GameObject.DestroyImmediate(villager.Prefab.GetComponent<PlayerController>()); //Destroy player controller
            UnityEngine.GameObject.DestroyImmediate(villager.Prefab.GetComponent<Talker>()); //destroy talking comp
            UnityEngine.GameObject.DestroyImmediate(villager.Prefab.GetComponent<Skills>()); //Disable skils
            UnityEngine.GameObject.DestroyImmediate(villager.Prefab.GetComponent<Player>()); //Disable skils


            //Add custom comps
            villager.Prefab.GetOrAddComponent<Tameable>(); //Add taming component so that it can be tamed if needed
            villager.Prefab.GetOrAddComponent<VillagerState>(); //villager state component
            villager.Prefab.GetOrAddComponent<Humanoid>();
            villager.Prefab.GetOrAddComponent<MonsterAI>();

            //YOU CAN'T CONFIGURE SOME VALUES UNTIL CREATURE HAS BEEN INSTANTIATED SUCH AS HP

            CreatureManager.Instance.AddCreature(villager);

            KLog.info($"Created Creature with Name : {villagerName}");

        }
    }
}
