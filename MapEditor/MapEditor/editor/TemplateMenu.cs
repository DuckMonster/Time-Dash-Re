using OpenTK;
using OpenTK.Graphics.OpenGL;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKTools;

namespace MapEditor
{
	public class TemplateMenu : IDisposable
	{
		class TemplateButton : IDisposable
		{
			Template template;
			TemplateMenu menu;
			Vector2 offset;
			Vector2 size;

			Mesh buttonMesh;

			public Vector2 Position
			{
				get
				{
					return menu.Position + new Vector2(0, menu.Size.Y/2 - offset.Y);
				}
			}

			public bool Hovered
			{
				get
				{
					return GetCollision(MouseInput.Current.PositionOrtho);
				}
			}

			public TemplateButton(Template t, int index, TemplateMenu menu)
			{
				this.menu = menu;

				template = t;
				offset = new Vector2(0f, 1.5f + 1.5f * index);
				size = t.Size;

				buttonMesh = Mesh.Box;
				buttonMesh.UIElement = true;
			}

			public void Dispose()
			{
				buttonMesh.Dispose();
			}

			public bool GetCollision(Vector2 position)
			{
				Vector2 pos = Position;

				return (position.X >= pos.X - size.X / 2 &&
					position.X <= pos.X + size.X / 2 &&
					position.Y >= pos.Y - size.Y / 2 &&
					position.Y <= pos.Y + size.Y / 2);
			}

			public void Logic()
			{
				if (Hovered && MouseInput.ButtonPressed(OpenTK.Input.MouseButton.Left))
					CloneToMap();
			}

			public void CloneToMap()
			{
				EditorObject obj = new EditorObject(template, menu.editor);
				menu.editor.CreateObject(obj);
				menu.editor.DeselectAll();

				obj.Select();

				foreach (Vertex v in obj.Vertices)
					v.Move(MouseInput.Current.Position);

				menu.editor.SetEditMode(EditMode.Move);
				menu.editor.CurrentManipulator.Enable(MouseInput.Current);
			}

			public void Draw()
			{
				DrawBox();

				template.Draw(Position, 1f);

				if (Hovered)
				{
					template.Draw(Position + new Vector2(4f, 0f), 5f);
				}
			}

			public void DrawBox()
			{
				bool hovered = Hovered;

				Color c = Color.White;

				GL.Enable(EnableCap.StencilTest);
				GL.Clear(ClearBufferMask.StencilBufferBit);

				GL.StencilFunc(StencilFunction.Always, 1, 0xff);
				GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);

				c.A = hovered ? 0.5f : 0.1f;

				buttonMesh.Color = c;
				
				buttonMesh.Reset();
				buttonMesh.Translate(Position);
				buttonMesh.Scale(size);

				buttonMesh.Draw();

				GL.StencilFunc(StencilFunction.Notequal, 1, 0xff);

				c.A = hovered ? 0.8f : 0.4f;

				buttonMesh.Color = c;

				buttonMesh.Reset();
				buttonMesh.Translate(Position);
				buttonMesh.Scale(size + new Vector2(0.2f, 0.2f));

				buttonMesh.Draw();

				GL.Disable(EnableCap.StencilTest);
			}
		}

		List<TemplateButton> buttonList = new List<TemplateButton>();

		Editor editor;

		Vector2 position;
		Vector2 size;

		Mesh menuMesh = Mesh.Box;

		Vector2 TargetPosition
		{
			get
			{
				return new Vector2(-Editor.screenWidth/2 + 1.5f - 4f * (Hidden ? 1 : 0), 0f);
			}
		}

		public Vector2 Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}

		public Vector2 Size
		{
			get
			{
				return new Vector2(2f, Editor.screenHeight);
			}
			set
			{
				size = value;
			}
		}

		public bool Hovered
		{
			get
			{
				foreach (TemplateButton btn in buttonList)
					if (btn.Hovered) return true;

				return false;
			}
		}

		public bool Hidden
		{
			get
			{
				return editor.CurrentManipulator.Active;
			}
		}

		public TemplateMenu(Editor e)
		{
			editor = e;
			position = new Vector2(-Editor.screenWidth + 1.5f, 0f);
			size = new Vector2(2f, Editor.screenHeight * 2);

			menuMesh.Color = new Color(1, 1, 1, 0.2f);
			menuMesh.UIElement = true;
		}

		public void Dispose()
		{
			foreach (TemplateButton btn in buttonList)
				btn.Dispose();

			buttonList.Clear();
		}

		public void AddTemplate(Template t)
		{
			int index = buttonList.Count;
			buttonList.Add(new TemplateButton(t, index, this));
		}

		public void Logic()
		{
			position += (TargetPosition - position) * 5f * Editor.delta;

			foreach (TemplateButton btn in buttonList)
				btn.Logic();
		}

		public void Draw()
		{
			menuMesh.Reset();

			menuMesh.Translate(Position);
			menuMesh.Scale(Size);

			menuMesh.Draw();

			foreach (TemplateButton btn in buttonList)
				btn.Draw();
		}
	}
}
