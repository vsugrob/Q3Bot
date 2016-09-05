namespace Q3Renderer
{
	partial class ChooseMapForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseMapForm));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.btnLoadMap = new System.Windows.Forms.Button();
			this.imgLstMaps = new System.Windows.Forms.ImageList(this.components);
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.btnListViewStyle = new System.Windows.Forms.ToolStripDropDownButton();
			this.btnDetails = new System.Windows.Forms.ToolStripMenuItem();
			this.btnIcons = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.lstMaps = new System.Windows.Forms.ListView();
			this.clmMap = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.clmLongname = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.clmBots = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.clmFraglimit = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.clmType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Path = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.btnOpenModelViewer = new System.Windows.Forms.ToolStripButton();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.btnLoadMap);
			this.splitContainer1.Size = new System.Drawing.Size(967, 373);
			this.splitContainer1.SplitterDistance = 339;
			this.splitContainer1.SplitterWidth = 5;
			this.splitContainer1.TabIndex = 1;
			// 
			// btnLoadMap
			// 
			this.btnLoadMap.Location = new System.Drawing.Point(851, 4);
			this.btnLoadMap.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnLoadMap.Name = "btnLoadMap";
			this.btnLoadMap.Size = new System.Drawing.Size(100, 28);
			this.btnLoadMap.TabIndex = 0;
			this.btnLoadMap.Text = "Load";
			this.btnLoadMap.UseVisualStyleBackColor = true;
			this.btnLoadMap.Click += new System.EventHandler(this.btnLoadMap_Click);
			// 
			// imgLstMaps
			// 
			this.imgLstMaps.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.imgLstMaps.ImageSize = new System.Drawing.Size(256, 192);
			this.imgLstMaps.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnListViewStyle,
            this.btnOpenModelViewer});
			this.toolStrip1.Location = new System.Drawing.Point(3, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(225, 27);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// btnListViewStyle
			// 
			this.btnListViewStyle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnListViewStyle.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDetails,
            this.btnIcons});
			this.btnListViewStyle.Image = ((System.Drawing.Image)(resources.GetObject("btnListViewStyle.Image")));
			this.btnListViewStyle.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnListViewStyle.Name = "btnListViewStyle";
			this.btnListViewStyle.Size = new System.Drawing.Size(69, 24);
			this.btnListViewStyle.Text = "Details";
			// 
			// btnDetails
			// 
			this.btnDetails.Name = "btnDetails";
			this.btnDetails.Size = new System.Drawing.Size(181, 26);
			this.btnDetails.Text = "Details";
			this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
			// 
			// btnIcons
			// 
			this.btnIcons.Name = "btnIcons";
			this.btnIcons.Size = new System.Drawing.Size(181, 26);
			this.btnIcons.Text = "LargeIcon";
			this.btnIcons.Click += new System.EventHandler(this.btnIcons_Click);
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.lstMaps);
			this.toolStripContainer1.ContentPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(967, 346);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(967, 373);
			this.toolStripContainer1.TabIndex = 2;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
			// 
			// lstMaps
			// 
			this.lstMaps.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmMap,
            this.clmLongname,
            this.clmBots,
            this.clmFraglimit,
            this.clmType,
            this.Path});
			this.lstMaps.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstMaps.LargeImageList = this.imgLstMaps;
			this.lstMaps.Location = new System.Drawing.Point(0, 0);
			this.lstMaps.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.lstMaps.MultiSelect = false;
			this.lstMaps.Name = "lstMaps";
			this.lstMaps.Size = new System.Drawing.Size(967, 346);
			this.lstMaps.TabIndex = 1;
			this.lstMaps.UseCompatibleStateImageBehavior = false;
			this.lstMaps.View = System.Windows.Forms.View.Details;
			this.lstMaps.DoubleClick += new System.EventHandler(this.lstMaps_DoubleClick);
			this.lstMaps.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstMaps_KeyDown);
			// 
			// clmMap
			// 
			this.clmMap.Text = "Map";
			// 
			// clmLongname
			// 
			this.clmLongname.Text = "Longname";
			this.clmLongname.Width = 153;
			// 
			// clmBots
			// 
			this.clmBots.Text = "Bots";
			this.clmBots.Width = 150;
			// 
			// clmFraglimit
			// 
			this.clmFraglimit.Text = "Fraglimit";
			// 
			// clmType
			// 
			this.clmType.Text = "Type";
			this.clmType.Width = 150;
			// 
			// Path
			// 
			this.Path.Text = "Path";
			this.Path.Width = 200;
			// 
			// btnOpenModelViewer
			// 
			this.btnOpenModelViewer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnOpenModelViewer.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenModelViewer.Image")));
			this.btnOpenModelViewer.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnOpenModelViewer.Name = "btnOpenModelViewer";
			this.btnOpenModelViewer.Size = new System.Drawing.Size(105, 24);
			this.btnOpenModelViewer.Text = "Model Viewer";
			this.btnOpenModelViewer.Click += new System.EventHandler(this.btnOpenModelViewer_Click);
			// 
			// ChooseMapForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(967, 373);
			this.Controls.Add(this.toolStripContainer1);
			this.Controls.Add(this.splitContainer1);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "ChooseMapForm";
			this.Text = "Choose Map";
			this.Load += new System.EventHandler(this.ChooseMapForm_Load);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button btnLoadMap;
		private System.Windows.Forms.ImageList imgLstMaps;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.ListView lstMaps;
		private System.Windows.Forms.ColumnHeader clmMap;
		private System.Windows.Forms.ColumnHeader clmLongname;
		private System.Windows.Forms.ColumnHeader clmBots;
		private System.Windows.Forms.ColumnHeader clmFraglimit;
		private System.Windows.Forms.ColumnHeader clmType;
		private System.Windows.Forms.ColumnHeader Path;
		private System.Windows.Forms.ToolStripDropDownButton btnListViewStyle;
		private System.Windows.Forms.ToolStripMenuItem btnDetails;
		private System.Windows.Forms.ToolStripMenuItem btnIcons;
		private System.Windows.Forms.ToolStripButton btnOpenModelViewer;
	}
}