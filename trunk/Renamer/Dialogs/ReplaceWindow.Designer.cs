namespace Renamer.Dialogs
{
    partial class ReplaceWindow
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
            this.cbReplaceIn = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnReplace = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbReplaceIn
            // 
            this.cbReplaceIn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbReplaceIn.FormattingEnabled = true;
            this.cbReplaceIn.Items.AddRange(new object[] {
            "Original Path",
            "Original Filename",
            "Destination Path",
            "Destination Filename"});
            this.cbReplaceIn.Location = new System.Drawing.Point(12, 121);
            this.cbReplaceIn.Name = "cbReplaceIn";
            this.cbReplaceIn.Size = new System.Drawing.Size(149, 21);
            this.cbReplaceIn.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Replace in:";
            // 
            // txtReplace
            // 
            this.txtReplace.Location = new System.Drawing.Point(12, 73);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(149, 20);
            this.txtReplace.TabIndex = 1;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(12, 25);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(149, 20);
            this.txtSearch.TabIndex = 0;
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(12, 148);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(75, 23);
            this.btnReplace.TabIndex = 3;
            this.btnReplace.Text = "Replace";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(250, 148);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Search for:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Replace with:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(167, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Allowed Variables:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(167, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 117);
            this.label5.TabIndex = 9;
            this.label5.Text = "%OP\r\n%DP \r\n%OF \r\n%DF \r\n%T \r\n%N \r\n%S \r\n%s\r\n%E ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(193, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(139, 117);
            this.label6.TabIndex = 10;
            this.label6.Text = "- Original Path\r\n- Destination Path\r\n- Original Filename\r\n- Destination Filename\r" +
                "\n- Series Title\r\n- Episode Name\r\n- Season Number, 2 Digits\r\n- Season Number, 1-2" +
                " Digits\r\n- Episdoe Number";
            // 
            // ReplaceWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 180);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.txtReplace);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbReplaceIn);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ReplaceWindow";
            this.Text = "ReplaceWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbReplaceIn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}