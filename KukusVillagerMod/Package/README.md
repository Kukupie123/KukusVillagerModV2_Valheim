# Kuku's Villager Mod
DELETE OLD VERSION's DLL AND RECOMMENDED TO START A NEW WORLD
## Installation (manual)
Paste the plugins folder in valheim/BepInEx/

## Features
1. Two types of villagers : Ranged and Melee, All with different strength based on the type of bed created.
2. Place  a bed for them to spawn. If they die there is a Cooldown.
3. Place Defensive Post in locations you want your villagers to guard.
4. Melee Defensive post is where Melee based villagers will go to and guard, Ranged Defensive post is where Ranged Based villagers will go.
5. Craft a Villager Commander from your crafting Menu to command them.

## Recommendations
6. Recommended to keep beds and defensive posts close because of the way game loads and unloads resources
7. The more the villagers, the longer the freeze there will be when they are loaded back in memory. 

## Commands
KeyPad1. Go to their Beds Location and Guard
Keypad2. Aim at a villager and press to make them follow you
Keypad3. Command Villagers to Guard Defensive posts
Keypad4. Delete all nearby Defensive posts
Keypad5. Delete all Nearby villagers
Keypad6. Delete all nearby Beds
Keypad7. Show stats such as villagers Count

## Changelog

1.0.0 : Re-released after updating the code from scratch.


## Known issues
Because of the way game handles Loading and unloading entites, it sometimes load entities but not the infomations saved in them so it is hard to always sync properly and I tried to the best of my ability to deal with it. 
The freeze is something that should be gone in future updates after I shift the functions that take time into coroutines

