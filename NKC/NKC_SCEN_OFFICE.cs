using System;
using NKC.Office;
using NKC.UI;
using NKC.UI.Office;
using NKM;
using NKM.Templet;
using NKM.Templet.Office;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_OFFICE : NKC_SCEN_BASIC
{
	private NKCUIOffice m_uiOffice;

	private NKCUIManager.LoadedUIData m_loadUIDataOffice;

	private NKCUIOfficeMapFront m_uiOfficeMapFront;

	private NKCUIManager.LoadedUIData m_loadUIDataMapFront;

	private NKM_SHORTCUT_TYPE m_eReserverdShortcutType;

	private string m_ReservedShortCutParam;

	public NKC_SCEN_OFFICE()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_OFFICE;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		NKCOfficeManager.LoadTemplets();
		if (!NKCUIManager.IsValid(m_loadUIDataOffice))
		{
			m_loadUIDataOffice = NKCUIOffice.OpenNewInstanceAsync();
		}
		if (!NKCUIManager.IsValid(m_loadUIDataMapFront))
		{
			m_loadUIDataMapFront = NKCUIOfficeMapFront.OpenNewInstanceAsync();
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (m_uiOffice == null)
		{
			if (m_loadUIDataOffice != null && m_loadUIDataOffice.CheckLoadAndGetInstance<NKCUIOffice>(out m_uiOffice))
			{
				m_uiOffice.Init();
				m_uiOffice.Preload();
				NKCUtil.SetGameobjectActive(m_uiOffice.gameObject, bValue: false);
			}
			else
			{
				Debug.LogError("NKC_SCEN_OFFICE.ScenLoadUIComplete - ui load AB_UI_OFFICE fail");
			}
		}
		if (m_uiOfficeMapFront == null)
		{
			if (m_loadUIDataMapFront != null && m_loadUIDataMapFront.CheckLoadAndGetInstance<NKCUIOfficeMapFront>(out m_uiOfficeMapFront))
			{
				m_uiOfficeMapFront.Init();
				NKCUtil.SetGameobjectActive(m_uiOfficeMapFront.gameObject, bValue: false);
			}
			else
			{
				Debug.LogError("NKC_SCEN_OFFICE.ScenLoadUIComplete - ui load AB_UI_OFFICE_MINIMAP_ROOM fail");
			}
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		if (m_uiOfficeMapFront == null)
		{
			Debug.LogError("MapFront ui not found");
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_GAME_LOAD_FAILED, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
			return;
		}
		m_uiOfficeMapFront.Open();
		if (m_eReserverdShortcutType != NKM_SHORTCUT_TYPE.SHORTCUT_NONE)
		{
			ProcessShortcut();
		}
		if (NKCScenManager.CurrentUserData().OfficeData.BizcardCount == 0)
		{
			NKCScenManager.CurrentUserData().OfficeData.TryRefreshOfficePost(bForce: false);
		}
		if (NKCUIJukeBox.IsHasInstance && NKCUIJukeBox.Instance.AlreadyJukeBoxMode)
		{
			NKCUIManager.GetNKCUIPowerSaveMode().SetFinishJukeBox(bFinish: true);
			NKCScenManager.GetScenManager().GetNKCPowerSaveMode().SetJukeBoxMode(bEnable: true);
		}
	}

	private void ProcessShortcut()
	{
		if (m_uiOffice.IsOpen)
		{
			m_uiOffice.Close();
		}
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = null;
		switch (m_eReserverdShortcutType)
		{
		case NKM_SHORTCUT_TYPE.SHORTCUT_OFFICE:
		{
			NKMOfficeRoomTemplet.RoomType result4;
			NKCUIOfficeMapFront.SectionType result5;
			if (int.TryParse(m_ReservedShortCutParam, out var result3))
			{
				m_uiOffice.Open(result3);
			}
			else if (Enum.TryParse<NKMOfficeRoomTemplet.RoomType>(m_ReservedShortCutParam, out result4))
			{
				nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(result4);
				if (nKMOfficeRoomTemplet != null)
				{
					m_uiOffice.Open(nKMOfficeRoomTemplet.ID);
				}
			}
			else if (Enum.TryParse<NKCUIOfficeMapFront.SectionType>(m_ReservedShortCutParam, out result5))
			{
				switch (result5)
				{
				case NKCUIOfficeMapFront.SectionType.Facility:
					m_uiOfficeMapFront.SelectFacilityTab();
					break;
				case NKCUIOfficeMapFront.SectionType.Room:
					m_uiOfficeMapFront.SelectRoomTab();
					break;
				}
			}
			else
			{
				if (!Enum.TryParse<ContentsType>(m_ReservedShortCutParam, out var result6) || result6 != ContentsType.EXTRACT)
				{
					break;
				}
				if (!NKCContentManager.IsContentsUnlocked(ContentsType.EXTRACT))
				{
					NKCContentManager.ShowLockedMessagePopup(ContentsType.EXTRACT);
					break;
				}
				NKMOfficeRoomTemplet nKMOfficeRoomTemplet6 = NKMOfficeRoomTemplet.Find(NKMOfficeRoomTemplet.RoomType.Lab);
				if (nKMOfficeRoomTemplet6 != null)
				{
					m_uiOffice.Open(nKMOfficeRoomTemplet6.ID);
					m_uiOffice.OfficeFacilityInterfaces.OnLabUnitExtract();
				}
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_SCOUT:
		{
			NKMOfficeRoomTemplet nKMOfficeRoomTemplet4 = NKMOfficeRoomTemplet.Find(NKMOfficeRoomTemplet.RoomType.CEO);
			if (nKMOfficeRoomTemplet4 != null)
			{
				m_uiOffice.Open(nKMOfficeRoomTemplet4.ID);
				m_uiOffice.OfficeFacilityInterfaces.OnCEOScout();
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_LIFETIME:
		{
			NKMOfficeRoomTemplet nKMOfficeRoomTemplet5 = NKMOfficeRoomTemplet.Find(NKMOfficeRoomTemplet.RoomType.CEO);
			if (nKMOfficeRoomTemplet5 != null)
			{
				m_uiOffice.Open(nKMOfficeRoomTemplet5.ID);
				if (long.TryParse(m_ReservedShortCutParam, out var result2))
				{
					m_uiOffice.OfficeFacilityInterfaces.OnCEOLifetime(result2);
				}
				else
				{
					m_uiOffice.OfficeFacilityInterfaces.OnCEOLifetime(0L);
				}
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_SHIP_MAKE:
		{
			NKMOfficeRoomTemplet nKMOfficeRoomTemplet3 = NKMOfficeRoomTemplet.Find(NKMOfficeRoomTemplet.RoomType.Hangar);
			if (nKMOfficeRoomTemplet3 != null)
			{
				m_uiOffice.Open(nKMOfficeRoomTemplet3.ID);
				m_uiOffice.OfficeFacilityInterfaces.OnHangarBuild();
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_EQUIP_MAKE:
		{
			NKMOfficeRoomTemplet nKMOfficeRoomTemplet2 = NKMOfficeRoomTemplet.Find(NKMOfficeRoomTemplet.RoomType.Forge);
			if (nKMOfficeRoomTemplet2 != null)
			{
				m_uiOffice.Open(nKMOfficeRoomTemplet2.ID);
				m_uiOffice.OfficeFacilityInterfaces.OnForgeBuild();
				if (!string.IsNullOrEmpty(m_ReservedShortCutParam) && NKCUIForgeCraftMold.IsInstanceOpen && Enum.TryParse<NKM_CRAFT_TAB_TYPE>(m_ReservedShortCutParam, out var result))
				{
					NKCUIForgeCraftMold.Instance.SelectCraftTab(result);
				}
			}
			break;
		}
		}
		if (NKCScenManager.CurrentUserData().OfficeData.GetFriendProfile() != null)
		{
			m_uiOfficeMapFront.SetFriendOfficeData();
		}
		else
		{
			m_uiOfficeMapFront.SetMyOfficeData(nKMOfficeRoomTemplet);
		}
		m_eReserverdShortcutType = NKM_SHORTCUT_TYPE.SHORTCUT_NONE;
		m_ReservedShortCutParam = null;
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		m_uiOffice?.Close();
		m_uiOfficeMapFront?.Close();
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_uiOffice = null;
		m_loadUIDataOffice?.CloseInstance();
		m_loadUIDataOffice = null;
		m_uiOfficeMapFront = null;
		m_loadUIDataMapFront?.CloseInstance();
		m_loadUIDataMapFront = null;
	}

	public void ReserveShortcut(NKM_SHORTCUT_TYPE shortCut, string shortcutParam = null)
	{
		m_eReserverdShortcutType = shortCut;
		m_ReservedShortCutParam = shortcutParam;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_OFFICE)
		{
			ProcessShortcut();
		}
	}
}
