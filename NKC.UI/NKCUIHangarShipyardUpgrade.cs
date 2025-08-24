using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIHangarShipyardUpgrade : MonoBehaviour
{
	public RectTransform m_rtNKM_UI_SHIPYARD_Upgrade_INFO_STAR_EFFECT;

	public Text m_txt_CurrentMaxLevel;

	public Text m_txt_NextMaxLevel;

	public List<GameObject> m_lstCurStar;

	public List<GameObject> m_lstNextStar;

	public Text m_lbCurrentShipStartCost;

	public Text m_lbNextShipStartCost;

	public void Init()
	{
	}

	public void UpdateShipData(NKCUIShipInfoRepair.ShipRepairInfo shipRepairData)
	{
		if (shipRepairData != null && shipRepairData.ShipData != null)
		{
			NKCUtil.SetLabelText(m_txt_CurrentMaxLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, shipRepairData.iCurShipMaxLevel));
			NKCUtil.SetLabelText(m_txt_NextMaxLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, shipRepairData.iNextShipMaxLevel));
			NKCUtil.SetLabelText(m_lbCurrentShipStartCost, NKCUtil.GetShipStartCost(shipRepairData.iCurStar).ToString());
			NKCUtil.SetLabelText(m_lbNextShipStartCost, NKCUtil.GetShipStartCost(shipRepairData.iNextStar).ToString());
			NKCUtil.SetStarRank(m_lstCurStar, shipRepairData.iCurStar, 6);
			NKCUtil.SetStarRank(m_lstNextStar, shipRepairData.iNextStar, 6);
			if (m_rtNKM_UI_SHIPYARD_Upgrade_INFO_STAR_EFFECT != null && shipRepairData.iNextStar > 0)
			{
				m_rtNKM_UI_SHIPYARD_Upgrade_INFO_STAR_EFFECT.localPosition = m_lstNextStar[shipRepairData.iNextStar - 1].GetComponent<RectTransform>().localPosition;
			}
		}
	}
}
