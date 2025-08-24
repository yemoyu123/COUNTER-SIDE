using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Module;

public class NKCUIModuleCollectionGroup : MonoBehaviour
{
	public enum COLLECTION_GRADE
	{
		CG_N = 100101,
		CG_R,
		CG_SR,
		CG_SSR,
		CG_AWAKEN
	}

	public Transform m_slotRoot;

	public Text m_lbGrade;

	public Text m_lbCount;

	public GameObject m_objSlotPrefab;

	private NKCAssetInstanceData m_InstanceData;

	private List<NKCUIModuleCollectionSlot> m_slotList = new List<NKCUIModuleCollectionSlot>();

	private void Init()
	{
		if (m_slotRoot != null)
		{
			NKCUIModuleCollectionSlot[] componentsInChildren = m_slotRoot.GetComponentsInChildren<NKCUIModuleCollectionSlot>();
			int num = ((componentsInChildren != null) ? componentsInChildren.Length : 0);
			for (int i = 0; i < num; i++)
			{
				componentsInChildren[i].Init();
				m_slotList.Add(componentsInChildren[i]);
			}
		}
	}

	public void SetData(int collectionGroupId, List<(int, bool)> unitList)
	{
		if (m_slotRoot == null || unitList == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_lbGrade, GetGradeString((COLLECTION_GRADE)collectionGroupId));
		int count = m_slotList.Count;
		int count2 = unitList.Count;
		int num = count2 - count;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = Object.Instantiate(m_objSlotPrefab, m_slotRoot);
			if (!(gameObject == null))
			{
				NKCUIModuleCollectionSlot component = gameObject.GetComponent<NKCUIModuleCollectionSlot>();
				if (component != null)
				{
					component.Init();
					m_slotList.Add(component);
				}
			}
		}
		int num2 = 0;
		count = m_slotList.Count;
		for (int j = 0; j < count; j++)
		{
			if (j >= count2)
			{
				NKCUtil.SetGameobjectActive(m_slotList[j].gameObject, bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(m_slotList[j].gameObject, bValue: true);
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeUnitData(unitList[j].Item1, 1);
			m_slotList[j].SetData(slotData, collectionGroupId == 100105, OnClickUnitSlot);
			bool item = unitList[j].Item2;
			if (item)
			{
				num2++;
			}
			m_slotList[j].SetOwnState(item);
		}
		NKCUtil.SetLabelText(m_lbCount, $"{num2}/{count2}");
	}

	public static NKCUIModuleCollectionGroup GetNewInstance(Transform parent, string bundleName, string assetName)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(bundleName, assetName);
		NKCUIModuleCollectionGroup nKCUIModuleCollectionGroup = nKCAssetInstanceData?.m_Instant.GetComponent<NKCUIModuleCollectionGroup>();
		if (nKCUIModuleCollectionGroup == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIModuleCollectionGroup Prefab null!");
			return null;
		}
		nKCUIModuleCollectionGroup.m_InstanceData = nKCAssetInstanceData;
		nKCUIModuleCollectionGroup.Init();
		if (parent != null)
		{
			nKCUIModuleCollectionGroup.transform.SetParent(parent);
		}
		nKCUIModuleCollectionGroup.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCUIModuleCollectionGroup.gameObject.SetActive(value: false);
		return nKCUIModuleCollectionGroup;
	}

	public void DestoryInstance()
	{
		m_slotList?.Clear();
		m_slotList = null;
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	private string GetGradeString(COLLECTION_GRADE grade)
	{
		return grade switch
		{
			COLLECTION_GRADE.CG_N => NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_N_FOR_EVENTDECK"), 
			COLLECTION_GRADE.CG_R => NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_R_FOR_EVENTDECK"), 
			COLLECTION_GRADE.CG_SR => NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_SR_FOR_EVENTDECK"), 
			COLLECTION_GRADE.CG_SSR => NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_SSR_FOR_EVENTDECK"), 
			COLLECTION_GRADE.CG_AWAKEN => NKCStringTable.GetString("SI_PF_FILTER_UNIT_TYPE_AWAKEN"), 
			_ => "", 
		};
	}

	public void OnClickUnitSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (slotData != null)
		{
			NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, slotData);
		}
	}
}
