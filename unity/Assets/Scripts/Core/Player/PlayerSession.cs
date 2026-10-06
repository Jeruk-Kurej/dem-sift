using UnityEngine;

namespace DEMSIFT.Player
{
    public static class PlayerSession
    {
        public static string PlayerName { get; private set; } = string.Empty;
        public static string ClassName { get; private set; } = string.Empty;
        public static bool HasPlayer => PlayerName.Length > 0;

        public static void Begin(string playerName, string className)
        {
            PlayerName = playerName.Trim();
            ClassName = className;
        }

        // --- Play mode without domain reload ---
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Clear()
        {
            PlayerName = string.Empty;
            ClassName = string.Empty;
        }
    }
}
