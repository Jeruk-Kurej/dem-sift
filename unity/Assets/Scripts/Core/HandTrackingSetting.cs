using System;
using UnityEngine;

namespace DEMSIFT.Core
{
    public static class HandTrackingSetting
    {
        public static bool IsEnabled { get; private set; } = true;

        public static event Action<bool> Changed;

        public static void Toggle()
        {
            IsEnabled = !IsEnabled;
            Changed?.Invoke(IsEnabled);
        }

        // --- Play mode without domain reload ---
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Clear()
        {
            IsEnabled = true;
            Changed = null;
        }
    }
}
