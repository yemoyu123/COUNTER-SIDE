using System.Collections.Generic;
using Cs.Protocol;
using NKM.Templet;

namespace NKM;

public sealed class NKMDiveSyncData : ISerializable
{
	private NKMDivePlayerBase updatedPlayer;

	private List<NKMDiveSquad> updatedSquads = new List<NKMDiveSquad>();

	private List<NKMDiveSlotSet> addedSlotSets = new List<NKMDiveSlotSet>();

	private List<NKMDiveSlotWithIndexes> updatedSlots = new List<NKMDiveSlotWithIndexes>();

	private NKMRewardData rewardData;

	private NKMRewardData artifactRewardData;

	private NKMItemMiscData stormMiscReward;

	public NKMDivePlayerBase UpdatedPlayer
	{
		get
		{
			return updatedPlayer;
		}
		set
		{
			updatedPlayer = value;
		}
	}

	public List<NKMDiveSquad> UpdatedSquads => updatedSquads;

	public List<NKMDiveSlotSet> AddedSlotSets => addedSlotSets;

	public List<NKMDiveSlotWithIndexes> UpdatedSlots => updatedSlots;

	public NKMRewardData RewardData
	{
		get
		{
			return rewardData;
		}
		set
		{
			rewardData = value;
		}
	}

	public NKMRewardData ArtifactRewardData
	{
		get
		{
			return artifactRewardData;
		}
		set
		{
			artifactRewardData = value;
		}
	}

	public NKMItemMiscData StormMiscReward
	{
		get
		{
			return stormMiscReward;
		}
		set
		{
			stormMiscReward = value;
		}
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref updatedPlayer);
		stream.PutOrGet(ref updatedSquads);
		stream.PutOrGet(ref addedSlotSets);
		stream.PutOrGet(ref updatedSlots);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref artifactRewardData);
		stream.PutOrGet(ref stormMiscReward);
	}
}
