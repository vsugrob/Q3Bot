using System;
using System.IO;

namespace Q3Network
{
	public class Q3DemoStream : Stream
	{
		#region Q3DemoStream Properties
		private const int MAX_PACKETLEN = 1400;
		private const int FRAGMENT_SIZE = MAX_PACKETLEN - 100;
		private const int FRAGMENT_BIT = 1 << 31;
		private const int FRAGMENT_BUFFER_SIZE = 0x4000;	// Large enough to hold all fragments if packet fragmented
		private byte [] fragmentBuffer = new byte [FRAGMENT_BUFFER_SIZE];
		private int fragmentSequence;
		private int fDataLen;
		public const int WRITE_BUFFER_INIT_SIZE = 0x0600;
		public const int READ_BUFFER_SIZE = FRAGMENT_BUFFER_SIZE;
		private Stream baseStream;
		private bool leaveOpen;
		private Q3Connection connection;
		private MemoryStream msWriteBuffer = new MemoryStream ( WRITE_BUFFER_INIT_SIZE );
		private byte [] readBuffer = new byte [READ_BUFFER_SIZE];
		private MemoryStream msReadBuffer;
		private int dGramLen = 0;
		private bool justFlushed = true;
		private PacketKind writePacketKind = PacketKind.ConnectionOriented;
		private PacketKind readPacketKind = PacketKind.ConnectionOriented;

		public Q3Connection Connection { get { return	connection; } }
		public PacketKind ReadPacketKind { get { return	readPacketKind; } }
		#endregion Q3DemoStream Properties

		#region Properties inherited from base class Stream
		public Stream BaseStream { get {
			if ( baseStream == null )
				throw new ObjectDisposedException ( "sock" );
			else
				return	this.baseStream;
		} }

		public override bool CanRead { get { return	true; } }
		public override bool CanSeek { get { return	false; } }
		public override bool CanTimeout { get { return	true; } }
		public override bool CanWrite { get { return	true; } }
		public virtual bool DataAvailable { get { return	dGramLen > msReadBuffer.Position; } }
		public int Available { get { return	 ( int ) ( dGramLen - msReadBuffer.Position ); } }
		public override long Length { get { throw new NotSupportedException (); } }

		public override long Position {
			get { throw new NotSupportedException (); }
			set { throw new NotSupportedException (); }
		}

		public override int ReadTimeout {
			get { return	this.baseStream.ReadTimeout; }
			set { this.baseStream.ReadTimeout = value; }
		}

		public override int WriteTimeout {
			get { return	this.baseStream.WriteTimeout; }
			set { this.baseStream.WriteTimeout = value; }
		}
		#endregion Properties inherited from base class Stream & implemented by underlying NetworkStream

		#region Constructors
		public Q3DemoStream ( Stream baseStream, Q3Connection connection ) {
			if ( baseStream == null )
				throw new ArgumentNullException ( "sock" );

			if ( connection == null )
				throw new ArgumentNullException ( "connection" );

			this.baseStream = baseStream;
			this.connection = connection;
			this.msReadBuffer = new MemoryStream ( this.readBuffer, false );
		}

		public Q3DemoStream ( Stream baseStream, Q3Connection connection, bool leaveOpen ) {
			if ( baseStream == null )
				throw new ArgumentNullException ( "sock" );

			if ( connection == null )
				throw new ArgumentNullException ( "connection" );

			this.baseStream = baseStream;
			this.connection = connection;
			this.leaveOpen  = leaveOpen;
			this.msReadBuffer = new MemoryStream ( this.readBuffer, false );
		}
		#endregion Constructors

		#region Methods inherited from base class Stream
		public void Close( int timeout ) {
			this.baseStream.Close ();
		}

		protected override void Dispose ( bool disposing ) {
			if ( disposing ) {
				if ( !this.leaveOpen ) {
					this.baseStream.Close ();
					this.baseStream = null;
				}
			}
		}

		public override void Flush () {
			#region Exception checks
			if ( baseStream == null )
				throw new ObjectDisposedException ( "sock" );
			#endregion Exception checks

			baseStream.Write ( msWriteBuffer.GetBuffer (), 0, ( int ) msWriteBuffer.Length );
			msWriteBuffer.SetLength ( 0 );
			justFlushed = true;
			writePacketKind = PacketKind.ConnectionOriented;
		}

		public override int Read ( byte [] array, int offset, int count )
		{
			#region Exception checks
			if ( array == null )
				throw new ArgumentNullException ( "array" );

			if ( baseStream == null )
				throw new ObjectDisposedException ( "baseStream" );

			if ( offset < 0 )
				throw new ArgumentOutOfRangeException ( "offset" );

			if ( count < 0 )
				throw new ArgumentOutOfRangeException ( "count" );

			if ( array.Length < offset )
				throw new ArgumentOutOfRangeException ( "offset" );

			if ( array.Length < offset + count )
				throw new ArgumentOutOfRangeException ( "count" );
			#endregion Exception checks

			if ( msReadBuffer.Position >= this.dGramLen && this.dGramLen != 0 ) {
				throw new InvalidOperationException
					( "Read position reached the end of the buffer." + 
					  "Call ReadNew () instead of Read () in this case." +
					  "By calling ReadNew () you will be also notified about kind of received datagram" );
			}
			
			if ( msReadBuffer.Position == 0 ) {
				// Ready to peek new datagram from underlying stream
				byte [] i32buf = new byte [4];
				int bytesRead, curSize;
				bool ok = false;
				
				if ( 4 == ( bytesRead = baseStream.Read ( i32buf, 0, 4 ) ) ) {
					connection.IncomingSequence = BitConverter.ToInt32 ( i32buf, 0 );

					if ( 4 == ( bytesRead = baseStream.Read ( i32buf, 0, 4 ) ) &&
						-1 != ( curSize = BitConverter.ToInt32 ( i32buf, 0 ) ) &&
						curSize == baseStream.Read ( readBuffer, 0, curSize ) ) {
						bytesRead = curSize;
						ok = true;
					}
				}

				if ( !ok )
					throw new OverflowException ( "Base stream couldn't provide requested amount of data" );
				
				this.dGramLen = bytesRead;

				if ( ( connection.IncomingSequence & FRAGMENT_BIT ) != 0 ) {
					int seq = connection.IncomingSequence & ( ~FRAGMENT_BIT );

					if ( this.fragmentSequence != seq ) {
						this.fragmentSequence = seq;
						Array.Copy ( BitConverter.GetBytes ( seq ), fragmentBuffer, 4 );
						this.fDataLen = 4;
					}

					int fragmentStart  = BitConverter.ToInt16 ( readBuffer, 4 );	// Needed only for warning (fragmentStart != this.fDataLen)
					int fragmentLength = BitConverter.ToInt16 ( readBuffer, 6 );

					Array.Copy ( readBuffer, 8, fragmentBuffer, this.fDataLen, fragmentLength );
					this.fDataLen += fragmentLength;

					if ( fragmentLength != FRAGMENT_SIZE ) {
						connection.IncomingSequence = seq;
						Array.Copy ( fragmentBuffer, readBuffer, fDataLen );
						this.dGramLen = fDataLen;

						return	bytesRead;
					} else {
						this.Read ( array, offset, count );

						if ( fragmentStart != 0 )	// We're not on the top of the stack (relative to first Read())
							return	bytesRead;
					}
				}

				msReadBuffer.Position = 0;
			}

			// Return the next part of previously obtained packet data
			int bytesToReturn = ( int ) ( count + msReadBuffer.Position > this.dGramLen ? 
										  this.dGramLen - msReadBuffer.Position : count );
			msReadBuffer.Read ( array, 0, bytesToReturn );

			return	bytesToReturn;
		}

		public override long Seek ( long offset, SeekOrigin origin ) {
			throw new NotSupportedException ();
		}

		public override void SetLength ( long value ) {
			throw new NotSupportedException ();
		}

		public override void Write ( byte [] array, int offset, int count ) {
			#region Exception checks
			if ( array == null )
				throw new ArgumentNullException ( "array" );

			if ( baseStream == null )
				throw new ObjectDisposedException ( "baseStream" );

			if ( offset < 0 )
				throw new ArgumentOutOfRangeException ( "offset" );

			if ( count < 0 )
				throw new ArgumentOutOfRangeException ( "count" );

			if ( array.Length < offset )
				throw new ArgumentOutOfRangeException ( "offset" );

			if ( array.Length < offset + count )
				throw new ArgumentOutOfRangeException ( "count" );
			#endregion Exception checks
			// WARNING: Fragmentation issues not included! But usually we can
			// easily avoid them by not sending redicilous giant packets.
			if ( writePacketKind == PacketKind.Connectionless ) {
				if ( justFlushed ) {
					msWriteBuffer.Write ( new byte [4] { 0xff, 0xff, 0xff, 0xff }, 0, 4 );
					justFlushed = false;
				}

				msWriteBuffer.Write ( array, offset, count );
			} else if ( writePacketKind == PacketKind.ConnectionOriented ) {
				if ( justFlushed ) {
					// Note that following code write bytes in Little Endian unlike most of quake source
					msWriteBuffer.Write ( BitConverter.GetBytes ( connection.OutgoingSequence ), 0, 4 );
					msWriteBuffer.Write ( BitConverter.GetBytes ( connection.QPort ), 0, 2 );
					connection.OutgoingSequence++;
					justFlushed = false;
				}

				msWriteBuffer.Write ( array, 0, count );
			}
		}
		#endregion Methods inherited from base class Stream & implemented by underlying NetworkStream

		#region Q3DemoStream methods
		public void Write ( byte [] array, int offset, int count, PacketKind kind ) {
			if ( this.writePacketKind != kind && msWriteBuffer.Length != 0 )
				this.Flush ();

			this.writePacketKind = kind;
			this.Write ( array, offset, count );
		}

		// By this call you specify that Q3DemoStream must poll underlying stream for a new datagram.
		// All unreaded data stored in readBuffer will be erased.
		// It is like Flush () but for reading purposes.
		public int ReadNew ( byte [] array, int offset, int count, out PacketKind kind ) {
			int bytesReceieved = this.ReadNew ( array, offset, count );
			kind = this.readPacketKind;

			return	bytesReceieved;
		}

		public int ReadNew ( byte [] array, int offset, int count ) {
			msReadBuffer.Position = dGramLen = 0;
			int bytesReceieved = this.Read ( array, offset, count );

			return	bytesReceieved;
		}
		#endregion Q3DemoStream methods
	}
}
