using System;
using System.Collections.Generic;

public class HistorySystem
{
	public abstract class HistoryAction : IDisposable
	{
		protected HistorySystem system;

		protected Editor Editor { get { return system.editor; } }
		protected Layer Layer { get { return system.layer; } }

		public HistoryAction(HistorySystem system)
		{
			this.system = system;
	    }

		public virtual void Dispose() { }
		public abstract void Undo();
		public abstract void Redo();
	}

	protected Editor editor;
	protected Layer layer;

	List<HistoryAction> actions = new List<HistoryAction>();
	int index = 0;

	public HistoryAction CurrentAction
	{
		get
		{
			if (index >= actions.Count) return null;
			return actions[index];
		}
	}
	public HistoryAction PreviousAction
	{
		get
		{
			if (index == 0) return null;
			return actions[index - 1];
		}
	}

	public HistorySystem(Layer l, Editor e)
	{
		layer = l;
		editor = e;
	}

	public void Add(HistoryAction action)
	{
		if (CurrentAction != null)
			ClearFrom(index);

		actions.Add(action);
		index = actions.Count;
	}

	public void Undo()
	{
		if (PreviousAction == null) return;

		PreviousAction.Undo();
		index--;
	}

	public void Redo()
	{
		if (CurrentAction == null) return;

		CurrentAction.Redo();
		index++;
	}

	void ClearFrom(int index)
	{
		for (int i = index; i < actions.Count; i++)
			actions[i].Dispose();

		actions.RemoveRange(index, actions.Count - index);
	}
}