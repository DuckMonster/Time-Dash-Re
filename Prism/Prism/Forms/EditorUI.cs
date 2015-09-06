using System.ComponentModel;
using System.Windows.Forms;

public class EditorUIForm : Form
{
	Editor editor;
	public Editor Editor
	{
		get { return editor; }
		set
		{
			editor = value;
			
			if (editor != null)
				UpdateUI();
		}
	}

	public EditorUIForm()
		: base()
	{
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		Hide();
		e.Cancel = true;
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (e.KeyCode == Keys.F5) UpdateUI();
	}

	public virtual void UpdateUI() { }
}

public class EditorUIControl : UserControl
{
	Editor editor;
	public Editor Editor
	{
		get { return editor; }
		set
		{
			editor = value;

			if (editor != null)
				UpdateUI();
		}
	}

	public EditorUIControl()
		: base()
	{
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (e.KeyCode == Keys.F5) UpdateUI();
	}

	public virtual void UpdateUI() { }
}