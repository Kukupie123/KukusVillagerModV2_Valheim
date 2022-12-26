using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace KukusVillagerMod.States
{
    class VillagerState : MonoBehaviour
    {
        public string uid = null;

        public BedState bedState;
        private void Awake()
        {
            GetComponentInParent<ZNetView>().SetPersistent(true);

            //Try to laod the uid
            LoadUID();

        }

        private void FixedUpdate()
        {
            if (!Global.villagerStates.Contains(this))
            {
                Global.villagerStates.Add(this);
            }
        }

        private void OnDestroy()
        {
            Global.villagerStates.Remove(this);
        }

        public void SetBed(BedState bed)
        {
            GetComponentInParent<ZNetView>().GetZDO().Set(Util.bedID, bed.uid);
            this.bedState = bed;
        }

        private void LoadUID()
        {
            uid = GetComponentInParent<ZNetView>().GetZDO().GetString(Util.villagerID);

            //Failed to load. Create a new uid
            if (uid == null || uid.Trim().Length == 0)
            {
                string guid = System.Guid.NewGuid().ToString();
                GetComponentInParent<ZNetView>().GetZDO().Set(Util.villagerID, guid);
                uid = GetComponentInParent<ZNetView>().GetZDO().GetString(Util.villagerID);
                KLog.warning($"Failed to load ID for villager, Saved new {uid}");
            }
            else
            {
                KLog.warning($"Loadedvillager, ID : {uid}");

            }
        }

        //Find bed which has key {villagerID : uid}
        private void FindBed()
        {
            var beds = FindObjectsOfType<BedState>();

            if (beds == null || uid.Trim().Length == 0)
            {
                //no beds found at all in world
                return;
            }

            foreach (var b in beds)
            {
                string vilID = b.GetComponentInParent<ZNetView>().GetZDO().GetString(Util.villagerID);
                if (vilID == null || vilID.Trim().Length == 0)
                {
                    continue;
                }
                if (vilID.Equals(uid))
                {
                    bedState = b;
                    return;
                }
            }
        }


    }
}
