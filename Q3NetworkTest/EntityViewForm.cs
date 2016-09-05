using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Q3Network;

namespace Q3NetworkTest
{
	public partial class EntityViewForm : Form
	{
		public Q3Connection Connection { get; set; }
		private Rectangle worldRect    = new Rectangle ( -1000, -1000, 2000, 2000 );
		private Rectangle newWorldRect = new Rectangle ();
		private Bitmap bmpCanvas;
		private Size canvasPrevSize;
		private Graphics grphCanvas;
		private Graphics grphView;
		private int entityWidth  = 4;
		private int entityHeight = 4;
		private Brush defaultEntityBrush = Brushes.Black;
		private Brush [] entityTypeBrushes = new Brush [14];

		private Color bkColor = Color.White;

		public EntityViewForm()
		{
			InitializeComponent();

			entityTypeBrushes [( int )EntityType.Item] = Brushes.Green;
			entityTypeBrushes [( int )EntityType.Missile] = Brushes.DarkGray;
			entityTypeBrushes [( int )EntityType.Player] = Brushes.Violet;
			entityTypeBrushes [( int )EntityType.PushTrigger] = Brushes.Yellow;
			/*General,
			Player,
			Item,
			Missile,
			Mover,
			Beam,
			Portal,
			Speaker,
			PushTrigger,
			TeleportTrigger,
			Invisible,
			Grapple,				// grapple hooked on wall
			Team,
			Events*/

			tmrRender.Tick += new EventHandler ( tmrRender_Tick );
		}

		void tmrRender_Tick ( object sender, EventArgs e )
		{
			if ( canvasPrevSize != pnlView.ClientSize ) {
				grphView = pnlView.CreateGraphics ();
				bmpCanvas = new Bitmap ( pnlView.ClientSize.Width, pnlView.ClientSize.Height );
				grphCanvas = Graphics.FromImage ( bmpCanvas );
				canvasPrevSize = pnlView.ClientSize;
			}

			//Connection.Snap.
			EntityState [] tits = radBaselines.Checked ? Connection.EntityBaselines : Connection.ParseEntities;
			grphCanvas.Clear ( bkColor );

			foreach ( EntityState entity in tits ) {
				int x = ( ( int ) entity.pos.trBase [0] - worldRect.Left ) * pnlView.Width  / worldRect.Width;
				int y = ( ( int ) entity.pos.trBase [1] - worldRect.Top  ) * pnlView.Height / worldRect.Height;
				
				Brush b = ( ( int ) entity.eType ) <= entityTypeBrushes.Length ?
					( entityTypeBrushes [( int ) entity.eType] != null ? entityTypeBrushes [( int ) entity.eType] : defaultEntityBrush ) :
					defaultEntityBrush;
				grphCanvas.FillRectangle ( b, bmpCanvas.Width - ( x - entityWidth / 2 ), y - entityHeight / 2,
										 entityWidth, entityHeight );

				newWorldRect.X = ( int ) entity.pos.trBase [0] < worldRect.Left ? ( int ) entity.pos.trBase [0] : worldRect.Left;
				newWorldRect.Y = ( int ) entity.pos.trBase [1] < worldRect.Top  ? ( int ) entity.pos.trBase [1] : worldRect.Top;
				newWorldRect.Width  = ( int ) entity.pos.trBase [0] > worldRect.Left + worldRect.Width  ? ( int ) entity.pos.trBase [0] - worldRect.Left : worldRect.Width;
				newWorldRect.Height = ( int ) entity.pos.trBase [1] < worldRect.Top  + worldRect.Height ? ( int ) entity.pos.trBase [1] - worldRect.Top  : worldRect.Height;
			}

			worldRect = Rectangle.Union ( worldRect, newWorldRect );
			grphView.DrawImageUnscaled ( bmpCanvas, 0, 0 );
		}

		private void EntityViewForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if ( e.CloseReason == CloseReason.UserClosing ) {
				e.Cancel = true;
				this.Hide ();
			}
		}

		private void btnStartRender_Click(object sender, EventArgs e)
		{
			tmrRender.Start ();
		}

		private void EntityViewForm_Load(object sender, EventArgs e)
		{
			grphView = pnlView.CreateGraphics ();
		}
	}
}
