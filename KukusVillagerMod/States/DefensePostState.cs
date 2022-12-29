using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace KukusVillagerMod.States
{
    class DefensePostState : MonoBehaviour
    {
        public int defenseType; // 1 = melee, 2 = ranged
        public VillagerLifeCycle villager; //The villager assigned to this post
        private Piece piece;
        public bool placed = false;
        private void Awake()
        {
            piece = GetComponent<Piece>();
        }
        private void FixedUpdate()
        {
            if (!piece) return;

            //Should execute only once
            if (piece.IsPlacedByPlayer())
            {
                placed = true;
            }


        }


    }
}
