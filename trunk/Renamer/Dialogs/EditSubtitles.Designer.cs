namespace Renamer.Dialogs
{
    partial class EditSubtitles
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpSub = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.nudMSSub = new System.Windows.Forms.NumericUpDown();
            this.cbSubs = new System.Windows.Forms.ComboBox();
            this.rbSub = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpDelay = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.nudMSDelay = new System.Windows.Forms.NumericUpDown();
            this.rbManually = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.scale = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label8 = new System.Windows.Forms.Label();
            this.cbSecond = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpSecond = new System.Windows.Forms.DateTimePicker();
            this.label12 = new System.Windows.Forms.Label();
            this.nudSecond = new System.Windows.Forms.NumericUpDown();
            this.rbOffset = new System.Windows.Forms.RadioButton();
            this.rbScale = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbRange = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.label10 = new System.Windows.Forms.Label();
            this.nudFrom = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.label14 = new System.Windows.Forms.Label();
            this.nudTo = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.cbBackwards = new System.Windows.Forms.CheckBox();
            this.btnVideo = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMSSub)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMSDelay)).BeginInit();
            this.scale.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSecond)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTo)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbBackwards);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dtpSub);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.nudMSSub);
            this.groupBox1.Controls.Add(this.cbSubs);
            this.groupBox1.Controls.Add(this.rbSub);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dtpDelay);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.nudMSDelay);
            this.groupBox1.Controls.Add(this.rbManually);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(548, 151);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(268, 111);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 26);
            this.label6.TabIndex = 15;
            this.label6.Text = "Time when selected \r\nSubtitle should appear";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(476, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Milliseconds";
            // 
            // dtpSub
            // 
            this.dtpSub.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpSub.Location = new System.Drawing.Point(389, 117);
            this.dtpSub.Name = "dtpSub";
            this.dtpSub.ShowUpDown = true;
            this.dtpSub.Size = new System.Drawing.Size(84, 20);
            this.dtpSub.TabIndex = 13;
            this.dtpSub.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dtpSub_MouseDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(386, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Time";
            // 
            // nudMSSub
            // 
            this.nudMSSub.Location = new System.Drawing.Point(479, 117);
            this.nudMSSub.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudMSSub.Name = "nudMSSub";
            this.nudMSSub.Size = new System.Drawing.Size(63, 20);
            this.nudMSSub.TabIndex = 11;
            this.nudMSSub.Click += new System.EventHandler(this.Subs_Click);
            // 
            // cbSubs
            // 
            this.cbSubs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSubs.FormattingEnabled = true;
            this.cbSubs.Location = new System.Drawing.Point(90, 116);
            this.cbSubs.Name = "cbSubs";
            this.cbSubs.Size = new System.Drawing.Size(172, 21);
            this.cbSubs.TabIndex = 10;
            this.cbSubs.SelectedIndexChanged += new System.EventHandler(this.cbSubs_SelectedIndexChanged);
            this.cbSubs.Click += new System.EventHandler(this.Subs_Click);
            // 
            // rbSub
            // 
            this.rbSub.AutoSize = true;
            this.rbSub.Location = new System.Drawing.Point(9, 117);
            this.rbSub.Name = "rbSub";
            this.rbSub.Size = new System.Drawing.Size(75, 17);
            this.rbSub.TabIndex = 9;
            this.rbSub.TabStop = true;
            this.rbSub.Text = "By Subtitle";
            this.rbSub.UseVisualStyleBackColor = true;
            this.rbSub.CheckedChanged += new System.EventHandler(this.rbSub_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(476, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Milliseconds";
            // 
            // dtpDelay
            // 
            this.dtpDelay.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpDelay.Location = new System.Drawing.Point(389, 63);
            this.dtpDelay.Name = "dtpDelay";
            this.dtpDelay.ShowUpDown = true;
            this.dtpDelay.Size = new System.Drawing.Size(84, 20);
            this.dtpDelay.TabIndex = 7;
            this.dtpDelay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dtpDelay_MouseDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(386, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Delay";
            // 
            // nudMSDelay
            // 
            this.nudMSDelay.Location = new System.Drawing.Point(479, 63);
            this.nudMSDelay.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudMSDelay.Name = "nudMSDelay";
            this.nudMSDelay.Size = new System.Drawing.Size(63, 20);
            this.nudMSDelay.TabIndex = 5;
            this.nudMSDelay.Click += new System.EventHandler(this.Manually_Click);
            // 
            // rbManually
            // 
            this.rbManually.AutoSize = true;
            this.rbManually.Location = new System.Drawing.Point(9, 63);
            this.rbManually.Name = "rbManually";
            this.rbManually.Size = new System.Drawing.Size(67, 17);
            this.rbManually.TabIndex = 1;
            this.rbManually.TabStop = true;
            this.rbManually.Text = "Manually";
            this.rbManually.UseVisualStyleBackColor = true;
            this.rbManually.CheckedChanged += new System.EventHandler(this.rbManually_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(455, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Change the time offset of the subtitle.\r\nEnter the delay manually, or select a su" +
                "btitle and enter the time it should appear in the movie file";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(404, 372);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(485, 372);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // scale
            // 
            this.scale.Controls.Add(this.label7);
            this.scale.Controls.Add(this.label11);
            this.scale.Controls.Add(this.dtpSecond);
            this.scale.Controls.Add(this.label12);
            this.scale.Controls.Add(this.nudSecond);
            this.scale.Controls.Add(this.cbSecond);
            this.scale.Controls.Add(this.label8);
            this.scale.Location = new System.Drawing.Point(12, 169);
            this.scale.Name = "scale";
            this.scale.Size = new System.Drawing.Size(548, 99);
            this.scale.TabIndex = 3;
            this.scale.TabStop = false;
            this.scale.Text = "Scale";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 61);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(148, 26);
            this.label8.TabIndex = 1;
            this.label8.Text = "Subtitle (should be as late as \r\npossible to increase precision)";
            // 
            // cbSecond
            // 
            this.cbSecond.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSecond.FormattingEnabled = true;
            this.cbSecond.Location = new System.Drawing.Point(160, 66);
            this.cbSecond.Name = "cbSecond";
            this.cbSecond.Size = new System.Drawing.Size(220, 21);
            this.cbSecond.TabIndex = 3;
            this.cbSecond.SelectedIndexChanged += new System.EventHandler(this.cbSecond_SelectedIndexChanged);
            this.cbSecond.Click += new System.EventHandler(this.cbSecond_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(476, 51);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(64, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "Milliseconds";
            // 
            // dtpSecond
            // 
            this.dtpSecond.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpSecond.Location = new System.Drawing.Point(389, 67);
            this.dtpSecond.Name = "dtpSecond";
            this.dtpSecond.ShowUpDown = true;
            this.dtpSecond.Size = new System.Drawing.Size(84, 20);
            this.dtpSecond.TabIndex = 22;
            this.dtpSecond.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dtpSecond_MouseDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(386, 51);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(30, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "Time";
            // 
            // nudSecond
            // 
            this.nudSecond.Location = new System.Drawing.Point(479, 67);
            this.nudSecond.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudSecond.Name = "nudSecond";
            this.nudSecond.Size = new System.Drawing.Size(63, 20);
            this.nudSecond.TabIndex = 20;
            this.nudSecond.Click += new System.EventHandler(this.nudSecond_Click);
            // 
            // rbOffset
            // 
            this.rbOffset.AutoSize = true;
            this.rbOffset.Checked = true;
            this.rbOffset.Location = new System.Drawing.Point(21, 10);
            this.rbOffset.Name = "rbOffset";
            this.rbOffset.Size = new System.Drawing.Size(53, 17);
            this.rbOffset.TabIndex = 4;
            this.rbOffset.TabStop = true;
            this.rbOffset.Text = "Offset";
            this.rbOffset.UseVisualStyleBackColor = true;
            // 
            // rbScale
            // 
            this.rbScale.AutoSize = true;
            this.rbScale.Location = new System.Drawing.Point(21, 167);
            this.rbScale.Name = "rbScale";
            this.rbScale.Size = new System.Drawing.Size(52, 17);
            this.rbScale.TabIndex = 5;
            this.rbScale.Text = "Scale";
            this.rbScale.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(392, 26);
            this.label7.TabIndex = 24;
            this.label7.Text = "Scale the subtitle times to correct framerate issues. Select one of the last subt" +
                "itles \r\nand enter the time when it should appear, others will be scaled accordin" +
                "gly.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.dtpTo);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.nudTo);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.dtpFrom);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.nudFrom);
            this.groupBox2.Location = new System.Drawing.Point(12, 274);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(548, 92);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            // 
            // cbRange
            // 
            this.cbRange.AutoSize = true;
            this.cbRange.Location = new System.Drawing.Point(21, 272);
            this.cbRange.Name = "cbRange";
            this.cbRange.Size = new System.Drawing.Size(171, 17);
            this.cbRange.TabIndex = 7;
            this.cbRange.Text = "Only apply to specific timespan";
            this.cbRange.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(146, 47);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "Milliseconds";
            // 
            // dtpFrom
            // 
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpFrom.Location = new System.Drawing.Point(59, 63);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.ShowUpDown = true;
            this.dtpFrom.Size = new System.Drawing.Size(84, 20);
            this.dtpFrom.TabIndex = 26;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(56, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(30, 13);
            this.label10.TabIndex = 25;
            this.label10.Text = "Time";
            // 
            // nudFrom
            // 
            this.nudFrom.Location = new System.Drawing.Point(149, 63);
            this.nudFrom.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudFrom.Name = "nudFrom";
            this.nudFrom.Size = new System.Drawing.Size(63, 20);
            this.nudFrom.TabIndex = 24;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(372, 47);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(64, 13);
            this.label13.TabIndex = 31;
            this.label13.Text = "Milliseconds";
            // 
            // dtpTo
            // 
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpTo.Location = new System.Drawing.Point(285, 63);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.ShowUpDown = true;
            this.dtpTo.Size = new System.Drawing.Size(84, 20);
            this.dtpTo.TabIndex = 30;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(282, 47);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(30, 13);
            this.label14.TabIndex = 29;
            this.label14.Text = "Time";
            // 
            // nudTo
            // 
            this.nudTo.Location = new System.Drawing.Point(375, 63);
            this.nudTo.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudTo.Name = "nudTo";
            this.nudTo.Size = new System.Drawing.Size(63, 20);
            this.nudTo.TabIndex = 28;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(257, 65);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(20, 13);
            this.label15.TabIndex = 32;
            this.label15.Text = "To";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(8, 65);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(30, 13);
            this.label16.TabIndex = 33;
            this.label16.Text = "From";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(8, 18);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(414, 13);
            this.label17.TabIndex = 34;
            this.label17.Text = "If there are cuts in the video, subtitles can be fixed only in certain areas with" +
                " this option";
            // 
            // cbBackwards
            // 
            this.cbBackwards.AutoSize = true;
            this.cbBackwards.Location = new System.Drawing.Point(273, 66);
            this.cbBackwards.Name = "cbBackwards";
            this.cbBackwards.Size = new System.Drawing.Size(110, 17);
            this.cbBackwards.TabIndex = 16;
            this.cbBackwards.Text = "Offset Backwards";
            this.cbBackwards.UseVisualStyleBackColor = true;
            this.cbBackwards.CheckedChanged += new System.EventHandler(this.cbBackwards_CheckedChanged);
            // 
            // btnVideo
            // 
            this.btnVideo.Location = new System.Drawing.Point(323, 372);
            this.btnVideo.Name = "btnVideo";
            this.btnVideo.Size = new System.Drawing.Size(75, 23);
            this.btnVideo.TabIndex = 8;
            this.btnVideo.Text = "Open Video";
            this.btnVideo.UseVisualStyleBackColor = true;
            this.btnVideo.Click += new System.EventHandler(this.button1_Click);
            // 
            // EditSubtitles
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(572, 407);
            this.ControlBox = false;
            this.Controls.Add(this.btnVideo);
            this.Controls.Add(this.cbRange);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.rbScale);
            this.Controls.Add(this.rbOffset);
            this.Controls.Add(this.scale);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditSubtitles";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Edit Subtitles";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMSSub)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMSDelay)).EndInit();
            this.scale.ResumeLayout(false);
            this.scale.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSecond)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbManually;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudMSDelay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpDelay;
        private System.Windows.Forms.RadioButton rbSub;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpSub;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudMSSub;
        private System.Windows.Forms.ComboBox cbSubs;
        private System.Windows.Forms.GroupBox scale;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox cbSecond;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpSecond;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown nudSecond;
        private System.Windows.Forms.RadioButton rbOffset;
        private System.Windows.Forms.RadioButton rbScale;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbRange;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown nudTo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nudFrom;
        private System.Windows.Forms.CheckBox cbBackwards;
        private System.Windows.Forms.Button btnVideo;
    }
}