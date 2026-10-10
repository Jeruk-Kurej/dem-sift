using System;
using UnityEngine;

namespace DEMSIFT.Core
{
    public static class AudioSetting
    {
        private const string EnabledKey = "audio.enabled";

        public static bool IsEnabled { get; private set; } = true;

        public static event Action<bool> Changed;

        public static void Toggle()
        {
            IsEnabled = !IsEnabled;
            PlayerPrefs.SetInt(EnabledKey, IsEnabled ? 1 : 0);
            Apply();
            Changed?.Invoke(IsEnabled);
        }

        // --- Play mode without domain reload ---
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Load()
        {
            IsEnabled = PlayerPrefs.GetInt(EnabledKey, 1) == 1;
            Changed = null;
            Apply();
        }

        private static void Apply()
        {
            AudioListener.volume = IsEnabled ? 1f : 0f;
        }
    }
}
