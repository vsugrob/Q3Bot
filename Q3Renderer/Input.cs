using System.Windows.Forms;
using Microsoft.DirectX.DirectInput;

namespace Q3Renderer
{
	public class Input
	{
		public enum MouseButton {
			Left = 0,
			Right,
			Middle
		}

		#region Properties
		private Device keyboard;
		private KeyboardState keyboardState;
		private Device mouse;
		private MouseState mouseState;
		#endregion Properties

		#region Constructors
		public Input ( Control parent, CooperativeLevelFlags keyboardFlags, CooperativeLevelFlags mouseFlags ) {
			keyboard = new Device ( SystemGuid.Keyboard );
			keyboard.SetCooperativeLevel ( parent, keyboardFlags );
			keyboard.Acquire ();

			mouse = new Device ( SystemGuid.Mouse );
			mouse.SetCooperativeLevel ( parent, mouseFlags );
			mouse.Acquire ();
		}
		#endregion Constructors

		#region Methods
		public void Poll () {
			keyboardState = keyboard.GetCurrentKeyboardState ();
			mouseState = ( MouseState ) mouse.CurrentMouseState;
		}

		public bool KeyDown ( Key key ) {
			if ( null == keyboardState ) Poll ();
			
			return	keyboardState [key];
		}

		public bool MouseButtonDown ( MouseButton button ) {
			return	( mouseState.GetMouseButtons () [( int ) button] & 0x80 ) != 0;
		}

		public int MouseDeltaX { get { return	mouseState.X; } }
		public int MouseDeltaY { get { return	mouseState.Y; } }
		#endregion Methods
	}
}
