using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Renamer.Logging
{
    /// <summary>
    /// Logger which can log to a file, specified by a filename;
    /// </summary>
    public class FileLogger : ILogger
    {
        private string fileName;
        private object m_sync = new object();
        private LogLevel filter;
        /// <summary>
        /// Creates a new Filelogger
        /// </summary>
        /// <param name="strFileName">Path to the logfile</param>
        /// <param name="clearFile">If true, a old logfile will be deleted, else the log will be continued at the end of the old file</param>
        /// <param name="filter">Log level the logger should listen to</param>
        public FileLogger(string strFileName, bool clearFile, LogLevel filter) {
            this.filter = filter;
            fileName = strFileName;
            if (File.Exists(fileName)) {
                File.Delete(fileName);
            }
        }

        public void LogMessage(string strMessage, LogLevel level) {
            if (level.CompareTo(filter) < 0) {
                return;
            }
            lock (m_sync) {
                File.AppendAllText(fileName, level.ToString() + ": " + strMessage + System.Environment.NewLine);
            }
        }
    }
}
