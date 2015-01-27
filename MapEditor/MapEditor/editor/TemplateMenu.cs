using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TKTools;

namespace MapEditor
{
	public class TemplateMenu : IDisposable
	{
		public class TemplateButton : IDisposable
		{
			int index;
			public Template template;
			TemplateMenu menu;
			Vector2 offset;
			Vector2 size;

			Mesh buttonMesh;

			public Vector2 Position
			{
				get
				{
					return menu.Position + new Vector2(0, menu.Size.Y / 2 - offset.Y + menu.scrollAmount[menu.tabIndex]);
				}
			}

			public bool Hovered
			{
				get
				{
					return GetCollision(MouseInput.Current.PositionOrtho);
				}
			}

			public int Index
			{
				get
				{
					return index;
				}
				set
				{
					index = value;
					offset = new Vector2(0f, 1.5f + 1.5f * index);
				}
			}

			public TemplateButton(Template t, int index, TemplateMenu menu)
			{
				this.menu = menu;

				Index = index;
				template = t;
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
				if (Hovered)
				{
					if (MouseInput.ButtonPressed(MouseButton.Left))
						CloneToMap();
					if (MouseInput.ButtonPressed(MouseButton.Right) && MessageBox.Show("Are you sure you want to delete this template?", "Please confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
						menu.removeBuffer = this;

					if (KeyboardInput.KeyPressed(Key.R))
					{
						menu.editor.templateCreator.replaceReference = this;
						menu.editor.templateCreator.Active = true;
					}
				}
			}

			public void CloneToMap()
			{
				EditorObject obj = new EditorObject(menu.editor.ActiveLayer, template, menu.editor);
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
				Vector2 position = Position;
				DrawBox();

				template.Draw(position, 1f);

				if (Hovered)
				{
					template.Draw(position + new Vector2(4f, 0f), 5f);
				}
			}

			public void DrawBox()
			{
				bool hovered = Hovered;
				Vector2 position = Position;

				Color c = Color.White;

				GL.Enable(EnableCap.StencilTest);
				GL.Clear(ClearBufferMask.StencilBufferBit);

				GL.StencilFunc(StencilFunction.Always, 1, 0xff);
				GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);

				c.A = hovered ? 0.5f : 0.1f;

				buttonMesh.Color = c;
				
				buttonMesh.Reset();
				buttonMesh.Translate(position);
				buttonMesh.Scale(size);

				buttonMesh.Draw();

				GL.StencilFunc(StencilFunction.Notequal, 1, 0xff);

				c.A = hovered ? 0.8f : 0.4f;

				buttonMesh.Color = c;

				buttonMesh.Reset();
				buttonMesh.Translate(position);
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

		int tabIndex = 0;

		float[] scrollAmount = new float[10];
		float scrollSpeed = 0f;

		TemplateButton removeBuffer = null;

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
				if (GetCollision(MouseInput.Current.PositionOrtho)) return true;

				return false;
			}
		}

		public bool TabHovered
		{
			get
			{
				return GetTabCollision(MouseInput.Current.PositionOrtho);
			}
		}

		public bool Hidden
		{
			get
			{
				return editor.CurrentManipulator.Active || editor.ActiveLayer.ID == 0;
			}
		}

		public float ScrollAvailable
		{
			get
			{
				return Math.Max(0, (CurrentTab.Count * 1.5f + 1.5f) - Size.Y);
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

			for (int i = 0; i < scrollAmount.Length; i++)
				scrollAmount[i] = 0f;

			tabMesh = new Mesh(PrimitiveType.Quads);
			tabMesh.UIElement = true;
			tabMesh.Vertices = new Vector2[] {
				new Vector2(0, -0.5f),
				new Vector2(1, -0.5f),
				new Vector2(1, 0.5f),
				new Vector2(0, 0.5f)
			};
		}

		public bool GetCollision(Vector2 p)
		{
			Vector2 pos = Position;
			Vector2 size = Size;

			return (p.X > pos.X - size.X / 2 &&
				p.X < pos.X + size.X / 2 &&
				p.Y > pos.Y - size.Y / 2 &&
				p.Y < pos.Y + size.Y / 2);
		}

		public bool GetTabCollision(Vector2 position)
		{
			Vector2 tabSize = new Vector2(0.7f, Size.Y);
			Vector2 pos = Position + new Vector2(size.X / 2 + tabSize.X / 2, 0);

			return (position.X >= pos.X - tabSize.X / 2 &&
				position.X <= pos.X + tabSize.X / 2 &&
				position.Y >= pos.Y - tabSize.Y / 2 &&
				position.Y <= pos.Y + tabSize.Y / 2);
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
		}

		public void AddTemplate(Template t) { AddTemplate(t, tabIndex); }
		public void AddTemplate(Template t, int tabIndex)
		{
			int index = tabList[tabIndex].Count;
			tabList[tabIndex].Add(new TemplateButton(t, index, this));
		}

		public void RemoveTemplate(Template t)
		{
			TemplateButton button = null;

			foreach (List<TemplateButton> tl in tabList)
				foreach (TemplateButton tb in tl)
					if (tb.template == t) button = tb;

			if (button != null) RemoveTemplate(button);
		}
		void RemoveTemplate(TemplateButton tb)
		{
			foreach (List<TemplateButton> t in tabList)
			{
				if (t.Contains(tb))
				{
					int index = t.IndexOf(tb);

					for (int i = index+1; i < t.Count; i++)
					{
						t[i].Index -= 1;
						tb.Dispose();
						editor.DeleteTemplate(tb.template);
					}

					t.Remove(tb);
				}
			}
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
			}

			if (MouseInput.Current.Wheel != MouseInput.Previous.Wheel)
			{
				if (Hovered)
				{
					float delta = MouseInput.Current.Wheel - MouseInput.Previous.Wheel;

					scrollSpeed -= 4.5f * delta;
				}
				else if (TabHovered)
				{
					tabIndex -= (int)(MouseInput.Current.Wheel - MouseInput.Previous.Wheel);
				}
			}

			tabIndex = MathHelper.Clamp(tabIndex, 0, tabList.Length - 1);

			if (Math.Abs(scrollSpeed) > 0.001f)
			{
				scrollAmount[tabIndex] += scrollSpeed * Editor.delta;
				scrollSpeed -= scrollSpeed * 5 * Editor.delta;
			}

			scrollAmount[tabIndex] = MathHelper.Clamp(scrollAmount[tabIndex], 0, ScrollAvailable);

			removeBuffer = null;

			foreach (TemplateButton btn in CurrentTab)
				btn.Logic();

			if (removeBuffer != null) RemoveTemplate(removeBuffer);
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
		}
	}
}
