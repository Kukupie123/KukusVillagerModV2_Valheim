# Kuku's Villager Mod

## NOTICE 
1. I would like to thank [Horem](https://valheim.thunderstore.io/package/Horem/) for helping me out so much for this update as well as providing his awesome Assetbundle. <br>
2. I will not be active for a while. Real life issues :) <br>
3. May work initially in Multiplayer as we got rid of beds BUT has not been designed with multiplayer in mind so there may be issues specially with work commands as it alters container's contents. <br>
4. Config file has a section called [Spawn Point HUT] Which is obselete and not used currently.<br>

## Installation (manual)
Paste the plugins folder in valheim/BepInEx/

## Features
1. Recruit villagers found in biomes. <br>
2. Villagers will have random stats. Every villager is going to be unique<br>
3. Upgrade Recruited villagers by using special items on them.<br>
4. Assign villagers bed, work posts, defense posts, containers. <br>
5. Command villagers to guard their beds & Defense Post, Follow you <br>
6. Villagers can also help player by picking up items, filling up smelters, Chopping trees <br>
7. Manage and control your villagers all at once or by faction using Runik Kuchuk Tablet. <br>
8. Create factions and assign individual villagers to control selected numbers of villagers. <br>
9. Simple GUI for interacting with villager and Runik Kuchuk Tablet <br>
10. Customization for flexible gameplay experience.

## Bug :
1. Villager may get lit on fire, especially when working.<br>
2. When chopping trees they may not attack or attack in the wrong direction. <br>
3. Villagers when chopping trees may sometime not play the attack animation. <br>
4. Villagers who are working will not attack enemies most of the time. This is the side effect of villagers AI being updated constantly to not keep staring at player.<br>
5. Villagers will get stuck if path is blocked but get teleported eventually. <br>

## Changelog
<h3>3.3.0</h3>
1. Faction System added. Now you can group villagers into factions and command all of the faction members at once. <br>
2. Assign Items of many villager at once using Runik Kuchuk Commander.
3. You can now upgrade villagers by interacting with the villagers and selecting the upgrade villagers option. It will select the best item from your inventory and use it to upgrade the villager.

<h3>3.2.0</h3>
1. Villagers learnt repairing skill. 1st iteration. <br>

<h3>3.1.7</h3>
1. Added ability for villager to announce when they take damage. <br>
2. Fixed another custom prefab bug related to mistland villagers. <br>
3. Fixed Gold requirement being 0 for mistland villagers. <br>
4. Worker Villagers will now be somewhat able to defend themselves. Configurable <br>

<h3>3.1.6</h3>
1. Fixed custom prefab not working. For custom models from other mods, it has to load before this mod does. RRRNPC will not work because it loads very late. I tried to get past this to no avail.<br>
2. Villager Bed will now drop back woods used to craft them. <br>

<h3>3.1.5</h3>
1. Villagers regen HP when close to bed and in Guarding_Bed State.<br>
2. Fixed health upgrade not working on villagers. <br>
3. AI will not get stuck as much as it use to in last update. <br>
4. Fixed bug where villagers HP would be super low everytime the game is loaded. <br>
5. Added option in config to use custom prefabs. <br>
6. Villagers will now need coins to be recruited. Configurable <br>

<h3> 3.1.4</h3>
1. Bug fixes related to unassignable work post, bed, defense post <br>
2. Chop wood bug fix<br>
3. Fixed healing spell killing player

<h3>3.1.0</h3>
1. 12 Unique Human NPC included, Thanks to Horem for his assets :) <br>
2. Different villagers found in different biomes.<br>
3. Villagers of Plains can generate shields to protect itself and nearby <br>
4. Villagers of mistland can generate shields as well as spray yellow smoke to heal themselves and nearby allies. <br>
5. Villagers should stare at the player less when working and do it's tasks more consistently.<br>
6. Villager can only do one type of work now. This is because works were conflicting and causing serious bugs and getting hard to manage. <br>
7. Next Major update will introduce villagers spawn area such as hut where you will find group of villagers chilling. This is to reduce villagers found in wild and have them in a small residence which will look nice and immersive. You can then visit the area to recruit villagers consistently.  Thanks to Horem for the idea. Couldn't get it to work this update :) <br>

<h3>3.0.1</h3>
1. Added config option for resources needed to upgrade villagers. As well as others config settings. Make sure to check out the config for full info :)<br>
2. Wanderer's (Villagers found in the wild) stats will have stats scaled based on the biome it was found at. (Configurable)<br>
3. Fixed versioning bug<br>
4. Villagers with special Combat ability will have "*" Mark on their name <br>
5. Next update will replace the villagers with human NPCs (3.1.0) <br>

<h3>3.0.0</h3>
1. Complete Overhaul of how the mod works.<br>
2. You will now find villagers scattered through the biome. <br>
3. Every Villager will have Random stats such as HP, DMG, and sometime Special Power such as fire, frost, etc. <br>
4. Upgrade Villager's Health and Combat stats by giving them certain items.<br>
5. Beds are not mandatory. You can assign beds or not if you don't want them. <br>
6. Completely new GUI when interacting with villager. It will show stats, orders and assignment options such as assign bed, container, defense post. <br>
7. Completely new GUI for Villager Commander (Now called Runik Kuchuk Tablet)<br>
8. Naming villagers. <br>
9. Once a villager is dead they are gone permanently. Be careful. <br>

<h3>2.1.4</h3>
- IMPORTANT! YOU WILL HAVE TO REASSIGN YOUR CONTAINERS OR ELSE THINGS WILL NOT WORK PROPERLY.
- Improved Logic for smelting and pickup job. Now the container and the work post can be far away from each other and still work. <br>

<h3>2.1.3</h3>
Hot fix update <br>
- Bug fixed where villager won't fill upblast furnace <br>

<h3>2.1.2</h3>
- Silly Bug I overlooked fix<br>
- Fixed Discovered Pickup bug<br>
- Fixed Smelting work bug where villagers would fill up smelts even when not needed<br>
- Added a boolean config variable that will decide if the villagers should run or walk when working<br>

<h3>2.1.0</h3>
- Worker Villagers Added
- Two work skills : Pickup and store & Fill Smelter
- Commands and AI Tweaks
- Ability to command individual villager by using new items added by this mod
- Ability to assign Work Post and a container for villagers to work with. Recommended to have Work post and container close by.
- More configuration options, Make sure to delete config file of old version<br>

<h3>2.0.0</h3>
1. Spawn system is based off of Valheim's spawn system which means it is going to be reliable.<br>
2. No more huge fps drops and freezes<br>
3. Revamped the way defense posts are handled. Now you can choose which villager to go to which post.<br>
4. The distance between the beds, posts, villagers no longer matter and will work regardless if they are loaded in memory or not.<br>
5. Changed few commander club commands.<br>

<h3>1.1.0:</h3> 
1. Villagers go back to their last state (Guarding Bed or Defending Post) When loaded.<br>
2. Replaced Teleport Following Villagers with Move villagers, they will now move to the aimed location and only teleport if moving to that area is not possible.

<h3>1.0.1 : </h3>
1. Added Configurations for heavy customization.<br> 
2. Added A new command to teleport following villagers to looking direction. <br>
3. Refined Bed&Villager Link system.

<h3>1.0.0 : </h3> Re-released after updating the code from scratch.


# Mirror Link
https://www.nexusmods.com/valheim/mods/2131