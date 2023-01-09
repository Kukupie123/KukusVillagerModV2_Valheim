using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Components.DefensePost;
using KukusVillagerMod.Components.Work_Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KukusVillagerMod.Prefabs
{
    class WorkPostPrefab
    {
        public WorkPostPrefab()
        {
            List<RequirementConfig> req = new List<RequirementConfig>();
            req.Add(new RequirementConfig("Wood", 1, 0, false));
            CreatePrefab(req);
        }
        private void CreatePrefab(List<RequirementConfig> requirements)
        {
            //jute_carpet_blue
            //Create Configuration of the bed
            PieceConfig dpConfig = new PieceConfig();
            dpConfig.Name = "WorkPost";
            dpConfig.PieceTable = "Hammer";
            dpConfig.Category = "Villager";

            //Update requirements
            foreach (var r in requirements)
            {
                dpConfig.AddRequirement(r);
            }
            dpConfig.Description = "Work post for your villagers to go to and work.\n Interact with a bed then with a work post to assign the work post to the villager.";
            //Create the Bed Piece (Custom Piece)
            var dp = new CustomPiece("DefensePost", "jute_carpet", dpConfig);
            //Add defenseState
            var spawner = dp.PiecePrefab.GetOrAddComponent<WorkPostState>();
            //Add the piece to PieceManager
            PieceManager.Instance.AddPiece(dp);


        }
    }
}
