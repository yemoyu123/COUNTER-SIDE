using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKCRandomMoldBoxTemplet : INKMTemplet
{
	public int index;

	public int m_RewardGroupID;

	public NKM_REWARD_TYPE m_reward_type;

	public int m_RewardID;

	public Dictionary<int, int> m_dicRewardGroup = new Dictionary<int, int>();

	public int Key => index;

	public static NKCRandomMoldBoxTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMItemManagerEx.cs", 206))
		{
			return null;
		}
		NKCRandomMoldBoxTemplet nKCRandomMoldBoxTemplet = new NKCRandomMoldBoxTemplet();
		if ((1u & (cNKMLua.GetData("IDX", ref nKCRandomMoldBoxTemplet.index) ? 1u : 0u) & (cNKMLua.GetData("m_RewardGroupID", ref nKCRandomMoldBoxTemplet.m_RewardGroupID) ? 1u : 0u) & (cNKMLua.GetData("m_RewardType", ref nKCRandomMoldBoxTemplet.m_reward_type) ? 1u : 0u) & (cNKMLua.GetData("m_RewardID", ref nKCRandomMoldBoxTemplet.m_RewardID) ? 1u : 0u)) == 0)
		{
			Log.ErrorAndExit($"NKCRandomMoldBoxTemplet 정보를 읽어오지 못하였습니다. index : {nKCRandomMoldBoxTemplet.index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMItemManagerEx.cs", 218);
		}
		nKCRandomMoldBoxTemplet.CheckValidation();
		return nKCRandomMoldBoxTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	private void CheckValidation()
	{
		if (m_reward_type == NKM_REWARD_TYPE.RT_EQUIP && NKMItemManager.GetEquipTemplet(m_RewardID) == null)
		{
			Log.ErrorAndExit($"[RandomMoldBox] 보상 정보가 존재하지 않음 m_RewardGroupID : {m_RewardGroupID}, m_reward_type : {m_reward_type}, m_RewardID : {m_RewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMItemManagerEx.cs", 244);
		}
	}
}
