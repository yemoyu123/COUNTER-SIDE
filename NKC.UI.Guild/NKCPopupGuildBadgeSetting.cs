using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildBadgeSetting : NKCUIBase
{
	public delegate void OnClose(long badgeId);

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_POPUP_MARK_SETTING";

	private static NKCPopupGuildBadgeSetting m_Instance;

	public NKCUIGuildBadge m_BadgeUI;

	public LoopScrollRect m_loopFrame;

	public Transform m_trFrameContentsParent;

	public NKCUIComToggleGroup m_tglGroupFrame;

	public LoopScrollRect m_loopFrameColor;

	public Transform m_trFrameColorContentsParent;

	public NKCUIComToggleGroup m_tglGroupFrameColor;

	public LoopScrollRect m_loopMark;

	public Transform m_trMarkContentsParent;

	public NKCUIComToggleGroup m_tglGroupMark;

	public LoopScrollRect m_loopMarkColor;

	public Transform m_trMarkColorContentsParent;

	public NKCUIComToggleGroup m_tglGroupMarkColor;

	public NKCUIComStateButton m_btnOK;

	public NKCUIComStateButton m_btnCancel;

	private Stack<NKCUIGuildBadgeSlot> m_stkSlot = new Stack<NKCUIGuildBadgeSlot>();

	private List<NKCUIGuildBadgeSlot> m_lstVisibleSlot = new List<NKCUIGuildBadgeSlot>();

	private OnClose m_dOnClose;

	private GuildBadgeInfo m_GuildBadgeInfo;

	public static NKCPopupGuildBadgeSetting Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildBadgeSetting>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_POPUP_MARK_SETTING", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CleanupInstance).GetInstance<NKCPopupGuildBadgeSetting>();
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

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		for (int i = 0; i < m_lstVisibleSlot.Count; i++)
		{
			m_stkSlot.Push(m_lstVisibleSlot[i]);
		}
		m_lstVisibleSlot.Clear();
	}

	public RectTransform GetObjectFrame(int index)
	{
		NKCUIGuildBadgeSlot nKCUIGuildBadgeSlot = null;
		nKCUIGuildBadgeSlot = ((m_stkSlot.Count <= 0) ? NKCUIGuildBadgeSlot.GetNewInstance(m_trFrameContentsParent, m_tglGroupFrame, OnChangeFrame) : m_stkSlot.Pop());
		m_lstVisibleSlot.Add(nKCUIGuildBadgeSlot);
		NKCUtil.SetGameobjectActive(nKCUIGuildBadgeSlot, bValue: false);
		return nKCUIGuildBadgeSlot?.GetComponent<RectTransform>();
	}

	public void ReturnObjectFrame(Transform tr)
	{
		NKCUIGuildBadgeSlot component = tr.GetComponent<NKCUIGuildBadgeSlot>();
		m_lstVisibleSlot.Remove(component);
		m_stkSlot.Push(component);
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
	}

	public void ProvideDataFrame(Transform tr, int idx)
	{
		NKCUIGuildBadgeSlot component = tr.GetComponent<NKCUIGuildBadgeSlot>();
		if (!(component == null))
		{
			component.SetData(NKCGuildManager.GetFrameTempletByIndex(idx));
			if (component.m_slotId == m_GuildBadgeInfo.FrameId)
			{
				component.m_tgl.Select(bSelect: true);
			}
			else
			{
				component.m_tgl.Select(bSelect: false);
			}
		}
	}

	public RectTransform GetObjectFrameColor(int index)
	{
		NKCUIGuildBadgeSlot nKCUIGuildBadgeSlot = null;
		nKCUIGuildBadgeSlot = ((m_stkSlot.Count <= 0) ? NKCUIGuildBadgeSlot.GetNewInstance(m_trFrameContentsParent, m_tglGroupFrameColor, OnChangeFrameColor) : m_stkSlot.Pop());
		m_lstVisibleSlot.Add(nKCUIGuildBadgeSlot);
		NKCUtil.SetGameobjectActive(nKCUIGuildBadgeSlot, bValue: false);
		return nKCUIGuildBadgeSlot?.GetComponent<RectTransform>();
	}

	public void ReturnObjectFrameColor(Transform tr)
	{
		NKCUIGuildBadgeSlot component = tr.GetComponent<NKCUIGuildBadgeSlot>();
		m_lstVisibleSlot.Remove(component);
		m_stkSlot.Push(component);
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
	}

	public void ProvideDataFrameColor(Transform tr, int idx)
	{
		NKCUIGuildBadgeSlot component = tr.GetComponent<NKCUIGuildBadgeSlot>();
		if (!(component == null))
		{
			component.SetData(NKCGuildManager.GetBadgeColorTempletByIndex(idx));
			if (component.m_slotId == m_GuildBadgeInfo.FrameColorId)
			{
				component.m_tgl.Select(bSelect: true);
			}
			else
			{
				component.m_tgl.Select(bSelect: false);
			}
		}
	}

	public RectTransform GetObjectMark(int index)
	{
		NKCUIGuildBadgeSlot nKCUIGuildBadgeSlot = null;
		nKCUIGuildBadgeSlot = ((m_stkSlot.Count <= 0) ? NKCUIGuildBadgeSlot.GetNewInstance(m_trFrameContentsParent, m_tglGroupMark, OnChangeMark) : m_stkSlot.Pop());
		m_lstVisibleSlot.Add(nKCUIGuildBadgeSlot);
		NKCUtil.SetGameobjectActive(nKCUIGuildBadgeSlot, bValue: false);
		return nKCUIGuildBadgeSlot?.GetComponent<RectTransform>();
	}

	public void ReturnObjectMark(Transform tr)
	{
		NKCUIGuildBadgeSlot component = tr.GetComponent<NKCUIGuildBadgeSlot>();
		m_lstVisibleSlot.Remove(component);
		m_stkSlot.Push(component);
	}

	public void ProvideDataMark(Transform tr, int idx)
	{
		NKCUIGuildBadgeSlot component = tr.GetComponent<NKCUIGuildBadgeSlot>();
		if (!(component == null))
		{
			component.SetData(NKCGuildManager.GetMarkTempletByIndex(idx));
			if (component.m_slotId == m_GuildBadgeInfo.MarkId)
			{
				component.m_tgl.Select(bSelect: true);
			}
			else
			{
				component.m_tgl.Select(bSelect: false);
			}
		}
	}

	public RectTransform GetObjectMarkColor(int index)
	{
		NKCUIGuildBadgeSlot nKCUIGuildBadgeSlot = null;
		nKCUIGuildBadgeSlot = ((m_stkSlot.Count <= 0) ? NKCUIGuildBadgeSlot.GetNewInstance(m_trFrameContentsParent, m_tglGroupMarkColor, OnChangeMarkColor) : m_stkSlot.Pop());
		m_lstVisibleSlot.Add(nKCUIGuildBadgeSlot);
		NKCUtil.SetGameobjectActive(nKCUIGuildBadgeSlot, bValue: false);
		return nKCUIGuildBadgeSlot?.GetComponent<RectTransform>();
	}

	public void ReturnObjectMarkColor(Transform tr)
	{
		NKCUIGuildBadgeSlot component = tr.GetComponent<NKCUIGuildBadgeSlot>();
		m_lstVisibleSlot.Remove(component);
		m_stkSlot.Push(component);
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
	}

	public void ProvideDataMarkColor(Transform tr, int idx)
	{
		NKCUIGuildBadgeSlot component = tr.GetComponent<NKCUIGuildBadgeSlot>();
		if (!(component == null))
		{
			component.SetData(NKCGuildManager.GetBadgeColorTempletByIndex(idx));
			if (component.m_slotId == m_GuildBadgeInfo.MarkColorId)
			{
				component.m_tgl.Select(bSelect: true);
			}
			else
			{
				component.m_tgl.Select(bSelect: false);
			}
		}
	}

	public void InitUI()
	{
		m_BadgeUI.InitUI();
		m_btnOK.PointerClick.RemoveAllListeners();
		m_btnOK.PointerClick.AddListener(OnClickOk);
		NKCUtil.SetHotkey(m_btnOK, HotkeyEventType.Confirm);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(base.Close);
		m_loopFrame.dOnGetObject += GetObjectFrame;
		m_loopFrame.dOnReturnObject += ReturnObjectFrame;
		m_loopFrame.dOnProvideData += ProvideDataFrame;
		m_loopFrame.PrepareCells();
		m_loopFrameColor.dOnGetObject += GetObjectFrameColor;
		m_loopFrameColor.dOnReturnObject += ReturnObjectFrameColor;
		m_loopFrameColor.dOnProvideData += ProvideDataFrameColor;
		m_loopFrameColor.PrepareCells();
		m_loopMark.dOnGetObject += GetObjectMark;
		m_loopMark.dOnReturnObject += ReturnObjectMark;
		m_loopMark.dOnProvideData += ProvideDataMark;
		m_loopMark.PrepareCells();
		m_loopMarkColor.dOnGetObject += GetObjectMarkColor;
		m_loopMarkColor.dOnReturnObject += ReturnObjectMarkColor;
		m_loopMarkColor.dOnProvideData += ProvideDataMarkColor;
		m_loopMarkColor.PrepareCells();
		m_GuildBadgeInfo = new GuildBadgeInfo(0L);
	}

	public void Open(OnClose onClose, long badgeId = 0L)
	{
		m_dOnClose = onClose;
		m_GuildBadgeInfo = new GuildBadgeInfo(badgeId);
		m_BadgeUI.SetData(m_GuildBadgeInfo);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loopFrame.TotalCount = NKMTempletContainer<NKMGuildBadgeFrameTemplet>.Values.Count();
		m_loopFrame.RefreshCells();
		m_loopFrameColor.TotalCount = NKMTempletContainer<NKMGuildBadgeColorTemplet>.Values.Count();
		m_loopFrameColor.RefreshCells();
		m_loopMark.TotalCount = NKMTempletContainer<NKMGuildBadgeMarkTemplet>.Values.Count();
		m_loopMark.RefreshCells();
		m_loopMarkColor.TotalCount = NKMTempletContainer<NKMGuildBadgeColorTemplet>.Values.Count();
		m_loopMarkColor.RefreshCells();
		UIOpened();
	}

	private void OnChangeFrame(int frameId)
	{
		if (frameId > 0)
		{
			m_GuildBadgeInfo = new GuildBadgeInfo(frameId, m_GuildBadgeInfo.FrameColorId, m_GuildBadgeInfo.MarkId, m_GuildBadgeInfo.MarkColorId);
			m_BadgeUI.SetData(m_GuildBadgeInfo);
		}
	}

	private void OnChangeFrameColor(int frameColorId)
	{
		if (frameColorId > 0)
		{
			m_GuildBadgeInfo = new GuildBadgeInfo(m_GuildBadgeInfo.FrameId, frameColorId, m_GuildBadgeInfo.MarkId, m_GuildBadgeInfo.MarkColorId);
			m_BadgeUI.SetData(m_GuildBadgeInfo);
		}
	}

	private void OnChangeMark(int markId)
	{
		if (markId > 0)
		{
			m_GuildBadgeInfo = new GuildBadgeInfo(m_GuildBadgeInfo.FrameId, m_GuildBadgeInfo.FrameColorId, markId, m_GuildBadgeInfo.MarkColorId);
			m_BadgeUI.SetData(m_GuildBadgeInfo);
		}
	}

	private void OnChangeMarkColor(int markColorId)
	{
		if (markColorId > 0)
		{
			m_GuildBadgeInfo = new GuildBadgeInfo(m_GuildBadgeInfo.FrameId, m_GuildBadgeInfo.FrameColorId, m_GuildBadgeInfo.MarkId, markColorId);
			m_BadgeUI.SetData(m_GuildBadgeInfo);
		}
	}

	public void OnClickOk()
	{
		if (m_GuildBadgeInfo.BadgeId > 0)
		{
			m_dOnClose?.Invoke(m_GuildBadgeInfo.BadgeId);
			Close();
		}
	}
}
