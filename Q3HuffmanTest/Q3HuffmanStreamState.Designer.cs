namespace Q3HuffmanTest
{
	partial class StreamStateForm
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
			this.txtBlocNode = new System.Windows.Forms.TextBox();
			this.txtBlocPtrs = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lstLoc = new System.Windows.Forms.ListView();
			this.clmIndex = new System.Windows.Forms.ColumnHeader();
			this.clmSymbol = new System.Windows.Forms.ColumnHeader();
			this.clmWeight = new System.Windows.Forms.ColumnHeader();
			this.clmConclusion = new System.Windows.Forms.ColumnHeader();
			this.txtFreelist = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.lblStatus = new System.Windows.Forms.Label();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.lblStatus);
			this.splitContainer1.Panel1.Controls.Add(this.txtFreelist);
			this.splitContainer1.Panel1.Controls.Add(this.label3);
			this.splitContainer1.Panel1.Controls.Add(this.txtBlocNode);
			this.splitContainer1.Panel1.Controls.Add(this.txtBlocPtrs);
			this.splitContainer1.Panel1.Controls.Add(this.label2);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.lstLoc);
			this.splitContainer1.Size = new System.Drawing.Size(663, 360);
			this.splitContainer1.SplitterDistance = 105;
			this.splitContainer1.TabIndex = 2;
			// 
			// txtBlocNode
			// 
			this.txtBlocNode.Enabled = false;
			this.txtBlocNode.Location = new System.Drawing.Point(74, 61);
			this.txtBlocNode.Name = "txtBlocNode";
			this.txtBlocNode.ReadOnly = true;
			this.txtBlocNode.Size = new System.Drawing.Size(60, 20);
			this.txtBlocNode.TabIndex = 1;
			// 
			// txtBlocPtrs
			// 
			this.txtBlocPtrs.Enabled = false;
			this.txtBlocPtrs.Location = new System.Drawing.Point(74, 35);
			this.txtBlocPtrs.Name = "txtBlocPtrs";
			this.txtBlocPtrs.ReadOnly = true;
			this.txtBlocPtrs.Size = new System.Drawing.Size(60, 20);
			this.txtBlocPtrs.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "blocNode:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 38);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "blocPtrs:";
			// 
			// lstLoc
			// 
			this.lstLoc.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmIndex,
            this.clmSymbol,
            this.clmWeight,
            this.clmConclusion});
			this.lstLoc.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstLoc.Font = new System.Drawing.Font("Courier New", 10F);
			this.lstLoc.Location = new System.Drawing.Point(0, 0);
			this.lstLoc.Name = "lstLoc";
			this.lstLoc.Size = new System.Drawing.Size(663, 251);
			this.lstLoc.TabIndex = 2;
			this.lstLoc.UseCompatibleStateImageBehavior = false;
			this.lstLoc.View = System.Windows.Forms.View.Details;
			// 
			// clmIndex
			// 
			this.clmIndex.Text = "Index";
			// 
			// clmSymbol
			// 
			this.clmSymbol.Text = "Symbol";
			// 
			// clmWeight
			// 
			this.clmWeight.Text = "Weight";
			// 
			// clmConclusion
			// 
			this.clmConclusion.Text = "Conclusion";
			this.clmConclusion.Width = 430;
			// 
			// txtFreelist
			// 
			this.txtFreelist.Enabled = false;
			this.txtFreelist.Location = new System.Drawing.Point(202, 35);
			this.txtFreelist.Name = "txtFreelist";
			this.txtFreelist.ReadOnly = true;
			this.txtFreelist.Size = new System.Drawing.Size(60, 20);
			this.txtFreelist.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(140, 38);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(40, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "freelist:";
			// 
			// lblStatus
			// 
			this.lblStatus.AutoSize = true;
			this.lblStatus.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblStatus.Location = new System.Drawing.Point(12, 9);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(185, 18);
			this.lblStatus.TabIndex = 4;
			this.lblStatus.Text = "Differences not found";
			// 
			// StreamStateForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(663, 360);
			this.Controls.Add(this.splitContainer1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "StreamStateForm";
			this.Text = "Stream State";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListView lstLoc;
		private System.Windows.Forms.ColumnHeader clmIndex;
		private System.Windows.Forms.ColumnHeader clmSymbol;
		private System.Windows.Forms.ColumnHeader clmWeight;
		private System.Windows.Forms.ColumnHeader clmConclusion;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtBlocPtrs;
		private System.Windows.Forms.TextBox txtBlocNode;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtFreelist;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblStatus;
	}
}