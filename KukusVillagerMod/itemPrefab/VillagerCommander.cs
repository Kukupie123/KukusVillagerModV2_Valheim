using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using KukusVillagerMod.Configuration;
using KukusVillagerMod.enums;
using KukusVillagerMod.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
            var kh = new KeyHintConfig
            {
                Item = "Village_Commander",
                ButtonConfigs = new[] { guardBedbtn, followPlayerBtn, defendBtn, deletePostsBtn, deleteVillagersBtn, deleteBedBtn, showStatsBtn }
            };
            KeyHintManager.Instance.AddKeyHint(kh);
        }

        void CreateEmptyKH()
        {
            var kh = new KeyHintConfig
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
                                if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.guardBedKey)
                                {
                                    if (ZNetScene.instance.IsAreaReady(Player.m_localPlayer.transform.position) == false)
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Area is not fully loaded. Please wait");
                                    }

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
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.followPlayerKey)
                                {
                                    if (ZNetScene.instance.IsAreaReady(Player.m_localPlayer.transform.position) == false)
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Area is not fully loaded. Please wait");
                                    }

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

                                    var villager = GetLookingAtVillager(Player.m_localPlayer);

                                    if (villager)
                                    {

                                        villager.FollowPlayer(Player.m_localPlayer);
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Following " + Player.m_localPlayer.GetHoverName());
                                    }
                                    else
                                    {
                                        foreach (var v in Global.followers)
                                        {
                                            v.FollowPlayer(Player.m_localPlayer);
                                        }
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Commanded all followers to come back");

                                    }


                                }
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.defendPostKey)
                                {
                                    if (ZNetScene.instance.IsAreaReady(Player.m_localPlayer.transform.position) == false)
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Area is not fully loaded. Please wait");
                                    }



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
                                    if (ZNetScene.instance.IsAreaReady(Player.m_localPlayer.transform.position) == false)
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Area is not fully loaded. Please wait");
                                    }

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




                                }
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.deleteVillagerKey)
                                {
                                    if (ZNetScene.instance.IsAreaReady(Player.m_localPlayer.transform.position) == false)
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Area is not fully loaded. Please wait");
                                    }

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

                                    foreach (var v in UnityEngine.GameObject.FindObjectsOfType<VillagerLifeCycle>())
                                    {
                                        //if (v == null || v.znv == null || v.znv.GetZDO() == null)
                                        ZNetScene.instance.Destroy(v.gameObject);
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Destroying all Villagers");
                                    }

                                }
                                else if (ZInput.instance.GetPressedKey().ToString() == VillagerModConfigurations.deleteBedsKey)
                                {
                                    if (ZNetScene.instance.IsAreaReady(Player.m_localPlayer.transform.position) == false)
                                    {
                                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Area is not fully loaded. Please wait");
                                    }

                                    if (deleteBedsPressed) return;
                                    guardBedPressed = false;
                                    followPlayerPressed = false;
                                    defendPostPressed = false;
                                    deletePostPressed = false;
                                    deleteVillagersPressed = false;
                                    deleteBedsPressed = true;
                                    moveToPressed = false;
                                    showStatsPressed = false;

                                    foreach (var v in UnityEngine.GameObject.FindObjectsOfType<BedVillagerProcessor>())
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

                                    //TEST
                                    /*
                                    foreach (var v in Global.followers)
                                    {
                                        //v.humanoid.UnequipAllItems();
                                        var axe = ItemManager.Instance.GetItem("axe");
                                        var a = ZNetScene.Instantiate(axe.ItemPrefab,v.transform);
                                        v.humanoid.EquipItem(a.GetComponent<ItemDrop>().m_itemData);
                                        if (v != null && ZNetScene.instance.IsAreaReady(v.transform.position))
                                        {
                                            v.CutTree();
                                        }
                                    }

                                    return;
                                    */


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

                                        foreach (var v in Global.followers)
                                        {
                                            if (v != null && ZNetScene.instance.IsAreaReady(v.transform.position))
                                            {
                                                v.MoveTo(hitData.point);
                                                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"Sending followers to {hitData.point}");
                                            }
                                        }
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

                                    int villagersCount = 0;
                                    int bedless = 0;
                                    int defending = 0;
                                    /*
                                    foreach (var v in UnityEngine.GameObject.FindObjectsOfType<VillagerLifeCycle>())
                                    {
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
                                    */
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
        VillagerLifeCycle GetLookingAtVillager(Player player)
        {
            var a = player.GetHoverObject();
            if (a == null) return null;
            var b = a.GetComponent<VillagerLifeCycle>();
            return b;
        }

        private void MakeVillagersGoToBed(string prefabName)
        {
            //Find spawner_id of every prefab and see if it has valid bedID, then based on if we found it's instance in game or not we command it to move guard bed or simply teleport by updating position

            List<ZDO> zdos = new List<ZDO>();
            ZDOMan.instance.GetAllZDOsWithPrefab(prefabName, zdos);
            foreach (ZDO z in zdos)
            {
                var bedID = z.GetZDOID("spawner_id");

                if (bedID.IsNone())
                {
                    continue;
                }

                //See if we can get an instance.
                var villager = ZNetScene.instance.FindInstance(z);

                if (villager != null)
                {
                    villager.GetComponent<VillagerLifeCycle>().GuardBed();
                }
                else //if we can't get instance we update the position in zdo
                {
                    var bedZDO = ZDOMan.instance.GetZDO(bedID);


                    z.SetPosition(bedZDO.GetPosition());
                    //Set it's state manually
                    bedZDO.Set("state", (int)VillagerState.Guarding_Bed);
                }


            }


        }

        private void MakeVillagersDefend(string prefabName)
        {
            //Find spawner_id of every prefab and see if it has valid bedID, then based on if we found it's instance in game or not we command it to move guard bed or simply teleport by updating position

            List<ZDO> zdos = new List<ZDO>();
            ZDOMan.instance.GetAllZDOsWithPrefab(prefabName, zdos);
            foreach (ZDO z in zdos)
            {
                var bedID = z.GetZDOID("spawner_id");

                if (bedID.IsNone())
                {
                    continue;
                }

                //See if we can get an instance.
                var villager = ZNetScene.instance.FindInstance(z);

                if (villager != null)
                {
                    villager.GetComponent<VillagerLifeCycle>().DefendPost();
                }
                else //if we can't get instance we are going to get defense zdo and use it's position
                {
                    var bedZDO = ZDOMan.instance.GetZDO(bedID);


                    var defenseID = bedZDO.GetZDOID("defense");
                    if (defenseID.IsNone()) continue;

                    var defenseZDO = ZDOMan.instance.GetZDO(defenseID);

                    z.SetPosition(defenseZDO.GetPosition());

                    //Set it's state manually
                    bedZDO.Set("state", (int)VillagerState.Defending_Post);
                }


            }


        }
    }
}
