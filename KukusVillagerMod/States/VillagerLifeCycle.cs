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
            try
            {
                //Wait for map data to load
                if (KukusVillagerMod.isMapDataLoaded)
                {
                    if (Player.m_localPlayer == null || Player.m_localPlayer.IsTeleporting()) return;
                    //Only search for bed if bed is null
                    if (!bed)
                    {


                        //This block will get executed every frame except first or first few
                        if (updatedOnce)
                        {

                        }

                        //This block will get executed only once, we search for bed ONLY ONCE
                        else
                        {
                            updatedOnce = true;
                            FindBed();
                            if (bed == null)
                            {
                                Destroy(this.gameObject);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                KLog.info(e.Message + " in fixed update of villager Life cycle");
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


        private void FindBed(BedCycle[] useThis = null)
        {
            var list = FindObjectsOfType<BedCycle>();

            if (useThis != null)
            {
                list = useThis;
            }

            List<BedCycle> NonReadyBeds = new List<BedCycle>();

            foreach (var b in FindObjectsOfType<BedCycle>())
            {
                if (b == null) continue;

                //if bed is not ready we are going to add this to nonReadybeds for recursion, znv to determine if they are placed yet or not
                if (b.znv == null || !b.znv.GetZDO().GetBool(Util.villagerSet, false))
                {
                    NonReadyBeds.Add(b);
                    continue;
                }


                //Bed has villagerSet so we can compare villagers IID
                string vilID = b.znv.GetZDO().GetString(Util.villagerID);
                KLog.warning($"SEACHING BED FOR VILLAGER {UID} FOUND : {b.znv.GetZDO().GetString(Util.bedID)} with villager {vilID}");
                if (vilID.Equals(UID))
                {
                    KLog.warning($"Villager {UID} has found BED {b.UID}");
                    znv.GetZDO().Set(Util.bedID, b.UID);
                    this.bed = b;
                    return;
                }
            }

            //After loop we are going to recursively call 
            if (NonReadyBeds.Count == 0)
            {
                KLog.warning($"Villager {UID} failed to find bed");
            }
            else
            {
                FindBed(NonReadyBeds.ToArray());
            }
        }
    }

}



