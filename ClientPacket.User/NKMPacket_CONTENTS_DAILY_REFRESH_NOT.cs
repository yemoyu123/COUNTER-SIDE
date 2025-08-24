using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_CONTENTS_DAILY_REFRESH_NOT)]
public sealed class NKMPacket_CONTENTS_DAILY_REFRESH_NOT : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMItemMiscData> refreshItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref refreshItemDataList);
	}
}
