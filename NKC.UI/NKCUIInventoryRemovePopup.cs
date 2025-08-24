using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIInventoryRemovePopup : MonoBehaviour
{
	public delegate void OnCloseSmartRemove(int maxEnchantLevel, HashSet<NKCEquipSortSystem.eFilterOption> hsOptions);

	[Header("\ufffd\ufffd\u07ba\ufffd")]
	public NKCUIComToggle m_tglSSR;

	public NKCUIComToggle m_tglSR;

	public NKCUIComToggle m_tglR;

	public NKCUIComToggle m_tglN;

	[Header("Ƽ\ufffd\ueeb0")]
	public NKCUIComToggle m_tgl_tier_7;

	public NKCUIComToggle m_tgl_tier_6;

	public NKCUIComToggle m_tgl_tier_5;

	public NKCUIComToggle m_tgl_tier_4_under;

	[Header("\ufffd\ufffdȭ\ufffd\ufffd")]
	public NKCUIComStateButton m_btnNotEnchanted;

	public NKCUIComStateButton m_btnEnchanted;

	public Text m_lbEnchantLevel;

	[Header("\ufffdϴ\ufffd \ufffd\ufffdư")]
	public NKCUIComStateButton m_btnCancel;

	public NKCUIComStateButton m_btnOK;

	private OnCloseSmartRemove m_dOnClose;

	private HashSet<NKCEquipSortSystem.eFilterOption> m_hsOptions = new HashSet<NKCEquipSortSystem.eFilterOption>();

	private int m_CurSelectedEnchantLevel;

	private Dictionary<NKCEquipSortSystem.eFilterOption, NKCUIComToggle> m_dicToggle = new Dictionary<NKCEquipSortSystem.eFilterOption, NKCUIComToggle>();

	public void InitUI(OnCloseSmartRemove dOnClose)
	{
		m_dOnClose = dOnClose;
		m_btnEnchanted.PointerClick.RemoveAllListeners();
		m_btnEnchanted.PointerClick.AddListener(OnClickEnchanted);
		m_btnNotEnchanted.PointerClick.RemoveAllListeners();
		m_btnNotEnchanted.PointerClick.AddListener(OnClickNotEnchanted);
		m_dicToggle.Clear();
		BindToggle(NKCEquipSortSystem.eFilterOption.Equip_Rarity_SSR, m_tglSSR);
		BindToggle(NKCEquipSortSystem.eFilterOption.Equip_Rarity_SR, m_tglSR);
		BindToggle(NKCEquipSortSystem.eFilterOption.Equip_Rarity_R, m_tglR);
		BindToggle(NKCEquipSortSystem.eFilterOption.Equip_Rarity_N, m_tglN);
		BindToggle(NKCEquipSortSystem.eFilterOption.Equip_Tier_7, m_tgl_tier_7);
		BindToggle(NKCEquipSortSystem.eFilterOption.Equip_Tier_6, m_tgl_tier_6);
		BindToggle(NKCEquipSortSystem.eFilterOption.Equip_Tier_5, m_tgl_tier_5);
		List<NKCEquipSortSystem.eFilterOption> lstFilterOption = new List<NKCEquipSortSystem.eFilterOption>
		{
			NKCEquipSortSystem.eFilterOption.Equip_Tier_4,
			NKCEquipSortSystem.eFilterOption.Equip_Tier_3,
			NKCEquipSortSystem.eFilterOption.Equip_Tier_2,
			NKCEquipSortSystem.eFilterOption.Equip_Tier_1
		};
		BindToggle(lstFilterOption, m_tgl_tier_4_under);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(OnClickClose);
		m_btnOK.PointerClick.RemoveAllListeners();
		m_btnOK.PointerClick.AddListener(OnClickOK);
	}

	private void BindToggle(List<NKCEquipSortSystem.eFilterOption> lstFilterOption, NKCUIComToggle tgl)
	{
		for (int i = 0; i < lstFilterOption.Count; i++)
		{
			if (!m_dicToggle.ContainsKey(lstFilterOption[i]))
			{
				m_dicToggle.Add(lstFilterOption[i], tgl);
			}
		}
		tgl.OnValueChanged.RemoveAllListeners();
		tgl.OnValueChanged.AddListener(delegate(bool bValue)
		{
			SetFilter(bValue, lstFilterOption);
		});
	}

	private void BindToggle(NKCEquipSortSystem.eFilterOption filterOption, NKCUIComToggle tgl)
	{
		if (!m_dicToggle.ContainsKey(filterOption))
		{
			m_dicToggle.Add(filterOption, tgl);
			tgl.OnValueChanged.RemoveAllListeners();
			tgl.OnValueChanged.AddListener(delegate(bool bValue)
			{
				SetFilter(bValue, filterOption);
			});
		}
	}

	private void SetFilter(bool bValue, List<NKCEquipSortSystem.eFilterOption> lstFilterOption)
	{
		if (bValue)
		{
			for (int i = 0; i < lstFilterOption.Count; i++)
			{
				if (!m_hsOptions.Contains(lstFilterOption[i]))
				{
					m_hsOptions.Add(lstFilterOption[i]);
				}
			}
			return;
		}
		for (int j = 0; j < lstFilterOption.Count; j++)
		{
			if (m_hsOptions.Contains(lstFilterOption[j]))
			{
				m_hsOptions.Remove(lstFilterOption[j]);
			}
		}
	}

	private void SetFilter(bool bValue, NKCEquipSortSystem.eFilterOption filterOption)
	{
		if (bValue)
		{
			if (!m_hsOptions.Contains(filterOption))
			{
				m_hsOptions.Add(filterOption);
			}
		}
		else if (m_hsOptions.Contains(filterOption))
		{
			m_hsOptions.Remove(filterOption);
		}
	}

	public void Open(int maxEnchantLevel, HashSet<NKCEquipSortSystem.eFilterOption> hsOptions)
	{
		m_CurSelectedEnchantLevel = maxEnchantLevel;
		m_hsOptions = hsOptions;
		SetToggleState();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void SetToggleState()
	{
		foreach (KeyValuePair<NKCEquipSortSystem.eFilterOption, NKCUIComToggle> item in m_dicToggle)
		{
			item.Value.Select(m_hsOptions.Contains(item.Key), bForce: true);
		}
		m_btnEnchanted.Select(m_CurSelectedEnchantLevel > 0);
		m_btnNotEnchanted.Select(m_CurSelectedEnchantLevel == 0);
		if (m_CurSelectedEnchantLevel > 0)
		{
			NKCUtil.SetLabelText(m_lbEnchantLevel, string.Format(NKCUtilString.GET_STRING_EQUIP_FILTER_SELECT_ENCHANT_COUNT, m_CurSelectedEnchantLevel));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbEnchantLevel, NKCUtilString.GET_STRING_EQUIP_FILTER_SELECT_ENCHANT);
		}
	}

	private void OnClickEnchanted()
	{
		m_btnNotEnchanted.Select(bSelect: false, bForce: true);
		m_btnEnchanted.Select(bSelect: true, bForce: true);
		if (!NKCPopupSelectEnchantLevel.IsInstanceOpen)
		{
			NKCPopupSelectEnchantLevel.Instance.Open(OnEnchantedClose, m_CurSelectedEnchantLevel);
		}
	}

	private void OnClickNotEnchanted()
	{
		m_btnNotEnchanted.Select(bSelect: true, bForce: true);
		m_btnEnchanted.Select(bSelect: false, bForce: true);
		if (m_CurSelectedEnchantLevel > 0)
		{
			m_CurSelectedEnchantLevel = 0;
			NKCUtil.SetLabelText(m_lbEnchantLevel, NKCUtilString.GET_STRING_EQUIP_FILTER_SELECT_ENCHANT);
		}
	}

	private void OnClickOK()
	{
		Close();
		m_dOnClose?.Invoke(m_CurSelectedEnchantLevel, m_hsOptions);
	}

	private void OnClickClose()
	{
		Close();
	}

	private void OnEnchantedClose(int enchantLevel)
	{
		m_CurSelectedEnchantLevel = enchantLevel;
		SetToggleState();
	}
}
