using OpenTK;
using OpenTK.Graphics.OpenGL;

using TKTools;

public class PlayerShadow
{
	Player player;
	Mesh mesh;

	public float updateRate = 0.01f, updateTimer = 0f, bufferLength = 3f;

	Vector2[] positionBuffer;
	int positionBufferIndex = 0;

	public bool warpHold;

	public Vector2 CurrentPosition
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

		positionBuffer = new Vector2[(int)(bufferLength / updateRate)];
	}

	public void Logic()
	{
		if (!warpHold)
		{
			updateTimer += Game.delta;

			while (updateTimer >= updateRate)
			{
				UpdateBuffer();
				updateTimer -= updateRate;
			}
		}
		else
		{
			updateTimer -= Game.delta * 2;

			while (updateTimer < 0)
			{
				positionBufferIndex--;
				if (positionBufferIndex < 0) positionBufferIndex += positionBuffer.Length;

				updateTimer += updateRate;
			}
		}

		Log.Debug(positionBufferIndex);
	}

	public void UpdateBuffer()
	{
		positionBuffer[positionBufferIndex] = player.position;
		positionBufferIndex = (positionBufferIndex + 1) % positionBuffer.Length;
	}

	public void Draw()
	{
		mesh.Color = new Color(0, 0, 0, 0.4f);

		if (CurrentPosition != null)
		{
			mesh.Reset();

			mesh.Translate(CurrentPosition);
			mesh.Scale(player.size);
			//mesh.Scale(new Vector2(1, 1));

			mesh.Draw();
		}
	}
}