using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildCoopBossInfoDetail : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM_COOP";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_CONSORTIUM_COOP_RAID_BOSS_INFO";

	private static NKCPopupGuildCoopBossInfoDetail m_Instance;

	public NKCPopupGuildCoopBossInfoDetailSlot m_pfbSlot;

	public NKCUIComStateButton m_btnClose;

	public Text m_lbName;

	public Image m_imgBoss;

	public LoopScrollRect m_loop;

	public Transform m_trParent;

	public GameObject m_ObjBossIcon;

	public GameObject m_ObjExtraBossIcon;

	private Stack<NKCPopupGuildCoopBossInfoDetailSlot> m_stkSlot = new Stack<NKCPopupGuildCoopBossInfoDetailSlot>();

	private List<GuildRaidTemplet> m_lstRaidTemplet = new List<GuildRaidTemplet>();

	public static NKCPopupGuildCoopBossInfoDetail Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildCoopBossInfoDetail>("AB_UI_NKM_UI_CONSORTIUM_COOP", "NKM_UI_POPUP_CONSORTIUM_COOP_RAID_BOSS_INFO", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CleanupInstance).GetInstance<NKCPopupGuildCoopBossInfoDetail>();
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

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		if (m_btnClose != null)
		{
			m_btnClose.PointerClick.RemoveAllListeners();
			m_btnClose.PointerClick.AddListener(base.Close);
		}
		if (m_loop != null)
		{
			m_loop.dOnGetObject += GetObject;
			m_loop.dOnReturnObject += ReturnObject;
			m_loop.dOnProvideData += ProvideData;
			m_loop.PrepareCells();
			NKCUtil.SetScrollHotKey(m_loop);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private RectTransform GetObject(int idx)
	{
		NKCPopupGuildCoopBossInfoDetailSlot nKCPopupGuildCoopBossInfoDetailSlot = null;
		nKCPopupGuildCoopBossInfoDetailSlot = ((m_stkSlot.Count <= 0) ? Object.Instantiate(m_pfbSlot) : m_stkSlot.Pop());
		nKCPopupGuildCoopBossInfoDetailSlot.transform.SetParent(m_trParent);
		return nKCPopupGuildCoopBossInfoDetailSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
		NKCPopupGuildCoopBossInfoDetailSlot component = tr.GetComponent<NKCPopupGuildCoopBossInfoDetailSlot>();
		if (!(component == null))
		{
			m_stkSlot.Push(component);
		}
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCPopupGuildCoopBossInfoDetailSlot component = tr.GetComponent<NKCPopupGuildCoopBossInfoDetailSlot>();
		if (component == null || m_lstRaidTemplet.Count <= idx)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_lstRaidTemplet[idx].GetStageId());
		if (dungeonTempletBase == null)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
		}
		else
		{
			component.SetData(m_lstRaidTemplet[idx], dungeonTempletBase, idx == 15);
		}
	}

	public void Open()
	{
		m_lstRaidTemplet = GuildDungeonTempletManager.GetRaidTempletList(NKCGuildCoopManager.m_cGuildRaidTemplet.GetSeasonRaidGrouop());
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_lstRaidTemplet[NKCGuildCoopManager.m_cGuildRaidTemplet.GetStageIndex() - 1].GetStageId());
		if (dungeonTempletBase == null)
		{
			Log.Error($"dungeonTempletBase is null - id : {m_lstRaidTemplet[NKCGuildCoopManager.m_cGuildRaidTemplet.GetStageIndex() - 1].GetStageId()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/GuildCoop/NKCPopupGuildCoopBossInfoDetail.cs", 149);
			return;
		}
		NKCUtil.SetLabelText(m_lbName, dungeonTempletBase.GetDungeonName());
		NKCUtil.SetImageSprite(m_imgBoss, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UNIT_FACE_CARD", dungeonTempletBase.m_DungeonIcon));
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		int num = m_lstRaidTemplet.Max((GuildRaidTemplet e) => e.GetStageIndex());
		bool flag = NKCGuildCoopManager.m_cGuildRaidTemplet.GetStageIndex() == num;
		NKCUtil.SetGameobjectActive(m_ObjBossIcon, !flag);
		NKCUtil.SetGameobjectActive(m_ObjExtraBossIcon, flag);
		m_loop.TotalCount = m_lstRaidTemplet.Count;
		m_loop.SetIndexPosition(0);
		UIOpened();
	}
}
