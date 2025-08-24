using System.Collections.Generic;
using System.Linq;
using ClientPacket.Mode;
using ClientPacket.Raid;
using ClientPacket.WorldMap;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Worldmap;

public class NKCPopupWorldMapEventList : NKCUIBase
{
	public enum eState
	{
		EventList,
		HelpList,
		JoinList,
		SeasonPoint,
		ExtraPoint
	}

	public delegate void OnSelectEvent(int cityID, int eventID, long eventUID);

	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_world_map_renewal";

	public const string UI_ASSET_NAME = "NKM_UI_WORLD_MAP_RENEWAL_EVENT_POPUP";

	public NKCPopupWorldMapEventListSlot m_pfbSlot;

	public LoopScrollRect m_LoopScrollRect;

	public EventTrigger m_evtBG;

	[Header("상단")]
	public GameObject m_objRaidSeasonInfo;

	public Text m_lbRaidBossName;

	public Text m_lbRaidSeasonTimeLeft;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComUnitSortOptions m_unitSortOptions;

	[Header("좌측 탭")]
	public NKCUIComToggle m_tglEventList;

	public NKCUIComToggle m_tglHelpList;

	public GameObject m_objHelpListReddot;

	public NKCUIComToggle m_tglJoinList;

	public GameObject m_objJoinListReddot;

	public NKCUIComToggle m_tglSeasonPoint;

	public GameObject m_objSeasonPointReddot;

	public NKCUIComToggle m_tglExtraPoint;

	public GameObject m_objExtraPointReddot;

	public GameObject m_objExtraPointLock;

	[Header("우측 정보")]
	public Image m_imgRaidAllReceive;

	public Text m_lbRaidAllReceive;

	public GameObject m_objRaidAllReceive;

	public NKCUIComStateButton m_csbtnRaidAllReceive;

	public GameObject m_objEmpty;

	public Text m_lbEmptyMessage;

	public GameObject m_objSlotList;

	public GameObject m_objSeasonPoint;

	public GameObject m_objSeasonNormalPoint;

	public NKCUISeasonPoint m_NKCUIRaidSeasonPoint;

	public GameObject m_objSeasonExtraPoint;

	public NKCUISeasonExtraPoint m_NKCUIRaidSeasonExtraPoint;

	public NKCUIComStateButton m_csbtnRaidCoopAll;

	private List<NKMWorldMapCityData> m_lstCityHasEvent = new List<NKMWorldMapCityData>();

	private eState m_eState;

	private OnSelectEvent dOnSelectEvent;

	private List<NKMCoopRaidData> m_CoopRaidDataList = new List<NKMCoopRaidData>();

	private List<NKMRaidResultData> m_RaidResultDataList = new List<NKMRaidResultData>();

	private Stack<NKCPopupWorldMapEventListSlot> m_stkSlot = new Stack<NKCPopupWorldMapEventListSlot>();

	private Color m_RaidAllReceiveOriginalColor;

	private List<NKCUISeasonPointSlot.SeasonPointSlotData> m_lstSeasonPointData = new List<NKCUISeasonPointSlot.SeasonPointSlotData>();

	private eState m_openState;

	private NKCUnitSortSystem m_unitSortSystem;

	private HashSet<NKCUnitSortSystem.eSortCategory> m_sortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>();

	private bool m_bCanReceiveAllReward;

	private float bDeltaTime;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_MENU_NAME_WORLDMAP_EVENT;

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(m_objSlotList, bValue: true);
		NKCUtil.SetGameobjectActive(m_objSeasonPoint, bValue: false);
		m_openState = m_eState;
		base.gameObject.SetActive(value: false);
	}

	public override void UnHide()
	{
		base.UnHide();
		if (m_eState == eState.SeasonPoint)
		{
			m_NKCUIRaidSeasonPoint.Refresh(NKCRaidSeasonManager.RaidSeason.monthlyPoint, NKCRaidSeasonManager.RaidSeason.recvRewardRaidPoint);
		}
		else
		{
			m_LoopScrollRect.RefreshCells();
		}
	}

	public void Init()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (m_csbtnClose != null)
		{
			m_csbtnClose.PointerClick.RemoveAllListeners();
			m_csbtnClose.PointerClick.AddListener(base.Close);
		}
		if (null != m_LoopScrollRect)
		{
			NKCUtil.SetGameobjectActive(m_objSlotList, bValue: true);
			m_LoopScrollRect.dOnGetObject += GetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnSlot;
			m_LoopScrollRect.dOnProvideData += ProvideSlotData;
			m_LoopScrollRect.PrepareCells();
			NKCUtil.SetScrollHotKey(m_LoopScrollRect);
		}
		NKCUtil.SetToggleValueChangedDelegate(m_tglEventList, delegate(bool x)
		{
			if (x)
			{
				SetState(eState.EventList);
			}
		});
		NKCUtil.SetToggleValueChangedDelegate(m_tglHelpList, delegate(bool x)
		{
			if (x)
			{
				SetState(eState.HelpList);
			}
		});
		NKCUtil.SetToggleValueChangedDelegate(m_tglJoinList, delegate(bool x)
		{
			if (x)
			{
				SetState(eState.JoinList);
			}
		});
		NKCUtil.SetToggleValueChangedDelegate(m_tglSeasonPoint, delegate(bool x)
		{
			if (x)
			{
				SetState(eState.SeasonPoint);
			}
		});
		NKCUtil.SetToggleValueChangedDelegate(m_tglExtraPoint, delegate(bool x)
		{
			if (x)
			{
				SetState(eState.ExtraPoint);
			}
		});
		if (m_evtBG != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnClickBG);
			m_evtBG.triggers.Add(entry);
		}
		if (m_lbRaidAllReceive != null)
		{
			m_RaidAllReceiveOriginalColor = m_lbRaidAllReceive.color;
		}
		if (m_NKCUIRaidSeasonPoint != null)
		{
			m_NKCUIRaidSeasonPoint.Init(bUseFixedDuration: true);
		}
		if (m_NKCUIRaidSeasonExtraPoint != null)
		{
			m_NKCUIRaidSeasonExtraPoint.Init(bUseFixedDuration: true);
		}
		NKCUtil.SetBindFunction(m_csbtnRaidAllReceive, OnClickReceiveAllRaidResult);
		NKCUtil.SetButtonClickDelegate(m_csbtnRaidCoopAll, OnClickRaidCoopRequestAll);
		NKCUnitSortSystem.UnitListOptions options = default(NKCUnitSortSystem.UnitListOptions);
		options.lstSortOption = new List<NKCUnitSortSystem.eSortOption>();
		options.lstCustomSortFunc = new Dictionary<NKCUnitSortSystem.eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>>();
		options.lstCustomSortFunc.Add(NKCUnitSortSystem.eSortCategory.Custom1, new KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>(NKCUtilString.GET_STRING_SORT_TIME, null));
		options.lstCustomSortFunc.Add(NKCUnitSortSystem.eSortCategory.Custom2, new KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>(NKCUtilString.GET_STRING_SORT_LEVEL, null));
		m_unitSortSystem = new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), options, new List<NKMUnitData>());
		if (m_unitSortOptions != null)
		{
			m_unitSortOptions.Init(OnSorted, bIsCollection: false);
			m_unitSortOptions.RegisterUnitSort(m_unitSortSystem);
			m_unitSortOptions.m_NKCPopupSort.m_bUseDefaultSortAdd = false;
		}
		m_sortCategory.Add(NKCUnitSortSystem.eSortCategory.Custom1);
		m_sortCategory.Add(NKCUnitSortSystem.eSortCategory.Custom2);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnClickBG(BaseEventData cBaseEventData)
	{
		Close();
	}

	public void OnRecv(NKMPacket_RAID_RESULT_ACCEPT_ACK cNKMPacket_RAID_RESULT_ACCEPT_ACK, int cityID)
	{
		if (m_tglEventList.m_bChecked)
		{
			int num = m_lstCityHasEvent.FindIndex((NKMWorldMapCityData x) => x.cityID == cityID);
			if (num != -1)
			{
				m_lstCityHasEvent.RemoveAt(num);
			}
			int count = m_lstCityHasEvent.Count;
			m_LoopScrollRect.TotalCount = count;
			m_LoopScrollRect.SetIndexPosition(0);
			m_lbEmptyMessage.text = NKCUtilString.GET_STRING_WORLDMAP_NO_EXIST_EVENT;
			NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
		}
		else if (m_tglJoinList.m_bChecked)
		{
			NKMRaidResultData nKMRaidResultData = m_RaidResultDataList.Find((NKMRaidResultData x) => x.raidUID == cNKMPacket_RAID_RESULT_ACCEPT_ACK.raidUID);
			if (nKMRaidResultData != null)
			{
				m_RaidResultDataList.Remove(nKMRaidResultData);
				m_LoopScrollRect.TotalCount = m_RaidResultDataList.Count;
				m_LoopScrollRect.SetIndexPosition(0);
				m_lbEmptyMessage.text = NKCUtilString.GET_STRING_WORLDMAP_NO_EXIST_JOIN;
				NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
			}
			UpdateReceiveAllRewardBtnUI();
		}
	}

	public void OnRecv(NKMPacket_RAID_RESULT_ACCEPT_ALL_ACK sPacket, List<int> lstCity)
	{
		if (m_tglEventList.m_bChecked)
		{
			foreach (int cityID in lstCity)
			{
				int num = m_lstCityHasEvent.FindIndex((NKMWorldMapCityData x) => x.cityID == cityID);
				if (num != -1)
				{
					m_lstCityHasEvent.RemoveAt(num);
				}
			}
			int count = m_lstCityHasEvent.Count;
			m_LoopScrollRect.TotalCount = count;
			m_LoopScrollRect.SetIndexPosition(0);
			m_lbEmptyMessage.text = NKCUtilString.GET_STRING_WORLDMAP_NO_EXIST_EVENT;
			NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
		}
		else
		{
			if (!m_tglJoinList.m_bChecked)
			{
				return;
			}
			bool flag = false;
			foreach (long raidUID in sPacket.raidUids)
			{
				NKMRaidResultData nKMRaidResultData = m_RaidResultDataList.Find((NKMRaidResultData x) => x.raidUID == raidUID);
				if (nKMRaidResultData != null)
				{
					m_RaidResultDataList.Remove(nKMRaidResultData);
					flag = true;
				}
			}
			if (flag)
			{
				m_LoopScrollRect.TotalCount = m_RaidResultDataList.Count;
				m_LoopScrollRect.SetIndexPosition(0);
				m_lbEmptyMessage.text = NKCUtilString.GET_STRING_WORLDMAP_NO_EXIST_JOIN;
				NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
			}
			UpdateReceiveAllRewardBtnUI();
		}
	}

	public void OnRecv(NKMPacket_RAID_COOP_LIST_ACK cNKMPacket_RAID_COOP_LIST_ACK)
	{
		if (cNKMPacket_RAID_COOP_LIST_ACK.coopRaidDataList == null)
		{
			NKCUtil.SetGameobjectActive(m_objHelpListReddot, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objHelpListReddot, cNKMPacket_RAID_COOP_LIST_ACK.coopRaidDataList.Count > 0);
		}
		if (m_eState != eState.HelpList)
		{
			return;
		}
		m_CoopRaidDataList = cNKMPacket_RAID_COOP_LIST_ACK.coopRaidDataList;
		m_CoopRaidDataList.Sort(delegate(NKMCoopRaidData e1, NKMCoopRaidData e2)
		{
			int num = SortCoopRaidDataWithSeason(e1, e2);
			if (num != 0)
			{
				return num;
			}
			num = SortCoopRaidWithExpireDate(e1, e2);
			return (num == 0) ? SortCoopRaidWithUserUId(e1, e2) : num;
		});
		m_LoopScrollRect.TotalCount = m_CoopRaidDataList.Count;
		m_LoopScrollRect.SetIndexPosition(0);
		m_lbEmptyMessage.text = NKCUtilString.GET_STRING_WORLDMAP_NO_EXIST_COOP;
		NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
	}

	public void OnRecv(NKMPacket_DIVE_EXPIRE_NOT cNKMPacket_DIVE_EXPIRE_NOT)
	{
		m_LoopScrollRect.RefreshCells();
	}

	public void OnRecv(NKMPacket_RAID_SEASON_NOT sPacket)
	{
		NKCUtil.SetGameobjectActive(m_objSeasonPointReddot, NKCAlarmManager.CheckRaidSeasonRewardNotify());
		NKCUtil.SetGameobjectActive(m_objExtraPointLock, !IsExtraPointOpen());
		RefreshUI();
	}

	public void OnRecv(NKMPacket_RAID_POINT_EXTRA_REWARD_ACK sPacket)
	{
		NKCUtil.SetGameobjectActive(m_objExtraPointReddot, NKCAlarmManager.CheckRaidSeasonExtraRewardNotify());
		RefreshUI();
	}

	private int GetCityIDHaveEventDiveOnGoing()
	{
		NKMDiveGameData diveGameData = NKCScenManager.CurrentUserData().m_DiveGameData;
		if (diveGameData != null)
		{
			return NKCScenManager.CurrentUserData().m_WorldmapData.GetCityIDByEventData(NKM_WORLDMAP_EVENT_TYPE.WET_DIVE, diveGameData.DiveUid);
		}
		return -1;
	}

	private bool CheckEventDiveOnGoing()
	{
		if (GetCityIDHaveEventDiveOnGoing() == -1)
		{
			return false;
		}
		return true;
	}

	private int GetStartedEventDiveCount()
	{
		return NKCScenManager.CurrentUserData().m_WorldmapData?.GetStartedEventCount(NKM_WORLDMAP_EVENT_TYPE.WET_DIVE) ?? 0;
	}

	private int GetStartedEventDiveCityID(int index)
	{
		return NKCScenManager.CurrentUserData().m_WorldmapData?.GetStartedEventCityID(NKM_WORLDMAP_EVENT_TYPE.WET_DIVE, index) ?? (-1);
	}

	public void OnRecv(NKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_ACK cNKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_ACK)
	{
		int num = m_lstCityHasEvent.FindIndex((NKMWorldMapCityData x) => x.cityID == cNKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_ACK.cityID);
		if (num != -1)
		{
			m_lstCityHasEvent.RemoveAt(num);
		}
		int num2 = m_lstCityHasEvent.Count;
		if (CheckEventDiveOnGoing())
		{
			num2++;
		}
		m_LoopScrollRect.TotalCount = num2;
		m_LoopScrollRect.SetIndexPosition(0);
		m_lbEmptyMessage.text = NKCUtilString.GET_STRING_WORLDMAP_NO_EXIST_EVENT;
		NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
	}

	public void OnRecv(NKMPacket_WORLDMAP_EVENT_CANCEL_ACK cNKMPacket_WORLDMAP_EVENT_CANCEL_ACK)
	{
		int num = m_lstCityHasEvent.FindIndex((NKMWorldMapCityData x) => x.cityID == cNKMPacket_WORLDMAP_EVENT_CANCEL_ACK.cityID);
		if (num != -1)
		{
			m_lstCityHasEvent.RemoveAt(num);
		}
		if (m_eState == eState.EventList)
		{
			m_LoopScrollRect.TotalCount = m_lstCityHasEvent.Count;
			m_LoopScrollRect.RefreshCells();
			m_lbEmptyMessage.text = NKCUtilString.GET_STRING_WORLDMAP_NO_EXIST_EVENT;
			NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
		}
		else
		{
			if (m_eState != eState.JoinList)
			{
				return;
			}
			for (int num2 = 0; num2 < m_RaidResultDataList.Count; num2++)
			{
				if (m_RaidResultDataList[num2].cityID == cNKMPacket_WORLDMAP_EVENT_CANCEL_ACK.cityID)
				{
					m_RaidResultDataList.RemoveAt(num2);
					break;
				}
			}
			int count = m_RaidResultDataList.Count;
			count += GetStartedEventDiveCount();
			m_LoopScrollRect.TotalCount = count;
			m_LoopScrollRect.SetIndexPosition(0);
			m_lbEmptyMessage.text = NKCUtilString.GET_STRING_WORLDMAP_NO_EXIST_JOIN;
			NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
		}
	}

	public void OnRecv(NKMPacket_RAID_RESULT_LIST_ACK cNKMPacket_RAID_RESULT_LIST_ACK)
	{
		m_RaidResultDataList = cNKMPacket_RAID_RESULT_LIST_ACK.raidResultDataList;
		int count = m_RaidResultDataList.Count;
		count += GetStartedEventDiveCount();
		UpdateJoinListReddot();
		if (m_eState == eState.JoinList)
		{
			m_LoopScrollRect.TotalCount = count;
			m_LoopScrollRect.SetIndexPosition(0);
			m_lbEmptyMessage.text = NKCUtilString.GET_STRING_WORLDMAP_NO_EXIST_JOIN;
			NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
			UpdateReceiveAllRewardBtnUI();
		}
	}

	private void UpdateJoinListReddot()
	{
		bool bValue = false;
		foreach (NKMRaidResultData raidResultData in m_RaidResultDataList)
		{
			if (!raidResultData.IsOnGoing())
			{
				bValue = true;
				break;
			}
		}
		NKCUtil.SetGameobjectActive(m_objJoinListReddot, bValue);
	}

	private void OnClickReceiveAllRaidResult()
	{
		if (m_bCanReceiveAllReward)
		{
			NKCPacketSender.Send_NKMPacket_RAID_RESULT_ACCEPT_ALL_REQ();
		}
	}

	private void UpdateReceiveAllRewardBtnUI()
	{
		m_bCanReceiveAllReward = false;
		foreach (NKMRaidResultData raidResultData in m_RaidResultDataList)
		{
			if (!raidResultData.IsOnGoing())
			{
				m_bCanReceiveAllReward = true;
				break;
			}
		}
		if (!m_bCanReceiveAllReward)
		{
			NKCUtil.SetImageSprite(m_imgRaidAllReceive, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
			NKCUtil.SetLabelTextColor(m_lbRaidAllReceive, NKCUtil.GetColor("#212122"));
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgRaidAllReceive, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW));
			NKCUtil.SetLabelTextColor(m_lbRaidAllReceive, m_RaidAllReceiveOriginalColor);
		}
	}

	private RectTransform GetSlot(int index)
	{
		NKCPopupWorldMapEventListSlot nKCPopupWorldMapEventListSlot = null;
		if (m_stkSlot.Count > 0)
		{
			nKCPopupWorldMapEventListSlot = m_stkSlot.Pop();
		}
		if (nKCPopupWorldMapEventListSlot == null)
		{
			nKCPopupWorldMapEventListSlot = Object.Instantiate(m_pfbSlot);
		}
		if (nKCPopupWorldMapEventListSlot != null)
		{
			nKCPopupWorldMapEventListSlot.transform.localPosition = Vector3.zero;
			nKCPopupWorldMapEventListSlot.transform.localScale = Vector3.one;
			nKCPopupWorldMapEventListSlot.transform.SetParent(m_LoopScrollRect.content);
			return nKCPopupWorldMapEventListSlot.GetComponent<RectTransform>();
		}
		return null;
	}

	private void ReturnSlot(Transform tr)
	{
		NKCPopupWorldMapEventListSlot component = tr.GetComponent<NKCPopupWorldMapEventListSlot>();
		if (component != null)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			m_stkSlot.Push(component);
		}
	}

	private void ProvideSlotData(Transform transform, int idx)
	{
		NKCPopupWorldMapEventListSlot component = transform.GetComponent<NKCPopupWorldMapEventListSlot>();
		if (component == null)
		{
			return;
		}
		switch (m_eState)
		{
		case eState.EventList:
			NKCUtil.SetGameobjectActive(m_objSlotList, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKCUIRaidSeasonPoint, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKCUIRaidSeasonExtraPoint, bValue: false);
			NKCUtil.SetGameobjectActive(component, bValue: true);
			component.SetDataForMyEventList(m_lstCityHasEvent[idx].worldMapEventGroup, m_lstCityHasEvent[idx], OnSlotMove);
			break;
		case eState.HelpList:
			NKCUtil.SetGameobjectActive(m_objSlotList, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKCUIRaidSeasonPoint, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKCUIRaidSeasonExtraPoint, bValue: false);
			NKCUtil.SetGameobjectActive(component, bValue: true);
			component.SetDataForHelpList(m_CoopRaidDataList[idx]);
			break;
		case eState.JoinList:
		{
			NKCUtil.SetGameobjectActive(m_objSlotList, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKCUIRaidSeasonPoint, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKCUIRaidSeasonExtraPoint, bValue: false);
			NKCUtil.SetGameobjectActive(component, bValue: true);
			int startedEventDiveCount = GetStartedEventDiveCount();
			if (startedEventDiveCount > 0)
			{
				if (idx < startedEventDiveCount)
				{
					int startedEventDiveCityID = GetStartedEventDiveCityID(idx);
					NKMWorldMapCityData cityData = NKCScenManager.CurrentUserData().m_WorldmapData.GetCityData(startedEventDiveCityID);
					if (cityData != null)
					{
						component.SetDataForMyEventList(cityData.worldMapEventGroup, cityData, OnSlotMove);
					}
				}
				else
				{
					component.SetDataForJoinList(m_RaidResultDataList[idx - startedEventDiveCount]);
				}
			}
			else
			{
				component.SetDataForJoinList(m_RaidResultDataList[idx]);
			}
			break;
		}
		}
	}

	public void Open(NKMWorldMapData worldmapData, OnSelectEvent onSelectEvent, bool bHelpListReddot, bool bJoinRaidReddot, bool selectLastTab)
	{
		NKCUtil.SetGameobjectActive(m_objHelpListReddot, bHelpListReddot);
		NKCUtil.SetGameobjectActive(m_objSeasonPointReddot, NKCAlarmManager.CheckRaidSeasonRewardNotify());
		NKCUtil.SetGameobjectActive(m_objExtraPointReddot, NKCAlarmManager.CheckRaidSeasonExtraRewardNotify());
		NKCUtil.SetGameobjectActive(m_objJoinListReddot, bJoinRaidReddot);
		NKCUtil.SetGameobjectActive(m_tglExtraPoint, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.RAID_REPEAT_REWARD));
		NKCUtil.SetGameobjectActive(m_objExtraPointLock, !IsExtraPointOpen());
		base.gameObject.SetActive(value: true);
		dOnSelectEvent = onSelectEvent;
		RefreshEventDataList(worldmapData);
		if (selectLastTab)
		{
			SetState(m_openState);
		}
		else
		{
			SetState(eState.EventList);
		}
		NKCUtil.SetGameobjectActive(m_objRaidSeasonInfo, bValue: false);
		NKMRaidSeasonTemplet seasonTemplet = NKCRaidSeasonManager.GetNowSeasonTemplet();
		if (seasonTemplet != null)
		{
			NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMTempletContainer<NKMWorldMapEventTemplet>.Values.ToList().Find((NKMWorldMapEventTemplet x) => x.raidBossId == seasonTemplet.RaidBossId);
			if (nKMWorldMapEventTemplet != null)
			{
				NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(nKMWorldMapEventTemplet.stageID);
				if (nKMRaidTemplet != null)
				{
					NKCUtil.SetGameobjectActive(m_objRaidSeasonInfo, bValue: true);
					NKCUtil.SetLabelText(m_lbRaidBossName, nKMRaidTemplet.DungeonTempletBase?.GetDungeonName());
					NKCUtil.SetLabelText(m_lbRaidSeasonTimeLeft, NKCUtilString.GetRemainTimeStringOneParam(seasonTemplet.IntervalTemplet.GetEndDateUtc()));
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRaidSeasonInfo, bValue: false);
		}
		m_bCanReceiveAllReward = false;
		UIOpened();
	}

	private void SetState(eState state)
	{
		NKMRaidSeasonTemplet nowSeasonTemplet = NKCRaidSeasonManager.GetNowSeasonTemplet();
		if (nowSeasonTemplet == null)
		{
			if (m_eState == eState.SeasonPoint)
			{
				m_tglEventList.Select(bSelect: true, bForce: true, bImmediate: true);
				m_eState = eState.EventList;
			}
			m_tglSeasonPoint.Lock();
			m_tglExtraPoint?.Lock();
		}
		else
		{
			m_tglSeasonPoint.UnLock();
			m_tglExtraPoint?.UnLock();
		}
		m_eState = state;
		switch (m_eState)
		{
		case eState.EventList:
			NKCUtil.SetGameobjectActive(m_objSlotList, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSeasonPoint, bValue: false);
			m_LoopScrollRect.TotalCount = m_lstCityHasEvent.Count;
			m_LoopScrollRect.SetIndexPosition(0);
			m_lbEmptyMessage.text = NKCUtilString.GET_STRING_WORLDMAP_NO_EXIST_EVENT;
			NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
			m_tglEventList.Select(bSelect: true, bForce: true);
			break;
		case eState.HelpList:
			NKCUtil.SetGameobjectActive(m_objSlotList, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSeasonPoint, bValue: false);
			m_LoopScrollRect.TotalCount = 0;
			m_LoopScrollRect.RefreshCells();
			m_tglHelpList.Select(bSelect: true, bForce: true);
			m_lbEmptyMessage.text = NKCUtilString.GET_STRING_WORLDMAP_NO_EXIST_COOP;
			NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
			ResetSortUI();
			NKCPacketSender.Send_NKMPacket_RAID_COOP_LIST_REQ();
			break;
		case eState.JoinList:
			NKCUtil.SetGameobjectActive(m_objSlotList, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSeasonPoint, bValue: false);
			m_LoopScrollRect.TotalCount = 0;
			m_LoopScrollRect.RefreshCells();
			m_tglJoinList.Select(bSelect: true, bForce: true);
			m_lbEmptyMessage.text = NKCUtilString.GET_STRING_WORLDMAP_NO_EXIST_JOIN;
			NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
			NKCPacketSender.Send_NKMPacket_RAID_RESULT_LIST_REQ();
			break;
		case eState.SeasonPoint:
			NKCUtil.SetGameobjectActive(m_objSlotList, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSeasonPoint, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSeasonNormalPoint, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSeasonExtraPoint, bValue: false);
			if (nowSeasonTemplet != null)
			{
				m_lstSeasonPointData = new List<NKCUISeasonPointSlot.SeasonPointSlotData>();
				m_lstSeasonPointData.Add(NKCUISeasonPointSlot.SeasonPointSlotData.MakeEmptyData());
				foreach (NKMRaidSeasonRewardTemplet value in NKMRaidSeasonRewardTemplet.Values)
				{
					if (value.RewardBoardId == nowSeasonTemplet.RaidBoardId)
					{
						m_lstSeasonPointData.Add(NKCUISeasonPointSlot.SeasonPointSlotData.MakeSeasonPointSlotData(value));
					}
				}
				NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
				m_NKCUIRaidSeasonPoint.Open(m_lstSeasonPointData, NKCStringTable.GetString("SI_PF_WORLD_MAP_RENEWAL_EVENT_POPUP_REWARD_TITLE"), NKCRaidSeasonManager.RaidSeason.monthlyPoint, NKCRaidSeasonManager.RaidSeason.recvRewardRaidPoint, nowSeasonTemplet.IntervalTemplet, OnClickSeasonPointReward);
				m_tglSeasonPoint.Select(bSelect: true, bForce: true);
			}
			else
			{
				SetState(eState.EventList);
			}
			break;
		case eState.ExtraPoint:
			NKCUtil.SetGameobjectActive(m_objSlotList, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSeasonPoint, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSeasonNormalPoint, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSeasonExtraPoint, bValue: true);
			if (!IsExtraPointOpen())
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_DP_RAID_EXTRA_REWARD_TAB_NOT_ENTER"));
				SetState(eState.EventList);
			}
			else
			{
				m_NKCUIRaidSeasonExtraPoint.Open();
				m_tglExtraPoint.Select(bSelect: true, bForce: true);
			}
			break;
		}
		NKCUtil.SetGameobjectActive(m_objRaidAllReceive, m_eState == eState.JoinList);
		if (m_objRaidAllReceive != null && m_objRaidAllReceive.activeSelf)
		{
			RectTransform component = m_objRaidAllReceive.GetComponent<RectTransform>();
			RectTransform component2 = m_LoopScrollRect.gameObject.GetComponent<RectTransform>();
			component2.offsetMin = new Vector2(component2.offsetMin.x, component.GetHeight());
		}
		else
		{
			RectTransform component3 = m_LoopScrollRect.gameObject.GetComponent<RectTransform>();
			component3.offsetMin = new Vector2(component3.offsetMin.x, 0f);
		}
		NKCUtil.SetGameobjectActive(m_unitSortOptions, m_eState == eState.HelpList);
		NKCUtil.SetGameobjectActive(m_csbtnRaidCoopAll, m_eState == eState.EventList || m_eState == eState.JoinList);
		if (m_csbtnRaidCoopAll != null && m_csbtnRaidCoopAll.gameObject.activeSelf)
		{
			m_csbtnRaidCoopAll.SetLock(!NKCUtil.CanRaidCoop());
		}
	}

	private bool IsExtraPointOpen()
	{
		bool result = false;
		if (NKCRaidSeasonManager.GetNowSeasonTemplet() != null)
		{
			NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
			result = NKCRaidSeasonManager.RaidSeason.recvRewardRaidPoint >= GetMaxSeasonPoint();
		}
		return result;
	}

	public static int GetMaxSeasonPoint()
	{
		NKMRaidSeasonTemplet nowSeasonTemplet = NKCRaidSeasonManager.GetNowSeasonTemplet();
		if (nowSeasonTemplet != null)
		{
			new List<NKCUISeasonPointSlot.SeasonPointSlotData>();
			int num = 0;
			{
				foreach (NKMRaidSeasonRewardTemplet value in NKMRaidSeasonRewardTemplet.Values)
				{
					if (value.RewardBoardId == nowSeasonTemplet.RaidBoardId)
					{
						NKCUISeasonPointSlot.SeasonPointSlotData seasonPointSlotData = NKCUISeasonPointSlot.SeasonPointSlotData.MakeSeasonPointSlotData(value);
						if (num < seasonPointSlotData.SlotPoint)
						{
							num = seasonPointSlotData.SlotPoint;
						}
					}
				}
				return num;
			}
		}
		return int.MaxValue;
	}

	public void RefreshEventDataList(NKMWorldMapData worldMapData)
	{
		m_lstCityHasEvent.Clear();
		foreach (KeyValuePair<int, NKMWorldMapCityData> item in worldMapData.worldMapCityDataMap)
		{
			NKMWorldMapCityData value = item.Value;
			if (value.worldMapEventGroup == null || value.worldMapEventGroup.worldmapEventID == 0)
			{
				continue;
			}
			NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(value.worldMapEventGroup.worldmapEventID);
			if (nKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_RAID)
			{
				NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(value.worldMapEventGroup.eventUid);
				if (nKMRaidDetailData != null && !nKMRaidDetailData.isNew)
				{
					continue;
				}
			}
			else if (nKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_DIVE && value.worldMapEventGroup.eventUid > 0)
			{
				continue;
			}
			m_lstCityHasEvent.Add(value);
		}
	}

	private void OnEventList(bool value)
	{
		if (value)
		{
			SetState(eState.EventList);
		}
	}

	private void OnHelpList(bool value)
	{
		if (value)
		{
			SetState(eState.HelpList);
		}
	}

	private void OnJoinList(bool value)
	{
		if (value)
		{
			SetState(eState.JoinList);
		}
	}

	private void OnSeasonPoint(bool bValue)
	{
		if (bValue)
		{
			SetState(eState.SeasonPoint);
		}
	}

	private void OnSlotMove(int cityID, int eventID, long eventUID)
	{
		dOnSelectEvent?.Invoke(cityID, eventID, eventUID);
	}

	private void OnClickSeasonPointReward(NKCUISeasonPointSlot.SeasonPointSlotData slotData)
	{
		if (slotData.SlotPoint <= NKCRaidSeasonManager.RaidSeason.recvRewardRaidPoint || slotData.SlotPoint > NKCRaidSeasonManager.RaidSeason.monthlyPoint)
		{
			NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, NKCUISlot.SlotData.MakeRewardTypeData(slotData.RewardType, slotData.RewardID, slotData.RewardCount));
			return;
		}
		int num = 0;
		foreach (NKMRaidSeasonRewardTemplet value in NKMRaidSeasonRewardTemplet.Values)
		{
			if (value.RewardBoardId == slotData.ID && value.RaidPoint >= num && value.RaidPoint > NKCRaidSeasonManager.RaidSeason.recvRewardRaidPoint && value.RaidPoint >= num && value.RaidPoint <= NKCRaidSeasonManager.RaidSeason.monthlyPoint)
			{
				num = value.RaidPoint;
			}
		}
		NKCPacketSender.Send_NKMPacket_RAID_POINT_REWARD_REQ(num);
	}

	private void OnClickRaidCoopRequestAll()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_WORLDMAP_RAID_COOP_REQUEST_ALL, NKCPacketSender.Send_NKMPacket_RAID_SET_COOP_ALL_REQ);
	}

	private void ResetSortUI()
	{
		m_unitSortSystem.Descending = false;
		if (m_unitSortSystem.lstSortOption == null)
		{
			m_unitSortSystem.lstSortOption = new List<NKCUnitSortSystem.eSortOption>();
		}
		m_unitSortSystem.lstSortOption.Clear();
		m_unitSortSystem.FilterSet?.Clear();
		NKCUnitSortSystem.eSortOption item = NKCUnitSortSystem.eSortOption.None;
		using (HashSet<NKCUnitSortSystem.eSortCategory>.Enumerator enumerator = m_sortCategory.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				item = NKCUnitSortSystem.GetSortOptionByCategory(enumerator.Current, bDescending: true);
			}
		}
		m_unitSortSystem.lstSortOption.Add(item);
		if (m_unitSortOptions != null)
		{
			m_unitSortOptions.RegisterCategories(null, m_sortCategory, bFavoriteFilterActive: false);
			m_unitSortOptions.ResetUI();
		}
	}

	private void OnSorted(bool bResetScroll)
	{
		if (m_unitSortSystem.lstSortOption == null || m_unitSortSystem.lstSortOption.Count <= 0 || m_unitSortSystem.lstSortOption[0] == NKCUnitSortSystem.eSortOption.None)
		{
			return;
		}
		if (m_unitSortSystem.lstSortOption[0] == NKCUnitSortSystem.eSortOption.CustomAscend1 || m_unitSortSystem.lstSortOption[0] == NKCUnitSortSystem.eSortOption.CustomDescend1)
		{
			m_CoopRaidDataList.Sort(delegate(NKMCoopRaidData e1, NKMCoopRaidData e2)
			{
				int num = SortCoopRaidDataWithSeason(e1, e2);
				if (num != 0)
				{
					return num;
				}
				num = SortCoopRaidWithExpireDate(e1, e2);
				return (num == 0) ? SortCoopRaidWithUserUId(e1, e2) : num;
			});
		}
		else if (m_unitSortSystem.lstSortOption[0] == NKCUnitSortSystem.eSortOption.CustomAscend2 || m_unitSortSystem.lstSortOption[0] == NKCUnitSortSystem.eSortOption.CustomDescend2)
		{
			m_CoopRaidDataList.Sort(delegate(NKMCoopRaidData e1, NKMCoopRaidData e2)
			{
				int num = SortCoopRaidDataWithSeason(e1, e2);
				if (num != 0)
				{
					return num;
				}
				num = SortCoopRaidWithRaidLevel(e1, e2);
				if (num != 0)
				{
					return num;
				}
				num = SortCoopRaidWithExpireDate(e1, e2);
				return (num == 0) ? SortCoopRaidWithUserUId(e1, e2) : num;
			});
		}
		m_LoopScrollRect.SetIndexPosition(0);
	}

	private int SortCoopRaidDataWithSeason(NKMCoopRaidData e1, NKMCoopRaidData e2)
	{
		if (e1 == null || e2 == null)
		{
			return 0;
		}
		NKMRaidSeasonTemplet nowSeasonTemplet = NKCRaidSeasonManager.GetNowSeasonTemplet();
		bool flag = nowSeasonTemplet != null && nowSeasonTemplet.RaidSeasonId == e1.seasonID;
		bool flag2 = nowSeasonTemplet != null && nowSeasonTemplet.RaidSeasonId == e2.seasonID;
		if (flag && !flag2)
		{
			return -1;
		}
		if (!flag && flag2)
		{
			return 1;
		}
		return 0;
	}

	private int SortCoopRaidWithRaidLevel(NKMCoopRaidData e1, NKMCoopRaidData e2)
	{
		if (e1 == null || e2 == null)
		{
			return 0;
		}
		NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(e1.stageID);
		NKMRaidTemplet nKMRaidTemplet2 = NKMRaidTemplet.Find(e2.stageID);
		if (nKMRaidTemplet.RaidLevel > nKMRaidTemplet2.RaidLevel)
		{
			if (!m_unitSortSystem.Descending)
			{
				return 1;
			}
			return -1;
		}
		if (nKMRaidTemplet.RaidLevel < nKMRaidTemplet2.RaidLevel)
		{
			if (!m_unitSortSystem.Descending)
			{
				return -1;
			}
			return 1;
		}
		return 0;
	}

	private int SortCoopRaidWithExpireDate(NKMCoopRaidData e1, NKMCoopRaidData e2)
	{
		if (e1 == null || e2 == null)
		{
			return 0;
		}
		if (e1.expireDate > e2.expireDate)
		{
			if (!m_unitSortSystem.Descending)
			{
				return 1;
			}
			return -1;
		}
		if (e1.expireDate < e2.expireDate)
		{
			if (!m_unitSortSystem.Descending)
			{
				return -1;
			}
			return 1;
		}
		return 0;
	}

	private int SortCoopRaidWithUserUId(NKMCoopRaidData e1, NKMCoopRaidData e2)
	{
		if (e1 == null || e2 == null)
		{
			return 0;
		}
		if (e1.userUID < e2.userUID)
		{
			if (!m_unitSortSystem.Descending)
			{
				return 1;
			}
			return -1;
		}
		if (e1.userUID > e2.userUID)
		{
			if (!m_unitSortSystem.Descending)
			{
				return -1;
			}
			return 1;
		}
		return 0;
	}

	public void RefreshUI()
	{
		SetState(m_eState);
	}

	private void Update()
	{
		if (m_eState != eState.SeasonPoint)
		{
			return;
		}
		bDeltaTime += Time.deltaTime;
		if (bDeltaTime > 1f)
		{
			bDeltaTime -= 1f;
			if (NKCRaidSeasonManager.GetNowSeasonTemplet() == null)
			{
				RefreshUI();
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_WORLD_MAP_RAID_SEASON_END);
			}
		}
	}
}
