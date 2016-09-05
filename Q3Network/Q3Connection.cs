using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Utils;

namespace Q3Network
{
	public enum ConnectionState {
		Unitialized = 0,
		Disconnected,		// not talking to a server
		Connecting,			// sending request packets to the server
		Challenging,		// sending challenge packets to the server
		Connected,			// connection established, getting gamestate
		Primed,				// got gamestate, waiting for first frame
		Active				// game views should be displayed
	}

	public enum ClientCommandType {
		Nop = 1,
		Move,
		MoveNoDelta,
		ClientCommand,
		EOF
	}

	public enum ServerCommandType {
		Bad,
		Nop,
		GameState,
		ConfigString,
		BaseLine,
		ServerCommand,
		Download,
		Snapshot,
		EOF
	}

	public enum ConnectionFrom {
		ClientSide,
		ServerSide
	}

	public delegate void ServerCommandReceivedEvent ( object sender, string cmdStr );

	public class Q3Connection
	{
		#region Q3Connection Properties
		private ConnectionFrom connectionFrom;
		public const int GENTITYNUM_BITS = 10;		// don't need to send any more	// Move it out here!
		public const int MAX_GENTITIES   =  1 << GENTITYNUM_BITS;	// Move it out here!
		private EntityState [] entityBaselines = new EntityState [MAX_GENTITIES];	// Move it out here!
		private AutoResetEvent entityBaselinesEvent = new AutoResetEvent ( true );
		public const int PACKET_BACKUP = 32;	// Move it out here!
		public const int PACKET_MASK = PACKET_BACKUP - 1;	// Move it out here!
		private Snapshot [] snapshots = new Snapshot [PACKET_BACKUP];	// Move it out here!
		private int parseEntitiesNum;	// Move it out here!
		public const int MAX_PARSE_ENTITIES = 2048;	// Move it out here!
		private Snapshot snap = new Snapshot ();	// Move it out here!
		private bool newSnapshots = false;
		private EntityState [] parseEntities = new EntityState [MAX_PARSE_ENTITIES];	// Move it out here!
		private AutoResetEvent parseEntitiesEvent = new AutoResetEvent ( true );
		private OutPacket [] outPackets = new OutPacket [PACKET_BACKUP];	// information about each packet we have sent out	// Consider move it out here!
		private int packetDup = 1;	// Consider move it out here!
		private const int MAX_PACKET_USERCMDS = 32;		// max number of usercmd_t in a packet	// Consider move it out here!
		private const int CMD_BACKUP = 64;	// Move it out here!
		private const int CMD_MASK = CMD_BACKUP - 1;	// Move it out here!
		private UserCommand [] cmds = new UserCommand [CMD_BACKUP];	// each mesage will send several old cmds
		private int cmdNumber;			// incremented each frame, because multiple
										// frames may need to be packed into a single packet	// Move it out here!
		private int clientNum;
		private int checksumFeed;
		public const int PROTOCOL_VERSION = 68;
		public const int MAX_RELIABLE_COMMANDS = 64;
		private string [] outgoingReliableCommands = new string [MAX_RELIABLE_COMMANDS];
		private string [] incomingReliableCommands = new string [MAX_RELIABLE_COMMANDS];
		private AutoResetEvent reliableCommandsEvent = new AutoResetEvent ( true );
		private ConnectionState connState = ConnectionState.Unitialized;
		private string userInfo = @"\name\debug\rate\25000\snaps\40\model\doom\headmodel\doom\team_model\james\team_headmodel\*james\color1\4\color2\5\handicap\100\sex\male\cl_anonymous\0\cg_predictItems\1\teamtask\0";
		// Checksums for pure client
		private string cp = @"cp 574634200 1412799621 1412799621 @ 1412799621 -1335195201 1293791034 -645996117 2045155859 -982201419 -1110025977 ";

		// these are the only configstrings that the system reserves, all the
		// other ones are strictly for servergame to clientgame communication
		public const int CS_SERVERINFO = 0;		// an info string with all the serverinfo cvars
		public const int CS_SYSTEMINFO = 1;		// an info string for server system to client system configuration (timescale, etc)
		private Dictionary <int, string> gameState = new Dictionary <int,string> ();
		private bool demoPlaying = false;
		private FileStream demoFileStream = null;
		private Q3DemoStream demoStream = null;
		private bool firstDemoFrameSkipped = false;
		private Q3HuffmanStream q3HuffDemoReadStream = null;

		private int incomingSequence;
		private int outgoingSequence;
		private ushort qport;
		private int serverId;
		private int incomingCommandSequence;
		private int messageAcknowledge;
		private int reliableSequence;
		private int reliableAcknowledge;
		private int sequenceNumber;
		private int challenge;

		private Socket sock;
		private Q3NetworkStream q3NetStream;
		private Q3CryptStream   q3CryptStream;
		private Q3HuffmanStream q3HuffCStream;
		private Q3HuffmanStream q3HuffDStream;

		public event ServerCommandReceivedEvent ServerCommandReceived;

		public ConnectionFrom ConnectionFrom { get { 
			return	connectionFrom;
		} }

		public byte MovementForward { get; set; }
		public byte MovementRight { get; set; }
		public byte MovementUp { get; set; }
		public WeaponType SelectedWeapon { get; set; }
		private int viewAngleX;	// [-32768 -- 32767] = [+180 -- -179]
		private int viewAngleY;	// [-16384 -- 16383] = [+90 -- -89]
		public int ViewAngleX {
			get {
				return	viewAngleX;
			}
			set {
				viewAngleX = value < -32768 ? 65535 + value : ( value > 32767 ? 32767 - 65535 : value );
			}
		}

		public int ViewAngleY {
			get {
				return	viewAngleY;
			}
			set {
				viewAngleY = value < -16384 ? -16384 : ( value > 16383 ? 16383 : value );
			}
		}
		private int buttons;

		public void ButtonDown ( Button button ) {
			this.buttons |= ( int ) button;
		}

		public void ButtonUp ( Button button ) {
			this.buttons &= ~( int ) button;
		}

		public int IncomingSequence {
			get { return	incomingSequence; }
			set { incomingSequence = value; }
		}

		public int OutgoingSequence {
			get { return	outgoingSequence; }
			set { outgoingSequence = value; }
		}

		public string [] IncomingReliableCommands {
			get { return	incomingReliableCommands; }
			set { incomingReliableCommands = value; }
		}

		public string [] OutgoingReliableCommands {
			get { return	outgoingReliableCommands; }
			set { outgoingReliableCommands = value; }
		}

		public ConnectionState ConnectionState {
			get { return	connState; }
		}

		public ushort QPort {
			get { return	qport; }
			set { qport = value; }
		}
		
		public int ServerId {
			get { return	serverId; }
			set { serverId = value; }
		}

		public int IncomingCommandSequence {
			get { return	incomingCommandSequence; }
			set { incomingCommandSequence = value; }
		}

		public int ReliableSequence {
			get { return	reliableSequence; }
			set { reliableSequence = value; }
		}

		public int ReliableAcknowledge {
			get { return	reliableAcknowledge; }
			set { reliableAcknowledge = value; }
		}

		public int SequenceNumber {
			get { return	sequenceNumber; }
			set { sequenceNumber = value; }
		}

		public int Challenge {
			get { return	challenge; }
			set { challenge = value; }
		}

		public EntityState [] ParseEntities {
			get {
				parseEntitiesEvent.WaitOne ();
				EntityState [] res = new EntityState [parseEntities.Length];
				parseEntities.CopyTo ( res, 0 );
				parseEntitiesEvent.Set ();

				return	res;
			}
		}

		public EntityState [] EntityBaselines {
			get {
				entityBaselinesEvent.WaitOne ();
				EntityState [] res = new EntityState [entityBaselines.Length];
				entityBaselines.CopyTo ( res, 0 );
				entityBaselinesEvent.Set ();

				return	res;
			}
		}

		public Snapshot Snap {
			get {
				// FIXIT: add AutoResetEvent
				return	snap;
			}
		}

		public bool DemoPlaying { get { return	demoPlaying; } }

		public Q3NetworkStream Q3NetStream { get { return	this.q3NetStream; } }
		public Q3CryptStream   Q3CryptStream { get { return	this.q3CryptStream; } }
		public Q3HuffmanStream Q3HuffCStream { get { return	this.q3HuffCStream; } }
		public Q3HuffmanStream Q3HuffDStream { get { return	this.q3HuffDStream; } }
		#endregion Q3Connection Properties

		public Q3Connection ( ConnectionFrom side ) {
			for ( int i = 0 ; i < this.snapshots.Length ; i++ )
				this.snapshots [i] = new Snapshot ();

			for ( int i = 0 ; i < this.parseEntities.Length ; i++ )
				this.parseEntities [i] = new EntityState ();

			for ( int i = 0 ; i < this.entityBaselines.Length ; i++ )
				this.entityBaselines [i] = new EntityState ();

			for ( int i = 0 ; i < this.outPackets.Length ; i++ )
				this.outPackets [i] = new OutPacket ();

			for ( int i = 0 ; i < this.cmds.Length ; i++ )
				this.cmds [i] = new UserCommand ();

			this.connectionFrom = side;
		}

		#region Methods
		public void Connect ( IPAddress ip, int port ) {
			Random rnd = new Random ();
			
			qport = ( ushort ) rnd.Next ( 65536 );	// Generate port from 0 to 65535 (inclusively)
			// STATIC_DEBUG
			//qport = 0x2233; // ( short ) rnd.Next ( 65536 );	// Generate port from 0 to 65535 (inclusively)
			incomingSequence = 0;
			outgoingSequence = 1;

			IPEndPoint ep = new IPEndPoint ( ip, port );
			sock = new Socket ( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
			try { sock.Connect ( ep ); } catch { throw; }
			q3NetStream = new Q3NetworkStream ( sock, this, FileAccess.ReadWrite );
			q3CryptStream = new Q3CryptStream ( q3NetStream, this, FileAccess.ReadWrite );
			q3HuffCStream = new Q3HuffmanStream ( q3CryptStream, CompressionMode.Compress );
			q3HuffCStream.InitWithQ3Data ();
			q3HuffCStream.TreeIsFrozen = true;

			q3HuffDStream = new Q3HuffmanStream ( q3CryptStream, CompressionMode.Decompress );
			q3HuffDStream.InitWithQ3Data ();
			q3HuffDStream.TreeIsFrozen = true;

			if ( DoConnectionlessHandshake () ) {
				// enter main loop

				SendUserInfo ();	// FIXIT: move this out of here

				while ( true ) {
					ReadPacket ( q3HuffDStream );
					SendCmd ();
					System.Threading.Thread.Sleep ( 10 );
				}
			} else
				throw new Exception ( string.Format ( "Could not connect to {0}", ep ) );
		}

		private void SendCmd () {
			if ( this.connState < ConnectionState.Connected ) {
				this.newSnapshots = false;

				return;
			}

			this.CreateNewCommands ();
			this.WritePacket ();
		}

		double a = 0;
		int sgnY = 1;
		int sgnX = 1;

		private void CreateNewCommands () {
			if ( this.connState < ConnectionState.Primed )
				return;

			this.cmdNumber++;
			int cmdNum = this.cmdNumber & CMD_MASK;
			UserCommand cmd = new UserCommand ();
			cmd.weapon = this.SelectedWeapon;
			cmd.forwardmove = this.MovementForward;
			cmd.rightmove = this.MovementRight;
			cmd.upmove = this.MovementUp;
			cmd.serverTime = this.snap.serverTime;
			cmd.buttons = this.buttons;
			cmd.angles [0] = ViewAngleY;
			cmd.angles [1] = ViewAngleX;

			//ViewAngleX = ( ViewAngleX - 200 ) & 0xffff;
			//ViewAngleY = ( ( int ) ( Math.Sin ( a ) * 0x7fff ) / 2 ) & 0xffff;
			//a += 0.01;

			/*if ( Math.Abs ( ViewAngleX ) >= 0x7fff )
				sgnX = -sgnX;

			ViewAngleX += 200 * sgnX;

			if ( Math.Abs ( ViewAngleY ) >= 0x3fff )
				sgnY = -sgnY;

			ViewAngleY += 200 * sgnY;*/


			this.cmds [cmdNum] = cmd;
		}

		/*
			During normal gameplay, a client packet will contain something like:
			
			4	sequence number
			2	qport
			4	serverid
			4	acknowledged sequence number
			4	serverCommandSequence
			<optional reliable commands>
			1	ClientCommands.Move or ClientCommands.MoveNoDelta
			1	command count
			<count * usercmds>
		 */
		private void WritePacket () {
			// 'sequence number' and 'qport' fields will be written by underlying
			// Q3NetworkStream to provide these values without compressing and cyphering
			q3HuffCStream.WriteInt32 ( this.serverId );					// serverid
			q3HuffCStream.WriteInt32 ( this.incomingSequence );	// acknowledged sequence number
			q3HuffCStream.WriteInt32 ( this.incomingCommandSequence );	// serverCommandSequence

			reliableCommandsEvent.WaitOne ();
			//<optional reliable commands>
			for ( int i = reliableAcknowledge + 1 ; i <= reliableSequence ; i++ ) {
				q3HuffCStream.WriteByte   ( ( byte ) ClientCommandType.ClientCommand );
				q3HuffCStream.WriteInt32  ( i );
				q3HuffCStream.WriteString ( outgoingReliableCommands [i & ( MAX_RELIABLE_COMMANDS - 1 )] );
			}
			reliableCommandsEvent.Set ();

			// we want to send all the usercmds that were generated in the last
			// few packet, so even if a couple packets are dropped in a row,
			// all the cmds will make it to the server
			if ( this.packetDup < 0 )
				this.packetDup = 0;
			else if ( this.packetDup > 5 )
				this.packetDup = 5;

			int oldPacketNum = ( this.outgoingSequence - 1 - packetDup ) & PACKET_MASK;
			int count = this.cmdNumber - this.outPackets [oldPacketNum].cmdNumber;

			if ( count > MAX_PACKET_USERCMDS )
				count = MAX_PACKET_USERCMDS;

			UserCommand oldcmd = new UserCommand ();
			
			if ( count >= 1 && this.newSnapshots ) {
				// begin a client move command
				if ( /*cl_nodelta->integer ||*/ !snap.valid /*|| clc.demowaiting*/
					|| this.incomingSequence != this.snap.messageNum )
					q3HuffCStream.WriteByte ( ( byte ) ClientCommandType.MoveNoDelta );
				else
					q3HuffCStream.WriteByte ( ( byte ) ClientCommandType.Move );

				// write the command count
				q3HuffCStream.WriteByte ( ( byte ) count );

				// use the checksum feed in the key
				int key = this.checksumFeed;
				// also use the message acknowledge
				key ^= this.incomingSequence;
				// also use the last acknowledged server command in the key
				key ^= this.HashKey ( this.incomingReliableCommands [this.incomingCommandSequence & ( MAX_RELIABLE_COMMANDS - 1 )], 32 );

				// write all the commands, including the predicted command
				for ( int i = 0 ; i < count ; i++ ) {
					int j = ( this.cmdNumber - count + i + 1 ) & CMD_MASK;
					WriteDeltaUsercmdKey ( key, oldcmd, ref this.cmds [j] );
					oldcmd = this.cmds [j];
				}

				this.newSnapshots = false;
			}

			int packetNum = this.outgoingSequence & PACKET_MASK;
			this.outPackets[ packetNum ].realtime = DateTime.Now.Millisecond;
			this.outPackets[ packetNum ].serverTime = oldcmd.serverTime;
			this.outPackets[ packetNum ].cmdNumber = this.cmdNumber;
			//clc.lastPacketSentTime = cls.realtime;

			q3HuffCStream.WriteByte ( ( byte ) ClientCommandType.EOF );
			q3HuffCStream.Flush ();
		}

		private void WriteDeltaUsercmdKey ( int key, UserCommand from, ref UserCommand to ) {
			if ( to.serverTime - from.serverTime < 256 ) {
				q3HuffCStream.WriteBits ( 1, 1 );
				q3HuffCStream.WriteByte ( ( byte ) ( to.serverTime - from.serverTime ) );
			} else {
				q3HuffCStream.WriteBits  ( 0, 1 );
				q3HuffCStream.WriteInt32 ( to.serverTime );
			}
			if ( from.angles [0] == to.angles [0] &&
				 from.angles [1] == to.angles [1] &&
				 from.angles [2] == to.angles [2] &&
				 from.forwardmove == to.forwardmove &&
				 from.rightmove   == to.rightmove   &&
				 from.upmove      == to.upmove      &&
				 from.buttons     == to.buttons     &&
				 from.weapon      == to.weapon ) {
					q3HuffCStream.WriteBits ( 0, 1 );				// no change
					
					return;
			}

			key ^= to.serverTime;
			q3HuffCStream.WriteBits ( 1, 1 );
			WriteDeltaKey ( key, from.angles [0], to.angles [0], 16 );
			WriteDeltaKey ( key, from.angles [1], to.angles [1], 16 );
			WriteDeltaKey ( key, from.angles [2], to.angles [2], 16 );
			WriteDeltaKey ( key, from.forwardmove, to.forwardmove, 8 );
			WriteDeltaKey ( key, from.rightmove  , to.rightmove  , 8 );
			WriteDeltaKey ( key, from.upmove     , to.upmove     , 8 );
			WriteDeltaKey ( key, from.buttons    , to.buttons    , 16 );
			WriteDeltaKey ( key, ( byte ) from.weapon, ( byte ) to.weapon, 8 );
		}

		private void WriteDeltaKey ( int key, int oldV, int newV, int bits ) {
			if ( oldV == newV ) {
				q3HuffCStream.WriteBits ( 0, 1 );

				return;
			}

			q3HuffCStream.WriteBits ( 1, 1 );
			q3HuffCStream.WriteBits ( newV ^ key, bits );
		}

		private int HashKey ( string str, int maxlen ) {
			int hash = 0, i;

			if ( str == null )
				return	0;

			for ( i = 0 ; i < maxlen && i < str.Length ; i++ )
				hash += str [i] * ( 119 + i );
			
			hash = ( hash ^ ( hash >> 10 ) ^ ( hash >> 20 ) );

			return hash;
		}

		private void ReadPacket ( Q3HuffmanStream stream ) {
			List <string> cmdLog = new List <string> ();
			PacketKind pktKind = stream.BeginRead ();
			this.reliableAcknowledge = stream.ReadInt32 ();
			ServerCommandType cmd;

			while ( ServerCommandType.EOF != ( cmd = ( ServerCommandType ) stream.ReadByte () ) ) {
				switch ( cmd ) {
				case ServerCommandType.Nop:
					cmdLog.Add ( "Nop" );
					break;
				case ServerCommandType.ServerCommand:
					this.ParseCommandString ( stream );
					cmdLog.Add ( "ServerCommand" );
					break;
				case ServerCommandType.GameState:
					this.ParseGamestate ( stream );
					cmdLog.Add ( "GameState" );
					break;
				case ServerCommandType.Snapshot:
					this.ParseSnapshot ( stream );
					cmdLog.Add ( "Snapshot" );
					break;
				case ServerCommandType.Download:
					// We never download ;)
					return;
				default:
					// Unknown command
					return;
				}
			}

			stream.EndRead ();
		}

		private void ParseCommandString ( Q3HuffmanStream stream ) {
			this.incomingCommandSequence = stream.ReadInt32 ();
			string cmdStr = stream.ReadString ();
			int index = this.incomingCommandSequence & ( MAX_RELIABLE_COMMANDS - 1 );

			this.incomingReliableCommands [index] = cmdStr;

			if ( cmdStr.StartsWith ( "cs " ) ) {
				string [] parts = cmdStr.Split ( ' ' );

				int csId;

				if ( parts.Length >= 2 ) {
					try {
						csId = Convert.ToInt32 ( parts [1] );
					} catch {
						csId = 0;
					}

					this.gameState [csId] = string.Join ( " ", parts, 2, parts.Length - 2 );

					if ( csId == 1 )
						SystemInfoChanged ();
				}
			}

			if ( this.ServerCommandReceived != null )
				this.ServerCommandReceived ( this, cmdStr );
		}

		private void ParseGamestate ( Q3HuffmanStream stream ) {
			this.gameState.Clear ();
			this.incomingCommandSequence = stream.ReadInt32 ();
			EntityState nullstate = new EntityState ();

			ServerCommandType cmd;

			entityBaselinesEvent.WaitOne ();

			while ( ServerCommandType.EOF != ( cmd = ( ServerCommandType ) stream.ReadByte () ) ) {
				switch ( cmd ) {
				case ServerCommandType.ConfigString:
					int i = stream.ReadInt16 ();
					gameState [i] = stream.ReadString ();
					break;
				case ServerCommandType.BaseLine:
					int newnum = stream.ReadBits ( 10 );

					ReadDeltaEntity ( stream, nullstate, ref entityBaselines [newnum], newnum );
					break;
				default:
					// Unknown command
					break;
				}
			}

			entityBaselinesEvent.Set ();

			this.clientNum = stream.ReadInt32 ();
			this.checksumFeed = stream.ReadInt32 ();

			SystemInfoChanged ();
			this.connState = ConnectionState.Primed;

			//SendPureChecksums ();
		}

		private void ReadDeltaEntity ( Q3HuffmanStream stream, EntityState from, ref EntityState to, int number ) {
			// check for a remove
			if ( stream.ReadBits ( 1 ) == 1 ) {
				new EntityState ().CopyTo ( to );
				to.number = MAX_GENTITIES - 1;

				return;
			}

			// check for no delta
			if ( stream.ReadBits ( 1 ) == 0 ) {
				from.CopyTo ( to );
				to.number = number;

				return;
			}

			int lc = stream.ReadByte ();
			to.number = number;
			int i;
			NetField field;
			int trunc;

			for ( i = 0, field = EntityState.fields [i] ; i < lc && i < EntityState.fields.Length ; i++ ) {
				field = EntityState.fields [i];

				if ( stream.ReadBits ( 1 ) == 0 ) {
					// no change
					KeyValueCoder.TrySetFieldValue ( to, field.name, 
						KeyValueCoder.TryGetFieldValue ( from, field.name ) );
				} else {
					if ( field.bits == 0 ) {
						// float
						if ( stream.ReadBits ( 1 ) == 0 ) {
							KeyValueCoder.TrySetFieldValue ( to, field.name, 0 );
						} else {
							if ( stream.ReadBits ( 1 ) == 0 ) {
								// integral float
								trunc = stream.ReadBits ( NetField.FLOAT_INT_BITS );
								// bias to allow equal parts positive and negative
								trunc -= NetField.FLOAT_INT_BIAS;
								KeyValueCoder.TrySetFieldValue ( to, field.name, trunc );
							} else {
								// full floating point value
								// FIXIT: wrong conversion from 32 bits to floating point value
								KeyValueCoder.TrySetFieldValue ( to, field.name, stream.ReadInt32 () );
							}
						}
					} else {
						if ( stream.ReadBits ( 1 ) == 0 ) {
							KeyValueCoder.TrySetFieldValue ( to, field.name, 0 );
						} else {
							// integer
							KeyValueCoder.TrySetFieldValue ( to, field.name, stream.ReadBits ( ( int ) field.bits ) );
						}
					}
				}
			}

			for ( i = lc ; i < EntityState.fields.Length ; i++ ) {
				field = EntityState.fields [i];

				// no change
				KeyValueCoder.TrySetFieldValue ( to, field.name, 
					KeyValueCoder.TryGetFieldValue ( from, field.name ) );
			}
		}

		private void SystemInfoChanged () {
			string value;

			if ( null != ( value = InfoValueForKey ( "sv_serverid" ) ) )
				this.serverId = Convert.ToInt32 ( value );
		}

		private string InfoValueForKey ( string key ) {
			string cfgStr = this.gameState [CS_SYSTEMINFO];
			int st;
			int end;

			if ( -1 == ( st = cfgStr.IndexOf ( key ) ) )
				return	null;

			if ( ( st + key.Length == cfgStr.Length ) || -1 == ( end = cfgStr.IndexOf ( @"\", st + key.Length + 1 ) ) )
				end = cfgStr.Length;

			return	cfgStr.Substring ( st + key.Length + 1, end - key.Length - st - 1 );
		}

		private void ParseSnapshot ( Q3HuffmanStream stream ) {
			Snapshot old;
			Snapshot newSnap = new Snapshot ();
			int deltaNum;

			newSnap.serverCommandNum = this.incomingCommandSequence;
			newSnap.serverTime = stream.ReadInt32 ();
			newSnap.messageNum = this.incomingSequence;

			if ( 0 == ( deltaNum = stream.ReadByte () ) )
				newSnap.deltaNum = -1;
			else
				newSnap.deltaNum = newSnap.messageNum - deltaNum;

			newSnap.snapFlags = stream.ReadByte ();

			// If the frame is delta compressed from data that we
			// no longer have available, we must suck up the rest of
			// the frame, but not use it, then ask for a non-compressed
			// message 
			if ( newSnap.deltaNum <= 0 ) {
				newSnap.valid = true;		// uncompressed frame
				old = null;
				//clc.demowaiting = false;	// we can start recording now
			} else {
				old = this.snapshots [newSnap.deltaNum & PACKET_MASK];

				if ( !old.valid ) {
					// should never happen
					// "Delta from invalid frame (not supposed to happen!)"
				} else if ( old.messageNum != newSnap.deltaNum ) {
					// The frame that the server did the delta from
					// is too old, so we can't reconstruct it properly.
					// "Delta frame too old."
				} else if ( parseEntitiesNum - old.parseEntitiesNum > MAX_PARSE_ENTITIES - 128 ) {
					// "Delta parseEntitiesNum too old."
				} else {
					newSnap.valid = true;	// valid delta parse
				}
			}

			// read areamask
			int len = stream.ReadByte ();
			stream.Read ( newSnap.areamask, 0, len );

			// read playerinfo
			if ( old != null )
				ReadDeltaPlayerstate ( stream, old.ps, ref newSnap.ps );
			else
				ReadDeltaPlayerstate ( stream, null, ref newSnap.ps );

			// read packet entities
			ParsePacketEntities ( stream, old, newSnap );

			// if not valid, dump the entire thing now that it has
			// been properly read
			if ( !newSnap.valid )
				return;

			// clear the valid flags of any snapshots between the last
			// received and this one, so if there was a dropped packet
			// it won't look like something valid to delta from next
			// time we wrap around in the buffer
			int oldMessageNum = this.snap.messageNum + 1;

			if ( newSnap.messageNum - oldMessageNum >= PACKET_BACKUP )
				oldMessageNum = newSnap.messageNum - ( PACKET_BACKUP - 1 );
			
			for ( ; oldMessageNum < newSnap.messageNum ; oldMessageNum++ )
				this.snapshots [oldMessageNum & PACKET_MASK].valid = false;

			// copy to the current good spot
			this.snap = newSnap;
			this.snap.ping = 999;

			// calculate ping time
			for ( int i = 0 ; i < PACKET_BACKUP ; i++ ) {
				int packetNum = ( this.outgoingSequence - 1 - i ) & PACKET_MASK;

				if ( this.snap.ps.commandTime >= this.outPackets [packetNum].serverTime ) {
					this.snap.ping = ( int ) ( DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond ) - this.outPackets[packetNum].realtime;
					break;
				}
			}

			// save the frame off in the backup array for later delta comparisons
			this.snapshots [this.snap.messageNum & PACKET_MASK] = this.snap;
			this.newSnapshots = true;
		}

		private void ReadDeltaPlayerstate ( Q3HuffmanStream stream, PlayerState from, ref PlayerState to ) {
			int i;

			if ( from == null )
				from = new PlayerState ();

			from.CopyTo ( to );

			int lc = stream.ReadByte ();
			NetField field;
			int trunc;

			for ( i = 0, field = PlayerState.fields [i] ; i < lc ; i++ ) {
				field = PlayerState.fields [i];

				if ( stream.ReadBits ( 1 ) == 0 ) {
					// no change
					KeyValueCoder.TrySetFieldValue ( to, field.name, 
						KeyValueCoder.TryGetFieldValue ( from, field.name ) );
				} else {
					if ( field.bits == 0 ) {
						// float
						if ( stream.ReadBits ( 1 ) == 0 ) {
							// integral float
							trunc = stream.ReadBits ( NetField.FLOAT_INT_BITS );
							// bias to allow equal parts positive and negative
							trunc -= NetField.FLOAT_INT_BIAS;
							KeyValueCoder.TrySetFieldValue ( to, field.name, trunc );
						} else {
							// full floating point value
							// FIXIT: wrong conversion from 32 bits to floating point value
							KeyValueCoder.TrySetFieldValue ( to, field.name, stream.ReadInt32 () );
						}
					} else {
						// integer
						KeyValueCoder.TrySetFieldValue ( to, field.name, stream.ReadBits ( ( int ) field.bits ) );
					}
				}
			}

			for ( i = lc, field = PlayerState.fields [lc] ; i < PlayerState.fields.Length ; i++ ) {
				field = PlayerState.fields [i];

				// no change
				KeyValueCoder.TrySetFieldValue ( to, field.name, 
					KeyValueCoder.TryGetFieldValue ( from, field.name ) );
			}

			short bits;

			// read the arrays
			if ( 0 != stream.ReadBits ( 1 ) ) {
				// parse stats
				if ( 0 != stream.ReadBits ( 1 ) ) {
					bits = stream.ReadInt16 ();

					for ( i = 0 ; i < 16 ; i++ ) {
						if ( 0 != ( bits & ( 1 << i ) ) ) {
							to.stats [i] = stream.ReadInt16 ();
						}
					}
				}

				// parse persistant stats
				if ( 0 != stream.ReadBits ( 1 ) ) {
					bits = stream.ReadInt16 ();

					for ( i = 0 ; i < 16 ; i++ ) {
						if ( 0 != ( bits & ( 1 << i ) ) ) {
							to.persistant [i] = stream.ReadInt16 ();
						}
					}
				}

				// parse ammo
				if ( 0 != stream.ReadBits ( 1 ) ) {
					bits = stream.ReadInt16 ();

					for ( i = 0 ; i < 16 ; i++ ) {
						if ( 0 != ( bits & ( 1 << i ) ) ) {
							to.ammo [i] = stream.ReadInt16 ();
						}
					}
				}

				// parse powerups
				if ( 0 != stream.ReadBits ( 1 ) ) {
					bits = stream.ReadInt16 ();

					for ( i = 0 ; i < 16 ; i++ ) {
						if ( 0 != ( bits & ( 1 << i ) ) ) {
							to.powerups [i] = stream.ReadInt32 ();
						}
					}
				}
			}
		}

		private void ParsePacketEntities ( Q3HuffmanStream stream, Snapshot oldFrame, Snapshot newFrame ) {
			EntityState oldState = null;
			int oldIndex = 0;
			int oldNum;

			parseEntitiesEvent.WaitOne ();
			entityBaselinesEvent.WaitOne ();
			newFrame.parseEntitiesNum = this.parseEntitiesNum;
			newFrame.numEntities = 0;

			if ( null == oldFrame)
				oldNum = 99999;
			else {
				if ( oldIndex >= oldFrame.numEntities ) {
					oldNum = 99999;
				} else {
					oldState = this.parseEntities [( oldFrame.parseEntitiesNum + oldIndex ) & ( MAX_PARSE_ENTITIES - 1 )];
					oldNum = oldState.number;
				}
			}

			int newNum;

			while ( ( MAX_GENTITIES - 1 ) != ( newNum = stream.ReadBits ( GENTITYNUM_BITS ) ) ) {
				while ( oldNum < newNum ) {
					// one or more entities from the old packet are unchanged
					DeltaEntity ( stream, newFrame, oldNum, oldState, true );
					
					oldIndex++;

					if ( oldIndex >= oldFrame.numEntities )
						oldNum = 99999;
					else {
						oldState = this.parseEntities[( oldFrame.parseEntitiesNum + oldIndex ) & ( MAX_PARSE_ENTITIES - 1 )];
						oldNum = oldState.number;
					}
				}

				if ( oldNum == newNum ) {
					// delta from previous 
					DeltaEntity ( stream, newFrame, newNum, oldState, false );

					oldIndex++;

					if ( oldIndex >= oldFrame.numEntities )
						oldNum = 99999;
					else {
						oldState = this.parseEntities[( oldFrame.parseEntitiesNum + oldIndex ) & ( MAX_PARSE_ENTITIES - 1 )];
						oldNum = oldState.number;
					}

					continue;
				}

				if ( oldNum > newNum ) {
					// delta from baseline
					DeltaEntity ( stream, newFrame, newNum, this.entityBaselines [newNum], false );

					continue;
				}
			}

			// any remaining entities in the old frame are copied over
			while ( oldNum != 99999 ) {
				// one or more entities from the old packet are unchanged
				DeltaEntity ( stream, newFrame, oldNum, oldState, true );
				
				oldIndex++;

				if ( oldIndex >= oldFrame.numEntities )
					oldNum = 99999;
				else {
					oldState = this.parseEntities[( oldFrame.parseEntitiesNum + oldIndex ) & ( MAX_PARSE_ENTITIES - 1 )];
					oldNum = oldState.number;
				}
			}

			entityBaselinesEvent.Set ();
			parseEntitiesEvent.Set ();
		}

		private void DeltaEntity ( Q3HuffmanStream stream, Snapshot frame, int newNum, EntityState old, bool unchanged ) {
			EntityState	state;

			// save the parsed entity state into the big circular buffer so
			// it can be used as the source for a later delta
			state = this.parseEntities [this.parseEntitiesNum & ( MAX_PARSE_ENTITIES - 1 )];

			if ( unchanged )
				old.CopyTo ( state );
			else
				this.ReadDeltaEntity ( stream, old, ref state, newNum );

			if ( state.number == ( MAX_GENTITIES - 1 ) )
				return;		// entity was delta removed
			
			this.parseEntitiesNum++;
			frame.numEntities++;
		}

		// FIXIT: This must be moved to overlying class, Q3Client
		public void SendUserInfo () {
			AddReliableCommand ( "userinfo \"" + this.userInfo + '"' );
		}

		public void SendPureChecksums () {
			AddReliableCommand ( this.cp );
		}

		public void AddReliableCommand ( string cmd ) {
			reliableCommandsEvent.WaitOne ();
			if ( reliableSequence - reliableAcknowledge > MAX_RELIABLE_COMMANDS )
				throw new OverflowException ( "reliableCommands overflow" );	// FIXIT: make it non-overflowable unlike original client

			outgoingReliableCommands [++reliableSequence & ( MAX_RELIABLE_COMMANDS - 1 )] = cmd;
			reliableCommandsEvent.Set ();
		}

		public void PlayDemo ( string filename ) {
			if ( File.Exists ( filename ) ) {
				this.connState = ConnectionState.Connected;
				this.demoPlaying = true;

				this.demoFileStream = File.OpenRead ( filename );
				this.demoStream = new Q3DemoStream ( this.demoFileStream, this );
				this.q3HuffDemoReadStream = new Q3HuffmanStream ( this.demoStream, CompressionMode.Decompress );
				q3HuffDemoReadStream.InitWithQ3Data ();
				q3HuffDemoReadStream.TreeIsFrozen = true;

				try {
					while ( this.connState >= ConnectionState.Connected && this.connState < ConnectionState.Primed )
						ReadPacket ( this.q3HuffDemoReadStream );

					this.firstDemoFrameSkipped = false;

					while ( true )
						ReadPacket ( this.q3HuffDemoReadStream );
				} catch ( Exception ex ) {
					// Demo end
				}
			} else
				throw new FileNotFoundException ( "Demo file not found", filename );
		}
		#endregion Methods

		#region Connectionless Handshake
		private bool DoConnectionlessHandshake () {
			string command;
			byte [] data;

			this.connState = ConnectionState.Connecting;
			WriteConnectionlessPacket ( "getchallenge", null, 0, 0 );

			if ( ReadConnectionlessPacket ( out command, out data ) ) {
				if ( command == "challengeResponse" ) {
					this.challenge = Convert.ToInt32 ( Encoding.Default.GetString ( data ) );

					MemoryStream ms = new MemoryStream ();
					ms.Position = 2;
					Q3HuffmanStream huff = new Q3HuffmanStream ( ms, System.IO.Compression.CompressionMode.Compress );

					string connStr = string.Format ( @"\challenge\{0}\qport\{1}\protocol\{2}{3}",
													  this.challenge, this.qport, PROTOCOL_VERSION, userInfo );
					huff.WriteString ( connStr );
					huff.Flush ();
					ms.Position = 0;
					ms.Write ( ExBitConverter.GetBytes ( ( short ) connStr.Length, false ), 0, 2 );

					this.connState = ConnectionState.Challenging;
					WriteConnectionlessPacket ( "connect ", ms.GetBuffer (), 0, ( int ) ms.Length );

					if ( ReadConnectionlessPacket ( out command, out data ) )
						if ( command == "connectResponse" ) {
							this.connState = ConnectionState.Connected;
							return	true;
						}
				}
			}

			return	false;
		}

		private void WriteConnectionlessPacket ( string command, byte [] data, int offset, int count ) {
			q3NetStream.BeginWritePacket ( PacketKind.Connectionless );
			q3NetStream.Write ( Encoding.Default.GetBytes ( command ), 0, command.Length );

			if ( data != null )
				q3NetStream.Write ( data, offset, count );

			q3NetStream.EndWritePacket ();
		}

		private bool ReadConnectionlessPacket ( out string command, out byte [] data ) {
			byte [] buffer = new byte [256];
			PacketKind packetKind = q3NetStream.BeginReadPacket ();
			int bytesRead = q3NetStream.Read ( buffer, 0, buffer.Length );
			bool res = false;

			if ( packetKind != PacketKind.Connectionless ) {
				command = null;
				data = null;

				return	false;
			}

			string data_cmd = Encoding.ASCII.GetString ( buffer, 0, 17 );

			if ( data_cmd.Contains ( "challengeResponse" ) ) {
				command = "challengeResponse";
				res = true;
			} else if ( data_cmd.Contains ( "connectResponse" ) ) {
				command = "connectResponse";
				res = true;
			} else
				command = null;

			if ( res ) {
				int data_len = bytesRead - command.Length;

				if ( data_len != 0 ) {
					data = new byte [bytesRead - command.Length];
					Array.Copy ( buffer, command.Length, data, 0, bytesRead - command.Length );
				} else
					data = null;
			} else
				data = null;

			return	res;
		}
		#endregion Connectionless Handshake
	}
}
