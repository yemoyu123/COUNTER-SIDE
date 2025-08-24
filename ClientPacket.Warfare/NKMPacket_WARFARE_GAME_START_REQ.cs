using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_GAME_START_REQ)]
public sealed class NKMPacket_WARFARE_GAME_START_REQ : ISerializable
{
	public sealed class UnitPosition : ISerializable
	{
		public bool isFlagShip;

		public byte deckIndex;

		public short tileIndex;

		void ISerializable.Serialize(IPacketStream stream)
		{
			stream.PutOrGet(ref isFlagShip);
			stream.PutOrGet(ref deckIndex);
			stream.PutOrGet(ref tileIndex);
		}
	}

	public int warfareTempletID;

	public List<UnitPosition> unitPositionList = new List<UnitPosition>();

	public long friendCode;

	public short friendTileIndex;

	public int rewardMultiply;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref warfareTempletID);
		stream.PutOrGet(ref unitPositionList);
		stream.PutOrGet(ref friendCode);
		stream.PutOrGet(ref friendTileIndex);
		stream.PutOrGet(ref rewardMultiply);
	}
}
