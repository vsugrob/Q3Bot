using System;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.DirectX;

namespace Q3Renderer
{
	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public unsafe struct Md3FrameUnsafe {
		public fixed float mins [3];
		public fixed float maxs [3];
		public fixed float localOrigin [3];
		public float radius;
		public fixed byte name [16];
	}

	public struct Md3Frame {
		public Vector3 mins;
		public Vector3 maxs;
		public Vector3 localOrigin;
		public float radius;
		public string name;

		public unsafe void FromUnsafe ( Md3FrameUnsafe s ) {
			mins = new Vector3 ( s.mins [0], s.mins [2], s.mins [1] );
			maxs = new Vector3 ( s.maxs [0], s.maxs [2], s.maxs [1] );
			localOrigin = new Vector3 ( s.localOrigin [0], s.localOrigin [2], s.localOrigin [1] );
			radius = s.radius;

			byte [] buf = new byte [16];
			Marshal.Copy ( new IntPtr ( s.name ), buf, 0, 16 );
			name = Encoding.ASCII.GetString ( buf );
			name = name.Substring ( 0, name.IndexOf ( '\0' ) );
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public unsafe struct Md3TagUnsafe {
		public fixed byte name [64];	// tag name
		public fixed float origin [3];
		public fixed float axisX [3];
		public fixed float axisY [3];
		public fixed float axisZ [3];
	}

	public struct Md3Tag {
		public string name;				// tag name
		public Vector3 origin;
		public Vector3 axisX;
		public Vector3 axisY;
		public Vector3 axisZ;

		public unsafe void FromUnsafe ( Md3TagUnsafe s ) {
			byte [] buf = new byte [64];
			Marshal.Copy ( new IntPtr ( s.name ), buf, 0, 64 );
			name = Encoding.ASCII.GetString ( buf );
			name = name.Substring ( 0, name.IndexOf ( '\0' ) );

			origin = new Vector3 ( s.origin [0], s.origin [2], s.origin [1] );
			axisX = new Vector3 ( s.axisX [0], s.axisX [2], s.axisX [1] );
			axisY = new Vector3 ( s.axisY [0], s.axisY [2], s.axisY [1] );
			axisZ = new Vector3 ( s.axisZ [0], s.axisZ [2], s.axisZ [1] );
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public unsafe struct Md3MeshUnsafe {
		public fixed byte id [4];		// should contain "IDP3"
		public fixed byte name [64];
		public int flags;
		public int numFrames;			// all meshes in a model should have the same
		public int numTextures;			// all meshes in a model should have the same
		public int numVertices;
		public int numFaces;
		public int facesStart;
		public int texturesStart;
		public int texCoordsStart;		// texture coords are common for all frames
		public int verticesStart;		// numVertices * numFrames
		public int size;				// next mesh follows
	}

	public struct Md3Mesh {
		public string id;				// should contain "IDP3"
		public string name;
		public int flags;
		public int numFrames;			// all meshes in a model should have the same
		public int numTextures;			// all meshes in a model should have the same
		public int numVertices;
		public int numFaces;
		public int facesStart;
		public int texturesStart;
		public int texCoordsStart;		// texture coords are common for all frames
		public int verticesStart;		// numVertices * numFrames
		public int size;				// next mesh follows
		public Md3Texture [] textures;
		public Md3Face [] faces;
		public Md3TexCoord [] texCoords;
		public Md3PositionNormal [] vertices;

		public unsafe void FromUnsafe ( Md3MeshUnsafe s ) {
			byte [] buf = new byte [4];
			Marshal.Copy ( new IntPtr ( s.id ), buf, 0, 4 );
			id = Encoding.ASCII.GetString ( buf );

			buf = new byte [64];
			Marshal.Copy ( new IntPtr ( s.name ), buf, 0, 64 );
			name = Encoding.ASCII.GetString ( buf );
			name = name.Substring ( 0, name.IndexOf ( '\0' ) );

			flags = s.flags;
			numFrames = s.numFrames;
			numTextures = s.numTextures;
			numVertices = s.numVertices;
			numFaces = s.numFaces;
			facesStart = s.facesStart;
			texturesStart = s.texturesStart;
			texCoordsStart = s.texCoordsStart;
			verticesStart = s.verticesStart;
			size = s.size;
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public unsafe struct Md3TextureUnsafe {
		public fixed byte name [64];
		public int index;
	}

	public struct Md3Texture {
		public string name;
		public int index;

		public unsafe void FromUnsafe ( Md3TextureUnsafe s ) {
			byte [] buf = new byte [64];
			Marshal.Copy ( new IntPtr ( s.name ), buf, 0, 64 );
			// CHEAT
			buf [0] = ( byte ) 'm';
			name = Encoding.ASCII.GetString ( buf );
			name = name.Substring ( 0, name.IndexOf ( '\0' ) ).ToLower ();

			index = s.index;
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public unsafe struct Md3FaceUnsafe {
		public fixed int indices [3];
	}

	public struct Md3Face {
		public int [] indices;

		public unsafe void FromUnsafe ( Md3FaceUnsafe s ) {
			indices = new int [3];
			indices [0] = s.indices [0];
			indices [1] = s.indices [1];
			indices [2] = s.indices [2];
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public struct Md3TexCoord {
		public float u;
		public float v;
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public unsafe struct Md3PositionNormalUnsafe {
		public fixed short pos [3];
		public fixed byte  normal [2];	// Where is it used?
	}

	public struct Md3PositionNormal {
		private const float MD3_XYZ_SCALE = 1.0f / 64.0f;
		public Vector3 pos;
		public Vector2 normal;

		public unsafe void FromUnsafe ( Md3PositionNormalUnsafe s ) {
			pos = new Vector3 ( s.pos [0] * MD3_XYZ_SCALE, s.pos [2] * MD3_XYZ_SCALE, s.pos [1] * MD3_XYZ_SCALE );
			normal = new Vector2 ( s.normal [0], s.normal [1] );
		}
	}

	[StructLayout ( LayoutKind.Sequential, Pack=1 )]
	public unsafe struct Md3HeaderUnsafe {
		public fixed byte id [4];
		public int version;
		public fixed byte name [64];	// model name
		public int flags;
		public int numFrames;
		public int numTags;	
		public int numMeshes;
		public int numSkins;
		public int framesStart;			// offset for first frame
		public int tagsStart;			// numFrames * numTags
		public int meshesStart;			// first mesh, others follow
		public int size;				// end of file
	}

	public struct Md3Header {
		public string id;
		public int version;
		public string name;				// model name
		public int flags;
		public int numFrames;
		public int numTags;	
		public int numMeshes;
		public int numSkins;
		public int framesStart;			// offset for first frame
		public int tagsStart;			// numFrames * numTags
		public int meshesStart;			// first mesh, others follow
		public int size;				// end of file

		public unsafe void FromUnsafe ( Md3HeaderUnsafe s ) {
			byte [] buf = new byte [4];
			Marshal.Copy ( new IntPtr ( s.id ), buf, 0, 4 );
			id = Encoding.ASCII.GetString ( buf );
			version = s.version;

			buf = new byte [64];
			Marshal.Copy ( new IntPtr ( s.name ), buf, 0, 64 );
			name = Encoding.ASCII.GetString ( buf );
			name = name.Substring ( 0, name.IndexOf ( '\0' ) );

			flags = s.flags;
			numFrames = s.numFrames;
			numTags = s.numTags;
			numMeshes = s.numMeshes;
			numSkins = s.numSkins;
			framesStart = s.framesStart;
			tagsStart = s.tagsStart;
			meshesStart = s.meshesStart;
			size = s.size;
		}
	}
}