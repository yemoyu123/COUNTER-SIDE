using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionIllustSlot : MonoBehaviour
{
	[Header("타이블")]
	public Text m_NKM_UI_COLLECTION_ALBUM_TITLE_TEXT_EP;

	public Text m_NKM_UI_COLLECTION_ALBUM_TITLE_TEXT;

	public RectTransform m_rt_NKM_UI_COLLECTION_ALBUM_SLOT_CONTENT;

	private List<RectTransform> m_lstRentalSlot = new List<RectTransform>();

	public void Init()
	{
	}

	public List<RectTransform> GetRentalSlot()
	{
		return m_lstRentalSlot;
	}

	public void ClearRentalList()
	{
		m_lstRentalSlot.Clear();
	}

	public void SetData(int CategoryID, List<RectTransform> lstSlot, NKCUICollectionIllust.OnIllustView CallBack = null)
	{
		NKCCollectionIllustTemplet illustTemplet = NKCCollectionManager.GetIllustTemplet(CategoryID);
		if (illustTemplet == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_ALBUM_TITLE_TEXT_EP, illustTemplet.GetCategoryTitle());
		NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_ALBUM_TITLE_TEXT, illustTemplet.GetCategorySubTitle());
		int num = 0;
		foreach (KeyValuePair<int, NKCCollectionIllustData> dicIllustDatum in illustTemplet.m_dicIllustData)
		{
			if (!(lstSlot[num] == null))
			{
				NKCUIIllustSlot component = lstSlot[num].GetComponent<NKCUIIllustSlot>();
				component.transform.SetParent(m_rt_NKM_UI_COLLECTION_ALBUM_SLOT_CONTENT);
				component.Init(CategoryID, dicIllustDatum.Key, CallBack);
				component.SetData(dicIllustDatum.Value);
				m_lstRentalSlot.Add(lstSlot[num]);
				num++;
			}
		}
	}
}
