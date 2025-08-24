using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIBattleConditionDetailSlot : MonoBehaviour
{
	public Text m_lbIdx;

	public Image m_ImgBCICon;

	public Text m_lbBCName;

	public Text m_txtBCDesc;

	[Header("팀업")]
	public NKCUIComToggle m_tglTeamUp;

	public GameObject m_objBCTeamUp;

	public RectTransform m_rtBCTeamUpParent;

	[Header("unit display id range")]
	public int m_iMinID = 1001;

	public int m_iMaxID = 9999;

	private List<NKCUISlot> m_lstTeamUpUnits = new List<NKCUISlot>();

	public void SetData(int idx, NKMBattleConditionTemplet battleTemplet, bool bAddTeamUpUnit = true)
	{
		if (battleTemplet == null)
		{
			return;
		}
		NKCUtil.SetButtonClickDelegate(m_tglTeamUp, ToggleTeamup);
		ToggleTeamup(value: false);
		string msg = (idx + 1).ToString("00");
		NKCUtil.SetLabelText(m_lbIdx, msg);
		NKCUtil.SetLabelKey(m_lbBCName, battleTemplet.BattleCondName);
		NKCUtil.SetLabelText(m_txtBCDesc, battleTemplet.BattleCondDesc_Translated);
		NKCUtil.SetImageSprite(m_ImgBCICon, NKCUtil.GetSpriteBattleConditionICon(battleTemplet));
		if (!bAddTeamUpUnit)
		{
			NKCUtil.SetGameobjectActive(m_tglTeamUp, bValue: false);
			return;
		}
		List<int> list = new List<int>();
		List<string> list2 = new List<string>();
		foreach (string item in battleTemplet.AffectTeamUpID)
		{
			if (!list2.Contains(item))
			{
				list2.Add(item);
			}
		}
		ClearTeamupSlot();
		if (list2.Count > 0)
		{
			foreach (string item2 in list2)
			{
				foreach (NKMUnitTempletBase item3 in NKMUnitManager.GetListTeamUPUnitTempletBase(item2))
				{
					if (item3.PickupEnableByTag && item3.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && item3.m_UnitID >= m_iMinID && item3.m_UnitID <= m_iMaxID && (item3.m_ShipGroupID == 0 || item3.m_ShipGroupID == item3.m_UnitID) && !battleTemplet.hashIgnoreUnitID.Contains(item3.m_UnitID) && !list.Contains(item3.m_UnitID))
					{
						list.Add(item3.m_UnitID);
					}
				}
			}
		}
		foreach (int item4 in battleTemplet.hashAffectUnitID)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(item4);
			if (unitTempletBase.PickupEnableByTag && unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && unitTempletBase.m_UnitID >= m_iMinID && unitTempletBase.m_UnitID <= m_iMaxID && (unitTempletBase.m_ShipGroupID == 0 || unitTempletBase.m_ShipGroupID == unitTempletBase.m_UnitID) && !battleTemplet.hashIgnoreUnitID.Contains(item4) && !list.Contains(unitTempletBase.m_UnitID))
			{
				list.Add(unitTempletBase.m_UnitID);
			}
		}
		bool flag = list.Count > 0;
		if (flag)
		{
			foreach (int item5 in list)
			{
				NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_rtBCTeamUpParent);
				if (null != newInstance)
				{
					newInstance.transform.localPosition = Vector3.zero;
					newInstance.transform.localScale = Vector3.one;
					NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(NKM_REWARD_TYPE.RT_UNIT, item5, 1);
					NKCUtil.SetGameobjectActive(newInstance.gameObject, bValue: true);
					newInstance.SetData(data);
					m_lstTeamUpUnits.Add(newInstance);
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_tglTeamUp, flag);
	}

	public void ClearTeamupSlot()
	{
		for (int i = 0; i < m_lstTeamUpUnits.Count; i++)
		{
			if (!(null == m_lstTeamUpUnits[i]))
			{
				Object.Destroy(m_lstTeamUpUnits[i]);
				m_lstTeamUpUnits[i] = null;
			}
		}
		m_lstTeamUpUnits.Clear();
	}

	private void ToggleTeamup(bool value)
	{
		if (m_tglTeamUp != null)
		{
			m_tglTeamUp.Select(value, bForce: true);
		}
		NKCUtil.SetGameobjectActive(m_objBCTeamUp, value);
		RectTransform rectTransform = base.transform.parent as RectTransform;
		if (rectTransform != null)
		{
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
		}
	}
}
