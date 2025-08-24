using System.Collections;
using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKCDiveGameSectorSet : MonoBehaviour
{
	private bool m_bEven = true;

	private int m_RealSetSize;

	public GameObject m_NKM_UI_DIVE_PROCESS_SECTOR_SET_EVEN;

	public GameObject m_NKM_UI_DIVE_PROCESS_SECTOR_SET_ODD;

	public List<NKCDiveGameSector> m_lstNKCDiveGameSectorEven = new List<NKCDiveGameSector>();

	public List<NKCDiveGameSector> m_lstNKCDiveGameSectorOdd = new List<NKCDiveGameSector>();

	private int m_Distance;

	private bool m_bRealActive;

	private Coroutine m_coAni;

	private NKCAssetInstanceData m_InstanceData;

	public int GetDistance()
	{
		return m_Distance;
	}

	public bool IsAnimating()
	{
		return m_coAni != null;
	}

	public void StopAni()
	{
		if (m_coAni != null)
		{
			StopCoroutine(m_coAni);
			m_coAni = null;
		}
	}

	public static NKCDiveGameSectorSet GetNewInstance(int _Distance, Transform parent, NKCDiveGameSector.OnClickSector dOnClickSector = null)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WORLD_MAP_DIVE", "NKM_UI_DIVE_PROCESS_SECTOR_SET");
		NKCDiveGameSectorSet component = nKCAssetInstanceData.m_Instant.GetComponent<NKCDiveGameSectorSet>();
		if (component == null)
		{
			Debug.LogError("NKCDiveGameSectorSet Prefab null!");
			return null;
		}
		component.m_Distance = _Distance;
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		int num = 0;
		for (num = 0; num < component.m_lstNKCDiveGameSectorEven.Count; num++)
		{
			component.m_lstNKCDiveGameSectorEven[num].Init(component, dOnClickSector);
		}
		for (num = 0; num < component.m_lstNKCDiveGameSectorOdd.Count; num++)
		{
			component.m_lstNKCDiveGameSectorOdd[num].Init(component, dOnClickSector);
		}
		component.gameObject.SetActive(value: false);
		return component;
	}

	private void OnDestroy()
	{
		if (m_InstanceData != null)
		{
			NKCAssetResourceManager.CloseInstance(m_InstanceData);
		}
		m_InstanceData = null;
	}

	public void SetActive(bool bSet)
	{
		m_bRealActive = bSet;
	}

	public bool GetActive()
	{
		return m_bRealActive;
	}

	public int GetRealSetSize()
	{
		return m_RealSetSize;
	}

	private void SetEven(bool bEven)
	{
		m_bEven = bEven;
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SECTOR_SET_EVEN, bEven);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SECTOR_SET_ODD, !bEven);
	}

	private int GetStartIndex()
	{
		int num = 0;
		if (m_bEven)
		{
			num = m_lstNKCDiveGameSectorEven.Count / 2 - 1;
			return num - (m_RealSetSize / 2 - 1);
		}
		num = m_lstNKCDiveGameSectorOdd.Count / 2;
		return num - m_RealSetSize / 2;
	}

	public void SetBoss()
	{
		SetEven(bEven: false);
		int num = 0;
		for (num = 0; num < m_lstNKCDiveGameSectorOdd.Count; num++)
		{
			if (num == m_lstNKCDiveGameSectorOdd.Count / 2)
			{
				NKCUtil.SetGameobjectActive(m_lstNKCDiveGameSectorOdd[num], bValue: true);
				m_lstNKCDiveGameSectorOdd[num].SetUI(new NKMDiveSlot(NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_BOSS, NKM_DIVE_EVENT_TYPE.NDET_DUNGEON_BOSS, 0, 0, 0));
				m_lstNKCDiveGameSectorOdd[num].SetSelected(bSet: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstNKCDiveGameSectorOdd[num], bValue: false);
			}
		}
	}

	public void SetGrey()
	{
		InvalidGrey();
		List<NKCDiveGameSector> list = null;
		list = ((!m_bEven) ? m_lstNKCDiveGameSectorOdd : m_lstNKCDiveGameSectorEven);
		int num = 0;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		int num2 = 0;
		if (myUserData.m_DiveGameData != null)
		{
			num2 = myUserData.m_DiveGameData.Player.PlayerBase.SlotIndex;
		}
		int num3 = 0;
		for (num = 0; num < list.Count; num++)
		{
			if (num >= GetStartIndex() && num < GetStartIndex() + m_RealSetSize)
			{
				if (num2 != num3 && num2 - 1 != num3 && num2 + 1 != num3)
				{
					list[num].SetGrey();
				}
				num3++;
			}
		}
	}

	public void InvalidGrey()
	{
		int num = 0;
		for (num = 0; num < m_lstNKCDiveGameSectorEven.Count; num++)
		{
			m_lstNKCDiveGameSectorEven[num].InvaldGrey();
		}
		for (num = 0; num < m_lstNKCDiveGameSectorOdd.Count; num++)
		{
			m_lstNKCDiveGameSectorOdd[num].InvaldGrey();
		}
	}

	private void SetDiveGameSectorData(NKCDiveGameSector cNKCDiveGameSector, NKMDiveSlot cNKMDiveSlot, int index, int realIndex)
	{
		if (!(cNKCDiveGameSector == null) && cNKMDiveSlot != null)
		{
			cNKCDiveGameSector.SetUI(cNKMDiveSlot);
			cNKCDiveGameSector.SetSlotIndex(realIndex);
			cNKCDiveGameSector.SetUISlotIndex(index);
		}
	}

	private IEnumerator ProcessGameSectorSpawnAni(NKMDiveSlotSet cNKMDiveSlotSet, List<NKCDiveGameSector> lstNKCDiveGameSector, bool playSpawnSound)
	{
		int realIndex = 0;
		for (int i = 0; i < lstNKCDiveGameSector.Count; i++)
		{
			if (i >= GetStartIndex() && i < GetStartIndex() + m_RealSetSize)
			{
				NKMDiveSlot cNKMDiveSlot = cNKMDiveSlotSet.Slots[realIndex];
				SetDiveGameSectorData(lstNKCDiveGameSector[i], cNKMDiveSlot, i, realIndex);
				lstNKCDiveGameSector[i].PlayOpenAni(playSpawnSound);
				yield return new WaitForSeconds(0.1f);
				realIndex++;
			}
			else
			{
				NKCUtil.SetGameobjectActive(lstNKCDiveGameSector[i], bValue: false);
			}
		}
		m_coAni = null;
	}

	public void SetUIByData(NKMDiveSlotSet cNKMDiveSlotSet, bool bSpawnAni = false, bool bPlaySpawnSound = true)
	{
		SetRealSize(cNKMDiveSlotSet.Slots.Count);
		List<NKCDiveGameSector> list = null;
		list = ((!m_bEven) ? m_lstNKCDiveGameSectorOdd : m_lstNKCDiveGameSectorEven);
		int num = 0;
		if (m_coAni != null)
		{
			StopCoroutine(m_coAni);
			m_coAni = null;
		}
		if (bSpawnAni && base.gameObject.activeSelf)
		{
			m_coAni = StartCoroutine(ProcessGameSectorSpawnAni(cNKMDiveSlotSet, list, bPlaySpawnSound));
			return;
		}
		int num2 = 0;
		for (num = 0; num < list.Count; num++)
		{
			if (num >= GetStartIndex() && num < GetStartIndex() + m_RealSetSize)
			{
				NKMDiveSlot cNKMDiveSlot = cNKMDiveSlotSet.Slots[num2];
				SetDiveGameSectorData(list[num], cNKMDiveSlot, num, num2);
				num2++;
			}
			else
			{
				NKCUtil.SetGameobjectActive(list[num], bValue: false);
			}
		}
	}

	public void SetAllEmpty()
	{
		List<NKCDiveGameSector> currSectorList = GetCurrSectorList();
		int num = 0;
		for (num = 0; num < currSectorList.Count; num++)
		{
			if (num >= GetStartIndex() && num < GetStartIndex() + m_RealSetSize)
			{
				currSectorList[num].SetUI(new NKMDiveSlot(NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_NONE, NKM_DIVE_EVENT_TYPE.NDET_NONE, 0, 0, 0));
				currSectorList[num].SetSlotIndex(num - GetStartIndex());
				currSectorList[num].SetUISlotIndex(num);
			}
		}
	}

	public NKCDiveGameSector GetBossSector()
	{
		List<NKCDiveGameSector> currSectorList = GetCurrSectorList();
		int num = 0;
		for (num = 0; num < currSectorList.Count; num++)
		{
			if (currSectorList[num] != null && currSectorList[num].gameObject.activeSelf && currSectorList[num].GetNKMDiveSlot().EventType == NKM_DIVE_EVENT_TYPE.NDET_DUNGEON_BOSS)
			{
				return currSectorList[num];
			}
		}
		return null;
	}

	public NKCDiveGameSector GetSector(NKMDiveSlot cNKMDiveSlot)
	{
		if (cNKMDiveSlot == null)
		{
			return null;
		}
		List<NKCDiveGameSector> currSectorList = GetCurrSectorList();
		int num = 0;
		for (num = 0; num < currSectorList.Count; num++)
		{
			if (currSectorList[num].GetNKMDiveSlot() == cNKMDiveSlot)
			{
				return currSectorList[num];
			}
		}
		return null;
	}

	public NKCDiveGameSector GetSector(int index)
	{
		List<NKCDiveGameSector> currSectorList = GetCurrSectorList();
		int num = 0;
		int num2 = 0;
		for (num2 = 0; num2 < currSectorList.Count; num2++)
		{
			if (num2 >= GetStartIndex() && num2 < GetStartIndex() + m_RealSetSize)
			{
				if (num == index)
				{
					return currSectorList[num2];
				}
				num++;
			}
		}
		return null;
	}

	private List<NKCDiveGameSector> GetCurrSectorList()
	{
		List<NKCDiveGameSector> list = null;
		if (m_bEven)
		{
			return m_lstNKCDiveGameSectorEven;
		}
		return m_lstNKCDiveGameSectorOdd;
	}

	public void SetRealSize(int realSize)
	{
		m_RealSetSize = realSize;
		SetEven(m_RealSetSize % 2 == 0);
		List<NKCDiveGameSector> currSectorList = GetCurrSectorList();
		int num = 0;
		for (num = 0; num < currSectorList.Count; num++)
		{
			if (num >= GetStartIndex() && num < GetStartIndex() + m_RealSetSize)
			{
				NKCUtil.SetGameobjectActive(currSectorList[num], bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(currSectorList[num], bValue: false);
			}
		}
	}
}
