﻿using Jotunn.Managers;
using System;
using UnityEngine;
namespace KukusVillagerMod.States
{
    //Based off of CreatureSpawner of valheim
    class VillagerSpawner : MonoBehaviour
    {
        private ZNetView znv;
        public float respawnTimeMin = 1f;
        public string VillagerPrefabName;
        private Piece piece;

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
            tameable.name = "Villager";
            tameable.SetName();
            tameable.SetText("A Fighter");
            tameable.Tame();


            component.GetZDO().SetPGWVersion(this.znv.GetZDO().GetPGWVersion());
            this.znv.GetZDO().Set("spawn_id", component.GetZDO().m_uid);
            this.znv.GetZDO().Set("alive_time", ZNet.instance.GetTime().Ticks);
            return component;

        }
    }
}
