using BepInEx.Logging;

internal static class Log
{
    private static ManualLogSource _logSource;

    public static void SetLogSource(ManualLogSource logSource)
    {
        _logSource = logSource;
    }

    public static void Info(object data) => _logSource.LogInfo(data);
    public static void Debug(object data) => _logSource.LogDebug(data);
    public static void Error(object data) => _logSource.LogError(data);
    public static void Fatal(object data) => _logSource.LogFatal(data);
    public static void Message(object data) => _logSource.LogMessage(data);
    public static void Warning(object data) => _logSource.LogWarning(data);
    public static void Level(BepInEx.Logging.LogLevel level, object data) => _logSource.Log(level, data);
}
