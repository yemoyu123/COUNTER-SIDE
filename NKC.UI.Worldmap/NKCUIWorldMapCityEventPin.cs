using System;
using ClientPacket.Raid;
using ClientPacket.WorldMap;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Worldmap;

public class NKCUIWorldMapCityEventPin : MonoBehaviour
{
	public enum BossStatus
	{
		Start,
		Idle
	}

	private enum eMark
	{
		None,
		RaidReady,
		RaidComplete,
		RaidFailed,
		DiveReady,
		DiveExpired
	}

	public enum eState
	{
		None,
		Complete,
		New,
		Help,
		OnProgress
	}

	public delegate void OnClickEvent(int cityID, int eventID, long eventUID);

	public NKCUIComStateButton m_sbtnPin;

	public RectTransform m_rtSDRoot;

	[Header("Marker")]
	public GameObject m_objRaid;

	public GameObject m_objSoloRaid;

	public Image m_imgSoloRaid;

	public GameObject m_objRaidComplete;

	public GameObject m_objRaidFailed;

	public GameObject m_objDive;

	public GameObject m_objDiveSpecial;

	public GameObject m_objDiveExpired;

	[Header("Line")]
	public GameObject m_objRaidArrow;

	public GameObject m_objDiveArrow;

	[Header("State Object")]
	public GameObject m_objComplete;

	public GameObject m_objNew;

	public GameObject m_objHelp;

	public GameObject m_objProgress;

	[Header("Etc")]
	public GameObject m_objLevel;

	public Text m_lbLevel;

	public GameObject m_objTime;

	public Text m_lbTime;

	private int m_CityID;

	private int m_EventID;

	private long m_EventUID;

	private OnClickEvent dOnClickEvent;

	private float m_fUpdateTimeForUI;

	private NKCASUISpineIllust m_spineSD;

	private Animator m_aniSoloRaid;

	public void Init(OnClickEvent onClickEvent)
	{
		dOnClickEvent = onClickEvent;
		m_sbtnPin.PointerClick.RemoveAllListeners();
		m_sbtnPin.PointerClick.AddListener(OnClick);
	}

	public Vector3 GetPinSDPos()
	{
		return m_rtSDRoot.transform.localPosition + base.transform.localPosition;
	}

	public void PlaySDAnim(NKCASUIUnitIllust.eAnimation eAnim, bool bLoop = false)
	{
		if (m_spineSD == null && m_aniSoloRaid == null)
		{
			OpenSDIllust(eAnim, bLoop);
			return;
		}
		if (m_spineSD != null)
		{
			m_spineSD.SetAnimation(eAnim, bLoop);
		}
		if (m_aniSoloRaid != null)
		{
			if (eAnim == NKCASUIUnitIllust.eAnimation.SD_START)
			{
				m_aniSoloRaid.Play("NKM_UI_WORLD_HOLOGRAM_OPEN");
			}
			else
			{
				m_aniSoloRaid.Play("NKM_UI_WORLD_HOLOGRAM_IDLE");
			}
		}
	}

	public void CleanUpSpineSD()
	{
		if (m_spineSD != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_spineSD);
			m_spineSD = null;
		}
		if (m_aniSoloRaid != null)
		{
			m_aniSoloRaid = null;
		}
	}

	public void CleanUp()
	{
		CleanUpSpineSD();
	}

	private bool OpenSDIllust(NKCASUIUnitIllust.eAnimation eStartAnim = NKCASUIUnitIllust.eAnimation.NONE, bool bLoopStartAnim = false)
	{
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_spineSD);
		NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(m_EventID);
		if (nKMWorldMapEventTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: false);
			m_spineSD = null;
			return false;
		}
		GameObject objSoloRaid = m_objSoloRaid;
		NKMRaidTemplet raidTemplet = nKMWorldMapEventTemplet.raidTemplet;
		NKCUtil.SetGameobjectActive(objSoloRaid, raidTemplet != null && raidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID);
		if (nKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_RAID && nKMWorldMapEventTemplet.raidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
		{
			if (m_aniSoloRaid == null)
			{
				NKCUtil.SetImageSprite(m_imgSoloRaid, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_UNIT", nKMWorldMapEventTemplet.spineSDName));
				m_aniSoloRaid = m_objSoloRaid.GetComponentInChildren<Animator>();
			}
			if (m_aniSoloRaid != null)
			{
				if (eStartAnim == NKCASUIUnitIllust.eAnimation.SD_START)
				{
					m_aniSoloRaid.Play("NKM_UI_WORLD_HOLOGRAM_OPEN");
				}
				else
				{
					m_aniSoloRaid.Play("NKM_UI_WORLD_HOLOGRAM_IDLE");
				}
			}
			return true;
		}
		m_spineSD = NKCResourceUtility.OpenSpineSD(nKMWorldMapEventTemplet);
		if (m_spineSD != null && (m_spineSD.m_SpineIllustInstant == null || m_spineSD.m_SpineIllustInstant_SkeletonGraphic == null))
		{
			m_spineSD = null;
		}
		if (m_spineSD != null)
		{
			if (eStartAnim == NKCASUIUnitIllust.eAnimation.NONE)
			{
				m_spineSD.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE);
			}
			else
			{
				m_spineSD.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE, bPlay: false);
				m_spineSD.SetAnimation(eStartAnim, bLoopStartAnim);
			}
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: true);
			m_spineSD.SetParent(m_rtSDRoot, worldPositionStays: false);
			RectTransform rectTransform = m_spineSD.GetRectTransform();
			if (rectTransform != null)
			{
				rectTransform.localPosition = Vector2.zero;
				if (nKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_RAID)
				{
					if (NKMRandom.Range(0, 3) == 1)
					{
						rectTransform.localScale = Vector3.one;
					}
					else
					{
						rectTransform.localScale = new Vector3(-1f, 1f, 1f);
					}
				}
				else
				{
					rectTransform.localScale = Vector3.one;
				}
				rectTransform.localRotation = Quaternion.identity;
			}
			return true;
		}
		Debug.Log("spine SD data not found from worldmapEventID : " + nKMWorldMapEventTemplet.worldmapEventID);
		NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: false);
		return false;
	}

	private void OnClick()
	{
		dOnClickEvent?.Invoke(m_CityID, m_EventID, m_EventUID);
	}

	private void ProcessSDAnim()
	{
		NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(m_EventID);
		if (nKMWorldMapEventTemplet == null || nKMWorldMapEventTemplet.eventType != NKM_WORLDMAP_EVENT_TYPE.WET_RAID || NKCUIManager.CheckScreenInputBlock())
		{
			return;
		}
		NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_EventUID);
		if (nKMRaidDetailData == null || !(nKMRaidDetailData.curHP > 0f))
		{
			return;
		}
		NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(nKMRaidDetailData.stageID);
		if (nKMRaidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
		{
			NKMRaidJoinData nKMRaidJoinData = nKMRaidDetailData.FindJoinData(NKCScenManager.CurrentUserData().m_UserUID);
			if ((nKMRaidJoinData == null || nKMRaidJoinData.tryCount < nKMRaidTemplet.RaidTryCount) && m_aniSoloRaid != null)
			{
				m_aniSoloRaid.Play("NKM_UI_WORLD_HOLOGRAM_IDLE");
			}
		}
		else if (m_spineSD != null && NKMRandom.Range(0, 35) <= 1)
		{
			PlaySDAnim(NKCASUIUnitIllust.eAnimation.SD_ATTACK);
		}
	}

	private void Update()
	{
		if (m_fUpdateTimeForUI + 1f < Time.time)
		{
			m_fUpdateTimeForUI = Time.time;
			SetMarkAndStateUI();
			SetActiveSDLV();
			SetTimeUI();
			ProcessSDAnim();
		}
	}

	private void SetTimeUI()
	{
		NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(m_EventID);
		if (nKMWorldMapEventTemplet == null)
		{
			return;
		}
		switch (nKMWorldMapEventTemplet.eventType)
		{
		case NKM_WORLDMAP_EVENT_TYPE.WET_DIVE:
		{
			NKMWorldMapCityData cityData = NKCScenManager.GetScenManager().GetMyUserData().m_WorldmapData.GetCityData(m_CityID);
			if (cityData != null)
			{
				bool flag = NKCScenManager.CurrentUserData().m_DiveGameData != null && NKCScenManager.CurrentUserData().m_DiveGameData.DiveUid == cityData.worldMapEventGroup.eventUid;
				if (NKCSynchronizedTime.IsFinished(cityData.worldMapEventGroup.eventGroupEndDate) || (cityData.worldMapEventGroup.eventUid > 0 && !flag))
				{
					NKCUtil.SetGameobjectActive(m_objTime, bValue: false);
					break;
				}
				NKCUtil.SetGameobjectActive(m_objTime, bValue: true);
				DateTime serverUTCTime3 = NKCSynchronizedTime.GetServerUTCTime();
				TimeSpan timeSpan3 = cityData.worldMapEventGroup.eventGroupEndDate - serverUTCTime3;
				m_lbTime.text = NKCUtilString.GetTimeSpanString(timeSpan3);
			}
			break;
		}
		case NKM_WORLDMAP_EVENT_TYPE.WET_RAID:
		{
			NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_EventUID);
			if (nKMRaidDetailData == null)
			{
				break;
			}
			if (nKMRaidDetailData.curHP <= 0f || NKCSynchronizedTime.IsFinished(nKMRaidDetailData.expireDate))
			{
				NKCUtil.SetGameobjectActive(m_objTime, bValue: false);
				break;
			}
			NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(nKMRaidDetailData.stageID);
			if (nKMRaidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
			{
				NKMRaidJoinData nKMRaidJoinData = nKMRaidDetailData.FindJoinData(NKCScenManager.CurrentUserData().m_UserUID);
				if (nKMRaidJoinData != null && nKMRaidJoinData.tryCount >= nKMRaidTemplet.RaidTryCount)
				{
					NKCUtil.SetGameobjectActive(m_objTime, bValue: false);
					break;
				}
				NKCUtil.SetGameobjectActive(m_objTime, bValue: true);
				DateTime dateTime = new DateTime(nKMRaidDetailData.expireDate);
				DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
				TimeSpan timeSpan = dateTime - serverUTCTime;
				NKCUtil.SetLabelText(m_lbTime, NKCUtilString.GetTimeSpanString(timeSpan));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objTime, bValue: true);
				DateTime dateTime2 = new DateTime(nKMRaidDetailData.expireDate);
				DateTime serverUTCTime2 = NKCSynchronizedTime.GetServerUTCTime();
				TimeSpan timeSpan2 = dateTime2 - serverUTCTime2;
				NKCUtil.SetLabelText(m_lbTime, NKCUtilString.GetTimeSpanString(timeSpan2));
			}
			break;
		}
		default:
			NKCUtil.SetGameobjectActive(m_objTime, bValue: false);
			break;
		}
	}

	private bool CheckDiveExpired(NKMWorldMapCityData cNKMWorldMapCityData)
	{
		if (cNKMWorldMapCityData == null)
		{
			return false;
		}
		bool flag = NKCScenManager.CurrentUserData().m_DiveGameData != null && NKCScenManager.CurrentUserData().m_DiveGameData.DiveUid == m_EventUID;
		if (NKCSynchronizedTime.IsFinished(cNKMWorldMapCityData.worldMapEventGroup.eventGroupEndDate) || (!flag && cNKMWorldMapCityData.worldMapEventGroup.eventUid > 0))
		{
			return true;
		}
		return false;
	}

	private void SetMarkAndStateUI()
	{
		NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(m_EventID);
		if (nKMWorldMapEventTemplet == null)
		{
			return;
		}
		switch (nKMWorldMapEventTemplet.eventType)
		{
		case NKM_WORLDMAP_EVENT_TYPE.WET_DIVE:
		{
			eMark mark = eMark.DiveReady;
			eState state = eState.None;
			NKMWorldMapCityData cityData = NKCScenManager.GetScenManager().GetMyUserData().m_WorldmapData.GetCityData(m_CityID);
			if (cityData != null)
			{
				bool flag = NKCScenManager.CurrentUserData().m_DiveGameData != null && NKCScenManager.CurrentUserData().m_DiveGameData.DiveUid == m_EventUID;
				if (!CheckDiveExpired(cityData))
				{
					state = ((!flag) ? eState.New : eState.OnProgress);
				}
				else
				{
					state = eState.None;
					mark = eMark.DiveExpired;
				}
			}
			SetState(state);
			SetMark(mark);
			break;
		}
		case NKM_WORLDMAP_EVENT_TYPE.WET_RAID:
		{
			NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_EventUID);
			if (nKMRaidDetailData == null)
			{
				break;
			}
			NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(nKMRaidDetailData.stageID);
			if (nKMRaidTemplet == null)
			{
				break;
			}
			if (nKMRaidDetailData.curHP <= 0f)
			{
				SetMark(eMark.RaidComplete);
			}
			else if (NKCSynchronizedTime.IsFinished(nKMRaidDetailData.expireDate))
			{
				SetMark(eMark.RaidFailed);
			}
			else if (nKMRaidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
			{
				if (nKMRaidDetailData.FindJoinData(NKCScenManager.CurrentUserData().m_UserUID)?.tryCount >= nKMRaidTemplet.RaidTryCount)
				{
					SetMark(eMark.RaidFailed);
				}
				else
				{
					SetMark(eMark.RaidReady);
				}
			}
			else
			{
				SetMark(eMark.RaidReady);
			}
			if (nKMRaidDetailData.curHP <= 0f)
			{
				SetState(eState.Complete);
			}
			else if (NKCSynchronizedTime.IsFinished(nKMRaidDetailData.expireDate))
			{
				SetState(eState.None);
			}
			else if (nKMRaidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
			{
				NKMRaidJoinData nKMRaidJoinData = nKMRaidDetailData.FindJoinData(NKCScenManager.CurrentUserData().m_UserUID);
				if (nKMRaidJoinData != null)
				{
					if (nKMRaidJoinData.tryCount == 0)
					{
						SetState(eState.New);
					}
					else if (nKMRaidJoinData.tryCount >= nKMRaidTemplet.RaidTryCount)
					{
						SetState(eState.None);
					}
					else
					{
						SetState(eState.OnProgress);
					}
				}
				else
				{
					SetState(eState.None);
				}
			}
			else if (nKMRaidDetailData.isCoop)
			{
				SetState(eState.Help);
			}
			else if (!nKMRaidDetailData.isNew)
			{
				SetState(eState.OnProgress);
			}
			else
			{
				SetState(eState.New);
			}
			break;
		}
		default:
			SetMark(eMark.None);
			SetState(eState.None);
			break;
		}
	}

	public void SetData(int cityID, NKMWorldMapEventGroup eventGroupData)
	{
		m_CityID = cityID;
		if (eventGroupData == null)
		{
			m_EventID = 0;
			m_EventUID = 0L;
			SetMark(eMark.None);
			return;
		}
		m_EventID = eventGroupData.worldmapEventID;
		m_EventUID = eventGroupData.eventUid;
		NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(eventGroupData.worldmapEventID);
		if (nKMWorldMapEventTemplet == null)
		{
			Debug.LogError($"NKMWorldMapEventTemplet Null! id : {eventGroupData.worldmapEventID}");
			SetMark(eMark.None);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objDiveArrow, nKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_DIVE);
		NKCUtil.SetGameobjectActive(m_objRaidArrow, nKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_RAID);
		switch (nKMWorldMapEventTemplet.eventType)
		{
		case NKM_WORLDMAP_EVENT_TYPE.WET_DIVE:
			SetDive(eventGroupData, nKMWorldMapEventTemplet);
			break;
		case NKM_WORLDMAP_EVENT_TYPE.WET_RAID:
			SetRaid(eventGroupData, nKMWorldMapEventTemplet);
			break;
		default:
			SetMark(eMark.None);
			break;
		}
	}

	public void SetNew(bool value)
	{
		SetState(eState.New);
	}

	public void UpdateRaidData()
	{
		SetMarkAndStateUI();
	}

	private void SetDive(NKMWorldMapEventGroup eventGroupData, NKMWorldMapEventTemplet eventTemplet)
	{
		NKCUtil.SetGameobjectActive(m_objDiveSpecial, eventTemplet.eventGrade == NKM_WORLDMAP_EVENT_GRADE.WEG_SPECIAL);
		NKMDiveTemplet nKMDiveTemplet = NKMDiveTemplet.Find(eventTemplet.stageID);
		if (nKMDiveTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_lbLevel, bValue: true);
			NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, nKMDiveTemplet.StageLevel));
		}
		else
		{
			Debug.LogError($"DiveTemplet Not Found! stageID : {eventTemplet.stageID}");
			NKCUtil.SetGameobjectActive(m_lbLevel, bValue: false);
		}
		SetMarkAndStateUI();
		SetTimeUI();
		bool flag = false;
		NKMWorldMapCityData cityData = NKCScenManager.GetScenManager().GetMyUserData().m_WorldmapData.GetCityData(m_CityID);
		if (cityData != null && CheckDiveExpired(cityData))
		{
			flag = true;
		}
		if (!flag && OpenSDIllust())
		{
			if (m_spineSD != null)
			{
				m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE, loop: true);
			}
		}
		else
		{
			CleanUp();
		}
		SetActiveSDLV();
	}

	private void SetRaid(NKMWorldMapEventGroup eventGroupData, NKMWorldMapEventTemplet eventTemplet)
	{
		NKCUtil.SetGameobjectActive(m_objDiveSpecial, bValue: false);
		NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(eventGroupData.eventUid);
		NKCUtil.SetGameobjectActive(m_lbLevel, bValue: true);
		NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, "??"));
		if (nKMRaidDetailData == null)
		{
			return;
		}
		NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(nKMRaidDetailData.stageID);
		SetMarkAndStateUI();
		SetTimeUI();
		if (nKMRaidTemplet == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, nKMRaidTemplet.RaidLevel));
		bool flag = NKCSynchronizedTime.IsFinished(nKMRaidDetailData.expireDate);
		bool flag2 = nKMRaidDetailData.curHP <= 0f;
		NKCUtil.SetGameobjectActive(m_objSoloRaid, !flag || flag2);
		NKCUtil.SetGameobjectActive(m_rtSDRoot, !flag || flag2);
		NKCUtil.SetGameobjectActive(m_objLevel, !flag || flag2);
		if (!flag)
		{
			NKCUtil.SetGameobjectActive(m_objLevel, bValue: true);
			if (!flag2)
			{
				if (nKMRaidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
				{
					if (OpenSDIllust() && m_aniSoloRaid != null)
					{
						if (nKMRaidDetailData.FindJoinData(NKCScenManager.CurrentUserData().m_UserUID)?.tryCount >= nKMRaidTemplet.RaidTryCount)
						{
							m_aniSoloRaid.Play("NKM_UI_WORLD_HOLOGRAM_COMPLETE");
						}
						else
						{
							m_aniSoloRaid.Play("NKM_UI_WORLD_HOLOGRAM_IDLE");
						}
					}
					else
					{
						CleanUp();
					}
				}
				else if (OpenSDIllust() && m_spineSD != null)
				{
					m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE, loop: true);
				}
				else
				{
					CleanUp();
				}
			}
			else
			{
				ProcessWinRaid(nKMRaidDetailData, nKMRaidTemplet);
			}
		}
		else if (flag2)
		{
			ProcessWinRaid(nKMRaidDetailData, nKMRaidTemplet);
		}
		else
		{
			CleanUp();
		}
	}

	private void ProcessWinRaid(NKMRaidDetailData cNKMRaidDetailData, NKMRaidTemplet cNKMRaidTemplet)
	{
		if (cNKMRaidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
		{
			if (OpenSDIllust() && m_aniSoloRaid != null)
			{
				m_aniSoloRaid.Play("NKM_UI_WORLD_HOLOGRAM_COMPLETE");
			}
			else
			{
				CleanUp();
			}
		}
		else if (OpenSDIllust() && m_spineSD != null)
		{
			m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_DOWN, loop: true);
		}
		else
		{
			CleanUp();
		}
	}

	private void SetActiveSDLV()
	{
		NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(m_EventID);
		if (nKMWorldMapEventTemplet == null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		if (nKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_RAID)
		{
			NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_EventUID);
			if (nKMRaidDetailData == null)
			{
				return;
			}
			if (nKMRaidDetailData.curHP <= 0f)
			{
				flag2 = true;
			}
			NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(nKMRaidDetailData.stageID);
			if (nKMRaidTemplet == null)
			{
				return;
			}
			if (nKMRaidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
			{
				flag = NKCSynchronizedTime.IsFinished(nKMRaidDetailData.expireDate);
				if (!flag)
				{
					NKMRaidJoinData nKMRaidJoinData = nKMRaidDetailData.FindJoinData(NKCScenManager.CurrentUserData().m_UserUID);
					if (nKMRaidJoinData != null)
					{
						flag = nKMRaidJoinData.tryCount >= nKMRaidTemplet.RaidTryCount && nKMRaidDetailData.curHP > 0f;
					}
				}
			}
			else
			{
				flag = NKCSynchronizedTime.IsFinished(nKMRaidDetailData.expireDate);
			}
		}
		else
		{
			if (nKMWorldMapEventTemplet.eventType != NKM_WORLDMAP_EVENT_TYPE.WET_DIVE)
			{
				return;
			}
			NKMWorldMapCityData cityData = NKCScenManager.GetScenManager().GetMyUserData().m_WorldmapData.GetCityData(m_CityID);
			if (cityData == null)
			{
				return;
			}
			if (NKCScenManager.CurrentUserData().m_DiveGameData != null)
			{
				_ = NKCScenManager.CurrentUserData().m_DiveGameData.DiveUid == m_EventUID;
			}
			else
				_ = 0;
			flag = CheckDiveExpired(cityData);
		}
		NKCUtil.SetGameobjectActive(m_objSoloRaid, (!flag || flag2) && nKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_RAID && nKMWorldMapEventTemplet.raidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID);
		NKCUtil.SetGameobjectActive(m_rtSDRoot, !flag || flag2);
		NKCUtil.SetGameobjectActive(m_objLevel, !flag || flag2);
	}

	private void SetMark(eMark mark)
	{
		NKCUtil.SetGameobjectActive(m_objRaid, mark == eMark.RaidReady || mark == eMark.RaidComplete);
		NKCUtil.SetGameobjectActive(m_objRaidComplete, mark == eMark.RaidComplete);
		NKCUtil.SetGameobjectActive(m_objRaidFailed, mark == eMark.RaidFailed);
		NKCUtil.SetGameobjectActive(m_objDive, mark == eMark.DiveReady);
		NKCUtil.SetGameobjectActive(m_objDiveExpired, mark == eMark.DiveExpired);
	}

	private void SetState(eState state)
	{
		NKCUtil.SetGameobjectActive(m_objNew, state == eState.New);
		NKCUtil.SetGameobjectActive(m_objComplete, state == eState.Complete);
		NKCUtil.SetGameobjectActive(m_objHelp, state == eState.Help);
		NKCUtil.SetGameobjectActive(m_objProgress, state == eState.OnProgress);
	}
}
