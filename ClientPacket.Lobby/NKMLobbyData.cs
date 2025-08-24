using System.Collections.Generic;
using ClientPacket.Pvp;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Lobby;

public sealed class NKMLobbyData : ISerializable
{
	public NKM_GAME_STATE lobbyState;

	public bool isObserverMode;

	public NKMPrivateGameConfig config = new NKMPrivateGameConfig();

	public List<NKMPvpGameLobbyUserState> users = new List<NKMPvpGameLobbyUserState>();

	public List<NKMPvpGameLobbyUserState> observers = new List<NKMPvpGameLobbyUserState>();

	public string lobbyCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref lobbyState);
		stream.PutOrGet(ref isObserverMode);
		stream.PutOrGet(ref config);
		stream.PutOrGet(ref users);
		stream.PutOrGet(ref observers);
		stream.PutOrGet(ref lobbyCode);
	}
}
