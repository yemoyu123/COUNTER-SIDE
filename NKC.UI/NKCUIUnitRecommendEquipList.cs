using System.Collections.Generic;
using Cs.Logging;
using NKC.Templet;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitRecommendEquipList : MonoBehaviour
{
	public NKCUIUnitRecommendEquipListSlot m_pfbSlot;

	public Text m_lbTitle;

	public ScrollRect m_sr;

	public NKCUIComStateButton m_btnClose;

	private Stack<NKCUIUnitRecommendEquipListSlot> m_stkSlot = new Stack<NKCUIUnitRecommendEquipListSlot>();

	private List<NKCUIUnitRecommendEquipListSlot> m_lstVisible = new List<NKCUIUnitRecommendEquipListSlot>();

	public void Init()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(Close);
	}

	public void Open(int setOptionID, List<int> equipGroupID)
	{
		if (equipGroupID.Count <= 0)
		{
			return;
		}
		if (m_lstVisible.Count > 0)
		{
			ResetSlot();
		}
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(setOptionID);
		NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString(equipSetOptionTemplet.m_EquipSetName));
		for (int i = 0; i < equipGroupID.Count; i++)
		{
			NKCEquipRecommendListTemplet nKCEquipRecommendListTemplet = NKCEquipRecommendListTemplet.Find(equipGroupID[i]);
			if (nKCEquipRecommendListTemplet == null)
			{
				Log.Error($"NKCEquipRecommendListTemplet is null - {equipGroupID[i]}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/UnitInfo/NKCUIUnitRecommendEquipList.cs", 45);
				continue;
			}
			NKCUIUnitRecommendEquipListSlot slot = GetSlot();
			slot.transform.SetParent(m_sr.content);
			m_lstVisible.Add(slot);
			NKCUtil.SetGameobjectActive(slot, bValue: true);
			slot.SetData(nKCEquipRecommendListTemplet);
			slot.transform.SetAsLastSibling();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	public void Close()
	{
		ResetSlot();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void ResetSlot()
	{
		for (int i = 0; i < m_lstVisible.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstVisible[i], bValue: false);
			m_stkSlot.Push(m_lstVisible[i]);
		}
		m_lstVisible.Clear();
	}

	public NKCUIUnitRecommendEquipListSlot GetSlot()
	{
		if (m_stkSlot.Count > 0)
		{
			return m_stkSlot.Pop();
		}
		NKCUIUnitRecommendEquipListSlot nKCUIUnitRecommendEquipListSlot = Object.Instantiate(m_pfbSlot, m_sr.content);
		nKCUIUnitRecommendEquipListSlot.Init();
		return nKCUIUnitRecommendEquipListSlot;
	}
}
