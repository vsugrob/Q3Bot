namespace Q3HuffmanTest
{
	partial class CompressorTestForm
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
			this.txtDecompressed = new System.Windows.Forms.TextBox();
			this.btnCompress = new System.Windows.Forms.Button();
			this.txtCompressed = new System.Windows.Forms.TextBox();
			this.btnDecompress = new System.Windows.Forms.Button();
			this.btnShowCompressorState = new System.Windows.Forms.Button();
			this.btnShowDecompressorState = new System.Windows.Forms.Button();
			this.btnResetStreams = new System.Windows.Forms.Button();
			this.btnResetWithQ3Data = new System.Windows.Forms.Button();
			this.chkHexStream = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtNumBytes = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// txtDecompressed
			// 
			this.txtDecompressed.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.txtDecompressed.Location = new System.Drawing.Point(12, 12);
			this.txtDecompressed.Name = "txtDecompressed";
			this.txtDecompressed.Size = new System.Drawing.Size(284, 23);
			this.txtDecompressed.TabIndex = 0;
			this.txtDecompressed.Text = "aa bbb c";
			// 
			// btnCompress
			// 
			this.btnCompress.Location = new System.Drawing.Point(302, 10);
			this.btnCompress.Name = "btnCompress";
			this.btnCompress.Size = new System.Drawing.Size(75, 23);
			this.btnCompress.TabIndex = 1;
			this.btnCompress.Text = "Compress";
			this.btnCompress.UseVisualStyleBackColor = true;
			this.btnCompress.Click += new System.EventHandler(this.btnCompress_Click);
			// 
			// txtCompressed
			// 
			this.txtCompressed.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.txtCompressed.Location = new System.Drawing.Point(12, 38);
			this.txtCompressed.Name = "txtCompressed";
			this.txtCompressed.Size = new System.Drawing.Size(284, 23);
			this.txtCompressed.TabIndex = 2;
			// 
			// btnDecompress
			// 
			this.btnDecompress.Location = new System.Drawing.Point(302, 39);
			this.btnDecompress.Name = "btnDecompress";
			this.btnDecompress.Size = new System.Drawing.Size(75, 23);
			this.btnDecompress.TabIndex = 3;
			this.btnDecompress.Text = "Decompress";
			this.btnDecompress.UseVisualStyleBackColor = true;
			this.btnDecompress.Click += new System.EventHandler(this.btnDecompress_Click);
			// 
			// btnShowCompressorState
			// 
			this.btnShowCompressorState.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnShowCompressorState.Location = new System.Drawing.Point(383, 10);
			this.btnShowCompressorState.Name = "btnShowCompressorState";
			this.btnShowCompressorState.Size = new System.Drawing.Size(38, 23);
			this.btnShowCompressorState.TabIndex = 4;
			this.btnShowCompressorState.Text = "i";
			this.btnShowCompressorState.UseVisualStyleBackColor = true;
			this.btnShowCompressorState.Click += new System.EventHandler(this.btnShowCompressorState_Click);
			// 
			// btnShowDecompressorState
			// 
			this.btnShowDecompressorState.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnShowDecompressorState.Location = new System.Drawing.Point(383, 39);
			this.btnShowDecompressorState.Name = "btnShowDecompressorState";
			this.btnShowDecompressorState.Size = new System.Drawing.Size(38, 23);
			this.btnShowDecompressorState.TabIndex = 4;
			this.btnShowDecompressorState.Text = "i";
			this.btnShowDecompressorState.UseVisualStyleBackColor = true;
			this.btnShowDecompressorState.Click += new System.EventHandler(this.btnShowDecompressorState_Click);
			// 
			// btnResetStreams
			// 
			this.btnResetStreams.Location = new System.Drawing.Point(12, 67);
			this.btnResetStreams.Name = "btnResetStreams";
			this.btnResetStreams.Size = new System.Drawing.Size(88, 23);
			this.btnResetStreams.TabIndex = 5;
			this.btnResetStreams.Text = "Reset Streams";
			this.btnResetStreams.UseVisualStyleBackColor = true;
			this.btnResetStreams.Click += new System.EventHandler(this.btnResetStreams_Click);
			// 
			// btnResetWithQ3Data
			// 
			this.btnResetWithQ3Data.Location = new System.Drawing.Point(106, 67);
			this.btnResetWithQ3Data.Name = "btnResetWithQ3Data";
			this.btnResetWithQ3Data.Size = new System.Drawing.Size(120, 23);
			this.btnResetWithQ3Data.TabIndex = 6;
			this.btnResetWithQ3Data.Text = "Reset with q3 data";
			this.btnResetWithQ3Data.UseVisualStyleBackColor = true;
			this.btnResetWithQ3Data.Click += new System.EventHandler(this.btnResetWithQ3Data_Click);
			// 
			// chkHexStream
			// 
			this.chkHexStream.AutoSize = true;
			this.chkHexStream.Location = new System.Drawing.Point(427, 41);
			this.chkHexStream.Name = "chkHexStream";
			this.chkHexStream.Size = new System.Drawing.Size(81, 17);
			this.chkHexStream.TabIndex = 7;
			this.chkHexStream.Text = "Hex Stream";
			this.chkHexStream.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(501, 42);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "| Num bytes:";
			// 
			// txtNumBytes
			// 
			this.txtNumBytes.Location = new System.Drawing.Point(566, 38);
			this.txtNumBytes.Name = "txtNumBytes";
			this.txtNumBytes.Size = new System.Drawing.Size(51, 20);
			this.txtNumBytes.TabIndex = 9;
			this.txtNumBytes.TextChanged += new System.EventHandler(this.txtNumBytes_TextChanged);
			// 
			// CompressorTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(629, 101);
			this.Controls.Add(this.txtNumBytes);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.chkHexStream);
			this.Controls.Add(this.btnResetWithQ3Data);
			this.Controls.Add(this.btnResetStreams);
			this.Controls.Add(this.btnShowDecompressorState);
			this.Controls.Add(this.btnShowCompressorState);
			this.Controls.Add(this.btnDecompress);
			this.Controls.Add(this.txtCompressed);
			this.Controls.Add(this.btnCompress);
			this.Controls.Add(this.txtDecompressed);
			this.Name = "CompressorTestForm";
			this.Text = "Q3HuffmanStream test";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtDecompressed;
		private System.Windows.Forms.Button btnCompress;
		private System.Windows.Forms.TextBox txtCompressed;
		private System.Windows.Forms.Button btnDecompress;
		private System.Windows.Forms.Button btnShowCompressorState;
		private System.Windows.Forms.Button btnShowDecompressorState;
		private System.Windows.Forms.Button btnResetStreams;
		private System.Windows.Forms.Button btnResetWithQ3Data;
		private System.Windows.Forms.CheckBox chkHexStream;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtNumBytes;
	}
}

