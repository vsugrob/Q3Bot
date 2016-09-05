using System;

namespace Q3Network
{
	public struct NetField {
		public const int FLOAT_INT_BITS = 13;
		public const int FLOAT_INT_BIAS = 1 << FLOAT_INT_BITS - 1;
		public string name;
		public int offset;	// This is not needed if we're only parse server packets only on client side.
		public uint bits;

		public NetField ( string name, int offset, uint bits ) {
			this.name = name;
			this.offset = offset;
			this.bits = bits;
		}
	}
}
