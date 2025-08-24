using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_MISC_CONTRACT_OPEN_ACK)]
public sealed class NKMPacket_MISC_CONTRACT_OPEN_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMItemMiscData> costItems = new List<NKMItemMiscData>();

	public List<MiscContractResult> result = new List<MiscContractResult>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref costItems);
		stream.PutOrGet(ref result);
	}
}
