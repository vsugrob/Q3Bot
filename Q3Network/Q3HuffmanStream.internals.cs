using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

namespace Q3Network
{
	partial class Q3HuffmanStream
	{
		#region Constants
		private const int HMAX = 256;
		private const int NYT  = HMAX;
		private const int INTERNAL_NODE = HMAX + 1;
		#endregion Constants
		
		// NOTE: must be private
		public class Node {
			public Node left, right, parent;
			public Node next, prev;
			public int head = -1;
			public int weight;
			public int symbol;
		}

		#region Properties
		private Stream baseStream;
		private CompressionMode mode;
		private bool leaveOpen;

		private int blocNode;
		private int blocPtrs;

		private Node tree;
		private Node lhead;
		private Node ltail;
		private Node [] loc = new Node [HMAX + 1];
		private int freelist = -1;
		private Node [] nodeList = new Node [768];
		private List <Node> nodePtrs = new List <Node> ( 768 );
		private int [] nodePPtrs = new int [768];
		private int bloc;

		public int BitLocation {
			get { return	bloc; }
		}
		#endregion Properties

		private void Init () {
			for ( int i = 0 ; i < nodeList.Length ; i++ )
				nodeList [i] = new Node ();

			for ( int i = 0 ; i < nodePtrs.Capacity ; i++ )
				nodePtrs.Add ( null );

			for ( int i = 0 ; i < nodePPtrs.Length ; i++ )
				nodePPtrs [i] = -1;

			tree = lhead = loc [NYT] = nodeList [blocNode++];
			tree.symbol = NYT;

			if ( mode == CompressionMode.Compress )
				writeBuffer = new byte [WRITE_BUFFER_INIT_SIZE];
			else if ( mode == CompressionMode.Decompress )
				readBuffer = new byte [READ_BUFFER_SIZE];
		}

		private void AddBit ( byte bit ) {
			if ( ( bloc & 7 ) == 0 )
				writeBuffer [( bloc >> 3 )] = 0;

			writeBuffer [( bloc >> 3 )] |= ( byte ) ( bit << ( bloc & 7 ) );
			IncWriteBloc ();
		}

		private int GetBit () {
			int t;

			t = ( readBuffer [( bloc >> 3 )] >> ( bloc & 7 ) ) & 0x01;
			IncReadBloc ();

			return	t;
		}

		private int GetBitFromBuffer ( byte [] fin ) {
			int t;

			t = ( fin [( bloc >> 3 )] >> ( bloc & 7 ) ) & 0x01;
			bloc++;

			return	t;
		}

		private void SetPPNode ( Node node, int id ) {
			nodePtrs [id] = node;
			nodePPtrs [id] = -1;
		}

		private int GetPPNode () {
			if ( freelist == -1 ) {
				return	blocPtrs++;
			} else {
				int ppnode = freelist;

				if ( nodePPtrs [ppnode] != -1 )
					freelist = nodePPtrs [ppnode];
				else
					freelist = -1;

				/*if ( nodePtrs [freelist] != null && nodePtrs [freelist].left != null ) {
					int idx = nodePtrs.IndexOf ( nodePtrs [freelist].left );

					//if ( idx != -1 )
						freelist = idx;
					//else
					//	freelist = 0;
				} else
					freelist = -1;*/

				//freelist = nodePtrs [freelist] != null ?
				//	( nodePtrs [freelist].left != null ? nodePtrs.IndexOf ( nodePtrs [freelist].left ) : 0 ) : 0;

				//tppnode = huff->freelist;
				//huff->freelist = (node_t **)*tppnode;
				//return tppnode;
				return	ppnode;
			}
		}

		private void FreePPNode ( int id ) {
			if ( freelist == -1 )
				nodePtrs [id] = null;
			else
				nodePPtrs [id] = freelist;

			freelist = id;

			//nodePtrs [id] = freelist != -1 ? nodePtrs [freelist] : null;
			//freelist = id;

			//*ppnode = (node_t *)huff->freelist;
			//huff->freelist = ppnode;
		}

		/* Swap the location of these two nodes in the tree */
		private void Swap ( Node node1, Node node2 ) {
			Node par1, par2;

			par1 = node1.parent;
			par2 = node2.parent;

			if ( par1 != null ) {
				if ( par1.left == node1 )
					par1.left = node2;
				else
				  par1.right = node2;
			} else {
				tree = node2;
			}

			if ( par2 != null ) {
				if ( par2.left == node2 )
					par2.left = node1;
				else
					par2.right = node1;
			} else {
				tree = node1;
			}
			
			node1.parent = par2;
			node2.parent = par1;
		}

		/* Swap these two nodes in the linked list (update ranks) */
		private void swaplist ( Node node1, Node node2 ) {
			Node par1;

			par1 = node1.next;
			node1.next = node2.next;
			node2.next = par1;

			par1 = node1.prev;
			node1.prev = node2.prev;
			node2.prev = par1;

			if ( node1.next == node1 )
				node1.next = node2;
			
			if ( node2.next == node2 )
				node2.next = node1;
			
			if ( node1.next != null )
				node1.next.prev = node1;
			
			if ( node2.next != null )
				node2.next.prev = node2;
			
			if ( node1.prev != null )
				node1.prev.next = node1;
			
			if ( node2.prev != null )
				node2.prev.next = node2;
		}

		/* Do the increments */
		private void increment ( Node node ) {
			Node lnode;

			if ( node == null )
				return;

			if ( node.next != null && node.next.weight == node.weight ) {
				lnode = nodePtrs [node.head];

				if ( lnode != node.parent )
					Swap ( lnode, node );
				
				swaplist ( lnode, node );
			}

			if ( node.prev != null && node.prev.weight == node.weight ) {
				nodePtrs [node.head] = node.prev;
			} else {
				SetPPNode ( null, node.head );
				FreePPNode ( node.head );
			}

			node.weight++;

			if ( node.next != null && node.next.weight == node.weight ) {
				node.head = node.next.head;
			} else {
				node.head = GetPPNode ();
				SetPPNode ( node, node.head );
			}

			if ( node.parent != null ) {
				increment( node.parent );

				if ( node.prev == node.parent ) {
					swaplist ( node, node.parent );

					if ( nodePtrs [node.head] == node ) {
						nodePtrs [node.head] = node.parent;
					}
				}
			}
		}

		public void AddRef ( byte ch ) {
			Node tnode, tnode2;

			if ( loc [ch] == null ) { // if this is the first transmission of this node
				tnode  = nodeList [blocNode++];
				tnode2 = nodeList [blocNode++];

				tnode2.symbol = INTERNAL_NODE;
				tnode2.weight = 1;
				tnode2.next = lhead.next;

				if ( lhead.next != null ) {
					lhead.next.prev = tnode2;

					if ( lhead.next.weight == 1 ) {
						tnode2.head = lhead.next.head;
					} else {
						tnode2.head = GetPPNode ();
						SetPPNode ( tnode2, tnode2.head );
					}
				} else {
					tnode2.head = GetPPNode ();
					SetPPNode ( tnode2, tnode2.head );
				}

				lhead.next = tnode2;
				tnode2.prev = lhead;
		 
				tnode.symbol = ch;
				tnode.weight = 1;
				tnode.next = lhead.next;

				if ( lhead.next != null ) {
					lhead.next.prev = tnode;

					if ( lhead.next.weight == 1 ) {
						tnode.head = lhead.next.head;
					} else {
						// this should never happen
						tnode.head = GetPPNode ();
						SetPPNode ( tnode2, tnode.head );
					}
				} else {
					// this should never happen
					tnode.head = GetPPNode ();
					SetPPNode ( tnode2, tnode.head );
				}

				lhead.next = tnode;
				tnode.prev = lhead;
				tnode.left = tnode.right = null;
		 
				if ( lhead.parent != null ) {
					if ( lhead.parent.left == lhead ) // lhead is guaranteed to by the NYT
						lhead.parent.left = tnode2;
					else
						lhead.parent.right = tnode2;
				} else {
					tree = tnode2;
				}
				
				tnode2.right = tnode;
				tnode2.left  = lhead;
		 
				tnode2.parent = lhead.parent;
				lhead.parent  = tnode.parent = tnode2;
				
				loc [ch] = tnode;
				
				increment ( tnode2.parent );
			} else {
				increment ( loc [ch] );
			}
		}

		/* Get a symbol */
		private int Receive () {
			Node node = tree;

			while ( node != null && node.symbol == INTERNAL_NODE ) {
				if ( GetBit () != 0 )
					node = node.right;
				else
					node = node.left;
			}

			if ( node == null )
				return	0;

			return	node.symbol;
		}

		private int ReceiveFromBuffer ( byte [] fin ) {
			Node node = tree;

			while ( node != null && node.symbol == INTERNAL_NODE ) {
				if ( GetBitFromBuffer ( fin ) != 0 )
					node = node.right;
				else
					node = node.left;
			}

			if ( node == null )
				return	0;

			return	node.symbol;
		}
		
		private void Send ( Node node, Node child ) {
			if ( node.parent != null )
				Send ( node.parent, node );

			if ( child != null ) {
				if ( node.right == child )
					AddBit ( 1 );
				else
					AddBit ( 0 );
			}
		}

		/* Send a symbol */
		private void Transmit ( int ch ) {
			if ( loc [ch] == null ) {
				/* node_t hasn't been transmitted, send a NYT, then the symbol */
				Transmit ( NYT );

				for ( int i = 7 ; i >= 0 ; i-- )
					AddBit ( ( byte ) ( ( ch >> i ) & 0x01 ) );
			} else {
				Send ( loc [ch], null );
			}
		}

		private void IncReadBloc () {
			bloc++;

			if ( bloc >= lastBytesReadFromUnderlying << 3 ) {
				// Включаем подсос x_X
				bloc = 0;
				PumpReadBuffer ();
			}
		}

		private void PumpReadBuffer () {
			bytesReadFromUnderlying += lastBytesReadFromUnderlying;

			if ( lastBytesReadFromUnderlying != 0 )
				lastBytesReadFromUnderlying = baseStream.Read ( readBuffer, 0, READ_BUFFER_SIZE );
			else if ( baseStream is Q3DatagramStream ) {
				( baseStream as Q3DatagramStream ).BeginReadPacket ();
				lastBytesReadFromUnderlying = ( baseStream as Q3DatagramStream ).Read ( readBuffer, 0, 4 );
			} else if ( baseStream is Q3DemoStream )
				lastBytesReadFromUnderlying = ( ( Q3DemoStream ) baseStream ).ReadNew ( readBuffer, 0, READ_BUFFER_SIZE );
			else
				lastBytesReadFromUnderlying = baseStream.Read ( readBuffer, 0, READ_BUFFER_SIZE );
		}

		private void IncWriteBloc () {
			bloc++;

			if ( ( bloc >> 3 ) >= writeBuffer.Length ) {
				byte [] newBuf = new byte [writeBuffer.Length + WRITE_BUFFER_GROWTH];
				Array.Copy ( writeBuffer, newBuf, writeBuffer.Length );
				writeBuffer = newBuf;
			}
		}
	}
}
