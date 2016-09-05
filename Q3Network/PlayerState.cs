using System;
using System.Reflection;

namespace Q3Network
{
	// playerState_t is the information needed by both the client and server
	// to predict player motion and actions
	// nothing outside of pmove should modify these, or some degree of prediction error
	// will occur

	// you can't add anything to this without modifying the code in msg.c

	// playerState_t is a full superset of entityState_t as it is used by players,
	// so if a playerState_t is transmitted, the entityState_t can be fully derived
	// from it.
	public class PlayerState
	{
		public const int MAX_STATS      = 16;
		public const int MAX_PERSISTANT = 16;
		public const int MAX_POWERUPS   = 16;
		public const int MAX_WEAPONS    = 16;
		public const int MAX_PS_EVENTS  = 2;
		public const int PS_PMOVEFRAMECOUNTBITS = 6;

		public int commandTime;	// cmd->serverTime of last executed command
		public int pm_type;
		public int bobCycle;	// for view bobbing and footstep generation
		public int pm_flags;	// ducked, jump_held, etc
		public int pm_time;
		public float [] origin   = new float [3];
		public float [] velocity = new float [3];
		public int weaponTime;
		public int gravity;
		public int speed;
		public int [] delta_angles = new int [3];	// add to command angles to get view direction
													// changed by spawns, rotating objects, and teleporters
		public int groundEntityNum;	// ENTITYNUM_NONE = in air
		public int legsTimer;		// don't change low priority animations until this runs out
		public int legsAnim;		// mask off ANIM_TOGGLEBIT
		public int torsoTimer;		// don't change low priority animations until this runs out
		public int torsoAnim;		// mask off ANIM_TOGGLEBIT
		public int movementDir;		// a number 0 to 7 that represents the reletive angle
									// of movement to the view angle (axial and diagonals)
									// when at rest, the value will remain unchanged
									// used to twist the legs during strafing
		public float [] grapplePoint = new float [3];	// location of grapple to pull towards if PMF_GRAPPLE_PULL
		public int eFlags;				// copied to entityState_t->eFlags
		public int eventSequence;		// pmove generated events
		public int [] events = new int [MAX_PS_EVENTS];
		public int [] eventParms = new int [MAX_PS_EVENTS];
		public int externalEvent;		// events set on player from another source
		public int externalEventParm;
		public int externalEventTime;
		public int clientNum;			// ranges from 0 to MAX_CLIENTS-1
		public int weapon;				// copied to entityState_t->weapon
		public int weaponstate;
		public float [] viewangles = new float [3];	// for fixed views
		public int viewheight;
		// damage feedback
		public int damageEvent;	// when it changes, latch the other parms
		public int damageYaw;
		public int damagePitch;
		public int damageCount;
		public int [] stats = new int [MAX_STATS];
		public int [] persistant = new int [MAX_PERSISTANT];	// stats that aren't cleared on death
		public int [] powerups = new int [MAX_POWERUPS];	// level.time that the powerup runs out
		public int [] ammo = new int [MAX_WEAPONS];
		public int generic1;
		public int loopSound;
		public int jumppad_ent;	// jumppad entity hit this frame
								// not communicated over the net at all
		public int ping;				// server to game info for scoreboard
		public int pmove_framecount;	// FIXME: don't transmit over the network
		public int jumppad_frame;
		public int entityEventSequence;

		public void CopyTo ( PlayerState dest ) {
			FieldInfo [] fis = typeof ( PlayerState ).GetFields ();
			Array arrThisField, arrDestField;

			foreach ( FieldInfo fi in fis )
				if ( fi.FieldType.IsArray ) {
					if ( null != ( arrThisField  = ( Array ) fi.GetValue ( this  ) ) &&
						 null != ( arrDestField = ( Array ) fi.GetValue ( dest ) ) )
						for ( int i = 0 ; i < arrThisField.Length ; i++ )
							arrDestField.SetValue ( arrThisField.GetValue ( i ), i );
				} else if ( !fi.IsLiteral )
					fi.SetValue ( dest, fi.GetValue ( this ) );
		}

		public static NetField [] fields = new NetField [] {
				new NetField ( "commandTime", 0x00, 0x20 ),
				new NetField ( "origin[0]", 0x14, 0x00 ),
				new NetField ( "origin[1]", 0x18, 0x00 ),
				new NetField ( "bobCycle", 0x08, 0x08 ),
				new NetField ( "velocity[0]", 0x20, 0x00 ),
				new NetField ( "velocity[1]", 0x24, 0x00 ),
				new NetField ( "viewangles[1]", 0x9c, 0x00 ),
				new NetField ( "viewangles[0]", 0x98, 0x00 ),
				new NetField ( "weaponTime", 0x2c, 0xfffffff0 ),
				new NetField ( "origin[2]", 0x1c, 0x00 ),
				new NetField ( "velocity[2]", 0x28, 0x00 ),
				new NetField ( "legsTimer", 0x48, 0x08 ),
				new NetField ( "pm_time", 0x10, 0xfffffff0 ),
				new NetField ( "eventSequence", 0x6c, 0x10 ),
				new NetField ( "torsoAnim", 0x54, 0x08 ),
				new NetField ( "movementDir", 0x58, 0x04 ),
				new NetField ( "events[0]", 0x70, 0x08 ),
				new NetField ( "legsAnim", 0x4c, 0x08 ),
				new NetField ( "events[1]", 0x74, 0x08 ),
				new NetField ( "pm_flags", 0x0c, 0x10 ),
				new NetField ( "groundEntityNum", 0x44, 0x0a ),
				new NetField ( "weaponstate", 0x94, 0x04 ),
				new NetField ( "eFlags", 0x68, 0x10 ),
				new NetField ( "externalEvent", 0x80, 0x0a ),
				new NetField ( "gravity", 0x30, 0x10 ),
				new NetField ( "speed", 0x34, 0x10 ),
				new NetField ( "delta_angles[1]", 0x3c, 0x10 ),
				new NetField ( "externalEventParm", 0x84, 0x08 ),
				new NetField ( "viewheight", 0xa4, 0xfffffff8 ),
				new NetField ( "damageEvent", 0xa8, 0x08 ),
				new NetField ( "damageYaw", 0xac, 0x08 ),
				new NetField ( "damagePitch", 0xb0, 0x08 ),
				new NetField ( "damageCount", 0xb4, 0x08 ),
				new NetField ( "generic1", 0x1b8, 0x08 ),
				new NetField ( "pm_type", 0x04, 0x08 ),
				new NetField ( "delta_angles[0]", 0x38, 0x10 ),
				new NetField ( "delta_angles[2]", 0x40, 0x10 ),
				new NetField ( "torsoTimer", 0x50, 0x0c ),
				new NetField ( "eventParms[0]", 0x78, 0x08 ),
				new NetField ( "eventParms[1]", 0x7c, 0x08 ),
				new NetField ( "clientNum", 0x8c, 0x08 ),
				new NetField ( "weapon", 0x90, 0x05 ),
				new NetField ( "viewangles[2]", 0xa0, 0x00 ),
				new NetField ( "grapplePoint[0]", 0x5c, 0x00 ),
				new NetField ( "grapplePoint[1]", 0x60, 0x00 ),
				new NetField ( "grapplePoint[2]", 0x64, 0x00 ),
				new NetField ( "jumppad_ent", 0x1c0, 0x0a ),
				new NetField ( "loopSound", 0x1bc, 0x10 )
			};
	}
}
