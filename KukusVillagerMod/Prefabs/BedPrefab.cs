using Jotunn.Configs;
using Jotunn.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KukusVillagerMod.Prefabs
{
    class BedPrefab
    {
        private void createBed(string bedID, string bedDesc, string cloneName, string villagerCreatureName, List<RequirementConfig> requirements, int villagerLevel, float villagerHealth, int villagerType)
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


            //Add Spawner component and update spawner creature name
            var spawner = bed.PiecePrefab.GetOrAddComponent<BedState>();
            spawner.villagerName = villagerCreatureName;
            spawner.villagerLevel = villagerLevel;
            spawner.villagerType = villagerType;
            spawner.villagerHealth = villagerHealth;

            //Add a container. Will use in future versions maybe
            bed.PiecePrefab.AddComponent<Container>();

            //Add the piece to PieceManager
            PieceManager.Instance.AddPiece(bed);

            KLog.logInfo($"Created : {bedID} with villager : {spawner.villagerName}, level : {spawner.villagerLevel}");
        }
    }


}
