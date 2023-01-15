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
            createBed("Weak_Bed_Ranged", "Bed for Weak Villagers with ranged weapons", "bed", weakReq);
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


            bedConfig.Description = bedDesc + "\nTo Assign a defense post for the bed, Interact with the bed first then go to your desired defense post and interact with it.";


            //Create the Bed Piece (Custom Piece)
            var bed = new CustomPiece(bedID, cloneName, bedConfig);

            //Remove default interactions of the bed
            UnityEngine.Object.DestroyImmediate(bed.PiecePrefab.GetComponent(typeof(Interactable)));
            UnityEngine.Object.DestroyImmediate(bed.PiecePrefab.GetComponent(typeof(Hoverable)));
            UnityEngine.Object.DestroyImmediate(bed.PiecePrefab.GetComponent(typeof(Bed)));


            //Add BedState component
            var spawner = bed.PiecePrefab.GetOrAddComponent<BedVillagerProcessor>();
            //bedState.respawnDuration = respawnDuration;
            //Add the piece to PieceManager
            PieceManager.Instance.AddPiece(bed);
        }
    }


}
