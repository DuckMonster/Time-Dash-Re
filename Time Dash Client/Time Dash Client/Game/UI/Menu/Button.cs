using OpenTK;
using TKTools;

public class Button
{
	Menu menu;
	string text;
	public string Text
	{
		get { return text; }
		set
		{
			text = value;
			textBox.Text = text;

			textMesh.Texture = textBox.Texture;
			textMesh.Vertices = textBox.Vertices;
		}
	}

	Vector2 position, size;

	TextBox textBox;

	Mesh buttonMesh, textMesh;

	Timer hoveredEffectTimer = new Timer(0.5f, true);
	bool previousHovered = false;

	public bool Hovered
	{
		get
		{
			return CollidesWith(MouseInput.Current.PositionOrtho);
		}
	}

	float Alpha
	{
		get
		{
			if (Hovered)
				return (MouseInput.Current[OpenTK.Input.MouseButton.Left] ? 1f : 0.6f) + 0.3f * (1f - hoveredEffectTimer.PercentageDone);
			else
				return 0.2f;
		}
	}

	public Button(string text, Vector2 position, Vector2 size, Menu menu)
	{
		this.menu = menu;

		this.position = position;
		this.size = size;
		this.text = text;

		textBox = new TextBox(80f);
		textBox.Text = text;
		textBox.HorizontalAlign = TextBox.HorizontalAlignment.Center;
		textBox.VerticalAlign = TextBox.VerticalAlignment.Center;

		textBox.SetHeight = size.Y;

		textMesh = Mesh.OrthoBox;
		textMesh.Color = Color.Red;

		textMesh.Texture = textBox.Texture;
		textMesh.Vertices = textBox.Vertices;
		textMesh.UV = textBox.UV;

		buttonMesh = Mesh.OrthoBox;
		buttonMesh.Color = new Color(0, 0, 0, 0.4f);

		buttonMesh.Translate(position);
		buttonMesh.Scale(size);

		textMesh.Translate(position);
	}

	public bool CollidesWith(Vector2 point)
	{
		return (
			point.X >= position.X - size.X / 2 &&
			point.X <= position.X + size.X / 2 &&
			point.Y >= position.Y - size.Y / 2 &&
			point.Y <= position.Y + size.Y / 2);
	}

	public void Logic()
	{
		hoveredEffectTimer.Logic();

		if (Hovered && !previousHovered)
			hoveredEffectTimer.Reset();

		previousHovered = Hovered;
	}

	public void Draw()
	{
		buttonMesh.Color = new Color(1, 1, 1, Alpha);

		buttonMesh.Reset();

		buttonMesh.Translate(position);
		buttonMesh.Scale(size);

		if (!hoveredEffectTimer.IsDone)
		{
			buttonMesh.Rotate(10f * TKMath.Exp(hoveredEffectTimer.PercentageDone, 6f));
		}

		buttonMesh.Draw();

		textMesh.Texture = textBox.Texture;

		textMesh.Reset();

		textMesh.Translate(position);

		if (!hoveredEffectTimer.IsDone)
		{
			textMesh.Rotate(10f * TKMath.Exp(hoveredEffectTimer.PercentageDone, 6f));
		}

		textMesh.Draw();
	}
}