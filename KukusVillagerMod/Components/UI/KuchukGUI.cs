﻿using Jotunn.Managers;
using KukusVillagerMod.Components.Villager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace KukusVillagerMod.Components.UI
{
    enum KUITab
    {
        VillagersList, Commands
    }

    class KuchukGUI
    {
        private const float heightSubUI = 60f;
        private const float widthSubUI = 250f;
        private const float height = 600f;
        private const float width = 850f;
        private static GameObject MainBG;
        private static List<GameObject> SubUis = new List<GameObject>();
        private static KUITab currentTab = KUITab.VillagersList;

        //for villagers list tab
        static List<ZDO> tamedRangedVillagers = new List<ZDO>(); //stored outside because when we switch pages we still need to keep existing villagers list
        static List<ZDO> tamedMeleeVillagers = new List<ZDO>();
        static int rangedVillagerListstartingIndex = 0; //for switching pages
        static int meleeVillagerListstartingIndex = 0; //for switching pages

        public static void ShowMenu()
        {
            if (GUIManager.Instance == null) return;
            if (!GUIManager.CustomGUIFront) return;
            UpdateUI(true);
        }
        private static void UpdateUI(bool findVillagersAgain = false)
        {
            //Disable root ui first
            if (MainBG != null)
            {
                MainBG.SetActive(false);
                GUIManager.BlockInput(false);
            }
            RemoveSubUIs();
            SetupEssentialUI();
            switch (currentTab)
            {
                case KUITab.VillagersList:
                    SetupVillagersListTab(findVillagersAgain);
                    break;
                case KUITab.Commands:
                    SetupVillagerOrderTab();
                    break;
            }
            MainBG.SetActive(true);
            GUIManager.BlockInput(true);
        }
        private static void RemoveSubUIs()
        {
            foreach (var v in SubUis)
            {
                UnityEngine.GameObject.Destroy(v);
            }
        }
        private static void SetupEssentialUI()
        {
            if (!MainBG)
            {
                MainBG = GUIManager.Instance.CreateWoodpanel(
            parent: GUIManager.CustomGUIFront.transform,
            anchorMin: new Vector2(0.5f, 0.5f),
            anchorMax: new Vector2(0.5f, 0.5f),
            position: new Vector2(0, 0),
            width: width,
            height: height,
            draggable: false);

            }
            MainBG.SetActive(false);
            //Close button
            GameObject closeBtn = GUIManager.Instance.CreateButton(
                text: "Close",
                parent: MainBG.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0, -250f),
                width: 250f,
                height: 60f
                );
            SubUis.Add(closeBtn);
            //Add listener to the button
            Button button = closeBtn.GetComponent<Button>();
            button.onClick.AddListener(CloseMenu);

            GameObject UniversalCommandBtn = GUIManager.Instance.CreateButton(
                text: "Universal Command",
                parent: MainBG.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(200, -250f),
                width: 250f,
                height: 60f
                );
            SubUis.Add(UniversalCommandBtn);

            //Add listener to the button
            UniversalCommandBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                currentTab = KUITab.Commands;
                UpdateUI(true);
            });

            var villagersListbtn = GUIManager.Instance.CreateButton(
                text: "View Villagers",
                parent: MainBG.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(-200, -250f),
                width: 250f,
                height: 60f
                );
            SubUis.Add(villagersListbtn);

            //Add listener to the button
            villagersListbtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                currentTab = KUITab.VillagersList;
                UpdateUI(true);
            });

        }
        private static void CloseMenu()
        {

            //Hide Main component
            if (MainBG != null) MainBG.SetActive(false);
            tamedMeleeVillagers.Clear();
            tamedRangedVillagers.Clear();
            meleeVillagerListstartingIndex = 0;
            rangedVillagerListstartingIndex = 0;
            //Reset tab
            currentTab = KUITab.VillagersList;

            //Remove sub components
            RemoveSubUIs();

            //Destroy main component
            UnityEngine.GameObject.Destroy(MainBG);
            GUIManager.BlockInput(false);
        }

        static int listSize = 4;
        private static void SetupVillagersListTab(bool findVillagersAgain = false)
        {
            List<ZDO> foundRangedVillagers = new List<ZDO>();
            List<ZDO> foundMeleeVillagers = new List<ZDO>();

            //Scan for villagers only if we need to
            if (findVillagersAgain)
            {
                tamedMeleeVillagers.Clear();
                tamedRangedVillagers.Clear();
                rangedVillagerListstartingIndex = 0; //reset page count
                meleeVillagerListstartingIndex = 0;
                ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Ranged", foundRangedVillagers);
                ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Melee", foundMeleeVillagers);
                foreach (ZDO z in foundRangedVillagers)
                {
                    try
                    {
                        if (!Util.ValidateZDO(z) || !Util.ValidateZDOID(z.m_uid) || z.m_uid.id == 0) { }
                        else
                        {
                            if (VillagerGeneral.IsVillagerTamed(z.m_uid))
                            {
                                tamedRangedVillagers.Add(z);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        KLog.warning($"{e.Message} in kuchuk GUI find villager");
                        continue;
                    }

                }
                foreach (ZDO z in foundMeleeVillagers)
                {
                    try
                    {
                        if (!Util.ValidateZDO(z) || !Util.ValidateZDOID(z.m_uid) || z.m_uid.id == 0) { }
                        else
                        {
                            if (VillagerGeneral.IsVillagerTamed(z.m_uid))
                            {
                                tamedMeleeVillagers.Add(z);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        KLog.warning($"{e.Message} in kuchuk GUI");
                        continue;
                    }

                }
            }


            void GoBack(ref int currentPageNo)
            {
                currentPageNo = currentPageNo - listSize; //list size
                if (currentPageNo <= 0)
                {
                    currentPageNo = 0;
                }
                KLog.info("Going back to page : " + currentPageNo);
            }
            void GoFwd(List<ZDO> villagerList, ref int currentPageNo)
            {
                //Is there item for the next page? Eg. If we have 4 items and we show 4 in one list. There is none left to show so now rangedVillagerListstartingIndex = 8 but count is 4 so we know it's already shown.
                if (villagerList.Count - 1 < currentPageNo + listSize)
                {
                    return;
                }
                currentPageNo += 4; //If item exists for showing in next page we proceed.
                if (currentPageNo >= villagerList.Count - 1)
                {
                    currentPageNo = villagerList.Count - 1;
                }
                KLog.info("Going fwd to page: " + currentPageNo);
            }


            //RANGED VILLAGERS LIST

            GameObject RangedVillagersList = GUIManager.Instance.CreateText(
                text: $"Ranged Villagers:",
                parent: MainBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(-200f, 250f), // width & height
                width: 250f,
                height: 60f,
                color: Color.yellow,
                outline: false,
                outlineColor: Color.white,
                font: GUIManager.Instance.AveriaSerif,
                fontSize: 20,
                addContentSizeFitter: false
                );
            SubUis.Add(RangedVillagersList);

            GenerateVillagersList(tamedRangedVillagers, rangedVillagerListstartingIndex, -200f);

            //Add next and last page button
            GameObject goBackButton = GUIManager.Instance.CreateButton(
                    text: $"<",
                    parent: MainBG.transform,
                    anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(-250, -200),
                    width: 100f,
                    height: 60f
                    );
            SubUis.Add(goBackButton);
            GameObject goFwdBtn = GUIManager.Instance.CreateButton(
                    text: $">",
                    parent: MainBG.transform,
                    anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(-150f, -200),
                    width: 100f,
                    height: 60f
                    );
            SubUis.Add(goFwdBtn);
            goBackButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                GoBack(ref rangedVillagerListstartingIndex);
                UpdateUI();
            });
            goFwdBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                GoFwd(tamedRangedVillagers, ref rangedVillagerListstartingIndex);
                UpdateUI();
            });

            //Melee villagers list
            GameObject MeleeVillagersText = GUIManager.Instance.CreateText(
               text: $"Melee Villagers",
               parent: MainBG.transform,
               anchorMin: new Vector2(0.5f, 0.1f),
               anchorMax: new Vector2(0.5f, 0.5f),
               position: new Vector2(250f, 250f), // width & height
               width: 250f,
               height: 60f,
               color: Color.yellow,
               outline: false,
               outlineColor: Color.white,
               font: GUIManager.Instance.AveriaSerif,
               fontSize: 20,
               addContentSizeFitter: false
               );
            MeleeVillagersText.GetComponent<Text>().alignment = TextAnchor.UpperRight;
            SubUis.Add(MeleeVillagersText);

            GenerateVillagersList(tamedMeleeVillagers, meleeVillagerListstartingIndex, 200f);

            //Add next and last page button
            GameObject goBackButtonMelee = GUIManager.Instance.CreateButton(
                    text: $"<",
                    parent: MainBG.transform,
                    anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(150, -200),
                    width: 100f,
                    height: 60f
                    );
            SubUis.Add(goBackButtonMelee);
            GameObject goFwdBtnMelee = GUIManager.Instance.CreateButton(
                    text: $">",
                    parent: MainBG.transform,
                    anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(250f, -200),
                    width: 100f,
                    height: 60f
                    );
            SubUis.Add(goFwdBtnMelee);
            goBackButtonMelee.GetComponent<Button>().onClick.AddListener(() =>
            {
                GoBack(ref meleeVillagerListstartingIndex);
                UpdateUI();
            });
            goFwdBtnMelee.GetComponent<Button>().onClick.AddListener(() =>
            {
                GoFwd(tamedMeleeVillagers, ref meleeVillagerListstartingIndex);
                UpdateUI();
            });

        }


        private static void GenerateVillagersList(List<ZDO> villagerList, int startindIndex, float pos)
        {
            int endingIndex = startindIndex + listSize;
            float startingY = 200f;
            //If we exceed ending index we need to adjust ending index to the last index of the list
            if ((villagerList.Count - 1) < endingIndex)
            {
                endingIndex = villagerList.Count;
            }

            foreach (var v in villagerList)
            {
                VillagerGeneral.GetName(v.m_uid);
            }

            for (int i = startindIndex; i < endingIndex; i++)
            {
                ZDOID zdoid = villagerList[i].m_uid;
                //Villager button
                GameObject villagerBtn = GUIManager.Instance.CreateButton(
                    text: $"{VillagerGeneral.GetName(villagerList[i].m_uid)}({villagerList[i].m_uid.id})",
                    parent: MainBG.transform,
                    anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(pos, startingY),
                    width: 250f,
                    height: 60f
                    );
                SubUis.Add(villagerBtn);
                villagerBtn.GetComponent<Button>().onClick.AddListener(
                   () =>
                   {
                       VillagerGUI.OnShowMenu(zdoid);
                       CloseMenu();
                   }
                   );
                startingY = startingY - 100;
            }
        }

        private static void SetupVillagerOrderTab()
        {
            List<ZDO> foundRangedVillagers = new List<ZDO>();
            List<ZDO> foundMeleeVillagers = new List<ZDO>();

            List<ZDO> tamedVillagers = new List<ZDO>();

            ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Ranged", foundRangedVillagers);
            ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Melee", foundMeleeVillagers);
            foreach (ZDO z in foundRangedVillagers)
            {
                try
                {
                    if (!Util.ValidateZDO(z) || !Util.ValidateZDOID(z.m_uid) || z.m_uid.id == 0) { }
                    else
                    {
                        if (VillagerGeneral.IsVillagerTamed(z.m_uid))
                        {
                            tamedVillagers.Add(z);
                        }
                    }

                }
                catch (Exception e)
                {
                    KLog.warning($"{e.Message} in kuchuk GUI find villager");
                    continue;
                }

            }
            foreach (ZDO z in foundMeleeVillagers)
            {
                try
                {
                    if (!Util.ValidateZDO(z) || !Util.ValidateZDOID(z.m_uid) || z.m_uid.id == 0) { }
                    else
                    {
                        if (VillagerGeneral.IsVillagerTamed(z.m_uid))
                        {
                            tamedVillagers.Add(z);
                        }
                    }

                }
                catch (Exception e)
                {
                    KLog.warning($"{e.Message} in kuchuk GUI");
                    continue;
                }

            }


            //LEFT
            GameObject FollowMeBtn = GUIManager.Instance.CreateButton(
              text: "Follow Me(Only nearby villagers)",
              parent: MainBG.transform,
              anchorMin: new Vector2(0.5f, 0.1f),
              anchorMax: new Vector2(0.5f, 0.5f),
              position: new Vector2(-200f, 200f), // Left Top
              width: 250f,
              height: 60f
              );
            SubUis.Add(FollowMeBtn);
            FollowMeBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                foreach (var v in tamedVillagers)
                {
                    var villager = ZNetScene.instance.FindInstance(v.m_uid);
                    if (villager)
                    {
                        villager.GetComponent<VillagerAI>().FollowPlayer(Player.m_localPlayer.GetZDOID());
                    }

                }

            });

            GameObject GuardBedBtn = GUIManager.Instance.CreateButton(
              text: "Guard Bed",
              parent: MainBG.transform,
              anchorMin: new Vector2(0.5f, 0.1f),
              anchorMax: new Vector2(0.5f, 0.5f),
              position: new Vector2(-200f, 150f), // LEFT MID
              width: 250f,
              height: 60f
              );
            SubUis.Add(GuardBedBtn);
            GuardBedBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                foreach (var v in tamedVillagers)
                {
                    var villager = ZNetScene.instance.FindInstance(v.m_uid);
                    if (villager)
                    {
                        villager.GetComponent<VillagerAI>().GuardBed();
                    }
                    else
                    {
                        VillagerGeneral.SetVillagerState(v.m_uid, enums.VillagerState.Guarding_Bed);
                    }

                }
            });



            GameObject DefendPostBtn = GUIManager.Instance.CreateButton(
              text: "Defend Post",
              parent: MainBG.transform,
              anchorMin: new Vector2(0.5f, 0.1f),
              anchorMax: new Vector2(0.5f, 0.5f),
              position: new Vector2(200f, 200f), // width & height
              width: 250f,
              height: 60f
              );
            SubUis.Add(DefendPostBtn);
            DefendPostBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                foreach (var v in tamedVillagers)
                {
                    var villager = ZNetScene.instance.FindInstance(v.m_uid);
                    if (villager)
                    {
                        villager.GetComponent<VillagerAI>().DefendPost();
                    }
                    else
                    {
                        VillagerGeneral.SetVillagerState(v.m_uid, enums.VillagerState.Defending_Post);
                    }

                }
            });

            GameObject WorkBtn = GUIManager.Instance.CreateButton(
             text: "Start Working",
             parent: MainBG.transform,
             anchorMin: new Vector2(0.5f, 0.1f),
             anchorMax: new Vector2(0.5f, 0.5f),
             position: new Vector2(200f, 150f), // width & height
             width: 250f,
             height: 60f
             );
            SubUis.Add(WorkBtn);
            WorkBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                foreach (var v in tamedVillagers)
                {
                    var villager = ZNetScene.instance.FindInstance(v.m_uid);
                    if (villager)
                    {
                        villager.GetComponent<VillagerAI>().StartWork();
                    }
                    else
                    {
                        VillagerGeneral.SetVillagerState(v.m_uid, enums.VillagerState.Working);
                    }

                }
            });

            GameObject RoamBtn = GUIManager.Instance.CreateButton(
             text: "Roam",
             parent: MainBG.transform,
             anchorMin: new Vector2(0.5f, 0.1f),
             anchorMax: new Vector2(0.5f, 0.5f),
             position: new Vector2(200f, 50f), // width & height
             width: 250f,
             height: 60f
             );
            SubUis.Add(RoamBtn);
            RoamBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                foreach (var v in tamedVillagers)
                {
                    var villager = ZNetScene.instance.FindInstance(v.m_uid);
                    if (villager)
                    {
                        villager.GetComponent<VillagerAI>().RoamAround();
                    }
                    else
                    {
                        VillagerGeneral.SetVillagerState(v.m_uid, enums.VillagerState.Roaming);
                    }

                }
            });




        }
    }
}