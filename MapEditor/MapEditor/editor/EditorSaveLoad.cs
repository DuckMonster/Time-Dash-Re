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

			foreach (Layer l in layerList)
				l.Dispose();

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
				writer.Write(mapName == null ? "Untitled" : mapName);
				writer.Write(gameModeID);

				background.WriteToFile(writer);

				tilesetList.WriteToFile(writer);

				writer.Write(templateList.Count);
				foreach (Template t in templateList)
					t.WriteToFile(writer);

				EventTemplate.SaveTo(writer);

				writer.Write(layerList.Count);
				foreach (Layer l in layerList)
					l.WriteToFile(writer);

				templateMenu.WriteToFile(writer);
			}
		}

		public void LoadMap(string path)
		{
			fileName = path;

			using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
			{
				mapName = reader.ReadString();
				gameModeID = reader.ReadInt32();

				background.ReadFromFile(reader);

				tilesetList.ReadFromFile(reader);

				int templateNmbr = reader.ReadInt32();
				for (int i = 0; i < templateNmbr; i++)
					CreateTemplate(reader);

				EventTemplate.LoadFrom(reader);

				int layerNmbr = reader.ReadInt32();
				for (int i = 0; i < layerNmbr; i++)
				{
					if (i == 0)
						layerList.Add(new SolidLayer(this));
					else
						layerList.Add(new Layer(i, reader.ReadSingle(), this));

					layerList[i].ReadFromFile(reader);
				}

				templateMenu.ReadFromFile(reader);
			}
		}
	}
}