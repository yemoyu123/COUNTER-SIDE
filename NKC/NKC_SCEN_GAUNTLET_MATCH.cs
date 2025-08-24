using ClientPacket.Pvp;
using NKC.UI.Gauntlet;
using NKM;
using UnityEngine.Video;

namespace NKC;

public class NKC_SCEN_GAUNTLET_MATCH : NKC_SCEN_BASIC
{
	private NKCAssetResourceData m_UILoadResourceData;

	private NKCUIGauntletMatch m_NKCUIGauntletMatch;

	private NKM_GAME_TYPE m_ReservedGameType = NKM_GAME_TYPE.NGT_PVP_RANK;

	private AsyncPvpTarget m_reservedAsyncTarget;

	private NpcPvpTarget m_reservedAsyncNpcTarget;

	public NKCUIGauntletMatch NKCUIGuantletMatch => m_NKCUIGauntletMatch;

	public NKC_SCEN_GAUNTLET_MATCH()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_GAUNTLET_MATCH;
	}

	public void SetReservedGameType(NKM_GAME_TYPE eNKM_GAME_TYPE)
	{
		m_ReservedGameType = eNKM_GAME_TYPE;
	}

	public void ClearCacheData()
	{
		if (m_NKCUIGauntletMatch != null)
		{
			m_NKCUIGauntletMatch.CloseInstance();
			m_NKCUIGauntletMatch = null;
		}
	}

	public void ProcessReLogin()
	{
		if (m_NKCUIGauntletMatch != null)
		{
			if (m_NKCUIGauntletMatch.Get_NKC_GAUNTLET_MATCH_STATE() == NKCUIGauntletMatch.NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCH_COMPLETE)
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME);
			}
			else if (NKCScenManager.CurrentUserData().m_UserState != UserState.PVPReady)
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_MATCH_READY);
			}
		}
		else
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
		}
	}

	public void ProcessReLogin(NKMGameData gameData)
	{
		if (m_NKCUIGauntletMatch != null)
		{
			if (m_NKCUIGauntletMatch.Get_NKC_GAUNTLET_MATCH_STATE() == NKCUIGauntletMatch.NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCH_COMPLETE && m_NKCUIGauntletMatch.GetVSShowTime())
			{
				if (NKCScenManager.GetScenManager().GetGameClient().GetGameDataDummy() == null)
				{
					NKCScenManager.GetScenManager().GetGameClient().SetGameDataDummy(gameData);
				}
				return;
			}
			if (m_NKCUIGauntletMatch.Get_NKC_GAUNTLET_MATCH_STATE() == NKCUIGauntletMatch.NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCHING)
			{
				return;
			}
		}
		if (gameData != null)
		{
			NKCScenManager.GetScenManager().GetGameClient().SetGameDataDummy(gameData, bIntrude: true);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME);
		}
	}

	public void SetDeckIndex(byte index)
	{
		NKCUIGauntletMatch.SetDeckIndex(index);
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (m_NKCUIGauntletMatch == null)
		{
			m_UILoadResourceData = NKCUIGauntletMatch.OpenInstanceAsync();
		}
		else
		{
			m_UILoadResourceData = null;
		}
	}

	public override void ScenLoadUpdate()
	{
		if (!NKCAssetResourceManager.IsLoadEnd())
		{
			return;
		}
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null && subUICameraVideoPlayer.IsPreparing())
		{
			return;
		}
		if (m_NKCUIGauntletMatch == null && m_UILoadResourceData != null)
		{
			if (!NKCUIGauntletMatch.CheckInstanceLoaded(m_UILoadResourceData, out m_NKCUIGauntletMatch))
			{
				return;
			}
			m_UILoadResourceData = null;
		}
		ScenLoadLastStart();
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
		if (m_NKCUIGauntletMatch != null)
		{
			m_NKCUIGauntletMatch.InitUI();
		}
		SetBG();
	}

	private void SetBG()
	{
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
			subUICameraVideoPlayer.m_fMoviePlaySpeed = 1f;
			subUICameraVideoPlayer.Play("Gauntlet_BG.mp4", bLoop: true);
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCCamera.EnableBloom(bEnable: false);
		if (m_NKCUIGauntletMatch != null)
		{
			m_NKCUIGauntletMatch.Open(m_ReservedGameType);
			if (m_ReservedGameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC && m_reservedAsyncNpcTarget != null)
			{
				m_NKCUIGauntletMatch.SetTarget(m_reservedAsyncNpcTarget);
				m_reservedAsyncNpcTarget = null;
			}
			else if (m_reservedAsyncTarget != null)
			{
				m_NKCUIGauntletMatch.SetTarget(m_reservedAsyncTarget);
				m_reservedAsyncTarget = null;
			}
		}
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		if (m_NKCUIGauntletMatch != null)
		{
			m_NKCUIGauntletMatch.Close();
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

	public void OnRecv(NKMPacket_PVP_GAME_MATCH_COMPLETE_NOT cNKMPacket_PVP_GAME_MATCH_COMPLETE_NOT)
	{
		m_NKCUIGauntletMatch.OnRecv(cNKMPacket_PVP_GAME_MATCH_COMPLETE_NOT);
	}

	public void SetReservedAsyncTarget(AsyncPvpTarget target)
	{
		m_reservedAsyncTarget = target;
	}

	public void SetReservedAsyncTarget(NpcPvpTarget target)
	{
		m_reservedAsyncNpcTarget = target;
	}
}
