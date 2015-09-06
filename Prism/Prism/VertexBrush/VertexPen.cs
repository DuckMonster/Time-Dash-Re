using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections;
using System.Collections.Generic;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;
using TKTools.Mathematics;

public enum PenTarget
{
	All,
	Layer,
	Selected
}

public class VertexPen : Manipulator
{
	class PaintData
	{
		float weight;
		ColorHSL origin;

		public float Weight { get { return weight; } set { weight = value; } }
		public ColorHSL Origin { get { return origin; } set { origin = value; } }

		public PaintData(float w, ColorHSL o)
		{
			weight = w;
			origin = o;
		}
	}

	static readonly string vertexShader = @"
#version 330

in vec2 position;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

void main() {
	gl_Position = projection * view * model * vec4(position, 0.0, 1.0);
}
";

	static readonly string fragmentShader = @"
#version 330

uniform vec4 color;
out vec4 fragment;

void main() {
	fragment = color;
}
";

	static ShaderProgram shaderProgram;

	Dictionary<EVertex, PaintData> vertexData = new Dictionary<EVertex, PaintData>();

	VertexBrush brush = new VertexBrush(new ColorHSL(0f, 1f, 1f), 2f, 1f, 0.5f);
	Mesh mesh, crossMesh;

	BrushForm brushForm;

	public VertexBrush Brush
	{
		get { return brush; }
		set { brush = value; }
	}

	PenTarget target;
	public PenTarget Target
	{
		get { return target; }
		set { target = value; }
	}

	public IEnumerable<EVertex> SelectedVertices
	{
		get
		{
			switch (target) {
				case PenTarget.All:
					foreach (EMesh m in editor.Meshes)
						foreach (EVertex v in m)
							if ((v.Position - Position).Length <= brush.Size)
								yield return v;
					break;

				case PenTarget.Layer:
					if (editor.ActiveLayers.Count == 0) break;

					foreach(EMesh m in editor.ActiveLayers[0].Meshes)
						foreach(EVertex v in m)
							if ((v.Position - Position).Length <= brush.Size)
								yield return v;

					break;

				case PenTarget.Selected:
					foreach(EMesh m in editor.SelectedMeshes)
						foreach(EVertex v in m)
							if ((v.Position - Position).Length <= brush.Size)
								yield return v;
					break;
			}
		}
	}

	public override Vector2 Position
	{
		get { return mouse.Position.Xy; }
	}

	public VertexPen(Editor e)
		: base(e)
	{
		if (shaderProgram == null)
			shaderProgram = new ShaderProgram(vertexShader, fragmentShader);

		mesh = new Mesh(shaderProgram);
		crossMesh = new Mesh(shaderProgram);
		GenerateMesh(50);

		brushForm = new BrushForm(this);
	}

	void GenerateMesh(int resolution)
	{
		Vector2[] vectors = new Vector2[resolution];

		for (int i = 0; i < resolution; i++)
		{
			float angle = i / (float)resolution * 360f;
			vectors[i] = TKMath.GetAngleVector(angle);
		}

		mesh.GetAttribute<Vector2>("position").Data = vectors;

		crossMesh.GetAttribute<Vector2>("position").Data = new Vector2[]
		{
			new Vector2(-0.2f, 0f),
			new Vector2(0.2f, 0f),
			new Vector2(0f, 0.2f),
			new Vector2(0f, -0.2f)
		};
	}

	public void ShowPalette()
	{
		if (brushForm != null)
		{
			brushForm.Show();
			return;
		}

		brushForm = new BrushForm(this);
		brushForm.Show();
	}

	public void ClosePalette()
	{
		brushForm = null;
	}

	public override void Logic()
	{
		if (mouse.WheelDelta != 0)
		{
			if (keyboard.HasPrefix(KeyPrefix.Alt))
			{
				brush.Size += mouse.WheelDelta * 0.12f;
				if (brush.Size < 0) brush.Size = 0f;

				if (brushForm != null)
					brushForm.Size = brush.Size;
			}
			if (keyboard.HasPrefix(KeyPrefix.Control))
			{
				brush.Opacity += mouse.WheelDelta * 0.05f;
				brush.Opacity = MathHelper.Clamp(brush.Opacity, 0, 1);

				if (brushForm != null)
					brushForm.Opacity = brush.Opacity;
			}
			if (keyboard.HasPrefix(KeyPrefix.Shift))
			{
				brush.Hardness += mouse.WheelDelta * 0.05f;
				brush.Hardness = MathHelper.Clamp(brush.Hardness, 0, 1);

				if (brushForm != null)
					brushForm.Hardness = brush.Hardness;
			}
		}

		if (mouse[MouseButton.Left] && editor.form.Focused)
		{
			foreach (EVertex v in SelectedVertices)
			{
				float weight = 1 - (v.Position - Position).Length / brush.Size;

				if (weight < 1 - brush.Hardness)
					weight = weight / (1f - brush.Hardness);
				else
					weight = 1f;

				if (vertexData.ContainsKey(v))
				{
					if (vertexData[v].Weight > weight)
						continue;

					vertexData[v].Weight = weight;

				}
				else
					vertexData.Add(v, new PaintData(weight, v.HSL));

				v.Paint(ColorHSL.Blend(vertexData[v].Origin, brush.HSL, weight * brush.Opacity));

				DebugForm.debugString = "Weight: " + weight.ToString();
			}
		}
		if (!mouse[MouseButton.Left] && vertexData.Count > 0)
		{
			EVertex[] vertices = new EVertex[vertexData.Count];
			ColorHSL[] begins = new ColorHSL[vertexData.Count];
			ColorHSL[] ends = new ColorHSL[vertexData.Count];

			int index = 0;
			foreach (KeyValuePair<EVertex, PaintData> data in vertexData)
			{
				vertices[index] = data.Key;
				begins[index] = data.Value.Origin;
				ends[index] = data.Key.HSL;

				index++;
			}

			editor.ActiveLayers[0].History.Add(new PaintAction(vertices, begins, ends, editor.ActiveHistory));
			vertexData.Clear();
		}
	}

	public override void Draw()
	{
		Matrix4 model = Matrix4.CreateScale(brush.Size) * Matrix4.CreateTranslation(new Vector3(Position));

		shaderProgram["projection"].SetValue(editor.editorCamera.Projection);
		shaderProgram["view"].SetValue(editor.editorCamera.View);
		shaderProgram["model"].SetValue(model);

		shaderProgram["color"].SetValue(new Color(1f, 0f, 0f, brush.Opacity));

		mesh.Draw(PrimitiveType.Lines);
		shaderProgram["model"].SetValue(Matrix4.CreateScale(brush.Hardness) * model);
		mesh.Draw(PrimitiveType.LineLoop);
		shaderProgram["model"].SetValue(model);

		GL.LineWidth(2f);
		shaderProgram["color"].SetValue(Color.Red);
		shaderProgram["model"].SetValue(Matrix4.CreateScale(editor.editorCamera.Position.Z * 0.1f) * Matrix4.CreateTranslation(new Vector3(Position)));
		crossMesh.Draw(PrimitiveType.Lines);
		GL.LineWidth(1f);
	}
}