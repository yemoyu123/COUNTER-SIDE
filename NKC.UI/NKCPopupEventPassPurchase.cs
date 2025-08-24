using System.Collections;
using System.Collections.Generic;
using System.Text;
using NKM.EventPass;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupEventPassPurchase : NKCUIBase
{
	public delegate bool EventTimeCheck(bool alarm);

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_EVENT_PASS";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_EVENT_PASS_BUY";

	private static NKCPopupEventPassPurchase m_Instance;

	public NKCUIComEventPassBuySlot m_corePassPlan;

	public NKCUIComEventPassBuySlot m_corePassPlanPlus;

	public NKCUIComEventPassEquip m_eventPassEquip;

	public NKCUICharacterView m_characterView;

	public GameObject m_objEquipRoot;

	public GameObject m_objBuyNotice;

	public Animator m_animator;

	public int m_iRefundImpossibleMsgSize;

	public int m_iExpWarningMsgSize;

	public Color m_colExpOverflowMsg;

	public NKCUIComStateButton m_csbtnClose;

	private EventTimeCheck m_dEventTimeCheck;

	public static NKCPopupEventPassPurchase Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventPassPurchase>("AB_UI_NKM_UI_EVENT_PASS", "NKM_UI_POPUP_EVENT_PASS_BUY", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupEventPassPurchase>();
				m_Instance?.Init();
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

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_EVENTPASS_EVENT_PASS_MENU_TITLE;

	public override List<int> UpsideMenuShowResourceList
	{
		get
		{
			if (NKCUIEventPass.HasInstance)
			{
				return NKCUIEventPass.Instance.UpsideMenuShowResourceList;
			}
			return base.UpsideMenuShowResourceList;
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void Init()
	{
		NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
		if (eventPassDataManager != null && NKMTempletContainer<NKMEventPassTemplet>.Find(eventPassDataManager.EventPassId) != null)
		{
			m_corePassPlan?.Init();
			m_corePassPlanPlus?.Init();
			m_characterView?.Init();
			NKCUtil.SetButtonClickDelegate(m_csbtnClose, OnClickClose);
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Open(bool endTimeNotice, EventTimeCheck eventTimeCheck)
	{
		NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
		if (eventPassDataManager == null)
		{
			return;
		}
		NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(eventPassDataManager.EventPassId);
		if (nKMEventPassTemplet != null)
		{
			NKMEventPassCoreProductTemplet corePassProduct = nKMEventPassTemplet.CorePassProduct;
			NKMEventPassCorePlusProductTemplet corePassPlusPrdouct = nKMEventPassTemplet.CorePassPlusPrdouct;
			if (corePassProduct != null)
			{
				string title = NKCStringTable.GetString(corePassProduct.StrId);
				string desc = NKCStringTable.GetString(corePassProduct.DescStrId);
				m_corePassPlan.SetData(title, desc, NKCUIEventPass.Instance.UserPassLevel, 0, corePassProduct.PriceId, corePassProduct.PriceCount, 0f, 0, OnClickCorePassBuy);
			}
			if (corePassPlusPrdouct != null)
			{
				int addPassLevel = corePassPlusPrdouct.PassExp / nKMEventPassTemplet.PassLevelUpExp;
				string title2 = NKCStringTable.GetString(corePassPlusPrdouct.StrId);
				string desc2 = NKCStringTable.GetString(corePassPlusPrdouct.DescStrId);
				int corePassPriceDiscounted = nKMEventPassTemplet.GetCorePassPriceDiscounted(corePassPlusPrdouct.PriceCount);
				m_corePassPlanPlus.SetData(title2, desc2, NKCUIEventPass.Instance.UserPassLevel, addPassLevel, corePassPlusPrdouct.PriceId, corePassPlusPrdouct.PriceCount, nKMEventPassTemplet.CorePassDiscountPercent, corePassPriceDiscounted, OnClickCorePassPlusBuy);
			}
			NKCUIEventPass.SetMaxLevelMainRewardImage(nKMEventPassTemplet, m_characterView, m_eventPassEquip, m_objEquipRoot);
			NKCUtil.SetGameobjectActive(m_objBuyNotice, endTimeNotice);
			m_dEventTimeCheck = eventTimeCheck;
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			UIOpened();
		}
	}

	private void OnClickCorePassBuy()
	{
		if (m_dEventTimeCheck != null && !m_dEventTimeCheck(alarm: true))
		{
			return;
		}
		NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
		if (eventPassDataManager == null)
		{
			return;
		}
		NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(eventPassDataManager.EventPassId);
		if (nKMEventPassTemplet == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(NKCUtilString.GET_STRING_PURCHASE_POPUP_DESC, NKCStringTable.GetString(nKMEventPassTemplet.CorePassProduct.StrId));
		stringBuilder.Append("\n");
		stringBuilder.AppendFormat("<size={0}>{1}</size>", m_iRefundImpossibleMsgSize, NKCUtilString.GET_STRING_PURCHASE_REFUND_IMPOSSIBLE);
		NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_NOTICE, stringBuilder.ToString(), nKMEventPassTemplet.CorePassProduct.PriceId, nKMEventPassTemplet.CorePassProduct.PriceCount, delegate
		{
			if (m_dEventTimeCheck == null || m_dEventTimeCheck(alarm: true))
			{
				NKCPacketSender.Send_NKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_REQ();
			}
		});
	}

	private void OnClickCorePassPlusBuy()
	{
		if (m_dEventTimeCheck != null && !m_dEventTimeCheck(alarm: true))
		{
			return;
		}
		NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
		if (eventPassDataManager == null)
		{
			return;
		}
		NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(eventPassDataManager.EventPassId);
		if (nKMEventPassTemplet == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(NKCUtilString.GET_STRING_PURCHASE_POPUP_DESC, NKCStringTable.GetString(nKMEventPassTemplet.CorePassPlusPrdouct.StrId));
		stringBuilder.Append("\n");
		stringBuilder.AppendFormat("<size={0}>{1}</size>", m_iRefundImpossibleMsgSize, NKCUtilString.GET_STRING_PURCHASE_REFUND_IMPOSSIBLE);
		if (NKCUIEventPass.Instance.IsExpOverflowed(nKMEventPassTemplet, nKMEventPassTemplet.CorePassPlusPrdouct.PassExp))
		{
			stringBuilder.Append("\n");
			string arg = ColorUtility.ToHtmlStringRGB(m_colExpOverflowMsg);
			stringBuilder.AppendFormat("<color=#{0}><size={1}>{2}</size></color>", arg, m_iExpWarningMsgSize, NKCUtilString.GET_STRING_EVENTPASS_CORE_PASS_PLUS_PURCHASE_EXP_LOSS);
		}
		int num = 0;
		num = ((!(nKMEventPassTemplet.CorePassDiscountPercent > 0f)) ? nKMEventPassTemplet.CorePassPlusPrdouct.PriceCount : nKMEventPassTemplet.GetCorePassPriceDiscounted(nKMEventPassTemplet.CorePassPlusPrdouct.PriceCount));
		NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_NOTICE, stringBuilder.ToString(), nKMEventPassTemplet.CorePassPlusPrdouct.PriceId, num, delegate
		{
			if (m_dEventTimeCheck == null || m_dEventTimeCheck(alarm: true))
			{
				NKCPacketSender.Send_NKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_PLUS_REQ();
			}
		});
	}

	private void OnClickClose()
	{
		Close();
	}

	private IEnumerator IClosePopup()
	{
		m_animator.SetTrigger("out");
		while (!m_animator.GetCurrentAnimatorStateInfo(0).IsName("BUY_OUTRO") || !(m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f))
		{
			yield return null;
		}
		Close();
	}

	private void OnDestroy()
	{
		m_corePassPlan = null;
		m_corePassPlanPlus = null;
		m_eventPassEquip = null;
		m_characterView = null;
		m_objEquipRoot = null;
		m_objBuyNotice = null;
		m_animator = null;
		m_csbtnClose = null;
		m_characterView?.CleanUp();
		m_characterView = null;
	}
}
