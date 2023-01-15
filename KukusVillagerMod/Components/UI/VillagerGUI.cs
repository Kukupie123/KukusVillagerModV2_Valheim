﻿using Jotunn.GUI;
using Jotunn.Managers;
using KukusVillagerMod.Components.Villager;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KukusVillagerMod.Components.UI
{
    enum VUITab
    {
        Stats, ItemAssignment, Orders
    }
    class VillagerGUI
    {
        static GameObject MAINBG; //The root UI component
        private static ZDOID selected_villager = ZDOID.None; //The villager we are interacting with

        private static VUITab currentTab = VUITab.Stats; //The current tab we are at

        static List<GameObject> SubUIs = new List<GameObject>(); //Store all the child UI components

        //Remove all child components
        private static void RemoveSubUIs()
        {
            foreach (var v in SubUIs)
            {
                UnityEngine.GameObject.Destroy(v);
            }
        }

        //Sets up essential UI that persisnt first, then sets up tab based UI
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
                default:
                    break;
            }

            //View the UI
            MAINBG.SetActive(true);
            GUIManager.BlockInput(true);
        }

        private static void SetupVillagerStatUITab()
        {


            GameObject HealthTextGO = GUIManager.Instance.CreateText(
                text: $"Max Health : {VillagerGeneral.GetHealth(selected_villager)}",
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
                text: $"Efficiency : {(int)(VillagerGeneral.GetEfficiency(selected_villager) * 100)}%",
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

            string mining = "";

            switch (VillagerGeneral.GetMiningLevel(selected_villager))
            {
                case 0: mining = "Newbie"; break;
                case 1: mining = "Rokie"; break;
                case 2: mining = "Iron Arms"; break;
                case 3: mining = "Veteran"; break;
            }

            GameObject MiningLvlTextBtn = GUIManager.Instance.CreateText(
                text: $"Work Level : {mining}",
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(-200f, 0f), // width & height
                width: 250f,
                height: 60f,
                color: Color.yellow,
                outline: false,
                outlineColor: Color.white,
                font: GUIManager.Instance.AveriaSerif,
                fontSize: 20,
                addContentSizeFitter: false
                );
            SubUIs.Add(MiningLvlTextBtn);


            GameObject ChoppingSkillBtn = GUIManager.Instance.CreateText(
               text: $"Chopping Skill : {VillagerGeneral.GetChop(selected_villager)}",
               parent: MAINBG.transform,
               anchorMin: new Vector2(0.5f, 0.1f),
               anchorMax: new Vector2(0.5f, 0.5f),
               position: new Vector2(-200f, -50f), // width & height
               width: 250f,
               height: 60f,
               color: Color.yellow,
               outline: false,
               outlineColor: Color.white,
               font: GUIManager.Instance.AveriaSerif,
               fontSize: 20,
               addContentSizeFitter: false
               );

            SubUIs.Add(ChoppingSkillBtn);


            GameObject MiningSkillBtn = GUIManager.Instance.CreateText(
               text: $"Mining Skill : {VillagerGeneral.GetPickaxe(selected_villager)}",
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
            SubUIs.Add(MiningSkillBtn);

            GameObject WorkSkillPickup = GUIManager.Instance.CreateText(
             text: $"Can Do Pickup work : {VillagerGeneral.GetWorkSkill_Pickup(selected_villager)}",
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

            GameObject WorkSkillSmelt = GUIManager.Instance.CreateText(
            text: $"Can Do Smelt Fillup work : {VillagerGeneral.GetWorkSkill_Smelter(selected_villager)}",
            parent: MAINBG.transform,
            anchorMin: new Vector2(0.5f, 0.1f),
            anchorMax: new Vector2(0.5f, 0.5f),
            position: new Vector2(-200f, -200f), // width & height
            width: 250f,
            height: 60f,
            color: Color.yellow,
            outline: false,
            outlineColor: Color.white,
            font: GUIManager.Instance.AveriaSerif,
            fontSize: 15,
            addContentSizeFitter: false
            );
            SubUIs.Add(WorkSkillSmelt);

            /*
             *  weapon.m_shared.m_damages = new HitData.DamageTypes();
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
             */
            //Damage

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

        }
        private static void SetupVillagerOrderTab()
        {
            if (!VillagerGeneral.IsVillagerTamed(selected_villager))
            {
                GameObject Recruit = GUIManager.Instance.CreateButton(
                    text: "Recruit Villager",
                    parent: MAINBG.transform,
                    anchorMin: new Vector2(0.5f, 0.1f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(-200f, 100f), // width & height
                    width: 250f,
                    height: 60f
                    );
                SubUIs.Add(Recruit);
                Recruit.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var villager = ZNetScene.instance.FindInstance(selected_villager);
                    if (villager == null)
                    {
                        VillagerGeneral.TameVillager(selected_villager);
                        return;
                    }
                    VillagerGeneral vg = villager.GetComponent<VillagerGeneral>();
                    vg.TameVillager();
                    UpdateUI();
                    return;
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
                FollowMeBtn.GetComponent<Button>().onClick.AddListener(() => { KLog.info($"Villager is following"); });

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
                GuardBedBtn.GetComponent<Button>().onClick.AddListener(() => { KLog.info($"Villager is Guarding Bed"); });



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
                DefendPostBtn.GetComponent<Button>().onClick.AddListener(() => { KLog.info($"Defend Post"); });

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
                WorkBtn.GetComponent<Button>().onClick.AddListener(() => { KLog.info($"Working  Post"); });


                //WORK SKILLS
                GameObject WorkSkill = GUIManager.Instance.CreateText(
                 text: "Work Skills",
                 parent: MAINBG.transform,
                 anchorMin: new Vector2(0.5f, 0.1f),
                 anchorMax: new Vector2(0.5f, 0.5f),
                 position: new Vector2(0f, 70f), // width & height
                 width: 250f,
                 height: 60f,
                 font: GUIManager.Instance.AveriaSerifBold,
                 color: Color.black,
                 fontSize: 25,
                 outline: false,
                 outlineColor: Color.black,
                 addContentSizeFitter: false
                 );
                WorkSkill.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                SubUIs.Add(WorkSkill);
                WorkBtn.GetComponent<Button>().onClick.AddListener(() => { KLog.info($"Working  Post"); });


                if (VillagerGeneral.GetWorkSkill_Pickup(selected_villager))
                {
                    GameObject pickupWorkBtn = GUIManager.Instance.CreateButton(
                 text: "Disable Pickup Work Skill",
                 parent: MAINBG.transform,
                 anchorMin: new Vector2(0.5f, 0.1f),
                 anchorMax: new Vector2(0.5f, 0.5f),
                 position: new Vector2(200f, 50f), // width & height
                 width: 250f,
                 height: 60f
                 );
                    SubUIs.Add(pickupWorkBtn);
                    pickupWorkBtn.GetComponent<Button>().onClick.AddListener(() => { VillagerGeneral.SetWorkSkill_Pickup(selected_villager, false); UpdateUI(); });
                }
                else
                {

                    GameObject pickupWorkBtn = GUIManager.Instance.CreateButton(
                 text: "Enable Pickup Work Skill",
                 parent: MAINBG.transform,
                 anchorMin: new Vector2(0.5f, 0.1f),
                 anchorMax: new Vector2(0.5f, 0.5f),
                 position: new Vector2(200f, 50f), // width & height
                 width: 250f,
                 height: 60f
                 );
                    SubUIs.Add(pickupWorkBtn);
                    pickupWorkBtn.GetComponent<Button>().onClick.AddListener(() => { VillagerGeneral.SetWorkSkill_Pickup(selected_villager, true);UpdateUI(); });
                }
                if (VillagerGeneral.GetWorkSkill_Smelter(selected_villager))
                {

                    GameObject pickupWorkBtn = GUIManager.Instance.CreateButton(
                 text: "Disable Smelter Fillup Work Skill",
                 parent: MAINBG.transform,
                 anchorMin: new Vector2(0.5f, 0.1f),
                 anchorMax: new Vector2(0.5f, 0.5f),
                 position: new Vector2(-200f, 50f), // width & height
                 width: 250f,
                 height: 60f
                 );
                    SubUIs.Add(pickupWorkBtn);
                    pickupWorkBtn.GetComponent<Button>().onClick.AddListener(() => { VillagerGeneral.SetWorkSkill_Smelter(selected_villager, false); UpdateUI(); });
                }
                else
                {
                    GameObject pickupWorkBtn = GUIManager.Instance.CreateButton(
                text: "Enable Smelter Fillup Work Skill",
                parent: MAINBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(-200f, 50f), // width & height
                width: 250f,
                height: 60f
                );
                    SubUIs.Add(pickupWorkBtn);
                    pickupWorkBtn.GetComponent<Button>().onClick.AddListener(() => { VillagerGeneral.SetWorkSkill_Smelter(selected_villager, true); UpdateUI(); });
                }
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
            //Name text
            GameObject NameTextGO = GUIManager.Instance.CreateText(
                text: VillagerGeneral.GetName(selected_villager),
                parent: MAINBG.transform,
                 anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0f, 200f), // width & height
                width: 250f,
                height: 60f,
                color: Color.black,
                outline: true,
                outlineColor: Color.white,
                font: GUIManager.Instance.AveriaSerif,
                fontSize: 30,
                addContentSizeFitter: false
                );
            NameTextGO.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            SubUIs.Add(NameTextGO);
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
            StatsTabBtn.GetComponent<Button>().onClick.AddListener(() => { currentTab = VUITab.Stats; UpdateUI(); });

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
            ItemAssignmentBtn.GetComponent<Button>().onClick.AddListener(() => { currentTab = VUITab.ItemAssignment; UpdateUI(); });

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
            OrderTabBtn.GetComponent<Button>().onClick.AddListener(() => { currentTab = VUITab.Orders; UpdateUI(); });

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

            //Destroy main component
            UnityEngine.GameObject.Destroy(MAINBG);
            GUIManager.BlockInput(false);
        }
    }
}
