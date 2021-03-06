﻿using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
using TKTools;
namespace MapEditor
{
	public class SolidLayer : Layer
	{
		public static Color[] colorList = new Color[] {
			Color.White,
			Color.Orange
		};

		Vector2 origin;
		Vector2 a, b;
		Mesh creatorMesh;
		Mesh colorMesh = Mesh.Box;

		int typeIndex = 0;

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

			colorMesh.UIElement = true;
		}

		public override void Dispose()
		{
			base.Dispose();
			creatorMesh.Dispose();
		}

		public void CreateSolid(Vector2 a, Vector2 b)
		{
			EditorObject obj;

			if (typeIndex == 0)
			{
				obj = new SolidObject(this, a, b, editor);
				objectList.Add(obj);
			}
			else
			{
				obj = new EventObject(this, a, b, editor);
				objectList.Add(obj);
			}

			obj.Select();
		}

		public void CreateSolid(BinaryReader reader)
		{
			bool isEvent = reader.ReadBoolean();

			if (isEvent)
				objectList.Add(new EventObject(this, reader, editor));
			else
				objectList.Add(new SolidObject(this, reader, editor));
		}

		public override void DuplicateSelected()
		{
			List<EditorObject> newObjects = new List<EditorObject>();

			foreach (EditorObject obj in objectList)
			{
				if (obj.Selected)
				{
					EditorObject copy = null;

					if (obj is SolidObject)
						copy = new SolidObject(this, obj, editor);
					if (obj is EventObject)
						copy = new EventObject(this, (obj as EventObject), editor);

					if (copy != null)
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

		public override void Logic()
		{
			base.Logic();

			if (MouseInput.ButtonPressed(MouseButton.Right) && !KeyboardInput.Current[Key.LAlt])
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

			if (KeyboardInput.KeyPressed(Key.Number1)) typeIndex = 0;
			if (KeyboardInput.KeyPressed(Key.Number2)) typeIndex = 1;
		}

		public override void WriteToFile(BinaryWriter writer)
		{
			writer.Write(objectList.Count);

			foreach (EditorObject obj in objectList)
			{
				obj.WriteToFile(writer);
			}
		}

		public override void ReadFromFile(BinaryReader reader)
		{
			int nmbr = reader.ReadInt32();

			for (int i = 0; i < nmbr; i++)
				CreateSolid(reader);
		}

		public override void Draw()
		{
			if (editor.preview || editor.hideSolids) return;

			foreach (EditorObject obj in objectList)
				obj.Draw();

			buttonMesh.Color = new Color(1, 1, 0, (ButtonHovered ? 0.8f : 0.4f) + (Active ? 0.5f : 0));

			buttonMesh.Reset();

			buttonMesh.Translate(ButtonPosition);
			buttonMesh.Scale(ButtonSize);
			if (Active) buttonMesh.Translate(0, 0.2f);

			buttonMesh.Draw();

			if (MouseInput.Current[MouseButton.Right] && Active)
			{
				creatorMesh.Color = colorList[typeIndex] * new Color(1, 1, 1, 0.5f);

				creatorMesh.Reset();
				creatorMesh.Translate(a);
				creatorMesh.Scale(b - a);
				creatorMesh.Draw();
			}

			if (Active)
			{
				Vector2 colorPosition = new Vector2(-Editor.screenWidth / 2 + 1f, Editor.screenHeight / 2 - 1f);

				for (int i = 0; i < colorList.Length; i++)
				{
					colorMesh.Color = colorList[i] * new Color(1, 1, 1, typeIndex == i ? 1f : 0.4f);

					colorMesh.Reset();

					colorMesh.Translate(colorPosition + new Vector2(1f * i, 0f));
					colorMesh.Scale(0.8f);

					colorMesh.Draw();
				}
			}
		}
	}
}