using KukusVillagerMod.Components.Villager;
using UnityEngine;

namespace KukusVillagerMod.Components.Defense_Post
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
            if (villagerZDOID.IsNone())
            {
                if (VillagerGeneral.SELECTED_VILLAGERS_ID != null && VillagerGeneral.SELECTED_VILLAGERS_ID.Count > 0)
                {
                    foreach (var v in VillagerGeneral.SELECTED_VILLAGERS_ID)
                    {
                        AssignDP(v);
                    }

                    VillagerGeneral.SELECTED_VILLAGERS_ID = null;
                    VillagerGeneral.SELECTED_VILLAGER_ID = ZDOID.None;
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,
                        "Assigned Defense Post to a bunch of villagers");
                    return true;
                }

                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,
                    "Please Select villager(s) to assign first.");
                return false;
            }
            else
            {
                AssignDP(villagerZDOID);
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,
                    $"Assigned Defense Post {znv.GetZDO().m_uid.id} for {VillagerGeneral.GetName(VillagerGeneral.SELECTED_VILLAGER_ID)}");
                return true;
            }
        }

        private void AssignDP(ZDOID villagerZDOID)
        {
            VillagerGeneral.AssignDefense(VillagerGeneral.SELECTED_VILLAGER_ID, znv.GetZDO().m_uid);
        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return true;
        }
    }
}