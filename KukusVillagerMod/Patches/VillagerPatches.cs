using HarmonyLib;
using KukusVillagerMod.enums;
using KukusVillagerMod.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var vls = __instance.GetComponentInParent<VillagerLifeCycle>(); //instance is the object

            if (vls != null)
            {
                __result = $"Villager : {vls.znv.GetZDO().m_uid.id}\nBed : {vls.GetBedZDO().m_uid.id}\nState : {((VillagerState)vls.GetBedZDO().GetInt("state", (int)VillagerState.Guarding_Bed)).ToString().Replace("_", " ")}";
            }
        }

    }

    [HarmonyPatch(typeof(Tameable), nameof(Tameable.Interact))]
    static class VillagerTameInteract
    {
        public static void Postfix(Tameable __instance, ref Humanoid user, ref bool hold, ref bool alt, ref bool __result)
        {
            var vls = __instance.GetComponentInParent<VillagerLifeCycle>(); //instance is the object
            if (vls != null)
            {
                if (!hold)
                {
                    vls.FollowPlayer(user.GetComponent<Player>());
                    __result = true;
                }
                else
                {
                    vls.GuardBed();
                    __result = true;
                }
            }
        }
    }

}
