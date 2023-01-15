using KukusVillagerMod.Components.Villager;
using KukusVillagerMod.Components.VillagerBed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KukusVillagerMod.Components.DefensePost
{
    class DefenseState : MonoBehaviour, Interactable, Hoverable
    {

        Piece piece;
        private ZNetView znv;

        private void Awake()
        {
            piece = GetComponent<Piece>();
        }

        //Same deal as the one in BedVillagerProcessor. Please check that file to know about this variable
        bool fixedUpdateRanOnce = false;
        private void FixedUpdate()
        {
            if (!piece || KukusVillagerMod.isMapDataLoaded == false) return;

            if (piece.IsPlacedByPlayer())
            {
                if (fixedUpdateRanOnce == false)

                {
                    //Piece needs to be placed before ZNetView is Valid so we have to check if it has been placed every frame and run the codes below once
                    this.znv = base.GetComponent<ZNetView>();
                    this.znv.SetPersistent(true);
                    if (znv.GetZDO() == null)
                    {
                        fixedUpdateRanOnce = true;
                        return;
                    }

                    fixedUpdateRanOnce = true;
                }

            }
        }



        public string GetHoverName()
        {
            string defenseID = znv.GetZDO().m_uid.id.ToString();
            return $"Defense post ID {defenseID}";
        }

        public string GetHoverText()
        {
            string defenseID = znv.GetZDO().m_uid.id.ToString();
            return $"Defense post ID {defenseID}";
        }


        /// <summary>
        /// Interacting with the defense post will save the defense post's ZDOID in the bed's ZDO of the variable "SELECTED_BED_ID".
        /// If no bed was interacted with prior to interacting with this defense post we will show error message.
        /// Related : Interact method of "BedVillagerProcessor" class
        /// </summary>
        /// <param name="user"></param>
        /// <param name="hold"></param>
        /// <param name="alt"></param>
        /// <returns></returns>
        public bool Interact(Humanoid user, bool hold, bool alt)
        {
            //Check if user has a bed uid
            ZDOID villagerZDOID = VillagerGeneral.SELECTED_VILLAGER_ID;
            if (villagerZDOID == null || villagerZDOID.IsNone())
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Please Select a villager to assign first.");
                return false;
            }
            else
            {
                VillagerGeneral.AssignDefense(VillagerGeneral.SELECTED_VILLAGER_ID, znv.GetZDO().m_uid);
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Assigned Defense Post {znv.GetZDO().m_uid.id} for {VillagerGeneral.GetName(VillagerGeneral.SELECTED_VILLAGER_ID)}");
                return true;
            }

        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return true;
        }



    }
}
