using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Office;
using NKC.Office;
using NKC.UI.Component.Office;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIPopupOfficeInteract : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_office";

	private const string UI_ASSET_NAME = "AB_UI_POPUP_OFFICE_INTERACT";

	private static NKCUIPopupOfficeInteract m_Instance;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnRefresh;

	public NKCUIComStateButton m_csbtnSendBizCardAll;

	public NKCUIComStateButton m_csbtnGetBizCardAll;

	public GameObject m_objGetBizCardAllReddot;

	public NKCUIComStateButton m_csbtnRandomvisit;

	public Text m_lbItemReceiveLimit;

	public GameObject m_objSendBizAllReddot;

	public GameObject m_objSendBizAllNormal;

	public GameObject m_objSendBizAllLocked;

	public Transform m_trRootMyBizCard;

	private NKCUIComOfficeBizCard m_MyBizCard;

	public LoopScrollRect m_srBizCard;

	public GameObject m_objBizCardNone;

	private Transform m_trInactiveBizcard;

	private Dictionary<int, Stack<NKCUIComOfficeBizCard>> m_dicBizCardCache = new Dictionary<int, Stack<NKCUIComOfficeBizCard>>();

	public static NKCUIPopupOfficeInteract Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupOfficeInteract>("ab_ui_office", "AB_UI_POPUP_OFFICE_INTERACT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupOfficeInteract>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => string.Empty;

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private bool CheckRefreshInterval()
	{
		return NKCScenManager.CurrentUserData().OfficeData.CanRefreshOfficePost();
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnRefresh, OnRefresh);
		NKCUtil.SetButtonClickDelegate(m_csbtnSendBizCardAll, OnSendBizCardAll);
		NKCUtil.SetButtonClickDelegate(m_csbtnGetBizCardAll, OnGetBizCardAll);
		NKCUtil.SetButtonClickDelegate(m_csbtnRandomvisit, OnRandomVisit);
		if (m_srBizCard != null)
		{
			m_srBizCard.dOnGetObject += GetObject;
			m_srBizCard.dOnReturnObject += ReturnObject;
			m_srBizCard.dOnProvideData += ProvideData;
			m_srBizCard.SetAutoResize(2);
			m_srBizCard.PrepareCells();
		}
		if (m_trInactiveBizcard == null)
		{
			GameObject gameObject = new GameObject("inactiveBizCard");
			m_trInactiveBizcard = gameObject.transform;
			m_trInactiveBizcard.SetParent(base.transform);
			gameObject.SetActive(value: false);
		}
	}

	public void Open()
	{
		UIOpened();
		UpdateMyBizCard();
		UpdateBizCardList();
		UpdateSendBizCardAllState();
		TryRefresh();
	}

	public void UpdateMyBizCard()
	{
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		if (userProfileData == null)
		{
			NKCUtil.SetGameobjectActive(m_trRootMyBizCard, bValue: false);
			NKCPacketSender.Send_NKMPacket_MY_USER_PROFILE_INFO_REQ();
			return;
		}
		NKCUtil.SetGameobjectActive(m_trRootMyBizCard, bValue: true);
		if (m_MyBizCard == null)
		{
			m_MyBizCard = NKCUIComOfficeBizCard.GetInstance(0, m_trRootMyBizCard);
		}
		m_MyBizCard.SetData(userProfileData, null);
	}

	public void UpdateBizCardList()
	{
		NKCOfficeData officeData = NKCScenManager.CurrentUserData().OfficeData;
		m_srBizCard.TotalCount = officeData.BizcardCount;
		m_srBizCard.SetIndexPosition(0);
		bool flag = officeData.BizcardCount != 0;
		bool canReceiveBizcard = officeData.CanReceiveBizcard;
		if (m_csbtnGetBizCardAll != null)
		{
			m_csbtnGetBizCardAll.SetLock(!flag);
		}
		NKCUtil.SetGameobjectActive(m_objBizCardNone, !flag);
		NKCUtil.SetGameobjectActive(m_objGetBizCardAllReddot, flag && canReceiveBizcard);
		UpdateItemReceiveLimitState();
	}

	public void UpdateSendBizCardAllState()
	{
		if (m_csbtnSendBizCardAll != null)
		{
			bool flag = CanSendBizCardAll();
			NKCUtil.SetGameobjectActive(m_objSendBizAllReddot, flag && NKCFriendManager.FriendList.Count > 0);
			NKCUtil.SetGameobjectActive(m_objSendBizAllNormal, flag);
			NKCUtil.SetGameobjectActive(m_objSendBizAllLocked, !flag);
		}
	}

	private void UpdateItemReceiveLimitState()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			int recvCountLeft = nKMUserData.OfficeData.RecvCountLeft;
			NKCUtil.SetLabelText(m_lbItemReceiveLimit, $"{recvCountLeft}/{NKMCommonConst.Office.NameCard.DailyLimit}");
		}
	}

	private void TryRefresh()
	{
		NKCScenManager.CurrentUserData().OfficeData.TryRefreshOfficePost(bForce: false);
		if (m_csbtnRefresh != null)
		{
			m_csbtnRefresh.Lock();
		}
	}

	private bool CanSendBizCardAll()
	{
		return NKCScenManager.CurrentUserData()?.OfficeData.CanSendBizcardBroadcast ?? false;
	}

	private void Update()
	{
		if (m_csbtnRefresh != null)
		{
			m_csbtnRefresh.SetLock(!CheckRefreshInterval());
		}
		if (m_objSendBizAllLocked != null && m_objSendBizAllLocked.activeSelf)
		{
			UpdateSendBizCardAllState();
		}
	}

	private void OnRefresh()
	{
		TryRefresh();
	}

	private void OnSendBizCardAll()
	{
		if (NKCScenManager.CurrentUserData() != null)
		{
			if (!CanSendBizCardAll())
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("NEC_FAIL_OFFICE_POST_SEND_DAILY_LIMIT"));
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_OFFICE_POST_BROADCAST_REQ();
			}
		}
	}

	private void OnGetBizCardAll()
	{
		NKCPacketSender.Send_NKMPacket_OFFICE_POST_RECV_REQ();
	}

	private void OnRandomVisit()
	{
		NKCPacketSender.Send_NKMPacket_OFFICE_RANDOM_VISIT_REQ();
	}

	private RectTransform GetObject(int index)
	{
		if (index >= NKCScenManager.CurrentUserData().OfficeData.BizcardCount)
		{
			return null;
		}
		if (m_dicBizCardCache.TryGetValue(0, out var value) && value.Count > 0)
		{
			NKCUIComOfficeBizCard nKCUIComOfficeBizCard = value.Pop();
			nKCUIComOfficeBizCard.gameObject.SetActive(value: true);
			return nKCUIComOfficeBizCard.GetComponent<RectTransform>();
		}
		return NKCUIComOfficeBizCard.GetInstance(0, null).GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform go)
	{
		go.SetParent(m_trInactiveBizcard);
		NKCUIComOfficeBizCard component = go.GetComponent<NKCUIComOfficeBizCard>();
		if (component == null)
		{
			Object.Destroy(go);
			return;
		}
		if (m_dicBizCardCache.TryGetValue(0, out var value))
		{
			value.Push(component);
			return;
		}
		Stack<NKCUIComOfficeBizCard> stack = new Stack<NKCUIComOfficeBizCard>();
		stack.Push(component);
		m_dicBizCardCache[0] = stack;
	}

	private void ProvideData(Transform tr, int index)
	{
		if (index < NKCScenManager.CurrentUserData().OfficeData.BizcardCount)
		{
			NKMOfficePost bizCard = NKCScenManager.CurrentUserData().OfficeData.GetBizCard(index);
			NKCUIComOfficeBizCard component = tr.GetComponent<NKCUIComOfficeBizCard>();
			if (component != null)
			{
				component.SetData(bizCard, OnClickCard);
			}
		}
	}

	private void OnClickCard(long uid)
	{
		NKCPacketSender.Send_NKMPacket_USER_PROFILE_INFO_REQ(uid, NKM_DECK_TYPE.NDT_NORMAL);
	}
}
