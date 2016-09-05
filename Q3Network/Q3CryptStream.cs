using System;
using System.IO;

namespace Q3Network
{
	public class Q3CryptStream : Q3DatagramStream
	{
		#region Q3CryptStream Constants
		private const int CL_ENCODE_START = 12;
		private const int CL_DECODE_START = 4;
		private const int SV_ENCODE_START = 4;
		private const int SV_DECODE_START = 12;
		#endregion Q3CryptStream Constants

		#region Q3CryptStream Properties
		private string readPattern;
		private string writePattern;
		private int readKey;
		private int writeKey;
		private int readPatternIndex;
		private int writePatternIndex;

		private int EncodeStart { get { return	connection.ConnectionFrom == ConnectionFrom.ClientSide ? CL_ENCODE_START : SV_ENCODE_START; } }
		private int DecodeStart { get { return	connection.ConnectionFrom == ConnectionFrom.ClientSide ? CL_DECODE_START : SV_DECODE_START; } }
		#endregion Q3CryptStream Properties

		#region Q3CryptStream Constructors
		public Q3CryptStream ( Q3DatagramStream stream, Q3Connection connection, FileAccess access )
		{
			#region Check Arguments
			if ( stream == null ) throw new ArgumentNullException ( "stream" );
			if ( connection == null ) throw new ArgumentNullException ( "connection" );
			#endregion Check Arguments

			this.underlying = stream;
			this.connection = connection;
			this.access = access;
		}
		#endregion Q3CryptStream Constructors

		#region Q3CryptStream Methods
		public override int Read ( byte [] buffer, int offset, int count )
		{
			if ( bytesRead < DecodeStart && bytesRead + count > DecodeStart )
				throw new InvalidOperationException ( string.Format (
					"You must read till {0} byte boundary at the first time to allow " +
					"overlying streams obtain information needed to decode data beyond boundary.", DecodeStart ) );

			int num = ( underlying as Stream ).Read ( buffer, offset, count );

			if ( bytesRead + num == DecodeStart ) {
				readKey = ( byte ) ( connection.ConnectionFrom == ConnectionFrom.ClientSide ?
					connection.Challenge ^ connection.IncomingSequence :
					connection.Challenge ^ connection.ServerId ^ connection.IncomingSequence );
				readPatternIndex = 0;
			} else {
				readPattern = connection.OutgoingReliableCommands [connection.ReliableAcknowledge & ( Q3Connection.MAX_RELIABLE_COMMANDS - 1 )];

				for ( int i = offset, j = bytesRead ; i < offset + num ; i++, j++ ) {
					// modify the key with the last received now acknowledged server command
					if ( readPattern == null || readPatternIndex >= readPattern.Length ) readPatternIndex = 0;

					if ( readPattern != null && readPattern.Length > readPatternIndex &&
						( readPattern [readPatternIndex] > 127 || readPattern [readPatternIndex] == '%' ) )
						readKey ^= ( byte ) ( '.' << ( j & 1 ) );
					else
						readKey ^= ( byte ) ( ( readPattern != null && readPatternIndex < readPattern.Length ?
							readPattern [readPatternIndex] : 0 ) << ( j & 1 ) );

					readPatternIndex++;
					buffer [i] = ( byte ) ( buffer [i] ^ readKey );
				}
			}

			bytesRead += num;

			return	num;
		}

		public override void Write ( byte [] buffer, int offset, int count )
		{
			int passthruLen = 0;

			if ( bytesWritten < EncodeStart ) {
				passthruLen = EncodeStart > bytesWritten + count ? count - bytesWritten : EncodeStart - bytesWritten;
				( underlying as Stream ).Write ( buffer, offset, passthruLen );
				bytesWritten += passthruLen;

				writePattern = connection.IncomingReliableCommands [connection.IncomingCommandSequence & ( Q3Connection.MAX_RELIABLE_COMMANDS - 1 )];
				writeKey = ( byte ) ( connection.ConnectionFrom == ConnectionFrom.ClientSide ?
					connection.Challenge ^ connection.ServerId ^ connection.IncomingSequence :
					connection.Challenge ^ connection.OutgoingSequence );
				writePatternIndex = 0;
			}

			int encLen;

			if ( 0 > ( encLen = count - passthruLen ) )
				return;

			byte [] encBuf = new byte [encLen];

			for ( int i = offset + passthruLen, j = bytesWritten ; i < offset + count ; i++, j++ ) {
				// modify the key with the last received now acknowledged server command
				if ( writePattern == null || writePatternIndex >= writePattern.Length ) writePatternIndex = 0;

				if ( writePattern != null && writePattern.Length > writePatternIndex &&
					( writePattern [writePatternIndex] > 127 || writePattern [writePatternIndex] == '%' ) )
					writeKey ^= ( byte ) ( '.' << ( j & 1 ) );
				else
					writeKey ^= ( byte ) ( ( writePattern != null && writePatternIndex < writePattern.Length ?
						writePattern [writePatternIndex] : 0 ) << ( j & 1 ) );

				writePatternIndex++;
				encBuf [i - offset - passthruLen] = ( byte ) ( buffer [i] ^ writeKey );
			}

			( underlying as Stream ).Write ( encBuf, 0, encLen );
			bytesWritten += encLen;
		}
		#endregion Q3CryptStream Methods
	}
}
