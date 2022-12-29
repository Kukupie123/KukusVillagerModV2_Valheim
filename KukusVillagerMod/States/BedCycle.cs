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
            KLog.warning($"Bed deleted {UID}");
        }




        bool fixedUpdateDoneOnce = false;
        private void FixedUpdate()
        {


            if (piece.IsPlacedByPlayer())
            {

                if (fixedUpdateDoneOnce == false && Player.m_localPlayer != null && !Player.m_localPlayer.IsTeleporting() && !ZNetScene.instance.InLoadingScreen() && KukusVillagerMod.isMapDataLoaded && ZNetScene.instance.IsAreaReady(transform.position))

                {
                    //We have to wait for it to be placed before we can do anything so we have to run it inside FixedUpdate ONCE

                    LoadUID();

                    //After loading UID find villager
                    FindVillager();
                    if (villager == null)
                        CreateVillager();

                    fixedUpdateDoneOnce = true;
                }
                else if (fixedUpdateDoneOnce && KukusVillagerMod.isMapDataLoaded && Player.m_localPlayer != null && Player.m_localPlayer.IsTeleporting() == false && !ZNetScene.instance.InLoadingScreen())
                {
                    if (villager == null)
                    {
                        //if not first update and no villager exist then we cakk FindOrRespawnAfterWait. This will spawn or find the villager after a while
                        FindOrSpawnAfterWait(5000);
                    }
                }

            }


        }



        private void LoadUID()
        {
            znv = GetComponentInParent<ZNetView>();
            znv.SetPersistent(true);

            UID = znv.GetZDO().GetString(Util.bedID, null);
            if (UID == null)
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



        bool alreadyWaiting = false;
        async void FindOrSpawnAfterWait(int duration)
        {
            //We have to wait because for some gosh damned reason getObjectsOfType returns empty array
            if (alreadyWaiting) return;
            alreadyWaiting = true;
            await Task.Delay(duration);
            if (villager != null)
            {
                alreadyWaiting = false;
                return;
            }
            FindVillager();
            if (villager == null)
                CreateVillager();
            alreadyWaiting = false;
            KLog.info("Respawn timer Ended for villager bed");
        }

        void FindVillager()
        {
            if (znv == null || znv.GetZDO() == null) return; //Async process throws exception if object deleted in process
            //Check if we even have villagerID set. If we do not have set any villagerID it means no villager has claimed this bed
            if (znv.GetZDO().GetString(Util.villagerID, null) == null) return;
            var gg = FindObjectsOfType<VillagerLifeCycle>();

            KLog.info($"TOTAL VILLAGERS RN = {gg.Length}");

            foreach (var v in FindObjectsOfType<MonsterAI>())
            {
                var ls = v.GetComponentInParent<VillagerLifeCycle>();
                if (ls == null || ZNetScene.instance.IsAreaReady(ls.transform.position) == false) continue;

                string bedID = ls.znv.GetZDO().GetString(Util.bedID);

                if (bedID.Equals(UID) && ls.znv.GetZDO().GetString(Util.bedID).Equals(UID))
                {
                    KLog.warning($"BED {UID} HAS FOUND VILLAGER {ls.UID}");
                    PostVillagerSet(ls);
                    return;
                }

            }
            KLog.warning($"BED {UID} HAS NOT FOUND VILLAGER!");
        }

        void CreateVillager()
        {
            var prefab = CreatureManager.Instance.GetCreaturePrefab(villagerName);
            var villagerCreature = SpawnSystem.Instantiate(prefab, transform.position, transform.rotation);
            var vlc = villagerCreature.GetComponent<VillagerLifeCycle>();
            PostVillagerSet(vlc, true);
        }

        void PostVillagerSet(VillagerLifeCycle v, bool creating = false)
        {
            if (v == null)
            {
                KLog.warning($"VILLAGER V NOT VALID IN POSTVILLAGERSET()");
            }

            //Save villager in zdo
            this.villager = v;
            znv.GetZDO().Set(Util.villagerID, v.UID);

            //Save bed info in villager's zdo

            v.setBed(this);
            v.GetComponent<Tameable>().Tame();
            if (creating)
                KLog.warning($"BED {UID} HAS CREATED VILLAGER. NOW VILLAGER NEEDS TO LOOOK FOR THIS EMPTY BED!");

        }

        //Called by villager to set villager value of this object
        public void setVillager(VillagerLifeCycle villager)
        {
            znv.GetZDO().Set(Util.villagerID, villager.UID);
            this.villager = villager;
        }


    }

}
