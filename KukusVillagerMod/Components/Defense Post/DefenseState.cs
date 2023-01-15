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
            var bedID = BedState.SELECTED_BED_ID;

            if (bedID == null)
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Please Select a bed first by interacting.");
                return false;
            }
            else
            {
                //Save the id of the defense post in the bed and then empty the bed value from the static variable
                var bedZDO = ZDOMan.instance.GetZDO(bedID.Value);
                bedZDO.Set("defense", znv.GetZDO().m_uid);
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Defense {znv.GetZDO().m_uid} Linked with Bed {bedID.Value.id}");
                BedState.SELECTED_BED_ID = null;
                return true;
            }

        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return true;
        }



    }
}
