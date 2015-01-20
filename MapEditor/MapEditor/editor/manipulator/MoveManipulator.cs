using OpenTK;
using OpenTK.Input;
using System;
using TKTools;
namespace MapEditor.Manipulators
{
	public class MoveManipulator : Manipulator
	{
		class MoveButton
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

			MoveManipulator manipulator;
			Button centerButton;
			ArrowButton xAxisButton, yAxisButton;

			public MoveButton(MoveManipulator mani)
			{
				manipulator = mani;

				centerButton = new Button(mani.editor);
				xAxisButton = new ArrowButton(mani.editor);
				yAxisButton = new ArrowButton(mani.editor);

				centerButton.Color = Color.Yellow;
				xAxisButton.Color = Color.Red;
				yAxisButton.Color = Color.Green;
			}

			public void Logic()
			{
				Vector2 pos = manipulator.Origin;
				Vector2 normal = manipulator.Normal;
				float angle = TKMath.GetAngle(normal) - 90;

				centerButton.Position = pos;
				centerButton.Rotation = angle;
				xAxisButton.Position = pos + normal.PerpendicularRight;
				xAxisButton.Rotation = angle-90;
				yAxisButton.Position = pos + normal;
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

		public override bool Hovered
		{
			get
			{
				if (!Enabled) return false;
				return moveButton.Hovered;
			}
		}

		Vector2 moveOrigin;
		Vector2 moveStart;
		MoveButton moveButton;

		Vector2 moveAxis;

		public MoveManipulator(Editor editor)
			: base(editor)
		{
			moveButton = new MoveButton(this);
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public override void Enable(Vector2 pos)
		{
			moveButton.Logic();

			moveStart = Origin;
			moveOrigin = pos - moveStart;
			moveAxis = moveButton.Axis;
			base.Enable(pos);
		}

		public override void Logic()
		{
			if (!Enabled) return;

			base.Logic();
			moveButton.Logic();
		}

		public override void Manipulate()
		{
			Vector2 targetPosition = MouseInput.Current.Position - moveOrigin;

			//Calculate normal movement

			targetPosition -= moveStart;

			Vector2 normal = Normal;
			Vector2 xMovement = Vector2.Dot(targetPosition, normal.PerpendicularRight) * normal.PerpendicularRight;
			Vector2 yMovement = Vector2.Dot(targetPosition, normal) * normal;

			targetPosition = moveStart + xMovement * moveAxis.X + yMovement * moveAxis.Y;

			//

			if (KeyboardInput.Current[Key.LControl])
			{
				Vector2 gridPosition = targetPosition;

				gridPosition.X = (float)Math.Round(targetPosition.X);
				gridPosition.Y = (float)Math.Round(targetPosition.Y);

				gridPosition -= moveStart;

				Vector2 xpos = Vertex.Project(gridPosition, normal.PerpendicularRight);
				Vector2 ypos = Vertex.Project(gridPosition, normal);

				gridPosition = moveStart + xpos * moveAxis.X + ypos * moveAxis.Y;
				targetPosition = gridPosition;
			}

			if (KeyboardInput.Current[Key.C])
			{
				Vertex closest = null;
				float closestDistance = 0;

				foreach (EditorObject obj in editor.objectList)
					foreach (Vertex v in obj.Vertices)
					{
						if (editor.selectedList.Contains(v)) continue;

						float dist = (v.Position - MouseInput.Current.Position).Length;

						if (closest == null || dist < closestDistance)
						{
							closest = v;
							closestDistance = dist;
						}
					}

				if (closest != null)
				{
					Vector2 closestPosition = closest.Position - moveStart;

					Vector2 xpos = Vector2.Dot(closestPosition, normal.PerpendicularRight) * normal.PerpendicularRight;
					Vector2 ypos = Vector2.Dot(closestPosition, normal) * normal;

					targetPosition = moveStart + xpos * moveAxis.X + ypos * moveAxis.Y;
				}
			}

			//targetPosition = moveStart + (targetPosition - moveStart) * moveAxis;

			foreach (Vertex v in editor.selectedList)
				v.MoveTo(vertexOffsetList[v] + targetPosition);

			base.Manipulate();
		}

		public override void Draw()
		{
			if (!Enabled) return;
			moveButton.Draw();
		}
	}
}