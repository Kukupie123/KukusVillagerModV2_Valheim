using Jotunn.Managers;
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
        VillagersList,
        Commands
    }

    class KuchukGUI
    {
        private const float height = 600f;
        private const float width = 850f;
        private static GameObject MainBG;
        private static List<GameObject> SubUis = new List<GameObject>();
        private static KUITab currentTab = KUITab.VillagersList;

        private static string activeFaction;

        //for villagers list tab
        static List<ZDO>
            tamedVillagers =
                new List<ZDO>(); //stored outside because when we switch pages we still need to keep existing villagers list

        private static List<string> UniqueFactions = new List<string>();
        static int villagerStartingIndex = 0; //for switching villagers
        private static int factionsStartingIndex = 0; //for switching factions

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

            //Active faction notice :
            string activeFaction = "All";
            if (KuchukGUI.activeFaction != null)
            {
                activeFaction = KuchukGUI.activeFaction;
            }

            GameObject currentFactionText = GUIManager.Instance.CreateText(
                text: $"Selected faction : {activeFaction}",
                parent: MainBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0f, 250f), // width & height
                width: 250f,
                height: 60f,
                color: Color.yellow,
                outline: false,
                outlineColor: Color.white,
                font: GUIManager.Instance.AveriaSerif,
                fontSize: 17,
                addContentSizeFitter: false
            );
            SubUis.Add(currentFactionText);
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
            tamedVillagers.Clear();
            villagerStartingIndex = 0;
            //Reset tab
            currentTab = KUITab.VillagersList;

            //Remove sub components
            RemoveSubUIs();

            //Destroy main component
            UnityEngine.GameObject.Destroy(MainBG);
            GUIManager.BlockInput(false);
        }

        static int listSize = 4;
        private static int listSizeFac = listSize - 1;

        private static List<ZDO> GetTamedVillagers(string faction = null)
        {
            KLog.warning($"Getting villagers of {(faction == null ? "All" : faction)}");
            List<ZDO> meadow1 = new List<ZDO>();
            List<ZDO> meadow2 = new List<ZDO>();
            List<ZDO> bf1 = new List<ZDO>();
            List<ZDO> bf2 = new List<ZDO>();
            List<ZDO> mountain1 = new List<ZDO>();
            List<ZDO> mountain2 = new List<ZDO>();
            List<ZDO> plains1 = new List<ZDO>();
            List<ZDO> plains2 = new List<ZDO>();
            List<ZDO> plains3 = new List<ZDO>();
            List<ZDO> mist1 = new List<ZDO>();
            List<ZDO> mist2 = new List<ZDO>();
            List<ZDO> mist3 = new List<ZDO>();

            ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Meadow1", meadow1);
            ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Meadow2", meadow2);
            ZDOMan.instance.GetAllZDOsWithPrefab("Villager_BF1", bf1);
            ZDOMan.instance.GetAllZDOsWithPrefab("Villager_BF2", bf2);
            ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Mountain1", mountain1);
            ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Mountain2", mountain2);
            ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Plains1", plains1);
            ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Plains2", plains2);
            ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Plains3", plains3);
            ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Mist1", mist1);
            ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Mist2", mist2);
            ZDOMan.instance.GetAllZDOsWithPrefab("Villager_Mist3", mist3);
            List<ZDO> mainList = new List<ZDO>();
            if (faction != null)
            {
                foreach (var v in meadow1)
                {
                    if (VillagerGeneral.GetVillagerFaction(v.m_uid).Equals(faction))
                    {
                        mainList.Add(v);
                    }
                }

                foreach (var v in meadow2)
                {
                    if (VillagerGeneral.GetVillagerFaction(v.m_uid).Equals(faction))
                    {
                        mainList.Add(v);
                    }
                }

                foreach (var v in bf1)
                {
                    if (VillagerGeneral.GetVillagerFaction(v.m_uid).Equals(faction))
                    {
                        mainList.Add(v);
                    }
                }

                foreach (var v in bf2)
                {
                    if (VillagerGeneral.GetVillagerFaction(v.m_uid).Equals(faction))
                    {
                        mainList.Add(v);
                    }
                }

                foreach (var v in mountain1)
                {
                    if (VillagerGeneral.GetVillagerFaction(v.m_uid).Equals(faction))
                    {
                        mainList.Add(v);
                    }
                }

                foreach (var v in mountain2)
                {
                    if (VillagerGeneral.GetVillagerFaction(v.m_uid).Equals(faction))
                    {
                        mainList.Add(v);
                    }
                }

                foreach (var v in plains1)
                {
                    if (VillagerGeneral.GetVillagerFaction(v.m_uid).Equals(faction))
                    {
                        mainList.Add(v);
                    }
                }

                foreach (var v in plains2)
                {
                    if (VillagerGeneral.GetVillagerFaction(v.m_uid).Equals(faction))
                    {
                        mainList.Add(v);
                    }
                }

                foreach (var v in plains3)
                {
                    if (VillagerGeneral.GetVillagerFaction(v.m_uid).Equals(faction))
                    {
                        mainList.Add(v);
                    }
                }

                foreach (var v in mist1)
                {
                    if (VillagerGeneral.GetVillagerFaction(v.m_uid).Equals(faction))
                    {
                        mainList.Add(v);
                    }
                }

                foreach (var v in mist2)
                {
                    if (VillagerGeneral.GetVillagerFaction(v.m_uid).Equals(faction))
                    {
                        mainList.Add(v);
                    }
                }

                foreach (var v in mist3)
                {
                    if (VillagerGeneral.GetVillagerFaction(v.m_uid).Equals(faction))
                    {
                        mainList.Add(v);
                    }
                }
            }
            else
            {
                mainList.AddRange(meadow1);
                mainList.AddRange(meadow2);
                mainList.AddRange(bf1);
                mainList.AddRange(bf2);
                mainList.AddRange(mountain1);
                mainList.AddRange(mountain2);
                mainList.AddRange(plains1);
                mainList.AddRange(plains2);
                mainList.AddRange(plains3);
                mainList.AddRange(mist1);
                mainList.AddRange(mist2);
                mainList.AddRange(mist3);
            }


            List<ZDO> tamed = new List<ZDO>();
            foreach (ZDO z in mainList)
            {
                try
                {
                    if (!Util.ValidateZDO(z) || !Util.ValidateZDOID(z.m_uid) || z.m_uid.id == 0)
                    {
                    }
                    else
                    {
                        if (VillagerGeneral.IsVillagerTamed(z.m_uid))
                        {
                            tamed.Add(z);
                        }
                    }
                }
                catch (Exception e)
                {
                    KLog.warning($"{e.Message} in kuchuk GUI find villager");
                }
            }

            return tamed;
        }

        private static List<string> GetFactions(List<ZDO> villagers)
        {
            List<string> factions = new List<string>();

            foreach (var v in villagers)
            {
                string faction = VillagerGeneral.GetVillagerFaction(v.m_uid);
                if (!factions.Contains(faction))
                {
                    factions.Add(faction);
                }
            }

            return factions;
        }

        private static void SetupVillagersListTab(bool findVillagersAgain = false)
        {
            //Scan for villagers only if we need to
            if (findVillagersAgain)
            {
                villagerStartingIndex = 0; //reset page count
                factionsStartingIndex = 0;

                tamedVillagers = GetTamedVillagers();

                UniqueFactions = GetFactions(tamedVillagers);
            }


            void GoBack(ref int currentPageNo)
            {
                currentPageNo -= listSize; //list size
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

                currentPageNo += listSize; //If item exists for showing in next page we proceed.
                if (currentPageNo >= villagerList.Count - 1)
                {
                    currentPageNo = villagerList.Count - 1;
                }

                KLog.info("Going fwd to page: " + currentPageNo);
            }

            GameObject tamedVillagersList = GUIManager.Instance.CreateText(
                text: $"Recruited Villagers:",
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
            SubUis.Add(tamedVillagersList);

            GenerateVillagersList(tamedVillagers, villagerStartingIndex, -200f);

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
                GoBack(ref villagerStartingIndex);
                UpdateUI();
            });
            goFwdBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                GoFwd(tamedVillagers, ref villagerStartingIndex);
                UpdateUI();
            });


            //Faction list
            void GoBackFac(ref int currentPageNo)
            {
                currentPageNo -= listSizeFac; //list size
                if (currentPageNo <= 0)
                {
                    currentPageNo = 0;
                }

                KLog.info("Going back to page : " + currentPageNo);
            }

            void GoFwdFac(List<string> factionList, ref int currentPageNo)
            {
                //Is there item for the next page? Eg. If we have 4 items and we show 4 in one list. There is none left to show so now rangedVillagerListstartingIndex = 8 but count is 4 so we know it's already shown.
                if (factionList.Count - 1 < currentPageNo + listSizeFac)
                {
                    return;
                }

                currentPageNo += listSizeFac; //If item exists for showing in next page we proceed.
                if (currentPageNo >= factionList.Count - 1)
                {
                    currentPageNo = factionList.Count - 1;
                }

                KLog.info("Going fwd to page: " + currentPageNo);
            }


            GameObject FactionsList = GUIManager.Instance.CreateText(
                text: $"Factions:",
                parent: MainBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(200f, 250f), // width & height
                width: 250f,
                height: 60f,
                color: Color.yellow,
                outline: false,
                outlineColor: Color.white,
                font: GUIManager.Instance.AveriaSerif,
                fontSize: 20,
                addContentSizeFitter: false
            );
            SubUis.Add(FactionsList);
            GenerateFactionsList(UniqueFactions, factionsStartingIndex, 200f);
            //Add next and last page button
            GameObject goBackButtonFac = GUIManager.Instance.CreateButton(
                text: $"<",
                parent: MainBG.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(150, -200),
                width: 100f,
                height: 60f
            );
            SubUis.Add(goBackButtonFac);
            GameObject goFwdBtnFac = GUIManager.Instance.CreateButton(
                text: $">",
                parent: MainBG.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(250f, -200),
                width: 100f,
                height: 60f
            );
            SubUis.Add(goFwdBtnFac);
            goBackButtonFac.GetComponent<Button>().onClick.AddListener(() =>
            {
                GoBackFac(ref factionsStartingIndex);
                UpdateUI();
            });
            goFwdBtnFac.GetComponent<Button>().onClick.AddListener(() =>
            {
                GoFwdFac(UniqueFactions, ref factionsStartingIndex);
                UpdateUI();
            });
        }


        private static void GenerateVillagersList(List<ZDO> villagerList, int startingIndex, float pos)
        {
            int endingIndex = startingIndex + listSize;
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

            for (int i = startingIndex; i < endingIndex; i++)
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

        private static void GenerateFactionsList(List<string> factionsList, int startingIndex, float pos)
        {
            int endingIndex = startingIndex + listSizeFac;
            float startingY = 200f;
            //If we exceed ending index we need to adjust ending index to the last index of the list
            if ((factionsList.Count - 1) < endingIndex)
            {
                endingIndex = factionsList.Count;
            }


            for (int i = startingIndex; i < endingIndex; i++)
            {
                string factionName = factionsList[i];
                //Villager button
                GameObject villagerBtn = GUIManager.Instance.CreateButton(
                    text: $"{factionName}",
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
                        activeFaction = factionName;
                        UpdateUI();
                    }
                );
                startingY -= 100;
            }

            GameObject allVillagerBtn = GUIManager.Instance.CreateButton(
                text: $"All",
                parent: MainBG.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(pos, startingY),
                width: 250f,
                height: 60f
            );
            SubUis.Add(allVillagerBtn);
            allVillagerBtn.GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    activeFaction = null;
                    UpdateUI();
                }
            );
        }

        private static void SetupVillagerOrderTab()
        {
            tamedVillagers = GetTamedVillagers(activeFaction);
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

            GameObject AssignBtn = GUIManager.Instance.CreateButton(
                text: "Assign Item",
                parent: MainBG.transform,
                anchorMin: new Vector2(0.5f, 0.1f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(-200f, 50f), // LEFT LOW
                width: 250f,
                height: 60f
            );
            SubUis.Add(AssignBtn);
            AssignBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                VillagerGeneral.SELECTED_VILLAGER_ID = ZDOID.None;
                List<ZDOID> selectedVillagers = new List<ZDOID>();
                GetTamedVillagers(activeFaction).ForEach(z => selectedVillagers.Add(z.m_uid));

                VillagerGeneral.SELECTED_VILLAGERS_ID = selectedVillagers;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,
                    $"Faction {activeFaction}. Interact with WorkPost/DefensePost/Container to assign it to the group.");
                CloseMenu();
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