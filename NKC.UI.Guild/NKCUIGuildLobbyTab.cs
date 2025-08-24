using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Guild;

public class NKCUIGuildLobbyTab : MonoBehaviour
{
	public delegate void OnToggle(NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE tabType, bool bForce = false);

	public NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE m_tabType;

	public NKCUIComToggle m_tgl;

	public GameObject m_objRedDot;

	public OnToggle m_dOnValueChanged;

	public void InitUI(OnToggle onToggle)
	{
		m_tgl.OnValueChanged.RemoveAllListeners();
		m_tgl.OnValueChanged.AddListener(OnClickTab);
		m_dOnValueChanged = onToggle;
	}

	public void UpdateState()
	{
		switch (m_tabType)
		{
		case NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Mission:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_MISSION))
			{
				m_tgl.Lock();
			}
			else
			{
				m_tgl.UnLock();
			}
			break;
		case NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Point:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_POINT))
			{
				m_tgl.Lock();
			}
			else
			{
				m_tgl.UnLock();
			}
			break;
		case NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Ranking:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_RANKING))
			{
				m_tgl.Lock(bForce: true);
			}
			else
			{
				m_tgl.UnLock();
			}
			break;
		case NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.None:
		case NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Info:
		case NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Member:
		case NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Manage:
		case NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Invite:
			break;
		}
	}

	public void CheckRedDot()
	{
		bool bValue = false;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			switch (m_tabType)
			{
			case NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Mission:
			{
				NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(NKM_MISSION_TYPE.GUILD);
				if (missionTabTemplet != null)
				{
					bValue = nKMUserData.m_MissionData.CheckCompletableMission(nKMUserData, missionTabTemplet.m_tabID);
				}
				break;
			}
			}
		}
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue);
	}

	public NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE GetTabType()
	{
		return m_tabType;
	}

	public void OnClickTab(bool bValue)
	{
		if (bValue)
		{
			m_dOnValueChanged(m_tabType);
		}
	}
}
