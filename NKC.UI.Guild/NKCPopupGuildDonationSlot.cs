using System.Collections.Generic;
using Cs.Logging;
using NKM.Guild;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildDonationSlot : MonoBehaviour
{
	public delegate void OnSlot(int slot);

	public Text m_lbTitle;

	public Image m_imgIcon;

	public List<NKCUISlot> m_lstRewardSlot = new List<NKCUISlot>();

	public Image m_imgUseResourceIcon;

	public Text m_lbUseResourceCount;

	public NKCUIComStateButton m_btnSlot;

	public NKCUIComStateButton m_btnResource;

	private OnSlot m_dOnSlot;

	private GuildDonationTemplet m_cGuildDonationTemplet;

	public void InitUI(OnSlot onSlot)
	{
		m_dOnSlot = onSlot;
		m_btnSlot.PointerClick.RemoveAllListeners();
		m_btnSlot.PointerClick.AddListener(OnClickSlot);
		m_btnResource.PointerClick.RemoveAllListeners();
		m_btnResource.PointerClick.AddListener(OnClickSlot);
		for (int i = 0; i < m_lstRewardSlot.Count; i++)
		{
			m_lstRewardSlot[i].Init();
		}
	}

	public void SetData(GuildDonationTemplet templet)
	{
		if (templet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_cGuildDonationTemplet = templet;
		NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString(templet.DonateText));
		NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_Texture", templet.DonateImgFileName));
		for (int i = 0; i < m_lstRewardSlot.Count; i++)
		{
			if (i >= templet.m_DonationReward.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstRewardSlot[i], bValue: false);
			}
			else if (templet.m_DonationReward[i].RewardType == NKM_REWARD_TYPE.RT_MISC)
			{
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(templet.m_DonationReward[i].RewardID, templet.m_DonationReward[i].RewardValue);
				m_lstRewardSlot[i].SetData(data, bEnableLayoutElement: false);
			}
			else
			{
				Log.Error("기부 보상이 MISC 타입이 아님 - 해당 타입 작업 필요함", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/NKCPopupGuildDonationSlot.cs", 77);
			}
		}
		NKCUtil.SetImageSprite(m_imgUseResourceIcon, NKCResourceUtility.GetOrLoadMiscItemSmallIcon(templet.reqItemUnit.ItemId));
		NKCUtil.SetLabelText(m_lbUseResourceCount, templet.reqItemUnit.Count.ToString("N0"));
		CheckState(templet);
	}

	public void CheckState(GuildDonationTemplet templet)
	{
		if (NKCGuildManager.GetRemainDonationCount() <= 0)
		{
			m_btnSlot.Lock();
			m_btnResource.Lock();
			NKCUtil.SetLabelTextColor(m_lbUseResourceCount, NKCUtil.GetColor("#212122"));
			return;
		}
		if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(templet.reqItemUnit.ItemId) < templet.reqItemUnit.Count)
		{
			NKCUtil.SetLabelTextColor(m_lbUseResourceCount, Color.red);
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_lbUseResourceCount, NKCUtil.GetColor("#582817"));
		}
		m_btnSlot.UnLock();
		m_btnResource.UnLock();
	}

	private void OnClickSlot()
	{
		if (!m_btnSlot.m_bLock)
		{
			if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_cGuildDonationTemplet.reqItemUnit.ItemId) < m_cGuildDonationTemplet.reqItemUnit.Count)
			{
				NKCShopManager.OpenItemLackPopup(m_cGuildDonationTemplet.reqItemUnit.ItemId, m_cGuildDonationTemplet.reqItemUnit.Count32);
			}
			else
			{
				m_dOnSlot?.Invoke(m_cGuildDonationTemplet.ID);
			}
		}
	}
}
