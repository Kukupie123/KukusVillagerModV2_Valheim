using HarmonyLib;
using KukusVillagerMod.enums;
using KukusVillagerMod.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KukusVillagerMod.Components.Villager;

namespace KukusVillagerMod.Patches
{
    /*
     * Hijack methods and do stuff
     */

    //https://harmony.pardeike.net/articles/patching-injections.html
    [HarmonyPatch(typeof(Tameable), nameof(Tameable.GetHoverText))]
    static class VillagerPatches
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
    /*
    [HarmonyReversePatch]
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.GetCurrentWeapon))]
    static class VillagerDmgPatch
    {
        public static void Postfix(MonsterAI __instance, ref ItemDrop.ItemData __result, ref ItemDrop.ItemData ___m_rightItem, ref ItemDrop.ItemData ___m_leftItem, ref ItemDrop ___m_unarmedWeapon)
        {
            if (__instance.GetComponentInParent<VillagerAI>() != null)
            {
                var h = __instance.GetComponent<Humanoid>();
                //KLog.warning("CUSTOM GET CURRENT WEAPON CALLED");
                if (___m_rightItem != null && ___m_rightItem.IsWeapon())
                {
                    __result = ___m_rightItem;

                }
                if (___m_leftItem != null && ___m_leftItem.IsWeapon() && ___m_leftItem.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Torch)
                {
                    __result = ___m_leftItem;
                }
                if (___m_unarmedWeapon)
                {
                    __result = ___m_unarmedWeapon.m_itemData;
                }
                __result = null;
            }

        }
    }
    */

}
