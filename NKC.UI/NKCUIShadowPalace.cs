using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.LeaderBoard;
using ClientPacket.Mode;
using NKC.UI.Shop;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIShadowPalace : NKCUIBase
{
	private const string BUNDLE_NAME = "AB_UI_OPERATION_SHADOW";

	private const string ASSET_NAME = "AB_UI_OPERATION_SHADOW";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	[Header("PALACE")]
	public Transform m_trPalace;

	public NKCUIShadowPalaceSlot m_palacePrefab;

	public LoopScrollRect m_scrollRect;

	[Header("PALACE INFO")]
	public NKCUIShadowPalaceInfo m_palaceInfo;

	public NKCUIShadowPalaceRank m_palaceRank;

	[Header("BUTTON")]
	public NKCUIComStateButton m_btnShortcut;

	public NKCUIComStateButton m_btnShop;

	[Header("스킵")]
	public GameObject m_objSkip;

	public NKCUIOperationSkip m_NKCUIOperationSkip;

	public NKCUIComToggle m_tglSkip;

	private int m_CurrSkipCount = 1;

	private Stack<NKCUIShadowPalaceSlot> m_stkPalaceSlotPool = new Stack<NKCUIShadowPalaceSlot>();

	private List<NKCUIShadowPalaceSlot> m_lstPalaceSlot = new List<NKCUIShadowPalaceSlot>();

	private List<NKMShadowPalaceTemplet> m_lstPalaceTemplet = new List<NKMShadowPalaceTemplet>();

	private int m_selectPalaceID;

	private List<int> m_lstUpsideResource = new List<int>();

	private bool m_bOperationSkip;

	private List<int> m_leaderBoardIDs = new List<int>();

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

	public override string MenuName => NKCUtilString.GET_SHADOW_PALACE;

	public override string GuideTempletID => "ARTICLE_SHADOW_PALACE_INFO";

	public override List<int> UpsideMenuShowResourceList => m_lstUpsideResource;

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUIShadowPalace>("AB_UI_OPERATION_SHADOW", "AB_UI_OPERATION_SHADOW", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static NKCUIShadowPalace GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUIShadowPalace>();
		}
		return null;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public int GetCurrMultiplyRewardCount()
	{
		return m_CurrSkipCount;
	}

	public void Init()
	{
		m_palaceInfo?.Init(OnTouchPalaceStart, OnTouchPalaceProgress, OnTouchRank);
		m_palaceRank?.Init();
		NKCUtil.SetGameobjectActive(m_palaceRank, bValue: false);
		if (m_scrollRect != null)
		{
			m_scrollRect.dOnGetObject += OnGetObject;
			m_scrollRect.dOnProvideData += OnProvideData;
			m_scrollRect.dOnReturnObject += OnReturnObject;
			m_scrollRect.PrepareCells();
			NKCUtil.SetScrollHotKey(m_scrollRect);
		}
		m_btnShortcut.PointerClick.RemoveAllListeners();
		m_btnShortcut.PointerClick.AddListener(MoveToRank);
		NKCUtil.SetButtonClickDelegate(m_btnShop, OnShopShortcut);
		m_tglSkip.OnValueChanged.RemoveAllListeners();
		m_tglSkip.OnValueChanged.AddListener(OnClickSkip);
		NKCUtil.SetHotkey(m_tglSkip, HotkeyEventType.RotateLeft, null, bUpDownEvent: true);
		m_CurrSkipCount = 1;
		m_NKCUIOperationSkip.Init(OnSkipCountUpdate, OnSkipClose);
	}

	public void Open(int current_palace_id)
	{
		m_lstPalaceTemplet = NKMTempletContainer<NKMShadowPalaceTemplet>.Values.ToList();
		m_lstPalaceTemplet.Sort((NKMShadowPalaceTemplet a, NKMShadowPalaceTemplet b) => a.PALACE_NUM_UI.CompareTo(b.PALACE_NUM_UI));
		m_selectPalaceID = current_palace_id;
		m_bOperationSkip = false;
		if (m_selectPalaceID == 0)
		{
			m_selectPalaceID = NKMShadowPalaceManager.GetLastClearedPalace();
			NKMShadowPalaceTemplet nKMShadowPalaceTemplet = m_lstPalaceTemplet.Find((NKMShadowPalaceTemplet x) => x.PALACE_ID == m_selectPalaceID);
			if (nKMShadowPalaceTemplet == null || !NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), new UnlockInfo(nKMShadowPalaceTemplet.STAGE_UNLOCK_REQ_TYPE, nKMShadowPalaceTemplet.STAGE_UNLOCK_REQ_VALUE)))
			{
				m_selectPalaceID = 0;
			}
		}
		if (m_selectPalaceID == 0)
		{
			NKMUserData userData = NKCScenManager.CurrentUserData();
			NKMShadowPalaceTemplet nKMShadowPalaceTemplet2 = m_lstPalaceTemplet.FindLast(delegate(NKMShadowPalaceTemplet v)
			{
				UnlockInfo unlockInfo = new UnlockInfo(v.STAGE_UNLOCK_REQ_TYPE, v.STAGE_UNLOCK_REQ_VALUE);
				return NKMContentUnlockManager.IsContentUnlocked(userData, in unlockInfo);
			});
			if (nKMShadowPalaceTemplet2 != null)
			{
				m_selectPalaceID = nKMShadowPalaceTemplet2.PALACE_ID;
			}
			else
			{
				if (m_lstPalaceTemplet.Count <= 0)
				{
					Debug.LogError("Can't Find Shadow Palace Templet");
					return;
				}
				m_selectPalaceID = m_lstPalaceTemplet[0].PALACE_ID;
			}
		}
		m_scrollRect.TotalCount = m_lstPalaceTemplet.Count;
		m_scrollRect.RefreshCells();
		m_lstUpsideResource.Add(1);
		m_lstUpsideResource.Add(19);
		m_lstUpsideResource.Add(20);
		SetPalaceInfo(m_selectPalaceID);
		UIOpened();
		StartCoroutine(Intro());
	}

	private IEnumerator Intro()
	{
		yield return null;
		MovePalaceSlot(m_selectPalaceID);
		CheckTutorial();
	}

	public override void CloseInternal()
	{
	}

	public override void UnHide()
	{
		base.UnHide();
		MovePalaceSlot(m_selectPalaceID);
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		SetPalaceInfo(m_selectPalaceID, bPlayIntroAni: false);
	}

	private RectTransform OnGetObject(int index)
	{
		if (m_stkPalaceSlotPool.Count > 0)
		{
			return m_stkPalaceSlotPool.Pop().GetComponent<RectTransform>();
		}
		NKCUIShadowPalaceSlot nKCUIShadowPalaceSlot = Object.Instantiate(m_palacePrefab);
		nKCUIShadowPalaceSlot.transform.SetParent(m_trPalace);
		nKCUIShadowPalaceSlot.Init();
		m_lstPalaceSlot.Add(nKCUIShadowPalaceSlot);
		return nKCUIShadowPalaceSlot.GetComponent<RectTransform>();
	}

	private void OnProvideData(Transform tr, int idx)
	{
		NKCUIShadowPalaceSlot component = tr.GetComponent<NKCUIShadowPalaceSlot>();
		if (!(component == null))
		{
			NKMShadowPalaceTemplet palaceTemplet = m_lstPalaceTemplet[idx];
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			NKMShadowPalace shadowPalace = nKMUserData.m_ShadowPalace;
			List<NKMPalaceData> palaceDataList = shadowPalace.palaceDataList;
			bool flag = NKMContentUnlockManager.IsContentUnlocked(nKMUserData, new UnlockInfo(palaceTemplet.STAGE_UNLOCK_REQ_TYPE, palaceTemplet.STAGE_UNLOCK_REQ_VALUE));
			NKMPalaceData palaceData = palaceDataList.Find((NKMPalaceData v) => v.palaceId == palaceTemplet.PALACE_ID);
			component.SetData(palaceTemplet, palaceData, OnTouchPalaceSlot);
			component.SetLock(!flag);
			component.SetProgress(shadowPalace.currentPalaceId == palaceTemplet.PALACE_ID);
			component.SetLine(idx == 0, idx == m_lstPalaceTemplet.Count - 1);
			component.PlaySelect(palaceTemplet.PALACE_ID == m_selectPalaceID, bEffect: false);
		}
	}

	private void OnReturnObject(Transform go)
	{
		if (!(GetComponent<NKCUIShadowPalaceSlot>() != null))
		{
			NKCUtil.SetGameobjectActive(go, bValue: false);
			go.SetParent(base.transform);
			m_stkPalaceSlotPool.Push(go.GetComponent<NKCUIShadowPalaceSlot>());
		}
	}

	private void MovePalaceSlot(int palaceID)
	{
		int num = m_lstPalaceTemplet.FindIndex((NKMShadowPalaceTemplet v) => v.PALACE_ID == palaceID);
		if (num >= 0)
		{
			m_scrollRect.SetIndexPosition(num);
			OnTouchPalaceSlot(palaceID);
		}
	}

	private void SetPalaceInfo(int palaceID, bool bPlayIntroAni = true)
	{
		NKMShadowPalace shadowPalace = NKCScenManager.CurrentUserData().m_ShadowPalace;
		NKMShadowPalaceTemplet palaceTemplet = NKMShadowPalaceManager.GetPalaceTemplet(palaceID);
		UnlockInfo unlockInfo = new UnlockInfo(palaceTemplet.STAGE_UNLOCK_REQ_TYPE, palaceTemplet.STAGE_UNLOCK_REQ_VALUE);
		bool bUnlock = NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in unlockInfo);
		bool flag = shadowPalace.currentPalaceId == palaceID;
		int currSkipCount = 1;
		if (flag || shadowPalace.currentPalaceId == 0)
		{
			currSkipCount = m_CurrSkipCount;
		}
		m_palaceInfo.SetData(palaceTemplet, currSkipCount, flag, bUnlock);
		if (bPlayIntroAni)
		{
			m_palaceInfo.PlayIntroAni();
		}
	}

	private void OnTouchPalaceSlot(int palaceID)
	{
		for (int i = 0; i < m_lstPalaceSlot.Count; i++)
		{
			NKCUIShadowPalaceSlot nKCUIShadowPalaceSlot = m_lstPalaceSlot[i];
			if (nKCUIShadowPalaceSlot.PalaceID == palaceID)
			{
				nKCUIShadowPalaceSlot.PlaySelect(bSelect: true, bEffect: true);
			}
			else if (nKCUIShadowPalaceSlot.PalaceID == m_selectPalaceID)
			{
				nKCUIShadowPalaceSlot.PlaySelect(bSelect: false, bEffect: true);
			}
			else
			{
				nKCUIShadowPalaceSlot.PlaySelect(bSelect: false, bEffect: false);
			}
		}
		m_selectPalaceID = palaceID;
		m_tglSkip.Select(bSelect: false);
		SetPalaceInfo(palaceID);
	}

	private void OnTouchPalaceStart()
	{
		if (NKCScenManager.CurrentUserData().m_ShadowPalace.currentPalaceId != 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NED_FAIL_SHADOW_PALACE_DOING));
			return;
		}
		NKMShadowPalaceTemplet palaceTemplet = NKMShadowPalaceManager.GetPalaceTemplet(m_selectPalaceID);
		if (palaceTemplet == null)
		{
			return;
		}
		if (m_CurrSkipCount > 1)
		{
			int num = palaceTemplet.STAGE_REQ_ITEM_COUNT * (m_CurrSkipCount - 1);
			if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(palaceTemplet.STAGE_REQ_ITEM_ID) < num)
			{
				NKCShopManager.OpenItemLackPopup(palaceTemplet.STAGE_REQ_ITEM_ID, num);
				return;
			}
		}
		if (!m_bOperationSkip)
		{
			string content = string.Format(NKCUtilString.GET_SHADOW_PALACE_START_CONFIRM, palaceTemplet.PALACE_NUM_UI, palaceTemplet.PalaceName);
			NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_NOTICE, content, palaceTemplet.STAGE_REQ_ITEM_ID, palaceTemplet.STAGE_REQ_ITEM_COUNT, delegate
			{
				NKCPacketSender.Send_NKMPacket_SHADOW_PALACE_START_REQ(m_selectPalaceID);
			});
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_SHADOW_PALACE_SKIP_REQ(m_selectPalaceID, m_CurrSkipCount);
		}
	}

	private void OnTouchPalaceProgress()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData.m_ShadowPalace.life <= 0)
		{
			Debug.LogError("그림자 전당 - 라이프 부족한데 들어가려고 함 - life " + nKMUserData.m_ShadowPalace.life);
			return;
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_SHADOW_BATTLE().SetShadowPalaceID(nKMUserData.m_ShadowPalace.currentPalaceId);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_SHADOW_BATTLE);
	}

	private void OnTouchRank()
	{
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_SHADOW, m_selectPalaceID);
		if (nKMLeaderBoardTemplet != null)
		{
			if (!m_leaderBoardIDs.Contains(nKMLeaderBoardTemplet.m_BoardID))
			{
				NKCLeaderBoardManager.SendReq(nKMLeaderBoardTemplet, bAllReq: true);
			}
			else
			{
				OpenRank();
			}
		}
	}

	public void OpenRank()
	{
		NKMShadowPalaceTemplet palaceTemplet = NKMShadowPalaceManager.GetPalaceTemplet(m_selectPalaceID);
		if (palaceTemplet == null)
		{
			return;
		}
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_SHADOW, m_selectPalaceID);
		if (nKMLeaderBoardTemplet == null)
		{
			return;
		}
		if (!m_leaderBoardIDs.Contains(nKMLeaderBoardTemplet.m_BoardID))
		{
			m_leaderBoardIDs.Add(nKMLeaderBoardTemplet.m_BoardID);
		}
		NKCUtil.SetGameobjectActive(m_palaceRank, bValue: true);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		List<LeaderBoardSlotData> leaderBoardData = NKCLeaderBoardManager.GetLeaderBoardData(nKMLeaderBoardTemplet.m_BoardID);
		int rank = NKCLeaderBoardManager.GetMyRankSlotData(nKMLeaderBoardTemplet.m_BoardID).rank;
		NKMShadowPalaceData nKMShadowPalaceData = new NKMShadowPalaceData();
		nKMShadowPalaceData.commonProfile.userUid = nKMUserData.m_UserUID;
		nKMShadowPalaceData.commonProfile.friendCode = nKMUserData.m_FriendCode;
		nKMShadowPalaceData.commonProfile.nickname = nKMUserData.m_UserNickName;
		nKMShadowPalaceData.commonProfile.level = nKMUserData.m_UserLevel;
		NKMPalaceData nKMPalaceData = nKMUserData.m_ShadowPalace.palaceDataList.Find((NKMPalaceData v) => v.palaceId == m_selectPalaceID);
		List<NKMShadowBattleTemplet> battleTemplets = NKMShadowPalaceManager.GetBattleTemplets(m_selectPalaceID);
		int num = 0;
		if (nKMPalaceData != null && battleTemplets != null && nKMPalaceData.dungeonDataList.Count == battleTemplets.Count)
		{
			for (int num2 = 0; num2 < nKMPalaceData.dungeonDataList.Count; num2++)
			{
				num += nKMPalaceData.dungeonDataList[num2].bestTime;
			}
		}
		nKMShadowPalaceData.bestTime = num;
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		if (userProfileData != null)
		{
			nKMShadowPalaceData.commonProfile.mainUnitId = userProfileData.commonProfile.mainUnitId;
			nKMShadowPalaceData.commonProfile.mainUnitSkinId = userProfileData.commonProfile.mainUnitSkinId;
			nKMShadowPalaceData.commonProfile.frameId = userProfileData.commonProfile.frameId;
		}
		LeaderBoardSlotData myRankSlotData = NKCLeaderBoardManager.GetMyRankSlotData(nKMLeaderBoardTemplet.m_BoardID);
		m_palaceRank.SetData(palaceTemplet, leaderBoardData, myRankSlotData, rank);
		m_palaceRank.PlayAni(bOpen: true);
	}

	public override void OnBackButton()
	{
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedEpisodeCategory(EPISODE_CATEGORY.EC_SHADOW);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
	}

	private void MoveToRank()
	{
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_SHADOW, m_selectPalaceID);
		if (nKMLeaderBoardTemplet != null)
		{
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_RANKING, nKMLeaderBoardTemplet.m_BoardID.ToString());
		}
	}

	private void OnShopShortcut()
	{
		NKCUIShop.ShopShortcut("TAB_EXCHANGE_SHADOW_COIN");
	}

	private void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.ShadowPalace);
	}

	public RectTransform GetPalaceSlot(int palaceID)
	{
		int num = m_lstPalaceTemplet.FindIndex((NKMShadowPalaceTemplet v) => v.PALACE_ID == palaceID);
		if (num < 0)
		{
			return null;
		}
		m_scrollRect.SetIndexPosition(num);
		NKCUIShadowPalaceSlot[] componentsInChildren = m_scrollRect.content.GetComponentsInChildren<NKCUIShadowPalaceSlot>();
		for (int num2 = 0; num2 < componentsInChildren.Length; num2++)
		{
			if (componentsInChildren[num2] != null && componentsInChildren[num2].PalaceID == palaceID)
			{
				return componentsInChildren[num2].GetComponent<RectTransform>();
			}
		}
		return null;
	}

	private bool CheckSkip()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return false;
		}
		if (NKMShadowPalaceManager.GetPalaceTemplet(m_selectPalaceID) == null)
		{
			return false;
		}
		List<NKMShadowBattleTemplet> battleTemplets = NKMShadowPalaceManager.GetBattleTemplets(m_selectPalaceID);
		if (battleTemplets == null)
		{
			return false;
		}
		NKMPalaceData nKMPalaceData = nKMUserData.m_ShadowPalace.palaceDataList.Find((NKMPalaceData x) => x.palaceId == m_selectPalaceID);
		if (nKMPalaceData == null)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString(NKM_ERROR_CODE.NED_FAIL_SHADOW_PALACE_MULTIPLY_CLEAR_DUNGEON), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return false;
		}
		bool flag = true;
		foreach (NKMShadowBattleTemplet battleTemplet in battleTemplets)
		{
			if (nKMPalaceData.dungeonDataList.Find((NKMPalaceDungeonData x) => x.dungeonId == battleTemplet.DUNGEON_ID) == null)
			{
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString(NKM_ERROR_CODE.NED_FAIL_SHADOW_PALACE_MULTIPLY_CLEAR_DUNGEON), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return false;
		}
		if (nKMUserData.m_ShadowPalace.currentPalaceId > 0)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_SHADOW_SKIP_ERROR, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return false;
		}
		return true;
	}

	private void OnClickSkip(bool bSet)
	{
		if (bSet)
		{
			NKMShadowPalaceTemplet palaceTemplet = NKMShadowPalaceManager.GetPalaceTemplet(m_selectPalaceID);
			if (palaceTemplet == null)
			{
				return;
			}
			if (!CheckSkip())
			{
				m_tglSkip.Select(bSelect: false);
				return;
			}
			m_bOperationSkip = true;
			NKCUtil.SetGameobjectActive(m_NKCUIOperationSkip, bValue: true);
			m_NKCUIOperationSkip.SetData(0, 0, palaceTemplet.STAGE_REQ_ITEM_ID, palaceTemplet.STAGE_REQ_ITEM_COUNT, m_CurrSkipCount, 1, palaceTemplet.MaxRewardMultiply);
		}
		NKCUtil.SetGameobjectActive(m_NKCUIOperationSkip, bSet);
		if (!bSet)
		{
			m_bOperationSkip = false;
			m_CurrSkipCount = 1;
		}
		SetPalaceInfo(m_selectPalaceID, bPlayIntroAni: false);
	}

	private void OnSkipCountUpdate(int count)
	{
		m_CurrSkipCount = count;
		SetPalaceInfo(m_selectPalaceID, bPlayIntroAni: false);
	}

	private void OnSkipClose()
	{
		m_tglSkip.Select(bSelect: false);
	}
}
