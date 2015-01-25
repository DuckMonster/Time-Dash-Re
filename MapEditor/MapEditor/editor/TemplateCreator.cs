using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using TKTools;
using DRAW = System.Drawing;
using System;

namespace MapEditor
{
	public class TemplateCreator : IDisposable
	{
		class Marker
		{
			TemplateCreator creator;
			Vector2 a, b;

			Mesh mesh;

			public DRAW.RectangleF UV
			{
				get
				{
					Vector2 pointa = new Vector2(a.X + 0.5f, 1 - (a.Y + 0.5f));
					Vector2 pointb = new Vector2(b.X + 0.5f, 1 - (b.Y + 0.5f));

					Vector2 max = new Vector2(Math.Max(pointa.X, pointb.X), Math.Max(pointa.Y, pointb.Y));
					Vector2 min = new Vector2(Math.Min(pointa.X, pointb.X), Math.Min(pointa.Y, pointb.Y));

					Vector2 size = max - min;

					return new DRAW.RectangleF(min.X, min.Y, size.X, size.Y);
				}
			}

			public bool Active
			{
				get
				{
					return (a - b).Length > 0.01f;
				}
				set
				{
					if (!value)
					{
						a = Vector2.Zero;
						b = Vector2.Zero;
					}
				}
			}

			public Marker(TemplateCreator creator)
			{
				this.creator = creator;

				mesh = new Mesh(PrimitiveType.Quads);

				mesh.Vertices = new Vector2[] {
					new Vector2(0, 0),
					new Vector2(1, 0),
					new Vector2(1, 1),
					new Vector2(0, 1)
				};

				mesh.UIElement = true;
				mesh.InvertColors = true;
			}

			public void Logic()
			{
				if (!KeyboardInput.Current[Key.LAlt])
				{
					if (MouseInput.ButtonPressed(MouseButton.Left)) a = (MouseInput.Current.PositionOrtho - creator.Position) / creator.Size.X;
					if (MouseInput.Current[MouseButton.Left]) b = (MouseInput.Current.PositionOrtho - creator.Position) / creator.Size.Y;
				}

				a = Vector2.Clamp(a, new Vector2(-0.5f, -0.5f), new Vector2(0.5f, 0.5f));
				b = Vector2.Clamp(b, new Vector2(-0.5f, -0.5f), new Vector2(0.5f, 0.5f));

				if (KeyboardInput.KeyPressed(Key.Enter) && Active)
				{
					creator.editor.CreateTemplate(creator.tilesetIndex, UV);
					Active = false;
				}

				DRAW.RectangleF meshRect = UV;

				mesh.Vertices = new Vector2[] {
					new Vector2(meshRect.X - .5f, 0.5f - meshRect.Y),
					new Vector2(meshRect.X -.5f + meshRect.Width, 0.5f - meshRect.Y),
					new Vector2(meshRect.X -.5f + meshRect.Width, 0.5f - (meshRect.Y + meshRect.Height)),
					new Vector2(meshRect.X -.5f, 0.5f - (meshRect.Y + meshRect.Height))
				};

				mesh.UV = new Vector2[] {
					new Vector2(meshRect.X, meshRect.Y),
					new Vector2(meshRect.X + meshRect.Width, meshRect.Y),
					new Vector2(meshRect.X + meshRect.Width, meshRect.Y + meshRect.Height),
					new Vector2(meshRect.X, meshRect.Y + meshRect.Height)
				};
			}

			public void Draw()
			{
				mesh.Texture = creator.CurrentTileset.Texture;

				mesh.Reset();

				mesh.Translate(creator.Position);
				mesh.Scale(creator.Size);

				mesh.Draw();
			}
		}

		Editor editor;

		Mesh mesh;
		Marker marker;
		bool active = false;

		int tilesetIndex = 0;

		float zoom = 1f, zoomSpeed = 0f;
		Vector2 positionOffset = Vector2.Zero;

		public bool Active
		{
			get
			{
				return active;
			}
			set
			{
				active = value;
			}
		}

		public Vector2 Position
		{
			get
			{
				return positionOffset;
			}
		}

		public Vector2 Size
		{
			get
			{
				float ratio = TextureRatio;

				if (ratio < 1)
					return new Vector2(1, ratio) * 10f * zoom;
				else
					return new Vector2(1 / ratio, 1) * 10f * zoom;
			}
		}

		public TilesetList.Tileset CurrentTileset
		{
			get
			{
				return editor.tilesetList[tilesetIndex];
			}
		}

		public float TextureRatio
		{
			get
			{
				Texture text = CurrentTileset.Texture;
				return (float)text.Height / text.Width;
			}
		}

		public TemplateCreator(Editor editor)
		{
			this.editor = editor;

			mesh = Mesh.Box;
			mesh.UIElement = true;

			marker = new Marker(this);
		}

		public void Dispose()
		{
			tilesetIndex = 0;
		}

		public void RemoveTileset(TilesetList.Tileset tileset)
		{
			editor.tilesetList.RemoveTileset(tileset);

			if (editor.tilesetList.Count > 0)
			{
				tilesetIndex = Math.Max(0, tilesetIndex - 1);
			}
			else
			{
				Active = false;
			}
		}

		public void Logic()
		{
			if (KeyboardInput.KeyPressed(Key.Tab))
			{
				if (editor.tilesetList.Count == 0)
					editor.tilesetList.PromptLoad();

				if (editor.tilesetList.Count > 0)
					active = true;
			}

			if (!Active) return;

			marker.Logic();

			if (KeyboardInput.KeyPressed(Key.N))
				editor.tilesetList.PromptLoad();

			if (KeyboardInput.KeyPressed(Key.Delete))
				RemoveTileset(CurrentTileset);

			if (KeyboardInput.KeyPressed(Key.Escape))
				active = false;

			if (KeyboardInput.Current[Key.LAlt])
			{
				if (KeyboardInput.KeyPressed(Key.Right))
					SwitchTileset(1);
				if (KeyboardInput.KeyPressed(Key.Left))
					SwitchTileset(-1);

				if (MouseInput.Current[MouseButton.Left])
				{
					positionOffset += MouseInput.Current.PositionOrtho - MouseInput.Previous.PositionOrtho;
				}
			}

			if (MouseInput.Current[MouseButton.Middle])
				positionOffset += MouseInput.Current.PositionOrtho - MouseInput.Previous.PositionOrtho;

			zoomSpeed += (MouseInput.Current.Wheel - MouseInput.Previous.Wheel) * 0.2f;

			zoom += zoomSpeed * 5 * Editor.delta;
			zoomSpeed -= zoomSpeed * 5 * Editor.delta;
			zoom = MathHelper.Clamp(zoom, 0.4f, 5f);
		}

		public void SwitchTileset(int delta)
		{
			if (editor.tilesetList[tilesetIndex + delta] == null) return;

			tilesetIndex += delta;
			marker.Active = false;
		}

		public void Draw()
		{
			if (!Active) return;

			mesh.Color = new Color(0, 0, 0, 0.8f);
			mesh.UsingTexture = false;

			mesh.Reset();
			mesh.Scale(new Vector2(Editor.screenWidth, Editor.screenHeight));
			mesh.Draw();

			DrawBox();
			Draw(CurrentTileset, 0);
		}

		public void Draw(TilesetList.Tileset tileset, int indexoffset)
		{
			mesh.Texture = tileset.Texture;

			mesh.Color = Color.White;
			mesh.UsingTexture = true;

			mesh.Reset();
			mesh.Translate(Position);
			mesh.Scale(Size);

			mesh.Draw();

			marker.Draw();
		}

		public void DrawBox()
		{
			mesh.UsingTexture = false;

			Color c = Color.White;

			GL.Enable(EnableCap.StencilTest);
			GL.Clear(ClearBufferMask.StencilBufferBit);

			GL.StencilFunc(StencilFunction.Always, 1, 0xff);
			GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);

			mesh.Color = c * new Color(1, 1, 1, 0.2f);

			mesh.Reset();
			mesh.Translate(positionOffset);
			mesh.Scale(Size);

			mesh.Draw();

			GL.StencilFunc(StencilFunction.Notequal, 1, 0xff);

			mesh.Color = c;

			mesh.Reset();
			mesh.Translate(positionOffset);
			mesh.Scale(Size + new Vector2(0.2f, 0.2f));

			mesh.Draw();

			GL.Disable(EnableCap.StencilTest);
		}
	}
}