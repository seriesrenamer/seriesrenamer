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

namespace Renamer.Dialogs
{
    /// <summary>
    /// Enter season dialog for setting season value on multiple files at once
    /// </summary>
    public partial class EnterSeason : Form
    {
        /// <summary>
        /// Selected season
        /// </summary>
        public int season = 1;

        /// <summary>
        /// standard constructor
        /// </summary>
        public EnterSeason(int season)
        {
            this.season = season;
            InitializeComponent();
            nudSeason.Value = season;
        }

        /// <summary>
        /// Sets selected season value and DialogResult.OK and closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            season = (int)nudSeason.Value;
            Close();
        }

        /// <summary>
        /// Sets DialogResult.Cancel and closes
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
