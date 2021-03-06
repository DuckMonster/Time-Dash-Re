﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TKTools;

namespace MapEditor
{
	public partial class Editor
	{
		public List<Template> templateList = new List<Template>();
		public TemplateMenu templateMenu;
		public TemplateCreator templateCreator;

		public TilesetList tilesetList;

		public void CreateTemplate(int tilesetIndex, RectangleF uv)
		{
			int index = templateList.Count;
			Template t = new Template(tilesetIndex, uv, index, this);

			AddTemplate(t);
			templateMenu.AddTemplate(t);
		}

		public void CreateTemplate(BinaryReader reader)
		{
			int index = templateList.Count;
			Console.WriteLine("Added template " + index);
			AddTemplate(new Template(reader, index, this));
		}

		void AddTemplate(Template t)
		{
			templateList.Add(t);
		}

		public void DeleteTemplate(Template t)
		{
			if (!templateList.Contains(t)) return;

			for (int i = t.ID; i < templateList.Count; i++)
				templateList[i].ID--;

			templateList.Remove(t);

			EditorObject[] refe = t.references.ToArray();
			foreach (EditorObject obj in refe)
				DeleteObject(obj);

			t.Dispose();
		}
	}
}