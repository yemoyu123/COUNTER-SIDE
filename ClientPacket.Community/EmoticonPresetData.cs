using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Community;

public sealed class EmoticonPresetData : ISerializable
{
	public List<int> animationList = new List<int>();

	public List<int> textList = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref animationList);
		stream.PutOrGet(ref textList);
	}
}
