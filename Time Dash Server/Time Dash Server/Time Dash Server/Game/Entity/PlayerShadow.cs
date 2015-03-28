using OpenTK;

public class PlayerShadow
{
	Player player;
	public float updateRate = 0.01f, updateTimer = 0f, bufferLength = 0.8f;

	Vector2[] positionBuffer;
	int positionBufferIndex = 0;

	public Vector2 CurrentPosition
	{
		get
		{
			return positionBuffer[positionBufferIndex];
		}
	}

	public PlayerShadow(Player p)
	{
		player = p;

		positionBuffer = new Vector2[(int)(bufferLength / updateRate)];
		for (int i = 0; i < positionBuffer.Length; i++)
			positionBuffer[i] = p.Position;
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
		positionBuffer[positionBufferIndex] = player.Position;
		positionBufferIndex = (positionBufferIndex + 1) % positionBuffer.Length;
	}
}