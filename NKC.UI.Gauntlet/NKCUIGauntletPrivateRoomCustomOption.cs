using ClientPacket.Pvp;
using UnityEngine;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletPrivateRoomCustomOption : MonoBehaviour
{
	public GameObject m_objApplyEquipStat;

	public GameObject m_objApplyAllUnitMaxLevel;

	public GameObject m_objApplyBanUp;

	public GameObject m_objGameMode;

	public NKCUIComToggle m_tglApplyEquipStat;

	public NKCUIComToggle m_tglApplyAllUnitMaxLevel;

	public NKCUIComToggle m_tglApplyBanUp;

	public NKCUIComToggle m_tglGameModeNormal;

	public NKCUIComToggle m_tglGameModeDraft;

	public bool m_ProhibitToggle;

	public bool ProhibitToggle
	{
		set
		{
			m_ProhibitToggle = value;
			if (m_ProhibitToggle)
			{
				m_tglApplyEquipStat.enabled = false;
				m_tglApplyAllUnitMaxLevel.enabled = false;
				m_tglApplyBanUp.enabled = false;
				if (m_tglGameModeNormal != null)
				{
					m_tglGameModeNormal.enabled = false;
				}
				if (m_tglGameModeDraft != null)
				{
					m_tglGameModeDraft.enabled = false;
				}
			}
			else
			{
				m_tglApplyEquipStat.enabled = true;
				m_tglApplyAllUnitMaxLevel.enabled = true;
				m_tglApplyBanUp.enabled = true;
				if (m_tglGameModeNormal != null)
				{
					m_tglGameModeNormal.enabled = true;
				}
				if (m_tglGameModeDraft != null)
				{
					m_tglGameModeDraft.enabled = true;
				}
			}
		}
	}

	public void Init()
	{
		NKCUtil.SetToggleValueChangedDelegate(m_tglApplyEquipStat, OnToggleApplyEquipSet);
		NKCUtil.SetToggleValueChangedDelegate(m_tglApplyAllUnitMaxLevel, OnToggleAllUnitMaxLevel);
		NKCUtil.SetToggleValueChangedDelegate(m_tglApplyBanUp, OnToggleBanUp);
		NKCUtil.SetToggleValueChangedDelegate(m_tglGameModeNormal, OnToggleGameModeNormal);
		NKCUtil.SetToggleValueChangedDelegate(m_tglGameModeDraft, OnToggleGameModeDraft);
	}

	public void SetOption(NKMPrivateGameConfig privateGameConfig)
	{
		m_tglApplyEquipStat.Select(!privateGameConfig.applyEquipStat, bForce: true);
		m_tglApplyAllUnitMaxLevel.Select(privateGameConfig.applyAllUnitMaxLevel, bForce: true);
		m_tglApplyBanUp.Select(privateGameConfig.applyBanUpSystem, bForce: true);
		if (privateGameConfig.draftBanMode)
		{
			m_tglGameModeDraft?.Select(bSelect: true, bForce: true);
		}
		else
		{
			m_tglGameModeNormal?.Select(bSelect: true, bForce: true);
		}
		NKCUtil.SetGameobjectActive(m_objApplyEquipStat, bValue: true);
		NKCUtil.SetGameobjectActive(m_objApplyAllUnitMaxLevel, bValue: true);
		NKCUtil.SetGameobjectActive(m_objApplyBanUp, bValue: true);
		NKCUtil.SetGameobjectActive(m_objGameMode, bValue: true);
	}

	public void SetActive(bool value)
	{
		base.gameObject.SetActive(value);
	}

	private void OnToggleApplyEquipSet(bool value)
	{
		NKCPrivatePVPRoomMgr.SetApplyEquipSet(!value);
	}

	private void OnToggleAllUnitMaxLevel(bool value)
	{
		NKCPrivatePVPRoomMgr.SetApplyAllUnitMaxLevel(value);
	}

	private void OnToggleBanUp(bool value)
	{
		if (value && NKCPrivatePVPRoomMgr.PrivateGameConfig.draftBanMode)
		{
			m_tglGameModeNormal.Select(bSelect: true, bForce: true);
			NKCPrivatePVPRoomMgr.SetDraftBanMode(value: false);
		}
		NKCPrivatePVPRoomMgr.SetApplyBanUp(value);
	}

	private void OnToggleGameModeNormal(bool value)
	{
		if (value)
		{
			NKCPrivatePVPRoomMgr.SetDraftBanMode(!value);
		}
	}

	private void OnToggleGameModeDraft(bool value)
	{
		if (value)
		{
			if (NKCPrivatePVPRoomMgr.PrivateGameConfig.applyBanUpSystem)
			{
				m_tglApplyBanUp.Select(bSelect: false, bForce: true);
				NKCPrivatePVPRoomMgr.SetApplyBanUp(value: false);
			}
			NKCPrivatePVPRoomMgr.SetDraftBanMode(value);
		}
	}
}
