using Contracts;
using NLog;

namespace LoggerService;

public class LoggerManager : ILoggerManager
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    public void LogDebug(string message) => Logger.Debug(message: message);

    public void LogError(string message) => Logger.Error(message: message);

    public void LogInfo(string message) => Logger.Info(message: message);

    public void LogWarn(string message) => Logger.Warn(message: message);
}