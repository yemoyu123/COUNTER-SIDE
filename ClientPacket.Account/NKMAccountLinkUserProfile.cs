using ClientPacket.Common;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Account;

public sealed class NKMAccountLinkUserProfile : ISerializable
{
	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public NKM_PUBLISHER_TYPE publisherType;

	public long creditCount;

	public long eterniumCount;

	public long cashCount;

	public long medalCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGetEnum(ref publisherType);
		stream.PutOrGet(ref creditCount);
		stream.PutOrGet(ref eterniumCount);
		stream.PutOrGet(ref cashCount);
		stream.PutOrGet(ref medalCount);
	}
}
