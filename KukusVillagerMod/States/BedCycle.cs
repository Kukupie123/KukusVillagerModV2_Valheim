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


        bool fixedUpdateDoneOnce = false;
        private void FixedUpdate()
        {
            if (piece.IsPlacedByPlayer())
            {


                if (!fixedUpdateDoneOnce)
                {
                    //We have to wait for it to be placed before we can do anything so we have to run it inside FixedUpdate ONCE
                    fixedUpdateDoneOnce = true;
                    znv = GetComponentInParent<ZNetView>();
                    znv.SetPersistent(true);
                    LoadUID();

                    WaitNSetVillager(); //Do not spawn as soon as we set bed. We need to give it time to setup so we wait a little and then spawn villagers
                }

            }
        }


        bool alreadySetting = false;
        private async void WaitNSetVillager()
        {
            if (alreadySetting) return;
            alreadySetting = true;
            await Task.Delay(5000);
            FindVillager();
            if (!villager) CreateVillager();
            alreadySetting = false;
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

        // After spawning/Finding Villager we need to give it some time to setup it's ZDO and other stuff
        bool isPostVillagerActive = false;
        async void PostVillagerSet(VillagerLifeCycle v, bool creating = false)
        {
            if (isPostVillagerActive) return;
            isPostVillagerActive = true;
            await Task.Delay(500);
            znv.GetZDO().Set(Util.villagerID, v.UID); //Store the villager ID in ZDO of bed. Used by villager to find his bed
            this.villager = v;
            v.GetComponent<Tameable>().Tame();
            if (creating)
                KLog.warning($"BED {UID} CREATED VILLAGER {villager.UID}");
            isPostVillagerActive = false;

        }

        void FindVillager()
        {
            foreach (var v in FindObjectsOfType<VillagerLifeCycle>())
            {
                string bedID = v.znv.GetZDO().GetString(Util.bedID);

                if (bedID.Equals(UID))
                {
                    KLog.warning($"BED {UID} FOUND VILLAGER {v.UID}");
                    PostVillagerSet(v);
                    return;
                }

            }
        }

        void CreateVillager()
        {
            var prefab = CreatureManager.Instance.GetCreaturePrefab(villagerName);
            var villagerCreature = Instantiate(prefab);
            var vlc = villagerCreature.GetComponent<VillagerLifeCycle>();
            PostVillagerSet(vlc, true);

        }
    }
}

/*
 * SPAWNING VILLAGER FROM BED MIGHT HAVE ISSUES AS WE ARE SPAWNING VILLAGER AND THEN SETTING VILLAGER KEY IN BED BUT THE VILLAGER COMP MIGHT HAVE ALREADY LOOKED FOR BED WITH ITS VILLAGER ID AND FAILED SO GOT DESTROYED
 */