using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC.Game.Unit;

public class NKCGameUnitObject : MonoBehaviour
{
	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public List<NKCGameUnitBuffIcon> m_listNKCUnitBuffIcon = new List<NKCGameUnitBuffIcon>();

	[Header("\ufffd\ufffdȯü \ufffd\ufffdũ")]
	public GameObject m_objSummonAlly;

	public GameObject m_objSummonEnemy;

	public void Init()
	{
		foreach (NKCGameUnitBuffIcon item in m_listNKCUnitBuffIcon)
		{
			item.Init();
		}
	}

	public void Unload()
	{
		foreach (NKCGameUnitBuffIcon item in m_listNKCUnitBuffIcon)
		{
			item.Unload();
		}
	}

	public void SetData(NKCGameClient cNKCGame, NKCUnitClient cNKCUnit)
	{
		Init();
		if (cNKCUnit.IsSummonUnit())
		{
			bool flag = cNKCUnit.IsMyTeam();
			NKCUtil.SetGameobjectActive(m_objSummonAlly, flag);
			NKCUtil.SetGameobjectActive(m_objSummonEnemy, !flag);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objSummonAlly, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSummonEnemy, bValue: false);
		}
	}

	private void GageSetBuffIconActive(int index, bool bActive, NKMBuffData cNKMBuffData = null, float fLifeTimeRate = 1f)
	{
		if (index < m_listNKCUnitBuffIcon.Count)
		{
			NKCGameUnitBuffIcon nKCGameUnitBuffIcon = m_listNKCUnitBuffIcon[index];
			if (!(nKCGameUnitBuffIcon == null))
			{
				nKCGameUnitBuffIcon.GageSetBuffIconActive(bActive, cNKMBuffData, fLifeTimeRate);
			}
		}
	}

	public void ProcessBuffIcon(NKMUnit unit)
	{
		int num = 0;
		foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in unit.GetUnitFrameData().m_dicBuffData)
		{
			if (num >= m_listNKCUnitBuffIcon.Count)
			{
				break;
			}
			NKMBuffData value = dicBuffDatum.Value;
			if (value != null)
			{
				if ((value.m_BuffSyncData.m_MasterGameUnitUID == unit.GetUnitDataGame().m_GameUnitUID && !value.m_NKMBuffTemplet.m_AffectMe) || !value.m_NKMBuffTemplet.m_bShowBuffIcon)
				{
					continue;
				}
				if (value.m_fLifeTime == -1f || value.m_NKMBuffTemplet.m_bInfinity || value.m_BuffSyncData.m_bRangeSon)
				{
					GageSetBuffIconActive(num, bActive: true, value);
				}
				else
				{
					float fLifeTimeRate = value.m_fLifeTime / value.GetLifeTimeMax();
					GageSetBuffIconActive(num, bActive: true, value, fLifeTimeRate);
				}
			}
			num++;
		}
		for (int i = num; i < m_listNKCUnitBuffIcon.Count; i++)
		{
			GageSetBuffIconActive(i, bActive: false);
		}
	}
}
