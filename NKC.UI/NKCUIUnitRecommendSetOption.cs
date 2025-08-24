using System.Collections.Generic;
using NKC.Templet;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace NKC.UI;

public class NKCUIUnitRecommendSetOption : MonoBehaviour
{
	public delegate void OnEquip(int setOptionID);

	public delegate void OnClose();

	public NKCUIUnitRecommendSetOptionSlot m_pfbSlot;

	public NKCUIComStateButton m_btnHelp;

	public NKCUIComStateButton m_btnClose;

	public ScrollRect m_sr;

	public GameObject m_objRecommendOption;

	public Text m_lbOption_1;

	public Text m_lbOption_2;

	private Stack<NKCUIUnitRecommendSetOptionSlot> m_stkSlot = new Stack<NKCUIUnitRecommendSetOptionSlot>();

	private List<NKCUIUnitRecommendSetOptionSlot> m_lstVisible = new List<NKCUIUnitRecommendSetOptionSlot>();

	private OnEquip m_dOnEquip;

	private OnClose m_dOnClose;

	public void Init(OnClose onClose, OnEquip onEquip)
	{
		m_dOnClose = onClose;
		m_dOnEquip = onEquip;
		m_btnHelp.PointerClick.RemoveAllListeners();
		m_btnHelp.PointerClick.AddListener(OnClickHelp);
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(Close);
	}

	public void Close()
	{
		for (int i = 0; i < m_lstVisible.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstVisible[i], bValue: false);
			m_stkSlot.Push(m_lstVisible[i]);
		}
		m_lstVisible.Clear();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_dOnClose?.Invoke();
	}

	public void Open(int unitID)
	{
		NKCUnitEquipRecommendTemplet templet = NKCUnitEquipRecommendTemplet.Find(unitID);
		if (templet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objRecommendOption, templet.m_lstRecommendStatType.Count > 0);
		if (templet.m_lstRecommendStatType.Count > 0)
		{
			NKCStatInfoTemplet nKCStatInfoTemplet = NKMTempletContainer<NKCStatInfoTemplet>.Find((NKCStatInfoTemplet x) => x.StatType == templet.m_lstRecommendStatType[0]);
			if (string.IsNullOrEmpty(nKCStatInfoTemplet.Filter_Name))
			{
				NKCUtil.SetLabelText(m_lbOption_1, NKCStringTable.GetString(nKCStatInfoTemplet.Stat_Name));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbOption_1, NKCStringTable.GetString(nKCStatInfoTemplet.Filter_Name));
			}
			if (templet.m_lstRecommendStatType.Count > 1)
			{
				NKCUtil.SetGameobjectActive(m_lbOption_2, bValue: true);
				nKCStatInfoTemplet = NKMTempletContainer<NKCStatInfoTemplet>.Find((NKCStatInfoTemplet x) => x.StatType == templet.m_lstRecommendStatType[1]);
				if (string.IsNullOrEmpty(nKCStatInfoTemplet.Filter_Name))
				{
					NKCUtil.SetLabelText(m_lbOption_2, NKCStringTable.GetString(nKCStatInfoTemplet.Stat_Name));
				}
				else
				{
					NKCUtil.SetLabelText(m_lbOption_2, NKCStringTable.GetString(nKCStatInfoTemplet.Filter_Name));
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lbOption_2, bValue: false);
			}
		}
		for (int num = 0; num < templet.m_lstSetOptionID.Count; num++)
		{
			NKCUIUnitRecommendSetOptionSlot slot = GetSlot();
			slot.transform.SetParent(m_sr.content);
			m_lstVisible.Add(slot);
			NKCUtil.SetGameobjectActive(slot, bValue: true);
			slot.SetData(templet.m_lstSetOptionID[num]);
			slot.transform.SetAsLastSibling();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_sr.ScrollToTop();
	}

	private NKCUIUnitRecommendSetOptionSlot GetSlot()
	{
		if (m_stkSlot.Count > 0)
		{
			return m_stkSlot.Pop();
		}
		NKCUIUnitRecommendSetOptionSlot nKCUIUnitRecommendSetOptionSlot = Object.Instantiate(m_pfbSlot, m_sr.content);
		nKCUIUnitRecommendSetOptionSlot.Init(m_dOnEquip);
		return nKCUIUnitRecommendSetOptionSlot;
	}

	private void OnClickHelp()
	{
	}
}
