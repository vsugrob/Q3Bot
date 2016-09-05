namespace Controls
{
	partial class QuakeTextBox
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.rtfText = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// rtfText
			// 
			this.rtfText.BackColor = System.Drawing.Color.DarkGray;
			this.rtfText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtfText.Font = new System.Drawing.Font("Courier New", 10F);
			this.rtfText.Location = new System.Drawing.Point(0, 0);
			this.rtfText.Name = "rtfText";
			this.rtfText.Size = new System.Drawing.Size(134, 24);
			this.rtfText.TabIndex = 1;
			this.rtfText.Text = "";
			this.rtfText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtfText_KeyDown);
			this.rtfText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.rtfText_KeyUp);
			// 
			// QuakeTextBox
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.rtfText);
			this.Font = new System.Drawing.Font("Courier New", 10F);
			this.Name = "QuakeTextBox";
			this.Size = new System.Drawing.Size(134, 24);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox rtfText;
	}
}
