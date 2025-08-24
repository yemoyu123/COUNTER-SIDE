using System.Collections.Generic;
using ClientPacket.Guild;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildCoopStatus : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM_COOP";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_CONSORTIUM_COOP_STATUS";

	private static NKCPopupGuildCoopStatus m_Instance;

	public NKCPopupGuildCoopStatusSlot m_pfbSlot;

	public Image m_imgTopBanner;

	public NKCUIComStateButton m_btnClose;

	public LoopScrollRect m_loop;

	public Transform m_trSlotParent;

	private Stack<NKCPopupGuildCoopStatusSlot> m_stkSlot = new Stack<NKCPopupGuildCoopStatusSlot>();

	private List<GuildDungeonMemberInfo> m_lstMemberInfo = new List<GuildDungeonMemberInfo>();

	public static NKCPopupGuildCoopStatus Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildCoopStatus>("AB_UI_NKM_UI_CONSORTIUM_COOP", "NKM_UI_POPUP_CONSORTIUM_COOP_STATUS", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null).GetInstance<NKCPopupGuildCoopStatus>();
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
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loop);
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
		NKCPopupGuildCoopStatusSlot nKCPopupGuildCoopStatusSlot = null;
		if (m_stkSlot.Count > 0)
		{
			nKCPopupGuildCoopStatusSlot = m_stkSlot.Pop();
		}
		else
		{
			nKCPopupGuildCoopStatusSlot = Object.Instantiate(m_pfbSlot);
			nKCPopupGuildCoopStatusSlot.InitUI();
		}
		nKCPopupGuildCoopStatusSlot.transform.SetParent(m_trSlotParent);
		return nKCPopupGuildCoopStatusSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCPopupGuildCoopStatusSlot component = tr.GetComponent<NKCPopupGuildCoopStatusSlot>();
		if (component == null)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(component, bValue: false);
		component.transform.SetParent(base.transform);
		m_stkSlot.Push(component);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCPopupGuildCoopStatusSlot component = tr.GetComponent<NKCPopupGuildCoopStatusSlot>();
		if (component == null || m_lstMemberInfo.Count < idx)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		component.transform.SetParent(m_trSlotParent);
		NKCUtil.SetGameobjectActive(component, bValue: true);
		component.SetData(m_lstMemberInfo[idx], idx + 1);
	}

	public void Open()
	{
		m_lstMemberInfo = NKCGuildCoopManager.GetGuildDungeonMemberInfo();
		m_lstMemberInfo.Sort(NKCGuildCoopManager.CompMember);
		GuildSeasonTemplet guildSeasonTemplet = GuildDungeonTempletManager.GetGuildSeasonTemplet(NKCGuildCoopManager.m_SeasonId);
		if (guildSeasonTemplet != null)
		{
			NKCUtil.SetImageSprite(m_imgTopBanner, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_COOP_Texture", guildSeasonTemplet.GetSeasonBgBlurName()));
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UIOpened();
		RefreshUI(bResetScroll: true);
	}

	public void RefreshUI(bool bResetScroll = false)
	{
		m_loop.TotalCount = m_lstMemberInfo.Count;
		if (bResetScroll)
		{
			m_loop.SetIndexPosition(0);
		}
		else
		{
			m_loop.RefreshCells();
		}
	}
}
