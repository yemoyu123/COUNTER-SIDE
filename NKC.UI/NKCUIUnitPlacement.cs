using System.Collections;
using System.Collections.Generic;
using ClientPacket.Office;
using ClientPacket.User;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitPlacement : NKCUIBase
{
	public enum UnitType
	{
		Unit,
		Ship,
		Operator
	}

	public enum UnitPlacementType
	{
		Deck,
		WorldmapCityleader,
		OfficeRoom,
		MainLobby
	}

	public struct UnitPlacementData
	{
		public UnitPlacementType PlacementType;

		public NKMDeckIndex DeckIndex;

		public int Data;

		public int DeckPos => Data;

		public int CityID => Data;

		public int RoomID => Data;

		public int LobbyIndex => Data;

		public UnitPlacementData(UnitPlacementType type, NKMDeckIndex deckIndex, int data)
		{
			PlacementType = type;
			DeckIndex = deckIndex;
			Data = data;
		}

		public UnitPlacementData(UnitPlacementType type, int data)
		{
			PlacementType = type;
			DeckIndex = NKMDeckIndex.None;
			Data = data;
		}
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_unit_info";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_UNIT_PLACEMENT";

	private static NKCUIUnitPlacement m_Instance;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnReleaseAll;

	public LoopScrollRect m_ScrollRect;

	public NKCUIUnitPlacementSlot m_pfbSlot;

	public Text m_lbDesc;

	private UnitType m_eUnitType;

	private long m_unitUID;

	private List<UnitPlacementData> m_lstPlacementData;

	public static NKCUIUnitPlacement Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIUnitPlacement>("ab_ui_nkm_ui_unit_info", "NKM_UI_POPUP_UNIT_PLACEMENT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIUnitPlacement>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool HasInstance => m_Instance != null;

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

	public override string MenuName => "UnitPlacement";

	public override bool PassHotkeyToNextUI => true;

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

	public override void OnBackButton()
	{
		if (!NKMPopUpBox.IsOpenedWaitBox())
		{
			base.OnBackButton();
		}
	}

	public override void CloseInternal()
	{
		StopAllCoroutines();
		base.gameObject.SetActive(value: false);
	}

	private void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnReleaseAll, OnBtnReleaseAll);
		if (m_ScrollRect != null)
		{
			m_ScrollRect.dOnGetObject += GetSlot;
			m_ScrollRect.dOnReturnObject += ReturnSlot;
			m_ScrollRect.dOnProvideData += ProvideSlotData;
			m_ScrollRect.PrepareCells();
		}
	}

	public void Open(UnitType type, long uid)
	{
		UIOpened();
		string strID = ((type == UnitType.Operator) ? "SI_PF_DEPLOY_STATUS_DESC_OPR" : "SI_PF_DEPLOY_STATUS_DESC");
		NKCUtil.SetLabelText(m_lbDesc, NKCStringTable.GetString(strID));
		SetData(type, uid);
	}

	public void SetData(UnitType type, long uid)
	{
		m_eUnitType = type;
		m_unitUID = uid;
		UpdatePlacementList();
	}

	private bool CanRelease(UnitPlacementData data)
	{
		switch (data.PlacementType)
		{
		case UnitPlacementType.Deck:
		{
			if (data.DeckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_PVP_DEFENCE)
			{
				return false;
			}
			NKMDeckData deckData = NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(data.DeckIndex);
			if (deckData != null && deckData.m_DeckState != NKM_DECK_STATE.DECK_STATE_NORMAL)
			{
				return false;
			}
			break;
		}
		case UnitPlacementType.WorldmapCityleader:
			if (NKCScenManager.CurrentUserData().m_WorldmapData.GetCityData(data.CityID).HasMission())
			{
				return false;
			}
			break;
		}
		return true;
	}

	private void OnBtnRelease(UnitPlacementData placementData)
	{
		if (CanRelease(placementData))
		{
			if (TryRelease(placementData))
			{
				WaitAndUpdate();
			}
		}
		else
		{
			string strID = ((m_eUnitType == UnitType.Operator) ? "SI_DP_POPUP_DECK_REMOVE_SHORTCUT_OPR" : "SI_DP_POPUP_DECK_REMOVE_SHORTCUT");
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(strID), delegate
			{
				TryMove(placementData);
			});
		}
	}

	private bool TryRelease(UnitPlacementData data)
	{
		switch (data.PlacementType)
		{
		case UnitPlacementType.Deck:
			switch (m_eUnitType)
			{
			case UnitType.Unit:
				NKCPacketSender.Send_NKMPacket_DECK_UNIT_SET_REQ(data.DeckIndex, data.DeckPos, 0L);
				break;
			case UnitType.Ship:
				NKCPacketSender.Send_NKMPacket_DECK_SHIP_SET_REQ(data.DeckIndex, 0L);
				break;
			case UnitType.Operator:
				NKCPacketSender.Send_NKMPacket_DECK_OPERATOR_SET_REQ(data.DeckIndex, 0L);
				break;
			}
			return true;
		case UnitPlacementType.OfficeRoom:
		{
			List<long> list = new List<long>(NKCScenManager.CurrentUserData().OfficeData.GetOfficeRoom(data.RoomID).unitUids);
			if (!list.Contains(m_unitUID))
			{
				return false;
			}
			list.Remove(m_unitUID);
			NKCPacketSender.Send_NKMPacket_OFFICE_ROOM_SET_UNIT_REQ(data.RoomID, list);
			return true;
		}
		case UnitPlacementType.MainLobby:
		{
			NKMBackgroundInfo nKMBackgroundInfo = new NKMBackgroundInfo();
			nKMBackgroundInfo.DeepCopyFrom(NKCScenManager.CurrentUserData().backGroundInfo);
			if (!nKMBackgroundInfo.unitInfoList.Exists((NKMBackgroundUnitInfo x) => x.unitUid == m_unitUID))
			{
				return false;
			}
			foreach (NKMBackgroundUnitInfo unitInfo in nKMBackgroundInfo.unitInfoList)
			{
				if (unitInfo.unitUid == m_unitUID)
				{
					unitInfo.unitUid = 0L;
				}
			}
			NKCPacketSender.Send_NKMPacket_BACKGROUND_CHANGE_REQ(nKMBackgroundInfo);
			return true;
		}
		case UnitPlacementType.WorldmapCityleader:
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().Send_NKMPacket_WORLDMAP_SET_LEADER_REQ(data.CityID, 0L);
			return true;
		default:
			return false;
		}
	}

	private void TryMove(UnitPlacementData data)
	{
		switch (data.PlacementType)
		{
		case UnitPlacementType.Deck:
			switch (data.DeckIndex.m_eDeckType)
			{
			case NKM_DECK_TYPE.NDT_NORMAL:
			case NKM_DECK_TYPE.NDT_DAILY:
				NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_DECKSETUP, string.Empty);
				break;
			case NKM_DECK_TYPE.NDT_PVP:
			case NKM_DECK_TYPE.NDT_PVP_DEFENCE:
				NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_PVP_MAIN, string.Empty);
				break;
			case NKM_DECK_TYPE.NDT_RAID:
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().SetShowIntro();
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP, bForce: false);
				break;
			case NKM_DECK_TYPE.NDT_FRIEND:
				NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_FRIEND_MYPROFILE, string.Empty);
				break;
			case NKM_DECK_TYPE.NDT_TRIM:
				NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_TRIM, string.Empty);
				break;
			case NKM_DECK_TYPE.NDT_DIVE:
				NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_DIVE, string.Empty);
				break;
			}
			break;
		case UnitPlacementType.OfficeRoom:
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OFFICE, data.RoomID.ToString());
			break;
		case UnitPlacementType.MainLobby:
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
			break;
		case UnitPlacementType.WorldmapCityleader:
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().SetShowIntro();
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP, bForce: false);
			break;
		}
	}

	private void OnBtnReleaseAll()
	{
		StartCoroutine(ProcessReleaseAll());
	}

	private IEnumerator ProcessReleaseAll()
	{
		int releaseCount = 0;
		foreach (UnitPlacementData lstPlacementDatum in m_lstPlacementData)
		{
			if (CanRelease(lstPlacementDatum) && TryRelease(lstPlacementDatum))
			{
				releaseCount++;
				yield return null;
				while (NKMPopUpBox.IsOpenedWaitBox())
				{
					yield return null;
				}
			}
		}
		UpdatePlacementList();
		if (releaseCount > 0)
		{
			string strID = ((m_eUnitType == UnitType.Operator) ? "SI_DP_POPUP_DECK_REMOVE_ALL_COMPLETE_OPR" : "SI_DP_POPUP_DECK_REMOVE_ALL_COMPLETE");
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(strID));
		}
	}

	private void WaitAndUpdate()
	{
		StartCoroutine(ProcessWaitAndUpdate());
	}

	private IEnumerator ProcessWaitAndUpdate()
	{
		yield return null;
		while (NKMPopUpBox.IsOpenedWaitBox())
		{
			yield return null;
		}
		UpdatePlacementList();
	}

	public void UpdatePlacementList()
	{
		m_lstPlacementData = MakePlacementData(m_eUnitType, m_unitUID);
		m_ScrollRect.TotalCount = m_lstPlacementData.Count;
		m_ScrollRect.SetIndexPosition(0);
	}

	private List<UnitPlacementData> MakePlacementData(UnitType unitType, long unitUID)
	{
		List<UnitPlacementData> list = new List<UnitPlacementData>();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return list;
		}
		foreach (var item5 in FindAllDeckIndexByUnitUid(unitUID))
		{
			UnitPlacementData item = new UnitPlacementData(UnitPlacementType.Deck, item5.Item1, item5.Item2);
			list.Add(item);
		}
		int unitWorldMapLeaderCity = NKMWorldMapManager.GetUnitWorldMapLeaderCity(nKMUserData, unitUID);
		if (unitWorldMapLeaderCity >= 0)
		{
			UnitPlacementData item2 = new UnitPlacementData(UnitPlacementType.WorldmapCityleader, unitWorldMapLeaderCity);
			list.Add(item2);
		}
		if (nKMUserData.OfficeData != null)
		{
			foreach (NKMOfficeRoom room in nKMUserData.OfficeData.Rooms)
			{
				if (room != null && room.unitUids != null && room.unitUids.Contains(unitUID))
				{
					UnitPlacementData item3 = new UnitPlacementData(UnitPlacementType.OfficeRoom, room.id);
					list.Add(item3);
					break;
				}
			}
		}
		if (nKMUserData.backGroundInfo != null && nKMUserData.backGroundInfo.unitInfoList != null)
		{
			for (int i = 0; i < nKMUserData.backGroundInfo.unitInfoList.Count; i++)
			{
				if (nKMUserData.backGroundInfo.unitInfoList[i].unitUid == unitUID)
				{
					UnitPlacementData item4 = new UnitPlacementData(UnitPlacementType.MainLobby, i);
					list.Add(item4);
					break;
				}
			}
		}
		return list;
	}

	private RectTransform GetSlot(int index)
	{
		NKCUIUnitPlacementSlot nKCUIUnitPlacementSlot = Object.Instantiate(m_pfbSlot);
		nKCUIUnitPlacementSlot.Init(OnBtnRelease);
		return nKCUIUnitPlacementSlot.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		Object.Destroy(go.gameObject);
	}

	private void ProvideSlotData(Transform tr, int idx)
	{
		if (idx < 0 || idx >= m_lstPlacementData.Count)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(tr, bValue: true);
		if (tr.TryGetComponent<NKCUIUnitPlacementSlot>(out var component))
		{
			component.SetData(m_lstPlacementData[idx], m_eUnitType);
		}
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		if (unitData != null && unitData.m_UnitUID == m_unitUID)
		{
			m_unitUID = unitData.m_UnitUID;
			UpdatePlacementList();
		}
	}

	private List<(NKMDeckIndex, int)> FindAllDeckIndexByUnitUid(long unitUid)
	{
		List<(NKMDeckIndex, int)> list = new List<(NKMDeckIndex, int)>();
		foreach (NKMDeckSet deckSet in NKCScenManager.CurrentArmyData().DeckSets)
		{
			for (int i = 0; i < deckSet.Values.Count; i++)
			{
				NKMDeckData nKMDeckData = deckSet.Values[i];
				if (nKMDeckData != null)
				{
					int index;
					if (nKMDeckData.m_ShipUID == unitUid || nKMDeckData.m_OperatorUID == unitUid)
					{
						list.Add((new NKMDeckIndex(deckSet.DeckType, i), -1));
					}
					else if (nKMDeckData.HasUnitUid(unitUid, out index))
					{
						list.Add((new NKMDeckIndex(deckSet.DeckType, i), index));
					}
				}
			}
		}
		return list;
	}
}
