using System;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_GAME_LAW_SHUTDOWN_NOT)]
public sealed class NKMPacket_GAME_LAW_SHUTDOWN_NOT : ISerializable
{
	public enum ApplyType
	{
		None,
		TimeSelectionShutdown,
		ForceShutdown,
		ChildSelection
	}

	public ApplyType applyType;

	public DateTime applyTime;

	public TimeSpan remainSpan;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref applyType);
		stream.PutOrGet(ref applyTime);
		stream.PutOrGet(ref remainSpan);
	}
}
