using OpenTK;
namespace MapEditor
{
	public class Camera
	{
		Vector3 position = new Vector3(0, 0, 5);

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
			position.Z = 5 + (MouseInput.Current.Wheel * 0.3f);

			if (MouseInput.Current[OpenTK.Input.MouseButton.Middle])
			{
				position.Xy -= (MouseInput.Current.PositionOrtho - MouseInput.Previous.PositionOrtho) * position.Z;
			}
		}
	}
}