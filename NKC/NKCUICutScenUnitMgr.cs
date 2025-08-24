using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class NKCUICutScenUnitMgr : MonoBehaviour
{
	public NKCCutUnit m_nkcCutUnitLeft;

	public NKCCutUnit m_nkcCutUnitRight;

	public NKCCutUnit m_nkcCutUnitCenter;

	private NKCCutUnit[] m_arrCutUnit = new NKCCutUnit[3];

	private CutUnitPosType m_currentPos;

	private static NKCUICutScenUnitMgr m_scNKCUICutScenUnitMgr;

	private bool m_bPause;

	public static NKCUICutScenUnitMgr GetCutScenUnitMgr()
	{
		return m_scNKCUICutScenUnitMgr;
	}

	public void SetPause(bool bPause)
	{
		m_bPause = bPause;
	}

	public static void InitUI(GameObject goNKM_UI_CUTSCEN_PLAYER)
	{
		if (!(m_scNKCUICutScenUnitMgr != null))
		{
			m_scNKCUICutScenUnitMgr = goNKM_UI_CUTSCEN_PLAYER.transform.Find("NKM_UI_CUTSCEN_PLAYER_UNIT_MGR").gameObject.GetComponent<NKCUICutScenUnitMgr>();
			m_scNKCUICutScenUnitMgr.InitCutUnit();
			m_scNKCUICutScenUnitMgr.Close();
		}
	}

	private void InitCutUnit()
	{
		m_arrCutUnit[0] = m_nkcCutUnitLeft;
		m_arrCutUnit[1] = m_nkcCutUnitRight;
		m_arrCutUnit[2] = m_nkcCutUnitCenter;
		for (int i = 0; i < m_arrCutUnit.Length; i++)
		{
			m_arrCutUnit[i].InitCutUnit();
		}
	}

	public void Reset()
	{
		SetPause(bPause: false);
	}

	public bool IsFinished()
	{
		for (int i = 0; i < m_arrCutUnit.Length; i++)
		{
			if (!m_arrCutUnit[i].IsFinished())
			{
				return false;
			}
		}
		return true;
	}

	public bool IsExistUnitInScen()
	{
		for (int i = 0; i < m_arrCutUnit.Length; i++)
		{
			if (m_arrCutUnit[i].GoUnit.activeSelf)
			{
				return true;
			}
		}
		return false;
	}

	private void Update()
	{
		if (!m_bPause)
		{
			if (m_currentPos != CutUnitPosType.NONE)
			{
				m_arrCutUnit[(int)m_currentPos].UpdateUnitPos();
			}
			for (int i = 0; i < m_arrCutUnit.Length; i++)
			{
				m_arrCutUnit[i].ManualUpdate();
			}
		}
	}

	public void Finish()
	{
		for (int i = 0; i < m_arrCutUnit.Length; i++)
		{
			m_arrCutUnit[i].FinishUnit();
		}
	}

	public void StopCrash()
	{
		for (int i = 0; i < m_arrCutUnit.Length; i++)
		{
			m_arrCutUnit[i].StopUnitCrash();
		}
	}

	public RectTransform GetUnitRectTransform(CutUnitPosType posType)
	{
		return m_arrCutUnit[(int)posType].RectTransform;
	}

	public NKCUICharacterView GetUnitCharacterView(CutUnitPosType posType)
	{
		return m_arrCutUnit[(int)posType].CharacterView;
	}

	public NKCCutTemplet GetUnitCutTemplet(CutUnitPosType posType)
	{
		return m_arrCutUnit[(int)posType].NKCCutTemplet;
	}

	public void DarkenOtherUnitColor(CutUnitPosType posType)
	{
		for (int i = 0; i < m_arrCutUnit.Length; i++)
		{
			if (m_arrCutUnit[i].NKCCutTemplet != null && posType != (CutUnitPosType)i)
			{
				m_arrCutUnit[i].DarkenUnit();
			}
		}
	}

	public void ClearUnitByPos(NKCCutTemplet cNKCCutTemplet, CutUnitPosType posType)
	{
		if (!(m_arrCutUnit[(int)posType] == null))
		{
			m_arrCutUnit[(int)posType].ClearUnit(cNKCCutTemplet, bNone: true, bCur: false);
		}
	}

	public void SetUnit(NKCCutScenCharTemplet cNKCCutScenCharTemplet, NKCCutTemplet cNKCCutTemplet, Dictionary<string, int> dicSkin)
	{
		m_currentPos = cNKCCutTemplet.CutUnitPos;
		if (m_currentPos != CutUnitPosType.NONE)
		{
			m_arrCutUnit[(int)m_currentPos].SetUnit(cNKCCutScenCharTemplet, cNKCCutTemplet, dicSkin);
		}
	}

	public void ClearUnit(NKCCutTemplet cNKCCutTemplet)
	{
		m_currentPos = cNKCCutTemplet.CutUnitPos;
		bool bNone = false;
		if (m_currentPos == CutUnitPosType.NONE)
		{
			bNone = true;
		}
		for (int i = 0; i < m_arrCutUnit.Length; i++)
		{
			m_arrCutUnit[i].ClearUnit(cNKCCutTemplet, bNone, m_currentPos == (CutUnitPosType)i);
		}
	}

	public void Open()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
	}

	public void Close()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		for (int i = 0; i < m_arrCutUnit.Length; i++)
		{
			m_arrCutUnit[i].Close();
		}
		m_currentPos = CutUnitPosType.NONE;
	}
}
