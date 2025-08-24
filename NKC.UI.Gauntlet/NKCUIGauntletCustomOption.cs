using ClientPacket.Pvp;
using UnityEngine;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletCustomOption : MonoBehaviour
{
	public delegate void OnDraftBanModeChanged(bool value);

	public GameObject m_objApplyEquipStat;

	public GameObject m_objApplyAllUnitMaxLevel;

	public GameObject m_objApplyBanUp;

	public GameObject m_objDraftBanMode;

	public NKCUIComToggle m_tglApplyEquipStat;

	public NKCUIComToggle m_tglApplyAllUnitMaxLevel;

	public NKCUIComToggle m_tglApplyBanUp;

	public NKCUIComToggle m_tglDraftBanMode;

	public bool m_ProhibitToggle;

	private OnDraftBanModeChanged m_dOnDraftBanModeChanged;

	public void Init(OnDraftBanModeChanged dOnDraftBanModeChanged = null)
	{
		NKCUtil.SetToggleValueChangedDelegate(m_tglApplyEquipStat, OnToggleApplyEquipSet);
		NKCUtil.SetToggleValueChangedDelegate(m_tglApplyAllUnitMaxLevel, OnToggleAllUnitMaxLevel);
		NKCUtil.SetToggleValueChangedDelegate(m_tglApplyBanUp, OnToggleBanUp);
		NKCUtil.SetToggleValueChangedDelegate(m_tglDraftBanMode, OnToggleDraftBanMode);
		m_dOnDraftBanModeChanged = dOnDraftBanModeChanged;
	}

	public void SetOption(NKMPrivateGameConfig privateGameConfig, bool setActiveToggle = true)
	{
		m_tglApplyEquipStat.Select(!privateGameConfig.applyEquipStat, bForce: true);
		m_tglApplyAllUnitMaxLevel.Select(privateGameConfig.applyAllUnitMaxLevel, bForce: true);
		m_tglApplyBanUp.Select(privateGameConfig.applyBanUpSystem, bForce: true);
		m_tglDraftBanMode?.Select(privateGameConfig.draftBanMode, bForce: true);
		if (setActiveToggle)
		{
			NKCUtil.SetGameobjectActive(m_objApplyEquipStat, !privateGameConfig.applyEquipStat);
			NKCUtil.SetGameobjectActive(m_objApplyAllUnitMaxLevel, privateGameConfig.applyAllUnitMaxLevel);
			NKCUtil.SetGameobjectActive(m_objApplyBanUp, privateGameConfig.applyBanUpSystem);
			NKCUtil.SetGameobjectActive(m_objDraftBanMode, privateGameConfig.draftBanMode);
		}
	}

	public void SetActive(bool value)
	{
		base.gameObject.SetActive(value);
	}

	private void OnToggleApplyEquipSet(bool value)
	{
		if (m_ProhibitToggle)
		{
			m_tglApplyEquipStat.Select(!value, bForce: true);
		}
		else
		{
			NKCPrivatePVPRoomMgr.SetApplyEquipSet(!value);
		}
	}

	private void OnToggleAllUnitMaxLevel(bool value)
	{
		if (m_ProhibitToggle)
		{
			m_tglApplyAllUnitMaxLevel.Select(!value, bForce: true);
		}
		else
		{
			NKCPrivatePVPRoomMgr.SetApplyAllUnitMaxLevel(value);
		}
	}

	private void OnToggleBanUp(bool value)
	{
		if (value)
		{
			m_tglDraftBanMode.Select(bSelect: false, bForce: true);
			if (NKCPrivatePVPRoomMgr.PrivateGameConfig.draftBanMode)
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString("SI_PF_PVP_FRIENDLY_OPTION_POPUP_BOX_DESC"), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				NKCPrivatePVPRoomMgr.SetDraftBanMode(value: false);
				if (m_dOnDraftBanModeChanged != null)
				{
					m_dOnDraftBanModeChanged(!value);
				}
			}
		}
		if (m_ProhibitToggle)
		{
			m_tglApplyBanUp.Select(!value, bForce: true);
		}
		else
		{
			NKCPrivatePVPRoomMgr.SetApplyBanUp(value);
		}
	}

	private void OnToggleDraftBanMode(bool value)
	{
		if (value)
		{
			m_tglApplyBanUp.Select(bSelect: false, bForce: true);
			if (NKCPrivatePVPRoomMgr.LobbyData.config.applyBanUpSystem)
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString("SI_PF_PVP_FRIENDLY_OPTION_POPUP_BOX_DESC"), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				NKCPrivatePVPRoomMgr.SetApplyBanUp(value: false);
			}
		}
		if (m_ProhibitToggle)
		{
			m_tglDraftBanMode?.Select(!value, bForce: true);
			return;
		}
		NKCPrivatePVPRoomMgr.SetDraftBanMode(value);
		if (m_dOnDraftBanModeChanged != null)
		{
			m_dOnDraftBanModeChanged(value);
		}
	}
}
