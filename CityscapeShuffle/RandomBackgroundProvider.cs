using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CityscapeShuffle
{
    /// <summary>
    /// Provides random loading screen background materials to the LoadingPatches class.
    /// Creates new materials with textures from the game's background collection and manages their lifecycle to prevent memory leaks.
    /// Uses shuffle-without-replacement algorithm to ensure all backgrounds are shown before repeating.
    /// Yes, I went a little overboard with the comments in this file. Sorry. Better grab a coffee.
    /// </summary>
    public static class RandomBackgroundProvider
    {
        // Tracks all materials created by this mod for loading screens.
        // Each material wraps one of the game's background textures and must be destroyed to prevent memory leaks.
        private static List<Material> loadingScreenImageMaterials = new List<Material>();
       
        // Fallback "emergency" material used when no backgrounds are available yet (before main menu loads).
        // Intentionally ugly color signals to user that Cities: Skylines needs restarting
        // for the mod to access the game's background collection.
        private static Material emergencyColorMaterial = null;
        
        // Constants for fallback/emergency texture
        private const int FALLBACK_TEXTURE_SIZE = 1;
        private static readonly Color FALLBACK_COLOR = Color.red;
        
        // Shuffle-without-replacement tracking
        private static List<int> shuffledIndices = new List<int>();
        private static int currentIndexPosition = 0;
        private static int lastKnownBackgroundCount = 0;

        // Returns a material with a random background texture applied.
        // Falls back to "emergency" solid-color material if no backgrounds available yet.
        public static Material GetRandomBackgroundMaterial(Material originalMaterial)
        {
            DebugHelper.Log("[CityscapeShuffle] RandomBackgroundProvider.GetRandomBackgroundMaterial() called");

            // Try to get a random background texture from BackgroundPanelAccessor.
            // Returns null if user hasn't restarted Cities: Skylines (backgrounds not properly captured from main menu).
            Texture2D randomBackground = GetNextShuffledBackground();
            
            if (randomBackground != null)
            {
                DebugHelper.Log("[CityscapeShuffle] Creating material with random background texture");
                
                // Create new material using the game's original material as a template, then apply our random texture
                Material newLoadingScreenImageMaterial = new Material(originalMaterial);
                newLoadingScreenImageMaterial.mainTexture = randomBackground;
                
                // Track this material so we can clean it up later
                loadingScreenImageMaterials.Add(newLoadingScreenImageMaterial);
                
                DebugHelper.Log("[CityscapeShuffle] Background material created successfully");
                return newLoadingScreenImageMaterial;
            }
            else
            {
                DebugHelper.LogWarning("[CityscapeShuffle] No background texture available, falling back to ugly/obvious 'something is wrong' color");
                
                // Fallback to a solid "emergency" block of color if no backgrounds are available yet
                if (emergencyColorMaterial == null)
                {
                    DebugHelper.Log("[CityscapeShuffle] Creating fallback red material");
                    
                    Texture2D redTexture = new Texture2D(FALLBACK_TEXTURE_SIZE, FALLBACK_TEXTURE_SIZE);
                    redTexture.SetPixel(0, 0, FALLBACK_COLOR);
                    redTexture.Apply();
                    
                    emergencyColorMaterial = new Material(originalMaterial);
                    emergencyColorMaterial.mainTexture = redTexture;
                }
                
                return emergencyColorMaterial;
            }
        }

        // Gets the next background texture using shuffle-without-replacement algorithm.
        // Ensures all backgrounds are shown once before any repeats.
        private static Texture2D GetNextShuffledBackground()
        {
            List<Texture2D> availableBackgrounds = BackgroundPanelAccessor.GetAvailableBackgrounds();
            
            // DEFENSIVE: Check for empty background list
            // Prevents crash if called before BackgroundPanelAccessor has captured images
            if (availableBackgrounds == null || availableBackgrounds.Count == 0)
            {
                DebugHelper.LogWarning("[CityscapeShuffle] No backgrounds available yet - returning null");
                return null;
            }
            
            // DEFENSIVE: Detect background count changes
            // If user enables/disables DLC or game changes background list, reshuffle
            if (availableBackgrounds.Count != lastKnownBackgroundCount)
            {
                DebugHelper.Log($"[CityscapeShuffle] Background count changed from {lastKnownBackgroundCount} to {availableBackgrounds.Count} - triggering reshuffle");
                lastKnownBackgroundCount = availableBackgrounds.Count;
                ShuffleBackgroundOrder(availableBackgrounds.Count);
            }
            
            // DEFENSIVE: Check if shuffle needed (bounds checking)
            // Create shuffle once at start of session, then reuse it
            if (shuffledIndices.Count == 0)
            {
                DebugHelper.Log("[CityscapeShuffle] First shuffle - creating random order for this game session");
                ShuffleBackgroundOrder(availableBackgrounds.Count);
            }
            
            // Wrap back to start when we reach the end of the shuffle
            if (currentIndexPosition >= shuffledIndices.Count)
            {
                DebugHelper.Log($"[CityscapeShuffle] Completed full cycle through all {shuffledIndices.Count} backgrounds - restarting from position 0");
                currentIndexPosition = 0;
            }
            
            // Get next image from shuffled list
            int randomImageIndex = shuffledIndices[currentIndexPosition];
            currentIndexPosition++;
            
            // DEFENSIVE: Validate index is within bounds
            // Extra safety check in case of logic errors
            if (randomImageIndex < 0 || randomImageIndex >= availableBackgrounds.Count)
            {
                DebugHelper.LogError($"[CityscapeShuffle] ERROR: Shuffled index {randomImageIndex} out of bounds (0-{availableBackgrounds.Count-1}) - forcing reshuffle");
                ShuffleBackgroundOrder(availableBackgrounds.Count);
                randomImageIndex = shuffledIndices[0];
                currentIndexPosition = 1;
            }
            
            Texture2D selectedTexture = availableBackgrounds[randomImageIndex];
            
            DebugHelper.Log($"[CityscapeShuffle] Selected random background: {selectedTexture?.name ?? "null"} (shuffle position {currentIndexPosition-1}, actual index {randomImageIndex} of {availableBackgrounds.Count})");
            
            return selectedTexture;
        }

        // Shuffles the background display order using Fisher-Yates algorithm.
        // Creates a random permutation of indices to ensure all backgrounds shown before repeating.
        private static void ShuffleBackgroundOrder(int backgroundCount)
        {
            shuffledIndices.Clear();
            
            // DEFENSIVE: Validate background count
            // Don't shuffle if there are no backgrounds
            if (backgroundCount <= 0)
            {
                DebugHelper.LogWarning($"[CityscapeShuffle] Cannot shuffle with background count {backgroundCount}");
                return;
            }
            
            // Create list of indices: 0, 1, 2, 3, 4, 5...
            for (int i = 0; i < backgroundCount; i++)
            {
                shuffledIndices.Add(i);
            }
            
            // Fisher-Yates shuffle algorithm
            System.Random rng = new System.Random();
            int n = shuffledIndices.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int temp = shuffledIndices[k];
                shuffledIndices[k] = shuffledIndices[n];
                shuffledIndices[n] = temp;
            }
            
            currentIndexPosition = 0;
            
            // Build a readable list showing both index and name
            List<Texture2D> availableBackgrounds = BackgroundPanelAccessor.GetAvailableBackgrounds();
            string shuffleDetails = string.Join(", ", shuffledIndices.ConvertAll(i => 
                $"{i}:{(i < availableBackgrounds.Count ? availableBackgrounds[i].name : "?")}").ToArray());

            DebugHelper.Log($"[CityscapeShuffle] Shuffled {backgroundCount} backgrounds into new order: [{shuffleDetails}]");
        }

        // Resets the shuffle state when backgrounds are recaptured.
        // Called by BackgroundPanelAccessor when BackgroundPanel.Awake() runs.
        public static void ResetShuffle()
        {
            DebugHelper.Log($"[CityscapeShuffle] ResetShuffle() called - clearing shuffle state (was at position {currentIndexPosition} of {shuffledIndices.Count})");
            shuffledIndices.Clear();
            currentIndexPosition = 0;
            // Note: lastKnownBackgroundCount is NOT reset - we use it to detect count changes
        }

        // Destroys all materials created during the current loading session.
        // Called when loading screen closes to prevent memory leaks.
        public static void Cleanup()
        {
            DebugHelper.Log("[CityscapeShuffle] RandomBackgroundProvider.Cleanup() called");
            
            // Clean up materials for loading screens
            foreach (Material createdMaterial in loadingScreenImageMaterials)
            {
                if (createdMaterial != null)
                {
                    // Materials are created by this mod and must be destroyed.
                    // Textures come from BackgroundPanel and are managed by the game.
                    Object.Destroy(createdMaterial);
                }
            }
            loadingScreenImageMaterials.Clear();

            // Clean up emergency fallback material
            if (emergencyColorMaterial != null)
            {
                // Both texture and material were created by this mod, so destroy both.
                if (emergencyColorMaterial.mainTexture != null)
                {
                    Object.Destroy(emergencyColorMaterial.mainTexture);
                }
                Object.Destroy(emergencyColorMaterial);
                emergencyColorMaterial = null;
                DebugHelper.Log("[CityscapeShuffle] Fallback material cleaned up");
            }
            
            DebugHelper.Log("[CityscapeShuffle] All materials cleaned up successfully");
        }
    }
}
