using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Configuration;
using KukusVillagerMod.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KukusVillagerMod.Prefabs
{
    class BedPrefab
    {

        public BedPrefab()
        {
            //First age
            var weakReq = new List<RequirementConfig>();
            weakReq.Add(new RequirementConfig("Wood", 30, 0, false));
            var weakReqM = new List<RequirementConfig>();
            weakReqM.Add(new RequirementConfig("Wood", 10, 0, false));
            createBed("Weak_Bed_Ranged", "Bed for Weak Villagers with ranged weapons", VillagerModConfigurations.bed_weak_ranged_prefab, "Weak_Villager_Ranged", weakReq,VillagerModConfigurations.weak_bed_respawn);
            createBed("Weak_Bed", "Bed for Weak Villagers", VillagerModConfigurations.bed_weak_melee_prefab, "Weak_Villager", weakReqM, VillagerModConfigurations.weak_bed_respawn);

            //Bronze Age
            var bronzeReq = new List<RequirementConfig>();
            bronzeReq.Add(new RequirementConfig("Bronze", 45, 0, false));
            var bronzeReqM = new List<RequirementConfig>();
            bronzeReqM.Add(new RequirementConfig("Bronze", 15, 0, false));
            createBed("Bronze_Bed_Ranged", "Bed for Villagers with ranged weapons, better than Weak Villagers", VillagerModConfigurations.bed_bronze_ranged_prefab, "Bronze_Villager_Ranged", bronzeReq, VillagerModConfigurations.bronze_bed_respawn);
            createBed("Bronze_Bed", "Bed for Villagers, better than Weak Villagers", VillagerModConfigurations.bed_bronze_melee_prefab, "Bronze_Villager", bronzeReqM, VillagerModConfigurations.bronze_bed_respawn);

            //Iron Age
            var ironReq = new List<RequirementConfig>();
            ironReq.Add(new RequirementConfig("Iron", 65, 0, false));
            var ironReqM = new List<RequirementConfig>();
            ironReqM.Add(new RequirementConfig("Iron", 25, 0, false));
            createBed("Iron_Bed_Ranged", "Bed for Decent Villagers with ranged weapons, better than Bronze Villagers", VillagerModConfigurations.bed_iron_ranged_prefab, "Iron_Villager_Ranged", ironReq, VillagerModConfigurations.iron_bed_respawn);
            createBed("Iron_Bed", "Bed for Decent Villagers, better than Bronze Villagers", VillagerModConfigurations.bed_iron_melee_prefab, "Iron_Villager", ironReqM, VillagerModConfigurations.iron_bed_respawn);

            //Silver Age
            var silverReq = new List<RequirementConfig>();
            silverReq.Add(new RequirementConfig("Silver", 85, 0, false));
            var silverReqM = new List<RequirementConfig>();
            silverReqM.Add(new RequirementConfig("Silver", 35, 0, false));
            createBed("Silver_Bed_Ranged", "Bed for Good Villagers with ranged weapons, better than Iron Villagers", VillagerModConfigurations.bed_silver_ranged_prefab, "Silver_Villager_Ranged", silverReq, VillagerModConfigurations.silver_bed_respawn);
            createBed("Silver_Bed", "Bed for Good Villagers, better than Iron Villagers", VillagerModConfigurations.bed_silver_melee_prefab, "Silver_Villager", silverReqM, VillagerModConfigurations.silver_bed_respawn);


            //Black metal age
            var bmReq = new List<RequirementConfig>();
            bmReq.Add(new RequirementConfig("BlackMetal", 100, 0, false));
            var bmReqM = new List<RequirementConfig>();
            bmReqM.Add(new RequirementConfig("BlackMetal", 45, 0, false));
            createBed("BlackMetal_Bed_Ranged", "Bed for Great Villagers with ranged weapons, better than Silver Villagers", VillagerModConfigurations.bed_bm_ranged_prefab, "BlackMetal_Villager_Ranged", bmReq, VillagerModConfigurations.bm_bed_respawn);
            createBed("BlackMetal_Bed", "Bed for Great Villagers, better than Silver Villagers", VillagerModConfigurations.bed_bm_melee_prefab, "BlackMetal_Villager", bmReqM, VillagerModConfigurations.bm_bed_respawn);
        }

        private void createBed(string bedID, string bedDesc, string cloneName, string villagerID, List<RequirementConfig> requirements, int respawnDuration)
        {
            //Create Configuration of the bed
            PieceConfig bedConfig = new PieceConfig();
            bedConfig.Name = bedID.Replace("_", " ");
            bedConfig.PieceTable = "Hammer";
            bedConfig.Category = "Villager";

            //Update requirements
            foreach (var r in requirements)
            {
                bedConfig.AddRequirement(r);
            }


            bedConfig.Description = bedDesc;


            //Create the Bed Piece (Custom Piece)
            var bed = new CustomPiece(bedID, cloneName, bedConfig);

            //Remove default interactions of the bed
            UnityEngine.Object.DestroyImmediate(bed.PiecePrefab.GetComponent(typeof(Interactable)));
            UnityEngine.Object.DestroyImmediate(bed.PiecePrefab.GetComponent(typeof(Hoverable)));
            UnityEngine.Object.DestroyImmediate(bed.PiecePrefab.GetComponent(typeof(Bed)));


            //Add BedState component
            var spawner = bed.PiecePrefab.GetOrAddComponent<BedVillagerProcessor>();
            //bedState.respawnDuration = respawnDuration;
            spawner.VillagerPrefabName = villagerID;
            //Add the piece to PieceManager
            PieceManager.Instance.AddPiece(bed);
        }
    }


}
