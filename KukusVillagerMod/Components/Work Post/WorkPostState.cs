using KukusVillagerMod.Components.Villager;
using KukusVillagerMod.Components.VillagerBed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KukusVillagerMod.Components.Work_Post
{
    class WorkPostState : MonoBehaviour, Interactable, Hoverable
    {
        ZNetView znv;
        Piece piece;

        //Same deal as the one in BedVillagerProcessor. Please check that file to know about this variable
        private void FixedUpdate()
        {

            if (piece == null) piece = GetComponent<Piece>();

            if (this.znv == null && piece.IsPlacedByPlayer())
            {
                this.znv = base.GetComponent<ZNetView>();
                this.znv.SetPersistent(true);
            }
        }




        public string GetHoverName()
        {
            string defenseID = znv.GetZDO().m_uid.id.ToString();
            return $"Work post ID {defenseID}";
        }

        public string GetHoverText()
        {
            string defenseID = znv.GetZDO().m_uid.id.ToString();
            return $"Work Post ID {defenseID}";
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
            ZDOID? villagerZDOID = VillagerGeneral.SELECTED_VILLAGER_ID;

            if (villagerZDOID == null || villagerZDOID.Value.IsNone())
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Please Select a villager to assign first.");
                return false;
            }
            else
            {
                VillagerGeneral.AssignWorkPost(VillagerGeneral.SELECTED_VILLAGER_ID.Value, znv.GetZDO().m_uid);
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Assigned Work Post {znv.GetZDO().m_uid.id} for {VillagerGeneral.GetName(VillagerGeneral.SELECTED_VILLAGER_ID.Value)}");
                VillagerGeneral.SELECTED_VILLAGER_ID = ZDOID.None;
                return true;
            }

        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return true;
        }



    }
}
