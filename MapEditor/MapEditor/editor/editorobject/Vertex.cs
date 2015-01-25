using OpenTK;
using OpenTK.Graphics.OpenGL;
using TKTools;

namespace MapEditor
{
	public class Vertex : System.IDisposable
	{
		static float hoverRange = 0.2f;

		Editor editor;
		Vector2 position;
		Mesh mesh;

		Vector2 uv;

		public bool Hovered
		{
			get
			{
				if (editor.Paused) return false;
				return (MouseInput.Current.Position - position).LengthFast <= hoverRange;
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

		public Vector2 UV
		{
			get
			{
				return uv;
			}
			set
			{
				uv = value;
			}
		}

		public bool Selected
		{
			get
			{
				return editor.selectedList.Contains(this);
			}
		}

		public Vertex(Vector2 position, Vector2 UV, Editor e)
		{
			editor = e;
			this.position = position;
			this.UV = UV;
			mesh = Mesh.Box;
		}

		public void Dispose()
		{
			mesh.Dispose();
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

			mesh.Translate(position.X, position.Y, Editor.camera.TargetBaseZ);
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

			mesh.Translate(position, Editor.camera.TargetBaseZ);
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