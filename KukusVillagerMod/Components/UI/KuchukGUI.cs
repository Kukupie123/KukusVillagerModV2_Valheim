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
        private static List<ZDOID> selectedVillagersList = new List<ZDOID>();
        private static KUITab currentTab = KUITab.VillagersList;

        //for villagers list tab
        static List<ZDO> tamedRangedVillagers = new List<ZDO>(); //stored outside because when we switch pages we still need to keep existing villagers list
        static List<ZDO> tamedMeleeVillagers = new List<ZDO>();
        static int rangedVillagerListstartingIndex = 0; //for switching pages

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
        }
        private static void CloseMenu()
        {

            //Hide Main component
            if (MainBG != null) MainBG.SetActive(false);

            //Reset tab
            currentTab = KUITab.VillagersList;

            //Remove sub components
            RemoveSubUIs();

            //Destroy main component
            UnityEngine.GameObject.Destroy(MainBG);
            GUIManager.BlockInput(false);
        }


        private static void SetupVillagersListTab(bool findVillagersAgain = false)
        {
            List<ZDO> foundRangedVillagers = new List<ZDO>();
            List<ZDO> foundMeleeVillagers = new List<ZDO>();

            //Scan for villagers only if we need to
            if (findVillagersAgain)
            {
                rangedVillagerListstartingIndex = 0; //reset page count
                ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Ranged", foundRangedVillagers);
                ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Melee", foundMeleeVillagers);
                foreach (ZDO z in foundRangedVillagers)
                {
                    try
                    {
                        if (!Util.ValidateZDO(z)) continue;
                        if (VillagerGeneral.IsVillagerTamed(z.m_uid))
                        {
                            tamedRangedVillagers.Add(z);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                }
                foreach (ZDO z in foundMeleeVillagers)
                {
                    try
                    {
                        if (!Util.ValidateZDO(z)) continue;
                        if (VillagerGeneral.IsVillagerTamed(z.m_uid))
                        {
                            tamedMeleeVillagers.Add(z);
                        }
                    }
                    catch (Exception e)
                    {
                        KLog.warning($"{e.Message} in kuchuk GUI");
                        continue;
                    }

                }
            }




            GameObject RangedVillagersList = GUIManager.Instance.CreateText(
                text: $"Ranged Villagers :",
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


            float startingY = 200f;
            int endingIndex = rangedVillagerListstartingIndex + 8;
            //If we exceed ending index we need to adjust ending index to the last index of the list
            if ((tamedRangedVillagers.Count - 1) < endingIndex)
            {
                endingIndex = tamedRangedVillagers.Count - 1;
            }
            for (int i = rangedVillagerListstartingIndex; i < endingIndex; i++)
            {
                //Villager button
                GameObject villagerBtn = GUIManager.Instance.CreateButton(
                    text: $"{VillagerGeneral.GetName(tamedRangedVillagers[i].m_uid)}({tamedRangedVillagers[i].m_uid.id})",
                    parent: MainBG.transform,
                    anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(-200, startingY),
                    width: 250f,
                    height: 60f
                    );
                SubUis.Add(villagerBtn);
                startingY = startingY - 50;
            }
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
                rangedVillagerListstartingIndex = rangedVillagerListstartingIndex - 7;
                if (rangedVillagerListstartingIndex < 0)
                {
                    rangedVillagerListstartingIndex = 0;
                }
                UpdateUI();
            });
            goFwdBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                if ((tamedRangedVillagers.Count - 1) < rangedVillagerListstartingIndex + 9)
                {
                    return;
                }
                rangedVillagerListstartingIndex = rangedVillagerListstartingIndex + 9;
                if (rangedVillagerListstartingIndex > tamedRangedVillagers.Count - 1)
                {
                    rangedVillagerListstartingIndex = tamedRangedVillagers.Count - 1;
                }
                KLog.warning("RangedVillager Staring count : " + rangedVillagerListstartingIndex);
                UpdateUI();
            });
        }
    }
}
