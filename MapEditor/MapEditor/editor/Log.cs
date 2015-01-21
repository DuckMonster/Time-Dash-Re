using System;
using System.Threading;
namespace MapEditor
{
	public class Log
	{
		Editor editor;

		public Log(Editor e)
		{
			editor = e;

			new Thread(InputThread).Start();
		}

		public void InputThread()
		{
			while (true)
			{
				string[] input = Console.ReadLine().Split(' ');

				if (input[0] == "load") editor.LoadMap(input[1]);
				if (input[0] == "save") editor.SaveMap(input[1]);
				if (input[0] == "tileset") editor.tilesetList.LoadTileset(input[1]);
			}
		}
	}
}