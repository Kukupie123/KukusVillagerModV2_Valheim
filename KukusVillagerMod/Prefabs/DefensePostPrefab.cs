using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Components;
using KukusVillagerMod.Components.DefensePost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KukusVillagerMod.Prefabs
{
    class DefensePostPrefab
    {

        public DefensePostPrefab()
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
            dpConfig.Name = "DefensePost";
            dpConfig.PieceTable = "Hammer";
            dpConfig.Category = "Villager";

            //Update requirements
            foreach (var r in requirements)
            {
                dpConfig.AddRequirement(r);
            }
            dpConfig.Description = "Defense post for your villagers to defend.\nTo Assign A villager to a defense post, interact with their bed and then interact with the desired defense post.";
            //Create the Bed Piece (Custom Piece)
            var dp = new CustomPiece("DefensePost", "jute_carpet_blue", dpConfig);
            //Add defenseState
            var spawner = dp.PiecePrefab.GetOrAddComponent<DefenseState>();
            //Add the piece to PieceManager
            PieceManager.Instance.AddPiece(dp);


        }
    }
}
