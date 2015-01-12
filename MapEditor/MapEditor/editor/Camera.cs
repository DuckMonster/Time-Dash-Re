using OpenTK;
using OpenTK.Input;
namespace MapEditor
{
	public class Camera
	{
		Vector3 position = new Vector3(0, 0, 2);

		public Matrix4 ViewMatrix
		{
			get
			{
				return Matrix4.LookAt(position, new Vector3(position.X, position.Y, 0), Vector3.UnitY);
			}
		}

		public Vector3 Position
		{
			get
			{
				return position;
			}
		}

		public Camera()
		{
		}

		public void Logic()
		{
			//position.Z = 2 + (MouseInput.Current.Wheel * 0.3f);

			if (MouseInput.Current[OpenTK.Input.MouseButton.Middle] || (KeyboardInput.Current[OpenTK.Input.Key.LAlt] && MouseInput.Current[OpenTK.Input.MouseButton.Left]))
			{
				position.Xy -= (MouseInput.Current.PositionOrtho - MouseInput.Previous.PositionOrtho) * position.Z;
			}

			if (KeyboardInput.Current[Key.Plus]) position.Z -= 0.5f * (KeyboardInput.Current[Key.LAlt] ? -1 : 1) * Editor.delta;
		}
	}
}