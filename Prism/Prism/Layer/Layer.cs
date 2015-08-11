using System;
using System.Collections;
using System.Collections.Generic;

public class Layer : LayerNode, IEnumerable
{
	#region Enum Stuff
	public class LayerEnum : IEnumerator
	{
		int index = -1;
		EMesh[] meshes;

		public LayerEnum(Layer l)
		{
			meshes = l.meshes.ToArray();
		}

		public bool MoveNext()
		{
			index++;

			return (index < meshes.Length);
		}

		public void Reset() { index = -1; }
		public object Current { get { return meshes[index]; } }
	}
	public IEnumerator GetEnumerator() { return new LayerEnum(this); }
	#endregion

	List<EMesh> meshes = new List<EMesh>();

	public bool Visible
	{
		get { return true; }
	}
	public bool Enabled
	{
		get { return true; }
	}

	public List<EMesh> Meshes
	{
		get { return meshes; }
	}

	public Layer(string name, Editor e)
		:base(name, e)
	{
	}

	public void AddMesh(EMesh mesh)
	{
		if (meshes.Contains(mesh))
			return;

		meshes.Add(mesh);
		mesh.SetLayer(this);
	}

	public override void Logic()
	{
		throw new NotImplementedException();
	}

	public override void Draw()
	{
		throw new NotImplementedException();
	}

	public override string ToString()
	{
		return Name;
	}
}