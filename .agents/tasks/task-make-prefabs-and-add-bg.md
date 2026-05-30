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

## Step 4.