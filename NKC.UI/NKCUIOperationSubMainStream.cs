using System.Collections.Generic;
using DG.Tweening;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationSubMainStream : MonoBehaviour
{
	[Header("에피소드 선택")]
	public LoopVerticalScrollRect m_loop;

	public Transform m_trObjPool;

	public NKCUIOperationSubMainStreamEPSlot m_pfbSlot;

	public NKCUIComToggleGroup m_tglGroup;

	[Header("메인")]
	public TextMeshProUGUI m_lbEpisodeNum;

	public TextMeshProUGUI m_lbEpisodeTitle;

	public TextMeshProUGUI m_lbEpisodeDesc;

	public Image m_imgBG;

	public NKCUIComStateButton m_btnStart;

	[Header("페이드 연출 관련")]
	public float m_fTextFadeDuration;

	public Ease m_TextFadeEase;

	public Color m_cTextStartColor;

	public Color m_cTextEndColor;

	[Space]
	public float m_fFadeDuration;

	public Ease m_SideFadeEase;

	public RectTransform m_rtLeftFade;

	public Vector2 m_vLeftStartPos;

	public Vector2 m_vLeftEndPos;

	public RectTransform m_rtRightFade;

	public Vector2 m_vRightStartPos;

	public Vector2 m_vRightEndPos;

	[Space]
	public float m_fFadeDuration2;

	public Ease m_FullFadeEase;

	public Image m_imgFullFade;

	public Color m_cFadeStartColor;

	public Color m_cFadeEndColor;

	public float m_FadeTime = 0.3f;

	[Header("우상단 드롭다운")]
	public NKCUIOperationRelateEpList m_DropDown;

	private Dictionary<int, NKCUIOperationSubMainStreamEPSlot> m_dicSlot = new Dictionary<int, NKCUIOperationSubMainStreamEPSlot>();

	private List<NKCUIOperationSubMainStreamEPSlot> m_lstEpSlot = new List<NKCUIOperationSubMainStreamEPSlot>();

	private Stack<NKCUIOperationSubMainStreamEPSlot> m_stkSlot = new Stack<NKCUIOperationSubMainStreamEPSlot>();

	private List<NKMEpisodeTempletV2> m_lstEpisodeTemplet = new List<NKMEpisodeTempletV2>();

	private NKMEpisodeTempletV2 m_EpisodeTemplet;

	public void InitUI()
	{
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.PrepareCells();
		m_btnStart.PointerClick.RemoveAllListeners();
		m_btnStart.PointerClick.AddListener(OnClickStart);
		m_btnStart.m_bGetCallbackWhileLocked = true;
		m_DropDown.InitUI();
	}

	private RectTransform GetObject(int idx)
	{
		NKCUIOperationSubMainStreamEPSlot nKCUIOperationSubMainStreamEPSlot = null;
		nKCUIOperationSubMainStreamEPSlot = ((m_stkSlot.Count <= 0) ? Object.Instantiate(m_pfbSlot) : m_stkSlot.Pop());
		nKCUIOperationSubMainStreamEPSlot.InitUI(OnSelectEPSlot, m_tglGroup);
		return nKCUIOperationSubMainStreamEPSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUIOperationSubMainStreamEPSlot component = tr.GetComponent<NKCUIOperationSubMainStreamEPSlot>();
		if (!(component == null))
		{
			if (component.GetEpisodeID() > 0 && m_dicSlot.ContainsKey(component.GetEpisodeID()))
			{
				m_dicSlot.Remove(component.GetEpisodeID());
			}
			m_stkSlot.Push(component);
			tr.SetParent(m_trObjPool);
		}
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUIOperationSubMainStreamEPSlot component = tr.GetComponent<NKCUIOperationSubMainStreamEPSlot>();
		if (!(component == null))
		{
			m_dicSlot.Remove(component.GetEpisodeID());
			tr.SetParent(m_loop.content);
			component.SetData(m_lstEpisodeTemplet[idx].m_EpisodeID, m_lstEpisodeTemplet[idx].GetEpisodeTitle(), idx);
			component.SetSelected(m_lstEpisodeTemplet[idx].m_EpisodeID == m_EpisodeTemplet.m_EpisodeID);
			component.RefreshRedDot();
			m_dicSlot.Add(m_lstEpisodeTemplet[idx].m_EpisodeID, component);
		}
	}

	public void Open(bool bByPassContentUnlockPopup = false)
	{
		m_dicSlot.Clear();
		m_lstEpisodeTemplet = NKMEpisodeMgr.GetListNKMEpisodeTempletByCategory(EPISODE_CATEGORY.EC_MAINSTREAM, bOnlyOpen: true);
		NKMEpisodeTempletV2 reservedEpisodeTemplet = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeTemplet();
		if (reservedEpisodeTemplet != null)
		{
			m_EpisodeTemplet = reservedEpisodeTemplet;
		}
		else if (NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetLastPlayedMainStream() > 0)
		{
			m_EpisodeTemplet = NKMEpisodeTempletV2.Find(NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetLastPlayedMainStream(), EPISODE_DIFFICULTY.NORMAL);
		}
		else
		{
			m_EpisodeTemplet = NKMEpisodeTempletV2.Find(GetLatestEpisodeTemplet().m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
		}
		if (m_EpisodeTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_rtLeftFade, bValue: false);
			NKCUtil.SetGameobjectActive(m_rtRightFade, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgFullFade, bValue: false);
			ResetFade();
			m_loop.TotalCount = m_lstEpisodeTemplet.Count;
			m_loop.RefreshCells();
			Canvas.ForceUpdateCanvases();
			m_loop.ScrollToCell(m_lstEpisodeTemplet.FindIndex((NKMEpisodeTempletV2 x) => x.m_EpisodeID == m_EpisodeTemplet.m_EpisodeID), 0.1f, LoopScrollRect.ScrollTarget.Center);
			SetData(bByPassContentUnlockPopup);
			TutorialCheck();
		}
	}

	private void ResetFade()
	{
		m_rtLeftFade.anchoredPosition = m_vLeftStartPos;
		m_rtRightFade.anchoredPosition = m_vRightStartPos;
		m_imgFullFade.color = m_cFadeStartColor;
		NKCUtil.SetLabelTextColor(m_lbEpisodeTitle, m_cTextEndColor);
		NKCUtil.SetLabelTextColor(m_lbEpisodeNum, m_cTextEndColor);
		NKCUtil.SetLabelTextColor(m_lbEpisodeDesc, m_cTextEndColor);
		m_rtLeftFade.DOKill();
		m_rtRightFade.DOKill();
		m_imgFullFade.DOKill();
		m_lbEpisodeTitle.DOKill();
		m_lbEpisodeNum.DOKill();
		m_lbEpisodeDesc.DOKill();
	}

	private void StartFade()
	{
		m_rtLeftFade.DOAnchorPos(m_vLeftEndPos, m_fFadeDuration).SetEase(m_SideFadeEase);
		m_rtRightFade.DOAnchorPos(m_vRightEndPos, m_fFadeDuration).SetEase(m_SideFadeEase);
		m_imgFullFade.DOColor(m_cFadeEndColor, m_fFadeDuration2).SetEase(m_FullFadeEase);
		NKCUtil.SetLabelTextColor(m_lbEpisodeTitle, m_cTextStartColor);
		NKCUtil.SetLabelTextColor(m_lbEpisodeNum, m_cTextStartColor);
		NKCUtil.SetLabelTextColor(m_lbEpisodeDesc, m_cTextStartColor);
		m_lbEpisodeTitle.DOColor(m_cTextEndColor, m_fTextFadeDuration).SetEase(m_TextFadeEase);
		m_lbEpisodeNum.DOColor(m_cTextEndColor, m_fTextFadeDuration).SetEase(m_TextFadeEase);
		m_lbEpisodeDesc.DOColor(m_cTextEndColor, m_fTextFadeDuration).SetEase(m_TextFadeEase);
	}

	private void SetData(bool bByPassContentUnlockPopup = false)
	{
		NKCUtil.SetGameobjectActive(m_rtLeftFade, bValue: true);
		NKCUtil.SetGameobjectActive(m_rtRightFade, bValue: true);
		NKCUtil.SetGameobjectActive(m_imgFullFade, bValue: true);
		ResetFade();
		StartFade();
		bool num = !NKMEpisodeMgr.IsPossibleEpisode(NKCScenManager.CurrentUserData(), m_EpisodeTemplet.m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
		if (num)
		{
			m_btnStart.Lock();
		}
		else
		{
			m_btnStart.UnLock();
		}
		NKCUtil.SetLabelText(m_lbEpisodeNum, m_EpisodeTemplet.GetEpisodeName());
		NKCUtil.SetLabelText(m_lbEpisodeTitle, m_EpisodeTemplet.GetEpisodeTitle());
		if (num)
		{
			NKCUtil.SetLabelText(m_lbEpisodeDesc, "");
		}
		else
		{
			NKCUtil.SetLabelText(m_lbEpisodeDesc, m_EpisodeTemplet.GetEpisodeDesc());
		}
		NKCUtil.SetImageSprite(m_imgBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Bg", m_EpisodeTemplet.m_EPThumbnail));
		m_DropDown.SetData(m_EpisodeTemplet);
		NKMEpisodeTempletV2 reservedEpisodeTemplet = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeTemplet();
		if (reservedEpisodeTemplet != null)
		{
			NKCUIOperationNodeViewer.Instance.Open(reservedEpisodeTemplet, bByPassContentUnlockPopup);
		}
	}

	public void OnClickStart()
	{
		if (m_btnStart.m_bLock)
		{
			NKMStageTempletV2 firstStage = m_EpisodeTemplet.GetFirstStage(1);
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GetUnlockConditionRequireDesc(firstStage), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetLastPlayedMainStream(m_EpisodeTemplet.m_EpisodeID);
		NKCUIFadeInOut.FadeOut(m_FadeTime, delegate
		{
			NKCUIOperationNodeViewer.Instance.Open(m_EpisodeTemplet);
		});
	}

	public void OnSelectEPSlot(int episodeID)
	{
		m_EpisodeTemplet = NKMEpisodeTempletV2.Find(episodeID, EPISODE_DIFFICULTY.NORMAL);
		if (m_EpisodeTemplet == null)
		{
			return;
		}
		foreach (KeyValuePair<int, NKCUIOperationSubMainStreamEPSlot> item in m_dicSlot)
		{
			item.Value.ChangeSelected(item.Key == episodeID);
		}
		SetData();
	}

	private NKMEpisodeTempletV2 GetLatestEpisodeTemplet()
	{
		List<NKMEpisodeTempletV2> listNKMEpisodeTempletByCategory = NKMEpisodeMgr.GetListNKMEpisodeTempletByCategory(EPISODE_CATEGORY.EC_MAINSTREAM, bOnlyOpen: true, EPISODE_DIFFICULTY.HARD);
		if (listNKMEpisodeTempletByCategory.Count > 0)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			for (int num = listNKMEpisodeTempletByCategory.Count - 1; num >= 0; num--)
			{
				NKMEpisodeTempletV2 nKMEpisodeTempletV = listNKMEpisodeTempletByCategory[num];
				NKMStageTempletV2 firstStage = nKMEpisodeTempletV.GetFirstStage(1);
				if (NKMEpisodeMgr.CheckEpisodeMission(myUserData, firstStage))
				{
					return nKMEpisodeTempletV;
				}
			}
		}
		return null;
	}

	public void SetTutorialMainstreamGuide(NKCGameEventManager.NKCGameEventTemplet eventTemplet, UnityAction Complete)
	{
		NKCUIOperationSubMainStreamEPSlot nKCUIOperationSubMainStreamEPSlot = m_lstEpSlot.Find((NKCUIOperationSubMainStreamEPSlot x) => x.GetEpisodeID() == eventTemplet.Value);
		if (nKCUIOperationSubMainStreamEPSlot == null)
		{
			Complete?.Invoke();
			return;
		}
		m_loop.SetIndexPosition(nKCUIOperationSubMainStreamEPSlot.GetUIIndex());
		NKCGameEventManager.OpenTutorialGuideBySettedFace(nKCUIOperationSubMainStreamEPSlot.GetComponent<RectTransform>(), NKCUIOverlayTutorialGuide.ClickGuideType.Touch, eventTemplet, null);
		NKCUIOverlayTutorialGuide.Instance.SetStealInput(delegate
		{
			NKCUIOverlayTutorialGuide.CheckInstanceAndClose();
			Complete?.Invoke();
			NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(eventTemplet.Value, EPISODE_DIFFICULTY.NORMAL);
			NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedStage(nKMEpisodeTempletV.GetFirstStage(1));
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
		});
	}

	private void TutorialCheck()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Operation_MainStream);
	}
}
