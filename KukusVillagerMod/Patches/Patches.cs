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
using KukusVillagerMod.Components.UI;

namespace KukusVillagerMod.Patches
{
    /*
     * Hijack methods and do stuff
     * __instance = the object
     * ref Object name = method
     * ref Object ___name = class var
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
                if (vls.IsVillagerTamed())
                    __result = vls.GetVillagerState().ToString();
                else
                    __result = "Roaming in the wild";
            }
        }

    }

    [HarmonyPatch(typeof(Tameable), nameof(Tameable.Interact))]
    static class VillagerTameInteract
    {
        public static void Postfix(Tameable __instance, ref Humanoid user, ref bool hold, ref bool alt, ref bool __result)
        {
            var vls = __instance.GetComponentInParent<VillagerGeneral>(); //instance is the object
            if (vls != null)
            {
                VillagerGUI.OnShowMenu(vls.ZNV.GetZDO().m_uid);
            }
            else
            {
                __result = false;
            }
        }
    }

    //Villager interaction
    [HarmonyPatch(typeof(Tameable), nameof(Tameable.UseItem))]
    static class VillagerUseItem
    {
        public static void Postfix(Tameable __instance, ref Humanoid user, ref ItemDrop.ItemData item)
        {
            VillagerAI ai = __instance.GetComponentInParent<VillagerAI>();
            VillagerGeneral v = __instance.GetComponentInParent<VillagerGeneral>();
            if (ai == null || v == null || !v.IsVillagerTamed()) return;

            //Upgrade villager if using the right item
            if (item.m_shared.m_name.Equals("GuardianFruit"))
            {
                KLog.warning("Upgrading villager");
                v.UpgradeVillagerDamage(10);
            }
            //Upgrade working skill level 
            //Upgrade health
        }
    }

    [HarmonyPatch(typeof(Container), nameof(Container.Interact))]
    static class ContainerInteraction
    {
        public static void Postfix(Container __instance)
        {
            ZDOID? villagerZDOID = VillagerGeneral.SELECTED_VILLAGER_ID;
            if (villagerZDOID != null && villagerZDOID.Value.IsNone() == false)
            {
                ZNetView containerZNV = __instance.GetComponentInParent<ZNetView>();
                VillagerGeneral.AssignContainer(VillagerGeneral.SELECTED_VILLAGER_ID.Value, containerZNV.GetZDO().m_uid);
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Container {containerZNV.GetZDO().m_uid.id} Assigned to {VillagerGeneral.GetName(villagerZDOID.Value)}");

            }
        }
    }



    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.GetCurrentWeapon))]
    static class VillagerDamageModifier
    {
        public static void Postfix(Humanoid __instance, ref ItemDrop.ItemData __result, ref ItemDrop.ItemData ___m_rightItem, ref ItemDrop.ItemData ___m_leftItem, ref ItemDrop ___m_unarmedWeapon)
        {
            if (__instance.GetComponentInParent<VillagerGeneral>())

            {
                var villagerGeneral = __instance.GetComponentInParent<VillagerGeneral>();
                ItemDrop.ItemData weapon = null;
                if (___m_rightItem != null && ___m_rightItem.IsWeapon())
                {
                    weapon = ___m_rightItem;
                }
                if (___m_leftItem != null && ___m_leftItem.IsWeapon() && ___m_leftItem.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Torch)
                {
                    weapon = ___m_leftItem;
                }
                if (___m_unarmedWeapon)
                {
                    weapon = ___m_unarmedWeapon.m_itemData;
                }

                if (weapon != null)
                {
                    weapon.m_shared.m_damages = new HitData.DamageTypes();
                    weapon.m_shared.m_damages.m_damage = villagerGeneral.GetDamage();
                    weapon.m_shared.m_damages.m_slash = villagerGeneral.GetSlash();
                    weapon.m_shared.m_damages.m_blunt = villagerGeneral.GetBlunt();
                    weapon.m_shared.m_damages.m_chop = villagerGeneral.GetChop();
                    weapon.m_shared.m_damages.m_fire = villagerGeneral.GetFire();
                    weapon.m_shared.m_damages.m_frost = villagerGeneral.GetFrost();
                    weapon.m_shared.m_damages.m_lightning = villagerGeneral.Getlightning();
                    weapon.m_shared.m_damages.m_pickaxe = villagerGeneral.GetPickaxe();
                    weapon.m_shared.m_damages.m_pierce = villagerGeneral.GetPickaxe();
                    weapon.m_shared.m_damages.m_poison = villagerGeneral.GetPoison();
                    weapon.m_shared.m_damages.m_spirit = villagerGeneral.GetSpirit();
                    __result = weapon;
                }
                else
                {
                    KLog.warning($"Failed to modify weapon damage for {__instance.gameObject.name}. Please use a different Prefab for villager");
                };

            }
        }
    }


}
