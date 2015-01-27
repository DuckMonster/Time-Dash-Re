using System;
using OpenTK;
using System.Collections.Generic;
using TKTools;

namespace MapEditor.Manipulators
{
	public class ScaleManipulator : Manipulator
	{
		class ScaleButton : IDisposable
		{
			public bool Hovered
			{
				get
				{
					return centerButton.Hovered || xAxisButton.Hovered || yAxisButton.Hovered;
				}
			}

			public Vector2 Axis
			{
				get
				{
					if (centerButton.Hovered) return new Vector2(1, 1);
					if (xAxisButton.Hovered) return new Vector2(1, 0);
					if (yAxisButton.Hovered) return new Vector2(0, 1);

					return new Vector2(0, 0);
				}
			}

			ScaleManipulator manipulator;
			Button centerButton;
			Button xAxisButton, yAxisButton;

			public ScaleButton(ScaleManipulator mani)
			{
				manipulator = mani;

				centerButton = new Button(mani.editor);
				xAxisButton = new Button(mani.editor);
				yAxisButton = new Button(mani.editor);

				centerButton.Color = Color.Yellow;
				xAxisButton.Color = Color.Red;
				yAxisButton.Color = Color.Green;
			}

			public void Dispose()
			{
				centerButton.Dispose();
				xAxisButton.Dispose();
				yAxisButton.Dispose();
			}

			public void Logic()
			{
				Vector2 pos = manipulator.Origin;
				Vector2 normal = manipulator.Active ? manipulator.scaleNormal : manipulator.Normal;
				float angle = TKMath.GetAngle(normal) - 90;

				centerButton.Position = pos;
				centerButton.Rotation = angle;
				xAxisButton.Position = pos + normal.PerpendicularRight * Editor.camera.Position.Z * 0.1f;
				xAxisButton.Rotation = angle - 90;
				yAxisButton.Position = pos + normal * Editor.camera.Position.Z * 0.1f;
				yAxisButton.Rotation = angle;

				centerButton.Logic();
				xAxisButton.Logic();
				yAxisButton.Logic();
			}

			public void Draw()
			{
				centerButton.Draw();
				xAxisButton.Draw();
				yAxisButton.Draw();
			}
		}

		float originLength;
		ScaleButton scaleButton;
		Vector2 scaleAxis;
		Vector2 scaleOrigin;
		Vector2 scaleNormal;

		public override bool Hovered
		{
			get
			{
				if (!Enabled) return false;
				return scaleButton.Hovered;
			}
		}

		public ScaleManipulator(Editor e)
			: base(e)
		{
			scaleButton = new ScaleButton(this);
		}

		public override void Dispose()
		{
			scaleButton.Dispose();
			base.Dispose();
		}

		public override void Enable(Vector2 pos)
		{
			scaleAxis = scaleButton.Axis;
			originLength = (pos - Origin).Length;
			scaleOrigin = Origin;
			scaleNormal = Normal;

			base.Enable(pos);
		}

		public override void Logic()
		{
			if (!Enabled) return;

			base.Logic();
			scaleButton.Logic();
		}

		public override void Manipulate()
		{
			if (scaleAxis != Vector2.One)
			{
				float scalex = Vector2.Dot(MouseInput.Current.Position - scaleOrigin, scaleNormal.PerpendicularRight) / originLength - 1;
				float scaley = Vector2.Dot(MouseInput.Current.Position - scaleOrigin, scaleNormal) / originLength - 1;

				if (KeyboardInput.Current[OpenTK.Input.Key.LControl])
				{
					scalex *= 2;
					scalex = (float)Math.Round(scalex);
					scalex /= 2;

					scaley *= 2;
					scaley = (float)Math.Round(scaley);
					scaley /= 2;
				}

				Vector2 scalevecx = scaleNormal.PerpendicularRight * scalex * scaleAxis.X,
						scalevecy = scaleNormal * scaley * scaleAxis.Y;

				foreach (Vertex v in editor.selectedList)
				{
					Vector2 posx = Vector2.Dot(vertexOffsetList[v], scaleNormal.PerpendicularRight) * scaleNormal.PerpendicularRight * scalex;
					Vector2 posy = Vector2.Dot(vertexOffsetList[v], scaleNormal) * scaleNormal * scaley;

					v.MoveTo(scaleOrigin + vertexOffsetList[v] + posx * scaleAxis.X + posy * scaleAxis.Y);
				}

				base.Manipulate();
			}
			else
			{
				float scale = (MouseInput.Current.Position.X - scaleOrigin.X) * 0.4f + 1f;

				if (KeyboardInput.Current[OpenTK.Input.Key.LControl])
				{
					scale *= 2;
					scale = (float)Math.Round(scale);
					scale /= 2;
				}

				foreach (Vertex v in editor.selectedList)
				{
					v.ScaleTo(scaleOrigin, vertexOffsetList[v], scale);
				}

				base.Manipulate();
			}
		}

		public override void Draw()
		{
			if (!Enabled) return;
			scaleButton.Draw();
		}
	}
}