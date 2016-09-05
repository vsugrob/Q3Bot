using System;
using System.IO;
using System.Net.Sockets;
using Utils;

namespace Q3Network
{
	public class Q3NetworkStream : Q3DatagramStream
	{
		#region Q3NetworkStream Constants
		private const int MAX_PACKETLEN = 1400;
		private const int FRAGMENT_SIZE = MAX_PACKETLEN - 100;
		private const int FRAGMENT_BIT  = 1 << 31;
		private const int FRAGMENT_BUFFER_INIT_SIZE = 0x4000;	// Large enough to hold all fragments if packet fragmented
		private const int WRITE_BUFFER_INIT_SIZE = 0x0600;
		private const int READ_BUFFER_INIT_SIZE = FRAGMENT_BUFFER_INIT_SIZE;
		#endregion Q3NetworkStream Constants

		#region Q3NetworkStream Properties
		private byte [] packetBuffer;
		private int fragmentReadSequence;
		private bool readingFragmented;
		#endregion Q3NetworkStream Properties

		#region Q3NetworkStream Constructors
		public Q3NetworkStream ( Socket socket, Q3Connection connection, FileAccess access )
		{
			#region Check Arguments
			if ( socket == null ) throw new ArgumentNullException ( "socket" );
			if ( connection == null ) throw new ArgumentNullException ( "connection" );
			#endregion Check Arguments

			this.underlying = socket;
			this.connection = connection;
			this.access = access;

			Init ();
		}

		public void Init () {
			if ( CanRead ) {
				packetBuffer = new byte [MAX_PACKETLEN];
				msReadBuffer = new MemoryStream ( READ_BUFFER_INIT_SIZE );
			}

			if ( CanWrite )
				msWriteBuffer = new MemoryStream ( WRITE_BUFFER_INIT_SIZE );
		}
		#endregion Q3NetworkStream Constructors

		#region Q3NetworkStream Properties
		public override bool CanTimeout { get { return	true; } }
		public override int ReadTimeout {
			get { return	( underlying as Socket ).ReceiveTimeout; }
			set { ( underlying as Socket ).ReceiveTimeout = value; }
		}

		public override int WriteTimeout {
			get { return	( underlying as Socket ).SendTimeout; }
			set { ( underlying as Socket ).SendTimeout = value; }
		}
		#endregion Q3NetworkStream Properties

		#region Q3NetworkStream Methods
		public override void Close () {
			( underlying as Socket ).Close ();
		}

		public override void Flush () {
			if ( msWriteBuffer.Length > MAX_PACKETLEN ) {
				// send fragmented
				MemoryStream msFragmentedBuffer = new MemoryStream ( packetBuffer, true );

				int fragmentStart = 4;
				int fragmentLength;

				do {
					fragmentLength = fragmentStart + FRAGMENT_SIZE <= ( int ) msWriteBuffer.Length ?
										FRAGMENT_SIZE : ( int ) msWriteBuffer.Length - fragmentStart;

					msFragmentedBuffer.Position = 0;
					msFragmentedBuffer.Write ( ExBitConverter.GetBytes ( connection.OutgoingSequence | FRAGMENT_BIT, true ), 0, 4 );
					msFragmentedBuffer.Write ( ExBitConverter.GetBytes ( fragmentStart , true ), 0, 2 );
					msFragmentedBuffer.Write ( ExBitConverter.GetBytes ( fragmentLength, true ), 0, 2 );
					msFragmentedBuffer.Write ( msWriteBuffer.GetBuffer (), fragmentStart, fragmentLength );
					fragmentStart += fragmentLength;

					( underlying as Socket ).Send ( msFragmentedBuffer.GetBuffer (), ( int ) msFragmentedBuffer.Position, SocketFlags.None );
				} while ( fragmentLength == FRAGMENT_SIZE );
			} else
				( underlying as Socket ).Send ( msWriteBuffer.GetBuffer (), ( int ) msWriteBuffer.Length, SocketFlags.None );

			msWriteBuffer.SetLength ( 0 );
		}

		public override int Read ( byte[] buffer, int offset, int count ) {
			#region Check Arguments
			if ( buffer == null )
				throw new ArgumentNullException ( "buffer" );

			if ( underlying == null )
				throw new ObjectDisposedException ( "underlying" );

			if ( offset < 0 )
				throw new ArgumentOutOfRangeException ( "offset" );

			if ( count < 0 )
				throw new ArgumentOutOfRangeException ( "count" );

			if ( buffer.Length < offset )
				throw new ArgumentOutOfRangeException ( "offset" );

			if ( buffer.Length < offset + count )
				throw new ArgumentOutOfRangeException ( "count" );
			#endregion Check Arguments
			
			if ( msReadBuffer.Position == 0 || readingFragmented ) {
				// Ready to peek new datagram from network
				int bytesRead = ( underlying as Socket ).Receive ( packetBuffer, MAX_PACKETLEN, SocketFlags.None );
				msReadBuffer.Write ( packetBuffer, 0, bytesRead );

				if ( packetBuffer [0] == 0xff && packetBuffer [1] == 0xff &&
					 packetBuffer [2] == 0xff && packetBuffer [3] == 0xff ) {	// Connectionless packet
					readPacketKind = PacketKind.Connectionless;
				} else {
					connection.IncomingSequence = BitConverter.ToInt32 ( packetBuffer, 0 );
					readPacketKind = PacketKind.ConnectionOriented;

					if ( ( connection.IncomingSequence & FRAGMENT_BIT ) != 0 ) {
						int seq = connection.IncomingSequence & ( ~FRAGMENT_BIT );

						if ( fragmentReadSequence != seq ) {
							if ( readingFragmented )
								throw new IOException ( "Fragment belongs to another sequence though previous wasn't completed." );

							fragmentReadSequence = seq;
							msReadBuffer.Position = 0;
							msReadBuffer.Write ( ExBitConverter.GetBytes ( seq, true ), 0, 4 );
							readingFragmented = true;
						} else {
							msReadBuffer.Position -= bytesRead;
						}

						int fragmentStart  = BitConverter.ToInt16 ( packetBuffer, 4 );	// Needed only for warning (fragmentStart != msFragmentReadBuffer.Length)
						int fragmentLength = BitConverter.ToInt16 ( packetBuffer, 6 );

						msReadBuffer.Write ( packetBuffer, 8, fragmentLength );

						if ( fragmentLength != FRAGMENT_SIZE ) {
							connection.IncomingSequence = seq;
							readingFragmented = false;
							msReadBuffer.SetLength ( msReadBuffer.Position );

							return	bytesRead;
						} else {
							this.Read ( buffer, offset, count );

							if ( fragmentStart != 0 )	// We're not on the top of the stack (relative to first Read())
								return	bytesRead;
						}
					} else if ( readingFragmented )
						throw new IOException ( "New packet occured while expecting for new fragment." );
				}

				msReadBuffer.Position = 4;
			}
			
			if ( msReadBuffer.Position >= msReadBuffer.Length && count != 0 ) {
				throw new InvalidOperationException
					( "Read position reached end of the packet." + 
					  "Call BeginReadPacket () before subsequent Reads in this case." );
			}

			// Return the next part of previously obtained packet data
			int bytesToReturn = ( int ) ( count + msReadBuffer.Position > msReadBuffer.Length ? 
										  msReadBuffer.Length - msReadBuffer.Position : count );
			msReadBuffer.Read ( buffer, 0, bytesToReturn );

			return	bytesToReturn;
		}

		public override void Write ( byte [] buffer, int offset, int count ) {
			if ( msWriteBuffer.Position == 0 ) {
				if ( writePacketKind == PacketKind.Connectionless ) {
					msWriteBuffer.Write ( new byte [4] { 0xff, 0xff, 0xff, 0xff }, 0, 4 );
				} else if ( writePacketKind == PacketKind.ConnectionOriented ) {
					msWriteBuffer.Write ( ExBitConverter.GetBytes ( connection.OutgoingSequence, true ), 0, 4 );
					msWriteBuffer.Write ( ExBitConverter.GetBytes ( connection.QPort, true ), 0, 2 );
					connection.OutgoingSequence++;
				}
			}

			msWriteBuffer.Write ( buffer, offset, count );
		}

		public override PacketKind BeginReadPacket () {
			base.BeginReadPacket ();
			Read ( i32buf, 0, 0 );
			return	readPacketKind;
		}
		#endregion Q3NetworkStream Methods
	}
}
