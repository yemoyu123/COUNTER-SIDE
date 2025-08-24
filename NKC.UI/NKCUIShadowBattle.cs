using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Mode;
using ClientPacket.Warfare;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIShadowBattle : NKCUIBase
{
	private const string BUNDLE_NAME = "AB_UI_OPERATION_SHADOW";

	private const string ASSET_NAME = "AB_UI_OPERATION_SHADOW_READY";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	public Text m_txtName;

	[Header("배틀 슬롯")]
	public Transform m_trSlot;

	public NKCUIShadowBattleSlot m_prefabSlot;

	public LoopScrollRect m_scrollRect;

	[Header("라이프")]
	public List<Animator> m_lstLife;

	[Header("시간 기록")]
	public Text m_txtCurrentTime;

	public Text m_txtBestTime;

	[Header("버튼")]
	public NKCUIComStateButton m_btnGiveUp;

	public NKCUIComStateButton m_btnBack;

	[Header("애니")]
	public Animator m_ani;

	public Animator m_aniScroll;

	[Header("재화")]
	public NKCUIComItemCount m_item_1;

	public NKCUIComItemCount m_item_2;

	[Header("중첩작전")]
	public GameObject m_objPlayingMultiply;

	public Text m_txtPlayingMultiply;

	private int m_palaceID;

	private NKMPalaceData m_palaceData;

	private Stack<NKCUIShadowBattleSlot> m_stkBattleSlotPool = new Stack<NKCUIShadowBattleSlot>();

	private List<NKMShadowBattleTemplet> m_lstBattleTemplet = new List<NKMShadowBattleTemplet>();

	public static bool IsInstanceOpen
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsUIOpen;
			}
			return false;
		}
	}

	public static bool IsInstanceLoaded
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsLoadComplete;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override string MenuName => "그림자 전당";

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUIShadowBattle>("AB_UI_OPERATION_SHADOW", "AB_UI_OPERATION_SHADOW_READY", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public void Init()
	{
		m_btnGiveUp?.PointerClick.RemoveAllListeners();
		m_btnGiveUp?.PointerClick.AddListener(OnTouchGiveUp);
		m_btnBack?.PointerClick.RemoveAllListeners();
		m_btnBack?.PointerClick.AddListener(OnTouchBack);
		if (m_scrollRect != null)
		{
			m_scrollRect.dOnGetObject += OnGetObject;
			m_scrollRect.dOnProvideData += OnProvideData;
			m_scrollRect.dOnReturnObject += OnReturnObject;
			m_scrollRect.ContentConstraintCount = 1;
			m_scrollRect.PrepareCells();
			NKCUtil.SetScrollHotKey(m_scrollRect);
		}
		if (m_item_1 != null)
		{
			m_item_1.SetOnClickPlusBtn(m_item_1.OpenMoveToShopPopup);
		}
		if (m_item_2 != null)
		{
			m_item_2.SetOnClickPlusBtn(m_item_2.OpenMoveToShopPopup);
		}
	}

	public void Open(int palaceID)
	{
		NKMShadowPalaceTemplet palaceTemplet = NKMShadowPalaceManager.GetPalaceTemplet(palaceID);
		m_lstBattleTemplet.Clear();
		m_lstBattleTemplet = NKMShadowPalaceManager.GetBattleTemplets(palaceID);
		if (m_lstBattleTemplet == null)
		{
			return;
		}
		if (!string.IsNullOrEmpty(palaceTemplet.STAGE_MUSIC_NAME))
		{
			NKCSoundManager.PlayMusic(palaceTemplet.STAGE_MUSIC_NAME, bLoop: true);
		}
		m_palaceID = palaceID;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		m_palaceData = nKMUserData.m_ShadowPalace.palaceDataList.Find((NKMPalaceData v) => v.palaceId == palaceID);
		m_lstBattleTemplet.Sort((NKMShadowBattleTemplet a, NKMShadowBattleTemplet b) => a.BATTLE_ORDER.CompareTo(b.BATTLE_ORDER));
		m_scrollRect.TotalCount = m_lstBattleTemplet.Count;
		m_scrollRect.RefreshCells(bForce: true);
		NKCUtil.SetLabelText(m_txtName, palaceTemplet.PalaceName);
		string msg = "-:--:--";
		string msg2 = "-:--:--";
		if (m_palaceData != null)
		{
			double num = 0.0;
			double num2 = 0.0;
			for (int num3 = 0; num3 < m_palaceData.dungeonDataList.Count; num3++)
			{
				NKMPalaceDungeonData nKMPalaceDungeonData = m_palaceData.dungeonDataList[num3];
				num += (double)nKMPalaceDungeonData.bestTime;
				num2 += (double)nKMPalaceDungeonData.recentTime;
			}
			if (num > 0.0)
			{
				msg = NKCUtilString.GetTimeSpanString(TimeSpan.FromSeconds(num));
			}
			if (num2 > 0.0)
			{
				msg2 = NKCUtilString.GetTimeSpanString(TimeSpan.FromSeconds(num2));
			}
			bool flag = NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.SHADOW_PALACE_MULTIPLY);
			if (nKMUserData.m_ShadowPalace.rewardMultiply > 1 && flag)
			{
				NKCUtil.SetGameobjectActive(m_objPlayingMultiply, bValue: true);
				NKCUtil.SetLabelText(m_txtPlayingMultiply, NKCUtilString.GET_MULTIPLY_REWARD_ONE_PARAM, nKMUserData.m_ShadowPalace.rewardMultiply);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objPlayingMultiply, bValue: false);
			}
		}
		NKCUtil.SetLabelText(m_txtBestTime, msg);
		NKCUtil.SetLabelText(m_txtCurrentTime, msg2);
		SetLife();
		SetCost();
		UIOpened();
		m_ani.Play("DF");
		m_aniScroll.Play("NKM_UI_SHADOW_READY_SLOT_LIST");
		CheckTutorial();
	}

	private void SetLife()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		for (int i = 0; i < m_lstLife.Count; i++)
		{
			if (i < nKMUserData.m_ShadowPalace.life)
			{
				m_lstLife[i].Play("NKM_UI_SHADOW_READY_LIFE");
			}
			else
			{
				m_lstLife[i].Play("NKM_UI_SHADOW_READY_LIFE_OFF");
			}
		}
	}

	private void SetCost()
	{
		if (NKMShadowPalaceManager.GetPalaceTemplet(m_palaceID) != null)
		{
			NKMUserData userData = NKCScenManager.CurrentUserData();
			m_item_1.SetData(userData, 19);
			m_item_2.SetData(userData, 20);
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		base.OnInventoryChange(itemData);
		if (itemData != null && (itemData.ItemID == 19 || itemData.ItemID == 20))
		{
			SetCost();
		}
	}

	public override void CloseInternal()
	{
	}

	private RectTransform OnGetObject(int index)
	{
		if (m_stkBattleSlotPool.Count > 0)
		{
			return m_stkBattleSlotPool.Pop().GetComponent<RectTransform>();
		}
		NKCUIShadowBattleSlot nKCUIShadowBattleSlot = UnityEngine.Object.Instantiate(m_prefabSlot);
		nKCUIShadowBattleSlot.transform.SetParent(m_trSlot);
		nKCUIShadowBattleSlot.Init();
		return nKCUIShadowBattleSlot.GetComponent<RectTransform>();
	}

	private void OnProvideData(Transform tr, int idx)
	{
		NKCUIShadowBattleSlot component = tr.GetComponent<NKCUIShadowBattleSlot>();
		if (component == null)
		{
			return;
		}
		NKMShadowBattleTemplet battleTemplet = m_lstBattleTemplet[idx];
		NKMPalaceDungeonData dungeonData = null;
		int bATTLE_ORDER = m_lstBattleTemplet[0].BATTLE_ORDER;
		if (m_palaceData != null)
		{
			dungeonData = m_palaceData.dungeonDataList.Find((NKMPalaceDungeonData v) => v.dungeonId == battleTemplet.DUNGEON_ID);
			NKMShadowBattleTemplet nKMShadowBattleTemplet = m_lstBattleTemplet.Find((NKMShadowBattleTemplet v) => v.DUNGEON_ID == m_palaceData.currentDungeonId);
			if (nKMShadowBattleTemplet != null)
			{
				bATTLE_ORDER = nKMShadowBattleTemplet.BATTLE_ORDER;
			}
		}
		component.SetData(battleTemplet, dungeonData, bATTLE_ORDER, OnTouchBattle);
	}

	private void OnReturnObject(Transform go)
	{
		if (!(GetComponent<NKCUIShadowBattleSlot>() != null))
		{
			NKCUtil.SetGameobjectActive(go, bValue: false);
			go.SetParent(base.transform);
			m_stkBattleSlotPool.Push(go.GetComponent<NKCUIShadowBattleSlot>());
		}
	}

	private void OnTouchGiveUp()
	{
		NKMShadowPalaceTemplet palaceTemplet = NKMShadowPalaceManager.GetPalaceTemplet(m_palaceID);
		if (palaceTemplet != null)
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(palaceTemplet.STAGE_REQ_ITEM_ID);
			string content = NKCUtilString.GET_SHADOW_PALACE_GIVE_UP(palaceTemplet.PALACE_NUM_UI, palaceTemplet.PalaceName, itemMiscTempletByID.GetItemName());
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, content, delegate
			{
				NKCPacketSender.Send_NKMPacket_SHADOW_PALACE_GIVEUP_ACK(m_palaceID);
			});
		}
	}

	private void OnTouchBack()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_SHADOW_PALACE);
	}

	private void OnTouchBattle(NKMShadowBattleTemplet battleTemplet)
	{
		if (battleTemplet == null)
		{
			return;
		}
		NKMShadowPalaceTemplet palaceTemplet = NKMShadowPalaceManager.GetPalaceTemplet(m_palaceID);
		if (palaceTemplet == null)
		{
			return;
		}
		int bATTLE_ORDER = m_lstBattleTemplet.First().BATTLE_ORDER;
		if (m_palaceData != null)
		{
			NKMShadowBattleTemplet nKMShadowBattleTemplet = m_lstBattleTemplet.Find((NKMShadowBattleTemplet v) => v.DUNGEON_ID == m_palaceData.currentDungeonId);
			if (nKMShadowBattleTemplet != null)
			{
				bATTLE_ORDER = nKMShadowBattleTemplet.BATTLE_ORDER;
			}
		}
		if (battleTemplet.BATTLE_ORDER > bATTLE_ORDER)
		{
			return;
		}
		if (NKCScenManager.GetScenManager().WarfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EPISODE_GIVE_UP_WARFARE, OnClickOkGiveUpWarfare);
			return;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(battleTemplet.DUNGEON_ID);
		if (dungeonTempletBase != null)
		{
			if (dungeonTempletBase.m_UseEventDeck == 0)
			{
				Debug.LogError($"그림자 전당은 이벤트 덱만 사용 가능함! dungeonTempletBase.m_UseEventDeck : {dungeonTempletBase.m_UseEventDeck}");
				return;
			}
			NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().SetDungeonInfo(dungeonTempletBase, DeckContents.SHADOW_PALACE);
			NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().SetReservedBGM(palaceTemplet.STAGE_MUSIC_NAME);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DUNGEON_ATK_READY);
		}
	}

	private void OnClickOkGiveUpWarfare()
	{
		NKMPacket_WARFARE_GAME_GIVE_UP_REQ packet = new NKMPacket_WARFARE_GAME_GIVE_UP_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void StartCurrentBattle()
	{
		if (NKMShadowPalaceManager.GetPalaceTemplet(m_palaceID) == null)
		{
			return;
		}
		NKMShadowBattleTemplet nKMShadowBattleTemplet = null;
		if (m_palaceData != null)
		{
			nKMShadowBattleTemplet = m_lstBattleTemplet.Find((NKMShadowBattleTemplet v) => v.DUNGEON_ID == m_palaceData.currentDungeonId);
		}
		if (nKMShadowBattleTemplet == null)
		{
			nKMShadowBattleTemplet = m_lstBattleTemplet.First();
		}
		OnTouchBattle(nKMShadowBattleTemplet);
	}

	private void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.ShadowBattle);
	}
}
