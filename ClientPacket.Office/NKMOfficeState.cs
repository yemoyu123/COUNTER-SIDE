using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Office;

public sealed class NKMOfficeState : ISerializable
{
	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public List<int> openedSectionIds = new List<int>();

	public List<NKMOfficeRoom> rooms = new List<NKMOfficeRoom>();

	public List<NKMOfficeUnitData> units = new List<NKMOfficeUnitData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref openedSectionIds);
		stream.PutOrGet(ref rooms);
		stream.PutOrGet(ref units);
	}
}
