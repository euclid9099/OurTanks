# OurTanks

The game is very much based on the mini-game "Tanks!" from the Wii Play game. We want to adopt most of the original ideas, but as a version for the PC. The idea is that 1 - 4 players defeat increasingly difficult, 2-dimensional levels. Basically, there should be two game modes: Campaign, where all computer opponents in a level have to be eliminated, and free4all, where everyone fights everyone else, and the last survivor is the winner. (Optionally, more modes could be added at the end).

Each player controls a tank that can move, fire projectiles and plant bombs. The gun barrel moves independently of the base, it can aim in a different direction than the tank moves. Although player tanks usually have the same abilities, the tanks should still be able to be created with different values (e.g. faster tanks, no mines, etc.).

The levels also contain enemy tanks, which of course have to be equipped with a certain intelligence. This point probably also causes the greatest complexity. The bots have to move towards the player, fire projectiles at the player, and at the same time avoid death themselves. The enemies also use different tanks, because in order to make the levels progressively more difficult, it is very useful if the tanks become stronger.

Building elements that can be used in levels are:
- Normal ground (tanks, projectiles and bombs behave normally).
- Wall (tanks and projectiles are stopped, bombs also damage behind it)
- Destructible wall (tanks and projectiles are stopped, bombs destroy the wall) 
- Tank spikes (tanks are destroyed, projectiles and bombs are unaffected)
- Holes (tanks are stopped, projectiles and bombs are unaffected)

Campaigns can be themed by specifying new image sources for objects in a configuration file. For example, one campaign can take place in the desert, another in the ice.

Depending on how fast we progress, we would like to add the following:
- [ ] Level/Campaign Editor (players can create, share and play their own levels).
- [ ] More game modes (like capture the flag, zone defence, ...)
