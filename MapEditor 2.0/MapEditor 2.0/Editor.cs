using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections.Generic;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;

public class Editor : Context
{
	public static Editor CurrentEditor;
	public static MouseWatch mouse;
	public static KeyboardWatch keyboard;

	CameraControl cameraControl;

	List<EMesh> meshList = new List<EMesh>();
	public List<EMesh> selectedMeshes = new List<EMesh>();
	public List<EVertex> selectedVertices = new List<EVertex>();
	public Camera editorCamera;

	bool vertexSelection = false;
	public bool VertexSelection
	{
		get { return vertexSelection; }
	}

	Manipulator m;

	Mesh dragSelectMesh;
	Vector2? dragSelectOrigin = null, dragSelectOriginBuffer = null;

	Mesh gridMesh;

	public bool BlockSelect
	{
		get { return m.Active || m.Hovered || keyboard[Key.LAlt]; }
	}

	public bool DragSelecting
	{
		get
		{
			return dragSelectOrigin != null;
		}
	}

	public Polygon SelectArea
	{
		get
		{
			if (!DragSelecting) return new Polygon(new Vector3[] { mouse.Position });

			Vector3 s = mouse.Position - new Vector3(dragSelectOrigin.Value);

			return new Polygon(new Vector3[] {
				new Vector3(dragSelectOrigin.Value),
				new Vector3(dragSelectOrigin.Value) + s * new Vector3(1, 0, 0),
				new Vector3(dragSelectOrigin.Value) + s * new Vector3(1, 1, 0),
				new Vector3(dragSelectOrigin.Value) + s * new Vector3(0, 1, 0)
			});
		}
	}

	public List<EVertex> SelectedVertices
	{
		get
		{
			List<EVertex> list = new List<EVertex>(selectedMeshes.Count * 4);

			foreach (EMesh e in selectedMeshes)
				foreach (EVertex v in e)
					list.Add(v);

			if (VertexSelection)
				list.AddRange(selectedVertices);

			return list;
		}
	}

	public Editor()
	{
		CurrentEditor = this;

		OnBegin += Begin;
		OnUpdate += Update;
		OnRender += Render;

		editorCamera = new Camera();
		editorCamera.Use();

		dragSelectMesh = new Mesh(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) });
		dragSelectMesh.Color = new Color(1f, 1f, 1f, 0.4f);

		mouse = new MouseWatch();
		mouse.Perspective = editorCamera;

		keyboard = new KeyboardWatch();

		m = new ManipulatorTranslate(this);

		gridMesh = new Mesh();
		gridMesh.PrimitiveType = PrimitiveType.Lines;

		List<Vector2> gridVertices = new List<Vector2>();
		for (int i = -50; i <= 50; i++)
		{
			gridVertices.Add(new Vector2(-50f, i));
			gridVertices.Add(new Vector2(50f, i));
			gridVertices.Add(new Vector2(i, -50f));
			gridVertices.Add(new Vector2(i, 50f));
		}

		gridMesh.Vertices2 = gridVertices.ToArray();

		cameraControl = new CameraControl(this);
	}

	void Begin()
	{
		CreateMesh(Vector2.Zero);
		CreateMesh(Vector2.Zero);
	}

	public void Select(Polygon selection)
	{
		if (VertexSelection)
		{
			if (SelectVerticesAt(selection) == 0)
				SelectMeshesAt(selection);
		}
		else SelectMeshesAt(selection);
	}

	public int SelectMeshesAt(Polygon p)
	{
		List<EMesh> sel = GetMeshesAtSelection(p);

		if (p.pointList.Count == 1 && sel.Count > 0)
			SelectMesh(sel[0]);
		else
			SelectMesh(sel);

		return sel.Count;
	}
	public int SelectVerticesAt(Polygon p)
	{
		List<EVertex> sel = GetVerticesAtSelection(p);
		if (p.pointList.Count == 1 && sel.Count > 0)
			SelectVertex(sel[0]);
		else
			SelectVertex(sel);

		return sel.Count;
	}

	public List<EMesh> GetMeshesAtSelection(Polygon selection)
	{
		List<EMesh> list = new List<EMesh>();

		foreach (EMesh e in meshList)
			if (e.Intersects(selection)) list.Add(e);

		return list;
	}

	public List<EVertex> GetVerticesAtSelection(Polygon selection)
	{
		List<EVertex> list = new List<EVertex>();

		foreach (EMesh e in meshList)
			foreach (EVertex v in e)
				if (v.Intersects(selection)) list.Add(v);

		return list;
	}

	public void CreateMesh(Vector2 origin)
	{
		EMesh m = new EMesh(origin, this);
		meshList.Add(m);
	}

	public void SelectMesh(EMesh m)
	{
		if (!keyboard[Key.LShift]) selectedMeshes.Clear();

		if (m == null || selectedMeshes.Contains(m)) return;
		selectedMeshes.Add(m);
	}
	public void SelectMesh(IEnumerable<EMesh> m)
	{
		if (!keyboard[Key.LShift]) selectedMeshes.Clear();

		foreach (EMesh em in m)
		{
			if (selectedMeshes.Contains(em)) continue;
			selectedMeshes.Add(em);
		}
	}

	public void SelectVertex(EVertex v)
	{
		if (!keyboard[Key.LShift]) selectedVertices.Clear();

		if (v == null || selectedVertices.Contains(v)) return;
		selectedVertices.Add(v);
	}
	public void SelectVertex(IEnumerable<EVertex> v)
	{
		if (!keyboard[Key.LShift]) selectedVertices.Clear();

		foreach(EVertex vv in v)
		{
			if (selectedVertices.Contains(vv)) continue;
			selectedVertices.Add(vv);
		}
	}

	void Update()
	{
		mouse.PlaneDistance = editorCamera.Position.Z;

		cameraControl.Logic();
		editorCamera.Position = CameraControl.Position;
		editorCamera.Target = CameraControl.Position * new Vector3(1, 1, 0);

		if (keyboard.KeyPressed(Key.Tab)) vertexSelection = !vertexSelection;

		if (!BlockSelect)
		{
			if (mouse[MouseButton.Left])
			{
				if (dragSelectOriginBuffer == null)
					dragSelectOriginBuffer = mouse.Position.Xy;

				if (dragSelectOrigin != null || (mouse.Position.Xy - dragSelectOriginBuffer.Value).Length > 0.2f)
					dragSelectOrigin = dragSelectOriginBuffer;
			}
			else
			{
				if (!BlockSelect && mouse.ButtonReleased(MouseButton.Left))
					Select(SelectArea);

				dragSelectOrigin = null;
				dragSelectOriginBuffer = null;
			}
		}

		foreach (EMesh m in meshList)
			m.Logic();

		m.Logic();
	}

	void Render()
	{
		gridMesh.Color = new Color(0.4f, 0.4f, 0.4f, 1f);

		gridMesh.Reset();
		gridMesh.Draw();

		gridMesh.Color = new Color(0.8f, 0.8f, 0.8f, 1f);

		gridMesh.Scale(10f);
		gridMesh.Draw();

		foreach (EMesh m in meshList)
			m.Draw();

		m.Draw();

		if (dragSelectOrigin != null)
		{
			dragSelectMesh.Reset();

			dragSelectMesh.Translate(dragSelectOrigin.Value);
			dragSelectMesh.Scale(mouse.Position.Xy - dragSelectOrigin.Value);

			dragSelectMesh.Draw();
		}
	}
}