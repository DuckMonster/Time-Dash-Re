using OpenTK;
using OpenTK.Input;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;
using System;
using SYS = System.Drawing;

public class MeshCreator
{
	Editor editor;

	Mesh selectMesh;
	MouseWatch mouse = Editor.mouse;
	KeyboardWatch keyboard = Editor.keyboard;

	Vector2? origin;

	SYS.RectangleF Rectangle
	{
		get
		{
			Vector2 o = origin.Value, m = mouse.Position.Xy;

			Vector2 ori = new Vector2(Math.Min(o.X, m.X), Math.Min(o.Y, m.Y));
			Vector2 tar = new Vector2(Math.Max(o.X, m.X), Math.Max(o.Y, m.Y));
			Vector2 size = tar - ori;

			if (keyboard[Key.LControl])
			{
				ori = new Vector2((float)Math.Floor(ori.X), (float)Math.Floor(ori.Y));
				tar = new Vector2((float)Math.Ceiling(tar.X), (float)Math.Ceiling(tar.Y));
				size = tar - ori;
			}

			size = tar - ori;
			return new SYS.RectangleF(ori.X, ori.Y, size.X, size.Y);
		}
	}

	public MeshCreator(Editor e)
	{
		editor = e;

		selectMesh = new Mesh(new Vector2[] {
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(1, 1),
			new Vector2(0, 1)
		});

		selectMesh.Color = new Color(1, 1, 1, 0.4f);
		selectMesh.FillColor = true;
	}

	void UpdateMesh()
	{
		if (TilePicker.selectedTile == null)
		{
			selectMesh.Texture = null;
		}
		else
		{
			TextureSet.Tile tile = TilePicker.selectedTile;
			SYS.RectangleF rect = tile.UV;

			selectMesh.UV = new Vector2[] {
				new Vector2(rect.X, rect.Y),
				new Vector2(rect.X + rect.Width, rect.Y),
				new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
				new Vector2(rect.X, rect.Y + rect.Height)
			};
			selectMesh.Texture = tile.Texture;
		}
	}

	public void Logic()
	{
		if (mouse[MouseButton.Right] && !editor.DisableSelect)
		{
			if (origin == null)
			{
				origin = mouse.Position.Xy;
				UpdateMesh();
			}
		} else if (origin != null)
		{
			editor.CreateMesh(Rectangle, TilePicker.selectedTile);
			origin = null;
		}
	}

	public void Draw()
	{
		if (origin != null)
		{
			SYS.RectangleF r = Rectangle;

			selectMesh.Reset();

			selectMesh.Translate(r.X, r.Y);
			selectMesh.Scale(r.Width, r.Height);

			selectMesh.Color = Color.Green;
			selectMesh.TextureEnabled = false;
			selectMesh.Draw(OpenTK.Graphics.OpenGL.PrimitiveType.LineLoop);

			selectMesh.TextureEnabled = true;
			selectMesh.Color = new Color(0, 1f, 0f, 0.5f);
			selectMesh.Draw();
		}
	}
}