using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Office;

public sealed class NKMOfficePreset : ISerializable
{
	public int presetId;

	public string name;

	public List<NKMOfficeFurniture> furnitures = new List<NKMOfficeFurniture>();

	public int floorInteriorId;

	public int wallInteriorId;

	public int backgroundId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref presetId);
		stream.PutOrGet(ref name);
		stream.PutOrGet(ref furnitures);
		stream.PutOrGet(ref floorInteriorId);
		stream.PutOrGet(ref wallInteriorId);
		stream.PutOrGet(ref backgroundId);
	}
}
