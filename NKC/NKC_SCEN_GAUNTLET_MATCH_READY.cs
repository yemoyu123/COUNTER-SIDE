using System.Collections.Generic;
using ClientPacket.Common;
using NKC.UI;
using NKM;
using UnityEngine.Video;

namespace NKC;

public class NKC_SCEN_GAUNTLET_MATCH_READY : NKC_SCEN_BASIC
{
	private NKMDeckIndex m_LastDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_PVP, 0);

	private NKM_GAME_TYPE m_ReservedGameType = NKM_GAME_TYPE.NGT_PVP_RANK;

	private NKMUserProfileData m_NKMUserProfileDataTarget;

	private RANK_TYPE m_TargetRankType;

	public NKC_SCEN_GAUNTLET_MATCH_READY()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_GAUNTLET_MATCH_READY;
	}

	public NKMUserProfileData GetTargetProfileData()
	{
		return m_NKMUserProfileDataTarget;
	}

	public void SetNKMUserProfileDataTarget(NKMUserProfileData cNKMUserProfileData)
	{
		m_NKMUserProfileDataTarget = cNKMUserProfileData;
	}

	public void SetTargetRankType(RANK_TYPE type)
	{
		m_TargetRankType = type;
	}

	public RANK_TYPE GetTargetRankType()
	{
		return m_TargetRankType;
	}

	public void SetReservedGameType(NKM_GAME_TYPE eNKM_GAME_TYPE)
	{
		m_ReservedGameType = eNKM_GAME_TYPE;
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
		NKCUIDeckViewer.Instance.LoadComplete();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCCamera.EnableBloom(bEnable: false);
		NKCUIDeckViewer.DeckViewerOption options = new NKCUIDeckViewer.DeckViewerOption
		{
			MenuName = NKCUtilString.GET_STRING_GAUNTLET
		};
		if (m_ReservedGameType == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			options.eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.UnlimitedDeck;
		}
		else
		{
			options.eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.PvPBattleFindTarget;
		}
		options.dOnSideMenuButtonConfirm = OnClickDeckSelect;
		if (NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckData(m_LastDeckIndex) == null)
		{
			m_LastDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_PVP, 0);
		}
		options.DeckIndex = m_LastDeckIndex;
		if (m_ReservedGameType == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			options.dOnBackButton = delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			};
		}
		else
		{
			options.dOnBackButton = delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
			};
		}
		options.SelectLeaderUnitOnOpen = true;
		options.bEnableDefaultBackground = false;
		options.bUpsideMenuHomeButton = false;
		options.upsideMenuShowResourceList = new List<int> { 5, 101 };
		options.StageBattleStrID = string.Empty;
		NKCUIDeckViewer.Instance.Open(options);
	}

	public void OnClickStart()
	{
		NKCUIDeckViewer.Instance.OnSideMenuButtonConfirm();
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

	public void OnClickDeckSelect(NKMDeckIndex selectedDeckIndex, long supportUserUID = 0L)
	{
		if (m_ReservedGameType != NKM_GAME_TYPE.NGT_ASYNC_PVP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().SetReservedGameType(m_ReservedGameType);
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().SetDeckIndex(selectedDeckIndex.m_iIndex);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_MATCH);
		}
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		m_LastDeckIndex = NKCUIDeckViewer.Instance.GetSelectDeckIndex();
		NKCUIDeckViewer.CheckInstanceAndClose();
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.CleanUp();
		}
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}
}
