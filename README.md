# Cityscape Shuffle

**Minimal loading screen background randomizer for Cities: Skylines**

Sick of looking at the exact same backdrop every! single! time! you load a saved game?

This mod randomly selects from all available background images already included in the game (including any DLC content you own).

Just turn it on -- no custom files or detailed configuration needed.

Also works when...
- starting a new game
- starting the map editor
- starting the theme editor
- starting the asset editor
- starting the scenario editor.

---

## Installation

### Via Steam Workshop (Recommended)
1. Subscribe to this mod
2. Subscribe to [Harmony 2.2.2+](https://steamcommunity.com/sharedfiles/filedetails/?id=2040656402) (only requires installation, not activation)
3. Enable in Content Manager → Mods
4. **Important:** Restart Cities: Skylines after first enabling the mod

### Manual Installation
1. Download from [Releases](https://github.com/Jeaudoir/Cityscape-Shuffle/releases)
2. Extract to `%LOCALAPPDATA%\Colossal Order\Cities_Skylines\Addons\Mods\CityscapeShuffle\`
3. Install Harmony 2.x from Steam Workshop
4. Enable Cityscape Shuffle in Content Manager → Mods
5. **Important:** Restart Cities: Skylines after installation

---

## IMPORTANT: Restart required!

The mod captures all available background images the first time the main menu loads. If you enable it mid-session, those background images cannot be loaded (as BackgroundPanel.Awake() can only run once per game session).

The mod could have shown a fallback image -- but then you'd think "What the heck?! This is the exact same thing I've been staring at for years, this mod does not work!"

So I decided to show an ugly red box instead -- now you'll think "What the heck?! This mod is doing crazy stuff, I should go complain on Steam Workshop or GitHub," where you'll see the reminder that a restart is required.

---

## Configuration

Access via **Options → Mod Settings** in-game:
- **Enable debug logging** - Writes detailed logs to `output_log.txt` for troubleshooting

Settings are automatically saved to `CityscapeShuffle.xml` in your game's settings folder and persist between sessions.

---

## Compatibility

Safe for existing saves. Can be enabled/disabled anytime without breaking anything.

**Incompatible with:**
- **Loading Screen Mod Revisited** - Both mods patch the same game methods and will conflict.

If you need advanced loading features or customizable backgrounds, use [Loading Screen Mod Revisited](https://steamcommunity.com/sharedfiles/filedetails/?id=2858591409) instead. If you want a one-click solution that just works, use Cityscape Shuffle.

---

## How It Works

Uses Harmony to intercept `LoadingAnimation.SetImage()` and replaces the default background material with a randomly selected one from the game's existing collection.

Brief technical overview for the curious:

1. **Mod.cs** - Entry point; installs Harmony patches on game startup and manages settings persistence
2. **BackgroundPanelAccessor.cs** - Captures background images from the main menu using reflection (reads the game's internal list)
3. **LoadingPatches.cs** - Intercepts loading screen display, gets a random background from RandomBackgroundProvider, and swaps it in
4. **RandomBackgroundProvider.cs** - Selects random images and manages material lifecycle (creates and destroys them to prevent memory leaks)
5. **ModSettings.cs** - Persists configuration to XML file
6. **DebugHelper.cs** - Optional logging utility

---

## Known Issues

**Red Fallback Screen Bug (v1.1.0):**
A rare bug has been identified where the red fallback screen can occasionally appear when switching between game modes (e.g., returning from editors to main menu). This occurs because Unity sometimes destroys or nullifies background textures during mode transitions, and the mod randomly selects one of these null entries. The issue is cosmetic only and doesn't affect saves or gameplay. A fix is in development and will be released as v1.1.1 shortly.

**DLC Background Support:**
Technically this "should" load background images from all your DLC -- I can't test because I don't actually have any DLC myself! I've been waiting for someone on Steam Workshop to say "THIS DOESN'T WORK" but they haven't yet. If you have DLC, see that it works, and are willing to share your experience, please do so I can update this!

**Reporting Issues:**
If you encounter problems:
1. Enable debug logging in mod settings
2. Find `output_log.txt` in your game folder (\SteamLibrary\steamapps\common\Cities_Skylines\Cities_Data\output_log.txt)
3. Report issues on GitHub with your log file

---

## Contributing and Support

- **Bug reports:** [GitHub Issues](https://github.com/Jeaudoir/Cityscape-Shuffle/issues) (response times may be slow)
- **Feedback:** [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3571276458)
- **Creator:** [Bilbo Fraggins](https://steamcommunity.com/id/xd00d/myworkshopfiles/?appid=255710)

Pull requests accepted case-by-case - open an issue first to discuss.

**Maintenance:** Gaming is an occasional hobby. This mod does what I need. If you want to extend it, fork away!

---

## AI Assistance Disclosure

Developed with assistance from Claude AI. **This mod was built from scratch** by analyzing the game's decompiled code, not by modifying existing mods. Code was iteratively designed, reviewed, tested, and debugged across multiple sessions. All design decisions and testing were human-driven.

Audit the code yourself - it's all here! The mod is simple and well-commented.

---

## License

MIT License - see [LICENSE](LICENSE)

Free to use, modify, and distribute.

---

**Enjoy varied loading screens!**
