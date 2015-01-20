using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKTools;

namespace MapEditor
{
	public class Template
	{
		Texture texture;
		RectangleF uv;

		Mesh displayMesh;

		public Mesh Mesh
		{
			get
			{
				Mesh m = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.Quads);
				Vector2 size = Size / 2;

				m.Vertices = new Vector2[] {
					new Vector2(-size.X, size.Y),
					new Vector2(size.X, size.Y),
					new Vector2(size.X, -size.Y),
					new Vector2(-size.X, -size.Y)
				};

				m.UV = new Vector2[] {
					new Vector2(uv.X, uv.Y),
					new Vector2(uv.X + uv.Width, uv.Y),
					new Vector2(uv.X + uv.Width, uv.Y + uv.Height),
					new Vector2(uv.X, uv.Y + uv.Height)
				};

				m.Texture = texture;

				return m;
			}
		}

		public Vector2 Size
		{
			get
			{
				float ratio = uv.Height / uv.Width;

				if (uv.Width > uv.Height)
					return new Vector2(1f, ratio);
				else
					return new Vector2(1 / ratio, 1f);
			}
		}

		public Template(Texture texture, RectangleF uv)
		{
			this.texture = texture;
			this.uv = uv;

			displayMesh = Mesh;
		}

		public void Draw(Vector2 position)
		{
			displayMesh.Reset();

			displayMesh.Translate(position);

			displayMesh.Draw();
		}
	}
}
