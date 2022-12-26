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
    class DefensivePostPrefab
    {
        public DefensivePostPrefab()
        {
            createDefensivePoint("Defense_Post", "Defense Position for your Melee Villagers to take", "wood_floor_1x1", 1);
            createDefensivePoint("Defense_Post_Ranged", "Defense Position for your Ranged Villagers to take", "wood_floor", 2);
        }
        void createDefensivePoint(string postName, string desc, string cloneName, int defenseType)
        {


            //Create Configuration of the bed
            PieceConfig defensePostConfig = new PieceConfig();
            defensePostConfig.Name = postName.Replace("_", " ");
            defensePostConfig.PieceTable = "Hammer";
            defensePostConfig.Category = "Villager";
            defensePostConfig.AddRequirement(new RequirementConfig("Wood", 2, 0, false));
            defensePostConfig.Description = desc;

            //Create the Bed Piece (Custom Piece)
            var defensePost = new CustomPiece(postName, cloneName, defensePostConfig);

            //Remove default interactions of the bed
            UnityEngine.Object.DestroyImmediate(defensePost.PiecePrefab.GetComponent(typeof(Interactable)));
            UnityEngine.Object.DestroyImmediate(defensePost.PiecePrefab.GetComponent(typeof(Hoverable)));

            defensePost.PiecePrefab.AddComponent<DefensePostState>();
            defensePost.PiecePrefab.GetComponent<DefensePostState>().defenseType = defenseType; //The type of defense the post is going to be

            //Add the piece to PieceManager
            PieceManager.Instance.AddPiece(defensePost);


        }
    }
}
