using OpenTK;
using System.Collections.Generic;

public class Menu
{
	Map map;
	Vector2 size;

	Mesh menuMesh = Mesh.OrthoBox;

	List<Button> buttonList = new List<Button>();

	public Menu(Vector2 size, Map map)
	{
		this.map = map;
		this.size = size;

		menuMesh.Color = new TKTools.Color(0f, 0.1f, 0.4f, 0.4f);

		menuMesh.Scale(size);

		AddButton(new Button("Options", new Vector2(0f, 4f), new Vector2(5f, 0.8f), this));
		AddButton(new Button("Disconnect", new Vector2(0f, 2f), new Vector2(5f, 0.8f), this));
		AddButton(new Button("Push", new Vector2(0f, 1f), new Vector2(5f, 0.8f), this));
		AddButton(new Button("Pull", new Vector2(0f, 0f), new Vector2(5f, 0.8f), this));
		AddButton(new Button("Close", new Vector2(0f, -2f), new Vector2(5f, 0.8f), this));
	}

	void AddButton(Button b)
	{
		buttonList.Add(b);
	}

	public void Logic()
	{
		foreach (Button b in buttonList) b.Logic();
	}

	public void Draw()
	{
		menuMesh.Draw();
		foreach (Button b in buttonList) b.Draw();
	}
}