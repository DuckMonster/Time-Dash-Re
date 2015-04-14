using OpenTK;
using TKTools;

public class Button
{
	protected Menu menu;
	protected Map map;
	protected string text;
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

	protected Vector2 position, size;

	protected TextBox textBox;

	protected Mesh buttonMesh, textMesh;

	protected Timer hoveredEffectTimer = new Timer(0.5f, true);
	protected Timer pressEffectTimer = new Timer(0.5f, true);
	bool previousHovered = false;

	public virtual bool Enabled
	{
		get { return true; }
	}

	public bool Hovered
	{
		get
		{
			return CollidesWith(MouseInput.Current.PositionOrtho);
		}
	}

	protected virtual float Alpha
	{
		get
		{
			if (Hovered)
				return (MouseInput.Current[OpenTK.Input.MouseButton.Left] ? 1f : 0.6f);
			else
				return 0.2f;
		}
	}

	protected virtual Color Color
	{
		get
		{
			if (!Enabled) return new Color(0.2f, 0.2f, 0.3f, 0.4f);
			else return new Color(1, 1, 1, Alpha);
		}
	}

	protected float Rotation
	{
		get
		{
			if (!hoveredEffectTimer.IsDone)
			{
				return 10f * TKMath.Exp(hoveredEffectTimer.PercentageDone, 6f);
			}
			else return 0f;
		}
	}

	public Button(string text, Vector2 position, Vector2 size, Menu menu, Map map)
	{
		this.map = map;
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

	protected virtual void OnClicked()
	{

	}

	public bool CollidesWith(Vector2 point)
	{
		return (
			point.X >= position.X - size.X / 2 &&
			point.X <= position.X + size.X / 2 &&
			point.Y >= position.Y - size.Y / 2 &&
			point.Y <= position.Y + size.Y / 2);
	}

	public virtual void Logic()
	{
		hoveredEffectTimer.Logic();
		pressEffectTimer.Logic();

		if (Enabled && Hovered)
		{
			if (!previousHovered)
				hoveredEffectTimer.Reset();

			if (MouseInput.Current[OpenTK.Input.MouseButton.Left])
				pressEffectTimer.Reset();

			if (MouseInput.GetReleased(OpenTK.Input.MouseButton.Left))
				OnClicked();
		}

		previousHovered = Hovered;
	}

	public virtual void Draw()
	{
		if (pressEffectTimer.IsDone)
			buttonMesh.Color = Color;
		else
			buttonMesh.Color = Color.Blend(Color, Color.White, TKMath.Exp(pressEffectTimer.PercentageDone, 6f));

		buttonMesh.Reset();

		buttonMesh.Translate(position);
		buttonMesh.Scale(size);

		if (!hoveredEffectTimer.IsDone)
		{
			buttonMesh.Rotate(10f * TKMath.Exp(hoveredEffectTimer.PercentageDone, 6f));
		}

		buttonMesh.Draw();

		textMesh.Texture = textBox.Texture;
		textMesh.Vertices = textBox.Vertices;

		textMesh.Reset();

		textMesh.Translate(position);

		if (!hoveredEffectTimer.IsDone)
		{
			textMesh.Rotate(10f * TKMath.Exp(hoveredEffectTimer.PercentageDone, 6f));
		}

		textMesh.Draw();
	}
}