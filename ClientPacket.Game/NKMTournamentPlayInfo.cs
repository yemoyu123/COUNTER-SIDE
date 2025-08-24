using ClientPacket.Common;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Game;

public sealed class NKMTournamentPlayInfo : ISerializable
{
	public NKMCommonProfile profile = new NKMCommonProfile();

	public PvpSingleHistory history;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref profile);
		stream.PutOrGet(ref history);
	}
}
