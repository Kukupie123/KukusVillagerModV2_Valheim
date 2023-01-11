using HarmonyLib;
using KukusVillagerMod.enums;
using KukusVillagerMod.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KukusVillagerMod.Components.Villager;
using KukusVillagerMod.Components.VillagerBed;

namespace KukusVillagerMod.Patches
{
    /*
     * Hijack methods and do stuff
     */

    //https://harmony.pardeike.net/articles/patching-injections.html
    [HarmonyPatch(typeof(Tameable), nameof(Tameable.GetHoverText))]
    static class VillagerHoverText
    {
        public static void Postfix(Tameable __instance, ref string __result) //postfix = after the OG function is run
        {
            var vls = __instance.GetComponentInParent<VillagerGeneral>(); //instance is the object

            if (vls != null)
            {
                __result = $"Villager : {vls.ZNV.GetZDO().m_uid.id}\nBed : {vls.GetBedZDO().m_uid.id}\nState : {((VillagerState)vls.GetBedZDO().GetInt("state", (int)VillagerState.Guarding_Bed)).ToString().Replace("_", " ")}";
            }
        }

    }

    [HarmonyPatch(typeof(Tameable), nameof(Tameable.Interact))]
    static class VillagerTameInteract
    {
        public static void Postfix(Tameable __instance, ref Humanoid user, ref bool hold, ref bool alt, ref bool __result)
        {
            var vAI = __instance.GetComponentInParent<VillagerAI>(); //instance is the object
            if (vAI != null)
            {
                if (!hold)
                {
                    vAI.FollowPlayer(user.GetComponent<Player>());
                    __result = true;
                }
                else
                {
                    vAI.GuardBed();
                    __result = true;
                }
            }
            else
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(Tameable), nameof(Tameable.UseItem))]
    static class VillagerUseItem
    {
        public static void Postfix(Tameable __insta nce, ref Humanoid user, ref ItemDrop.ItemData item)
        {
            VillagerAI ai = __instance.GetComponentInParent<VillagerAI>();
            if (ai == null) return;
            string itemName = item.m_shared.m_name;

            switch (itemName)
            {
                case "LabourerFruit":
                    ai.StartWork();
                    break;
                case "WatcherFruit":
                    ai.DefendPost();
                    break;
                case "GuardianFruit":
                    ai.GuardBed();
                    break;
            }
        }
    }

    [HarmonyPatch(typeof(Container), nameof(Container.Interact))]
    static class ContainerInteraction
    {
        public static void Postfix(Container __instance)
        {
            if (BedVillagerProcessor.SELECTED_BED_ID != null && BedVillagerProcessor.SELECTED_BED_ID.Value.IsNone() == false)
            {
                ZNetView containerZNV = __instance.GetComponentInParent<ZNetView>();
                ZDO bedZDO = ZDOMan.instance.GetZDO(BedVillagerProcessor.SELECTED_BED_ID.Value);
                if (bedZDO != null && bedZDO.IsValid())
                {
                    bedZDO.Set("container", containerZNV.GetZDO().m_uid);
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Assigned Container {containerZNV.GetZDO().m_uid.id} to Bed {BedVillagerProcessor.SELECTED_BED_ID.Value.id}");
                    BedVillagerProcessor.SELECTED_BED_ID = null;
                }
            }
        }
    }


}
