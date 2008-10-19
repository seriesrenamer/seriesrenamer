using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Renamer
{
    /// <summary>
    /// Configuration dialog
    /// </summary>
    public partial class Configuration : Form
    {
        /// <summary>
        /// standard constructor
        /// </summary>
        public Configuration()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialization, sets all values from Config file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Configuration_Load(object sender, EventArgs e)
        {
            nudTimeout.Value = Convert.ToInt32(Helper.ReadProperty(Config.Timeout));
            txtReplace.Text = Helper.ReadProperty(Config.InvalidCharReplace);
            cbReplace.SelectedIndex = (int)(Helper.InvalidFilenameAction)Enum.Parse(typeof(Helper.InvalidFilenameAction), Helper.ReadProperty(Config.InvalidFilenameAction));
            cbUmlaut.SelectedIndex = (int)(Helper.UmlautAction)Enum.Parse(typeof(Helper.UmlautAction), Helper.ReadProperty(Config.Umlaute));
            cbCase.SelectedIndex = (int)(Helper.Case)Enum.Parse(typeof(Helper.Case), Helper.ReadProperty(Config.Case));
            nudSearchDepth.Text = Helper.ReadProperty(Config.MaxDepth);
            //Get line separated string for extensions and patterns
            txtExtensions.Text = "";
            foreach (string s in Helper.ReadProperties(Config.Extensions))
            {
                txtExtensions.Text += s.ToLower() + Environment.NewLine;
            }
            if (txtExtensions.Text.Length > 0)
            {
                txtExtensions.Text = txtExtensions.Text.Substring(0, txtExtensions.Text.Length - Environment.NewLine.Length);
            }
            txtSubs.Text = "";
            foreach (string s in Helper.ReadProperties(Config.SubtitleExtensions))
            {
                txtSubs.Text += s.ToLower() + Environment.NewLine;
            }
            if (txtSubs.Text.Length > 0)
            {
                txtSubs.Text = txtSubs.Text.Substring(0, txtSubs.Text.Length - Environment.NewLine.Length);
            }
            txtPattern.Text = "";
            foreach (string s in Helper.ReadProperties(Config.EpIdentifier))
            {
                txtPattern.Text += s + Environment.NewLine;
            }
            if (txtPattern.Text.Length > 0)
            {
                txtPattern.Text = txtPattern.Text.Substring(0, txtPattern.Text.Length - Environment.NewLine.Length);
            }
            cbError.SelectedIndex = (int)(Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelError));
            cbWarning.SelectedIndex = (int)(Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelWarning));
            cbStatus.SelectedIndex = (int)(Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelStatus));
            cbInfo.SelectedIndex = (int)(Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelInfo));
            cbDebug.SelectedIndex = (int)(Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelDebug));
            txtExtract.Text = Helper.ReadProperty(Config.Extract);
            chkCreateDirectoryStructure.Checked = Helper.StringToBool(Helper.ReadProperty(Config.CreateDirectoryStructure));
            chkDeleteEmptyFolders.Checked = Helper.StringToBool(Helper.ReadProperty(Config.DeleteEmptyFolders));
            chkDeleteAllEmptyFolders.Checked = Helper.StringToBool(Helper.ReadProperty(Config.DeleteAllEmptyFolders));
            chkUseSeasonSubdirs.Checked = Helper.StringToBool(Helper.ReadProperty(Config.UseSeasonSubDir));            
            txtIgnoreFiles.Text = Helper.ReadProperty(Config.IgnoreFiles);
        }

        /// <summary>
        /// Discarts all changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Saves all settings to config file cache
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            Helper.WriteProperty(Config.Timeout, nudTimeout.Value.ToString());
            Helper.WriteProperty(Config.MaxDepth, nudSearchDepth.Value.ToString());
            Helper.WriteProperty(Config.InvalidCharReplace, txtReplace.Text);
            string[] extensions=txtExtensions.Text.Split(new string[]{Environment.NewLine},StringSplitOptions.RemoveEmptyEntries);
            //Convert to lowercase
            for (int i = 0; i < extensions.GetLength(0); i++)
            {
                extensions[i] = extensions[i].ToLower();
            }
            Helper.WriteProperties(Config.Extensions, extensions);
            string[] subextensions = txtSubs.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //Convert to lowercase
            for (int i = 0; i < subextensions.GetLength(0); i++)
            {
                subextensions[i] = subextensions[i].ToLower();
            }
            Helper.WriteProperties(Config.SubtitleExtensions, subextensions);
            string[] patterns = txtPattern.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Helper.WriteProperties(Config.EpIdentifier, patterns);
            Helper.WriteProperty(Config.LogLevelError, ((Helper.LogLevel)cbError.SelectedIndex).ToString());
            Helper.WriteProperty(Config.LogLevelInfo, ((Helper.LogLevel)cbInfo.SelectedIndex).ToString());
            Helper.WriteProperty(Config.LogLevelStatus, ((Helper.LogLevel)cbStatus.SelectedIndex).ToString());
            Helper.WriteProperty(Config.LogLevelWarning, ((Helper.LogLevel)cbWarning.SelectedIndex).ToString());
            Helper.WriteProperty(Config.LogLevelDebug, ((Helper.LogLevel)cbDebug.SelectedIndex).ToString());
            Helper.WriteProperty(Config.Umlaute,Enum.GetName(typeof(Helper.UmlautAction), cbUmlaut.SelectedIndex));
            Helper.WriteProperty(Config.Case,Enum.GetName(typeof(Helper.Case), cbCase.SelectedIndex));
            Helper.WriteProperty(Config.InvalidFilenameAction, ((Helper.InvalidFilenameAction)cbReplace.SelectedIndex).ToString());
            Helper.WriteProperty(Config.Extract, txtExtract.Text);
            if (chkCreateDirectoryStructure.Checked)
            {
                Helper.WriteProperty(Config.CreateDirectoryStructure, "1");
            }
            else
            {
                Helper.WriteProperty(Config.CreateDirectoryStructure, "0");
            }
            if (chkDeleteEmptyFolders.Checked)
            {
                Helper.WriteProperty(Config.DeleteEmptyFolders, "1");
            }
            else
            {
                Helper.WriteProperty(Config.DeleteEmptyFolders, "0");
            }
            if (chkDeleteAllEmptyFolders.Checked)
            {
                Helper.WriteProperty(Config.DeleteAllEmptyFolders, "1");
            }
            else
            {
                Helper.WriteProperty(Config.DeleteAllEmptyFolders, "0");
            }
            if (chkUseSeasonSubdirs.Checked)
            {
                Helper.WriteProperty(Config.UseSeasonSubDir, "1");
            }
            else
            {
                Helper.WriteProperty(Config.UseSeasonSubDir, "0");
            }
            Helper.WriteProperties(Config.IgnoreFiles, txtIgnoreFiles.Text);

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Resets config file to default values stored in Settings class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDefaults_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset to defaults?", "Reset", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //find config file, delete in memory, overwrite physical file and reload it. Then, refresh this dialog
                for(int i=0;i<Settings.files.Count;i++){
                    if (Settings.files[i].FilePath == Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + Settings.ConfigName)
                    {
                        Settings.files[i].variables = ((Hashtable)Settings.Defaults.variables.Clone());
                        Settings.files[i].Flush();
                        break;
                    }
                }           

                //refresh the gui
                Configuration_Load(null,null);
            }
        }

    }
}
