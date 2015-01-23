using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
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
			public Template template;
			TemplateMenu menu;
			Vector2 offset;
			Vector2 size;

			Mesh buttonMesh;

			public Vector2 Position
			{
				get
				{
					return menu.Position + new Vector2(0, menu.Size.Y/2 - offset.Y) - new Vector2(0, menu.TotalScroll * 1.5f * menu.scrollPosition);
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

		List<TemplateButton>[] tabList = new List<TemplateButton>[10];

		Editor editor;

		Vector2 position;
		Vector2 size;

		Mesh menuMesh = Mesh.Box;
		Mesh tabMesh;
		Mesh scrollMesh;

		int tabIndex = 0;

		float scrollPosition = 0f;

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
				foreach (TemplateButton btn in CurrentTab)
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

		public float TotalScroll
		{
			get
			{
				float sizey = Size.Y;

				float totalButtonSize = CurrentTab.Count * 1.5f;
				float buttonSizeAvailable = Size.Y / 1.5f;
				return (float)Math.Max(1, (totalButtonSize - buttonSizeAvailable) / buttonSizeAvailable);
			}
		}

		List<TemplateButton> CurrentTab
		{
			get
			{
				return tabList[tabIndex];
			}
		}

		public TemplateMenu(Editor e)
		{
			editor = e;
			position = new Vector2(-Editor.screenWidth + 1.5f, 0f);
			size = new Vector2(2f, Editor.screenHeight * 2);

			menuMesh.Color = new Color(1, 1, 1, 0.2f);
			menuMesh.UIElement = true;

			for (int i = 0; i < tabList.Length; i++)
				tabList[i] = new List<TemplateButton>();

			tabMesh = new Mesh(PrimitiveType.Quads);
			tabMesh.UIElement = true;
			tabMesh.Vertices = new Vector2[] {
				new Vector2(0, -0.5f),
				new Vector2(1, -0.5f),
				new Vector2(1, 0.5f),
				new Vector2(0, 0.5f)
			};

			scrollMesh = Mesh.Box;
			scrollMesh.UIElement = true;
		}
		
		public void Dispose()
		{
			foreach (List<TemplateButton> tab in tabList)
			{
				foreach (TemplateButton btn in tab)
					btn.Dispose();

				tab.Clear();
			}

			tabMesh.Dispose();
			scrollMesh.Dispose();
		}

		public void AddTemplate(Template t) { AddTemplate(t, tabIndex); }
		public void AddTemplate(Template t, int tabIndex)
		{
			int index = tabList[tabIndex].Count;
			tabList[tabIndex].Add(new TemplateButton(t, index, this));
		}

		public void WriteToFile(BinaryWriter writer)
		{
			int n = 0;

			foreach (List<TemplateButton> tab in tabList)
				n += tab.Count;

			writer.Write(n);

			for (int i = 0; i < tabList.Length; i++)
				foreach (TemplateButton btn in tabList[i])
				{
					writer.Write(i);
					writer.Write(btn.template.ID);
				}
		}

		public void ReadFromFile(BinaryReader reader)
		{
			int nmbr = reader.ReadInt32();

			for (int i = 0; i < nmbr; i++)
			{
				int tab = reader.ReadInt32(), temp = reader.ReadInt32();

				AddTemplate(editor.templateList[temp], tab);
			}
		}

		public void Logic()
		{
			position += (TargetPosition - position) * 5f * Editor.delta;

			//Change tab
			if (KeyboardInput.Current[Key.LAlt])
			{
				if (KeyboardInput.KeyPressed(Key.Down)) tabIndex++;
				if (KeyboardInput.KeyPressed(Key.Up)) tabIndex--;

				tabIndex = MathHelper.Clamp(tabIndex, 0, tabList.Length - 1);
			}

			foreach (TemplateButton btn in CurrentTab)
				btn.Logic();
		}

		public void Draw()
		{
			menuMesh.Reset();

			menuMesh.Translate(Position);
			menuMesh.Scale(Size);

			menuMesh.Draw();

			foreach (TemplateButton btn in CurrentTab)
				btn.Draw();

			Vector2 tabPosition = Position + new Vector2(Size.X / 2, Size.Y / 2 - 1f);

			for (int i = 0; i < tabList.Length; i++)
			{
				bool selected = tabIndex == i;

				tabMesh.Color = new Color(1, 1, 1, selected ? 0.8f : 0.4f);

				tabMesh.Reset();

				tabMesh.Translate(tabPosition - new Vector2(0, 1.0f * i));
				tabMesh.Scale(selected ? 0.6f : 0.3f, 0.8f);

				tabMesh.Draw();
			}

			tabMesh.Reset();
			tabMesh.Translate(Position + new Vector2(-Size.X / 2, Size.Y/2 - (1f / TotalScroll)/2 * Size.Y));
			tabMesh.Scale(0.2f, (1f / TotalScroll) * Size.Y);

			tabMesh.Draw();
		}
	}
}
