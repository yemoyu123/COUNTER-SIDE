using NKC.UI;
using NKC.UI.Gauntlet;
using NKM;
using UnityEngine;
using UnityEngine.Video;

namespace NKC;

public class NKC_SCEN_GAUNTLET_LEAGUE_ROOM : NKC_SCEN_BASIC
{
	private NKCUIManager.LoadedUIData m_loadUIDataLeagueMatch;

	private NKCUIManager.LoadedUIData m_loadUIDataLeagueGlobalBan;

	private NKCUIManager.LoadedUIData m_loadUIDataLeagueMain;

	public NKCUIGauntletLeagueMatch m_gauntletLeagueMatch;

	public NKCUIGauntletLeagueMain m_gauntletLeagueMain;

	public NKC_SCEN_GAUNTLET_LEAGUE_ROOM()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_GAUNTLET_LEAGUE_ROOM;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (!NKCUIManager.IsValid(m_loadUIDataLeagueMatch))
		{
			m_loadUIDataLeagueMatch = NKCUIGauntletLeagueMatch.OpenNewInstanceAsync();
		}
		if (!NKCUIManager.IsValid(m_loadUIDataLeagueMain))
		{
			m_loadUIDataLeagueMain = NKCUIGauntletLeagueMain.OpenNewInstanceAsync();
		}
	}

	private void SetBG()
	{
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
			subUICameraVideoPlayer.m_fMoviePlaySpeed = 1f;
			subUICameraVideoPlayer.SetAlpha(0.6f);
			subUICameraVideoPlayer.Play("Gauntlet_BG.mp4", bLoop: true);
		}
		if (!NKCSoundManager.IsSameMusic("UI_PVP_02"))
		{
			NKCSoundManager.PlayMusic("UI_PVP_02", bLoop: true);
		}
	}

	public override void ScenLoadUpdate()
	{
		if (NKCAssetResourceManager.IsLoadEnd())
		{
			NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
			if (!(subUICameraVideoPlayer != null) || !subUICameraVideoPlayer.IsPreparing())
			{
				ScenLoadLastStart();
			}
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (m_gauntletLeagueMain == null)
		{
			if (m_loadUIDataLeagueMain != null && m_loadUIDataLeagueMain.CheckLoadAndGetInstance<NKCUIGauntletLeagueMain>(out m_gauntletLeagueMain))
			{
				m_gauntletLeagueMain.Init();
			}
			else
			{
				Debug.LogError("NKC_SCEN_GAUNTLET_LEAGUE_ROOM.ScenLoadUIComplete - m_gauntletLeagueMain load fail");
			}
		}
		if (m_gauntletLeagueMatch == null)
		{
			if (m_loadUIDataLeagueMatch != null && m_loadUIDataLeagueMatch.CheckLoadAndGetInstance<NKCUIGauntletLeagueMatch>(out m_gauntletLeagueMatch))
			{
				m_gauntletLeagueMatch.Init();
			}
			else
			{
				Debug.LogError("NKC_SCEN_GAUNTLET_LEAGUE_ROOM.ScenLoadUIComplete - m_gauntletLeagueMatch load fail ");
			}
		}
		SetBG();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCLeaguePVPMgr.m_LeagueRoomStarted = true;
		NKCLeaguePVPMgr.OnRoomStateChanged();
		NKCLeaguePVPMgr.UpdateReservedRoomData();
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		if (m_gauntletLeagueMatch != null)
		{
			m_gauntletLeagueMatch.Close();
			m_gauntletLeagueMatch = null;
		}
		m_loadUIDataLeagueMatch?.CloseInstance();
		m_loadUIDataLeagueMatch = null;
		m_loadUIDataLeagueMain?.CloseInstance();
		m_loadUIDataLeagueMain = null;
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.CleanUp();
		}
	}

	public void OnCancelAllProcess()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_LEAGUE);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
	}
}
