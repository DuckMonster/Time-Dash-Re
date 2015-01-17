using OpenTK;
using OpenTK.Input;
namespace MapEditor.Manipulators
{
	public class MoveManipulator : Manipulator
	{
		Vector2 moveOrigin;

		public MoveManipulator(Editor editor)
			: base(editor)
		{

		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public override void Logic()
		{
			if (!Enabled) return;

			if (Hovered && MouseInput.Current[MouseButton.Left])
			{
				if (MouseInput.ButtonPressed(MouseButton.Left)) moveOrigin = MouseInput.Current.Position;

				foreach (EditorObject obj in editor.selectedList)
					obj.Move(MouseInput.Delta);
			}

			base.Logic();
		}

		public override void Draw()
		{
			base.Draw();

			if (!Enabled) return;
		}
	}
}