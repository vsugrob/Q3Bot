using System;
using System.Collections.Generic;

namespace Q3Network
{
	partial class Q3HuffmanStream
	{
		#region Debug properties
		public Node [] Loc {
			get { return	( Node [] ) this.loc.Clone (); }
		}

		public int BlocPtrs {
			get { return	this.blocPtrs; }
		}

		public int BlocNode {
			get { return	this.blocNode; }
		}

		public int Freelist {
			get { return	this.freelist; }
		}
		#endregion Debug properties
	}
}
