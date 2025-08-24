using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuGuild : NKCUILobbyMenuButtonBase
{
	public NKCUIComStateButton m_csbtnMenu;

	public GameObject m_objNotify;

	public NKCUIGuildBadge m_GuildBadgeUI;

	public Text m_lbGuildName;

	public GameObject m_objNone;

	public void Init(ContentsType contentsType = ContentsType.None)
	{
		if (m_csbtnMenu != null)
		{
			m_csbtnMenu.PointerClick.RemoveAllListeners();
			m_csbtnMenu.PointerClick.AddListener(OnButton);
		}
		NKCUtil.SetLabelText(m_lbGuildName, string.Empty);
		NKCUtil.SetGameobjectActive(m_lbGuildName, bValue: false);
		NKCUtil.SetGameobjectActive(m_GuildBadgeUI, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNone, bValue: false);
		m_ContentsType = contentsType;
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		bool flag = NKCAlarmManager.CheckGuildNotify(userData);
		NKCUtil.SetGameobjectActive(m_objNotify, flag);
		SetNotify(flag);
		SetGuildName();
	}

	private void OnButton()
	{
		if (m_bLocked)
		{
			NKCContentManager.ShowLockedMessagePopup(m_ContentsType);
		}
		else if (NKCGuildManager.HasGuild())
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_LOBBY);
		}
		else
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_INTRO);
		}
	}

	public void SetGuildName()
	{
		if (m_GuildBadgeUI != null)
		{
			NKCUtil.SetGameobjectActive(m_GuildBadgeUI, NKCGuildManager.HasGuild());
			if (m_GuildBadgeUI.gameObject.activeSelf)
			{
				m_GuildBadgeUI.SetData(NKCGuildManager.MyGuildData.badgeId, bOpponent: false);
			}
			NKCUtil.SetGameobjectActive(m_lbGuildName, NKCGuildManager.HasGuild());
			if (m_lbGuildName.gameObject.activeSelf)
			{
				NKCUtil.SetLabelText(m_lbGuildName, $"[{NKCUtilString.GetUserGuildName(NKCGuildManager.MyGuildData.name, bOpponent: false)}]");
			}
			NKCUtil.SetGameobjectActive(m_objNone, !NKCGuildManager.HasGuild());
		}
	}
}
