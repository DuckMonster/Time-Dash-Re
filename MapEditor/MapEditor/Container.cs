using OpenTK.Input;
using System.Windows.Forms;
namespace MapEditor
{
	public class Container
	{
		public static string ExecutableLocation;
		public static string FindLocalFile(string filename)
		{
			string final = "";
			string[] split = filename.Split('/');

			foreach(string s in split)
				final += "\\" + s;

			return ExecutableLocation + final;
		}

		public EditorProgram program;
		public Editor editor;

		Log log;

		public Container(EditorProgram prog)
		{
			program = prog;
			editor = new Editor(this);

			//log = new Log(this);
		}

		public void Save()
		{
			string file = editor.FileName;

			if (file == null || KeyboardInput.Current[Key.ShiftLeft])
			{
				using (SaveFileDialog dialog = new SaveFileDialog())
				{
					dialog.Filter = "Time Dash Map (*.tdm)|*.tdm";

					if (dialog.ShowDialog() == DialogResult.OK)
						file = dialog.FileName;
					else
						return;
				}
			}

			editor.SaveMap(file);
		}

		public void Load()
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.Filter = "Time Dash Map (*.tdm)|*.tdm";

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					DisposeEditor();
					editor = new Editor(dialog.FileName, this);
				}
			}
		}

		public void Load(string path)
		{
			DisposeEditor();
			editor = new Editor(path, this);
		}

		public void DisposeEditor()
		{
			if (editor == null) return;

			editor.Dispose();
			editor = null;
		}

		public void Logic()
		{
			if (KeyboardInput.Current[Key.LControl])
			{
				if (KeyboardInput.KeyPressed(Key.S)) Save();
				if (KeyboardInput.KeyPressed(Key.O)) Load();
				if (KeyboardInput.KeyPressed(Key.N))
				{
					DisposeEditor();
					editor = new Editor(this);
				}
			}

			if (editor != null)
			{
				string fn = editor.FileName == null ? "Untitled" : editor.FileName;

				if (editor.SaveFlag) fn += " *";
				program.Title = fn + " - Map Editor";

				editor.Logic();
			}
		}

		public void Draw()
		{
			if (editor != null) editor.Draw();
		}
	}
}