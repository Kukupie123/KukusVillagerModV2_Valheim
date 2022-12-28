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

            InputManager.Instance.AddButton("com.jotunn.KukusVillagerMod", guardBedbtn);
            InputManager.Instance.AddButton("com.jotunn.KukusVillagerMod", followPlayerBtn);
            InputManager.Instance.AddButton("com.jotunn.KukusVillagerMod", defendBtn);
            InputManager.Instance.AddButton("com.jotunn.KukusVillagerMod", deletePostsBtn);

        }

        void createKeyhints()
        {
            var kh = new KeyHintConfig
            {
                Item = "Village_Commander",
                ButtonConfigs = new[] { guardBedbtn, followPlayerBtn, defendBtn, deletePostsBtn }
            };
            KeyHintManager.Instance.AddKeyHint(kh);
        }

        bool keyPad1Pressed = false;
        bool keyPad2Pressed = false;
        bool keyPad3Pressed = false;
        bool keyPad4Pressed = false;
        private ButtonConfig guardBedbtn;
        private ButtonConfig followPlayerBtn;
        private ButtonConfig defendBtn;
        private ButtonConfig deletePostsBtn;

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
                                    //go to bed point
                                    if (keyPad1Pressed) return;
                                    keyPad1Pressed = true;
                                    keyPad2Pressed = false;
                                    keyPad3Pressed = false;
                                    keyPad4Pressed = false;

                                    //Clear out all defence's villagerState as all are going to go home
                                    foreach (var d in Global.defences)
                                    {
                                        if (d == null) continue;
                                        d.villager = null;
                                    }

                                    //Make all villager guard their bed
                                    foreach (var vv in Global.villagers)
                                    {
                                        if (vv == null) continue;
                                        vv.GetComponentInParent<VillagerLifeCycle>().GuardBed();
                                    }


                                }
                                else if (ZInput.instance.GetPressedKey() == UnityEngine.KeyCode.Keypad2)
                                {
                                    //Follow Player
                                    if (keyPad2Pressed) return;
                                    keyPad1Pressed = false;
                                    keyPad2Pressed = true;
                                    keyPad3Pressed = false;
                                    keyPad4Pressed = false;

                                    var villager = GetLookingAtVillager(Player.m_localPlayer);

                                    if (villager)
                                    {

                                        villager.FollowPlayer(Player.m_localPlayer);
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Following " + Player.m_localPlayer.GetHoverName());
                                    }
                                    else
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Please look at a close by villager and try again.");

                                    }


                                }
                                else if (ZInput.instance.GetPressedKey() == UnityEngine.KeyCode.Keypad3)
                                {
                                    //Go defensive position
                                    if (keyPad3Pressed) return;
                                    keyPad1Pressed = false;
                                    keyPad2Pressed = false;
                                    keyPad3Pressed = true;
                                    keyPad4Pressed = false;

                                    //Make two list. One without followers and one with followers. First we will try to send the non followers, if still vacant, we will send followers
                                    foreach (var v in Global.villagers)
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Going to defense posts");
                                        v.GetComponentInParent<VillagerLifeCycle>().DefendPost();
                                    }
                                }
                                else if (ZInput.instance.GetPressedKey() == UnityEngine.KeyCode.Keypad4)
                                {
                                    //Go to aiming location
                                    if (keyPad4Pressed) return;
                                    keyPad1Pressed = false;
                                    keyPad2Pressed = false;
                                    keyPad3Pressed = false;
                                    keyPad4Pressed = true;

                                    foreach (var v in UnityEngine.GameObject.FindObjectsOfType<DefensePostState>())
                                    {
                                        ZNetScene.instance.Destroy(v.gameObject);
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Destroying all defense posts");
                                    }



                                }

                                else
                                {
                                    keyPad1Pressed = false;
                                    keyPad2Pressed = false;
                                    keyPad3Pressed = false;
                                    keyPad4Pressed = false;
                                }
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
