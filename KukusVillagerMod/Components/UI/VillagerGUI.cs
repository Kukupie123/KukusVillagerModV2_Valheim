using Jotunn.GUI;
using Jotunn.Managers;
using KukusVillagerMod.Components.Villager;
using KukusVillagerMod.enums.Work_Enum;
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
        public static ZDOID selected_villager = ZDOID.None; //The villager we are interacting with

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
                case VUITab.ItemAssignment:
                    SetupVillagerItemAssignmentTab();
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

            string mining = "";

            switch (VillagerGeneral.GetWorkLevel(selected_villager))
            {
                case 0: mining = "Newbie"; break;
                case 1: mining = "Rokie"; break;
                case 2: mining = "Iron Arms"; break;
                case 3: mining = "Veteran"; break;
            }
            /*
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

            */



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
                    position: new Vector2(0f, 100f), // width & height
                    width: 250f,
                    height: 60f
                    );
                SubUIs.Add(Recruit);
                Recruit.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var villager = ZNetScene.instance.FindInstance(selected_villager);
                    if (villager == null)
                    {
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
                var workSkilLDropDown = GUIManager.Instance.CreateDropDown(
           parent: MAINBG.transform,
           anchorMin: new Vector2(0.5f, 0.1f),
           anchorMax: new Vector2(0.5f, 0.5f),
           position: new Vector2(-200, 100f),
           fontSize: 20,
            width: 250f,
           height: 40f);
                SubUIs.Add(workSkilLDropDown);
                var dropdownComp = workSkilLDropDown.GetComponent<Dropdown>();

                dropdownComp.AddOptions(
                     new List<string> { "Pickup items", "Fill Smelters", "Chop Wood" }
                     );

                dropdownComp.onValueChanged.AddListener((int val) =>
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
                    }
                });




                //Cut trees button
                /*
                GameObject cutTreeBtn = GUIManager.Instance.CreateButton(
               text: "Cut trees (WARNING! TEST_VERSION )",
               parent: MAINBG.transform,
               anchorMin: new Vector2(0.5f, 0.1f),
               anchorMax: new Vector2(0.5f, 0.5f),
               position: new Vector2(0f, 0f), // width & height
               width: 250f,
               height: 60f
               );m bu
                SubUIs.Add(cutTreeBtn);
                cutTreeBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var v = ZNetScene.instance.FindInstance(selected_villager);
                    if (v)
                    {
                        v.GetComponent<VillagerAI>().StartWork(true);
                        CloseVillagerMenu();
                    }
                    else
                    {
                        VillagerGeneral.SetVillagerState(selected_villager, enums.VillagerState.Mining);
                        CloseVillagerMenu();
                    }
                });
                */
            }
        }

        private static void SetupVillagerItemAssignmentTab()
        {
            if (!VillagerGeneral.IsVillagerTamed(selected_villager))
            {
                GameObject Recruit = GUIManager.Instance.CreateButton(
                    text: "Recruit Villager",
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
                GameObject AssignBedBtn = GUIManager.Instance.CreateButton(
                  text: "Assign (Bed, Defense Post, Work Post, Container)",
                  parent: MAINBG.transform,
                  anchorMin: new Vector2(0.5f, 0.1f),
                  anchorMax: new Vector2(0.5f, 0.5f),
                  position: new Vector2(0f, 100f),
                  width: 250f,
                  height: 60f
                  );
                SubUIs.Add(AssignBedBtn);
                AssignBedBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    VillagerGeneral.SELECTED_VILLAGER_ID = selected_villager;
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Interact with a Bed/Defense Post/Work Post/Container to assign it to {VillagerGeneral.GetName(selected_villager)}");
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
                SubUIs.Add(WPText);

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
            selected_villager = ZDOID.None;
            //Destroy main component
            UnityEngine.GameObject.Destroy(MAINBG);
            GUIManager.BlockInput(false);
        }
    }
}
