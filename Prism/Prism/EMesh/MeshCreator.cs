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

	Model2D selectModel;
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

	public bool Active
	{
		get
		{
			return Math.Min(Rectangle.Width, Rectangle.Height) > 0.1f;
		}
	}

	public MeshCreator(Editor e)
	{
		editor = e;

		selectModel = new Model2D(new Vector2[] {
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(1, 1),
			new Vector2(0, 1)
		});

		selectModel.Color = new Color(1, 1, 1, 0.4f);
		selectModel.FillColor = true;
	}

	void UpdateMesh()
	{
		MeshDesign design = Program.meshDesigner.CurrentDesign;

		if (design.Tile == null)
		{
			selectModel.Texture = null;
		}
		else
		{
			TextureSet.Tile tile = design.Tile;
			SYS.RectangleF rect = tile.UV;

			selectModel.VertexUV = new Vector2[] {
				new Vector2(rect.X, rect.Y),
				new Vector2(rect.X + rect.Width, rect.Y),
				new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
				new Vector2(rect.X, rect.Y + rect.Height)
			};
			selectModel.Texture = tile.Texture;
		}
	}

	public void Logic()
	{
		if (editor.form.Focused)
		{
			if (mouse[MouseButton.Right] && (!editor.DisableSelect || origin != null))
			{
				if (origin == null)
				{
					origin = mouse.Position.Xy;
					UpdateMesh();
				}
			}
			else if (origin != null)
			{
				if (Active)
					editor.CreateMesh(Rectangle, Program.meshDesigner.CurrentDesign);

				origin = null;
			}
		}
	}

	public void Draw()
	{
		if (origin != null && Active)
		{
			SYS.RectangleF r = Rectangle;

			selectModel.Reset();

			selectModel.Translate(r.X, r.Y);
			selectModel.Scale(r.Width, r.Height);

			selectModel.Color = Color.Green;
			selectModel.TextureEnabled = false;
			selectModel.Draw(OpenTK.Graphics.OpenGL.PrimitiveType.LineLoop);

			selectModel.TextureEnabled = true;
			selectModel.Color = new Color(0, 1f, 0f, 0.2f);
			selectModel.Draw();
		}
	}
}