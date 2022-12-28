using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            piece = GetComponent<Piece>();
        }

        private void OnDestroy()
        {
            Global.bedStates.Remove(this);
        }


        bool fixedUpdateDoneOnce = false;
        private void FixedUpdate()
        {

            if (piece.IsPlacedByPlayer())
            {
                if (!KukusVillagerMod.isMapDataLoaded) return;

                if (fixedUpdateDoneOnce == false && Player.m_localPlayer != null && !Player.m_localPlayer.IsTeleporting() && !ZNetScene.instance.InLoadingScreen())

                {
                    //We have to wait for it to be placed before we can do anything so we have to run it inside FixedUpdate ONCE
                    znv = GetComponentInParent<ZNetView>();
                    znv.SetPersistent(true);
                    LoadUID();
                    //After loading UID find villager
                    FindVillager();
                    if (villager == null)
                        CreateVillager();

                    fixedUpdateDoneOnce = true;
                }
                else
                {
                    if (!villager)
                    {
                        //if not first update and no villager exist then we cakk FindOrRespawnAfterWait. This will spawn or find the villager after a while
                        FindOrSpawnAfterWait();
                    }
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
            Global.bedStates.Add(this);
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
            villager.znv.GetZDO().Set(Util.bedID, UID); //Save the bedID on the villager's ZDO
            v.GetComponent<Tameable>().Tame();
            if (creating)
                KLog.warning($"BED {UID} HAS CREATED VILLAGER {villager.UID}");

        }

        bool alreadyWaiting = false;
        async void FindOrSpawnAfterWait()
        {
            //We have to wait because for some gosh damned reason getObjectsOfType returns empty array
            if (alreadyWaiting) return;
            alreadyWaiting = true;
            await Task.Delay(10000);
            FindVillager();
            if (villager == null)
                CreateVillager();
            alreadyWaiting = false;
        }
        void FindVillager()
        {
            var gg = FindObjectsOfType<VillagerLifeCycle>();

            KLog.info($"TOTAL VILLAGERS RN = {gg.Length}");

            foreach (var v in FindObjectsOfType<MonsterAI>())
            {
                var ls = v.GetComponentInParent<VillagerLifeCycle>();
                if (ls == null) continue;


                KLog.warning($"LOOP");
                string bedID = ls.znv.GetZDO().GetString(Util.bedID);
                KLog.warning($"Villager {ls.UID} HAS BEDID : {ls.znv.GetZDO().GetString(Util.bedID)}");

                if (bedID.Equals(UID))
                {
                    KLog.warning($"BED {UID} HAS FOUND VILLAGER {ls.UID}");
                    PostVillagerSet(ls);
                    return;
                }

            }
            KLog.warning($"BED {UID} HAS NOT FOUND VILLAGER!");
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