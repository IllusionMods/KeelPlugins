namespace UnityEngine.Events
{
    internal static class UnityEventExtensions
    {
        public static void ActuallyRemoveAllListeners(this UnityEventBase evt)
        {
            evt.RemoveAllListeners();
            for(var i = 0; i < evt.GetPersistentEventCount(); i++)
                evt.SetPersistentListenerState(i, UnityEventCallState.Off);
        }
    }
}
