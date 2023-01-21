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
           
            CreateVillager("Villager_Ranged", "HumanNPCBob_DoD ");
            CreateVillager("Villager_Melee", "HumanNPCBob_DoD");

        }

        void CreateVillager(string villagerName, string prefabCloneName, bool melee = false)
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
                KLog.info("Spawn location for villagers :");
                KLog.info(v.ToString());
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
            if (!PrefabManager.Instance.GetPrefab(prefabCloneName))
            {
                KLog.warning($"Failed to load prefab {prefabCloneName} for villager {villagerName}, using default prefab");
                if (!melee)
                    prefabCloneName = "Dverger";
                else prefabCloneName = "Skeleton";
            }
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



            Tameable existingTameable = villager.Prefab.GetComponent<Tameable>();

            if (!existingTameable)
            {
                existingTameable = villager.Prefab.GetComponentInChildren<Tameable>();
            }
            if (!existingTameable)
            {
                existingTameable = villager.Prefab.GetComponentInParent<Tameable>();

            }

            UnityEngine.GameObject.DestroyImmediate(npcTalk);
            UnityEngine.GameObject.DestroyImmediate(npcTalkP);
            UnityEngine.GameObject.DestroyImmediate(interactionP);
            UnityEngine.GameObject.DestroyImmediate(interaction);
            UnityEngine.GameObject.DestroyImmediate(randAnim);
            UnityEngine.GameObject.DestroyImmediate(existingTameable);

            villager.Prefab.AddComponent<NpcTalk>(); //Add our custom talk component
            villager.Prefab.AddComponent<VillagerGeneral>(); //Add villager General component 
            villager.Prefab.AddComponent<VillagerAI>();
            villager.Prefab.AddComponent<Tameable>();

            villager.Prefab.GetComponent<MonsterAI>().m_avoidFire = true;
            villager.Prefab.GetComponent<MonsterAI>().m_huntPlayer = false;

            CreatureManager.Instance.AddCreature(villager);

            KLog.info($"Created Creature with Name : {villagerName} cloned from {prefabCloneName}");

        }
    }
}
