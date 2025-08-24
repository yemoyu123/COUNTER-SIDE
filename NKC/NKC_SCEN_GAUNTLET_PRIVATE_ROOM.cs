using ClientPacket.Lobby;
using NKC.UI;
using NKC.UI.Gauntlet;
using NKM;
using UnityEngine;
using UnityEngine.Video;

namespace NKC;

public class NKC_SCEN_GAUNTLET_PRIVATE_ROOM : NKC_SCEN_BASIC
{
	private NKCUIManager.LoadedUIData m_loadUIData;

	private NKCUIGauntletPrivateRoom m_gauntletPrivateRoom;

	private NKMLobbyData m_pvpGameLobbyState;

	public NKCUIGauntletPrivateRoom GauntletPrivateRoom => m_gauntletPrivateRoom;

	public NKC_SCEN_GAUNTLET_PRIVATE_ROOM()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (!NKCUIManager.IsValid(m_loadUIData))
		{
			m_loadUIData = NKCUIGauntletPrivateRoom.OpenNewInstanceAsync();
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
		if (m_gauntletPrivateRoom == null)
		{
			if (m_loadUIData == null || !m_loadUIData.CheckLoadAndGetInstance<NKCUIGauntletPrivateRoom>(out m_gauntletPrivateRoom))
			{
				Debug.LogError("NKC_SCEN_GAUNTLET_PRIVATE_ROOM.ScenLoadUIComplete - ui load fail");
				return;
			}
			m_gauntletPrivateRoom.Init();
		}
		SetBG();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		m_gauntletPrivateRoom?.Open();
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		if (m_gauntletPrivateRoom != null)
		{
			m_gauntletPrivateRoom.Close();
			m_gauntletPrivateRoom = null;
		}
		m_loadUIData?.CloseInstance();
		m_loadUIData = null;
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.CleanUp();
		}
	}

	public void OnCancelAllProcess()
	{
		m_gauntletPrivateRoom?.ProcessBackButton();
	}

	public void SetPvpGameLobbyState(NKMLobbyData state)
	{
		m_pvpGameLobbyState = state;
	}
}
