using TKTools;

public struct MeshDesign
{
	public enum DesignType
	{
		Tile,
		Color,
		Event
	}

	DesignType type;
	TextureSet.Tile tile;
	Color color;

	public DesignType Type
	{
		get { return type; }
	}
	public TextureSet.Tile Tile
	{
		get { return tile; }
	}
	public Color Color
	{
		get { return color; }
	}

	public MeshDesign(TextureSet.Tile tile)
	{
		this.type = DesignType.Tile;
		this.tile = tile;
		this.color = Color.White;
	}

	public MeshDesign(Color color)
	{
		this.type = DesignType.Color;
		this.tile = null;
		this.color = color;
	}
}