using System;
using System.Reflection;
//using System.Windows.Media.Media3D;

namespace Q3Network	// FIXIT: Move this out of here!
{
	public enum EntityType {
		General,
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
		Events					// any of the EV_* events can be added freestanding
								// by setting eType to ET_EVENTS + eventNum
								// this avoids having to set eFlags and eventNum
	}

	// entityState_t is the information conveyed from the server
	// in an update message about entities that the client will
	// need to render in some way
	// Different eTypes may use the information in different ways
	// The messages are delta compressed, so it doesn't really matter if
	// the structure size is fairly large

	public class EntityState
	{
		public int number;				// entity index
		public EntityType eType;
		public int eFlags;
		public Trajectory pos  = new Trajectory ();		// for calculating position
		public Trajectory apos = new Trajectory ();		// for calculating angles
		public int time;
		public int time2;
		public float [] origin  = new float [3];
		public float [] origin2 = new float [3];
		public float [] angles  = new float [3];
		public float [] angles2 = new float [3];
		public int otherEntityNum;		// shotgun sources, etc
		public int otherEntityNum2;
		public int groundEntityNum;		// -1 = in air
		public int constantLight;		// r + (g<<8) + (b<<16) + (intensity<<24)
		public int loopSound;			// constantly loop this sound
		public int modelindex;
		public int modelindex2;
		public int clientNum;			// 0 to (MAX_CLIENTS - 1), for players and corpses
		public int frame;
		public int solid;				// for client side prediction, trap_linkentity sets this properly
		public int impulseEvent;		// impulse events -- muzzle flashes, footsteps, etc
		public int eventParm;

		// for players
		public int powerups;			// bit flags
		public int weapon;				// determines weapon and flash model, etc
		public int legsAnim;			// mask off ANIM_TOGGLEBIT
		public int torsoAnim;			// mask off ANIM_TOGGLEBIT

		public int generic1;

		public void CopyTo ( EntityState dest ) {
			FieldInfo [] fis = typeof ( EntityState ).GetFields ();
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
				new NetField ( "pos.trTime"     , 0x10, 0x20 ),
				new NetField ( "pos.trBase[0]"  , 0x18, 0x00 ),
				new NetField ( "pos.trBase[1]"  , 0x1c, 0x00 ),
				new NetField ( "pos.trDelta[0]" , 0x24, 0x00 ),
				new NetField ( "pos.trDelta[1]" , 0x28, 0x00 ),
				new NetField ( "pos.trBase[2]"  , 0x20, 0x00 ),
				new NetField ( "apos.trBase[1]" , 0x40, 0x00 ),
				new NetField ( "pos.trDelta[2]" , 0x2c, 0x00 ),
				new NetField ( "apos.trBase[0]" , 0x3c, 0x00 ),
				new NetField ( "impulseEvent"   , 0xb4, 0x0a ),
				new NetField ( "angles2[1]"     , 0x84, 0x00 ),
				new NetField ( "eType"          , 0x04, 0x08 ),
				new NetField ( "torsoAnim"      , 0xc8, 0x08 ),
				new NetField ( "eventParm"      , 0xb8, 0x08 ),
				new NetField ( "legsAnim"       , 0xc4, 0x08 ),
				new NetField ( "groundEntityNum", 0x94, 0x0a ),
				new NetField ( "pos.trType"     , 0x0c, 0x08 ),
				new NetField ( "eFlags"         , 0x08, 0x13 ),
				new NetField ( "otherEntityNum" , 0x8c, 0x0a ),
				new NetField ( "weapon"         , 0xc0, 0x08 ),
				new NetField ( "clientNum"      , 0xa8, 0x08 ),
				new NetField ( "angles[1]"      , 0x78, 0x00 ),
				new NetField ( "pos.trDuration" , 0x14, 0x20 ),
				new NetField ( "apos.trType"    , 0x30, 0x08 ),
				new NetField ( "origin[0]"      , 0x5c, 0x00 ),
				new NetField ( "origin[1]"      , 0x60, 0x00 ),
				new NetField ( "origin[2]"      , 0x64, 0x00 ),
				new NetField ( "solid"          , 0xb0, 0x18 ),
				new NetField ( "powerups"       , 0xbc, 0x10 ),
				new NetField ( "modelindex"     , 0xa0, 0x08 ),
				new NetField ( "otherEntityNum2", 0x90, 0x0a ),
				new NetField ( "loopSound"      , 0x9c, 0x08 ),
				new NetField ( "generic1"       , 0xcc, 0x08 ),
				new NetField ( "origin2[2]"     , 0x70, 0x00 ),
				new NetField ( "origin2[0]"     , 0x68, 0x00 ),
				new NetField ( "origin2[1]"     , 0x6c, 0x00 ),
				new NetField ( "modelindex2"    , 0xa4, 0x08 ),
				new NetField ( "angles[0]"      , 0x74, 0x00 ),
				new NetField ( "time"           , 0x54, 0x20 ),
				new NetField ( "apos.trTime"    , 0x34, 0x20 ),
				new NetField ( "apos.trDuration", 0x38, 0x20 ),
				new NetField ( "apos.trBase[2]" , 0x44, 0x00 ),
				new NetField ( "apos.trDelta[0]", 0x48, 0x00 ),
				new NetField ( "apos.trDelta[1]", 0x4c, 0x00 ),
				new NetField ( "apos.trDelta[2]", 0x50, 0x00 ),
				new NetField ( "time2"          , 0x58, 0x20 ),
				new NetField ( "angles[2]"      , 0x7c, 0x00 ),
				new NetField ( "angles2[0]"     , 0x80, 0x00 ),
				new NetField ( "angles2[2]"     , 0x88, 0x00 ),
				new NetField ( "constantLight"  , 0x98, 0x20 ),
				new NetField ( "frame"          , 0xac, 0x10 )
			};
	}
}
