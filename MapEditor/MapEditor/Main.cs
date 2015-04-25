using System;
using System.IO;
using System.Threading;
using GRFX = OpenTK.Graphics;

namespace MapEditor
{
	public class MainClass
	{
		[STAThread]
		static void Main(string[] args)
		{
			System.Windows.Forms.Application.EnableVisualStyles();
			Container.ExecutableLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

			using (EditorProgram p = new EditorProgram(1025, 768, new GRFX.GraphicsMode(new GRFX.ColorFormat(32), 24, 8, 3)))
			{
				if (args.Length > 0) p.OpenFile(args[0]);
				p.Run(150.0);
			}
		}
	}
}