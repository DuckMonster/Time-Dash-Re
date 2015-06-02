using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.IO;
using TKTools;

namespace MapEditor
{
	public class EventTemplate
	{
		public static EventTemplate blankTemplate = new EventTemplate(-1, "[UNDEFINED]", new Color(1f, 1f, 1f, 0.2f));

		public static List<EventTemplate> eventList = new List<EventTemplate>();

		public static EventTemplate GetEvent(int id)
		{
			EventTemplate t = eventList.Find((x) => x.ID == id);
			return t;
		}

		public static int GetEventIndex(int id)
		{
			return eventList.FindIndex((x) => x.ID == id);
		}

		public static void CreateEvent(string name, int id, Color color)
		{
			if (EventExists(id)) return;

			eventList.Add(new EventTemplate(id, name, color));
		}

		public static bool EventExists(int id)
		{
			return eventList.Exists(x => x.ID == id);
		}

		public static void SaveTo(BinaryWriter writer)
		{
			writer.Write(eventList.Count);
			foreach(EventTemplate t in eventList)
			{
				writer.Write(t.ID);
				writer.Write(t.Name);

				writer.Write(t.Color.R);
				writer.Write(t.Color.G);
				writer.Write(t.Color.B);
			}
		}

		public static void LoadFrom(BinaryReader reader)
		{
			int nmbr = reader.ReadInt32();

			for(int i=0; i<nmbr; i++)
			{
				int id = reader.ReadInt32();
				string name = reader.ReadString();
				Color color = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

				CreateEvent(name, id, color);
			}
		}

		int eventID;
		string eventName;
		Color eventColor;

		public int ID
		{
			get { return eventID; }
		}
		public string Name
		{
			get { return eventName; }
		}
		public Color Color
		{
			get { return eventColor; }
		}

		public EventTemplate(int eventID, string eventName, Color eventColor)
		{
			this.eventID = eventID;
			this.eventName = eventName;
			this.eventColor = eventColor;
		}

		public void Edit(int id, string name, Color color)
		{
			eventID = id;
			eventName = name;
			eventColor = color;
		}
	}

	public class EventObject : EditorObject
	{
		TextBox nameBox = new TextBox(60f), paramBox = new TextBox(30f);
		Mesh nameMesh = Mesh.Box, paramMesh = Mesh.Box;

		EventTemplate eventTemplate;
		public EventTemplate Template
		{
			get
			{
				if (eventTemplate == null)
					return EventTemplate.blankTemplate;
				else
					return eventTemplate;
			}
		}

		public List<int> parameters = new List<int>();

		public int EventID
		{
			get
			{
				return Template.ID;
			}
		}

		public override Color Color
		{
			get
			{
				return Template.Color;
			}
		}

		public string Name
		{
			get { return Template.Name; }
		}

		public EventObject(Layer layer, Vector2 a, Vector2 b, Editor e)
			:base(layer, e)
		{
			Vector2 sizex = (b - a) * new Vector2(1, 0);
			Vector2 sizey = (b - a) * new Vector2(0, 1);

			Vertices[0] = new Vertex(a, Vector2.Zero, e);
			Vertices[1] = new Vertex(a + sizex, Vector2.Zero, e);
			Vertices[2] = new Vertex(a + sizex + sizey, Vector2.Zero, e);
			Vertices[3] = new Vertex(a + sizey, Vector2.Zero, e);

			nameBox.VerticalAlign = TextBox.VerticalAlignment.Center;
			nameBox.HorizontalAlign = TextBox.HorizontalAlignment.Center;

			paramBox.VerticalAlign = TextBox.VerticalAlignment.Top;
			paramBox.HorizontalAlign = TextBox.HorizontalAlignment.Center;
		}

		public EventObject(Layer layer, BinaryReader reader, Editor e)
			: base(layer, e)
		{
			nameBox.VerticalAlign = TextBox.VerticalAlignment.Center;
			nameBox.HorizontalAlign = TextBox.HorizontalAlignment.Center;

			paramBox.VerticalAlign = TextBox.VerticalAlignment.Top;
			paramBox.HorizontalAlign = TextBox.HorizontalAlignment.Center;
			
			int tempID = reader.ReadInt32();

			if (tempID != -1)
				SetTemplate(EventTemplate.GetEvent(tempID));

			int paramNmbr = reader.ReadInt32();

			int[] parList = new int[paramNmbr];

			for (int i = 0; i < paramNmbr; i++)
				parList[i] = reader.ReadInt32();

			SetParameters(parList);

			foreach (Vertex v in vertices)
			{
				v.Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
			}
		}
		public EventObject(Layer layer, EventObject copy, Editor e)
			:base(layer, copy, e)
		{
			nameBox.VerticalAlign = TextBox.VerticalAlignment.Center;
			nameBox.HorizontalAlign = TextBox.HorizontalAlignment.Center;

			paramBox.VerticalAlign = TextBox.VerticalAlignment.Top;
			paramBox.HorizontalAlign = TextBox.HorizontalAlignment.Center;

			SetTemplate(copy.Template);
			SetParameters(copy.parameters);
		}

		public void SetTemplate(EventTemplate temp)
		{
			eventTemplate = temp;
		}

		public void SetParameters(IEnumerable<int> p)
		{
			parameters.Clear();
			parameters.AddRange(p);

			string pstr = "";

			for(int i=0; i<parameters.Count; i++)
			{
				pstr += parameters[i].ToString();
				if (i < parameters.Count - 1)
					pstr += ", ";
			}

			paramBox.Text = pstr;

			paramMesh.Vertices = paramBox.Vertices;
			paramMesh.UV = paramBox.UV;
			paramMesh.Texture = paramBox.Texture;
		}

		public override void Logic()
		{
			nameBox.Text = Name;
			base.Logic();
		}

		public override void WriteToFile(BinaryWriter writer)
		{
			writer.Write(true);

			writer.Write(eventTemplate == null ? -1 : eventTemplate.ID);
			writer.Write(parameters.Count);
			foreach (int i in parameters)
				writer.Write(i);

			foreach(Vertex v in Vertices)
			{
				writer.Write(v.Position.X);
				writer.Write(v.Position.Y);
			}
		}

		public override void Draw()
		{
			if (layer.Z < editor.ActiveLayer.Z || editor.preview || !layer.Active) return;

			GL.Enable(EnableCap.DepthTest);

			mesh.Color = Color;
			mesh.UsingTexture = true;
			mesh.Reset();
			mesh.Translate(0, 0, -layer.Z);
			mesh.Draw();

			GL.Disable(EnableCap.DepthTest);

			nameMesh.Vertices = nameBox.Vertices;
			nameMesh.UV = nameBox.UV;
			nameMesh.Texture = nameBox.Texture;

			Vector2 center = mesh.Polygon.Center;

			nameMesh.Reset();
			nameMesh.Translate(center);
			nameMesh.Draw();

			if (parameters.Count > 0)
			{
				paramMesh.Reset();
				paramMesh.Translate(center + new Vector2(0, -0.4f));
				paramMesh.Draw();
			}

			if (Hovered)
			{
				mesh.UsingTexture = false;
				mesh.Color = new Color(0, 1, 0, 0.2f);

				mesh.Draw();
			}

			if (Active) DrawVertices();
		}
	}
}