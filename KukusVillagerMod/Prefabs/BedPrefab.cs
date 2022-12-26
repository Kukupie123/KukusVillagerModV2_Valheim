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
    class BedPrefab
    {

        public BedPrefab()
        {
            //First age
            var weakReq = new List<RequirementConfig>();
            weakReq.Add(new RequirementConfig("Wood", 20, 0, false));
            createBed("Weak_Bed_Ranged", "Bed for Weak Villagers with ranged weapons", "piece_bed02", "Weak_Villager_Ranged", weakReq, 1, 250, 1);
        }

        private void createBed(string bedID, string bedDesc, string cloneName, string villagerID, List<RequirementConfig> requirements, int villagerLevel, float villagerHealth, int villagerType)
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
            var bedState = bed.PiecePrefab.GetOrAddComponent<BedState>();
            bedState.villagerID = villagerID;
            bedState.villagerLevel = villagerLevel;
            bedState.villagerHealth = villagerHealth;
            bedState.villagerType = villagerType;

            //Add a container. Will use in future versions maybe
            bed.PiecePrefab.AddComponent<Container>();

            //Add the piece to PieceManager
            PieceManager.Instance.AddPiece(bed);

            KLog.info($"Created : {bedID} with villager : {bedState.villagerID}, level : {bedState.villagerLevel}");
        }
    }


}
