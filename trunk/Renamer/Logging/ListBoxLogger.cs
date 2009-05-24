using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Renamer.Logging
{
    public class ListBoxLogger : ILogger
    {
        ListBox m_listBox;
        private LogLevel filter;
        public ListBoxLogger(ListBox listBox, LogLevel filter) {
            this.filter = filter;
            m_listBox = listBox;
        }

        public void LogMessage(string strMessage, LogLevel level) {
            if (level.CompareTo(filter) < 0) {
                return;
            }
            MethodInvoker logDelegate = delegate {
                m_listBox.Items.Add(level.ToString() + ": " + strMessage);
            };

            if (m_listBox.InvokeRequired)
                m_listBox.Invoke(logDelegate);
            else
                logDelegate();
        }
    }
}
