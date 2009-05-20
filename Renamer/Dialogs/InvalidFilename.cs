using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using Renamer.Classes.Configuration.Keywords;

namespace Renamer.Dialogs
{
    /// <summary>
    /// Invalid Filename dialog
    /// </summary>
    public partial class InvalidFilename : Form
    {
        /// <summary>
        /// Filename this wonderful dialog is used for
        /// </summary>
        public string FileName = "";

        /// <summary>
        /// Replace invalid characters with this string
        /// </summary>
        public string Replace = "";

        /// <summary>
        /// Remember this choice for the future
        /// </summary>
        public bool remember = false;

        /// <summary>
        /// Possible actions
        /// </summary>
        public enum Action { None, Skip, SkipAll, Filename, Replace};

        /// <summary>
        /// Selected action
        /// </summary>
        public Action action=Action.None;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename">filename</param>
        public InvalidFilename(string filename)
        {
            InitializeComponent();
            FileName = filename;
        }

        private void TextDialog_Load(object sender, EventArgs e)
        {

            lblError.Text += "\" < > | : * ? \\ /";
            txtFilename.Text = FileName;
            txtReplace.Text = Helper.ReadProperty(Config.InvalidCharReplace);
        }

        private void rbFilename_Click(object sender, EventArgs e)
        {
            if (rbFilename.Checked)
            {
                rbReplace.Checked = false;
            }
            else
            {
                rbReplace.Checked = true;
            }
        }

        private void rbReplace_Click(object sender, EventArgs e)
        {
            if (rbReplace.Checked)
            {
                rbFilename.Checked = false;
            }
            else
            {
                rbFilename.Checked = true;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbFilename.Checked)
            {
                action = Action.Filename;
                FileName = txtFilename.Text;
            }
            else if (rbReplace.Checked)
            {
                if (chkReplace.Checked)
                {
                    remember = true;
                }                
                action = Action.Replace;
                Replace = txtReplace.Text;
            }
            Close();
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            action = Action.Skip;
            Close();
        }

        private void btnSkipAll_Click(object sender, EventArgs e)
        {
            action = Action.SkipAll;
            if (chkReplace.Checked)
            {
                remember = true;
            } 
            Close();
        }

        private void txtReplace_Click(object sender, EventArgs e)
        {
            rbReplace.Checked = true;
        }

        private void txtFilename_Click(object sender, EventArgs e)
        {
            rbFilename.Checked = true;
        }       

        private void TextDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (action == Action.None)
            {
                action = Action.Skip;
            }
        }
    }
}
