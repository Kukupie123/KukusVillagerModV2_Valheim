using Jotunn.Managers;
using KukusVillagerMod.Components.Villager;
using KukusVillagerMod.enums;
using System;
using UnityEngine;
namespace KukusVillagerMod.Components.VillagerBed
{
    //Based off of CreatureSpawner of valheim
    class BedState : MonoBehaviour, Hoverable, Interactable
    {
        private ZNetView znv;
        private Piece piece;

        /*
         * When assigning posts and containers for a bed we need to keep track of the bed we interacted with. We store that bed's ZDO in this static variable
         */


        private void Awake()
        {
            piece = base.GetComponent<Piece>();
        }


        private void FixedUpdate()
        {

            if (piece == null) piece = GetComponent<Piece>();

            if (this.znv == null && piece.IsPlacedByPlayer())
            {
                this.znv = base.GetComponent<ZNetView>();
                this.znv.SetPersistent(true);
            }
        }




        //Ignore collision with player
        private void OnCollisionEnter(Collision collision)
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (character != null
                && character.m_faction == Character.Faction.Players
                && character.GetComponent<VillagerGeneral>() == null) // allow collision between minions
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
                return;
            }

            VillagerGeneral villager = collision.gameObject.GetComponent<VillagerGeneral>();
            if (villager != null)
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
                return;
            }


        }



        public static ZDOID GetVillagerZDOID(ZDOID bedZDOID)
        {
            return Util.GetZDO(bedZDOID).GetZDOID("villager");
        }
        public ZDOID GetVillagerZDOID()
        {
            return GetVillagerZDOID(znv.GetZDO().m_uid);
        }
        public static bool IsVillagerAssigned(ZDOID bedZDOID)
        {
            var villagerZDOID = Util.GetZDO(bedZDOID).GetZDOID("villager");
            if (!Util.ValidateZDO(Util.GetZDO(villagerZDOID)) || !Util.ValidateZDOID(villagerZDOID)) return false;
            return true;
        }
        public bool IsVillagerAssigned()
        {
            return IsVillagerAssigned(znv.GetZDO().m_uid);
        }

        //Interface

        public string GetHoverText()
        {
            if (IsVillagerAssigned())
            {
                return $"Belongs to {VillagerGeneral.GetName(GetVillagerZDOID())}({Util.GetZDO(GetVillagerZDOID()).m_uid.id})";
            }
            return "Empty bed";
        }

        public string GetHoverName()
        {
            return name;
        }


        public bool Interact(Humanoid user, bool hold, bool alt)
        {
            if (VillagerGeneral.SELECTED_VILLAGER_ID != null && !VillagerGeneral.SELECTED_VILLAGER_ID.Value.IsNone())
            {
                VillagerGeneral.AssignBed(VillagerGeneral.SELECTED_VILLAGER_ID.Value, znv.GetZDO().m_uid);
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Assigned bed {znv.GetZDO().m_uid.id} for {VillagerGeneral.GetName(VillagerGeneral.SELECTED_VILLAGER_ID.Value)}");
                VillagerGeneral.SELECTED_VILLAGER_ID = ZDOID.None;
            }
            else
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Please Select a villager first");
            }
            return true;
        }

        //Does nothing as of now
        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {

            return true;
        }
    }
}
