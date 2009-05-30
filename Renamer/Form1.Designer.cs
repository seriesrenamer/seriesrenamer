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

namespace Renamer
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.scContainer = new System.Windows.Forms.SplitContainer();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnRename = new System.Windows.Forms.Button();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.cbSubs = new System.Windows.Forms.ComboBox();
            this.lblSubFrom = new System.Windows.Forms.Label();
            this.lblTargetFilename = new System.Windows.Forms.Label();
            this.btnSubs = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.cbProviders = new System.Windows.Forms.ComboBox();
            this.lblTitlesFrom = new System.Windows.Forms.Label();
            this.btnTitles = new System.Windows.Forms.Button();
            this.lstFiles = new ListViewEx.ListViewEx();
            this.cOldName = new System.Windows.Forms.ColumnHeader();
            this.cPath = new System.Windows.Forms.ColumnHeader();
            this.cSeason = new System.Windows.Forms.ColumnHeader();
            this.cEpID = new System.Windows.Forms.ColumnHeader();
            this.cTitle = new System.Windows.Forms.ColumnHeader();
            this.cNewName = new System.Windows.Forms.ColumnHeader();
            this.cDestination = new System.Windows.Forms.ColumnHeader();
            this.cShowname = new System.Windows.Forms.ColumnHeader();
            this.contextFiles = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSubtitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.renamingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createDirectoryStructureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createDirectoryStructureToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.dontCreateDirectoryStructureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useUmlautsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useUmlautsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.dontUseUmlautsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useProvidedNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.caseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.igNorEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cAPSLOCKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceInPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectByKeywordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectSimilarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.setShownameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setSeasonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setEpisodesFromtoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setDestinationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.originalNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pathOrigNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.titleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.destinationNewFileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.operationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regexTesterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnConfig = new System.Windows.Forms.Button();
            this.btnPath = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lblFolder = new System.Windows.Forms.Label();
            this.txtEdit = new System.Windows.Forms.TextBox();
            this.numEdit = new System.Windows.Forms.NumericUpDown();
            this.comEdit = new System.Windows.Forms.ComboBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.fbdPath = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.scContainer.Panel1.SuspendLayout();
            this.scContainer.Panel2.SuspendLayout();
            this.scContainer.SuspendLayout();
            this.contextFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // scContainer
            // 
            this.scContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.scContainer.Location = new System.Drawing.Point(0, 0);
            this.scContainer.Name = "scContainer";
            this.scContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scContainer.Panel1
            // 
            this.scContainer.Panel1.Controls.Add(this.btnOpen);
            this.scContainer.Panel1.Controls.Add(this.btnRename);
            this.scContainer.Panel1.Controls.Add(this.txtTarget);
            this.scContainer.Panel1.Controls.Add(this.cbSubs);
            this.scContainer.Panel1.Controls.Add(this.lblSubFrom);
            this.scContainer.Panel1.Controls.Add(this.lblTargetFilename);
            this.scContainer.Panel1.Controls.Add(this.btnSubs);
            this.scContainer.Panel1.Controls.Add(this.btnAbout);
            this.scContainer.Panel1.Controls.Add(this.cbProviders);
            this.scContainer.Panel1.Controls.Add(this.lblTitlesFrom);
            this.scContainer.Panel1.Controls.Add(this.btnTitles);
            this.scContainer.Panel1.Controls.Add(this.lstFiles);
            this.scContainer.Panel1.Controls.Add(this.btnConfig);
            this.scContainer.Panel1.Controls.Add(this.btnPath);
            this.scContainer.Panel1.Controls.Add(this.txtPath);
            this.scContainer.Panel1.Controls.Add(this.lblFolder);
            this.scContainer.Panel1.Controls.Add(this.txtEdit);
            this.scContainer.Panel1.Controls.Add(this.numEdit);
            this.scContainer.Panel1.Controls.Add(this.comEdit);
            this.scContainer.Panel1MinSize = 300;
            // 
            // scContainer.Panel2
            // 
            this.scContainer.Panel2.Controls.Add(this.txtLog);
            this.scContainer.Panel2.Controls.Add(this.rtbLog);
            this.scContainer.Panel2MinSize = 100;
            this.scContainer.Size = new System.Drawing.Size(1016, 561);
            this.scContainer.SplitterDistance = 453;
            this.scContainer.TabIndex = 0;
            this.scContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.scContainer_MouseDown);
            this.scContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.scContainer_MouseUp);
            // 
            // btnOpen
            // 
            this.btnOpen.Image = global::Renamer.Properties.Resources.Browse;
            this.btnOpen.Location = new System.Drawing.Point(524, 4);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(28, 23);
            this.btnOpen.TabIndex = 15;
            this.toolTip.SetToolTip(this.btnOpen, "Open Folder");
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnRename
            // 
            this.btnRename.AutoSize = true;
            this.btnRename.Location = new System.Drawing.Point(743, 33);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(85, 23);
            this.btnRename.TabIndex = 11;
            this.btnRename.Text = "Rename !";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // txtTarget
            // 
            this.txtTarget.Location = new System.Drawing.Point(743, 6);
            this.txtTarget.MinimumSize = new System.Drawing.Size(165, 20);
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(176, 20);
            this.txtTarget.TabIndex = 9;
            this.txtTarget.Text = "S%sE%E - %N";
            this.toolTip.SetToolTip(this.txtTarget, "Valid variables: %S - Season, %E - Episode, %T - Title, %N - Name, Small letter=1" +
                    " digit");
            this.txtTarget.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTarget_KeyDown);
            this.txtTarget.Leave += new System.EventHandler(this.txtTarget_Leave);
            // 
            // cbSubs
            // 
            this.cbSubs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSubs.FormattingEnabled = true;
            this.cbSubs.Location = new System.Drawing.Point(447, 35);
            this.cbSubs.Name = "cbSubs";
            this.cbSubs.Size = new System.Drawing.Size(196, 21);
            this.cbSubs.TabIndex = 13;
            this.cbSubs.SelectedIndexChanged += new System.EventHandler(this.cbSubs_SelectedIndexChanged);
            // 
            // lblSubFrom
            // 
            this.lblSubFrom.AutoSize = true;
            this.lblSubFrom.Location = new System.Drawing.Point(353, 38);
            this.lblSubFrom.Name = "lblSubFrom";
            this.lblSubFrom.Size = new System.Drawing.Size(88, 13);
            this.lblSubFrom.TabIndex = 12;
            this.lblSubFrom.Text = "Get subtitles from";
            // 
            // lblTargetFilename
            // 
            this.lblTargetFilename.AutoSize = true;
            this.lblTargetFilename.Location = new System.Drawing.Point(649, 9);
            this.lblTargetFilename.Name = "lblTargetFilename";
            this.lblTargetFilename.Size = new System.Drawing.Size(80, 13);
            this.lblTargetFilename.TabIndex = 8;
            this.lblTargetFilename.Text = "Target filename";
            this.toolTip.SetToolTip(this.lblTargetFilename, "Valid variables: %S - Season, %E - Episode, %T - Title, %N - Name, Small letter=1" +
                    " digit");
            // 
            // btnSubs
            // 
            this.btnSubs.Location = new System.Drawing.Point(652, 33);
            this.btnSubs.Name = "btnSubs";
            this.btnSubs.Size = new System.Drawing.Size(85, 23);
            this.btnSubs.TabIndex = 11;
            this.btnSubs.Text = "Get Subtitles !";
            this.btnSubs.UseVisualStyleBackColor = true;
            this.btnSubs.Click += new System.EventHandler(this.btnSubs_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(834, 33);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(85, 23);
            this.btnAbout.TabIndex = 10;
            this.btnAbout.Text = "About...";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // cbProviders
            // 
            this.cbProviders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProviders.FormattingEnabled = true;
            this.cbProviders.Location = new System.Drawing.Point(89, 35);
            this.cbProviders.Name = "cbProviders";
            this.cbProviders.Size = new System.Drawing.Size(167, 21);
            this.cbProviders.TabIndex = 9;
            this.cbProviders.SelectedIndexChanged += new System.EventHandler(this.cbProviders_SelectedIndexChanged);
            // 
            // lblTitlesFrom
            // 
            this.lblTitlesFrom.AutoSize = true;
            this.lblTitlesFrom.Location = new System.Drawing.Point(12, 38);
            this.lblTitlesFrom.Name = "lblTitlesFrom";
            this.lblTitlesFrom.Size = new System.Drawing.Size(71, 13);
            this.lblTitlesFrom.TabIndex = 8;
            this.lblTitlesFrom.Text = "Get titles from";
            // 
            // btnTitles
            // 
            this.btnTitles.Location = new System.Drawing.Point(262, 33);
            this.btnTitles.Name = "btnTitles";
            this.btnTitles.Size = new System.Drawing.Size(85, 23);
            this.btnTitles.TabIndex = 5;
            this.btnTitles.Text = "Get Titles !";
            this.btnTitles.UseVisualStyleBackColor = true;
            this.btnTitles.Click += new System.EventHandler(this.btnTitles_Click);
            // 
            // lstFiles
            // 
            this.lstFiles.AllowColumnReorder = true;
            this.lstFiles.AllowDrop = true;
            this.lstFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFiles.CheckBoxes = true;
            this.lstFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cOldName,
            this.cPath,
            this.cSeason,
            this.cEpID,
            this.cTitle,
            this.cNewName,
            this.cDestination,
            this.cShowname});
            this.lstFiles.ContextMenuStrip = this.contextFiles;
            this.lstFiles.DoubleClickActivation = true;
            this.lstFiles.FullRowSelect = true;
            this.lstFiles.Location = new System.Drawing.Point(15, 59);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(993, 391);
            this.lstFiles.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstFiles.TabIndex = 4;
            this.toolTip.SetToolTip(this.lstFiles, "Double click on an entry in Season, Episode, Title or New File Name column to edi" +
                    "t it");
            this.lstFiles.UseCompatibleStateImageBehavior = false;
            this.lstFiles.View = System.Windows.Forms.View.Details;
            this.lstFiles.SubItemClicked += new ListViewEx.SubItemEventHandler(this.lstFiles_SubItemClicked);
            this.lstFiles.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lstFiles_ItemChecked);
            this.lstFiles.DoubleClick += new System.EventHandler(this.lstFiles_DoubleClick);
            this.lstFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstFiles_DragDrop);
            this.lstFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstFiles_ColumnClick);
            this.lstFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstFiles_DragEnter);
            this.lstFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstFiles_KeyDown);
            this.lstFiles.SubItemEndEditing += new ListViewEx.SubItemEndEditingEventHandler(this.lstFiles_SubItemEndEditing);
            // 
            // cOldName
            // 
            this.cOldName.Text = "Old File Name";
            this.cOldName.Width = 171;
            // 
            // cPath
            // 
            this.cPath.Text = "Path";
            this.cPath.Width = 134;
            // 
            // cSeason
            // 
            this.cSeason.DisplayIndex = 3;
            this.cSeason.Text = "Season";
            this.cSeason.Width = 54;
            // 
            // cEpID
            // 
            this.cEpID.DisplayIndex = 4;
            this.cEpID.Text = "Episode";
            this.cEpID.Width = 56;
            // 
            // cTitle
            // 
            this.cTitle.DisplayIndex = 5;
            this.cTitle.Text = "Title";
            this.cTitle.Width = 140;
            // 
            // cNewName
            // 
            this.cNewName.DisplayIndex = 6;
            this.cNewName.Text = "New File Name";
            this.cNewName.Width = 181;
            // 
            // cDestination
            // 
            this.cDestination.DisplayIndex = 7;
            this.cDestination.Text = "Destination";
            this.cDestination.Width = 197;
            // 
            // cShowname
            // 
            this.cShowname.DisplayIndex = 2;
            this.cShowname.Text = "Show Name";
            this.cShowname.Width = 110;
            // 
            // contextFiles
            // 
            this.contextFiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.editSubtitleToolStripMenuItem,
            this.removeTagsToolStripMenuItem,
            this.toolStripSeparator4,
            this.renamingToolStripMenuItem,
            this.selectToolStripMenuItem,
            this.checkToolStripMenuItem,
            this.toolStripSeparator2,
            this.setShownameToolStripMenuItem,
            this.setSeasonToolStripMenuItem,
            this.setEpisodesFromtoToolStripMenuItem,
            this.setDestinationToolStripMenuItem,
            this.toolStripSeparator3,
            this.copyToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.regexTesterToolStripMenuItem});
            this.contextFiles.Name = "contextFiles";
            this.contextFiles.ShowImageMargin = false;
            this.contextFiles.Size = new System.Drawing.Size(241, 352);
            this.contextFiles.Opening += new System.ComponentModel.CancelEventHandler(this.contextFiles_Opening);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.viewToolStripMenuItem.Text = "Open";
            this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
            // 
            // editSubtitleToolStripMenuItem
            // 
            this.editSubtitleToolStripMenuItem.Name = "editSubtitleToolStripMenuItem";
            this.editSubtitleToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.editSubtitleToolStripMenuItem.Text = "Edit Subtitle";
            this.editSubtitleToolStripMenuItem.Click += new System.EventHandler(this.editSubtitleToolStripMenuItem_Click);
            // 
            // removeTagsToolStripMenuItem
            // 
            this.removeTagsToolStripMenuItem.Name = "removeTagsToolStripMenuItem";
            this.removeTagsToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.removeTagsToolStripMenuItem.Text = "Remove Tags";
            this.removeTagsToolStripMenuItem.Click += new System.EventHandler(this.removeTagsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(237, 6);
            // 
            // renamingToolStripMenuItem
            // 
            this.renamingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createDirectoryStructureToolStripMenuItem,
            this.useUmlautsToolStripMenuItem,
            this.caseToolStripMenuItem,
            this.replaceInPathToolStripMenuItem});
            this.renamingToolStripMenuItem.Name = "renamingToolStripMenuItem";
            this.renamingToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.renamingToolStripMenuItem.Text = "Renaming";
            // 
            // createDirectoryStructureToolStripMenuItem
            // 
            this.createDirectoryStructureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createDirectoryStructureToolStripMenuItem1,
            this.dontCreateDirectoryStructureToolStripMenuItem});
            this.createDirectoryStructureToolStripMenuItem.Name = "createDirectoryStructureToolStripMenuItem";
            this.createDirectoryStructureToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.createDirectoryStructureToolStripMenuItem.Text = "Create DirectoryStructure";
            // 
            // createDirectoryStructureToolStripMenuItem1
            // 
            this.createDirectoryStructureToolStripMenuItem1.Name = "createDirectoryStructureToolStripMenuItem1";
            this.createDirectoryStructureToolStripMenuItem1.Size = new System.Drawing.Size(237, 22);
            this.createDirectoryStructureToolStripMenuItem1.Text = "Create directory structure";
            this.createDirectoryStructureToolStripMenuItem1.Click += new System.EventHandler(this.createDirectoryStructureToolStripMenuItem1_Click);
            // 
            // dontCreateDirectoryStructureToolStripMenuItem
            // 
            this.dontCreateDirectoryStructureToolStripMenuItem.Name = "dontCreateDirectoryStructureToolStripMenuItem";
            this.dontCreateDirectoryStructureToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.dontCreateDirectoryStructureToolStripMenuItem.Text = "Don\'t create directory structure";
            this.dontCreateDirectoryStructureToolStripMenuItem.Click += new System.EventHandler(this.dontCreateDirectoryStructureToolStripMenuItem_Click);
            // 
            // useUmlautsToolStripMenuItem
            // 
            this.useUmlautsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.useUmlautsToolStripMenuItem1,
            this.dontUseUmlautsToolStripMenuItem,
            this.useProvidedNamesToolStripMenuItem});
            this.useUmlautsToolStripMenuItem.Name = "useUmlautsToolStripMenuItem";
            this.useUmlautsToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.useUmlautsToolStripMenuItem.Text = "Use umlauts";
            // 
            // useUmlautsToolStripMenuItem1
            // 
            this.useUmlautsToolStripMenuItem1.Name = "useUmlautsToolStripMenuItem1";
            this.useUmlautsToolStripMenuItem1.Size = new System.Drawing.Size(182, 22);
            this.useUmlautsToolStripMenuItem1.Text = "Use umlauts";
            this.useUmlautsToolStripMenuItem1.Click += new System.EventHandler(this.useUmlautsToolStripMenuItem1_Click);
            // 
            // dontUseUmlautsToolStripMenuItem
            // 
            this.dontUseUmlautsToolStripMenuItem.Name = "dontUseUmlautsToolStripMenuItem";
            this.dontUseUmlautsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.dontUseUmlautsToolStripMenuItem.Text = "Don\'t use umlauts";
            this.dontUseUmlautsToolStripMenuItem.Click += new System.EventHandler(this.dontUseUmlautsToolStripMenuItem_Click);
            // 
            // useProvidedNamesToolStripMenuItem
            // 
            this.useProvidedNamesToolStripMenuItem.Name = "useProvidedNamesToolStripMenuItem";
            this.useProvidedNamesToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.useProvidedNamesToolStripMenuItem.Text = "Use provided names";
            this.useProvidedNamesToolStripMenuItem.Click += new System.EventHandler(this.useProvidedNamesToolStripMenuItem_Click);
            // 
            // caseToolStripMenuItem
            // 
            this.caseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.largeToolStripMenuItem,
            this.smallToolStripMenuItem,
            this.igNorEToolStripMenuItem,
            this.cAPSLOCKToolStripMenuItem});
            this.caseToolStripMenuItem.Name = "caseToolStripMenuItem";
            this.caseToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.caseToolStripMenuItem.Text = "Case";
            // 
            // largeToolStripMenuItem
            // 
            this.largeToolStripMenuItem.Name = "largeToolStripMenuItem";
            this.largeToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.largeToolStripMenuItem.Text = "Large";
            this.largeToolStripMenuItem.Click += new System.EventHandler(this.largeToolStripMenuItem_Click);
            // 
            // smallToolStripMenuItem
            // 
            this.smallToolStripMenuItem.Name = "smallToolStripMenuItem";
            this.smallToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.smallToolStripMenuItem.Text = "small";
            this.smallToolStripMenuItem.Click += new System.EventHandler(this.smallToolStripMenuItem_Click);
            // 
            // igNorEToolStripMenuItem
            // 
            this.igNorEToolStripMenuItem.Name = "igNorEToolStripMenuItem";
            this.igNorEToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.igNorEToolStripMenuItem.Text = "IgNorE";
            this.igNorEToolStripMenuItem.Click += new System.EventHandler(this.igNorEToolStripMenuItem_Click);
            // 
            // cAPSLOCKToolStripMenuItem
            // 
            this.cAPSLOCKToolStripMenuItem.Name = "cAPSLOCKToolStripMenuItem";
            this.cAPSLOCKToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.cAPSLOCKToolStripMenuItem.Text = "CAPSLOCK";
            this.cAPSLOCKToolStripMenuItem.Click += new System.EventHandler(this.cAPSLOCKToolStripMenuItem_Click);
            // 
            // replaceInPathToolStripMenuItem
            // 
            this.replaceInPathToolStripMenuItem.Name = "replaceInPathToolStripMenuItem";
            this.replaceInPathToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.replaceInPathToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.replaceInPathToolStripMenuItem.Text = "Replace";
            this.replaceInPathToolStripMenuItem.Click += new System.EventHandler(this.replaceInPathToolStripMenuItem_Click);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.invertSelectionToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.selectByKeywordToolStripMenuItem,
            this.selectSimilarToolStripMenuItem});
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.selectToolStripMenuItem.Text = "Select";
            // 
            // invertSelectionToolStripMenuItem
            // 
            this.invertSelectionToolStripMenuItem.Name = "invertSelectionToolStripMenuItem";
            this.invertSelectionToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.invertSelectionToolStripMenuItem.Text = "Invert selection";
            this.invertSelectionToolStripMenuItem.Click += new System.EventHandler(this.invertSelectionToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // selectByKeywordToolStripMenuItem
            // 
            this.selectByKeywordToolStripMenuItem.Name = "selectByKeywordToolStripMenuItem";
            this.selectByKeywordToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.selectByKeywordToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.selectByKeywordToolStripMenuItem.Text = "Select by keyword";
            this.selectByKeywordToolStripMenuItem.Click += new System.EventHandler(this.selectByKeywordToolStripMenuItem_Click);
            // 
            // selectSimilarToolStripMenuItem
            // 
            this.selectSimilarToolStripMenuItem.Name = "selectSimilarToolStripMenuItem";
            this.selectSimilarToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.selectSimilarToolStripMenuItem.Text = "Select similar by name";
            this.selectSimilarToolStripMenuItem.Click += new System.EventHandler(this.byNameToolStripMenuItem_Click);
            // 
            // checkToolStripMenuItem
            // 
            this.checkToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkAllToolStripMenuItem,
            this.uncheckAllToolStripMenuItem,
            this.invertCheckToolStripMenuItem});
            this.checkToolStripMenuItem.Name = "checkToolStripMenuItem";
            this.checkToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.checkToolStripMenuItem.Text = "Check";
            // 
            // checkAllToolStripMenuItem
            // 
            this.checkAllToolStripMenuItem.Name = "checkAllToolStripMenuItem";
            this.checkAllToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.checkAllToolStripMenuItem.Text = "Check all";
            this.checkAllToolStripMenuItem.Click += new System.EventHandler(this.checkAllToolStripMenuItem_Click);
            // 
            // uncheckAllToolStripMenuItem
            // 
            this.uncheckAllToolStripMenuItem.Name = "uncheckAllToolStripMenuItem";
            this.uncheckAllToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.uncheckAllToolStripMenuItem.Text = "Uncheck all";
            this.uncheckAllToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllToolStripMenuItem_Click);
            // 
            // invertCheckToolStripMenuItem
            // 
            this.invertCheckToolStripMenuItem.Name = "invertCheckToolStripMenuItem";
            this.invertCheckToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.invertCheckToolStripMenuItem.Text = "Invert Check";
            this.invertCheckToolStripMenuItem.Click += new System.EventHandler(this.invertCheckToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(237, 6);
            // 
            // setShownameToolStripMenuItem
            // 
            this.setShownameToolStripMenuItem.Name = "setShownameToolStripMenuItem";
            this.setShownameToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.setShownameToolStripMenuItem.Text = "Set Showname";
            this.setShownameToolStripMenuItem.Click += new System.EventHandler(this.setShownameToolStripMenuItem_Click);
            // 
            // setSeasonToolStripMenuItem
            // 
            this.setSeasonToolStripMenuItem.Name = "setSeasonToolStripMenuItem";
            this.setSeasonToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.setSeasonToolStripMenuItem.Text = "Set Season";
            this.setSeasonToolStripMenuItem.Click += new System.EventHandler(this.setSeasonToolStripMenuItem_Click);
            // 
            // setEpisodesFromtoToolStripMenuItem
            // 
            this.setEpisodesFromtoToolStripMenuItem.Name = "setEpisodesFromtoToolStripMenuItem";
            this.setEpisodesFromtoToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.setEpisodesFromtoToolStripMenuItem.Text = "Set Episodes from...to...";
            this.setEpisodesFromtoToolStripMenuItem.Click += new System.EventHandler(this.setEpisodesFromtoToolStripMenuItem_Click);
            // 
            // setDestinationToolStripMenuItem
            // 
            this.setDestinationToolStripMenuItem.Name = "setDestinationToolStripMenuItem";
            this.setDestinationToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.setDestinationToolStripMenuItem.Text = "Set Destination";
            this.setDestinationToolStripMenuItem.Click += new System.EventHandler(this.setDestinationToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(237, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.originalNameToolStripMenuItem,
            this.pathOrigNameToolStripMenuItem,
            this.titleToolStripMenuItem,
            this.newFileNameToolStripMenuItem,
            this.destinationNewFileNameToolStripMenuItem,
            this.operationToolStripMenuItem});
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.copyToolStripMenuItem.Text = "Copy...";
            // 
            // originalNameToolStripMenuItem
            // 
            this.originalNameToolStripMenuItem.Name = "originalNameToolStripMenuItem";
            this.originalNameToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.originalNameToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.originalNameToolStripMenuItem.Text = "Original Name";
            this.originalNameToolStripMenuItem.Click += new System.EventHandler(this.originalNameToolStripMenuItem_Click);
            // 
            // pathOrigNameToolStripMenuItem
            // 
            this.pathOrigNameToolStripMenuItem.Name = "pathOrigNameToolStripMenuItem";
            this.pathOrigNameToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.pathOrigNameToolStripMenuItem.Text = "Path + Orig. Name";
            this.pathOrigNameToolStripMenuItem.Click += new System.EventHandler(this.pathOrigNameToolStripMenuItem_Click);
            // 
            // titleToolStripMenuItem
            // 
            this.titleToolStripMenuItem.Name = "titleToolStripMenuItem";
            this.titleToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.titleToolStripMenuItem.Text = "Title";
            this.titleToolStripMenuItem.Click += new System.EventHandler(this.titleToolStripMenuItem_Click);
            // 
            // newFileNameToolStripMenuItem
            // 
            this.newFileNameToolStripMenuItem.Name = "newFileNameToolStripMenuItem";
            this.newFileNameToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.newFileNameToolStripMenuItem.Text = "New Filename";
            this.newFileNameToolStripMenuItem.Click += new System.EventHandler(this.newFileNameToolStripMenuItem_Click);
            // 
            // destinationNewFileNameToolStripMenuItem
            // 
            this.destinationNewFileNameToolStripMenuItem.Name = "destinationNewFileNameToolStripMenuItem";
            this.destinationNewFileNameToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.destinationNewFileNameToolStripMenuItem.Text = "Destination + New Filename";
            this.destinationNewFileNameToolStripMenuItem.Click += new System.EventHandler(this.destinationNewFileNameToolStripMenuItem_Click);
            // 
            // operationToolStripMenuItem
            // 
            this.operationToolStripMenuItem.Name = "operationToolStripMenuItem";
            this.operationToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.operationToolStripMenuItem.Text = "Operation";
            this.operationToolStripMenuItem.Click += new System.EventHandler(this.operationToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // regexTesterToolStripMenuItem
            // 
            this.regexTesterToolStripMenuItem.Name = "regexTesterToolStripMenuItem";
            this.regexTesterToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.R)));
            this.regexTesterToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.regexTesterToolStripMenuItem.Text = "RegexTester";
            this.regexTesterToolStripMenuItem.Visible = false;
            this.regexTesterToolStripMenuItem.Click += new System.EventHandler(this.regexTesterToolStripMenuItem_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnConfig.Location = new System.Drawing.Point(558, 4);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(85, 23);
            this.btnConfig.TabIndex = 3;
            this.btnConfig.Text = "Configuration";
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // btnPath
            // 
            this.btnPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPath.Location = new System.Drawing.Point(490, 4);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(28, 23);
            this.btnPath.TabIndex = 2;
            this.btnPath.Text = "...";
            this.toolTip.SetToolTip(this.btnPath, "Browse");
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
            // 
            // txtPath
            // 
            this.txtPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtPath.Location = new System.Drawing.Point(89, 6);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(395, 20);
            this.txtPath.TabIndex = 1;
            this.txtPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPath_KeyDown);
            this.txtPath.Leave += new System.EventHandler(this.txtPath_Leave);
            // 
            // lblFolder
            // 
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(12, 9);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(36, 13);
            this.lblFolder.TabIndex = 0;
            this.lblFolder.Text = "Folder";
            // 
            // txtEdit
            // 
            this.txtEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEdit.Location = new System.Drawing.Point(32, 104);
            this.txtEdit.Multiline = true;
            this.txtEdit.Name = "txtEdit";
            this.txtEdit.Size = new System.Drawing.Size(80, 16);
            this.txtEdit.TabIndex = 3;
            this.txtEdit.Visible = false;
            // 
            // numEdit
            // 
            this.numEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numEdit.Location = new System.Drawing.Point(32, 104);
            this.numEdit.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numEdit.Name = "numEdit";
            this.numEdit.Size = new System.Drawing.Size(80, 20);
            this.numEdit.TabIndex = 3;
            this.numEdit.Visible = false;
            // 
            // comEdit
            // 
            this.comEdit.Location = new System.Drawing.Point(32, 104);
            this.comEdit.Name = "comEdit";
            this.comEdit.Size = new System.Drawing.Size(80, 21);
            this.comEdit.TabIndex = 3;
            this.comEdit.Visible = false;
            this.comEdit.SelectedIndexChanged += new System.EventHandler(this.cbEdit_SelectedIndexChanged);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(15, 10);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(985, 87);
            this.txtLog.TabIndex = 1;
            this.txtLog.Visible = false;
            // 
            // rtbLog
            // 
            this.rtbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLog.Location = new System.Drawing.Point(15, 3);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLog.Size = new System.Drawing.Size(993, 91);
            this.rtbLog.TabIndex = 0;
            this.rtbLog.Text = "";
            this.rtbLog.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbLog_LinkClicked);
            // 
            // fbdPath
            // 
            this.fbdPath.Description = "Browse for folder containing series.";
            this.fbdPath.ShowNewFolderButton = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 573);
            this.Controls.Add(this.scContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(944, 506);
            this.Name = "Form1";
            this.Text = "Series Renamer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeBegin += new System.EventHandler(this.Form1_ResizeBegin);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.scContainer.Panel1.ResumeLayout(false);
            this.scContainer.Panel1.PerformLayout();
            this.scContainer.Panel2.ResumeLayout(false);
            this.scContainer.Panel2.PerformLayout();
            this.scContainer.ResumeLayout(false);
            this.contextFiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numEdit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer scContainer;
        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.FolderBrowserDialog fbdPath;
        private ListViewEx.ListViewEx lstFiles;
        private System.Windows.Forms.ColumnHeader cOldName;
        private System.Windows.Forms.ColumnHeader cSeason;
        private System.Windows.Forms.ColumnHeader cEpID;
        private System.Windows.Forms.ColumnHeader cTitle;
        private System.Windows.Forms.ColumnHeader cNewName;
        private System.Windows.Forms.Button btnTitles;
        private System.Windows.Forms.Label lblTargetFilename;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.ColumnHeader cPath;
        private System.Windows.Forms.TextBox txtEdit;
        private System.Windows.Forms.NumericUpDown numEdit;
        private System.Windows.Forms.ComboBox comEdit;
        private System.Windows.Forms.ComboBox cbProviders;
        private System.Windows.Forms.Label lblTitlesFrom;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.ContextMenuStrip contextFiles;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem setSeasonToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Label lblSubFrom;
        private System.Windows.Forms.Button btnSubs;
        private System.Windows.Forms.ComboBox cbSubs;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem setEpisodesFromtoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editSubtitleToolStripMenuItem;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.ColumnHeader cDestination;
        private System.Windows.Forms.ToolStripMenuItem setDestinationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem originalNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pathOrigNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem titleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newFileNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem destinationNewFileNameToolStripMenuItem;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ToolStripMenuItem removeTagsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem operationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renamingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createDirectoryStructureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceInPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invertSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectByKeywordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectSimilarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invertCheckToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader cShowname;
        private System.Windows.Forms.ToolStripMenuItem createDirectoryStructureToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem dontCreateDirectoryStructureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useUmlautsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useUmlautsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem dontUseUmlautsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useProvidedNamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem caseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem largeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem igNorEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cAPSLOCKToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setShownameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem regexTesterToolStripMenuItem;
    }
}

