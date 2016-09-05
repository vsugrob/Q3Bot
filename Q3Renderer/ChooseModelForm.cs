using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ionic.Zip;

namespace Q3Renderer
{
	public partial class ChooseModelForm : Form
	{
		private Q3ModelViewerForm frmModelViewer = new Q3ModelViewerForm ();

		public ChooseModelForm()
		{
			InitializeComponent();
			Q3FileSystem.InitWithDirectory ( @"C:\games\kvaka\" );
			Q3FileSystem.InitWithDirectory ( @".\" );
			Q3FileSystem.InitWithDirectory ( @"C:\games\American McGee's Alice(tm)\" );
		}

		private void ChooseModelForm_Load(object sender, EventArgs e)
		{
			foreach ( KeyValuePair <string, ZipEntry> keyVal in Q3FileSystem.models ) {
				string modelName = keyVal.Key.Substring ( 0, keyVal.Key.Length - 4 );
				modelName = modelName.Substring ( modelName.LastIndexOf ( '/' ) + 1 );

				ListViewItem item = lstModels.Items.Add ( modelName );
				item.SubItems.Add ( keyVal.Key );
				item.Tag = keyVal.Key;
			}

			this.Left = 20;

			frmModelViewer.Show ();
			frmModelViewer.Location = new Point ( this.Left + this.Width + 20, this.Top );
			frmModelViewer.Update ();
		}

		private void lstModels_DoubleClick(object sender, EventArgs e)
		{
			LoadSelectedModel ();
		}

		private void lstModels_KeyDown(object sender, KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.Enter )
				LoadSelectedModel ();
		}

		private void LoadSelectedModel () {
			if ( frmModelViewer == null )
				frmModelViewer = new Q3ModelViewerForm ();

			ListViewItem item = lstModels.SelectedItems [0];
			frmModelViewer.LoadModel ( ( string ) item.Tag );
		}
	}
}
