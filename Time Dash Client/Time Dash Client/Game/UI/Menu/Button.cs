using OpenTK;
using OpenTK.Input;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;
using TKTools.Mathematics;

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
		}
	}

	protected Vector2 position, size;

	protected TextBox textBox;

	protected Mesh buttonMesh;

	protected Timer hoveredEffectTimer = new Timer(0.5f, true);
	protected Timer pressEffectTimer = new Timer(0.5f, true);
	bool previousHovered = false;

	protected MouseWatch mouse;

	public virtual Vector2 Position
	{
		get { return position; }
		set { position = value; }
	}

	public virtual bool Enabled
	{
		get { return true; }
	}

	public bool Hovered
	{
		get
		{
			return CollidesWith(mouse.Position.Xy);
		}
	}

	protected virtual float Alpha
	{
		get
		{
			if (Hovered)
				return (mouse[OpenTK.Input.MouseButton.Left] ? 1f : 0.6f);
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

		buttonMesh = Mesh.CreateFromPrimitive(MeshPrimitive.Quad);
		buttonMesh.Color = new Color(0, 0, 0, 0.4f);

		buttonMesh.Translate(position);
		buttonMesh.Scale(size);

		mouse = new MouseWatch();
		mouse.Perspective = Map.UICamera;
	}

	protected virtual void OnClicked()
	{

	}

	public bool CollidesWith(Vector2 point)
	{
		return (
			point.X >= Position.X - size.X / 2 &&
			point.X <= Position.X + size.X / 2 &&
			point.Y >= Position.Y - size.Y / 2 &&
			point.Y <= Position.Y + size.Y / 2);
	}

	public virtual void Logic()
	{
		hoveredEffectTimer.Logic();
		pressEffectTimer.Logic();

		if (Enabled && Hovered)
		{
			if (!previousHovered)
				hoveredEffectTimer.Reset();

			if (mouse[MouseButton.Left])
				pressEffectTimer.Reset();

			if (mouse.ButtonReleased(MouseButton.Left))
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

		buttonMesh.Translate(Position);
		buttonMesh.Scale(size);

		if (!hoveredEffectTimer.IsDone)
		{
			buttonMesh.RotateZ(10f * TKMath.Exp(hoveredEffectTimer.PercentageDone, 6f));
		}

		buttonMesh.Draw();

		Mesh m = textBox.Mesh;
		m.Reset();
		m.Translate(Position);

		if (!hoveredEffectTimer.IsDone)
		{
			m.RotateZ(10f * TKMath.Exp(hoveredEffectTimer.PercentageDone, 6f));
		}

		m.Draw();
	}
}