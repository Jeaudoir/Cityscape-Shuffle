using UnityEngine;
using System;

namespace CityscapeShuffle
{
    /// <summary>
    /// Simple debug logging utility with a toggleable flag to control log output.
    /// </summary>
    public static class DebugHelper
    {
        public static bool EnableDebugLogging = false;

        public static void Log(string message)
        {
            if (EnableDebugLogging)
            {
                Debug.Log($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}");
            }
        }

        public static void LogWarning(string message)
        {
            if (EnableDebugLogging)
            {
                Debug.LogWarning($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}");
            }
        }

        public static void LogError(string message)
        {
            // Errors are always logged regardless of debug setting
            Debug.LogError($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}");
        }
    }
}
