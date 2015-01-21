using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TKTools;

namespace MapEditor
{
	public partial class Editor
	{
		public List<Template> templateList = new List<Template>();
		TemplateMenu templateMenu;
		public TemplateCreator templateCreator;

		public TilesetList tilesetList;

		public void AddTemplate(int tilesetIndex, RectangleF uv)
		{
			int index = templateList.Count;
			AddTemplate(new Template(tilesetIndex, uv, index, this));
		}

		public void AddTemplate(BinaryReader reader)
		{
			int index = templateList.Count;
			AddTemplate(new Template(reader, index, this));
		}

		void AddTemplate(Template t)
		{
			templateList.Add(t);
			templateMenu.AddTemplate(t);
		}
	}
}