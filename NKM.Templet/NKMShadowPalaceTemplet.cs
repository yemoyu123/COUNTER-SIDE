using System.Collections.Generic;
using Cs.Logging;
using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMShadowPalaceTemplet : INKMTemplet
{
	public static int RewardMultiplyItemId;

	public static int RewardMultiplyItemCount;

	public int PALACE_ID;

	public STAGE_UNLOCK_REQ_TYPE STAGE_UNLOCK_REQ_TYPE;

	public int STAGE_UNLOCK_REQ_VALUE;

	public int STAGE_REQ_ITEM_ID;

	public int STAGE_REQ_ITEM_COUNT;

	public int BATTLE_GROUP_ID;

	public List<NKMRewardInfo> COMPLETE_REWARDS = new List<NKMRewardInfo>();

	public int MaxRewardMultiply = 5;

	private string PALACE_NAME;

	private string PALACE_NAME_SUB;

	public int PALACE_NUM_UI;

	public string BUTTON_Prefab;

	public string MUSIC_ASSET_BUNDLE_NAME;

	public string STAGE_MUSIC_NAME;

	public int RECOMMEND_OPERATION_POWER;

	public string PALACE_IMG;

	public string PlaceName => PALACE_NAME;

	public string PlaceSubName => PALACE_NAME_SUB;

	public int Key => PALACE_ID;

	public string PalaceName => NKCStringTable.GetString(PALACE_NAME);

	public string PalaceDesc => NKCStringTable.GetString(PALACE_NAME_SUB);

	public static NKMShadowPalaceTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMShadowPalaceTemplet.cs", 44))
		{
			return null;
		}
		NKMShadowPalaceTemplet nKMShadowPalaceTemplet = new NKMShadowPalaceTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("PALACE_ID", ref nKMShadowPalaceTemplet.PALACE_ID);
		flag &= cNKMLua.GetData("STAGE_UNLOCK_REQ_TYPE", ref nKMShadowPalaceTemplet.STAGE_UNLOCK_REQ_TYPE);
		flag &= cNKMLua.GetData("STAGE_UNLOCK_REQ_VALUE", ref nKMShadowPalaceTemplet.STAGE_UNLOCK_REQ_VALUE);
		flag &= cNKMLua.GetData("STAGE_REQ_ITEM_ID", ref nKMShadowPalaceTemplet.STAGE_REQ_ITEM_ID);
		flag &= cNKMLua.GetData("STAGE_REQ_ITEM_COUNT", ref nKMShadowPalaceTemplet.STAGE_REQ_ITEM_COUNT);
		flag &= cNKMLua.GetData("BATTLE_GROUP_ID", ref nKMShadowPalaceTemplet.BATTLE_GROUP_ID);
		flag &= cNKMLua.GetData("m_RewardMultiplyMax", ref nKMShadowPalaceTemplet.MaxRewardMultiply);
		for (int i = 1; i <= 3; i++)
		{
			NKM_REWARD_TYPE result = NKM_REWARD_TYPE.RT_NONE;
			int rValue = 0;
			int rValue2 = 0;
			cNKMLua.GetData($"COMPLETE_REWARD_TYPE_{i}", ref result);
			cNKMLua.GetData($"COMPLETE_REWARD_ID_{i}", ref rValue);
			cNKMLua.GetData($"COMPLETE_REWARD_QUANTITY_{i}", ref rValue2);
			if (rValue != 0)
			{
				nKMShadowPalaceTemplet.COMPLETE_REWARDS.Add(new NKMRewardInfo
				{
					rewardType = result,
					ID = rValue,
					Count = rValue2,
					paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_FREE
				});
			}
		}
		flag &= cNKMLua.GetData("PALACE_NAME", ref nKMShadowPalaceTemplet.PALACE_NAME);
		flag &= cNKMLua.GetData("PALACE_NUM_UI", ref nKMShadowPalaceTemplet.PALACE_NUM_UI);
		flag &= cNKMLua.GetData("PALACE_NAME_SUB", ref nKMShadowPalaceTemplet.PALACE_NAME_SUB);
		flag &= cNKMLua.GetData("BUTTON_Prefab", ref nKMShadowPalaceTemplet.BUTTON_Prefab);
		flag &= cNKMLua.GetData("RECOMMEND_OPERATION_POWER", ref nKMShadowPalaceTemplet.RECOMMEND_OPERATION_POWER);
		cNKMLua.GetData("MUSIC_ASSET_BUNDLE_NAME", ref nKMShadowPalaceTemplet.MUSIC_ASSET_BUNDLE_NAME);
		cNKMLua.GetData("STAGE_MUSIC_NAME", ref nKMShadowPalaceTemplet.STAGE_MUSIC_NAME);
		cNKMLua.GetData("PALACE_IMG", ref nKMShadowPalaceTemplet.PALACE_IMG);
		if (!flag)
		{
			return null;
		}
		return nKMShadowPalaceTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (MaxRewardMultiply <= 1)
		{
			Log.ErrorAndExit($"보상 배수 맥스 수치가 1을 초과해야 합니다. palaceId: {PALACE_ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMShadowPalaceTemplet.cs", 100);
		}
	}
}
