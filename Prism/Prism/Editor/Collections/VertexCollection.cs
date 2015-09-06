using System;
using System.Collections;
using System.Collections.Generic;

public class VertexCollection : ICollection<EVertex>, IList<EVertex>
{
	public IEnumerator GetEnumerator() { foreach (EVertex v in vertices) yield return v; }
	IEnumerator<EVertex> IEnumerable<EVertex>.GetEnumerator() { foreach (EVertex v in vertices) yield return v; }

	List<EVertex> vertices = new List<EVertex>();

	public int Count { get { return vertices.Count; } }
	public bool IsReadOnly { get { return false; } }

	public VertexCollection()
	{
	}

	void OnListChanged()
	{
		Program.outlinerForm.UpdateSelected();
	}

	public void Add(EVertex v)
	{
		vertices.Add(v);
		OnListChanged();
    }

	public bool Remove(EVertex v)
	{
		bool removed = vertices.Remove(v);
		OnListChanged();

		return removed;
	}

	public void Clear()
	{
		vertices.Clear();
		OnListChanged();
	}

	public bool Contains(EVertex v)
	{
		return vertices.Contains(v);
	}

	public void CopyTo(EVertex[] array, int index)
	{
		vertices.CopyTo(array, index);
	}

	public int IndexOf(EVertex item)
	{
		return vertices.IndexOf(item);
	}

	public void Insert(int index, EVertex item)
	{
		vertices.Insert(index, item);
		OnListChanged();
	}

	public void RemoveAt(int index)
	{
		vertices.RemoveAt(index);
		OnListChanged();
	}

	public EVertex this[int index]
	{
		get { return vertices[index]; }
		set
		{
			vertices[index] = value;
			Program.outlinerForm.UpdateUI();
		}
	}
}