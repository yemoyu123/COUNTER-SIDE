using System;
using System.Collections.Generic;
using ClientPacket.Mode;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupShadowRecord : NKCUIBase
{
	public delegate void OnClose();

	private const string BUNDLE_NAME = "AB_UI_OPERATION_SHADOW";

	private const string ASSET_NAME = "AB_UI_OPERATION_SHADOW_PALACE_RECORD_POPUP";

	private static NKCPopupShadowRecord m_Instance;

	public Text m_txtTitle;

	public Text m_txtTotalTime;

	public Text m_txtBestTIme;

	public GameObject m_objNewRecord;

	public LoopScrollRect m_scrollRect;

	public NKCPopupShadowRecordSlot m_slotPrefab;

	public Animator m_animator;

	public EventTrigger m_eventTrigger;

	private Stack<NKCPopupShadowRecordSlot> m_stkSlotPool = new Stack<NKCPopupShadowRecordSlot>();

	private List<NKMPalaceDungeonData> m_lstDungeonData = new List<NKMPalaceDungeonData>();

	private int m_palaceID;

	private OnClose dOnClose;

	public static NKCPopupShadowRecord Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupShadowRecord>("AB_UI_OPERATION_SHADOW", "AB_UI_OPERATION_SHADOW_PALACE_RECORD_POPUP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupShadowRecord>();
				m_Instance.Init();
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

	public override string MenuName => "shadow palace record popup";

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void Init()
	{
		if (m_scrollRect != null)
		{
			m_scrollRect.dOnGetObject += OnGetObject;
			m_scrollRect.dOnProvideData += OnProvideData;
			m_scrollRect.dOnReturnObject += OnReturnObject;
			m_scrollRect.ContentConstraintCount = 1;
			m_scrollRect.PrepareCells();
		}
		if (m_eventTrigger != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnTouchBG);
			m_eventTrigger.triggers.Clear();
			m_eventTrigger.triggers.Add(entry);
		}
	}

	public void Open(NKMShadowGameResult shadowResult, List<int> bestTime, OnClose onClose)
	{
		if (shadowResult == null || shadowResult.life == 0)
		{
			return;
		}
		m_palaceID = shadowResult.palaceId;
		dOnClose = onClose;
		NKMPalaceData nKMPalaceData = NKCScenManager.CurrentUserData().m_ShadowPalace.palaceDataList.Find((NKMPalaceData v) => v.palaceId == m_palaceID);
		if (nKMPalaceData == null)
		{
			return;
		}
		NKMShadowPalaceTemplet palaceTemplet = NKMShadowPalaceManager.GetPalaceTemplet(m_palaceID);
		if (palaceTemplet == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_txtTitle, NKCUtilString.GET_SHADOW_RECORD_POPUP_TITLE, palaceTemplet.PALACE_NUM_UI, palaceTemplet.PalaceName);
		List<NKMShadowBattleTemplet> battleTemplets = NKMShadowPalaceManager.GetBattleTemplets(m_palaceID);
		m_lstDungeonData = nKMPalaceData.dungeonDataList;
		m_lstDungeonData.Sort(delegate(NKMPalaceDungeonData a, NKMPalaceDungeonData b)
		{
			NKMShadowBattleTemplet nKMShadowBattleTemplet = battleTemplets.Find((NKMShadowBattleTemplet v) => v.DUNGEON_ID == a.dungeonId);
			NKMShadowBattleTemplet nKMShadowBattleTemplet2 = battleTemplets.Find((NKMShadowBattleTemplet v) => v.DUNGEON_ID == b.dungeonId);
			if (nKMShadowBattleTemplet == null)
			{
				return 1;
			}
			return (nKMShadowBattleTemplet2 == null) ? (-1) : nKMShadowBattleTemplet.BATTLE_ORDER.CompareTo(nKMShadowBattleTemplet2.BATTLE_ORDER);
		});
		m_scrollRect.TotalCount = m_lstDungeonData.Count;
		m_scrollRect.RefreshCells();
		int num = 0;
		int num2 = 0;
		for (int num3 = 0; num3 < m_lstDungeonData.Count; num3++)
		{
			num += m_lstDungeonData[num3].recentTime;
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(num);
		NKCUtil.SetLabelText(m_txtTotalTime, NKCUtilString.GetTimeSpanString(timeSpan));
		string msg = "-:--:--";
		if (bestTime.Count == m_lstDungeonData.Count)
		{
			for (int num4 = 0; num4 < bestTime.Count; num4++)
			{
				num2 += bestTime[num4];
			}
			if (num2 > 0)
			{
				msg = NKCUtilString.GetTimeSpanString(TimeSpan.FromSeconds(num2));
			}
		}
		NKCUtil.SetLabelText(m_txtBestTIme, msg);
		NKCUtil.SetGameobjectActive(m_objNewRecord, shadowResult.newRecord);
		UIOpened();
		m_animator.Play("NKM_UI_SHADOW_PALACE_RECORD_POPUP_INTRO");
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (dOnClose != null)
		{
			dOnClose();
		}
	}

	private RectTransform OnGetObject(int index)
	{
		if (m_stkSlotPool.Count > 0)
		{
			return m_stkSlotPool.Pop().GetComponent<RectTransform>();
		}
		NKCPopupShadowRecordSlot nKCPopupShadowRecordSlot = UnityEngine.Object.Instantiate(m_slotPrefab);
		nKCPopupShadowRecordSlot.transform.SetParent(m_scrollRect.content);
		return nKCPopupShadowRecordSlot.GetComponent<RectTransform>();
	}

	private void OnProvideData(Transform tr, int idx)
	{
		NKCPopupShadowRecordSlot component = tr.GetComponent<NKCPopupShadowRecordSlot>();
		if (!(component == null))
		{
			NKMPalaceDungeonData dungeonData = m_lstDungeonData[idx];
			NKMShadowBattleTemplet templet = NKMShadowPalaceManager.GetBattleTemplets(m_palaceID).Find((NKMShadowBattleTemplet v) => v.DUNGEON_ID == dungeonData.dungeonId);
			component.SetData(templet, dungeonData);
		}
	}

	private void OnReturnObject(Transform go)
	{
		if (!(GetComponent<NKCPopupShadowRecordSlot>() != null))
		{
			NKCUtil.SetGameobjectActive(go, bValue: false);
			go.SetParent(base.transform);
			m_stkSlotPool.Push(go.GetComponent<NKCPopupShadowRecordSlot>());
		}
	}

	private void OnTouchBG(BaseEventData eventData)
	{
		Close();
	}
}
