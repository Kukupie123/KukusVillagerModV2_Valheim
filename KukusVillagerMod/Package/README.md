# Kuku's Villager Mod

## NOTICE:
Not tested in multiplayer and will probably not work as intended.
DELETE OLD VERSION's DLL, CONFIGS EVERY UPDATE.

## Installation (manual)
Paste the plugins folder in valheim/BepInEx/

## Features
- Place beds for villagers to spawn from.
- Villagers can help you by pickuping up and storing valuables, Filling up smelting stations.
- Decide which villager can do what kind of work
- Villagers can Guard a Defensive Post position by assigning
- Villagers can go on journey with you
- Villager can understand several commands such as Guard Bed, Guard Defense Post, Follow Player, Move to location, Start Working
- Order individual Villagers by using items added by this game
- Order All villagers at once by using Villager Commander Club

## USAGE GUIDE :
### Spawning :
- To Spawn villagers Craft a Villager's Bed by using your hammer. You will find a new Section "Villager"
### Working Villager
- To Make them Work we need to assign them Work Post and a container to store items. 
- Craft a Work Post if not done already.
- Interact with the bed of the villager you want to assign Work post to, then go to the desired Work post and interact with it. You will see a message that will confirm that the Work post was assigned to the bed.
- Interact with the bed of the villager again and interact with the container you want the villager to use. You will see a message to confirm that it was successfull..
- If you hover over your villager you will see something called "Work Skill". By default a villager will have no work skill.
- To teach your villager how to Pickup and store items, Open your crafting menu (TAB by default) and craft a "LabourSkill_Pickup". For teaching Smelting skill craft "LabourSkill_Smelt" item<br>
Use this item on the villager the same way you use items in original items objects in the game such as placing items in itemstand.
- For commanding all villagers to work craft&Equip a Villager Commander Club and press the Start Work Key (Numpad 3) by default.
- For commanding villagers individually craft "LabourerFruit" and use it on the villager.
### Defending Post:
- To make villagers defend a post we need to assign them Defense Post by interacting with their bed and then interacting with a defense post we want them to defend at.
- To command all villagers to defend their assigned post craft and equip Villager Commander Club and press Defend Post key (Numpad 2) by default.
- To command villagers individually craft "WatcherFruit" and use it on the villager
### Guarding Bed:
- To command all villagers to guard their bed craft&Equip Villager Commander Club and press Guard Bed key (Numpad 1) by default.
- To command villagers individually craft "GuardianFruit" and use it on the villager
### Taking villagers along with you.
- To make villagers follow you, interact wit them.
- To make your followers move to a location, equip your villager Commander Club and press Move followers key (Numpad 6) by default.
- To make followers come back to you, equip your villager Commander Club and press Call followers back key (Numpad 5) by default.

## KNOWN ISSUES:
- If you are close to villagers they might sometime not do anything and just stare at you until you move far away. I will try to fix this in future release
- Villagers may go the wrong way, no idea why this happens. To help fight this I added Teleporting which will TP the villager after a while
- Report bugs in my nexus page
- Doesn't work in Multiplayer Scenario. Don't have any idea how multiplayer works in unity&valheim so I can not do anything without guidance :)

## Changelog
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
<h3>1.1.0:</h3>  <br>1. Villagers go back to their last state (Guarding Bed or Defending Post) When loaded.
<br>2. Replaced Teleport Following Villagers with Move villagers, they will now move to the aimed location and only teleport if moving to that area is not possible.

<h3>1.0.1 : </h3>Added Configurations for heavy customization,<br> Added A new command to teleport following villagers to looking direction. Refined Bed&Villager Link system.

<h3>1.0.0 : </h3> Re-released after updating the code from scratch.


# Mirror Link
https://www.nexusmods.com/valheim/mods/2131