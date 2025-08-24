using NKC.UI;
using NKC.UI.Gauntlet;
using NKM;
using NKM.Templet;
using UnityEngine.Video;

namespace NKC;

public class NKC_SCEN_GAUNTLET_EVENT_READY : NKC_SCEN_BASIC
{
	public NKC_SCEN_GAUNTLET_EVENT_READY()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_GAUNTLET_EVENT_READY;
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

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
		SetBG();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCCamera.EnableBloom(bEnable: false);
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
		if (eventPvpSeasonTemplet == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_GAUNTLET_EVENTMATCH_CANNOT_ENTER, delegate
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
			});
		}
		else
		{
			NKCUIPrepareEventDeck.Instance.Open(null, null, eventPvpSeasonTemplet, OnEventDeckConfirm, OnBackButton, DeckContents.EVENT_PVP);
		}
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		NKCUIPrepareEventDeck.CheckInstanceAndClose();
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.CleanUp();
		}
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
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

	private void OnBackButton()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
	}

	private void OnEventDeckConfirm(NKMStageTempletV2 stageTemplet, NKMDungeonTempletBase dungeonTempletBase, NKMEventDeckData eventDeckData, long supportUserUID = 0L)
	{
		if (!NKCEventPvpMgr.IsEventPvpTime())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_EVENTMATCH_CANNOT_ENTER);
			return;
		}
		NKCEventPvpMgr.EventDeckData = new NKMEventDeckData();
		NKCEventPvpMgr.EventDeckData.DeepCopy(eventDeckData);
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().SetReservedGameType(NKM_GAME_TYPE.NGT_PVP_EVENT);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_MATCH);
	}
}
