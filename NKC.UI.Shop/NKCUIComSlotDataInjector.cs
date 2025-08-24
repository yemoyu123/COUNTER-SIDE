using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIComSlotDataInjector : MonoBehaviour
{
	[Header("UI")]
	public NKCUISlot m_itemSlot;

	public Text m_lbName;

	public Text m_lbDesc;

	public Text m_lbCount;

	[Header("UI 컨트롤")]
	public bool m_bClickAction = true;

	public string CountFormat = "{0:n0}";

	[Header("Data")]
	public NKM_REWARD_TYPE m_rewardType;

	public int m_rewardID;

	public int m_count;

	private bool m_bSetDataComplete;

	private void Start()
	{
		if (!m_bSetDataComplete)
		{
			SetData();
		}
	}

	public void SetData()
	{
		SetData(m_rewardType, m_rewardID, m_count, 0);
	}

	public void SetData(NKMRewardInfo rewardInfo)
	{
		SetData(rewardInfo.rewardType, rewardInfo.ID, rewardInfo.Count, 0);
	}

	public void SetData(NKMRandomBoxItemTemplet boxItemTemplet)
	{
		SetData(boxItemTemplet.m_reward_type, boxItemTemplet.m_RewardID, boxItemTemplet.FreeQuantity_Max, boxItemTemplet.PaidQuantity_Max);
	}

	public void SetData(NKM_REWARD_TYPE rewardType, int rewardID, int freeCount, int paidCount)
	{
		int num = freeCount + paidCount;
		if (m_itemSlot != null)
		{
			m_itemSlot.Init();
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(rewardType, rewardID, num);
			m_itemSlot.SetData(slotData);
			if (m_bClickAction)
			{
				m_itemSlot.SetOnClickAction(NKCUISlot.SlotClickType.RatioList, NKCUISlot.SlotClickType.ChoiceList, NKCUISlot.SlotClickType.BoxList, NKCUISlot.SlotClickType.MoldList, NKCUISlot.SlotClickType.ItemBox);
			}
			NKCShopManager.ShowShopItemCashCount(m_itemSlot, slotData, freeCount, paidCount);
		}
		string text = null;
		string text2 = null;
		string countString = GetCountString(freeCount, paidCount);
		switch (rewardType)
		{
		case NKM_REWARD_TYPE.RT_MISC:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(rewardID);
			if (itemMiscTempletByID != null)
			{
				text = itemMiscTempletByID.GetItemName();
				text2 = itemMiscTempletByID.GetItemDesc();
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_EMOTICON:
		{
			NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(rewardID);
			if (nKMEmoticonTemplet != null)
			{
				text = nKMEmoticonTemplet.GetEmoticonName();
				text2 = nKMEmoticonTemplet.GetEmoticonDesc();
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_EQUIP:
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(rewardID);
			if (equipTemplet != null)
			{
				text = equipTemplet.GetItemName();
				text2 = equipTemplet.GetItemDesc();
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_SHIP:
		case NKM_REWARD_TYPE.RT_OPERATOR:
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(rewardID);
			if (nKMUnitTempletBase != null)
			{
				text = nKMUnitTempletBase.GetUnitName();
				text2 = nKMUnitTempletBase.GetUnitDesc();
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_SKIN:
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(rewardID);
			if (skinTemplet != null)
			{
				text = skinTemplet.GetTitle();
				text2 = skinTemplet.GetSkinDesc();
			}
			break;
		}
		default:
			NKCUtil.SetGameobjectActive(m_lbName, bValue: false);
			NKCUtil.SetGameobjectActive(m_lbDesc, bValue: false);
			NKCUtil.SetGameobjectActive(m_lbCount, bValue: true);
			if (!NKCShopManager.ShowShopItemCashCount(m_lbCount, freeCount, paidCount))
			{
				if (string.IsNullOrEmpty(CountFormat))
				{
					NKCUtil.SetLabelText(m_lbCount, num.ToString());
				}
				else
				{
					NKCUtil.SetLabelText(m_lbCount, string.Format(CountFormat, num));
				}
			}
			break;
		}
		NKCUtil.SetGameobjectActive(m_lbName, text != null);
		NKCUtil.SetGameobjectActive(m_lbDesc, text2 != null);
		NKCUtil.SetGameobjectActive(m_lbCount, countString != null);
		if (text != null)
		{
			NKCUtil.SetLabelText(m_lbName, text);
		}
		if (text2 != null)
		{
			NKCUtil.SetLabelText(m_lbDesc, text2);
		}
		if (countString != null)
		{
			NKCUtil.SetLabelText(m_lbCount, countString);
		}
		m_bSetDataComplete = true;
	}

	private string GetCountString(int freeCount, int paidCount)
	{
		int num = freeCount + paidCount;
		if (NKCShopManager.UseSuperuserItemCount())
		{
			return NKCShopManager.GetItemCountString(freeCount, paidCount);
		}
		if (string.IsNullOrEmpty(CountFormat))
		{
			return num.ToString();
		}
		return string.Format(CountFormat, num);
	}
}
