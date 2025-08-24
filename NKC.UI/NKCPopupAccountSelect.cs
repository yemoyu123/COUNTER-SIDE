using ClientPacket.Account;
using Cs.Logging;
using NKM;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupAccountSelect : NKCUIBase
{
	private const string DEBUG_HEADER = "[SteamLink][Select]";

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_ACCOUNT_LINK";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_ACCOUNT_SELECT";

	private static NKCPopupAccountSelect m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCUIComStateButton m_ok;

	public NKCUIComStateButton m_cancel;

	public NKCUIComToggleGroup m_toggleGroup;

	public Text m_titleText;

	public Text m_DescText;

	public NKCPopupAccountSelectSlot m_steamUserProfileSlot;

	public NKCPopupAccountSelectSlot m_mobileUserProfileSlot;

	private NKMAccountLinkUserProfile m_userProfileSteam;

	private NKMAccountLinkUserProfile m_userProfileMobile;

	private NKMAccountLinkUserProfile m_selectedUserProfile;

	public static NKCPopupAccountSelect Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupAccountSelect>("AB_UI_NKM_UI_ACCOUNT_LINK", "NKM_UI_POPUP_ACCOUNT_SELECT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupAccountSelect>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "AccountLink";

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

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_steamUserProfileSlot?.InitData();
		m_mobileUserProfileSlot?.InitData();
		m_ok?.PointerClick.AddListener(OnSelectConfirm);
		m_cancel?.PointerClick.AddListener(OnClickClose);
	}

	public override void OnBackButton()
	{
		OnClickClose();
	}

	public void OnClickClose()
	{
		Log.Debug("[SteamLink][Select] OnClickClose", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCPopupAccountSelect.cs", 78);
		NKCAccountLinkMgr.CheckForCancelProcess();
	}

	public void Open(NKMAccountLinkUserProfile requestUserProfile, NKMAccountLinkUserProfile targetUserProfile)
	{
		Log.Debug("[SteamLink][Select] Open", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCPopupAccountSelect.cs", 88);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		if (requestUserProfile.publisherType == NKM_PUBLISHER_TYPE.NPT_STEAM)
		{
			m_userProfileSteam = requestUserProfile;
			m_userProfileMobile = targetUserProfile;
		}
		else
		{
			m_userProfileSteam = targetUserProfile;
			m_userProfileMobile = requestUserProfile;
		}
		m_steamUserProfileSlot?.SetData(m_userProfileSteam, OnSteamSelected);
		m_mobileUserProfileSlot?.SetData(m_userProfileMobile, OnMobileSelected);
		m_toggleGroup.SetAllToggleUnselected();
		if (NKCScenManager.GetScenManager().GetMyUserData().m_UserUID == requestUserProfile.commonProfile.userUid)
		{
			m_toggleGroup.enabled = true;
			m_titleText.text = NKCStringTable.GetString("SI_PF_STEAMLINK_CHOOSE_ACCOUNT_TITLE");
			m_DescText.text = NKCStringTable.GetString("SI_PF_STEAMLINK_CHOOSE_ACCOUNT_DESC");
			NKCUtil.SetGameobjectActive(m_ok, bValue: true);
			NKCUtil.SetGameobjectActive(m_cancel, bValue: true);
		}
		else
		{
			m_toggleGroup.enabled = false;
			m_titleText.text = NKCStringTable.GetString("SI_PF_STEAMLINK_CHOOSE_ACCOUNT_TITLE");
			m_DescText.text = NKCStringTable.GetString("SI_PF_STEAMLINK_CHOOSE_ACCOUNT_DESC_WAIT");
			NKCUtil.SetGameobjectActive(m_ok, bValue: false);
			NKCUtil.SetGameobjectActive(m_cancel, bValue: false);
		}
		UIOpened();
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public override void CloseInternal()
	{
		Log.Debug("[SteamLink][Select] CloseInternal", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCPopupAccountSelect.cs", 140);
		m_userProfileSteam = null;
		m_userProfileMobile = null;
		m_selectedUserProfile = null;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void OnSelectConfirm()
	{
		if (m_selectedUserProfile == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_STEAM_LINK_SELECT_USERPROFILE);
		}
		else
		{
			NKCPopupAccountSelectConfirm.Instance.Open(m_selectedUserProfile, OnClickSendConfirm);
		}
	}

	public void OnClickSendConfirm()
	{
		NKCPopupAccountSelectConfirm.Instance.Close();
		NKCAccountLinkMgr.Send_NKMPacket_ACCOUNT_LINK_SELECT_USERDATA_REQ(m_selectedUserProfile.commonProfile.userUid);
	}

	public void OnSteamSelected(bool bSelected)
	{
		m_selectedUserProfile = m_userProfileSteam;
	}

	public void OnMobileSelected(bool bSelected)
	{
		m_selectedUserProfile = m_userProfileMobile;
	}
}
