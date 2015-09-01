using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;
using TKTools.Mathematics;

class ManipulatorRotate : Manipulator
{
	class Tool
	{
		ManipulatorRotate manipulator;
		Model2D model;

		MouseWatch mouse = Editor.mouse;

		float? previousAngle = null;

		public bool Active
		{
			get
			{
				return previousAngle != null;
			}
		}

		public bool Hovered
		{
			get
			{
				if (!manipulator.Visible) return false;

				float dis = (mouse.Position.Xy - manipulator.Position).LengthFast / Editor.CurrentEditor.editorCamera.Position.Z;

				return dis <= 0.1f * 1.1f && dis >= 0.1f * 0.8f;
			}
		}

		public float Angle
		{
			get
			{
				return TKMath.GetAngle(mouse.Position.Xy - manipulator.Position);
			}
		}

		public float DeltaAngle
		{
			get
			{
				if (!Active) return 0f;
				return Angle - previousAngle.Value;
			}
		}

		Color Color
		{
			get
			{
				if (Active)
					return new Color(1f, 1f, 1f, 1f);
				else
				{
					if (Hovered)
						return new Color(1f, 1f, 1f, 0.7f);
					else
						return new Color(1f, 0.8f, 0f, 0.4f);
				}
			}
		}

		public Tool(ManipulatorRotate manip)
		{
			manipulator = manip;
			model = (Model2D)Model.CreateFromTexture(new Texture("circle.png"));
			model.Color = new Color(1f, 1f, 1f, 0.4f);
		}

		public void Logic()
		{
			if ((Hovered && mouse.ButtonPressed(MouseButton.Left)) || (previousAngle != null && mouse[MouseButton.Left]))
				previousAngle = Angle;

			if (!mouse[MouseButton.Left] && previousAngle != null)
				previousAngle = null;
		}

		public void Draw()
		{
			GL.Enable(EnableCap.StencilTest);
			GL.Clear(ClearBufferMask.StencilBufferBit);

			GL.StencilFunc(StencilFunction.Always, 1, 0xff);
			GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
			
			GL.ColorMask(false, false, false, false);

			model.Reset();

			model.Translate(manipulator.Position);
			model.Scale(Editor.CurrentEditor.editorCamera.Position.Z * 0.2f * 0.94f);

			model.Draw();

			GL.StencilFunc(StencilFunction.Notequal, 1, 0xff);
			GL.ColorMask(true, true, true, true);

			model.Color = Color;

			model.Reset();

			model.Translate(manipulator.Position);
			model.Scale(Editor.CurrentEditor.editorCamera.Position.Z * 0.2f);

			model.Draw();

			GL.Disable(EnableCap.StencilTest);
		}
	}

	Tool tool;

	public override bool Active
	{
		get
		{
			return tool.Active;
		}
	}

	public override bool Hovered
	{
		get
		{
			if (!Visible) return false;
			return tool.Hovered;
		}
	}

	public ManipulatorRotate(Editor e)
		:base(e)
	{
		tool = new Tool(this);
	}

	public override void Logic()
	{
		if (!Visible) return;

		base.Logic();

		if (tool.Active)
		{
			Manipulate(tool.DeltaAngle);
		}

		tool.Logic();
	}

	void Manipulate(float delta)
	{
		delta = TKMath.ToRadians(delta);

		Vector2 origin = Position;

		foreach (EVertex v in editor.selectedVertices)
		{
			Vector2 diff = v.Position - origin;

			float sin = (float)Math.Sin(delta);
			float cos = (float)Math.Cos(delta);

			Vector2 deltaPos = new Vector2(diff.X * cos - diff.Y * sin, diff.Y * cos + diff.X * sin);
			v.Position = origin + deltaPos;
		}
	}

	public override void Draw()
	{
		if (!Visible) return;

		base.Draw();
		tool.Draw();
	}
}
