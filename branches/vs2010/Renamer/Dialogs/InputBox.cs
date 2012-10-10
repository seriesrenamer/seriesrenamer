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
using System.IO;

namespace Renamer.Dialogs
{
    /// <summary>
    /// Generic Text/Folder Input Box
    /// </summary>
    public partial class InputBox : Form
    {
        /// <summary>
        /// Browse behaviour
        /// </summary>
        private BrowseType browse;

        /// <summary>
        /// if empty strings or (if browse button is visible) invalid folders are accepted
        /// </summary>
        private bool Accept;

        /// <summary>
        /// user input
        /// </summary>
        public string input;

        /// <summary>
        /// Browse file behaviour
        /// </summary>
        public enum BrowseType : int { None, Folder, Files };

        /// <summary>
        /// InputBox Constructor
        /// </summary>
        /// <param name="title">caption title</param>
        /// <param name="description">Description text</param>
        /// <param name="InitialValue">Initial textbox value</param>
        /// <param name="Browse">Show browse button?</param>
        /// <param name="AcceptEmptyOrInexistant">if empty strings or (if browse button is visible) invalid folders are accepted</param>
        public InputBox(string title, string description, string InitialValue, BrowseType Browse, bool AcceptEmptyOrInexistant)
        {
            browse = Browse;
            Accept = AcceptEmptyOrInexistant;
            InitializeComponent();
            if (Browse == BrowseType.None)
            {
                btnBrowse.Visible = false;
                btnBrowse.Enabled = false;
                txtInput.Width = 339;
            }
            this.Text = title;
            lblText.Text = description;
            txtInput.Text = InitialValue;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            input = txtInput.Text;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if(browse==BrowseType.Folder){
                FolderBrowserDialog fbd=new FolderBrowserDialog();
                if(Directory.Exists(txtInput.Text)){
                    fbd.SelectedPath=txtInput.Text;
                }
                if(fbd.ShowDialog(this)==DialogResult.OK){
                    txtInput.Text=fbd.SelectedPath;
                }
            }else if(browse==BrowseType.Files){
                OpenFileDialog ofd=new OpenFileDialog();
                if(Directory.Exists(txtInput.Text)){
                    ofd.InitialDirectory=txtInput.Text;
                }else if(File.Exists(txtInput.Text)){
                    ofd.FileName=txtInput.Text;
                }
                if(ofd.ShowDialog(this)==DialogResult.OK){
                    txtInput.Text=ofd.FileName;
                }
            }
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            if (!Accept)
            {
                if (browse == BrowseType.Files)
                {
                    if (!File.Exists(txtInput.Text))
                    {
                        btnOK.Enabled = false;
                    }
                    else
                    {
                        btnOK.Enabled = true;
                    }
                }
                else if (browse == BrowseType.Folder)
                {
                    if (!Directory.Exists(txtInput.Text))
                    {
                        btnOK.Enabled = false;
                    }
                    else
                    {
                        btnOK.Enabled = true;
                    }
                }
                else
                {
                    if (txtInput.Text == "")
                    {
                        btnOK.Enabled = false;
                    }
                    else
                    {
                        btnOK.Enabled = true;
                    }
                }
            }
            else
            {
                if (browse == BrowseType.Folder || browse == BrowseType.Files)
                {
                    try
                    {
                        Path.GetDirectoryName(txtInput.Text);
                        btnOK.Enabled = true;
                    }
                    catch (Exception)
                    {
                        btnOK.Enabled = false;
                    }
                }
            }
        }
    }
}

