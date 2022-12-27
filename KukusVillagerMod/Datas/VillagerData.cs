using KukusVillagerMod.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace KukusVillagerMod.Datas
{
    class VillagerData : MonoBehaviour
    {
        public string uid;
        public int villagerType;
        public int villagerLevel;

        private BedState bed;

        private void Awake()
        {
            if (Global.villagerData.Contains(this) == false) Global.villagerData.Add(this);
            LoadUID();
        }
        private void OnDestroy()
        {
            Global.villagerData.Remove(this);
        }

        private void LoadUID()
        {
            GetComponentInParent<ZNetView>().SetPersistent(true);
            uid = GetComponentInParent<ZNetView>().GetZDO().GetString(Util.villagerID);

            //Failed to load. Create a new uid
            if (uid == null || uid.Trim().Length == 0)
            {
                string guid = System.Guid.NewGuid().ToString();
                GetComponentInParent<ZNetView>().GetZDO().Set(Util.villagerID, guid);
                uid = GetComponentInParent<ZNetView>().GetZDO().GetString(Util.villagerID);
                KLog.warning($"Failed to load ID for villagerData, Saved new {uid}");
            }
            else
            {
                KLog.warning($"Loaded villagerData w ID : {uid}");

            }
        }



        public void SetBed(BedState bed)
        {
            GetComponentInParent<ZNetView>().SetPersistent(true);
            GetComponentInParent<ZNetView>().GetZDO().Set(Util.bedID, bed.uid);
            this.bed = bed;
        }

        public BedState GetBed()
        {
            return this.bed;
        }
    }
}
