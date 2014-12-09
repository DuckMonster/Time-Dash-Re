using System.Collections.Generic;
//public sealed class Protocol
//{
//	public const Protocol
//		EnterMap = new Protocol(0),
//		PlayerJoin = new Protocol(1),
//		PlayerLeave = new Protocol(2);

//	private readonly static Dictionary<short, Protocol> protoList = new Dictionary<short, Protocol>();

//	private readonly short value;

//	private Protocol(short value)
//	{
//		this.value = value;
//		protoList.Add(value, this);
//	}

//	public static implicit operator short(Protocol p)
//	{
//		return p.value;
//	}

//	public static implicit operator Protocol(short b)
//	{
//		return protoList[b];
//	}
//}

public class Port
{
	public const int TCP = 1227, UDP = 1280;
}

public enum Protocol
{
	EnterMap,
	PlayerJoin,
	PlayerLeave,
	PlayerInputToggle
}