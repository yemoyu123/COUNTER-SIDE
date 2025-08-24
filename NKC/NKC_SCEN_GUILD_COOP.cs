using NKC.UI;
using NKC.UI.Guild;
using NKM;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_GUILD_COOP : NKC_SCEN_BASIC
{
	private NKCUIGuildCoop m_NKCUIGuildCoop;

	private NKCUIManager.LoadedUIData m_NKCUIGuildCoopUIData;

	private const float FIVE_SECONDS = 5f;

	private float m_deltaTime;

	public NKC_SCEN_GUILD_COOP()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_GUILD_COOP;
	}

	public override void ScenDataReq()
	{
		if (!NKCGuildCoopManager.m_bGuildCoopMemberDataRecved)
		{
			NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_MEMBER_INFO_REQ(NKCGuildManager.MyData.guildUid);
		}
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
		else if (NKCGuildCoopManager.m_bGuildCoopMemberDataRecved)
		{
			m_deltaTime = 0f;
			base.ScenDataReqWaitUpdate();
		}
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (!NKCUIManager.IsValid(m_NKCUIGuildCoopUIData))
		{
			m_NKCUIGuildCoopUIData = NKCUIGuildCoop.OpenNewInstanceAsync();
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (m_NKCUIGuildCoop == null)
		{
			if (m_NKCUIGuildCoopUIData != null && m_NKCUIGuildCoopUIData.CheckLoadAndGetInstance<NKCUIGuildCoop>(out m_NKCUIGuildCoop))
			{
				m_NKCUIGuildCoop.InitUI();
			}
			else
			{
				Debug.LogError("Error - NKC_SCEN_GUILD_DUNGEON.ScenLoadComplete() : UI Load Failed!");
			}
		}
	}

	public override void ScenLoadUpdate()
	{
		if (NKCAssetResourceManager.IsLoadEnd())
		{
			ScenLoadLastStart();
		}
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		OpenGuildDungeon();
		NKCCamera.GetCamera().orthographic = false;
		TutorialCheck();
	}

	private void OpenGuildDungeon()
	{
		if (m_NKCUIGuildCoop != null)
		{
			m_NKCUIGuildCoop.Open();
		}
	}

	public override void ScenEnd()
	{
		if (m_NKCUIGuildCoop != null)
		{
			m_NKCUIGuildCoop.Close();
		}
		m_NKCUIGuildCoop = null;
		m_NKCUIGuildCoopUIData?.CloseInstance();
		m_NKCUIGuildCoopUIData = null;
		base.ScenEnd();
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void TutorialCheck()
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_COOP)
		{
			NKCTutorialManager.TutorialRequired(TutorialPoint.ConsortiumDungeon);
		}
	}

	public void OnCloseInfoPopup()
	{
		if (m_NKCUIGuildCoop != null && m_NKCUIGuildCoop.IsOpen)
		{
			m_NKCUIGuildCoop?.OnCloseInfoPopup();
		}
	}

	public void Refresh()
	{
		if (NKCPopupGuildCoopSeasonReward.IsInstanceOpen)
		{
			NKCPopupGuildCoopSeasonReward.Instance.Close();
		}
		if (NKCUIGuildCoopEnd.IsInstanceOpen)
		{
			NKCUIGuildCoopEnd.Instance.Close();
		}
		OpenGuildDungeon();
	}

	public void RefreshDungeonNotice()
	{
		if (NKCUIGuildCoop.IsInstanceOpen)
		{
			m_NKCUIGuildCoop?.SetGuildCoopNotice();
		}
	}

	public void RefreshArenaSlot(int arenaIdx)
	{
		if (NKCPopupGuildCoopArenaInfo.IsInstanceOpen)
		{
			NKCPopupGuildCoopArenaInfo.Instance.Refresh();
		}
		m_NKCUIGuildCoop?.RefreshArenaSlot(arenaIdx);
	}

	public void RefreshBossInfo()
	{
		if (NKCPopupGuildCoopBossInfo.IsInstanceOpen)
		{
			NKCPopupGuildCoopBossInfo.Instance.Refresh();
		}
		m_NKCUIGuildCoop?.RefreshBossSlot();
	}
}
