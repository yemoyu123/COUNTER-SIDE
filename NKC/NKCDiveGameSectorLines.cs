using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class NKCDiveGameSectorLines : MonoBehaviour
{
	public List<GameObject> m_lstLinesEven;

	public List<GameObject> m_lstLinesOdd;

	public List<GameObject> m_lstLinesEven0;

	public List<GameObject> m_lstLinesEven1;

	public List<GameObject> m_lstLinesEven2;

	public List<GameObject> m_lstLinesEven3;

	public List<GameObject> m_lstLinesOdd0;

	public List<GameObject> m_lstLinesOdd1;

	public List<GameObject> m_lstLinesOdd2;

	public List<GameObject> m_lstLinesOdd3;

	public List<GameObject> m_lstLinesOdd4;

	private List<List<GameObject>> m_dlstLinesEven = new List<List<GameObject>>();

	private List<List<GameObject>> m_dlstLinesOdd = new List<List<GameObject>>();

	public List<GameObject> m_lstBossLinesEven;

	public List<GameObject> m_lstBossLinesOdd;

	public List<GameObject> m_lstStartLinesEven;

	public List<GameObject> m_lstStartLinesOdd;

	public Animator m_Animator;

	public void Init()
	{
		int num = 0;
		for (num = 0; num < m_lstLinesEven.Count; num++)
		{
			m_dlstLinesEven.Add(new List<GameObject>());
		}
		for (num = 0; num < m_lstLinesEven0.Count; num++)
		{
			m_dlstLinesEven[0].Add(m_lstLinesEven0[num]);
		}
		for (num = 0; num < m_lstLinesEven1.Count; num++)
		{
			m_dlstLinesEven[1].Add(m_lstLinesEven1[num]);
		}
		for (num = 0; num < m_lstLinesEven2.Count; num++)
		{
			m_dlstLinesEven[2].Add(m_lstLinesEven2[num]);
		}
		for (num = 0; num < m_lstLinesEven3.Count; num++)
		{
			m_dlstLinesEven[3].Add(m_lstLinesEven3[num]);
		}
		for (num = 0; num < m_lstLinesOdd.Count; num++)
		{
			m_dlstLinesOdd.Add(new List<GameObject>());
		}
		for (num = 0; num < m_lstLinesOdd0.Count; num++)
		{
			m_dlstLinesOdd[0].Add(m_lstLinesOdd0[num]);
		}
		for (num = 0; num < m_lstLinesOdd1.Count; num++)
		{
			m_dlstLinesOdd[1].Add(m_lstLinesOdd1[num]);
		}
		for (num = 0; num < m_lstLinesOdd2.Count; num++)
		{
			m_dlstLinesOdd[2].Add(m_lstLinesOdd2[num]);
		}
		for (num = 0; num < m_lstLinesOdd3.Count; num++)
		{
			m_dlstLinesOdd[3].Add(m_lstLinesOdd3[num]);
		}
		for (num = 0; num < m_lstLinesOdd4.Count; num++)
		{
			m_dlstLinesOdd[4].Add(m_lstLinesOdd4[num]);
		}
	}

	private void SetInActiveList(List<GameObject> lstLines)
	{
		int num = 0;
		for (num = 0; num < lstLines.Count; num++)
		{
			NKCUtil.SetGameobjectActive(lstLines[num], bValue: false);
		}
	}

	private void SetActiveList(List<GameObject> lstLines)
	{
		int num = 0;
		for (num = 0; num < lstLines.Count; num++)
		{
			NKCUtil.SetGameobjectActive(lstLines[num], bValue: true);
		}
	}

	private void SetActiveGO(List<GameObject> lstLines, int index)
	{
		if (index >= 0 && index < lstLines.Count)
		{
			NKCUtil.SetGameobjectActive(lstLines[index], bValue: true);
		}
	}

	private int GetStartIndex(bool bEven, int maxEvenCount, int maxOddCount, int realSetSize)
	{
		int num = 0;
		if (bEven)
		{
			num = maxEvenCount / 2 - 1;
			return num - (realSetSize / 2 - 1);
		}
		num = maxOddCount / 2;
		return num - realSetSize / 2;
	}

	public void OpenFromMyPos(int realSetSize, int toRealSetSize, int uiIndex, int realIndex, bool bFromStart, bool bToBoss)
	{
		bool flag = realSetSize % 2 == 0;
		bool flag2 = toRealSetSize % 2 == 0;
		int num = 0;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetInActiveList(m_lstLinesEven);
		SetInActiveList(m_lstLinesOdd);
		SetInActiveList(m_lstBossLinesEven);
		SetInActiveList(m_lstBossLinesOdd);
		SetInActiveList(m_lstStartLinesEven);
		SetInActiveList(m_lstStartLinesOdd);
		if (bFromStart)
		{
			if (flag2)
			{
				int startIndex = GetStartIndex(bEven: true, m_lstStartLinesEven.Count, m_lstStartLinesOdd.Count, toRealSetSize);
				for (num = 0; num < m_lstStartLinesEven.Count; num++)
				{
					if (num >= startIndex && num < toRealSetSize + startIndex)
					{
						SetActiveGO(m_lstStartLinesEven, num);
					}
				}
			}
			else
			{
				int startIndex2 = GetStartIndex(bEven: false, m_lstStartLinesEven.Count, m_lstStartLinesOdd.Count, toRealSetSize);
				for (num = 0; num < m_lstStartLinesOdd.Count; num++)
				{
					if (num >= startIndex2 && num < toRealSetSize + startIndex2)
					{
						SetActiveGO(m_lstStartLinesOdd, num);
					}
				}
			}
		}
		else if (bToBoss)
		{
			if (flag)
			{
				SetActiveGO(m_lstBossLinesEven, uiIndex);
			}
			else
			{
				SetActiveGO(m_lstBossLinesOdd, uiIndex);
			}
		}
		else if (flag)
		{
			SetActiveGO(m_lstLinesEven, uiIndex);
			if (uiIndex == 0 || uiIndex == m_dlstLinesEven.Count - 1)
			{
				SetActiveList(m_dlstLinesEven[uiIndex]);
			}
			else if (realIndex == 0)
			{
				for (num = 0; num < m_dlstLinesEven[uiIndex].Count; num++)
				{
					if (num == 0)
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesEven[uiIndex][num], bValue: false);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesEven[uiIndex][num], bValue: true);
					}
				}
			}
			else if (realIndex == realSetSize - 1)
			{
				for (num = 0; num < m_dlstLinesEven[uiIndex].Count; num++)
				{
					if (num == m_dlstLinesEven[uiIndex].Count - 1)
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesEven[uiIndex][num], bValue: false);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesEven[uiIndex][num], bValue: true);
					}
				}
			}
			else
			{
				SetActiveList(m_dlstLinesEven[uiIndex]);
			}
		}
		else
		{
			SetActiveGO(m_lstLinesOdd, uiIndex);
			if (uiIndex == 0 || uiIndex == m_dlstLinesOdd.Count - 1)
			{
				SetActiveList(m_dlstLinesOdd[uiIndex]);
			}
			else if (realSetSize == 1)
			{
				for (num = 0; num < m_dlstLinesOdd[uiIndex].Count; num++)
				{
					if (num == 1)
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: true);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: false);
					}
				}
			}
			else if (realIndex == 0)
			{
				for (num = 0; num < m_dlstLinesOdd[uiIndex].Count; num++)
				{
					if (num == 0)
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: false);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: true);
					}
				}
			}
			else if (realIndex == realSetSize - 1)
			{
				for (num = 0; num < m_dlstLinesOdd[uiIndex].Count; num++)
				{
					if (num == m_dlstLinesOdd[uiIndex].Count - 1)
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: false);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: true);
					}
				}
			}
			else
			{
				SetActiveList(m_dlstLinesOdd[uiIndex]);
			}
		}
		if (m_Animator != null)
		{
			m_Animator.Play("NKM_UI_DIVE_PROCESS_SECTOR_LINES_INTRO");
		}
	}

	public void OpenFromSelectedMyPos(int realSetSize, int toRealSetSize, int uiIndex, int toUIIndex, int realIndex, int toRealIndex, bool bFromStart, bool bToBoss)
	{
		bool flag = realSetSize % 2 == 0;
		bool flag2 = toRealSetSize % 2 == 0;
		int num = 0;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetInActiveList(m_lstLinesEven);
		SetInActiveList(m_lstLinesOdd);
		SetInActiveList(m_lstBossLinesEven);
		SetInActiveList(m_lstBossLinesOdd);
		SetInActiveList(m_lstStartLinesEven);
		SetInActiveList(m_lstStartLinesOdd);
		if (bFromStart)
		{
			if (flag2)
			{
				SetActiveGO(m_lstStartLinesEven, toUIIndex);
			}
			else
			{
				SetActiveGO(m_lstStartLinesOdd, toUIIndex);
			}
			return;
		}
		if (bToBoss)
		{
			if (flag)
			{
				SetActiveGO(m_lstBossLinesEven, uiIndex);
			}
			else
			{
				SetActiveGO(m_lstBossLinesOdd, uiIndex);
			}
			return;
		}
		int num2 = 0;
		num2 = ((uiIndex >= 1) ? 1 : 0);
		if (flag)
		{
			SetActiveGO(m_lstLinesEven, uiIndex);
			for (num = 0; num < m_dlstLinesEven[uiIndex].Count; num++)
			{
				if (num != num2 + (toUIIndex - uiIndex))
				{
					NKCUtil.SetGameobjectActive(m_dlstLinesEven[uiIndex][num], bValue: false);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_dlstLinesEven[uiIndex][num], bValue: true);
				}
			}
			return;
		}
		SetActiveGO(m_lstLinesOdd, uiIndex);
		if (realSetSize == 1)
		{
			for (num = 0; num < m_dlstLinesOdd[uiIndex].Count; num++)
			{
				if (num == 1)
				{
					NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: true);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: false);
				}
			}
			return;
		}
		for (num = 0; num < m_dlstLinesOdd[uiIndex].Count; num++)
		{
			if (num != num2 + (toUIIndex - uiIndex))
			{
				NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: true);
			}
		}
	}

	public void OpenFromSelected(int realSetSize, int uiIndex, int realIndex, bool bToBoss)
	{
		bool flag = realSetSize % 2 == 0;
		int num = 0;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetInActiveList(m_lstLinesEven);
		SetInActiveList(m_lstLinesOdd);
		SetInActiveList(m_lstBossLinesEven);
		SetInActiveList(m_lstBossLinesOdd);
		SetInActiveList(m_lstStartLinesEven);
		SetInActiveList(m_lstStartLinesOdd);
		if (bToBoss)
		{
			if (flag)
			{
				SetActiveGO(m_lstBossLinesEven, uiIndex);
			}
			else
			{
				SetActiveGO(m_lstBossLinesOdd, uiIndex);
			}
		}
		else if (flag)
		{
			SetActiveGO(m_lstLinesEven, uiIndex);
			if (uiIndex == 0 || uiIndex == m_dlstLinesEven.Count - 1)
			{
				SetActiveList(m_dlstLinesEven[uiIndex]);
			}
			else if (realIndex == 0)
			{
				for (num = 0; num < m_dlstLinesEven[uiIndex].Count; num++)
				{
					if (num == 0)
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesEven[uiIndex][num], bValue: false);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesEven[uiIndex][num], bValue: true);
					}
				}
			}
			else if (realIndex == realSetSize - 1)
			{
				for (num = 0; num < m_dlstLinesEven[uiIndex].Count; num++)
				{
					if (num == m_dlstLinesEven[uiIndex].Count - 1)
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesEven[uiIndex][num], bValue: false);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesEven[uiIndex][num], bValue: true);
					}
				}
			}
			else
			{
				SetActiveList(m_dlstLinesEven[uiIndex]);
			}
		}
		else
		{
			SetActiveGO(m_lstLinesOdd, uiIndex);
			if (uiIndex == 0 || uiIndex == m_dlstLinesOdd.Count - 1)
			{
				SetActiveList(m_dlstLinesOdd[uiIndex]);
			}
			else if (realSetSize == 1)
			{
				for (num = 0; num < m_dlstLinesOdd[uiIndex].Count; num++)
				{
					if (num == 1)
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: true);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: false);
					}
				}
			}
			else if (realIndex == 0)
			{
				for (num = 0; num < m_dlstLinesOdd[uiIndex].Count; num++)
				{
					if (num == 0)
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: false);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: true);
					}
				}
			}
			else if (realIndex == realSetSize - 1)
			{
				for (num = 0; num < m_dlstLinesOdd[uiIndex].Count; num++)
				{
					if (num == m_dlstLinesOdd[uiIndex].Count - 1)
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: false);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_dlstLinesOdd[uiIndex][num], bValue: true);
					}
				}
			}
			else
			{
				SetActiveList(m_dlstLinesOdd[uiIndex]);
			}
		}
		if (m_Animator != null)
		{
			m_Animator.Play("NKM_UI_DIVE_PROCESS_SECTOR_LINES_INTRO");
		}
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
