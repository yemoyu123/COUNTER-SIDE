using Cs.Logging;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupShipCommandModuleSlotSocket : MonoBehaviour
{
	public enum SHIP_MODULE_SOCKET_STATE
	{
		UNLOCK,
		UNLOCK_TO_LOCK,
		LOCK,
		LOCK_TO_UNLOCK
	}

	public Text m_lbSocketNum;

	public GameObject m_objCurrent;

	public Text m_lbOptionCurrent;

	public GameObject m_objCurrentMax;

	public Text m_lbOptionCurrentMax;

	public GameObject m_objAfter;

	public Text m_lbOptionAfter;

	public GameObject m_objAfterMax;

	public Text m_lbOptionAfterMax;

	public GameObject m_objFocusLine;

	public Animator m_Ani;

	public NKCUIComStateButton m_btnLock;

	public GameObject m_objLocked;

	public GameObject m_objOpen;

	public GameObject m_objRerollFx;

	public GameObject m_objMaxFx;

	private int m_moduleIndex;

	private int m_socketIndex;

	private bool m_bLockChanged;

	private NKMShipCmdSlot m_curSlotData;

	private NKMShipCmdSlot m_nextSlotData;

	private int m_StatGroupId;

	public void Init()
	{
		m_btnLock.PointerClick.RemoveAllListeners();
		m_btnLock.PointerClick.AddListener(OnClickLock);
	}

	public void CloseInternal()
	{
		m_moduleIndex = -1;
		m_socketIndex = -1;
		m_bLockChanged = false;
	}

	public void SetData(int moduleIndex, int socketIndex, NKMShipCmdSlot curSlotData, NKMShipCmdSlot nextSlotData = null)
	{
		m_moduleIndex = moduleIndex;
		m_socketIndex = socketIndex;
		m_curSlotData = curSlotData;
		m_nextSlotData = nextSlotData;
		NKCUtil.SetLabelText(m_lbSocketNum, (socketIndex + 1).ToString());
		NKCUtil.SetGameobjectActive(m_objOpen, curSlotData == null);
		if (curSlotData != null)
		{
			NKCUtil.SetGameobjectActive(m_objFocusLine, !curSlotData.isLock);
			string format = "[{0}] {1}";
			NKMUnitTempletBase unitTempletBase = NKCPopupShipCommandModule.Instance.GetShipData().GetUnitTempletBase();
			if (unitTempletBase != null)
			{
				NKMShipCommandModuleTemplet nKMShipCommandModuleTemplet = NKMShipManager.GetNKMShipCommandModuleTemplet(unitTempletBase.m_NKM_UNIT_STYLE_TYPE, unitTempletBase.m_NKM_UNIT_GRADE, moduleIndex + 1);
				if (nKMShipCommandModuleTemplet != null)
				{
					if (socketIndex == 0)
					{
						m_StatGroupId = nKMShipCommandModuleTemplet.Slot1Id;
					}
					else
					{
						m_StatGroupId = nKMShipCommandModuleTemplet.Slot2Id;
					}
				}
			}
			bool flag = NKMShipManager.IsMaxStat(m_StatGroupId, m_curSlotData);
			NKCUtil.SetGameobjectActive(m_objCurrent, !flag);
			NKCUtil.SetGameobjectActive(m_objCurrentMax, flag);
			NKCUtil.SetLabelText(m_lbOptionCurrent, NKCUtilString.GetSlotOptionString(curSlotData, format));
			NKCUtil.SetLabelText(m_lbOptionCurrentMax, NKCUtilString.GetSlotOptionString(curSlotData, format));
			bool flag2 = NKMShipManager.IsMaxStat(m_StatGroupId, nextSlotData);
			if (nextSlotData != null)
			{
				NKCUtil.SetGameobjectActive(m_objAfter, !flag2);
				NKCUtil.SetGameobjectActive(m_objAfterMax, flag2);
				NKCUtil.SetLabelText(m_lbOptionAfter, NKCUtilString.GetSlotOptionString(nextSlotData, format));
				NKCUtil.SetLabelText(m_lbOptionAfterMax, NKCUtilString.GetSlotOptionString(nextSlotData, format));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objAfter, bValue: true);
				NKCUtil.SetGameobjectActive(m_objAfterMax, bValue: false);
				NKCUtil.SetLabelText(m_lbOptionAfter, "-");
				NKCUtil.SetLabelText(m_lbOptionAfterMax, "-");
			}
			SetLockState(curSlotData.isLock, m_bLockChanged);
			m_bLockChanged = false;
			m_btnLock.Select(curSlotData.isLock, bForce: true, bImmediate: true);
			NKCUtil.SetGameobjectActive(m_objMaxFx, flag || flag2);
		}
		else
		{
			m_btnLock.Select(bSelect: false, bForce: true, bImmediate: true);
			NKCUtil.SetGameobjectActive(m_objFocusLine, bValue: false);
			NKCUtil.SetGameobjectActive(m_objCurrent, bValue: true);
			NKCUtil.SetGameobjectActive(m_objCurrentMax, bValue: false);
			NKCUtil.SetGameobjectActive(m_objAfter, bValue: false);
			NKCUtil.SetGameobjectActive(m_objAfterMax, bValue: false);
			NKCUtil.SetLabelText(m_lbOptionCurrent, string.Empty);
			NKCUtil.SetLabelText(m_lbOptionCurrentMax, string.Empty);
			NKCUtil.SetLabelText(m_lbOptionAfter, string.Empty);
			NKCUtil.SetLabelText(m_lbOptionAfterMax, string.Empty);
		}
	}

	private void OnClickLock()
	{
		NKMUnitData shipData = NKCPopupShipCommandModule.Instance.GetShipData();
		if (shipData == null)
		{
			SetLockState(m_btnLock.m_bSelect, bChanged: false);
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipData.m_UnitID);
		if (unitTempletBase == null)
		{
			SetLockState(m_btnLock.m_bSelect, bChanged: false);
			return;
		}
		NKMShipCommandModuleTemplet nKMShipCommandModuleTemplet = NKMShipManager.GetNKMShipCommandModuleTemplet(unitTempletBase.m_NKM_UNIT_STYLE_TYPE, unitTempletBase.m_NKM_UNIT_GRADE, m_moduleIndex + 1);
		if (nKMShipCommandModuleTemplet == null)
		{
			Log.Error($"NKMShipCommandModuleTemplet is null - NKM_UNIT_STYLE_TYPE : {unitTempletBase.m_NKM_UNIT_STYLE_TYPE}, NKM_UNIT_GRADE : {unitTempletBase.m_NKM_UNIT_GRADE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCPopupShipCommandModuleSlotSocket.cs", 155);
			SetLockState(m_btnLock.m_bSelect, bChanged: false);
		}
		else if (NKCScenManager.CurrentUserData().GetShipCandidateData().shipUid > 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_NEC_FAIL_SHIP_COMMAND_MODULE_LOCK_NO_CONFIRM);
		}
		else if (!m_btnLock.m_bSelect)
		{
			NKCPopupResourceConfirmBox.Instance.Open($"{NKCUtilString.GET_STRING_SHIP_COMMAND_MODULE} {m_moduleIndex + 1}", string.Format(NKCUtilString.GET_STRING_SHIP_COMMANDMODULE_SLOT_LOCK, m_socketIndex + 1), nKMShipCommandModuleTemplet.ModuleLockItems[m_socketIndex].ItemId, nKMShipCommandModuleTemplet.ModuleLockItems[m_socketIndex].Count32, delegate
			{
				OnConfirmLock(!m_btnLock.m_bSelect);
			});
		}
		else
		{
			NKCPopupOKCancel.OpenOKCancelBox($"{NKCUtilString.GET_STRING_SHIP_COMMAND_MODULE} {m_moduleIndex + 1}", string.Format(NKCUtilString.GET_STRING_SHIP_COMMANDMODULE_SLOT_UNLOCK, m_socketIndex + 1) + "\n" + NKCUtilString.GET_STRING_SHIP_COMMAND_MODULE_SLOT_LOCK_COST_NOT_RECOVERED, delegate
			{
				OnConfirmLock(!m_btnLock.m_bSelect);
			});
		}
	}

	private void OnConfirmLock(bool bValue)
	{
		m_bLockChanged = true;
		m_btnLock.m_bSelect = bValue;
		NKCUtil.SetGameobjectActive(m_objFocusLine, bValue);
		NKCPacketSender.Send_NKMPacket_SHIP_SLOT_LOCK_REQ(NKCPopupShipCommandModule.Instance.GetShipUID(), m_moduleIndex, m_socketIndex, bValue);
	}

	public void SetLockState(bool bLocked, bool bChanged)
	{
		if (bLocked)
		{
			if (bChanged)
			{
				SetLockState(SHIP_MODULE_SOCKET_STATE.UNLOCK_TO_LOCK);
			}
			else
			{
				SetLockState(SHIP_MODULE_SOCKET_STATE.LOCK);
			}
		}
		else if (bChanged)
		{
			SetLockState(SHIP_MODULE_SOCKET_STATE.LOCK_TO_UNLOCK);
		}
		else
		{
			SetLockState(SHIP_MODULE_SOCKET_STATE.UNLOCK);
		}
		NKCUtil.SetGameobjectActive(m_objLocked, bLocked);
		NKCUtil.SetGameobjectActive(m_objAfter, !bLocked && !NKMShipManager.IsMaxStat(m_StatGroupId, m_nextSlotData));
		NKCUtil.SetGameobjectActive(m_objAfterMax, !bLocked && NKMShipManager.IsMaxStat(m_StatGroupId, m_nextSlotData));
	}

	public void SetLockState(SHIP_MODULE_SOCKET_STATE state)
	{
		switch (state)
		{
		case SHIP_MODULE_SOCKET_STATE.LOCK:
			m_Ani.SetTrigger("LOCK_IDLE");
			break;
		case SHIP_MODULE_SOCKET_STATE.LOCK_TO_UNLOCK:
			m_Ani.SetTrigger("LOCK_TO_UNLOCK");
			break;
		case SHIP_MODULE_SOCKET_STATE.UNLOCK:
			m_Ani.SetTrigger("UNLOCK_IDLE");
			break;
		case SHIP_MODULE_SOCKET_STATE.UNLOCK_TO_LOCK:
			m_Ani.SetTrigger("UNLOCK_TO_LOCK");
			break;
		}
	}

	public void ShowRerollFx()
	{
		if (!m_curSlotData.isLock)
		{
			NKCUtil.SetGameobjectActive(m_objRerollFx, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRerollFx, bValue: true);
		}
	}

	public void DisableAllFx()
	{
		NKCUtil.SetGameobjectActive(m_objRerollFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMaxFx, bValue: false);
	}
}
