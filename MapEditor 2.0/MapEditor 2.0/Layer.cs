using System.Collections;
using System.Collections.Generic;

public class Layer : IEnumerable
{
	public class MeshEnum : IEnumerator
	{
		EMesh[] layerList;
		int index = -1;

		public MeshEnum(EMesh[] l)
		{
			layerList = l;
		}

		public bool MoveNext()
		{
			index++;

			return (index < layerList.Length);
		}

		public void Reset()
		{
			index = -1;
		}

		public object Current
		{
			get { return layerList[index]; }
		}
	}

	Editor editor;
	List<EMesh> meshList = new List<EMesh>();

	string name = "";
	bool visible = true;

	public bool Active
	{
		get { return editor.activeLayers.Contains(this); }
	}

	public bool Visible
	{
		get { return visible; }
		set { visible = value; }
	}

	public string Name
	{
		get { return name; }
		set { name = value; }
	}

	public List<EMesh> Meshes
	{
		get { return meshList; }
	}

	public Layer(string name, Editor e)
	{
		editor = e;
		this.name = name;
	}

	public void MoveMesh(EMesh e, int n)
	{
		int index = meshList.IndexOf(e);
		if (index + n < 0 || index + n > meshList.Count-1) return;

		EMesh temp = meshList[index + n];
		meshList[index + n] = e;
		meshList[index] = temp;
	}

	public void AddMesh(EMesh e)
	{
		if (meshList.Contains(e)) return;

		if (e.Layer != null) e.Layer.RemoveMesh(e);

		meshList.Add(e);
		e.SetLayer(this);
	}

	public void RemoveMesh(EMesh e)
	{
		meshList.Remove(e);
		e.SetLayer(null);
	}

	public IEnumerator GetEnumerator()
	{
		return new MeshEnum(meshList.ToArray());
	}
}