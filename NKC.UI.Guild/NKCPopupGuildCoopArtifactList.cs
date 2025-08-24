using System.Collections.Generic;
using ClientPacket.Guild;
using Cs.Logging;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildCoopArtifactList : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM_COOP";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_CONSORTIUM_COOP_ARITFACT_LIST";

	private static NKCPopupGuildCoopArtifactList m_Instance;

	public NKCPopupGuildCoopArtifactListSlot m_pfbSlot;

	public Text m_lbTitle;

	public Text m_lbArtifactCount;

	public NKCUIComStateButton m_btnClose;

	[Header("목표 설정 관련")]
	public GameObject m_objFlagParent;

	public NKCUIComToggle m_tglFlagSetting;

	public GameObject m_objButtonParent;

	public NKCUIComStateButton m_btnFlagReset;

	public NKCUIComStateButton m_btnFlagCancel;

	public NKCUIComStateButton m_btnFlagOK;

	[Space]
	public LoopScrollRect m_loop;

	public Transform m_trParent;

	private Stack<NKCPopupGuildCoopArtifactListSlot> m_stkSlot = new Stack<NKCPopupGuildCoopArtifactListSlot>();

	private List<GuildDungeonArtifactTemplet> m_lstTemplet = new List<GuildDungeonArtifactTemplet>();

	private int m_ArenaIndex;

	private int m_CurrentClearCount;

	private int m_FlagIndex;

	private bool m_bFlagSetting;

	public static NKCPopupGuildCoopArtifactList Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildCoopArtifactList>("AB_UI_NKM_UI_CONSORTIUM_COOP", "NKM_UI_POPUP_CONSORTIUM_COOP_ARITFACT_LIST", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null).GetInstance<NKCPopupGuildCoopArtifactList>();
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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	public void InitUI()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_tglFlagSetting.OnValueChanged.RemoveAllListeners();
		m_tglFlagSetting.OnValueChanged.AddListener(OnFlagTgl);
		m_btnFlagReset.PointerClick.RemoveAllListeners();
		m_btnFlagReset.PointerClick.AddListener(OnFlagReset);
		m_btnFlagCancel.PointerClick.RemoveAllListeners();
		m_btnFlagCancel.PointerClick.AddListener(OnFlagCancel);
		m_btnFlagOK.PointerClick.RemoveAllListeners();
		m_btnFlagOK.PointerClick.AddListener(OnFlagOK);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loop);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void CloseInternal()
	{
		m_bFlagSetting = false;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private RectTransform GetObject(int idx)
	{
		NKCPopupGuildCoopArtifactListSlot nKCPopupGuildCoopArtifactListSlot = null;
		if (m_stkSlot.Count > 0)
		{
			nKCPopupGuildCoopArtifactListSlot = m_stkSlot.Pop();
		}
		else
		{
			nKCPopupGuildCoopArtifactListSlot = Object.Instantiate(m_pfbSlot);
			nKCPopupGuildCoopArtifactListSlot.Init(OnClickSlot);
		}
		nKCPopupGuildCoopArtifactListSlot.transform.SetParent(m_trParent);
		return nKCPopupGuildCoopArtifactListSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
		tr.GetComponent<NKCPopupGuildCoopArtifactListSlot>();
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCPopupGuildCoopArtifactListSlot component = tr.GetComponent<NKCPopupGuildCoopArtifactListSlot>();
		if (component == null)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		tr.SetParent(m_trParent);
		NKCUtil.SetGameobjectActive(component, bValue: true);
		component.SetData(m_lstTemplet[idx], idx);
		component.SetClear(idx < m_CurrentClearCount);
		component.SetCurrent(idx == m_CurrentClearCount);
		component.SetFlag(idx <= m_FlagIndex, m_bFlagSetting);
		component.m_slot.GetComponent<NKCUIComRaycastTarget>().enabled = false;
	}

	public void Open(GuildDungeonInfoTemplet guildDungeonInfoTemplet, int clearCount, int flagIndex = -1)
	{
		m_bFlagSetting = false;
		m_CurrentClearCount = clearCount;
		m_FlagIndex = flagIndex;
		m_ArenaIndex = guildDungeonInfoTemplet.GetArenaIndex();
		m_lstTemplet = GuildDungeonTempletManager.GetDungeonArtifactList(guildDungeonInfoTemplet.GetStageRewardArtifactGroup());
		if (m_lstTemplet == null || m_lstTemplet.Count == 0)
		{
			m_lstTemplet = new List<GuildDungeonArtifactTemplet>();
			Log.Error($"ArtifactCount is 0 - id : {guildDungeonInfoTemplet.GetStageRewardArtifactGroup()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/GuildCoop/NKCPopupGuildCoopArtifactList.cs", 176);
		}
		NKCUtil.SetLabelText(m_lbTitle, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_ARTIFACT_DUNGEON_POPUP_TITLE, guildDungeonInfoTemplet.GetArenaIndex()));
		NKCUtil.SetLabelText(m_lbArtifactCount, $"<color=#FFCF3B>{m_CurrentClearCount}</color>/{m_lstTemplet.Count}");
		m_bFlagSetting = false;
		NKCUtil.SetGameobjectActive(m_objButtonParent, m_bFlagSetting);
		m_tglFlagSetting.Select(m_bFlagSetting, bForce: true);
		NKCUtil.SetGameobjectActive(m_objFlagParent, NKCGuildManager.GetMyGuildGrade() <= GuildMemberGrade.Staff);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loop.TotalCount = m_lstTemplet.Count;
		m_loop.SetIndexPosition(Mathf.Min(m_loop.TotalCount, m_CurrentClearCount));
		UIOpened();
	}

	private void OnFlagTgl(bool bValue)
	{
		m_bFlagSetting = bValue;
		Refresh();
	}

	private void OnFlagReset()
	{
		m_bFlagSetting = false;
		GuildDungeonArena guildDungeonArena = NKCGuildCoopManager.m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == m_ArenaIndex);
		if (guildDungeonArena != null && guildDungeonArena.flagIndex >= 0)
		{
			NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_FLAG_REQ(NKCGuildManager.GetMyGuildSimpleData().guildUid, m_ArenaIndex, -1);
		}
		else
		{
			Refresh();
		}
	}

	private void OnFlagCancel()
	{
		m_bFlagSetting = false;
		GuildDungeonArena guildDungeonArena = NKCGuildCoopManager.m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == m_ArenaIndex);
		if (guildDungeonArena != null && guildDungeonArena.flagIndex != m_FlagIndex)
		{
			m_FlagIndex = guildDungeonArena.flagIndex;
		}
		Refresh();
	}

	private void OnFlagOK()
	{
		m_bFlagSetting = false;
		GuildDungeonArena guildDungeonArena = NKCGuildCoopManager.m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == m_ArenaIndex);
		if (guildDungeonArena != null && guildDungeonArena.flagIndex != m_FlagIndex)
		{
			NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_FLAG_REQ(NKCGuildManager.GetMyGuildSimpleData().guildUid, m_ArenaIndex, m_FlagIndex);
		}
		else
		{
			Refresh();
		}
	}

	public void OnFlagChanged()
	{
		m_FlagIndex = NKCGuildCoopManager.m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == m_ArenaIndex).flagIndex;
		Refresh();
	}

	public void Refresh()
	{
		m_FlagIndex = NKCGuildCoopManager.m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == m_ArenaIndex).flagIndex;
		m_loop.RefreshCells();
		m_tglFlagSetting.Select(m_bFlagSetting, bForce: true);
		NKCUtil.SetGameobjectActive(m_objButtonParent, m_bFlagSetting);
	}

	private void OnClickSlot(int slotIdx)
	{
		if (m_bFlagSetting)
		{
			m_FlagIndex = slotIdx;
			m_loop.RefreshCells();
		}
	}
}
