using ICities;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CityscapeShuffle
{
    /// <summary>
    /// Main entry point for the Cityscape Shuffle mod.
    /// Installs Harmony patches to intercept LoadingAnimation.SetImage() and swap loading screen backgrounds
    /// with random images from the game's collection (collected later by BackgroundPanelAccessor at main menu).
    /// </summary>
    public class Mod : IUserMod
    {
        public string Name => 
            GetAssemblyAttribute<AssemblyTitleAttribute>()?.Title 
            ?? "Cityscape Shuffle (Randomized Loading Screen Backgrounds)";
        
        public string Description => 
            GetAssemblyAttribute<AssemblyDescriptionAttribute>()?.Description 
            ?? "Banish repetitive loading backgrounds with random ones from the existing image collection. Auto-includes all DLC content. No more crimes against your eyeballs!";

        private static T GetAssemblyAttribute<T>() where T : Attribute
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0 ? (T)attributes[0] : null;
        }

        private const string HarmonyId = "CityscapeShuffle";
        private static Harmony harmony;
        
        // Settings persistence
        private const string SETTINGS_FILENAME = "CityscapeShuffle.xml";
        private static string configPath;
        private static ModSettings modSettings;

        public void OnEnabled()
        {
            DebugHelper.Log("[CityscapeShuffle] Mod.OnEnabled() called");
            
            // Load settings first so debug logging is available
            InitConfigFile();
            
            try
            {
                harmony = new Harmony(HarmonyId);
                harmony.PatchAll();
                DebugHelper.Log("[CityscapeShuffle] Harmony patches applied successfully");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError($"[CityscapeShuffle] Failed to apply Harmony patches: {ex}");
            }
        }

        public void OnDisabled()
        {
            DebugHelper.Log("[CityscapeShuffle] Mod.OnDisabled() called");
            
            try
            {
                if (harmony != null)
                {
                    harmony.UnpatchAll(HarmonyId);
                    harmony = null;
                    DebugHelper.Log("[CityscapeShuffle] Harmony patches removed successfully");
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError($"[CityscapeShuffle] Failed to remove Harmony patches: {ex}");
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            InitConfigFile();
            var settingsGroup = helper.AddGroup("Cityscape Shuffle Settings");
            settingsGroup.AddCheckbox("Enable debug logging to output_log.txt", modSettings.EnableDebugLogging, 
                (bool value) => 
                { 
                    modSettings.EnableDebugLogging = value;
                    DebugHelper.EnableDebugLogging = value;
                    ModSettings.Serialize(configPath, modSettings);
                });
        }
        
        private void InitConfigFile()
        {
            try
            {
                string pathName = ColossalFramework.GameSettings.FindSettingsFileByName("gameSettings").pathName;
                string directory = "";
                if (pathName != "")
                {
                    directory = Path.GetDirectoryName(pathName) + Path.DirectorySeparatorChar.ToString();
                }
                configPath = directory + SETTINGS_FILENAME;
                modSettings = ModSettings.Deserialize(configPath);
                
                // Try old location if not found in settings directory
                if (modSettings == null)
                {
                    modSettings = ModSettings.Deserialize(SETTINGS_FILENAME);
                    if (modSettings != null && ModSettings.Serialize(directory + SETTINGS_FILENAME, modSettings))
                    {
                        try
                        {
                            File.Delete(SETTINGS_FILENAME);
                        }
                        catch { }
                    }
                }
                
                // Create new settings file if none exists
                if (modSettings == null)
                {
                    modSettings = new ModSettings();
                    if (!ModSettings.Serialize(configPath, modSettings))
                    {
                        configPath = SETTINGS_FILENAME;
                        ModSettings.Serialize(configPath, modSettings);
                    }
                }
                
                // Apply loaded setting to DebugHelper
                DebugHelper.EnableDebugLogging = modSettings.EnableDebugLogging;
            }
            catch { }
        }
    }
}
