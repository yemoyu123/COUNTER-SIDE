using ClientPacket.Pvp;
using NKC.UI;
using NKC.UI.Gauntlet;
using NKM;
using UnityEngine;
using UnityEngine.Video;

namespace NKC;

public class NKC_SCEN_GAUNTLET_ASYNC_READY : NKC_SCEN_BASIC
{
	private NKCUIManager.LoadedUIData m_loadUIData;

	private NKCUIGauntletAsyncReady m_AsyncReadyUI;

	private AsyncPvpTarget m_target;

	private NpcPvpTarget m_NpcTarget;

	private NKM_GAME_TYPE m_gameType = NKM_GAME_TYPE.NGT_ASYNC_PVP;

	private NKMDeckIndex m_lastDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_PVP, 0);

	public NKC_SCEN_GAUNTLET_ASYNC_READY()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_GAUNTLET_ASYNC_READY;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (!NKCUIManager.IsValid(m_loadUIData))
		{
			m_loadUIData = NKCUIGauntletAsyncReady.OpenNewInstanceAsync();
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
		if (m_AsyncReadyUI == null)
		{
			if (m_loadUIData == null || !m_loadUIData.CheckLoadAndGetInstance<NKCUIGauntletAsyncReady>(out m_AsyncReadyUI))
			{
				Debug.LogError("NKC_SCEN_GAUNTLET_ASYNC_READY.ScenLoadUIComplete - ui load fail");
				return;
			}
			m_AsyncReadyUI.Init();
		}
		SetBG();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		if (m_gameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC)
		{
			m_AsyncReadyUI?.Open(m_NpcTarget, m_lastDeckIndex, m_gameType);
		}
		else
		{
			m_AsyncReadyUI?.Open(m_target, m_lastDeckIndex, m_gameType);
		}
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		if (m_AsyncReadyUI != null)
		{
			m_lastDeckIndex = m_AsyncReadyUI.GetLastDeckIndex();
			m_AsyncReadyUI.Close();
			m_AsyncReadyUI = null;
		}
		m_loadUIData?.CloseInstance();
		m_loadUIData = null;
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.CleanUp();
		}
	}

	public void SetReserveData(NpcPvpTarget target)
	{
		m_NpcTarget = target;
		m_gameType = NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC;
	}

	public void SetReserveData(AsyncPvpTarget target, NKM_GAME_TYPE gameType = NKM_GAME_TYPE.NGT_ASYNC_PVP)
	{
		m_target = target;
		m_gameType = gameType;
	}

	public void OnRecv(NKMPacket_ASYNC_PVP_START_GAME_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetTargetData(sPacket.refreshedTargetData);
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetAsyncTargetList(sPacket.targetList);
		m_AsyncReadyUI?.OnRecv(sPacket);
	}
}
