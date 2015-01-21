using System;
using System.IO;

namespace MapEditor
{
	public partial class Editor
	{
		public void CloseMap()
		{
			foreach (EditorObject obj in objectList)
				obj.Dispose();

			objectList.Clear();

			foreach (Template t in templateList)
				t.Dispose();

			templateList.Clear();

			tilesetList.Dispose();
			templateMenu.Dispose();
			templateCreator.Dispose();

			selectedList.Clear();
		}

		public void SaveMap(string path)
		{
			Directory.CreateDirectory(path);

			using (BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.Create)))
			{
				tilesetList.WriteToFile(writer);

				writer.Write(templateList.Count);
				foreach (Template t in templateList)
					t.WriteToFile(writer);

				writer.Write(objectList.Count);
				foreach (EditorObject obj in objectList)
					obj.WriteToFile(writer);
			}
		}

		public void LoadMap(string path)
		{
			using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
			{
				tilesetList.ReadFromFile(reader);

				int templateNmbr = reader.ReadInt32();
				for (int i = 0; i < templateNmbr; i++)
					AddTemplate(reader);

				int objectNmbr = reader.ReadInt32();
				for (int i = 0; i < objectNmbr; i++)
					CreateObject(new EditorObject(reader, this));
			}
		}
	}
}