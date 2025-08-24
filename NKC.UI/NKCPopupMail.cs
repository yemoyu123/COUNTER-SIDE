using System.Collections.Generic;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupMail : NKCUIBase
{
	public delegate void OnReceive(long index);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_mail";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_MAIL";

	private static NKCPopupMail m_Instance;

	public NKCComTMPUIHyperLink m_lbContent;

	public Text m_lbDate;

	public GameObject m_objTimeLeft;

	public Text m_lbTimeLeft;

	[Header("아이템 슬롯")]
	public List<NKCUISlot> m_lstSlot;

	[Header("하단 버튼")]
	public NKCUIComStateButton m_cbtnReceive;

	public NKCUIComStateButton m_cbtnClose;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private NKMPostData m_PostData;

	private OnReceive dOnReceive;

	public static NKCPopupMail Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupMail>("ab_ui_nkm_ui_mail", "NKM_UI_POPUP_MAIL", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupMail>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "MAIL CONTENT";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void InitUI()
	{
		if (m_NKCUIOpenAnimator == null)
		{
			m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		}
		foreach (NKCUISlot item in m_lstSlot)
		{
			item.Init();
		}
		NKCUtil.SetButtonClickDelegate(m_cbtnReceive, OnBtnReceive);
		NKCUtil.SetHotkey(m_cbtnReceive, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_cbtnClose, OnBtnClose);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_PostData = null;
	}

	public void Open(NKMPostData postData, OnReceive onReceive)
	{
		m_PostData = postData;
		dOnReceive = onReceive;
		NKCUtil.SetLabelText(m_lbContent, NKCUtilString.GetFinalMailContents(postData.contents));
		NKCUtil.SetLabelText(m_lbDate, NKMTime.UTCtoLocal(postData.sendDate).ToString("yyyy-MM-dd"));
		NKCUtil.SetLabelText(m_lbTimeLeft, NKCUtilString.GetRemainTimeString(postData.expirationDate, 2));
		SetSlot(postData.items);
		if (postData.expirationDate >= NKMConst.Post.UnlimitedExpirationUtcDate)
		{
			NKCUtil.SetLabelText(m_lbTimeLeft, NKCUtilString.GET_STRING_TIME_NO_LIMIT);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbTimeLeft, NKCUtilString.GetRemainTimeString(postData.expirationDate, 2));
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	public static void CheckInstanceAndClose()
	{
		if (!(Instance == null))
		{
			Instance.Close();
		}
	}

	private void SetSlot(List<NKMRewardInfo> lstPostItem)
	{
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			NKCUISlot nKCUISlot = m_lstSlot[i];
			if (i < lstPostItem.Count)
			{
				NKMRewardInfo nKMRewardInfo = lstPostItem[i];
				bool flag = NKCUIMailSlot.IsSlotVisible(nKMRewardInfo.rewardType, nKMRewardInfo.ID, nKMRewardInfo.Count);
				NKCUtil.SetGameobjectActive(nKCUISlot, flag);
				if (flag)
				{
					nKCUISlot.SetData(NKCUISlot.SlotData.MakePostItemData(nKMRewardInfo), bEnableLayoutElement: false);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUISlot, bValue: false);
			}
		}
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	private void OnBtnReceive()
	{
		if (dOnReceive != null)
		{
			dOnReceive((m_PostData != null) ? m_PostData.postIndex : (-1));
		}
	}

	private void OnBtnClose()
	{
		Close();
	}
}
