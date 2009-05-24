using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Renamer.Logging
{
    public class FileLogger : ILogger
    {
        private string fileName;
        private object m_sync = new object();
        private LogLevel filter;
        public FileLogger(string strFileName, bool clearFile, LogLevel filter) {
            this.filter = filter;
            fileName = strFileName;
            File.Delete(fileName);
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
