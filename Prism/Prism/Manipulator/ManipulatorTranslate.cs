using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;
using TKTools.Mathematics;

public class ManipulatorTranslate : Manipulator
{
	class Tool
	{
		public enum Axis
		{
			None,
			X,
			Y,
			Plane
		}

		ManipulatorTranslate manipulator;
		Model2D xAxis, yAxis, plane;
		MouseWatch mouse = Editor.mouse;

		public Axis HoveredAxis
		{
			get
			{
				if (xAxis.Polygon.Intersects(mouse.Position))
					return Axis.X;
				else if (yAxis.Polygon.Intersects(mouse.Position))
					return Axis.Y;
				else if (plane.Polygon.Intersects(mouse.Position))
					return Axis.Plane;

				return Axis.None;
			}
		}
		public Axis ActiveAxis
		{
			get
			{
				return activeAxis;
			}
		}
		public Vector2 TranslateVector
		{
			get
			{
				Vector2 delta = mouse.Position.Xy - origin.Value;

				Vector2 x = Vector2.Dot(delta, manipulator.NormalX) * manipulator.NormalX;
				Vector2 y = Vector2.Dot(delta, manipulator.NormalY) * manipulator.NormalY;

				switch(ActiveAxis)
				{
					case Axis.X: return x;
					case Axis.Y: return y;
					case Axis.Plane: return x + y;
					default: return Vector2.Zero;
				}
			}
		}

		Matrix4 ModelMatrix
		{
			get
			{
				Vector2 p = manipulator.Position;

				float cos = (float)Math.Cos(TKMath.ToRadians(TKMath.GetAngle(manipulator.NormalX)));
				float sin = (float)Math.Sin(TKMath.ToRadians(TKMath.GetAngle(manipulator.NormalX)));
				float scale = 0.04f * CameraControl.Position.Z;

				return new Matrix4(
					scale * cos, scale * sin, 0f, 0f,
					scale * -sin, scale * cos, 0f, 0f,
					0f, 0f, 1f, 0f,
					p.X, p.Y, 0f, 1f
					);
			}
		}

		Axis activeAxis = Axis.None;
		Vector2? origin = null;

		public Tool(ManipulatorTranslate mt)
		{
			manipulator = mt;

			xAxis = new Model2D(new Vector2[] {
				new Vector2(1f, 0.4f),
				new Vector2(1f, -0.4f),
				new Vector2(2f, 0f)
			});

			yAxis = new Model2D(new Vector2[] {
				new Vector2(-0.4f, 1f),
				new Vector2(0.4f, 1f),
				new Vector2(0f, 2f)
			});

			plane = new Model2D(new Vector2[] {
				new Vector2(-0.4f, -0.4f),
				new Vector2(0.4f, -0.4f),
				new Vector2(0.4f, 0.4f),
				new Vector2(-0.4f, 0.4f)
			});
		}

		Color GetAxisColor(Axis a)
		{
			Color baseColor;
			switch(a)
			{
				case Axis.Plane: baseColor = Color.Yellow;break;
				case Axis.X: baseColor = Color.Green;break;
				case Axis.Y:baseColor = Color.Red; break;
				default:baseColor = new Color(0, 0, 0, 0); break;
			}

			bool hovered = HoveredAxis == a;

			if (ActiveAxis == a) return new Color(1f, 1f, 1f, 0.8f);
			else return baseColor * new Color(1f, 1f, 1f, hovered ? 0.8f : 0f);
		}

		public void Logic()
		{
			if (HoveredAxis != Axis.None && mouse.ButtonPressed(MouseButton.Left))
			{
				activeAxis = HoveredAxis;
				origin = mouse.Position.Xy;
				manipulator.BakeVertexPosition();
			}

			if (!mouse[MouseButton.Left] && activeAxis != Axis.None)
			{
				activeAxis = Axis.None;
				origin = null;
			}
		}

		public void Draw()
		{
			Matrix4 mat = ModelMatrix;
			xAxis.ModelMatrix = mat;
			yAxis.ModelMatrix = mat;
			plane.ModelMatrix = mat;

			xAxis.Color = Color.Green;
			yAxis.Color = Color.Red;
			plane.Color = Color.Yellow;

			xAxis.Draw(PrimitiveType.LineLoop);
			yAxis.Draw(PrimitiveType.LineLoop);
			plane.Draw(PrimitiveType.LineLoop);

			xAxis.Color = GetAxisColor(Axis.X);
			yAxis.Color = GetAxisColor(Axis.Y);
			plane.Color = GetAxisColor(Axis.Plane);

			xAxis.Draw();
			yAxis.Draw();
			plane.Draw();
		}
	}

	Tool tool;
	Vector2[] vertexOriginPosition;

	public override bool Hovered
	{
		get
		{
			if (!Visible) return false;
			return tool.HoveredAxis != Tool.Axis.None;
		}
	}

	public override bool Active
	{
		get
		{
			return tool.ActiveAxis != Tool.Axis.None;
		}
	}

	public ManipulatorTranslate(Editor e)
		:base(e)
	{
		tool = new Tool(this);
	}

	public override void Logic()
	{
		if (!Visible) return;

		if (Active)
		{
			Manipulate(tool.TranslateVector);
		}
		tool.Logic();
	}

	protected void BakeVertexPosition()
	{
		vertexOriginPosition = new Vector2[editor.SelectedVertices.Count];
		for (int i = 0; i < editor.SelectedVertices.Count; i++)
			vertexOriginPosition[i] = editor.SelectedVertices[i].Position;
	}

	void Manipulate(Vector2 delta)
	{
		for (int i = 0; i < editor.SelectedVertices.Count; i++)
			editor.SelectedVertices[i].Position = vertexOriginPosition[i] + delta;
	}

	public override void Draw()
	{
		if (!Visible) return;
		tool.Draw();
	}
}