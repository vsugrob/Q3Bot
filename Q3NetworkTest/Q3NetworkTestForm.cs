using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;
using Q3Network;

namespace Q3NetworkTest
{
	public partial class Q3NetworkTestForm : Form
	{
		Q3Connection conn;
		Thread connThread;
		Thread demoThread;
		EntityViewForm frmEntityView = new EntityViewForm ();
		Rectangle prevCursorClip;
		bool mouseClipped;
		Point prevMouseLocation;

		public Q3NetworkTestForm()
		{
			InitializeComponent();

			conn = new Q3Connection ( ConnectionFrom.ClientSide );
			conn.ServerCommandReceived += ServerCommandReceived;
			//refPassTimer = new System.Threading.Timer ( new TimerCallback ( RefPassTimerCallback ), null, 0, 100 );
			frmEntityView.Show ();
			frmEntityView.Connection = conn;
		}

		private void btnConnect_Click(object sender, EventArgs e)
		{
			connThread = new Thread ( ConnectionThread );
			connThread.Start ();
		}

		private void ConnectionThread () {
			//try {
				conn.Connect ( IPAddress.Parse ( txtIP.Text.Trim () ), Convert.ToInt32 ( txtPort.Text ) );
			//} catch ( Exception ex ) {
			//	MessageBox.Show ( ex.Message );
			//}
		}

		private delegate void ServerCommandReceivedInvoker ( object sender, string cmdStr );

		public void ServerCommandReceived ( object sender, string cmdStr ) {
			if ( qtfServerCommands.InvokeRequired ) {
				qtfServerCommands.Invoke ( new ServerCommandReceivedInvoker ( ServerCommandReceived ), sender, cmdStr );
			} else {
				qtfServerCommands.Qtf += "^0" + cmdStr + "\r\n";
				qtfServerCommands.SelectionStart = qtfServerCommands.Text.Length - 1;
				qtfServerCommands.ScrollToCaret ();
			}
		}

		private void btnSendCommand_Click(object sender, EventArgs e)
		{
			conn.AddReliableCommand ( txtCommand.Text );
			txtCommand.Text = "";
		}

		private void txtCommand_KeyDown(object sender, KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.Enter )
				btnSendCommand_Click ( this, null );
		}

		private void txtCommand_TextChanged(object sender, EventArgs e)
		{

		}

		private void Q3NetworkTestForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			// Disconnect
			//if ( connThread != null )
			//	connThread.Interrupt ();
		}

		private void btnPlayDemo_Click(object sender, EventArgs e)
		{
			demoThread = new Thread ( DemoThread );
			demoThread.Start ();
		}

		private void DemoThread () {
			//try {
				conn.PlayDemo ( txtDemoFileName.Text );
			//} catch ( Exception ex ) {
			//	MessageBox.Show ( ex.Message );
			//}
		}

		private void txtUserCommands_KeyDown(object sender, KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.E )
				conn.MovementForward = 127;
			else if ( e.KeyCode == Keys.D )
				conn.MovementForward = 255 - 127;
			else if ( e.KeyCode == Keys.F )
				conn.MovementRight = 127;
			else if ( e.KeyCode == Keys.S )
				conn.MovementRight = 255 - 127;
			else if ( e.KeyCode == Keys.Space )
				conn.MovementUp = 127;
			else if ( e.KeyCode == Keys.ShiftKey )
				conn.ButtonDown ( Q3Network.Button.Attack );
			else if ( e.KeyCode == Keys.T )
				conn.SelectedWeapon = WeaponType.Railgun;
			else if ( e.KeyCode == Keys.G )
				conn.SelectedWeapon = WeaponType.RocketLauncher;
			else if ( e.KeyCode == Keys.A )
				conn.SelectedWeapon = WeaponType.Plasmagun;
			else if ( e.KeyCode == Keys.Q )
				conn.SelectedWeapon = WeaponType.Bfg;
			else if ( e.KeyCode == Keys.W )
				conn.SelectedWeapon = WeaponType.Machinegun;
			else if ( e.KeyCode == Keys.R )
				conn.SelectedWeapon = WeaponType.Shotgun;
			else if ( e.KeyCode == Keys.C )
				conn.SelectedWeapon = WeaponType.Lightning;
			else if ( e.KeyCode == Keys.V )
				conn.SelectedWeapon = WeaponType.GrenadeLauncher;
			else if ( e.KeyCode == Keys.Z )
				conn.SelectedWeapon = WeaponType.Gauntlet;
		}

		private void txtUserCommands_KeyUp(object sender, KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.E )
				conn.MovementForward = 0;
			else if ( e.KeyCode == Keys.D )
				conn.MovementForward = 0;
			else if ( e.KeyCode == Keys.F )
				conn.MovementRight = 0;
			else if ( e.KeyCode == Keys.S )
				conn.MovementRight = 0;
			else if ( e.KeyCode == Keys.Space )
				conn.MovementUp = 0;
			else if ( e.KeyCode == Keys.ShiftKey )
				conn.ButtonUp ( Q3Network.Button.Attack );
			else if ( e.KeyCode == Keys.Escape ) {
				if ( mouseClipped ) {
					Cursor.Clip = prevCursorClip;
					mouseClipped = false;
				}
			}

			txtUserCommands.Text = "";
		}

		private void txtUserCommands_MouseDown(object sender, MouseEventArgs e)
		{
			if ( !mouseClipped ) {
				prevCursorClip = Cursor.Clip;
				Cursor.Clip = txtUserCommands.RectangleToScreen ( new Rectangle ( Point.Empty, new Size ( txtUserCommands.Size.Width - 4, txtUserCommands.Size.Height - 4 ) ) );
				mouseClipped = true;
			} else {
				if ( e.Button == MouseButtons.Left )
					conn.ButtonDown ( Q3Network.Button.Attack );
			}
		}

		private void txtUserCommands_MouseUp(object sender, MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Left )
				conn.ButtonUp ( Q3Network.Button.Attack );
		}

		private void txtUserCommands_MouseMove(object sender, MouseEventArgs e)
		{
			if ( mouseClipped && e.Location != prevMouseLocation ) {
				prevMouseLocation = e.Location;
				Point cp = new Point ( txtUserCommands.Size.Width / 2, txtUserCommands.Size.Height / 2 );
				conn.ViewAngleX -= ( e.X - cp.X ) * 20;
				conn.ViewAngleY += ( e.Y - cp.Y ) * 20;
				Cursor.Position = txtUserCommands.PointToScreen ( cp );
			}
		}
	}
}
