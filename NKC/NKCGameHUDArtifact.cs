using System.Collections.Generic;
using System.Text;
using NKM;
using NKM.Guild;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCGameHUDArtifact : MonoBehaviour
{
	public Animator m_AmtorOpenClose;

	public Animator m_AmtorArtifactEffectAlarm;

	public Text m_lbTotalViewDesc;

	public NKCUIComStateButton m_csbtnOpen;

	public NKCUIComStateButton m_csbtnClose;

	public Image m_NUF_GAME_HUD_ARTIFACT_ICON;

	public Sprite m_spDiveArtifact;

	public Sprite m_spBattleCondition;

	public GameObject m_NUF_GAME_HUD_ARTIFACT_ANI_TEXT;

	public GameObject m_NUF_GAME_HUD_BATTLE_CONDITION_ANI_TEXT;

	public void Awake()
	{
		m_csbtnOpen.PointerClick.RemoveAllListeners();
		m_csbtnOpen.PointerClick.AddListener(OnClickOpen);
		m_csbtnClose.PointerClick.RemoveAllListeners();
		m_csbtnClose.PointerClick.AddListener(OnClickClose);
	}

	public static bool GetActive(NKMGameData cNKMGameData)
	{
		switch (cNKMGameData.GetGameType())
		{
		case NKM_GAME_TYPE.NGT_DIVE:
		{
			NKMDiveGameData diveGameData = NKCScenManager.CurrentUserData().m_DiveGameData;
			if (diveGameData == null)
			{
				return false;
			}
			for (int i = 0; i < diveGameData.Player.PlayerBase.Artifacts.Count; i++)
			{
				NKMDiveArtifactTemplet nKMDiveArtifactTemplet = NKMDiveArtifactTemplet.Find(diveGameData.Player.PlayerBase.Artifacts[i]);
				if (nKMDiveArtifactTemplet != null && nKMDiveArtifactTemplet.BattleConditionID > 0)
				{
					return true;
				}
			}
			if (cNKMGameData.m_BattleConditionIDs != null)
			{
				return cNKMGameData.m_BattleConditionIDs.Count > 0;
			}
			return false;
		}
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_ARENA:
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS:
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS_PRACTICE:
			if (NKCGuildCoopManager.GetMyArtifactDictionary().Count > 0)
			{
				return true;
			}
			if (cNKMGameData.m_BattleConditionIDs != null)
			{
				return cNKMGameData.m_BattleConditionIDs.Count > 0;
			}
			return false;
		default:
			if (cNKMGameData.m_BattleConditionIDs != null)
			{
				return cNKMGameData.m_BattleConditionIDs.Count > 0;
			}
			return false;
		}
	}

	public void SetUI(NKMGameData cNKMGameData)
	{
		if (cNKMGameData == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		bool flag = false;
		switch (cNKMGameData.GetGameType())
		{
		case NKM_GAME_TYPE.NGT_DIVE:
			flag = SetDiveArtifacts(cNKMGameData);
			break;
		case NKM_GAME_TYPE.NGT_FIERCE:
			flag = SetFierceArtifacts(cNKMGameData);
			break;
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_ARENA:
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS:
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS_PRACTICE:
			flag = SetGuildCoopArtifacts(cNKMGameData);
			break;
		case NKM_GAME_TYPE.NGT_PVP_RANK:
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
			flag = SetPvpBattleCond(cNKMGameData);
			break;
		default:
			flag = SetBattleCond(cNKMGameData, bUseArtifactIcon: false, NKMBattleConditionTemplet.USE_CONTENT_TYPE.UCT_BATTLE_CONDITION);
			break;
		}
		NKCUtil.SetGameobjectActive(m_NUF_GAME_HUD_ARTIFACT_ANI_TEXT, cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_DIVE);
		NKCUtil.SetGameobjectActive(m_NUF_GAME_HUD_BATTLE_CONDITION_ANI_TEXT, cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_FIERCE);
		NKCUtil.SetGameobjectActive(base.gameObject, flag);
		if (flag)
		{
			m_AmtorOpenClose.Play("NUF_GAME_HUD_ARTIFACT_CLOSE_IDLE");
		}
	}

	private bool SetDiveArtifacts(NKMGameData cNKMGameData)
	{
		NKMDiveGameData diveGameData = NKCScenManager.CurrentUserData().m_DiveGameData;
		if (diveGameData == null)
		{
			return false;
		}
		bool flag = false;
		for (int i = 0; i < diveGameData.Player.PlayerBase.Artifacts.Count; i++)
		{
			NKMDiveArtifactTemplet nKMDiveArtifactTemplet = NKMDiveArtifactTemplet.Find(diveGameData.Player.PlayerBase.Artifacts[i]);
			if (nKMDiveArtifactTemplet != null && nKMDiveArtifactTemplet.BattleConditionID > 0)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return false;
		}
		NKCUtil.SetLabelText(m_lbTotalViewDesc, NKCUtilString.GetDiveArtifactTotalViewDesc(diveGameData.Player.PlayerBase.Artifacts));
		NKCUtil.SetImageSprite(m_NUF_GAME_HUD_ARTIFACT_ICON, m_spDiveArtifact);
		return true;
	}

	private bool SetBattleCond(NKMGameData cNKMGameData, bool bUseArtifactIcon, params NKMBattleConditionTemplet.USE_CONTENT_TYPE[] useTypes)
	{
		if (cNKMGameData.m_BattleConditionIDs == null || cNKMGameData.m_BattleConditionIDs.Count <= 0)
		{
			return false;
		}
		int num = 0;
		HashSet<NKMBattleConditionTemplet.USE_CONTENT_TYPE> hashSet = new HashSet<NKMBattleConditionTemplet.USE_CONTENT_TYPE>(useTypes);
		StringBuilder stringBuilder = new StringBuilder();
		foreach (int key in cNKMGameData.m_BattleConditionIDs.Keys)
		{
			NKMBattleConditionTemplet templetByID = NKMBattleConditionManager.GetTempletByID(key);
			if (!templetByID.m_bHide && templetByID != null && hashSet.Contains(templetByID.UseContentsType))
			{
				stringBuilder.AppendLine(templetByID.BattleCondName_Translated ?? "");
				stringBuilder.AppendLine(templetByID.BattleCondDesc_Translated ?? "");
				stringBuilder.AppendLine();
				num++;
			}
		}
		if (num > 0)
		{
			NKCUtil.SetLabelText(m_lbTotalViewDesc, stringBuilder.ToString());
			NKCUtil.SetImageSprite(m_NUF_GAME_HUD_ARTIFACT_ICON, bUseArtifactIcon ? m_spDiveArtifact : m_spBattleCondition);
			return true;
		}
		return false;
	}

	private bool SetBattleCond(NKMGameData cNKMGameData, bool bUseArtifactIcon, NKMBattleConditionTemplet.USE_CONTENT_TYPE useType)
	{
		if (cNKMGameData.m_BattleConditionIDs == null || cNKMGameData.m_BattleConditionIDs.Count <= 0)
		{
			return false;
		}
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (int key in cNKMGameData.m_BattleConditionIDs.Keys)
		{
			NKMBattleConditionTemplet templetByID = NKMBattleConditionManager.GetTempletByID(key);
			if (!templetByID.m_bHide && templetByID != null && templetByID.UseContentsType == useType)
			{
				stringBuilder.AppendLine(templetByID.BattleCondName_Translated ?? "");
				stringBuilder.AppendLine(templetByID.BattleCondDesc_Translated ?? "");
				stringBuilder.AppendLine();
				num++;
			}
		}
		if (num > 0)
		{
			NKCUtil.SetLabelText(m_lbTotalViewDesc, stringBuilder.ToString());
			NKCUtil.SetImageSprite(m_NUF_GAME_HUD_ARTIFACT_ICON, bUseArtifactIcon ? m_spDiveArtifact : m_spBattleCondition);
			return true;
		}
		return false;
	}

	private bool SetPvpBattleCond(NKMGameData cNKMGameData)
	{
		return SetBattleCond(cNKMGameData, bUseArtifactIcon: false, NKMBattleConditionTemplet.USE_CONTENT_TYPE.UCT_BATTLE_CONDITION);
	}

	private bool SetFierceArtifacts(NKMGameData cNKMGameData)
	{
		return SetBattleCond(cNKMGameData, false, NKMBattleConditionTemplet.USE_CONTENT_TYPE.UCT_BATTLE_CONDITION, NKMBattleConditionTemplet.USE_CONTENT_TYPE.UCT_FIERCE_PENALTY);
	}

	private bool SetGuildCoopArtifacts(NKMGameData cNKMGameData)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (cNKMGameData.m_BattleConditionIDs != null && cNKMGameData.m_BattleConditionIDs.Count > 0)
		{
			foreach (int key in cNKMGameData.m_BattleConditionIDs.Keys)
			{
				NKMBattleConditionTemplet templetByID = NKMBattleConditionManager.GetTempletByID(key);
				if (!templetByID.m_bHide && templetByID != null && templetByID.UseContentsType == NKMBattleConditionTemplet.USE_CONTENT_TYPE.UCT_BATTLE_CONDITION)
				{
					stringBuilder.AppendLine(templetByID.BattleCondName_Translated ?? "");
					stringBuilder.AppendLine(templetByID.BattleCondDesc_Translated ?? "");
					stringBuilder.AppendLine();
				}
			}
		}
		Dictionary<int, List<GuildDungeonArtifactTemplet>> myArtifactDictionary = NKCGuildCoopManager.GetMyArtifactDictionary();
		if (myArtifactDictionary.Count > 0)
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, List<GuildDungeonArtifactTemplet>> item in myArtifactDictionary)
			{
				for (int i = 0; i < item.Value.Count; i++)
				{
					list.Add(item.Value[i].GetArtifactId());
				}
			}
			stringBuilder.Append(NKCUtilString.GetGuildArtifactTotalViewDesc(list));
		}
		if (stringBuilder.Length > 0)
		{
			NKCUtil.SetLabelText(m_lbTotalViewDesc, stringBuilder.ToString());
			NKCUtil.SetImageSprite(m_NUF_GAME_HUD_ARTIFACT_ICON, m_spBattleCondition);
			return true;
		}
		return false;
	}

	public void PlayEffectNoticeAni()
	{
		if (base.gameObject.activeSelf)
		{
			m_AmtorArtifactEffectAlarm.Play("NUF_GAME_HUD_ARTIFACT");
		}
	}

	private void OnClickOpen()
	{
		m_AmtorOpenClose.Play("NUF_GAME_HUD_ARTIFACT_OPEN");
	}

	private void OnClickClose()
	{
		m_AmtorOpenClose.Play("NUF_GAME_HUD_ARTIFACT_CLOSE");
	}
}
