using System;
using System.Threading;
using GRFX = OpenTK.Graphics;

namespace MapEditor
{
	public class MainClass
	{
		[STAThread]
		static void Main(string[] args)
		{
			using (EditorProgram p = new EditorProgram(1025, 768, new GRFX.GraphicsMode(new GRFX.ColorFormat(32), 24, 8, 3)))
			{
				p.Run(80.0, 150.0);
			}
		}
	}
}