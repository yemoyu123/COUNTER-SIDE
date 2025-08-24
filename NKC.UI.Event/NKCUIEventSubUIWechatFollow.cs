using System.Collections.Generic;
using ClientPacket.Event;
using NKC.Publisher;
using NKM.Event;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventSubUIWechatFollow : NKCUIEventSubUIBase
{
	public List<NKCUISlot> m_lstRewardSlot;

	public Text m_lbCode;

	public NKCUIComStateButton m_csbtnCopy;

	public NKCUIComStateButton m_csbtnGet;

	public GameObject m_objComplete;

	private static float s_fLastUpdateTime = float.MinValue;

	private static WechatCouponData s_WechatCouponData = new WechatCouponData();

	private static bool s_bSendPacketAfterRefresh = true;

	public static void SetSendPacketAfterRefresh(bool bSet)
	{
		s_bSendPacketAfterRefresh = bSet;
	}

	public static void DoAfterLogout()
	{
		s_WechatCouponData = null;
		s_fLastUpdateTime = float.MinValue;
		SetSendPacketAfterRefresh(bSet: true);
	}

	public static void SetWechatCouponData(WechatCouponData cWechatCouponData)
	{
		s_WechatCouponData = cWechatCouponData;
	}

	public override void Init()
	{
		base.Init();
		if (m_csbtnCopy != null)
		{
			m_csbtnCopy.PointerClick.RemoveAllListeners();
			m_csbtnCopy.PointerClick.AddListener(OnClickCopy);
		}
		if (m_csbtnGet != null)
		{
			m_csbtnGet.PointerClick.RemoveAllListeners();
			m_csbtnGet.PointerClick.AddListener(OnClickGet);
		}
		if (m_lstRewardSlot == null)
		{
			return;
		}
		for (int i = 0; i < m_lstRewardSlot.Count; i++)
		{
			NKCUISlot nKCUISlot = m_lstRewardSlot[i];
			if (!(nKCUISlot == null))
			{
				nKCUISlot.Init();
			}
		}
	}

	private void OnClickGet()
	{
		if (m_tabTemplet != null)
		{
			NKCPacketSender.Send_NKMPacket_WECHAT_COUPON_REWARD_REQ(m_tabTemplet.Key);
		}
	}

	public override void Open(NKMEventTabTemplet tabTemplet)
	{
		m_tabTemplet = tabTemplet;
		UpdateUI();
		if (CheckComplete())
		{
			CheckCoupon(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL, bForce: true);
		}
	}

	private bool CheckComplete()
	{
		if (s_WechatCouponData == null || s_WechatCouponData.state != WechatCouponState.Completed)
		{
			return true;
		}
		return false;
	}

	private void CheckCoupon(NKC_OPEN_WAIT_BOX_TYPE eNKC_OPEN_WAIT_BOX_TYPE, bool bForce = false)
	{
		if (bForce || s_fLastUpdateTime + 600f < Time.time)
		{
			s_fLastUpdateTime = Time.time;
			NKCPacketSender.Send_NKMPacket_WECHAT_COUPON_CHECK_REQ(m_tabTemplet.Key, eNKC_OPEN_WAIT_BOX_TYPE);
		}
	}

	private void Update()
	{
		if (CheckComplete())
		{
			CheckCoupon(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}
	}

	public override void Refresh()
	{
		UpdateUI();
		if (!s_bSendPacketAfterRefresh)
		{
			s_bSendPacketAfterRefresh = true;
		}
		else if (CheckComplete())
		{
			CheckCoupon(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL, bForce: true);
		}
	}

	private void OnClickCopy()
	{
		if (!(m_lbCode == null))
		{
			TextEditor textEditor = new TextEditor();
			textEditor.text = m_lbCode.text;
			textEditor.OnFocus();
			textEditor.Copy();
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_COPY_COMPLETE);
		}
	}

	private void UpdateUI()
	{
		if (m_tabTemplet == null)
		{
			return;
		}
		NKMEventWechatCouponTemplet nKMEventWechatCouponTemplet = NKMEventWechatCouponTemplet.Find(m_tabTemplet.m_EventID);
		if (nKMEventWechatCouponTemplet == null)
		{
			Debug.LogError("WechatCouponTemplet eventID not found, ID = " + m_tabTemplet.m_EventID);
			return;
		}
		for (int i = 0; i < m_lstRewardSlot.Count; i++)
		{
			NKCUISlot nKCUISlot = m_lstRewardSlot[i];
			if (nKCUISlot == null)
			{
				continue;
			}
			if (i >= nKMEventWechatCouponTemplet.RewardList.Count)
			{
				nKCUISlot.SetActive(bSet: false);
				continue;
			}
			nKCUISlot.SetActive(bSet: true);
			NKMRewardInfo nKMRewardInfo = nKMEventWechatCouponTemplet.RewardList[i];
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(nKMRewardInfo.rewardType, nKMRewardInfo.ID, nKMRewardInfo.Count);
			nKCUISlot.SetData(data);
			if (s_WechatCouponData != null && s_WechatCouponData.state == WechatCouponState.Completed)
			{
				nKCUISlot.SetCompleteMark(bValue: true);
			}
			else
			{
				nKCUISlot.SetCompleteMark(bValue: false);
			}
		}
		NKCUtil.SetLabelText(m_lbCode, NKCPublisherModule.Marketing.MakeWechatFollowCode(nKMEventWechatCouponTemplet.ZlongActivityInstanceId));
		if (s_WechatCouponData == null)
		{
			NKCUtil.SetGameobjectActive(m_csbtnGet.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_objComplete, bValue: false);
		}
		else if (s_WechatCouponData.state == WechatCouponState.Completed)
		{
			NKCUtil.SetGameobjectActive(m_csbtnGet.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_objComplete, bValue: true);
		}
		else if (s_WechatCouponData.state == WechatCouponState.Registered)
		{
			NKCUtil.SetGameobjectActive(m_csbtnGet.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_objComplete, bValue: false);
			m_csbtnGet.SetLock(value: false);
		}
		else if (s_WechatCouponData.state == WechatCouponState.Initialized)
		{
			NKCUtil.SetGameobjectActive(m_csbtnGet.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_objComplete, bValue: false);
			m_csbtnGet.SetLock(value: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_csbtnGet.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_objComplete, bValue: false);
		}
	}
}
