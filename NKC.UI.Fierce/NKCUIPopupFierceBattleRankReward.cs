using System.Collections.Generic;
using ClientPacket.Office;
using NKM;
using NKM.Templet;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Fierce;

public class NKCUIPopupFierceBattleRankReward : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_fierce_battle";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FIERCE_BATTLE_FINAL_REWARD";

	private static NKCUIPopupFierceBattleRankReward m_Instance;

	public Text m_POPUP_FIERCE_BATTLE_FINAL_RANK_REWARD_RANK1;

	public Text m_POPUP_FIERCE_BATTLE_FINAL_RANK_REWARD_RANK2;

	public List<NKCUISlot> m_RewardSlots;

	public NKCUIComStateButton m_POPUP_FIERCE_BATTLE_FINAL_RANK_REWARD_BUTTON;

	public EventTrigger m_Bg;

	public static NKCUIPopupFierceBattleRankReward Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupFierceBattleRankReward>("ab_ui_nkm_ui_fierce_battle", "NKM_UI_POPUP_FIERCE_BATTLE_FINAL_REWARD", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPopupFierceBattleRankReward>();
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

	public override string MenuName => "NKM_UI_POPUP_FIERCE_BATTLE_FINAL_REWARD";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Init()
	{
		NKCUtil.SetBindFunction(m_POPUP_FIERCE_BATTLE_FINAL_RANK_REWARD_BUTTON, CheckInstanceAndClose);
		NKCUtil.SetHotkey(m_POPUP_FIERCE_BATTLE_FINAL_RANK_REWARD_BUTTON, HotkeyEventType.Confirm);
		if (m_Bg != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				CheckInstanceAndClose();
			});
			m_Bg.triggers.Add(entry);
		}
	}

	public void Open(NKMRewardData reward)
	{
		NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		if (nKCFierceBattleSupportDataMgr != null)
		{
			NKCUtil.SetLabelText(m_POPUP_FIERCE_BATTLE_FINAL_RANK_REWARD_RANK1, nKCFierceBattleSupportDataMgr.GetRankingTotalDesc());
			NKCUtil.SetLabelText(m_POPUP_FIERCE_BATTLE_FINAL_RANK_REWARD_RANK2, nKCFierceBattleSupportDataMgr.GetTotalPoint().ToString());
		}
		List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
		if (reward == null)
		{
			return;
		}
		foreach (NKMEquipItemData equipItemData in reward.EquipItemDataList)
		{
			if (equipItemData != null)
			{
				NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeEquipData(equipItemData.m_ItemEquipID, equipItemData.m_EnchantLevel);
				list.Add(item);
			}
		}
		foreach (NKMItemMiscData miscItemData in reward.MiscItemDataList)
		{
			if (miscItemData != null)
			{
				NKCUISlot.SlotData item2 = NKCUISlot.SlotData.MakeMiscItemData(miscItemData.ItemID, miscItemData.TotalCount);
				list.Add(item2);
			}
		}
		foreach (NKMMoldItemData moldItemData in reward.MoldItemDataList)
		{
			if (moldItemData != null)
			{
				NKCUISlot.SlotData item3 = NKCUISlot.SlotData.MakeMoldItemData(moldItemData.m_MoldID, moldItemData.m_Count);
				list.Add(item3);
			}
		}
		foreach (NKMInteriorData interior in reward.Interiors)
		{
			if (interior != null)
			{
				NKCUISlot.SlotData item4 = NKCUISlot.SlotData.MakeMiscItemData(interior.itemId, interior.count);
				list.Add(item4);
			}
		}
		for (int i = 0; i < m_RewardSlots.Count; i++)
		{
			if (!(m_RewardSlots[i] == null))
			{
				if (list.Count <= i || list[i] == null)
				{
					NKCUtil.SetGameobjectActive(m_RewardSlots[i], bValue: false);
					continue;
				}
				NKCUtil.SetGameobjectActive(m_RewardSlots[i], bValue: true);
				m_RewardSlots[i].SetData(list[i]);
			}
		}
		UIOpened();
	}
}
