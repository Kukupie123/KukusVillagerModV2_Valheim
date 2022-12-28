using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KukusVillagerMod.States
{
    class BedCycle : MonoBehaviour
    {
        public string villagerName; //The name of the villager it will spawn
        public string UID;

        public ZNetView znv;

        Piece piece;
        VillagerLifeCycle villager;

        private void Awake()
        {
            LoadUID();
            piece = GetComponent<Piece>();
        }


        bool fixedUpdateDoneOnce = false;
        private void FixedUpdate()
        {
            if (piece.IsPlacedByPlayer() && KukusVillagerMod.isMapDataLoaded)
            {


                if (fixedUpdateDoneOnce == false)
                {
                    //We have to wait for it to be placed before we can do anything so we have to run it inside FixedUpdate ONCE
                    fixedUpdateDoneOnce = true;
                    znv = GetComponentInParent<ZNetView>();
                    znv.SetPersistent(true);
                    LoadUID();

                    //After loading UID find villager
                    FindVillager();
                    if (!villager) CreateVillager();
                }

            }
        }


        private void LoadUID()
        {
            UID = znv.GetZDO().GetString(Util.bedID, null);
            if (UID == null || UID.Trim().Length == 0)
            {
                string uid = System.Guid.NewGuid().ToString();
                znv.GetZDO().Set(Util.bedID, uid);
                UID = znv.GetZDO().GetString(Util.bedID, null);
                KLog.warning($"Bed UID Created {UID}");
            }
            else
            {
                KLog.warning($"Bed ID Loaded {UID}");
            }
        }

        // Save villager's ID in ZDO and mark villagerSet as true
        void PostVillagerSet(VillagerLifeCycle v, bool creating = false)
        {
            if (v == null)
            {
                KLog.warning($"VILLAGER V NOT VALID IN POSTVILLAGERSET()");
            }
            znv.GetZDO().Set(Util.villagerID, v.UID); //Store the villager ID in ZDO of bed. Used by villager to find his bed
            znv.GetZDO().Set(Util.villagerSet, true); //Mark villager set as true as this bed now has villager, used by spawned villagers to wait for a bed to finish saving villager's ID and then only continue searching for bed. A bed NEEDS to have villagerID
            this.villager = v;
            v.GetComponent<Tameable>().Tame();
            if (creating)
                KLog.warning($"BED {UID} HAS CREATED VILLAGER {villager.UID}");

        }

        void FindVillager()
        {
            foreach (var v in FindObjectsOfType<VillagerLifeCycle>())
            {
                string bedID = v.znv.GetZDO().GetString(Util.bedID);

                if (bedID.Equals(UID))
                {
                    KLog.warning($"BED {UID} HAS FOUND VILLAGER {v.UID}");
                    PostVillagerSet(v);
                    return;
                }

            }
        }

        //Create villager
        void CreateVillager()
        {

            var prefab = CreatureManager.Instance.GetCreaturePrefab(villagerName);
            var villagerCreature = SpawnSystem.Instantiate(prefab, transform.position, transform.rotation);
            var vlc = villagerCreature.GetComponent<VillagerLifeCycle>();
            PostVillagerSet(vlc, true);

        }
    }
}

/*
 * SPAWNING VILLAGER FROM BED MIGHT HAVE ISSUES AS WE ARE SPAWNING VILLAGER AND THEN SETTING VILLAGER KEY IN BED BUT THE VILLAGER COMP MIGHT HAVE ALREADY LOOKED FOR BED WITH ITS VILLAGER ID AND FAILED SO GOT DESTROYED
 */


/*
 * Bed will have 3 keys
 * UID
 * VillagerID
 * villagerID SET
 * 
 * 
 * When a bed is created
 * It will create UID
 * 
 * It will then spawn a villager and save its id as villagerID
 * once done it will set villagerIDSet to True to signify that villager has been saved
 * 
 * 
 * In villger spawn side:
 * When spawned it waits for map data to load
 * 
 * It finds all bed
 * 
 * it then waits for the bed to set villagerIDSet to true
 * 
 * when true it checks villagerID and validates. if none match we destroy
 */