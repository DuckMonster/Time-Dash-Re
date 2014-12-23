using OpenTK;
using OpenTK.Graphics.OpenGL;

using TKTools;

public class PlayerShadow
{
	public class ShadowPosition
	{
		public Vector2 position;
		public int tilex, tiley, direction;

		public ShadowPosition(Vector2 pos, Tileset t, int dir)
		{
			position = pos;
			tilex = t.X;
			tiley = t.Y;
			direction = dir;
		}
		public ShadowPosition(Vector2 pos, int tx, int ty, int dir)
		{
			position = pos;
			tilex = tx;
			tiley = ty;
			direction = dir;
		}

		public static implicit operator Vector2(ShadowPosition pos)
		{
			return pos.position;
		}
	}

	Player player;
	Mesh mesh;

	public float updateRate = 0.01f, updateTimer = 0f, bufferLength = 0.6f;

	ShadowPosition[] positionBuffer;
	int positionBufferIndex = 0;

	public ShadowPosition CurrentPosition
	{
		get
		{
			return positionBuffer[positionBufferIndex];
		}
	}

	public PlayerShadow(Player p, Mesh m)
	{
		player = p;
		mesh = m;

		positionBuffer = new ShadowPosition[(int)(bufferLength / updateRate)];
		for (int i = 0; i < positionBuffer.Length; i++)
			positionBuffer[i] = new ShadowPosition(p.position, p.playerTileset, p.dir);
	}

	public void Logic()
	{
		updateTimer += Game.delta;

		while (updateTimer >= updateRate)
		{
			UpdateBuffer();
			updateTimer -= updateRate;
		}
	}

	public void UpdateBuffer()
	{
		positionBuffer[positionBufferIndex] = new ShadowPosition(player.position, player.playerTileset, player.dir);
		positionBufferIndex = (positionBufferIndex + 1) % positionBuffer.Length;
	}

	public void Draw()
	{
		mesh.Color = new Color(0, 0, 0, 0.4f);

		if (CurrentPosition != null)
		{
			mesh.Reset();

			mesh.Translate(CurrentPosition.position);
			mesh.Scale(player.size);
			mesh.Scale(-CurrentPosition.direction, 1);

			mesh.Draw(player.playerTileset, CurrentPosition.tilex, CurrentPosition.tiley);
		}
	}
}