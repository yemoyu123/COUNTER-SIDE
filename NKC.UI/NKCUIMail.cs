using System;
using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIMail : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_mail";

	private const string UI_ASSET_NAME = "NKM_UI_MAIL";

	private static NKCUIMail m_Instance;

	private List<NKCUIMailSlot> m_lstMailSlot = new List<NKCUIMailSlot>();

	public NKCUIMailSlot m_pfbMailSlot;

	public LoopScrollRect m_LoopScrollRect;

	public RectTransform m_rtMailSlotPool;

	private Stack<RectTransform> m_stkMailPool = new Stack<RectTransform>();

	public NKCUIComStateButton m_cbtnRefresh;

	public NKCUIComStateButton m_cbtnReceiveAll;

	public Text m_lbMailCount;

	public GameObject m_objNoMail;

	private const float MAIL_TIMER_UPDATE_INVERVAL = 10f;

	private float m_fUpdateTimer;

	private bool bSlotReady;

	private readonly List<int> lstResources = new List<int> { 1, 2, 101, 102 };

	public static NKCUIMail Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIMail>("ab_ui_nkm_ui_mail", "NKM_UI_MAIL", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIMail>();
				m_Instance.Init();
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

	public override string MenuName => NKCUtilString.GET_STRING_MAIL;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.ON_PLAY_GAME;

	public override List<int> UpsideMenuShowResourceList => lstResources;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCMailManager.dOnMailCountChange = (NKCMailManager.OnMailCountChange)Delegate.Remove(NKCMailManager.dOnMailCountChange, new NKCMailManager.OnMailCountChange(OnMailCountChanged));
		base.gameObject.SetActive(value: false);
	}

	public void Init()
	{
		m_LoopScrollRect.dOnGetObject += MakeSlot;
		m_LoopScrollRect.dOnReturnObject += ReturnSlot;
		m_LoopScrollRect.dOnProvideData += ProvideSlotData;
		m_LoopScrollRect.ContentConstraintCount = 1;
		NKCUtil.SetScrollHotKey(m_LoopScrollRect);
		m_cbtnReceiveAll.PointerClick.AddListener(ReceiveAll);
		m_cbtnRefresh.PointerClick.AddListener(TryRefreshMail);
	}

	private RectTransform MakeSlot(int index)
	{
		if (m_stkMailPool.Count > 0)
		{
			RectTransform rectTransform = m_stkMailPool.Pop();
			NKCUtil.SetGameobjectActive(rectTransform, bValue: true);
			return rectTransform;
		}
		NKCUIMailSlot nKCUIMailSlot = UnityEngine.Object.Instantiate(m_pfbMailSlot);
		nKCUIMailSlot.Init();
		nKCUIMailSlot.transform.localPosition = Vector3.zero;
		nKCUIMailSlot.transform.localScale = Vector3.one;
		return nKCUIMailSlot.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
		go.SetParent(m_rtMailSlotPool);
		m_stkMailPool.Push(go.GetComponent<RectTransform>());
	}

	private void ProvideSlotData(Transform tr, int idx)
	{
		NKMPostData mailByIndex = NKCMailManager.GetMailByIndex(idx);
		if (mailByIndex != null)
		{
			NKCUIMailSlot component = tr.GetComponent<NKCUIMailSlot>();
			if (component != null)
			{
				component.SetData(mailByIndex, TryReceiveMail, OpenMail);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
		}
	}

	public void Open()
	{
		NKCMailManager.dOnMailCountChange = (NKCMailManager.OnMailCountChange)Delegate.Combine(NKCMailManager.dOnMailCountChange, new NKCMailManager.OnMailCountChange(OnMailCountChanged));
		base.gameObject.SetActive(value: true);
		if (!bSlotReady)
		{
			bSlotReady = true;
			m_LoopScrollRect.PrepareCells();
		}
		if (NKCMailManager.CanRefreshMail())
		{
			TryRefreshMail();
		}
		else
		{
			SetMailCount(NKCMailManager.GetTotalMailCount());
			UpdateMailList();
		}
		m_LoopScrollRect.velocity = new Vector2(0f, 0f);
		m_LoopScrollRect.SetIndexPosition(0);
		UIOpened();
	}

	private void Update()
	{
		SetRefreshMailEnable(NKCMailManager.CanRefreshMail());
		if (m_LoopScrollRect.normalizedPosition.y > 1f)
		{
			GetNextMail();
		}
		m_fUpdateTimer += Time.deltaTime;
		if (m_fUpdateTimer > 10f)
		{
			CheckAndUpdateMailTimer();
		}
	}

	private void CheckAndUpdateMailTimer()
	{
		NKCMailManager.CheckAndRemoveExpiredMail();
		foreach (NKCUIMailSlot item in m_lstMailSlot)
		{
			NKMPostData mailByPostID = NKCMailManager.GetMailByPostID(item.Index);
			if (mailByPostID != null)
			{
				if (mailByPostID.expirationDate < NKMConst.Post.UnlimitedExpirationUtcDate)
				{
					item.UpdateTime();
				}
			}
			else
			{
				Debug.LogError("Logic error");
			}
		}
	}

	public void SetMailCount(int count)
	{
		m_lbMailCount.text = string.Format(NKCUtilString.GET_STRING_MAIL_HAVE_COUNT, count);
	}

	public void OnMailCountChanged(int newTotalCount)
	{
		if (base.IsOpen)
		{
			UpdateMailList();
			SetMailCount(newTotalCount);
			NKCPopupMail.CheckInstanceAndClose();
		}
	}

	private void UpdateMailList()
	{
		NKCUtil.SetGameobjectActive(m_objNoMail, NKCMailManager.GetTotalMailCount() == 0);
		m_LoopScrollRect.TotalCount = NKCMailManager.GetReceivedMailCount();
		m_LoopScrollRect.RefreshCells();
		SetReceiveAllEnable(NKCMailManager.GetAllowedReceiveAllCount() > 0);
	}

	private void TryReceiveMail(long index)
	{
		NKCPacketSender.Send_NKMPacket_POST_RECEIVE_REQ(index);
	}

	private void ReceiveAll()
	{
		if (NKCMailManager.GetTotalMailCount() > 0)
		{
			NKCPacketSender.Send_NKMPacket_POST_RECEIVE_REQ(0L);
		}
	}

	public void TryRefreshMail()
	{
		m_LoopScrollRect.StopMovement();
		m_LoopScrollRect.SetIndexPosition(0);
		NKCMailManager.RefreshMailList();
	}

	public void GetNextMail()
	{
		NKCMailManager.GetNextMail();
	}

	private void SetRefreshMailEnable(bool value)
	{
		if (value)
		{
			m_cbtnRefresh.UnLock();
		}
		else
		{
			m_cbtnRefresh.Lock();
		}
	}

	private void SetReceiveAllEnable(bool value)
	{
		if (value)
		{
			m_cbtnReceiveAll.UnLock();
		}
		else
		{
			m_cbtnReceiveAll.Lock();
		}
	}

	private void OpenMail(NKMPostData postData)
	{
		NKCPopupMail.Instance.Open(postData, TryReceiveMail);
	}
}
