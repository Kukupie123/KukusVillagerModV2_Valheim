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
            loadUID();
        }

        private void loadUID()
        {
            uid = GetComponentInParent<ZNetView>().GetZDO().GetString(Util.villagerID);

            //Failed to load. Create a new uid
            if (uid == null)
            {
                string guid = System.Guid.NewGuid().ToString();
                GetComponentInParent<ZNetView>().GetZDO().Set(Util.villagerID, guid);
                uid = GetComponentInParent<ZNetView>().GetZDO().GetString(Util.villagerID);
                KLog.warning($"Failed to load ID for villager, Saved new {uid}");
            }
        }
        private void loadBed()
        {
            var beds = FindObjectsOfType<BedState>();

            if (beds == null)
            {
                //no beds found at all in world
                return;
            }

            foreach (var b in beds)
            {
                string vilID = b.GetComponentInParent<ZNetView>().GetZDO().GetString(Util.villagerID);
                if (vilID == null)
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
