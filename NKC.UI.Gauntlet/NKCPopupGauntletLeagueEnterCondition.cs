using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCPopupGauntletLeagueEnterCondition : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	public const string UI_ASSET_NAME = "NKM_UI_ENTER_CONDITION";

	private static NKCPopupGauntletLeagueEnterCondition m_Instance;

	public Text m_lbUnitCount;

	public Text m_lbShipCount;

	public NKCUIComStateButton m_btnClose;

	public static NKCPopupGauntletLeagueEnterCondition Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGauntletLeagueEnterCondition>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_ENTER_CONDITION", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupGauntletLeagueEnterCondition>();
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

	public override string MenuName => "";

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

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Init()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
	}

	public void Open()
	{
		NKCUtil.SetLabelText(m_lbUnitCount, string.Format(NKCUtilString.GET_STRING_GAUNTLET_LEAGUE_START_REQ_UNIT_POPUP_DESC, NKMPvpCommonConst.Instance.DraftBan.MinUnitCount));
		NKCUtil.SetLabelText(m_lbShipCount, string.Format(NKCUtilString.GET_STRING_GAUNTLET_LEAGUE_START_REQ_SHIP_POPUP_DESC, NKMPvpCommonConst.Instance.DraftBan.MinShipCount));
		if (NKCScenManager.CurrentUserData().m_ArmyData.GetUnitTypeCount() < NKMPvpCommonConst.Instance.DraftBan.MinUnitCount)
		{
			NKCUtil.SetLabelTextColor(m_lbUnitCount, NKCUtil.GetColor("#FF2626"));
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_lbUnitCount, Color.white);
		}
		if (NKCScenManager.CurrentUserData().m_ArmyData.GetShipTypeCount() < NKMPvpCommonConst.Instance.DraftBan.MinShipCount)
		{
			NKCUtil.SetLabelTextColor(m_lbShipCount, NKCUtil.GetColor("#FF2626"));
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_lbShipCount, Color.white);
		}
		UIOpened();
	}
}
