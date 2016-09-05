using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Q3Network
{
	public enum Button {
		Attack = 1,
		Talk = 2,			// displays talk balloon and disables actions
		UseHoldable = 4,
		Gesture = 8,
		Walking = 16,		// walking can't just be infered from MOVE_RUN
							// because a key pressed late in the frame will
							// only generate a small move value for that frame
							// walking will use different animations and
							// won't generate footsteps
		Affirmative = 32,
		Negative = 64,
		GetFlag = 128,
		Guardable = 256,
		Patrol = 512,
		Followme = 1024,
		Any = 2048,			// any key whatsoever
		MoveRun = 120		// if forwardmove or rightmove are >= MOVE_RUN,
							// then BUTTON_WALKING should be set
	}

	// usercmd_t is sent to the server each client frame
	public class UserCommand
	{
		public int serverTime;
		public int [] angles = new int [3];
		public int buttons;
		public WeaponType weapon;
		public byte forwardmove, rightmove, upmove;
	}
}
