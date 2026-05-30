
# Dungeon generator
there is a dungeon generator gameobject prefab that is also in SampleScene.

It contains:
## LevelGraph
### also a prefab, has rooms layout and connections between them. room templates can be added globally, as also into each node separately.

## DungeonGenerator Script

## Simple Room Content Spawner Script
### Here the content for rooms is defined - these are RoomContentData prefabs. also prefabs that are later used in roomconentdata are listerd here

## RoomContentData
### A data object, where the prefabs are set and weights can be adjusted for each enemy. also ranges of spawned objects can be set for separate objects as well as for types


## Spawn Points
### Objects with SpawnPoint script, where the content is spawned. they are placed in rooms and have a type, so that only content of the same type is spawned there. also they have priority and radius, but radius should be mostly kept low.