using OpenTK;
using OpenTK.Graphics.OpenGL;
using TKTools;

namespace MapEditor
{
	public class Vertex
	{
		Editor editor;
		Vector2 position;
		Mesh mesh;

		public bool Hovered
		{
			get
			{
				return (mesh.Polygon * 2).Intersects(new Polygon(MouseInput.Current.Position));
			}
		}

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

		public bool Selected
		{
			get
			{
				return editor.selectedList.Contains(this);
			}
		}

		public Vertex(Vector2 position, Editor e)
		{
			editor = e;
			this.position = position;
			mesh = Mesh.Box;
		}

		public void Move(Vector2 delta)
		{
			position += delta;
		}

		public void MoveTo(Vector2 position)
		{
			this.position = position;
		}

		public void Scale(Vector2 origin, float scale)
		{
			Vector2 diff = (position - origin) * scale;
			position = origin + diff;
		}

		public void Scale(Vector2 origin, Vector2 scale)
		{
			Vector2 diff = (position - origin) * scale;
			position = origin + diff;
		}

		public void ScaleTo(Vector2 origin, Vector2 reference, float scale)
		{
			position = origin + reference * scale;
		}

		public void ScaleTo(Vector2 origin, Vector2 reference, Vector2 scale)
		{
			position = origin + reference * scale;
		}

		public void Rotate(Vector2 origin, float delta)
		{
			float currentAngle = TKMath.GetAngle(position - origin) + delta;
			RotateTo(origin, currentAngle);
		}

		public void RotateTo(Vector2 origin, float angle)
		{
			float distance = (position - origin).Length;
			position = origin + TKMath.GetAngleVector(angle) * distance;
		}

		public void Logic()
		{
		}

		public void Draw()
		{
			mesh.Color = new Color(1, 1, 1, Hovered ? 0.5f : 0.2f);

			mesh.Reset();

			mesh.Translate(position);
			mesh.Scale(0.2f);

			mesh.Draw();

			if (Selected) DrawSelectedBox();
		}

		public void DrawSelectedBox()
		{
			mesh.Color = Color.Yellow;

			GL.Enable(EnableCap.StencilTest);
			GL.Clear(ClearBufferMask.StencilBufferBit);

			GL.StencilFunc(StencilFunction.Never, 1, 0xff);
			GL.StencilOp(StencilOp.Replace, StencilOp.Keep, StencilOp.Keep);

			mesh.Reset();

			mesh.Translate(position);
			mesh.Scale(0.3f);

			mesh.Draw();

			GL.StencilFunc(StencilFunction.Notequal, 1, 0xff);

			mesh.Scale(1.1f);
			mesh.Draw();

			GL.Disable(EnableCap.StencilTest);
		}

		public static implicit operator Vector2(Vertex v)
		{
			return v.position;
		}

		public static Vector2 GetNormal(Vector2 a, Vector2 b)
		{
			return (b - a).Normalized().PerpendicularLeft;
		}

		public static Vector2 Project(Vector2 vec, Vector2 normal)
		{
			normal.Normalize();

			return Vector2.Dot(vec, normal) * normal;
		}
	}
}