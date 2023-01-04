using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Configuration;
using KukusVillagerMod.States;
using UnityEngine;

namespace KukusVillagerMod.Prefabs
{
    class VillagerPrefab
    {

        public VillagerPrefab()
        {
            //Weak
            createCreature2("Weak_Villager_Ranged", VillagerModConfigurations.weak_villager_ranged_prefab, 2, VillagerModConfigurations.weak_villager_ranged_level, VillagerModConfigurations.weak_villager_ranged_health);
            createCreature2("Weak_Villager", VillagerModConfigurations.weak_villager_melee_prefab, 1, VillagerModConfigurations.weak_villager_level, VillagerModConfigurations.weak_villager_health);

            //Bronze
            createCreature2("Bronze_Villager_Ranged", VillagerModConfigurations.bronze_villager_ranged_prefab, 2, VillagerModConfigurations.bronze_villager_ranged_level, VillagerModConfigurations.bronze_villager_ranged_health);
            createCreature2("Bronze_Villager", VillagerModConfigurations.bronze_villager_melee_prefab, 1, VillagerModConfigurations.bronze_villager_level, VillagerModConfigurations.bronze_villager_health);

            //Iron
            createCreature2("Iron_Villager_Ranged", VillagerModConfigurations.iron_villager_ranged_prefab, 2, VillagerModConfigurations.iron_villager_ranged_level, VillagerModConfigurations.iron_villager_ranged_health);
            createCreature2("Iron_Villager", VillagerModConfigurations.iron_villager_melee_prefab, 1, VillagerModConfigurations.iron_villager_level, VillagerModConfigurations.iron_villager_health);

            //Silver
            createCreature2("Silver_Villager_Ranged", VillagerModConfigurations.silver_villager_ranged_prefab, 2, VillagerModConfigurations.silver_villager_ranged_level, VillagerModConfigurations.silver_villager_ranged_health);
            createCreature2("Silver_Villager", VillagerModConfigurations.silver_villager_melee_prefab, 1, VillagerModConfigurations.silver_villager_level, VillagerModConfigurations.silver_villager_health);

            //BM
            createCreature2("BlackMetal_Villager_Ranged", VillagerModConfigurations.bm_villager_ranged_prefab, 2, VillagerModConfigurations.bm_villager_ranged_level, VillagerModConfigurations.bm_villager_ranged_health);
            createCreature2("BlackMetal_Villager", VillagerModConfigurations.bm_villager_melee_prefab, 1, VillagerModConfigurations.bm_villager_level, VillagerModConfigurations.bm_villager_health);

            //test
            testHuman();
        }

        void testHuman()
        {
            //Get a monster prefab and copy some stuff

            var monsterPrefab = CreatureManager.Instance.GetCreaturePrefab("Dverger");
            var setHumanoid = monsterPrefab.GetComponent<Humanoid>();
            var setAI = monsterPrefab.GetComponent<MonsterAI>();




            CreatureConfig villagerConfig = new CreatureConfig();
            villagerConfig.Name = "qqpie";
            villagerConfig.Faction = Character.Faction.Players;
            villagerConfig.Group = "Player";
            CustomCreature villager = new CustomCreature("qqpie", "Player", villagerConfig);
            var aiez = villager.Prefab.GetComponent<Player>().GetBaseAI();
            UnityEngine.Object.Destroy(villager.Prefab.GetComponent<PlayerController>());
            UnityEngine.Object.Destroy(villager.Prefab.GetComponent<Player>());
            UnityEngine.Object.Destroy(villager.Prefab.GetComponent<Talker>());
            UnityEngine.Object.Destroy(villager.Prefab.GetComponent<Skills>());

            villager.Prefab.AddComponent<Humanoid>();
            villager.Prefab.AddComponent<BaseAI>();
            //villager.Prefab.AddComponent<Tameable>();

            //Not sure how to setup further

            //disabled by default hmmm
            villager.Prefab.SetActive(true);

            CreatureManager.Instance.AddCreature(villager);
            KLog.warning("ADDED HUMAN KUKUPIE");


        }
        void createCreature2(string villagerName, string prefabCloneName, int villagerType, int level, int health)
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

            villager.Prefab.AddComponent<NpcTalk>(); //Add our custom talk component
            villager.Prefab.AddComponent<Tameable>(); //Add taming component to be able to tame it
            //UnityEngine.GameObject.DestroyImmediate(t.GetComponent(typeof(Interactable))); //Remove the interaction interface from taming comp
            villager.Prefab.AddComponent<VillagerLifeCycle>(); //Add villager lifecycle and setup values
            villager.Prefab.GetComponent<VillagerLifeCycle>().villagerType = villagerType;
            villager.Prefab.GetComponent<VillagerLifeCycle>().villagerLevel = level;
            villager.Prefab.GetComponent<VillagerLifeCycle>().health = health;


            CreatureManager.Instance.AddCreature(villager);

            KLog.info($"Created Creature with Name : {villagerName}");

        }
    }
}
