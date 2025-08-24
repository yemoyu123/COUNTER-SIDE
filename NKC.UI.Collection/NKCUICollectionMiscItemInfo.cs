using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;

namespace NKC.UI.Collection;

public class NKCUICollectionMiscItemInfo : MonoBehaviour
{
	public delegate void OnClose();

	public TMP_Text m_name;

	public TMP_Text m_desc;

	public NKCUIComStateButton m_closeX;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_rewardRoot;

	public GameObject m_completedReward;

	public NKCUISlot m_rewardIconSlot;

	private int m_miscItemId;

	private OnClose m_dOnClose;

	public void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_closeX, Close);
		m_rewardIconSlot?.Init();
	}

	public void Open(int itemId, NKCUICollectionGeneral.CollectionType collectionType, OnClose dOnClose)
	{
		m_miscItemId = itemId;
		m_dOnClose = dOnClose;
		base.gameObject.SetActive(value: true);
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemId);
		NKCUtil.SetLabelText(m_name, itemMiscTempletByID.GetItemName());
		NKCUtil.SetLabelText(m_desc, itemMiscTempletByID.GetItemDesc());
		bool num = UpdateRewardCompleted();
		NKMCollectionV2MiscTemplet nKMCollectionV2MiscTemplet = NKMCollectionV2MiscTemplet.Find(itemId);
		bool flag = (NKCScenManager.CurrentUserData()?.m_InventoryData).GetCountMiscItem(itemId) > 0;
		if (nKMCollectionV2MiscTemplet != null)
		{
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(nKMCollectionV2MiscTemplet.RewardType, nKMCollectionV2MiscTemplet.RewardId, nKMCollectionV2MiscTemplet.RewardValue);
			m_rewardIconSlot?.SetData(data);
		}
		if (!num && flag && !nKMCollectionV2MiscTemplet.DefaultCollection)
		{
			NKCPacketSender.Send_NKMPacket_MISC_COLLECTION_REWARD_REQ(itemId);
		}
	}

	public void SetActive(bool value)
	{
		base.gameObject.SetActive(value);
	}

	public bool IsOpened()
	{
		return base.gameObject.activeSelf;
	}

	public bool UpdateRewardCompleted()
	{
		bool flag = false;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			flag = nKMUserData.m_InventoryData.GetMiscCollectionData(m_miscItemId)?.IsRewardComplete() ?? false;
		}
		NKCUtil.SetGameobjectActive(m_completedReward, flag);
		return flag;
	}

	public void Close()
	{
		base.gameObject.SetActive(value: false);
		m_miscItemId = 0;
		if (m_dOnClose != null)
		{
			m_dOnClose();
		}
	}
}
