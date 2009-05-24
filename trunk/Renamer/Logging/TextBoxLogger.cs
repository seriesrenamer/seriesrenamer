using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace Renamer.Logging
{
    public class TextBoxLogger : ILogger
    {
        private TextBox m_textBox;
        private LogLevel filter;
        public TextBoxLogger(TextBox txtBox, LogLevel filter) {
            this.filter = filter;
            m_textBox = txtBox;
        }

        public void LogMessage(string strLogMessage, LogLevel level) {
            if (level.CompareTo(filter) < 0) {
                return;
            }
            MethodInvoker logDelegate = delegate { m_textBox.AppendText(level.ToString() + ": " + strLogMessage + System.Environment.NewLine); };
            if (m_textBox.InvokeRequired)
                m_textBox.Invoke(logDelegate);
            else
                logDelegate();
        }
    }
}
