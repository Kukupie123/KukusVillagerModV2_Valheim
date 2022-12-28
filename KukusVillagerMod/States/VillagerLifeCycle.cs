using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace KukusVillagerMod.States
{
    class VillagerLifeCycle : MonoBehaviour
    {
        /*
         * When a villager is spawned it is going to firstly load it's UID
         * Then it is going to look for a bed, if found we will set the the bed as this villager's bed
         * Or else we will destroy this
         */

        public ZNetView znv;

        public string UID; //VillagerID
        public int villagerType;
        public int villagerLevel;

        BedCycle bed;

        private void Awake()
        {
            znv = GetComponentInParent<ZNetView>();
            znv.SetPersistent(true);

            //Load/Create UID
            LoadUID();
        }

        void OnDestroy()
        {
            KLog.warning($"Destroying Villager {UID}");
        }

        //In fixed update we need to look for villager once mapData has been loaded. If it fails to do so we delete

        bool updatedOnce = false;
        private void FixedUpdate()
        {
            if (KukusVillagerMod.isMapDataLoaded)
            {
                if (!bed)
                {


                    //This block will get executed every frame except first or first few
                    if (updatedOnce)
                    {

                    }

                    //This block will get executed only once
                    else
                    {
                        updatedOnce = true;
                        FindBed(true);
                    }
                    //Will wait a while for the bed to be setup with this villager's ID as a key then we try to find it. If not found the villager is homeless and needs to be destroyed
                }
            }
        }

        private void LoadUID()
        {
            if (!znv)
            {
                KLog.warning("Villager !!! ZNV Value is null");
                return;
            }

            UID = znv.GetZDO().GetString(Util.villagerID, null);

            //If failed to load UID Create a new one. It fails When new villager is created
            if (UID == null || UID.Trim().Length == 0)
            {
                string uid = System.Guid.NewGuid().ToString();
                znv.GetZDO().Set(Util.villagerID, uid);
                UID = znv.GetZDO().GetString(Util.villagerID);
                KLog.warning($"Villager Created UID ${UID}");
            }
            else
            {
                KLog.warning($"Villager Found UID ${UID}");
            }

        }

        //We need to put a little delay so that we can let the newly created beds have time to setup their ZNV and stuff
        bool findingBedAlready = false;
        private async void FindBed(bool destroyIfNotFound = false)
        {
            if (findingBedAlready) return;
            findingBedAlready = true;
            await Task.Delay(1500);
            foreach (var b in FindObjectsOfType<BedCycle>())
            {
                string vilID = b.znv.GetZDO().GetString(Util.villagerID);
                KLog.warning($"SEACHING BED FOUND : {b.znv.GetZDO().GetString(Util.bedID)} with villager {vilID}");
                if (vilID.Equals(UID))
                {
                    KLog.warning($"Villager {UID} has found BED {b.UID}");
                    this.bed = b;
                    findingBedAlready = false;
                    return;
                }
            }
            if (bed == null && destroyIfNotFound)
            {
                KLog.warning($"DESTROYING VILLAGER AS NO BED WITH VILLAGERID {UID} FOUND");
                DestroyImmediate(this);
            }
            findingBedAlready = false;
        }
    }

}



