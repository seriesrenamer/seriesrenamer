namespace Renamer.Dialogs
{
    partial class RegexTester
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
            this.txtText = new System.Windows.Forms.TextBox();
            this.txtRegex = new System.Windows.Forms.TextBox();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btnMatch = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.chkCase = new System.Windows.Forms.CheckBox();
            this.chkRtL = new System.Windows.Forms.CheckBox();
            this.chkShorter = new System.Windows.Forms.CheckBox();
            this.chkSingle = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtText
            // 
            this.txtText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtText.Location = new System.Drawing.Point(12, 12);
            this.txtText.Multiline = true;
            this.txtText.Name = "txtText";
            this.txtText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtText.Size = new System.Drawing.Size(717, 180);
            this.txtText.TabIndex = 0;
            // 
            // txtRegex
            // 
            this.txtRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRegex.Location = new System.Drawing.Point(12, 198);
            this.txtRegex.Name = "txtRegex";
            this.txtRegex.Size = new System.Drawing.Size(717, 20);
            this.txtRegex.TabIndex = 1;
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Location = new System.Drawing.Point(12, 224);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(717, 190);
            this.txtOutput.TabIndex = 2;
            // 
            // btnMatch
            // 
            this.btnMatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMatch.Location = new System.Drawing.Point(573, 438);
            this.btnMatch.Name = "btnMatch";
            this.btnMatch.Size = new System.Drawing.Size(75, 23);
            this.btnMatch.TabIndex = 4;
            this.btnMatch.Text = "Match()";
            this.btnMatch.UseVisualStyleBackColor = true;
            this.btnMatch.Click += new System.EventHandler(this.btnMatch_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(654, 438);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // chkCase
            // 
            this.chkCase.AutoSize = true;
            this.chkCase.Location = new System.Drawing.Point(12, 438);
            this.chkCase.Name = "chkCase";
            this.chkCase.Size = new System.Drawing.Size(83, 17);
            this.chkCase.TabIndex = 6;
            this.chkCase.Text = "Ignore Case";
            this.chkCase.UseVisualStyleBackColor = true;
            // 
            // chkRtL
            // 
            this.chkRtL.AutoSize = true;
            this.chkRtL.Location = new System.Drawing.Point(101, 438);
            this.chkRtL.Name = "chkRtL";
            this.chkRtL.Size = new System.Drawing.Size(84, 17);
            this.chkRtL.TabIndex = 7;
            this.chkRtL.Text = "Right to Left";
            this.chkRtL.UseVisualStyleBackColor = true;
            // 
            // chkShorter
            // 
            this.chkShorter.AutoSize = true;
            this.chkShorter.Location = new System.Drawing.Point(275, 438);
            this.chkShorter.Name = "chkShorter";
            this.chkShorter.Size = new System.Drawing.Size(121, 17);
            this.chkShorter.TabIndex = 8;
            this.chkShorter.Text = "Try shorter Regexes";
            this.chkShorter.UseVisualStyleBackColor = true;
            // 
            // chkSingle
            // 
            this.chkSingle.AutoSize = true;
            this.chkSingle.Location = new System.Drawing.Point(191, 438);
            this.chkSingle.Name = "chkSingle";
            this.chkSingle.Size = new System.Drawing.Size(78, 17);
            this.chkSingle.TabIndex = 9;
            this.chkSingle.Text = "Single Line";
            this.chkSingle.UseVisualStyleBackColor = true;
            // 
            // RegexTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 473);
            this.Controls.Add(this.chkSingle);
            this.Controls.Add(this.chkShorter);
            this.Controls.Add(this.chkRtL);
            this.Controls.Add(this.chkCase);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnMatch);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.txtRegex);
            this.Controls.Add(this.txtText);
            this.Name = "RegexTester";
            this.Text = "RegexTester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.TextBox txtRegex;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnMatch;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox chkCase;
        private System.Windows.Forms.CheckBox chkRtL;
        private System.Windows.Forms.CheckBox chkShorter;
        private System.Windows.Forms.CheckBox chkSingle;
    }
}