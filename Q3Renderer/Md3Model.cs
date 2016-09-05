using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Q3Renderer
{
	public class Md3Model
	{
		#region Properties
		private const int MD3_MAX_LODS = 3;

		private Device d3dDevice;
		private Q3ModelViewerForm parent;
		private string name;
		public Md3Header header;
		public Md3Frame [] frames;
		public Md3Tag [] tags;
		public Md3Mesh [] meshes;
		public Mesh [] dxMeshes;
		public float [] radiuses;
		public float totalRadius;
		public Vector3 [] centers;
		public Vector3 totalCenter;
		public List <Texture> textures = new List <Texture> ();
		public List <string> realnames = new List <string> ();
		private MemoryStream msTexture = new MemoryStream ();

		private string [] possibleExtensions = new string [] { "", ".jpg", ".jpeg", ".tga", ".bmp", ".png", ".gif", ".tiff" };
		public string Name {
			get { return	this.name; }
		}

		public int TotalVertices {
			get {
				int num = 0;

				foreach ( Md3Mesh mesh in meshes )
					num += mesh.numVertices;

				return	num;
			}
		}

		public int TotalFaces {
			get {
				int num = 0;

				foreach ( Md3Mesh mesh in meshes )
					num += mesh.numFaces;

				return	num;
			}
		}
		#endregion Properties

		#region Constructors
		public Md3Model ( string filename, Device d3dDevice, Q3ModelViewerForm parent ) {
			this.d3dDevice = d3dDevice;
			this.parent = parent;

			int lastSlash = filename.LastIndexOf ( '/' );
			name = filename.Substring ( lastSlash + 1 );
			name = name.Substring ( 0, name.Length - 4 );

			MemoryStream ms = new MemoryStream ();
			Q3FileSystem.WriteResourceToStream ( filename, ms );

			header.FromUnsafe ( ( Md3HeaderUnsafe ) ReadStruct ( ms, typeof ( Md3HeaderUnsafe ) ) );

			// Frames
			ms.Position = header.framesStart;
			frames = new Md3Frame [header.numFrames];

			for ( int i = 0 ; i < header.numFrames ; i++ )
				frames [i].FromUnsafe ( ( Md3FrameUnsafe ) ReadStruct ( ms, typeof ( Md3FrameUnsafe ) ) );

			// Tags
			ms.Position = header.tagsStart;
			tags = new Md3Tag [header.numTags];

			for ( int i = 0 ; i < header.numTags ; i++ )
				tags [i].FromUnsafe ( ( Md3TagUnsafe ) ReadStruct ( ms, typeof ( Md3TagUnsafe ) ) );

			// Meshes
			int meshStart = header.meshesStart;
			ms.Position = meshStart;
			meshes = new Md3Mesh [header.numMeshes];

			for ( int i = 0 ; i < header.numMeshes ; i++ ) {
				Md3Mesh md3Mesh = new Md3Mesh ();
				md3Mesh.FromUnsafe ( ( Md3MeshUnsafe ) ReadStruct ( ms, typeof ( Md3MeshUnsafe ) ) );

				// Mesh Textures
				ms.Position = meshStart + md3Mesh.texturesStart;
				md3Mesh.textures = new Md3Texture [md3Mesh.numTextures];

				for ( int j = 0 ; j < md3Mesh.numTextures ; j++ )
					md3Mesh.textures [j].FromUnsafe ( ( Md3TextureUnsafe ) ReadStruct ( ms, typeof ( Md3TextureUnsafe ) ) );

				// Mesh Faces
				ms.Position = meshStart + md3Mesh.facesStart;
				md3Mesh.faces = new Md3Face [md3Mesh.numFaces];

				for ( int j = 0 ; j < md3Mesh.numFaces ; j++ )
					md3Mesh.faces [j].FromUnsafe ( ( Md3FaceUnsafe ) ReadStruct ( ms, typeof ( Md3FaceUnsafe ) ) );

				// Mesh TexCoords
				ms.Position = meshStart + md3Mesh.texCoordsStart;
				md3Mesh.texCoords = new Md3TexCoord [md3Mesh.numVertices];

				for ( int j = 0 ; j < md3Mesh.numVertices ; j++ )
					md3Mesh.texCoords [j] = ( Md3TexCoord ) ReadStruct ( ms, typeof ( Md3TexCoord ) );

				// Vertices
				ms.Position = meshStart + md3Mesh.verticesStart;
				md3Mesh.vertices = new Md3PositionNormal [md3Mesh.numVertices];

				for ( int j = 0 ; j < md3Mesh.numVertices ; j++ )
					md3Mesh.vertices [j].FromUnsafe ( ( Md3PositionNormalUnsafe ) ReadStruct ( ms, typeof ( Md3PositionNormalUnsafe ) ) );

				meshes [i] = md3Mesh;
				ms.Position = meshStart + md3Mesh.size;
				meshStart = ( int ) ms.Position;
			}

			// Load to dx mesh objects
			dxMeshes = new Mesh [meshes.Length];
			radiuses = new float [meshes.Length];
			centers = new Vector3 [meshes.Length];

			for ( int i = 0 ; i < meshes.Length ; i++ ) {
				Md3Mesh md3Mesh = meshes [i];
				Mesh mesh = new Mesh ( md3Mesh.numFaces, md3Mesh.numVertices,
					MeshFlags.WriteOnly,
					CustomVertex.PositionNormalTextured.Format, d3dDevice );

				GraphicsStream vbStream = mesh.VertexBuffer.Lock ( 0, 0, LockFlags.Discard );

				for ( int j = 0 ; j < md3Mesh.numVertices ; j++ ) {
					Md3TexCoord texCoord = md3Mesh.texCoords [j];
					vbStream.Write ( new CustomVertex.PositionNormalTextured ( md3Mesh.vertices [j].pos, Vector3.Empty, texCoord.u, texCoord.v ) );
				}

				vbStream.Position = 0;
				radiuses [i] = Geometry.ComputeBoundingSphere ( vbStream,
					md3Mesh.numVertices,
					CustomVertex.PositionNormalTextured.Format,
					out centers [i] );

				mesh.VertexBuffer.Unlock ();

				GraphicsStream ibStream = mesh.IndexBuffer.Lock ( 0, 0, LockFlags.Discard );

				for ( int j = 0 ; j < md3Mesh.numFaces ; j++ ) {
					Md3Face face = md3Mesh.faces [j];
					ibStream.Write ( ( short ) face.indices [0] );
					ibStream.Write ( ( short ) face.indices [1] );
					ibStream.Write ( ( short ) face.indices [2] );
				}

				mesh.IndexBuffer.Unlock ();

				dxMeshes [i] = mesh;

				// Load textures
				for ( int j = 0 ; j < md3Mesh.numTextures ; j++ ) {
					string realname = "";
					Texture t = null;

					try { t = LoadTexture ( md3Mesh.textures [j].name, out realname ); }
					catch {}

					textures.Add ( t != null ? t : parent.textureNotFound );
					realnames.Add ( realname );
				}
			}

			if ( header.numMeshes > 0 ) {
				totalCenter = centers [0];
				totalRadius = radiuses [0];

				for ( int i = 1 ; i < centers.Length ; i++ ) {
					totalCenter = ( totalCenter + centers [i] ) * 0.5f;
					totalRadius = ( ( totalCenter - centers [i] ).Length () + totalRadius + radiuses [i] ) / 2;
				}
			}
		}
		#endregion Constructors

		#region Methods
		public void DrawSubmesh ( int id ) {
			d3dDevice.SetTexture ( 0, textures [id] );
			dxMeshes [id].DrawSubset ( 0 );
		}

		public void Draw () {
			for ( int i = 0 ; i < dxMeshes.Length ; i++ )
				DrawSubmesh ( i );
		}

		private object ReadStruct ( Stream stream, Type type ) {
			int size = Marshal.SizeOf ( type );
			byte [] buffer = new byte [size];

			stream.Read ( buffer, 0, size );

			GCHandle handle = GCHandle.Alloc ( buffer, GCHandleType.Pinned );
			object obj = Marshal.PtrToStructure ( handle.AddrOfPinnedObject (), type );
			handle.Free ();

			return	obj;
		}

		private Texture LoadTexture ( string filename, out string realname ) {
			filename = filename.Substring ( 0, filename.Length - 4 );
			Console.Write ( "Loading texture {0}", filename );
			realname = "";

			foreach ( string ext in possibleExtensions ) {
				if ( Q3FileSystem.WriteResourceToStream ( filename + ext, msTexture ) ) {
					Texture texture;

					try { texture = TextureLoader.FromStream ( d3dDevice, msTexture ); }
					catch { break; }
					
					Console.WriteLine ( ext + "\tsuccess" );

					realname = filename + ext;

					return	texture;
				}
			}

			Console.WriteLine ( "...\tfailed" );

			return	null;
		}
		#endregion Methods
	}
}
