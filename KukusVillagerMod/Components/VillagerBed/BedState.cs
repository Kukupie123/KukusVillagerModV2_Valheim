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
        public float respawnTimeInMinute = 1f; //The respawn time for the villager. Has to be set during prefab creation
        private Piece piece;

        /*
         * When assigning posts and containers for a bed we need to keep track of the bed we interacted with. We store that bed's ZDO in this static variable
         */
        public static ZDOID? SELECTED_BED_ID;


        private void Awake()
        {
            piece = base.GetComponent<Piece>();
        }


        bool fixedUpdateRanOnce = false; //Used to determine if we have ran the fixed update atleast once, we need to perform few actions during the first update call and then never perform them. We use this boolean to determine it.
        private void FixedUpdate()
        {
            if (!piece || KukusVillagerMod.isMapDataLoaded == false) return; //Map data has to be loaded before we can proceed

            if (piece.IsPlacedByPlayer())
            {
                if (fixedUpdateRanOnce == false)

                {
                    //Piece needs to be placed before ZNetView is Valid so we have to check if it has been placed every frame and run the codes below once
                    this.znv = base.GetComponent<ZNetView>();
                    this.znv.SetPersistent(true);

                    //Load/Create Villager's state (Guarding, Defending etc)

                    if (znv.GetZDO() == null)
                    {
                        fixedUpdateRanOnce = true;
                        return;
                    }
                    fixedUpdateRanOnce = true;
                }

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



        //Interface

        public string GetHoverText()
        {
            return "WIP";
        }

        public string GetHoverName()
        {
            return name;
        }


        public bool Interact(Humanoid user, bool hold, bool alt)
        {
            return true;
        }

        //Does nothing as of now
        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {

            return true;
        }
    }
}
