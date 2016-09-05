namespace Q3Renderer
{
	public class Q3BezierPatch
	{
		#region Properties
		public Q3BspVertex [] controls = new Q3BspVertex [9];
		public Q3BspVertex [] vertices;
		public uint [] indices;
		public uint [] trisPerRow;
		public uint [] rowIndices;
		public int baseBufferIndex;
		public int baseVertexIndex;
		#endregion Properties

		#region Methods
		public void Tessellate ( int level ) {
			int l1 = level + 1;

			vertices = new Q3BspVertex [l1 * l1];

			for ( int i = 0 ; i <= level ; ++i ) {
				float a = ( float ) i / level;
				float b = 1.0f - a;

				vertices [i] = controls [0] * ( b * b ) + 
							   controls [3] * ( 2 * b * a ) +
							   controls [6] * ( a * a );
			}

			for ( int i = 1 ; i <= level ; i++ )
			{
				float a = ( float ) i / level;
				float b = 1.0f - a;

				Q3BspVertex [] temp = new Q3BspVertex [3];

				for ( int j = 0 ; j < 3 ; j++ ) {
					int k = 3 * j;

					temp [j] = controls [k + 0] * ( b * b ) + 
							   controls [k + 1] * ( 2 * b * a ) +
							   controls [k + 2] * ( a * a );
				}

				for ( int j = 0 ; j <= level ; ++j ) {
					a = ( float ) j / level;
					b = 1.0f - a;

					vertices [i * l1 + j] = temp [0] * ( b * b ) + 
											temp [1] * ( 2 * b * a ) +
											temp [2] * ( a * a );
				}
			}

			indices = new uint [level * ( level + 1 ) * 2];

			for (int row = 0 ; row < level ; ++row ) {
				for ( int col = 0 ; col <= level ; ++col ) {
					indices [( row * ( level + 1 ) + col ) * 2 + 1] = ( uint ) ( row         * l1 + col );
					indices [( row * ( level + 1 ) + col ) * 2    ] = ( uint ) ( ( row + 1 ) * l1 + col );
				}
			}

			trisPerRow = new uint [level];
			rowIndices = new uint [level];

			for ( int row = 0 ; row < level ; ++row ) {
				trisPerRow [row] = ( uint ) ( 2 * l1 );
				rowIndices [row] = ( uint ) ( row * 2 * l1 );
			}

			for ( int i = 0 ; i < l1 * l1 ; i++ )
				vertices [i].Normalize ();
		}
		#endregion Methods
	}
}
