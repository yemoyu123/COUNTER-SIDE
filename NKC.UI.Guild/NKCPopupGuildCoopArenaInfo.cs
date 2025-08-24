using System.Collections.Generic;
using NKC.PacketHandler;
using NKM;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildCoopArenaInfo : NKCUIBase
{
	public delegate void OnClickStart(NKMDungeonTempletBase templetBase, int arenaIdx);

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM_COOP";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_CONSORTIUM_COOP_ARENA_INFO";

	private static NKCPopupGuildCoopArenaInfo m_Instance;

	public NKCUIComStateButton m_btnClose;

	[Header("타이틀")]
	public Text m_lbArenaNum;

	public Text m_lbArenaName;

	[Header("정화도")]
	public Text m_lbArenaClearPoint;

	public Image m_imgClearPoint;

	[Header("다음 아티팩트 숫자")]
	public GameObject m_objNextArtifact;

	public NKCUISlot m_slotArtifact;

	public Text m_lbNextArtifaceDesc;

	[Header("메달")]
	public NKCUIComDungeonMission m_NKCUIComDungeonMission;

	[Header("전투 환경")]
	public GameObject m_objBattleCondition;

	public Image m_imgBattleCondition;

	public Text m_lbBattleConditionName;

	public Text m_lbBattleConditionDesc;

	[Header("등장 적 리스트")]
	public NKCUIComEnemyList m_NKCUIComEnemyList;

	[Header("하단")]
	public NKCUIComStateButton m_btnStart;

	public NKCUIComStateButton m_btnShowArtifact;

	private OnClickStart m_dOnClickStart;

	private GuildDungeonInfoTemplet m_GuildDungeonInfoTemplet;

	private NKMDungeonTempletBase m_dungeonTempletBase;

	private bool m_bOpened;

	public static NKCPopupGuildCoopArenaInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildCoopArenaInfo>("AB_UI_NKM_UI_CONSORTIUM_COOP", "NKM_UI_POPUP_CONSORTIUM_COOP_ARENA_INFO", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CleanupInstance).GetInstance<NKCPopupGuildCoopArenaInfo>();
				if (m_Instance != null)
				{
					m_Instance.InitUI();
				}
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override string MenuName => "";

	public override eMenutype eUIType => eMenutype.Popup;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void InitUI()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_NKCUIComEnemyList.InitUI();
		m_btnStart.PointerClick.RemoveAllListeners();
		m_btnStart.PointerClick.AddListener(OnClickBattle);
		m_btnStart.m_bGetCallbackWhileLocked = true;
		m_btnShowArtifact.PointerClick.RemoveAllListeners();
		m_btnShowArtifact.PointerClick.AddListener(OnClickShowArtifact);
	}

	public override void CloseInternal()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().OnCloseInfoPopup();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_bOpened = false;
		m_dOnClickStart = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public void Open(GuildDungeonInfoTemplet templet, OnClickStart onClickStart)
	{
		m_GuildDungeonInfoTemplet = templet;
		m_dOnClickStart = onClickStart;
		if (templet == null)
		{
			return;
		}
		m_dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(templet.GetSeasonDungeonId());
		if (m_dungeonTempletBase == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			Debug.LogError($"dungeonTempletBase is null - {templet.GetSeasonDungeonId()}");
			return;
		}
		NKCUtil.SetLabelText(m_lbArenaNum, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_DUNGEON_UI_ARENA_INFO, templet.GetArenaIndex()));
		NKCUtil.SetLabelText(m_lbArenaName, m_dungeonTempletBase.GetDungeonName());
		m_NKCUIComDungeonMission.SetData(m_dungeonTempletBase, bTextOnly: true);
		List<NKMBattleConditionTemplet> battleConditions = m_dungeonTempletBase.BattleConditions;
		int num = 0;
		foreach (NKMBattleConditionTemplet item in battleConditions)
		{
			if (item != null && !item.m_bHide)
			{
				if (NKCUtil.GetSpriteBattleConditionICon(item) == null)
				{
					NKCUtil.SetImageSprite(m_imgBattleCondition, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_BC", "AB_UI_NKM_UI_COMMON_BC_ICON_NONE"));
				}
				else
				{
					NKCUtil.SetImageSprite(m_imgBattleCondition, NKCUtil.GetSpriteBattleConditionICon(item));
				}
				NKCUtil.SetLabelText(m_lbBattleConditionName, item.BattleCondName_Translated);
				NKCUtil.SetLabelText(m_lbBattleConditionDesc, item.BattleCondDesc_Translated);
				num++;
			}
		}
		if (num <= 0)
		{
			NKCUtil.SetImageSprite(m_imgBattleCondition, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_BC", "AB_UI_NKM_UI_COMMON_BC_ICON_NONE"));
			NKCUtil.SetLabelText(m_lbBattleConditionName, NKCUtilString.GET_STRING_NO_EXIST);
			NKCUtil.SetLabelText(m_lbBattleConditionDesc, string.Empty);
		}
		UpdateClearPoint();
		UpdateButtonState();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NKCUIComEnemyList.SetData(m_dungeonTempletBase);
		if (!m_bOpened)
		{
			UIOpened();
		}
		m_bOpened = true;
	}

	private void UpdateClearPoint()
	{
		int currentArtifactCountByArena = NKCGuildCoopManager.GetCurrentArtifactCountByArena(m_GuildDungeonInfoTemplet.GetArenaIndex());
		int count = GuildDungeonTempletManager.GetDungeonArtifactList(m_GuildDungeonInfoTemplet.GetStageRewardArtifactGroup()).Count;
		if (currentArtifactCountByArena == count)
		{
			NKCUtil.SetGameobjectActive(m_objNextArtifact, bValue: false);
			float num = 1f;
			NKCUtil.SetLabelText(m_lbArenaClearPoint, string.Format("{0}%", (num * 100f).ToString("N0")));
			m_imgClearPoint.fillAmount = num;
			return;
		}
		float clearPointPercentage = NKCGuildCoopManager.GetClearPointPercentage(m_GuildDungeonInfoTemplet.GetArenaIndex());
		NKCUtil.SetLabelText(m_lbArenaClearPoint, string.Format("{0}%", (clearPointPercentage * 100f).ToString("N0")));
		m_imgClearPoint.fillAmount = clearPointPercentage;
		int nextArtifactID = NKCGuildCoopManager.GetNextArtifactID(m_GuildDungeonInfoTemplet.GetArenaIndex());
		GuildDungeonArtifactTemplet artifactTemplet = GuildDungeonTempletManager.GetArtifactTemplet(nextArtifactID);
		if (artifactTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_objNextArtifact, bValue: true);
			m_slotArtifact.SetData(NKCUISlot.SlotData.MakeGuildArtifactData(nextArtifactID));
			NKCUtil.SetLabelText(m_lbNextArtifaceDesc, artifactTemplet.GetDescFull());
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objNextArtifact, bValue: false);
		}
	}

	private void UpdateButtonState()
	{
		if (NKCGuildCoopManager.CanStartArena(m_GuildDungeonInfoTemplet.GetArenaIndex()) == NKM_ERROR_CODE.NEC_OK)
		{
			m_btnStart.UnLock();
		}
		else
		{
			m_btnStart.Lock();
		}
	}

	public void OnClickBattle()
	{
		if (!m_btnStart.m_bLock)
		{
			m_dOnClickStart?.Invoke(m_dungeonTempletBase, m_GuildDungeonInfoTemplet.GetArenaIndex());
		}
		else
		{
			NKCPacketHandlers.Check_NKM_ERROR_CODE(NKCGuildCoopManager.CanStartArena(m_GuildDungeonInfoTemplet.GetArenaIndex()));
		}
	}

	public void OnClickShowArtifact()
	{
		int arenaIndex = m_GuildDungeonInfoTemplet.GetArenaIndex();
		NKCPopupGuildCoopArtifactList.Instance.Open(m_GuildDungeonInfoTemplet, NKCGuildCoopManager.GetCurrentArtifactCountByArena(arenaIndex), NKCGuildCoopManager.GetCurrentArtifactFlagIndex(arenaIndex));
	}

	public void Refresh()
	{
		UpdateClearPoint();
		UpdateButtonState();
	}
}
