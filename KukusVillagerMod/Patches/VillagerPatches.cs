using HarmonyLib;
using KukusVillagerMod.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KukusVillagerMod.Patches
{
    //https://harmony.pardeike.net/articles/patching-injections.html
    [HarmonyPatch(typeof(Tameable),"GetHoverText")]
    static class VillagerPatches
    {
        public static void Postfix(Tameable __instance, ref string __result)
        {
            var vls = __instance.GetComponentInParent<VillagerLifeCycle>();

            if (vls != null)
            {
                __result = "HELLO FROM PATCH";
            }
        }
    }


}
