using System.Collections.Generic;
using Cs.Protocol;
using NKM.Templet.Office;

namespace ClientPacket.Office;

public sealed class NKMOfficeRoom : ISerializable
{
	public int id;

	public string name;

	public OfficeGrade grade;

	public int interiorScore;

	public List<NKMOfficeFurniture> furnitures = new List<NKMOfficeFurniture>();

	public List<long> unitUids = new List<long>();

	public int floorInteriorId;

	public int wallInteriorId;

	public int backgroundId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref id);
		stream.PutOrGet(ref name);
		stream.PutOrGetEnum(ref grade);
		stream.PutOrGet(ref interiorScore);
		stream.PutOrGet(ref furnitures);
		stream.PutOrGet(ref unitUids);
		stream.PutOrGet(ref floorInteriorId);
		stream.PutOrGet(ref wallInteriorId);
		stream.PutOrGet(ref backgroundId);
	}
}
