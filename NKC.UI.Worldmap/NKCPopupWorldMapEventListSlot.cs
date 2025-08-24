using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Raid;
using ClientPacket.WorldMap;
using Cs.Math;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Worldmap;

public class NKCPopupWorldMapEventListSlot : MonoBehaviour
{
	private enum MiddleState
	{
		Normal,
		Raid
	}

	private enum eButtonState_Top
	{
		None,
		Process,
		Help,
		Result,
		Delete
	}

	private enum eButtonState_Bottom
	{
		None,
		Help,
		Move,
		OK
	}

	private enum eEventState
	{
		None,
		Fail,
		MVP,
		Progress,
		Complete,
		Expired
	}

	public delegate void OnMove(int cityID, int eventID, long eventUID);

	private MiddleState m_eMiddleState;

	private DateTime m_dtEndTime;

	private NKCPopupWorldMapEventList.eState m_TabState;

	private NKM_WORLDMAP_EVENT_TYPE m_NKM_WORLDMAP_EVENT_TYPE = NKM_WORLDMAP_EVENT_TYPE.WET_NONE;

	private NKMCoopRaidData m_cNKMCoopRaidData;

	private NKMRaidResultData m_cNKMRaidResultData;

	public Image m_imgThumbnail;

	public GameObject m_objEventTypeRaid;

	public GameObject m_objEventTypeRaidExpried;

	public GameObject m_objEventTypeDive;

	[Header("일반")]
	public GameObject m_objRootEventNormal;

	public Text m_lbLevel_N;

	private int m_level_N;

	public Text m_lbName_N;

	public Text m_lbDiveLevel;

	public GameObject m_objTimeLeft;

	public Text m_lbTimeLeft;

	public Text m_lbDiveSlotCount;

	public Image m_imgEventPointColor_N;

	[Header("레이드 뷰")]
	public GameObject m_objRootEventRaid;

	public Text m_lbLevel_R;

	private int m_level_R;

	public Text m_lbName_R;

	public Text m_lbHPLeft_R;

	public Text m_lbTargetHP_R;

	public Image m_imgTargetHP_R;

	public Image m_imgEventPointColor_R;

	public GameObject m_objEntryCheck;

	public GameObject m_objTryCount;

	public Text m_lbTryCount;

	[Header("최고 피해 표시")]
	public GameObject m_objMVP_R;

	public GameObject m_objMyMVP;

	public Text m_lbMVPName_R;

	[Header("이벤트 상태표시")]
	public GameObject m_objProgress_R;

	public GameObject m_objFail_R;

	public Text m_lbFail_R;

	public GameObject m_objComplete_R;

	[Header("참가인원 표시")]
	public GameObject m_objAttendLimit;

	public Text m_lbAttendLimit;

	[Header("도시정보")]
	public GameObject m_objRootCityInfo;

	public Text m_lbCityLevel;

	public Image m_imgCityExp;

	public Image m_imgCityManager;

	public Text m_lbCityName;

	public Text m_lbCityTitle;

	[Header("지원 도시정보")]
	public GameObject m_objRootCityInfoCoop;

	public Text m_lbCityLevelCoop;

	public Image m_imgCityExpCoop;

	public Image m_imgCityManagerCoop;

	public Text m_lbCityNameCoop;

	public Text m_lbCityTitleCoop;

	[Header("지원요청")]
	public GameObject m_objRootHelp;

	public Image m_imgHelpUserIcon;

	public Text m_lbHelpUserName;

	public Text m_lbHelpUserUID;

	public Color m_colUserName;

	public Color m_colUserNameWhenMe;

	public GameObject m_objGuildRoot;

	public NKCUIGuildBadge m_guildBadge;

	public Text m_lbGuildName;

	public GameObject m_objMyHelp;

	public GameObject m_objRootHelpUserIcon;

	public GameObject m_objGuildMemberIcon;

	public GameObject m_objFriendIcon;

	[Header("버튼 상단")]
	public GameObject m_objBtnOnProgress;

	public GameObject m_objCoop;

	public GameObject m_csbtnResult;

	public NKCUIComStateButton m_csbtnDelete;

	[Header("버튼 하단")]
	public NKCUIComStateButton m_csbtnPlay;

	public GameObject m_objBtnBlue;

	public Text m_lbBtnBlue;

	public GameObject m_objBtnOK;

	private int m_cityID;

	private int m_eventID;

	private long m_eventUID;

	private bool m_bNeedTimeUpdate = true;

	private OnMove dOnMove;

	private float m_fTimer;

	public void SetDataForMyEventList(NKMWorldMapEventGroup eventGroupData, NKMWorldMapCityData cityData, OnMove onMove)
	{
		dOnMove = onMove;
		m_TabState = NKCPopupWorldMapEventList.eState.EventList;
		SetCityData(cityData);
		SetDataCommon(eventGroupData);
		SetCityDataCoop(0);
	}

	public void SetDataForJoinList(NKMRaidResultData cNKMRaidResultData)
	{
		if (cNKMRaidResultData != null)
		{
			m_NKM_WORLDMAP_EVENT_TYPE = NKM_WORLDMAP_EVENT_TYPE.WET_RAID;
			m_TabState = NKCPopupWorldMapEventList.eState.JoinList;
			m_eventUID = cNKMRaidResultData.raidUID;
			SetSupportReqUser(cNKMRaidResultData.userUID, cNKMRaidResultData.nickname, cNKMRaidResultData.mainUnitID, cNKMRaidResultData.mainUnitSkinID, cNKMRaidResultData.friendCode, cNKMRaidResultData.guildData);
			m_dtEndTime = new DateTime(cNKMRaidResultData.expireDate);
			SetRaidInfo(cNKMRaidResultData);
			SetRaidThumbnail(cNKMRaidResultData.stageID);
			SetCityDataCoop(cNKMRaidResultData.cityID);
		}
	}

	private void SetRaidThumbnail(int stageID)
	{
		NKMWorldMapEventTemplet worldMapEventTempletByStageID = NKMWorldMapManager.GetWorldMapEventTempletByStageID(stageID);
		Sprite thumbnail = GetThumbnail(worldMapEventTempletByStageID);
		NKCUtil.SetImageSprite(m_imgThumbnail, thumbnail, bDisableIfSpriteNull: true);
	}

	public void SetDataForHelpList(NKMCoopRaidData cNKMCoopRaidData)
	{
		if (cNKMCoopRaidData != null)
		{
			m_TabState = NKCPopupWorldMapEventList.eState.HelpList;
			m_NKM_WORLDMAP_EVENT_TYPE = NKM_WORLDMAP_EVENT_TYPE.WET_RAID;
			m_eventUID = cNKMCoopRaidData.raidUID;
			SetSupportReqUser(cNKMCoopRaidData.userUID, cNKMCoopRaidData.nickname, cNKMCoopRaidData.mainUnitID, cNKMCoopRaidData.mainUnitSkinID, cNKMCoopRaidData.friendCode, cNKMCoopRaidData.guildData);
			m_dtEndTime = new DateTime(cNKMCoopRaidData.expireDate);
			SetRaidInfo(cNKMCoopRaidData);
			SetRaidThumbnail(cNKMCoopRaidData.stageID);
			SetCityDataCoop(cNKMCoopRaidData.cityId);
		}
	}

	private void SetSupportReqUser(long userUID, string nickName, int mainUnitID, int mainUnitSkinID, long friendCode, NKMGuildSimpleData guildData)
	{
		NKCUtil.SetGameobjectActive(m_objRootCityInfo, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRootHelp, bValue: true);
		NKCUtil.SetGameobjectActive(m_objRootHelpUserIcon, bValue: true);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(mainUnitID);
		m_imgHelpUserIcon.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase, mainUnitSkinID);
		m_lbHelpUserName.text = nickName;
		m_lbHelpUserUID.text = NKCUtilString.GetFriendCode(friendCode);
		SetGuildData(guildData);
		if (NKCScenManager.CurrentUserData().m_UserUID == userUID)
		{
			m_lbHelpUserName.color = m_colUserNameWhenMe;
			NKCUtil.SetGameobjectActive(m_objGuildMemberIcon, bValue: false);
			NKCUtil.SetGameobjectActive(m_objFriendIcon, bValue: false);
		}
		else
		{
			m_lbHelpUserName.color = m_colUserName;
			bool flag = NKCGuildManager.IsGuildMember(friendCode);
			bool flag2 = NKCFriendManager.IsFriend(friendCode);
			NKCUtil.SetGameobjectActive(m_objGuildMemberIcon, flag);
			NKCUtil.SetGameobjectActive(m_objFriendIcon, !flag && flag2);
		}
		NKCUtil.SetGameobjectActive(m_objMyHelp, NKCScenManager.CurrentUserData().m_UserUID == userUID);
	}

	private void SetGuildData(NKMGuildSimpleData guildSimpleData)
	{
		bool flag = guildSimpleData != null && guildSimpleData.guildUid > 0;
		NKCUtil.SetGameobjectActive(m_objGuildRoot, flag);
		if (flag)
		{
			m_guildBadge?.SetData(guildSimpleData.badgeId);
			NKCUtil.SetLabelText(m_lbGuildName, guildSimpleData.guildName);
		}
	}

	private void SetCityData(NKMWorldMapCityData cityData)
	{
		NKCUtil.SetGameobjectActive(m_objRootHelp, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRootHelpUserIcon, bValue: false);
		if (cityData == null)
		{
			NKCUtil.SetGameobjectActive(m_objRootCityInfo, bValue: false);
			m_cityID = 0;
			return;
		}
		m_cityID = cityData.cityID;
		NKCUtil.SetGameobjectActive(m_objRootCityInfo, bValue: true);
		NKCUtil.SetLabelText(m_lbCityLevel, cityData.level.ToString());
		if (m_imgCityExp != null)
		{
			m_imgCityExp.fillAmount = NKMWorldMapManager.GetCityExpPercent(cityData);
		}
		NKMWorldMapCityTemplet cityTemplet = NKMWorldMapManager.GetCityTemplet(cityData.cityID);
		NKCUtil.SetLabelText(m_lbCityName, (cityTemplet != null) ? cityTemplet.GetName() : "");
		NKCUtil.SetLabelText(m_lbCityTitle, (cityTemplet != null) ? cityTemplet.GetTitle() : "");
		NKMUnitData nKMUnitData = NKCScenManager.CurrentUserData()?.m_ArmyData.GetUnitFromUID(cityData.leaderUnitUID);
		Sprite sp = null;
		if (nKMUnitData != null)
		{
			sp = NKCResourceUtility.GetOrLoadMinimapFaceIcon(NKMUnitManager.GetUnitTempletBase(nKMUnitData.m_UnitID));
		}
		NKCUtil.SetImageSprite(m_imgCityManager, sp, bDisableIfSpriteNull: true);
	}

	private void SetCityDataCoop(int cityId)
	{
		NKMWorldMapCityData nKMWorldMapCityData = NKCScenManager.CurrentUserData()?.m_WorldmapData.GetCityData(cityId);
		if (nKMWorldMapCityData == null)
		{
			NKCUtil.SetGameobjectActive(m_objRootCityInfoCoop, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objRootCityInfoCoop, bValue: true);
		NKCUtil.SetLabelText(m_lbCityLevelCoop, nKMWorldMapCityData.level.ToString());
		float cityExpPercent = NKMWorldMapManager.GetCityExpPercent(nKMWorldMapCityData);
		NKCUtil.SetImageFillAmount(m_imgCityExpCoop, cityExpPercent);
		NKMWorldMapCityTemplet cityTemplet = NKMWorldMapManager.GetCityTemplet(nKMWorldMapCityData.cityID);
		NKCUtil.SetLabelText(m_lbCityNameCoop, (cityTemplet != null) ? cityTemplet.GetName() : "");
		NKCUtil.SetLabelText(m_lbCityTitleCoop, (cityTemplet != null) ? cityTemplet.GetTitle() : "");
		NKMUnitData nKMUnitData = NKCScenManager.CurrentUserData()?.m_ArmyData.GetUnitFromUID(nKMWorldMapCityData.leaderUnitUID);
		Sprite sp = null;
		if (nKMUnitData != null)
		{
			sp = NKCResourceUtility.GetOrLoadMinimapFaceIcon(NKMUnitManager.GetUnitTempletBase(nKMUnitData.m_UnitID));
		}
		NKCUtil.SetImageSprite(m_imgCityManagerCoop, sp, bDisableIfSpriteNull: true);
	}

	private void SetDataCommon(NKMWorldMapEventGroup eventGroupData)
	{
		if (eventGroupData == null)
		{
			Debug.LogError("EventData Null!");
			m_eventID = 0;
			m_eventUID = 0L;
			return;
		}
		m_eventID = eventGroupData.worldmapEventID;
		m_eventUID = eventGroupData.eventUid;
		m_dtEndTime = eventGroupData.eventGroupEndDate;
		NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(eventGroupData.worldmapEventID);
		if (nKMWorldMapEventTemplet == null)
		{
			Debug.LogError($"Worldmap eventtemplet null! ID : {eventGroupData.worldmapEventID}");
		}
		switch (nKMWorldMapEventTemplet.eventType)
		{
		default:
			return;
		case NKM_WORLDMAP_EVENT_TYPE.WET_DIVE:
		{
			NKMDiveTemplet diveInfo = NKMDiveTemplet.Find(nKMWorldMapEventTemplet.stageID);
			SetDiveInfo(diveInfo);
			break;
		}
		case NKM_WORLDMAP_EVENT_TYPE.WET_RAID:
		{
			NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(eventGroupData.eventUid);
			nKMRaidDetailData.SortJoinDataByDamage();
			string mvpName = "-";
			if (nKMRaidDetailData.raidJoinDataList.Count > 0 && nKMRaidDetailData.raidJoinDataList[0].tryCount > 0)
			{
				mvpName = nKMRaidDetailData.raidJoinDataList[0].nickName;
			}
			int myTryCount = 0;
			NKMRaidJoinData nKMRaidJoinData = nKMRaidDetailData.FindJoinData(NKCScenManager.CurrentUserData().m_UserUID);
			if (nKMRaidJoinData != null)
			{
				myTryCount = nKMRaidJoinData.tryCount;
			}
			NKMRaidSeasonTemplet nowSeasonTemplet = NKCRaidSeasonManager.GetNowSeasonTemplet();
			bool bCurrentSeason = nowSeasonTemplet != null && nowSeasonTemplet.RaidSeasonId == nKMRaidDetailData.seasonID;
			SetRaidInfo(nKMRaidDetailData.stageID, nKMRaidDetailData.curHP, nKMRaidDetailData.maxHP, nKMRaidDetailData.raidJoinDataList.Count, mvpName, myTryCount, bCurrentSeason);
			break;
		}
		}
		Sprite thumbnail = GetThumbnail(nKMWorldMapEventTemplet);
		NKCUtil.SetImageSprite(m_imgThumbnail, thumbnail, bDisableIfSpriteNull: true);
	}

	private Sprite GetThumbnail(NKMWorldMapEventTemplet eventTemplet)
	{
		if (eventTemplet == null)
		{
			return null;
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_RENEWAL_EVENT_THUMBNAIL", eventTemplet.thumbnail);
	}

	private void SetDiveInfo(NKMDiveTemplet diveTemplet)
	{
		m_NKM_WORLDMAP_EVENT_TYPE = NKM_WORLDMAP_EVENT_TYPE.WET_DIVE;
		m_eMiddleState = MiddleState.Normal;
		NKCUtil.SetGameobjectActive(m_objEventTypeRaid, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEventTypeRaidExpried, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEventTypeDive, bValue: true);
		NKCUtil.SetGameobjectActive(m_objRootEventNormal, bValue: true);
		NKCUtil.SetGameobjectActive(m_objRootEventRaid, bValue: false);
		NKCUtil.SetGameobjectActive(m_objAttendLimit, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEntryCheck, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTryCount, bValue: false);
		if (diveTemplet != null)
		{
			m_level_N = diveTemplet.StageLevel;
			NKCUtil.SetLabelText(m_lbLevel_N, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, diveTemplet.StageLevel));
			NKCUtil.SetLabelText(m_lbName_N, diveTemplet.Get_STAGE_NAME());
			NKCUtil.SetLabelText(m_lbDiveLevel, diveTemplet.Get_STAGE_NAME_SUB());
			NKCUtil.SetLabelText(m_lbDiveSlotCount, diveTemplet.SlotCount.ToString());
			NKCUtil.SetGameobjectActive(m_imgEventPointColor_N, bValue: true);
			NKCUtil.SetDiveEventPoint(m_imgEventPointColor_N, diveTemplet.GetCommonDifficultyData() == EPISODE_DIFFICULTY.HARD);
		}
		else
		{
			m_level_N = 0;
			NKCUtil.SetLabelText(m_lbLevel_N, "");
			NKCUtil.SetLabelText(m_lbName_N, "");
			NKCUtil.SetLabelText(m_lbDiveLevel, "");
			NKCUtil.SetLabelText(m_lbDiveSlotCount, "");
			NKCUtil.SetGameobjectActive(m_imgEventPointColor_N, bValue: false);
		}
		m_bNeedTimeUpdate = true;
		NKCUtil.SetLabelText(m_lbTimeLeft, NKCSynchronizedTime.GetTimeLeftString(m_dtEndTime));
		SetButtonNEventStateByCurr();
	}

	private void SetRaidInfo(int stageID, float curHP, float maxHP, int attendCount, string mvpName, int myTryCount, bool bCurrentSeason)
	{
		m_NKM_WORLDMAP_EVENT_TYPE = NKM_WORLDMAP_EVENT_TYPE.WET_RAID;
		m_eMiddleState = MiddleState.Raid;
		NKCUtil.SetGameobjectActive(m_objEventTypeRaid, bCurrentSeason);
		NKCUtil.SetGameobjectActive(m_objEventTypeRaidExpried, !bCurrentSeason);
		NKCUtil.SetGameobjectActive(m_objEventTypeDive, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRootEventNormal, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRootEventRaid, bValue: true);
		NKCUtil.SetGameobjectActive(m_objEntryCheck, myTryCount > 0);
		NKCUtil.SetGameobjectActive(m_objTryCount, bValue: true);
		NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(stageID);
		if (nKMRaidTemplet != null)
		{
			m_level_R = nKMRaidTemplet.RaidLevel;
			NKCUtil.SetLabelText(m_lbLevel_R, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, nKMRaidTemplet.RaidLevel));
			m_lbName_R.text = nKMRaidTemplet.DungeonTempletBase.GetDungeonName();
			NKCUtil.SetGameobjectActive(m_objAttendLimit, nKMRaidTemplet.AttendLimit > 0);
			if (nKMRaidTemplet.AttendLimit > 0)
			{
				NKCUtil.SetLabelText(m_lbAttendLimit, $"{attendCount}/{nKMRaidTemplet.AttendLimit}");
			}
			NKCUtil.SetGameobjectActive(m_imgEventPointColor_R, bValue: true);
			NKCUtil.SetRaidEventPoint(m_imgEventPointColor_R, nKMRaidTemplet);
			NKCUtil.SetLabelText(m_lbTryCount, string.Format(NKCUtilString.GET_STRING_RAID_REMAIN_COUNT_ONE_PARAM, nKMRaidTemplet.RaidTryCount - myTryCount));
		}
		else
		{
			m_level_R = 0;
			NKCUtil.SetGameobjectActive(m_objAttendLimit, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgEventPointColor_R, bValue: false);
			NKCUtil.SetLabelText(m_lbTryCount, "");
		}
		NKCUtil.SetLabelText(m_lbMVPName_R, (attendCount > 0) ? mvpName : "-");
		NKCUtil.SetLabelText(m_lbTargetHP_R, $"{(int)curHP}/{(int)maxHP}");
		int num = (int)(curHP / maxHP * 100f);
		NKCUtil.SetLabelText(m_lbHPLeft_R, $"{num}%");
		if (maxHP.IsNearlyZero())
		{
			m_imgTargetHP_R.fillAmount = 0f;
		}
		else
		{
			m_imgTargetHP_R.fillAmount = curHP / maxHP;
		}
		SetButtonNEventStateByCurr();
		m_bNeedTimeUpdate = curHP > 0f;
		NKCUtil.SetGameobjectActive(m_objTimeLeft, curHP > 0f);
		NKCUtil.SetLabelText(m_lbTimeLeft, NKCSynchronizedTime.GetTimeLeftString(m_dtEndTime));
	}

	private void SetRaidInfo(NKMRaidResultData cNKMRaidResultData)
	{
		m_cNKMRaidResultData = cNKMRaidResultData;
		m_cityID = cNKMRaidResultData.cityID;
		m_cNKMRaidResultData.SortJoinDataByDamage();
		string mvpName = "-";
		if (cNKMRaidResultData.raidJoinDataList.Count > 0 && cNKMRaidResultData.raidJoinDataList[0].tryCount > 0)
		{
			mvpName = cNKMRaidResultData.raidJoinDataList[0].nickName;
		}
		int myTryCount = 0;
		NKMRaidJoinData nKMRaidJoinData = cNKMRaidResultData.raidJoinDataList.Find((NKMRaidJoinData x) => x.userUID == NKCScenManager.CurrentUserData().m_UserUID);
		if (nKMRaidJoinData != null)
		{
			myTryCount = nKMRaidJoinData.tryCount;
		}
		NKMRaidSeasonTemplet nowSeasonTemplet = NKCRaidSeasonManager.GetNowSeasonTemplet();
		bool bCurrentSeason = nowSeasonTemplet != null && nowSeasonTemplet.RaidSeasonId == cNKMRaidResultData.seasonID;
		SetRaidInfo(cNKMRaidResultData.stageID, cNKMRaidResultData.curHP, cNKMRaidResultData.maxHP, cNKMRaidResultData.raidJoinDataList.Count, mvpName, myTryCount, bCurrentSeason);
	}

	private void SetRaidInfo(NKMCoopRaidData cNKMCoopRaidData)
	{
		cNKMCoopRaidData.SortJoinDataByDamage();
		string mvpName = "-";
		if (cNKMCoopRaidData.raidJoinDataList.Count > 0 && cNKMCoopRaidData.raidJoinDataList[0].tryCount > 0)
		{
			mvpName = cNKMCoopRaidData.raidJoinDataList[0].nickName;
		}
		int myTryCount = 0;
		NKMRaidJoinData nKMRaidJoinData = cNKMCoopRaidData.raidJoinDataList.Find((NKMRaidJoinData x) => x.userUID == NKCScenManager.CurrentUserData().m_UserUID);
		if (nKMRaidJoinData != null)
		{
			myTryCount = nKMRaidJoinData.tryCount;
		}
		NKMRaidSeasonTemplet nowSeasonTemplet = NKCRaidSeasonManager.GetNowSeasonTemplet();
		bool bCurrentSeason = nowSeasonTemplet != null && nowSeasonTemplet.RaidSeasonId == cNKMCoopRaidData.seasonID;
		SetRaidInfo(cNKMCoopRaidData.stageID, cNKMCoopRaidData.curHP, cNKMCoopRaidData.maxHP, cNKMCoopRaidData.raidJoinDataList.Count, mvpName, myTryCount, bCurrentSeason);
	}

	private void SetEventState(List<eEventState> listEventState)
	{
		NKCUtil.SetGameobjectActive(m_objProgress_R, bValue: false);
		NKCUtil.SetGameobjectActive(m_objFail_R, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMVP_R, m_NKM_WORLDMAP_EVENT_TYPE == NKM_WORLDMAP_EVENT_TYPE.WET_RAID);
		NKCUtil.SetGameobjectActive(m_objMyMVP, bValue: false);
		NKCUtil.SetGameobjectActive(m_objComplete_R, bValue: false);
		for (int i = 0; i < listEventState.Count; i++)
		{
			if (listEventState[i] == eEventState.Progress)
			{
				NKCUtil.SetGameobjectActive(m_objProgress_R, bValue: true);
			}
			else if (listEventState[i] == eEventState.Fail)
			{
				NKCUtil.SetGameobjectActive(m_objFail_R, bValue: true);
				NKCUtil.SetLabelText(m_lbFail_R, NKCUtilString.GET_STRING_WORLDMAP_EVENT_STATE_FAIL);
			}
			else if (listEventState[i] == eEventState.MVP)
			{
				NKCUtil.SetGameobjectActive(m_objMyMVP, bValue: true);
			}
			else if (listEventState[i] == eEventState.Complete)
			{
				NKCUtil.SetGameobjectActive(m_objComplete_R, bValue: true);
			}
			else if (listEventState[i] == eEventState.Expired)
			{
				NKCUtil.SetGameobjectActive(m_objFail_R, bValue: true);
				NKCUtil.SetLabelText(m_lbFail_R, NKCUtilString.GET_STRING_WORLDMAP_EVENT_STATE_TIME_EXPIRED);
			}
		}
	}

	private void _OnDelete()
	{
		NKCPacketSender.Send_NKMPacket_WORLDMAP_EVENT_CANCEL_REQ(m_cityID);
	}

	private void OnDeleteNewRaid()
	{
		NKC_SCEN_WORLDMAP cNKC_SCEN_WORLDMAP = NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP();
		cNKC_SCEN_WORLDMAP.NKCPopupWorldmapEventOKCancel.Open(NKCUtilString.GET_STRING_WORLDMAP_EVENT_POPUP_OK_CANCEL_NEW_RAID_DELETE_WARN, int.Parse(m_lbCityLevel.text), m_imgCityExp.fillAmount, m_lbCityName.text, NKM_WORLDMAP_EVENT_TYPE.WET_RAID, m_level_R, _OnDelete, delegate
		{
			cNKC_SCEN_WORLDMAP.NKCPopupWorldmapEventOKCancel.Close();
		});
	}

	private void OnDeleteNewDive()
	{
		NKC_SCEN_WORLDMAP cNKC_SCEN_WORLDMAP = NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP();
		cNKC_SCEN_WORLDMAP.NKCPopupWorldmapEventOKCancel.Open(NKCUtilString.GET_STRING_WORLDMAP_EVENT_POPUP_OK_CANCEL_NEW_DIVE_DELETE_WARN, int.Parse(m_lbCityLevel.text), m_imgCityExp.fillAmount, m_lbCityName.text, NKM_WORLDMAP_EVENT_TYPE.WET_DIVE, m_level_N, _OnDelete, delegate
		{
			cNKC_SCEN_WORLDMAP.NKCPopupWorldmapEventOKCancel.Close();
		});
	}

	private void OnDeleteOnGoingRaid()
	{
		NKMWorldMapCityData cityData = NKCScenManager.GetScenManager().GetMyUserData().m_WorldmapData.GetCityData(m_cNKMRaidResultData.cityID);
		if (cityData == null)
		{
			return;
		}
		NKMWorldMapCityTemplet cityTemplet = NKMWorldMapManager.GetCityTemplet(cityData.cityID);
		if (cityTemplet != null)
		{
			NKC_SCEN_WORLDMAP cNKC_SCEN_WORLDMAP = NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP();
			cNKC_SCEN_WORLDMAP.NKCPopupWorldmapEventOKCancel.Open(NKCUtilString.GET_STRING_WORLDMAP_EVENT_POPUP_OK_CANCEL_ON_GOING_RAID_DELETE_WARN, cityData.level, NKMWorldMapManager.GetCityExpPercent(cityData), cityTemplet.GetName(), NKM_WORLDMAP_EVENT_TYPE.WET_RAID, m_level_R, _OnDelete, delegate
			{
				cNKC_SCEN_WORLDMAP.NKCPopupWorldmapEventOKCancel.Close();
			});
		}
	}

	private void OnDeleteOnGoingDive()
	{
		NKC_SCEN_WORLDMAP cNKC_SCEN_WORLDMAP = NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP();
		cNKC_SCEN_WORLDMAP.NKCPopupWorldmapEventOKCancel.Open(NKCUtilString.GET_STRING_WORLDMAP_EVENT_POPUP_OK_CANCEL_ON_GOING_DIVE_DELETE_WARN, int.Parse(m_lbCityLevel.text), m_imgCityExp.fillAmount, m_lbCityName.text, NKM_WORLDMAP_EVENT_TYPE.WET_DIVE, m_level_N, _OnDelete, delegate
		{
			cNKC_SCEN_WORLDMAP.NKCPopupWorldmapEventOKCancel.Close();
		});
	}

	private void SetButtonNEventStateByCurr()
	{
		eButtonState_Top eButtonState_Top = eButtonState_Top.None;
		eButtonState_Bottom eButtonState_Bottom = eButtonState_Bottom.None;
		List<eEventState> list = new List<eEventState>();
		if (m_TabState == NKCPopupWorldMapEventList.eState.EventList)
		{
			if (NKCSynchronizedTime.IsFinished(m_dtEndTime))
			{
				eButtonState_Top = eButtonState_Top.None;
				eButtonState_Bottom = eButtonState_Bottom.OK;
				list.Add(eEventState.Expired);
			}
			else
			{
				eButtonState_Top = eButtonState_Top.Delete;
				eButtonState_Bottom = eButtonState_Bottom.Move;
			}
			if (m_NKM_WORLDMAP_EVENT_TYPE == NKM_WORLDMAP_EVENT_TYPE.WET_DIVE)
			{
				bool num = NKCScenManager.CurrentUserData().m_DiveGameData != null && NKCScenManager.CurrentUserData().m_DiveGameData.DiveUid == m_eventUID;
				m_csbtnDelete.PointerClick.RemoveAllListeners();
				if (num)
				{
					m_csbtnDelete.PointerClick.AddListener(OnDeleteOnGoingDive);
				}
				else
				{
					m_csbtnDelete.PointerClick.AddListener(OnDeleteNewDive);
				}
			}
			else
			{
				NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_eventUID);
				if (nKMRaidDetailData != null)
				{
					NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(nKMRaidDetailData.stageID);
					if (nKMRaidTemplet != null && nKMRaidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
					{
						NKMRaidJoinData nKMRaidJoinData = nKMRaidDetailData.FindJoinData(NKCScenManager.CurrentUserData().m_UserUID);
						if (nKMRaidJoinData != null && nKMRaidJoinData.tryCount >= nKMRaidTemplet.RaidTryCount && nKMRaidDetailData.curHP > 0f)
						{
							eButtonState_Top = eButtonState_Top.None;
							eButtonState_Bottom = eButtonState_Bottom.OK;
							if (list.Contains(eEventState.Expired))
							{
								list.Remove(eEventState.Expired);
							}
							list.Add(eEventState.Fail);
						}
					}
				}
				m_csbtnDelete.PointerClick.RemoveAllListeners();
				m_csbtnDelete.PointerClick.AddListener(OnDeleteNewRaid);
			}
		}
		else if (m_TabState == NKCPopupWorldMapEventList.eState.HelpList)
		{
			if (NKCSynchronizedTime.IsFinished(m_dtEndTime))
			{
				eButtonState_Top = eButtonState_Top.None;
				eButtonState_Bottom = eButtonState_Bottom.OK;
				list.Add(eEventState.Expired);
			}
			else
			{
				eButtonState_Top = eButtonState_Top.Help;
				eButtonState_Bottom = eButtonState_Bottom.Help;
			}
			if (m_NKM_WORLDMAP_EVENT_TYPE == NKM_WORLDMAP_EVENT_TYPE.WET_RAID)
			{
				eEventState item = eEventState.None;
				if (m_cNKMCoopRaidData != null)
				{
					if (m_cNKMCoopRaidData.curHP <= 0f)
					{
						item = eEventState.Complete;
					}
					else if (!m_cNKMCoopRaidData.IsOnGoing())
					{
						item = eEventState.Fail;
					}
					else
					{
						NKMRaidDetailData nKMRaidDetailData2 = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_eventUID);
						if (nKMRaidDetailData2 != null)
						{
							NKMRaidTemplet nKMRaidTemplet2 = NKMRaidTemplet.Find(nKMRaidDetailData2.stageID);
							if (nKMRaidTemplet2 != null && nKMRaidTemplet2.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
							{
								Debug.LogWarning($"솔로레이드는 지원요청을 못함 - stageID : {nKMRaidTemplet2.Key}, category : {nKMRaidTemplet2.DungeonTempletBase.m_DungeonType}");
								NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
							}
							else
							{
								item = eEventState.Progress;
							}
						}
					}
				}
				list.Add(item);
			}
		}
		else if (m_TabState == NKCPopupWorldMapEventList.eState.JoinList)
		{
			if (m_NKM_WORLDMAP_EVENT_TYPE == NKM_WORLDMAP_EVENT_TYPE.WET_DIVE)
			{
				bool flag = NKCScenManager.CurrentUserData().m_DiveGameData != null && NKCScenManager.CurrentUserData().m_DiveGameData.DiveUid == m_eventUID;
				if (NKCSynchronizedTime.IsFinished(m_dtEndTime) || (m_eventUID > 0 && !flag))
				{
					eButtonState_Top = eButtonState_Top.None;
					eButtonState_Bottom = eButtonState_Bottom.OK;
					list.Add(eEventState.Fail);
				}
				else
				{
					eButtonState_Top = eButtonState_Top.Delete;
					eButtonState_Bottom = eButtonState_Bottom.Move;
				}
				m_csbtnDelete.PointerClick.RemoveAllListeners();
				if (flag)
				{
					m_csbtnDelete.PointerClick.AddListener(OnDeleteOnGoingDive);
				}
				else
				{
					m_csbtnDelete.PointerClick.AddListener(OnDeleteNewDive);
				}
			}
			else if (m_NKM_WORLDMAP_EVENT_TYPE == NKM_WORLDMAP_EVENT_TYPE.WET_RAID)
			{
				if (NKCSynchronizedTime.IsFinished(m_dtEndTime))
				{
					eButtonState_Bottom = eButtonState_Bottom.OK;
					eButtonState_Top = eButtonState_Top.Result;
					if (m_cNKMRaidResultData != null)
					{
						if (m_cNKMRaidResultData.curHP > 0f)
						{
							list.Add(eEventState.Fail);
						}
						else
						{
							list.Add(eEventState.Complete);
						}
					}
				}
				else if (m_cNKMRaidResultData != null)
				{
					if (m_cNKMRaidResultData.raidJoinDataList.Count > 0 && m_cNKMRaidResultData.raidJoinDataList[0].userUID == NKCScenManager.CurrentUserData().m_UserUID)
					{
						list.Add(eEventState.MVP);
					}
					if (m_cNKMRaidResultData.curHP > 0f)
					{
						if (!m_cNKMRaidResultData.isCoop)
						{
							eButtonState_Top = eButtonState_Top.Delete;
							eButtonState_Bottom = eButtonState_Bottom.Move;
							m_csbtnDelete.PointerClick.RemoveAllListeners();
							m_csbtnDelete.PointerClick.AddListener(OnDeleteOnGoingRaid);
						}
						else
						{
							eButtonState_Top = eButtonState_Top.Process;
							eButtonState_Bottom = eButtonState_Bottom.Move;
						}
						list.Add(eEventState.Progress);
					}
					else
					{
						eButtonState_Top = eButtonState_Top.Result;
						eButtonState_Bottom = eButtonState_Bottom.OK;
						list.Add(eEventState.Complete);
					}
				}
			}
		}
		SetEventState(list);
		NKCUtil.SetGameobjectActive(m_objBtnOnProgress, eButtonState_Top == eButtonState_Top.Process);
		NKCUtil.SetGameobjectActive(m_objCoop, eButtonState_Top == eButtonState_Top.Help);
		NKCUtil.SetGameobjectActive(m_csbtnResult, eButtonState_Top == eButtonState_Top.Result);
		NKCUtil.SetGameobjectActive(m_csbtnDelete, eButtonState_Top == eButtonState_Top.Delete);
		NKCUtil.SetGameobjectActive(m_objBtnBlue, eButtonState_Bottom == eButtonState_Bottom.Move || eButtonState_Bottom == eButtonState_Bottom.Help);
		NKCUtil.SetGameobjectActive(m_objBtnOK, eButtonState_Bottom == eButtonState_Bottom.OK);
		switch (eButtonState_Bottom)
		{
		case eButtonState_Bottom.Move:
			NKCUtil.SetLabelText(m_lbBtnBlue, NKCUtilString.GET_STRING_WORLDMAP_GO_BUTTON);
			m_csbtnPlay.PointerClick.RemoveAllListeners();
			m_csbtnPlay.PointerClick.AddListener(OnClickBtn_Move);
			break;
		case eButtonState_Bottom.Help:
			NKCUtil.SetLabelText(m_lbBtnBlue, NKCUtilString.GET_STRING_WORLDMAP_HELP_BUTTON);
			m_csbtnPlay.PointerClick.RemoveAllListeners();
			m_csbtnPlay.PointerClick.AddListener(OnClickBtn_Help);
			break;
		case eButtonState_Bottom.OK:
			m_csbtnPlay.PointerClick.RemoveAllListeners();
			m_csbtnPlay.PointerClick.AddListener(OnClickBtn_OK);
			break;
		}
	}

	private void OnClickBtn_Move()
	{
		if (m_NKM_WORLDMAP_EVENT_TYPE == NKM_WORLDMAP_EVENT_TYPE.WET_RAID)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().SetRaidUID(m_eventUID);
			NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().SetFromEventListPopup();
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_RAID);
		}
		else
		{
			dOnMove?.Invoke(m_cityID, m_eventID, m_eventUID);
		}
	}

	private void OnClickBtn_Help()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().SetRaidUID(m_eventUID);
		NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().SetFromEventListPopup();
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_RAID);
	}

	private void OnClickBtn_Progress()
	{
		NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_WORLDMAP_PROGRESS_BUTTON_POPUP);
	}

	private void OnClickBtn_OK()
	{
		if (m_NKM_WORLDMAP_EVENT_TYPE == NKM_WORLDMAP_EVENT_TYPE.WET_DIVE)
		{
			NKCPacketSender.Send_NKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_REQ(m_cityID);
		}
		else
		{
			if (m_NKM_WORLDMAP_EVENT_TYPE != NKM_WORLDMAP_EVENT_TYPE.WET_RAID)
			{
				return;
			}
			if (m_TabState == NKCPopupWorldMapEventList.eState.EventList)
			{
				NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_eventUID);
				if (nKMRaidDetailData != null && nKMRaidDetailData.curHP > 0f)
				{
					NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().SetRaidUID(m_eventUID);
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_RAID);
				}
			}
			else
			{
				if (m_TabState == NKCPopupWorldMapEventList.eState.HelpList || m_TabState != NKCPopupWorldMapEventList.eState.JoinList || m_cNKMRaidResultData == null)
				{
					return;
				}
				NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(m_cNKMRaidResultData.stageID);
				if (nKMRaidTemplet != null)
				{
					if (m_cNKMRaidResultData.curHP <= 0f)
					{
						NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OpenTopPlayerPopup(nKMRaidTemplet, m_cNKMRaidResultData.raidJoinDataList, m_cNKMRaidResultData.raidUID);
						return;
					}
					NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().SetRaidUID(m_eventUID);
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_RAID);
				}
			}
		}
	}

	private void Update()
	{
		m_fTimer += Time.deltaTime;
		if (m_fTimer >= 1f)
		{
			m_fTimer = 0f;
			if (m_bNeedTimeUpdate)
			{
				UpdateClock();
			}
			SetButtonNEventStateByCurr();
		}
	}

	private void UpdateClock()
	{
		Text lbTimeLeft;
		switch (m_eMiddleState)
		{
		default:
			return;
		case MiddleState.Normal:
			lbTimeLeft = m_lbTimeLeft;
			break;
		case MiddleState.Raid:
			lbTimeLeft = m_lbTimeLeft;
			break;
		}
		NKCUtil.SetGameobjectActive(m_objTimeLeft, bValue: true);
		NKCUtil.SetLabelText(lbTimeLeft, NKCSynchronizedTime.GetTimeLeftString(m_dtEndTime));
	}
}
