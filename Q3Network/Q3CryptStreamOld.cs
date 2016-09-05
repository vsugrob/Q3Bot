using System;
using System.IO;
using System.Collections.Generic;
using Utils;

namespace Q3Network
{
	// Summary:
	//     Specifies whether to encode or decode the underlying stream.
	public enum CypherMode {
		// Summary:
		//     Decode the underlying stream.
		Decode = 0,
		// Summary:
		//     Encode the underlying stream.
		Encode
	}

	// Summary:
	//     Provides methods and properties used to encode and decode streams.
	public class Q3CryptStreamOld : Stream
	{
		#region Q3CypherStream Properties
		public const int CL_ENCODE_START = 12;
		public const int CL_DECODE_START = 4;
		private Stream baseStream;
		private CypherMode mode;
		private bool leaveOpen;
		private Q3Connection connection;
		private byte [] readBuffer = new byte [CL_DECODE_START];
		private int readPosition = 0;		// Bytes read from underlying stream
		private int readReliableAcknowledge;
		private string readPattern;
		private int readKey;
		private int readPatternIndex;

		public Q3Connection Connection { get { return	connection; } }
		public PacketKind ReadPacketKind { get { return	( ( Q3DatagramStream ) baseStream ).ReadPacketKind; } }
		#endregion Q3CypherStream Properties

		#region Properties inherited from base class Stream
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
		public Stream BaseStream { get {
			if ( baseStream == null )
				throw new ObjectDisposedException ( "baseStream" );
			else
				return	this.baseStream;
		} }
		//
		// Summary:
		//     Gets a value indicating whether the stream supports reading while decompressing
		//     a file.
		//
		// Returns:
		//     true if the System.IO.Compression.CompressionMode value is Decompress, and
		//     the underlying stream supports reading and is not closed; otherwise, false.
		public override bool CanRead { get {
			return	this.mode == CypherMode.Decode && this.baseStream.CanRead;
		} }
		//
		// Summary:
		//     Gets a value indicating whether the stream supports seeking.
		//
		// Returns:
		//     false in all cases.
		public override bool CanSeek { get { return	false; } }
		//
		// Summary:
		//     Gets a value indicating whether the stream supports writing.
		//
		// Returns:
		//     true if the System.IO.Compression.CompressionMode value is Compress, and
		//     the underlying stream supports writing and is not closed; otherwise, false.
		public override bool CanWrite { get {
			return	this.mode == CypherMode.Encode && this.baseStream.CanWrite;
		} }
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
		#endregion Properties inherited from base class Stream

		#region Constructors inherited from base class Stream
		//
		// Summary:
		//     Initializes a new instance of the Q3Cypher.Q3CypherStream class
		//     using the specified stream and Q3Cypher.CypherMode value.
		//
		// Parameters:
		//   stream:
		//     The stream to compress or decompress.
		//
		//   mode:
		//     One of the Q3Cypher.CypherMode values that indicates the
		//     action to take.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     stream is null.
		//
		//   System.ArgumentException:
		//     mode is not a valid Q3Cypher.CypherMode enumeration value.
		//      -or- Q3Cypher.CypherMode is Q3Cypher.CypherMode.Encode
		//     and System.IO.Stream.CanWrite is false.  -or- Q3Cypher.CypherMode
		//     is Q3Cypher.CypherMode.Decode and System.IO.Stream.CanRead
		//     is false.
		public Q3CryptStreamOld ( Stream stream, CypherMode mode, Q3Connection connection ) {
			if ( stream == null )
				throw new ArgumentNullException ( "stream" );
			
			if ( mode == CypherMode.Encode && !stream.CanWrite )
				throw new ArgumentException ( "A given stream cannot be written while mode was set to CypherMode.Encode" );

			if ( mode == CypherMode.Decode && !stream.CanRead )
				throw new ArgumentException ( "A given stream cannot be read while mode was set to CypherMode.Decode" );

			this.baseStream = stream;
			this.mode = mode;
			this.connection = connection;
		}
		//
		// Summary:
		//     Initializes a new instance of the System.IO.Compression.GZipStream class
		//     using the specified stream and System.IO.Compression.CompressionMode value,
		//     and a value that specifies whether to leave the stream open.
		//
		// Parameters:
		//   stream:
		//     The stream to compress or decompress.
		//
		//   mode:
		//     One of the System.IO.Compression.CompressionMode values that indicates the
		//     action to take.
		//
		//   leaveOpen:
		//     true to leave the stream open; otherwise, false.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     stream is null.
		//
		//   System.ArgumentException:
		//     mode is not a valid Q3Cypher.CypherMode value.  -or- Q3Cypher.CypherMode
		//     is Q3Cypher.CypherMode.Encode and System.IO.Stream.CanWrite
		//     is false.  -or- Q3Cypher.CypherMode is Q3Cypher.CypherMode.Decode
		//     and System.IO.Stream.CanRead is false.
		public Q3CryptStreamOld ( Stream stream, CypherMode mode, Q3Connection connection, bool leaveOpen ) {
			if ( stream == null )
				throw new ArgumentNullException ( "stream" );

			if ( mode == CypherMode.Encode && !stream.CanWrite )
				throw new ArgumentException ( "A given stream cannot be written while mode was set to CypherMode.Encode" );

			if ( mode == CypherMode.Decode && !stream.CanRead )
				throw new ArgumentException ( "A given stream cannot be read while mode was set to CypherMode.Decode" );

			this.baseStream = stream;
			this.mode = mode;
			this.leaveOpen = leaveOpen;
			this.connection = connection;
		}
		#endregion Constructors inherited from base class Stream

		#region Methods inherited from base class Stream
		//
		// Summary:
		//     Releases the unmanaged resources used by the Q3Cypher.Q3CypherStream
		//     and optionally releases the managed resources.
		//
		// Parameters:
		//   disposing:
		//     true to release both managed and unmanaged resources; false to release only
		//     unmanaged resources.
		protected override void Dispose ( bool disposing ) {
			if ( disposing ) {
				if ( !this.leaveOpen ) {
					this.baseStream.Dispose ();
					this.baseStream = null;
				}
			}
		}
		//
		// Summary:
		//     FIXIT: implement Flush () and make Write () buffered!
		//
		// Exceptions:
		//   System.ObjectDisposedException:
		//     The stream is closed.
		public override void Flush () {
			#region Exception checks
			if ( baseStream == null )
				throw new ObjectDisposedException ( "baseStream" );
			#endregion Exception checks

			baseStream.Flush ();
		}
		//
		// Summary:
		//     Reads a number of decoded bytes into the specified byte array.
		//
		// Parameters:
		//   array:
		//     The array used to store decoded bytes.
		//
		//   offset:
		//     The byte offset in array at which to begin writing data read from the stream.
		//
		//   count:
		//     The number of bytes to decode. 
		//
		// Returns:
		//     The number of bytes that were decoded into the byte array. If the end
		//     of the stream has been reached, zero or the number of bytes read is returned.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     array is null.
		//
		//   System.InvalidOperationException:
		//     The Q3Cypher.CypherMode value was Encode when the object
		//     was created.  - or - The underlying stream does not support reading.
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

			if ( baseStream == null )
				throw new ObjectDisposedException ( "baseStream" );

			if ( mode == CypherMode.Encode )
				throw new InvalidOperationException ( "Stream cannot be read while its mode set to CypherMode.Encode" );

			if ( !baseStream.CanRead )
				throw new InvalidOperationException ( "Underlying stream cannot be read" );

			if ( offset < 0 )
				throw new ArgumentOutOfRangeException ( "offset" );

			if ( count < 0 )
				throw new ArgumentOutOfRangeException ( "count" );

			if ( array.Length < offset )
				throw new ArgumentOutOfRangeException ( "offset" );

			if ( array.Length < offset + count )
				throw new ArgumentOutOfRangeException ( "count" );
			#endregion Exception checks

			if ( this.readPosition == 0 )
				( ( Q3DatagramStream ) baseStream ).BeginReadPacket ();

			int bytesRead = baseStream.Read ( array, offset, count );

			int encStart = 0,
				encEnd = 0;
			
			if ( this.readPosition < CL_DECODE_START ) {
				Array.Copy ( array, offset,
							 this.readBuffer, this.readPosition,
							 bytesRead + this.readPosition > CL_DECODE_START ?
								CL_DECODE_START - this.readPosition : bytesRead );

				if ( this.readPosition + bytesRead >= CL_DECODE_START ) {
					Q3HuffmanStream huff = connection.Q3HuffDStream;
					int bloc = 0;
					this.readReliableAcknowledge = huff.DecompressBufferToInt32 ( this.readBuffer, 0, ref bloc );
					encStart = CL_DECODE_START - this.readPosition + offset;
					encEnd = bytesRead + offset;
					this.readPattern = connection.OutgoingReliableCommands [this.readReliableAcknowledge & ( Q3Connection.MAX_RELIABLE_COMMANDS - 1 )];
					this.readKey = ( byte ) ( connection.Challenge ^ connection.IncomingSequence );
					this.readPatternIndex = 0;
				}
			} else {
				encStart = offset;
				encEnd = bytesRead;
			}

			for ( int i = encStart ; i < encEnd ; i++ ) {
				// modify the key with the last received now acknowledged server command
				if ( this.readPattern == null || this.readPatternIndex >= this.readPattern.Length ) this.readPatternIndex = 0;

				if ( this.readPattern != null && ( this.readPattern [this.readPatternIndex] > 127 || this.readPattern [this.readPatternIndex] == '%' ) )
					this.readKey ^= ( byte ) ( '.' << ( i & 1 ) );
				else
					this.readKey ^= ( byte ) ( ( this.readPattern != null ? this.readPattern [this.readPatternIndex] : 0 ) << ( i & 1 ) );

				this.readPatternIndex++;
				array [i] = ( byte ) ( array [i] ^ this.readKey );
			}

			this.readPosition += bytesRead;

			//string dump = BufferInfo.DumpBuffer ( "Decrypted buffer", array, offset, bytesRead, true, true, true );
			//Console.Write ( dump );

			return	bytesRead;
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
		//     Writes encoded bytes to the underlying stream from the specified byte
		//     array.
		//
		// Parameters:
		//   array:
		//     The array used to store encoded bytes.
		//
		//   offset:
		//     The location in the array to begin reading.
		//
		//   count:
		//     The number of bytes encoded.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     array is null.
		//
		//   System.InvalidOperationException:
		//     The Q3Cypher.CypherMode value was Decode when the object
		//     was created.  - or - The underlying stream does not support writing.
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

			if ( baseStream == null )
				throw new ObjectDisposedException ( "baseStream" );

			if ( mode == CypherMode.Decode )
				throw new InvalidOperationException ( "Stream cannot be written while its mode set to Q3Cypher.CypherMode.Decode" );

			if ( !baseStream.CanWrite )
				throw new InvalidOperationException ( "Underlying stream cannot be written" );

			if ( offset < 0 )
				throw new ArgumentOutOfRangeException ( "offset" );

			if ( count < 0 )
				throw new ArgumentOutOfRangeException ( "count" );

			if ( array.Length < offset )
				throw new ArgumentOutOfRangeException ( "offset" );

			if ( array.Length < offset + count )
				throw new ArgumentOutOfRangeException ( "count" );
			#endregion Exception checks

			( baseStream as Q3DatagramStream ).BeginWritePacket ( PacketKind.ConnectionOriented );
			Q3HuffmanStream huff = connection.Q3HuffDStream;

			int bloc = 0;
			int serverId = huff.DecompressBufferToInt32 ( array, 0, ref bloc );
			int messageAcknowledge = huff.DecompressBufferToInt32 ( array, 0, ref bloc );
			int reliableAcknowledge = huff.DecompressBufferToInt32 ( array, 0, ref bloc );

			byte [] buffer = new byte [count];
			Array.Copy ( array, buffer, CL_ENCODE_START < count ? CL_ENCODE_START : count );
			string pattern = connection.IncomingReliableCommands [reliableAcknowledge & ( Q3Connection.MAX_RELIABLE_COMMANDS - 1 )];
			int index = 0;

			byte key = ( byte ) ( connection.Challenge ^ serverId ^ messageAcknowledge );

			for ( int i = CL_ENCODE_START ; i < count ; i++ ) {
				// modify the key with the last received now acknowledged server command
				if ( pattern == null || index >= pattern.Length ) index = 0;

				if ( pattern != null && ( pattern [index] > 127 || pattern [index] == '%' ) )
					key ^= ( byte ) ( '.' << ( i & 1 ) );
				else
					key ^= ( byte ) ( ( pattern != null ? pattern [index] : 0 ) << ( i & 1 ) );

				index++;

				// encode the data with this key
				buffer [i] = ( byte ) ( array [i + offset] ^ key );
			}

			baseStream.Write ( buffer, 0, count );
		}
		#endregion Methods inherited from base class Stream

		#region Q3CypherStream Methods
		// By this call you specify that Q3NetworkStream must poll underlying stream for a new datagram.
		// All unreaded data stored in readBuffer will be erased.
		// It is like Flush () but for reading purposes.
		public int ReadNew ( byte [] array, int offset, int count, out PacketKind kind ) {
			int bytesReceieved = this.ReadNew ( array, offset, count );
			kind = ( ( Q3NetworkStreamOld ) baseStream ).ReadPacketKind;	// FIXIT: make q3-specified abstract stream instead of System.IO.Stream

			return	bytesReceieved;
		}

		public int ReadNew ( byte [] array, int offset, int count ) {
			this.readPosition = 0;

			return	this.Read ( array, offset, count );
		}
		#endregion Q3CypherStream Methods
	}
}
