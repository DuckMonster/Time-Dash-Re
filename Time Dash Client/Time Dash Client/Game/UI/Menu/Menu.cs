﻿using OpenTK;
using System.Collections.Generic;

public class Menu
{
	protected Map map;
	protected Vector2 size;

	Mesh menuMesh = Mesh.OrthoBox;

	List<Button> buttonList = new List<Button>();

	Timer alphaTimer = new Timer(0.6f, false);

	public Menu(Vector2 size, Map map)
	{
		this.map = map;
		this.size = size;
	}

	protected void AddButton(Button b)
	{
		buttonList.Add(b);
	}

	public virtual void Open()
	{
		alphaTimer.Reset();
	}

	public virtual void Close()
	{
	}

	public virtual void Logic()
	{
		foreach (Button b in buttonList) b.Logic();
		alphaTimer.Logic();
	}

	public virtual void Draw()
	{
		menuMesh.Color = new TKTools.Color(0, 0, 0, 0.7f * (1f - TKTools.TKMath.Exp(alphaTimer.PercentageDone, 5f)));

		menuMesh.Reset();
		menuMesh.Scale(20f, 20f * Game.windowRatio);

		menuMesh.Draw();
		foreach (Button b in buttonList) b.Draw();
	}
}