using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_GUEST_LIST_NOT)]
public sealed class NKMPacket_OFFICE_GUEST_LIST_NOT : ISerializable
{
	public List<NKMUserProfileData> guestList = new List<NKMUserProfileData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guestList);
	}
}
