using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Configuration;
using KukusVillagerMod.States;

namespace KukusVillagerMod.Prefabs
{
    class VillagerPrefab
    {

        public VillagerPrefab()
        {
            //Weak
            createCreature2("Weak_Villager_Ranged", VillagerModConfigurations.weak_villager_ranged_prefab, 2, VillagerModConfigurations.weak_villager_ranged_level);
            createCreature2("Weak_Villager", VillagerModConfigurations.weak_villager_melee_prefab, 1, VillagerModConfigurations.weak_villager_level);

            //Bronze
            createCreature2("Bronze_Villager_Ranged", VillagerModConfigurations.bronze_villager_ranged_prefab, 2, VillagerModConfigurations.bronze_villager_ranged_level);
            createCreature2("Bronze_Villager", VillagerModConfigurations.bronze_villager_melee_prefab, 1, VillagerModConfigurations.bronze_villager_level);

            //Iron
            createCreature2("Iron_Villager_Ranged", VillagerModConfigurations.iron_villager_ranged_prefab, 2, VillagerModConfigurations.weak_villager_ranged_level);
            createCreature2("Iron_Villager", VillagerModConfigurations.iron_villager_melee_prefab, 1, VillagerModConfigurations.weak_villager_level);

            //Silver
            createCreature2("Silver_Villager_Ranged", VillagerModConfigurations.silver_villager_ranged_prefab, 2, VillagerModConfigurations.weak_villager_ranged_level);
            createCreature2("Silver_Villager", VillagerModConfigurations.silver_villager_melee_prefab, 1, VillagerModConfigurations.weak_villager_level);

            //BM
            createCreature2("BlackMetal_Villager_Ranged", VillagerModConfigurations.bm_villager_ranged_prefab, 2, VillagerModConfigurations.weak_villager_ranged_level);
            createCreature2("BlackMetal_Villager", VillagerModConfigurations.bm_villager_melee_prefab, 1, VillagerModConfigurations.weak_villager_level);


        }
        void createCreature2(string villagerName, string prefabCloneName, int villagerType, int level)
        {
            CreatureConfig villagerConfig = new CreatureConfig();
            villagerConfig.Name = villagerName.Replace("_", " "); //Replace the "_" with " " Eg: Weak_Mage becomes Weak Mage
            villagerConfig.Faction = Character.Faction.Players;
            villagerConfig.Group = "Player";

            CustomCreature villager = new CustomCreature(villagerName, prefabCloneName, villagerConfig);

            //Remove components that we do not need from the villagers
            var npcTalk = villager.Prefab.GetComponent<NpcTalk>();
            var charDrop = villager.Prefab.GetComponent<CharacterDrop>();
            var npcTalkP = villager.Prefab.GetComponentInParent<NpcTalk>();
            var charDropP = villager.Prefab.GetComponentInParent<CharacterDrop>();
            var interactionP = villager.Prefab.GetComponentInParent(typeof(Interactable));
            var interaction = villager.Prefab.GetComponent(typeof(Interactable));

            UnityEngine.GameObject.DestroyImmediate(npcTalk);
            UnityEngine.GameObject.DestroyImmediate(npcTalkP);
            UnityEngine.GameObject.DestroyImmediate(charDrop);
            UnityEngine.GameObject.DestroyImmediate(charDropP);
            UnityEngine.GameObject.DestroyImmediate(interactionP);
            UnityEngine.GameObject.DestroyImmediate(interaction);


            //Add Components that we will need for the villager
            villager.Prefab.AddComponent<Tameable>();
            villager.Prefab.AddComponent<VillagerLifeCycle>();
            villager.Prefab.GetComponent<VillagerLifeCycle>().villagerType = villagerType;
            villager.Prefab.GetComponent<VillagerLifeCycle>().villagerLevel = level;

            CreatureManager.Instance.AddCreature(villager);

            KLog.info($"Created Creature with Name : {villagerName}");

        }
    }
}
