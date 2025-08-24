using System.Collections.Generic;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationSubStory : MonoBehaviour
{
	public NKCUIComStateButton m_btnSeasonal;

	public NKCUIComStateButton m_btnCounterCase;

	public Text m_lbTotalProgress;

	public Text m_lbSideProgress;

	public Text m_lbSplProgress;

	public NKCUIComStateButton m_btnSupplement;

	public NKCUIComStateButton m_btnSideStory;

	public ScrollRect m_sr;

	public GameObject m_objFirstEpisode;

	public GameObject m_objLastEpisode;

	public NKCUIOperationSubStoryEpViewer m_EpViewer;

	public float m_fFadeTime = 0.3f;

	public void InitUI()
	{
		m_btnSeasonal.PointerClick.RemoveAllListeners();
		m_btnSeasonal.PointerClick.AddListener(OnClickSeasonal);
		m_btnCounterCase.PointerClick.RemoveAllListeners();
		m_btnCounterCase.PointerClick.AddListener(OnClickCounterCase);
		m_btnSupplement.PointerClick.RemoveAllListeners();
		m_btnSupplement.PointerClick.AddListener(delegate
		{
			OpenList(bSupplement: true);
		});
		m_btnSideStory.PointerClick.RemoveAllListeners();
		m_btnSideStory.PointerClick.AddListener(delegate
		{
			OpenList(bSupplement: false);
		});
		m_EpViewer.InitUI();
	}

	public void Open()
	{
		SetProgressData();
		m_EpViewer.SetData();
		NKMEpisodeTempletV2 reservedEpisodeTemplet = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeTemplet();
		if (reservedEpisodeTemplet != null)
		{
			if (reservedEpisodeTemplet.m_EpisodeID == 50)
			{
				OnClickCounterCase();
			}
			else
			{
				NKCUIOperationNodeViewer.Instance.Open(reservedEpisodeTemplet);
			}
		}
		else
		{
			EPISODE_CATEGORY reservedEpisodeCategory = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeCategory();
			int lastPlayedSeasonal = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetLastPlayedSeasonal();
			if (reservedEpisodeCategory == EPISODE_CATEGORY.EC_SEASONAL && lastPlayedSeasonal > 0)
			{
				NKCUIOperationSubSeasonal.Instance.Open();
			}
		}
		int lastPlayedSubStream = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetLastPlayedSubStream();
		if (lastPlayedSubStream != 0 && m_objLastEpisode != null && m_objFirstEpisode != null)
		{
			ScrollToEpisode(lastPlayedSubStream, bShowFx: false);
		}
		NKCUtil.SetGameobjectActive(m_btnSeasonal, NKMEpisodeMgr.GetListNKMEpisodeTempletByCategory(EPISODE_CATEGORY.EC_SEASONAL).Count > 0);
		TutorialCheck();
	}

	private void ScrollToEpisode(int targetEpisodeID, bool bShowFx)
	{
		float num = m_objLastEpisode.transform.localPosition.x - m_objFirstEpisode.transform.localPosition.x;
		float num2 = 0f;
		List<NKCUIOperationSubStorySlot> epList = m_EpViewer.GetEpList();
		if (epList == null)
		{
			return;
		}
		for (int i = 0; i < epList.Count; i++)
		{
			if (!(epList[i] == null) && epList[i].GetEpisodeID() == targetEpisodeID)
			{
				num2 = epList[i].transform.localPosition.x - m_objFirstEpisode.transform.localPosition.x;
				if (bShowFx)
				{
					epList[i].ShowFocusFx();
				}
				break;
			}
		}
		m_sr.DOKill();
		m_sr.DOHorizontalNormalizedPos(num2 / num, 0.1f);
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetLastPlayedSubStream(0);
	}

	private void SetProgressData()
	{
		List<NKMEpisodeTempletV2> listNKMEpisodeTempletByCategory = NKMEpisodeMgr.GetListNKMEpisodeTempletByCategory(EPISODE_CATEGORY.EC_SIDESTORY);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < listNKMEpisodeTempletByCategory.Count; i++)
		{
			bool flag = NKMEpisodeMgr.IsClearedEpisode(listNKMEpisodeTempletByCategory[i]);
			if (listNKMEpisodeTempletByCategory[i].m_bIsSupplement)
			{
				num4++;
				if (flag)
				{
					num3++;
				}
			}
			else
			{
				num2++;
				if (flag)
				{
					num++;
				}
			}
		}
		NKCUtil.SetLabelText(m_lbSideProgress, $"{num}/{num2}");
		NKCUtil.SetLabelText(m_lbSplProgress, $"{num3}/{num4}");
		NKCUtil.SetLabelText(m_lbTotalProgress, $"{num + num3}/{num2 + num4}");
	}

	private void OnClickSeasonal()
	{
		NKCUIOperationSubSeasonal.Instance.Open();
	}

	private void OnClickCounterCase()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.COUNTERCASE))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.COUNTERCASE);
			return;
		}
		NKMEpisodeTempletV2 reservedEpisodeTemplet = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeTemplet();
		if (reservedEpisodeTemplet != null && reservedEpisodeTemplet.m_EpisodeID == 50)
		{
			NKCScenManager.GetScenManager().Get_SCEN_OPERATION().OpenCounterCaseViewer();
			return;
		}
		NKCUIFadeInOut.FadeOut(m_fFadeTime, delegate
		{
			NKCScenManager.GetScenManager().Get_SCEN_OPERATION().OpenCounterCaseViewer();
		});
	}

	private void OpenList(bool bSupplement)
	{
		NKCPopupOperationSubStoryList.Instance.Open(OnClickListSlot, bSupplement);
	}

	private void OnClickListSlot(int episodeID)
	{
		ScrollToEpisode(episodeID, bShowFx: true);
	}

	private void TutorialCheck()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Operation_SubStream);
	}
}
