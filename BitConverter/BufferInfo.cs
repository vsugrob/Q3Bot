using System;
using System.Text;

namespace Utils
{
	public static class BufferInfo
	{
		public static string DumpBuffer ( string name, byte [] buffer, int offset, int count, bool offsets, bool bytes, bool text ) {
			string dump = string.Format ( "Buffer \"{0}\" [0x{1:x4}-0x{2:x4}], len={3}\r\n",
										name, offset, offset + count, count );
			string b = "",
				   t = "";

			for ( int i = offset ; i < offset + count ; i++ ) {
				b += string.Format ( "0x{0:x2} ", buffer [i] );
				t += ( buffer [i] < ( byte ) '.' || 
					buffer [i] > ( byte ) 'z' ) ? '.' : ( char ) buffer [i];

				if ( offsets && i % 8 == 0 )
					dump += string.Format ( "0x{0:x4}\t", i );

				if ( ( ( i + 1 ) % 8 ) == 0 ) {
					if ( bytes ) { dump += b; b = ""; }
					if ( text  ) { dump += "\t" + t; t = ""; }
					dump += "\r\n";
				} else if ( ( ( i + 1 ) % 4 ) == 0 ) {
					if ( bytes ) b += ' ';
					if ( text  ) t += ' ';
				}
			}

			if ( bytes ) { dump += b; b = ""; }
			if ( text  ) { dump += "\t" + t; t = ""; }

			dump += "\r\n\r\n";

			return	dump;
		}
	}
}
