using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIHangarShipyardLimitBreak : MonoBehaviour
{
	public Text m_lbCurLimitBreakMaxLevel;

	public Text m_lbNextLimitBreakMaxLevel;

	public Text m_lbModuleUnlockText;

	public NKCUIComStarRank m_StarRank;

	public void Init()
	{
	}

	public void UpdateShipData(NKCUIShipInfoRepair.ShipRepairInfo shipRepairData)
	{
		if (shipRepairData == null || shipRepairData.ShipData == null)
		{
			return;
		}
		if (m_lbCurLimitBreakMaxLevel != null)
		{
			NKCUIComTextUnitLevel nKCUIComTextUnitLevel = m_lbCurLimitBreakMaxLevel as NKCUIComTextUnitLevel;
			if (nKCUIComTextUnitLevel != null)
			{
				nKCUIComTextUnitLevel.SetLevel(shipRepairData.ShipData, 0);
				NKCUtil.SetLabelText(nKCUIComTextUnitLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, nKCUIComTextUnitLevel.text));
			}
			else
			{
				m_lbCurLimitBreakMaxLevel.text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, shipRepairData.ShipData.m_UnitLevel.ToString());
			}
		}
		NKCUtil.SetLabelText(m_lbNextLimitBreakMaxLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, shipRepairData.iNextShipMaxLevel));
		NKCUtil.SetLabelText(m_lbModuleUnlockText, string.Format(NKCUtilString.GET_STRING_SHIP_INFO_01_SHIPYARD_MODULE_STEP_INFO, shipRepairData.ShipData.m_LimitBreakLevel + 1));
		m_StarRank.SetStarRankShip(shipRepairData.ShipData.m_UnitID, shipRepairData.ShipData.m_LimitBreakLevel + 1);
	}
}
