using Log;
using System.Reflection;

namespace Logging;

public static class MethodTimeLogger
{
    public static void Log(MethodBase methodBase, long milliseconds, string message)
    {
        Task.Run(() => Logger.SendTimerData(methodBase.Name, milliseconds, message));
    }
}
