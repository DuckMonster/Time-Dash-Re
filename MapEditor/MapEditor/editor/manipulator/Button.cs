using OpenTK;
using System;
using TKTools;

namespace MapEditor.Manipulators
{
	public class Button : IDisposable
	{
		Vector2 position, size;
		float rotation;

		Color color;

		protected Mesh mesh;

		public Vector2 Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}

		public Vector2 Size
		{
			get
			{
				return size;
			}
			set
			{
				size = value;
			}
		}

		public float Rotation
		{
			get
			{
				return rotation;
			}
			set
			{
				rotation = value;
			}
		}

		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}

		public bool Hovered
		{
			get
			{
				RecalculateMesh();
				return (mesh.Polygon * 2f).Intersects(new Polygon(MouseInput.Current.Position));
			}
		}

		public Button(Editor e)
		{
			mesh = Mesh.Box;
		}

		public void Dispose()
		{
			mesh.Dispose();
		}

		public void RecalculateMesh()
		{
			mesh.Reset();

			mesh.Translate(position.X, position.Y, Editor.camera.TargetBaseZ);
			mesh.Scale(size * Editor.camera.Position.Z);
			mesh.Rotate(rotation);

			Size = new Vector2(0.4f, 0.4f);
		}

		public void Logic()
		{
			RecalculateMesh();
		}

		public void Draw()
		{
			Color c = Color;
			c.A = Hovered ? 0.8f : 0.4f;

			mesh.Color = new Color(0, 0, 0, 0.5f);
			mesh.Draw();

			mesh.Color = c;
			mesh.Draw();
		}
	}
}