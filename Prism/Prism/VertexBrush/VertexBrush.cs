using OpenTK;
using TKTools;

public struct VertexBrush
{
	ColorHSL hsl;
	float size, opacity, hardness;

	public ColorHSL HSL
	{
		get { return hsl; }
		set { hsl = value; }
	}

	public float Size
	{
		get { return size; }
		set { size = value; }
	}
	public float Opacity
	{
		get { return opacity; }
		set { opacity = value; }
	}
	public float Hardness
	{
		get { return hardness; }
		set { hardness = value; }
	}

	public VertexBrush(ColorHSL _hsl, float _size, float _opacity, float _hardness)
	{
		hsl = _hsl;

		size = _size;
		opacity = _opacity;
		hardness = _hardness;
	}

	public static VertexBrush Blend(VertexBrush a, VertexBrush b, float value)
	{
		MathHelper.Clamp(value, 0f, 1f);

		return new VertexBrush(
			ColorHSL.Blend(a.hsl, b.hsl, value),
			a.size, a.opacity, a.hardness
			);
	}
}