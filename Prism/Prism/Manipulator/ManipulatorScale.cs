using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;
using TKTools.Mathematics;

public class ManipulatorScale : Manipulator
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

		ManipulatorScale manipulator;
		Mesh xAxis, yAxis, plane;
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
				return scaleAxis;
			}
		}

		Matrix4 ModelMatrix
		{
			get
			{
				Vector2 nx = manipulator.scaleNormalX == null ? manipulator.NormalX : manipulator.scaleNormalX.Value;

				Vector2 p = manipulator.Position;

				float cos = (float)Math.Cos(TKMath.ToRadians(TKMath.GetAngle(nx)));
				float sin = (float)Math.Sin(TKMath.ToRadians(TKMath.GetAngle(nx)));
				float scale = 0.04f * CameraControl.Position.Z;

				return new Matrix4(
					scale * cos, scale * sin, 0f, 0f,
					scale * -sin, scale * cos, 0f, 0f,
					0f, 0f, 1f, 0f,
					p.X, p.Y, 0f, 1f
					);
			}
		}

		public Vector2 ScaleFactor
		{
			get
			{
				if (scaleOrigin == null) return Vector2.One;
				else
				{
					Vector2 delta = mouse.Position.Xy - manipulator.Position;

					DebugForm.debugString = delta.ToString();

					float x = Vector2.Dot(delta, manipulator.NormalX) / Vector2.Dot(scaleOrigin.Value, manipulator.NormalX);
					float y = Vector2.Dot(delta, manipulator.NormalY) / Vector2.Dot(scaleOrigin.Value, manipulator.NormalY);

					switch (scaleAxis)
					{
						case Axis.X: return new Vector2(x, 1);
						case Axis.Y: return new Vector2(1, y);
						case Axis.Plane:
							float s = ((mouse.Position.X - (scaleOrigin.Value.X + manipulator.Position.X)) + 1) / 1f;
							return new Vector2(s, s);

						default:
							return Vector2.One;
					}
				}
			}
		}

		Vector2? scaleOrigin;
		Axis scaleAxis = Axis.None;

		public Tool(ManipulatorScale mt)
		{
			manipulator = mt;

			xAxis = new Mesh(new Vector2[] {
				new Vector2(1.4f, 0.3f),
				new Vector2(1.4f, -0.3f),
				new Vector2(2f, -0.3f),
				new Vector2(2f, 0.3f)
			});

			yAxis = new Mesh(new Vector2[] {
				new Vector2(-0.3f, 1.4f),
				new Vector2(0.3f, 1.4f),
				new Vector2(0.3f, 2f),
				new Vector2(-0.3f, 2f)
			});

			plane = new Mesh(new Vector2[] {
				new Vector2(-0.4f, -0.4f),
				new Vector2(0.4f, -0.4f),
				new Vector2(0.4f, 0.4f),
				new Vector2(-0.4f, 0.4f)
			});
		}

		Color GetAxisColor(Axis a)
		{
			Color baseColor;
			switch (a)
			{
				case Axis.Plane: baseColor = Color.Yellow; break;
				case Axis.X: baseColor = Color.Green; break;
				case Axis.Y: baseColor = Color.Red; break;
				default: baseColor = new Color(0, 0, 0, 0); break;
			}

			bool hovered = HoveredAxis == a;

			if (ActiveAxis == a) return new Color(1f, 1f, 1f, 0.8f);
			else return baseColor * new Color(1f, 1f, 1f, hovered ? 0.8f : 0f);
		}

		public void Logic()
		{
			if (scaleOrigin == null && mouse.ButtonPressed(MouseButton.Left) && HoveredAxis != Axis.None)
			{
				scaleOrigin = mouse.Position.Xy - manipulator.Position;
				scaleAxis = HoveredAxis;
				manipulator.BakeVertexPosition();
			}

			if (!mouse[MouseButton.Left])
			{
				scaleOrigin = null;
				scaleAxis = Axis.None;
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

	public override bool Active
	{
		get { return tool.ActiveAxis != Tool.Axis.None; }
	}

	public override bool Hovered
	{
		get { return tool.HoveredAxis != Tool.Axis.None; }
	}

	public override Vector2 Position
	{
		get
		{
			if (scaleOrigin != null)
				return scaleOrigin.Value;
			else
				return base.Position;
		}
	}

	Vector2[] vertexOriginPosition;
	Vector2? scaleOrigin;
	Vector2? scaleNormalX, scaleNormalY;

	public ManipulatorScale(Editor e)
		: base(e)
	{
		tool = new Tool(this);
	}

	public override void Logic()
	{
		if (!Visible) return;

		tool.Logic();

		if (tool.ActiveAxis != Tool.Axis.None)
			Manipulate();

		else if (scaleOrigin != null)
		{
			scaleOrigin = null;
			scaleNormalX = null;
			scaleNormalY = null;
		}
	}

	protected void BakeVertexPosition()
	{
		scaleOrigin = Position;
		scaleNormalX = NormalX;
		scaleNormalY = NormalY;

		vertexOriginPosition = new Vector2[editor.SelectedVertices.Count];
		for (int i = 0; i < editor.SelectedVertices.Count; i++)
			vertexOriginPosition[i] = editor.SelectedVertices[i].Position;
	}

	void Manipulate()
	{
		Vector2 origin = scaleOrigin.Value;
		Vector2 scale = tool.ScaleFactor;

		for (int i = 0; i < vertexOriginPosition.Length; i++)
		{
			Vector2 delta = vertexOriginPosition[i] - origin;

			Vector2 scaleX = Vector2.Dot(delta, scaleNormalX.Value) * scaleNormalX.Value;
			Vector2 scaleY = Vector2.Dot(delta, scaleNormalY.Value) * scaleNormalY.Value;

			delta = scaleX * scale.X + scaleY * scale.Y;

			EVertex v = editor.SelectedVertices[i];
			v.Position = origin + delta;
		}
	}

	public override void Draw()
	{
		if (!Visible) return;

		tool.Draw();
	}
}