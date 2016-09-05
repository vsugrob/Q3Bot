namespace Q3Renderer
{
	partial class Q3ModelViewerForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Q3ModelViewerForm));
			this.tlsViewerOptions = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.lblSubmeshId = new System.Windows.Forms.ToolStripLabel();
			this.btnNextSubmesh = new System.Windows.Forms.ToolStripButton();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.tlsViewerOptions.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tlsViewerOptions
			// 
			this.tlsViewerOptions.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.tlsViewerOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.lblSubmeshId,
            this.btnNextSubmesh});
			this.tlsViewerOptions.Location = new System.Drawing.Point(0, 0);
			this.tlsViewerOptions.Name = "tlsViewerOptions";
			this.tlsViewerOptions.Size = new System.Drawing.Size(832, 27);
			this.tlsViewerOptions.TabIndex = 0;
			this.tlsViewerOptions.Text = "toolStrip1";
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(89, 24);
			this.toolStripLabel1.Text = "Submesh Id:";
			// 
			// lblSubmeshId
			// 
			this.lblSubmeshId.Name = "lblSubmeshId";
			this.lblSubmeshId.Size = new System.Drawing.Size(17, 24);
			this.lblSubmeshId.Text = "0";
			// 
			// btnNextSubmesh
			// 
			this.btnNextSubmesh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnNextSubmesh.Image = ((System.Drawing.Image)(resources.GetObject("btnNextSubmesh.Image")));
			this.btnNextSubmesh.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnNextSubmesh.Name = "btnNextSubmesh";
			this.btnNextSubmesh.Size = new System.Drawing.Size(44, 24);
			this.btnNextSubmesh.Text = "&Next";
			this.btnNextSubmesh.Click += new System.EventHandler(this.btnNextSubmesh_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 521);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(832, 25);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(404, 20);
			this.toolStripStatusLabel1.Text = "Controls: E, F, D, S to move in horizontal space, Space to lift.";
			// 
			// Q3ModelViewerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(832, 546);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.tlsViewerOptions);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MinimumSize = new System.Drawing.Size(194, 112);
			this.Name = "Q3ModelViewerForm";
			this.Text = "Q3 Model Viewer";
			this.Load += new System.EventHandler(this.Q3ModelViewerForm_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Q3ModelViewerForm_Paint);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Q3ModelViewerForm_KeyDown);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Q3ModelViewerForm_MouseDown);
			this.tlsViewerOptions.ResumeLayout(false);
			this.tlsViewerOptions.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip tlsViewerOptions;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripLabel lblSubmeshId;
		private System.Windows.Forms.ToolStripButton btnNextSubmesh;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
	}
}