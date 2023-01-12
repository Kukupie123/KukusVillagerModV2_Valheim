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
    static class Patches
    {
        public static void Postfix(Tameable __instance, ref string __result) //postfix = after the OG function is run
        {
            var vls = __instance.GetComponentInParent<VillagerGeneral>(); //instance is the object

            if (vls != null)
            {
                __result = $"Villager : {vls.ZNV.GetZDO().m_uid.id}\nBed : {vls.GetBedZDO().m_uid.id}\nState : {((VillagerState)vls.GetBedZDO().GetInt("state", (int)VillagerState.Guarding_Bed)).ToString().Replace("_", " ")}\nWork Skills : Pickup = {vls.GetWorkSkill_CanPickUp()}, Smelt = {vls.GetWorkSkill_CanSmelt()}";
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
                    vAI.FollowPlayer(user.GetComponent<Player>().GetZDOID());
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
        public static void Postfix(Tameable __instance, ref Humanoid user, ref ItemDrop.ItemData item)
        {
            VillagerAI ai = __instance.GetComponentInParent<VillagerAI>();
            VillagerGeneral v = __instance.GetComponentInParent<VillagerGeneral>();
            if (ai == null || v == null) return;
            string itemName = item.m_shared.m_name;

            switch (itemName)
            {
                case "LabourerFruit":
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Villager is going to Work");
                    ai.StartWork();
                    break;
                case "WatcherFruit":
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Villager is going to Defend Post");
                    ai.DefendPost();
                    break;
                case "GuardianFruit":
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Villager is going to Guard Bed");
                    ai.GuardBed();
                    break;
                case "FreeSpiritFruit":
                    ai.RoamAround();
                    break;
                case "LabourSkill_Pickup":
                    bool canPick = !v.GetWorkSkill_CanPickUp();
                    v.SetWorkSkill_Pickup(canPick);
                    break;
                case "LabourSkill_Smelt":
                    bool canSmelt = !v.GetWorkSkill_CanSmelt();
                    v.SetWorkSkill_Smelt(canSmelt);
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
