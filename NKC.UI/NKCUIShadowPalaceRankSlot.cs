using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIShadowPalaceRankSlot : MonoBehaviour
{
	public Text m_txtRank;

	public Text m_txtLevel;

	public Text m_txtName;

	public Text m_txtScore;

	public NKCUISlotProfile m_unitSlot;

	public Image m_imgRank;

	public GameObject m_objMyRank;

	public void Init()
	{
	}

	public void SetData(LeaderBoardSlotData rankData, int rank, bool bMyRank)
	{
		NKCUtil.SetLabelText(m_txtRank, rank.ToString());
		NKCUtil.SetLabelText(m_txtLevel, NKCStringTable.GetString("SI_DP_LEVEL_ONE_PARAM", rankData.level));
		NKCUtil.SetLabelText(m_txtName, rankData.nickname);
		NKCUtil.SetLabelText(m_txtScore, rankData.score);
		m_unitSlot.SetProfiledata(rankData.Profile, null);
		Sprite sprite = null;
		switch (rank)
		{
		case 1:
			sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_SHADOW_SPRITE", "Rank_01");
			break;
		case 2:
			sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_SHADOW_SPRITE", "Rank_02");
			break;
		case 3:
			sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_SHADOW_SPRITE", "Rank_03");
			break;
		}
		m_imgRank.enabled = sprite != null;
		if (sprite != null)
		{
			m_imgRank.sprite = sprite;
		}
		NKCUtil.SetGameobjectActive(m_objMyRank, bMyRank);
	}
}
