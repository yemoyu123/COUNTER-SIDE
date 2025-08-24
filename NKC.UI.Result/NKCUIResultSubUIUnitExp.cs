using System.Collections;
using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIResultSubUIUnitExp : NKCUIResultSubUIBase
{
	public enum UNIT_LOYALTY
	{
		None,
		Up,
		Down
	}

	public class UnitLevelupUIData
	{
		public NKMUnitData m_UnitData;

		public int m_iLevelOld;

		public int m_iExpOld;

		public int m_iLevelNew;

		public int m_iExpNew;

		public int m_iTotalExpGain;

		public bool m_bIsLeader;

		public UNIT_LOYALTY m_loyalty;
	}

	public RectTransform m_rtSlotsNormalRoot;

	public RectTransform m_rtSlotsAdditionalRoot;

	public List<NKCUIResultUnitSlot> m_lstUnitSlotList;

	public Text m_lbBonusExp;

	public NKCUIResultUnitSlot m_AdditionalSlot;

	public Sprite m_spSlotBGN;

	public Sprite m_spSlotBGR;

	public Sprite m_spSlotBGSR;

	public Sprite m_spSlotBGSSR;

	public GameObject m_DUNGEON_RESULT_EXP;

	public CanvasGroup m_cgLayout;

	private List<UnitLevelupUIData> m_lstUnitLevelupData;

	private UnitLevelupUIData m_AdditionalUnitLevelupData;

	public void SetData(List<UnitLevelupUIData> lstUnitLevelupData, int expBonusRate = 0, UnitLevelupUIData AdditionalLevelupData = null, bool bIgnoreAutoClose = false)
	{
		if (AdditionalLevelupData == null && (lstUnitLevelupData == null || lstUnitLevelupData.Count == 0))
		{
			base.ProcessRequired = false;
			return;
		}
		base.ProcessRequired = true;
		m_bIgnoreAutoClose = bIgnoreAutoClose;
		m_lstUnitLevelupData = new List<UnitLevelupUIData>(lstUnitLevelupData);
		if (AdditionalLevelupData != null)
		{
			m_lstUnitLevelupData.RemoveAll((UnitLevelupUIData x) => x.m_UnitData.m_UnitUID == AdditionalLevelupData.m_UnitData.m_UnitUID);
		}
		m_AdditionalUnitLevelupData = AdditionalLevelupData;
		if (AdditionalLevelupData != null)
		{
			if (m_lstUnitLevelupData.Count > 0)
			{
				NKCUtil.SetGameobjectActive(m_AdditionalSlot, bValue: true);
				NKCUtil.SetGameobjectActive(m_rtSlotsAdditionalRoot, bValue: true);
				NKCUtil.SetGameobjectActive(m_rtSlotsNormalRoot, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_AdditionalSlot, bValue: true);
				NKCUtil.SetGameobjectActive(m_rtSlotsAdditionalRoot, bValue: true);
				NKCUtil.SetGameobjectActive(m_rtSlotsNormalRoot, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_AdditionalSlot, bValue: false);
			NKCUtil.SetGameobjectActive(m_rtSlotsAdditionalRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_rtSlotsNormalRoot, bValue: true);
		}
		for (int num = 0; num < m_lstUnitSlotList.Count; num++)
		{
			NKCUIResultUnitSlot nKCUIResultUnitSlot = m_lstUnitSlotList[num];
			if (m_lstUnitLevelupData.Count <= num)
			{
				NKCUtil.SetGameobjectActive(nKCUIResultUnitSlot, bValue: false);
				continue;
			}
			UnitLevelupUIData unitLevelupUIData = m_lstUnitLevelupData[num];
			if (unitLevelupUIData.m_UnitData == null)
			{
				NKCUtil.SetGameobjectActive(nKCUIResultUnitSlot, bValue: false);
				continue;
			}
			if (NKMUnitManager.GetUnitTempletBase(unitLevelupUIData.m_UnitData.m_UnitID) == null)
			{
				NKCUtil.SetGameobjectActive(nKCUIResultUnitSlot, bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(nKCUIResultUnitSlot, bValue: true);
			nKCUIResultUnitSlot.SetData(unitLevelupUIData, m_spSlotBGN, m_spSlotBGR, m_spSlotBGSR, m_spSlotBGSSR);
			nKCUIResultUnitSlot.SetLeader(unitLevelupUIData.m_bIsLeader);
		}
		if (AdditionalLevelupData != null)
		{
			NKCUtil.SetGameobjectActive(m_AdditionalSlot, bValue: true);
			m_AdditionalSlot.SetData(AdditionalLevelupData, m_spSlotBGN, m_spSlotBGR, m_spSlotBGSR, m_spSlotBGSSR);
			m_AdditionalSlot.SetLeader(bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_lbBonusExp, expBonusRate > 0);
		if (expBonusRate > 0)
		{
			m_lbBonusExp.text = string.Format(NKCUtilString.GET_STRING_RESULT_BONUS_EXP, expBonusRate);
		}
		NKCUtil.SetGameobjectActive(m_DUNGEON_RESULT_EXP, bValue: false);
	}

	protected override IEnumerator InnerProcess(bool bAutoSkip)
	{
		yield return null;
		while (m_cgLayout.alpha < 1f)
		{
			yield return null;
		}
		for (int i = 0; i < m_lstUnitLevelupData.Count && m_lstUnitSlotList.Count > i; i++)
		{
			UnitLevelupUIData unitLevelupUIData = m_lstUnitLevelupData[i];
			NKCUIResultUnitSlot nKCUIResultUnitSlot = m_lstUnitSlotList[i];
			if (unitLevelupUIData.m_UnitData == null)
			{
				NKCUtil.SetGameobjectActive(nKCUIResultUnitSlot, bValue: false);
			}
			else
			{
				nKCUIResultUnitSlot.StartExpProcess(unitLevelupUIData.m_UnitData.m_UnitID, unitLevelupUIData.m_iLevelOld, unitLevelupUIData.m_iExpOld, unitLevelupUIData.m_iLevelNew, unitLevelupUIData.m_iExpNew, unitLevelupUIData.m_iTotalExpGain);
			}
		}
		if (m_AdditionalUnitLevelupData != null)
		{
			m_AdditionalSlot.StartExpProcess(m_AdditionalUnitLevelupData.m_UnitData.m_UnitID, m_AdditionalUnitLevelupData.m_iLevelOld, m_AdditionalUnitLevelupData.m_iExpOld, m_AdditionalUnitLevelupData.m_iLevelNew, m_AdditionalUnitLevelupData.m_iExpNew, m_AdditionalUnitLevelupData.m_iTotalExpGain);
		}
		while (!IsProcessFinished())
		{
			yield return null;
		}
		FinishProcess();
		yield return null;
	}

	public override bool IsProcessFinished()
	{
		bool num = !m_lstUnitSlotList.Exists((NKCUIResultUnitSlot x) => x.gameObject.activeInHierarchy && !x.ProgressFinished);
		bool flag = !m_AdditionalSlot.gameObject.activeInHierarchy || m_AdditionalSlot.ProgressFinished;
		return num && flag;
	}

	public override void FinishProcess()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		foreach (NKCUIResultUnitSlot lstUnitSlot in m_lstUnitSlotList)
		{
			lstUnitSlot.Finish();
		}
		m_AdditionalSlot.Finish();
		m_AdditionalUnitLevelupData = null;
		m_lstUnitLevelupData.Clear();
	}
}
