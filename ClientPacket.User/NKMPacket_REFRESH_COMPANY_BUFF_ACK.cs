using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_REFRESH_COMPANY_BUFF_ACK)]
public sealed class NKMPacket_REFRESH_COMPANY_BUFF_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMCompanyBuffData> companyBuffDataList = new List<NKMCompanyBuffData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref companyBuffDataList);
	}
}
