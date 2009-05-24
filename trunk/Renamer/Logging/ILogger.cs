using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Logging
{
    public enum LogLevel : int { VERBOSE = 0, DEBUG, LOG, INFO, WARNING, ERROR, CRITICAL, NONE};

    public interface ILogger
    {
        void LogMessage(string strMessage, LogLevel level);
    }
}
