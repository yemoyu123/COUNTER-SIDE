using NKC.UI;
using NKM.Templet;
using UnityEngine;

namespace NKC.Office;

public class NKCOfficeFacilityCEO : NKCOfficeFacility
{
	[Header("사장실 가구")]
	public NKCOfficeFacilityFuniture m_fnScout;

	public NKCOfficeFacilityFuniture m_fnLifetime;

	public NKCOfficeFacilityFuniture m_fnJukeBox;

	public override void Init()
	{
		base.Init();
		if (m_fnScout != null)
		{
			m_fnScout.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.PERSONNAL_SCOUT));
			m_fnScout.dOnClickFuniture = OnClickScout;
		}
		if (m_fnLifetime != null)
		{
			m_fnLifetime.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.PERSONNAL_LIFETIME));
			m_fnLifetime.dOnClickFuniture = OnClickLifetime;
		}
		if (m_fnJukeBox != null)
		{
			m_fnJukeBox.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_PERSONNAL));
			m_fnJukeBox.dOnClickFuniture = OnClickJukebox;
		}
	}

	public override void UpdateAlarm()
	{
		if (m_fnScout != null)
		{
			m_fnScout.SetAlarm(NKCAlarmManager.CheckScoutNotify(NKCScenManager.CurrentUserData()));
		}
		if (m_fnJukeBox != null)
		{
			m_fnJukeBox.SetAlarm(NKCAlarmManager.CheckJukeBoxNotifiy());
		}
	}

	private void OnClickScout(int id, long uid)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.PERSONNAL_SCOUT))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.PERSONNAL_SCOUT);
		}
		else
		{
			NKCUIScout.Instance.Open();
		}
	}

	private void OnClickLifetime(int id, long uid)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.PERSONNAL_LIFETIME))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.PERSONNAL_LIFETIME);
		}
		else
		{
			NKCUIPersonnel.Instance.Open();
		}
	}

	private void OnClickJukebox(int id, long uid)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_PERSONNAL))
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_JUKEBOX_CONTENTS_UNLOCK);
		}
		else
		{
			NKCUIJukeBox.Instance.Open(bLobbyMusicSelectMode: false);
		}
	}
}
