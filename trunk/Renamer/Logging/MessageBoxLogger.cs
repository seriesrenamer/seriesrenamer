using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Renamer.Logging
{
    class MessageBoxLogger : ILogger
    {
        protected LogLevel filter;
        /// <summary>
        /// Creates a new MessageBoxLogger
        /// </summary>
        /// <param name="filter">Log level the logger should listen to</param>
        public MessageBoxLogger(LogLevel filter) {
            this.filter = filter;
        }

        public void LogMessage(string strLogMessage, LogLevel level) {
            if (level.CompareTo(filter) < 0) {
                return;
            }
            MessageBoxIcon icon;
            switch(level){
                case LogLevel.DEBUG:
                case LogLevel.VERBOSE:
                    icon = MessageBoxIcon.Exclamation;
                    break;
                case LogLevel.INFO:
                case LogLevel.LOG:
                    icon = MessageBoxIcon.Information;
                    break;
                case LogLevel.WARNING:
                    icon = MessageBoxIcon.Warning;
                    break;
                case LogLevel.ERROR:
                    icon = MessageBoxIcon.Error;
                    break;
                case LogLevel.CRITICAL:
                    icon = MessageBoxIcon.Stop;
                    break;
                default:
                    icon = MessageBoxIcon.None;
                    break;
            }
            MessageBox.Show(strLogMessage, level.ToString(), MessageBoxButtons.OK, icon);
        }
    }
}
