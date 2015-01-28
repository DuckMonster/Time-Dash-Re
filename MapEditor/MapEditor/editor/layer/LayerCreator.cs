using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using TKTools;
namespace MapEditor
{
	public class LayerCreator
	{
		Editor editor;
		Mesh mesh;

		Mesh gridMesh, originMesh;

		float sliderPosition = 0f;

		bool active = false;

		public bool SliderHovered
		{
			get
			{
				return MouseInput.Current.X < sliderPosition + 0.2f && MouseInput.Current.X > sliderPosition - 0.2f;
			}
		}

		public bool Active
		{
			get
			{
				return active;
			}
		}

		public LayerCreator(Editor e)
		{
			editor = e;

			mesh = Mesh.Box;
			mesh.UIElement = true;

			gridMesh = new Mesh(PrimitiveType.Quads);
			gridMesh.Color = new Color(1, 1, 1, 0.2f);

			List<Vector2> gridVectorList = new List<Vector2>();

			for (int x = -50; x <= 50; x++)
			{
				gridVectorList.Add(new Vector2(1 * x - 0.01f, -50));
				gridVectorList.Add(new Vector2(1 * x + 0.01f, -50));
				gridVectorList.Add(new Vector2(1 * x + 0.01f, 50));
				gridVectorList.Add(new Vector2(1 * x - 0.01f, 50));

				gridVectorList.Add(new Vector2(-50, 1 * x - 0.01f));
				gridVectorList.Add(new Vector2(-50, 1 * x + 0.01f));
				gridVectorList.Add(new Vector2(50, 1 * x + 0.01f));
				gridVectorList.Add(new Vector2(50, 1 * x - 0.01f));
			}

			gridMesh.Vertices = gridVectorList.ToArray();

			originMesh = new Mesh(PrimitiveType.Quads);
			originMesh.Color = new Color(1, 1, 1, 0.4f);

			gridVectorList.Clear();

			gridVectorList.Add(new Vector2(-0.02f, -50));
			gridVectorList.Add(new Vector2(+0.02f, -50));
			gridVectorList.Add(new Vector2(+0.02f, 50));
			gridVectorList.Add(new Vector2(-0.02f, 50));

			gridVectorList.Add(new Vector2(-50, -0.02f));
			gridVectorList.Add(new Vector2(-50, +0.02f));
			gridVectorList.Add(new Vector2(50, +0.02f));
			gridVectorList.Add(new Vector2(50, -0.02f));

			originMesh.Vertices = gridVectorList.ToArray();

			gridMesh.Color = new Color(1, 1, 0, 0.4f);
			originMesh.Color = new Color(1, 1, 0, 0.9f);
		}

		public void Logic()
		{
			if (!Active && KeyboardInput.KeyPressed(Key.L) && KeyboardInput.Current[Key.LControl])
			{
				active = true;
				sliderPosition = 0f;
				editor.SetActiveLayer(editor.layerList[0]);
			}

			if (!Active) return;

			if (MouseInput.Current[OpenTK.Input.MouseButton.Left])
			{
				sliderPosition = MouseInput.Current.X;
				if (KeyboardInput.Current[Key.LControl])
				{
					sliderPosition *= 4f;
					sliderPosition = (float)Math.Round(sliderPosition);
					sliderPosition /= 4f;
				}
			}

			if (KeyboardInput.KeyPressed(Key.Enter))
			{
				editor.CreateLayer(-sliderPosition * 10f);
				active = false;
			}

			if (KeyboardInput.KeyPressed(Key.Escape))
				active = false;
		}

		public void Draw()
		{
			if (!Active) return;

			GL.Enable(EnableCap.DepthTest);
			gridMesh.Reset();
			originMesh.Reset();

			gridMesh.Translate(0, 0, sliderPosition * 10f);
			originMesh.Translate(0, 0, sliderPosition * 10f);

			gridMesh.Draw();
			originMesh.Draw();
			GL.Disable(EnableCap.DepthTest);

			mesh.Color = new Color(0, 0, 0, 0.1f);

			mesh.Reset();
			mesh.Scale(Editor.screenWidth, Editor.screenHeight);
			mesh.Draw();

			mesh.Color = new Color(1, 1, 1, SliderHovered ? 1f : 0.5f);

			mesh.Reset();
			mesh.Translate(sliderPosition, 0);
			mesh.Scale(0.4f, 2f);
			mesh.Draw();
		}
	}
}