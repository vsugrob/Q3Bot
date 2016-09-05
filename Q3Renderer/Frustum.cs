using Microsoft.DirectX;

namespace Q3Renderer
{
	public class Frustum
	{
		#region Properties
		private Plane [] planes = new Plane [6];
		#endregion Properties

		#region Methods
		public bool isBoxInside ( Vector3 min, Vector3 max ) {
			for ( int p = 0; p < 6; p++ ) {
				if ( planes [p].Dot ( min ) >= 0.0f ) continue;
				if ( planes [p].Dot ( new Vector3 ( max.X, min.Y, min.Z ) ) >= 0.0f ) continue;
				if ( planes [p].Dot ( new Vector3 ( min.X, max.Y, min.Z ) ) >= 0.0f ) continue;
				if ( planes [p].Dot ( new Vector3 ( max.X, max.Y, min.Z ) ) >= 0.0f ) continue;
				if ( planes [p].Dot ( new Vector3 ( min.X, min.Y, max.Z ) ) >= 0.0f ) continue;
				if ( planes [p].Dot ( new Vector3 ( max.X, min.Y, max.Z ) ) >= 0.0f ) continue;
				if ( planes [p].Dot ( new Vector3 ( min.X, max.Y, max.Z ) ) >= 0.0f ) continue;
				if ( planes [p].Dot ( new Vector3 ( max.X, max.Y, max.Z ) ) >= 0.0f ) continue;

				return	false;
			}

			return	true;
		}

		public void Update ( Matrix view, Matrix projection )
		{
			Matrix fov = Matrix.Multiply ( view, projection );

			// the right plane.
			planes [0].A = fov.M14 - fov.M11;
			planes [0].B = fov.M24 - fov.M21;
			planes [0].C = fov.M34 - fov.M31;
			planes [0].D = fov.M44 - fov.M41;

			// the left plane.
			planes [1].A = fov.M14 + fov.M11;
			planes [1].B = fov.M24 + fov.M21;
			planes [1].C = fov.M34 + fov.M31;
			planes [1].D = fov.M44 + fov.M41;

			// the top plane.
			planes [2].A = fov.M14 - fov.M12;
			planes [2].B = fov.M24 - fov.M22;
			planes [2].C = fov.M34 - fov.M32;
			planes [2].D = fov.M44 - fov.M42;

			// the bottom plane.
			planes [3].A = fov.M14 + fov.M12;
			planes [3].B = fov.M24 + fov.M22;
			planes [3].C = fov.M34 + fov.M32;
			planes [3].D = fov.M44 + fov.M42;

			// the far plane.
			planes [4].A = fov.M14 - fov.M13;
			planes [4].B = fov.M24 - fov.M23;
			planes [4].C = fov.M34 - fov.M33;
			planes [4].D = fov.M44 - fov.M43;

			// the near plane
			planes [5].A = fov.M13;
			planes [5].B = fov.M23;
			planes [5].C = fov.M33;
			planes [5].D = fov.M43;

			// Normalize the planes.
			for ( int i = 0 ; i < planes.Length ; i++ )
				planes [i].Normalize ();
		}
		#endregion Methods
	}
}
