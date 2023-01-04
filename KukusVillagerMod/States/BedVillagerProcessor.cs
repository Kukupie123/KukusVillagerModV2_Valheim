using Jotunn.Managers;
using System;
using UnityEngine;
namespace KukusVillagerMod.States
{
    //Based off of CreatureSpawner of valheim
    class BedVillagerProcessor : MonoBehaviour, Hoverable, Interactable
    {
        private ZNetView znv;
        public float respawnTimeMin = 1f;
        public string VillagerPrefabName;
        private Piece piece;


        public static ZDOID? SELECTED_BED_ID;


        private void Awake()
        {
            piece = base.GetComponent<Piece>();
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

        private void UpdateSpawner()
        {
            if (!this.znv.IsOwner())
            {
                return;
            }

            ZDOID zdoid = this.znv.GetZDO().GetZDOID("spawn_id");
            if (this.respawnTimeMin <= 0f && !zdoid.IsNone())
            {
                return;
            }

            if (!zdoid.IsNone() && ZDOMan.instance.GetZDO(zdoid) != null)
            {
                this.znv.GetZDO().Set("alive_time", ZNet.instance.GetTime().Ticks);
                return;
            }

            if (this.respawnTimeMin > 0f)
            {
                DateTime time = ZNet.instance.GetTime();
                DateTime d = new DateTime(this.znv.GetZDO().GetLong("alive_time", 0L));
                if ((time - d).TotalMinutes < (double)this.respawnTimeMin)
                {
                    return;
                }
            }


            this.Spawn();

        }


        private bool HasSpawned()
        {
            return !(this.znv == null) && this.znv.GetZDO() != null && !this.znv.GetZDO().GetZDOID("spawn_id").IsNone();
        }

        private ZNetView Spawn()
        {
            KLog.warning("Spawning CREATURE!");
            Vector3 position = base.transform.position;
            float y;
            if (ZoneSystem.instance.FindFloor(position, out y))
            {
                position.y = y;
            }

            var villagerPrefab = CreatureManager.Instance.GetCreaturePrefab(this.VillagerPrefabName);
            Quaternion rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(villagerPrefab, position, rotation);


            ZNetView component = gameObject.GetComponent<ZNetView>();
            BaseAI component2 = gameObject.GetComponent<BaseAI>();
            Tameable tameable = gameObject.GetComponent<Tameable>();
            tameable.Tame();

            component.GetZDO().Set("spawner_id", this.znv.GetZDO().m_uid); //Save the bed's ID in the villager's ZDO
            component.GetZDO().SetPGWVersion(this.znv.GetZDO().GetPGWVersion()); //not sure that this is for
            this.znv.GetZDO().Set("spawn_id", component.GetZDO().m_uid); //Save the villager's ID in this bed's ZDO
            this.znv.GetZDO().Set("alive_time", ZNet.instance.GetTime().Ticks); //Save alive time in this bed's ZDO
            return component;
        }

        //Interface

        public string GetHoverText()
        {
            string bedID = this.znv.GetZDO().m_uid.id.ToString();

            var villagerID = this.znv.GetZDO().GetZDOID("spawn_id");
            string villager = "None";
            if (!villagerID.IsNone())
            {
                villager = villagerID.id.ToString();
            }

            string containerID = "None";

            var defenseID = this.znv.GetZDO().GetZDOID("defense");
            string defense = "None";
            if (!defenseID.IsNone()) defense = defenseID.id.ToString();

            return $"Bed ID : {bedID}\nVillager ID {villager}\nContainer ID : {containerID}\nDefense Post ID :{defense}";
        }

        public string GetHoverName()
        {
            return "Villager's Bed NAME";
        }

        public bool Interact(Humanoid user, bool hold, bool alt)
        {
            if (!hold) //Save bed in ZDO of user temporarily, when interacted with defense post, we will make use of this bed
            {
                SELECTED_BED_ID = this.znv.GetZDO().m_uid;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Bed {SELECTED_BED_ID.Value.id} selected. Interact with a Defense to let the villager know where to defend");
                return true;
            }
            return false;
        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return true;
        }
    }
}
