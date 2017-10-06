namespace RollingLEAPOptionsSimulator
{
    partial class Form1
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
            this.usrName = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.loginButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.output = new System.Windows.Forms.TextBox();
            this.sourceIdTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.fileLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // usrName
            // 
            this.usrName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.usrName.Location = new System.Drawing.Point(77, 67);
            this.usrName.MinimumSize = new System.Drawing.Size(75, 30);
            this.usrName.Name = "usrName";
            this.usrName.Size = new System.Drawing.Size(99, 13);
            this.usrName.TabIndex = 0;
            // 
            // password
            // 
            this.password.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.password.Location = new System.Drawing.Point(77, 99);
            this.password.MinimumSize = new System.Drawing.Size(98, 30);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(99, 13);
            this.password.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Username:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Psssword:";
            // 
            // loginButton
            // 
            this.loginButton.Location = new System.Drawing.Point(195, 36);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(75, 37);
            this.loginButton.TabIndex = 4;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.login_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(195, 88);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 35);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancel_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(321, 88);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(97, 35);
            this.button2.TabIndex = 6;
            this.button2.Text = "Refresh";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // output
            // 
            this.output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.output.Location = new System.Drawing.Point(15, 151);
            this.output.Margin = new System.Windows.Forms.Padding(2);
            this.output.Multiline = true;
            this.output.Name = "output";
            this.output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.output.Size = new System.Drawing.Size(530, 448);
            this.output.TabIndex = 7;
            // 
            // sourceIdTextBox
            // 
            this.sourceIdTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sourceIdTextBox.Location = new System.Drawing.Point(77, 40);
            this.sourceIdTextBox.MinimumSize = new System.Drawing.Size(75, 30);
            this.sourceIdTextBox.Name = "sourceIdTextBox";
            this.sourceIdTextBox.Size = new System.Drawing.Size(99, 13);
            this.sourceIdTextBox.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Source Id:";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(97, 25);
            this.button3.TabIndex = 8;
            this.button3.Text = "Select File";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // fileLabel
            // 
            this.fileLabel.AutoSize = true;
            this.fileLabel.Location = new System.Drawing.Point(115, 12);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(182, 13);
            this.fileLabel.TabIndex = 9;
            this.fileLabel.Text = "Please select an Excel template file...";
            this.fileLabel.Click += new System.EventHandler(this.fileLabel_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.loginButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(571, 623);
            this.Controls.Add(this.fileLabel);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.output);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.password);
            this.Controls.Add(this.sourceIdTextBox);
            this.Controls.Add(this.usrName);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox usrName;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox output;
        private System.Windows.Forms.TextBox sourceIdTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label fileLabel;
    }
}

