using System;
using System.Collections;
using System.Collections.Generic;

public class Layer : LayerNode, IEnumerable
{
	#region Enum Stuff
	public IEnumerator GetEnumerator()
	{
		EMesh[] ma = meshes.ToArray();
		foreach (EMesh m in ma) yield return m;
	}
	#endregion

	List<EMesh> meshes = new List<EMesh>();

	public bool Visible
	{
		get { return true; }
	}
	public bool Enabled
	{
		get { return editor.activeLayers.Contains(this); }
	}

	public List<EMesh> Meshes
	{
		get { return meshes; }
	}

	public Layer(string name, Editor e)
		:base(name, e)
	{
	}

	public Layer(Layer copy)
		: base(copy)
	{
		foreach (EMesh m in copy)
			AddMesh(new EMesh(m, this, editor));
	}

	public override void Dispose()
	{
		foreach (EMesh mesh in this)
			mesh.Remove();
	}

	public void AddMesh(EMesh mesh)
	{
		if (meshes.Contains(mesh))
			return;

		meshes.Add(mesh);
		mesh.Layer = this;
	}

	public void RemoveMesh(EMesh mesh)
	{
		if (!meshes.Contains(mesh))
			return;

		meshes.Remove(mesh);
	}

	public override void Logic()
	{
		throw new NotImplementedException();
	}

	public override void Draw()
	{
		throw new NotImplementedException();
	}

	public override LayerNode GetCopy()
	{
		return new Layer(this);
	}

	public override string ToString()
	{
		return Name;
	}
}