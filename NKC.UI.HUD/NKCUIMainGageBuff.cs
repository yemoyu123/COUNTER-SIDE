using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC.UI.HUD;

public class NKCUIMainGageBuff : MonoBehaviour
{
	public List<NKCUIMainGageBuffCell> m_lstCell;

	public void InitUI()
	{
		for (int i = 0; i < m_lstCell.Count; i++)
		{
			m_lstCell[i].InitUI();
		}
	}

	public void SetUnit(NKCUnitClient cUnit)
	{
		int num = 0;
		foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in cUnit.GetUnitFrameData().m_dicBuffData)
		{
			NKMBuffData value = dicBuffDatum.Value;
			if (value != null)
			{
				if (value.m_NKMBuffTemplet == null || (value.m_BuffSyncData.m_MasterGameUnitUID == cUnit.GetUnitDataGame().m_GameUnitUID && !value.m_NKMBuffTemplet.m_AffectMe) || !value.m_NKMBuffTemplet.m_bShowBuffIcon)
				{
					continue;
				}
				if (value.m_fLifeTime == -1f || value.m_NKMBuffTemplet.m_bInfinity || value.m_BuffSyncData.m_bRangeSon)
				{
					GageSetBuffIconActive(num, bActive: true, value.m_BuffSyncData.m_OverlapCount, value.m_NKMBuffTemplet);
				}
				else
				{
					GageSetBuffIconActive(num, bActive: true, value.m_BuffSyncData.m_OverlapCount, value.m_NKMBuffTemplet, value.m_fLifeTime / value.GetLifeTimeMax());
				}
			}
			num++;
			if (num >= 6)
			{
				break;
			}
		}
		for (int i = num; i < 6; i++)
		{
			GageSetBuffIconActive(i, bActive: false, 0);
		}
	}

	private void GageSetBuffIconActive(int index, bool bActive, int overlapCount, NKMBuffTemplet cNKMBuffTemplet = null, float fLifeTimeRate = 1f)
	{
		if (cNKMBuffTemplet != null && !cNKMBuffTemplet.m_bShowBuffIcon)
		{
			bActive = false;
		}
		NKCUtil.SetGameobjectActive(m_lstCell[index], bActive);
		if (bActive)
		{
			m_lstCell[index].SetData(cNKMBuffTemplet, fLifeTimeRate, overlapCount);
		}
	}
}
