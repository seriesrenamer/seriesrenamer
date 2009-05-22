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

namespace Renamer.Dialogs
{
    partial class InvalidFilename
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
            this.lblError = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnSkip = new System.Windows.Forms.Button();
            this.rbFilename = new System.Windows.Forms.RadioButton();
            this.rbReplace = new System.Windows.Forms.RadioButton();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.chkReplace = new System.Windows.Forms.CheckBox();
            this.btnSkipAll = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(12, 9);
            this.lblError.MinimumSize = new System.Drawing.Size(360, 0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(360, 13);
            this.lblError.TabIndex = 0;
            this.lblError.Text = "Filename must not contain these characters: ";
            // 
            // txtFilename
            // 
            this.txtFilename.Location = new System.Drawing.Point(199, 25);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(172, 20);
            this.txtFilename.TabIndex = 1;
            this.toolTip.SetToolTip(this.txtFilename, "Enter new filename directly here");
            this.txtFilename.Click += new System.EventHandler(this.txtFilename_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(15, 98);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.toolTip.SetToolTip(this.btnOK, "Applies selected action to this file");
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnSkip
            // 
            this.btnSkip.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSkip.Location = new System.Drawing.Point(155, 98);
            this.btnSkip.Name = "btnSkip";
            this.btnSkip.Size = new System.Drawing.Size(75, 23);
            this.btnSkip.TabIndex = 3;
            this.btnSkip.Text = "Skip";
            this.toolTip.SetToolTip(this.btnSkip, "Skips this file");
            this.btnSkip.UseVisualStyleBackColor = true;
            this.btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
            // 
            // rbFilename
            // 
            this.rbFilename.AutoSize = true;
            this.rbFilename.Checked = true;
            this.rbFilename.Location = new System.Drawing.Point(15, 28);
            this.rbFilename.Name = "rbFilename";
            this.rbFilename.Size = new System.Drawing.Size(95, 17);
            this.rbFilename.TabIndex = 4;
            this.rbFilename.TabStop = true;
            this.rbFilename.Text = "New Filename:";
            this.rbFilename.UseVisualStyleBackColor = true;
            this.rbFilename.Click += new System.EventHandler(this.rbFilename_Click);
            // 
            // rbReplace
            // 
            this.rbReplace.AutoSize = true;
            this.rbReplace.Location = new System.Drawing.Point(15, 52);
            this.rbReplace.Name = "rbReplace";
            this.rbReplace.Size = new System.Drawing.Size(178, 17);
            this.rbReplace.TabIndex = 5;
            this.rbReplace.Text = "Replace Invalid Characters with:";
            this.rbReplace.UseVisualStyleBackColor = true;
            this.rbReplace.Click += new System.EventHandler(this.rbReplace_Click);
            // 
            // txtReplace
            // 
            this.txtReplace.Location = new System.Drawing.Point(199, 51);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(172, 20);
            this.txtReplace.TabIndex = 6;
            this.toolTip.SetToolTip(this.txtReplace, "All occurences of invalid filenmae characters will be replaced by the text entere" +
                    "d here");
            this.txtReplace.Click += new System.EventHandler(this.txtReplace_Click);
            // 
            // chkReplace
            // 
            this.chkReplace.AutoSize = true;
            this.chkReplace.Location = new System.Drawing.Point(15, 75);
            this.chkReplace.Name = "chkReplace";
            this.chkReplace.Size = new System.Drawing.Size(234, 17);
            this.chkReplace.TabIndex = 7;
            this.chkReplace.Text = "Remember decision (for replace and skip all)";
            this.toolTip.SetToolTip(this.chkReplace, "If checked and second option is selected or skip all is pressed, same action will" +
                    " be taken on future occurences, even after program is exited");
            this.chkReplace.UseVisualStyleBackColor = true;
            // 
            // btnSkipAll
            // 
            this.btnSkipAll.Location = new System.Drawing.Point(296, 98);
            this.btnSkipAll.Name = "btnSkipAll";
            this.btnSkipAll.Size = new System.Drawing.Size(75, 23);
            this.btnSkipAll.TabIndex = 8;
            this.btnSkipAll.Text = "Skip all";
            this.toolTip.SetToolTip(this.btnSkipAll, "Skips all files in this renaming process");
            this.btnSkipAll.UseVisualStyleBackColor = true;
            this.btnSkipAll.Click += new System.EventHandler(this.btnSkipAll_Click);
            // 
            // InvalidFilename
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnSkip;
            this.ClientSize = new System.Drawing.Size(383, 133);
            this.Controls.Add(this.btnSkipAll);
            this.Controls.Add(this.chkReplace);
            this.Controls.Add(this.txtReplace);
            this.Controls.Add(this.rbReplace);
            this.Controls.Add(this.rbFilename);
            this.Controls.Add(this.btnSkip);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.lblError);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InvalidFilename";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Invalid Filename Characters";
            this.Load += new System.EventHandler(this.TextDialog_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnSkip;
        private System.Windows.Forms.RadioButton rbFilename;
        private System.Windows.Forms.RadioButton rbReplace;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.CheckBox chkReplace;
        private System.Windows.Forms.Button btnSkipAll;
        private System.Windows.Forms.ToolTip toolTip;
    }
}