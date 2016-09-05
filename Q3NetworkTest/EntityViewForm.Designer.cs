namespace Q3NetworkTest
{
	partial class EntityViewForm
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
			this.tmrRender = new System.Windows.Forms.Timer(this.components);
			this.pnlView = new System.Windows.Forms.Panel();
			this.btnStartRender = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.radBaselines = new System.Windows.Forms.RadioButton();
			this.radParse = new System.Windows.Forms.RadioButton();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tmrRender
			// 
			this.tmrRender.Interval = 40;
			// 
			// pnlView
			// 
			this.pnlView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlView.Location = new System.Drawing.Point(0, 0);
			this.pnlView.Name = "pnlView";
			this.pnlView.Size = new System.Drawing.Size(684, 289);
			this.pnlView.TabIndex = 2;
			// 
			// btnStartRender
			// 
			this.btnStartRender.Location = new System.Drawing.Point(3, 3);
			this.btnStartRender.Name = "btnStartRender";
			this.btnStartRender.Size = new System.Drawing.Size(85, 23);
			this.btnStartRender.TabIndex = 0;
			this.btnStartRender.Text = "Start Render";
			this.btnStartRender.UseVisualStyleBackColor = true;
			this.btnStartRender.Click += new System.EventHandler(this.btnStartRender_Click);
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
			this.splitContainer1.Panel1.Controls.Add(this.radParse);
			this.splitContainer1.Panel1.Controls.Add(this.radBaselines);
			this.splitContainer1.Panel1.Controls.Add(this.btnStartRender);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.pnlView);
			this.splitContainer1.Size = new System.Drawing.Size(684, 327);
			this.splitContainer1.SplitterDistance = 34;
			this.splitContainer1.TabIndex = 3;
			// 
			// radBaselines
			// 
			this.radBaselines.AutoSize = true;
			this.radBaselines.Checked = true;
			this.radBaselines.Location = new System.Drawing.Point(94, 6);
			this.radBaselines.Name = "radBaselines";
			this.radBaselines.Size = new System.Drawing.Size(70, 17);
			this.radBaselines.TabIndex = 1;
			this.radBaselines.TabStop = true;
			this.radBaselines.Text = "Baselines";
			this.radBaselines.UseVisualStyleBackColor = true;
			// 
			// radParse
			// 
			this.radParse.AutoSize = true;
			this.radParse.Location = new System.Drawing.Point(170, 6);
			this.radParse.Name = "radParse";
			this.radParse.Size = new System.Drawing.Size(52, 17);
			this.radParse.TabIndex = 1;
			this.radParse.TabStop = true;
			this.radParse.Text = "Parse";
			this.radParse.UseVisualStyleBackColor = true;
			// 
			// EntityViewForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(684, 327);
			this.Controls.Add(this.splitContainer1);
			this.Name = "EntityViewForm";
			this.Text = "EntityView";
			this.Load += new System.EventHandler(this.EntityViewForm_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EntityViewForm_FormClosing);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer tmrRender;
		private System.Windows.Forms.Panel pnlView;
		private System.Windows.Forms.Button btnStartRender;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.RadioButton radParse;
		private System.Windows.Forms.RadioButton radBaselines;
	}
}