using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TKTools;
using System.Windows.Forms;
using System.IO;

namespace MapEditor
{
	public class Background : IDisposable
	{
		Texture texture;
		string filename;

		Mesh mesh;

		public bool show = true;

		public Background()
		{
			mesh = Mesh.Box;
			mesh.UIElement = true;

			mesh.Scale(Editor.screenWidth);
		}

		public void Dispose()
		{
			texture.Dispose();
			mesh.Dispose();
		}

		public void LoadTexture()
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					LoadTexture(dialog.FileName);
				}
			}
		}

		public void LoadTexture(string filename)
		{
			this.filename = filename;
			if (texture != null) texture.Dispose();
			texture = new Texture(filename);
			mesh.Texture = texture;
		}

		public void WriteToFile(BinaryWriter writer)
		{
			writer.Write(texture != null);
			if (texture == null)
				return;

			byte[] buffer;
			using (FileStream str = new FileStream(filename, FileMode.Open))
			{
				buffer = new byte[str.Length];
				str.Read(buffer, 0, buffer.Length);
			}

			writer.Write(buffer.Length);
			writer.Write(buffer, 0, buffer.Length);
		}

		public void ReadFromFile(BinaryReader reader)
		{
			if (!reader.ReadBoolean()) return;

			string newFile = "tmpbg";

			using (FileStream str = new FileStream(newFile, FileMode.Create))
			{
				byte[] buffer = new byte[reader.ReadInt32()];
				reader.Read(buffer, 0, buffer.Length);

				str.Write(buffer, 0, buffer.Length);
			}

			LoadTexture(newFile);
		}

		public void Draw()
		{
			if (texture == null || !show) return;
			mesh.Draw();
		}
	}
}