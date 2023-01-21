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
            //Compatible NPCs (No ZNV and Tameable error)
            /*
             * HumanNPCBob_DoD ~
             * HumanNPCFred_DoD ~
             * HumanNPCBarry_DoD ~
             * HumanNPCBobby_DoD ~
             * HumanNPCJeff_DoD ~
             * HumanNPCMandy_DoD ~
             * HumanNPCBarbara_DoD
             * HumanNPCSandra_DoD ~
             * HumanNPCDaisy_DoD
             * HumanNPCCathrine_DoD ~
             * HumanNPCKaren_DoD
             * HumanNPCFletch_DoD //RANGED BOW ~
             */

            //Meadows : 2 type of villager
            //Black forest : 2 type of villager
            //Mountains : 2 types of villagers
            //Plains : 3 types of villager
            //mistland : 3 types of villager

            CreateVillager("Villager_Meadow1", "HumanNPCBob_DoD", Heightmap.Biome.Meadows);
            CreateVillager("Villager_Meadow2", "HumanNPCMandy_DoD", Heightmap.Biome.Meadows);

            CreateVillager("Villager_BF1", "HumanNPCFred_DoD", Heightmap.Biome.BlackForest);
            CreateVillager("Villager_BF2", "HumanNPCBarbara_DoD", Heightmap.Biome.BlackForest);

            CreateVillager("Villager_Mountain1", "HumanNPCJeff_DoD", Heightmap.Biome.Mountain);
            CreateVillager("Villager_Mountain2", "HumanNPCSandra_DoD", Heightmap.Biome.Mountain);

            CreateVillager("Villager_Plains1", "HumanNPCBobby_DoD", Heightmap.Biome.Plains);
            CreateVillager("Villager_Plains2", "HumanNPCCathrine_DoD", Heightmap.Biome.Plains);

            CreateVillager("Villager_Plains1", "HumanNPCKaren_DoD", Heightmap.Biome.Mistlands);
            CreateVillager("Villager_Plains2", "HumanNPCFletch_DoD", Heightmap.Biome.Mistlands);

        }

        void CreateVillager(string villagerName, string prefabCloneName, Heightmap.Biome biome)
        {
            prefabCloneName = prefabCloneName.Trim();
            CreatureConfig villagerConfig = new CreatureConfig();
            villagerConfig.Faction = Character.Faction.Players;
            villagerConfig.Group = "Player";
            SpawnConfig spawnConfig = new SpawnConfig
            {
                Name = villagerName,
                Biome = biome,
                HuntPlayer = false,
                GroupRadius = VillagerModConfigurations.GroupRadius,
                MaxGroupSize = VillagerModConfigurations.MaxGroupSize,
                MaxSpawned = VillagerModConfigurations.MaxSpawned,
                MinGroupSize = VillagerModConfigurations.MinGroupSize,
                SpawnChance = VillagerModConfigurations.SpawnChance,
                SpawnDistance = VillagerModConfigurations.SpawnDistance

            };
            villagerConfig.AddSpawnConfig(spawnConfig);
            if (!PrefabManager.Instance.GetPrefab(prefabCloneName))
            {
                prefabCloneName = "Dverger";
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

            if (existingTameable == null)
            {
                existingTameable = villager.Prefab.GetComponentInChildren<Tameable>();
            }
            if (existingTameable == null)
            {
                existingTameable = villager.Prefab.GetComponentInParent<Tameable>();

            }

            ZNetView znv = villager.Prefab.GetComponent<ZNetView>();
            if (znv == null)
            {
                znv = villager.Prefab.GetComponentInChildren<ZNetView>();
            }
            if (znv == null)
            {
                znv = villager.Prefab.GetComponentInParent<ZNetView>();

            }

            UnityEngine.GameObject.DestroyImmediate(npcTalk);
            UnityEngine.GameObject.DestroyImmediate(npcTalkP);
            UnityEngine.GameObject.DestroyImmediate(interactionP);
            UnityEngine.GameObject.DestroyImmediate(interaction);
            UnityEngine.GameObject.DestroyImmediate(randAnim);

            villager.Prefab.AddComponent<NpcTalk>(); //Add our custom talk component
            villager.Prefab.AddComponent<VillagerGeneral>(); //Add villager General component 
            villager.Prefab.AddComponent<VillagerAI>();
            if (existingTameable == null)
            {
                villager.Prefab.AddComponent<Tameable>();
                KLog.warning($"Failed to find tameable component in {prefabCloneName}. Adding new one");
            }
            if (znv == null)
            {
                //villager.Prefab.AddComponent<ZNetView>();
                KLog.warning($"Failed to find ZNetView component in {prefabCloneName}. Adding new one");

            }


            villager.Prefab.GetComponent<MonsterAI>().m_avoidFire = true;
            villager.Prefab.GetComponent<MonsterAI>().m_huntPlayer = false;

            CreatureManager.Instance.AddCreature(villager);

            KLog.warning($"Created Creature with Name : {villagerName} cloned from {prefabCloneName}");
        }
    }
}
