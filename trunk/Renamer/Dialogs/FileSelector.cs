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
    /// File selector class used to assign a subtitle file to a video file
    /// </summary>
    public partial class FileSelector : Form
    {
        /// <summary>
        /// Selected index
        /// </summary>
        public int selection = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="files">List of files to select</param>
        public FileSelector(List<string> files)
        {
            InitializeComponent();
            for (int i = 0; i < files.Count; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            cbFiles.Items.AddRange(files.ToArray());
            cbFiles.SelectedIndex = 0;
        }

        /// <summary>
        /// Set selected index and DialogResult.OK and close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            selection = cbFiles.SelectedIndex;
            DialogResult = DialogResult.OK;
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
    }
}
