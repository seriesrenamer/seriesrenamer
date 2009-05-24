using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Logging
{
    /// <summary>
    /// AbstractLogger offers the possibility to compate the level of a message to a filter.
    /// </summary>
    public abstract class AbstractLogger : ILogger
    {

        protected LogLevel filter;

        #region ILogger Member
        protected bool checkFilter(LogLevel level) {
            return (level.CompareTo(filter) < 0);
        }

        protected AbstractLogger(LogLevel filter) {
            this.filter = filter;
        }

        public abstract void LogMessage(string strMessage, LogLevel level);

        public LogLevel Filter {
            get { return filter; }
        }

        #endregion
    }
}
