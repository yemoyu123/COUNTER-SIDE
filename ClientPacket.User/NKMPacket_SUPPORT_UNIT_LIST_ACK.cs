using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_SUPPORT_UNIT_LIST_ACK)]
public sealed class NKMPacket_SUPPORT_UNIT_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMSupportUnitProfileData> supportUnitData = new List<NKMSupportUnitProfileData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref supportUnitData);
	}
}
