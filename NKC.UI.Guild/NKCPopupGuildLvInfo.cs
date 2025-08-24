using System.Collections.Generic;
using System.Linq;
using NKM.Guild;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildLvInfo : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_POPUP_LV_INFO";

	private static NKCPopupGuildLvInfo m_Instance;

	public NKCPopupGuildLvInfoSlot m_pfbSlot;

	public ScrollRect m_scList;

	public Transform m_trSlotParent;

	public NKCUIComStateButton m_btnBG;

	public NKCUIComStateButton m_btnOK;

	private List<NKCPopupGuildLvInfoSlot> m_lstSlot = new List<NKCPopupGuildLvInfoSlot>();

	public static NKCPopupGuildLvInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildLvInfo>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_POPUP_LV_INFO", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CleanupInstance).GetInstance<NKCPopupGuildLvInfo>();
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
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			Object.Destroy(m_lstSlot[i].gameObject);
		}
		m_lstSlot.Clear();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void InitUI()
	{
		m_btnBG.PointerClick.RemoveAllListeners();
		m_btnBG.PointerClick.AddListener(base.Close);
		m_btnOK.PointerClick.RemoveAllListeners();
		m_btnOK.PointerClick.AddListener(base.Close);
		NKCUtil.SetHotkey(m_btnOK, HotkeyEventType.Confirm);
	}

	public void Open()
	{
		List<GuildExpTemplet> list = NKMTempletContainer<GuildExpTemplet>.Values.ToList();
		list.Sort(Comparer);
		for (int i = 0; i < list.Count; i++)
		{
			NKCPopupGuildLvInfoSlot nKCPopupGuildLvInfoSlot = Object.Instantiate(m_pfbSlot, m_trSlotParent);
			nKCPopupGuildLvInfoSlot.SetData(list[i]);
			m_lstSlot.Add(nKCPopupGuildLvInfoSlot);
		}
		m_scList.normalizedPosition = new Vector2(0f, 1f);
		UIOpened();
	}

	private int Comparer(GuildExpTemplet left, GuildExpTemplet right)
	{
		return left.GuildLevel.CompareTo(right.GuildLevel);
	}
}
