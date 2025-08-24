using System.Collections.Generic;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Common;

public sealed class NKMUserProfileData : ISerializable
{
	public sealed class PvpProfileData : ISerializable
	{
		public int seasonId;

		public int leagueTierId;

		public int score;

		void ISerializable.Serialize(IPacketStream stream)
		{
			stream.PutOrGet(ref seasonId);
			stream.PutOrGet(ref leagueTierId);
			stream.PutOrGet(ref score);
		}
	}

	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public string friendIntro;

	public PvpProfileData rankPvpData = new PvpProfileData();

	public PvpProfileData asyncPvpData = new PvpProfileData();

	public PvpProfileData leaguePvpData = new PvpProfileData();

	public NKMDummyDeckData profileDeck;

	public NKMDummyDeckData leagueDeck;

	public NKMAsyncDeckData defenceDeck = new NKMAsyncDeckData();

	public List<NKMEmblemData> emblems = new List<NKMEmblemData>();

	public int selfiFrameId;

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	public bool hasOffice;

	public PrivatePvpInvitation privatePvpInvitation;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref friendIntro);
		stream.PutOrGet(ref rankPvpData);
		stream.PutOrGet(ref asyncPvpData);
		stream.PutOrGet(ref leaguePvpData);
		stream.PutOrGet(ref profileDeck);
		stream.PutOrGet(ref leagueDeck);
		stream.PutOrGet(ref defenceDeck);
		stream.PutOrGet(ref emblems);
		stream.PutOrGet(ref selfiFrameId);
		stream.PutOrGet(ref guildData);
		stream.PutOrGet(ref hasOffice);
		stream.PutOrGetEnum(ref privatePvpInvitation);
	}
}
