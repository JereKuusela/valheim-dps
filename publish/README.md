# DPS

5hows DPS, stamina usage and other statistics. Accurate and configurable. Also includes an experience meter.

# Manual Installation

1. Install the [BepInExPack Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim).
2. Download the latest zip.
3. Extract it in the \<GameDirectory\>\BepInEx\plugins\ folder.
4. Optionally also install the [Configuration manager](https://github.com/BepInEx/BepInEx.ConfigurationManager/releases/tag/v16.4).
5. Optionally also install [Server devcommands](https://valheim.thunderstore.io/package/JereKuusela/Server_devcommands/) to use it as an admin on servers.

# Instructions

Use `/bind [key] [command]` to quickly toggle features on and off. For example `bind keypad1 dps`.

Available commands:

- `dps`: Enables, resets or disables the DPS meter.
- `exp`: Enables, resets or disables the experience meter.
- `dummy_reset [PARAMETERS]`: Kills all training dummies and spawns a new one.
- `dummy_spawn [PARAMETERS]`: Spawns a training dummy. Use if you need multiple dummies.
- `dummy_kill`: Kills all training dummies. Use for a final clean up.

# Parameters

Damage resistances and status effects of the training dummies can be configured. By default, dummies have no status effects and are neutral to all damage (except chop and pickaxe which they ignore).

Format for resistances is `damagetype=amount`. If amount is omitted, neutral resistance is used.

Available damage types are \*, blunt, chop, fire, frost, lightning, pickaxe, pierce, poison, slash and spirit.

Available amounts are ignore, immune, very resistant, resistant, normal, weak and very weak. Numeric values also work: -, 0, 0.25, 0.5, 1, 1.5, 2.

Format for status effects is `effect=seconds`. If seconds is omitted, default duration is used.

Available status effects are barleywine, bonemass, cold, corpserun, eikthyr, elder, freezing, frostresist, moder, poisonresist, shield, tared, wet and yagluth.

For example `dummy_toggle \*=- chop wet` would spawn a wet dummy that is only vulnerable to chop damage (like trees).

# Configuration

After first start up, the config file can be found in the \<GameDirectory\>\BepInEx\config\ folder.

Configuration can be used modify how the game works to make testing easier.

Configuration is only active when either DPS or experience meter is displayed.

- Override skill levels: Overrides all skill checks to use a given skill level.
- Min/max damage range for players: Overrides the random damage range for player attacks.
- Min/max damage range for creatures: Overrides the random damage range for creature attacks.
- Maximum attack chain levels: Caps the maximum attack chain.
- No stamina usage: When enabled, removes all stamina costs.
- Auto shoot bow: When enabled, the bow is shot automatically when fully drawn.

For example for a controlled setup: remove stamina usage, override skill levels to 100 and set damage range to 0.

# Usage

DPS meter automatically starts tracking when you start attacking. The tracking automatically pauses until another attack is performed. For new tests, use the dps command to reset the meter.

- Time shows the total tracked time and amount of hits.
- DPS shows the damage per second, total amount of damage and damage per used stamina.
- Stamina shows used stamina per second and total amount of used stamina.
- Total stamina shows all used damage, including usage from non-attacks. Only displayed if different from above.
- Staggering shows caused staggering per second and also the total amount.
- Attack speed shows hits per second and also average time per hit.
- Damage taken shows damage taken per second and the total amount. Only displayed if taken any damage.
- Structure DPS shows damage caused to structures and destructibles. Only displayed if any damage done.

Experience meter automatically starts tracking new skills when gaining experience. The tracking automatically pauses until more experience is gained.

- Experience gain shows multiplier to all experience gain.
- Shows skill level and progress towards the next level for all tracked skills.
- Shows experience per second and total amount of gained experience for all skills.

Note: Skill level can be a decimal number. This is a bug in the game and not an issue with the mod.

# Changelog

- v1.2
	- Adds a new setting to automatically shoot with the bow.

- v1.1
	- Adds a new icon.
	- Replaces the admim check with a cheat access check.

- v1.0
	- Initial release.

Thanks for Azumatt for creating the mod icon!