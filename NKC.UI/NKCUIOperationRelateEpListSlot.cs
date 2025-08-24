using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;

namespace NKC.UI;

public class NKCUIOperationRelateEpListSlot : MonoBehaviour
{
	public TMP_Text m_Category;

	public TMP_Text m_lbTitle;

	public GameObject m_objClear;

	public bool SetData(int episodeID)
	{
		return SetData(NKMEpisodeTempletV2.Find(episodeID, EPISODE_DIFFICULTY.NORMAL));
	}

	public bool SetData(NKMEpisodeTempletV2 epTemplet)
	{
		if (epTemplet == null || !epTemplet.IsOpen)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return false;
		}
		if (epTemplet.m_EPCategory == EPISODE_CATEGORY.EC_SIDESTORY)
		{
			if (epTemplet.m_bIsSupplement)
			{
				NKCUtil.SetLabelText(m_Category, NKCUtilString.GET_STRING_EPISODE_SUPPLEMENT);
			}
			else
			{
				NKCUtil.SetLabelText(m_Category, NKCUtilString.GET_STRING_EPISODE_CATEGORY_EC_SIDESTORY);
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_Category, NKCUtilString.GetEpisodeCategory(epTemplet.m_EPCategory));
		}
		NKCUtil.SetLabelText(m_lbTitle, epTemplet.GetEpisodeName());
		NKCUtil.SetGameobjectActive(m_objClear, NKMEpisodeMgr.CheckClear(NKCScenManager.CurrentUserData(), epTemplet));
		return true;
	}
}
