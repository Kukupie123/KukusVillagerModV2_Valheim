using HarmonyLib;
using System;
using KukusVillagerMod.Components.Villager;
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
                string text;
                if (vls.IsVillagerTamed())
                {
                    text = $"{vls.GetVillagerState().ToString().Replace("_", " ")}";
                    text = $"{text}\nFaction : {vls.GetVillagerFaction()}";
                }
                else
                    text = "Roaming in the wild";

                __result = text;
            }
        }
    }

    [HarmonyPatch(typeof(Tameable), nameof(Tameable.Interact))]
    static class VillagerTameInteract
    {
        public static void Postfix(Tameable __instance, ref Humanoid user, ref bool hold, ref bool alt,
            ref bool __result)
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
                case "KukuVillager_Bronze_Warlord_Set":
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
            ZDOID villagerZDOID = VillagerGeneral.SELECTED_VILLAGER_ID;
            if (villagerZDOID.IsNone() == false)
            {
              AssignBed(villagerZDOID,__instance);
              VillagerGeneral.SELECTED_VILLAGER_ID = ZDOID.None;
              VillagerGeneral.SELECTED_VILLAGERS_ID = null;
              MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,
                  $"Container Assigned to {VillagerGeneral.GetName(villagerZDOID)}");
            }
            else if (VillagerGeneral.SELECTED_VILLAGERS_ID != null && VillagerGeneral.SELECTED_VILLAGERS_ID.Count > 0)
            {
                foreach (var v in VillagerGeneral.SELECTED_VILLAGERS_ID)
                {
                    AssignBed(v,__instance);
                }
                VillagerGeneral.SELECTED_VILLAGER_ID = ZDOID.None;
                VillagerGeneral.SELECTED_VILLAGERS_ID = null;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,
                    $"Container Assigned to a bunch of villagers.");
            }
        }

        private static void AssignBed(ZDOID villagerZDIOD,Container __instance)
        {
            ZNetView containerZNV = __instance.GetComponentInParent<ZNetView>();
            var containerZDO = containerZNV.GetZDO();
            //VERY IMPORTANT. NECESSARY TO CREATE INVENTORY
            containerZDO.Set("m_name", __instance.m_name);
            containerZDO.Set("width", __instance.m_width);
            containerZDO.Set("height", __instance.m_height);
            VillagerGeneral.AssignContainer(villagerZDIOD,
                containerZNV.GetZDO().m_uid);
           
        }
    }


    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.GetCurrentWeapon))]
    static class VillagerDamageModifier
    {
        public static void Postfix(Humanoid __instance, ref ItemDrop.ItemData __result,
            ref ItemDrop.ItemData ___m_rightItem, ref ItemDrop.ItemData ___m_leftItem, ref ItemDrop ___m_unarmedWeapon)
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

                    if (___m_leftItem != null && ___m_leftItem.IsWeapon() &&
                        ___m_leftItem.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Torch)
                    {
                        weapon = ___m_leftItem;
                    }

                    if (___m_unarmedWeapon)
                    {
                        weapon = ___m_unarmedWeapon.m_itemData;
                    }

                    if (weapon != null)
                    {
                        if (weapon.m_shared.m_name.Contains("Health") || weapon.m_shared.m_name.Contains("Shield") ||
                            weapon.m_shared.m_name.Contains("Heal"))
                        {
                            weapon.m_shared.m_damages = new HitData.DamageTypes
                            {
                                m_damage = 0,
                                m_slash = 0,
                                m_blunt = 0,
                                m_chop = 0,
                                m_fire = 0,
                                m_frost = 0,
                                m_lightning = 0,
                                m_pickaxe = 0,
                                m_pierce = 0,
                                m_poison = 0,
                                m_spirit = 0
                            };
                        }
                        else
                        {
                            weapon.m_shared.m_damages = new HitData.DamageTypes
                            {
                                m_damage = villagerGeneral.GetDamage(),
                                m_slash = villagerGeneral.GetSlash(),
                                m_blunt = villagerGeneral.GetBlunt(),
                                m_chop = villagerGeneral.GetChop(),
                                m_fire = villagerGeneral.GetFire(),
                                m_frost = villagerGeneral.GetFrost(),
                                m_lightning = villagerGeneral.Getlightning(),
                                m_pickaxe = villagerGeneral.GetPickaxe(),
                                m_pierce = villagerGeneral.GetPickaxe(),
                                m_poison = villagerGeneral.GetPoison(),
                                m_spirit = villagerGeneral.GetSpirit()
                            };
                        }

                        __result = weapon;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}