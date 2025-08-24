using System.Collections.Generic;
using System.Text;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

public class NKCUIBattlePreConditionDetailSubSlot : MonoBehaviour
{
	public GameObject m_objON;

	public GameObject m_objOff;

	public Text m_lbDetailON;

	public Text m_lbDetailOff;

	public void SetData(NKMPreconditionBCGroupTemplet templet, List<GameUnitData> lstUnit, NKMUnitData shipData, NKMOperator operatorData, long leaderUID)
	{
		if (templet == null || templet.BattleConditionTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_objOff, bValue: true);
			NKCUtil.SetGameobjectActive(m_objON, bValue: false);
			SetText("");
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(templet.BattleConditionTemplet.BattleCondDesc_Translated);
		stringBuilder.Append(NKCUIBattlePreConditionDetailSlot.GetCurrentConditionValueString(out var bComplete, templet, lstUnit, shipData, operatorData, leaderUID));
		NKCUtil.SetGameobjectActive(m_objON, bComplete);
		NKCUtil.SetGameobjectActive(m_objOff, !bComplete);
		SetText(stringBuilder.ToString());
	}

	private void SetText(string text)
	{
		NKCUtil.SetLabelText(m_lbDetailON, text);
		NKCUtil.SetLabelText(m_lbDetailOff, text);
	}
}
