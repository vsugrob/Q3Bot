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
	public partial class ChooseMapForm : Form
	{
		private Q3RendererForm frmRenderer;
		private ChooseModelForm frmChooseModel;

		public ChooseMapForm()
		{
			InitializeComponent();
			Q3FileSystem.InitWithDirectory ( @"C:\games\kvaka\" );
			Q3FileSystem.InitWithDirectory ( @".\" );
			Q3FileSystem.InitWithDirectory ( @"C:\games\American McGee's Alice(tm)\" );
		}

		private void ChooseMapForm_Load(object sender, EventArgs e)
		{
			foreach ( KeyValuePair <string, ZipEntry> map in Q3FileSystem.maps ) {
				int lastSlash = map.Key.LastIndexOf ( '/' );
				string mapName = map.Key.Substring ( lastSlash + 1 );
				mapName = mapName.Substring ( 0, mapName.Length - 4 );
				Dictionary <string, string> mapProps;
				
				string longName = "";
				string bots = "";
				string fraglimit = "";
				string type = "";

				if ( Q3FileSystem.arenas.TryGetValue ( mapName, out mapProps ) ) {
					mapProps.TryGetValue ( "bots", out bots );
					mapProps.TryGetValue ( "longname", out longName );
					mapProps.TryGetValue ( "fraglimit", out fraglimit );
					mapProps.TryGetValue ( "type", out type );
				}

				ListViewItem item = new ListViewItem ( mapName );
				item.SubItems.Add ( longName );
				item.SubItems.Add ( bots );
				item.SubItems.Add ( fraglimit );
				item.SubItems.Add ( type );
				item.SubItems.Add ( map.Key );
				item.Tag = map.Key;
				//item.ImageKey = mapName;
				item.ImageIndex = 0;
				lstMaps.Items.Add ( item );
			}

			Image unknownmap = Q3FileSystem.ResourceAsImage ( "menu/art/unknownmap.jpg" );

			if ( unknownmap != null )
				imgLstMaps.Images.Add ( unknownmap, Color.Empty );

			foreach ( ListViewItem item in lstMaps.Items ) {
				Image shot = Q3FileSystem.GetLevelShot ( item.Text );

				if ( shot != null )
					item.ImageIndex = imgLstMaps.Images.Add ( shot, Color.Empty );
			}

			btnListViewStyle.Text = lstMaps.View.ToString ();
		}

		private void btnLoadMap_Click(object sender, EventArgs e)
		{
			LoadSelectedMap ();
		}

		private void lstMaps_DoubleClick(object sender, EventArgs e)
		{
			LoadSelectedMap ();
		}

		private void lstMaps_KeyDown(object sender, KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.Enter )
				LoadSelectedMap ();
		}

		private void LoadSelectedMap () {
			if ( lstMaps.SelectedItems.Count > 0 ) {
				if ( frmRenderer != null )
					frmRenderer.Close ();

				ListViewItem item = lstMaps.SelectedItems [0];
				frmRenderer = new Q3RendererForm ( ( string ) item.Tag );
				frmRenderer.Show ();
			}
		}

		private void btnDetails_Click(object sender, EventArgs e)
		{
			lstMaps.View = View.Details;
			btnListViewStyle.Text = lstMaps.View.ToString ();
		}

		private void btnIcons_Click(object sender, EventArgs e)
		{
			lstMaps.View = View.LargeIcon;
			btnListViewStyle.Text = lstMaps.View.ToString ();
		}

		private void btnOpenModelViewer_Click ( object sender, EventArgs e ) {
			if ( frmChooseModel != null )
				frmChooseModel.Close ();

			frmChooseModel = new ChooseModelForm ();
			frmChooseModel.Show ();
		}
	}
}
