using System;
using System.Diagnostics;
using BepInEx.Logging;

internal static class Log
{
    private static ManualLogSource _logSource;

    public static void SetLogSource(ManualLogSource logSource)
    {
        _logSource = logSource;
    }

    public static void Info(object data) => CheckAndRunLogger(() => _logSource.LogInfo(data));
    public static void Debug(object data) => CheckAndRunLogger(() => _logSource.LogDebug(data));
    public static void Error(object data) => CheckAndRunLogger(() => _logSource.LogError(data));
    public static void Fatal(object data) => CheckAndRunLogger(() => _logSource.LogFatal(data));
    public static void Message(object data) => CheckAndRunLogger(() => _logSource.LogMessage(data));
    public static void Warning(object data) => CheckAndRunLogger(() => _logSource.LogWarning(data));
    public static void Level(BepInEx.Logging.LogLevel level, object data) => CheckAndRunLogger(() => _logSource.Log(level, data));

    private static void CheckAndRunLogger(Action action)
    {
        if(_logSource == null)
        {
            Console.WriteLine($"Use {nameof(SetLogSource)} before logging. {new StackTrace().ToString()}");
            return;
        }

        action();
    }
}
