using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.States;

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
            createCreature2("Bronze_Villager", "Skeleton_NoArcher", 1, 6);

            //Iron
            createCreature2("Iron_Villager_Ranged", "Dverger", 2, 3);
            createCreature2("Iron_Villager", "Skeleton_NoArcher", 1, 9);

            //Silver
            createCreature2("Silver_Villager_Ranged", "Dverger", 2, 4);
            createCreature2("Silver_Villager", "Skeleton_NoArcher", 1, 12);

            //BM
            createCreature2("BlackMetal_Villager_Ranged", "Dverger", 2, 5);
            createCreature2("BlackMetal_Villager", "Skeleton_NoArcher", 1, 15);


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
