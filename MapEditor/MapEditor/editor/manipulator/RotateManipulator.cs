using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using TKTools;
using System.Collections.Generic;

namespace MapEditor.Manipulators
{
	public class RotateManipulator : Manipulator
	{
		class RotateButton
		{
			RotateManipulator manipulator;
			Mesh mesh = Mesh.Box;
			Texture circleTexture = new Texture("res/circle.png");

			Vector2 position;

			public bool Hovered
			{
				get
				{
					if (manipulator.Active) return true;
					float distance = (MouseInput.Current.Position - position).Length * 2;
					return distance > 4.5f && distance < 5.5f;
				}
			}

			public RotateButton(RotateManipulator mani)
			{
				manipulator = mani;
				mesh.Texture = circleTexture;
			}

			public void Logic()
			{
				position = manipulator.Origin;
			}

			public void Draw()
			{
				Color c = manipulator.Active ? Color.White : Color.Yellow;
				c.A = Hovered ? 0.8f : 0.2f;

				mesh.Color = c;

				GL.Enable(EnableCap.StencilTest);
				GL.Clear(ClearBufferMask.StencilBufferBit);

				GL.StencilFunc(StencilFunction.Never, 1, 0xff);
				GL.StencilOp(StencilOp.Replace, StencilOp.Keep, StencilOp.Keep);

				mesh.Reset();

				mesh.Translate(position);
				mesh.Scale(5f);

				mesh.Draw();

				GL.StencilFunc(StencilFunction.Notequal, 1, 0xff);

				mesh.Scale(1.02f);
				mesh.Draw();

				GL.Disable(EnableCap.StencilTest);
			}
		}

		Vector2 rotateOrigin;
		RotateButton rotateButton;
		float startAngle;

		public override bool Hovered
		{
			get
			{
				if (!Enabled) return false;
				return rotateButton.Hovered;
			}
		}

		public override Vector2 Origin
		{
			get
			{
				if (Active) return rotateOrigin;
				return base.Origin;
			}
		}

		public RotateManipulator(Editor editor)
			: base(editor)
		{
			rotateButton = new RotateButton(this);
		}

		public override void Enable(Vector2 pos)
		{
			startAngle = TKMath.GetAngle(pos - Origin);
			rotateOrigin = Origin;

			base.Enable(pos);
		}

		public override void Logic()
		{
			if (!Enabled) return;

			base.Logic();
			rotateButton.Logic();
		}

		public override void Manipulate()
		{
			float angle = TKMath.GetAngle(MouseInput.Current.Position - rotateOrigin) - startAngle;
			if (KeyboardInput.Current[Key.LControl])
			{
				angle /= 22.5f;

				angle = (float)Math.Round(angle);

				angle *= 22.5f;
			}

			foreach(Vertex v in editor.selectedList)
				v.RotateTo(rotateOrigin, angle + TKMath.GetAngle(vertexOffsetList[v]));

			base.Manipulate();
		}

		public override void Draw()
		{
			if (!Enabled) return;

			rotateButton.Draw();
		}
	}
}