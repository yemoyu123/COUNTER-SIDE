using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIEpisodeViewerCCS : NKCUIBase
{
	private GameObject m_NKM_EPISODE_CCS;

	private static int m_EpisodeID = 0;

	private static int m_ActID = -1;

	private RectTransform m_rectListContent;

	private RectTransform m_rectViewPort;

	private ScrollRect m_SREPList;

	public Animator m_Animator;

	public Text m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_TITLE_TEXT;

	public Text m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_SUBTITLE_TEXT;

	public Text m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_CONTENT_TEXT;

	public GameObject m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO2;

	public GameObject m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_DOT;

	public GameObject m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO;

	public List<GameObject> m_lst_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK_DISABLE;

	public List<GameObject> m_lst_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK_CLEAR;

	public List<NKCUIComToggle> m_lst_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK;

	public GameObject m_AB_ICON_SLOT;

	public NKCUISlot m_NKCUISlot;

	public Text m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO2_START_BUTTON_TEXT;

	public GameObject m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO2_START_BUTTON_COST;

	public Text m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO2_START_BUTTON_COST_TEXT;

	public NKCUIComButton m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO2_START_BUTTON;

	public NKCUIComToggle m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK1;

	public NKCUIComToggle m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK2;

	public NKCUIComToggle m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK3;

	public NKCUIComToggle m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK4;

	private float m_ViewPortCenterX;

	private float m_fRectListContentOrgX;

	private bool m_bReserveSnapToGrid;

	private float m_fElapsedTimeReserveSnapToGrid;

	private float MIN_VELOCITY_TO_SNAP_TO_GRID = 170f;

	private List<NKCUICCSecretSlot> m_listNKCUICCSecretSlot = new List<NKCUICCSecretSlot>();

	private const int DEFAULT_SLOT_COUNT = 10;

	private NKCUICCSecretSlot m_CandidateSlotForCenter;

	private NKMTrackingFloat m_NKMTrackingFloat = new NKMTrackingFloat();

	private bool m_bBookOpen;

	private static int m_LastStageIndex = -1;

	private bool m_bReservedResetBook;

	public override string MenuName => NKCUtilString.GET_STRING_MENU_NAME_CCS;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public void SetReservedResetBook(bool bSet)
	{
		m_bReservedResetBook = bSet;
	}

	public void InitOutComponents(GameObject cNUM_EPISODE_PREFAB)
	{
		m_NKM_EPISODE_CCS = cNUM_EPISODE_PREFAB.transform.Find("NKM_EPISODE_CCS").gameObject;
		m_rectListContent = cNUM_EPISODE_PREFAB.transform.Find("NKM_EPISODE_CCS/NKM_UI_OPERATION_EP_LIST/NKM_UI_OPERATION_EP_LIST_ScrollView/NKM_UI_OPERATION_EP_LIST_Viewport/NKM_UI_OPERATION_EP_LIST_Content").gameObject.GetComponent<RectTransform>();
		m_rectViewPort = cNUM_EPISODE_PREFAB.transform.Find("NKM_EPISODE_CCS/NKM_UI_OPERATION_EP_LIST/NKM_UI_OPERATION_EP_LIST_ScrollView/NKM_UI_OPERATION_EP_LIST_Viewport").gameObject.GetComponent<RectTransform>();
		m_SREPList = cNUM_EPISODE_PREFAB.transform.Find("NKM_EPISODE_CCS/NKM_UI_OPERATION_EP_LIST/NKM_UI_OPERATION_EP_LIST_ScrollView").gameObject.GetComponent<ScrollRect>();
	}

	public static NKCUIEpisodeViewerCCS InitUI()
	{
		return NKCUIManager.OpenUI<NKCUIEpisodeViewerCCS>("NKM_EPISODE_CCS_Panel");
	}

	public void InitUI2()
	{
		NKCUIManager.OpenUI(m_NKM_EPISODE_CCS);
		NKCUtil.SetGameobjectActive(m_NKM_EPISODE_CCS, bValue: false);
		m_ViewPortCenterX = m_rectViewPort.anchoredPosition.x + m_rectViewPort.sizeDelta.x / 2f + -300f;
		m_fRectListContentOrgX = m_rectListContent.anchoredPosition.x;
		NKCUtil.SetGameobjectActive(m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO2, bValue: false);
		NKCUtil.SetGameobjectActive(m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_DOT, bValue: false);
		NKCUtil.SetGameobjectActive(m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO, bValue: false);
		if (m_NKCUISlot != null)
		{
			m_NKCUISlot.Init();
		}
		if (m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO2_START_BUTTON != null)
		{
			m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO2_START_BUTTON.PointerClick.RemoveAllListeners();
			m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO2_START_BUTTON.PointerClick.AddListener(OnStartButton);
		}
		if (m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK1 != null)
		{
			m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK1.OnValueChanged.RemoveAllListeners();
			m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK1.OnValueChanged.AddListener(OnMissionSlotSelect0);
		}
		if (m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK2 != null)
		{
			m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK2.OnValueChanged.RemoveAllListeners();
			m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK2.OnValueChanged.AddListener(OnMissionSlotSelect1);
		}
		if (m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK3 != null)
		{
			m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK3.OnValueChanged.RemoveAllListeners();
			m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK3.OnValueChanged.AddListener(OnMissionSlotSelect2);
		}
		if (m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK4 != null)
		{
			m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK4.OnValueChanged.RemoveAllListeners();
			m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_BOOK4.OnValueChanged.AddListener(OnMissionSlotSelect3);
		}
		if ((bool)base.gameObject)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void SetEpisodeID(int episodeID)
	{
		if (episodeID > 0)
		{
			m_EpisodeID = episodeID;
		}
	}

	public override void Hide()
	{
		base.Hide();
		NKCUtil.SetGameobjectActive(m_NKM_EPISODE_CCS, bValue: false);
	}

	public override void UnHide()
	{
		base.UnHide();
		NKCUtil.SetGameobjectActive(m_NKM_EPISODE_CCS, bValue: true);
	}

	private void OnSelectedActSlot(int actID)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
			if (nKMEpisodeTempletV != null && !NKMEpisodeMgr.CheckLockCounterCase(myUserData, nKMEpisodeTempletV, actID))
			{
				OnSlotSelected(actID);
			}
		}
	}

	public void OnSlotPointerUp()
	{
		m_bReserveSnapToGrid = true;
		m_fElapsedTimeReserveSnapToGrid = 0f;
	}

	private void ScrollForCenter(NKCUICCSecretSlot cNKCUICCSecretSlot, float fTime = 0.6f)
	{
		if (!(cNKCUICCSecretSlot == null))
		{
			float targetVal = cNKCUICCSecretSlot.GetHalfOfWidth() + m_rectListContent.anchoredPosition.x + (m_ViewPortCenterX - (m_rectListContent.anchoredPosition.x + cNKCUICCSecretSlot.GetCenterX()));
			m_NKMTrackingFloat.SetNowValue(m_rectListContent.anchoredPosition.x);
			m_NKMTrackingFloat.SetTracking(targetVal, fTime, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
	}

	private void SnapToGrid()
	{
		float num = float.MaxValue;
		NKCUICCSecretSlot nKCUICCSecretSlot = null;
		for (int i = 0; i < m_listNKCUICCSecretSlot.Count; i++)
		{
			NKCUICCSecretSlot nKCUICCSecretSlot2 = m_listNKCUICCSecretSlot[i];
			if (nKCUICCSecretSlot2.IsActive())
			{
				float num2 = Mathf.Abs(m_ViewPortCenterX - (nKCUICCSecretSlot2.GetCenterX() - nKCUICCSecretSlot2.GetHalfOfWidth() + (m_rectListContent.anchoredPosition.x - m_fRectListContentOrgX)));
				if (num2 < num)
				{
					num = num2;
					nKCUICCSecretSlot = nKCUICCSecretSlot2;
				}
			}
		}
		if (nKCUICCSecretSlot != null)
		{
			ScrollForCenter(nKCUICCSecretSlot);
		}
	}

	public void ResetBook()
	{
		for (int i = 0; i < m_listNKCUICCSecretSlot.Count; i++)
		{
			NKCUICCSecretSlot nKCUICCSecretSlot = m_listNKCUICCSecretSlot[i];
			if (nKCUICCSecretSlot != null && nKCUICCSecretSlot.IsActive() && nKCUICCSecretSlot.IsBookOpen())
			{
				nKCUICCSecretSlot.SetBookOpen(bSet: false);
				nKCUICCSecretSlot.m_Animator.Play("AB_UI_NKM_UI_COUNTER_CASE_SECRET_SLOT_CLOSE");
			}
		}
		if (m_bBookOpen)
		{
			m_bBookOpen = false;
			m_Animator.Play("AB_UI_NKM_UI_COUNTER_CASE_SECRET_CLOSE");
		}
		m_LastStageIndex = -1;
		m_ActID = -1;
	}

	public void OnSlotDragStart()
	{
		m_NKMTrackingFloat.StopTracking();
		ResetBook();
	}

	public void OnStartButton()
	{
		NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
	}

	private void SendUnLockPacket()
	{
		NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
	}

	public void SetMissionUI()
	{
		if (m_LastStageIndex != -1)
		{
			SetMissionUI(m_LastStageIndex);
		}
	}

	private void SetMissionUI(int stageIndex)
	{
		m_LastStageIndex = stageIndex;
		NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
	}

	public void OnMissionSlotSelect0(bool bSet)
	{
		if (bSet)
		{
			SetMissionUI(0);
		}
	}

	public void OnMissionSlotSelect1(bool bSet)
	{
		if (bSet)
		{
			SetMissionUI(1);
		}
	}

	public void OnMissionSlotSelect2(bool bSet)
	{
		if (bSet)
		{
			SetMissionUI(2);
		}
	}

	public void OnMissionSlotSelect3(bool bSet)
	{
		if (bSet)
		{
			SetMissionUI(3);
		}
	}

	private void Update()
	{
		if (!m_bOpen)
		{
			return;
		}
		if (m_bReserveSnapToGrid)
		{
			m_fElapsedTimeReserveSnapToGrid += Time.deltaTime;
			if (m_fElapsedTimeReserveSnapToGrid > 0.1f && Mathf.Abs(m_SREPList.velocity.x) <= MIN_VELOCITY_TO_SNAP_TO_GRID)
			{
				m_SREPList.velocity = Vector2.zero;
				SnapToGrid();
				m_bReserveSnapToGrid = false;
			}
		}
		m_NKMTrackingFloat.Update(Time.deltaTime);
		if (m_NKMTrackingFloat.IsTracking())
		{
			m_rectListContent.anchoredPosition = new Vector2(m_NKMTrackingFloat.GetNowValue(), m_rectListContent.anchoredPosition.y);
		}
		float num = float.MaxValue;
		NKCUICCSecretSlot nKCUICCSecretSlot = null;
		for (int i = 0; i < m_listNKCUICCSecretSlot.Count; i++)
		{
			NKCUICCSecretSlot nKCUICCSecretSlot2 = m_listNKCUICCSecretSlot[i];
			if (nKCUICCSecretSlot2.IsActive())
			{
				float num2 = Mathf.Abs(m_ViewPortCenterX - (nKCUICCSecretSlot2.GetCenterX() - nKCUICCSecretSlot2.GetHalfOfWidth() + (m_rectListContent.anchoredPosition.x - m_fRectListContentOrgX)));
				if (num2 < num)
				{
					num = num2;
					nKCUICCSecretSlot = nKCUICCSecretSlot2;
				}
				if (num2 > nKCUICCSecretSlot2.GetHalfOfWidth())
				{
					nKCUICCSecretSlot2.m_RTButton.localScale = new Vector3(0.9f, 0.9f, 1f);
				}
				else
				{
					nKCUICCSecretSlot2.m_RTButton.localScale = new Vector3(0.9f + 0.1f * ((nKCUICCSecretSlot2.GetHalfOfWidth() - num2) / nKCUICCSecretSlot2.GetHalfOfWidth()), 0.9f + 0.1f * ((nKCUICCSecretSlot2.GetHalfOfWidth() - num2) / nKCUICCSecretSlot2.GetHalfOfWidth()), 1f);
				}
			}
		}
		if (nKCUICCSecretSlot != null && m_CandidateSlotForCenter != nKCUICCSecretSlot)
		{
			nKCUICCSecretSlot.transform.SetSiblingIndex(m_listNKCUICCSecretSlot.Count - 1);
			nKCUICCSecretSlot.ResetPos();
			m_CandidateSlotForCenter = nKCUICCSecretSlot;
		}
	}

	private void SetRightInfo()
	{
		if (m_ActID != -1)
		{
			SetRightInfo(m_ActID, bRefreshComboButtonsState: false);
		}
	}

	private void SetRightInfo(int actID, bool bRefreshComboButtonsState = true)
	{
		m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_CONTENT_TEXT.text = "";
		m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_INFO_SUBTITLE_TEXT.text = "";
		NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
	}

	private void OnSlotSelected(int actID)
	{
		if (m_CandidateSlotForCenter != null && m_CandidateSlotForCenter.GetActID() == actID && !m_CandidateSlotForCenter.IsBookOpen())
		{
			m_Animator.Play("AB_UI_NKM_UI_COUNTER_CASE_SECRET");
			m_CandidateSlotForCenter.m_Animator.Play("AB_UI_NKM_UI_COUNTER_CASE_SECRET_SLOT_OPEN");
			m_CandidateSlotForCenter.SetBookOpen(bSet: true);
			m_bBookOpen = true;
			m_ActID = actID;
			m_LastStageIndex = -1;
			SetRightInfo(actID);
		}
		for (int i = 0; i < m_listNKCUICCSecretSlot.Count; i++)
		{
			NKCUICCSecretSlot nKCUICCSecretSlot = m_listNKCUICCSecretSlot[i];
			if (!(nKCUICCSecretSlot == m_CandidateSlotForCenter) && nKCUICCSecretSlot.IsActive() && nKCUICCSecretSlot.GetActID() == actID)
			{
				ResetBook();
				ScrollForCenter(nKCUICCSecretSlot);
				break;
			}
		}
	}

	public void Open()
	{
		NKCUIFadeInOut.FadeIn(0.1f);
		if (NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL) != null)
		{
			NKCUtil.SetGameobjectActive(m_NKM_EPISODE_CCS, bValue: true);
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			if (m_bReservedResetBook)
			{
				ResetBook();
			}
			m_bReservedResetBook = false;
			SetRightInfo();
			SetMissionUI();
			UIOpened();
		}
	}

	public override void CloseInternal()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		NKCUtil.SetGameobjectActive(m_NKM_EPISODE_CCS, bValue: false);
	}

	public override void OnBackButton()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
	}
}
