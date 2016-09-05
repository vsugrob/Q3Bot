using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Q3Network;

namespace Q3HuffmanTest
{
	public partial class StreamStateForm : Form
	{
		public Q3HuffmanStream stream;
		public Q3HuffmanStream compareWith;

		public StreamStateForm()
		{
			InitializeComponent();
		}

		public void UpdateState () {
			int diff_count = 0;

			txtBlocPtrs.Text = stream.BlocPtrs.ToString ();
			txtBlocNode.Text = stream.BlocNode.ToString ();
			txtFreelist.Text = stream.Freelist.ToString ();

			// Update loc:
			Q3HuffmanStream.Node [] loc  = stream.Loc;
			Q3HuffmanStream.Node [] loc2 = compareWith.Loc;

			lstLoc.Items.Clear ();

			Font f = new Font ( "Courier New", 10 );

			for ( int i = 0 ; i < loc.Length ; i++ ) {
				Q3HuffmanStream.Node n  = loc  [i];
				Q3HuffmanStream.Node n2 = loc2 [i];

				if ( n != null && n.symbol != 0 ) {
					string conclusion = null;
					Color symbolColor = Color.Black;
					Color weightColor = Color.Black;
					Color lineColor = Color.White;

					if ( n2 == null ) {
						conclusion = "Other stream missing this symbol";
						symbolColor = Color.Red;
						lineColor = Color.LightGray;
					} else if ( n2.weight != n.weight ) {
						conclusion = "Other stream's symbol weight differs";
						weightColor = Color.Red;
						lineColor = Color.LightGray;
					}
					
					ListViewItem item = new ListViewItem ( i.ToString () );
					item.ForeColor = conclusion != null ? Color.Red : Color.Black;
					item.SubItems.Add ( new String ( ( char ) n.symbol, 1 ), symbolColor, lineColor, f );
					item.SubItems.Add ( n.weight.ToString (), weightColor, lineColor, f );
					item.SubItems.Add ( conclusion );
					lstLoc.Items.Add ( item );

					if ( conclusion != null )
						diff_count++;
				}
			}

			if ( diff_count > 0 ) {
				lblStatus.Text = string.Format ( "Found {0} difference(s)", diff_count );
				lblStatus.ForeColor = Color.Red;
			} else {
				lblStatus.Text = "Differences not found";
				lblStatus.ForeColor = Color.Black;
			}
		}
	}
}
