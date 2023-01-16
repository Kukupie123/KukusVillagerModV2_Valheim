using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Configuration;
using KukusVillagerMod.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KukusVillagerMod.Components.VillagerBed;

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
            createBed("Villager_Bed", "Bed for villager to use", "bed", weakReq);
        }

        private void createBed(string bedID, string bedDesc, string cloneName, List<RequirementConfig> requirements)
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
            bed.Piece.m_name = bedID.Replace("_", " ");

            //Remove default interactions of the bed
            UnityEngine.Object.DestroyImmediate(bed.PiecePrefab.GetComponent(typeof(Interactable)));
            UnityEngine.Object.DestroyImmediate(bed.PiecePrefab.GetComponent(typeof(Hoverable)));
            UnityEngine.Object.DestroyImmediate(bed.PiecePrefab.GetComponent(typeof(Bed)));


            //Add BedState component
            bed.PiecePrefab.GetOrAddComponent<BedState>();
            //Add the piece to PieceManager
            PieceManager.Instance.AddPiece(bed);
        }
    }


}
