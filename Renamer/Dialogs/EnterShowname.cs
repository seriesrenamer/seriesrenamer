using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Renamer.Classes.Configuration.Keywords;

namespace Renamer.Dialogs
{
    /// <summary>
    /// Dialog to set a showname
    /// </summary>
    public partial class EnterShowname : Form
    {
        /// <summary>
        /// Selected showname
        /// </summary>
        public string SelectedName = "";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="files">List of files to select</param>
        public EnterShowname(string Showname)
        {
            InitializeComponent();
            cbFiles.Items.AddRange(Helper.ReadProperties(Config.LastTitles));
            cbFiles.SelectedIndex = 0;
            for (int i = 0; i < cbFiles.Items.Count; i++)
            {
                string str = (string)cbFiles.Items[i];
                if (str.ToLower() == Showname.ToLower())
                {
                    cbFiles.SelectedIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// Set selected index and DialogResult.OK and close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedName = cbFiles.Text;
            DialogResult = DialogResult.OK;
            List<string> LastTitles = new List<string>(Helper.ReadProperties(Config.LastTitles));
            for (int i = 0; i < LastTitles.Count; i++)
            {
                string str = (string)LastTitles[i];
                if (str.ToLower() == cbFiles.Text.ToLower())
                {
                    LastTitles.Remove(str);
                    LastTitles.Insert(0, SelectedName);
                    Helper.WriteProperties(Config.LastTitles, LastTitles.ToArray());
                    break;
                }
            }
            Close();
        }

        /// <summary>
        /// Set DialogResult.Cancel and close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cbFiles_TextChanged(object sender, EventArgs e)
        {
            
            //when nothing is selected, assume there is no autocompletion taking place right now
            if (cbFiles.SelectedText == "" && cbFiles.Text != "")
            {
                for (int i = 0; i < cbFiles.Items.Count; i++)
                {
                    if (((string)cbFiles.Items[i]).ToLower() == cbFiles.Text.ToLower())
                    {
                        bool found = false;
                        foreach (object o in cbFiles.Items)
                        {
                            if ((string)o == cbFiles.Text)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            cbFiles.Items.Add(cbFiles.Text);
                        }
                        break;
                    }
                }
            }
        }
        bool UsedDropDown = false;
        private void cbFiles_DropDownClosed(object sender, EventArgs e)
        {
            UsedDropDown = true;
        }

        private void cbFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UsedDropDown)
            {
                UsedDropDown = !UsedDropDown;
                btnOK.PerformClick();
            }
        }
    }
}
