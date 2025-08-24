using Cs.Protocol;

namespace NKM;

public sealed class NKMPotentialOption : ISerializable
{
	public sealed class SocketData : ISerializable
	{
		public float statValue;

		public int precision;

		void ISerializable.Serialize(IPacketStream stream)
		{
			stream.PutOrGet(ref statValue);
			stream.PutOrGet(ref precision);
		}
	}

	public int optionKey;

	public NKM_STAT_TYPE statType;

	public SocketData[] sockets = new SocketData[3];

	public int precisionChangeCount;

	public int OpenedSocketCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < sockets.Length; i++)
			{
				if (sockets[i] != null)
				{
					num++;
				}
			}
			return num;
		}
	}

	public float GetTotalStatValue()
	{
		float num = 0f;
		SocketData[] array = sockets;
		foreach (SocketData socketData in array)
		{
			if (socketData == null)
			{
				break;
			}
			num += socketData.statValue;
		}
		return num;
	}

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref optionKey);
		stream.PutOrGetEnum(ref statType);
		stream.PutOrGet(ref sockets);
		stream.PutOrGet(ref precisionChangeCount);
	}
}
