﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace Renamer.Logging
{
    public class RichTextBoxLogger : ILogger
    {
        private RichTextBox richTextBox;
        protected string logColorString;

        private List<Color> logColors;
        protected Hashtable logColorIndex;
        protected Hashtable logColor;
        private LogLevel filter;

        string rtf;
        string rtfInserPosString;

        public RichTextBoxLogger(RichTextBox txtBox, LogLevel filter) {
            this.filter = filter;
            richTextBox = txtBox;
            logColorIndex = new Hashtable();
            logColor = new Hashtable();
            logColors = new List<Color>();
            rtf = txtBox.Rtf;
            

            // get searchstring for position, to remove the starting empty line
            int tmp = rtf.IndexOf("\\par\r\n");
            int tmplen = rtf.LastIndexOf("}", tmp);
            rtfInserPosString = rtf.Substring(tmplen, tmp - tmplen);
            this.initColors();
        }

        private void initColors() {

            logColor.Add(LogLevel.VERBOSE, Color.Gray);
            logColor.Add(LogLevel.DEBUG, Color.Gray);
            logColor.Add(LogLevel.LOG, Color.Black);
            logColor.Add(LogLevel.INFO, Color.DarkGreen);
            logColor.Add(LogLevel.WARNING, Color.Orange);
            logColor.Add(LogLevel.ERROR, Color.Red);
            logColor.Add(LogLevel.CRITICAL, Color.DarkRed);
        }

        private void generateColorString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("colortbl;");
            Color col;
            for (int index = 0; index < logColors.Count; index++) {
                col = logColors[index];
                sb.AppendFormat("\\red{0}", col.R);
                sb.AppendFormat("\\green{0}", col.G);
                sb.AppendFormat("\\blue{0}", col.B);
                sb.Append(";");
            }
            sb.Append("}");
            this.logColorString = sb.ToString();
        }

        public void LogMessage(string strLogMessage, LogLevel level) {
            if (level.CompareTo(filter) < 0) {
                return;
            }
            //Add Color to color table if not avaiable
            
            int colorindex = logColors.IndexOf((Color)logColor[level]);
            if (colorindex == -1) {
                logColors.Add((Color)logColor[level]);
                colorindex = logColors.IndexOf((Color)logColor[level]);
                this.generateColorString();
                addColorTableToRtb();
            }
            string rtMessage = "\\cf" + (colorindex + 1) + "\\b " + level.ToString() + ": \\b0\\cf0 " + strLogMessage + "\\par";
            int insertPos = rtf.IndexOf(rtfInserPosString) + rtfInserPosString.Length;

            rtf = rtf.Insert(insertPos, rtMessage);
            MethodInvoker logDelegate = delegate {
                richTextBox.Rtf = rtf;
            };
            if (richTextBox.InvokeRequired) {
                richTextBox.Invoke(logDelegate);
            }
            else {
                logDelegate();
            }
        }

        private string updateColors(string rtf) {
            for (int index = logColors.Count - 1; index >= 0; index--) {
                rtf = rtf.Replace("\\cf" + (index + 1), "\\cf" + (index + 2));
            }
            return rtf;
        }

        private void addColorTableToRtb() {
            int iInsertLoc = rtf.IndexOf("colortbl;");

            if (iInsertLoc != -1) //then colortbl exists
            {
                //find end of colortbl tab by searching
                //forward from the colortbl tab itself
                int iCTableEnd = rtf.IndexOf('}', iInsertLoc) + 1;

                //remove the existing colour table
                rtf = rtf.Remove(iInsertLoc, iCTableEnd - iInsertLoc);
            }

            //colour table doesn't exist yet, so let's make one
            else 
            {
                // find index of start of header
                int iRTFLoc = rtf.IndexOf("\\rtf");
                // get index of where we'll insert the colour table
                // try finding opening bracket of first property of header first 
                iInsertLoc = rtf.IndexOf('}', iRTFLoc) + 1;

                rtf = rtf.Insert(iInsertLoc, "{\\");
                iInsertLoc += 2;
            }
            rtf = rtf.Insert(iInsertLoc, this.logColorString);
        }
    }
}
