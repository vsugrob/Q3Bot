using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using Q3Network;

namespace Q3HuffmanTest
{
	public partial class CompressorTestForm : Form
	{
		protected byte [] recBuf = new byte [1024];
		Q3HuffmanStream cs;	// Compressing stream
		Q3HuffmanStream ds;	// Decompressing stream
		MemoryStream cms;	// Underlying stream for compression
		MemoryStream dms;	// Underlying stream for decompression
		int bytesExpected;	// Bytes expected to decompress

		StreamStateForm frmCompressorState;
		StreamStateForm frmDecompressorState;

		public CompressorTestForm()
		{
			InitializeComponent();

			cms = new MemoryStream ();
			dms = new MemoryStream ();
			cs = new Q3HuffmanStream ( cms, CompressionMode.Compress   );
			ds = new Q3HuffmanStream ( dms, CompressionMode.Decompress );

			frmCompressorState = new StreamStateForm ();
			frmCompressorState.stream = cs;
			frmCompressorState.compareWith = ds;
			frmCompressorState.Text = "Compressor state";

			frmDecompressorState = new StreamStateForm ();
			frmDecompressorState.stream = ds;
			frmDecompressorState.compareWith = cs;
			frmDecompressorState.Text = "Decompressor state";
		}

		private void btnCompress_Click(object sender, EventArgs e)
		{
			cms.SetLength ( 0 );

			// Compress
			byte [] src_buf = Encoding.Default.GetBytes ( txtDecompressed.Text );
			cs.Write ( src_buf, 0, src_buf.Length );
			cs.Flush ();
			bytesExpected = src_buf.Length;	// Notify decompress how many bytes to expect
			txtNumBytes.Text = bytesExpected.ToString ();

			// Display result as hex sequence
			byte [] c_buf = cms.GetBuffer ();
			txtCompressed.Text = "";

			for ( int i = 0 ; i < cms.Length ; i++ ) {
				string escaped = Uri.HexEscape ( ( char ) c_buf [i] );
				txtCompressed.Text += "0x" + escaped.Substring ( 1 ) + " ";
			}

			frmCompressorState.UpdateState ();
			frmDecompressorState.UpdateState ();
		}

		private void btnDecompress_Click(object sender, EventArgs e)
		{
			// Convert hex string to byte sequence
			int c = 0;
			byte [] bytes;

			if ( !chkHexStream.Checked ) {
				string [] hexes = txtCompressed.Text.Split ( ' ' );	if ( hexes.Length == 0 ) return;
				bytes = new byte [hexes.Length];

				for ( int i = 0 ; i < hexes.Length ; i++ ) {
					string hex = hexes [i];	if ( hex.Length > 4 || hex.Length < 3 ) continue;
					string escaped = "%" + hexes [i].Substring ( 2 );
					int index = 0;
					bytes [c] = ( byte ) Uri.HexUnescape ( escaped, ref index );
					c++;
				}
			} else {
				int hex_count = txtCompressed.Text.Length / 2;
				bytes = new byte [hex_count];
				
				for ( int i = 0 ; i < hex_count ; i++ ) {
					string hex = txtCompressed.Text.Substring ( i * 2, 2 );
					string escaped = "%" + hex;
					int index = 0;
					bytes [c] = ( byte ) Uri.HexUnescape ( escaped, ref index );
					c++;
				}
			}

			// Put results to stream
			dms.SetLength ( 0 );
			dms.Write ( bytes, 0, c );
			dms.Position = 0;

			// Decompress
			byte [] d_buf = new byte [bytesExpected];
			ds.ReadNew ( d_buf, 0, d_buf.Length );

			// Display result as human readable string
			txtDecompressed.Text = Encoding.Default.GetString ( d_buf );

			frmCompressorState.UpdateState ();
			frmDecompressorState.UpdateState ();
		}

		private void btnShowCompressorState_Click(object sender, EventArgs e)
		{
			frmCompressorState.Show ();
		}

		private void btnShowDecompressorState_Click(object sender, EventArgs e)
		{
			frmDecompressorState.Show ();
		}

		private void btnResetStreams_Click(object sender, EventArgs e)
		{
			cs = new Q3HuffmanStream ( cms, CompressionMode.Compress   );
			ds = new Q3HuffmanStream ( dms, CompressionMode.Decompress );

			frmCompressorState.stream = cs;
			frmCompressorState.compareWith = ds;

			frmDecompressorState.stream = ds;
			frmDecompressorState.compareWith = cs;

			frmCompressorState.UpdateState ();
			frmDecompressorState.UpdateState ();
		}

		private void btnResetWithQ3Data_Click(object sender, EventArgs e)
		{
			cs = new Q3HuffmanStream ( cms, CompressionMode.Compress   );
			ds = new Q3HuffmanStream ( dms, CompressionMode.Decompress );
			cs.InitWithQ3Data ();
			ds.InitWithQ3Data ();

			frmCompressorState.stream = cs;
			frmCompressorState.compareWith = ds;

			frmDecompressorState.stream = ds;
			frmDecompressorState.compareWith = cs;

			frmCompressorState.UpdateState ();
			frmDecompressorState.UpdateState ();
		}

		private void txtNumBytes_TextChanged(object sender, EventArgs e)
		{
			bytesExpected = Convert.ToInt32 ( txtNumBytes.Text );
		}
	}
}
