﻿using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Configuration;
using KukusVillagerMod.enums;
using KukusVillagerMod.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KukusVillagerMod.Components.Villager;
using KukusVillagerMod.Components.VillagerBed;
using KukusVillagerMod.Components.DefensePost;

namespace KukusVillagerMod.itemPrefab
{
    class VillagerCommander
    {
        public VillagerCommander()
        {
            createCommanderPrefab();
            //createCommanderButtons();
            CreateEmptyKH();
        }

        CustomItem commander;

        void createCommanderPrefab()
        {
            ItemConfig commanderConfig = new ItemConfig();
            commanderConfig.Name = "Village Commander";
            commanderConfig.Description = $"Command the villagers in your village.\nCommands:\n{VillagerModConfigurations.guardBedKey} : Guard Bed\n{VillagerModConfigurations.CallFollowers} : Call Followers back to the commander\n{VillagerModConfigurations.defendPostKey} : Defend Defend Post\n{VillagerModConfigurations.moveToKey} : Move followers to aimed location";
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

        bool guardBedPressed = false;
        bool followPlayerPressed = false;
        bool defendPostPressed = false;
        bool deletePostPressed = false;
        bool deleteVillagersPressed = false;
        bool deleteBedsPressed = false;
        bool moveToPressed = false;
        bool showStatsPressed = false;

        private ButtonConfig guardBedbtn;
        private ButtonConfig followPlayerBtn;
        private ButtonConfig defendBtn;
        private ButtonConfig deletePostsBtn;
        private ButtonConfig deleteVillagersBtn;
        private ButtonConfig deleteBedBtn;
        private ButtonConfig showStatsBtn;



        public void HandleInputs()
        {


            if (ZInput.instance == null || MessageHud.instance == null || Player.m_localPlayer == null) return;
            if (Player.m_localPlayer.GetInventory() == null) return;
            List<ItemDrop.ItemData> allItems = Player.m_localPlayer.GetInventory().GetAllItems();

            if (allItems == null) return;

            for (int i = 0; i < allItems.Count; i++)
            {
                if (Player.m_localPlayer.GetInventory().GetEquipedtems().Contains(allItems[i]))
                {
                    foreach (ItemDrop.ItemData e in Player.m_localPlayer.GetInventory().GetEquipedtems())
                    {
                        if (e == null) continue;
                        if (allItems[i] == null) continue;

                        if (e == allItems[i] || e.Equals(allItems[i]))
                        {
                            if (e.TokenName().Equals("Village Commander"))
                            {
                                if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.guardBedKey)
                                {


                                    //go to bed point
                                    if (guardBedPressed) return;
                                    guardBedPressed = true;
                                    followPlayerPressed = false;
                                    defendPostPressed = false;
                                    deletePostPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = false;
                                    moveToPressed = false;
                                    showStatsPressed = false;


                                    MakeVillagersGoToBed("Weak_Villager_Ranged");
                                    MakeVillagersGoToBed("Weak_Villager");
                                    MakeVillagersGoToBed("Bronze_Villager_Ranged");
                                    MakeVillagersGoToBed("Bronze_Villager");
                                    MakeVillagersGoToBed("Iron_Villager_Ranged");
                                    MakeVillagersGoToBed("Iron_Villager");
                                    MakeVillagersGoToBed("Silver_Villager");
                                    MakeVillagersGoToBed("Silver_Villager_Ranged");
                                    MakeVillagersGoToBed("BlackMetal_Villager_Ranged");
                                    MakeVillagersGoToBed("BlackMetal_Villager");


                                }
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.CallFollowers)
                                {

                                    //Follow Player
                                    if (followPlayerPressed) return;
                                    guardBedPressed = false;
                                    followPlayerPressed = true;
                                    defendPostPressed = false;
                                    deletePostPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = false;
                                    moveToPressed = false;
                                    showStatsPressed = false;

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
                                    guardBedPressed = false;
                                    followPlayerPressed = false;
                                    defendPostPressed = true;
                                    deletePostPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = false;
                                    moveToPressed = false;
                                    showStatsPressed = false;

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
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.deletePostKey)
                                {


                                    //Go to aiming location
                                    if (deletePostPressed) return;
                                    guardBedPressed = false;
                                    followPlayerPressed = false;
                                    defendPostPressed = false;
                                    deletePostPressed = true;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = false;
                                    moveToPressed = false;
                                    showStatsPressed = false;

                                    foreach (DefenseState v in UnityEngine.GameObject.FindObjectsOfType<DefenseState>())
                                    {
                                        //if (v == null || v.znv == null || v.znv.GetZDO() == null)
                                        ZNetScene.instance.Destroy(v.gameObject);
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Destroying all Post");
                                    }


                                }
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.deleteVillagerKey)
                                {


                                    //Destroy all villagers
                                    if (deleteVillagersPressed) return;
                                    guardBedPressed = false;
                                    followPlayerPressed = false;
                                    defendPostPressed = false;
                                    deletePostPressed = false;
                                    deleteVillagersPressed = true;
                                    deleteBedsPressed = false;
                                    moveToPressed = false;
                                    showStatsPressed = false;

                                    foreach (VillagerGeneral v in UnityEngine.GameObject.FindObjectsOfType<VillagerGeneral>())
                                    {
                                        //if (v == null || v.znv == null || v.znv.GetZDO() == null)
                                        ZNetScene.instance.Destroy(v.gameObject);
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Destroying all Villagers");
                                    }

                                }
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.deleteBedsKey)
                                {

                                    if (deleteBedsPressed) return;
                                    guardBedPressed = false;
                                    followPlayerPressed = false;
                                    defendPostPressed = false;
                                    deletePostPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = true;
                                    moveToPressed = false;
                                    showStatsPressed = false;

                                    foreach (BedVillagerProcessor v in UnityEngine.GameObject.FindObjectsOfType<BedVillagerProcessor>())
                                    {
                                        ZNetScene.instance.Destroy(v.gameObject);
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Destroying all Beds");
                                    }
                                }
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.moveToKey)
                                {
                                    if (moveToPressed) return;
                                    guardBedPressed = false;
                                    followPlayerPressed = false;
                                    defendPostPressed = false;
                                    deletePostPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = false;
                                    moveToPressed = true;
                                    showStatsPressed = false;


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
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.showStatKey)
                                {
                                    if (showStatsPressed) return;
                                    guardBedPressed = false;
                                    followPlayerPressed = false;
                                    defendPostPressed = false;
                                    deletePostPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = false;
                                    moveToPressed = false;
                                    showStatsPressed = true;


                                }
                                else
                                {
                                    guardBedPressed = false;
                                    followPlayerPressed = false;
                                    defendPostPressed = false;
                                    deletePostPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = false;
                                    moveToPressed = false;
                                    showStatsPressed = false;
                                }

                            }
                        }
                    }
                }
            }
        }

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
                    KLog.warning($"defenseZDOID is invalid for villager {zdo.m_uid.id} & bed {bedZDOID}");
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

                if (villager != null && ZNetScene.instance.IsAreaReady(villager.transform.position)) //if instance is valid we call DefendPost function
                {
                    //Check if playerID matches

                    if (villager.GetComponent<VillagerAI>().followingPlayerZDOID != Player.m_localPlayer.GetZDOID())
                    {
                        continue;
                    }

                    villager.GetComponent<VillagerAI>().MoveVillagerToLoc(location, true); //Move the villager to the location and also keep the villager as follower
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
                    //Check if playerID matches

                    if (villager.GetComponent<VillagerAI>().followingPlayerZDOID != Player.m_localPlayer.GetZDOID())
                    {
                        continue;
                    }

                    villager.GetComponent<VillagerAI>().FollowPlayer(Player.m_localPlayer);
                }
                else
                {
                    //Follower is not close to player, Make them guard bed.
                    MakeVillagersGoToBed(prefabName);
                }

            }
        }
    }
}
