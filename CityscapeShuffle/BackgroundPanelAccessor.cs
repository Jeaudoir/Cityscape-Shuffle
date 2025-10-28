using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace CityscapeShuffle
{
    /// <summary>
    /// Captures background textures from the game's BackgroundPanel class using reflection.
    /// Intercepts BackgroundPanel.Awake() to steal access to the private m_Images field,
    /// which contains the textures used in the main menu slideshow.
    /// Other classes call GetRandomBackgroundTexture() to retrieve a random texture for loading screens.
    /// </summary>
    [HarmonyPatch(typeof(BackgroundPanel))]
    public static class BackgroundPanelAccessor
    {
        // Stores the list of background textures from the game's BackgroundPanel class (private m_Images field).
        private static List<Texture2D> availableBackgrounds = null;
        
        // Maximum number of texture names to log during initialization (reduces log spam)
        private const int MAX_SAMPLE_LOG_COUNT = 10;

        // Harmony postfix patch: runs after BackgroundPanel.Awake() completes.
        // Captures the game's background texture list for our mod to use.
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake_Postfix(BackgroundPanel __instance)
        {
            DebugHelper.Log("[CityscapeShuffle] BackgroundPanelAccessor.Awake_Postfix() called");
            
            try
            {
                // Use reflection to access BackgroundPanel's private m_Images field.
                // Reflection allows us to read private data that the game doesn't normally expose.
                // m_Images contains the background textures used in the main menu slideshow.
                FieldInfo imageListField = typeof(BackgroundPanel).GetField("m_Images", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (imageListField != null)
                {
                    List<Texture2D> rawList = (List<Texture2D>)imageListField.GetValue(__instance);
                    
                    if (rawList != null)
                    {
                        // Filter out null textures to prevent red fallback screen
                        availableBackgrounds = rawList.Where(tex => tex != null).ToList();
                        
                        if (availableBackgrounds.Count > 0)
                        {
                            
                            DebugHelper.Log($"[CityscapeShuffle] Successfully accessed {availableBackgrounds.Count} background images!");
                            
                            // Log the first few texture names as samples (to avoid cluttering the log file)
                            for (int i = 0; i < Mathf.Min(MAX_SAMPLE_LOG_COUNT, availableBackgrounds.Count); i++)
                            {
                                if (availableBackgrounds[i] != null)
                                {
                                    DebugHelper.Log($"[CityscapeShuffle] Sample image {i}: {availableBackgrounds[i].name} ({availableBackgrounds[i].width}x{availableBackgrounds[i].height})");
                                }
                            }
                            DebugHelper.Log($"[CityscapeShuffle] Background images now available for loading screen replacement!");
                        }
                        else
                        {
                            DebugHelper.LogWarning("[CityscapeShuffle] All textures were null after filtering!");
                        }
                    }
                    else
                    {
                        DebugHelper.LogWarning("[CityscapeShuffle] m_Images list is null");
                    }
                }
                else
                {
                    DebugHelper.LogError("[CityscapeShuffle] Could not find m_Images field via reflection");
                }
            }
            catch (System.Exception ex)
            {
                DebugHelper.LogError($"[CityscapeShuffle] Exception in BackgroundPanelAccessor: {ex}");
            }
        }

        // Returns a randomly selected background texture from the available collection.
        // Returns null if no backgrounds have been collected yet (main menu hasn't loaded).
        public static Texture2D GetRandomBackgroundTexture()
        {
            if (availableBackgrounds != null && availableBackgrounds.Count > 0)
            {
                int randomImageIndex = UnityEngine.Random.Range(0, availableBackgrounds.Count);
                Texture2D selectedTexture = availableBackgrounds[randomImageIndex];
                
                DebugHelper.Log($"[CityscapeShuffle] Selected random background: {selectedTexture?.name} (index {randomImageIndex} of {availableBackgrounds.Count})");
                
                return selectedTexture;
            }
            
            DebugHelper.LogWarning("[CityscapeShuffle] No background images available for selection");
            return null;
        }

        // Returns the number of available background textures (0 if not yet collected).
        public static int GetBackgroundCount()
        {
            return availableBackgrounds?.Count ?? 0;
        }

        /// <summary>
        /// Returns the list of available background textures captured from the game.
        /// Used by RandomBackgroundProvider to select backgrounds.
        /// </summary>
        public static List<Texture2D> GetAvailableBackgrounds()
        {
            return availableBackgrounds;
        }
    }
}
