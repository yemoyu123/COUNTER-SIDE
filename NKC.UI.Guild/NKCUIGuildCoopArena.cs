using ClientPacket.Guild;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildCoopArena : MonoBehaviour
{
	public NKCUIComStateButton m_btn;

	public Text m_lbArenaNum;

	[Header("정화도 표시")]
	public Image m_imgClearPoint;

	[Header("보유 아티팩트 숫자")]
	public GameObject m_objArtifactCount;

	public Text m_lbArtifactHaveCount;

	[Header("다른 유저가 전투중")]
	public GameObject m_objFight;

	public Text m_lbFightUserName;

	[Header("목표 아티팩트 표시")]
	public GameObject m_objFlag;

	public GameObject m_objFlagClear;

	private GuildDungeonInfoTemplet m_GuildDungeonInfoTemplet;

	private NKCUIGuildCoopBack.OnClickArena m_dOnClickArena;

	public void InitUI(NKCUIGuildCoopBack.OnClickArena onClickArena = null)
	{
		m_dOnClickArena = onClickArena;
		m_btn.PointerClick.RemoveAllListeners();
		m_btn.PointerClick.AddListener(OnClickArena);
	}

	public void SetData(GuildDungeonInfoTemplet templet)
	{
		m_GuildDungeonInfoTemplet = templet;
		if (m_GuildDungeonInfoTemplet == null)
		{
			m_btn.Lock();
			NKCUtil.SetGameobjectActive(m_imgClearPoint, bValue: false);
			NKCUtil.SetGameobjectActive(m_objArtifactCount, bValue: false);
			NKCUtil.SetGameobjectActive(m_objFight, bValue: false);
		}
		else
		{
			m_btn.UnLock();
			NKCUtil.SetGameobjectActive(m_imgClearPoint, bValue: true);
			NKCUtil.SetGameobjectActive(m_objArtifactCount, bValue: true);
			NKCUtil.SetLabelText(m_lbArenaNum, m_GuildDungeonInfoTemplet.GetArenaIndex().ToString());
			UpdateClearPoint();
			UpdatePlayUser();
		}
	}

	private void UpdateClearPoint()
	{
		GuildDungeonArena guildDungeonArena = NKCGuildCoopManager.m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == m_GuildDungeonInfoTemplet.GetArenaIndex());
		if (guildDungeonArena != null)
		{
			int currentArtifactCountByArena = NKCGuildCoopManager.GetCurrentArtifactCountByArena(m_GuildDungeonInfoTemplet.GetArenaIndex());
			int count = GuildDungeonTempletManager.GetDungeonArtifactList(m_GuildDungeonInfoTemplet.GetStageRewardArtifactGroup()).Count;
			NKCUtil.SetLabelText(m_lbArtifactHaveCount, currentArtifactCountByArena.ToString());
			if (currentArtifactCountByArena == count)
			{
				m_imgClearPoint.fillAmount = 1f;
			}
			else
			{
				m_imgClearPoint.fillAmount = NKCGuildCoopManager.GetClearPointPercentage(m_GuildDungeonInfoTemplet.GetArenaIndex());
			}
			if (guildDungeonArena.flagIndex >= 0)
			{
				NKCUtil.SetGameobjectActive(m_objFlag, guildDungeonArena.flagIndex >= currentArtifactCountByArena);
				NKCUtil.SetGameobjectActive(m_objFlagClear, guildDungeonArena.flagIndex < currentArtifactCountByArena);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objFlag, bValue: false);
				NKCUtil.SetGameobjectActive(m_objFlagClear, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objFlag, bValue: false);
			NKCUtil.SetGameobjectActive(m_objFlagClear, bValue: false);
			NKCUtil.SetLabelText(m_lbArtifactHaveCount, "0");
			m_imgClearPoint.fillAmount = 0f;
		}
	}

	private void UpdatePlayUser()
	{
		GuildDungeonArena arena = NKCGuildCoopManager.m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == m_GuildDungeonInfoTemplet.GetArenaIndex());
		if (arena != null)
		{
			if (arena.playUserUid == 0L)
			{
				NKCUtil.SetGameobjectActive(m_objFight, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(m_objFight, bValue: true);
			GuildDungeonMemberInfo guildDungeonMemberInfo = NKCGuildCoopManager.GetGuildDungeonMemberInfo().Find((GuildDungeonMemberInfo x) => x.profile.userUid == arena.playUserUid);
			NKCUtil.SetLabelText(m_lbFightUserName, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_SESSION_ATTACK_ING_USER_INFO, guildDungeonMemberInfo.profile.nickname));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objFight, bValue: false);
		}
	}

	public void OnClickArena()
	{
		if (m_GuildDungeonInfoTemplet != null)
		{
			m_dOnClickArena?.Invoke(m_GuildDungeonInfoTemplet);
		}
	}

	public void Refresh()
	{
		UpdatePlayUser();
		UpdateClearPoint();
	}
}
