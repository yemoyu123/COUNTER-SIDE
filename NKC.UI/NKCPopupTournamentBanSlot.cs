using System.Collections.Generic;
using ClientPacket.Common;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupTournamentBanSlot : MonoBehaviour
{
	public GameObject m_objTitle;

	public NKCComTMPUIText m_lbTitle;

	public GameObject m_objUnit;

	public Transform m_trUnitParent;

	public NKCUIUnitSelectListSlot m_pfbUnitSlot;

	public GameObject m_objShip;

	public Transform m_trShipParent;

	public NKCUIShipSelectListSlot m_pfbShipSlot;

	private Stack<NKCUIUnitSelectListSlot> m_stkUnitSlot = new Stack<NKCUIUnitSelectListSlot>();

	private List<NKCUIUnitSelectListSlot> m_lstUnitSlot = new List<NKCUIUnitSelectListSlot>();

	private Stack<NKCUIShipSelectListSlot> m_stkShipSlot = new Stack<NKCUIShipSelectListSlot>();

	private List<NKCUIShipSelectListSlot> m_lstShipSlot = new List<NKCUIShipSelectListSlot>();

	public void SetData(NKM_UNIT_TYPE unitType, List<int> lstIDs, NKCBanManager.BAN_DATA_TYPE banDataType, NKMTournamentCountryCode countryCode)
	{
		NKCUtil.SetGameobjectActive(m_objUnit, unitType == NKM_UNIT_TYPE.NUT_NORMAL);
		NKCUtil.SetGameobjectActive(m_objShip, unitType == NKM_UNIT_TYPE.NUT_SHIP);
		NKCUtil.SetGameobjectActive(m_objTitle, countryCode != NKMTournamentCountryCode.None);
		if (countryCode != NKMTournamentCountryCode.None)
		{
			NKCUtil.SetLabelText(m_lbTitle, countryCode.ToString());
		}
		for (int i = 0; i < m_lstUnitSlot.Count; i++)
		{
			NKCUIUnitSelectListSlot nKCUIUnitSelectListSlot = m_lstUnitSlot[i];
			if (nKCUIUnitSelectListSlot != null)
			{
				m_stkUnitSlot.Push(nKCUIUnitSelectListSlot);
				NKCUtil.SetGameobjectActive(nKCUIUnitSelectListSlot, bValue: false);
			}
		}
		m_lstUnitSlot.Clear();
		for (int j = 0; j < m_lstShipSlot.Count; j++)
		{
			NKCUIShipSelectListSlot nKCUIShipSelectListSlot = m_lstShipSlot[j];
			if (nKCUIShipSelectListSlot != null)
			{
				m_stkShipSlot.Push(nKCUIShipSelectListSlot);
				NKCUtil.SetGameobjectActive(nKCUIShipSelectListSlot, bValue: false);
			}
		}
		m_lstShipSlot.Clear();
		switch (unitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
		{
			for (int l = 0; l < lstIDs.Count; l++)
			{
				NKCUIUnitSelectListSlot component2 = GetSlot(unitType).GetComponent<NKCUIUnitSelectListSlot>();
				if (component2 != null)
				{
					NKCUtil.SetGameobjectActive(component2.gameObject, bValue: true);
					m_lstUnitSlot.Add(component2);
					component2.transform.SetAsLastSibling();
					NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(lstIDs[l]);
					if (unitTempletBase2 != null)
					{
						component2.SetEnableShowBan(bSet: false);
						component2.SetBanDataType(banDataType);
						component2.SetDataForBan(unitTempletBase2, bEnableLayoutElement: true, null, bUp: false, bSetOriginalCost: true);
						component2.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
					}
				}
			}
			break;
		}
		case NKM_UNIT_TYPE.NUT_SHIP:
		{
			for (int k = 0; k < lstIDs.Count; k++)
			{
				NKCUIShipSelectListSlot component = GetSlot(unitType).GetComponent<NKCUIShipSelectListSlot>();
				if (component != null)
				{
					NKCUtil.SetGameobjectActive(component.gameObject, bValue: true);
					m_lstShipSlot.Add(component);
					component.transform.SetAsLastSibling();
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lstIDs[k]);
					if (unitTempletBase != null)
					{
						component.SetEnableShowBan(bSet: false);
						component.SetBanDataType(banDataType);
						component.SetDataForBan(unitTempletBase, bEnableLayoutElement: true, null, bUp: false, bSetOriginalCost: true);
					}
				}
			}
			break;
		}
		}
	}

	private NKCUIUnitSelectListSlotBase GetSlot(NKM_UNIT_TYPE unitType)
	{
		switch (unitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			if (m_stkUnitSlot.Count > 0)
			{
				return m_stkUnitSlot.Pop();
			}
			return Object.Instantiate(m_pfbUnitSlot, m_trUnitParent);
		case NKM_UNIT_TYPE.NUT_SHIP:
			if (m_stkShipSlot.Count > 0)
			{
				return m_stkShipSlot.Pop();
			}
			return Object.Instantiate(m_pfbShipSlot, m_trShipParent);
		default:
			return null;
		}
	}
}
