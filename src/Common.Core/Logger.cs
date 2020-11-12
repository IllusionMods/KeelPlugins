using BepInEx.Logging;

namespace KeelPlugins.Core
{
    public static class Logger
    {
        private static ManualLogSource _logSource;

        public static void RegisterLogSource(ManualLogSource logSource)
        {
            _logSource = logSource;
        }

        public static void LogInfo(object data) => _logSource.LogInfo(data);
        public static void LogDebug(object data) => _logSource.LogDebug(data);
        public static void LogError(object data) => _logSource.LogError(data);
        public static void LogFatal(object data) => _logSource.LogFatal(data);
        public static void LogMessage(object data) => _logSource.LogMessage(data);
        public static void LogWarning(object data) => _logSource.LogWarning(data);
    }
}
