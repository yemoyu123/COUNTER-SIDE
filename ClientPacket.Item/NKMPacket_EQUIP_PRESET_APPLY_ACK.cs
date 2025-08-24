using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PRESET_APPLY_ACK)]
public sealed class NKMPacket_EQUIP_PRESET_APPLY_ACK : ISerializable
{
	public sealed class UnitEquipUidSet : ISerializable
	{
		public long unitUid;

		public List<long> equipUids = new List<long>();

		void ISerializable.Serialize(IPacketStream stream)
		{
			stream.PutOrGet(ref unitUid);
			stream.PutOrGet(ref equipUids);
		}
	}

	public NKM_ERROR_CODE errorCode;

	public int presetIndex;

	public List<UnitEquipUidSet> updateUnitDatas = new List<UnitEquipUidSet>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref presetIndex);
		stream.PutOrGet(ref updateUnitDatas);
	}
}
