using UnityEngine;

namespace CityscapeShuffle
{
    /// <summary>
    /// Debug logging utility that respects the EnableDebugLogging flag.
    /// </summary>
    public static class DebugHelper
    {
        public static bool EnableDebugLogging = false;

        public static void Log(string message)
        {
            if (EnableDebugLogging)
                Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            if (EnableDebugLogging)
                Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            // Always log errors regardless of debug setting
            Debug.LogError(message);
        }
    }
}