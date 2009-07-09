namespace Renamer.Dialogs
{
    partial class CollidingFiles
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSkipFirst = new System.Windows.Forms.Button();
            this.btnSkipSecond = new System.Windows.Forms.Button();
            this.btnRenameFirst = new System.Windows.Forms.Button();
            this.btnRenameSecond = new System.Windows.Forms.Button();
            this.btnMoveFirst = new System.Windows.Forms.Button();
            this.btnMoveSecond = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Cannot move";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 22);
            this.label2.MaximumSize = new System.Drawing.Size(500, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "to";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 48);
            this.label4.MaximumSize = new System.Drawing.Size(500, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "label4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(414, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "because there is already a file in that place or another one is meant to be moved" +
                " there.";
            // 
            // btnSkipFirst
            // 
            this.btnSkipFirst.Location = new System.Drawing.Point(7, 77);
            this.btnSkipFirst.Name = "btnSkipFirst";
            this.btnSkipFirst.Size = new System.Drawing.Size(75, 23);
            this.btnSkipFirst.TabIndex = 5;
            this.btnSkipFirst.Text = "Skip first";
            this.btnSkipFirst.UseVisualStyleBackColor = true;
            this.btnSkipFirst.Click += new System.EventHandler(this.btnSkipFirst_Click);
            // 
            // btnSkipSecond
            // 
            this.btnSkipSecond.Location = new System.Drawing.Point(250, 77);
            this.btnSkipSecond.Name = "btnSkipSecond";
            this.btnSkipSecond.Size = new System.Drawing.Size(75, 23);
            this.btnSkipSecond.TabIndex = 6;
            this.btnSkipSecond.Text = "Skip second";
            this.btnSkipSecond.UseVisualStyleBackColor = true;
            this.btnSkipSecond.Click += new System.EventHandler(this.btnSkipSecond_Click);
            // 
            // btnRenameFirst
            // 
            this.btnRenameFirst.Location = new System.Drawing.Point(88, 77);
            this.btnRenameFirst.Name = "btnRenameFirst";
            this.btnRenameFirst.Size = new System.Drawing.Size(75, 23);
            this.btnRenameFirst.TabIndex = 7;
            this.btnRenameFirst.Text = "Rename first";
            this.btnRenameFirst.UseVisualStyleBackColor = true;
            this.btnRenameFirst.Click += new System.EventHandler(this.btnRenameFirst_Click);
            // 
            // btnRenameSecond
            // 
            this.btnRenameSecond.Location = new System.Drawing.Point(331, 77);
            this.btnRenameSecond.Name = "btnRenameSecond";
            this.btnRenameSecond.Size = new System.Drawing.Size(96, 23);
            this.btnRenameSecond.TabIndex = 8;
            this.btnRenameSecond.Text = "Rename Second";
            this.btnRenameSecond.UseVisualStyleBackColor = true;
            this.btnRenameSecond.Click += new System.EventHandler(this.btnRenameSecond_Click);
            // 
            // btnMoveFirst
            // 
            this.btnMoveFirst.Location = new System.Drawing.Point(169, 77);
            this.btnMoveFirst.Name = "btnMoveFirst";
            this.btnMoveFirst.Size = new System.Drawing.Size(75, 23);
            this.btnMoveFirst.TabIndex = 9;
            this.btnMoveFirst.Text = "Move first";
            this.btnMoveFirst.UseVisualStyleBackColor = true;
            this.btnMoveFirst.Click += new System.EventHandler(this.btnMoveFirst_Click);
            // 
            // btnMoveSecond
            // 
            this.btnMoveSecond.Location = new System.Drawing.Point(433, 77);
            this.btnMoveSecond.Name = "btnMoveSecond";
            this.btnMoveSecond.Size = new System.Drawing.Size(95, 23);
            this.btnMoveSecond.TabIndex = 10;
            this.btnMoveSecond.Text = "Move Second";
            this.btnMoveSecond.UseVisualStyleBackColor = true;
            this.btnMoveSecond.Click += new System.EventHandler(this.btnMoveSecond_Click);
            // 
            // CollidingFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 113);
            this.Controls.Add(this.btnMoveSecond);
            this.Controls.Add(this.btnMoveFirst);
            this.Controls.Add(this.btnRenameSecond);
            this.Controls.Add(this.btnRenameFirst);
            this.Controls.Add(this.btnSkipSecond);
            this.Controls.Add(this.btnSkipFirst);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "CollidingFiles";
            this.Text = "File already exists";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSkipFirst;
        private System.Windows.Forms.Button btnSkipSecond;
        private System.Windows.Forms.Button btnRenameFirst;
        private System.Windows.Forms.Button btnRenameSecond;
        private System.Windows.Forms.Button btnMoveFirst;
        private System.Windows.Forms.Button btnMoveSecond;
    }
}