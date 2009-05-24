using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Renamer.Logging
{
    /// <summary>
    /// Logger which can log to a ListBox
    /// </summary>
    public class ListBoxLogger : AbstractLogger
    {
        ListBox m_listBox;
        /// <summary>
        /// Creates a new ListBoxLogger
        /// </summary>
        /// <param name="listBox">ListBox, log entries should e added</param>
        /// <param name="filter">Log level the logger should listen to</param>
        public ListBoxLogger(ListBox listBox, LogLevel filter)
            : base(filter) {
            m_listBox = listBox;
        }

        public override void LogMessage(string strMessage, LogLevel level) {
            if (checkFilter(level)) {
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
