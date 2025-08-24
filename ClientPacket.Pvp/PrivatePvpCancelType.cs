namespace ClientPacket.Pvp;

public enum PrivatePvpCancelType
{
	None,
	HostCancelInvitation,
	OtherPlayerCancelGame,
	OtherPlayerLogout,
	MyInvitationRejected,
	IRejectInvitation,
	InvitationTimeout,
	HostWasGone,
	AccountDisconnect
}
