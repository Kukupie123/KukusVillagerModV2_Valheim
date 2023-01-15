﻿using HarmonyLib;
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
                string finalText = "";

                //Health
                finalText = $"{finalText}\nHealth : { vls.GetHealth(true)}";

                //Efficiency
                float efficiency = (vls.GetEfficiency() * 100.0f);

                //Special skill
                finalText = $"{finalText}\nEfficiency : {(int)efficiency}";
                Tuple<HitData.DamageType, float> specialSkill = vls.GetSpecialSkill();
                if (specialSkill != null)
                {
                    finalText = $"{finalText}\nSkill : {specialSkill.Item1} ({specialSkill.Item2})";
                }

                //Damage stats
                finalText = $"{finalText}\nDamage : {vls.GetDamage()}";
                finalText = $"{finalText}\nSlash : {vls.GetSlash()}";
                finalText = $"{finalText}\nBlunt : {vls.GetBlunt()}";
                finalText = $"{finalText}\nPierce : {vls.GetPierce()}";
                __result = finalText;
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
                    //Save name, width, height in zdo for use in villager AI to load inventory without container instance
                    var znv = __instance.GetComponentInParent<ZNetView>();
                    znv.GetZDO().Set("m_name", __instance.m_name);
                    znv.GetZDO().Set("width", __instance.m_width);
                    znv.GetZDO().Set("height", __instance.m_height);
                    BedVillagerProcessor.SELECTED_BED_ID = null;
                }
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
