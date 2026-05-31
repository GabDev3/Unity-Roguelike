# TODO
## Step 1. read mechanisms/rooms.md and understand how the dungeon generator works, what are the prefabs and scripts used for it and how they are connected
## Step 2. Make prefabs and also link them to the dungeon generator, so they will get instantiated properly when i hit the play button:
### 3 more default rooms - call them the same as curent default room, just add number at the end, rooms info:
#### each room needs to have different tilemap - check the tilemaps in the current default room and use simillar ones - just make the layout and dimensions different
#### for each room make 3-4 different content data prefabs, with different enemies and items, so that the rooms will be more varied - check the current default room content data prefabs and use them as a reference for the new ones, i need: 1-3 enemies, 20% chance for a chest (also fix chests currently being placed inside the tight space in default rooms - you can actually add chests to the Obstacles layer as well)
### Boss room - create boss npc and a boss room. I will add some cool sprite later - just go with the monk sprite so far. make a prefab of it and add it to the boss room content data prefab, so that it will spawn in the boss room. Boss room needs to be muuuuch bigger than other rooms, because the boss itself will be bigger, will have 10x hp of current npcs and will spawn 10 projectiles all around him as his attack pattern. Also handle finishing game - if boss dies then user wins and success screen gets showed

## Step 3. Fix current rooms
### In starting room spawn ALWAYS a:
#### sword and armor (one prefab of each)
#### upgrader npc (already done - dont do nothing here)
#### NO ENEMIES IN STARTING ROOM

## Step 4. UI 
### Death screen - display "Game Over" text and a button to restart the game, which will reload the scene, or quit
### Success screen - display "You Win!" text and a button to restart the game, which will reload the scene, or quit
### Hit range indicator - add small sword icon when player is in range of hitting the enemy. disappear when he is not
### Hit timing indicator - there is attack speed here implemented. When player launches attack (range or melee - doesnt matter) show a small circle icon that will be filled in as the attack gets ready, and when it is fully filled, player can attack again.
### main menu - in controls just show controls - no editting there
### game start - before instantiating a SampleScene add class choosing - user can choose melee or ranged class. Currently, the player is kind of a hybrid - it has 2 scripts on him. Differentiate that - change current Player prefab to 2 prefabs one for melee and second for ranged. just copy the current prefab and remove ranged component and also fireball gameobject from him to get melee. on the crrent one remove melee component and activate ranged script component to get ranged. adjust ranges as well so they won't be astronomical



# Summary:
The task list is long, so take your time and think long time if needed - it can even take you up to 10 minutes or more, so I need you to **think carefully, HARD, DEEPLY, with a lot of effort** - I need this done perfectly