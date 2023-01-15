using Jotunn.GUI;
using Jotunn.Managers;
using KukusVillagerMod.Components.Villager;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KukusVillagerMod.Components.UI
{
    class VillagerGUI
    {
        static GameObject VillagerStatsUI;




        public static void ShowVillagerStatsUI(ZDOID villagerZDOID)
        {
            if (GUIManager.Instance == null)
            {

                return;
            }

            if (!GUIManager.CustomGUIFront)
            {

                return;
            }

            if (!Util.ValidateZDOID(villagerZDOID))
            {
                return;
            }


            if (VillagerStatsUI == null)
            {


                VillagerStatsUI = GUIManager.Instance.CreateWoodpanel(
                parent: GUIManager.CustomGUIFront.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0, 0),
                width: 850,
                height: 600,
                draggable: false);

                //Hide it
                VillagerStatsUI.SetActive(false);

                //Add close button
                GameObject closeBtn = GUIManager.Instance.CreateButton(
                    text: "Close",
                    parent: VillagerStatsUI.transform,
                    anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(0, -250f),
                    width: 250f,
                    height: 60f
                    );

                closeBtn.SetActive(true);

                //Add listener to the button
                Button button = closeBtn.GetComponent<Button>();
                button.onClick.AddListener(CloseVillagerStatMenu);

                //Name text
                GameObject NameTextGO = GUIManager.Instance.CreateText(
                    text: VillagerGeneral.GetName(villagerZDOID),
                    parent: VillagerStatsUI.transform,
                     anchorMin: new Vector2(0.5f, 0.5f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(0f, 250f), // width & height
                    width: 250f,
                    height: 60f,
                    color: Color.black,
                    outline: true,
                    outlineColor: Color.white,
                    font: GUIManager.Instance.AveriaSerif,
                    fontSize: 30,
                    addContentSizeFitter: false
                    );

                GameObject HealthTextGO = GUIManager.Instance.CreateText(
                    text: $"Max Health : {VillagerGeneral.GetHealth(villagerZDOID)}",
                    parent: VillagerStatsUI.transform,
                    anchorMin: new Vector2(0.5f, 0.1f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(-200f, 200f), // width & height
                    width: 250f,
                    height: 60f,
                    color: Color.yellow,
                    outline: false,
                    outlineColor: Color.white,
                    font: GUIManager.Instance.AveriaSerif,
                    fontSize: 20,
                    addContentSizeFitter: false
                    );

                GameObject EfficiencyTextBtn = GUIManager.Instance.CreateText(
                    text: $"Efficiency : {(int)(VillagerGeneral.GetEfficiency(villagerZDOID) * 100)}%",
                    parent: VillagerStatsUI.transform,
                    anchorMin: new Vector2(0.5f, 0.1f),
                    anchorMax: new Vector2(0.5f, 0.5f),
                    position: new Vector2(-200f, 150f), // width & height
                    width: 250f,
                    height: 60f,
                    color: Color.yellow,
                    outline: false,
                    outlineColor: Color.white,
                    font: GUIManager.Instance.AveriaSerif,
                    fontSize: 20,
                    addContentSizeFitter: false
                    );

                string mining = "";

                switch (VillagerGeneral.GetMiningLevel(villagerZDOID))
                {
                    case 0: mining = "Newbie"; break;
                    case 1: mining = "Rokie"; break;
                    case 2: mining = "Iron Arms"; break;
                    case 3: mining = "Veteran"; break;
                }

                GameObject MiningLvlTextBtn = GUIManager.Instance.CreateText(
                    text: $"Work Level : {mining}",
                    parent: VillagerStatsUI.transform,
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

                GameObject ChoppingSkillBtn = GUIManager.Instance.CreateText(
                   text: $"Chopping Skill : {VillagerGeneral.GetChop(villagerZDOID)}",
                   parent: VillagerStatsUI.transform,
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



                GameObject MiningSkillBtn = GUIManager.Instance.CreateText(
                   text: $"Mining Skill : {VillagerGeneral.GetPickaxe(villagerZDOID)}",
                   parent: VillagerStatsUI.transform,
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

                Tuple<HitData.DamageType, float> specialSKill = VillagerGeneral.GetSpecialSkill(villagerZDOID);
                if (specialSKill != null)
                {
                    GameObject SpecialDmgBtn = GUIManager.Instance.CreateText(
                 text: $"Special Damage : {specialSKill.Item1} ({specialSKill.Item2})",
                 parent: VillagerStatsUI.transform,
                 anchorMin: new Vector2(0.5f, 0.1f),
                 anchorMax: new Vector2(0.5f, 0.5f),
                 position: new Vector2(200f, 250f), // width & height
                 width: 250f,
                 height: 60f,
                 color: Color.white,
                 outline: false,
                 outlineColor: Color.yellow,
                 font: GUIManager.Instance.AveriaSerif,
                 fontSize: 20,
                 addContentSizeFitter: false
                 );
                }

                GameObject DmgTextBtn = GUIManager.Instance.CreateText(
                 text: $"Damage : {VillagerGeneral.GetDamage(villagerZDOID)}",
                 parent: VillagerStatsUI.transform,
                 anchorMin: new Vector2(0.5f, 0.1f),
                 anchorMax: new Vector2(0.5f, 0.5f),
                 position: new Vector2(200f, 200f), // width & height
                 width: 250f,
                 height: 60f,
                 color: Color.yellow,
                 outline: false,
                 outlineColor: Color.white,
                 font: GUIManager.Instance.AveriaSerif,
                 fontSize: 20,
                 addContentSizeFitter: false
                 );

                GameObject SlashTextBtn = GUIManager.Instance.CreateText(
                 text: $"Slash : {VillagerGeneral.GetSlash(villagerZDOID)}",
                 parent: VillagerStatsUI.transform,
                 anchorMin: new Vector2(0.5f, 0.1f),
                 anchorMax: new Vector2(0.5f, 0.5f),
                 position: new Vector2(200f, 150f), // width & height
                 width: 250f,
                 height: 60f,
                 color: Color.yellow,
                 outline: false,
                 outlineColor: Color.white,
                 font: GUIManager.Instance.AveriaSerif,
                 fontSize: 20,
                 addContentSizeFitter: false
                 );

                GameObject BluntTextBtn = GUIManager.Instance.CreateText(
                 text: $"Blunt : {VillagerGeneral.GetBlunt(villagerZDOID)}",
                 parent: VillagerStatsUI.transform,
                 anchorMin: new Vector2(0.5f, 0.1f),
                 anchorMax: new Vector2(0.5f, 0.5f),
                 position: new Vector2(200f, 100f), // width & height
                 width: 250f,
                 height: 60f,
                 color: Color.yellow,
                 outline: false,
                 outlineColor: Color.white,
                 font: GUIManager.Instance.AveriaSerif,
                 fontSize: 20,
                 addContentSizeFitter: false
                 );

                GameObject PierceTextbtn = GUIManager.Instance.CreateText(
                 text: $"Pierce : {VillagerGeneral.GetPierce(villagerZDOID)}",
                 parent: VillagerStatsUI.transform,
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
            }

            if (!VillagerStatsUI.activeSelf)
            {
                VillagerStatsUI.SetActive(true);
                GUIManager.BlockInput(true);
            }
            else
            {
                VillagerStatsUI.SetActive(false);
                GUIManager.BlockInput(false);
            }
        }

        private static void CloseVillagerStatMenu()
        {
            if (VillagerStatsUI != null) VillagerStatsUI.SetActive(false);

            GUIManager.BlockInput(false);
        }
    }
}
