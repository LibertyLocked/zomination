namespace Launcher
{
    partial class FormMsgbox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMsgbox));
            this.closeButton = new System.Windows.Forms.Label();
            this.messageLabel1 = new System.Windows.Forms.Label();
            this.messageLabel2 = new System.Windows.Forms.Label();
            this.confirmButton = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.BackColor = System.Drawing.Color.Transparent;
            this.closeButton.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.closeButton.Location = new System.Drawing.Point(266, 77);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(80, 30);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "CLOSE";
            this.closeButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.closeButton.Click += new System.EventHandler(this.okayButton_Click);
            this.closeButton.MouseEnter += new System.EventHandler(this.buttonEffects_MouseEnter);
            this.closeButton.MouseLeave += new System.EventHandler(this.buttonEffects_MouseLeave);
            // 
            // messageLabel1
            // 
            this.messageLabel1.AutoSize = true;
            this.messageLabel1.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.messageLabel1.Location = new System.Drawing.Point(12, 18);
            this.messageLabel1.Name = "messageLabel1";
            this.messageLabel1.Size = new System.Drawing.Size(103, 19);
            this.messageLabel1.TabIndex = 1;
            this.messageLabel1.Text = "message title";
            this.messageLabel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormMessagebox_MouseDown);
            // 
            // messageLabel2
            // 
            this.messageLabel2.AutoSize = true;
            this.messageLabel2.BackColor = System.Drawing.Color.Transparent;
            this.messageLabel2.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageLabel2.ForeColor = System.Drawing.Color.White;
            this.messageLabel2.Location = new System.Drawing.Point(12, 46);
            this.messageLabel2.MaximumSize = new System.Drawing.Size(334, 0);
            this.messageLabel2.Name = "messageLabel2";
            this.messageLabel2.Size = new System.Drawing.Size(103, 16);
            this.messageLabel2.TabIndex = 2;
            this.messageLabel2.Text = "message detail";
            this.messageLabel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormMessagebox_MouseDown);
            // 
            // confirmButton
            // 
            this.confirmButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.confirmButton.BackColor = System.Drawing.Color.Transparent;
            this.confirmButton.Enabled = false;
            this.confirmButton.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.confirmButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.confirmButton.Location = new System.Drawing.Point(180, 77);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(80, 30);
            this.confirmButton.TabIndex = 3;
            this.confirmButton.Text = "OK";
            this.confirmButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.confirmButton.Visible = false;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            this.confirmButton.MouseEnter += new System.EventHandler(this.buttonEffects_MouseEnter);
            this.confirmButton.MouseLeave += new System.EventHandler(this.buttonEffects_MouseLeave);
            // 
            // FormMsgbox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(358, 116);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.messageLabel2);
            this.Controls.Add(this.messageLabel1);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMsgbox";
            this.ShowInTaskbar = false;
            this.Text = "formMessagebox";
            this.Load += new System.EventHandler(this.FormMessagebox_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormMessagebox_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label closeButton;
        private System.Windows.Forms.Label messageLabel1;
        private System.Windows.Forms.Label messageLabel2;
        private System.Windows.Forms.Label confirmButton;
    }
}