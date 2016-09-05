using System;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace Q3Network
{
	// Summary:
	//     Provides the underlying stream of data for Quake III network access.
	public class Q3NetworkStreamOld : Stream
	{
		#region Q3NetworkStream Properties
		private const int MAX_PACKETLEN = 1400;
		private const int FRAGMENT_SIZE = MAX_PACKETLEN - 100;
		private const int FRAGMENT_BIT = 1 << 31;
		private const int FRAGMENT_BUFFER_SIZE = 0x4000;	// Large enough to hold all fragments if packet fragmented
		private byte [] fragmentBuffer = new byte [FRAGMENT_BUFFER_SIZE];
		private int fragmentSequence;
		private int fDataLen;
		public const int WRITE_BUFFER_INIT_SIZE = 0x0600;
		public const int READ_BUFFER_SIZE = FRAGMENT_BUFFER_SIZE;
		private Socket sock;
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
		#endregion Q3NetworkStream Properties

		#region Properties inherited from base class Stream & implemented by underlying NetworkStream
		//
		// Summary:
		//     Gets a reference to the underlying stream.
		//
		// Returns:
		//     A stream object that represents the underlying stream.
		//
		// Exceptions:
		//   System.ObjectDisposedException:
		//     The underlying stream is closed.
		public Socket Socket { get {
			if ( sock == null )
				throw new ObjectDisposedException ( "sock" );
			else
				return	this.sock;
		} }
		//
		// Summary:
		//     Gets a value indicating whether the stream supports reading.
		//
		// Returns:
		//     true if the underlying stream supports reading and is not closed;
		//     otherwise, false.
		public override bool CanRead { get { return	true; } }
		//
		// Summary:
		//     Gets a value indicating whether the stream supports seeking.
		//
		// Returns:
		//     false in all cases.
		public override bool CanSeek { get { return	false; } }
		//
		// Summary:
		//     Indicates whether timeout properties are usable for Q3Network.Q3NetworkStream.
		//
		// Returns:
		//     true in all cases.
		public override bool CanTimeout { get { return	true; } }
		//
		// Summary:
		//     Gets a value indicating whether the stream supports writing.
		//
		// Returns:
		//     true if the underlying stream supports writing and is not closed;
		//     otherwise, false.
		public override bool CanWrite { get { return	true; } }
		//
		// Summary:
		//     Gets a value that indicates whether data is available on the Q3Network.Q3NetworkStream
		//     to be read.
		//
		// Returns:
		//     true if data is available on the stream to be read; otherwise, false.
		//
		// Exceptions:
		//   System.ObjectDisposedException:
		//     The Q3Network.Q3NetworkStream is closed.
		//
		//   System.IO.IOException:
		//     The underlying System.Net.Sockets.Socket is closed.
		//
		//   System.Net.Sockets.SocketException:
		//     Use the System.Net.Sockets.SocketException.ErrorCode property to obtain the
		//     specific error code, and refer to the Windows Sockets version 2 API error
		//     code documentation in MSDN for a detailed description of the error.
		public virtual bool DataAvailable { get { return	dGramLen > msReadBuffer.Position; } }
		public int Available { get { return	 ( int ) ( dGramLen - msReadBuffer.Position ); } }
		public override long Length { get { throw new NotSupportedException (); } }
		//
		// Summary:
		//     This property is not supported and always throws a System.NotSupportedException.
		//
		// Returns:
		//     A long value.
		//
		// Exceptions:
		//   System.NotSupportedException:
		//     This property is not supported on this stream.
		public override long Position {
			get { throw new NotSupportedException (); }
			set { throw new NotSupportedException (); }
		}
		//
		// Summary:
		//     Gets or sets the amount of time that a read operation blocks waiting for
		//     data.
		//
		// Returns:
		//     A System.Int32 that specifies the amount of time, in milliseconds, that will
		//     elapse before a read operation fails. The default value, System.Threading.Timeout.Infinite,
		//     specifies that the read operation does not time out.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     The value specified is less than or equal to zero and is not System.Threading.Timeout.Infinite.
		public override int ReadTimeout {
			get { return	this.sock.ReceiveTimeout; }
			set { this.sock.ReceiveTimeout = value; }
		}
		//
		// Summary:
		//     Gets or sets the amount of time that a write operation blocks waiting for
		//     data.
		//
		// Returns:
		//     A System.Int32 that specifies the amount of time, in milliseconds, that will
		//     elapse before a write operation fails. The default value, System.Threading.Timeout.Infinite,
		//     specifies that the write operation does not time out.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     The value specified is less than or equal to zero and is not System.Threading.Timeout.Infinite.
		public override int WriteTimeout {
			get { return	this.sock.SendTimeout; }
			set { this.sock.SendTimeout = value; }
		}
		#endregion Properties inherited from base class Stream & implemented by underlying NetworkStream

		#region Constructors
		//
		// Summary:
		//     Initializes a new instance of the Q3Network.Q3NetworkStream class.
		//
		// Parameters:
		//   stream:
		//     The stream to compress or decompress.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     stream is null.
		public Q3NetworkStreamOld ( Socket sock, Q3Connection connection ) {
			if ( sock == null )
				throw new ArgumentNullException ( "sock" );

			if ( connection == null )
				throw new ArgumentNullException ( "connection" );

			this.sock = sock;
			this.connection = connection;
			this.msReadBuffer = new MemoryStream ( this.readBuffer, false );
		}
		//
		// Summary:
		//     Initializes a new instance of the Q3Network.Q3NetworkStream class
		//     using a value that specifies whether to leave the stream open.
		//
		// Parameters:
		//   stream:
		//     The stream to compress or decompress.
		//
		//   leaveOpen:
		//     true to leave the stream open; otherwise, false.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     stream is null.
		public Q3NetworkStreamOld ( Socket sock, Q3Connection connection, bool leaveOpen ) {
			if ( sock == null )
				throw new ArgumentNullException ( "sock" );

			if ( connection == null )
				throw new ArgumentNullException ( "connection" );

			this.sock = sock;
			this.connection = connection;
			this.leaveOpen  = leaveOpen;
			this.msReadBuffer = new MemoryStream ( this.readBuffer, false );
		}
		#endregion Constructors

		#region Methods inherited from base class Stream & implemented by underlying NetworkStream
		//
		// Summary:
		//     Closes the Q3Network.Q3NetworkStream after waiting the specified time
		//     to allow data to be sent.
		//
		// Parameters:
		//   timeout:
		//     A 32-bit signed integer that specifies the number of milliseconds to wait
		//     to send any remaining data before closing.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     The timeout parameter is less than -1.
		public void Close( int timeout ) {
			this.sock.Close ( timeout );
		}
		//
		// Summary:
		//     Releases the unmanaged resources used by the Q3Network.Q3NetworkStream
		//     and optionally releases the managed resources.
		//
		// Parameters:
		//   disposing:
		//     true to release both managed and unmanaged resources; false to release only
		//     unmanaged resources.
		protected override void Dispose ( bool disposing ) {
			if ( disposing ) {
				if ( !this.leaveOpen ) {
					this.sock.Close ();
					this.sock = null;
				}
			}
		}
		//
		// Summary:
		//     Flush is not necessary for this kind of stream
		//
		// Exceptions:
		//   System.ObjectDisposedException:
		//     The stream is closed.
		public override void Flush () {
			#region Exception checks
			if ( sock == null )
				throw new ObjectDisposedException ( "sock" );
			#endregion Exception checks

			sock.Send ( msWriteBuffer.GetBuffer (), ( int ) msWriteBuffer.Length, SocketFlags.None );
			msWriteBuffer.SetLength ( 0 );
			justFlushed = true;
			writePacketKind = PacketKind.ConnectionOriented;
		}
		//
		// Summary:
		//     Reads data from the Q3Network.Q3NetworkStream.
		//
		// Parameters:
		//   array:
		//     The array used to store obtained bytes.
		//
		//   offset:
		//     The byte offset in array at which to begin writing data read from the stream.
		//
		//   count:
		//     The number of bytes to read. 
		//
		// Returns:
		//     The number of bytes that were read into the byte array. If the end
		//     of the stream has been reached, zero or the number of bytes read is returned.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     array is null.
		//
		//   System.ArgumentOutOfRangeException:
		//     offset or count is less than zero.  -or- array length minus the index starting
		//     point is less than count.
		//
		//   System.ObjectDisposedException:
		//     The stream is closed.
		public override int Read ( byte [] array, int offset, int count )
		{
			#region Exception checks
			if ( array == null )
				throw new ArgumentNullException ( "array" );

			if ( sock == null )
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
				// Ready to peek new datagram from network
				int bytesRead = sock.Receive ( readBuffer, READ_BUFFER_SIZE, SocketFlags.None );
				this.dGramLen = bytesRead;

				if ( readBuffer [0] == 0xff && readBuffer [1] == 0xff &&
					 readBuffer [2] == 0xff && readBuffer [3] == 0xff ) {	// Connectionless packet
					this.readPacketKind = PacketKind.Connectionless;
				} else {
					connection.IncomingSequence = BitConverter.ToInt32 ( readBuffer, 0 );
					this.readPacketKind = PacketKind.ConnectionOriented;

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
				}

				msReadBuffer.Position = 4;
			}

			// Return the next part of previously obtained packet data
			int bytesToReturn = ( int ) ( count + msReadBuffer.Position > this.dGramLen ? 
										  this.dGramLen - msReadBuffer.Position : count );
			msReadBuffer.Read ( array, 0, bytesToReturn );

			return	bytesToReturn;
		}
		//
		// Summary:
		//     This property is not supported and always throws a System.NotSupportedException.
		//
		// Parameters:
		//   offset:
		//     The location in the stream.
		//
		//   origin:
		//     One of the System.IO.SeekOrigin values.
		//
		// Returns:
		//     A long value.
		//
		// Exceptions:
		//   System.NotSupportedException:
		//     This property is not supported on this stream.
		public override long Seek ( long offset, SeekOrigin origin ) {
			throw new NotSupportedException ();
		}
		//
		// Summary:
		//     This property is not supported and always throws a System.NotSupportedException.
		//
		// Parameters:
		//   value:
		//     The length of the stream.
		//
		// Exceptions:
		//   System.NotSupportedException:
		//     This property is not supported on this stream.
		public override void SetLength ( long value ) {
			throw new NotSupportedException ();
		}
		//
		// Summary:
		//     Writes bytes to the underlying stream from the specified byte
		//     array.
		//
		// Parameters:
		//   array:
		//     An array of type System.Byte that contains the data to write to the stream.
		//
		//   offset:
		//     The location in the array to begin reading.
		//
		//   count:
		//     The number of bytes to write to the stream.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     array is null.
		//
		//   System.ArgumentOutOfRangeException:
		//     offset or count is less than zero.  -or- array length minus the index starting
		//     point is less than count.
		//
		//   System.ObjectDisposedException:
		//     The stream is closed.
		public override void Write ( byte [] array, int offset, int count ) {
			#region Exception checks
			if ( array == null )
				throw new ArgumentNullException ( "array" );

			if ( sock == null )
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

		#region Q3NetworkStream methods
		public void Write ( byte [] array, int offset, int count, PacketKind kind ) {
			if ( this.writePacketKind != kind && msWriteBuffer.Length != 0 )
				this.Flush ();

			this.writePacketKind = kind;
			this.Write ( array, offset, count );
		}

		// By this call you specify that Q3NetworkStream must poll underlying stream for a new datagram.
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
		#endregion Q3NetworkStream methods
	}
}
