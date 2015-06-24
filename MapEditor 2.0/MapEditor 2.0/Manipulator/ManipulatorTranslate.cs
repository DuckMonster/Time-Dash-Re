using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;

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
				if (!mouse[OpenTK.Input.MouseButton.Left] || HoveredAxis == Axis.None) return Axis.None;
				return HoveredAxis;
			}
		}

		Matrix4 ModelMatrix
		{
			get
			{
				Vector2 p = manipulator.Position;

				return new Matrix4(
					0.04f * CameraControl.Position.Z, 0f, 0f, 0f,
					0f, 0.04f * CameraControl.Position.Z, 0f, 0f,
					0f, 0f, 1f, 0f,
					p.X, p.Y, 0f, 1f
					);
			}
		}

		public Tool(ManipulatorTranslate mt)
		{
			manipulator = mt;

			xAxis = new Mesh(new Vector2[] {
				new Vector2(1f, 0.4f),
				new Vector2(1f, -0.4f),
				new Vector2(2f, 0f)
			});

			yAxis = new Mesh(new Vector2[] {
				new Vector2(-0.4f, 1f),
				new Vector2(0.4f, 1f),
				new Vector2(0f, 2f)
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
			switch(a)
			{
				case Axis.Plane: baseColor = Color.Yellow;break;
				case Axis.X: baseColor = Color.Green;break;
				case Axis.Y:baseColor = Color.Red; break;
				default:baseColor = new Color(0, 0, 0, 0); break;
			}

			bool hovered = HoveredAxis == a;

			if (manipulator.activeAxis == a) return new Color(1f, 1f, 1f, 0.8f);
			else return baseColor * new Color(1f, 1f, 1f, hovered ? 0.8f : 0f);
		}

		public void Logic()
		{

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
	MouseWatch mouse = Editor.mouse;
	Vector2 previousMousePosition;

	Tool.Axis activeAxis = Tool.Axis.None;

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
			return activeAxis != Tool.Axis.None;
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
		tool.Logic();

		if (activeAxis == Tool.Axis.None)
		{
			if (mouse.ButtonPressed(MouseButton.Left) && tool.HoveredAxis != Tool.Axis.None)
				activeAxis = tool.HoveredAxis;
		}
		else
		{
			Manipulate(activeAxis);

			if (!mouse[MouseButton.Left]) activeAxis = Tool.Axis.None;
		}

		previousMousePosition = mouse.Position.Xy;
	}

	void Manipulate(Tool.Axis axis)
	{
		Vector2 multiplier = Vector2.One;
		switch(axis)
		{
			case Tool.Axis.X: multiplier = new Vector2(1, 0); break;
			case Tool.Axis.Y: multiplier = new Vector2(0, 1); break;
		}

		Vector2 delta = (mouse.Position.Xy - previousMousePosition) * multiplier;

		foreach (EVertex v in editor.SelectedVertices)
			v.Position += delta;

		foreach (EMesh m in editor.selectedMeshes)
			m.UpdateMesh();
	}

	public override void Draw()
	{
		if (!Visible) return;
		tool.Draw();
	}
}