using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCDiveGameSectorSetMgr
{
	private List<NKCDiveGameSectorSet> m_lstNKCDiveGameSectorSet = new List<NKCDiveGameSectorSet>();

	private Transform m_trParentOfSets;

	private int m_RealDepth;

	private int m_RealSetSize;

	private bool m_bOpenAniForSecondLine;

	private float m_fElapsedTimeToOpenAniForSecondLine;

	private int m_UISectorSetIndexToOpenAniFor2ndLine;

	private int m_SectorSetIndexToOpenAniFor2ndLine;

	public void Init(Transform parentOfSets)
	{
		m_trParentOfSets = parentOfSets;
	}

	public bool IsAnimating()
	{
		if (m_bOpenAniForSecondLine)
		{
			return true;
		}
		for (int i = 0; i < m_lstNKCDiveGameSectorSet.Count; i++)
		{
			if (m_lstNKCDiveGameSectorSet[i].IsAnimating())
			{
				return true;
			}
		}
		return false;
	}

	public void StopAni()
	{
		for (int i = 0; i < m_lstNKCDiveGameSectorSet.Count; i++)
		{
			if (m_lstNKCDiveGameSectorSet[i] != null)
			{
				m_lstNKCDiveGameSectorSet[i].StopAni();
			}
		}
	}

	public NKCDiveGameSector GetBossSector()
	{
		for (int num = m_lstNKCDiveGameSectorSet.Count - 1; num >= 0; num--)
		{
			NKCDiveGameSectorSet nKCDiveGameSectorSet = m_lstNKCDiveGameSectorSet[num];
			if (nKCDiveGameSectorSet != null)
			{
				NKCDiveGameSector bossSector = nKCDiveGameSectorSet.GetBossSector();
				if (bossSector != null)
				{
					return bossSector;
				}
			}
		}
		return null;
	}

	public void Update()
	{
		if (!m_bOpenAniForSecondLine)
		{
			return;
		}
		float num = Time.deltaTime;
		if (num > NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f)
		{
			num = NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f;
		}
		m_fElapsedTimeToOpenAniForSecondLine += num;
		if (m_fElapsedTimeToOpenAniForSecondLine >= 0.5f)
		{
			NKMDiveGameData diveGameData = NKCScenManager.GetScenManager().GetMyUserData().m_DiveGameData;
			if (diveGameData != null)
			{
				m_lstNKCDiveGameSectorSet[m_UISectorSetIndexToOpenAniFor2ndLine].SetUIByData(diveGameData.Floor.SlotSets[m_SectorSetIndexToOpenAniFor2ndLine], bSpawnAni: true);
			}
			m_bOpenAniForSecondLine = false;
		}
	}

	public bool CheckExistEuclidInNextSectors()
	{
		for (int i = 0; i < m_lstNKCDiveGameSectorSet.Count; i++)
		{
			if (!m_lstNKCDiveGameSectorSet[i].GetActive())
			{
				continue;
			}
			for (int j = 0; j < m_lstNKCDiveGameSectorSet[i].GetRealSetSize(); j++)
			{
				NKCDiveGameSector sector = m_lstNKCDiveGameSectorSet[i].GetSector(j);
				if (!(sector == null) && sector.GetNKMDiveSlot() != null && sector.CheckSelectable() && NKCDiveManager.IsEuclidSectorType(sector.GetNKMDiveSlot().SectorType))
				{
					return true;
				}
			}
		}
		return false;
	}

	public NKCDiveGameSector GetNextDiveGameSectorByAuto(bool bStartPos)
	{
		int num = 0;
		List<int> list = new List<int>();
		for (num = 0; num < m_lstNKCDiveGameSectorSet.Count; num++)
		{
			if (!m_lstNKCDiveGameSectorSet[num].GetActive())
			{
				continue;
			}
			list.Clear();
			for (int i = 0; i < m_lstNKCDiveGameSectorSet[num].GetRealSetSize(); i++)
			{
				NKCDiveGameSector sector = m_lstNKCDiveGameSectorSet[num].GetSector(i);
				if (!(sector == null) && sector.GetNKMDiveSlot() != null && sector.CheckSelectable() && NKCDiveManager.IsEuclidSectorType(sector.GetNKMDiveSlot().SectorType))
				{
					list.Add(i);
				}
			}
			if (list.Count > 0)
			{
				int index = list[NKMRandom.Range(0, list.Count)];
				return m_lstNKCDiveGameSectorSet[num].GetSector(index);
			}
		}
		int num2 = 0;
		int num3 = 1;
		if (bStartPos)
		{
			num3 = 0;
		}
		for (num = 0; num < m_lstNKCDiveGameSectorSet.Count; num++)
		{
			if (!m_lstNKCDiveGameSectorSet[num].GetActive())
			{
				continue;
			}
			if (num3 == num2)
			{
				if (m_lstNKCDiveGameSectorSet[num].GetRealSetSize() == 1)
				{
					return m_lstNKCDiveGameSectorSet[num].GetSector(0);
				}
				list.Clear();
				for (int j = 0; j < m_lstNKCDiveGameSectorSet[num].GetRealSetSize(); j++)
				{
					NKCDiveGameSector sector2 = m_lstNKCDiveGameSectorSet[num].GetSector(j);
					if (!(sector2 == null) && sector2.GetNKMDiveSlot() != null && sector2.CheckSelectable())
					{
						list.Add(j);
					}
				}
				if (list.Count > 0)
				{
					int index2 = list[NKMRandom.Range(0, list.Count)];
					return m_lstNKCDiveGameSectorSet[num].GetSector(index2);
				}
			}
			num2++;
		}
		return null;
	}

	public NKCDiveGameSector GetActiveDiveGameSector(int slotSetIndex, int slotIndex)
	{
		if (slotSetIndex < 0 || slotSetIndex >= m_lstNKCDiveGameSectorSet.Count)
		{
			return null;
		}
		int num = 0;
		int num2 = 0;
		for (num2 = 0; num2 < m_lstNKCDiveGameSectorSet.Count; num2++)
		{
			if (m_lstNKCDiveGameSectorSet[num2].GetActive())
			{
				if (slotSetIndex == num)
				{
					return m_lstNKCDiveGameSectorSet[num2].GetSector(slotIndex);
				}
				num++;
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
		int num = 0;
		for (num = 0; num < m_lstNKCDiveGameSectorSet.Count; num++)
		{
			if (m_lstNKCDiveGameSectorSet[num].GetActive())
			{
				NKCDiveGameSector sector = m_lstNKCDiveGameSectorSet[num].GetSector(cNKMDiveSlot);
				if (sector != null)
				{
					return sector;
				}
			}
		}
		return null;
	}

	public void SetUIWhenStartBeforeScan(NKMDiveGameData cNKMDiveGameData)
	{
		if (cNKMDiveGameData == null)
		{
			return;
		}
		int num = 0;
		for (num = 0; num < m_lstNKCDiveGameSectorSet.Count; num++)
		{
			if (num < cNKMDiveGameData.Floor.Templet.RandomSetCount + 1)
			{
				NKCUtil.SetGameobjectActive(m_lstNKCDiveGameSectorSet[num], bValue: true);
				if (NKCDiveManager.IsDiveJump() || num + 1 == cNKMDiveGameData.Floor.Templet.RandomSetCount + 1)
				{
					m_lstNKCDiveGameSectorSet[num].SetBoss();
					m_lstNKCDiveGameSectorSet[num].SetActive(bSet: false);
				}
				else
				{
					m_lstNKCDiveGameSectorSet[num].SetRealSize(m_RealSetSize);
					m_lstNKCDiveGameSectorSet[num].SetAllEmpty();
					m_lstNKCDiveGameSectorSet[num].SetActive(bSet: false);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstNKCDiveGameSectorSet[num], bValue: false);
			}
		}
	}

	public void SetUIWhenAddSectorBeforeScan(NKMDiveGameData cNKMDiveGameData)
	{
		if (cNKMDiveGameData == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		for (num = 0; num < m_lstNKCDiveGameSectorSet.Count; num++)
		{
			if (num < cNKMDiveGameData.Floor.Templet.RandomSetCount + 1)
			{
				NKCUtil.SetGameobjectActive(m_lstNKCDiveGameSectorSet[num], bValue: true);
				if (num >= cNKMDiveGameData.Player.PlayerBase.Distance - 1 && num < cNKMDiveGameData.Player.PlayerBase.Distance - 1 + cNKMDiveGameData.Floor.SlotSets.Count - 1)
				{
					if (num == cNKMDiveGameData.Player.PlayerBase.Distance - 1 && num + 1 != cNKMDiveGameData.Floor.Templet.RandomSetCount + 1)
					{
						m_lstNKCDiveGameSectorSet[num].SetRealSize(m_RealSetSize);
						m_lstNKCDiveGameSectorSet[num].SetAllEmpty();
						int num3 = 0;
						num3 = ((cNKMDiveGameData.Player.PlayerBase.State != NKMDivePlayerState.Exploring && cNKMDiveGameData.Player.PlayerBase.State != NKMDivePlayerState.SelectArtifact) ? cNKMDiveGameData.Player.PlayerBase.PrevSlotIndex : cNKMDiveGameData.Player.PlayerBase.SlotIndex);
						NKCDiveGameSector sector = m_lstNKCDiveGameSectorSet[num].GetSector(num3);
						if (sector != null)
						{
							sector.SetUI(cNKMDiveGameData.Floor.SlotSets[num2].Slots[0]);
						}
					}
					else
					{
						m_lstNKCDiveGameSectorSet[num].SetUIByData(cNKMDiveGameData.Floor.SlotSets[num2]);
					}
					m_lstNKCDiveGameSectorSet[num].SetActive(bSet: true);
					num2++;
				}
				else if (NKCDiveManager.IsDiveJump() || num + 1 == cNKMDiveGameData.Floor.Templet.RandomSetCount + 1)
				{
					m_lstNKCDiveGameSectorSet[num].SetBoss();
					m_lstNKCDiveGameSectorSet[num].SetActive(bSet: false);
				}
				else
				{
					m_lstNKCDiveGameSectorSet[num].SetRealSize(m_RealSetSize);
					m_lstNKCDiveGameSectorSet[num].SetAllEmpty();
					m_lstNKCDiveGameSectorSet[num].SetActive(bSet: false);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstNKCDiveGameSectorSet[num], bValue: false);
			}
		}
	}

	public void SetUI(NKMDiveGameData cNKMDiveGameData, bool bShowSpawnAni = false)
	{
		if (cNKMDiveGameData == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		if (cNKMDiveGameData.Player.PlayerBase.Distance == 0)
		{
			for (num = 0; num < m_lstNKCDiveGameSectorSet.Count; num++)
			{
				if (num < cNKMDiveGameData.Floor.Templet.RandomSetCount + 1)
				{
					NKCUtil.SetGameobjectActive(m_lstNKCDiveGameSectorSet[num], bValue: true);
					if (num >= cNKMDiveGameData.Player.PlayerBase.Distance && num < cNKMDiveGameData.Player.PlayerBase.Distance + cNKMDiveGameData.Floor.SlotSets.Count)
					{
						m_lstNKCDiveGameSectorSet[num].SetActive(bSet: true);
						m_lstNKCDiveGameSectorSet[num].SetUIByData(cNKMDiveGameData.Floor.SlotSets[num2], bShowSpawnAni, num == 0);
						num2++;
					}
					else if (NKCDiveManager.IsDiveJump() || num + 1 == cNKMDiveGameData.Floor.Templet.RandomSetCount + 1)
					{
						m_lstNKCDiveGameSectorSet[num].SetBoss();
						m_lstNKCDiveGameSectorSet[num].SetActive(bSet: false);
					}
					else
					{
						m_lstNKCDiveGameSectorSet[num].SetRealSize(m_RealSetSize);
						m_lstNKCDiveGameSectorSet[num].SetAllEmpty();
						m_lstNKCDiveGameSectorSet[num].SetActive(bSet: false);
					}
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstNKCDiveGameSectorSet[num], bValue: false);
				}
			}
			return;
		}
		for (num = 0; num < m_lstNKCDiveGameSectorSet.Count; num++)
		{
			if (num < cNKMDiveGameData.Floor.Templet.RandomSetCount + 1)
			{
				NKCUtil.SetGameobjectActive(m_lstNKCDiveGameSectorSet[num], bValue: true);
				if (num >= cNKMDiveGameData.Player.PlayerBase.Distance - 1 && num < cNKMDiveGameData.Player.PlayerBase.Distance - 1 + cNKMDiveGameData.Floor.SlotSets.Count)
				{
					if (num == cNKMDiveGameData.Player.PlayerBase.Distance - 1 && num + 1 != cNKMDiveGameData.Floor.Templet.RandomSetCount + 1)
					{
						m_lstNKCDiveGameSectorSet[num].SetRealSize(m_RealSetSize);
						m_lstNKCDiveGameSectorSet[num].SetAllEmpty();
						int num3 = 0;
						num3 = ((cNKMDiveGameData.Player.PlayerBase.State != NKMDivePlayerState.Exploring && cNKMDiveGameData.Player.PlayerBase.State != NKMDivePlayerState.SelectArtifact) ? cNKMDiveGameData.Player.PlayerBase.PrevSlotIndex : cNKMDiveGameData.Player.PlayerBase.SlotIndex);
						NKCDiveGameSector sector = m_lstNKCDiveGameSectorSet[num].GetSector(num3);
						if (sector != null)
						{
							sector.SetUI(cNKMDiveGameData.Floor.SlotSets[num2].Slots[0]);
						}
					}
					else
					{
						m_lstNKCDiveGameSectorSet[num].SetUIByData(cNKMDiveGameData.Floor.SlotSets[num2], bShowSpawnAni && num2 == cNKMDiveGameData.Floor.SlotSets.Count - 1);
					}
					if (num == cNKMDiveGameData.Player.PlayerBase.Distance && num + 1 != cNKMDiveGameData.Floor.Templet.RandomSetCount + 1)
					{
						m_lstNKCDiveGameSectorSet[num].SetGrey();
					}
					else
					{
						m_lstNKCDiveGameSectorSet[num].InvalidGrey();
					}
					m_lstNKCDiveGameSectorSet[num].SetActive(bSet: true);
					num2++;
				}
				else if (num + 1 == cNKMDiveGameData.Floor.Templet.RandomSetCount + 1)
				{
					m_lstNKCDiveGameSectorSet[num].SetBoss();
					m_lstNKCDiveGameSectorSet[num].SetActive(bSet: false);
				}
				else
				{
					m_lstNKCDiveGameSectorSet[num].SetRealSize(m_RealSetSize);
					m_lstNKCDiveGameSectorSet[num].SetAllEmpty();
					m_lstNKCDiveGameSectorSet[num].SetActive(bSet: false);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstNKCDiveGameSectorSet[num], bValue: false);
			}
		}
	}

	public void Reset(int depth, int setSize, NKCDiveGameSector.OnClickSector dOnClickSector = null)
	{
		m_bOpenAniForSecondLine = false;
		m_RealDepth = (NKCDiveManager.IsDiveJump() ? depth : (depth + 1));
		m_RealSetSize = setSize;
		int num = 0;
		if (m_lstNKCDiveGameSectorSet.Count < m_RealDepth)
		{
			int num2 = m_RealDepth - m_lstNKCDiveGameSectorSet.Count;
			int count = m_lstNKCDiveGameSectorSet.Count;
			for (num = 0; num < num2; num++)
			{
				NKCDiveGameSectorSet newInstance = NKCDiveGameSectorSet.GetNewInstance(count + num + 1, m_trParentOfSets, dOnClickSector);
				m_lstNKCDiveGameSectorSet.Add(newInstance);
			}
		}
		for (num = 0; num < m_lstNKCDiveGameSectorSet.Count; num++)
		{
			if (num < m_RealDepth)
			{
				NKCUtil.SetGameobjectActive(m_lstNKCDiveGameSectorSet[num], bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstNKCDiveGameSectorSet[num], bValue: false);
			}
		}
	}
}
