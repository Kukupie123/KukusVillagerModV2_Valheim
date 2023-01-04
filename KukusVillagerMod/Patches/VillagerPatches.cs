using HarmonyLib;
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
    [HarmonyPatch(typeof(Tameable),"GetHoverText")]
    static class VillagerPatches
    {
        public static void Postfix(Tameable __instance, ref string __result) //postfix = after the OG function is run
        {
            var vls = __instance.GetComponentInParent<VillagerLifeCycle>(); //instance is the object

            if (vls != null)
            {
                __result = "HELLO FROM PATCH"; //result value
            }
        }

    }


}
