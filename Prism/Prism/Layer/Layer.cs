using System;
using System.Collections;
using System.Collections.Generic;
using Prism.Parser;
using OpenTK;
using TKTools;

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
	HistorySystem history;

	public bool Enabled
	{
		get { return editor.activeLayers.Contains(this); }
	}

	public List<EMesh> Meshes
	{
		get { return meshes; }
	}

	public HistorySystem History { get { return history; } }

	public Layer(string name, Editor e)
		:base(name, e)
	{
		history = new HistorySystem(this, editor);
	}

	public Layer(Layer copy)
		: base(copy)
	{
		history = new HistorySystem(this, editor);

		foreach (EMesh m in copy)
			AddMesh(new EMesh(m, this, editor));
	}

	public Layer(PrismLayer copy, Dictionary<PrismTexture.Tile, TextureSet.Tile> tiles, Editor e) 
		:base(copy, tiles, e)
	{
		history = new HistorySystem(this, e);

		foreach(PrismMesh mesh in copy.Meshes)
			AddMesh(new EMesh(mesh, tiles, this, e));
	}

	public override void Dispose()
	{
		foreach (EMesh mesh in this)
			mesh.Remove();
	}

	public void AddMesh(EMesh mesh)
	{
		if (mesh.Layer == this) return;

		mesh.Layer = this;
	}

	public void RemoveMesh(EMesh mesh)
	{
		if (mesh.Layer != this)
			return;

		mesh.Layer = null;
	}

	public void MergeAndRemove(Layer target)
	{
		editor.DeselectAll();
		EMesh[] mlist = Meshes.ToArray();

		foreach (EMesh m in mlist)
			m.Layer = target;

		Remove();
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

	public override PrismLayerNode GetPrismNode(PrismLayerNode parent, Dictionary<TextureSet.Tile, PrismTexture.Tile> tiles)
	{
		PrismLayer node = new PrismLayer(Name, parent);

		foreach (EMesh m in Meshes)
		{
			Vector2[] pos = new Vector2[m.Vertices.Count];
			ColorHSL[] col = new ColorHSL[m.Vertices.Count];

			for (int i = 0; i < pos.Length; i++)
			{
				pos[i] = m.Vertices[i].Position;
				col[i] = m.Vertices[i].HSL;
			}

			PrismMesh mesh = new PrismMesh(pos, col, m.Tile != null ? tiles[m.Tile] : null);
			node.Meshes.Add(mesh);
		}

		return node;
	}
}