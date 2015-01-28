using EZUDP;
using System.Net;
using System.Net.Sockets;

public class TrackerHandler
{
	Game game;

	public TrackerHandler(Game g)
	{
		game = g;
	}

	public void SendInfoTo(IPEndPoint ip)
	{
		MessageBuffer msg = new MessageBuffer();
		msg.WriteString(Game.serverName);

		Game.server.SendExternal(msg, ip);
	}
}