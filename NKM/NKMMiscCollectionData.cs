using Cs.Protocol;
using NKM.Templet;

namespace NKM;

public class NKMMiscCollectionData : ISerializable
{
	private int miscId;

	private bool reward;

	public int MiscId => miscId;

	public bool Reward => reward;

	public NKMMiscCollectionData()
	{
	}

	public NKMMiscCollectionData(int teamId, bool reward)
	{
		miscId = teamId;
		this.reward = reward;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref miscId);
		stream.PutOrGet(ref reward);
	}

	public void GiveReward()
	{
		reward = true;
	}

	public bool IsRewardComplete()
	{
		NKMCollectionV2MiscTemplet nKMCollectionV2MiscTemplet = NKMCollectionV2MiscTemplet.Find(miscId);
		if (nKMCollectionV2MiscTemplet != null && nKMCollectionV2MiscTemplet.DefaultCollection)
		{
			return true;
		}
		return reward;
	}
}
