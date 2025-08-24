using ClientPacket.Game;
using ClientPacket.LeaderBoard;
using NKC.UI;
using NKC.UI.Fierce;
using NKM;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_FIERCE_BATTLE_SUPPORT : NKC_SCEN_BASIC
{
	private NKCUIFierceBattleSupport m_NKCUIFierceBattleSupport;

	private NKCUIManager.LoadedUIData m_UIFierceBattleSupportData;

	private bool m_bChangeDataReceived;

	private const float FIVE_SECONDS = 5f;

	private float m_deltaTime;

	public NKC_SCEN_FIERCE_BATTLE_SUPPORT()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT;
	}

	public override void ScenLoadUIStart()
	{
		if (!NKCUIManager.IsValid(m_UIFierceBattleSupportData))
		{
			m_UIFierceBattleSupportData = NKCUIManager.OpenNewInstanceAsync<NKCUIFierceBattleSupport>("ab_ui_nkm_ui_fierce_battle", "NKM_UI_FIERCE_BATTLE", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), null);
		}
		base.ScenLoadUIStart();
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (m_NKCUIFierceBattleSupport == null)
		{
			if (m_UIFierceBattleSupportData != null && m_UIFierceBattleSupportData.CheckLoadAndGetInstance<NKCUIFierceBattleSupport>(out m_NKCUIFierceBattleSupport))
			{
				m_NKCUIFierceBattleSupport.Init();
			}
			else
			{
				Debug.LogError("Error - NKC_SCEN_FIERCE_BATTLE_SUPPORT.ScenLoadComplete() : UI Load Failed!");
			}
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		Open();
		CheckTutorial();
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		Close();
		m_UIFierceBattleSupportData?.CloseInstance();
		m_UIFierceBattleSupportData = null;
		m_NKCUIFierceBattleSupport = null;
		NKCCamera.GetTrackingPos().SetPause(bSet: false);
	}

	public void Open()
	{
		if (m_NKCUIFierceBattleSupport != null)
		{
			m_NKCUIFierceBattleSupport.Open();
		}
	}

	public void Close()
	{
		if (m_NKCUIFierceBattleSupport != null)
		{
			m_NKCUIFierceBattleSupport.Close();
		}
	}

	public void RefreshLeaderBoard()
	{
		if (m_NKCUIFierceBattleSupport != null)
		{
			m_NKCUIFierceBattleSupport.RefreshLeaderBoard();
		}
	}

	public void SetDataReq(bool bReceived)
	{
		m_bChangeDataReceived = bReceived;
	}

	public override void ScenDataReq()
	{
		m_bChangeDataReceived = false;
		UpdateFirceData();
		m_deltaTime = 0f;
		base.ScenDataReq();
	}

	public override void ScenDataReqWaitUpdate()
	{
		m_deltaTime += Time.deltaTime;
		if (m_deltaTime > 5f)
		{
			m_deltaTime = 0f;
			Set_NKC_SCEN_STATE(NKC_SCEN_STATE.NSS_FAIL);
		}
		else if (m_bChangeDataReceived)
		{
			Debug.LogFormat("{0}.ScenDataReqWaitUpdate", m_NKM_SCEN_ID.ToString());
			ScenLoadUIStart();
		}
	}

	public void ResetUI()
	{
		if (m_NKCUIFierceBattleSupport.IsOpen)
		{
			m_NKCUIFierceBattleSupport.ResetUI();
		}
	}

	private void UpdateFirceData()
	{
		NKCPacketSender.Send_NKMPacket_FIERCE_DATA_REQ();
	}

	public void OnRecv(NKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_ACK sPacket)
	{
		m_NKCUIFierceBattleSupport.UpdateFierceBattleRank();
	}

	public void OnRecv(NKMPacket_FIERCE_COMPLETE_POINT_REWARD_ACK sPacket)
	{
		m_NKCUIFierceBattleSupport.UpdatePointRewardRedDot();
	}

	public void OnRecv(NKMPacket_FIERCE_COMPLETE_POINT_REWARD_ALL_ACK sPacket)
	{
		m_NKCUIFierceBattleSupport.UpdatePointRewardRedDot();
	}

	public void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.FierceLobby);
	}
}
