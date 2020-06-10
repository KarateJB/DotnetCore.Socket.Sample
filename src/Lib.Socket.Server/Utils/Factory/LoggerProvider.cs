using NLog.Web;

namespace Lib.Socket.Server.Utils.Factory
{
    /// <summary>
    /// Logger provider
    /// </summary>
    public class LoggerProvider
    {
        /// <summary>
        /// Logger
        /// </summary>
        public static NLog.Logger Logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
    }
}
