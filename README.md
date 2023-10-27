# COMP4990_Dimension-Tamer
COMP 4990 Final Project (Jake Haas and Drew St. Amour)

Our game is a roguelike, monster-taming, and turn-based combat game that is inspired by the original series Pokemon games. The player begins the game with a level 1 creature and has 30 minutes to collect progressively more powerful creatures spread across 5 worlds. The main objective of the game is to defeat a boss enemy which is found in the final world. In order to make each playthrough more interesting, generative AI is used to generate new creatures and different looking environments for each run. In addition to this, procedurally generated maps ensure a unique but balanced experience for every playthrough.


Roguelike
- Main menu that is returned to after player finishes run (defeat or victory)
- World generates before each playthrough
- No save features

Monster-Taming
- The player has an inventory with six slots to hold tamed creatures
- One slot is dedicated as the equipped slot
- Creatures are found in encounter zones spread across the world
- Creatures = type(ID), image, hp, 3-moves from pool of 6 for that type, element, attribute stats (max hp, defence, speed, attack), level
- Wild creatures can be captured and put into the player's inventory

Turn-Based Combat
- Inspired heavily by the original pokemon games
- On players turn they can choose one of their equiped creature's actions, equip a new different creature, flee from battle, or use a tranquilizer
- Player's creature is shown on one side of the screen, opponent's creature is on the other
- Creatures are static images
- Encounter ends when the opponent is captured, the player's creatures all die, the player flees, or the game timer runs out.

Generative AI
- Creature types and images are generated upon starting the game
- World tiles are generated upon starting the game

Game World
- Starting world and ending world are the same every run
- 5 Procedurally generated worlds in-between
- Made of tiles (3 types; walkable, encounterable, solid)
- Maps are split into many biomes (Minimum 10 areas)
- Each biome hosts certain element typed creatures
- Portals are found within the map which lead to the next world (generation begins upon entering the portal)
- The same creature type can be found in a local area

Procedural Generation
- Biomes must connect to each other (no unreachable areas)
- Biomes must be diverse, at least three biome types are able to be found per map
- Each biome has a unique shape and layout
- Specific biomes follow unique generation rules (affect shape, location of encounter zones)

Final Boss
- Found in the final world
- Defeating this enemy is the win condition
- Must be defeated within 30 minutes of starting the game
- Always the same



