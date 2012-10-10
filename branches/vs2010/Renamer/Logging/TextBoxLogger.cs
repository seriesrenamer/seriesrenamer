using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace Renamer.Logging
{
    /// <summary>
    /// Logger offers the posibility to Log to a textbox in a form
    /// </summary>
    public class TextBoxLogger : AbstractLogger
    {
        private TextBox textBox;
        /// <summary>
        /// Creates a new TextBoxLogger
        /// </summary>
        /// <param name="txtBox">Textbox the logging message should be appended</param>
        /// <param name="filter">Log level the logger should listen to</param>
        public TextBoxLogger(TextBox txtBox, LogLevel filter) : base(filter){
            textBox = txtBox;
        }

        public override void LogMessage(string strLogMessage, LogLevel level) {
            if (checkFilter(level)) {
                return;
            }
            MethodInvoker logDelegate = delegate { textBox.Text = level.ToString() + ": " + strLogMessage + System.Environment.NewLine + textBox.Text; };
            if (textBox.InvokeRequired)
                textBox.Invoke(logDelegate);
            else
                logDelegate();
        }
    }
}
