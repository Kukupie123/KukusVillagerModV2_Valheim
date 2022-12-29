using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KukusVillagerMod.itemPrefab
{
    class VillagerCommander
    {
        public VillagerCommander()
        {
            createCommanderPrefab();
            createCommanderButtons();
            createKeyhints();
        }

        CustomItem commander;

        void createCommanderPrefab()
        {
            ItemConfig commanderConfig = new ItemConfig();
            commanderConfig.Name = "Village Commander";
            commanderConfig.Description = "Command the villagers in your village";
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
            showStatsBtn.Hint = "Remove Current Stats";
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
            var kh = new KeyHintConfig
            {
                Item = "Village_Commander",
                ButtonConfigs = new[] { guardBedbtn, followPlayerBtn, defendBtn, deletePostsBtn, deleteVillagersBtn, deleteBedBtn }
            };
            KeyHintManager.Instance.AddKeyHint(kh);
        }

        bool keyPad1Pressed = false;
        bool keyPad2Pressed = false;
        bool keyPad3Pressed = false;
        bool keyPad4Pressed = false;
        bool keyPad5Pressed = false;
        bool keyPad6Pressed = false;
        bool keyPad7Pressed = false;

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
            var allItems = Player.m_localPlayer.GetInventory().GetAllItems();

            if (allItems == null) return;

            for (int i = 0; i < allItems.Count; i++)
            {
                if (Player.m_localPlayer.GetInventory().GetEquipedtems().Contains(allItems[i]))
                {
                    foreach (var e in Player.m_localPlayer.GetInventory().GetEquipedtems())
                    {
                        if (e == null) continue;
                        if (allItems[i] == null) continue;

                        if (e == allItems[i] || e.Equals(allItems[i]))
                        {
                            if (e.TokenName().Equals("Village Commander"))
                            {
                                if (ZInput.instance.GetPressedKey() == UnityEngine.KeyCode.Keypad1)
                                {
                                    if (ZNetScene.instance.IsAreaReady(Player.m_localPlayer.transform.position) == false)
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Area is not fully loaded. Please wait");
                                    }

                                    //go to bed point
                                    if (keyPad1Pressed) return;
                                    keyPad1Pressed = true;
                                    keyPad2Pressed = false;
                                    keyPad3Pressed = false;
                                    keyPad4Pressed = false;
                                    keyPad5Pressed = false;
                                    keyPad6Pressed = false;
                                    keyPad7Pressed = false;

                                    //Make all villager guard their bed
                                    foreach (var vv in UnityEngine.GameObject.FindObjectsOfType<VillagerLifeCycle>())
                                    {
                                        if (vv == null || vv.znv == null || vv.znv.GetZDO() == null) continue;
                                        if (vv.GetComponentInParent<VillagerLifeCycle>() == null) continue;
                                        vv.GetComponentInParent<VillagerLifeCycle>().GuardBed();
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Guarding Bed");
                                    }


                                }
                                else if (ZInput.instance.GetPressedKey() == UnityEngine.KeyCode.Keypad2)
                                {
                                    if (ZNetScene.instance.IsAreaReady(Player.m_localPlayer.transform.position) == false)
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Area is not fully loaded. Please wait");
                                    }

                                    //Follow Player
                                    if (keyPad2Pressed) return;
                                    keyPad1Pressed = false;
                                    keyPad2Pressed = true;
                                    keyPad3Pressed = false;
                                    keyPad4Pressed = false;
                                    keyPad5Pressed = false;
                                    keyPad6Pressed = false;
                                    keyPad7Pressed = false;

                                    var villager = GetLookingAtVillager(Player.m_localPlayer);

                                    if (villager)
                                    {

                                        villager.FollowPlayer(Player.m_localPlayer);
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Following " + Player.m_localPlayer.GetHoverName());
                                    }
                                    else
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Please look at a close by villager and try again.");

                                    }


                                }
                                else if (ZInput.instance.GetPressedKey() == UnityEngine.KeyCode.Keypad3)
                                {
                                    if (ZNetScene.instance.IsAreaReady(Player.m_localPlayer.transform.position) == false)
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Area is not fully loaded. Please wait");
                                    }

                                    //Go defensive position
                                    if (keyPad3Pressed) return;
                                    keyPad1Pressed = false;
                                    keyPad2Pressed = false;
                                    keyPad3Pressed = true;
                                    keyPad4Pressed = false;
                                    keyPad5Pressed = false;
                                    keyPad6Pressed = false;
                                    keyPad7Pressed = false;

                                    //Make two list. One without followers and one with followers. First we will try to send the non followers, if still vacant, we will send followers
                                    foreach (var v in UnityEngine.GameObject.FindObjectsOfType<VillagerLifeCycle>())
                                    {
                                        if (v == null || v.znv == null || v.znv.GetZDO() == null) continue;
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Going to defense posts");
                                        v.GetComponentInParent<VillagerLifeCycle>().DefendPost();
                                    }
                                }
                                else if (ZInput.instance.GetPressedKey() == UnityEngine.KeyCode.Keypad4)
                                {
                                    if (ZNetScene.instance.IsAreaReady(Player.m_localPlayer.transform.position) == false)
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Area is not fully loaded. Please wait");
                                    }

                                    //Go to aiming location
                                    if (keyPad4Pressed) return;
                                    keyPad1Pressed = false;
                                    keyPad2Pressed = false;
                                    keyPad3Pressed = false;
                                    keyPad4Pressed = true;
                                    keyPad5Pressed = false;
                                    keyPad6Pressed = false;
                                    keyPad7Pressed = false;

                                    foreach (var v in UnityEngine.GameObject.FindObjectsOfType<DefensePostState>())
                                    {
                                        ZNetScene.instance.Destroy(v.gameObject);
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Destroying all defense posts");
                                    }



                                }
                                else if (ZInput.instance.GetPressedKey() == UnityEngine.KeyCode.Keypad5)
                                {
                                    if (ZNetScene.instance.IsAreaReady(Player.m_localPlayer.transform.position) == false)
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Area is not fully loaded. Please wait");
                                    }

                                    //Destroy all villagers
                                    if (keyPad5Pressed) return;
                                    keyPad1Pressed = false;
                                    keyPad2Pressed = false;
                                    keyPad3Pressed = false;
                                    keyPad4Pressed = false;
                                    keyPad5Pressed = true;
                                    keyPad6Pressed = false;
                                    keyPad7Pressed = false;

                                    foreach (var v in UnityEngine.GameObject.FindObjectsOfType<VillagerLifeCycle>())
                                    {
                                        if (v == null || v.znv == null || v.znv.GetZDO() == null)
                                            ZNetScene.instance.Destroy(v.gameObject);
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Destroying all Villagers");
                                    }

                                }
                                else if (ZInput.instance.GetPressedKey() == UnityEngine.KeyCode.Keypad6)
                                {
                                    if (ZNetScene.instance.IsAreaReady(Player.m_localPlayer.transform.position) == false)
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Area is not fully loaded. Please wait");
                                    }

                                    if (keyPad6Pressed) return;
                                    keyPad1Pressed = false;
                                    keyPad2Pressed = false;
                                    keyPad3Pressed = false;
                                    keyPad4Pressed = false;
                                    keyPad5Pressed = false;
                                    keyPad6Pressed = true;
                                    keyPad7Pressed = false;

                                    foreach (var v in UnityEngine.GameObject.FindObjectsOfType<BedCycle>())
                                    {
                                        ZNetScene.instance.Destroy(v.gameObject);
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Destroying all Beds");
                                    }
                                }

                                else if (ZInput.instance.GetPressedKey() == UnityEngine.KeyCode.Keypad7)
                                {
                                    if (keyPad7Pressed) return;
                                    keyPad1Pressed = false;
                                    keyPad2Pressed = false;
                                    keyPad3Pressed = false;
                                    keyPad4Pressed = false;
                                    keyPad5Pressed = false;
                                    keyPad6Pressed = false;
                                    keyPad7Pressed = true;

                                    int villagersCount = 0;
                                    int bedless = 0;
                                    int defending = 0;
                                    foreach (var v in UnityEngine.GameObject.FindObjectsOfType<VillagerLifeCycle>())
                                    {
                                        if (v == null || v.znv == null || v.znv.GetZDO() == null) continue;
                                        if (v != null)
                                        {
                                            villagersCount++;
                                        }

                                        if (v.bed == null) bedless++;

                                        if (v.followingTarget != null && v.followingTarget.GetComponent<DefensePostState>() != null) defending++;

                                    }

                                    int bedCount = 0;
                                    foreach (var v in UnityEngine.GameObject.FindObjectsOfType<BedCycle>())
                                    {
                                        if (v != null)
                                        {
                                            bedCount++;
                                        }
                                    }

                                    int dpC = 0;
                                    foreach (var v in UnityEngine.GameObject.FindObjectsOfType<DefensePostState>())
                                    {
                                        if (v != null)
                                        {
                                            dpC++;
                                        }
                                    }


                                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Villagers Loaded : {villagersCount}, Beddless Villagers : {bedless}, Defending villagers : {defending}, Beds Loaded : {bedCount}, DP Loaded : {dpC} {Player.m_localPlayer.GetGroup()}");
                                }
                                else
                                {
                                    keyPad1Pressed = false;
                                    keyPad2Pressed = false;
                                    keyPad3Pressed = false;
                                    keyPad4Pressed = false;
                                    keyPad5Pressed = false;
                                    keyPad6Pressed = false;
                                }
                                keyPad7Pressed = false;

                            }
                        }
                    }
                }
            }
        }
        VillagerLifeCycle GetLookingAtVillager(Player player)
        {
            var a = player.GetHoverObject();
            if (a == null) return null;
            var b = a.GetComponent<VillagerLifeCycle>();
            return b;
        }
    }
}
