using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Renamer
{
    /// <summary>
    /// File selection filter dialog
    /// </summary>
    public partial class Filter : Form
    {
        /// <summary>
        /// entered filter string
        /// </summary>
        public string result;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="str">Initial value to show in textbox</param>
        public Filter(string str)
        {
            InitializeComponent();
            txtFilter.Text = str;
        }

        /// <summary>
        /// Sets entered filter text and DialogResult.OK and closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult=DialogResult.OK;
            result=txtFilter.Text;
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
