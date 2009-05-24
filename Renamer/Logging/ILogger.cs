using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Logging
{
    /// <summary>
    /// Indicator for the level of a log message
    /// </summary>
    public enum LogLevel : int { 
        /// <summary>
        /// Highest available level, shich enables very deep logging, my cause system to run slower
        /// </summary>
        VERBOSE = 0,
        /// <summary>
        /// Standard for messages that only relevant for debugging issues
        /// </summary>
        DEBUG,
        /// <summary>
        /// A level for things you want to log, but user might not be interessted in
        /// </summary>
        LOG,
        /// <summary>
        /// The first log level the user may read
        /// </summary>
        INFO,
        /// <summary>
        /// The log level the user should read
        /// </summary>
        WARNING,
        /// <summary>
        /// The log level the user must read
        /// </summary>
        ERROR,
        /// <summary>
        /// The log level, if program stops working after logging this message
        /// </summary>
        CRITICAL,
        NONE};

    public interface ILogger
    {
        /// <summary>
        /// Logs a message with the given level
        /// </summary>
        /// <param name="strMessage">Log message, should be saved</param>
        /// <param name="level">Loglevel of the message</param>
        void LogMessage(string strMessage, LogLevel level);
    }
}
