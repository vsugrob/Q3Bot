using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Q3Renderer
{
	public partial class Md3PropertiesForm : Form
	{
		public Md3PropertiesForm()
		{
			InitializeComponent();
		}

		public void SetModel ( Md3Model model ) {
			this.Text = string.Format ( @"Model '{0}' Properties", model.Name );

			trvModelProperties.Nodes.Clear ();
			TreeNode root = trvModelProperties.Nodes.Add ( model.Name );
			TreeNode texturesNode = root.Nodes.Add ( string.Format ( "Textures ({0}): ", model.textures.Count ) );

			foreach ( string filename in model.realnames )
				texturesNode.Nodes.Add ( filename );

			root.Nodes.Add ( "Num Vertices: " + model.TotalVertices.ToString () );
			root.Nodes.Add ( "Num Faces: " + model.TotalFaces.ToString () );
			root.Nodes.Add ( "Num Frames: " + model.frames.Length.ToString () );
			TreeNode tagsNode = root.Nodes.Add ( string.Format ( "Tags ({0}): ", model.tags.Length ) );

			for ( int i = 0 ; i < model.tags.Length ; i++ ) {
				TreeNode tagNode = tagsNode.Nodes.Add ( model.tags [i].name );
				tagNode.Nodes.Add ( "Origin: " + model.tags [i].origin.ToString () );
				tagNode.Nodes.Add ( "Axis X: " + model.tags [i].axisX.ToString () );
				tagNode.Nodes.Add ( "Axis Y: " + model.tags [i].axisY.ToString () );
				tagNode.Nodes.Add ( "Axis Z: " + model.tags [i].axisZ.ToString () );
			}

			TreeNode meshesNode = root.Nodes.Add ( string.Format ( "Meshes ({0}): ", model.dxMeshes.Length ) );

			for ( int i = 0 ; i < model.meshes.Length ; i++ ) {
				Md3Mesh mesh = model.meshes [i];
				TreeNode meshNode = meshesNode.Nodes.Add ( mesh.name );
				meshNode.Nodes.Add ( "Texture: " + mesh.textures [0].name );
				meshNode.Nodes.Add ( "Num Vertices: " + mesh.numVertices.ToString () );
				meshNode.Nodes.Add ( "Num Faces: " + mesh.numFaces.ToString () );
				meshNode.Nodes.Add ( "Num Textures: " + mesh.numTextures.ToString () );

				TreeNode framesNode = meshNode.Nodes.Add ( string.Format (  "Frames ({0}): ", mesh.numFrames ) );

				for ( int j = 0 ; j < model.frames.Length ; j++ ) {
					Md3Frame frame = model.frames [j];
					TreeNode frameNode = framesNode.Nodes.Add ( string.Format ( "Frame [{0}] {1}:", j, frame.name ) );
					frameNode.Nodes.Add ( "Local Origin: " + frame.localOrigin.ToString () );
					frameNode.Nodes.Add ( "Raduis: " + frame.radius.ToString () );
					frameNode.Nodes.Add ( "Mins: " + frame.mins.ToString () );
					frameNode.Nodes.Add ( "Maxs: " + frame.maxs.ToString () );
				}
			}

			trvModelProperties.ExpandAll ();
		}
	}
}
