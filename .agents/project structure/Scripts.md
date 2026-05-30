most important - has all the code of the project. classes dragged on gameobjects in unity

# Attacker
### Classes handling attacking logic for npc and user.
### Base Attacker -> Base Melee or Base ranged
### Base Melee -> Melee Enemy or Melee Player
### Base Ranged -> Ranged Enemy or Ranged Player
### contain handling states of npc. 3 states: patrol - moving randomly. chase - moving toward player. attack - currently casting an attack

# Character stats
Only health

# Controller
### Master classes: abstract base adn npc and player.
### Controlls global actions of in game characters, uses rest of classes from project

# Dungeon
## Contain everything related to dangeon generating
### door placement
### item placement in room
### specific items placed on spawn points

# Editor
Main menu handling

# Helper
Attack range trigger script for attackers

# Interaction
Interactible objects

# Interfaces
Taking damage and attacking handler

# Movement
Player Movement

# UI
## Management whole game ui
### main menu
### pause
### player hud
### upgrader npc ui

# Weapons
spell loging and data for creating it