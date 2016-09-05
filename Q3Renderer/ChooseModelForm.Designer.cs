namespace Q3Renderer
{
	partial class ChooseModelForm
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
			this.lstModels = new System.Windows.Forms.ListView();
			this.clmName = new System.Windows.Forms.ColumnHeader();
			this.clmPath = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// lstModels
			// 
			this.lstModels.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmName,
            this.clmPath});
			this.lstModels.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstModels.Location = new System.Drawing.Point(0, 0);
			this.lstModels.Name = "lstModels";
			this.lstModels.Size = new System.Drawing.Size(437, 428);
			this.lstModels.TabIndex = 0;
			this.lstModels.UseCompatibleStateImageBehavior = false;
			this.lstModels.View = System.Windows.Forms.View.Details;
			this.lstModels.DoubleClick += new System.EventHandler(this.lstModels_DoubleClick);
			this.lstModels.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstModels_KeyDown);
			// 
			// clmName
			// 
			this.clmName.Text = "Model";
			this.clmName.Width = 130;
			// 
			// clmPath
			// 
			this.clmPath.Text = "Path";
			this.clmPath.Width = 300;
			// 
			// ChooseModelForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(437, 428);
			this.Controls.Add(this.lstModels);
			this.Name = "ChooseModelForm";
			this.Text = "ChooseModelForm";
			this.Load += new System.EventHandler(this.ChooseModelForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView lstModels;
		private System.Windows.Forms.ColumnHeader clmName;
		private System.Windows.Forms.ColumnHeader clmPath;
	}
}