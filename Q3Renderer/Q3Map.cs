using System;
using System.IO;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.DirectX;

namespace Q3Renderer
{
	public class Q3Map
	{
		#region Properties
		private string mapName;
		private Q3BspHeader bspHeader;
		private string entitiesStr;
		private Dictionary <string, object> [] entities;
		private Dictionary <string, List <Dictionary <string, object>>> groupedEntities = new Dictionary <string, List <Dictionary <string, object>>> ();
		private Q3BspTexture [] textures;
		private Q3BspPlane [] planes;
		private Q3BspNode [] nodes;
		private Q3BspLeaf [] leafs;
		private int [] leafFaces;
		private int [] leafBrushes;
		private Q3BspBrush [] brushes;
		private Q3BspBrushSide [] brushSides;
		private Q3BspVertex [] vertices;
		private int [] meshVertices;
		private Q3BspFace [] faces;
		private Q3BspLightMap [] lightmaps;
		private Q3BspVisData visData;
		private byte [] vecs;
		private int [] visibleFaces;

		public string MapName { get { return	mapName; } }
		public Dictionary <string, object> [] Entities { get { return	entities; } }
		public Dictionary <string, List <Dictionary <string, object>>> GroupedEntities {
			get { return	groupedEntities; }
		}

		public ReadOnlyCollection <Q3BspTexture> Textures {
			get { return	Array.AsReadOnly <Q3BspTexture> ( textures ); }
		}

		public ReadOnlyCollection <Q3BspVertex> Vertices {
			get { return	Array.AsReadOnly <Q3BspVertex> ( vertices ); }
		}

		public ReadOnlyCollection <int> MeshVertices {
			get { return	Array.AsReadOnly <int> ( meshVertices ); }
		}

		public ReadOnlyCollection <Q3BspLeaf> Leafs {
			get { return	Array.AsReadOnly <Q3BspLeaf> ( leafs ); }
		}

		public ReadOnlyCollection <Q3BspPlane> Planes {
			get { return	Array.AsReadOnly <Q3BspPlane> ( planes ); }
		}

		public ReadOnlyCollection <Q3BspNode> Nodes {
			get { return	Array.AsReadOnly <Q3BspNode> ( nodes ); }
		}

		public ReadOnlyCollection <int> LeafBrushes {
			get { return	Array.AsReadOnly <int> ( leafBrushes ); }
		}

		public ReadOnlyCollection <Q3BspBrush> Brushes {
			get { return	Array.AsReadOnly <Q3BspBrush> ( brushes ); }
		}

		public ReadOnlyCollection <Q3BspBrushSide> BrushSides {
			get { return	Array.AsReadOnly <Q3BspBrushSide> ( brushSides ); }
		}

		public ReadOnlyCollection <Q3BspFace> Faces {
			get { return	Array.AsReadOnly <Q3BspFace> ( faces ); }
		}

		public ReadOnlyCollection <Q3BspLightMap> Lightmaps {
			get { return	Array.AsReadOnly <Q3BspLightMap> ( lightmaps ); }
		}
		#endregion Properties

		#region Constructors
		public Q3Map ( string filename ) {
			Q3BspTextureUnsafe [] textures_u;
			Q3BspPlaneUnsafe [] planes_u;
			Q3BspNodeUnsafe [] nodes_u;
			Q3BspLeafUnsafe [] leafs_u;
			Q3BspVertexUnsafe [] vertices_u;
			Q3BspFaceUnsafe [] faces_u;
			Q3BspLightMapUnsafe [] lightMaps_u;

			MemoryStream ms = new MemoryStream ();

			Q3FileSystem.WriteResourceToStream ( filename, ms );
			bspHeader = ( Q3BspHeader ) ReadStruct ( ms, typeof ( Q3BspHeader ) );
			string m = bspHeader.Magic;

			if ( bspHeader.Magic == "IBSP" ) {
			} else if ( bspHeader.Magic == "FAKK" ) {
			}
			
			ReadEntities ( ms );
			textures_u   = ( Q3BspTextureUnsafe  [] ) ReadLump ( ms, typeof ( Q3BspTextureUnsafe   ), LumpType.Textures     );
			planes_u     = ( Q3BspPlaneUnsafe    [] ) ReadLump ( ms, typeof ( Q3BspPlaneUnsafe     ), LumpType.Planes       );
			nodes_u      = ( Q3BspNodeUnsafe     [] ) ReadLump ( ms, typeof ( Q3BspNodeUnsafe      ), LumpType.Nodes        );
			leafs_u      = ( Q3BspLeafUnsafe     [] ) ReadLump ( ms, typeof ( Q3BspLeafUnsafe      ), LumpType.Leafs        );
			leafFaces    = ( int                 [] ) ReadLump ( ms, typeof ( int                  ), LumpType.LeafFaces    );
			leafBrushes  = ( int                 [] ) ReadLump ( ms, typeof ( int                  ), LumpType.LeafBrushes  );
			brushes      = ( Q3BspBrush          [] ) ReadLump ( ms, typeof ( Q3BspBrush           ), LumpType.Brushes      );
			brushSides   = ( Q3BspBrushSide      [] ) ReadLump ( ms, typeof ( Q3BspBrushSide       ), LumpType.BrushSides   );
			vertices_u   = ( Q3BspVertexUnsafe   [] ) ReadLump ( ms, typeof ( Q3BspVertexUnsafe    ), LumpType.Vertices     );
			meshVertices = ( int                 [] ) ReadLump ( ms, typeof ( int                  ), LumpType.MeshVertices );
			faces_u      = ( Q3BspFaceUnsafe     [] ) ReadLump ( ms, typeof ( Q3BspFaceUnsafe      ), LumpType.Faces        );
			lightMaps_u  = ( Q3BspLightMapUnsafe [] ) ReadLump ( ms, typeof ( Q3BspLightMapUnsafe  ), LumpType.LightMaps    );
			ReadVisData ( ms );

			textures  = new Q3BspTexture  [textures_u.Length];
			planes    = new Q3BspPlane    [planes_u.Length];
			nodes     = new Q3BspNode     [nodes_u.Length];
			leafs     = new Q3BspLeaf     [leafs_u.Length];
			vertices  = new Q3BspVertex   [vertices_u.Length];
			faces     = new Q3BspFace     [faces_u.Length];
			lightmaps = new Q3BspLightMap [lightMaps_u.Length];

			int i = 0;

			for ( i = 0 ; i < textures.Length  ; i++ ) textures  [i].FromUnsafe ( textures_u  [i] );
			for ( i = 0 ; i < planes.Length    ; i++ ) planes    [i].FromUnsafe ( planes_u    [i] );
			for ( i = 0 ; i < nodes.Length     ; i++ ) nodes     [i].FromUnsafe ( nodes_u     [i] );
			for ( i = 0 ; i < leafs.Length     ; i++ ) leafs     [i].FromUnsafe ( leafs_u     [i] );
			for ( i = 0 ; i < vertices.Length  ; i++ ) vertices  [i].FromUnsafe ( vertices_u  [i] );
			for ( i = 0 ; i < faces.Length     ; i++ ) faces     [i].FromUnsafe ( faces_u     [i], vertices );
			for ( i = 0 ; i < lightmaps.Length ; i++ ) lightmaps [i].FromUnsafe ( lightMaps_u [i] );

			visibleFaces = new int [faces.Length];

			int lastSlash = filename.LastIndexOf ( '/' ) + 1;
			mapName = filename.Substring ( lastSlash, filename.Length - lastSlash - 4 );
		}
		#endregion Constructors

		#region Methods
		public int FindVisibleFaces ( FpsCamera camera, int [] facesToRender ) {
			int leaf;
			int visCluster;

			for ( int i = 0 ; i < visibleFaces.Length ; i++ )
				visibleFaces [i] = 0;

			leaf = FindLeaf ( camera.Position );

			visCluster = leafs [leaf].cluster;

			int faceindex;
			int renderindex = 0;

			for ( int i = 0 ; i < leafs.Length ; i++ ) {
				if ( isClusterVisible ( visCluster, leafs [i].cluster ) ) {
					bool vis = camera.ViewFrustum.isBoxInside ( leafs [i].mins, leafs [i].maxs );

					if ( vis ) {
						for ( int k = 0 ; k < leafs [i].n_leaffaces; k++ )
						{					
							faceindex =	leafFaces [leafs [i].leafface + k];

							if ( visibleFaces [faceindex] == 0 ) {
								visibleFaces [faceindex] = 1;
								facesToRender [renderindex++] = faceindex;
							}
						}
					}
				}
			}

			facesToRender [renderindex] = -1;

			return	renderindex;
		}

		private int FindLeaf ( Vector3 camPos ) {
			int index = 0;

			while ( index >= 0 ) {
				Q3BspNode  node  = nodes  [index];
				Q3BspPlane plane = planes [node.plane];

				float distance = Vector3.Dot ( plane.normal, camPos ) - plane.dist;

				if ( distance >= 0 )
					index = node.children [0];
				else
					index = node.children [1];
			}

			return -index - 1;
		}

		private bool isClusterVisible ( int visCluster, int testCluster ) {
			if ( vecs == null || vecs.Length == 0 || visCluster < 0 || testCluster < 0 )
				return	true;    

			int i = visCluster * visData.sz_vecs + ( testCluster >> 3 );
			int visSet = vecs[i];

			return	0 != ( visSet & ( 1 << ( testCluster & 7 ) ) );
		}

		private void ReadEntities ( MemoryStream ms ) {
			byte [] buf = new byte [bspHeader.entities.iLength];
			ms.Position = bspHeader.entities.iOffset;
			ms.Read ( buf, 0, buf.Length );
			entitiesStr = Encoding.ASCII.GetString ( buf );

			ParseEntities ();
		}

		private void ReadVisData ( MemoryStream ms ) {
			ms.Position = bspHeader.visData.iOffset;
			visData = ( Q3BspVisData ) ReadStruct ( ms, typeof ( Q3BspVisData ) );
			int vecSize = visData.n_vecs * visData.sz_vecs;
			vecs = new byte [vecSize];
			ms.Read ( vecs, 0, vecSize );
		}

		private Array ReadLump ( MemoryStream ms, Type elementType, LumpType lumpType ) {
			DirEntry lump = bspHeader.GetLump ( lumpType );
			int count = lump.iLength / Marshal.SizeOf ( elementType );
			Array arr = Array.CreateInstance ( elementType, count );
			ms.Position = lump.iOffset;
			
			for ( int i = 0 ; i < count ; i++ )
				arr.SetValue ( ReadStruct ( ms, elementType ), i );

			Type arrType = elementType.MakeArrayType ();

			return	arr;
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

		private void ParseEntities () {
			entitiesStr = entitiesStr.Substring ( 2, entitiesStr.Length - 5 );
			string [] scopes = entitiesStr.Split ( new string [] { "\n}\n{\n" }, StringSplitOptions.RemoveEmptyEntries );

			entities = new Dictionary <string, object> [scopes.Length];

			for ( int i = 0 ; i < scopes.Length ; i++ ) {
				string [] props = scopes [i].Split ( new char [] { '\n' }, StringSplitOptions.RemoveEmptyEntries );
				Dictionary <string, object> dictProps = new Dictionary <string, object> ( props.Length );

				string entityClassname = null;

				for ( int j = 0 ; j < props.Length ; j++ ) {
					string [] keyVal = props [j].Substring ( 1, props [j].Length - 2 ).
												 Split ( new string [] { "\" \"" }, StringSplitOptions.RemoveEmptyEntries );

					if ( keyVal.Length == 2 ) {
						string key = keyVal [0];
						string val = keyVal [1];
						object valObj = val;

						if ( key == "origin" ) {
							valObj = ParseVector3 ( val );
						} else if ( key == "angle" ) {
							valObj = ParseFloat ( val );
						} else if ( key == "classname" )
							entityClassname = val;

						dictProps [keyVal [0]] = valObj;
					}
				}

				if ( entityClassname != null ) {
					List <Dictionary <string, object>> group;

					if ( !groupedEntities.TryGetValue ( entityClassname, out group ) ) {
						groupedEntities [entityClassname] = new List <Dictionary <string, object>> ();
						group = groupedEntities [entityClassname];
					}
					
					group.Add ( dictProps );
				}

				entities [i] = dictProps;
			}
		}

		private Vector3 ParseVector3 ( string val ) {
			string [] components = val.Split ( ' ' );
			float [] floatComponents = new float [3];

			for ( int k = 0 ; k < 3 ; k++ ) {
				float result;

				if ( float.TryParse ( components [k], out result ) )
					floatComponents [k] = result;
			}

			return	new Vector3 (  floatComponents [0],
								   floatComponents [2],
								   floatComponents [1] );
		}

		private float ParseFloat ( string val ) {
			float f;
										
			float.TryParse ( val, out f );

			return	f;
		}
		#endregion Methods
	}
}
