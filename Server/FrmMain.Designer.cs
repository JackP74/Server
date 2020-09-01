namespace Server
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.ConsoleMain = new ConsoleControl.ShellControl();
            this.SuspendLayout();
            // 
            // ConsoleMain
            // 
            this.ConsoleMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConsoleMain.Font = new System.Drawing.Font("Cascadia Code", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConsoleMain.Location = new System.Drawing.Point(0, 0);
            this.ConsoleMain.Margin = new System.Windows.Forms.Padding(0);
            this.ConsoleMain.Name = "ConsoleMain";
            this.ConsoleMain.Prompt = "# ";
            this.ConsoleMain.ShellTextBackColor = System.Drawing.Color.Black;
            this.ConsoleMain.ShellTextFont = new System.Drawing.Font("Cascadia Code", 9F);
            this.ConsoleMain.ShellTextForeColor = System.Drawing.Color.LawnGreen;
            this.ConsoleMain.Size = new System.Drawing.Size(645, 377);
            this.ConsoleMain.TabIndex = 0;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 377);
            this.Controls.Add(this.ConsoleMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server";
            this.ResumeLayout(false);

        }

        #endregion

        private ConsoleControl.ShellControl ConsoleMain;
    }
}

