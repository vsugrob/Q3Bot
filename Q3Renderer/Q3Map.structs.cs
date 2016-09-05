using System;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.DirectX;

namespace Q3Renderer
{
	internal enum LumpType {
		Entities = 0,
		Textures = 1,
		Planes = 2,
		Nodes = 3,
		Leafs = 4,
		LeafFaces = 5,
		LeafBrushes = 6,
		Brushes = 8,
		BrushSides = 9,
		Vertices = 10,
		MeshVertices = 11,
		Faces = 13,
		LightMaps = 14,
		VisData = 16
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	internal struct DirEntry {
		public int iOffset;
		public int iLength;
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public unsafe struct Q3BspTextureUnsafe {
		public fixed byte name [64];
		public int flags;
		public int contents;
	}

	public struct Q3BspTexture {
		public string name;
		public int flags;
		public int contents;

		public void FromUnsafe ( Q3BspTextureUnsafe s ) {
			byte [] buf = new byte [64];
			unsafe { Marshal.Copy ( new IntPtr ( s.name ), buf, 0, 64 ); }
			name = Encoding.ASCII.GetString ( buf ).TrimEnd ( '\0' );
			flags = s.flags;
			contents = s.contents;
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	internal struct AliceBspHeader {
		public char magic1;
		public char magic2;
		public char magic3;
		public char magic4;
		public int version;
		public DirEntry unknown0;
		public DirEntry textures;
		public DirEntry entities;
		public DirEntry planes;
		public DirEntry nodes;
		public DirEntry leafs;
		public DirEntry leafFaces;
		public DirEntry leafBrushes;
		public DirEntry unknown1;
		public DirEntry brushes;
		public DirEntry brushSides;
		public DirEntry vertices;
		public DirEntry meshVertices;
		public DirEntry unknown2;
		public DirEntry faces;
		public DirEntry lightmaps;
		public DirEntry unknown3;
		public DirEntry visData;

		public string Magic {
			get { return	string.Format ( "{0}{1}{2}{3}", magic1, magic2, magic3, magic4 ); }
		}

		public DirEntry GetLump ( LumpType index ) {
			if      ( LumpType.Entities    == index )	return	entities;
			else if ( LumpType.Textures    == index )	return	textures;
			else if ( LumpType.Planes      == index )	return	planes;
			else if ( LumpType.Nodes       == index )	return	nodes;
			else if ( LumpType.Leafs       == index )	return	leafs;
			else if ( LumpType.LeafFaces   == index )	return	leafFaces;
			else if ( LumpType.LeafBrushes == index )	return	leafBrushes;
			else if ( LumpType.Brushes     == index )	return	brushes;
			else if ( LumpType.BrushSides  == index )	return	brushSides;
			else if ( LumpType.Vertices    == index )	return	vertices;
			else if ( LumpType.MeshVertices   == index )	return	meshVertices;
			else if ( LumpType.Faces       == index )	return	faces;
			else if ( LumpType.LightMaps   == index )	return	lightmaps;
			else if ( LumpType.VisData     == index )	return	visData;
			else throw new ArgumentException ( "Wrong lump index specified", "index" );
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	internal struct Q3BspHeader {
		public char magic1;
		public char magic2;
		public char magic3;
		public char magic4;
		public int version;
		public DirEntry entities;
		public DirEntry textures;
		public DirEntry planes;
		public DirEntry nodes;
		public DirEntry leafs;
		public DirEntry leafFaces;
		public DirEntry leafBrushes;
		public DirEntry unknown1;
		public DirEntry brushes;
		public DirEntry brushSides;
		public DirEntry vertices;
		public DirEntry meshVertices;
		public DirEntry unknown2;
		public DirEntry faces;
		public DirEntry lightmaps;
		public DirEntry unknown3;
		public DirEntry visData;

		public string Magic {
			get { return	string.Format ( "{0}{1}{2}{3}", magic1, magic2, magic3, magic4 ); }
		}

		public DirEntry GetLump ( LumpType index ) {
			if      ( LumpType.Entities    == index )	return	entities;
			else if ( LumpType.Textures    == index )	return	textures;
			else if ( LumpType.Planes      == index )	return	planes;
			else if ( LumpType.Nodes       == index )	return	nodes;
			else if ( LumpType.Leafs       == index )	return	leafs;
			else if ( LumpType.LeafFaces   == index )	return	leafFaces;
			else if ( LumpType.LeafBrushes == index )	return	leafBrushes;
			else if ( LumpType.Brushes     == index )	return	brushes;
			else if ( LumpType.BrushSides  == index )	return	brushSides;
			else if ( LumpType.Vertices    == index )	return	vertices;
			else if ( LumpType.MeshVertices   == index )	return	meshVertices;
			else if ( LumpType.Faces       == index )	return	faces;
			else if ( LumpType.LightMaps   == index )	return	lightmaps;
			else if ( LumpType.VisData     == index )	return	visData;
			else throw new ArgumentException ( "Wrong lump index specified", "index" );
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public unsafe struct Q3BspFaceUnsafe {
		public int texture;
		public int effect;
		public int type;
		public int vertex;
		public int n_vertexes;
		public int meshvert;
		public int n_meshverts;
		public int lm_index;
		public fixed int lm_start [2];
		public fixed int lm_size [2];
		public fixed float lm_origin [3];
		public fixed float lm_vecs [6];	//[2][3];
		public fixed float normal [3];
		public fixed int size [2];
	}

	public enum Q3BspFaceType {
		Polygon = 1,
		Patch,
		Mesh,
		Billboard
	}

	public struct Q3BspFace {
		public int texture;
		public int effect;
		public Q3BspFaceType type;
		public int vertex;
		public int n_vertexes;
		public int meshvert;
		public int n_meshverts;
		public int n_triangles;
		public int lm_index;
		public int [] lm_start;
		public int [] lm_size;
		public Vector3 lm_origin;
		public Vector3 [] lm_vecs;
		public Vector3 normal;
		public int width;
		public int height;
		public Q3BspPatch patch;

		public void FromUnsafe ( Q3BspFaceUnsafe s, Q3BspVertex [] vertices ) {
			texture = s.texture;
			effect = s.effect;
			type = ( Q3BspFaceType ) s.type;
			vertex = s.vertex;
			n_vertexes = s.n_vertexes;
			meshvert = s.meshvert;
			n_meshverts = s.n_meshverts;
			n_triangles = s.n_meshverts / 3;
			lm_index = s.lm_index;

			lm_start = new int [2];
			lm_size = new int [2];
			lm_vecs = new Vector3 [2];

			unsafe {
				lm_start [0] = s.lm_start [0];
				lm_start [1] = s.lm_start [1];
				lm_size [0] = s.lm_size [0];
				lm_size [1] = s.lm_size [1];
				lm_origin = new Vector3 ( s.lm_origin [0], s.lm_origin [2], s.lm_origin [1] );
				lm_vecs [0] = new Vector3 ( s.lm_vecs [0], s.lm_vecs [1], s.lm_vecs [2] );
				lm_vecs [1] = new Vector3 ( s.lm_vecs [3], s.lm_vecs [4], s.lm_vecs [5] );
				// swapping axes
				normal = new Vector3 ( s.normal [0], s.normal [2], s.normal [1] );
				width  = s.size [0];
				height = s.size [1];
			}

			if ( type == Q3BspFaceType.Patch ) {
				Q3BspPatch q3patch = new Q3BspPatch ();

				int patch_size_x = ( width  - 1 ) / 2;
				int patch_size_y = ( height - 1 ) / 2;
				int num_bezier_patches = patch_size_y * patch_size_x;

				q3patch.size = num_bezier_patches;
				q3patch.bezier = new Q3BezierPatch [q3patch.size];

				int patchIndex =  0;
				int ii, n, j, nn;

				for ( ii = 0, n = 0 ; n < patch_size_x ; n++, ii = 2 * n ) {
					for ( j = 0, nn = 0 ; nn < patch_size_y ; nn++, j = 2 * nn ) {
						Q3BezierPatch bezier = new Q3BezierPatch ();

						for ( int ctr = 0, index = 0 ; ctr < 3 ; ctr++ ) {
							int pos = ctr * width;
							int vIdx = vertex + ii + width * j + pos;

							bezier.controls [index++] = vertices [vIdx].Copy ();
							bezier.controls [index++] = vertices [vIdx + 1].Copy ();
							bezier.controls [index++] = vertices [vIdx + 2].Copy ();
						}

						bezier.Tessellate ( 5 );
						q3patch.bezier [patchIndex] = bezier;
						patchIndex++;
					}
				}

				patch = q3patch;
			}
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public unsafe struct Q3BspVertexUnsafe {
		public fixed float position [3];
		public fixed float texcoord [4]; //[2][2];
		public fixed float normal [3];
		public fixed byte color [4];
	}

	public struct Q3BspVertex {
		public Vector3 position;
		public Vector2 [] texcoord;
		public Vector3 normal;
		public Color   color;

		public Q3BspVertex ( Vector3 position, Vector3 normal, Vector2 [] texcoord, Color color ) {
			this.position = position;
			this.texcoord = new Vector2 [2];
			this.texcoord [0] = texcoord [0];
			this.texcoord [1] = texcoord [1];
			this.normal = normal;
			this.color  = color;
		}

		public void FromUnsafe ( Q3BspVertexUnsafe s ) {
			texcoord = new Vector2 [2];

			unsafe {
				// swapping axes
				position = new Vector3 ( s.position [0], s.position [2], s.position [1] );
				normal = new Vector3 ( s.normal [0], s.normal [2], s.normal [1] );
				texcoord [0] = new Vector2 ( s.texcoord [0], s.texcoord [1] );
				texcoord [1] = new Vector2 ( s.texcoord [2], s.texcoord [3] );
				// CHEKIT:
				color = Color.FromArgb ( s.color [3], s.color [0], s.color [1], s.color [2] );
			}
		}

		public void Normalize () {
			this.normal.Normalize ();
		}

		public static Q3BspVertex operator + ( Q3BspVertex v1, Q3BspVertex v2 ) {
			Q3BspVertex r =
				new Q3BspVertex ( v1.position + v2.position,
								  v1.normal,
								  new Vector2 [2] { v1.texcoord [0] + v2.texcoord [0], v1.texcoord [1] + v2.texcoord [1] },
								  v1.color );

			return	r;
		}

		public static Q3BspVertex operator * ( Q3BspVertex v1, float a ) {
			Q3BspVertex r =
				new Q3BspVertex ( v1.position * a,
								  v1.normal * a,
								  new Vector2 [2] { v1.texcoord [0] * a, v1.texcoord [1] * a },
								  v1.color );

			return	r;
		}

		public Q3BspVertex Copy () {
			return	new Q3BspVertex ( this.position, this.normal, new Vector2 [2] { this.texcoord [0], this.texcoord [1] }, this.color );
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public unsafe struct Q3BspLeafUnsafe {
		public int cluster;
		public int area;
		public fixed int mins [3];
		public fixed int maxs [3];
		public int leafface;
		public int n_leaffaces;
		public int leafbrush;
		public int n_leafbrushes;
	}

	public struct Q3BspLeaf {
		public int cluster;
		public int area;
		public Vector3 mins;
		public Vector3 maxs;
		public int leafface;
		public int n_leaffaces;
		public int leafbrush;
		public int n_leafbrushes;

		public void FromUnsafe ( Q3BspLeafUnsafe s ) {
			cluster = s.cluster;
			area = s.area;
			leafface = s.leafface;
			n_leaffaces = s.n_leaffaces;
			leafbrush = s.leafbrush;
			n_leafbrushes = s.n_leafbrushes;

			unsafe {
				// swapping axes
				mins = new Vector3 ( s.mins [0], s.mins [2], s.mins [1] );
				maxs = new Vector3 ( s.maxs [0], s.maxs [2], s.maxs [1] );
			}
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public unsafe struct Q3BspPlaneUnsafe {
		public fixed float normal [3];
		public float dist;
	}

	public struct Q3BspPlane {
		public Vector3 normal;
		public float dist;

		public void FromUnsafe ( Q3BspPlaneUnsafe s ) {
			dist = s.dist;

			unsafe {
				// swapping axes
				normal = new Vector3 ( s.normal [0], s.normal [2], s.normal [1] );
			}
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public unsafe struct Q3BspNodeUnsafe {
		public int plane;
		public fixed int children [2];
		public fixed int mins [3];
		public fixed int maxs [3];
	}

	public struct Q3BspNode {
		public int plane;
		public int [] children;
		public Vector3 mins;
		public Vector3 maxs;

		public void FromUnsafe ( Q3BspNodeUnsafe s ) {
			plane = s.plane;

			children = new int [2];

			unsafe {
				children [0] = s.children [0];
				children [1] = s.children [1];
				// swapping axes
				mins = new Vector3 ( s.mins [0], s.mins [2], s.mins [1] );
				maxs = new Vector3 ( s.maxs [0], s.maxs [2], s.maxs [1] );
			}
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public struct Q3BspBrush {
		public int brushside;
		public int n_brushsides;
		public int texture;
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public struct Q3BspBrushSide {
		public int plane;
		public int texture;
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public unsafe struct Q3BspLightMapUnsafe {
		public fixed byte lightmap [49152];	// [128][128][3];
	}

	public struct Q3BspLightMap {
		public byte [,,] lightmap;

		public void FromUnsafe ( Q3BspLightMapUnsafe s ) {
			lightmap = new byte [128, 128, 3];
			int n = 0;

			unsafe {
				for ( int x = 0 ; x < 128 ; x++ )
					for ( int y = 0 ; y < 128 ; y++ )
						for ( int c = 0 ; c < 3 ; c++, n++ )
							lightmap [x, y, c] = s.lightmap [n];
			}
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	internal struct Q3BspVisData {
		public int n_vecs;
		public int sz_vecs;
	}

	public class Q3BspPatch {
		public int size;
		public Q3BezierPatch [] bezier;
	}
}
