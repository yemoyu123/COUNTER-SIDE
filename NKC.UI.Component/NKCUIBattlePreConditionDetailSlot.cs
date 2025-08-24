using System.Collections.Generic;
using System.Text;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

public class NKCUIBattlePreConditionDetailSlot : MonoBehaviour
{
	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public NKCUIComToggle m_tglSlot;

	public GameObject m_objMainActive;

	public GameObject m_objMainDeactive;

	public Text m_lbMainTextActive;

	public Text m_lbMainTextDeactive;

	[Header("\ufffd\ufffd\ufffd꽽\ufffd\ufffd")]
	public NKCUIBattlePreConditionDetailSubSlot m_pfbSubSlot;

	public RectTransform m_rtSubSlotParent;

	private List<NKCUIBattlePreConditionDetailSubSlot> m_lstSubSlots = new List<NKCUIBattlePreConditionDetailSubSlot>();

	public void SetData(int groupID, List<GameUnitData> lstUnit, NKMUnitData shipData, NKMOperator operatorData, long leaderUID, bool bReset)
	{
		NKCUtil.SetButtonClickDelegate(m_tglSlot, OpenSubList);
		List<NKMPreconditionBCGroupTemplet> preConditionTypeByGroupId = NKMBattleConditionManager.GetPreConditionTypeByGroupId(groupID);
		if (preConditionTypeByGroupId == null || preConditionTypeByGroupId.Count == 0)
		{
			Debug.LogError($"PreConditionGroup {groupID} does not exist!");
			SetMainSlotText("");
			if (m_tglSlot != null)
			{
				m_tglSlot.Lock();
			}
			OpenSubList(value: false);
			return;
		}
		if (preConditionTypeByGroupId.Count == 1)
		{
			if (m_tglSlot != null)
			{
				m_tglSlot.Lock();
			}
			SetMainSlotData(preConditionTypeByGroupId[0], lstUnit, shipData, operatorData, leaderUID);
			OpenSubList(value: false);
			return;
		}
		if (m_tglSlot != null)
		{
			m_tglSlot.UnLock();
		}
		if (bReset)
		{
			OpenSubList(value: true);
		}
		NKMBattleConditionTemplet enabledBCByPreCondition = NKMBattleConditionManager.GetEnabledBCByPreCondition(groupID, NKCScenManager.CurrentUserData().m_InventoryData, lstUnit, shipData, operatorData, leaderUID);
		int num;
		int EnabledTempletID;
		if (enabledBCByPreCondition == null)
		{
			num = 0;
			EnabledTempletID = preConditionTypeByGroupId[0].m_BCondID;
		}
		else
		{
			EnabledTempletID = enabledBCByPreCondition.BattleCondID;
			num = preConditionTypeByGroupId.FindIndex((NKMPreconditionBCGroupTemplet x) => x.m_BCondID == EnabledTempletID);
		}
		SetMainSlotData(preConditionTypeByGroupId[num], lstUnit, shipData, operatorData, leaderUID);
		List<NKMPreconditionBCGroupTemplet> list = new List<NKMPreconditionBCGroupTemplet>(preConditionTypeByGroupId.Count - 1);
		for (int num2 = num + 1; num2 < preConditionTypeByGroupId.Count; num2++)
		{
			list.Add(preConditionTypeByGroupId[num2]);
		}
		for (int num3 = 0; num3 < num; num3++)
		{
			list.Add(preConditionTypeByGroupId[num3]);
		}
		while (m_lstSubSlots.Count < list.Count)
		{
			NKCUIBattlePreConditionDetailSubSlot item = Object.Instantiate(m_pfbSubSlot, m_rtSubSlotParent);
			m_lstSubSlots.Add(item);
		}
		for (int num4 = 0; num4 < m_lstSubSlots.Count; num4++)
		{
			NKCUIBattlePreConditionDetailSubSlot nKCUIBattlePreConditionDetailSubSlot = m_lstSubSlots[num4];
			if (num4 < list.Count)
			{
				NKCUtil.SetGameobjectActive(nKCUIBattlePreConditionDetailSubSlot, bValue: true);
				NKMPreconditionBCGroupTemplet templet = list[num4];
				nKCUIBattlePreConditionDetailSubSlot.SetData(templet, lstUnit, shipData, operatorData, leaderUID);
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUIBattlePreConditionDetailSubSlot, bValue: false);
			}
		}
	}

	private void SetMainSlotText(string text)
	{
		NKCUtil.SetLabelText(m_lbMainTextDeactive, text);
		NKCUtil.SetLabelText(m_lbMainTextActive, text);
	}

	private void SetMainSlotData(NKMPreconditionBCGroupTemplet templet, List<GameUnitData> lstUnit, NKMUnitData shipData, NKMOperator operatorData, long leaderUID)
	{
		if (templet == null || templet.BattleConditionTemplet == null)
		{
			Debug.LogError("NKMPreconditionBCGroupTemplet or BCTemplet null!");
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(templet.BattleConditionTemplet.BattleCondDesc_Translated);
		stringBuilder.Append(GetCurrentConditionValueString(out var bComplete, templet, lstUnit, shipData, operatorData, leaderUID));
		NKCUtil.SetGameobjectActive(m_objMainActive, bComplete);
		NKCUtil.SetGameobjectActive(m_objMainDeactive, !bComplete);
		SetMainSlotText(stringBuilder.ToString());
	}

	public static string GetCurrentConditionValueString(out bool bComplete, NKMPreconditionBCGroupTemplet templet, List<GameUnitData> lstUnit, NKMUnitData shipData, NKMOperator operatorData, long leaderUID)
	{
		if (lstUnit != null)
		{
			NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
			int preconditionCurrentValue = NKMBattleConditionManager.GetPreconditionCurrentValue(templet, inventoryData, lstUnit, shipData, operatorData, leaderUID);
			bComplete = preconditionCurrentValue >= templet.PreConditionValue;
			if (bComplete)
			{
				return $" ({preconditionCurrentValue}/{templet.PreConditionValue})";
			}
			return $" (<color=#ff0000>{preconditionCurrentValue}</color>/{templet.PreConditionValue})";
		}
		bComplete = false;
		return string.Empty;
	}

	private void OpenSubList(bool value)
	{
		if (m_tglSlot != null)
		{
			m_tglSlot.Select(value, bForce: true);
		}
		NKCUtil.SetGameobjectActive(m_rtSubSlotParent, value);
		RectTransform rectTransform = base.transform.parent as RectTransform;
		if (rectTransform != null)
		{
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
		}
	}
}
