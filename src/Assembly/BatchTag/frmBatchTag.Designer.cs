namespace Assembly
{
    partial class frmStatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStatus));
            this.pbMap = new System.Windows.Forms.ProgressBar();
            this.pbTag = new System.Windows.Forms.ProgressBar();
            this.txtMap = new System.Windows.Forms.TextBox();
            this.txtTag = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // pbMap
            // 
            this.pbMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pbMap.ForeColor = System.Drawing.Color.HotPink;
            this.pbMap.Location = new System.Drawing.Point(10, 11);
            this.pbMap.Name = "pbMap";
            this.pbMap.Size = new System.Drawing.Size(146, 20);
            this.pbMap.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbMap.TabIndex = 0;
            this.pbMap.UseWaitCursor = true;
            this.pbMap.Click += new System.EventHandler(this.pbMap_Click);
            // 
            // pbTag
            // 
            this.pbTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pbTag.ForeColor = System.Drawing.Color.HotPink;
            this.pbTag.Location = new System.Drawing.Point(10, 42);
            this.pbTag.Name = "pbTag";
            this.pbTag.Size = new System.Drawing.Size(146, 20);
            this.pbTag.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbTag.TabIndex = 3;
            this.pbTag.UseWaitCursor = true;
            // 
            // txtMap
            // 
            this.txtMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtMap.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtMap.Location = new System.Drawing.Point(162, 10);
            this.txtMap.Name = "txtMap";
            this.txtMap.ReadOnly = true;
            this.txtMap.Size = new System.Drawing.Size(600, 22);
            this.txtMap.TabIndex = 4;
            this.txtMap.UseWaitCursor = true;
            // 
            // txtTag
            // 
            this.txtTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtTag.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtTag.Location = new System.Drawing.Point(162, 41);
            this.txtTag.Name = "txtTag";
            this.txtTag.ReadOnly = true;
            this.txtTag.Size = new System.Drawing.Size(600, 22);
            this.txtTag.TabIndex = 5;
            this.txtTag.UseWaitCursor = true;
            // 
            // frmStatus
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.ProgressBar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(770, 71);
            this.ControlBox = false;
            this.Controls.Add(this.txtTag);
            this.Controls.Add(this.txtMap);
            this.Controls.Add(this.pbTag);
            this.Controls.Add(this.pbMap);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmStatus";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Applying template to open maps...";
            this.TopMost = true;
            this.UseWaitCursor = true;
            this.Load += new System.EventHandler(this.frmStatus_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbMap;
        private System.Windows.Forms.ProgressBar pbTag;
        private System.Windows.Forms.TextBox txtMap;
        private System.Windows.Forms.TextBox txtTag;
    }
}