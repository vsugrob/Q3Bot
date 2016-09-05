using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Microsoft.DirectX.Direct3D;

namespace Q3Renderer
{
	public partial class Q3RendererForm : Form
	{
		Device d3dDevice;
		Renderer renderer;
		PresentParameters pp = new PresentParameters ();
		Thread renderingThread;
		long prevTicks;
		private bool paused;
		private string mapPath = "maps/q3ctf1.bsp";

		private float deltaSum;
		private int frames;

		public Q3RendererForm()
		{
			InitializeComponent();
		}

		public Q3RendererForm( string mapPath )
		{
			InitializeComponent();

			this.mapPath = mapPath;
		}

		public void Init () {
			Q3Map map = new Q3Map ( mapPath );
			CreateDevice ();

			renderer = new Renderer ( map, d3dDevice );
			renderer.OnResetDevice ();
		}

		private void CreateDevice () {
			pp.BackBufferFormat = Format.Unknown;
			pp.BackBufferWidth  = 0;
			pp.BackBufferHeight = 0;	
			pp.BackBufferCount  = 1;
			pp.MultiSample      = MultiSampleType.None;
			pp.SwapEffect       = SwapEffect.Discard; 
			pp.DeviceWindow     = this;
			pp.Windowed         = true;
			pp.PresentFlag      = PresentFlag.None;
			pp.MultiSampleQuality     = 0;
			pp.EnableAutoDepthStencil = true; 
			pp.AutoDepthStencilFormat = DepthFormat.D24X8;
			pp.FullScreenRefreshRateInHz = 0;
			pp.BackBufferFormat = Format.A8R8G8B8;
			pp.PresentationInterval      = PresentInterval.Default;
			
			d3dDevice = new Device ( 0, DeviceType.Hardware, this,
				CreateFlags.HardwareVertexProcessing | CreateFlags.PureDevice, pp );
			
			d3dDevice.DeviceLost  += new EventHandler ( d3dDevice_DeviceLost  );
			d3dDevice.DeviceReset += new EventHandler ( d3dDevice_DeviceReset );
		}

		private void d3dDevice_DeviceReset ( object sender, EventArgs e ) {
			renderer.OnResetDevice ();
		}

		private void d3dDevice_DeviceLost ( object sender, EventArgs e ) {
			renderer.OnLostDevice ();
		}

		public void Draw () {
			renderer.Draw ();
		}

		public void UpdateScene ( float delta ) {
			renderer.Update ( delta );
		}

		private void Q3RendererForm_Load(object sender, EventArgs e)
		{
			Init ();
			renderingThread = new Thread ( new ThreadStart ( renderingThreadCallback ) );
			prevTicks = DateTime.Now.Ticks;
			renderingThread.Start ();
		}

		private void renderingThreadCallback () {
			while ( Thread.CurrentThread.IsAlive ) {
				if ( paused )
					Thread.Sleep ( 10 );

				float delta = ( float ) ( DateTime.Now.Ticks - prevTicks ) / ( float ) TimeSpan.TicksPerSecond;
				prevTicks = DateTime.Now.Ticks;

				frames++;

				if ( deltaSum >= 1.0f ) {
					UpdateFps ( frames );

					deltaSum = 0.0f;
					frames = 0;
				} else
					deltaSum += delta;

				UpdateScene ( delta );
				Draw ();
			}
		}

		private void Q3RendererForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			renderingThread.Abort ();
		}

		private void Q3RendererForm_SizeChanged(object sender, EventArgs e)
		{
			if ( paused && WindowState != FormWindowState.Minimized )
				paused = false;
			else if ( this.WindowState == FormWindowState.Minimized )
				paused = true;

			if ( WindowState == FormWindowState.Maximized ) {
				d3dDevice.PresentationParameters.BackBufferWidth  = ClientRectangle.Width;
				d3dDevice.PresentationParameters.BackBufferHeight = ClientRectangle.Height;
				d3dDevice_DeviceLost ( this, e );
				d3dDevice_DeviceReset ( this, e );
			}
		}

		private delegate void UpdateFpsInvoker ( int fps );

		private void UpdateFps ( int fps ) {
			if ( this.InvokeRequired )
				this.Invoke ( new UpdateFpsInvoker ( UpdateFps ), fps );
			else
				this.Text = string.Format ( "{0} FPS: {1}", mapPath, fps );
		}

		private void Q3RendererForm_KeyDown(object sender, KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.Enter )
				renderer.VisibilityFixed = !renderer.VisibilityFixed;
			else if ( e.KeyCode == Keys.Back )
				renderer.FillMode = renderer.FillMode == FillMode.Solid ? FillMode.WireFrame : FillMode.Solid;
			else if ( e.KeyCode == Keys.CapsLock )
				renderer.TexturingFlags = ( TexturingFlags ) ( ( int ) renderer.TexturingFlags + 1 );
			else if ( e.KeyCode == Keys.D1 )
				renderer.RtsEnabled = !renderer.RtsEnabled;
			else if ( e.KeyCode == Keys.D2 )
				renderer.RtsOn3DPoly = !renderer.RtsOn3DPoly;
			else if ( e.KeyCode == Keys.D3 )
				renderer.RtsOnSprite = !renderer.RtsOnSprite;
			else if ( e.KeyCode == Keys.D4 )
				renderer.RteEnabled = !renderer.RteEnabled;
			else if ( e.KeyCode == Keys.L )
				renderer.ReloadShader ();
			else if ( e.KeyCode == Keys.P )
				renderer.ShaderEnabled = !renderer.ShaderEnabled;
			else if ( e.KeyCode == Keys.O )
				renderer.LightEnabled = !renderer.LightEnabled;
			else if ( e.KeyCode == Keys.D0 ) {
				PresentParameters pp = new PresentParameters ( d3dDevice.PresentationParameters );
				pp.Windowed = !pp.Windowed;
				pp.BackBufferWidth = 1440;
				pp.BackBufferHeight = 900;
				pp.DeviceWindow = null;
				d3dDevice.Reset ( pp );
			}
		}
	}
}
