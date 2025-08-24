using ClientPacket.Common;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Pvp;

public sealed class NKMPvpGameLobbyUserState : ISerializable
{
	public NKMUserProfileData profileData = new NKMUserProfileData();

	public bool isReady;

	public bool isHost;

	public NKMDeckIndex deckIndex;

	public NKMDummyDeckData deckData;

	public LobbyPlayerState playerState;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref profileData);
		stream.PutOrGet(ref isReady);
		stream.PutOrGet(ref isHost);
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref deckData);
		stream.PutOrGetEnum(ref playerState);
	}
}
