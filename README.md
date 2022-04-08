# GOYA's NIGHTMARE
"IV Cultura Abierta" Game Jam - Unity Project

## GVD
### Vision
In his darkest era, Federico de Goya enclosed himself at home and painted macabre art on its walls. In this Slenderman-style game, you will impersonate Goya himself being trapped inside a nightmare and being chased by his own art, in a terrorific intent of recovering the control of his work.

### Moodboard
![unnamed](https://user-images.githubusercontent.com/58336040/162426137-2b81420f-9db8-468e-ab64-7685c119e29d.png)

### Story and Gameplay
Explore a nightmarish house, supposedly Goya’s home, where you will find his art scattered around on the walls. The goal is to find and recover these five pieces of art without being trapped by Saturno, a soul-eating monster that will chase you around the house.

The sensation of being trapped in a laberynth will never disappear, as the walls and hallways will move around while you collect the art. The layout of the house will be different every time.

The only room that will not be affected by these changes is the central hub. Your starting point and safe area. Here will be displayed the empty paintings that you need to find, and they will be filled with the corresponding art pieces when you recover them. There is no HUD in the game so this will be your only option to keep track of your progress.

### Core Pillars
| Gameplay Pillars        | Main Mechanics          |
|-------------------------|-------------------------|
| - Terror                | - Seek objectives       |
| - Labyrinth exploration | - Lantern switch on/off |
| - Being chased          | - Changing map layout   |

----

   **(STOP READING IF YOU ARE WILLING TO PLAY THE GAME!)**  
   **(Spoilers below can harm the illusion of fear)**
   
----

### Level Design
Requirements:
 - 5 layouts for each obtained Painting, with increasing difficulty about getting trapped by Saturno.
 - Must feel labyrinthic.
 - Avoid cul-de-sacs.
 - The player must be able to reach any art room with all layouts.
 - The art rooms must be equitably distributed in the map.
 - The central hub must be at the center of the map, and have several exits that will always stay the same.

![imagen](https://user-images.githubusercontent.com/58336040/162427669-5f6d2684-cdeb-4438-8fd6-73af85eed7ca.png)

### Lose Condition - SATURNO BEHAVIOR
 - Saturno will chase the player through the map, using the Navmesh system.
 - He will be able to traverse ghost walls.
 - Saturno will not move when the player has the lantern shut off.
 - Saturno speed is between the players walking and sprinting speed. The player will always be faster than Saturno when sprinting, but he can get tired, so Saturno will be faster when he is walking.
 - If Saturno gets to the player, trigger Game Over. This can happen if the player can’t react, or if he was tired after sprinting.
 - If the player escapes, after 3 seconds of not watching him, Saturno will teleport to a random spawn point from the spawn point list. (Check distance(player,spawnpoint) to avoid spawning him in a too close position).

Since escaping Saturno is relatively easy, we can consider that the experience of fear itself is the challenge that the player has to face. The player loses if he can’t stand the tension and quits the game before finishing.

At the beginning of the game, provide the player with some clue that turning off the lantern can be helpful.

### Necessary Assets
 - Mesh Collider/NavMesh
 - House assets (walls and decoration, victorian style)
 - Painting Billboards
 - Saturno (glows, audio effects)
 - Music:
   - Ambient
   - Tense breathing
 - SFX:
   - Walksteps
   - Collectable
   - Scarejumps
 - VFX
   - Vision distortion shader
   - Fire 
   - Painting glow or particles, to show interactable
 - Illumination
   - Ambient light
   - Lantern (flicking)
   - Player light
   - Room light (principal)
 - Prefabs
   - Hallway tile, corner & intersection (will also fit doorways and ghost walls)
   - ![imagen](https://user-images.githubusercontent.com/58336040/162430268-25ac4e90-c040-4373-8e07-80f3e5957893.png)
   - Ghost wall & ghost door
   - Doorways
   - Decoration with colliders
   - Painting canvas

### Scripting
 - Main Menu buttons
   - Start
   - Exit
   - Controls/Settings
 - Player Controller
   - Player Movement (WASD)
   - Lantern (F)
   - Pick Up Art (E)
 - Game Controller    
   - What paintings have been picked up
   - Play/Pause/Exit/GameOver controls
 - Camera Controller
   - Mouse direction
 - Saturn AI
   - Follow
   - Pathfinder
   - Pathing randomizer
 - Art Paintings Controller
   - Art disappears when it is picked up and appears at central hub
   - Trigger Hallway reorganization
 - Hallway reorganization (int)
   - List of ghost walls
   - math relation → open o closed
 - Lantern
   - Relation to Saturno behavior
   - Feedback visual and audio

###ROADMAP
9-Dec -> Inscription

10-Dec+
 - Level design
 - Prepare and find Assets

13-Dec+
 - Level Building
 - Scripting

16-Dec+
 - Polish
 - Build

19-Dec -> Submit

20-Dec+ -> Post-Polish








