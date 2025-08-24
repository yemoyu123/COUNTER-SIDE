using System;
using System.Collections.Generic;
using NKC.Templet;
using NKC.UI.NPC;
using NKC.UI.Shop;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCPopupEventPayReward : NKCUIBase
{
	private enum TabType
	{
		Normal,
		Extra
	}

	public NKCUINPCSpineIllust m_npcSpineIllust;

	public NKCUIComStateButton m_csbtnClose;

	public Text m_lbEventInterval;

	[Header("\ufffd\ufffd")]
	public NKCUIComToggleGroup m_tglgrpTab;

	public NKCUIComToggle m_tglTabNormal;

	public NKCUIComToggle m_tglTabExtra;

	[Header("\ufffdϹ\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objTabNormal;

	public Text m_lbConsumeAmount;

	public LoopScrollRect m_loopScrollRect;

	public Image m_payIcon;

	public GameObject m_objFinalMissionDeco;

	[Header("\ufffd\ufffd\ufffd\ufffdƮ\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objTabExtra;

	public Text m_lbExtraConsumeAmount;

	public Image m_imgExtraReward;

	public NKCUIComStateButton m_csbtnExtraDetail;

	public NKCUIComStateButton m_csbtnExtraReward;

	[Header("ĳ\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objUnitInfoRoot;

	public NKCUIComStateButton m_csbtnUnitInfo;

	public Image m_imgUnitIcon;

	public Text m_lbUnitName;

	public Text m_lbUnitTitle;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public NKCPopupEventPayRewardSlot m_finalRewardSlot;

	private TabType m_eCurrentTab;

	private NKCEventPaybackTemplet m_paybackTemplet;

	private List<NKMMissionTemplet> m_missionTempletList;

	private int m_missionTabId;

	private long m_consumeAmount;

	private bool m_scrollRectInit;

	public override string MenuName => "Payback Mission";

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override eMenutype eUIType => eMenutype.FullScreen;

	private List<NKMMissionTemplet> MissionTempletList
	{
		get
		{
			if (m_missionTempletList == null)
			{
				m_missionTempletList = GetMissionTempletList(m_missionTabId);
			}
			return m_missionTempletList;
		}
	}

	public static NKCPopupEventPayReward OpenInstance(string bundleName, string assetName)
	{
		NKCPopupEventPayReward instance = NKCUIManager.OpenNewInstance<NKCPopupEventPayReward>(bundleName, assetName, NKCUIManager.eUIBaseRect.UIFrontPopup, null).GetInstance<NKCPopupEventPayReward>();
		if (instance != null)
		{
			instance.InitUI();
		}
		return instance;
	}

	public static NKCPopupEventPayReward OpenInstance(string bundleName, string assetName, int eventId, int missionTabId)
	{
		NKCPopupEventPayReward nKCPopupEventPayReward = OpenInstance(bundleName, assetName);
		if (nKCPopupEventPayReward == null)
		{
			return null;
		}
		nKCPopupEventPayReward.Open(eventId, missionTabId);
		return nKCPopupEventPayReward;
	}

	private void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnUnitInfo, OnClickUnitInfo);
		NKCUtil.SetToggleValueChangedDelegate(m_tglTabNormal, OnTglNormal);
		NKCUtil.SetToggleValueChangedDelegate(m_tglTabExtra, OnTglExtra);
		NKCUtil.SetButtonClickDelegate(m_csbtnExtraDetail, OnBtnExtraDetail);
		NKCUtil.SetButtonClickDelegate(m_csbtnExtraReward, OnBtnExtraReward);
		m_finalRewardSlot?.Init();
		m_scrollRectInit = false;
	}

	public override void CloseInternal()
	{
		if (m_npcSpineIllust != null)
		{
			m_npcSpineIllust.m_dOnTouch = null;
		}
		m_paybackTemplet = null;
		m_missionTempletList?.Clear();
		m_missionTempletList = null;
		NKCUIVoiceManager.StopVoice();
		base.gameObject.SetActive(value: false);
	}

	public void Open(int eventId, int missionTabId)
	{
		m_missionTabId = missionTabId;
		m_paybackTemplet = NKCEventPaybackTemplet.Find(eventId);
		if (m_paybackTemplet != null)
		{
			base.gameObject.SetActive(value: true);
			if (!m_scrollRectInit && m_loopScrollRect != null)
			{
				m_loopScrollRect.dOnGetObject += GetSlot;
				m_loopScrollRect.dOnReturnObject += ReturnSlot;
				m_loopScrollRect.dOnProvideData += ProvideData;
				m_loopScrollRect.ContentConstraintCount = 1;
				m_loopScrollRect.PrepareCells();
				m_loopScrollRect.TotalCount = 0;
				m_loopScrollRect.RefreshCells();
				m_scrollRectInit = true;
			}
			if (m_npcSpineIllust != null)
			{
				m_npcSpineIllust.m_dOnTouch = OnTouchSpineIllust;
			}
			if (string.IsNullOrEmpty(m_paybackTemplet.UnitStrId))
			{
				NKCUtil.SetGameobjectActive(m_objUnitInfoRoot, bValue: false);
			}
			else
			{
				SetUnitInfo(m_paybackTemplet);
			}
			NKMMissionTemplet extraMissionTemplet = GetExtraMissionTemplet();
			NKCUtil.SetGameobjectActive(m_tglgrpTab, extraMissionTemplet != null);
			TabType targetTab = FindDefaultTab();
			SelectTab(targetTab);
			UIOpened();
		}
	}

	public void Refresh()
	{
		switch (m_eCurrentTab)
		{
		case TabType.Normal:
			SetNormalTabData();
			break;
		case TabType.Extra:
			SetExtraTabData();
			break;
		}
		SetTimeLeft(m_paybackTemplet);
	}

	private TabType FindDefaultTab()
	{
		NKMMissionTemplet extraMissionTemplet = GetExtraMissionTemplet();
		if (extraMissionTemplet == null)
		{
			return TabType.Normal;
		}
		if (NKMMissionManager.GetMissionStateData(extraMissionTemplet).state == NKMMissionManager.MissionState.LOCKED)
		{
			return TabType.Normal;
		}
		foreach (NKMMissionTemplet missionTemplet in MissionTempletList)
		{
			if (!NKMMissionManager.GetMissionStateData(missionTemplet).IsMissionCompleted)
			{
				return TabType.Normal;
			}
		}
		return TabType.Extra;
	}

	private void SetNormalTabData()
	{
		int indexPosition = 0;
		int count = MissionTempletList.Count;
		for (int i = 0; i < count; i++)
		{
			NKMMissionManager.MissionState state = NKMMissionManager.GetMissionStateData(MissionTempletList[i]).state;
			if (state == NKMMissionManager.MissionState.REPEAT_COMPLETED || state == NKMMissionManager.MissionState.COMPLETED)
			{
				indexPosition = i;
			}
		}
		SetConsumeAmount(MissionTempletList);
		if (m_loopScrollRect != null)
		{
			m_loopScrollRect.TotalCount = MissionTempletList.Count;
			m_loopScrollRect.SetIndexPosition(indexPosition);
		}
		if (MissionTempletList.Count > 0)
		{
			SetFinalMissionInfo(MissionTempletList[MissionTempletList.Count - 1]);
			SetPayIcon(MissionTempletList[0]);
		}
		else
		{
			SetFinalMissionInfo(null);
			SetPayIcon(null);
		}
	}

	public override void UnHide()
	{
		base.UnHide();
		Refresh();
	}

	private void SetUnitInfo(NKCEventPaybackTemplet paybackTemplet)
	{
		NKCUtil.SetGameobjectActive(m_objUnitInfoRoot, bValue: true);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(paybackTemplet.UnitStrId);
		if (unitTempletBase != null)
		{
			NKCUtil.SetLabelText(m_lbUnitName, unitTempletBase.GetUnitName());
			NKCUtil.SetLabelText(m_lbUnitTitle, unitTempletBase.GetUnitTitle());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbUnitName, "");
			NKCUtil.SetLabelText(m_lbUnitTitle, "");
		}
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(paybackTemplet.SkinId);
		if (skinTemplet != null)
		{
			NKCUtil.SetImageSprite(m_imgUnitIcon, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, skinTemplet.m_SkinEquipUnitID, 0));
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgUnitIcon, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase));
		}
	}

	private List<NKMMissionTemplet> GetMissionTempletList(int missionTabId)
	{
		List<NKMMissionTemplet> list = NKMMissionManager.GetMissionTempletListByType(missionTabId);
		if (list == null)
		{
			list = new List<NKMMissionTemplet>();
		}
		list.Sort(delegate(NKMMissionTemplet e1, NKMMissionTemplet e2)
		{
			if (e1.m_MissionID < e2.m_MissionID)
			{
				return -1;
			}
			return (e1.m_MissionID > e2.m_MissionID) ? 1 : 0;
		});
		return list;
	}

	private void SetConsumeAmount(List<NKMMissionTemplet> missionTempletList)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		m_consumeAmount = 0L;
		if (nKMUserData != null && missionTempletList != null)
		{
			int count = missionTempletList.Count;
			for (int i = 0; i < count; i++)
			{
				if (missionTempletList[i] == null)
				{
					continue;
				}
				NKMMissionData missionDataByGroupId = nKMUserData.m_MissionData.GetMissionDataByGroupId(missionTempletList[i].m_GroupId);
				if (missionDataByGroupId != null)
				{
					long num = Math.Min(missionTempletList[i].m_Times, missionDataByGroupId.times);
					if (m_consumeAmount < num)
					{
						m_consumeAmount = num;
					}
				}
			}
		}
		NKCUtil.SetLabelText(m_lbConsumeAmount, $"{m_consumeAmount:#,0}");
	}

	private void SetFinalMissionInfo(NKMMissionTemplet finalMissionTemplet)
	{
		if (finalMissionTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_finalRewardSlot, bValue: true);
			if (finalMissionTemplet.m_MissionReward.Count > 0 && finalMissionTemplet.m_MissionReward[0].reward_type == NKM_REWARD_TYPE.RT_SKIN)
			{
				m_finalRewardSlot?.SetData(finalMissionTemplet, 0f, SetFinalMissionDeco, OnMissionComplete, OnClickRewardIcon);
			}
			else
			{
				m_finalRewardSlot?.SetData(finalMissionTemplet, 0f, SetFinalMissionDeco, OnMissionComplete, null);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_finalRewardSlot, bValue: false);
		}
	}

	private void SetPayIcon(NKMMissionTemplet missionTemplet)
	{
		if (missionTemplet != null && missionTemplet.m_MissionCond.value1 != null && missionTemplet.m_MissionCond.value1.Count > 0)
		{
			Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(missionTemplet.m_MissionCond.value1[0]);
			NKCUtil.SetImageSprite(m_payIcon, orLoadMiscItemSmallIcon);
		}
	}

	private void SetTimeLeft(NKCEventPaybackTemplet paybackTemplet)
	{
		NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(paybackTemplet.IntervalTag);
		if (nKMIntervalTemplet == null)
		{
			NKCUtil.SetLabelText(m_lbEventInterval, "");
			return;
		}
		string remainTimeStringOneParam = NKCUtilString.GetRemainTimeStringOneParam(NKCSynchronizedTime.ToUtcTime(nKMIntervalTemplet.EndDate));
		NKCUtil.SetLabelText(m_lbEventInterval, remainTimeStringOneParam);
	}

	private void SetFinalMissionDeco(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objFinalMissionDeco, !value);
	}

	private void OnTouchSpineIllust()
	{
		if (!string.IsNullOrEmpty(m_paybackTemplet.UnitStrId) && m_paybackTemplet.SkinId > 0)
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_TOUCH, m_paybackTemplet.UnitStrId, m_paybackTemplet.SkinId, bIgnoreShowNormalAfterLifeTimeOption: false, bShowCaption: true);
		}
	}

	private void OnClickUnitInfo()
	{
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(m_paybackTemplet.SkinId);
		NKCUIShopSkinPopup.Instance.OpenForSkinInfo(skinTemplet, 0);
	}

	private void OnClickRewardIcon(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (slotData != null)
		{
			switch (slotData.eType)
			{
			case NKCUISlot.eSlotMode.Skin:
			{
				NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(slotData.ID);
				NKCUIShopSkinPopup.Instance.OpenForSkinInfo(skinTemplet, 0);
				break;
			}
			case NKCUISlot.eSlotMode.UnitCount:
				NKCPopupItemBox.Instance.OpenUnitBox(slotData.ID);
				break;
			case NKCUISlot.eSlotMode.ItemMisc:
				NKCPopupItemBox.Instance.OpenItemBox(slotData.ID);
				break;
			}
		}
	}

	private void OnMissionComplete()
	{
		NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_ALL_REQ(m_missionTabId);
	}

	private RectTransform GetSlot(int index)
	{
		return NKCPopupEventPayRewardSlot.GetNewInstance(null, m_paybackTemplet.BannerPrefabId, m_paybackTemplet.BannerPrefabId + "_SLOT")?.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform tr)
	{
		NKCPopupEventPayRewardSlot component = tr.GetComponent<NKCPopupEventPayRewardSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	private void ProvideData(Transform tr, int index)
	{
		NKCPopupEventPayRewardSlot component = tr.GetComponent<NKCPopupEventPayRewardSlot>();
		if (component == null)
		{
			return;
		}
		if (MissionTempletList.Count <= 0 || MissionTempletList.Count <= index)
		{
			component.SetData(null, 0f, null, null, null);
			return;
		}
		long num = 0L;
		if (index > 0)
		{
			num = MissionTempletList[index - 1].m_Times;
		}
		float a = (float)(m_consumeAmount - num) / (float)(MissionTempletList[index].m_Times - num);
		a = Mathf.Max(a, 0f);
		if (index == MissionTempletList.Count - 1 && MissionTempletList[index].m_MissionReward.Count > 0 && MissionTempletList[index].m_MissionReward[0].reward_type == NKM_REWARD_TYPE.RT_SKIN)
		{
			component.SetData(MissionTempletList[index], a, null, OnMissionComplete, OnClickRewardIcon);
		}
		else
		{
			component.SetData(MissionTempletList[index], a, null, OnMissionComplete, null);
		}
	}

	private void SelectTab(TabType targetTab)
	{
		m_eCurrentTab = targetTab;
		if (GetExtraMissionTemplet() == null)
		{
			m_eCurrentTab = TabType.Normal;
		}
		NKCUtil.SetGameobjectActive(m_objTabNormal, m_eCurrentTab == TabType.Normal);
		NKCUtil.SetGameobjectActive(m_objTabExtra, m_eCurrentTab == TabType.Extra);
		switch (m_eCurrentTab)
		{
		case TabType.Normal:
			if (m_tglTabNormal != null)
			{
				m_tglTabNormal.Select(bSelect: true, bForce: true);
			}
			break;
		case TabType.Extra:
			if (m_tglTabExtra != null)
			{
				m_tglTabExtra.Select(bSelect: true, bForce: true);
			}
			break;
		}
		Refresh();
	}

	private void OnTglNormal(bool value)
	{
		if (value)
		{
			SelectTab(TabType.Normal);
		}
	}

	private void OnTglExtra(bool value)
	{
		if (value)
		{
			SelectTab(TabType.Extra);
		}
	}

	private void SetExtraTabData()
	{
		if (m_paybackTemplet == null)
		{
			return;
		}
		NKMMissionTemplet extraMissionTemplet = GetExtraMissionTemplet();
		if (extraMissionTemplet != null && extraMissionTemplet.m_MissionReward.Count != 0 && extraMissionTemplet.m_MissionReward != null)
		{
			SetExtraConsumeAmount();
			Sprite rewardInvenIcon = NKCResourceUtility.GetRewardInvenIcon(extraMissionTemplet.m_MissionReward[0].reward_type, extraMissionTemplet.m_MissionReward[0].reward_id);
			NKCUtil.SetImageSprite(m_imgExtraReward, rewardInvenIcon, bDisableIfSpriteNull: true);
			if (m_csbtnExtraReward != null)
			{
				NKMMissionManager.MissionStateData missionStateData = NKMMissionManager.GetMissionStateData(extraMissionTemplet);
				m_csbtnExtraReward.SetLock(!missionStateData.IsMissionCanClear);
			}
		}
	}

	private void SetExtraConsumeAmount()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		long num = 0L;
		if (nKMUserData != null)
		{
			NKMMissionTemplet extraMissionTemplet = GetExtraMissionTemplet();
			if (extraMissionTemplet == null)
			{
				return;
			}
			NKMMissionData missionData = nKMUserData.m_MissionData.GetMissionData(extraMissionTemplet);
			if (missionData == null)
			{
				NKMMissionManager.GetMissionStateData(extraMissionTemplet);
				NKCUtil.SetLabelText(m_lbExtraConsumeAmount, "-");
				return;
			}
			num = missionData.times - extraMissionTemplet.m_MinRewardTimes;
		}
		if (num < 0)
		{
			NKCUtil.SetLabelText(m_lbExtraConsumeAmount, "-");
		}
		else
		{
			NKCUtil.SetLabelText(m_lbExtraConsumeAmount, $"{num:#,0}");
		}
	}

	private void OnBtnExtraDetail()
	{
		MissionReward missionReward = GetExtraMissionTemplet().m_MissionReward[0];
		if (missionReward.reward_type == NKM_REWARD_TYPE.RT_MISC)
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(missionReward.reward_id);
			if (itemMiscTempletByID != null && itemMiscTempletByID.IsUsable() && itemMiscTempletByID.IsRatioOpened())
			{
				NKCUISlotListViewer newInstance = NKCUISlotListViewer.GetNewInstance();
				if (newInstance != null)
				{
					newInstance.OpenItemBoxRatio(missionReward.reward_id);
				}
				return;
			}
		}
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(missionReward.reward_type, missionReward.reward_id, missionReward.reward_value);
		NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, data);
	}

	private void OnBtnExtraReward()
	{
		NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(GetExtraMissionTemplet());
	}

	private NKMMissionTemplet GetExtraMissionTemplet()
	{
		return NKMMissionManager.GetMissionTemplet(m_paybackTemplet.ExtraMissionID);
	}
}
