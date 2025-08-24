using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_COMPANY_BUFF_ADD_NOT)]
public sealed class NKMPacket_COMPANY_BUFF_ADD_NOT : ISerializable
{
	public NKMCompanyBuffData companyBuffData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref companyBuffData);
	}
}
