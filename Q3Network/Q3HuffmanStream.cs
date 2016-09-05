using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Collections;
using Utils;

namespace Q3Network
{
	// Summary:
	//     Provides methods and properties used to compress and decompress streams.
	public partial class Q3HuffmanStream : Stream
	{
		#region Q3HuffmanStream Properties
		public const int WRITE_BUFFER_INIT_SIZE = 0x600;
		public const int WRITE_BUFFER_GROWTH = 0x100;
		public byte [] writeBuffer;		// FIXIT: private
		private int bytesWrittenToUnderlying;
		private Encoding textEncoding = Encoding.ASCII;
		public const int READ_BUFFER_SIZE = 0x20;
		private byte [] readBuffer;
		private int lastBytesReadFromUnderlying;
		private int bytesReadFromUnderlying;
		private int bitsRead;
		// Temporary buffers for reading
		byte [] i16buf = new byte [2];
		byte [] i32buf = new byte [4];

		public int BytesWrittenToUnderlying { get { return	bytesWrittenToUnderlying; } }
		public int BytesReadFromUnderlying { get { return	bytesReadFromUnderlying; } }
		public int BitsRead  { get { return	bitsRead; } }
		public int BytesRead { get { return	( bitsRead >> 3 ) + 1; } }

		public Encoding TextEncoding {
			get { return	textEncoding; }
			set { textEncoding = value; }
		}

		public bool TreeIsFrozen { get; set; }

		public PacketKind ReadPacketKind { get { return	( ( Q3DatagramStream ) baseStream ).ReadPacketKind; } }
		#endregion Q3HuffmanStream Properties

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
			return	this.mode == CompressionMode.Decompress && this.baseStream.CanRead;
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
			return	this.mode == CompressionMode.Compress && this.baseStream.CanWrite;
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
		//     Initializes a new instance of the Q3Huffman.Q3HuffmanStream class
		//     using the specified stream and System.IO.Compression.CompressionMode value.
		//
		// Parameters:
		//   stream:
		//     The stream to compress or decompress.
		//
		//   mode:
		//     One of the System.IO.Compression.CompressionMode values that indicates the
		//     action to take.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     stream is null.
		//
		//   System.ArgumentException:
		//     mode is not a valid System.IO.Compression.CompressionMode enumeration value.
		//      -or- System.IO.Compression.CompressionMode is System.IO.Compression.CompressionMode.Compress
		//     and System.IO.Stream.CanWrite is false.  -or- System.IO.Compression.CompressionMode
		//     is System.IO.Compression.CompressionMode.Decompress and System.IO.Stream.CanRead
		//     is false.
		public Q3HuffmanStream ( Stream stream, CompressionMode mode ) {
			if ( stream == null )
				throw new ArgumentNullException ( "stream" );
			
			if ( mode == CompressionMode.Compress && !stream.CanWrite )
				throw new ArgumentException ( "A given stream cannot be written while mode was set to CompressionMode.Compress" );

			if ( mode == CompressionMode.Decompress && !stream.CanRead )
				throw new ArgumentException ( "A given stream cannot be read while mode was set to CompressionMode.Decompress" );

			this.baseStream = stream;
			this.mode = mode;
			this.Init ();
		}
		//
		// Summary:
		//     Initializes a new instance of the Q3Huffman.Q3HuffmanStream class
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
		//     mode is not a valid System.IO.Compression.CompressionMode value.  -or- System.IO.Compression.CompressionMode
		//     is System.IO.Compression.CompressionMode.Compress and System.IO.Stream.CanWrite
		//     is false.  -or- System.IO.Compression.CompressionMode is System.IO.Compression.CompressionMode.Decompress
		//     and System.IO.Stream.CanRead is false.
		public Q3HuffmanStream ( Stream stream, CompressionMode mode, bool leaveOpen ) {
			if ( stream == null )
				throw new ArgumentNullException ( "stream" );

			if ( mode == CompressionMode.Compress && !stream.CanWrite )
				throw new ArgumentException ( "A given stream cannot be written while mode was set to CompressionMode.Compress" );

			if ( mode == CompressionMode.Decompress && !stream.CanRead )
				throw new ArgumentException ( "A given stream cannot be read while mode was set to CompressionMode.Decompress" );

			this.baseStream = stream;
			this.mode = mode;
			this.leaveOpen = leaveOpen;
			this.Init ();
		}
		#endregion Constructors inherited from base class Stream

		#region Methods inherited from base class Stream
		//
		// Summary:
		//     Releases the unmanaged resources used by the Q3Huffman.Q3HuffmanStream
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

				tree = lhead = ltail = null;
				loc  = nodeList = null;
				nodePtrs = null;
			}
		}
		//
		// Summary:
		//     Flushes buffered contents to underlying stream.
		//
		// Exceptions:
		//   System.ObjectDisposedException:
		//     The stream is closed.
		public override void Flush () {
			#region Exception checks
			if ( baseStream == null )
				throw new ObjectDisposedException ( "baseStream" );
			#endregion Exception checks

			if ( baseStream is Q3DatagramStream ) ( baseStream as Q3DatagramStream ).BeginWritePacket ( PacketKind.ConnectionOriented );
			baseStream.Write ( writeBuffer, 0, this.BytesWrittenToUnderlying );
			baseStream.Flush ();
			this.bloc = 0;
		}
		//
		// Summary:
		//     Reads a number of decompressed bytes into the specified byte array.
		//
		// Parameters:
		//   array:
		//     The array used to store decompressed bytes.
		//
		//   offset:
		//     The byte offset in array at which to begin writing data read from the stream.
		//
		//   count:
		//     The number of bytes to decompress. Note that due to some peculiar properties
		//     of Huffman's algorithm, you must specify EXACT number of decompressed bytes 
		//     held by stream.
		//
		// Returns:
		//     The number of bytes that were decompressed into the byte array. If the end
		//     of the stream has been reached, zero or the number of bytes read is returned.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     array is null.
		//
		//   System.InvalidOperationException:
		//     The System.IO.Compression.CompressionMode value was Compress when the object
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

			if ( mode == CompressionMode.Compress )
				throw new InvalidOperationException ( "Stream cannot be read while its mode set to CompressionMode.Compress" );

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

			int i;

			for ( i = 0 ; i < count ; i++ ) {
				if ( ( this.bloc >> 3 ) >= lastBytesReadFromUnderlying )
					break;

				int ch = this.Receive ();

				if ( ch == NYT ) {	// We got a NYT, get the symbol associated with it
					ch = 0;

					for ( int j = 0; j < 8; j++ )
						ch = ( ch << 1 ) + this.GetBit ();
				}
				
				array [i + offset] = ( byte ) ch;	// Write symbol
				if ( !TreeIsFrozen ) this.AddRef ( ( byte ) ch );		// Increment node
			}

			this.bitsRead += i << 3;

			return	i;
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
		//     Writes compressed bytes to the underlying stream from the specified byte
		//     array.
		//
		// Parameters:
		//   array:
		//     The array used to store compressed bytes.
		//
		//   offset:
		//     The location in the array to begin reading.
		//
		//   count:
		//     The number of bytes compressed.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     array is null.
		//
		//   System.InvalidOperationException:
		//     The System.IO.Compression.CompressionMode value was Decompress when the object
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

			if ( mode == CompressionMode.Decompress )
				throw new InvalidOperationException ( "Stream cannot be written while its mode set to CompressionMode.Decompress" );

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

			for ( int i = 0 ; i < count ; i++ ) {
				byte ch = array [i + offset];
				this.Transmit ( ch );
				if ( !TreeIsFrozen ) this.AddRef ( ch );
				this.bytesWrittenToUnderlying = ( bloc >> 3 ) + 1;	// According to Quake III sources though i think it's not right
			}
		}
		#endregion Methods inherited from base class Stream

		#region Q3HuffmanStream Methods
		// By this call you specify that Q3HuffmanStream must poll underlying stream for a new datagram.
		// All unreaded data stored in readBuffer will be erased.
		// It is like Flush () but for reading purposes.
		public int ReadNew ( byte [] array, int offset, int count, out PacketKind kind ) {
			int bytesReceieved = this.ReadNew ( array, offset, count );
			kind = baseStream is Q3DatagramStream ? ( ( Q3DatagramStream ) baseStream ).ReadPacketKind : PacketKind.Unknown;	// FIXIT: make q3-specified abstract stream instead of System.IO.Stream

			return	bytesReceieved;
		}

		public int ReadNew ( byte [] array, int offset, int count ) {
			this.bloc = 0;
			this.lastBytesReadFromUnderlying = 0;
			this.bytesReadFromUnderlying = 0;
			this.bitsRead = 0;
			PumpReadBuffer ();	// первый подсос

			return	this.Read ( array, offset, count );
		}

		public PacketKind BeginRead () {
			this.bloc = 0;
			this.lastBytesReadFromUnderlying = 0;
			this.bytesReadFromUnderlying = 0;
			this.bitsRead = 0;
			PumpReadBuffer ();

			return	baseStream is Q3DatagramStream ? ( ( Q3DatagramStream ) baseStream ).ReadPacketKind :
				( baseStream is Q3DemoStream ? ( ( Q3DemoStream ) baseStream ).ReadPacketKind : PacketKind.Unknown );	// FIXIT: make q3-specified abstract stream instead of System.IO.Stream
		}

		public void EndRead () {
			// do nothing
		}

		public int ReadBits ( int bits ) {
			int i, nbits = 0;
			int value = 0;
			bool sgn;

			if ( bits < 0 ) {
				bits = -bits;
				sgn = true;
			} else {
				sgn = false;
			}

			if ( ( bits & 7 ) != 0 ) {
				nbits = bits & 7;
				this.bitsRead += nbits;

				for ( i = 0 ; i < nbits ; i++ ) {
					int t = ( readBuffer [( bloc >> 3 )] >> ( bloc & 7 ) ) & 0x1;
					IncReadBloc ();
					value |= t << i;
				}

				bits = bits - nbits;
			}

			int nbytes = bits >> 3;
			byte [] buf = new byte [nbytes];
			this.Read ( buf, 0, nbytes );

			for ( i = 0 ; i < nbytes ; i++ )
				value |= buf [i] << nbits;

			if ( sgn && ( 0 != ( value & ( 1 << ( bits - 1 ) ) ) ) )
				value |= -1 ^ ( ( 1 << bits ) - 1 );

			return	value;
		}

		public Int16 ReadInt16 () {
			this.Read ( i16buf, 0, 2 );
			return	BitConverter.ToInt16 ( i16buf, 0 );
		}

		public Int32 ReadInt32 () {
			this.Read ( i32buf, 0, 4 );
			return	BitConverter.ToInt32 ( i32buf, 0 );
		}

		public string ReadString () {
			int c = this.ReadByte ();
			string result = "";

			while ( c != 0 && c != -1 ) {
				result += ( char ) c;
				c = this.ReadByte ();
			}

			return	result;
		}

		public void WriteBits ( int value, int bits ) {
			int i, nbits = 0;

			if ( ( bits & 7 ) != 0 ) {
				nbits = bits & 7;

				for ( i = 0 ; i < nbits ; i++ ) {
					if ( ( bloc & 7 ) == 0 )
						writeBuffer [( bloc >> 3 )] = 0;

					writeBuffer [( bloc >> 3 )] |= ( byte ) ( ( value & 0x1 ) << ( bloc & 7 ) );
					IncWriteBloc ();
					value >>= 1;
				}

				bits = bits - nbits;
			}

			int nbytes = bits >> 3;
			byte [] buf = BitConverter.GetBytes ( value );
			this.Write ( buf, 0, nbytes );
		}

		public void WriteInt16 ( Int16 value ) {
			this.Write ( BitConverter.GetBytes ( value ), 0, 2 );
		}

		public void WriteInt32 ( Int32 value ) {
			this.Write ( BitConverter.GetBytes ( value ), 0, 4 );
		}

		public void WriteString ( string value, Encoding encoding ) {
			this.Write ( encoding.GetBytes ( value ), 0, value.Length );
			this.WriteByte ( 0x00 );
		}

		public void WriteString ( string value ) {
			this.WriteString ( value, this.textEncoding );
		}

		public int DecompressBuffer ( byte [] src_array, int src_offset, int count, byte [] dst_array, int dst_offset, ref int bufBloc )
		{
			#region Exception checks
			if ( src_array == null )
				throw new ArgumentNullException ( "array" );

			if ( src_offset < 0 )
				throw new ArgumentOutOfRangeException ( "offset" );

			if ( count < 0 )
				throw new ArgumentOutOfRangeException ( "count" );

			if ( src_array.Length < src_offset )
				throw new ArgumentOutOfRangeException ( "offset" );

			if ( src_array.Length < src_offset + count )
				throw new ArgumentOutOfRangeException ( "count" );
			#endregion Exception checks

			int sbloc = this.bloc;
			this.bloc = bufBloc;
			byte [] buf = src_array;
			int baseBytesRead = src_array.Length;
			
			if ( baseBytesRead == 0 )
				return	0;

			this.bloc += dst_offset << 3;
			int i;

			for ( i = 0 ; i < count ; i++ ) {
				if ( ( this.bloc >> 3 ) >= baseBytesRead )
					break;

				int ch = this.ReceiveFromBuffer ( buf );

				if ( ch == NYT ) {	// We got a NYT, get the symbol associated with it
					ch = 0;

					for ( int j = 0; j < 8; j++ )
						ch = ( ch << 1 ) + this.GetBitFromBuffer ( buf );
				}
				
				dst_array [i + src_offset] = ( byte ) ch;	// Write symbol
				if ( !TreeIsFrozen ) this.AddRef ( ( byte ) ch );		// Increment node
			}

			bufBloc = this.bloc;
			this.bloc = sbloc;

			return	i;
		}

		public Int32 DecompressBufferToInt32 ( byte [] array, int offset, ref int bufBloc ) {
			byte [] dst_array = new byte [4];

			DecompressBuffer ( array, offset, 4, dst_array, 0, ref bufBloc );

			return	BitConverter.ToInt32 ( dst_array, 0 );
		}

		public string LocDump {
			get {
				string dump = "";

				for ( int i = 0 ; i < 256 ; i++ ) {
					dump += string.Format ( "[{0}]=[0x{1:x8}]\r\n", i, loc [i] != null ? loc [i].weight : 0 );
				}

				return	dump;
			}
		}
		#endregion Q3HuffmanStream Methods
	}
}
