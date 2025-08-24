using System.Collections.Generic;
using Cs.Core.Util;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildRankSeasonSelect : NKCUIBase
{
	public delegate void OnSelectSeason(int seasonId);

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_POPUP_RANKING_SELECT";

	private static NKCPopupGuildRankSeasonSelect m_Instance;

	public NKCPopupGuildRankSeasonSelectSlot m_pfbSlot;

	public NKCUIComStateButton m_btnClose;

	public LoopScrollRect m_loop;

	public Transform m_trSlotParent;

	private Stack<NKCPopupGuildRankSeasonSelectSlot> m_stkSlot = new Stack<NKCPopupGuildRankSeasonSelectSlot>();

	private List<GuildSeasonTemplet> m_lstTemplet = new List<GuildSeasonTemplet>();

	private int m_selectedSeasonId;

	private OnSelectSeason m_dOnSelectSeason;

	public static NKCPopupGuildRankSeasonSelect Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildRankSeasonSelect>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_POPUP_RANKING_SELECT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupGuildRankSeasonSelect>();
				m_Instance.InitUI();
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

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public void InitUI()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loop);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private RectTransform GetObject(int idx)
	{
		NKCPopupGuildRankSeasonSelectSlot nKCPopupGuildRankSeasonSelectSlot = null;
		if (m_stkSlot.Count > 0)
		{
			nKCPopupGuildRankSeasonSelectSlot = m_stkSlot.Pop();
		}
		else
		{
			nKCPopupGuildRankSeasonSelectSlot = Object.Instantiate(m_pfbSlot);
			nKCPopupGuildRankSeasonSelectSlot.InitUI();
		}
		nKCPopupGuildRankSeasonSelectSlot.transform.SetParent(m_trSlotParent);
		NKCUtil.SetGameobjectActive(nKCPopupGuildRankSeasonSelectSlot, bValue: false);
		return nKCPopupGuildRankSeasonSelectSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.gameObject.transform);
		NKCPopupGuildRankSeasonSelectSlot component = tr.GetComponent<NKCPopupGuildRankSeasonSelectSlot>();
		m_stkSlot.Push(component);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCPopupGuildRankSeasonSelectSlot component = tr.GetComponent<NKCPopupGuildRankSeasonSelectSlot>();
		if (component == null)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		tr.SetParent(m_trSlotParent);
		NKCUtil.SetGameobjectActive(tr, bValue: true);
		component.SetData(m_lstTemplet[idx].Key, NKCStringTable.GetString(m_lstTemplet[idx].GetSeasonNameID()), m_lstTemplet[idx].Key == m_selectedSeasonId, OnClickSlot);
	}

	public void Open(OnSelectSeason dOnSelectSeason, int selectedSeasonId)
	{
		m_dOnSelectSeason = dOnSelectSeason;
		m_selectedSeasonId = selectedSeasonId;
		m_lstTemplet.Clear();
		foreach (GuildSeasonTemplet value in GuildSeasonTemplet.Values)
		{
			if (value.GetSeasonStartDate() < ServiceTime.Recent)
			{
				m_lstTemplet.Add(value);
			}
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loop.TotalCount = m_lstTemplet.Count;
		m_loop.SetIndexPosition(0);
		m_loop.RefreshCells();
		UIOpened();
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnClickSlot(int seasonId)
	{
		Close();
		m_dOnSelectSeason?.Invoke(seasonId);
	}
}
