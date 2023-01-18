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
using Jotunn.Managers;
using KukusVillagerMod.Prefabs;

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
            string itemName = item.m_shared.m_name;

            float multiplier = 1;
            bool upgrade = false;
            switch (itemName)
            {
                case "KukuVillager_Rag_Set":
                    upgrade = true;
                    multiplier = 0.2f;
                    break;
                case "KukuVillager_Troll_Set":
                    upgrade = true;
                    multiplier = 0.4f;
                    break;
                case "KukuVillager_Bronze_Set":
                    upgrade = true;
                    multiplier = 0.6f;
                    break;
                case "KukuVillager_Iron_Set":
                    upgrade = true;
                    multiplier = 1.0f;
                    break;
            }
            if (upgrade)
            {
                v.UpgradeVillagerHealth(multiplier);
                user.GetInventory().RemoveItem(item, 1);
            }
            upgrade = false;
            switch (itemName)
            {
                case "KukuVillager_Stone_Warlord_Set":
                    upgrade = true;
                    multiplier = 0.1f;
                    break;
                case "KukuVillager_Bronze_Warlod_Set":
                    upgrade = true;
                    multiplier = 0.4f;
                    break;
                case "KukuVillager_Iron_Warlord_Set":
                    upgrade = true;
                    multiplier = 0.6f;
                    break;
                case "KukuVillager_BM_Warlord_Set":
                    upgrade = true;
                    multiplier = 1.2f;
                    break;
            }
            if (upgrade)
            {
                v.UpgradeVillagerDamage(multiplier);
                user.GetInventory().RemoveItem(item, 1);
            }

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
                var containerZDO = containerZNV.GetZDO();
                //VERY IMPORTANT. NECESSARY TO CREATE INVENTORY
                containerZDO.Set("m_name", __instance.m_name);
                containerZDO.Set("width", __instance.m_width);
                containerZDO.Set("height", __instance.m_height);
                VillagerGeneral.AssignContainer(VillagerGeneral.SELECTED_VILLAGER_ID.Value, containerZNV.GetZDO().m_uid);
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Container {containerZNV.GetZDO().m_uid.id} Assigned to {VillagerGeneral.GetName(villagerZDOID.Value)}");
                VillagerGeneral.SELECTED_VILLAGER_ID = ZDOID.None;

            }
        }
    }



    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.GetCurrentWeapon))]
    static class VillagerDamageModifier
    {
        public static void Postfix(Humanoid __instance, ref ItemDrop.ItemData __result, ref ItemDrop.ItemData ___m_rightItem, ref ItemDrop.ItemData ___m_leftItem, ref ItemDrop ___m_unarmedWeapon)
        {
            try
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

                }
            }
            catch (Exception)
            {

            }

        }
    }
    //[HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.UpdateItemHashes))]
    static class LoadRRRNPC
    {
        public static void Prefix()
        {
            var rrrnpc = PrefabManager.Instance.GetPrefab("RRRN_qqpie");

            if (rrrnpc != null)
            {
                KLog.info("RRR NPC LOADED, You should be able to use it now");
                new VillagerPrefab(); //create custom creature cloned from RRR_NPC
            }
            else
            {
                KLog.info("RRR NPC NOT LOADED");
            }
        }
    }
}
