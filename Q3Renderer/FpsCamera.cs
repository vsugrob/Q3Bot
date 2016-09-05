using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;

namespace Q3Renderer
{
	public class FpsCamera
	{
		#region Properties
		private Input input;
		private float aspect;
		private float fov;
		private float nearZ;
		private float farZ;
		private Vector3 pos   = new Vector3 ( 0.0f, 0.0f, 0.0f );
		private Vector3 right = new Vector3 ( 1.0f, 0.0f, 0.0f );
		private Vector3 up    = new Vector3 ( 0.0f, 1.0f, 0.0f );
		private Vector3 look  = new Vector3 ( 0.0f, 0.0f, 1.0f );
		private Matrix viewMatrix = Matrix.Identity;
		private Matrix projMatrix = Matrix.Identity;
		private Matrix viewProjMatrix = Matrix.Identity;
		private Frustum viewFrustum = new Frustum ();
		private float moveSpeed = 400.0f;
		private float sensitivity = 5.0f;
		private float fovSens = 1.0f;
		private const float SENS_DIVIDER = 2000.0f;
		private float walkSpeedDivider = 2.0f;

		public Input Input { get { return	input; } }
		public float Aspect {
			get { return	aspect; }
			set {
				aspect = value;
				SetupLens ();
			}
		}

		public float Fov {
			get { return	fov; }
			set {
				fov = value;
				SetupLens ();
			}
		}

		public float NearZ {
			get { return	nearZ; }
			set {
				nearZ = value;
				SetupLens ();
			}
		}

		public float FarZ {
			get { return	farZ; }
			set {
				farZ = value;
				SetupLens ();
			}
		}

		public Vector3 Position {
			get { return	pos; }
			set {
				pos = value;
				BuildViewMatrix ();
			}
		}

		public Vector3 Look {
			get { return	look; }
			set {
				look = value;
				BuildViewMatrix ();
			}
		}

		public Vector3 Right {
			get { return	right; }
			set {
				right = value;
				BuildViewMatrix ();
			}
		}

		public Vector3 Up {
			get { return	up; }
			set {
				up = value;
				BuildViewMatrix ();
			}
		}

		public Matrix ViewMatrix { get { return	viewMatrix; } }
		public Matrix ProjMatrix { get { return	projMatrix; } }

		public float MoveSpeed {
			get { return	moveSpeed; }
			set { moveSpeed = value; }
		}

		public float Sensitivity {
			get { return	sensitivity; }
			set { sensitivity = value; }
		}

		public Frustum ViewFrustum { get { return	viewFrustum; } }
		public Matrix ViewProjMatrix { get { return	viewProjMatrix; } }
		#endregion Properties

		#region Constructors
		public FpsCamera ( Input input, float aspect, float fov, float nearZ, float farZ ) {
			this.aspect = aspect;
			this.input  = input;
			this.fov    = fov;
			this.nearZ  = nearZ;
			this.farZ   = farZ;

			BuildViewMatrix ();
			SetupLens ();
		}
		#endregion Constructors

		#region Methods
		public void Reset () {
			pos   = new Vector3 ( 0.0f, 0.0f, 0.0f );
			right = new Vector3 ( 1.0f, 0.0f, 0.0f );
			up    = new Vector3 ( 0.0f, 1.0f, 0.0f );
			look  = new Vector3 ( 0.0f, 0.0f, 1.0f );
			viewMatrix = Matrix.Identity;
			projMatrix = Matrix.Identity;
			viewProjMatrix = Matrix.Identity;
			viewFrustum = new Frustum ();

			BuildViewMatrix ();
			SetupLens ();
		}

		public void SetupLens () {
			projMatrix = Matrix.PerspectiveFovLH ( fov, aspect, nearZ, farZ );
		}

		public void Update ( float delta ) {
			Vector3 moveDir = new Vector3 ( 0.0f, 0.0f, 0.0f );

			bool walk = false;

			if ( input.KeyDown ( Key.E ) )	moveDir += look;
			if ( input.KeyDown ( Key.D ) )	moveDir -= look;
			if ( input.KeyDown ( Key.F ) )	moveDir += right;
			if ( input.KeyDown ( Key.S ) )	moveDir -= right;
			if ( input.KeyDown ( Key.Space ) )	moveDir += up;
			if ( input.KeyDown ( Key.Z ) )	walk = true;
			//if ( input.MouseButtonDown ( Input.MouseButton.Left  ) )	Fov = fov + delta * fovSens;
			//if ( input.MouseButtonDown ( Input.MouseButton.Right ) )	Fov = fov - delta * fovSens;

			moveDir.Normalize ();
			pos += moveDir * ( walk ? moveSpeed / walkSpeedDivider : moveSpeed ) * delta;
			
			float dX = input.MouseDeltaY * sensitivity / SENS_DIVIDER;
			float dY = input.MouseDeltaX * sensitivity / SENS_DIVIDER;

			Matrix rotMatrix = Matrix.RotationAxis ( right, dX );
			look.TransformCoordinate ( rotMatrix );
			up.TransformCoordinate ( rotMatrix );

			rotMatrix = Matrix.RotationY ( dY );
			right.TransformCoordinate ( rotMatrix );
			up.TransformCoordinate ( rotMatrix );
			look.TransformCoordinate ( rotMatrix );

			BuildViewMatrix ();
			viewFrustum.Update ( viewMatrix, projMatrix );
			viewProjMatrix = viewMatrix * projMatrix;
		}

		private void BuildViewMatrix () {
			look.Normalize ();

			up = Vector3.Cross ( look, right );
			up.Normalize ();

			right = Vector3.Cross ( up, look );
			right.Normalize ();

			float x = -Vector3.Dot ( pos, right );
			float y = -Vector3.Dot ( pos, up    );
			float z = -Vector3.Dot ( pos, look  );

			viewMatrix.M11 = right.X;
			viewMatrix.M21 = right.Y;
			viewMatrix.M31 = right.Z;
			viewMatrix.M41 = x;

			viewMatrix.M12 = up.X;
			viewMatrix.M22 = up.Y;
			viewMatrix.M32 = up.Z;
			viewMatrix.M42 = y;

			viewMatrix.M13 = look.X;
			viewMatrix.M23 = look.Y;
			viewMatrix.M33 = look.Z;
			viewMatrix.M43 = z;

			viewMatrix.M14 = 0.0f;
			viewMatrix.M24 = 0.0f;
			viewMatrix.M34 = 0.0f;
			viewMatrix.M44 = 1.0f;
		}
		#endregion Methods
	}
}
