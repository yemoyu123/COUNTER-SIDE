using System.Collections.Generic;
using Cs.Math;
using NKC.PacketHandler;
using NKC.UI.Guide;
using NKM;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildCoopRaidRightSide : NKCUIInstantiatable
{
	public delegate void onClickAttackBtn(long raidUID, List<int> _buffs, int reqItemID, int reqItemCount, bool bIsTryAssist, bool isPracticeMode);

	private onClickAttackBtn m_dOnClickAttackBtn;

	[Header("기본 정보")]
	public Text m_lbLevel;

	public Text m_lbName;

	public NKCUIComStateButton m_btnGuide;

	public NKCUIComStateButton m_csbtnInfo;

	public NKCUIComStateButton m_csbtnEnemy;

	public Image m_imgBossHP;

	public Text m_lbRemainHP;

	[Header("아티팩트 세팅")]
	public NKCUIComGuildArtifactContent m_Artifact;

	[Header("남은 횟수")]
	public GameObject m_objRemainCount;

	public Text m_lbRemainCount;

	[Header("맨 아래 버튼 모음")]
	public NKCUIComStateButton m_csbtnClear;

	public NKCUIComStateButton m_csbtnPractice;

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM_COOP";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_COOP_RAID_RIGHT";

	private int m_RaidGroupId;

	private int m_BossStageId;

	public GuildRaidTemplet GetRaidTemplet()
	{
		return GuildDungeonTempletManager.GetGuildRaidTemplet(m_RaidGroupId, m_BossStageId);
	}

	public static NKCUIGuildCoopRaidRightSide OpenInstance(Transform trParent, onClickAttackBtn _onClickAttackBtn = null)
	{
		NKCUIGuildCoopRaidRightSide nKCUIGuildCoopRaidRightSide = NKCUIInstantiatable.OpenInstance<NKCUIGuildCoopRaidRightSide>("AB_UI_NKM_UI_CONSORTIUM_COOP", "NKM_UI_CONSORTIUM_COOP_RAID_RIGHT", trParent);
		if (nKCUIGuildCoopRaidRightSide != null)
		{
			nKCUIGuildCoopRaidRightSide.Init(_onClickAttackBtn);
		}
		return nKCUIGuildCoopRaidRightSide;
	}

	public void CloseInstance()
	{
		CloseInstance("AB_UI_NKM_UI_CONSORTIUM_COOP", "NKM_UI_CONSORTIUM_COOP_RAID_RIGHT");
	}

	public void Init(onClickAttackBtn _onClickAttackBtn = null)
	{
		m_dOnClickAttackBtn = _onClickAttackBtn;
		m_csbtnClear.PointerClick.RemoveAllListeners();
		m_csbtnClear.PointerClick.AddListener(OnClickAttackBtn);
		m_csbtnClear.m_bGetCallbackWhileLocked = true;
		NKCUtil.SetHotkey(m_csbtnClear, HotkeyEventType.Confirm);
		m_csbtnPractice.PointerClick.RemoveAllListeners();
		m_csbtnPractice.PointerClick.AddListener(OnClickPracticeBtn);
		m_csbtnInfo.PointerClick.RemoveAllListeners();
		m_csbtnInfo.PointerClick.AddListener(OnClickInfoBtn);
		if (m_btnGuide != null)
		{
			m_btnGuide.PointerClick.RemoveAllListeners();
			m_btnGuide.PointerClick.AddListener(OnClickGuide);
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnEnemy, OnClickEnemy);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_Artifact.Init();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void SetUI(int raidGroupId, int bossStageId)
	{
		m_RaidGroupId = raidGroupId;
		m_BossStageId = bossStageId;
		GuildRaidTemplet raidTemplet = GetRaidTemplet();
		if (raidTemplet == null)
		{
			return;
		}
		if (NKCGuildCoopManager.IsExtraBoss(NKCGuildCoopManager.m_cGuildRaidTemplet.GetStageIndex() - 1))
		{
			NKCUtil.SetLabelText(m_lbLevel, NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_RAID_EXTRA_BOSS);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_RESULT_BOSS_LEVEL_INFO, NKCGuildCoopManager.m_cGuildRaidTemplet.GetStageIndex()));
		}
		float num = NKCGuildCoopManager.m_BossData.remainHp / NKCGuildCoopManager.m_BossMaxHp;
		if (num.IsNearlyZero())
		{
			m_imgBossHP.fillAmount = 0f;
		}
		else
		{
			m_imgBossHP.fillAmount = num;
		}
		NKCUtil.SetLabelText(m_lbRemainHP, string.Format("{0} ({1:0.##}%)", ((int)NKCGuildCoopManager.m_BossData.remainHp).ToString("N0"), num * 100f));
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(raidTemplet.GetStageId());
		if (dungeonTempletBase != null)
		{
			NKCUtil.SetLabelText(m_lbName, dungeonTempletBase.GetDungeonName());
		}
		NKCUtil.SetGameobjectActive(m_btnGuide, !string.IsNullOrEmpty(raidTemplet.GetGuideShortCut()));
		m_Artifact.SetData(NKCGuildCoopManager.GetMyArtifactDictionary());
		if (m_csbtnClear != null)
		{
			if (NKCGuildCoopManager.m_BossData.playCount > 0)
			{
				m_csbtnClear.UnLock();
			}
			else
			{
				m_csbtnClear.Lock();
			}
			NKCUtil.SetGameobjectActive(m_objRemainCount, NKCGuildCoopManager.m_BossData.playCount > 0);
			NKCUtil.SetLabelText(m_lbRemainCount, string.Format(NKCUtilString.GET_STRING_RAID_REMAIN_COUNT_ONE_PARAM, NKCGuildCoopManager.m_BossData.playCount));
		}
	}

	private void OnClickAttackBtn()
	{
		if (!m_csbtnClear.m_bLock)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MOVE_TO_BOSS_ELIMINATE, delegate
			{
				m_dOnClickAttackBtn?.Invoke(0L, null, 0, 0, bIsTryAssist: false, isPracticeMode: false);
			});
		}
		else
		{
			NKCPacketHandlers.Check_NKM_ERROR_CODE(NKM_ERROR_CODE.NEC_FAIL_GUILD_DUNGEON_BOSS_PLAYABLE);
		}
	}

	private void OnClickGuide()
	{
		if (GetRaidTemplet() != null && !string.IsNullOrEmpty(GetRaidTemplet().GetGuideShortCut()))
		{
			NKCUIPopupTutorialImagePanel.Instance.Open(GetRaidTemplet().GetGuideShortCut(), null);
		}
	}

	private void OnClickInfoBtn()
	{
		NKCPopupGuildCoopBossInfoDetail.Instance.Open();
	}

	private void OnClickPracticeBtn()
	{
		m_dOnClickAttackBtn?.Invoke(0L, null, 0, 0, bIsTryAssist: false, isPracticeMode: true);
	}

	private void OnClickEnemy()
	{
		GuildRaidTemplet raidTemplet = GetRaidTemplet();
		if (raidTemplet != null)
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(raidTemplet.GetStageId());
			NKCPopupEnemyList.Instance.Open(dungeonTempletBase);
		}
	}
}
