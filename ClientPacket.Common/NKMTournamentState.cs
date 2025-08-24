namespace ClientPacket.Common;

public enum NKMTournamentState
{
	Ended = 0,
	Progressing = 1,
	BanVote = 2,
	PreBooking = 10,
	Tryout = 20,
	Final32 = 30,
	Final4 = 40,
	Closing = 50
}
