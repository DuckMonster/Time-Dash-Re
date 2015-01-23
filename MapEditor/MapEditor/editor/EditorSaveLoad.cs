using MapEditor.Manipulators;
using System;
using System.IO;
using System.Collections.Generic;

namespace MapEditor
{
	public partial class Editor
	{
		bool saveFlag = false;
		public bool SaveFlag
		{
			get
			{
				return saveFlag;
			}
		}

		string fileName = null;
		public string FileName
		{
			get
			{
				return fileName;
			}
			set
			{
				fileName = value;
			}
		}

		public void Dispose()
		{
			gridMesh.Dispose();

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

			foreach (KeyValuePair<EditMode, Manipulator> m in manipulators)
				m.Value.Dispose();

			manipulators.Clear();
		}

		public void SaveMap(string path)
		{
			saveFlag = false;
			fileName = path;

			using (BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.Create)))
			{
				tilesetList.WriteToFile(writer);

				writer.Write(templateList.Count);
				foreach (Template t in templateList)
					t.WriteToFile(writer);

				writer.Write(objectList.Count);
				foreach (EditorObject obj in objectList)
					obj.WriteToFile(writer);

				templateMenu.WriteToFile(writer);
			}
		}

		public void LoadMap(string path)
		{
			fileName = path;

			using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
			{
				tilesetList.ReadFromFile(reader);

				int templateNmbr = reader.ReadInt32();
				for (int i = 0; i < templateNmbr; i++)
					CreateTemplate(reader);

				int objectNmbr = reader.ReadInt32();
				for (int i = 0; i < objectNmbr; i++)
					CreateObject(new EditorObject(reader, this));

				templateMenu.ReadFromFile(reader);
			}
		}
	}
}