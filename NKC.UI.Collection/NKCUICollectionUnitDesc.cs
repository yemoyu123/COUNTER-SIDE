using System.Collections.Generic;
using NKC.Templet;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionUnitDesc : MonoBehaviour
{
	public ScrollRect m_ScrollRect;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public NKCUICollectionUnitDescSlot m_prfCollectionUnitDescSlot;

	private const int MinUnitDescSlotCount = 3;

	private List<NKCUICollectionUnitDescSlot> m_UnitDescSlotList = new List<NKCUICollectionUnitDescSlot>();

	public void Init()
	{
		int childCount = m_ScrollRect.content.childCount;
		for (int i = 0; i < childCount; i++)
		{
			m_ScrollRect.content.GetChild(i).GetComponent<NKCUICollectionUnitDescSlot>()?.Init();
		}
	}

	public void SetData(int unitId)
	{
		NKCCollectionEmployeeTemplet employeeTemplet = NKCCollectionManager.GetEmployeeTemplet(unitId);
		int num = m_UnitDescSlotList.Count;
		if (num <= 0)
		{
			num = m_ScrollRect.content.childCount;
			for (int i = 0; i < num; i++)
			{
				NKCUICollectionUnitDescSlot component = m_ScrollRect.content.GetChild(i).GetComponent<NKCUICollectionUnitDescSlot>();
				m_UnitDescSlotList.Add(component);
			}
			if (num < 3)
			{
				int num2 = 3 - num;
				for (int j = 0; j < num2; j++)
				{
					NKCUICollectionUnitDescSlot nKCUICollectionUnitDescSlot = Object.Instantiate(m_prfCollectionUnitDescSlot, m_ScrollRect.content);
					nKCUICollectionUnitDescSlot.Init();
					m_UnitDescSlotList.Add(nKCUICollectionUnitDescSlot);
				}
				num = m_UnitDescSlotList.Count;
			}
		}
		int num3 = Mathf.Max(employeeTemplet.ProfileType.Count, employeeTemplet.ProfileValue.Count, 3);
		int num4 = Mathf.Max(num3, num);
		for (int k = 0; k < num4; k++)
		{
			NKCUICollectionUnitDescSlot nKCUICollectionUnitDescSlot2 = null;
			if (k >= num)
			{
				nKCUICollectionUnitDescSlot2 = Object.Instantiate(m_prfCollectionUnitDescSlot, m_ScrollRect.content);
				nKCUICollectionUnitDescSlot2.Init();
				m_UnitDescSlotList.Add(nKCUICollectionUnitDescSlot2);
			}
			else
			{
				nKCUICollectionUnitDescSlot2 = m_UnitDescSlotList[k];
			}
			if (employeeTemplet == null || k >= num3)
			{
				NKCUtil.SetGameobjectActive(nKCUICollectionUnitDescSlot2, bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(nKCUICollectionUnitDescSlot2, bValue: true);
			string type = ((employeeTemplet.ProfileType.Count > k) ? employeeTemplet.ProfileType[k] : "");
			string value = ((employeeTemplet.ProfileValue.Count > k) ? employeeTemplet.ProfileValue[k] : "");
			nKCUICollectionUnitDescSlot2?.SetData(k, unitId, type, value);
		}
		m_ScrollRect.verticalNormalizedPosition = 1f;
	}

	public void SetData(NKMOperator operData)
	{
		NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(operData.id);
		if (unitTemplet == null)
		{
			return;
		}
		int count = m_UnitDescSlotList.Count;
		if (count <= 0)
		{
			count = m_ScrollRect.content.childCount;
			for (int i = 0; i < count; i++)
			{
				NKCUICollectionUnitDescSlot component = m_ScrollRect.content.GetChild(i).GetComponent<NKCUICollectionUnitDescSlot>();
				m_UnitDescSlotList.Add(component);
			}
			if (count < 3)
			{
				int num = 3 - count;
				for (int j = 0; j < num; j++)
				{
					NKCUICollectionUnitDescSlot nKCUICollectionUnitDescSlot = Object.Instantiate(m_prfCollectionUnitDescSlot, m_ScrollRect.content);
					nKCUICollectionUnitDescSlot.Init();
					m_UnitDescSlotList.Add(nKCUICollectionUnitDescSlot);
				}
				count = m_UnitDescSlotList.Count;
			}
		}
		for (int k = 0; k < m_UnitDescSlotList.Count; k++)
		{
			NKCUICollectionUnitDescSlot nKCUICollectionUnitDescSlot2 = m_UnitDescSlotList[k];
			if (!(nKCUICollectionUnitDescSlot2 == null))
			{
				if (k > 0)
				{
					NKCUtil.SetGameobjectActive(nKCUICollectionUnitDescSlot2, bValue: false);
					continue;
				}
				NKCUtil.SetGameobjectActive(nKCUICollectionUnitDescSlot2, bValue: true);
				nKCUICollectionUnitDescSlot2?.SetData(k, operData.id, "SI_COLLECTION_PROFILE_TYPE_PROFILE_PROFILE", unitTemplet.UnitIntro);
			}
		}
		m_ScrollRect.verticalNormalizedPosition = 1f;
	}

	private void OnDestroy()
	{
		m_UnitDescSlotList?.Clear();
	}
}
