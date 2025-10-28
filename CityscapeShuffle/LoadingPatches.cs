using HarmonyLib;
using UnityEngine;

namespace CityscapeShuffle
{
    /// <summary>
    /// Harmony patches for LoadingAnimation to intercept and modify loading screen backgrounds.
    /// SetImage_Prefix() swaps the default loading material with a random background before display.
    /// OnDisable_Postfix() cleans up created materials after the loading screen closes to prevent memory leaks.
    /// </summary>
    [HarmonyPatch(typeof(LoadingAnimation))]
    public static class LoadingPatches
    {
        // Harmony prefix patch: intercepts LoadingAnimation.SetImage() before it runs.
        // Swaps the default loading screen material with a random background from our collection.
        // Returns true to allow the original method to continue with our modified material.
        [HarmonyPatch("SetImage")]
        [HarmonyPrefix]
        public static bool SetImage_Prefix(Mesh mesh, ref Material material, float scale, bool showAnimation)
        {
            DebugHelper.Log("[CityscapeShuffle] LoadingPatches.SetImage_Prefix() called");
            DebugHelper.Log($"[CityscapeShuffle] Original material: {(material != null ? material.name : "null")}");
            DebugHelper.Log($"[CityscapeShuffle] Scale: {scale}, ShowAnimation: {showAnimation}");

            // Only replace loading screen backgrounds, not intro logos:
            // We identify loading screens by checking if the material name contains specific keywords.
            string materialName = material?.name ?? "";
            bool isLoadingBackground = materialName.Contains("Loading Image") || materialName.Contains("Background");
            
            DebugHelper.Log($"[CityscapeShuffle] Material name: '{materialName}', IsLoadingBackground: {isLoadingBackground}");

            if (!isLoadingBackground)
            {
                DebugHelper.Log("[CityscapeShuffle] Skipping non-loading-screen material");
                return true; // Let intro logos and other materials pass through unchanged
            }

            try
            {
                // Get a random background material from RandomBackgroundProvider
                Material randomBackgroundMaterial = RandomBackgroundProvider.GetRandomBackgroundMaterial(material);
                
                if (randomBackgroundMaterial != null)
                {
                    DebugHelper.Log("[CityscapeShuffle] Replacing material with random background");
                    
                    // Replace the game's default material with our random background material
                    material = randomBackgroundMaterial;
                    
                    DebugHelper.Log("[CityscapeShuffle] Material replacement successful");
                }
                else
                {
                    DebugHelper.LogWarning("[CityscapeShuffle] Failed to create random background material, using original");
                }
            }
            catch (System.Exception ex)
            {
                DebugHelper.LogError($"[CityscapeShuffle] Exception in SetImage_Prefix: {ex}");
            }

            // Return true to let the game's SetImage method continue normally with our swapped material.
            // This ensures we don't interfere with the actual loading process.
            return true;
        }

        // Harmony postfix patch: runs after LoadingAnimation.OnDisable() completes.
        // Cleans up materials to prevent memory leaks when transitioning out of loading screens.
        [HarmonyPatch("OnDisable")]
        [HarmonyPostfix]
        public static void OnDisable_Postfix()
        {
            DebugHelper.Log("[CityscapeShuffle] LoadingPatches.OnDisable_Postfix() called");
            
            try
            {
                RandomBackgroundProvider.Cleanup();
            }
            catch (System.Exception ex)
            {
                DebugHelper.LogError($"[CityscapeShuffle] Exception in OnDisable_Postfix: {ex}");
            }
        }
    }
}
