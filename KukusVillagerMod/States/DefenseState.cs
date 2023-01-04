using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KukusVillagerMod.States
{
    class DefenseState : MonoBehaviour, Interactable, Hoverable
    {

        Piece piece;
        private ZNetView znv;

        private void Awake()
        {
            piece = GetComponent<Piece>();
        }


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
                    base.InvokeRepeating("UpdateSpawner", UnityEngine.Random.Range(3f, 5f), 5f);

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

        public bool Interact(Humanoid user, bool hold, bool alt)
        {
            //Check if user has a bed uid
            var bedID = BedVillagerProcessor.SELECTED_BED_ID;

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
                BedVillagerProcessor.SELECTED_BED_ID = null;
                return true;
            }

        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return true;
        }



    }
}
