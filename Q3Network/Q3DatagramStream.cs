using System;
using System.IO;
using System.Text;
using Utils;

namespace Q3Network
{
	public enum PacketKind {
		Unknown,
		Connectionless,
		ConnectionOriented
	}

	public abstract class Q3DatagramStream : Stream
	{
		#region Q3PacketStream Properties
		protected object underlying;
		protected FileAccess access;
		protected Q3Connection connection;
		protected PacketKind readPacketKind  = PacketKind.Unknown;
		protected PacketKind writePacketKind = PacketKind.ConnectionOriented;
		protected int readTimeout;
		protected int writeTimeout;
		protected MemoryStream msReadBuffer;
		protected MemoryStream msWriteBuffer;
		protected int bytesRead;	// In case when descending class avoids use of msReadBuffer
		protected int bytesWritten;	// In case when descending class avoids use of msWriteBuffer
		protected Encoding textEncoding = Encoding.ASCII;
		protected bool isLittleEndian = true;
		protected byte [] i16buf = new byte [4];
		protected byte [] i32buf = new byte [4];

		public object Underlying { get {
			if ( underlying == null )
				throw new ObjectDisposedException ( "underlying" );
			else
				return	underlying;
		} }
		public FileAccess Access { get { return	access; } }
		public Q3Connection Connection { get { return	connection; } }
		public PacketKind ReadPacketKind  { get {
			return	underlying is Q3DatagramStream ? ( underlying as Q3DatagramStream ).ReadPacketKind : readPacketKind;
		} }

		public PacketKind WritePacketKind { get {
			return	underlying is Q3DatagramStream ? ( underlying as Q3DatagramStream ).WritePacketKind : writePacketKind;
		} }

		public int BytesRead { get { return	msReadBuffer != null ? ( int ) msReadBuffer.Position : bytesRead; } }
		public int BytesWritten { get { return	msWriteBuffer != null ? ( int ) msWriteBuffer.Position : bytesWritten; } }
		public Encoding TextEncoding {
			get { return textEncoding; }
			set { textEncoding = value; }
		}

		public bool IsLittleEndian {
			get { return	isLittleEndian; }
			set { isLittleEndian = value; }
		}

		public override bool CanRead { get { return	( ( int ) access & ( int ) FileAccess.Read ) == ( int ) FileAccess.Read; } }
		public override bool CanSeek { get { return	false; } }
		public override bool CanTimeout { get {
			return	underlying is Stream ? ( underlying as Stream ).CanTimeout : false;
		} }

		public override bool CanWrite { get { return	( ( int ) access & ( int ) FileAccess.Write ) == ( int ) FileAccess.Write; } }
		public override long Length { get { throw new NotSupportedException (); } }
		public override long Position {
			get { throw new NotSupportedException (); }
			set { throw new NotSupportedException (); }
		}

		public override int ReadTimeout {
			get { return	underlying is Stream ? ( underlying as Stream ).ReadTimeout : readTimeout; }
			set { if ( underlying is Stream ) ( underlying as Stream ).ReadTimeout = value; else readTimeout = value; }
		}

		public override int WriteTimeout {
			get { return	underlying is Stream ? ( underlying as Stream ).WriteTimeout : writeTimeout; }
			set { if ( underlying is Stream ) ( underlying as Stream ).WriteTimeout = value; else writeTimeout = value; }
		}
		#endregion Q3PacketStream Properties

		#region Q3PacketStream Methods
		public override void Close () {
			if ( underlying is Stream ) ( underlying as Stream ).Close ();
			base.Close ();
		}

		protected override void Dispose ( bool disposing ) {
			if ( underlying is Q3DatagramStream ) ( underlying as Q3DatagramStream ).Dispose ( disposing );
			base.Dispose ();
		}

		public override void Flush () {
			if ( underlying is Stream ) ( underlying as Stream ).Flush ();
		}

		public override long Seek ( long offset, SeekOrigin origin ) {
			throw new NotSupportedException ();
		}

		public override void SetLength ( long value ) {
			throw new NotSupportedException ();
		}

		//public abstract int  Read  ( byte [] buffer, int offset, int count );
		//public abstract void Write ( byte [] buffer, int offset, int count );

		public virtual Int16 ReadInt16 () {
			Read ( i16buf, 0, 2 );
			return	ExBitConverter.ToInt16 ( i16buf, 0, isLittleEndian );
		}

		public virtual Int32 ReadInt32 () {
			Read ( i32buf, 0, 4 );
			return	ExBitConverter.ToInt32 ( i32buf, 0, isLittleEndian );
		}

		public virtual string ReadString () {
			return	ReadString ( textEncoding );
		}

		public virtual string ReadString ( Encoding encoding ) {
			MemoryStream ms = new MemoryStream ();
			int c = ReadByte ();
			int l = 0;

			while ( c != 0x00 && c != -1 ) {
				ms.WriteByte ( ( byte ) c );
				c = ReadByte ();
				l++;
			}

			if ( !encoding.IsSingleByte )
				ReadByte ();	// Should be also 0x00

			return	encoding.GetString ( ms.GetBuffer (), 0, l );
		}

		public void WriteInt16 ( Int16 value ) {
			Write ( ExBitConverter.GetBytes ( value, isLittleEndian ), 0, 2 );
		}

		public void WriteInt32 ( Int32 value ) {
			Write ( ExBitConverter.GetBytes ( value, isLittleEndian ), 0, 4 );
		}

		public void WriteString ( string value, Encoding encoding ) {
			Write ( encoding.GetBytes ( value ), 0, value.Length );
			WriteByte ( 0x00 );

			if ( !encoding.IsSingleByte )
				WriteByte ( 0x00 );
		}

		public void WriteString ( string value ) {
			WriteString ( value, textEncoding );
		}

		public virtual PacketKind BeginReadPacket () {
			if ( msReadBuffer != null ) msReadBuffer.SetLength ( 0 );
			bytesRead = 0;

			return	( underlying is Q3DatagramStream ) ? ( underlying as Q3DatagramStream ).BeginReadPacket () : PacketKind.Unknown;
		}

		public virtual int EndReadPacket () {
			return	( int ) msReadBuffer.Position;
		}

		public virtual void BeginWritePacket ( PacketKind packetKind ) {
			if ( msWriteBuffer != null ) msWriteBuffer.SetLength ( 0 );
			bytesWritten = 0;

			if ( underlying is Q3DatagramStream )
				( underlying as Q3DatagramStream ).BeginWritePacket ( packetKind );
			else
				writePacketKind = packetKind;
		}

		public virtual int EndWritePacket () {
			Flush ();

			return	( int ) msWriteBuffer.Position;
		}
		#endregion Q3PacketStream Methods
	}
}
