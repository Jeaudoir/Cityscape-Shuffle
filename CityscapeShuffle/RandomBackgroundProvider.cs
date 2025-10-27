using UnityEngine;
using System.Collections.Generic;

namespace CityscapeShuffle
{
	/// <summary>
	/// Provides random loading screen background materials to the LoadingPatches class.
	/// Creates new materials with random textures from the game's background collection,
	/// and manages their lifecycle to prevent memory leaks.
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

		// Returns a material with a random background texture applied.
		// Falls back to "emergency" solid-color material if no backgrounds available yet.
        public static Material GetRandomBackgroundMaterial(Material originalMaterial)
        {
            DebugHelper.Log("[CityscapeShuffle] RandomBackgroundProvider.GetRandomBackgroundMaterial() called");

			// Try to get a random background texture from BackgroundPanelAccessor.
			// Returns null if user hasn't restarted Cities: Skylines (backgrounds not properly captured from main menu).
            Texture2D randomBackground = BackgroundPanelAccessor.GetRandomBackgroundTexture();
            
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

		// Destroys all materials created during the current loading session.
		// Called when loading screen closes to prevent memory leaks.
        public static void Cleanup()
        {
            DebugHelper.Log("[CityscapeShuffle] RandomBackgroundProvider.Cleanup() called");
            
            // Destroy all materials created by this mod during the current loading session
            foreach (Material createdMaterial in loadingScreenImageMaterials)
            {
                if (createdMaterial != null)
                {
                    if (createdMaterial.mainTexture != null)
                    {
                        Object.Destroy(createdMaterial.mainTexture);
                    }
                    Object.Destroy(createdMaterial);
                }
            }
            loadingScreenImageMaterials.Clear();
            
            // Clean up fallback material if it exists
            if (emergencyColorMaterial != null)
            {
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