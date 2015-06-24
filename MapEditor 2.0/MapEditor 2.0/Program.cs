using TKTools.Context;

public class Program
{
	public static void Main(string[] args)
	{
		Program p = new Program();
	}

	Editor e;
	DebugForm debugForm;

	public Program()
	{
		e = new Editor();

		e.OnUpdate += Update;
		e.OnBegin += Load;

		e.Run();
	}

	void Load()
	{
		debugForm = new DebugForm();
		debugForm.Show();
	}

	void Update()
	{
		debugForm.Logic();
	}
}