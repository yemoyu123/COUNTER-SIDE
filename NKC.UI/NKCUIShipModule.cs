using System.Collections.Generic;
using System.Text;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIShipModule : MonoBehaviour
{
	[Header("\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objNoModule;

	public GameObject m_objLockedModule;

	public GameObject m_objEnabledModule;

	public List<GameObject> m_lstModuleStep = new List<GameObject>();

	public Text m_lbModuleStep;

	public NKCUIComToggle m_tglSocket_01;

	public NKCUIComToggle m_tglSocket_02;

	public ScrollRect m_srSocketOptions;

	public Text m_lbSocketOptions;

	private bool m_bDoOnce;

	private NKMUnitData m_curShipData;

	private void DoOnce()
	{
		if (!m_bDoOnce)
		{
			NKCUtil.SetToggleValueChangedDelegate(m_tglSocket_01, OnValueChangedSocket_01);
			NKCUtil.SetToggleValueChangedDelegate(m_tglSocket_02, OnValueChangedSocket_02);
		}
	}

	public void SetData(NKMUnitData shipData)
	{
		DoOnce();
		m_curShipData = shipData;
		NKCUtil.SetGameobjectActive(m_objEnabledModule, shipData.m_LimitBreakLevel > 0);
		NKMShipLimitBreakTemplet shipLimitBreakTemplet = NKMShipManager.GetShipLimitBreakTemplet(NKMShipManager.GetMaxLevelShipID(shipData.m_UnitID), 1);
		NKCUtil.SetGameobjectActive(m_objNoModule, shipLimitBreakTemplet == null);
		NKCUtil.SetGameobjectActive(m_objLockedModule, shipData.m_LimitBreakLevel == 0 && shipLimitBreakTemplet != null);
		if (m_objEnabledModule != null && m_objEnabledModule.activeSelf)
		{
			for (int i = 0; i < m_lstModuleStep.Count; i++)
			{
				NKCUtil.SetGameobjectActive(m_lstModuleStep[i], i < shipData.m_LimitBreakLevel);
			}
			NKCUtil.SetLabelText(m_lbModuleStep, string.Format(NKCUtilString.GET_STRING_SHIP_INFO_MODULE_STEP_TEXT, shipData.ShipCommandModule.Count));
			m_tglSocket_01.Select(bSelect: true, bForce: true, bImmediate: true);
			ShowTotalSocketOptions(0);
		}
	}

	private void OnValueChangedSocket_01(bool bValue)
	{
		if (bValue)
		{
			ShowTotalSocketOptions(0);
		}
	}

	private void OnValueChangedSocket_02(bool bValue)
	{
		if (bValue)
		{
			ShowTotalSocketOptions(1);
		}
	}

	private void ShowTotalSocketOptions(int socketIndex)
	{
		List<NKMShipCmdSlot> lstSocket = new List<NKMShipCmdSlot>();
		for (int i = 0; i < m_curShipData.ShipCommandModule.Count; i++)
		{
			if (m_curShipData.ShipCommandModule[i] == null || m_curShipData.ShipCommandModule[i].slots == null)
			{
				continue;
			}
			for (int j = 0; j < m_curShipData.ShipCommandModule[i].slots.Length; j++)
			{
				if (socketIndex == j)
				{
					NKMShipCmdSlot nKMShipCmdSlot = m_curShipData.ShipCommandModule[i].slots[j];
					if (nKMShipCmdSlot != null && nKMShipCmdSlot.statType != NKM_STAT_TYPE.NST_RANDOM)
					{
						AddSameBuff(ref lstSocket, nKMShipCmdSlot);
					}
				}
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int k = 0; k < lstSocket.Count; k++)
		{
			stringBuilder.AppendLine(NKCUtilString.GetSlotOptionString(lstSocket[k]));
		}
		NKCUtil.SetLabelText(m_lbSocketOptions, stringBuilder.ToString());
		m_srSocketOptions.normalizedPosition = Vector2.zero;
	}

	private void AddSameBuff(ref List<NKMShipCmdSlot> lstSocket, NKMShipCmdSlot targetSocket)
	{
		bool flag = false;
		for (int i = 0; i < lstSocket.Count; i++)
		{
			NKMShipCmdSlot nKMShipCmdSlot = lstSocket[i];
			if (nKMShipCmdSlot.statType == targetSocket.statType && nKMShipCmdSlot.targetStyleType.SetEquals(targetSocket.targetStyleType) && nKMShipCmdSlot.targetRoleType.SetEquals(targetSocket.targetRoleType))
			{
				flag = true;
				nKMShipCmdSlot.statValue += targetSocket.statValue;
				break;
			}
		}
		if (!flag)
		{
			NKMShipCmdSlot item = new NKMShipCmdSlot(targetSocket.targetStyleType, targetSocket.targetRoleType, targetSocket.statType, targetSocket.statValue, targetSocket.isLock);
			lstSocket.Add(item);
		}
	}
}
