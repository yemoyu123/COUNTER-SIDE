using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.User;

public sealed class NKMBackgroundInfo : ISerializable
{
	public int backgroundItemId;

	public int backgroundBgmId;

	public List<NKMBackgroundUnitInfo> unitInfoList = new List<NKMBackgroundUnitInfo>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref backgroundItemId);
		stream.PutOrGet(ref backgroundBgmId);
		stream.PutOrGet(ref unitInfoList);
	}
}
