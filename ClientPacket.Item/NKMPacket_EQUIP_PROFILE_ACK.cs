using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PROFILE_ACK)]
public sealed class NKMPacket_EQUIP_PROFILE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<EquipProfileInfo> equipProfileInfos = new List<EquipProfileInfo>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref equipProfileInfos);
	}
}
