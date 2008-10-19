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
            this.btnClear = new System.Windows.Forms.Button();
            this.cbSubs = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSubs = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.cbProviders = new System.Windows.Forms.ComboBox();
            this.lblFrom = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbTitle = new System.Windows.Forms.ComboBox();
            this.btnTitles = new System.Windows.Forms.Button();
            this.lstFiles = new ListViewEx.ListViewEx();
            this.cOldName = new System.Windows.Forms.ColumnHeader();
            this.cPath = new System.Windows.Forms.ColumnHeader();
            this.cSeason = new System.Windows.Forms.ColumnHeader();
            this.cEpID = new System.Windows.Forms.ColumnHeader();
            this.cTitle = new System.Windows.Forms.ColumnHeader();
            this.cNewName = new System.Windows.Forms.ColumnHeader();
            this.cDestination = new System.Windows.Forms.ColumnHeader();
            this.contextFiles = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSubtitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectByKeywordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.checkAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.setSeasonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setDestinationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setEpisodesFromtoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.originalNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pathOrigNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.titleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.destinationNewFileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnConfig = new System.Windows.Forms.Button();
            this.btnPath = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEdit = new System.Windows.Forms.TextBox();
            this.numEdit = new System.Windows.Forms.NumericUpDown();
            this.comEdit = new System.Windows.Forms.ComboBox();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.btnRename = new System.Windows.Forms.Button();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.fbdPath = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtLog = new System.Windows.Forms.TextBox();
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
            this.scContainer.Panel1.Controls.Add(this.btnClear);
            this.scContainer.Panel1.Controls.Add(this.cbSubs);
            this.scContainer.Panel1.Controls.Add(this.label5);
            this.scContainer.Panel1.Controls.Add(this.btnSubs);
            this.scContainer.Panel1.Controls.Add(this.btnAbout);
            this.scContainer.Panel1.Controls.Add(this.cbProviders);
            this.scContainer.Panel1.Controls.Add(this.lblFrom);
            this.scContainer.Panel1.Controls.Add(this.label2);
            this.scContainer.Panel1.Controls.Add(this.cbTitle);
            this.scContainer.Panel1.Controls.Add(this.btnTitles);
            this.scContainer.Panel1.Controls.Add(this.lstFiles);
            this.scContainer.Panel1.Controls.Add(this.btnConfig);
            this.scContainer.Panel1.Controls.Add(this.btnPath);
            this.scContainer.Panel1.Controls.Add(this.txtPath);
            this.scContainer.Panel1.Controls.Add(this.label1);
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
            this.scContainer.Size = new System.Drawing.Size(1016, 541);
            this.scContainer.SplitterDistance = 437;
            this.scContainer.TabIndex = 0;
            this.scContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.scContainer_MouseDown);
            this.scContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.scContainer_MouseUp);
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Image = global::Renamer.Properties.Resources.Browse;
            this.btnOpen.Location = new System.Drawing.Point(524, 4);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(28, 23);
            this.btnOpen.TabIndex = 15;
            this.toolTip1.SetToolTip(this.btnOpen, "Open Folder");
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Image = global::Renamer.Properties.Resources.icon_cancel;
            this.btnClear.Location = new System.Drawing.Point(524, 33);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(28, 23);
            this.btnClear.TabIndex = 14;
            this.toolTip1.SetToolTip(this.btnClear, "Clear title relations data. Rarely needed.");
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // cbSubs
            // 
            this.cbSubs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSubs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSubs.FormattingEnabled = true;
            this.cbSubs.Location = new System.Drawing.Point(652, 6);
            this.cbSubs.Name = "cbSubs";
            this.cbSubs.Size = new System.Drawing.Size(138, 21);
            this.cbSubs.TabIndex = 13;
            this.cbSubs.SelectedIndexChanged += new System.EventHandler(this.cbSubs_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(558, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Get subtitles from";
            // 
            // btnSubs
            // 
            this.btnSubs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubs.Location = new System.Drawing.Point(796, 4);
            this.btnSubs.Name = "btnSubs";
            this.btnSubs.Size = new System.Drawing.Size(83, 23);
            this.btnSubs.TabIndex = 11;
            this.btnSubs.Text = "Get Subtitles !";
            this.btnSubs.UseVisualStyleBackColor = true;
            this.btnSubs.Click += new System.EventHandler(this.btnSubs_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbout.Location = new System.Drawing.Point(885, 33);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(115, 23);
            this.btnAbout.TabIndex = 10;
            this.btnAbout.Text = "About...";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // cbProviders
            // 
            this.cbProviders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbProviders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProviders.FormattingEnabled = true;
            this.cbProviders.Location = new System.Drawing.Point(652, 35);
            this.cbProviders.Name = "cbProviders";
            this.cbProviders.Size = new System.Drawing.Size(138, 21);
            this.cbProviders.TabIndex = 9;
            this.cbProviders.SelectedIndexChanged += new System.EventHandler(this.cbProviders_SelectedIndexChanged);
            // 
            // lblFrom
            // 
            this.lblFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(575, 38);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(71, 13);
            this.lblFrom.TabIndex = 8;
            this.lblFrom.Text = "Get titles from";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Series Title";
            // 
            // cbTitle
            // 
            this.cbTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTitle.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbTitle.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbTitle.Location = new System.Drawing.Point(77, 35);
            this.cbTitle.MaxDropDownItems = 10;
            this.cbTitle.Name = "cbTitle";
            this.cbTitle.Size = new System.Drawing.Size(441, 21);
            this.cbTitle.TabIndex = 6;
            this.toolTip1.SetToolTip(this.cbTitle, "Name of the series");
            this.cbTitle.SelectedIndexChanged += new System.EventHandler(this.cbTitle_SelectedIndexChanged);
            this.cbTitle.Leave += new System.EventHandler(this.cbTitle_Leave);
            this.cbTitle.DropDownClosed += new System.EventHandler(this.cbTitle_DropDownClosed);
            this.cbTitle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbTitle_KeyDown);
            this.cbTitle.TextChanged += new System.EventHandler(this.cbTitle_TextChanged);
            // 
            // btnTitles
            // 
            this.btnTitles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTitles.Location = new System.Drawing.Point(796, 33);
            this.btnTitles.Name = "btnTitles";
            this.btnTitles.Size = new System.Drawing.Size(83, 23);
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
            this.cDestination});
            this.lstFiles.ContextMenuStrip = this.contextFiles;
            this.lstFiles.DoubleClickActivation = true;
            this.lstFiles.FullRowSelect = true;
            this.lstFiles.Location = new System.Drawing.Point(15, 59);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(985, 376);
            this.lstFiles.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstFiles.TabIndex = 4;
            this.toolTip1.SetToolTip(this.lstFiles, "Double click on an entry in Season, Episode, Title or New File Name column to edi" +
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
            this.cOldName.Width = 200;
            // 
            // cPath
            // 
            this.cPath.Text = "Path";
            this.cPath.Width = 134;
            // 
            // cSeason
            // 
            this.cSeason.Text = "Season";
            this.cSeason.Width = 54;
            // 
            // cEpID
            // 
            this.cEpID.Text = "Episode";
            this.cEpID.Width = 56;
            // 
            // cTitle
            // 
            this.cTitle.Text = "Title";
            this.cTitle.Width = 140;
            // 
            // cNewName
            // 
            this.cNewName.Text = "New File Name";
            this.cNewName.Width = 181;
            // 
            // cDestination
            // 
            this.cDestination.Text = "Destination";
            this.cDestination.Width = 197;
            // 
            // contextFiles
            // 
            this.contextFiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.editSubtitleToolStripMenuItem,
            this.toolStripSeparator4,
            this.selectAllToolStripMenuItem,
            this.invertSelectionToolStripMenuItem,
            this.selectByKeywordToolStripMenuItem,
            this.toolStripSeparator1,
            this.checkAllToolStripMenuItem,
            this.uncheckAllToolStripMenuItem,
            this.invertCheckToolStripMenuItem,
            this.toolStripSeparator2,
            this.setSeasonToolStripMenuItem,
            this.setDestinationToolStripMenuItem,
            this.setEpisodesFromtoToolStripMenuItem,
            this.toolStripSeparator3,
            this.copyToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextFiles.Name = "contextFiles";
            this.contextFiles.ShowImageMargin = false;
            this.contextFiles.Size = new System.Drawing.Size(190, 336);
            this.contextFiles.Opening += new System.ComponentModel.CancelEventHandler(this.contextFiles_Opening);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.viewToolStripMenuItem.Text = "Open";
            this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
            // 
            // editSubtitleToolStripMenuItem
            // 
            this.editSubtitleToolStripMenuItem.Name = "editSubtitleToolStripMenuItem";
            this.editSubtitleToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.editSubtitleToolStripMenuItem.Text = "Edit Subtitle";
            this.editSubtitleToolStripMenuItem.Click += new System.EventHandler(this.editSubtitleToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(186, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // invertSelectionToolStripMenuItem
            // 
            this.invertSelectionToolStripMenuItem.Name = "invertSelectionToolStripMenuItem";
            this.invertSelectionToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.invertSelectionToolStripMenuItem.Text = "Invert Selection";
            this.invertSelectionToolStripMenuItem.Click += new System.EventHandler(this.invertSelectionToolStripMenuItem_Click);
            // 
            // selectByKeywordToolStripMenuItem
            // 
            this.selectByKeywordToolStripMenuItem.Name = "selectByKeywordToolStripMenuItem";
            this.selectByKeywordToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.selectByKeywordToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.selectByKeywordToolStripMenuItem.Text = "Select by keyword";
            this.selectByKeywordToolStripMenuItem.Click += new System.EventHandler(this.selectByKeywordToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(186, 6);
            // 
            // checkAllToolStripMenuItem
            // 
            this.checkAllToolStripMenuItem.Name = "checkAllToolStripMenuItem";
            this.checkAllToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.checkAllToolStripMenuItem.Text = "Check all";
            this.checkAllToolStripMenuItem.Click += new System.EventHandler(this.checkAllToolStripMenuItem_Click);
            // 
            // uncheckAllToolStripMenuItem
            // 
            this.uncheckAllToolStripMenuItem.Name = "uncheckAllToolStripMenuItem";
            this.uncheckAllToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.uncheckAllToolStripMenuItem.Text = "Uncheck all";
            this.uncheckAllToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllToolStripMenuItem_Click);
            // 
            // invertCheckToolStripMenuItem
            // 
            this.invertCheckToolStripMenuItem.Name = "invertCheckToolStripMenuItem";
            this.invertCheckToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.invertCheckToolStripMenuItem.Text = "Invert Check";
            this.invertCheckToolStripMenuItem.Click += new System.EventHandler(this.invertCheckToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(186, 6);
            // 
            // setSeasonToolStripMenuItem
            // 
            this.setSeasonToolStripMenuItem.Name = "setSeasonToolStripMenuItem";
            this.setSeasonToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.setSeasonToolStripMenuItem.Text = "Set Season";
            this.setSeasonToolStripMenuItem.Click += new System.EventHandler(this.setSeasonToolStripMenuItem_Click);
            // 
            // setDestinationToolStripMenuItem
            // 
            this.setDestinationToolStripMenuItem.Name = "setDestinationToolStripMenuItem";
            this.setDestinationToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.setDestinationToolStripMenuItem.Text = "Set Destination";
            this.setDestinationToolStripMenuItem.Click += new System.EventHandler(this.setDestinationToolStripMenuItem_Click);
            // 
            // setEpisodesFromtoToolStripMenuItem
            // 
            this.setEpisodesFromtoToolStripMenuItem.Name = "setEpisodesFromtoToolStripMenuItem";
            this.setEpisodesFromtoToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.setEpisodesFromtoToolStripMenuItem.Text = "Set Episodes from...to...";
            this.setEpisodesFromtoToolStripMenuItem.Click += new System.EventHandler(this.setEpisodesFromtoToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(186, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.originalNameToolStripMenuItem,
            this.pathOrigNameToolStripMenuItem,
            this.titleToolStripMenuItem,
            this.newFileNameToolStripMenuItem,
            this.destinationNewFileNameToolStripMenuItem});
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
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
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfig.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnConfig.Location = new System.Drawing.Point(885, 4);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(115, 23);
            this.btnConfig.TabIndex = 3;
            this.btnConfig.Text = "Configuration";
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // btnPath
            // 
            this.btnPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPath.Location = new System.Drawing.Point(490, 4);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(28, 23);
            this.btnPath.TabIndex = 2;
            this.btnPath.Text = "...";
            this.toolTip1.SetToolTip(this.btnPath, "Browse");
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtPath.Location = new System.Drawing.Point(77, 6);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(407, 20);
            this.txtPath.TabIndex = 1;
            this.txtPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPath_KeyDown);
            this.txtPath.Leave += new System.EventHandler(this.txtPath_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Folder";
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
            // rtbLog
            // 
            this.rtbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLog.Location = new System.Drawing.Point(15, 3);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLog.Size = new System.Drawing.Size(985, 87);
            this.rtbLog.TabIndex = 0;
            this.rtbLog.Text = "";
            this.rtbLog.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbLog_LinkClicked);
            // 
            // btnRename
            // 
            this.btnRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRename.AutoSize = true;
            this.btnRename.Location = new System.Drawing.Point(925, 537);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(75, 23);
            this.btnRename.TabIndex = 11;
            this.btnRename.Text = "Rename !";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // txtTarget
            // 
            this.txtTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTarget.Location = new System.Drawing.Point(98, 539);
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(165, 20);
            this.txtTarget.TabIndex = 9;
            this.txtTarget.Text = "S%sE%E - %N";
            this.toolTip1.SetToolTip(this.txtTarget, "Valid variables: %S - Season, %E - Episode, %T - Title, %N - Name, Small letter=1" +
                    " digit");
            this.txtTarget.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTarget_KeyDown);
            this.txtTarget.Leave += new System.EventHandler(this.txtTarget_Leave);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 542);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Target filename";
            this.toolTip1.SetToolTip(this.label3, "Valid variables: %S - Season, %E - Episode, %T - Title, %N - Name, Small letter=1" +
                    " digit");
            // 
            // fbdPath
            // 
            this.fbdPath.Description = "Browse for folder containing series.";
            this.fbdPath.ShowNewFolderButton = false;
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(15, 3);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(985, 87);
            this.txtLog.TabIndex = 1;
            this.txtLog.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 571);
            this.Controls.Add(this.btnRename);
            this.Controls.Add(this.txtTarget);
            this.Controls.Add(this.scContainer);
            this.Controls.Add(this.label3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(759, 506);
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
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.SplitContainer scContainer;
        private System.Windows.Forms.Label label1;
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
        private System.Windows.Forms.ComboBox cbTitle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.ColumnHeader cPath;
        private System.Windows.Forms.TextBox txtEdit;
        private System.Windows.Forms.NumericUpDown numEdit;
        private System.Windows.Forms.ComboBox comEdit;
        private System.Windows.Forms.ComboBox cbProviders;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.ContextMenuStrip contextFiles;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invertSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem checkAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invertCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectByKeywordToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem setSeasonToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSubs;
        private System.Windows.Forms.ComboBox cbSubs;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem setEpisodesFromtoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editSubtitleToolStripMenuItem;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Button btnClear;
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
    }
}

