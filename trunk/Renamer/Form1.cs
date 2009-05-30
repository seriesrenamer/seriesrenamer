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
using System.Text.RegularExpressions;
using System.Net;
using System.Collections;
using System.Diagnostics;
using Schematrix;
using ICSharpCode.SharpZipLib.Zip;
using Renamer.Classes;
using Renamer.Dialogs;
using Renamer.Classes.Configuration.Keywords;
using Renamer.Classes.Configuration;
using System.Runtime.InteropServices;
using Renamer.Logging;
using Renamer.Classes.Provider;
namespace Renamer
{
    /// <summary>
    /// Main Form
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Settings class contains some stuff which can't be stored in config file for one reason or another
        /// </summary>
        private Settings settings;

        /// <summary>
        /// Custom sorting class to sort list view for 2 columns
        /// </summary>
        private ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();

        /// <summary>
        /// Column width ratios needed to keep them during resize
        /// </summary>
        private float[] columnsizes;

        /// <summary>
        /// Temp variable to store a previously focused control
        /// </summary>
        private Control focused = null;

        /// <summary>
        /// Program arguments
        /// </summary>
        private List<string> args;


        /// <summary>
        /// GUI constructor
        /// </summary>
        /// <param name="args">program arguments</param>
        public Form1(string[] args) {
            this.args = new List<string>(args);
            InitializeComponent();
        }

        #region processing
        #region Name Creation




        /// <summary>
        /// Removes unneeded tags from videos
        /// </summary>
        private void RemoveVideoTags() {

            //Tags should be preceeded by . or _
            string[] tags = Helper.ReadProperties(Config.Tags);
            List<string> regexes = new List<string>();
            foreach (string s in tags) {
                regexes.Add("[\\._\\(\\[-]+" + s);
            }
            foreach (ListViewItem lvi in lstFiles.Items) {
                InfoEntry ie = InfoEntryManager.Instance[((int)lvi.Tag)];
                ie.ProcessingRequested = false;
                //Go through all selected files and remove tags and clean them up
                if (lvi.Selected) {
                    ie.RemoveVideoTags(regexes.ToArray());
                    lvi.Selected = false;
                }
                SyncItem(((int)lvi.Tag), false);
            }
        }
        #endregion

        /// <summary>
        /// Main Rename function
        /// </summary>
        private void Rename() {
            InfoEntryManager.Instance.Rename();
            //Get a list of all involved folders
            FillListView();
        }

        #endregion
        #region Subtitles
        #region Parsing


       


        /// <summary>
        /// This function is needed if Subtitle links are located on a series page, not implemented yet
        /// </summary>
        /// <param name="url">URL of the page to get subtitle links from</param>
        private void GetSubtitleFromSeriesPage(string url) {
            //Don't forget the cropping
            //
            //Source cropping
            //source = source.Substring(Math.Max(source.IndexOf(provider.SearchStart),0));
            //source = source.Substring(0, Math.Max(source.LastIndexOf(provider.SearchEnd),0));
            /*
            //if episode infos are stored on a new page for each season, this should be marked with %S in url, so we can iterate through all those pages
            int season = 1;
            string url2 = url;
            while (true)
            {
                    if (url2.Contains("%S"))
                    {
                            url = url2.Replace("%S", season.ToString());
                    }
                    if (url == null || url == "") return;
                    // request
                    url = System.Web.HttpUtility.UrlPathEncode(url);
                    HttpWebRequest requestHtml = null;
                    try
                    {
                            requestHtml = (HttpWebRequest)(HttpWebRequest.Create(url));
                    }
                    catch (Exception ex)
                    {
                            Logger.Instance.LogMessage(ex.Message, LogLevel.ERROR);
                            return;
                    }
                    requestHtml.Timeout = 5000;
                    // get response
                    HttpWebResponse responseHtml;
                    try
                    {
                            responseHtml = (HttpWebResponse)(requestHtml.GetResponse());
                    }
                    catch (WebException e)
                    {
                            Logger.Instance.LogMessage(e.Message, LogLevel.ERROR);
                            return;
                    }
                    //if we get redirected, lets assume this page does not exist
                    if (responseHtml.ResponseUri.AbsoluteUri != url)
                    {
                            break;
                    }
                    // and download
                    //Logger.Instance.LogMessage("charset=" + responseHtml.CharacterSet, LogLevel.INFO);

                    StreamReader r = new StreamReader(responseHtml.GetResponseStream(), Encoding.GetEncoding(responseHtml.CharacterSet));
                    string source = r.ReadToEnd();
                    r.Close();
                    responseHtml.Close();
                    string pattern = Settings.subprovider.RelationsRegExp;
                    MatchCollection mc = Regex.Matches(source, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    foreach (Match m in mc)
                    {
                            //if we are iterating through season pages, take season from page url directly
                            if (url != url2)
                            {
                                    Info.AddRelationCollection(new Relation(season.ToString(), m.Groups["Episode"].Value, System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value)));
                                    //Logger.Instance.LogMessage("Found Relation: " + "S" + season.ToString() + "E" + m.Groups["Episode"].Value + " - " + System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value), LogLevel.INFO);
                            }
                            else
                            {
                                    Info.AddRelationCollection(new Relation(m.Groups["Season"].Value, m.Groups["Episode"].Value, System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value)));
                                    //Logger.Instance.LogMessage("Found Relation: " + "S" + m.Groups["Season"].Value + "E" + m.Groups["Episode"].Value + " - " + System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value), LogLevel.INFO);
                            }
                    }
                    // THOU SHALL NOT FORGET THE BREAK
                    if (!url2.Contains("%S")) break;
                    season++;
            }*/
        }
        #endregion

 

       
        #endregion
        #region LstFilesEvents
        //Update Coloring when file is checked/unchecked and set process flag
        private void lstFiles_ItemChecked(object sender, ItemCheckedEventArgs e) {
            InfoEntryManager.Instance[(int)e.Item.Tag].ProcessingRequested = e.Item.Checked;
            Colorize(e.Item);
        }

        //Since sorting after the last two selected columns is supported, we need some event handling here
        private void lstFiles_ColumnClick(object sender, ColumnClickEventArgs e) {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn) {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending) {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else {
                // Set the column number that is to be sorted; default to ascending.

                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.lstFiles.Sort();
        }

        //End editing with combo box data types
        private void cbEdit_SelectedIndexChanged(object sender, EventArgs e) {
            lstFiles.EndEditing(true);
        }

        //Start editing single values
        private void lstFiles_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e) {
            if (e.SubItem != 0 && e.SubItem != 1) {
                if (settings.IsMonoCompatibilityMode) {
                    Logger.Instance.LogMessage("Editing Entries dynamically is not supported in Mono unfortunately :(", LogLevel.WARNING);
                    return;
                }
                RelationCollection rc = RelationManager.Instance.GetRelationCollection(InfoEntryManager.Instance[(int)e.Item.Tag].Showname);
                if (e.SubItem == 4) {
                    //if season is valid and there are relations at all, show combobox. Otherwise, just show edit box
                    if (rc != null && RelationManager.Instance.Count > 0 && Convert.ToInt32(e.Item.SubItems[2].Text) >= rc.FindMinSeason() && Convert.ToInt32(e.Item.SubItems[2].Text) <= rc.FindMaxSeason()) {
                        comEdit.Items.Clear();
                        foreach (Relation rel in rc) {
                            if (rel.Season == InfoEntryManager.Instance[(int)e.Item.Tag].Season) {
                                comEdit.Items.Add(rel.Name);
                            }
                        }
                        lstFiles.StartEditing(comEdit, e.Item, e.SubItem);
                    }
                    else {
                        lstFiles.StartEditing(txtEdit, e.Item, e.SubItem);
                    }
                }
                else if (e.SubItem == 5 || e.SubItem == 6 || e.SubItem == 7) {
                    lstFiles.StartEditing(txtEdit, e.Item, e.SubItem);
                }
                else {
                    //clamp season and episode values to allowed values
                    if (rc != null && rc.Count > 0) {
                        if (e.SubItem == 2) {
                            numEdit.Minimum = rc.FindMinSeason();
                            numEdit.Maximum = rc.FindMaxSeason();
                        }
                        else if (e.SubItem == 3) {
                            numEdit.Minimum = rc.FindMinEpisode(Convert.ToInt32(InfoEntryManager.Instance[(int)e.Item.Tag].Season));
                            numEdit.Maximum = rc.FindMaxEpisode(Convert.ToInt32(InfoEntryManager.Instance[(int)e.Item.Tag].Season));
                        }
                    }
                    else {
                        numEdit.Minimum = 0;
                        numEdit.Maximum = 10000;
                    }
                    lstFiles.StartEditing(numEdit, e.Item, e.SubItem);
                }
            }
        }

        //End editing a value, apply possible changes and process them
        private void lstFiles_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e) {
            string dir = Helper.ReadProperty(Config.LastDirectory);
            bool CreateDirectoryStructure = Helper.ReadInt(Config.CreateDirectoryStructure) == 1;
            bool UseSeasonSubdirs = Helper.ReadInt(Config.UseSeasonSubDir) == 1;
            int tmp = -1;
            //add lots of stuff here
            switch (e.SubItem) {
                //season
                case 2:
                    try{
                        tmp = Int32.Parse(e.DisplayText);
                    }
                    catch (Exception ex){
                        Logger.Instance.LogMessage("Cannot parse '" + e.DisplayText + "' to an integer", LogLevel.WARNING);
                    }
                    InfoEntryManager.Instance[(int)e.Item.Tag].Season = tmp;
                    if (e.DisplayText == "") {
                        InfoEntryManager.Instance[(int)e.Item.Tag].Movie = true;
                    }
                    else {
                        InfoEntryManager.Instance[(int)e.Item.Tag].Movie = false;
                    }
                    //SetupRelation((int)e.Item.Tag);
                    //foreach (InfoEntry ie in InfoEntryManager.Episodes)
                    //{
                    //    SetDestinationPath(ie, dir, CreateDirectoryStructure, UseSeasonSubdirs);
                    //}
                    break;
                //Episode
                case 3:
                    try {
                        tmp = Int32.Parse(e.DisplayText);
                    }
                    catch (Exception ex) {
                        Logger.Instance.LogMessage("Cannot parse '" + e.DisplayText + "' to an integer", LogLevel.WARNING);
                    }
                    InfoEntryManager.Instance[(int)e.Item.Tag].Episode = tmp;
                    if (e.DisplayText == "") {
                        InfoEntryManager.Instance[(int)e.Item.Tag].Movie = true;
                    }
                    else {
                        InfoEntryManager.Instance[(int)e.Item.Tag].Movie = false;
                    }
                    //SetupRelation((int)e.Item.Tag);                    
                    //SetDestinationPath(InfoEntryManager.Instance[(int)e.Item.Tag], dir, CreateDirectoryStructure, UseSeasonSubdirs);
                    break;
                //name
                case 4:
                    //backtrack to see if entered text matches a season/episode
                    RelationCollection rc = RelationManager.Instance.GetRelationCollection(InfoEntryManager.Instance[(int)e.Item.Tag].Showname);
                    if (rc != null) {
                        foreach (Relation rel in rc) {
                            //if found, set season and episode in gui and sync back to data
                            if (e.DisplayText == rel.Name) {
                                e.Item.SubItems[2].Text = rel.Season.ToString();
                                e.Item.SubItems[3].Text = rel.Episode.ToString();
                            }
                        }
                    }
                    InfoEntryManager.Instance[(int)e.Item.Tag].Name = e.DisplayText;
                    break;
                //Filename
                case 5:
                    InfoEntryManager.Instance[(int)e.Item.Tag].NewFileName = e.DisplayText;
                    break;
                //Destination
                case 6:
                    try {
                        Path.GetDirectoryName(e.DisplayText);
                        InfoEntryManager.Instance[(int)e.Item.Tag].Destination = e.DisplayText;
                    }
                    catch (Exception) {
                        e.Cancel = true;
                    }
                    break;
                case 7:
                    InfoEntryManager.Instance[(int)e.Item.Tag].Showname = e.DisplayText;
                    break;
                default:
                    throw new Exception("Unreachable code");
            }
            SyncItem((int)e.Item.Tag, false);
        }

        //Double click = Invert process flag
        private void lstFiles_DoubleClick(object sender, EventArgs e) {
            lstFiles.Items[lstFiles.SelectedIndices[0]].Checked = !lstFiles.Items[lstFiles.SelectedIndices[0]].Checked;
        }

        //Enter = Open file
        private void lstFiles_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                    Process myProc = Process.Start(InfoEntryManager.Instance[(int)lvi.Tag].Filepath + Path.DirectorySeparatorChar + InfoEntryManager.Instance[(int)lvi.Tag].Filename);
                }
            }
        }
        #endregion
        #region GUI-Events
        //Main Initialization
        private void Form1_Load(object sender, EventArgs e) {
            settings = Settings.Instance;
            Logger logger = Logger.Instance;
            // Add Logger
            logger.addLogger(new FileLogger(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "Renamer.log", true, Helper.ReadEnum<LogLevel>(Config.LogFileLevel)));
            logger.addLogger(new MessageBoxLogger(Helper.ReadEnum<LogLevel>(Config.LogMessageBoxLevel)));

            //mono compatibility fixes
            if (Type.GetType("Mono.Runtime") != null) {
                logger.addLogger(new TextBoxLogger(txtLog, Helper.ReadEnum<LogLevel>(Config.LogTextBoxLevel)));
                rtbLog.Visible = false;
                txtLog.Visible = true;
                Logger.Instance.LogMessage("Running on Mono", LogLevel.INFO);
            }
            else {
                logger.addLogger(new RichTextBoxLogger(rtbLog, Helper.ReadEnum<LogLevel>(Config.LogTextBoxLevel)));
            }

            // Init logging here:

            


            //and read a value to make sure it is loaded into memory
            Helper.ReadProperty(Config.Case);

            lstFiles.ListViewItemSorter = lvwColumnSorter;
            txtTarget.Text = Helper.ReadProperty(Config.TargetPattern);

            //relations provider combo box
            cbProviders.Items.AddRange(RelationProvider.ProviderNames);

            string LastProvider = Helper.ReadProperty(Config.LastProvider);
            if (LastProvider == null)
                LastProvider = "";
            cbProviders.SelectedIndex = Math.Max(0, cbProviders.Items.IndexOf(LastProvider));
            RelationProvider provider = RelationProvider.GetCurrentProvider();
            if (provider == null) {
                Logger.Instance.LogMessage("No relation provider found/selected", LogLevel.ERROR);
                return;
            }
            Helper.WriteProperty(Config.LastProvider, cbProviders.Text);

            //subtitle provider combo box
            cbSubs.Items.AddRange(SubtitleProvider.ProviderNames);
            string LastSubProvider = Helper.ReadProperty(Config.LastSubProvider);
            if (LastSubProvider == null)
                LastSubProvider = "";
            cbSubs.SelectedIndex = Math.Max(0, cbSubs.Items.IndexOf(LastSubProvider));
            Helper.WriteProperty(Config.LastSubProvider, cbSubs.Text);

            //Last directory
            string lastdir = Helper.ReadProperty(Config.LastDirectory);

            //First argument=folder
            if (args.Count > 0) {
                string dir = args[0].Replace("\"", "");
                if (Directory.Exists(args[0])) {
                    lastdir = dir;
                    Helper.WriteProperty(Config.LastDirectory, lastdir);
                }
            }
            if (lastdir != null && lastdir != "" && Directory.Exists(lastdir)) {
                txtPath.Text = lastdir;
                Environment.CurrentDirectory = lastdir;
                UpdateList(true);
            }


            string[] ColumnWidths = Helper.ReadProperties(Config.ColumnWidths);
            string[] ColumnOrder = Helper.ReadProperties(Config.ColumnOrder);
            for (int i = 0; i < lstFiles.Columns.Count; i++) {
                try {
                    int width = lstFiles.Columns[i].Width;
                    Int32.TryParse(ColumnWidths[i], out width);
                    lstFiles.Columns[i].Width = width;
                }
                catch (Exception) {
                    Logger.Instance.LogMessage("Invalid Value for ColumnWidths[" + i + "]", LogLevel.ERROR);
                }
                try {
                    int order = lstFiles.Columns[i].DisplayIndex;
                    Int32.TryParse(ColumnOrder[i], out order);
                    lstFiles.Columns[i].DisplayIndex = order;
                }
                catch (Exception) {
                    Logger.Instance.LogMessage("Invalid Value for ColumnOrder[" + i + "]", LogLevel.ERROR);
                }
            }
            string[] Windowsize = Helper.ReadProperties(Config.WindowSize);
            if (Windowsize.GetLength(0) >= 2) {
                try {
                    int w, h;
                    Int32.TryParse(Windowsize[0], out w);
                    Int32.TryParse(Windowsize[1], out h);
                    this.Width = w;
                    this.Height = h;
                }
                catch (Exception) {
                    Logger.Instance.LogMessage("Couldn't process WindowSize property: " + Helper.ReadProperty(Config.WindowSize), LogLevel.ERROR);
                }
                //focus fix
                txtPath.Focus();
                txtPath.Select(txtPath.Text.Length, 0);
            }
        }

        //Auto column resize by storing column width ratios at resize start
        private void Form1_ResizeBegin(object sender, EventArgs e) {
            if (Helper.ReadInt(Config.ResizeColumns) == 1) {
                columnsizes = new float[]{
                (float)(lstFiles.Columns[0].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[1].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[2].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[3].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[4].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[5].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[6].Width)/(float)(lstFiles.ClientRectangle.Width),};
                float sum = 0;
                for (int i = 0; i < 7; i++) {
                    sum += columnsizes[i];
                }
                //some numeric correction to make ratios:
                for (int i = 0; i < 7; i++) {
                    columnsizes[i] *= (float)1 / sum;
                }
            }
        }

        //Auto column resize, restore Column width ratios at resize end (to make sure!)
        private void Form1_ResizeEnd(object sender, EventArgs e) {
            if (Helper.ReadInt(Config.ResizeColumns) == 1) {
                if (lstFiles != null && lstFiles.Columns.Count == 7 && columnsizes != null) {
                    lstFiles.Columns[0].Width = (int)(columnsizes[0] * (float)(lstFiles.ClientRectangle.Width));
                    lstFiles.Columns[1].Width = (int)(columnsizes[1] * (float)(lstFiles.ClientRectangle.Width));
                    lstFiles.Columns[2].Width = (int)(columnsizes[2] * (float)(lstFiles.ClientRectangle.Width));
                    lstFiles.Columns[3].Width = (int)(columnsizes[3] * (float)(lstFiles.ClientRectangle.Width));
                    lstFiles.Columns[4].Width = (int)(columnsizes[4] * (float)(lstFiles.ClientRectangle.Width));
                    lstFiles.Columns[5].Width = (int)(columnsizes[5] * (float)(lstFiles.ClientRectangle.Width));
                    lstFiles.Columns[6].Width = (int)(columnsizes[5] * (float)(lstFiles.ClientRectangle.Width));
                }
            }
        }

        //Auto column resize, restore Column width ratios during resize
        private void Form1_Resize(object sender, EventArgs e) {
            if (this.Visible) {
                if (Helper.ReadInt(Config.ResizeColumns) == 1) {
                    if (lstFiles != null && lstFiles.Columns.Count == 6 && columnsizes != null) {
                        lstFiles.Columns[0].Width = (int)(columnsizes[0] * (float)(lstFiles.ClientRectangle.Width));
                        lstFiles.Columns[1].Width = (int)(columnsizes[1] * (float)(lstFiles.ClientRectangle.Width));
                        lstFiles.Columns[2].Width = (int)(columnsizes[2] * (float)(lstFiles.ClientRectangle.Width));
                        lstFiles.Columns[3].Width = (int)(columnsizes[3] * (float)(lstFiles.ClientRectangle.Width));
                        lstFiles.Columns[4].Width = (int)(columnsizes[4] * (float)(lstFiles.ClientRectangle.Width));
                        lstFiles.Columns[5].Width = (int)(columnsizes[5] * (float)(lstFiles.ClientRectangle.Width));
                        lstFiles.Columns[6].Width = (int)(columnsizes[6] * (float)(lstFiles.ClientRectangle.Width));
                    }
                }
            }
        }

        //Save last focussed control so it can be restored after splitter between file and log window is moved
        private void scContainer_MouseDown(object sender, MouseEventArgs e) {
            // Get the focused control before the splitter is focused
            focused = getFocused(this.Controls);
        }

        //restore last focussed control so splitter doesn't keep focus
        private void scContainer_MouseUp(object sender, MouseEventArgs e) {
            // If a previous control had focus
            if (focused != null) {
                // Return focus and clear the temp variable for
                // garbage collection (is this needed?)
                focused.Focus();
                focused = null;
            }
        }

        //Update Current Provider setting
        private void cbProviders_SelectedIndexChanged(object sender, EventArgs e) {
            Helper.WriteProperty(Config.LastProvider, cbProviders.SelectedItem.ToString());
        }

        //Update Current Subtitle Provider setting
        private void cbSubs_SelectedIndexChanged(object sender, EventArgs e) {
            Helper.WriteProperty(Config.LastSubProvider, cbSubs.SelectedItem.ToString());
        }

        //Show About box dialog
        private void btnAbout_Click(object sender, EventArgs e) {
            AboutBox ab = new AboutBox();
            ab.AppMoreInfo += Environment.NewLine;
            ab.AppMoreInfo += "Using:" + Environment.NewLine;
            ab.AppMoreInfo += "ListViewEx " + "http://www.codeproject.com/KB/list/ListViewCellEditors.aspx" + Environment.NewLine;
            ab.AppMoreInfo += "About Box " + "http://www.codeproject.com/KB/vb/aboutbox.aspx" + Environment.NewLine;
            ab.AppMoreInfo += "Unrar.dll " + "http://www.rarlab.com" + Environment.NewLine;
            ab.AppMoreInfo += "SharpZipLib " + "http://www.icsharpcode.net/OpenSource/SharpZipLib/" + Environment.NewLine;
            ab.ShowDialog(this);
        }

        //Open link in browser when clicked in log
        private void rtbLog_LinkClicked(object sender, LinkClickedEventArgs e) {
            try {
                Process myProc = Process.Start(e.LinkText);
            }
            catch (Exception ex) {
                Logger.Instance.LogMessage("Couldn't open " + e.LinkText + ":" + ex.Message, LogLevel.ERROR);
            }
        }

        /*//Update Destination paths when title of show is changed
        private void cbTitle_TextChanged(object sender, EventArgs e)
        {
                //when nothing is selected, assume there is no autocompletion taking place right now
                if (cbTitle.SelectedText == "" && cbTitle.Text != "")
                {
                        for (int i = 0; i < cbTitle.Items.Count; i++)
                        {
                                if (((string)cbTitle.Items[i]).ToLower() == cbTitle.Text.ToLower())
                                {
                                        bool found = false;
                                        foreach (object o in cbTitle.Items)
                                        {
                                                if ((string)o == cbTitle.Text)
                                                {
                                                        found = true;
                                                        break;
                                                }
                                        }
                                        if (!found)
                                        {
                                                cbTitle.Items.Add(cbTitle.Text);
                                        }
                                        break;
                                }
                        }
                }
        }*/

        //Clear episode informations fetched from providers
        private void btnClear_Click(object sender, EventArgs e) {
            RelationManager.Instance.Clear();
            UpdateList(true);
        }

        //Show File Open dialog and update file list
        private void btnPath_Click(object sender, EventArgs e) {
            //weird mono hackfix
            if (settings.IsMonoCompatibilityMode) {
                fbdPath.SelectedPath = Environment.CurrentDirectory;
            }
            string lastdir = Helper.ReadProperty(Config.LastDirectory);
            if (!settings.IsMonoCompatibilityMode) {
                if (lastdir != null && lastdir != "" && Directory.Exists(lastdir)) {
                    fbdPath.SelectedPath = lastdir;
                }
                else {
                    fbdPath.SelectedPath = Environment.CurrentDirectory;
                }
            }

            DialogResult dr = fbdPath.ShowDialog();
            if (dr == DialogResult.OK) {
                string path = fbdPath.SelectedPath;
                InfoEntryManager.Instance.SetPath(ref path);
                UpdateList(true);
            }
        }

        //Show configuration dialog
        private void btnConfig_Click(object sender, EventArgs e) {
            Configuration cfg = new Configuration();
            if (cfg.ShowDialog() == DialogResult.OK) {
                UpdateList(true);
            }
        }

        //Fetch all title information etc yada yada yada blalblabla
        private void btnTitles_Click(object sender, EventArgs e) {
            DataGenerator.GetAllTitles();
            FillListView();
        }



        /*//Enter = Click "Get Titles" button
        private void cbTitle_KeyDown(object sender, KeyEventArgs e)
        {
                if (e.KeyCode == Keys.Enter)
                {
                        SetNewTitle(cbTitle.Text);
                        btnTitles.PerformClick();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                        cbTitle.Text = Helper.ReadProperties(Config.LastTitles)[0];
                        cbTitle.SelectionStart = cbTitle.Text.Length;
                }
        }*/

        //Enter = Change current directory
        private void txtPath_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                //need to update the displayed path because it might be changed (tailing backlashes are removed, and added for drives ("C:\" instead of "C:"))
                string path = txtPath.Text;
                InfoEntryManager.Instance.SetPath(ref path);
                txtPath.Text = path;
                txtPath.SelectionStart = txtPath.Text.Length;
                UpdateList(true);
            }
            else if (e.KeyCode == Keys.Escape) {
                txtPath.Text = Helper.ReadProperty(Config.LastDirectory);
                txtPath.SelectionStart = txtPath.Text.Length;
            }
        }

        //Focus lost = change current directory
        private void txtPath_Leave(object sender, EventArgs e) {
            InfoEntryManager.Instance.SetPath(txtPath.Text);
        }

        //Start renaming
        private void btnRename_Click(object sender, EventArgs e) {
            Rename();
        }

        //Focus lost = store desired pattern and update names
        private void txtTarget_Leave(object sender, EventArgs e) {
            if (txtTarget.Text != Helper.ReadProperty(Config.TargetPattern)) {
                Helper.WriteProperty(Config.TargetPattern, txtTarget.Text);
                InfoEntryManager.Instance.CreateNewNames();
                FillListView();
            }
        }

        //Enter = store desired pattern and update names
        private void txtTarget_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                if (txtTarget.Text != Helper.ReadProperty(Config.TargetPattern)) {
                    Helper.WriteProperty(Config.TargetPattern, txtTarget.Text);
                    InfoEntryManager.Instance.CreateNewNames();
                    FillListView();
                }
            }
            else if (e.KeyCode == Keys.Escape) {
                txtTarget.Text = Helper.ReadProperty(Config.TargetPattern);
                txtTarget.SelectionStart = txtTarget.Text.Length;
            }
        }

        //start fetching subtitles
        private void btnSubs_Click(object sender, EventArgs e) {
            DataGenerator.GetSubtitles();
        }

        //Cleanup, save some stuff etc
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            Helper.WriteProperty(Config.LastSubProvider, cbSubs.SelectedItem.ToString());

            //Save column order and sizes
            string[] ColumnWidths = new string[lstFiles.Columns.Count];
            string[] ColumnOrder = new string[lstFiles.Columns.Count];
            for (int i = 0; i < lstFiles.Columns.Count; i++) {
                ColumnOrder[i] = lstFiles.Columns[i].DisplayIndex.ToString();
                ColumnWidths[i] = lstFiles.Columns[i].Width.ToString();
            }
            Helper.WriteProperties(Config.ColumnOrder, ColumnOrder);
            Helper.WriteProperties(Config.ColumnWidths, ColumnWidths);

            //save window size
            string[] WindowSize = new string[2];
            if (this.WindowState == FormWindowState.Normal) {
                WindowSize[0] = this.Size.Width.ToString();
                WindowSize[1] = this.Size.Height.ToString();
            }
            else {
                WindowSize[0] = this.RestoreBounds.Width.ToString();
                WindowSize[1] = this.RestoreBounds.Height.ToString();
            }
            Helper.WriteProperties(Config.WindowSize, WindowSize);

            foreach (DictionaryEntry dict in settings) {
                ((ConfigFile)dict.Value).Flush();
            }
        }
        #endregion
        #region Contextmenu
        //Set which context menu options should be avaiable
        private void contextFiles_Opening(object sender, CancelEventArgs e) {
            editSubtitleToolStripMenuItem.Visible = false;
            viewToolStripMenuItem.Visible = false;
            renamingToolStripMenuItem.Visible = false;
            if (lstFiles.SelectedItems.Count == 1) {
                //if selected file is a subtitle
                List<string> subext = new List<string>(Helper.ReadProperties(Config.SubtitleExtensions));
                if (subext.Contains(InfoEntryManager.Instance[((int)lstFiles.SelectedItems[0].Tag)].Extension.ToLower())) {
                    editSubtitleToolStripMenuItem.Visible = true;
                }

                //if selected file is a video and there is a matching subtitle
                if (InfoEntryManager.Instance.GetSubtitle(InfoEntryManager.Instance[((int)lstFiles.SelectedItems[0].Tag)]) != null) {
                    editSubtitleToolStripMenuItem.Visible = true;
                }

                //if there is a matching video
                if (InfoEntryManager.Instance.GetVideo(InfoEntryManager.Instance[((int)lstFiles.SelectedItems[0].Tag)]) != null) {
                    viewToolStripMenuItem.Visible = true;
                }
            }
            if (lstFiles.SelectedItems.Count > 0) {
                renamingToolStripMenuItem.Visible = true;
                createDirectoryStructureToolStripMenuItem1.Checked = false;
                dontCreateDirectoryStructureToolStripMenuItem.Checked = false;
                largeToolStripMenuItem.Checked = false;
                smallToolStripMenuItem.Checked = false;
                igNorEToolStripMenuItem.Checked = false;
                cAPSLOCKToolStripMenuItem.Checked = false;
                useUmlautsToolStripMenuItem.Checked = false;
                dontUseUmlautsToolStripMenuItem.Checked = false;
                useProvidedNamesToolStripMenuItem.Checked = false;
                copyToolStripMenuItem.Visible = true;
                bool OldPath = false;
                bool OldFilename = false;
                bool Name = false;
                bool Destination = false;
                bool NewFilename = false;
                InfoEntry.DirectoryStructure CreateDirectoryStructure = InfoEntry.DirectoryStructure.Unset;
                InfoEntry.Case Case = InfoEntry.Case.Unset;
                InfoEntry.UmlautAction Umlaute = InfoEntry.UmlautAction.Unset;
                for (int i = 0; i < lstFiles.SelectedItems.Count; i++) {
                    ListViewItem lvi = lstFiles.SelectedItems[i];
                    if (i == 0) {
                        CreateDirectoryStructure = InfoEntryManager.Instance[((int)lvi.Tag)].CreateDirectoryStructure;
                        Case = InfoEntryManager.Instance[((int)lvi.Tag)].Casing;
                        Umlaute = InfoEntryManager.Instance[((int)lvi.Tag)].UmlautUsage;
                    }
                    else {
                        if (CreateDirectoryStructure != InfoEntryManager.Instance[((int)lvi.Tag)].CreateDirectoryStructure) {
                            CreateDirectoryStructure = InfoEntry.DirectoryStructure.Unset;
                        }
                        if (Case != InfoEntryManager.Instance[((int)lvi.Tag)].Casing) {
                            Case = InfoEntry.Case.Unset;
                        }
                        if (Umlaute != InfoEntryManager.Instance[((int)lvi.Tag)].UmlautUsage) {
                            Umlaute = InfoEntry.UmlautAction.Unset;
                        }
                    }
                }
                if (CreateDirectoryStructure == InfoEntry.DirectoryStructure.CreateDirectoryStructure) {
                    createDirectoryStructureToolStripMenuItem1.Checked = true;
                }
                else if (CreateDirectoryStructure == InfoEntry.DirectoryStructure.NoDirectoryStructure) {
                    dontCreateDirectoryStructureToolStripMenuItem.Checked = true;
                }
                if (Case == InfoEntry.Case.Large) {
                    largeToolStripMenuItem.Checked = true;
                }
                else if (Case == InfoEntry.Case.small) {
                    smallToolStripMenuItem.Checked = true;
                }
                else if (Case == InfoEntry.Case.Ignore) {
                    igNorEToolStripMenuItem.Checked = true;
                }
                else if (Case == InfoEntry.Case.CAPSLOCK) {
                    cAPSLOCKToolStripMenuItem.Checked = true;
                }
                if (Umlaute == InfoEntry.UmlautAction.Use) {
                    useUmlautsToolStripMenuItem.Checked = true;
                }
                else if (Umlaute == InfoEntry.UmlautAction.Dont_Use) {
                    dontUseUmlautsToolStripMenuItem.Checked = true;
                }
                else if (Umlaute == InfoEntry.UmlautAction.Ignore) {
                    useProvidedNamesToolStripMenuItem.Checked = true;
                }
                for (int i = 0; i < lstFiles.SelectedItems.Count; i++) {
                    ListViewItem lvi = lstFiles.SelectedItems[i];
                    if (InfoEntryManager.Instance[((int)lvi.Tag)].Filename != "")
                        OldFilename = true;
                    if (InfoEntryManager.Instance[((int)lvi.Tag)].Filepath != "")
                        OldPath = true;
                    if (InfoEntryManager.Instance[((int)lvi.Tag)].Name != "")
                        Name = true;
                    if (InfoEntryManager.Instance[((int)lvi.Tag)].Destination != "")
                        Destination = true;
                    if (InfoEntryManager.Instance[((int)lvi.Tag)].NewFileName != "")
                        NewFilename = true;
                }
                originalNameToolStripMenuItem.Visible = OldFilename;
                pathOrigNameToolStripMenuItem.Visible = OldPath && OldFilename;
                titleToolStripMenuItem.Visible = Name;
                newFileNameToolStripMenuItem.Visible = NewFilename;
                destinationNewFileNameToolStripMenuItem.Visible = Destination && NewFilename;
            }
            else {
                copyToolStripMenuItem.Visible = false;
            }
        }

        //Select all list items
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) {
            lstFiles.SelectedIndices.Clear();
            foreach (ListViewItem lvi in lstFiles.Items) {
                lstFiles.SelectedIndices.Add(lvi.Index);
            }
        }

        //Invert file list selection
        private void invertSelectionToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (ListViewItem lvi in lstFiles.Items) {
                if (lstFiles.SelectedIndices.Contains(lvi.Index)) {
                    lstFiles.SelectedIndices.Remove(lvi.Index);
                }
                else {
                    lstFiles.SelectedIndices.Add(lvi.Index);
                }
            }
        }

        //Check all list boxes
        private void checkAllToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (InfoEntry ie in InfoEntryManager.Instance) {
                ie.ProcessingRequested = true;
            }
            SyncAllItems(false);
        }

        //Uncheck all list boxes
        private void uncheckAllToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (InfoEntry ie in InfoEntryManager.Instance) {
                ie.ProcessingRequested = false;
            }
            SyncAllItems(false);
        }

        //Invert check status of Selected list boxes
        private void invertCheckToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (InfoEntry ie in InfoEntryManager.Instance) {
                ie.ProcessingRequested = !ie.ProcessingRequested;
            }
            SyncAllItems(false);
        }

        //Filter function to select files by keyword
        private void selectByKeywordToolStripMenuItem_Click(object sender, EventArgs e) {
            Filter f = new Filter("");
            if (f.ShowDialog() == DialogResult.OK) {
                lstFiles.SelectedIndices.Clear();
                foreach (ListViewItem lvi in lstFiles.Items) {
                    if (lvi.Text.ToLower().Contains(f.result.ToLower())) {
                        lstFiles.SelectedIndices.Add(lvi.Index);
                    }
                }
            }

        }

        //Set season property for selected items
        private void setSeasonToolStripMenuItem_Click(object sender, EventArgs e) {
            //yes we are smart and guess the season from existing ones
            int sum = 0;
            int count = 0;
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                if (InfoEntryManager.Instance[(int)lvi.Tag].Season != -1) {
                    sum += InfoEntryManager.Instance[(int)lvi.Tag].Season;
                    count++;
                }
            }
            int EstimatedSeason = (int)Math.Round(((float)sum / (float)count));
            EnterSeason es = new EnterSeason(EstimatedSeason);
            if (es.ShowDialog() == DialogResult.OK) {
                string basepath = Helper.ReadProperty(Config.LastDirectory);
                bool createdirectorystructure = (Helper.ReadInt(Config.CreateDirectoryStructure) > 0);
                bool UseSeasonDir = (Helper.ReadInt(Config.UseSeasonSubDir) > 0);
                foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                    int season = es.season;
                    InfoEntryManager.Instance[(int)lvi.Tag].Season = season;
                    //SetupRelation((int)lvi.Tag);
                    //SetDestinationPath(InfoEntryManager.Instance[(int)lvi.Tag], basepath, createdirectorystructure, UseSeasonDir);
                    if (InfoEntryManager.Instance[(int)lvi.Tag].Destination != "") {
                        InfoEntryManager.Instance[(int)lvi.Tag].ProcessingRequested = true;
                    }
                }
                FillListView();
            }
        }

        //Set episodes for selected items to a range
        private void setEpisodesFromtoToolStripMenuItem_Click(object sender, EventArgs e) {
            SetEpisodes se = new SetEpisodes(lstFiles.SelectedIndices.Count);
            if (se.ShowDialog() == DialogResult.OK) {
                for (int i = 0; i < lstFiles.SelectedIndices.Count; i++) {
                    InfoEntryManager.Instance[((int)lstFiles.SelectedItems[i].Tag)].Episode = (i + se.From);
                    lstFiles.SelectedItems[i].SubItems[3].Text = (i + se.From).ToString();
                }
            }



        }

        //Refresh file list
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e) {
            UpdateList(true);
        }

        //Delete file
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Delete selected files?", "Delete selected files?", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            List<InfoEntry> lie = new List<InfoEntry>();
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                lie.Add(InfoEntryManager.Instance[(int)lvi.Tag]);
            }
            foreach (InfoEntry ie in lie) {
                try {
                    File.Delete(ie.Filepath + Path.DirectorySeparatorChar + ie.Filename);
                    InfoEntryManager.Instance.Remove(ie);
                }
                catch (Exception ex) {
                    Logger.Instance.LogMessage("Error deleting file: " + ex.Message, LogLevel.ERROR);
                }
            }
            FillListView();
        }

        //Open file
        private void viewToolStripMenuItem_Click(object sender, EventArgs e) {
            InfoEntry ie = InfoEntryManager.Instance.GetVideo(InfoEntryManager.Instance[(int)lstFiles.SelectedItems[0].Tag]);
            string VideoPath = ie.Filepath + Path.DirectorySeparatorChar + ie.Filename;
            try {
                Process myProc = Process.Start(VideoPath);
            }
            catch (Exception ex) {
                Logger.Instance.LogMessage("Couldn't open " + VideoPath + ":" + ex.Message, LogLevel.ERROR);
            }
        }

        //Edit subtitle
        private void editSubtitleToolStripMenuItem_Click(object sender, EventArgs e) {
            InfoEntry sub = InfoEntryManager.Instance.GetSubtitle(InfoEntryManager.Instance[((int)lstFiles.SelectedItems[0].Tag)]);
            InfoEntry video = InfoEntryManager.Instance.GetVideo(InfoEntryManager.Instance[((int)lstFiles.SelectedItems[0].Tag)]);
            if (sub != null) {
                string path = sub.Filepath + Path.DirectorySeparatorChar + sub.Filename;
                string videopath = "";
                if (video != null) {
                    videopath = video.Filepath + Path.DirectorySeparatorChar + video.Filename;
                }
                EditSubtitles es = new EditSubtitles(path, videopath);
                es.ShowDialog();
            }
        }

        //Set Destination
        private void setDestinationToolStripMenuItem_Click(object sender, EventArgs e) {
            InputBox ib = new InputBox("Set Destination", "Set Destination directory for selected files", Helper.ReadProperty(Config.LastDirectory), InputBox.BrowseType.Folder, true);
            if (ib.ShowDialog(this) == DialogResult.OK) {
                foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                    string destination = ib.input;
                    InfoEntryManager.Instance[(int)lvi.Tag].Destination = destination;
                    lvi.SubItems[6].Text = destination;
                }
            }
        }
        #endregion
        #region misc


      



        /// <summary>
        /// Deletes all empty folders recursively, ignoring files from IgnoredFiles list
        /// </summary>
        /// <param name="path">Path from which to delete folders</param>
        /// <param name="IgnoredFiletypes">List of extensions(without '.' at start) of filetypes which may be deleted</param>
        public void DeleteAllEmptyFolders(string path, List<string> IgnoredFiletypes) {
            bool delete = true;
            string[] folders = Directory.GetDirectories(path);
            if (folders.GetLength(0) > 0) {
                foreach (string folder in folders) {
                    DeleteAllEmptyFolders(folder, IgnoredFiletypes);
                }
            }
            folders = Directory.GetDirectories(path);
            if (folders.Length != 0) {
                return;
            }
            string[] files = Directory.GetFiles(path);
            if (files.Length != 0) {
                foreach (string s in files) {

                    if (Path.GetExtension(s) == "" || !IgnoredFiletypes.Contains(Path.GetExtension(s).Substring(1))) {
                        delete = false;
                        break;
                    }
                }
            }
            if (delete) {
                try {
                    Directory.Delete(path, true);
                }
                catch (Exception ex) {
                    Logger.Instance.LogMessage("Couldn't delete " + path + ": " + ex.Message, LogLevel.ERROR);
                }
            }
        }

        /// <summary>
        /// Syncs an item between data and GUI, will bitch if item doesn't exist already though. To create new items, add one to the data manually and use FillListView()
        /// </summary>
        /// <param name="item">Item to be synchronized. The index which is used here is that of the InfoEntry list in info class. ListViewItems Use Tag Property.</param>
        /// <param name="direction">Direction in which synching should occur. 0 = data->GUI, 1 = GUI->data</param>
        private void SyncItem(int item, bool direction) {
            ListViewItem lvi = null;
            foreach (ListViewItem lv in lstFiles.Items) {
                if ((int)lv.Tag == item) {
                    lvi = lv;
                    break;
                }
            }
            if (lvi == null) {
                Logger.Instance.LogMessage("Synching between data and gui failed because item doesn't exist in GUI.", LogLevel.ERROR);
                return;
            }
            InfoEntry ie = InfoEntryManager.Instance[item];
            if (direction == false) {
                lvi.SubItems[0].Text = ie.Filename;
                lvi.SubItems[1].Text = ie.Filepath;
                lvi.SubItems[2].Text = ie.Season.ToString();
                lvi.SubItems[3].Text = ie.Episode.ToString();
                lvi.SubItems[4].Text = ie.Name;
                lvi.SubItems[5].Text = ie.NewFileName;
                lvi.SubItems[6].Text = ie.Destination;
                lvi.SubItems[7].Text = ie.Showname;
                lvi.Checked = ie.ProcessingRequested;
            }
            else {
                ie.Filename = lvi.SubItems[0].Text;
                ie.Filepath = lvi.SubItems[1].Text;
                try {
                    ie.Season = Int32.Parse(lvi.SubItems[2].Text);
                }
                catch {
                    ie.Season = -1;
                }
                try {
                    ie.Episode = Int32.Parse(lvi.SubItems[3].Text);
                }
                catch {
                    ie.Episode = -1;
                }

                ie.Name = lvi.SubItems[4].Text;
                ie.NewFileName = lvi.SubItems[5].Text;
                ie.Destination = lvi.SubItems[6].Text;
                ie.Showname = lvi.SubItems[7].Text;
                ie.ProcessingRequested = lvi.Checked;
            }

            Colorize(lvi);
        }

        /// <summary>
        /// Fills list view control with info data
        /// </summary>
        private void FillListView() {
            // TODO: show at least a progressbar while adding items, user can't see anything but processor utilization will be very high
            lstFiles.Items.Clear();
            for (int i = 0; i < InfoEntryManager.Instance.Count; i++) {
                InfoEntry ie = InfoEntryManager.Instance[i];
                ListViewItem lvi = new ListViewItem(ie.Filename);
                lvi.Tag = i;
                lvi.SubItems.Add(ie.Filepath);
                lvi.SubItems.Add(ie.Season.ToString());
                lvi.SubItems.Add(ie.Episode.ToString());
                lvi.SubItems.Add(ie.Name);
                lvi.SubItems.Add(ie.NewFileName);
                lvi.SubItems.Add(ie.Destination);
                lvi.SubItems.Add(ie.Showname);
                lvi.Checked = ie.ProcessingRequested;
                lstFiles.Items.Add(lvi);
            }
            Colorize();
            lstFiles.Sort();
            lstFiles.Refresh();
        }

        /// <summary>
        /// colorizes the file list
        /// </summary>
        private void Colorize() {
            foreach (ListViewItem lvi1 in lstFiles.Items) {
                Colorize(lvi1);
            }
        }

        /// <summary>
        /// Colorizes single list item
        /// </summary>
        /// <param name="lvi">List item to be colorized</param>
        private void Colorize(ListViewItem lvi) {
            if ((lvi.SubItems[5].Text == "" && lvi.SubItems[1].Text == lvi.SubItems[6].Text && lvi.SubItems[6].Text != "") || !lvi.Checked) {
                lvi.ForeColor = Color.Gray;
            }
            else {
                lvi.ForeColor = Color.Black;
            }
            if (lvi.SubItems[5].Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
                lvi.BackColor = Color.Yellow;
            }
            else {
                lvi.BackColor = Color.White;
            }
            foreach (ListViewItem lvi2 in lstFiles.Items) {
                if (lvi != lvi2) {
                    if (lvi.SubItems[1].Text == lvi2.SubItems[1].Text && lvi.SubItems[5].Text == lvi2.SubItems[5].Text && lvi.SubItems[5].Text != "") {
                        lvi.BackColor = Color.IndianRed;
                    }
                    else if (lvi.BackColor != Color.Yellow) {
                        lvi.BackColor = Color.White;
                    }
                }
            }
        }

        /// <summary>
        /// Gets focussed control
        /// </summary>
        /// <param name="controls">Array of controls in which to search for</param>
        /// <returns>focussed control or null</returns>
        private Control getFocused(Control.ControlCollection controls) {
            foreach (Control c in controls) {
                if (c.Focused) {
                    // Return the focused control
                    return c;
                }
                else if (c.ContainsFocus) {
                    // If the focus is contained inside a control's children
                    // return the child
                    return getFocused(c.Controls);
                }
            }
            // No control on the form has focus
            return null;
        }
        #endregion


        /*bool UsedDropDown = false;
        private void cbTitle_DropDownClosed(object sender, EventArgs e)
        {
            UsedDropDown = true;
        }

        private void cbTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UsedDropDown)
            {
                UsedDropDown = !UsedDropDown;
                SetNewTitle(cbTitle.Text);
            }
        }*/

        private void originalNameToolStripMenuItem_Click(object sender, EventArgs e) {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                clipboard += InfoEntryManager.Instance[((int)lvi.Tag)].Filename + Environment.NewLine;
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - Environment.NewLine.Length, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void pathOrigNameToolStripMenuItem_Click(object sender, EventArgs e) {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                clipboard += InfoEntryManager.Instance[((int)lvi.Tag)].Filepath + Path.DirectorySeparatorChar + InfoEntryManager.Instance[((int)lvi.Tag)].Filename + Environment.NewLine;
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - Environment.NewLine.Length, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void titleToolStripMenuItem_Click(object sender, EventArgs e) {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                clipboard += InfoEntryManager.Instance[((int)lvi.Tag)].Name + Environment.NewLine;
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - Environment.NewLine.Length, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void newFileNameToolStripMenuItem_Click(object sender, EventArgs e) {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                clipboard += InfoEntryManager.Instance[((int)lvi.Tag)].NewFileName + Environment.NewLine;
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - Environment.NewLine.Length, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void destinationNewFileNameToolStripMenuItem_Click(object sender, EventArgs e) {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                if (InfoEntryManager.Instance[((int)lvi.Tag)].Destination != "" && InfoEntryManager.Instance[((int)lvi.Tag)].NewFileName != "") {
                    clipboard += InfoEntryManager.Instance[((int)lvi.Tag)].Destination + Path.DirectorySeparatorChar + InfoEntryManager.Instance[((int)lvi.Tag)].NewFileName + Environment.NewLine;
                }
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - Environment.NewLine.Length, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void operationToolStripMenuItem_Click(object sender, EventArgs e) {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                InfoEntry ie = InfoEntryManager.Instance[((int)lvi.Tag)];
                if (ie.Destination != "") {
                    clipboard += ie.Filename + " --> " + ie.Destination + Path.DirectorySeparatorChar + ie.NewFileName + Environment.NewLine;
                }
                else {
                    clipboard += ie.Filename + " --> " + ie.NewFileName + Environment.NewLine;
                }
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - Environment.NewLine.Length, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            if (Directory.Exists(txtPath.Text)) {
                Process myProc = Process.Start(txtPath.Text);
            }
        }

        private void lstFiles_DragDrop(object sender, DragEventArgs e) {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (s.Length == 1 && Directory.Exists(s[0])) {
                InfoEntryManager.Instance.SetPath(s[0]);
            }
        }

        private void lstFiles_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                if (s.Length == 1 && Directory.Exists(s[0])) {
                    e.Effect = DragDropEffects.Move;
                }
            }
        }



        private void removeTagsToolStripMenuItem_Click(object sender, EventArgs e) {
            RemoveVideoTags();
        }

        /// <summary>
        /// Syncs all items
        /// </summary>
        /// <param name="direction">Direction in which synching should occur. false = data->GUI, true = GUI->data</param>
        private void SyncAllItems(bool direction) {
            foreach (ListViewItem lvi in lstFiles.Items) {
                SyncItem((int)lvi.Tag, direction);
            }
        }

        private void replaceInPathToolStripMenuItem_Click(object sender, EventArgs e) {
            ReplaceWindow rw = new ReplaceWindow(this);
            rw.Show();
            rw.TopMost = true;
        }




        /// <summary>
        /// Replaces strings from various fields in selected files
        /// </summary>
        /// <param name="SearchString">String to look for, may contain parameters</param>
        /// <param name="ReplaceString">Replace string, may contain parameters</param>
        /// <param name="Source">Field from which the source string is taken</param>
        public void Replace(string SearchString, string ReplaceString, string Source) {
            int count = 0;
            string title = Helper.ReadProperties(Config.LastTitles)[0];
            string basedir = Helper.ReadProperty(Config.LastDirectory);
            string destination = "Filename";
            if (Source.Contains("Path"))
                destination = "Path";
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                InfoEntry ie = InfoEntryManager.Instance[(int)lvi.Tag];
                string source = "";

                string LocalSearchString = SearchString;
                string LocalReplaceString = ReplaceString;
                //aquire source string
                switch (Source) {
                    case "Original Filename":
                        source = ie.Filename;
                        break;
                    case "Original Path":
                        source = ie.Filepath;
                        break;
                    case "Destination Filename":
                        source = ie.NewFileName;
                        break;
                    case "Destination Path":
                        source = ie.Destination;
                        break;
                }
                //Insert parameter values
                LocalSearchString = LocalSearchString.Replace("%OF", ie.Filename);
                LocalSearchString = LocalSearchString.Replace("%DF", ie.NewFileName);
                LocalSearchString = LocalSearchString.Replace("%OP", ie.Filepath);
                LocalSearchString = LocalSearchString.Replace("%DP", ie.Destination);
                LocalSearchString = LocalSearchString.Replace("%T", title);
                LocalSearchString = LocalSearchString.Replace("%N", ie.Name);
                LocalSearchString = LocalSearchString.Replace("%E", ie.Episode.ToString());
                LocalSearchString = LocalSearchString.Replace("%s", ie.Season.ToString());
                LocalSearchString = LocalSearchString.Replace("%BD", basedir);
                LocalSearchString = LocalSearchString.Replace("%S", ie.Season.ToString("00"));
                LocalReplaceString = LocalReplaceString.Replace("%OF", ie.Filename);
                LocalReplaceString = LocalReplaceString.Replace("%DF", ie.NewFileName);
                LocalReplaceString = LocalReplaceString.Replace("%OP", ie.Filepath);
                LocalReplaceString = LocalReplaceString.Replace("%DP", ie.Destination);
                LocalReplaceString = LocalReplaceString.Replace("%T", title);
                LocalReplaceString = LocalReplaceString.Replace("%N", ie.Name);
                LocalReplaceString = LocalReplaceString.Replace("%E", ie.Episode.ToString());
                LocalReplaceString = LocalReplaceString.Replace("%s", ie.Season.ToString());
                LocalReplaceString = LocalReplaceString.Replace("%S", ie.Season.ToString("00"));

                //see if replace will be done for count var
                if (source.Contains(SearchString))
                    count++;
                //do the replace
                source = source.Replace(LocalSearchString, LocalReplaceString);
                if (destination == "Filename") {
                    ie.NewFileName = source;
                }
                else if (destination == "Path") {
                    ie.Destination = source;
                }

                //mark files for processing
                ie.ProcessingRequested = true;
                SyncItem((int)lvi.Tag, false);
            }
            if (count > 0) {
                Logger.Instance.LogMessage(SearchString + " was replaced with " + ReplaceString + " in " + count + " fields.", LogLevel.INFO);
            }
            else {
                Logger.Instance.LogMessage(SearchString + " was not found in any of the selected files.", LogLevel.INFO);
            }
        }

        private void byNameToolStripMenuItem_Click(object sender, EventArgs e) {
            List<string> names = new List<string>();
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                string Showname = InfoEntryManager.Instance[(int)lvi.Tag].Showname;
                if (!names.Contains(Showname)) {
                    names.Add(Showname);
                }
            }
            List<InfoEntry> similar = new List<InfoEntry>();
            foreach (string str in names) {
                similar.AddRange(InfoEntryManager.Instance.FindSimilarByName(str));
            }
            foreach (ListViewItem lvi in lstFiles.Items) {
                if (similar.Contains(InfoEntryManager.Instance[(int)lvi.Tag])) {
                    lvi.Selected = true;
                }
                else {
                    lvi.Selected = false;
                }
            }
        }

        private void byPathToolStripMenuItem_Click(object sender, EventArgs e) {
            List<string> paths = new List<string>();
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                string path = InfoEntryManager.Instance[(int)lvi.Tag].Filepath;
                if (!paths.Contains(path)) {
                    paths.Add(path);
                }
            }
            List<InfoEntry> similar = new List<InfoEntry>();
            foreach (string str in paths) {
                similar.AddRange(InfoEntryManager.Instance.FindSimilarByName(str));
            }
            foreach (ListViewItem lvi in lstFiles.Items) {
                if (similar.Contains(InfoEntryManager.Instance[(int)lvi.Tag])) {
                    lvi.Selected = true;
                }
                else {
                    lvi.Selected = false;
                }
            }
        }

        private void createDirectoryStructureToolStripMenuItem1_Click(object sender, EventArgs e) {
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                InfoEntryManager.Instance[(int)lvi.Tag].CreateDirectoryStructure = InfoEntry.DirectoryStructure.CreateDirectoryStructure;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            dontCreateDirectoryStructureToolStripMenuItem.Checked = false;
        }

        private void dontCreateDirectoryStructureToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                InfoEntryManager.Instance[(int)lvi.Tag].CreateDirectoryStructure = InfoEntry.DirectoryStructure.NoDirectoryStructure;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            createDirectoryStructureToolStripMenuItem.Checked = false;
        }

        private void useUmlautsToolStripMenuItem1_Click(object sender, EventArgs e) {
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                InfoEntryManager.Instance[(int)lvi.Tag].UmlautUsage = InfoEntry.UmlautAction.Use;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            dontUseUmlautsToolStripMenuItem.Checked = false;
            useProvidedNamesToolStripMenuItem.Checked = false;
        }

        private void dontUseUmlautsToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                InfoEntryManager.Instance[(int)lvi.Tag].UmlautUsage = InfoEntry.UmlautAction.Dont_Use;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            useUmlautsToolStripMenuItem.Checked = false;
            useProvidedNamesToolStripMenuItem.Checked = false;
        }

        private void useProvidedNamesToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                InfoEntryManager.Instance[(int)lvi.Tag].UmlautUsage = InfoEntry.UmlautAction.Ignore;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            useUmlautsToolStripMenuItem.Checked = false;
            dontUseUmlautsToolStripMenuItem.Checked = false;
        }

        private void largeToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                InfoEntryManager.Instance[(int)lvi.Tag].Casing = InfoEntry.Case.Large;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            smallToolStripMenuItem.Checked = false;
            igNorEToolStripMenuItem.Checked = false;
            cAPSLOCKToolStripMenuItem.Checked = false;
        }

        private void smallToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                InfoEntryManager.Instance[(int)lvi.Tag].Casing = InfoEntry.Case.small;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            largeToolStripMenuItem.Checked = false;
            igNorEToolStripMenuItem.Checked = false;
            cAPSLOCKToolStripMenuItem.Checked = false;
        }

        private void igNorEToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                InfoEntryManager.Instance[(int)lvi.Tag].Casing = InfoEntry.Case.Ignore;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            smallToolStripMenuItem.Checked = false;
            largeToolStripMenuItem.Checked = false;
            cAPSLOCKToolStripMenuItem.Checked = false;
        }

        private void cAPSLOCKToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                InfoEntryManager.Instance[(int)lvi.Tag].Casing = InfoEntry.Case.CAPSLOCK;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            smallToolStripMenuItem.Checked = false;
            igNorEToolStripMenuItem.Checked = false;
            largeToolStripMenuItem.Checked = false;
        }

        private void setShownameToolStripMenuItem_Click(object sender, EventArgs e) {
            Dictionary<string, int> ht = new Dictionary<string, int>();
            foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                if (!ht.ContainsKey(InfoEntryManager.Instance[(int)lvi.Tag].Showname)) {
                    ht.Add(InfoEntryManager.Instance[(int)lvi.Tag].Showname, 1);
                }
                else {
                    ht[InfoEntryManager.Instance[(int)lvi.Tag].Showname] += 1;
                }
            }
            int max = 0;
            string Showname = "";
            foreach (KeyValuePair<string, int> pair in ht) {
                if (pair.Value > max) {
                    Showname = pair.Key;
                }
            }
            EnterShowname es = new EnterShowname(Showname);
            if (es.ShowDialog() == DialogResult.OK) {
                foreach (ListViewItem lvi in lstFiles.SelectedItems) {
                    InfoEntryManager.Instance[(int)lvi.Tag].Showname = es.SelectedName;
                    SyncItem((int)lvi.Tag, false);
                }
            }
        }

        private void regexTesterToolStripMenuItem_Click(object sender, EventArgs e) {
            RegexTester rt = new RegexTester();
            rt.Show();
        }



        #region Functioncs remaining in Form1
        private void UpdateList(bool clear) {
            DataGenerator.UpdateList(clear);
            FillListView();

            //also update some gui elements for the sake of it
            txtTarget.Text = Helper.ReadProperty(Config.TargetPattern);
            txtPath.Text = Helper.ReadProperty(Config.LastDirectory);
            string LastProvider = Helper.ReadProperty(Config.LastProvider);
            if (LastProvider == null)
                LastProvider = "";
            cbProviders.SelectedIndex = Math.Max(0, cbProviders.Items.IndexOf(LastProvider));
            string LastSubProvider = Helper.ReadProperty(Config.LastSubProvider);
            if (LastSubProvider == null)
                LastSubProvider = "";
            cbSubs.SelectedIndex = Math.Max(0, cbSubs.Items.IndexOf(LastSubProvider));
        }
        #endregion        
        
    }
}

