using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_RAID_GAME_LOAD_REQ)]
public sealed class NKMPacket_RAID_GAME_LOAD_REQ : ISerializable
{
	public byte selectDeckIndex;

	public long raidUID;

	public List<int> buffList = new List<int>();

	public bool isTryAssist;

	public long supportingUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref selectDeckIndex);
		stream.PutOrGet(ref raidUID);
		stream.PutOrGet(ref buffList);
		stream.PutOrGet(ref isTryAssist);
		stream.PutOrGet(ref supportingUserUid);
	}
}
