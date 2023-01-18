using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Components.Villager;
using KukusVillagerMod.Configuration;
using KukusVillagerMod.Components;
using UnityEngine;
using System.Collections.Generic;
using static CharacterDrop;
using System;

namespace KukusVillagerMod.Prefabs
{

    class VillagerPrefab
    {

        public VillagerPrefab()
        {
            createCreature2("Villager_Ranged", VillagerModConfigurations.VillagerRangedPrefabName);
            createCreature2("Villager_Melee", VillagerModConfigurations.VillagerMeleePrefabName);
        }

        void createCreature2(string villagerName, string prefabCloneName)
        {
            CreatureConfig villagerConfig = new CreatureConfig();
            villagerConfig.Name = villagerName.Replace("_", " "); //Replace the "_" with " " Eg: Weak_Mage becomes Weak Mage
            villagerConfig.Faction = Character.Faction.Players;
            villagerConfig.Group = "Player";
            string biomes = VillagerModConfigurations.biomeToSpawn;
            var biomesaArray = biomes.Split(',');
            List<Heightmap.Biome> biomesList = new List<Heightmap.Biome>();

            foreach (string s in biomesaArray)
            {
                switch (s)
                {
                    case "blackforest":
                        biomesList.Add(Heightmap.Biome.BlackForest);
                        break;
                    case "deepnorth":
                        biomesList.Add(Heightmap.Biome.DeepNorth);
                        break;
                    case "meadows":
                        biomesList.Add(Heightmap.Biome.Meadows);
                        break;
                    case "mistlands":
                        biomesList.Add(Heightmap.Biome.Mistlands);
                        break;
                    case "mountains":
                        biomesList.Add(Heightmap.Biome.Mountain);
                        break;
                    case "plains":
                        biomesList.Add(Heightmap.Biome.Plains);
                        break;
                    case "swamp":
                        biomesList.Add(Heightmap.Biome.Swamp);
                        break;
                    case "ocean":
                        biomesList.Add(Heightmap.Biome.Ocean);
                        break;
                }
            }

            foreach (var v in biomesList)
            {
                KLog.warning(v.ToString());
            }
            villagerConfig.AddSpawnConfig(
                new SpawnConfig
                {
                    Name = villagerName,
                    Biome = ZoneManager.AnyBiomeOf(biomesList.ToArray()),
                    HuntPlayer = false,
                    GroupRadius = VillagerModConfigurations.GroupRadius,
                    MaxGroupSize = VillagerModConfigurations.MaxGroupSize,
                    MaxSpawned = VillagerModConfigurations.MaxSpawned,
                    MinGroupSize = VillagerModConfigurations.MinGroupSize,
                    SpawnChance = VillagerModConfigurations.SpawnChance,
                    SpawnDistance = VillagerModConfigurations.SpawnDistance
                }

                );

            CustomCreature villager = new CustomCreature(villagerName, prefabCloneName, villagerConfig);

            //Remove components that we do not need from the villagers
            var npcTalk = villager.Prefab.GetComponent<NpcTalk>();
            var charDrop = villager.Prefab.GetComponent<CharacterDrop>();
            var npcTalkP = villager.Prefab.GetComponentInParent<NpcTalk>();
            var interactionP = villager.Prefab.GetComponentInParent(typeof(Interactable));
            var interaction = villager.Prefab.GetComponent(typeof(Interactable));
            var randAnim = villager.Prefab.GetComponent<RandomAnimation>();

            //Edit drops
            charDrop.SetDropsEnabled(false);
            charDrop.m_drops = new List<Drop>();

            UnityEngine.GameObject.DestroyImmediate(npcTalk);
            UnityEngine.GameObject.DestroyImmediate(npcTalkP);
            UnityEngine.GameObject.DestroyImmediate(interactionP);
            UnityEngine.GameObject.DestroyImmediate(interaction);
            UnityEngine.GameObject.DestroyImmediate(randAnim);

            villager.Prefab.AddComponent<NpcTalk>(); //Add our custom talk component
            villager.Prefab.AddComponent<Tameable>(); //Add taming component to be able to tame it
            villager.Prefab.AddComponent<VillagerGeneral>(); //Add villager General component 
            villager.Prefab.AddComponent<VillagerAI>();



            CreatureManager.Instance.AddCreature(villager);

            KLog.info($"Created Creature with Name : {villagerName} cloned from {prefabCloneName}");

        }
    }
}
