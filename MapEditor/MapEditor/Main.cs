using System.Threading;
using GRFX = OpenTK.Graphics;

namespace MapEditor
{
	public class MainClass
	{
		static void Main(string[] args)
		{
			using (EditorProgram p = new EditorProgram(1025, 768, new GRFX.GraphicsMode(new GRFX.ColorFormat(32), 24, 8, 3)))
			{
				p.Run();
			}
		}
	}
}