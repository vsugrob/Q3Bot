using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Q3Renderer.Properties;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Q3Renderer
{
	public partial class Q3ModelViewerForm : Form
	{
		private Md3PropertiesForm frmMd3Properties = new Md3PropertiesForm ();
		private Device d3dDevice;
		private Input input;

		// Camera
		private FpsCamera camera;

		private long prevTicks;
		Md3Model model;
		private int currentSubmeshId = 0;

		public Texture textureNotFound;
		private bool inputGrabbed = false;
		private Point grabbedAtPosition;

		public Q3ModelViewerForm()
		{
			InitializeComponent();

			this.SetStyle ( ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true );
		}

		private void Q3ModelViewerForm_Load(object sender, EventArgs e)
		{
			PresentParameters pp = new PresentParameters ();
			pp.Windowed = true;
			pp.SwapEffect = SwapEffect.Discard;
			pp.EnableAutoDepthStencil = true;
			pp.AutoDepthStencilFormat = DepthFormat.D24X8;
			pp.BackBufferFormat = Format.A8R8G8B8;

			d3dDevice = new Device ( 0, DeviceType.Hardware, this,
				CreateFlags.HardwareVertexProcessing | CreateFlags.PureDevice, pp );

			d3dDevice.DeviceReset += new EventHandler(d3dDevice_DeviceReset);

			d3dDevice.RenderState.CullMode = Cull.None;
			d3dDevice.TextureState [0].ColorOperation = TextureOperation.SelectArg1;
			d3dDevice.SamplerState [0].MinFilter = TextureFilter.Anisotropic;
			d3dDevice.SamplerState [0].MagFilter = TextureFilter.Anisotropic;
			d3dDevice.SamplerState [0].MipFilter = TextureFilter.Anisotropic;

			input = new Input ( this,
				Microsoft.DirectX.DirectInput.CooperativeLevelFlags.Background | Microsoft.DirectX.DirectInput.CooperativeLevelFlags.NonExclusive,
				Microsoft.DirectX.DirectInput.CooperativeLevelFlags.Background | Microsoft.DirectX.DirectInput.CooperativeLevelFlags.NonExclusive );
			camera = new FpsCamera ( input,
				( float ) d3dDevice.PresentationParameters.BackBufferWidth / d3dDevice.PresentationParameters.BackBufferHeight,
				( float ) Math.PI / 4, 1.0f, 5000.0f );
			camera.Position = new Vector3 ( 0.0f, 20.0f, -20 );
			camera.Sensitivity = 5.0f;
			camera.MoveSpeed = 50.0f;

			textureNotFound = Texture.FromBitmap ( d3dDevice, Resources.texture_not_found, Usage.None, Pool.Managed );

			this.LoadModel ( @"models/weapons2/rocketl/rocketl.md3" );
		}

		void d3dDevice_DeviceReset(object sender, EventArgs e)
		{
			camera.Aspect = ( float ) d3dDevice.PresentationParameters.BackBufferWidth / d3dDevice.PresentationParameters.BackBufferHeight;
		}

		private void Q3ModelViewerForm_Paint(object sender, PaintEventArgs e)
		{
			UpdateWorld ();

			try {
				d3dDevice.Clear ( ClearFlags.Target | ClearFlags.ZBuffer, Color.White, 1.0f, 0 );
				d3dDevice.BeginScene ();

				d3dDevice.Transform.World = Matrix.Identity;
				d3dDevice.Transform.View = camera.ViewMatrix;
				d3dDevice.Transform.Projection = camera.ProjMatrix;

				DrawAxes ();

				if ( currentSubmeshId == model.meshes.Length )
					model.Draw ();
				else
					model.DrawSubmesh ( currentSubmeshId );

				d3dDevice.EndScene ();
				d3dDevice.Present ();
			} catch {}

			this.Invalidate ();
			tlsViewerOptions.Update ();
		}

		private void UpdateWorld () {
			long curTicks = DateTime.Now.Ticks;
			float timeDelta = ( float ) ( curTicks - prevTicks ) / TimeSpan.TicksPerSecond;
			input.Poll ();

			if ( inputGrabbed ) {
				camera.Update ( timeDelta );
				Cursor.Position = grabbedAtPosition;
			}

			prevTicks = curTicks;
		}

		public void LoadModel ( string path ) {
			Md3Model mdl = new Md3Model ( path, d3dDevice, this );
			SetModel ( mdl );
		}

		public void SetModel ( Md3Model model ) {
			if ( frmMd3Properties == null )
				frmMd3Properties = new Md3PropertiesForm ();

			this.model = model;
			frmMd3Properties.SetModel ( model );

			frmMd3Properties.Show ();
			frmMd3Properties.Location = new Point ( this.Left + this.Width + 20, this.Top );
			frmMd3Properties.Update ();

			camera.Reset ();
			camera.Position = model.totalCenter - new Vector3 ( 0.0f, 0.0f, model.totalRadius * 3 );
			camera.MoveSpeed = model.totalRadius * 3;

			SetSubmeshId ( model.meshes.Length );
		}

		private void Q3ModelViewerForm_MouseDown(object sender, MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Left ) {
				if ( inputGrabbed = !inputGrabbed ) {
					Cursor.Hide ();
					grabbedAtPosition = Cursor.Position;
				} else {
					Cursor.Show ();
					Cursor.Position = grabbedAtPosition;
				}
			}
		}

		public void SetSubmeshId ( int id ) {
			if ( id == model.meshes.Length ) {
				currentSubmeshId = id;
				lblSubmeshId.Text = "Show All";
			} else if ( id == model.meshes.Length + 1 ) {
				currentSubmeshId = 0;
				lblSubmeshId.Text = currentSubmeshId.ToString ();
			} else {
				currentSubmeshId = Math.Abs ( id ) % model.meshes.Length;
				lblSubmeshId.Text = currentSubmeshId.ToString ();
			}
		}

		private void btnNextSubmesh_Click(object sender, EventArgs e)
		{
			SetSubmeshId ( currentSubmeshId + 1 );
		}

		private void Q3ModelViewerForm_KeyDown(object sender, KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.N )
				SetSubmeshId ( currentSubmeshId + 1 );
			else if ( e.KeyCode == Keys.W )
				d3dDevice.RenderState.FillMode = d3dDevice.RenderState.FillMode == FillMode.WireFrame ? FillMode.Solid : FillMode.WireFrame;
		}

		private void DrawAxes () {
			// Axes
			float discretion = 10;
			float side = model.totalRadius * 2;
			float sideStep = side / discretion;

			CustomVertex.PositionColored [] axis = new CustomVertex.PositionColored [2];
			axis [0] = new CustomVertex.PositionColored ( -side, 0.0f, 0.0f, 0x00cccccc );
			axis [1] = new CustomVertex.PositionColored (  side, 0.0f, 0.0f, 0x00cccccc );
			d3dDevice.VertexFormat = CustomVertex.PositionColored.Format;

			d3dDevice.Transform.World *= Matrix.Translation ( 0.0f, 0.0f, -side );

			for ( int i = 0 ; i <= discretion * 2; i++ ) {
				d3dDevice.DrawUserPrimitives ( PrimitiveType.LineList, 1, axis );
				d3dDevice.Transform.World *= Matrix.Translation ( 0.0f, 0.0f, sideStep );
			}

			d3dDevice.Transform.World = Matrix.RotationZ ( ( float ) Math.PI / 2 );
			//d3dDevice.Transform.World *= Matrix.Translation ( 0.0f, -side, 0.0f );

			//for ( int i = 0 ; i <= discretion * 2; i++ ) {
				d3dDevice.DrawUserPrimitives ( PrimitiveType.LineList, 1, axis );
				d3dDevice.Transform.World *= Matrix.Translation ( 0.0f, 0.0f, sideStep );
			//}

			d3dDevice.Transform.World = Matrix.RotationY ( ( float ) Math.PI / 2 );
			d3dDevice.Transform.World *= Matrix.Translation ( -side, 0.0f, 0.0f );

			for ( int i = 0 ; i <= discretion * 2 ; i++ ) {
				d3dDevice.DrawUserPrimitives ( PrimitiveType.LineList, 1, axis );
				d3dDevice.Transform.World *= Matrix.Translation ( sideStep, 0.0f, 0.0f );
			}

			d3dDevice.Transform.World = Matrix.Identity;
		}
	}
}
