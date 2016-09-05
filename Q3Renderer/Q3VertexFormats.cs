using System.Runtime.InteropServices;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Q3Renderer
{
	public class Q3VertexFormats
	{
		[StructLayout ( LayoutKind.Sequential, Pack=1 )]
		public struct PositionNormalTexturedLightened
		{
			public const VertexFormats Format = VertexFormats.Position | VertexFormats.Normal | VertexFormats.Texture0 | VertexFormats.Texture1;
			public Vector3 p;
			public Vector3 n;
			public Vector2 t0;
			public Vector2 t1;

			public PositionNormalTexturedLightened ( Vector3 position, Vector3 normal, Vector2 texture0, Vector2 texture1 ) {
				p = position;
				n = normal;
				t0 = texture0;
				t1 = texture1;
			}

			public Vector3 Position {
				get { return	p; }
				set { p = value; }
			}

			public Vector3 Normal {
				get { return	n; }
				set { n = value; }
			}

			public Vector2 Texture0 {
				get { return	t0; }
				set { t0 = value; }
			}

			public Vector2 Texture1 {
				get { return	t1; }
				set { t1 = value; }
			}

			public static int StrideSize { get { return	40; } }

			public static VertexDeclaration Declaration ( Device d3dDevice ) {
				VertexElement [] velements = new VertexElement[] 
				{
					new VertexElement ( 0,  0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0 ),
					new VertexElement ( 0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0 ),
					new VertexElement ( 0, 24, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0 ),
					new VertexElement ( 0, 32, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 1 ),
					VertexElement.VertexDeclarationEnd
				};

				return	new VertexDeclaration ( d3dDevice, velements );
			}

			public override string ToString () {
				return	string.Format ( "p({0}, {1}, {2}), n({0}, {1}, {2}), t0({0}, {1}), t1({0}, {1})",
					p.X, p.Y, p.Z, n.X, n.Y, n.Z, t0.X, t0.Y, t1.X, t1.Y );
			}
		}
	}
}
