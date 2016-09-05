using System;

namespace Q3Network
{
	public struct OutPacket
	{
		public int cmdNumber;		// cl.cmdNumber when packet was sent
		public int serverTime;		// usercmd->serverTime when packet was sent
		public int realtime;		// cls.realtime when packet was sent
	}
}
