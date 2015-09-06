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
			UpdateUI();
		}
	}

	public EditorUIForm()
		:base()
	{
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (e.KeyCode == Keys.F5) UpdateUI();
	}

	public virtual void UpdateUI() { }
}