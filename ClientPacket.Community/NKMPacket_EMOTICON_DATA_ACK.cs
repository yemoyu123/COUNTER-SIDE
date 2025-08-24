using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_EMOTICON_DATA_ACK)]
public sealed class NKMPacket_EMOTICON_DATA_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public EmoticonPresetData presetData = new EmoticonPresetData();

	public HashSet<int> collections = new HashSet<int>();

	public List<NKMEmoticonData> emoticonDatas = new List<NKMEmoticonData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref presetData);
		stream.PutOrGet(ref collections);
		stream.PutOrGet(ref emoticonDatas);
	}
}
