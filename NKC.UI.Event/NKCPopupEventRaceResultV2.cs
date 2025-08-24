using ClientPacket.Event;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCPopupEventRaceResultV2 : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "UI_SINGLE_RACE";

	private const string UI_ASSET_NAME = "UI_SINGLE_RACE_PLAY_RESULT";

	private static NKCPopupEventRaceResultV2 m_Instance;

	public NKCUIComStateButton m_btnClose;

	public Animator m_AniResult;

	public GameObject m_objWin;

	public GameObject m_objLose;

	public Transform m_trSpineRoot;

	private NKCASUIUnitIllust m_UnitIllust;

	public Image m_imgRewardIcon;

	public NKCComTMPUIText m_lbDate;

	public NKCComTMPUIText m_lbRewardInfo;

	public GameObject m_objWinnerMinimumGuarantee;

	[Header("Setting")]
	public string m_TameAUnitImageType = "SKIN";

	public int m_TeamAUnit = 117816;

	public string m_TameBUnitImageType = "SKIN";

	public int m_TeamBUnit = 116616;

	public static NKCPopupEventRaceResultV2 Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventRaceResultV2>("UI_SINGLE_RACE", "UI_SINGLE_RACE_PLAY_RESULT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEventRaceResultV2>();
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

	public void Open(int iRaceIndex, NKMRewardData rewardData, bool bUserWin, EventBetTeam selectTeam, bool bMinimumGuarantee)
	{
		foreach (NKMItemMiscData miscItemData in rewardData.MiscItemDataList)
		{
			if (miscItemData != null)
			{
				Sprite orLoadMiscItemIcon = NKCResourceUtility.GetOrLoadMiscItemIcon(miscItemData.ItemID);
				if (!(null == orLoadMiscItemIcon))
				{
					NKCUtil.SetImageSprite(m_imgRewardIcon, orLoadMiscItemIcon);
					break;
				}
			}
		}
		NKCUtil.SetLabelText(m_lbDate, string.Format(NKCUtilString.GET_STRING_EVENT_RACE_BET_HISTORY_DAY_01, iRaceIndex + 1));
		string text = ((selectTeam == EventBetTeam.TeamA) ? NKCUtilString.GET_STRING_EVENT_RACE_BET_TEAM_RED : NKCUtilString.GET_STRING_EVENT_RACE_BET_TEAM_BLUE);
		NKMEventBetPrivateResult betPrivateResult = NKCScenManager.CurrentUserData().GetRaceData().GetBetPrivateResult(iRaceIndex);
		if (betPrivateResult != null)
		{
			string msg = string.Format(NKCUtilString.GET_STRING_EVENT_RACE_BET_HISTORY_REWARD_DESC_04, text, betPrivateResult.betCount, text, betPrivateResult.dividentRate, betPrivateResult.rewardCount);
			NKCUtil.SetLabelText(m_lbRewardInfo, msg);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbRewardInfo, "");
		}
		NKCUtil.SetGameobjectActive(m_objWin, bUserWin);
		NKCUtil.SetGameobjectActive(m_objLose, !bUserWin);
		NKCUtil.SetGameobjectActive(m_objWinnerMinimumGuarantee, bMinimumGuarantee);
		if (m_UnitIllust != null)
		{
			m_UnitIllust.Unload();
			m_UnitIllust = null;
		}
		if (selectTeam == EventBetTeam.TeamA)
		{
			string unitIllustName = GetUnitIllustName(m_TameAUnitImageType, m_TeamAUnit);
			m_UnitIllust = NKCResourceUtility.OpenSpineIllust(unitIllustName, unitIllustName);
		}
		else
		{
			string unitIllustName2 = GetUnitIllustName(m_TameBUnitImageType, m_TeamBUnit);
			m_UnitIllust = NKCResourceUtility.OpenSpineIllust(unitIllustName2, unitIllustName2);
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
			m_AniResult.SetTrigger("WIN");
		}
		else
		{
			m_AniResult.SetTrigger("LOSE");
		}
		UIOpened();
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
