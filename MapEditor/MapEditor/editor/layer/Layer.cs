using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using TKTools;
namespace MapEditor
{
	public class Layer : IDisposable
	{
		protected int id;
		protected Editor editor;
		protected List<EditorObject> objectList = new List<EditorObject>();

		protected float zposition;

		protected Mesh buttonMesh = Mesh.Box;

		public List<EditorObject> Objects
		{
			get
			{
				return objectList;
			}
		}

		public int ID
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		public bool Active
		{
			get
			{
				return editor.ActiveLayer == this;
			}
		}

		public Vector2 ButtonPosition
		{
			get
			{
				float splitx = ((1f / SplitNmbr) * SplitIndex + 0.05f) * ButtonMaxSize.X;

				return new Vector2(Editor.screenWidth / 2 - 2f - zposition * 0.08f - ButtonMaxSize.X/2 + splitx, -Editor.screenHeight/2 + 2f + zposition * 0.08f);
			}
		}

		public Vector2 ButtonMaxSize
		{
			get
			{
				return new Vector2(1, 0.7f) * (3f - zposition * 0.1f);
			}
		}

		public Vector2 ButtonSize
		{
			get
			{
				return ButtonMaxSize * new Vector2(SplitSize, 1);
			}
		}

		public bool ButtonHovered
		{
			get
			{
				if (MouseInput.Current == null) return false;

				return buttonMesh.Polygon.Intersects(MouseInput.Current.PositionOrtho);
			}
		}

		public float Z
		{
			get
			{
				return zposition;
			}
		}

		public int SplitNmbr
		{
			get
			{
				int nmbrOfLayers = 0;

				foreach (Layer l in editor.layerList)
					if (Math.Abs(l.Z - Z) < 6) nmbrOfLayers++;

				return nmbrOfLayers;
			}
		}

		public float SplitSize
		{
			get
			{
				return 1f / SplitNmbr - 0.05f;
			}
		}

		public int SplitIndex
		{
			get
			{
				int index = 0;

				for (int i = 0; i < editor.layerList.Count; i++)
				{
					if (Math.Abs(editor.layerList[i].Z - Z) >= 6) continue;

					if (editor.layerList[i].ID > ID) index++;
				}

				return index;
			}
		}

		public Layer(int id, float zposition, Editor e)
		{
			this.id = id;
			this.zposition = zposition;
			editor = e;

			buttonMesh.UIElement = true;
			buttonMesh.Vertices = new Vector2[] {
				new Vector2(0, -0.5f),
				new Vector2(1, -0.5f),
				new Vector2(1, 0.5f),
				new Vector2(0, 0.5f)
			};
		}

		public virtual void Dispose()
		{
			foreach (EditorObject obj in objectList)
				obj.Dispose();

			objectList.Clear();
		}

		public virtual void Logic()
		{
			foreach (EditorObject obj in objectList)
				obj.Logic();
		}

		public void CreateObject(params EditorObject[] obj)
		{
			objectList.AddRange(obj);
			foreach (EditorObject o in obj)
			{
				o.UpdateMesh();
			}
		}

		public void MoveObject(EditorObject obj, int delta)
		{
			int index = objectList.IndexOf(obj);
			objectList.Remove(obj);
			objectList.Insert(MathHelper.Clamp(index + delta, 0, objectList.Count), obj);
		}

		public virtual void DuplicateSelected()
		{
			List<EditorObject> newObjects = new List<EditorObject>();

			foreach (EditorObject obj in objectList)
			{
				if (obj.Selected)
				{
					EditorObject copy = new EditorObject(this, obj, editor);
					newObjects.Add(copy);
				}
			}

			if (newObjects.Count > 0)
			{
				CreateObject(newObjects.ToArray());

				editor.DeselectAll();

				foreach (EditorObject obj in newObjects)
					editor.SelectAdd(obj.Vertices);
			}
		}

		public virtual void WriteToFile(BinaryWriter writer)
		{
			writer.Write(Z);
			writer.Write(objectList.Count);

			foreach (EditorObject obj in objectList)
				obj.WriteToFile(writer);
		}

		public virtual void ReadFromFile(BinaryReader reader)
		{
			int nmbr = reader.ReadInt32();

			for (int i = 0; i < nmbr; i++)
			{
				CreateObject(new EditorObject(this, reader, editor));
			}
		}

		public virtual void Draw()
		{
			foreach (EditorObject obj in objectList)
				obj.Draw();

			buttonMesh.Color = new Color(1, 1, 1, (ButtonHovered ? 0.8f : 0.4f) + (Active ? 0.5f : 0));

			buttonMesh.Reset();

			buttonMesh.Translate(ButtonPosition);
			buttonMesh.Scale(ButtonSize);
			if (Active) buttonMesh.Translate(0, 0.2f);

			buttonMesh.Draw();
		}
	}
}