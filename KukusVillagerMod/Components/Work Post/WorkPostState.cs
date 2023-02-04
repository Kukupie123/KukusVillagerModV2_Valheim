using KukusVillagerMod.Components.Villager;
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
                this.znv = GetComponent<ZNetView>();
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
            ZDOID villagerZDOID = VillagerGeneral.SELECTED_VILLAGER_ID;
            if (villagerZDOID.IsNone() == false)
            {
                AssignWP(villagerZDOID);
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,
                    $"Work Post {znv.GetZDO().m_uid.m_id} Assigned to {VillagerGeneral.GetName(villagerZDOID)}");
                VillagerGeneral.SELECTED_VILLAGER_ID = ZDOID.None;
                VillagerGeneral.SELECTED_VILLAGERS_ID = null;
                return true;
            }
            else if (VillagerGeneral.SELECTED_VILLAGERS_ID != null && VillagerGeneral.SELECTED_VILLAGERS_ID.Count > 0)
            {
                foreach (var v in VillagerGeneral.SELECTED_VILLAGERS_ID)
                {
                    AssignWP(v);
                }


                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,
                    $"Work Post {znv.GetZDO().m_uid.m_id} Assigned to a bunch of villagers.");
                VillagerGeneral.SELECTED_VILLAGER_ID = ZDOID.None;
                VillagerGeneral.SELECTED_VILLAGERS_ID = null;
                return true;
            }

            return false;
        }

        private void AssignWP(ZDOID villagerZDOID)
        {
            VillagerGeneral.AssignWorkPost(villagerZDOID, znv.GetZDO().m_uid);
        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return true;
        }
    }
}