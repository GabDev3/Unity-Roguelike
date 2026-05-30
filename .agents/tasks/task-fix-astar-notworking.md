# Problem
## The walls get generated through edgar at runtime, end all walls are one gamobject inside GeneratedLevel gameobject.
## Walls dont have layer set to obstacles - a layer which i have in Astar defined. I did set the layer manually during game runtime, but it did not work.
## Another problem is that the grid doesnt always spawn to entangle all rooms - I can set it to basically be wider so no problem here as well, but even when it randomly spawned correctly, it did not work.
## I did some debugging and when i have option "Show Unwalkable nodes" off it also shows the nodes that are inside the walls.
## When I tuned on "Show Search Tree" option it did highlight the wall nodes as well, so it basically means that it finds them walkable. It happened after I set the layer to obstacles manually at runtime (eventhough Im not pretty sure that counts).



# tasks
## Fix the Obstacle not getting set - set them during start.
## Identify if there is another underlying problem with pathfinding, that makes "Obstacles" layer objects walkable. Check prefabs if needed - scan them to check if all my prefabs have everything set as necessary. If not then fix it.
## Make no mistakes!