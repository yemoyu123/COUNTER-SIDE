using System.Collections.Generic;
using UnityEngine;

namespace NKC.UI.Shop;

public class NKCUIShopCategoryChange : MonoBehaviour
{
	public NKCUIComStateButton m_csbtnClose;

	public List<NKCUIShopCategoryChangeSlot> m_lstSlot;

	public void Init(NKCUIShopCategoryChangeSlot.OnSelectCategory onSelectCategory)
	{
		foreach (NKCUIShopCategoryChangeSlot item in m_lstSlot)
		{
			item.Init(onSelectCategory);
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, Close);
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			m_lstSlot[i].CheckReddot();
		}
	}

	private void Close()
	{
		base.gameObject.SetActive(value: false);
	}
}
