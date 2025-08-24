using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupGuildDonationMultiply : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_POPUP_DONATION_MULTIPLY";

	private static NKCPopupGuildDonationMultiply m_Instance;

	public Text m_lbRemainCount;

	public NKCUIItemCostSlot m_slot;

	public NKCUIComStateButton m_btnPlus;

	public NKCUIComStateButton m_btnMinus;

	public NKCUIComStateButton m_btnMax;

	public Text m_lbDonationCount;

	public NKCUIComStateButton m_btnOk;

	public NKCUIComStateButton m_btnCancel;

	private GuildDonationTemplet m_donationTemplet;

	private int m_MultiplyCount = 1;

	private bool m_bWasHold;

	public static NKCPopupGuildDonationMultiply Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildDonationMultiply>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_POPUP_DONATION_MULTIPLY", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CleanupInstance).GetInstance<NKCPopupGuildDonationMultiply>();
				if (m_Instance != null)
				{
					m_Instance.InitUI();
				}
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

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void InitUI()
	{
		m_btnPlus.PointerClick.RemoveAllListeners();
		m_btnPlus.PointerClick.AddListener(OnClickPlus);
		m_btnPlus.dOnPointerHoldPress = OnClickPlus;
		m_btnPlus.SetHotkey(HotkeyEventType.Plus);
		m_btnMinus.PointerClick.RemoveAllListeners();
		m_btnMinus.PointerClick.AddListener(OnClickMinus);
		m_btnMinus.dOnPointerHoldPress = OnHoldMinus;
		m_btnMinus.SetHotkey(HotkeyEventType.Minus);
		m_btnMax.PointerClick.RemoveAllListeners();
		m_btnMax.PointerClick.AddListener(OnClickMax);
		m_btnOk.PointerClick.RemoveAllListeners();
		m_btnOk.PointerClick.AddListener(OnClickOk);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(base.Close);
	}

	public void Open(GuildDonationTemplet templet)
	{
		if (templet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_donationTemplet = templet;
		m_MultiplyCount = 1;
		NKCUtil.SetLabelText(m_lbRemainCount, string.Format(NKCUtilString.GET_STRING_COUNT_ONE_PARAM, NKCGuildManager.GetRemainDonationCount()));
		OnValueChanged();
		UIOpened();
	}

	private void OnClickPlus()
	{
		m_MultiplyCount++;
		OnValueChanged();
	}

	private void OnClickMinus()
	{
		m_MultiplyCount--;
		if (!m_bWasHold && m_MultiplyCount < 1)
		{
			m_MultiplyCount = GetMaxCount();
		}
		m_bWasHold = false;
		OnValueChanged();
	}

	private void OnHoldMinus()
	{
		m_bWasHold = true;
		m_MultiplyCount--;
		OnValueChanged();
	}

	private void OnValueChanged()
	{
		m_MultiplyCount = Mathf.Clamp(m_MultiplyCount, 1, GetMaxCount());
		if (m_slot != null)
		{
			m_slot.SetData(m_donationTemplet.reqItemUnit.ItemId, m_donationTemplet.reqItemUnit.Count32 * m_MultiplyCount, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_donationTemplet.reqItemUnit.ItemId));
		}
		NKCUtil.SetLabelText(m_lbDonationCount, m_MultiplyCount.ToString());
	}

	private int GetMaxCount()
	{
		return Mathf.Min((int)(NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_donationTemplet.reqItemUnit.ItemId) / m_donationTemplet.reqItemUnit.Count32), NKCGuildManager.GetRemainDonationCount());
	}

	private void OnClickMax()
	{
		m_MultiplyCount = GetMaxCount();
		OnValueChanged();
	}

	private void OnClickOk()
	{
		Close();
		NKCPacketSender.Send_NKMPacket_GUILD_DONATION_REQ(m_donationTemplet.ID, m_MultiplyCount);
	}
}
