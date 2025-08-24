using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Office;

public sealed class NKMMyOfficeState : ISerializable
{
	public List<int> openedSectionIds = new List<int>();

	public List<NKMOfficeRoom> rooms = new List<NKMOfficeRoom>();

	public List<NKMInteriorData> interiors = new List<NKMInteriorData>();

	public NKMOfficePostState postState = new NKMOfficePostState();

	public List<NKMOfficePreset> presets = new List<NKMOfficePreset>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref openedSectionIds);
		stream.PutOrGet(ref rooms);
		stream.PutOrGet(ref interiors);
		stream.PutOrGet(ref postState);
		stream.PutOrGet(ref presets);
	}
}
