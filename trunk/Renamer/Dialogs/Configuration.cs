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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Renamer.Classes.Configuration.Keywords;
using Renamer.Classes;
using Renamer.Classes.Provider;
using Renamer.Logging;

namespace Renamer.Dialogs
{
    /// <summary>
    /// Configuration dialog
    /// </summary>
    public partial class Configuration : Form
    {
        /// <summary>
        /// standard constructor
        /// </summary>
        public Configuration() {
            InitializeComponent();
        }

        /// <summary>
        /// Initialization, sets all values from Config file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Configuration_Load(object sender, EventArgs e) {
            //Get some strings            
            txtReplace.Text = Helper.ReadProperty(Config.InvalidCharReplace);
            nudSearchDepth.Text = Helper.ReadProperty(Config.MaxDepth);
            txtIgnoreFiles.Text = Helper.ReadProperty(Config.IgnoreFiles);
            txtDestination.Text = Helper.ReadProperty(Config.DestinationDirectory);

            //Get multiline strings
            txtExtensions.Text = "";
            foreach (string s in Helper.ReadProperties(Config.Extensions)) {
                txtExtensions.Text += s.ToLower() + Environment.NewLine;
            }
            if (txtExtensions.Text.Length > 0)
                txtExtensions.Text = txtExtensions.Text.Substring(0, txtExtensions.Text.Length - Environment.NewLine.Length);

            txtSubs.Text = "";
            foreach (string s in Helper.ReadProperties(Config.SubtitleExtensions)) {
                txtSubs.Text += s.ToLower() + Environment.NewLine;
            }
            if (txtSubs.Text.Length > 0)
                txtSubs.Text = txtSubs.Text.Substring(0, txtSubs.Text.Length - Environment.NewLine.Length);

            txtPattern.Text = "";
            foreach (string s in Helper.ReadProperties(Config.EpIdentifier)) {
                txtPattern.Text += s + Environment.NewLine;
            }
            if (txtPattern.Text.Length > 0)
                txtPattern.Text = txtPattern.Text.Substring(0, txtPattern.Text.Length - Environment.NewLine.Length);

            txtStringReplace.Text = "";
            foreach (string s in Helper.ReadProperties(Config.Replace)) {
                txtStringReplace.Text += s + Environment.NewLine;
            }
            if (txtStringReplace.Text.Length > 0)
                txtStringReplace.Text = txtStringReplace.Text.Substring(0, txtStringReplace.Text.Length - Environment.NewLine.Length);

            txtTags.Text = "";
            foreach (string s in Helper.ReadProperties(Config.Tags)) {
                txtTags.Text += s + Environment.NewLine;
            }
            if (txtTags.Text.Length > 0)
                txtTags.Text = txtTags.Text.Substring(0, txtTags.Text.Length - Environment.NewLine.Length);

            txtExtract.Text = "";
            foreach (string s in Helper.ReadProperties(Config.Extract)) {
                txtExtract.Text += s + Environment.NewLine;
            }
            if (txtExtract.Text.Length > 0)
                txtExtract.Text = txtExtract.Text.Substring(0, txtExtract.Text.Length - Environment.NewLine.Length);
            
            //Get some enums
            cbLogfile.SelectedIndex = (int)(Helper.ReadEnum<Logging.LogLevel>(Config.LogFileLevel));
            cbLogwindow.SelectedIndex = (int)(Helper.ReadEnum<Logging.LogLevel>(Config.LogTextBoxLevel));
            cbLogmessagebox.SelectedIndex = (int)(Helper.ReadEnum<Logging.LogLevel>(Config.LogMessageBoxLevel));

            cbReplace.SelectedIndex = (int)Helper.ReadEnum<Helper.InvalidFilenameAction>(Config.InvalidFilenameAction);
            cbUmlaut.SelectedIndex = (int)Helper.ReadEnum<InfoEntry.UmlautAction>(Config.Umlaute) - 1;
            cbCase.SelectedIndex = (int)Helper.ReadEnum<InfoEntry.Case>(Config.Case) - 1;

            //Get some ints
            nudTimeout.Value = Helper.ReadInt(Config.Timeout);

            //Get some bools
            chkCreateDirectoryStructure.Checked = Helper.ReadBool(Config.CreateDirectoryStructure);
            chkDeleteEmptyFolders.Checked = Helper.ReadBool(Config.DeleteEmptyFolders);
            chkDeleteAllEmptyFolders.Checked = Helper.ReadBool(Config.DeleteAllEmptyFolders);
            chkUseSeasonSubdirs.Checked = Helper.ReadBool(Config.UseSeasonSubDir);
            chkResize.Checked = Helper.ReadBool(Config.ResizeColumns);
            chkFindMissingEpisodes.Checked = Helper.ReadBool(Config.FindMissingEpisodes);
            chkDeleteSampleFiles.Checked = Helper.ReadBool(Config.DeleteSampleFiles);         
   
            //relation provider combobox
            cbProviders.Items.AddRange(RelationProvider.ProviderNames);

            string LastProvider = Helper.ReadProperty(Config.LastProvider);
            if (LastProvider == null)
                LastProvider = "";
            cbProviders.SelectedIndex = Math.Max(0, cbProviders.Items.IndexOf(LastProvider));
            RelationProvider provider = RelationProvider.GetCurrentProvider();
            if (provider == null)
            {
                Logger.Instance.LogMessage("No relation provider found/selected", LogLevel.ERROR);
                return;
            }
            Helper.WriteProperty(Config.LastProvider, cbProviders.Text);
        }

        /// <summary>
        /// Discarts all changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e) {
            Close();
        }

        /// <summary>
        /// Saves all settings to config file cache
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e) {
            Helper.WriteProperty(Config.Timeout, nudTimeout.Value.ToString());
            Helper.WriteProperty(Config.MaxDepth, nudSearchDepth.Value.ToString());
            Helper.WriteProperty(Config.InvalidCharReplace, txtReplace.Text);
            Helper.WriteProperty(Config.DestinationDirectory, txtDestination.Text);
            string[] extensions = txtExtensions.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //Convert to lowercase
            for (int i = 0; i < extensions.GetLength(0); i++) {
                extensions[i] = extensions[i].ToLower();
            }
            Helper.WriteProperties(Config.Extensions, extensions);
            string[] subextensions = txtSubs.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //Convert to lowercase
            for (int i = 0; i < subextensions.GetLength(0); i++) {
                subextensions[i] = subextensions[i].ToLower();
            }
            Helper.WriteProperties(Config.SubtitleExtensions, subextensions);
            string[] patterns = txtPattern.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Helper.WriteProperties(Config.EpIdentifier, patterns);
            string[] extract = txtExtract.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Helper.WriteProperties(Config.Extract, extract);

            Helper.WriteProperty(Config.LogFileLevel, cbLogfile.SelectedIndex.ToString());
            Helper.WriteProperty(Config.LogTextBoxLevel, cbLogwindow.SelectedIndex.ToString());
            Helper.WriteProperty(Config.LogMessageBoxLevel, cbLogmessagebox.SelectedIndex.ToString());

            Helper.WriteProperty(Config.Umlaute, Enum.GetName(typeof(InfoEntry.UmlautAction), cbUmlaut.SelectedIndex + 1));
            Helper.WriteProperty(Config.Case, Enum.GetName(typeof(InfoEntry.Case), cbCase.SelectedIndex + 1));
            Helper.WriteProperty(Config.InvalidFilenameAction, ((Helper.InvalidFilenameAction)cbReplace.SelectedIndex).ToString());
            string[] replace = txtStringReplace.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Helper.WriteProperties(Config.Replace, replace);
            string[] tags = txtTags.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Helper.WriteProperties(Config.Tags, tags);
            
            Helper.WriteBool(Config.CreateDirectoryStructure, chkCreateDirectoryStructure.Checked);
            Helper.WriteBool(Config.DeleteEmptyFolders, chkDeleteEmptyFolders.Checked);
            Helper.WriteBool(Config.DeleteAllEmptyFolders, chkDeleteAllEmptyFolders.Checked);
            Helper.WriteBool(Config.UseSeasonSubDir, chkUseSeasonSubdirs.Checked);
            Helper.WriteBool(Config.ResizeColumns, chkResize.Checked);
            Helper.WriteBool(Config.FindMissingEpisodes, chkFindMissingEpisodes.Checked);
            Helper.WriteBool(Config.DeleteSampleFiles, chkDeleteSampleFiles.Checked);

            Helper.WriteProperties(Config.IgnoreFiles, txtIgnoreFiles.Text);
            Helper.WriteProperty(Config.LastProvider, cbProviders.SelectedItem.ToString());
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Resets config file to default values stored in Settings class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDefaults_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Reset to defaults?", "Reset", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                Settings settings = Settings.Instance;
                //find config file, delete in memory, overwrite physical file and reload it. Then, refresh this dialog
                settings[Helper.DefaultConfigFile()].LoadDefaults();
                settings[Helper.DefaultConfigFile()].Flush();

                //refresh the gui
                Configuration_Load(null, null);
            }
        }

        private void chkDeleteEmptyFolders_CheckedChanged(object sender, EventArgs e) {
            label17.Enabled = chkDeleteEmptyFolders.Checked;
            label18.Enabled = chkDeleteEmptyFolders.Checked;
            chkDeleteAllEmptyFolders.Enabled = chkDeleteEmptyFolders.Checked;
            txtIgnoreFiles.Enabled = chkDeleteEmptyFolders.Checked;
        }

        private void btnDestination_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = txtDestination.Text;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtDestination.Text = folderBrowserDialog1.SelectedPath;
            }
        }

    }
}
