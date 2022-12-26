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

        ButtonConfig guardBedbtn;
        ButtonConfig followPlayerBtn;
        ButtonConfig defendButton;
        ButtonConfig deleteAllPosts;
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
            createButton(guardBedbtn, "KP1", "Guard Bed", UnityEngine.KeyCode.Keypad1);
            createButton(followPlayerBtn, "KP2", "Follow Player", UnityEngine.KeyCode.Keypad2);
            createButton(defendButton, "KP3", "Defend Post", UnityEngine.KeyCode.Keypad3);
            createButton(deleteAllPosts, "KP4", "Destroy Nearby Defense Posts", UnityEngine.KeyCode.Keypad4);
        }

        private void createButton(ButtonConfig btnConfig, string name, string hint, UnityEngine.KeyCode key)
        {
            btnConfig = new ButtonConfig();
            btnConfig.Name = name;
            btnConfig.Hint = hint;
            btnConfig.ActiveInCustomGUI = true;
            btnConfig.BlockOtherInputs = true;
            btnConfig.Key = key;
            InputManager.Instance.AddButton("com.jotunn.KukuVillagers", btnConfig);
        }

        void createKeyhints()
        {
            var kh = new KeyHintConfig
            {
                Item = "Village_Commander",
                ButtonConfigs = new[] { guardBedbtn, followPlayerBtn, defendButton, deleteAllPosts }
            };
            KeyHintManager.Instance.AddKeyHint(kh);
        }

        bool keyPad1Pressed = false;
        bool keyPad2Pressed = false;
        bool keyPad3Pressed = false;
        bool keyPad4Pressed = false;

        void handleInputs()
        {
            if (ZInput.instance == null || MessageHud.instance == null || Player.m_localPlayer == null) return;
            if (Player.m_localPlayer.GetInventory() == null) return;
            var allItems = Player.m_localPlayer.GetInventory().GetAllItems();

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
                                        d.villagerState = null;
                                    }

                                    //Make all villager guard their bed
                                    foreach (var vv in Global.villagerStates)
                                    {
                                        if (vv == null) continue;
                                        vv.GuardBed();
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
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This villager is following you");
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

                                    foreach (var v in Global.villagerStates)
                                    {
                                        v.DefendPost();
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

                                    foreach (var v in UnityEngine.Object.FindObjectsOfType<DefensePostState>())
                                    {
                                        if (v != null) continue;
                                        UnityEngine.Object.Destroy(v.gameObject);
                                    }

                                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Destroying all defense posts");

                                    //MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Beds : " + Global.beds.Count + " Villagers : " + Global.villagers.Count + " Defense Posts : " + Global.defenses.Count + " Followers : " + Global.followers.Count);


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
        VillagerState GetLookingAtVillager(Player player)
        {
            var a = player.GetHoverObject();
            if (a == null) return null;
            var b = a.GetComponent<VillagerState>();
            return b;
        }
    }
}
