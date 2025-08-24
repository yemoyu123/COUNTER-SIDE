using System;
using System.Collections;
using ClientPacket.Event;
using NKC.UI.Guide;
using NKM;
using NKM.Event;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventSubUIRace : NKCUIEventSubUIBase
{
	public Text m_lbDaysElapsed;

	public Text m_lbEventDesc;

	[Header("팀 점수")]
	public Text m_lbTeamRedScore;

	public Text m_lbTeamBlueScore;

	[Header("승리 표시 오브젝트")]
	public GameObject m_objTeamRedWin;

	public GameObject m_objTeamRedSymbol;

	public GameObject m_objTeamBlueWin;

	public GameObject m_objTeamBlueSymbol;

	[Header("팀 선택 표시 오브젝트")]
	public GameObject m_objRedTeamSelect;

	public GameObject m_objBlueTeamSelect;

	[Header("캐릭터")]
	public NKCUICharacterView m_redTeamCharacter;

	public NKCUICharacterView m_blueTeamCharacter;

	[Header("버튼")]
	public NKCUIComStateButton m_csbtnHelp;

	public NKCUIComStateButton m_csbtnRewardInfo;

	public NKCUIComStateButton m_csbtnMission;

	public NKCUIComStateButton m_csbtnStartRace;

	[Header("시작 버튼 표시")]
	public Image m_imgTicket;

	public Text m_lbRaceRemainCount;

	public Text m_lbRaceTicketCount;

	public Color m_enableText;

	public Color m_disableText;

	[Header("레드 닷")]
	public GameObject m_objMissionRedDot;

	[Header("GUIDE ID(숫자)")]
	public int m_guideId;

	private int MaxPlayCount;

	private bool m_bBeginnerOpen;

	private const string ALREADY_OPENED_KEY = "EVENT_RACE_ALREADY_OPENED";

	private static NKMRaceSummary m_raceSummary;

	private static int m_raceDay;

	private static int m_eventId;

	public static int RaceDay
	{
		set
		{
			m_raceDay = value;
		}
	}

	public static NKMRaceSummary RaceSummary
	{
		get
		{
			return m_raceSummary;
		}
		set
		{
			m_raceSummary = value;
		}
	}

	public override void Init()
	{
		base.Init();
		NKCUtil.SetButtonClickDelegate(m_csbtnHelp, OnClickHelp);
		NKCUtil.SetButtonClickDelegate(m_csbtnMission, OnClickMission);
		NKCUtil.SetButtonClickDelegate(m_csbtnRewardInfo, OnClickRewardInfo);
		NKCUtil.SetButtonClickDelegate(m_csbtnStartRace, OnClickStartRace);
		m_redTeamCharacter.Init();
		m_blueTeamCharacter.Init();
		NKCUtil.SetHotkey(m_csbtnHelp, HotkeyEventType.Help);
		NKCUtil.SetHotkey(m_csbtnStartRace, HotkeyEventType.Confirm);
		MaxPlayCount = NKMCommonConst.EVENT_RACE_PLAY_COUNT;
		int num = 0;
		foreach (NKMEventRaceTemplet value in NKMTempletContainer<NKMEventRaceTemplet>.Values)
		{
			_ = value;
			num++;
		}
		if (num <= 0)
		{
			NKMTempletContainer<NKMEventRaceTemplet>.Load("AB_SCRIPT", "LUA_EVENT_RACE_TEMPLET", "EVENT_RACE_TEMPLET", NKMEventRaceTemplet.LoadFromLua);
		}
	}

	public override void Open(NKMEventTabTemplet tabTemplet)
	{
		if (tabTemplet == null)
		{
			return;
		}
		m_tabTemplet = tabTemplet;
		m_eventId = tabTemplet.m_EventID;
		NKCUtil.SetLabelText(m_lbEventDesc, NKCStringTable.GetString(m_tabTemplet.m_EventHelpDesc));
		SetDateLimit();
		Refresh();
		bool flag = m_raceSummary == null || m_raceSummary.racePrivate == null;
		int num = PlayerPrefs.GetInt("EVENT_RACE_ALREADY_OPENED");
		if (num == 0 && flag)
		{
			m_bBeginnerOpen = true;
			StartCoroutine(OpenHelp());
			return;
		}
		if (num == 0)
		{
			PlayerPrefs.SetInt("EVENT_RACE_ALREADY_OPENED", 1);
		}
		m_bBeginnerOpen = false;
	}

	public override void Refresh()
	{
		long num = 0L;
		long num2 = 0L;
		bool bValue = false;
		bool bValue2 = false;
		if (m_raceSummary != null)
		{
			if (m_raceSummary.raceResult != null)
			{
				num = m_raceSummary.raceResult.TeamAPoint;
				num2 = m_raceSummary.raceResult.TeamBPoint;
			}
			if (m_raceSummary.racePrivate != null)
			{
				switch (m_raceSummary.racePrivate.SelectTeam)
				{
				case RaceTeam.TeamA:
					bValue = true;
					break;
				case RaceTeam.TeamB:
					bValue2 = true;
					break;
				}
			}
		}
		NKCUtil.SetLabelText(m_lbDaysElapsed, string.Format(NKCStringTable.GetString("SI_PF_EVENT_RACE_COMMON_UI_REWARD_TEXT"), m_raceDay + 1));
		NKCUtil.SetLabelText(m_lbTeamRedScore, num.ToString());
		NKCUtil.SetLabelText(m_lbTeamBlueScore, num2.ToString());
		NKCUtil.SetGameobjectActive(m_objRedTeamSelect, bValue);
		NKCUtil.SetGameobjectActive(m_objBlueTeamSelect, bValue2);
		NKCUtil.SetGameobjectActive(m_objTeamRedWin, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTeamRedSymbol, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTeamBlueWin, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTeamBlueSymbol, bValue: false);
		if (num > 0 || num2 > 0)
		{
			bool bValue3 = num >= num2;
			bool bValue4 = num <= num2;
			NKCUtil.SetGameobjectActive(m_objTeamRedWin, bValue3);
			NKCUtil.SetGameobjectActive(m_objTeamRedSymbol, bValue3);
			NKCUtil.SetGameobjectActive(m_objTeamBlueWin, bValue4);
			NKCUtil.SetGameobjectActive(m_objTeamBlueSymbol, bValue4);
		}
		if (m_tabTemplet != null)
		{
			SetCharacter(m_tabTemplet.m_EventID);
			SetRaceStartButtonInfo(m_tabTemplet.m_EventID);
			NKCUtil.SetGameobjectActive(m_objMissionRedDot, NKCPopupEventRaceMission.Instance.GetMissionRedDotState(m_tabTemplet.m_EventID));
		}
	}

	public static void OpenRace()
	{
		if (NKMEventRaceTemplet.Find(m_eventId) != null && m_raceSummary != null)
		{
			_ = m_raceSummary.racePrivate;
		}
	}

	public override void Close()
	{
		base.Close();
		m_redTeamCharacter.CleanUp();
		m_blueTeamCharacter.CleanUp();
	}

	private IEnumerator OpenHelp()
	{
		float delayTime = 1f;
		while (delayTime > 0f)
		{
			delayTime -= Time.deltaTime;
			yield return null;
		}
		PlayerPrefs.SetInt("EVENT_RACE_ALREADY_OPENED", 1);
		m_bBeginnerOpen = false;
		OnClickHelp();
	}

	private void SetElapsedDays(DateTime startUtc)
	{
		int num = 0;
		DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
		if (serverUTCTime > startUtc)
		{
			num = (serverUTCTime - startUtc).Days + 1;
		}
		NKCUtil.SetLabelText(m_lbDaysElapsed, string.Format(NKCStringTable.GetString("SI_PF_EVENT_RACE_COMMON_UI_REWARD_TEXT"), num));
	}

	private void SetRaceStartButtonInfo(int eventId)
	{
		NKMEventRaceTemplet nKMEventRaceTemplet = NKMEventRaceTemplet.Find(eventId);
		if (nKMEventRaceTemplet != null)
		{
			Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(NKMItemManager.GetItemMiscTempletByID(nKMEventRaceTemplet.RaceItemId));
			NKCUtil.SetImageSprite(m_imgTicket, orLoadMiscItemSmallIcon);
			int num = MaxPlayCount;
			if (m_raceSummary != null && m_raceSummary.racePrivate != null)
			{
				num = MaxPlayCount - m_raceSummary.racePrivate.racePlayCount;
			}
			long num2 = 0L;
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null && myUserData.m_InventoryData != null)
			{
				num2 = myUserData.m_InventoryData.GetCountMiscItem(nKMEventRaceTemplet.RaceItemId);
			}
			string arg = "#" + ColorUtility.ToHtmlStringRGB((num > 0) ? m_enableText : m_disableText);
			NKCUtil.SetLabelText(m_lbRaceRemainCount, string.Format(NKCStringTable.GetString("SI_PF_EVENT_RACE_COMMON_UI_START_COUNT_TEXT"), arg, num, MaxPlayCount));
			string arg2 = "#" + ColorUtility.ToHtmlStringRGB((num2 > 0) ? m_enableText : m_disableText);
			NKCUtil.SetLabelText(m_lbRaceTicketCount, $"<color={arg2}>{num2}</color>");
			m_csbtnStartRace.SetLock(num <= 0 || num2 <= 0);
		}
	}

	private void SetCharacter(int eventId)
	{
		NKMEventRaceTemplet.Find(eventId);
	}

	private void SetCharacterIllust(NKCUICharacterView characterView, string type, int Id)
	{
		if (type != null && type == "SKIN")
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(Id);
			characterView.SetCharacterIllust(skinTemplet, bAsync: false, bEnableBackground: false);
		}
		else
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(Id);
			characterView.SetCharacterIllust(unitTempletBase, 0, bAsync: false, bEnableBackground: false);
		}
	}

	private static string GetSDUnitName(string type, int id)
	{
		if (type != null && type == "SKIN")
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(id);
			if (skinTemplet == null)
			{
				return "";
			}
			return skinTemplet.m_SpineSDName;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(id);
		if (unitTempletBase == null)
		{
			return "";
		}
		return unitTempletBase.m_SpineSDName;
	}

	private void OnClickHelp()
	{
		if (!m_bBeginnerOpen)
		{
			NKCUIPopupTutorialImagePanel.Instance.Open(m_guideId, null);
		}
	}

	private void OnClickMission()
	{
		if (!m_bBeginnerOpen && m_tabTemplet != null)
		{
			NKCPopupEventRaceMission.Instance.Open(m_tabTemplet.m_EventID);
		}
	}

	private void OnClickRewardInfo()
	{
		if (!m_bBeginnerOpen && m_tabTemplet != null)
		{
			NKCPopupEventRaceReward.Instance.Open(m_tabTemplet.m_EventID);
		}
	}

	private void OnClickStartRace()
	{
		if (m_bBeginnerOpen)
		{
			return;
		}
		if (m_raceSummary == null || m_raceSummary.racePrivate == null)
		{
			if (m_tabTemplet != null)
			{
				NKCPopupEventRaceTeamSelect.Instance.Open(m_tabTemplet.m_EventID);
			}
		}
		else
		{
			OpenRace();
		}
	}

	private void OnDestroy()
	{
		m_tabTemplet = null;
		if (m_redTeamCharacter != null)
		{
			m_redTeamCharacter.CleanUp();
			m_redTeamCharacter = null;
		}
		if (m_blueTeamCharacter != null)
		{
			m_blueTeamCharacter.CleanUp();
			m_blueTeamCharacter = null;
		}
	}
}
