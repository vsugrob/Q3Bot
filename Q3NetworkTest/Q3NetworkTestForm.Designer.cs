namespace Q3NetworkTest
{
	partial class Q3NetworkTestForm
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.qtfServerCommands = new Controls.QuakeTextBox();
			this.txtUserCommands = new System.Windows.Forms.TextBox();
			this.txtDemoFileName = new System.Windows.Forms.TextBox();
			this.btnPlayDemo = new System.Windows.Forms.Button();
			this.txtCommand = new System.Windows.Forms.TextBox();
			this.btnSendCommand = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.txtIP = new System.Windows.Forms.TextBox();
			this.btnConnect = new System.Windows.Forms.Button();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.qtfServerCommands);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.txtUserCommands);
			this.splitContainer1.Panel2.Controls.Add(this.txtDemoFileName);
			this.splitContainer1.Panel2.Controls.Add(this.btnPlayDemo);
			this.splitContainer1.Panel2.Controls.Add(this.txtCommand);
			this.splitContainer1.Panel2.Controls.Add(this.btnSendCommand);
			this.splitContainer1.Panel2.Controls.Add(this.label2);
			this.splitContainer1.Panel2.Controls.Add(this.label1);
			this.splitContainer1.Panel2.Controls.Add(this.txtPort);
			this.splitContainer1.Panel2.Controls.Add(this.txtIP);
			this.splitContainer1.Panel2.Controls.Add(this.btnConnect);
			this.splitContainer1.Size = new System.Drawing.Size(602, 387);
			this.splitContainer1.SplitterDistance = 285;
			this.splitContainer1.TabIndex = 7;
			// 
			// qtfServerCommands
			// 
			this.qtfServerCommands.Dock = System.Windows.Forms.DockStyle.Fill;
			this.qtfServerCommands.Font = new System.Drawing.Font("Courier New", 10F);
			this.qtfServerCommands.Location = new System.Drawing.Point(0, 0);
			this.qtfServerCommands.Name = "qtfServerCommands";
			this.qtfServerCommands.Qtf = "\r\n";
			this.qtfServerCommands.SelectionStart = 0;
			this.qtfServerCommands.Size = new System.Drawing.Size(602, 285);
			this.qtfServerCommands.TabIndex = 7;
			// 
			// txtUserCommands
			// 
			this.txtUserCommands.Location = new System.Drawing.Point(3, 6);
			this.txtUserCommands.Multiline = true;
			this.txtUserCommands.Name = "txtUserCommands";
			this.txtUserCommands.Size = new System.Drawing.Size(121, 77);
			this.txtUserCommands.TabIndex = 16;
			this.txtUserCommands.Text = "Movement here";
			this.txtUserCommands.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.txtUserCommands.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txtUserCommands_MouseMove);
			this.txtUserCommands.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUserCommands_KeyDown);
			this.txtUserCommands.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtUserCommands_KeyUp);
			this.txtUserCommands.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtUserCommands_MouseDown);
			this.txtUserCommands.MouseUp += new System.Windows.Forms.MouseEventHandler(this.txtUserCommands_MouseUp);
			// 
			// txtDemoFileName
			// 
			this.txtDemoFileName.Location = new System.Drawing.Point(130, 63);
			this.txtDemoFileName.Name = "txtDemoFileName";
			this.txtDemoFileName.Size = new System.Drawing.Size(374, 20);
			this.txtDemoFileName.TabIndex = 15;
			this.txtDemoFileName.Text = "C:\\games\\kvaka\\osp\\demos\\demo0000.dm_68";
			// 
			// btnPlayDemo
			// 
			this.btnPlayDemo.Location = new System.Drawing.Point(510, 61);
			this.btnPlayDemo.Name = "btnPlayDemo";
			this.btnPlayDemo.Size = new System.Drawing.Size(75, 23);
			this.btnPlayDemo.TabIndex = 14;
			this.btnPlayDemo.Text = "Play Demo";
			this.btnPlayDemo.UseVisualStyleBackColor = true;
			this.btnPlayDemo.Click += new System.EventHandler(this.btnPlayDemo_Click);
			// 
			// txtCommand
			// 
			this.txtCommand.Location = new System.Drawing.Point(130, 6);
			this.txtCommand.Name = "txtCommand";
			this.txtCommand.Size = new System.Drawing.Size(374, 20);
			this.txtCommand.TabIndex = 12;
			this.txtCommand.TextChanged += new System.EventHandler(this.txtCommand_TextChanged);
			this.txtCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCommand_KeyDown);
			// 
			// btnSendCommand
			// 
			this.btnSendCommand.Location = new System.Drawing.Point(510, 3);
			this.btnSendCommand.Name = "btnSendCommand";
			this.btnSendCommand.Size = new System.Drawing.Size(75, 23);
			this.btnSendCommand.TabIndex = 11;
			this.btnSendCommand.Text = "Send";
			this.btnSendCommand.UseVisualStyleBackColor = true;
			this.btnSendCommand.Click += new System.EventHandler(this.btnSendCommand_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(420, 37);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(29, 13);
			this.label2.TabIndex = 10;
			this.label2.Text = "Port:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(288, 37);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(20, 13);
			this.label1.TabIndex = 9;
			this.label1.Text = "IP:";
			// 
			// txtPort
			// 
			this.txtPort.Location = new System.Drawing.Point(455, 34);
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(49, 20);
			this.txtPort.TabIndex = 7;
			this.txtPort.Text = "27960";
			this.txtPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtIP
			// 
			this.txtIP.Location = new System.Drawing.Point(314, 34);
			this.txtIP.Name = "txtIP";
			this.txtIP.Size = new System.Drawing.Size(100, 20);
			this.txtIP.TabIndex = 8;
			this.txtIP.Text = "127.0.0.1";
			// 
			// btnConnect
			// 
			this.btnConnect.Location = new System.Drawing.Point(510, 32);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(75, 23);
			this.btnConnect.TabIndex = 6;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// Q3NetworkTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(602, 387);
			this.Controls.Add(this.splitContainer1);
			this.Name = "Q3NetworkTestForm";
			this.Text = "Q3Network Test";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Q3NetworkTestForm_FormClosed);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private Controls.QuakeTextBox qtfServerCommands;
		private System.Windows.Forms.TextBox txtCommand;
		private System.Windows.Forms.Button btnSendCommand;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtPort;
		private System.Windows.Forms.TextBox txtIP;
		private System.Windows.Forms.Button btnConnect;
		private System.Windows.Forms.TextBox txtDemoFileName;
		private System.Windows.Forms.Button btnPlayDemo;
		private System.Windows.Forms.TextBox txtUserCommands;

	}
}

