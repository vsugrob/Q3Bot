using Controls;

namespace QuakeTextBoxTest
{
	partial class QuakeTextBoxTestForm
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
			this.btnQtf2Rtf = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.txtQtfText = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btnRtf2Qtf = new System.Windows.Forms.Button();
			this.btnIncrementRtf = new System.Windows.Forms.Button();
			this.qtfText = new Controls.QuakeTextBox();
			this.SuspendLayout();
			// 
			// btnQtf2Rtf
			// 
			this.btnQtf2Rtf.Location = new System.Drawing.Point(410, 182);
			this.btnQtf2Rtf.Name = "btnQtf2Rtf";
			this.btnQtf2Rtf.Size = new System.Drawing.Size(35, 23);
			this.btnQtf2Rtf.TabIndex = 1;
			this.btnQtf2Rtf.Text = "<-";
			this.btnQtf2Rtf.UseVisualStyleBackColor = true;
			this.btnQtf2Rtf.Click += new System.EventHandler(this.btnQtf2Rtf_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(448, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(31, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "QTF:";
			// 
			// txtQtfText
			// 
			this.txtQtfText.Font = new System.Drawing.Font("Courier New", 10F);
			this.txtQtfText.Location = new System.Drawing.Point(451, 24);
			this.txtQtfText.Multiline = true;
			this.txtQtfText.Name = "txtQtfText";
			this.txtQtfText.Size = new System.Drawing.Size(381, 385);
			this.txtQtfText.TabIndex = 3;
			this.txtQtfText.Text = "^7in^0S^7.evi^b^0L^N^7ove";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(31, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "RTF:";
			// 
			// btnRtf2Qtf
			// 
			this.btnRtf2Qtf.Location = new System.Drawing.Point(410, 211);
			this.btnRtf2Qtf.Name = "btnRtf2Qtf";
			this.btnRtf2Qtf.Size = new System.Drawing.Size(35, 23);
			this.btnRtf2Qtf.TabIndex = 1;
			this.btnRtf2Qtf.Text = "->";
			this.btnRtf2Qtf.UseVisualStyleBackColor = true;
			this.btnRtf2Qtf.Click += new System.EventHandler(this.btnRtf2Qtf_Click);
			// 
			// btnIncrementRtf
			// 
			this.btnIncrementRtf.Location = new System.Drawing.Point(410, 153);
			this.btnIncrementRtf.Name = "btnIncrementRtf";
			this.btnIncrementRtf.Size = new System.Drawing.Size(35, 23);
			this.btnIncrementRtf.TabIndex = 7;
			this.btnIncrementRtf.Text = "<++";
			this.btnIncrementRtf.UseVisualStyleBackColor = true;
			this.btnIncrementRtf.Click += new System.EventHandler(this.btnIncrementRtf_Click);
			// 
			// qtfText
			// 
			this.qtfText.Font = new System.Drawing.Font("Courier New", 10F);
			this.qtfText.Location = new System.Drawing.Point(12, 25);
			this.qtfText.Name = "qtfText";
			this.qtfText.Qtf = "\r\n";
			this.qtfText.SelectionStart = 0;
			this.qtfText.Size = new System.Drawing.Size(392, 384);
			this.qtfText.TabIndex = 6;
			// 
			// QuakeTextBoxTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(844, 423);
			this.Controls.Add(this.btnIncrementRtf);
			this.Controls.Add(this.qtfText);
			this.Controls.Add(this.txtQtfText);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnRtf2Qtf);
			this.Controls.Add(this.btnQtf2Rtf);
			this.Name = "QuakeTextBoxTestForm";
			this.Text = "QTF<->RTF";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnQtf2Rtf;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtQtfText;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnRtf2Qtf;
		private QuakeTextBox qtfText;
		private System.Windows.Forms.Button btnIncrementRtf;
	}
}

