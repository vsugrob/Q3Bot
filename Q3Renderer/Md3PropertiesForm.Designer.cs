namespace Q3Renderer
{
	partial class Md3PropertiesForm
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
			this.trvModelProperties = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// trvModelProperties
			// 
			this.trvModelProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.trvModelProperties.Location = new System.Drawing.Point(0, 0);
			this.trvModelProperties.Name = "trvModelProperties";
			this.trvModelProperties.Size = new System.Drawing.Size(276, 376);
			this.trvModelProperties.TabIndex = 1;
			// 
			// Md3PropertiesForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(276, 376);
			this.Controls.Add(this.trvModelProperties);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "Md3PropertiesForm";
			this.Text = "Model Properties";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView trvModelProperties;
	}
}