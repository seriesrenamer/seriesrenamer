namespace Renamer
{
    partial class Configuration
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configuration));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.nudTimeout = new System.Windows.Forms.NumericUpDown();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.nudSearchDepth = new System.Windows.Forms.NumericUpDown();
            this.txtExtensions = new System.Windows.Forms.TextBox();
            this.txtPattern = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cbError = new System.Windows.Forms.ComboBox();
            this.cbWarning = new System.Windows.Forms.ComboBox();
            this.cbStatus = new System.Windows.Forms.ComboBox();
            this.cbInfo = new System.Windows.Forms.ComboBox();
            this.cbReplace = new System.Windows.Forms.ComboBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.txtSubs = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cbCase = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cbUmlaut = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtExtract = new System.Windows.Forms.TextBox();
            this.chkCreateDirectoryStructure = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.chkUseSeasonSubdirs = new System.Windows.Forms.CheckBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.chkDeleteAllEmptyFolders = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtIgnoreFiles = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.chkDeleteEmptyFolders = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.btnDefaults = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.cbDebug = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSearchDepth)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Timeout (ms)";
            this.toolTip.SetToolTip(this.label1, "Timeout setting for all web related connections");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(214, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "On encountering invalid filename characters";
            this.toolTip.SetToolTip(this.label2, "Action to take when invalid filename characters are encountered");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(234, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "String to replace invalid filename characters with";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Subdirectory search depth";
            this.toolTip.SetToolTip(this.label4, "Search depth for scanning subdirectories for files");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Video file extensions";
            this.toolTip.SetToolTip(this.label5, "File extensions to include in renaming process");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(350, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(147, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Filename identification pattern";
            this.toolTip.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // nudTimeout
            // 
            this.nudTimeout.AccessibleDescription = "";
            this.nudTimeout.AccessibleName = "";
            this.nudTimeout.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudTimeout.Location = new System.Drawing.Point(391, 11);
            this.nudTimeout.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudTimeout.Name = "nudTimeout";
            this.nudTimeout.Size = new System.Drawing.Size(163, 20);
            this.nudTimeout.TabIndex = 6;
            this.toolTip.SetToolTip(this.nudTimeout, "Timeout setting for all web related connections");
            // 
            // txtReplace
            // 
            this.txtReplace.Location = new System.Drawing.Point(391, 38);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(163, 20);
            this.txtReplace.TabIndex = 8;
            this.toolTip.SetToolTip(this.txtReplace, "String to replace invalid filename characters with");
            // 
            // nudSearchDepth
            // 
            this.nudSearchDepth.Location = new System.Drawing.Point(391, 38);
            this.nudSearchDepth.Name = "nudSearchDepth";
            this.nudSearchDepth.Size = new System.Drawing.Size(163, 20);
            this.nudSearchDepth.TabIndex = 9;
            this.toolTip.SetToolTip(this.nudSearchDepth, "Search depth for scanning subdirectories for files");
            // 
            // txtExtensions
            // 
            this.txtExtensions.Location = new System.Drawing.Point(15, 82);
            this.txtExtensions.Multiline = true;
            this.txtExtensions.Name = "txtExtensions";
            this.txtExtensions.Size = new System.Drawing.Size(163, 161);
            this.txtExtensions.TabIndex = 10;
            this.toolTip.SetToolTip(this.txtExtensions, "File extensions to include in renaming process");
            // 
            // txtPattern
            // 
            this.txtPattern.Location = new System.Drawing.Point(353, 83);
            this.txtPattern.Multiline = true;
            this.txtPattern.Name = "txtPattern";
            this.txtPattern.Size = new System.Drawing.Size(201, 161);
            this.txtPattern.TabIndex = 11;
            this.toolTip.SetToolTip(this.txtPattern, resources.GetString("txtPattern.ToolTip"));
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(508, 299);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(420, 299);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(82, 23);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "On Error Message";
            this.toolTip.SetToolTip(this.label7, "How error messages are processed");
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 41);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "On Warning Message";
            this.toolTip.SetToolTip(this.label8, "How warning messages are processed");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 68);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "On Status Message";
            this.toolTip.SetToolTip(this.label9, "How status messages are processed");
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 95);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(88, 13);
            this.label10.TabIndex = 17;
            this.label10.Text = "On Info Message";
            this.toolTip.SetToolTip(this.label10, "How info messages are processed");
            // 
            // cbError
            // 
            this.cbError.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbError.FormattingEnabled = true;
            this.cbError.Items.AddRange(new object[] {
            "Do Nothing",
            "Show in Log File",
            "Show Message Box",
            "Show in Log File and Message Box"});
            this.cbError.Location = new System.Drawing.Point(394, 11);
            this.cbError.Name = "cbError";
            this.cbError.Size = new System.Drawing.Size(163, 21);
            this.cbError.TabIndex = 18;
            this.toolTip.SetToolTip(this.cbError, "How error messages are processed");
            // 
            // cbWarning
            // 
            this.cbWarning.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWarning.FormattingEnabled = true;
            this.cbWarning.Items.AddRange(new object[] {
            "Do Nothing",
            "Show in Log File",
            "Show Message Box",
            "Show in Log File and Message Box"});
            this.cbWarning.Location = new System.Drawing.Point(394, 38);
            this.cbWarning.Name = "cbWarning";
            this.cbWarning.Size = new System.Drawing.Size(163, 21);
            this.cbWarning.TabIndex = 19;
            this.toolTip.SetToolTip(this.cbWarning, "How warning messages are processed");
            // 
            // cbStatus
            // 
            this.cbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStatus.FormattingEnabled = true;
            this.cbStatus.Items.AddRange(new object[] {
            "Do Nothing",
            "Show in Log File",
            "Show Message Box",
            "Show in Log File and Message Box"});
            this.cbStatus.Location = new System.Drawing.Point(394, 65);
            this.cbStatus.Name = "cbStatus";
            this.cbStatus.Size = new System.Drawing.Size(163, 21);
            this.cbStatus.TabIndex = 20;
            this.toolTip.SetToolTip(this.cbStatus, "How status messages are processed");
            // 
            // cbInfo
            // 
            this.cbInfo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInfo.FormattingEnabled = true;
            this.cbInfo.Items.AddRange(new object[] {
            "Do Nothing",
            "Show in Log File",
            "Show Message Box",
            "Show in Log File and Message Box"});
            this.cbInfo.Location = new System.Drawing.Point(394, 92);
            this.cbInfo.Name = "cbInfo";
            this.cbInfo.Size = new System.Drawing.Size(163, 21);
            this.cbInfo.TabIndex = 21;
            this.toolTip.SetToolTip(this.cbInfo, "How info messages are processed");
            // 
            // cbReplace
            // 
            this.cbReplace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbReplace.FormattingEnabled = true;
            this.cbReplace.Items.AddRange(new object[] {
            "Ask",
            "Skip",
            "Replace"});
            this.cbReplace.Location = new System.Drawing.Point(391, 11);
            this.cbReplace.Name = "cbReplace";
            this.cbReplace.Size = new System.Drawing.Size(163, 21);
            this.cbReplace.TabIndex = 22;
            this.toolTip.SetToolTip(this.cbReplace, "Action to take when invalid filename characters are encountered");
            // 
            // txtSubs
            // 
            this.txtSubs.Location = new System.Drawing.Point(184, 83);
            this.txtSubs.Multiline = true;
            this.txtSubs.Name = "txtSubs";
            this.txtSubs.Size = new System.Drawing.Size(163, 160);
            this.txtSubs.TabIndex = 23;
            this.toolTip.SetToolTip(this.txtSubs, "File extensions used for subtitles");
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(191, 66);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(111, 13);
            this.label11.TabIndex = 24;
            this.label11.Text = "Subtitle file extensions";
            this.toolTip.SetToolTip(this.label11, "File extensions used for subtitles");
            // 
            // cbCase
            // 
            this.cbCase.FormattingEnabled = true;
            this.cbCase.Items.AddRange(new object[] {
            "IgNOrE",
            "small",
            "Large",
            "CAPSLOCK"});
            this.cbCase.Location = new System.Drawing.Point(391, 91);
            this.cbCase.Name = "cbCase";
            this.cbCase.Size = new System.Drawing.Size(163, 21);
            this.cbCase.TabIndex = 26;
            this.toolTip.SetToolTip(this.cbCase, "Select the desired case of the words in the filenames");
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 94);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(31, 13);
            this.label13.TabIndex = 25;
            this.label13.Text = "Case";
            this.toolTip.SetToolTip(this.label13, "Select the desired case of the words in the filenames");
            // 
            // cbUmlaut
            // 
            this.cbUmlaut.FormattingEnabled = true;
            this.cbUmlaut.Items.AddRange(new object[] {
            "Ignore",
            "Use",
            "Don\'t Use"});
            this.cbUmlaut.Location = new System.Drawing.Point(391, 64);
            this.cbUmlaut.Name = "cbUmlaut";
            this.cbUmlaut.Size = new System.Drawing.Size(163, 21);
            this.cbUmlaut.TabIndex = 24;
            this.toolTip.SetToolTip(this.cbUmlaut, "Select if Umlaute in names should be ignored, enforced or removed");
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 67);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(343, 13);
            this.label12.TabIndex = 23;
            this.label12.Text = "Filter out weird dots/lines/etc above characters (Umlauts, Accents, etc)";
            this.toolTip.SetToolTip(this.label12, "Select if Umlaute in names should be ignored, enforced or removed");
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(15, 52);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(347, 13);
            this.label14.TabIndex = 27;
            this.label14.Text = "--> Season subdirectory name (needs %S and optionally %T placeholder)";
            this.toolTip.SetToolTip(this.label14, "Setting this allows the program to get the show name and the seasons from folder " +
                    "names\r\n%S - Season\r\n%T - Series Title");
            // 
            // txtExtract
            // 
            this.txtExtract.Location = new System.Drawing.Point(391, 49);
            this.txtExtract.Name = "txtExtract";
            this.txtExtract.Size = new System.Drawing.Size(163, 20);
            this.txtExtract.TabIndex = 28;
            this.toolTip.SetToolTip(this.txtExtract, "Setting this allows the program to get the show name and the seasons from folder " +
                    "names\r\n%S - Season\r\n%T - Series Title");
            // 
            // chkCreateDirectoryStructure
            // 
            this.chkCreateDirectoryStructure.AutoSize = true;
            this.chkCreateDirectoryStructure.Location = new System.Drawing.Point(539, 6);
            this.chkCreateDirectoryStructure.Name = "chkCreateDirectoryStructure";
            this.chkCreateDirectoryStructure.Size = new System.Drawing.Size(15, 14);
            this.chkCreateDirectoryStructure.TabIndex = 29;
            this.toolTip.SetToolTip(this.chkCreateDirectoryStructure, "If checked, files will be moved to ShowName\\Season x\\");
            this.chkCreateDirectoryStructure.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(15, 6);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(125, 13);
            this.label15.TabIndex = 30;
            this.label15.Text = "Create directory structure";
            this.toolTip.SetToolTip(this.label15, "If checked, files will be moved to ShowName\\Season x\\");
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.txtExtensions);
            this.tabPage1.Controls.Add(this.txtPattern);
            this.tabPage1.Controls.Add(this.txtSubs);
            this.tabPage1.Controls.Add(this.nudSearchDepth);
            this.tabPage1.Controls.Add(this.nudTimeout);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(560, 253);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.toolTip.SetToolTip(this.tabPage1, "General settings which don\'t fit anywhere else");
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cbCase);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.cbUmlaut);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.txtReplace);
            this.tabPage2.Controls.Add(this.cbReplace);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(560, 253);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Renaming";
            this.toolTip.SetToolTip(this.tabPage2, "Settings related to the renaming itsself");
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label20);
            this.tabPage3.Controls.Add(this.cbDebug);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Controls.Add(this.cbError);
            this.tabPage3.Controls.Add(this.cbInfo);
            this.tabPage3.Controls.Add(this.cbStatus);
            this.tabPage3.Controls.Add(this.cbWarning);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(560, 253);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Logging";
            this.toolTip.SetToolTip(this.tabPage3, "Settings related to the logging functions");
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.chkUseSeasonSubdirs);
            this.tabPage4.Controls.Add(this.label19);
            this.tabPage4.Controls.Add(this.label18);
            this.tabPage4.Controls.Add(this.chkDeleteAllEmptyFolders);
            this.tabPage4.Controls.Add(this.label17);
            this.tabPage4.Controls.Add(this.txtIgnoreFiles);
            this.tabPage4.Controls.Add(this.label16);
            this.tabPage4.Controls.Add(this.chkDeleteEmptyFolders);
            this.tabPage4.Controls.Add(this.label15);
            this.tabPage4.Controls.Add(this.chkCreateDirectoryStructure);
            this.tabPage4.Controls.Add(this.txtExtract);
            this.tabPage4.Controls.Add(this.label14);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(560, 253);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Folder Structure";
            this.toolTip.SetToolTip(this.tabPage4, "Settings related to moving the files, deleting empty folders, ...");
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // chkUseSeasonSubdirs
            // 
            this.chkUseSeasonSubdirs.AutoSize = true;
            this.chkUseSeasonSubdirs.Location = new System.Drawing.Point(539, 29);
            this.chkUseSeasonSubdirs.Name = "chkUseSeasonSubdirs";
            this.chkUseSeasonSubdirs.Size = new System.Drawing.Size(15, 14);
            this.chkUseSeasonSubdirs.TabIndex = 38;
            this.chkUseSeasonSubdirs.UseVisualStyleBackColor = true;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(15, 29);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(125, 13);
            this.label19.TabIndex = 37;
            this.label19.Text = "Use Season subdirectory";
            this.toolTip.SetToolTip(this.label19, "If checked, files are put into showname\\season x\\ directories, if unchecked, only" +
                    " showname\\ directory is set");
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(15, 98);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(131, 13);
            this.label18.TabIndex = 36;
            this.label18.Text = "--> Delete all empty folders";
            this.toolTip.SetToolTip(this.label18, "All empty subfolders will be deleted");
            // 
            // chkDeleteAllEmptyFolders
            // 
            this.chkDeleteAllEmptyFolders.AutoSize = true;
            this.chkDeleteAllEmptyFolders.Location = new System.Drawing.Point(539, 98);
            this.chkDeleteAllEmptyFolders.Name = "chkDeleteAllEmptyFolders";
            this.chkDeleteAllEmptyFolders.Size = new System.Drawing.Size(15, 14);
            this.chkDeleteAllEmptyFolders.TabIndex = 35;
            this.toolTip.SetToolTip(this.chkDeleteAllEmptyFolders, "All empty subfolders will be deleted");
            this.chkDeleteAllEmptyFolders.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(15, 121);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(289, 13);
            this.label17.TabIndex = 34;
            this.label17.Text = "--> Also delete folders if they contain only filetypes on this list";
            this.toolTip.SetToolTip(this.label17, resources.GetString("label17.ToolTip"));
            // 
            // txtIgnoreFiles
            // 
            this.txtIgnoreFiles.Location = new System.Drawing.Point(391, 118);
            this.txtIgnoreFiles.Name = "txtIgnoreFiles";
            this.txtIgnoreFiles.Size = new System.Drawing.Size(163, 20);
            this.txtIgnoreFiles.TabIndex = 33;
            this.toolTip.SetToolTip(this.txtIgnoreFiles, resources.GetString("txtIgnoreFiles.ToolTip"));
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(15, 75);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(215, 13);
            this.label16.TabIndex = 32;
            this.label16.Text = "Delete empty folders emptied by moving files";
            this.toolTip.SetToolTip(this.label16, "If files are moved out of a folder and the folder is then empty, it will be delet" +
                    "ed");
            // 
            // chkDeleteEmptyFolders
            // 
            this.chkDeleteEmptyFolders.AutoSize = true;
            this.chkDeleteEmptyFolders.Location = new System.Drawing.Point(539, 75);
            this.chkDeleteEmptyFolders.Name = "chkDeleteEmptyFolders";
            this.chkDeleteEmptyFolders.Size = new System.Drawing.Size(15, 14);
            this.chkDeleteEmptyFolders.TabIndex = 31;
            this.toolTip.SetToolTip(this.chkDeleteEmptyFolders, "If files are moved out of a folder and the folder is then empty, it will be delet" +
                    "ed");
            this.chkDeleteEmptyFolders.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(15, 14);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(568, 279);
            this.tabControl1.TabIndex = 25;
            // 
            // btnDefaults
            // 
            this.btnDefaults.Location = new System.Drawing.Point(15, 299);
            this.btnDefaults.Name = "btnDefaults";
            this.btnDefaults.Size = new System.Drawing.Size(75, 23);
            this.btnDefaults.TabIndex = 26;
            this.btnDefaults.Text = "Defaults";
            this.btnDefaults.UseVisualStyleBackColor = true;
            this.btnDefaults.Click += new System.EventHandler(this.btnDefaults_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(12, 122);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(102, 13);
            this.label20.TabIndex = 22;
            this.label20.Text = "On Debug Message";
            this.toolTip.SetToolTip(this.label20, "How debug messages are processed");
            // 
            // cbDebug
            // 
            this.cbDebug.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDebug.FormattingEnabled = true;
            this.cbDebug.Items.AddRange(new object[] {
            "Do Nothing",
            "Show in Log File",
            "Show Message Box",
            "Show in Log File and Message Box"});
            this.cbDebug.Location = new System.Drawing.Point(394, 119);
            this.cbDebug.Name = "cbDebug";
            this.cbDebug.Size = new System.Drawing.Size(163, 21);
            this.cbDebug.TabIndex = 23;
            this.toolTip.SetToolTip(this.cbDebug, "How debug messages are processed");
            // 
            // Configuration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(599, 338);
            this.Controls.Add(this.btnDefaults);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Configuration";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuration";
            this.Load += new System.EventHandler(this.Configuration_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSearchDepth)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudTimeout;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.NumericUpDown nudSearchDepth;
        private System.Windows.Forms.TextBox txtExtensions;
        private System.Windows.Forms.TextBox txtPattern;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cbError;
        private System.Windows.Forms.ComboBox cbWarning;
        private System.Windows.Forms.ComboBox cbStatus;
        private System.Windows.Forms.ComboBox cbInfo;
        private System.Windows.Forms.ComboBox cbReplace;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TextBox txtSubs;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ComboBox cbUmlaut;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cbCase;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnDefaults;
        private System.Windows.Forms.TextBox txtExtract;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox chkCreateDirectoryStructure;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox chkDeleteEmptyFolders;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtIgnoreFiles;
        private System.Windows.Forms.CheckBox chkDeleteAllEmptyFolders;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.CheckBox chkUseSeasonSubdirs;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox cbDebug;
    }
}