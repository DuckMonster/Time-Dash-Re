using OpenTK;
using OpenTK.Input;
using System;
using System.Diagnostics;
using TKTools;
namespace MapEditor
{
	public class EditorObject
	{
		Editor editor;
		public Mesh mesh;

		public Vector2 position = Vector2.Zero, scale = new Vector2(1, 1);
		public float rotation = 0f;

		bool clicked = false;
		Vector2 clickedOffset;
		float rotationOffset;
		Vector2 scaleBuffer, startScale;
		Vector2 moveBuffer, startMove;

		public bool Hovered
		{
			get
			{
				return GetCollision(MouseInput.Current, Vector2.Zero);
			}
		}

		public bool Selected
		{
			get
			{
				return editor.selectedList.Contains(this);
			}
		}

		public bool GetCollision(Vector2 pos, Vector2 size)
		{
			//float len = (pos - position).Length, angle = TKMath.GetAngle(position, pos);
			//pos = position + TKMath.GetAngleVector(-rotation + angle) * len;

			//return (pos.X + size.X/2 >= position.X - scale.X/2 &&
			//	pos.X - size.X/2 <= position.X + scale.X/2 &&
			//	pos.Y + size.Y/2 >= position.Y - scale.Y/2 &&
			//	pos.Y - size.Y/2 <= position.Y + scale.Y/2);

			return GetCollision(new Polygon(pos));
		}

		public bool GetCollision(Polygon p)
		{
			return mesh.Polygon.Intersects(p);
		}

		public EditorObject(Editor e)
		{
			editor = e;

			mesh = Mesh.Box;
		}

		public void Logic()
		{
			/*
			if (KeyboardInput.Current[Key.D]) position.X += 5f * Editor.delta;
			if (KeyboardInput.Current[Key.A]) position.X -= 5f * Editor.delta;
			if (KeyboardInput.Current[Key.S]) position.Y -= 5f * Editor.delta;
			if (KeyboardInput.Current[Key.W]) position.Y += 5f * Editor.delta;

			if (KeyboardInput.Current[Key.Right]) scale.X += 0.5f * Editor.delta;
			if (KeyboardInput.Current[Key.Left]) scale.X -= 0.5f * Editor.delta;
			if (KeyboardInput.Current[Key.Up]) scale.Y += 0.5f * Editor.delta;
			if (KeyboardInput.Current[Key.Down]) scale.Y -= 0.5f * Editor.delta;

			if (KeyboardInput.Current[Key.E]) rotation += 50f * Editor.delta;
			if (KeyboardInput.Current[Key.Q]) rotation -= 50f * Editor.delta;
			 * */

			if (MouseInput.Current[MouseButton.Left])
			{
				if (!MouseInput.Previous[MouseButton.Left] && Hovered)
				{
					clicked = true;
				}

				if (clicked)
				{
					//Manipulate();
				}
			}
			else if (clicked)
			{
				clicked = false;
			}
		}

		public void Select()
		{
			
		}

		public void BeginManipulate()
		{
			clickedOffset = MouseInput.Current.Position - position;
			rotationOffset = TKMath.GetAngle(position, MouseInput.Current.Position) - rotation;
			scaleBuffer = new Vector2(0, 0);
			startScale = scale;

			startMove = position;
			moveBuffer = new Vector2(0, 0);
		}

		public void Move(Vector2 delta)
		{
			position += delta;
		}

		public void MoveTo(Vector2 pos)
		{
			position = pos;
		}

		public void Scale(Vector2 delta)
		{
			scale += delta;
		}

		public void Rotate(float delta)
		{
			rotation += delta;
		}

		public void Manipulate()
		{
			switch (editor.editMode)
			{
				case EditMode.Move:
					moveBuffer += MouseInput.Delta;
					Vector2 finalPosition = startMove + moveBuffer;

					if (KeyboardInput.Current[Key.LControl])
					{
						finalPosition.X = (float)Math.Round(finalPosition.X);
						finalPosition.Y = (float)Math.Round(finalPosition.Y);
					}

					position = finalPosition;
					break;

				case EditMode.Scale:
					float len = MouseInput.Delta.Length, angle = TKMath.GetAngle(MouseInput.Delta);
					Vector2 dirDelta = TKMath.GetAngleVector(angle - rotation) * len;

					scaleBuffer += dirDelta;
					Vector2 scaleBufferFinal = scaleBuffer;

					if (KeyboardInput.Current[Key.LShift])
					{
						scaleBufferFinal = new Vector2(scaleBufferFinal.X + scaleBufferFinal.Y, scaleBufferFinal.X + scaleBufferFinal.Y);
					}
					else scaleBufferFinal *= 2;
					if (KeyboardInput.Current[Key.LControl])
					{
						scaleBufferFinal = new Vector2(
							(float)Math.Round(scaleBufferFinal.X + startScale.X),
							(float)Math.Round(scaleBufferFinal.Y + startScale.Y));

						scaleBufferFinal -= startScale;
					}

					scale = startScale + scaleBufferFinal;
					break;

				case EditMode.Rotate:
					rotation = (TKMath.GetAngle(position, MouseInput.Current.Position) - rotationOffset);

					if (KeyboardInput.Current[Key.LControl])
					{
						rotation = rotation / 45;
						rotation = (float)Math.Round(rotation);
						rotation *= 45;
					}

					break;
			}
		}

		public void Draw()
		{
			Color c;

			if (Selected) c = Color.Yellow;
			else c = Color.White;

			if (Hovered || (Selected && editor.manipulating)) c.A = 1f;
			else c.A = 0.5f;

			mesh.Color = c;

			mesh.Reset();

			mesh.Translate(position);
			mesh.Rotate(rotation);
			mesh.Scale(scale);

			mesh.Draw();
		}
	}
}