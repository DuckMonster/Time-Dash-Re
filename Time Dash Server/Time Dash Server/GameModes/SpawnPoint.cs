using OpenTK;
public class SpawnPoint
{
	Map map;
	Vector2 position, size;

	public SpawnPoint(Vector2 position, Vector2 size, Map map)
	{
		this.map = map;
		this.position = position;
		this.size = size;
	}

	public Vector2 GetSpawnPosition()
	{
		float x = (float)map.rng.NextDouble() - 0.5f;
		float y = (float)map.rng.NextDouble() - 0.5f;

		return position + size * new Vector2(x, y);
	}
}