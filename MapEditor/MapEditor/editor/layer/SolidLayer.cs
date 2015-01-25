using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
using TKTools;
namespace MapEditor
{
	public class SolidLayer : Layer
	{
		Vector2 origin;
		Vector2 a, b;
		Mesh creatorMesh;

		public SolidLayer(Editor e)
			: base(0, 0, e)
		{
			creatorMesh = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.Quads);
			creatorMesh.Vertices = new Vector2[] {
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1)
			};

			creatorMesh.Color = new Color(1, 1, 1, 0.4f);
		}

		public override void Dispose()
		{
			base.Dispose();
			creatorMesh.Dispose();
		}

		public void CreateSolid(Vector2 a, Vector2 b)
		{
			SolidObject obj = new SolidObject(this, a, b, editor);
			objectList.Add(obj);

			obj.Select();
		}

		public void CreateSolid(BinaryReader reader)
		{
			objectList.Add(new SolidObject(this, reader, editor));
		}

		public override void DuplicateSelected()
		{
			List<SolidObject> newObjects = new List<SolidObject>();

			foreach (EditorObject obj in objectList)
			{
				if (obj.Selected)
				{
					SolidObject copy = new SolidObject(this, obj, editor);
					newObjects.Add(copy);
				}
			}

			if (newObjects.Count > 0)
			{
				CreateObject(newObjects.ToArray());

				editor.DeselectAll();

				foreach (SolidObject obj in newObjects)
					editor.SelectAdd(obj.Vertices);
			}
		}

		public override void Logic()
		{
			base.Logic();

			if (MouseInput.ButtonPressed(MouseButton.Right))
			{
				origin = new Vector2((float)Math.Floor(MouseInput.Current.Position.X), (float)Math.Ceiling(MouseInput.Current.Position.Y));
				a = origin;
				b = origin + new Vector2(1, 1);
			}
			if (MouseInput.Current[MouseButton.Right])
			{
				Vector2 pos = new Vector2((float)Math.Floor(MouseInput.Current.Position.X), (float)Math.Ceiling(MouseInput.Current.Position.Y));
				if (pos.X <= origin.X)
				{
					a.X = pos.X;
					b.X = origin.X + 1;
				}
				else
				{
					b.X = pos.X + 1;
					a.X = origin.X;
				}

				if (pos.Y >= origin.Y)
				{
					a.Y = pos.Y;
					b.Y = origin.Y - 1;
				}
				else
				{
					b.Y = pos.Y - 1;
					a.Y = origin.Y;
				}
			}

			if (MouseInput.ButtonReleased(MouseButton.Right))
			{
				CreateSolid(a, b);
			}
		}

		public override void WriteToFile(System.IO.BinaryWriter writer)
		{
			writer.Write(objectList.Count);

			foreach (EditorObject obj in objectList)
			{
				obj.WriteToFile(writer);
			}
		}

		public override void ReadFromFile(System.IO.BinaryReader reader)
		{
			int nmbr = reader.ReadInt32();

			for (int i = 0; i < nmbr; i++)
				CreateSolid(reader);
		}

		public override void Draw()
		{
			foreach (EditorObject obj in objectList)
				obj.Draw();

			buttonMesh.Color = new Color(1, 1, 0, (ButtonHovered ? 0.8f : 0.4f) + (Active ? 0.5f : 0));

			buttonMesh.Reset();

			buttonMesh.Translate(ButtonPosition);
			buttonMesh.Scale(ButtonSize);
			if (Active) buttonMesh.Translate(0, 0.2f);

			buttonMesh.Draw();

			if (MouseInput.Current[MouseButton.Right])
			{
				creatorMesh.Reset();
				creatorMesh.Translate(a);
				creatorMesh.Scale(b - a);
				creatorMesh.Draw();
			}
		}
	}
}