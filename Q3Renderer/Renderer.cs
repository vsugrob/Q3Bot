using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SysDraw = System.Drawing;
using Q3Renderer.Properties;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using DI = Microsoft.DirectX.DirectInput;
using DR = System.Drawing;

namespace Q3Renderer
{
	public enum TexturingFlags {
		Nothing = 0,
		Textures = 1,
		Lightmaps = 2,
		TexturesAndLightmaps = 3
	}

	public class Renderer
	{
		#region Properties
		private Q3Map map;
		private Device d3dDevice;
		private FpsCamera fpsCamera;
		private Input input;
		private float time = 0.0f;
		private TextureFilter magFilter = TextureFilter.Anisotropic;
		private TextureFilter minFilter = TextureFilter.Anisotropic;
		private TextureFilter mipFilter = TextureFilter.Anisotropic;
		SysDraw.Color bgColor = SysDraw.Color.Black;
		private bool fullscreen = false;

		private bool visibilityFixed = false;

		Texture textureNotFound;
		Texture [] textures;
		Texture [] lightmaps;
		MemoryStream msTexture = new MemoryStream ();
		private string [] possibleExtensions = new string [] { "", ".jpg", ".jpeg", ".tga", ".bmp", ".png", ".gif", ".tiff" };
		private TexturingFlags texturingFlags = TexturingFlags.TexturesAndLightmaps;
		private TextureOperation lightmapTexOp = TextureOperation.Modulate4X;
		private FillMode fillMode = FillMode.Solid;

		private int numVisibleFaces;
		private int [] visibleFaces;

		private VertexBuffer vb;
		private IndexBuffer  ib;
		private VertexBuffer bezierVb;
		private IndexBuffer  bezierIb;
		// FIXIT: locks must be already implemented by vb.Lock and ib.Lock!
		private AutoResetEvent vbLock = new AutoResetEvent ( true );
		private AutoResetEvent ibLock = new AutoResetEvent ( true );
		private AutoResetEvent bezierVbLock = new AutoResetEvent ( true );
		private AutoResetEvent bezierIbLock = new AutoResetEvent ( true );
		private WaitHandle [] bufferLocks = new WaitHandle [4];

		private VertexDeclaration vDecl;

		private int lastTextureId;
		private int lastLightmapId;
		private bool ibNeedSet;
		private bool bezierIbNeedSet;

		private Font font;
		private SysDraw.Color fontColor = SysDraw.Color.MediumAquamarine;
		private float fontSize = 12.0f;
		private SysDraw.FontStyle fontStyle = SysDraw.FontStyle.Bold;
		private string hintText = "Q3Renderer by evi";
		private float hintStartTime = 0.0f;
		private float hintDisplayTime;
		private float hintDisplayTimeDefault = 3.0f;

		// <RenderToSurface Test>
		public bool rtsEnabled = false;
		public bool RtsEnabled {
			get { return	rtsEnabled; }
			set {
				rtsEnabled = value;
				DisplayHint ( string.Format ( "RenderToSurface enabled: {0}", rtsEnabled ) );
			}
		}

		public bool rtsOn3DPoly = true;
		public bool RtsOn3DPoly {
			get { return	rtsOn3DPoly; }
			set {
				rtsOn3DPoly = value;
				DisplayHint ( string.Format ( "Draw RenderToSurface on 3d polygon: {0}", rtsOn3DPoly ) );
			}
		}

		public bool rtsOnSprite = false;
		public bool RtsOnSprite {
			get { return	rtsOnSprite; }
			set {
				rtsOnSprite = value;
				DisplayHint ( string.Format ( "Draw RenderToSurface on sprite: {0}", rtsOnSprite ) );
			}
		}

		private const int surfWidth  = 256;
		private const int surfHeight = 256;
		private RenderToSurface rts;
		private Surface surface;
		private Texture surfTexture;
		private IndexBuffer polyIb;
		private VertexBuffer polyVb;
		// </RenderToSurface Test>

		// <RenderToEnvironmentMap Test>
		private bool rteEnabled = false;

		public bool RteEnabled {
			get { return	rteEnabled; }
			set {
				if ( rteEnabled = value )
					teapotPos = fpsCamera.Position + fpsCamera.Look * 50;

				DisplayHint ( string.Format ( "RenderToEnvironment enabled: {0}", rteEnabled ) );
			}
		}

		private Vector3 teapotPos;
		private const int envSize = 256;
		private RenderToEnvironmentMap rte;
		private CubeTexture cubeTexture;
		private IndexBuffer  [] envPolysIndices  = new IndexBuffer  [6];
		private VertexBuffer [] envPolysVertices = new VertexBuffer [6];
		// </RenderToEnvironmentMap Test>

		// <Effect Test>
		private Effect effect;
		private string compilationErrors;
		private bool reloadShader = true;
		private bool shaderEnabled = true;
		public bool ShaderEnabled {
			get { return	shaderEnabled; }
			set {
				if ( shaderEnabled = value )
					reloadShader = true;
			}
		}

		private Vector4 lightPos;
		private bool lightEnabled = false;
		public bool LightEnabled {
			get { return	lightEnabled; }
			set {
				if ( lightEnabled = value ) {
					Vector3 lightPos3 = fpsCamera.Position + fpsCamera.Look * 100 + fpsCamera.Up * 2;
					lightPos = new Vector4 ( lightPos3.X, lightPos3.Y, lightPos3.Z, 1.0f );
				}

				DisplayHint ( string.Format ( "Light enabled: {0}", lightEnabled ) );
			}
		}
		// </Effect Test>

		public bool FullScreen {
			get { return	fullscreen; }
			set {
				fullscreen = value;
				DisplayHint ( string.Format ( "Fullscreen: {0}", fullscreen ) );
			}
		}

		public bool VisibilityFixed {
			get { return	visibilityFixed; }
			set {
				visibilityFixed = value;
				DisplayHint ( string.Format ( "Visibility Fixed: {0}", visibilityFixed ) );
			}
		}

		public TexturingFlags TexturingFlags {
			get { return	texturingFlags; }
			set {
				if ( value >= 0 && value <= TexturingFlags.TexturesAndLightmaps )
					texturingFlags = value;
				else
					texturingFlags = TexturingFlags.Nothing;

				DisplayHint ( string.Format ( "Texturing Flags: {0}", texturingFlags ) );
			}
		}

		public FillMode FillMode {
			get { return	fillMode; }
			set {
				fillMode = value;
				DisplayHint ( string.Format ( "Fill Mode: {0}", fillMode ) );
			}
		}

		public Q3Map Map {
			get { return	map; }
			set {
				map = value;
				InitRenderer ();
				DisplayHint ( string.Format ( "Map: {0}", map.MapName ) );
			}
		}
		public Device Device { get { return	d3dDevice; } }
		public Input Input { get { return	input; } }
		#endregion Properties

		#region Constructors
		public Renderer ( Q3Map map, Device d3dDevice ) {
			bufferLocks [0] = ibLock;
			bufferLocks [1] = vbLock;
			bufferLocks [2] = bezierIbLock;
			bufferLocks [3] = bezierVbLock;

			this.d3dDevice = d3dDevice;
			this.Map = map;

			input = new Input ( d3dDevice.PresentationParameters.DeviceWindow,
								DI.CooperativeLevelFlags.Background | DI.CooperativeLevelFlags.NonExclusive,
								DI.CooperativeLevelFlags.Background | DI.CooperativeLevelFlags.NonExclusive );
			int w = d3dDevice.PresentationParameters.BackBufferWidth;
			int h = d3dDevice.PresentationParameters.BackBufferHeight;

			fpsCamera = new FpsCamera ( input, ( float ) w / ( float ) h, 105.0f * ( float ) System.Math.PI / 360.0f, 1.0f, 10000.0f );

			List <Dictionary <string, object>> respawns;

			if ( map.GroupedEntities.TryGetValue ( "info_player_deathmatch", out respawns ) ) {
				Random rnd = new Random ();
				int respIdx = rnd.Next ( respawns.Count );
				Dictionary <string, object> props = respawns [respIdx];
				Vector3 respPos = ( Vector3 ) props ["origin"];
				//float angle = ( float ) props ["angle"];
				fpsCamera.Position = respPos;
			}

			// Create Vertex Buffer
			vb = new VertexBuffer ( typeof ( Q3VertexFormats.PositionNormalTexturedLightened ),
									map.Vertices.Count,
									d3dDevice,
									Usage.WriteOnly,
									Q3VertexFormats.PositionNormalTexturedLightened.Format,
									Pool.Default );
			vb.Created += new EventHandler ( vb_Created );
			vb_Created ( this, null );

			// Create Index Buffer
			ib = new IndexBuffer ( typeof ( int ),
									map.MeshVertices.Count,
									d3dDevice,
									Usage.WriteOnly,
									Pool.Default );
			ib.Created += new EventHandler ( ib_Created );
			ib_Created ( this, null );

			// Calculate Bezier Index Buffer size and ViertexBuffer Size
			int numIndex = 0;
			int numVertex = 0;

			ReadOnlyCollection <Q3BspFace> faces = map.Faces;
			Q3BspFace face;

			for ( int i = 0 ; i < faces.Count ; i++ ) {
				face = faces [i];

				if ( face.type == Q3BspFaceType.Patch ) {
					Q3BspPatch patch = face.patch;

					if ( patch != null ) {
						for ( int j = 0 ; j < patch.size ; j++ ) {
							numIndex  += patch.bezier [j].indices.Length;
							numVertex += patch.bezier [j].vertices.Length;
						}
					}
				}
			}

			if ( numIndex != 0 && numVertex != 0 ) {
				// Create bezier buffers
				bezierIb = new IndexBuffer ( typeof ( int ),
											 numIndex,
											 d3dDevice,
											 Usage.WriteOnly,
											 Pool.Default );
				bezierIb.Created += new EventHandler ( bezierIb_Created );
				bezierIb_Created ( this, null );

				bezierVb = new VertexBuffer ( typeof ( Q3VertexFormats.PositionNormalTexturedLightened ),
											  numVertex,
											  d3dDevice,
											  Usage.WriteOnly,
											  Q3VertexFormats.PositionNormalTexturedLightened.Format,
											  Pool.Default );
				bezierVb.Created += new EventHandler ( bezierVb_Created );
				bezierVb_Created ( this, null );
			}

			font = new Font ( d3dDevice, new SysDraw.Font ( "Courier New", fontSize, fontStyle ) );
		}
		#endregion Constructors

		#region Methods
		private void SetupDeviceState () {
			d3dDevice.SetRenderState ( RenderStates.Lighting, false );
			d3dDevice.SetRenderState ( RenderStates.ZEnable , true  );
			//d3dDevice.SetRenderState ( RenderStates.CullMode, ( int ) Cull.CounterClockwise );
			d3dDevice.SetRenderState ( RenderStates.CullMode, ( int ) Cull.None );	// FIXIT: is it right for quake?

			d3dDevice.VertexDeclaration = vDecl;

			d3dDevice.SetSamplerState ( 0, SamplerStageStates.MagFilter, ( int ) magFilter );
			d3dDevice.SetSamplerState ( 0, SamplerStageStates.MinFilter, ( int ) minFilter );
			d3dDevice.SetSamplerState ( 0, SamplerStageStates.MipFilter, ( int ) mipFilter );

			d3dDevice.SetSamplerState ( 1, SamplerStageStates.MagFilter, ( int ) magFilter );
			d3dDevice.SetSamplerState ( 1, SamplerStageStates.MinFilter, ( int ) minFilter );
			d3dDevice.SetSamplerState ( 1, SamplerStageStates.MipFilter, ( int ) mipFilter );

			d3dDevice.SetTextureStageState ( 0, TextureStageStates.TextureCoordinateIndex, 0 );
			d3dDevice.SetTextureStageState ( 0, TextureStageStates.ColorOperation, ( int ) TextureOperation.SelectArg1 );

			d3dDevice.SetTextureStageState ( 1, TextureStageStates.TextureCoordinateIndex, 1 );
			d3dDevice.SetTextureStageState ( 1, TextureStageStates.ColorOperation, ( int ) lightmapTexOp );
			d3dDevice.SetTextureStageState ( 1, TextureStageStates.AlphaOperation, ( int ) TextureOperation.SelectArg1 );
		}

		public void ReloadShader () {
			reloadShader = true;
		}

		public void SetShaderTexture ( int stage, BaseTexture texture ) {
			if ( shaderEnabled && effect != null ) {
				EffectHandle handle = stage == 0 ?
					EffectHandle.FromString ( "tex" ) :
					EffectHandle.FromString ( "lightmap" );

				effect.SetValue ( handle, texture );
			}
		}

		private void ResetDeviceState () {
			d3dDevice.RenderState.FillMode = fillMode;

			lastLightmapId = -1;
			lastTextureId  = -1;
			ibNeedSet       = true;
			bezierIbNeedSet = true;
		}

		public void OnResetDevice () {
			int w = d3dDevice.PresentationParameters.BackBufferWidth;
			int h = d3dDevice.PresentationParameters.BackBufferHeight;
			fpsCamera.Aspect = ( float ) w / ( float ) h;

			SetupDeviceState ();

			if ( rts == null /*&& RtsEnabled*/ ) {
				rts = new RenderToSurface ( d3dDevice, surfWidth, surfHeight, Format.X8R8G8B8, true, DepthFormat.D24X8 );
				rts.Reset += new EventHandler ( rts_Reset );
				rts_Reset ( this, null );
			}

			if ( rte == null /*&& RteEnabled*/ ) {
				rte = new RenderToEnvironmentMap ( d3dDevice, envSize, 1, Format.X8R8G8B8, true, DepthFormat.D24X8 );
				rte.Reset += new EventHandler ( rte_Reset );
				rte_Reset ( this, null );
			}
		}

		void rts_Reset ( object sender, EventArgs e ) {
			surfTexture = new Texture ( d3dDevice, surfWidth, surfHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default );
			surface = surfTexture.GetSurfaceLevel ( 0 );
		}

		void rte_Reset ( object sender, EventArgs e ) {
			cubeTexture = new CubeTexture ( d3dDevice, envSize, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default );
		}

		public void OnLostDevice () {
		}

		public void Update ( float delta ) {
			time += delta;

			input.Poll ();
			fpsCamera.Update ( delta );

			if ( !visibilityFixed ) {
				numVisibleFaces = map.FindVisibleFaces ( fpsCamera, visibleFaces );
				QSortFaces ( 0, numVisibleFaces - 1 );
			}

			if ( shaderEnabled && effect != null )
				effect.SetValue ( EffectHandle.FromString ( "Time" ), time );
		}

		public void DisplayHint ( string text, float displayTime ) {
			hintStartTime = time;
			hintDisplayTime = displayTime;
			hintText = text;
		}

		public void DisplayHint ( string text ) {
			DisplayHint ( text, hintDisplayTimeDefault );
		}

		public void Draw () {
			// FIXIT: try :'(
			try {
				if ( !fullscreen != d3dDevice.PresentationParameters.Windowed ) {
					PresentParameters pp = d3dDevice.PresentationParameters;
					pp.Windowed = !fullscreen;
					d3dDevice.Reset ( pp );
				}

				if ( shaderEnabled && reloadShader ) {
					effect = Effect.FromFile ( d3dDevice, @"..\..\Shader.fx", null, null, ShaderFlags.None, null, out compilationErrors );

					if ( compilationErrors != null && compilationErrors.Length > 0 )
						DisplayHint ( "Shader Load Fail: " + compilationErrors, 7.0f );
					else
						DisplayHint ( "Shader Load Success" );

					reloadShader = false;
				}

				if ( rtsEnabled )
					DrawToSurface ();

				if ( RteEnabled )
					DrawToEnvironment ();

				d3dDevice.BeginScene ();
				DrawQ3Map ();

				if ( rtsEnabled )
					DrawSurface ();

				if ( RteEnabled )
					DrawEnvironment ();

				if ( time - hintStartTime < hintDisplayTime )
					font.DrawText ( null, hintText,
						new SysDraw.Rectangle ( 0, 0,
							d3dDevice.PresentationParameters.BackBufferWidth,
							d3dDevice.PresentationParameters.BackBufferHeight ),
						DrawTextFormat.NoClip | DrawTextFormat.WordBreak, fontColor.ToArgb () );

				d3dDevice.EndScene ();
				d3dDevice.Present ();
			} catch {}
		}

		private void DrawToSurface () {
			rts.BeginScene ( surface );
			FillMode prevFillMode = d3dDevice.RenderState.FillMode;
			d3dDevice.RenderState.FillMode = FillMode.WireFrame;
			DrawQ3Map ();
			d3dDevice.RenderState.FillMode = prevFillMode;
			rts.EndScene ( Filter.None );
		}

		private void DrawSurface () {
			if ( rtsOn3DPoly ) {
				if ( polyIb == null )
					CreatePoly ( out polyIb, out polyVb );

				d3dDevice.SetTransform ( TransformType.World, Matrix.Translation ( fpsCamera.Position + fpsCamera.Look * 5 - fpsCamera.Right * 2 - fpsCamera.Up ) );
				d3dDevice.SetTransform ( TransformType.View , fpsCamera.ViewMatrix );
				d3dDevice.SetTransform ( TransformType.Projection, fpsCamera.ProjMatrix );

				lastTextureId = -1;
				lastLightmapId = -1;
				d3dDevice.SetTexture ( 0, surfTexture );
				d3dDevice.SetTextureStageState ( 0, TextureStageStates.TextureCoordinateIndex, ( int ) TextureCoordinateIndex.PassThru );
				d3dDevice.SetTextureStageState ( 1, TextureStageStates.ColorOperation, ( int ) TextureOperation.Disable );
				ibNeedSet = true;
				bezierIbNeedSet = true;
				d3dDevice.SetStreamSource ( 0, polyVb, 0 );
				d3dDevice.Indices = polyIb;
				d3dDevice.DrawIndexedPrimitives ( PrimitiveType.TriangleList, 0, 0, 4, 0, 2 );
			}

			if ( rtsOnSprite )
				DrawTextureOnSprite ( surfTexture );
		}

		private void DrawTextureOnSprite ( Texture surfTexture ) {
			using ( Sprite sprite = new Sprite ( d3dDevice ) ) {
				sprite.Begin ( SpriteFlags.None );
				sprite.Draw ( surfTexture, new Vector3 ( 0.0f, 0.0f, 0.0f ), new Vector3 ( 0.0f, 0.0f, 0.0f ), 0x00ffffff );
				sprite.End ();
			}
		}

		private void CreatePoly ( out IndexBuffer polyIb, out VertexBuffer polyVb ) {
			polyVb = new VertexBuffer ( typeof ( CustomVertex.PositionNormalTextured ),
				4, d3dDevice, Usage.WriteOnly,
				VertexFormats.PositionNormal | VertexFormats.Texture0, Pool.Managed );

			CustomVertex.PositionNormalTextured [] polyVertices =
				( CustomVertex.PositionNormalTextured [] ) polyVb.Lock ( 0, LockFlags.Discard );
			polyVertices [0].Position = new Vector3 ( -1.0f, -1.0f, 0.0f );
			polyVertices [0].Tu       = 1.0f;
			polyVertices [0].Tv       = 1.0f;
			polyVertices [1].Position = new Vector3 ( -1.0f,  1.0f, 0.0f );
			polyVertices [1].Tu       = 1.0f;
			polyVertices [1].Tv       = 0.0f;
			polyVertices [2].Position = new Vector3 (  1.0f,  1.0f, 0.0f );
			polyVertices [2].Tu       = 0.0f;
			polyVertices [2].Tv       = 0.0f;
			polyVertices [3].Position = new Vector3 (  1.0f, -1.0f, 0.0f );
			polyVertices [3].Tu       = 0.0f;
			polyVertices [3].Tv       = 1.0f;

			polyVb.Unlock ();

			polyIb = new IndexBuffer ( typeof ( int ), 6, d3dDevice, Usage.WriteOnly, Pool.Managed );
			int [] polyIndices = ( int [] ) polyIb.Lock ( 0, LockFlags.Discard );
			polyIndices [0] = 0;
			polyIndices [1] = 1;
			polyIndices [2] = 2;
			polyIndices [3] = 2;
			polyIndices [4] = 3;
			polyIndices [5] = 0;

			polyIb.Unlock ();
		}

		private void DrawToEnvironment () {
			rte.BeginCube ( cubeTexture );

			for ( int i = 0 ; i < 6 ; i++ ) {
				rte.Face ( ( CubeMapFace ) i, ( int ) Filter.None );

				d3dDevice.Transform.World = Matrix.Translation ( teapotPos );
				d3dDevice.Transform.Projection = Matrix.PerspectiveFovLH ( ( float ) Math.PI / 4, 1.0f, 1.0f, 10000.0f );
				d3dDevice.Transform.View = GetViewMatrixForSide ( ( CubeMapFace ) i );

				DrawQ3Map ();
			}
			
			rte.End ( ( int ) Filter.None );
		}

		private Matrix GetViewMatrixForSide ( CubeMapFace face ) {
			Vector3 pos = teapotPos, look = new Vector3 ( 1.0f, 0.0f, 0.0f ), up = new Vector3 ( 0.0f, 1.0f, 0.0f );

			switch ( face ) {
				case CubeMapFace.PositiveX:
					look = new Vector3 (  1.0f,  0.0f,  0.0f );
					up   = new Vector3 (  0.0f,  1.0f,  0.0f );
					break;
				case CubeMapFace.NegativeX:
					look = new Vector3 ( -1.0f,  0.0f,  0.0f );
					up   = new Vector3 (  0.0f,  1.0f,  0.0f );
					break;
				case CubeMapFace.PositiveY:
					look = new Vector3 (  0.0f,  1.0f,  0.0f );
					up   = new Vector3 (  0.0f,  0.0f, -1.0f );
					break;
				case CubeMapFace.NegativeY:
					look = new Vector3 (  0.0f, -1.0f,  0.0f );
					up   = new Vector3 (  0.0f,  0.0f,  1.0f );
					break;
				case CubeMapFace.PositiveZ:
					look = new Vector3 (  0.0f,  0.0f,  1.0f );
					up   = new Vector3 (  0.0f,  1.0f,  0.0f );
					break;
				case CubeMapFace.NegativeZ:
					look = new Vector3 (  1.0f,  0.0f, -1.0f );
					up   = new Vector3 (  0.0f,  1.0f,  0.0f );
					break;
				default:
					break;
			}

			return	Matrix.LookAtLH ( pos, look, up );
		}

		private void DrawEnvironment () {
			/*if ( RteEnabled ) {
				if ( envPolysIndices [0] == null ) {
					for ( int i = 0 ; i < 6 ; i++ )
						CreatePoly ( out envPolysIndices [0], out envPolysVertices [0] );
				}

				lastTextureId = -1;
				lastLightmapId = -1;
				ibNeedSet = true;
				bezierIbNeedSet = true;

				d3dDevice.SetTextureStageState ( 0, TextureStageStates.TextureCoordinateIndex, ( int ) TextureCoordinateIndex.PassThru );
				d3dDevice.SetTextureStageState ( 1, TextureStageStates.ColorOperation, ( int ) TextureOperation.Disable );
				d3dDevice.SetTransform ( TransformType.View , fpsCamera.ViewMatrix );
				d3dDevice.SetTransform ( TransformType.Projection, fpsCamera.ProjMatrix );

				for ( int i = 0 ; i < 1 ; i++ ) {
					d3dDevice.SetTransform ( TransformType.World, Matrix.Translation ( fpsCamera.Position + fpsCamera.Look * 5 + fpsCamera.Right * 2 - fpsCamera.Up ) );
					d3dDevice.DrawIndexedPrimitives ( PrimitiveType.TriangleList, 0, 0, 4, 0, 2 );
				}
			}*/

			if ( RteEnabled ) {
				using ( Mesh teapot = Mesh.Sphere ( d3dDevice, 2.0f, 16, 16 ) ) {
					lastTextureId = -1;
					lastLightmapId = -1;
					d3dDevice.SetTexture ( 0, cubeTexture );
					d3dDevice.SetTextureStageState ( 0, TextureStageStates.TextureCoordinateIndex, ( int ) TextureCoordinateIndex.SphereMap );
					d3dDevice.SetTextureStageState ( 0, TextureStageStates.ColorOperation, ( int ) TextureOperation.SelectArg1 );
					d3dDevice.SetTextureStageState ( 0, TextureStageStates.ColorArgument1, ( int ) TextureArgument.TextureColor );
					/*d3dDevice.SetTextureStageState ( 0, TextureStageStates.TextureTransform, ( int ) TextureTransform.Count3 );
					d3dDevice.SamplerState [0].AddressU = TextureAddress.Clamp;
					d3dDevice.SamplerState [0].AddressV = TextureAddress.Clamp;
					d3dDevice.SamplerState [0].AddressW = TextureAddress.Clamp;*/
					d3dDevice.SetTextureStageState ( 1, TextureStageStates.ColorOperation, ( int ) TextureOperation.Disable );

					d3dDevice.Transform.World = Matrix.Scaling ( 10.0f, 10.0f, 10.0f ) * Matrix.Translation ( teapotPos );
					d3dDevice.Transform.View = fpsCamera.ViewMatrix;
					d3dDevice.Transform.Projection = fpsCamera.ProjMatrix;

					using ( VertexBuffer vb = teapot.VertexBuffer ) {
						using ( IndexBuffer ib = teapot.IndexBuffer ) {
							ibNeedSet = true;
							bezierIbNeedSet = true;
							d3dDevice.VertexFormat = teapot.VertexFormat;
							d3dDevice.SetStreamSource ( 0, vb, 0, VertexInformation.GetFormatSize ( teapot.VertexFormat ) );
							d3dDevice.Indices = ib;
							d3dDevice.DrawIndexedPrimitives ( PrimitiveType.TriangleList, 0, 0, teapot.NumberVertices, 0, teapot.NumberFaces );
							//teapot.DrawSubset ( 0 );
						}
					}
				}
			}
		}

		private void DrawQ3Map () {
			d3dDevice.Clear ( ClearFlags.Target | ClearFlags.ZBuffer, bgColor, 1.0f, 0 );
			ResetDeviceState ();

			d3dDevice.SetTransform ( TransformType.World, Matrix.Identity );
			d3dDevice.SetTransform ( TransformType.View , fpsCamera.ViewMatrix );
			d3dDevice.SetTransform ( TransformType.Projection, fpsCamera.ProjMatrix );

			int faceIndex = 0;

			ReadOnlyCollection <Q3BspFace> faces = map.Faces;

			//AutoResetEvent.WaitAll ( bufferLocks );

			if ( shaderEnabled && effect != null ) {
				effect.SetValue ( EffectHandle.FromString ( "worldViewProj" ),
					d3dDevice.Transform.World *
					d3dDevice.Transform.View  * 
					d3dDevice.Transform.Projection );

				effect.SetValue ( EffectHandle.FromString ( "LightEnabled" ), lightEnabled );

				if ( lightEnabled ) {
					effect.SetValue ( EffectHandle.FromString ( "LightPos" ), lightPos );
					effect.SetValue ( EffectHandle.FromString ( "EyePos" ),
						new Vector4 ( fpsCamera.Position.X, fpsCamera.Position.Y, fpsCamera.Position.Z, 1.0f ) );
				}

				effect.SetValue ( EffectHandle.FromString ( "LightmapsEnabled" ),
					( texturingFlags & TexturingFlags.Lightmaps ) == TexturingFlags.Lightmaps );
			}

			while ( visibleFaces [faceIndex] != -1 ) {
				if ( shaderEnabled && effect != null ) {
					int numPasses = effect.Begin ( FX.None );

					for ( int i = 0 ; i < numPasses ; i++ ) {
						effect.BeginPass ( i );
						DrawFace ( visibleFaces [faceIndex] );
						effect.EndPass ();
					}

					effect.End ();
				} else
					DrawFace ( visibleFaces [faceIndex] );

				faceIndex++;
			}

			//foreach ( WaitHandle handle in bufferLocks )
			//	( handle as AutoResetEvent ).Set ();
		}

		private void DrawFace ( int faceIndex ) {
			ReadOnlyCollection <Q3BspFace> faces = map.Faces;
			Q3BspFace face = faces [faceIndex];

			d3dDevice.SetTextureStageState ( 0, TextureStageStates.ColorOperation, ( int ) TextureOperation.SelectArg1 );

			if ( texturingFlags == TexturingFlags.Nothing ) {
				lastLightmapId = -1;
				lastTextureId  = -1;
				SetShaderTexture ( 0, null );
				SetShaderTexture ( 1, null );
				d3dDevice.SetTexture ( 0, null );
				d3dDevice.SetTexture ( 1, null );
			} else {
				if ( ( ( texturingFlags & TexturingFlags.Textures ) == TexturingFlags.Textures ) ) {
					if ( lastTextureId != face.texture ) {
						d3dDevice.SetTexture ( 0, textures [face.texture] != null ? textures [face.texture] : textureNotFound );
						SetShaderTexture ( 0, textures [face.texture] != null ? textures [face.texture] : textureNotFound );
						lastTextureId = face.texture;
					}
				}

				if ( ( ( texturingFlags & TexturingFlags.Lightmaps ) == TexturingFlags.Lightmaps ) ) {
					if ( face.lm_index < 0 ) {
						d3dDevice.SetTextureStageState ( 1, TextureStageStates.ColorOperation, ( int ) TextureOperation.Disable );

						if ( texturingFlags == TexturingFlags.Lightmaps )
							d3dDevice.SetTextureStageState ( 0, TextureStageStates.ColorOperation, ( int ) TextureOperation.Disable );
					} else if ( lastLightmapId != face.lm_index ) {
						if ( texturingFlags == TexturingFlags.Lightmaps ) {
							d3dDevice.SetTextureStageState ( 1, TextureStageStates.ColorOperation, ( int ) TextureOperation.SelectArg1 );
						} else
							d3dDevice.SetTextureStageState ( 1, TextureStageStates.ColorOperation, ( int ) lightmapTexOp );

						d3dDevice.SetTexture ( 1, lightmaps [face.lm_index] );
						SetShaderTexture ( 1, lightmaps [face.lm_index] );
						lastLightmapId = face.lm_index;
					}
				} else
					d3dDevice.SetTextureStageState ( 1, TextureStageStates.ColorOperation, ( int ) TextureOperation.Disable );
			}

			switch ( face.type ) {
			case Q3BspFaceType.Mesh:
			case Q3BspFaceType.Polygon:
				ibLock.WaitOne ();
				vbLock.WaitOne ();

				if ( !ib.Disposed && !vb.Disposed ) {
					if ( ibNeedSet ) {
						d3dDevice.SetStreamSource ( 0, vb, 0 );
						d3dDevice.Indices = ib;

						ibNeedSet = false;
					}

					bezierIbNeedSet = true;

					try {
						d3dDevice.DrawIndexedPrimitives ( PrimitiveType.TriangleList, face.vertex,
														  0, face.n_vertexes,
														  face.meshvert, face.n_triangles );
					} catch {}
				}

				vbLock.Set ();
				ibLock.Set ();

				break;
			case Q3BspFaceType.Patch:
				bezierIbLock.WaitOne ();
				bezierVbLock.WaitOne ();

				if ( !bezierIb.Disposed && !bezierVb.Disposed ) {
					if ( bezierIbNeedSet ) {
						d3dDevice.SetStreamSource ( 0, bezierVb, 0 );
						d3dDevice.Indices = bezierIb;
						
						bezierIbNeedSet = false;
					}

					ibNeedSet = true;

					for ( int i = 0 ; i < face.patch.size ; i++ ) {						
						for ( int j = 0 ; j < 5 ; j++ ) {
							Q3BezierPatch bezier = face.patch.bezier [i];

							try {
								d3dDevice.DrawIndexedPrimitives ( PrimitiveType.TriangleStrip,
									bezier.baseVertexIndex,
									0,
									bezier.vertices.Length,
									( int ) ( bezier.baseBufferIndex + bezier.rowIndices [j] ),
									( int ) ( bezier.trisPerRow [j] - 2 ) );
							} catch {}
						}
					}
				}

				bezierVbLock.Set ();
				bezierIbLock.Set ();

				break;
			//case BILLBOARD:
			//	break;
			default:
				break;
			}
		}

		private void InitRenderer () {
			visibleFaces = new int [map.Faces.Count + 1];

			vDecl = Q3VertexFormats.PositionNormalTexturedLightened.Declaration ( d3dDevice );

			LoadTextures ();
			LoadLightmaps ();
		}

		private void LoadTextures () {
			ReadOnlyCollection <Q3BspTexture> texInfos = map.Textures;
			textures = new Texture [texInfos.Count];

			textureNotFound = Texture.FromBitmap ( d3dDevice, Resources.texture_not_found, Usage.None, Pool.Managed );

			for ( int i = 0 ; i < texInfos.Count ; i++ )
				textures [i] = LoadTexture ( texInfos [i] );
		}

		private Texture LoadTexture ( Q3BspTexture texInfo ) {
			Console.Write ( "Loading texture {0}", texInfo.name );

			string saveDir = Path.Combine ( @"D:\Temp\q3bw\", map.MapName );
			var encoders = DR.Imaging.ImageCodecInfo.GetImageEncoders ();
			
			var cmpRegex = new System.Text.RegularExpressions.Regex (
				"(" + string.Join ( "|",
					encoders.Select ( e => string.Join ( "|", e.FilenameExtension.Split ( new [] { "*.", ";" }, StringSplitOptions.RemoveEmptyEntries ) ) ).ToArray ()
				) + ")$",
				System.Text.RegularExpressions.RegexOptions.IgnoreCase );
			

			foreach ( string ext in possibleExtensions ) {
				if ( Q3FileSystem.WriteResourceToStream ( texInfo.name + ext, msTexture ) ) {
					Texture texture;

					try { texture = TextureLoader.FromStream ( d3dDevice, msTexture ); }
					catch { break; }
					
					Console.WriteLine ( ext + "\tsuccess" );

					if ( cmpRegex.IsMatch ( ext ) ) {
						string texPkgPath = texInfo.name + ext;
						string texDir = Path.Combine ( saveDir, Path.GetDirectoryName ( texPkgPath ) );

						/*try {
							Directory.CreateDirectory ( texDir );
							DR.Bitmap bmp = new DR.Bitmap ( msTexture );
							
							for ( int x = 0 ; x < bmp.Width ; x++ ) {
								for ( int y = 0 ; y < bmp.Height ; y++ ) {
									DR.Color c = bmp.GetPixel ( x, y );
									int l = ( c.R + c.G + c.B ) / 3;

									bmp.SetPixel ( x, y, DR.Color.FromArgb ( l, l, l ) );
								}
							}

							string texPath = Path.Combine ( texDir, Path.GetFileNameWithoutExtension ( texInfo.name ) + ext );

							DR.Imaging.ImageCodecInfo encoder = null;

							foreach ( var enc in encoders ) {
								string extWoDot = ext.Substring ( 1 ).ToLower ();

								if ( enc.FilenameExtension.ToLower ().Contains ( extWoDot ) ) {
									encoder = enc;
									break;
								}
							}
							//new DR.Imaging.EncoderParameters ()
							if ( encoder != null )
								bmp.Save ( texPath, encoder, null );
						} catch {}*/
					}

					return	texture;
				}
			}

			Console.WriteLine ( "...\tfailed" );

			return	null;
		}

		private void LoadLightmaps () {
			ReadOnlyCollection <Q3BspLightMap> lightmapInfos = map.Lightmaps;
			lightmaps = new Texture [lightmapInfos.Count];

			for ( int i = 0 ; i < lightmapInfos.Count ; i++ )
				lightmaps [i] = LoadLightmap ( lightmapInfos [i] );
		}

		private Texture LoadLightmap ( Q3BspLightMap lightmapInfo ) {
			Texture lightmap = new Texture ( d3dDevice, 128, 128, 0, Usage.None, Format.X8R8G8B8, Pool.Managed );
			
			Array arr = lightmap.LockRectangle ( typeof ( int ), 0, LockFlags.None, new int [] { 128, 128 } );

			for ( int x = 0 ; x < 128 ; x++ )
			    for ( int y = 0 ; y < 128 ; y++ ) {
			        arr.SetValue ( /*0xff << 24 |*/
								   lightmapInfo.lightmap [x, y, 0] << 16 |
								   lightmapInfo.lightmap [x, y, 1] << 8  |
								   lightmapInfo.lightmap [x, y, 2],
								   x, y );
			    }

			lightmap.UnlockRectangle ( 0 );


			//GraphicsStream gStream = lightmap.LockRectangle ( 0, LockFlags.None );

			//for ( int x = 0 ; x < 128 ; x++ )
			//    for ( int y = 0 ; y < 128 ; y++ ) {
			//        gStream.WriteByte ( 0x00 );
			//        gStream.WriteByte ( lightmapInfo.lightmap [x, y, 0] );
			//        gStream.WriteByte ( lightmapInfo.lightmap [x, y, 1] );
			//        gStream.WriteByte ( lightmapInfo.lightmap [x, y, 2] );
			//    }

			//lightmap.UnlockRectangle ( 0 );

			return	lightmap;
		}

		void vb_Created ( object sender, EventArgs e ) {
			vbLock.WaitOne ();
			GraphicsStream vbStream = vb.Lock ( 0, 0, LockFlags.None );
			ReadOnlyCollection <Q3BspVertex> vertices = map.Vertices;

			for ( int i = 0 ; i < vertices.Count ; i++ ) {
				Q3BspVertex q3v = vertices [i];
				vbStream.Write ( new Q3VertexFormats.PositionNormalTexturedLightened (
					q3v.position, q3v.normal, q3v.texcoord [0], q3v.texcoord [1] ) );
			}

			vb.Unlock ();
			vbLock.Set ();
		}

		void ib_Created ( object sender, EventArgs e ) {
			ibLock.WaitOne ();
			ReadOnlyCollection <int> meshVertices = map.MeshVertices;
			GraphicsStream ibStream = ib.Lock ( 0, 0, LockFlags.None );

			for ( int i = 0 ; i < meshVertices.Count ; i++ )
				ibStream.Write ( meshVertices [i] );

			ib.Unlock ();
			ibLock.Set ();
		}

		void bezierIb_Created ( object sender, EventArgs e ) {
			bezierIbLock.WaitOne ();
			GraphicsStream bezierIbStream = bezierIb.Lock ( 0, 0, LockFlags.Discard );
			ReadOnlyCollection <Q3BspFace> faces = map.Faces;
			Q3BspFace face;

			int indexBufferindex  = 0;

			for ( int faceIndex = 0 ; faceIndex < faces.Count ; faceIndex++ ) {
				face = faces [faceIndex];

				if ( face.type == Q3BspFaceType.Patch ) {
					Q3BspPatch patch = face.patch;

					if ( patch != null ) {
						for ( int bezierIndex = 0 ; bezierIndex < patch.size ; bezierIndex++ ) {
							Q3BezierPatch bezier = patch.bezier [bezierIndex];
							bezier.baseBufferIndex = indexBufferindex;

							for ( uint index = 0 ; index < bezier.indices.Length ; index++ ) {
								bezierIbStream.Write ( bezier.indices [index] );
								indexBufferindex++;
							}
						}
					}
				}
			}

			bezierIb.Unlock ();
			bezierIbLock.Set ();
		}

		void bezierVb_Created ( object sender, EventArgs e ) {
			bezierVbLock.WaitOne ();
			ReadOnlyCollection <Q3BspFace> faces = map.Faces;
			Q3BspFace face;

			GraphicsStream bezierVbStream = bezierVb.Lock ( 0, 0, LockFlags.Discard );

			int vertexBufferindex = 0;

			for ( int faceIndex = 0 ; faceIndex < faces.Count ; faceIndex++ ) {
				face = faces [faceIndex];

				if ( face.type == Q3BspFaceType.Patch ) {
					Q3BspPatch patch = face.patch;

					if ( patch != null ) {
						for ( int bezierIndex = 0 ; bezierIndex < patch.size ; bezierIndex++ ) {
							Q3BezierPatch bezier = patch.bezier [bezierIndex];

							bezier.baseVertexIndex = vertexBufferindex;

							for ( uint vertex = 0 ; vertex < bezier.vertices.Length ; vertex++ ) {
								Q3BspVertex q3v = bezier.vertices [vertex];
								bezierVbStream.Write ( new Q3VertexFormats.PositionNormalTexturedLightened (
									q3v.position, q3v.normal, q3v.texcoord [0], q3v.texcoord [1] ) );
								vertexBufferindex++;
							}
						}
					}
				}
			}

			bezierVb.Unlock ();
			bezierVbLock.Set ();
		}

		private void QSortFaces ( int start, int end ) {
			if ( start < end ) {
				int elem = QSortPartition ( start, end );

				QSortFaces ( start, elem - 1 );
				QSortFaces ( elem + 1, end );
			}
		}

		private int QSortPartition ( int start, int end ) {
			ReadOnlyCollection <Q3BspFace> faces = map.Faces;
			int texture  = faces [visibleFaces [end]].texture;
			int lm_index = faces [visibleFaces [end]].lm_index;
			Q3BspFaceType type = faces[visibleFaces [end]].type;
			int i = start - 1;

			for ( int j = start ; j < end ; j++ ) {
				if ( faces [visibleFaces [j]].texture == texture && 
					 faces [visibleFaces [j]].lm_index == lm_index &&
					 faces [visibleFaces [j]].type <= type ) {
					i++;
					SwapFaces ( i, j );
				} else if ( faces [visibleFaces [j]].texture == texture &&
							faces [visibleFaces [j]].lm_index <= lm_index ) {
					i++;
					SwapFaces ( i, j );
				} else if ( faces [visibleFaces [j]].texture < texture ) {
					i++;
					SwapFaces ( i, j );
				}
			}

			SwapFaces ( i + 1, end );

			return	i + 1;
		}

		private void SwapFaces ( int i, int j ) {
			int temp = visibleFaces [i];
			visibleFaces [i] = visibleFaces [j];
			visibleFaces [j] = temp;
		}
		#endregion Methods
	}
}
