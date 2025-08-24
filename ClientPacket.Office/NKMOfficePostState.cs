using System;
using Cs.Protocol;

namespace ClientPacket.Office;

public sealed class NKMOfficePostState : ISerializable
{
	public bool broadcastExecution;

	public int sendCount;

	public int recvCount;

	public DateTime nextResetDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref broadcastExecution);
		stream.PutOrGet(ref sendCount);
		stream.PutOrGet(ref recvCount);
		stream.PutOrGet(ref nextResetDate);
	}
}
