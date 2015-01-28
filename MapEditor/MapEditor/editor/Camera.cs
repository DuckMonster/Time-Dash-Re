using OpenTK;
using OpenTK.Input;
using System;
namespace MapEditor
{
	public class Camera
	{
		Editor editor;
		Vector3 position = new Vector3(0, 0, 2);
		Vector2 mouseZoomPositionOrigin;
		float mouseZoomOrigin;
		float scrollSpeed = 0f;
		float baseZ = 0f;

		public Matrix4 ViewMatrix
		{
			get
			{
				return Matrix4.LookAt(position + new Vector3(0, 0, baseZ), new Vector3(position.X, position.Y, baseZ), Vector3.UnitY);
			}
		}

		public Vector3 Position
		{
			get
			{
				return position;
			}
		}

		public float TargetBaseZ
		{
			get
			{
				return -editor.ActiveLayer.Z;
			}
		}

		public Camera(Editor e)
		{
			editor = e;
		}

		public void Logic()
		{
			if (!editor.templateMenu.Hovered && !editor.templateMenu.TabHovered && !editor.templateCreator.Active)
			{
				if (MouseInput.Current[OpenTK.Input.MouseButton.Middle] || (KeyboardInput.Current[OpenTK.Input.Key.LAlt] && MouseInput.Current[OpenTK.Input.MouseButton.Left]))
				{
					position.Xy -= (MouseInput.Current.Position - MouseInput.Previous.Position);
				}
				if (KeyboardInput.Current[Key.LAlt] && MouseInput.ButtonPressed(MouseButton.Right))
				{
					mouseZoomPositionOrigin = MouseInput.Current.Position;
					mouseZoomOrigin = position.Z;
				}
				if (KeyboardInput.Current[Key.LAlt] && MouseInput.Current[MouseButton.Right])
				{
					position.Z = mouseZoomOrigin * ((MouseInput.Current.PositionOrtho.X - mouseZoomPositionOrigin.X) * 0.2f + 1f);
				}

				if (KeyboardInput.Current[Key.Plus]) position.Z -= 0.5f * (KeyboardInput.Current[Key.LAlt] ? -1 : 1) * Editor.delta;

				if (MouseInput.Current.Wheel != MouseInput.Previous.Wheel)
				{
					float delta = MouseInput.Current.Wheel - MouseInput.Previous.Wheel;
					scrollSpeed -= delta;
				}
			}

			baseZ += (TargetBaseZ - baseZ) * 5f * Editor.delta;
			if (Math.Abs(TargetBaseZ - baseZ) < 0.1f) baseZ = TargetBaseZ;

			if (Math.Abs(scrollSpeed) > 0.001f)
			{
				position.Z += scrollSpeed * position.Z * 0.8f * Editor.delta;
				scrollSpeed -= scrollSpeed * 5 * Editor.delta;
			}

			if (position.Z < 1.01f) position.Z = 1.01f;
		}
	}
}