using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_CONTRACT_STATE_LIST_ACK)]
public sealed class NKMPacket_CONTRACT_STATE_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMContractState> contractState = new List<NKMContractState>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref contractState);
	}
}
