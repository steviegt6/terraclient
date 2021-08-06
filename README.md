<h1 align="center">
Terraclient
</h1>
<p align="center">
  <img src="https://forthebadge.com/images/badges/made-with-c-sharp.svg"> <img src="https://forthebadge.com/images/badges/0-percent-optimized.svg"> <img src="https://forthebadge.com/images/badges/contains-tasty-spaghetti-code.svg"><br>
  <img src="https://forthebadge.com/images/badges/open-source.svg"> <img src="https://forthebadge.com/images/badges/uses-git.svg"> <img src="https://forthebadge.com/images/badges/powered-by-black-magic.svg"><br>
  <img src="https://forthebadge.com/images/badges/reading-6th-grade-level.svg"> <img src="https://forthebadge.com/images/badges/built-by-neckbeards.svg">
</p>

----

Terraclient is a new-age Terraria client built for Terraria 1.4.2.3. As of now, it is a heavy work-in-progress and not ready for public distribution; however, anyone is capable of building it themselves.

## Features

(Note that this list is ever-expanding and not always complete)
- Godmode.
- Journey character access bypassing (does not work in some servers, namely ones with multiple worlds linked together).
- Fullbright.
  - Plus options for slightly better fullbright performance.
- Map teleportation.
- Journey Mode spoofing.
  - Gives your character access to all Journey Mode features like research, even on servers. This does not make the server register that you have a Journey Mode character, though.
- Player ESP using multiplayer nameplates.
  - Extra info option as well.
- Forced wide-screen scaling removal and forced main-menu UI scaling removal.
- Forced PvP/team-switching cooldown removal (does not work in TShock servers).
- The ability to switch between actual player difficulties (saves the file as well).
- The ability to unlock all Journey Mode items in the item spawning UI (requires rejoining the world/server).
- The ability to unlock all bestiary entries (requires re-opening the UI).
  - Furthermore, added a "Spawn" button to spawn any NPC on the bestiary (does not work on TShock servers).
- The ability to regenerate your UUID from the main menu.
- Chat command system (W.I.P., functions).
- Infinite chest reach cheat.
- Player bank (i.e. piggy, safe, etc.) cycling from the inventory. Requires the aforementioned infinite chest reach cheat to work.
- Deprecated item support.
- Sexy menu changes that allow for UUID regeneration, etc.
- Command that spawns a torch that will crash a vanilla user's game.
  - *And* safeguards against this crash on your own client, meaning anyone else that has a similar hack won't work on you.

## Planned Features

- Modular command support.
- Infinite flight.
- Freelook (being able to unhook and move the camera view without moving the player).
- Direct player stat modifications.
  - Such as: Directly modifying HP, mana, defense, speed, etc.
- Modification of downed bools (Bosses, events etc.) and other important fields (through commands).
- The incorporation of some cheat commands from ModHelpers (thanks Hamstar!).
  - See: https://github.com/hamstar0/tml-hamstarhelpers-mod/tree/master/HamstarHelpers/Commands/Cheats, https://github.com/hamstar0/tml-hamstarhelpers-mod/blob/master/HamstarHelpers/Commands/Cheats/CheatToggleCommand.cs#L20 (will be the part of the command system).
- InventoryPeek.
- Blink/Checkpoint
  - Basically not sending movement packets to the server, causing your player to teleport once the hack is again disabled.
- Butcher.
- [Tome of Greater Manipulation](https://terrariamods.fandom.com/wiki/Joostmod/Tome_of_Greater_Manipulation)-esque capabilities.
- Auto-Armor (equips the best armor in the inventory).
- Infinite build reach and speed.
- Change respawn time / no respawn timer.
- Demi-God (Constantly healing, or cannot die even when reaching 0 HP).
- No Collide / Ghost.
  - Potential no-clipping instead of flight and ghost being separate.
  - The ability to not collide with platforms.
- Forced spawn rate changer.
- Forced Time changer.
- Magnetz: suck up all dropped items in the world.
- Reveal map.
- Command to give the player the required items for the stage of the game.
- Keybind that swaps the hotbar with the bottom / top row of the inventory.
- Right-click debuffs to cancel them.
- "Favorite" a buff to make it infinite.
- A cheat that cycles through any selection of dyes to apply to the player render on the client and send in the netmessage that concerns vanity when on multiplayer. 
  - Kind of like skin blinking on Minecraft, just a way to make your vanity more dynamic. If not this, it would be super cool to have other visual things to look flashy with.
- A way to paint with particles in multiplayer.
- A nickname tool to allow the player to change their visible player name on servers.
- Remove camera zoom limit.
- The ability to detect players who are invisible.
- Teleport function with map off, might add a hotkey to replicate the RoD's function. Also, settings for teleportation:
  1. Raw relocation.
  2. Use teleportation packet.
  3. Raw relocation, but teleports for every frame you hold down the button.
  4. Teleportation packet, but teleports for every frame you hold down the button.
- Something like Tool God, but it makes weapons that are favorited OP.
- No knockback.
- Command that causes damage to other player.
- Antisocial-like feature.
- Lock-on Targeting even without a controller for an aimbot-like system that even works on players.

## Contributing
Anyone is free to contribute. You can view the compilation process and more specific details in the `CONTRIBUTING.md` file (TODO).
View a list of planned features [here](https://github.com/TML-Patcher/Terraclient/issues/1), and view issues [here](https://github.com/TML-Patcher/Terraclient/issues).

### And remember...
![Screenshot 2021-05-11 at 7 53 07 AM](https://user-images.githubusercontent.com/27323911/117837093-ff370380-b22d-11eb-9cbf-107253645ffb.png)

Be afraid! Ignore the fact that this program is open-source and utilitizes *trusted tModLoader software* to perform its *open-source and public patches*! :((((
If you have any actual questions about functionality, I can usually explain almost everything in full. Happy hacking.
