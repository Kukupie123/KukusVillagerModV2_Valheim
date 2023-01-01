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
         * When villager is loaded in memory it checks if bed is null or not
         * if bed is not null it means that it was just spawned and setAsActive by a bed.
         * If bed is null it means that it was loaded by the game engine and has lost reference to it's bed.
         * 
         * If it was loaded by game engine then it will search for bed in vicinity and if found it will save it's reference.
         * If it couldn't find it. It will get deleted
         * 
         * if it was spawned by a bed then we are going to save the bed's UID as our UID for future use
         */
        ZNetView znv;

        public BedCycle bed;

        private void Awake()
        {
            znv = GetComponentInParent<ZNetView>();
            znv.SetPersistent(true);
            loadOrCreateUID();
        }

        internal int villagerLevel;
        internal int villagerType;

        bool fixedUpdateRanOnce = false;
        private void FixedUpdate()
        {
            if (Player.m_localPlayer == null) return;


            if (KukusVillagerMod.isMapDataLoaded == false)
            {
                return;
            }

            if (GetIsSet() == false)
            {
                KLog.warning($"Villager {GetUID()} has no bed YET!");
                return; //After spawning we have to wait for the bed to be linked with this villager. Once done this will be true persistently.
            }

            if (fixedUpdateRanOnce == false)
            {
                if (bed == null && ZNetScene.instance.IsAreaReady(transform.position))
                {
                    fixedUpdateRanOnce = true;
                    FindBed();
                    if (bed == null)
                    {
                        KLog.info($"destroying villager {GetUID()}");
                        ZNetScene.instance.Destroy(this.gameObject);
                    }
                }
            }
        }

        void loadOrCreateUID()
        {
            var uid = znv.GetZDO().GetString(Util.uid, null);
            if (uid == null)
            {
                //Create new UID
                uid = System.Guid.NewGuid().ToString();

                znv.GetZDO().Set(Util.uid, uid);
                KLog.warning($"Created UID for Villager {GetUID()}");
            }
            {
                KLog.warning($"Found UID for Villager {uid}");
            }

        }

        bool GetIsSet()
        {
            if (znv == null) return false;
            return znv.GetZDO().GetBool("set", false);
        }

        void markAsActivePersistent()
        {
            znv.GetZDO().Set("set", true);
        }

        public void SaveBed(BedCycle bed)
        {
            znv.GetZDO().Set(Util.bedID, bed.GetUID());
            KLog.warning($"Villager {GetUID()}  saved beds UID {GetLinkedBedID()}");
            markAsActivePersistent();
        }

        public string GetUID()
        {
            if (znv == null) return null;
            return znv.GetZDO().GetString(Util.uid, null);
        }

        public string GetLinkedBedID()
        {
            if (znv == null) return null;
            return znv.GetZDO().GetString(Util.bedID, null);
        }


        private void FindBed()
        {
            string bedID = znv.GetZDO().GetString(Util.bedID, "");
            var beds = FindObjectsOfType<BedCycle>();
            KLog.warning($"Seaching bed for villager {GetUID()}");
            foreach (var bed in beds)
            {
                if (ZNetScene.instance.IsAreaReady(bed.transform.position) == false || bed == null || bed.znv == null || bed.GetUID() == null) continue;
                if (bed.GetUID().Equals(GetLinkedBedID()) && GetUID().Equals(bed.GetLinkedVillagerID()))
                {
                    this.bed = bed;
                    SaveBed(bed);
                    KLog.warning($"Found bed for villager {GetUID()} , Bed : {GetLinkedBedID()}");

                    return;
                }
            }
            KLog.warning($"Seaching bed FAILED for villager {GetUID()}");

        }

    }

}



