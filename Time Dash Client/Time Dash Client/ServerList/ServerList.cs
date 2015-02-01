using EZUDP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TKTools;
public class ServerList
{
	class ServerButton : IDisposable
	{
		int index;

		IPEndPoint endpoint;

		TextDrawer textDrawer;
		Mesh buttonMesh;

		string serverName;
		string mapName;
		int nmbrOfPlayers;

		public ServerButton(int index, string ip)
		{
			this.index = index;

			endpoint = new IPEndPoint(IPAddress.Parse(ip), Port.UDP);

			textDrawer = new TextDrawer(2000, 500);
			textDrawer.Write(ip, 0.5f, 0.5f, 1f);

			buttonMesh = new Mesh(textDrawer);
			buttonMesh.Orthographic = true;

			FetchInfo();
		}

		public void FetchInfo()
		{

			using (UdpClient client = new UdpClient())
			{
				try
				{
					client.Client.ReceiveTimeout = 500;
					client.Connect(endpoint);

					MessageBuffer msg = new MessageBuffer();
					msg.WriteShort((short)Protocol.RequestInfo);

					client.Send(msg.Array, msg.Size);
					msg = new MessageBuffer(client.Receive(ref endpoint));

					serverName = msg.ReadString();
					mapName = msg.ReadString();
					nmbrOfPlayers = msg.ReadInt();

					textDrawer.Clear();
					textDrawer.Write(string.Format(serverName + " | {0} | {1}/6", mapName, nmbrOfPlayers), 0.5f, 0.5f, 0.1f);
				}
				catch (Exception e)
				{
					Console.WriteLine("Couldn't connect to " + endpoint.Address.ToString());
					Console.WriteLine(e);
				}
			}

		}

		public void Dispose()
		{
			textDrawer.Dispose();
			buttonMesh.Dispose();
		}

		public void Logic()
		{

		}

		public void Draw()
		{
			buttonMesh.Reset();

			buttonMesh.Translate(0.5f, -Game.windowRatio/2);
			buttonMesh.Translate(0f, 0.1f * index);
			buttonMesh.Scale(0.2f);

			buttonMesh.Draw();
		}
	}

	List<ServerButton> buttonList = new List<ServerButton>();
	Game game;

	UdpClient client;
	IPEndPoint trackerEP = new IPEndPoint(Dns.GetHostAddresses("tracker.timedashgame.com")[0], 1260);

	public ServerList(Game g)
	{
		game = g;
		client = new UdpClient();

		FetchServers();
	}

	public void Logic()
	{
		if (buttonList != null)
			foreach (ServerButton btn in buttonList)
				btn.Logic();
	}

	public void Draw()
	{
		if (buttonList != null)
			foreach (ServerButton btn in buttonList)
				btn.Draw();
	}

	public void FetchServers()
	{
		try
		{
			foreach (ServerButton btn in buttonList)
				btn.Dispose();

			buttonList.Clear();
			buttonList = null;

			client.Send(new byte[] { 0 }, 1, trackerEP);
			MessageBuffer response = new MessageBuffer(client.Receive(ref trackerEP));

			int nmbr = response.ReadInt();

			buttonList = new List<ServerButton>(nmbr);

			for (int i = 0; i < nmbr; i++)
				buttonList.Add(new ServerButton(i, response.ReadString()));
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
	}
}