namespace DEMSIFT.Data
{
    public readonly struct PlayerResult
    {
        public readonly string PlayerName;
        public readonly string ClassName;
        public readonly int Total;

        public PlayerResult(string playerName, string className, int total)
        {
            PlayerName = playerName;
            ClassName = className;
            Total = total;
        }

        public string Key => KeyOf(PlayerName, ClassName);

        public static string KeyOf(string playerName, string className)
        {
            return $"{playerName.Trim().ToLowerInvariant()}|{className}";
        }
    }
}
