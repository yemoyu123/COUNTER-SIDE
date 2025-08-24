using System.Collections.Generic;
using ClientPacket.Event;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCPopupEventRaceResult : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_EVENT_PF_RACE";

	private const string UI_ASSET_NAME = "NKM_UI_EVENT_RACE_RESULT";

	private static NKCPopupEventRaceResult m_Instance;

	public NKCUIComStateButton m_btnClose;

	public Animator m_AniResult;

	public GameObject m_objWin;

	public GameObject m_objLose;

	public Transform m_trSpineRoot;

	public Text m_lbTeamName;

	public Image m_imgPoint;

	public Text m_lbEarnPoint;

	private const string POINT_ICON_BUNDLE_NAME = "AB_UI_NKM_UI_EVENT_RACE_SPRITE";

	private const string POINT_ICON_RED = "NKM_UI_EVENT_RACE_ICON_TEAM_RED";

	private const string POINT_ICON_BLUE = "NKM_UI_EVENT_RACE_ICON_TEAM_BLUE";

	public NKCUISlot m_slot;

	private NKCASUIUnitIllust m_UnitIllust;

	public static NKCPopupEventRaceResult Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventRaceResult>("AB_UI_NKM_UI_EVENT_PF_RACE", "NKM_UI_EVENT_RACE_RESULT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEventRaceResult>();
				m_Instance.InitUI();
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

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (NKCPopupEventRace.IsInstanceOpen)
		{
			NKCPopupEventRace.Instance.Close();
		}
	}

	public void Open(NKMRewardData rewardData, bool bUserWin, EventBetTeam selectTeam)
	{
		List<NKCUISlot.SlotData> list = NKCUISlot.MakeSlotDataListFromReward(rewardData);
		for (int i = 0; i < list.Count; i++)
		{
			m_slot.SetData(list[i]);
		}
		NKCUtil.SetGameobjectActive(m_objWin, bUserWin);
		NKCUtil.SetGameobjectActive(m_objLose, !bUserWin);
		if (selectTeam == EventBetTeam.TeamA)
		{
			NKCUtil.SetLabelText(m_lbTeamName, NKCUtilString.GET_STRING_EVENT_RACE_RESULT_TEAM_RED);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbTeamName, NKCUtilString.GET_STRING_EVENT_RACE_RESULT_TEAM_BLUE);
		}
		NKCUtil.SetImageSprite(m_imgPoint, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EVENT_RACE_SPRITE", GetIconName(selectTeam)));
		NKCUtil.SetLabelText(m_lbEarnPoint, $"+{0}");
		if (m_UnitIllust != null)
		{
			m_UnitIllust.Unload();
			m_UnitIllust = null;
		}
		m_UnitIllust.SetIllustBackgroundEnable(bValue: false);
		m_UnitIllust.SetParent(m_trSpineRoot, worldPositionStays: false);
		m_UnitIllust.GetRectTransform().localPosition = Vector3.zero;
		m_UnitIllust.GetRectTransform().localScale = Vector3.one;
		if (bUserWin)
		{
			m_UnitIllust.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.UNIT_LAUGH);
		}
		else
		{
			m_UnitIllust.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.UNIT_DESPAIR);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (bUserWin)
		{
			m_AniResult.Play("NKM_UI_EVENT_RACE_RESULT_WIN");
		}
		else
		{
			m_AniResult.Play("NKM_UI_EVENT_RACE_RESULT_LOSE");
		}
		UIOpened();
	}

	public void Open(NKMPACKET_RACE_START_ACK sPacket)
	{
		List<NKCUISlot.SlotData> list = NKCUISlot.MakeSlotDataListFromReward(sPacket.rewardData);
		for (int i = 0; i < list.Count; i++)
		{
			m_slot.SetData(list[i]);
		}
		NKCUtil.SetGameobjectActive(m_objWin, sPacket.isWin);
		NKCUtil.SetGameobjectActive(m_objLose, !sPacket.isWin);
		if (sPacket.racePrivate.SelectTeam == RaceTeam.TeamA)
		{
			NKCUtil.SetLabelText(m_lbTeamName, NKCUtilString.GET_STRING_EVENT_RACE_RESULT_TEAM_RED);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbTeamName, NKCUtilString.GET_STRING_EVENT_RACE_RESULT_TEAM_BLUE);
		}
		if (NKMEventRaceTemplet.Find(sPacket.racePrivate.RaceId) != null)
		{
			NKCUtil.SetImageSprite(m_imgPoint, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EVENT_RACE_SPRITE", GetIconName(sPacket.racePrivate.SelectTeam)));
			NKCUtil.SetLabelText(m_lbEarnPoint, $"+{0}");
		}
		if (m_UnitIllust != null)
		{
			m_UnitIllust.Unload();
			m_UnitIllust = null;
		}
		m_UnitIllust.SetIllustBackgroundEnable(bValue: false);
		m_UnitIllust.SetParent(m_trSpineRoot, worldPositionStays: false);
		m_UnitIllust.GetRectTransform().localPosition = Vector3.zero;
		m_UnitIllust.GetRectTransform().localScale = Vector3.one;
		if (sPacket.isWin)
		{
			m_UnitIllust.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.UNIT_LAUGH);
		}
		else
		{
			m_UnitIllust.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.UNIT_DESPAIR);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (sPacket.isWin)
		{
			m_AniResult.Play("NKM_UI_EVENT_RACE_RESULT_WIN");
		}
		else
		{
			m_AniResult.Play("NKM_UI_EVENT_RACE_RESULT_LOSE");
		}
		UIOpened();
	}

	private string GetIconName(RaceTeam team)
	{
		if (team != RaceTeam.TeamA)
		{
			return "NKM_UI_EVENT_RACE_ICON_TEAM_BLUE";
		}
		return "NKM_UI_EVENT_RACE_ICON_TEAM_RED";
	}

	private string GetIconName(EventBetTeam team)
	{
		if (team != EventBetTeam.TeamA)
		{
			return "NKM_UI_EVENT_RACE_ICON_TEAM_BLUE";
		}
		return "NKM_UI_EVENT_RACE_ICON_TEAM_RED";
	}

	private string GetUnitIllustName(string type, int id)
	{
		if (type != null && type == "SKIN")
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(id);
			if (skinTemplet == null)
			{
				return "";
			}
			return skinTemplet.m_SpineIllustName;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(id);
		if (unitTempletBase == null)
		{
			return "";
		}
		return unitTempletBase.m_SpineIllustName;
	}
}
