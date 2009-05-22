#region SVN Info
/***************************************************************
 * $Author$
 * $Revision$
 * $Date$
 * $LastChangedBy$
 * $LastChangedDate$
 * $URL$
 * 
 * License: GPLv3
 * 
****************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Renamer.Dialogs
{
    public partial class RegexTester : Form
    {
        public RegexTester()
        {
            InitializeComponent();
        }

        private void btnMatch_Click(object sender, EventArgs e)
        {
            string test = txtText.Text;
            string regex = txtRegex.Text;
            List<string> groups = new List<string>();
            MatchCollection mc = Regex.Matches(regex, "\\(\\?\\<(?<groups>.*?)\\>");
            foreach (Match m in mc)
            {
                groups.Add(m.Groups["groups"].Value);
            }
            string output="";
            RegexOptions ro = RegexOptions.None;
            if (chkCase.Checked) ro = RegexOptions.IgnoreCase;
            if (chkRtL.Checked) ro |= RegexOptions.RightToLeft;
            if (chkSingle.Checked) ro |= RegexOptions.Singleline;
            try
            {
                mc = Regex.Matches(test, regex, ro);
                while (chkShorter.Checked && regex != "" && !(mc.Count > 0))
                {
                    regex = regex.Substring(0, regex.Length - 1);
                    try
                    {
                        mc = Regex.Matches(test, regex, ro);
                    }
                    catch (Exception) { }
                }
                txtRegex.SelectionStart = 0;
                txtRegex.SelectionLength = regex.Length;
                output = "Regex= " + regex + Environment.NewLine;
                output += "Found " + mc.Count + "matches." + Environment.NewLine;
                for (int i = 0; i < mc.Count; i++)
                {
                    Match m = mc[i];
                    output += "Match " + i + ": " + m.Value + Environment.NewLine;
                    if (m.Groups.Count > 0)
                    {
                        output += "Groups:" + Environment.NewLine;
                        for (int j = 0; j < groups.Count; j++)
                        {
                            output += "Key = "+groups[j]+Environment.NewLine+m.Groups[groups[j]].Value + Environment.NewLine;
                        }
                    }
                }
                txtOutput.Text = output;
            }
            catch (Exception ex)
            {
                txtOutput.Text = ex.Message;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
