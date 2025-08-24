using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKCUIOPCounterCase : MonoBehaviour
{
	public enum NKC_COUNTER_CASE_TYPE
	{
		NDMT_NORMAL,
		NDMT_SECRET,
		NDMT_COUNT
	}

	private readonly Color DISABLED_COLOR = new Color(1f, 69f / 85f, 0.23137255f);

	private readonly Color ENABLED_COLOR = Color.white;

	private static NKCUIOPCounterCase m_scNKCUIOPCounterCase;

	public NKCUIOPCounterCaseSlot m_COUNTERCASE_NORMAL;

	public NKCUIOPCounterCaseSlot m_COUNTERCASE_SECRET;

	public static NKCUIOPCounterCase GetInstance()
	{
		return m_scNKCUIOPCounterCase;
	}

	public void InitUI()
	{
		m_scNKCUIOPCounterCase = base.gameObject.GetComponent<NKCUIOPCounterCase>();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKMEpisodeTempletV2 nKMEpisodeTempletV = null;
		nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(50, EPISODE_DIFFICULTY.NORMAL);
		m_COUNTERCASE_NORMAL.SetData(nKMEpisodeTempletV, ContentsType.COUNTERCASE, OnClickCounterCaseNormal);
		nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(51, EPISODE_DIFFICULTY.NORMAL);
		m_COUNTERCASE_SECRET.SetData(nKMEpisodeTempletV, ContentsType.COUNTERCASE_SECRET, OnClickCounterCaseSecret);
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void SelectEP(NKMEpisodeTempletV2 cNKMEpisodeTemplet)
	{
		if (cNKMEpisodeTemplet != null && !NKMEpisodeMgr.IsPossibleEpisode(NKCScenManager.GetScenManager().GetMyUserData(), cNKMEpisodeTemplet))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.COUNTERCASE_SECRET);
		}
	}

	public void OnClickCounterCaseNormal()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.COUNTERCASE))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.COUNTERCASE);
		}
		else
		{
			SelectEP(NKMEpisodeTempletV2.Find(50, EPISODE_DIFFICULTY.NORMAL));
		}
	}

	public void OnClickCounterCaseSecret()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.COUNTERCASE_SECRET))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.COUNTERCASE_SECRET);
		}
		else
		{
			SelectEP(NKMEpisodeTempletV2.Find(51, EPISODE_DIFFICULTY.NORMAL));
		}
	}
}
