using OpenTK;
using OpenTK.Input;
using System.Diagnostics;
using TKTools;
namespace MapEditor
{
	public class EditorObject
	{
		Editor editor;
		Mesh mesh;

		Vector2 position = Vector2.Zero, scale = new Vector2(1, 1);
		float rotation = 0f;

		bool clicked = false;
		Vector2 clickedOffset;

		public bool Hovered
		{
			get
			{
				return GetCollision(MouseInput.Current, Vector2.Zero);
			}
		}

		public bool GetCollision(Vector2 pos, Vector2 size)
		{
			float len = (pos - position).Length, angle = TKMath.GetAngle(position, pos);
			pos = position + TKMath.GetAngleVector(-rotation + angle) * len;

			return (pos.X + size.X/2 >= position.X - scale.X/2 &&
				pos.X - size.X/2 <= position.X + scale.X/2 &&
				pos.Y + size.Y/2 >= position.Y - scale.Y/2 &&
				pos.Y - size.Y/2 <= position.Y + scale.Y/2);
		}

		public EditorObject(Editor e)
		{
			editor = e;

			mesh = Mesh.Box;
		}

		public void Logic()
		{
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

			if (MouseInput.Current[MouseButton.Left])
			{
				if (!MouseInput.Previous[MouseButton.Left] && Hovered)
				{
					clicked = true;
					clickedOffset = MouseInput.Current.Position - position;
				}

				if (clicked)
				{
					position = MouseInput.Current.Position - clickedOffset;
				}
			}
			else if (clicked) clicked = false;
		}

		public void Draw()
		{
			mesh.Color = Hovered ? Color.Blue : Color.White;
			if (clicked) mesh.Color = Color.Yellow;

			mesh.Reset();

			mesh.Translate(position);
			mesh.Rotate(rotation);
			mesh.Scale(scale);

			mesh.Draw();
		}
	}
}