using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;
using TKTools;

namespace MapEditor
{
	public class EventObject : EditorObject
	{
		int eventID = 0;
		public Color? color = null;

		public int EventID
		{
			get
			{
				return eventID;
			}
			set
			{
				eventID = value;
			}
		}

		public override Color Color
		{
			get
			{
				if (color == null)
					return base.Color * Color.Orange;
				else
					return base.Color * color.Value;
			}
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
		}

		public EventObject(Layer layer, BinaryReader reader, Editor e)
			: base(layer, e)
		{
			eventID = reader.ReadInt32();

			color = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

			foreach (Vertex v in vertices)
			{
				v.Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
			}
		}

		public override void Logic()
		{
			base.Logic();
		}

		public override void WriteToFile(BinaryWriter writer)
		{
			writer.Write(true);

			writer.Write(EventID);
			writer.Write(Color.R);
			writer.Write(Color.G);
			writer.Write(Color.B);

			foreach(Vertex v in Vertices)
			{
				writer.Write(v.Position.X);
				writer.Write(v.Position.Y);
			}
		}

		public override void Draw()
		{
			if (layer.Z < editor.ActiveLayer.Z || editor.preview) return;

			GL.Enable(EnableCap.DepthTest);
			mesh.Color = Color;
			mesh.UsingTexture = true;
			mesh.Reset();
			mesh.Translate(0, 0, -layer.Z);
			mesh.Draw();
			GL.Disable(EnableCap.DepthTest);

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