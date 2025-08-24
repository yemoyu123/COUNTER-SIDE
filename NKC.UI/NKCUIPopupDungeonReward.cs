using System.Collections.Generic;
using ClientPacket.Office;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupDungeonReward : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "UI_EVENT_MD_POPUP_SINGLE_DEF_2";

	private const string UI_ASSET_NAME = "UI_EVENT_MD_POPUP_SINGLE_DEF_2_REWARD_POPUP";

	private static NKCUIPopupDungeonReward m_Instance;

	public Text m_lbTitle;

	public Text m_lbDesc;

	public TMP_Text m_lbRank;

	public TMP_Text m_lbScore;

	public NKCUISlotProfile m_profile;

	public List<NKCUISlot> m_lstRewardSlot = new List<NKCUISlot>();

	public NKCUIComStateButton m_btnOK;

	public static NKCUIPopupDungeonReward Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupDungeonReward>("UI_EVENT_MD_POPUP_SINGLE_DEF_2", "UI_EVENT_MD_POPUP_SINGLE_DEF_2_REWARD_POPUP", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPopupDungeonReward>();
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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private void Init()
	{
		m_profile.Init();
		for (int i = 0; i < m_lstRewardSlot.Count; i++)
		{
			if (m_lstRewardSlot[i] != null)
			{
				m_lstRewardSlot[i].Init();
			}
		}
		m_btnOK.PointerClick.RemoveAllListeners();
		m_btnOK.PointerClick.AddListener(OnClickOK);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(NKMRewardData rewardData)
	{
		m_profile.SetProfiledata(NKCScenManager.CurrentUserData().UserProfileData, null);
		NKCUtil.SetLabelText(m_lbRank, NKCDefenceDungeonManager.GetRankingTotalDesc());
		NKCUtil.SetLabelText(m_lbScore, NKCDefenceDungeonManager.m_BestClearScore.ToString());
		List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
		if (rewardData != null)
		{
			foreach (NKMEquipItemData equipItemData in rewardData.EquipItemDataList)
			{
				if (equipItemData != null)
				{
					NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeEquipData(equipItemData.m_ItemEquipID, equipItemData.m_EnchantLevel);
					list.Add(item);
				}
			}
			foreach (NKMItemMiscData miscItemData in rewardData.MiscItemDataList)
			{
				if (miscItemData != null)
				{
					NKCUISlot.SlotData item2 = NKCUISlot.SlotData.MakeMiscItemData(miscItemData.ItemID, miscItemData.TotalCount);
					list.Add(item2);
				}
			}
			foreach (NKMMoldItemData moldItemData in rewardData.MoldItemDataList)
			{
				if (moldItemData != null)
				{
					NKCUISlot.SlotData item3 = NKCUISlot.SlotData.MakeMoldItemData(moldItemData.m_MoldID, moldItemData.m_Count);
					list.Add(item3);
				}
			}
			foreach (NKMInteriorData interior in rewardData.Interiors)
			{
				if (interior != null)
				{
					NKCUISlot.SlotData item4 = NKCUISlot.SlotData.MakeMiscItemData(interior.itemId, interior.count);
					list.Add(item4);
				}
			}
			foreach (NKMUnitData unitData in rewardData.UnitDataList)
			{
				if (unitData != null)
				{
					NKCUISlot.SlotData item5 = NKCUISlot.SlotData.MakeUnitData(unitData);
					list.Add(item5);
				}
			}
			for (int i = 0; i < m_lstRewardSlot.Count; i++)
			{
				if (!(m_lstRewardSlot[i] == null))
				{
					if (list.Count <= i || list[i] == null)
					{
						NKCUtil.SetGameobjectActive(m_lstRewardSlot[i], bValue: false);
						continue;
					}
					NKCUtil.SetGameobjectActive(m_lstRewardSlot[i], bValue: true);
					m_lstRewardSlot[i].SetData(list[i]);
				}
			}
			UIOpened();
		}
		else
		{
			CloseInternal();
		}
	}

	private void OnClickOK()
	{
		Close();
	}
}
