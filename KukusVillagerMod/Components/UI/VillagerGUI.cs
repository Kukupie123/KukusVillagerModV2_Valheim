﻿using Jotunn.Managers;
using KukusVillagerMod.Components.Villager;
using KukusVillagerMod.enums.Work_Enum;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace KukusVillagerMod.Components.UI
{
    enum VUITab
    {
        Stats,
        ItemAssignment,
        Orders
    }

    static class VillagerGUI
    {
        static GameObject MAINBG; //The root UI component
        public static ZDOID selected_villager = ZDOID.None; //The villager we are interacting with

        private static VUITab currentTab = VUITab.Stats; //The current tab we are at

        static readonly List<GameObject> SubUIs = new List<GameObject>(); //Store all the child UI components

        //Remove all child components
        private static void RemoveSubUIs()
        {
            foreach (var v in SubUIs)
            {
                Object.Destroy(v);
            }
        }

        //Sets up essential UI that Persistent first, then sets up tab based UI
        private static void UpdateUI()
        {
            //Disable root ui first
            if (MAINBG != null)
            {
                MAINBG.SetActive(false);
                GUIManager.BlockInput(false);
            }

            RemoveSubUIs(); //Remove existing UI
            SetupEssentialUI(); //Setup UI again, first the basics

            //Setup UI again, now tab based UI components
            switch (currentTab)
            {
                case VUITab.Stats:
                    SetupVillagerStatUITab();
                    break;
                case VUITab.Orders:
                    SetupVillagerOrderTab();
                    break;
                case VUITab.ItemAssignment:
                    SetupVillagerItemAssignmentTab();
                    break;
            }

            //View the UI
            MAINBG.SetActive(true);
            GUIManager.BlockInput(true);
        }

        private static void SetupVillagerStatUITab()
        {
            string hpText = $"Max HP : {VillagerGeneral.GetStatHealth(selected_villager)}";
            var villager = ZNetScene.instance.FindInstance(selected_villager);
            if (villager)
            {
                var villagerGen = villager.GetComponent<VillagerGeneral>();
                if (villagerGen)
                {
                    hpText = $"HP : {villagerGen.GetAIHP()}/{villagerGen.GetStatHealth()}";
                }
            }

            GameObject HealthTextGO = GUIManager.Instance.CreateText(
                text: hpText,
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(-200f, 100f), // width & height
                width: 250f,
                height: 60f,
                color: Color.yellow,
                outline: false,
                outlineColor: Color.white,
                font: GUIManager.Instance.AveriaSerif,
                fontSize: 20,
                addContentSizeFitter: false
            );
            SubUIs.Add(HealthTextGO);

            GameObject EfficiencyTextBtn = GUIManager.Instance.CreateText(
                text: $"Efficiency : {VillagerGeneral.GetEfficiency(selected_villager)}",
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(-200f, 50f), // width & height
                width: 250f,
                height: 60f,
                color: Color.yellow,
                outline: false,
                outlineColor: Color.white,
                font: GUIManager.Instance.AveriaSerif,
                fontSize: 20,
                addContentSizeFitter: false
            );
            SubUIs.Add(EfficiencyTextBtn);

            GameObject SpawnRegion = GUIManager.Instance.CreateText(
                text: $"Spawn Region: {VillagerGeneral.GetVillagerSpawnRegion(selected_villager)}",
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(-200f, -100f), // width & height
                width: 250f,
                height: 60f,
                color: Color.yellow,
                outline: false,
                outlineColor: Color.white,
                font: GUIManager.Instance.AveriaSerif,
                fontSize: 20,
                addContentSizeFitter: false
            );
            SubUIs.Add(SpawnRegion);

            GameObject WorkSkillPickup = GUIManager.Instance.CreateText(
                text: $"Work Skill: {VillagerGeneral.GetWorkSkill(selected_villager)}",
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(-200f, -150f), // width & height
                width: 250f,
                height: 60f,
                color: Color.yellow,
                outline: false,
                outlineColor: Color.white,
                font: GUIManager.Instance.AveriaSerif,
                fontSize: 15,
                addContentSizeFitter: false
            );
            SubUIs.Add(WorkSkillPickup);


            Tuple<HitData.DamageType, float> specialSKill = VillagerGeneral.GetSpecialSkill(selected_villager);
            if (specialSKill != null)
            {
                GameObject SpecialDmgBtn = GUIManager.Instance.CreateText(
                    text: $"Special Damage : {specialSKill.Item1} ({specialSKill.Item2})",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.1f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(200f, 100f), // width & height
                    width: 250f,
                    height: 60f,
                    color: Color.white,
                    outline: false,
                    outlineColor: Color.yellow,
                    font: GUIManager.Instance.AveriaSerif,
                    fontSize: 20,
                    addContentSizeFitter: false
                );
                SubUIs.Add(SpecialDmgBtn);
            }

            GameObject DmgTextBtn = GUIManager.Instance.CreateText(
                text: $"Damage : {VillagerGeneral.GetDamage(selected_villager)}",
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(200f, 50f), // width & height
                width: 250f,
                height: 60f,
                color: Color.yellow,
                outline: false,
                outlineColor: Color.white,
                font: GUIManager.Instance.AveriaSerif,
                fontSize: 20,
                addContentSizeFitter: false
            );
            SubUIs.Add(DmgTextBtn);

            GameObject SlashTextBtn = GUIManager.Instance.CreateText(
                text: $"Slash : {VillagerGeneral.GetSlash(selected_villager)}",
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(200f, 0f), // width & height
                width: 250f,
                height: 60f,
                color: Color.yellow,
                outline: false,
                outlineColor: Color.white,
                font: GUIManager.Instance.AveriaSerif,
                fontSize: 20,
                addContentSizeFitter: false
            );
            SubUIs.Add(SlashTextBtn);

            GameObject BluntTextBtn = GUIManager.Instance.CreateText(
                text: $"Blunt : {VillagerGeneral.GetBlunt(selected_villager)}",
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(200f, -50f), // width & height
                width: 250f,
                height: 60f,
                color: Color.yellow,
                outline: false,
                outlineColor: Color.white,
                font: GUIManager.Instance.AveriaSerif,
                fontSize: 20,
                addContentSizeFitter: false
            );
            SubUIs.Add(BluntTextBtn);

            GameObject PierceTextbtn = GUIManager.Instance.CreateText(
                text: $"Pierce : {VillagerGeneral.GetPierce(selected_villager)}",
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(200f, -100f), // width & height
                width: 250f,
                height: 60f,
                color: Color.yellow,
                outline: false,
                outlineColor: Color.white,
                font: GUIManager.Instance.AveriaSerif,
                fontSize: 20,
                addContentSizeFitter: false
            );
            SubUIs.Add(PierceTextbtn);

            GameObject UpgradeBtn = GUIManager.Instance.CreateButton(
                text: $"Upgrade villager",
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(200f, -60f), // width & height
                width: 250f,
                height: 60f
            );
            SubUIs.Add(UpgradeBtn);
            var btn = UpgradeBtn.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                if (!VillagerGeneral.IsVillagerTamed(selected_villager))
                {
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft,
                        "Villager has not been recruited yet.");
                    return;
                }

                KLog.info($"Upgrading villager : {selected_villager.m_id}");

                Inventory playerInv = Player.m_localPlayer.GetInventory();
                bool upgrade = false;
                float multiplier = 1;

                List<ItemDrop.ItemData> usedItems = new List<ItemDrop.ItemData>();
                foreach (var item in playerInv.GetAllItems())
                {
                    string itemName = item.m_shared.m_name;
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
                        VillagerGeneral.UpgradeVillagerHealth(selected_villager, multiplier);
                        usedItems.Add(item);
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft,
                            $"Upgrading Health with {multiplier} multiplier");
                        break;
                    }

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
                        VillagerGeneral.UpgradeVillagerDamage(selected_villager, multiplier);
                        usedItems.Add(item);
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft,
                            $"Upgrading Damage with {multiplier} Multiplier");
                        break;
                    }
                }

                foreach (var i in usedItems)
                {
                    playerInv.RemoveItem(i, 1);
                    UpdateUI();
                }
            });
        }

        private static void SetupVillagerOrderTab()
        {
            if (!VillagerGeneral.IsVillagerTamed(selected_villager))
            {
                var villager = ZNetScene.instance.FindInstance(selected_villager);
                if (villager == null)
                {
                    return;
                }

                VillagerGeneral vg = villager.GetComponent<VillagerGeneral>();
                if (vg == null) return;

                GameObject Recruit = GUIManager.Instance.CreateButton(
                    text: $"Recruit Villager with {vg.goldToRecruit} Coins",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.1f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(0f, 100f), // width & height
                    width: 250f,
                    height: 60f
                );
                SubUIs.Add(Recruit);
                Recruit.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Inventory playerInv = Player.m_localPlayer.GetInventory();

                    int coinCount = 0;
                    ItemDrop.ItemData gold = null;
                    foreach (var i in playerInv.GetAllItems())
                    {
                        if (i.m_shared.m_name.Equals("$item_coins"))
                        {
                            coinCount += i.m_stack;

                            if (coinCount >= vg.goldToRecruit)
                            {
                                gold = i;
                                break;
                            }
                        }
                    }

                    if (gold != null && coinCount >= vg.goldToRecruit)
                    {
                        if (playerInv.RemoveItem(gold, vg.goldToRecruit))
                        {
                            vg.TameVillager();
                        }
                        else
                        {
                            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,
                                "Failed to recruit villager");
                        }
                    }

                    UpdateUI();
                });
            }
            else
            {
                //LEFT
                GameObject FollowMeBtn = GUIManager.Instance.CreateButton(
                    text: "Follow Me",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.1f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(-200f, 200f), // Left Top
                    width: 250f,
                    height: 60f
                );
                SubUIs.Add(FollowMeBtn);
                FollowMeBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    GameObject villagerGO = VillagerGeneral.GetVillagerInstance(selected_villager);
                    if (!villagerGO)
                    {
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Villager is too far away");
                        return;
                    }

                    VillagerAI ai = villagerGO.GetComponent<VillagerAI>();
                    if (ai == null) return;
                    ai.FollowPlayer(Player.m_localPlayer.GetZDOID());
                    KLog.info($"Villager is following");
                    CloseVillagerMenu();
                });

                GameObject GuardBedBtn = GUIManager.Instance.CreateButton(
                    text: "Guard Bed",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.1f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(-200f, 150f), // LEFT MID
                    width: 250f,
                    height: 60f
                );
                SubUIs.Add(GuardBedBtn);
                GuardBedBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    GameObject villagerGO = VillagerGeneral.GetVillagerInstance(selected_villager);
                    if (!villagerGO)
                    {
                        VillagerGeneral.SetVillagerState(selected_villager, enums.VillagerState.Guarding_Bed);
                        return;
                    }

                    VillagerAI ai = villagerGO.GetComponent<VillagerAI>();
                    if (ai == null) return;
                    ai.GuardBed();
                    KLog.info($"Villager is Guarding Bed");
                    CloseVillagerMenu();
                });


                //RIGHT
                GameObject DefendPostBtn = GUIManager.Instance.CreateButton(
                    text: "Defend Post",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.1f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(200f, 200f), // width & height
                    width: 250f,
                    height: 60f
                );
                SubUIs.Add(DefendPostBtn);
                DefendPostBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    GameObject villagerGO = VillagerGeneral.GetVillagerInstance(selected_villager);
                    if (!villagerGO)
                    {
                        VillagerGeneral.SetVillagerState(selected_villager, enums.VillagerState.Defending_Post);
                        return;
                    }

                    VillagerAI ai = villagerGO.GetComponent<VillagerAI>();
                    if (ai == null) return;
                    ai.DefendPost();
                    KLog.info($"Defend Post");
                    CloseVillagerMenu();
                });

                GameObject WorkBtn = GUIManager.Instance.CreateButton(
                    text: "Start Working",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.1f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(200f, 150f), // width & height
                    width: 250f,
                    height: 60f
                );
                SubUIs.Add(WorkBtn);
                WorkBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    GameObject villagerGO = VillagerGeneral.GetVillagerInstance(selected_villager);
                    if (!villagerGO)
                    {
                        VillagerGeneral.SetVillagerState(selected_villager, enums.VillagerState.Working);
                        return;
                    }

                    VillagerAI ai = villagerGO.GetComponent<VillagerAI>();
                    if (ai == null) return;
                    ai.StartWork();
                    KLog.info($"Villager is Starting to Work");
                    CloseVillagerMenu();
                });

                GameObject RoamBtn = GUIManager.Instance.CreateButton(
                    text: "Roam",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.1f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(200f, 100f), // width & height
                    width: 250f,
                    height: 60f
                );
                SubUIs.Add(RoamBtn);
                RoamBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    GameObject villagerGO = VillagerGeneral.GetVillagerInstance(selected_villager);
                    if (!villagerGO)
                    {
                        VillagerGeneral.SetVillagerState(selected_villager, enums.VillagerState.Roaming);
                        return;
                    }
                    else
                    {
                        VillagerAI ai = villagerGO.GetComponent<VillagerAI>();
                        ai.RoamAround();
                    }

                    KLog.info($"Villager is Roaming Around");
                    CloseVillagerMenu();
                });


                //WORK SKILLS
                var workSkillLDropDown = GUIManager.Instance.CreateDropDown(
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.1f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(-200, 100f),
                    fontSize: 20,
                    width: 250f,
                    height: 40f);
                SubUIs.Add(workSkillLDropDown);
                var dropdownComp = workSkillLDropDown.GetComponent<Dropdown>();

                dropdownComp.AddOptions(
                    new List<string> { "Pickup items", "Fill Smelters", "Chop Wood", "Repair Base" }
                );

                dropdownComp.onValueChanged.AddListener((val) =>
                {
                    switch (val)
                    {
                        case 0:
                            KLog.info($"{selected_villager.id} work skill is pickup");
                            VillagerGeneral.SetWorkSkill(selected_villager, WorkSkill.Pickup);
                            break;
                        case 1:
                            KLog.info($"{selected_villager.id} work skill is Fill Smelters");
                            VillagerGeneral.SetWorkSkill(selected_villager, WorkSkill.Fill_Smelt);
                            break;
                        case 2:
                            KLog.info($"{selected_villager.id} work skill is Chop Wood");
                            VillagerGeneral.SetWorkSkill(selected_villager, WorkSkill.Chop_Wood);
                            break;
                        case 3:
                            KLog.info($"{selected_villager.id} work skill is Repair Base");
                            VillagerGeneral.SetWorkSkill(selected_villager, WorkSkill.RepairBase);
                            break;
                    }
                });
            }
        }

        private static void SetupVillagerItemAssignmentTab()
        {
            if (!VillagerGeneral.IsVillagerTamed(selected_villager))
            {
                var villager = ZNetScene.instance.FindInstance(selected_villager);
                if (villager == null)
                {
                    return;
                }

                VillagerGeneral vg = villager.GetComponent<VillagerGeneral>();
                if (vg == null) return;

                GameObject Recruit = GUIManager.Instance.CreateButton(
                    text: $"Recruit Villager with {vg.goldToRecruit} Coins",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.1f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(0f, 100f), // width & height
                    width: 250f,
                    height: 60f
                );
                SubUIs.Add(Recruit);
                Recruit.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Inventory playerInv = Player.m_localPlayer.GetInventory();

                    int coinCount = 0;
                    ItemDrop.ItemData gold = null;
                    foreach (var i in playerInv.GetAllItems())
                    {
                        if (i.m_shared.m_name.Equals("$item_coins"))
                        {
                            coinCount += i.m_stack;

                            if (coinCount >= vg.goldToRecruit)
                            {
                                gold = i;
                                break;
                            }
                        }
                    }

                    if (gold != null && coinCount >= vg.goldToRecruit)
                    {
                        if (playerInv.RemoveItem(gold, vg.goldToRecruit))
                        {
                            vg.TameVillager();
                        }
                        else
                        {
                            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,
                                "Failed to recruit villager");
                        }
                    }

                    UpdateUI();
                });
            }
            else
            {
                GameObject AssignBedBtn = GUIManager.Instance.CreateButton(
                    text: "Assign (Bed, Defense Post, Work Post, Container)",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.1f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(0f, -50f),
                    width: 250f,
                    height: 60f
                );
                SubUIs.Add(AssignBedBtn);
                AssignBedBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    VillagerGeneral.SELECTED_VILLAGER_ID = selected_villager;
                    VillagerGeneral.SELECTED_VILLAGERS_ID = null;
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,
                        $"Interact with a Bed/Defense Post/Work Post/Container to assign it to {VillagerGeneral.GetName(selected_villager)}");
                    UpdateUI();
                    CloseVillagerMenu();
                });

                GameObject BedText = GUIManager.Instance.CreateText(
                    text: $"Assigned Bed ID : {VillagerGeneral.GetBedZDOID(selected_villager).id}",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(-200f, 150f), // width & height
                    width: 250f,
                    height: 60f,
                    color: Color.black,
                    outline: true,
                    outlineColor: Color.white,
                    font: GUIManager.Instance.AveriaSerif,
                    fontSize: 15,
                    addContentSizeFitter: false
                );
                SubUIs.Add(BedText);

                GameObject DPText = GUIManager.Instance.CreateText(
                    text: $"Assigned Defense Post ID : {VillagerGeneral.GetDefenseZDOID(selected_villager).id}",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(200f, 150f), // width & height
                    width: 250f,
                    height: 60f,
                    color: Color.black,
                    outline: true,
                    outlineColor: Color.white,
                    font: GUIManager.Instance.AveriaSerif,
                    fontSize: 15,
                    addContentSizeFitter: false
                );
                DPText.GetComponent<Text>().alignment = TextAnchor.UpperRight;
                SubUIs.Add(DPText);

                GameObject WPText = GUIManager.Instance.CreateText(
                    text: $"Faction: {VillagerGeneral.GetVillagerFaction(selected_villager)}",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(-200f, -50f), // width & height
                    width: 250f,
                    height: 60f,
                    color: Color.black,
                    outline: true,
                    outlineColor: Color.white,
                    font: GUIManager.Instance.AveriaSerif,
                    fontSize: 15,
                    addContentSizeFitter: false
                );
                SubUIs.Add(WPText);

                GameObject FactionText = GUIManager.Instance.CreateText(
                    text: $"Assigned Work Post ID : {VillagerGeneral.GetWorkPostZDOID(selected_villager).id}",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(-200f, 50f), // width & height
                    width: 250f,
                    height: 60f,
                    color: Color.black,
                    outline: true,
                    outlineColor: Color.white,
                    font: GUIManager.Instance.AveriaSerif,
                    fontSize: 15,
                    addContentSizeFitter: false
                );
                SubUIs.Add(FactionText);

                GameObject ContainerText = GUIManager.Instance.CreateText(
                    text: $"Assigned Container ID : {VillagerGeneral.GetContainerZDOID(selected_villager).id}",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(200f, 50f), // width & height
                    width: 250f,
                    height: 60f,
                    color: Color.black,
                    outline: true,
                    outlineColor: Color.white,
                    font: GUIManager.Instance.AveriaSerif,
                    fontSize: 15,
                    addContentSizeFitter: false
                );
                ContainerText.GetComponent<Text>().alignment = TextAnchor.UpperRight;
                SubUIs.Add(ContainerText);

                GameObject SetFactionBtn = GUIManager.Instance.CreateInputField(
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(-200f, -80f), // width & height
                    width: 250f,
                    height: 60f,
                    fontSize: 15,
                    placeholderText: VillagerGeneral.GetName(selected_villager)
                );
                var factionInputField = SetFactionBtn.GetComponent<InputField>();
                factionInputField.text = VillagerGeneral.GetVillagerFaction(selected_villager);
                factionInputField.onEndEdit.AddListener((factionName) =>
                {
                    if (factionName.Trim().Length <= 0)
                    {
                        VillagerGeneral.SetVillagerFaction(selected_villager, "None");
                    }
                    else
                    {
                        VillagerGeneral.SetVillagerFaction(selected_villager, factionName);
                    }

                    UpdateUI();
                });
                SubUIs.Add(SetFactionBtn);
            }
        }

        private static void SetupEssentialUI()
        {
            if (MAINBG == null)
            {
                MAINBG = GUIManager.Instance.CreateWoodpanel(
                    parent: GUIManager.CustomGUIFront.transform,
                    anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(0, 0),
                    width: 850,
                    height: 600,
                    draggable: false);

                //Hide it
                MAINBG.SetActive(false);
            }

            //NAME OF VILLAGER
            GameObject inputField = GUIManager.Instance.CreateInputField(
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0f, 150f), // width & height
                width: 250f,
                height: 60f,
                fontSize: 30,
                placeholderText: VillagerGeneral.GetName(selected_villager)
            );
            inputField.GetComponent<InputField>().onEndEdit.AddListener((name) =>
            {
                GameObject villager = ZNetScene.instance.FindInstance(selected_villager);
                if (villager)
                {
                    villager.GetComponent<VillagerGeneral>().SetName(name);
                    UpdateUI();
                }
                else
                {
                    VillagerGeneral.SetName(selected_villager, name);
                    UpdateUI();
                }
            });

            //Close button
            GameObject closeBtn = GUIManager.Instance.CreateButton(
                text: "Close",
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0, -250f),
                width: 250f,
                height: 60f
            );
            SubUIs.Add(closeBtn);
            //Add listener to the button
            Button button = closeBtn.GetComponent<Button>();
            button.onClick.AddListener(CloseVillagerMenu);


            //TABS


            //Add Stats Tabs button
            GameObject StatsTabBtn = GUIManager.Instance.CreateButton(
                text: "Stats",
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(-250, 250f),
                width: 250f,
                height: 60f
            );
            SubUIs.Add(StatsTabBtn);
            StatsTabBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                currentTab = VUITab.Stats;
                UpdateUI();
            });

            //Add Items Assignments
            GameObject ItemAssignmentBtn = GUIManager.Instance.CreateButton(
                text: "Items Assignment",
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0, 250f),
                width: 250f,
                height: 60f
            );
            SubUIs.Add(ItemAssignmentBtn);
            ItemAssignmentBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                currentTab = VUITab.ItemAssignment;
                UpdateUI();
            });

            //Add order tab
            GameObject OrderTabBtn = GUIManager.Instance.CreateButton(
                text: "Orders",
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(250, 250f),
                width: 250f,
                height: 60f
            );
            SubUIs.Add(OrderTabBtn);
            OrderTabBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                currentTab = VUITab.Orders;
                UpdateUI();
            });
        }


        //Called by interacting with a villager
        public static void OnShowMenu(ZDOID villagerZDOID)
        {
            selected_villager = villagerZDOID;
            if (GUIManager.Instance == null)
            {
                return;
            }

            if (!GUIManager.CustomGUIFront)
            {
                return;
            }

            if (!Util.ValidateZDOID(selected_villager))
            {
                return;
            }

            UpdateUI();
        }

        private static void CloseVillagerMenu()
        {
            //Hide Main component
            if (MAINBG != null) MAINBG.SetActive(false);

            //Reset tab
            currentTab = VUITab.Stats;

            //Remove sub components
            RemoveSubUIs();
            selected_villager = ZDOID.None;
            //Destroy main component
            Object.Destroy(MAINBG);
            GUIManager.BlockInput(false);
        }
    }
}