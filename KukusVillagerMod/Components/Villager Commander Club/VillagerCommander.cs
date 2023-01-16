using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Configuration;
using KukusVillagerMod.enums;
using System.Collections.Generic;
using UnityEngine;
using KukusVillagerMod.Components.Villager;
using KukusVillagerMod.Components.VillagerBed;
using KukusVillagerMod.Components.DefensePost;
using KukusVillagerMod.Components.UI;

/*
 * Commands
 * 1. All villagers go to Guard Bed
 * 2. All villagers go to work
 * 3. All villagers go to defense post
 * 4. Move Followers to aimed Location
 * 
 * 
 * This needs to see a massive change.
 * 1. List all the villagers you recruited.
 * 2. Selection of villagers and commanding them. or command all!
 * 3. Selection of villagers and editing their properties such as Work SKill, Defense Post, Work Post
 * 4. Follwoers action will still be button based such as call back and go there
 */

namespace KukusVillagerMod.itemPrefab
{
    class VillagerCommander
    {
        public VillagerCommander()
        {
            createCommanderPrefab();
            CreateEmptyKH();
        }

        CustomItem commander;
        private string name = "Runik Kuchuk Tablet";
        void createCommanderPrefab()
        {
            ItemConfig commanderConfig = new ItemConfig();
            commanderConfig.Name = name;
            commanderConfig.Description = $"";
            commanderConfig.CraftingStation = null;
            commanderConfig.AddRequirement(new RequirementConfig("Wood", 1, 0, false));
            commander = new CustomItem("Village_Commander", "Club", commanderConfig);
            ItemManager.Instance.AddItem(commander);
        }

        void createCommanderButtons()
        {

            guardBedbtn = new ButtonConfig();
            guardBedbtn.Name = "KP1";
            guardBedbtn.Hint = "Guard Bed";
            guardBedbtn.ActiveInCustomGUI = true;
            guardBedbtn.BlockOtherInputs = true;
            guardBedbtn.Key = UnityEngine.KeyCode.Keypad1;

            followPlayerBtn = new ButtonConfig();
            followPlayerBtn.Name = "KP2";
            followPlayerBtn.Hint = "Follow Player";
            followPlayerBtn.ActiveInCustomGUI = true;
            followPlayerBtn.BlockOtherInputs = true;
            followPlayerBtn.Key = UnityEngine.KeyCode.Keypad2;

            defendBtn = new ButtonConfig();
            defendBtn.Name = "KP3";
            defendBtn.Hint = "Defend Posts";
            defendBtn.ActiveInCustomGUI = true;
            defendBtn.BlockOtherInputs = true;
            defendBtn.Key = UnityEngine.KeyCode.Keypad3;

            deletePostsBtn = new ButtonConfig();
            deletePostsBtn.Name = "KP4";
            deletePostsBtn.Hint = "Remove Nearby Posts";
            deletePostsBtn.ActiveInCustomGUI = true;
            deletePostsBtn.BlockOtherInputs = true;
            deletePostsBtn.Key = UnityEngine.KeyCode.Keypad4;

            deleteVillagersBtn = new ButtonConfig();
            deleteVillagersBtn.Name = "KP5";
            deleteVillagersBtn.Hint = "Remove Nearby Posts";
            deleteVillagersBtn.ActiveInCustomGUI = true;
            deleteVillagersBtn.BlockOtherInputs = true;
            deleteVillagersBtn.Key = UnityEngine.KeyCode.Keypad5;

            guardBedbtn = new ButtonConfig();
            guardBedbtn.Name = "KP1";
            guardBedbtn.Hint = "Guard Bed";
            guardBedbtn.ActiveInCustomGUI = true;
            guardBedbtn.BlockOtherInputs = true;
            guardBedbtn.Key = UnityEngine.KeyCode.Keypad1;

            followPlayerBtn = new ButtonConfig();
            followPlayerBtn.Name = "KP2";
            followPlayerBtn.Hint = "Follow Player";
            followPlayerBtn.ActiveInCustomGUI = true;
            followPlayerBtn.BlockOtherInputs = true;
            followPlayerBtn.Key = UnityEngine.KeyCode.Keypad2;

            defendBtn = new ButtonConfig();
            defendBtn.Name = "KP3";
            defendBtn.Hint = "Defend Posts";
            defendBtn.ActiveInCustomGUI = true;
            defendBtn.BlockOtherInputs = true;
            defendBtn.Key = UnityEngine.KeyCode.Keypad3;

            deletePostsBtn = new ButtonConfig();
            deletePostsBtn.Name = "KP4";
            deletePostsBtn.Hint = "Remove Nearby Posts";
            deletePostsBtn.ActiveInCustomGUI = true;
            deletePostsBtn.BlockOtherInputs = true;
            deletePostsBtn.Key = UnityEngine.KeyCode.Keypad4;

            deleteVillagersBtn = new ButtonConfig();
            deleteVillagersBtn.Name = "KP5";
            deleteVillagersBtn.Hint = "Remove Nearby Villagers";
            deleteVillagersBtn.ActiveInCustomGUI = true;
            deleteVillagersBtn.BlockOtherInputs = true;
            deleteVillagersBtn.Key = UnityEngine.KeyCode.Keypad5;

            deleteBedBtn = new ButtonConfig();
            deleteBedBtn.Name = "KP6";
            deleteBedBtn.Hint = "Remove Nearby Beds";
            deleteBedBtn.ActiveInCustomGUI = true;
            deleteBedBtn.BlockOtherInputs = true;
            deleteBedBtn.Key = UnityEngine.KeyCode.Keypad6;

            showStatsBtn = new ButtonConfig();
            showStatsBtn.Name = "KP7";
            showStatsBtn.Hint = "Show Current Stats";
            showStatsBtn.ActiveInCustomGUI = true;
            showStatsBtn.BlockOtherInputs = true;
            showStatsBtn.Key = UnityEngine.KeyCode.Keypad7;

            InputManager.Instance.AddButton("com.jotunn.KukusVillagerMod", guardBedbtn);
            InputManager.Instance.AddButton("com.jotunn.KukusVillagerMod", followPlayerBtn);
            InputManager.Instance.AddButton("com.jotunn.KukusVillagerMod", defendBtn);
            InputManager.Instance.AddButton("com.jotunn.KukusVillagerMod", deletePostsBtn);
            InputManager.Instance.AddButton("com.jotunn.KukusVillagerMod", deleteVillagersBtn);
            InputManager.Instance.AddButton("com.jotunn.KukusVillagerMod", deleteBedBtn);
            InputManager.Instance.AddButton("com.jotunn.KukusVillagerMod", showStatsBtn);


        }

        void createKeyhints()
        {
            KeyHintConfig kh = new KeyHintConfig
            {
                Item = "Village_Commander",
                ButtonConfigs = new[] { guardBedbtn, followPlayerBtn, defendBtn, deletePostsBtn, deleteVillagersBtn, deleteBedBtn, showStatsBtn }
            };
            KeyHintManager.Instance.AddKeyHint(kh);
        }

        void CreateEmptyKH()
        {
            KeyHintConfig kh = new KeyHintConfig
            {
                Item = "Village_Commander",
                ButtonConfigs = new ButtonConfig[] { }
            };
            KeyHintManager.Instance.AddKeyHint(kh);
        }

        //Keeps track of which key has been pressed down.
        bool OpenMenuPressed = false;
        bool callFollowersKeyPressed = false;
        bool defendPostPressed = false;
        bool RandomRoamPressed = false;
        bool deleteVillagersPressed = false;
        bool deleteBedsPressed = false;
        bool moveToPressed = false;
        bool workKeyPressed = false;

        private ButtonConfig guardBedbtn;
        private ButtonConfig followPlayerBtn;
        private ButtonConfig defendBtn;
        private ButtonConfig deletePostsBtn;
        private ButtonConfig deleteVillagersBtn;
        private ButtonConfig deleteBedBtn;
        private ButtonConfig showStatsBtn;


        //This is not the right way to handle input but I couldn't get those to work so I ended up doing it this way
        public void HandleInputs()
        {


            if (ZInput.instance == null || MessageHud.instance == null || Player.m_localPlayer == null) return;
            if (Player.m_localPlayer.GetInventory() == null) return;
            List<ItemDrop.ItemData> allItems = Player.m_localPlayer.GetInventory().GetAllItems();


            if (allItems == null) return;

            //I tried foreach loop but it just wouldn't work so I looped using iterative numbers
            for (int i = 0; i < allItems.Count; i++)
            {
                if (Player.m_localPlayer.GetInventory().GetEquipedtems().Contains(allItems[i]))
                {
                    //I tried to avoid nesting this much but foreach would never loop so I ended up doing this. Planning to fix once major stuffs are done
                    foreach (ItemDrop.ItemData e in Player.m_localPlayer.GetInventory().GetEquipedtems())
                    {
                        if (e == null) continue;
                        if (allItems[i] == null) continue;

                        if (e == allItems[i] || e.Equals(allItems[i]))
                        {
                            if (e.TokenName().Equals(name))
                            {
                                if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.OpenMenuKey)
                                {


                                    //go to bed point
                                    if (OpenMenuPressed) return;
                                    OpenMenuPressed = true;
                                    callFollowersKeyPressed = false;
                                    defendPostPressed = false;
                                    RandomRoamPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = false;
                                    moveToPressed = false;
                                    workKeyPressed = false;
                                    KuchukGUI.ShowMenu();

                                }
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.CallFollowers)
                                {

                                    //Follow Player
                                    if (callFollowersKeyPressed) return;
                                    OpenMenuPressed = false;
                                    callFollowersKeyPressed = true;
                                    defendPostPressed = false;
                                    RandomRoamPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = false;
                                    moveToPressed = false;
                                    workKeyPressed = false;

                                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Calling back followers");
                                    MakeFollowersComeBack("Weak_Villager_Ranged");
                                    MakeFollowersComeBack("Weak_Villager");
                                    MakeFollowersComeBack("Bronze_Villager_Ranged");
                                    MakeFollowersComeBack("Bronze_Villager");
                                    MakeFollowersComeBack("Iron_Villager_Ranged");
                                    MakeFollowersComeBack("Iron_Villager");
                                    MakeFollowersComeBack("Silver_Villager");
                                    MakeFollowersComeBack("Silver_Villager_Ranged");
                                    MakeFollowersComeBack("BlackMetal_Villager_Ranged");
                                    MakeFollowersComeBack("BlackMetal_Villager");


                                }
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.defendPostKey)
                                {

                                    //Go defensive position
                                    if (defendPostPressed) return;
                                    OpenMenuPressed = false;
                                    callFollowersKeyPressed = false;
                                    defendPostPressed = true;
                                    RandomRoamPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = false;
                                    moveToPressed = false;
                                    workKeyPressed = false;

                                    MakeVillagersDefend("Weak_Villager_Ranged");
                                    MakeVillagersDefend("Weak_Villager");
                                    MakeVillagersDefend("Bronze_Villager_Ranged");
                                    MakeVillagersDefend("Bronze_Villager");
                                    MakeVillagersDefend("Iron_Villager_Ranged");
                                    MakeVillagersDefend("Iron_Villager");
                                    MakeVillagersDefend("Silver_Villager");
                                    MakeVillagersDefend("Silver_Villager_Ranged");
                                    MakeVillagersDefend("BlackMetal_Villager_Ranged");
                                    MakeVillagersDefend("BlackMetal_Villager");
                                }
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.RoamKey)
                                {
                                    //Roam around
                                    if (RandomRoamPressed) return;
                                    OpenMenuPressed = false;
                                    callFollowersKeyPressed = false;
                                    defendPostPressed = false;
                                    RandomRoamPressed = true;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = false;
                                    moveToPressed = false;
                                    workKeyPressed = false;

                                    MakeVillagersRoam("Weak_Villager_Ranged");
                                    MakeVillagersRoam("Weak_Villager");
                                    MakeVillagersRoam("Bronze_Villager_Ranged");
                                    MakeVillagersRoam("Bronze_Villager");
                                    MakeVillagersRoam("Iron_Villager_Ranged");
                                    MakeVillagersRoam("Iron_Villager");
                                    MakeVillagersRoam("Silver_Villager");
                                    MakeVillagersRoam("Silver_Villager_Ranged");
                                    MakeVillagersRoam("BlackMetal_Villager_Ranged");
                                    MakeVillagersRoam("BlackMetal_Villager");

                                }
                                else if (ZInput.instance.GetPressedKey().ToString() == "VillagerModConfigurations.deleteVillagerKey")
                                {


                                    //Destroy all villagers
                                    if (deleteVillagersPressed) return;
                                    OpenMenuPressed = false;
                                    callFollowersKeyPressed = false;
                                    defendPostPressed = false;
                                    RandomRoamPressed = false;
                                    deleteVillagersPressed = true;
                                    deleteBedsPressed = false;
                                    moveToPressed = false;
                                    workKeyPressed = false;

                                    foreach (VillagerGeneral v in UnityEngine.GameObject.FindObjectsOfType<VillagerGeneral>())
                                    {
                                        ZNetScene.instance.Destroy(v.gameObject);
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Destroying all Villagers");
                                    }

                                }
                                else if (ZInput.instance.GetPressedKey().ToString() == "VillagerModConfigurations.deleteBedsKey")
                                {

                                    if (deleteBedsPressed) return;
                                    OpenMenuPressed = false;
                                    callFollowersKeyPressed = false;
                                    defendPostPressed = false;
                                    RandomRoamPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = true;
                                    moveToPressed = false;
                                    workKeyPressed = false;

                                    foreach (BedState v in UnityEngine.GameObject.FindObjectsOfType<BedState>())
                                    {
                                        ZNetScene.instance.Destroy(v.gameObject);
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Destroying all Beds");
                                    }
                                }
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.moveToKey)
                                {
                                    if (moveToPressed) return;
                                    OpenMenuPressed = false;
                                    callFollowersKeyPressed = false;
                                    defendPostPressed = false;
                                    RandomRoamPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = false;
                                    moveToPressed = true;
                                    workKeyPressed = false;


                                    //Ray cast and see if that area is available
                                    Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
                                    RaycastHit hitData;
                                    if (Physics.Raycast(ray, out hitData, 5000f))
                                    {
                                        //Is area ready
                                        if (ZNetScene.instance.IsAreaReady(hitData.point) == false)
                                        {
                                            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "The Move area is too far");
                                            return;
                                        }

                                        MakeFollowersGoToLocation("Weak_Villager_Ranged", ZoneSystem.instance.GetRandomPointInRadius(hitData.point, 3f));
                                        MakeFollowersGoToLocation("Weak_Villager", ZoneSystem.instance.GetRandomPointInRadius(hitData.point, 3f));
                                        MakeFollowersGoToLocation("Bronze_Villager_Ranged", ZoneSystem.instance.GetRandomPointInRadius(hitData.point, 3f));
                                        MakeFollowersGoToLocation("Bronze_Villager", ZoneSystem.instance.GetRandomPointInRadius(hitData.point, 3f));
                                        MakeFollowersGoToLocation("Iron_Villager_Ranged", ZoneSystem.instance.GetRandomPointInRadius(hitData.point, 3f));
                                        MakeFollowersGoToLocation("Iron_Villager", ZoneSystem.instance.GetRandomPointInRadius(hitData.point, 3f));
                                        MakeFollowersGoToLocation("Silver_Villager", ZoneSystem.instance.GetRandomPointInRadius(hitData.point, 3f));
                                        MakeFollowersGoToLocation("Silver_Villager_Ranged", ZoneSystem.instance.GetRandomPointInRadius(hitData.point, 3f));
                                        MakeFollowersGoToLocation("BlackMetal_Villager_Ranged", ZoneSystem.instance.GetRandomPointInRadius(hitData.point, 3f));
                                        MakeFollowersGoToLocation("BlackMetal_Villager", ZoneSystem.instance.GetRandomPointInRadius(hitData.point, 3f));
                                    }



                                }
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.WorkKey)
                                {
                                    if (workKeyPressed) return;
                                    OpenMenuPressed = false;
                                    callFollowersKeyPressed = false;
                                    defendPostPressed = false;
                                    RandomRoamPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = false;
                                    moveToPressed = false;
                                    workKeyPressed = true;

                                    MakeVillagersWork("Weak_Villager_Ranged");
                                    MakeVillagersWork("Weak_Villager");
                                    MakeVillagersWork("Bronze_Villager_Ranged");
                                    MakeVillagersWork("Bronze_Villager");
                                    MakeVillagersWork("Iron_Villager_Ranged");
                                    MakeVillagersWork("Iron_Villager");
                                    MakeVillagersWork("Silver_Villager");
                                    MakeVillagersWork("Silver_Villager_Ranged");
                                    MakeVillagersWork("BlackMetal_Villager_Ranged");
                                    MakeVillagersWork("BlackMetal_Villager");


                                }
                                else
                                {
                                    OpenMenuPressed = false;
                                    callFollowersKeyPressed = false;
                                    defendPostPressed = false;
                                    RandomRoamPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = false;
                                    moveToPressed = false;
                                    workKeyPressed = false;
                                }

                            }
                        }
                    }
                }
            }
        }

        ///Make all villager go to their bed.
        private void MakeVillagersGoToBed(string prefabName)
        {


            List<ZDO> zdos = new List<ZDO>(); //Store ZDOs of all villager

            ZDOMan.instance.GetAllZDOsWithPrefab(prefabName, zdos);

            foreach (ZDO zdo in zdos)
            {
                ZDOID bedZDOID = zdo.GetZDOID("spawner_id"); //Get BedZDOID Stored in the villager's ZDO

                if (bedZDOID == null || bedZDOID.IsNone()) //Validate if bedZDOID is valid
                {
                    KLog.warning($"Villager {zdo.m_uid.id} Does not have bed ZDOID Stored");
                    continue;
                }

                ZDO bedZDO = ZDOMan.instance.GetZDO(bedZDOID); //Get the ZDO of the bed to get the location of the bed

                //Validate bedZDO
                if (bedZDO == null || bedZDO.IsValid() == false)
                {
                    KLog.warning($"BedZDO is invalid for villager {zdo.m_uid.id}");
                    continue;
                }

                GameObject villager = ZNetScene.instance.FindInstance(zdo.m_uid);  //Get ZNV of the villager
                if (villager != null && villager.GetComponent<VillagerAI>() != null)
                {
                    VillagerAI ai = villager.GetComponent<VillagerAI>();
                    ai.GuardBed();
                }
                else //Not valid so we TP the Villager's ZDO to the bed's ZDO and update the state of the villager to Guarding_Bed
                {
                    bedZDO.Set("state", (int)VillagerState.Guarding_Bed); //Update the state of the villager's ZDO Manually. The ORDER IS IMP. Or else if loaded in memory before State is set, it will go back to it's old state and overwrite this
                    zdo.SetPosition(bedZDO.GetPosition()); //Set the position of the 
                }


            }

        }

        ///Make all villagers defend their Defense post
        private void MakeVillagersDefend(string prefabName)
        {


            List<ZDO> zdos = new List<ZDO>(); //Store villagers ZDO here
            ZDOMan.instance.GetAllZDOsWithPrefab(prefabName, zdos);

            foreach (ZDO zdo in zdos)
            {
                ZDOID bedZDOID = zdo.GetZDOID("spawner_id"); //Get BedZDOID Stored in the villager's ZDO

                if (bedZDOID == null || bedZDOID.IsNone()) //Validate if bedZDOID is valid
                {
                    KLog.warning($"Villager {zdo.m_uid.id} Does not have bed ZDOID Stored");
                    continue;
                }

                //Get the ZDO if the bed using bedZDOID
                ZDO bedZDO = ZDOMan.instance.GetZDO(bedZDOID);

                //Validate bedZDO
                if (bedZDO == null || bedZDO.IsValid() == false)
                {
                    KLog.warning($"BedZDO is invalid for villager {zdo.m_uid.id}");
                    continue;
                }

                ZDOID defenseZDOID = bedZDO.GetZDOID("defense"); //Get ZDOID of defense post

                //Validate defenseZDOID
                if (defenseZDOID == null || defenseZDOID.IsNone())
                {
                    KLog.warning($"defenseZDOID is invalid for villager {zdo.m_uid.id} & bed {bedZDOID.id}");
                    continue;
                }

                //Get ZDO of defense post using defenseZDOID
                ZDO defenseZDO = ZDOMan.instance.GetZDO(defenseZDOID);

                //Validate bedZDO
                if (defenseZDO == null || defenseZDO.IsValid() == false)
                {
                    KLog.warning($"DefenseZDO is invalid for villager {zdo.m_uid.id}");
                    continue;
                }

                GameObject villager = ZNetScene.instance.FindInstance(zdo.m_uid);  //Get ZNV of the villager
                if (villager != null && villager.GetComponent<VillagerAI>() != null)
                {
                    VillagerAI ai = villager.GetComponent<VillagerAI>();
                    ai.DefendPost();
                }
                else //Not valid so we TP the Villager's ZDO to the Defend Post's ZDO and update the state of the villager to Defending_Post
                {
                    bedZDO.Set("state", (int)VillagerState.Defending_Post); //Update the state of the villager's ZDO Manually. The ORDER IS IMP. Or else if loaded in memory before State is set, it will go back to it's old state and overwrite this
                    zdo.SetPosition(defenseZDO.GetPosition()); //Set the position of the 
                }

            }


        }

        ///Make followers go to the location
        private void MakeFollowersGoToLocation(string prefabName, Vector3 location)
        {

            List<ZDO> zdos = new List<ZDO>();
            ZDOMan.instance.GetAllZDOsWithPrefab(prefabName, zdos);
            foreach (ZDO z in zdos)
            {
                ZDOID bedID = z.GetZDOID("spawner_id");

                if (bedID.IsNone())
                {
                    continue;
                }

                //If they are not following then ignore
                VillagerState state = (VillagerState)ZDOMan.instance.GetZDO(bedID).GetInt("state", (int)VillagerState.Guarding_Bed);
                if (state != VillagerState.Following) continue;

                //See if we can get an instance. We only make those who are nearby follow player
                GameObject villager = ZNetScene.instance.FindInstance(z.m_uid);

                if (villager != null && ZNetScene.instance.IsAreaReady(villager.transform.position))
                {

                    villager.GetComponent<VillagerAI>().MoveVillagerToLoc(location, 2f, false, true, true); //Move the villager to the location and also keep the villager as follower
                }
                else
                {
                    //Villager instance not valid so we make them guard bed
                    MakeVillagersGoToBed(prefabName);
                }

            }
        }

        /// <summary>
        /// Makes all followers who were commanded to go to a location come back to player.
        /// Those who are not in range (no instance/ not loaded in game world), will be made to guard bed
        /// </summary>
        /// <param name="prefabName"></param>
        private void MakeFollowersComeBack(string prefabName)
        {

            //Check followingPlayerID and see if it matches this local player. If match then make them follow player again

            List<ZDO> zdos = new List<ZDO>();
            ZDOMan.instance.GetAllZDOsWithPrefab(prefabName, zdos);
            foreach (ZDO z in zdos)
            {
                ZDOID bedID = z.GetZDOID("spawner_id");

                if (bedID.IsNone())
                {
                    continue;
                }

                //If they are not following then ignore
                VillagerState state = (VillagerState)ZDOMan.instance.GetZDO(bedID).GetInt("state", (int)VillagerState.Guarding_Bed);
                if (state != VillagerState.Following) continue;

                //See if we can get an instance. We only make those who are nearby follow player
                GameObject villager = ZNetScene.instance.FindInstance(z.m_uid);

                if (villager != null && ZNetScene.instance.IsAreaReady(villager.transform.position)) //if instance is valid we call DefendPost function
                {

                    villager.GetComponent<VillagerAI>().FollowPlayer(Player.m_localPlayer.GetZDOID()); ;
                }
                else
                {
                    //Follower is not close to player, Make them guard bed.
                    MakeVillagersGoToBed(prefabName);
                }

            }
        }


        private void MakeVillagersWork(string prefabName)
        {
            List<ZDO> zdos = new List<ZDO>();
            ZDOMan.instance.GetAllZDOsWithPrefab(prefabName, zdos);
            foreach (ZDO z in zdos)
            {
                ZDOID bedZDOID = z.GetZDOID("spawner_id"); //Get BedZDOID Stored in the villager's ZDO

                if (bedZDOID == null || bedZDOID.IsNone()) //Validate if bedZDOID is valid
                {
                    KLog.warning($"Villager {z.m_uid.id} Does not have bed ZDOID Stored");
                    continue;
                }

                ZDO bedZDO = ZDOMan.instance.GetZDO(bedZDOID); //Get the ZDO of the bed to get the location of the bed

                //Validate bedZDO
                if (bedZDO == null || bedZDO.IsValid() == false)
                {
                    KLog.warning($"BedZDO is invalid for villager {z.m_uid.id}");
                    continue;
                }

                //Validate WorkPost location
                ZDOID workID = bedZDO.GetZDOID("work");
                if (workID == null || workID.IsNone())
                {
                    KLog.warning($"Villager {z.m_uid.id} has no work post assigned");
                    continue;
                }

                ZDO workZDO = ZDOMan.instance.GetZDO(workID);
                if (workZDO == null || workZDO.IsValid() == false)
                {
                    KLog.warning($"Villager {z.m_uid.id} has invalid work post assigned");
                    continue;
                }

                //validate container
                ZDOID containerID = bedZDO.GetZDOID("container");
                if (containerID == null || containerID.IsNone())
                {
                    KLog.warning($"Villager {z.m_uid.id} has no container assigned");
                    continue;
                }

                ZDO containerZDO = ZDOMan.instance.GetZDO(containerID);
                if (containerZDO == null || containerZDO.IsValid() == false)
                {
                    KLog.warning($"Villager {z.m_uid.id} has invalid container assigned");
                    continue;
                }

                GameObject villager = ZNetScene.instance.FindInstance(z.m_uid);  //Get ZNV of the villager
                if (villager != null && villager.GetComponent<VillagerAI>() != null)
                {
                    villager.GetComponent<VillagerAI>().StartWork();
                }
                else
                {
                    //TP to work location
                    bedZDO.Set("state", (int)VillagerState.Working); //Update the state of the villager's ZDO Manually. The ORDER IS IMP. Or else if loaded in memory before State is set, it will go back to it's old state and overwrite this
                    z.SetPosition(workZDO.GetPosition());
                }
            }
        }

        private void MakeVillagersRoam(string prefabName)
        {
            List<ZDO> zdos = new List<ZDO>();
            ZDOMan.instance.GetAllZDOsWithPrefab(prefabName, zdos);
            foreach (ZDO z in zdos)
            {

                ZDOID bedZDOID = z.GetZDOID("spawner_id"); //Get BedZDOID Stored in the villager's ZDO

                if (bedZDOID == null || bedZDOID.IsNone()) //Validate if bedZDOID is valid
                {
                    KLog.warning($"Villager {z.m_uid.id} Does not have bed ZDOID Stored");
                    continue;
                }

                ZDO bedZDO = ZDOMan.instance.GetZDO(bedZDOID); //Get the ZDO of the bed to get the location of the bed

                //Validate bedZDO
                if (bedZDO == null || bedZDO.IsValid() == false)
                {
                    KLog.warning($"BedZDO is invalid for villager {z.m_uid.id}");
                    continue;
                }

                GameObject villager = ZNetScene.instance.FindInstance(z.m_uid);  //Get ZNV of the villager
                if (villager != null && villager.GetComponent<VillagerAI>() != null)
                {
                    villager.GetComponent<VillagerAI>().RoamAround();
                }
                else
                {
                    bedZDO.Set("state", (int)VillagerState.Roaming); //Update the state of the villager's ZDO Manually. The ORDER IS IMP. Or else if loaded in memory before State is set, it will go back to it's old state and overwrite this
                }
            }
        }
    }
}
