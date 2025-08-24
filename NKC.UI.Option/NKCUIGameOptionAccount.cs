using Cs.Logging;
using NKC.PacketHandler;
using NKC.Publisher;
using NKC.UI.Guide;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Option;

public class NKCUIGameOptionAccount : NKCUIGameOptionContentBase
{
	private readonly Color cDisonnectedColor = Color.red;

	public Text m_NKM_UI_GAME_OPTION_ACCOUNT_ID_INFO;

	public Text m_NKM_UI_GAME_OPTION_ACCOUNT_NICKNAME_INFO;

	public Text m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_INFO;

	public Text m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED;

	public GameObject m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_COPY;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_OFFICIAL_ANNOUNCEMENT;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_CUSTOMER_SERVICE_CENTER;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_PURCHASE_RECOVERY;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_LOGOUT;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_GUEST_ACCOUNT_TRANSFER;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_ACCOUNT_RESET;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_ACCOUNT_CONNECTION;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_SERVER_INITIALIZATION;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_GUIDE;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_COMMUNITY;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_NOTICE;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_QnA;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_GAME_GRADE_CHECK;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_RESOURCE_DIVISION;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_ACCOUNT_LINK;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_INPUT;

	public Text m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_TEXT;

	public Text m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_TEXT_LOCK;

	public Text m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_INPUT_TEXT;

	public Text m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_INPUT_TEXT_LOCK;

	public GameObject m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_REWARD;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_IMPORTANT_NOTICE;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_IMPORTANT_NOTICE_REFUSE;

	public Text m_NKM_UI_GAME_OPTION_IMPORTANT_NOTICE_TEXT;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_SELECT_SERVER;

	public GameObject m_objNKM_UI_GAME_OPTION_GUEST_ACCOUNT_TRANSFER;

	public GameObject m_objNKM_UI_GAME_OPTION_GUEST_ACCOUNT_TRANSFER_dummy;

	public GameObject m_objNKM_UI_GAME_OPTION_LOGOUT;

	public GameObject m_objNKM_UI_GAME_OPTION_LOGOUT_dummy;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_ADMOB_SHOW_PRIVACY_OPTION;

	public GameObject m_objOptionAllowPush;

	public NKCUIComToggle m_ctglOptionAllowPush;

	[Header("쿠폰 입력 버튼")]
	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_ACCOUNT_COUPON;

	public GameObject m_objNKM_UI_GAME_OPTION_ACCOUNT_COUPON;

	[Header("계정 연동 버튼")]
	public GameObject m_FACEBOOK;

	public GameObject m_GOOGLE;

	public GameObject m_NEXON;

	public GameObject m_APPLE;

	public GameObject m_NAVER;

	public GameObject m_TWITTER;

	public GameObject m_LINE;

	public GameObject m_STEAM;

	private string CONNECTED_STRING => NKCUtilString.GET_STRING_OPTION_CONNECTED;

	private string DISCONNECTED_STRING => NKCUtilString.GET_STRING_OPTION_DISCONNECTED;

	private string LOGOUT_WARNING_TITLE_STRING => NKCUtilString.GET_STRING_WARNING;

	private string LOGOUT_USABLE_CONTENT_STRING => NKCUtilString.GET_STRING_OPTION_LOGOUT_REQ;

	private string LOGOUT_UNUSABLE_CONTENT_STRING => NKCUtilString.GET_STRING_OPTION_CANNOT_LOG_OUT_WHEN_IN_GAME_BATTLE;

	public override void Init()
	{
		NKCUtil.SetGameobjectActive(m_FACEBOOK, bValue: false);
		NKCUtil.SetGameobjectActive(m_GOOGLE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NEXON, bValue: false);
		NKCUtil.SetGameobjectActive(m_APPLE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NAVER, bValue: false);
		NKCUtil.SetGameobjectActive(m_TWITTER, bValue: false);
		NKCUtil.SetGameobjectActive(m_LINE, bValue: false);
		NKCUtil.SetGameobjectActive(m_STEAM, bValue: false);
		NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_GAME_GRADE_CHECK, OnClickGameGradeCheck);
		NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_COPY, OnClickAccountCodeCopy);
		NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_OFFICIAL_ANNOUNCEMENT, OnClickAnnouncement);
		NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_CUSTOMER_SERVICE_CENTER, OnClickServiceCenter);
		NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_PURCHASE_RECOVERY, OnClickBillingRestore);
		NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_LOGOUT, OnClickLogoutButton);
		NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_ACCOUNT_CONNECTION, OnClickSyncAccount);
		NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_GUIDE, OnClickGuide);
		NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_COMMUNITY, OnClickCommunity);
		if (NKCDefineManager.DEFINE_NXTOY())
		{
			NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL, OnClickWithdrawalNexon);
			NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_ACCOUNT_RESET, OnClickWithdrawal);
		}
		else
		{
			if (!NKCDefineManager.DEFINE_SELECT_SERVER())
			{
				NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL, OnClickWithdrawal);
			}
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_RESET, bValue: false);
		}
		if (NKCDefineManager.DEFINE_SELECT_SERVER())
		{
			NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_SERVER_INITIALIZATION, OnClickInitServer);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_SERVER_INITIALIZATION, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL, bValue: false);
			if (NKCConnectionInfo.GetLoginServerCount() > 1)
			{
				NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_SELECT_SERVER, OnClickSelectServer);
				NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_SELECT_SERVER, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_SELECT_SERVER, bValue: false);
			}
		}
		if (NKCPublisherModule.Marketing.IsCouponEnabled())
		{
			NKCUtil.SetGameobjectActive(m_objNKM_UI_GAME_OPTION_ACCOUNT_COUPON, bValue: true);
			if (NKCPublisherModule.Marketing.IsUseSelfCouponPopup())
			{
				NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_ACCOUNT_COUPON, NKCPublisherModule.Marketing.OpenCoupon);
			}
			else
			{
				NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_ACCOUNT_COUPON, OnClickCouponPopup);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objNKM_UI_GAME_OPTION_ACCOUNT_COUPON, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_CUSTOMER_SERVICE_CENTER, NKCPublisherModule.Notice.IsActiveCustomerCenter());
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_GAME_GRADE_CHECK, NKCPublisherModule.ShowGameOptionGradeCheck());
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_INPUT, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_REWARD, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_IMPORTANT_NOTICE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_IMPORTANT_NOTICE_REFUSE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_RESOURCE_DIVISION, bValue: false);
		if (NKCPublisherModule.InAppPurchase.ShowCashResourceState())
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_RESOURCE_DIVISION, bValue: true);
			NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_RESOURCE_DIVISION, OnClickResourceDivision);
		}
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.TWN) || NKMContentsVersionManager.HasCountryTag(CountryTagType.CHN) || NKMContentsVersionManager.HasCountryTag(CountryTagType.SEA))
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_CONNECTION, bValue: false);
			NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_NOTICE, OnClickCommunity);
			NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_QnA, OnClickQnA);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_COMMUNITY, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_NOTICE, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_QnA, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_COPY.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNKM_UI_GAME_OPTION_GUEST_ACCOUNT_TRANSFER, bValue: false);
		}
		else if (NKCPublisherModule.IsNexonPCBuild())
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_CONNECTION, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_NOTICE, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_QnA, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_COPY.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID, bValue: true);
			NKCUtil.SetGameobjectActive(m_objNKM_UI_GAME_OPTION_GUEST_ACCOUNT_TRANSFER, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNKM_UI_GAME_OPTION_LOGOUT, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNKM_UI_GAME_OPTION_LOGOUT_dummy, bValue: false);
		}
		else if (NKCPublisherModule.IsSteamPC())
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_CONNECTION, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_NOTICE, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_QnA, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_COPY.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID, bValue: true);
			NKCUtil.SetGameobjectActive(m_objNKM_UI_GAME_OPTION_GUEST_ACCOUNT_TRANSFER, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNKM_UI_GAME_OPTION_LOGOUT, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNKM_UI_GAME_OPTION_LOGOUT_dummy, bValue: false);
		}
		else if (NKCPublisherModule.IsGamebasePublished())
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_NOTICE, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_QnA, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_COPY.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID, bValue: true);
			NKCUtil.SetGameobjectActive(m_objNKM_UI_GAME_OPTION_GUEST_ACCOUNT_TRANSFER, bValue: false);
			NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_ADMOB_SHOW_PRIVACY_OPTION, delegate
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_OPTION_ACCOUNT_GDPR_DESC"), OnClickAdmobShowPrivacy);
			});
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_NOTICE, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_QnA, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_COPY.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID, bValue: true);
			NKCUtil.SetGameobjectActive(m_objNKM_UI_GAME_OPTION_GUEST_ACCOUNT_TRANSFER, bValue: true);
			if (NKCDefineManager.DEFINE_NXTOY_JP())
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL, bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_objOptionAllowPush, !NKCDefineManager.DEFINE_STEAM() && NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.NOT_USE_LOCAL_PUSH));
		NKCUtil.SetToggleValueChangedDelegate(m_ctglOptionAllowPush, OnClickTogglePush);
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_PURCHASE_RECOVERY, NKCPublisherModule.InAppPurchase.IsBillingRestoreActive());
	}

	private void OnClickCouponPopup()
	{
		NKCPopupCoupon.Instance.Open(OnClickCouponPopupOK);
	}

	private void OnClickCouponPopupOK(string code)
	{
		NKCPublisherModule.Marketing.SendUseCouponReqToCSServer(code);
	}

	private void SetContentForNxToy(NKMUserData userData)
	{
	}

	private void SetContentForNexonPC(NKMUserData userData)
	{
		NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_INFO, NKCPublisherModule.Auth.GetPublisherAccountCode());
		if (string.IsNullOrWhiteSpace(NKCPublisherModule.Auth.GetPublisherAccountCode()))
		{
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.text = DISCONNECTED_STRING;
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.color = cDisonnectedColor;
		}
		else
		{
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.text = CONNECTED_STRING;
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.color = Color.yellow;
		}
		m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL.enabled = true;
		m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL.UnLock();
		m_NKM_UI_GAME_OPTION_ACCOUNT_CONNECTION.Lock();
	}

	private void SetContentForSteamPC(NKMUserData userData)
	{
		NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_INFO, NKCPublisherModule.Auth.GetPublisherAccountCode());
		if (string.IsNullOrWhiteSpace(NKCPublisherModule.Auth.GetPublisherAccountCode()))
		{
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.text = DISCONNECTED_STRING;
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.color = cDisonnectedColor;
		}
		else
		{
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.text = CONNECTED_STRING;
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.color = Color.yellow;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_CONNECTION, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_GUEST_ACCOUNT_TRANSFER, bValue: false);
		m_NKM_UI_GAME_OPTION_ACCOUNT_CONNECTION.Lock();
		NKCUtil.SetGameobjectActive(m_STEAM, bValue: true);
	}

	private void SetContentForGamebase(NKMUserData userData)
	{
		if (NKCPublisherModule.Auth.IsGuest())
		{
			m_NKM_UI_GAME_OPTION_ACCOUNT_CONNECTION.UnLock();
			m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL.enabled = true;
			m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL.UnLock();
			NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED, NKCUtilString.GET_STRING_TOY_LOGGED_IN_GUEST_KOR);
			NKCUtil.SetLabelTextColor(m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED, Color.white);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NEXON, bValue: false);
			NKCUtil.SetGameobjectActive(m_NAVER, bValue: false);
			NKCUtil.SetGameobjectActive(m_FACEBOOK, NKCPublisherModule.Auth.GetLoginIdpType() == NKCPublisherModule.NKCPMAuthentication.LOGIN_IDP_TYPE.facebook);
			NKCUtil.SetGameobjectActive(m_GOOGLE, NKCPublisherModule.Auth.GetLoginIdpType() == NKCPublisherModule.NKCPMAuthentication.LOGIN_IDP_TYPE.google);
			NKCUtil.SetGameobjectActive(m_APPLE, NKCPublisherModule.Auth.GetLoginIdpType() == NKCPublisherModule.NKCPMAuthentication.LOGIN_IDP_TYPE.appleid);
			NKCUtil.SetGameobjectActive(m_TWITTER, NKCPublisherModule.Auth.GetLoginIdpType() == NKCPublisherModule.NKCPMAuthentication.LOGIN_IDP_TYPE.twitter);
			m_NKM_UI_GAME_OPTION_ACCOUNT_CONNECTION.Lock();
			m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL.UnLock();
		}
		if (string.IsNullOrWhiteSpace(NKCPublisherModule.Auth.GetPublisherAccountCode()))
		{
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.text = DISCONNECTED_STRING;
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.color = cDisonnectedColor;
		}
		else
		{
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_INFO.text = NKCPublisherModule.Auth.GetPublisherAccountCode();
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.text = CONNECTED_STRING;
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.color = Color.yellow;
		}
	}

	public override void SetContent()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			m_NKM_UI_GAME_OPTION_ACCOUNT_ID_INFO.text = myUserData.m_FriendCode.ToString();
			m_NKM_UI_GAME_OPTION_ACCOUNT_NICKNAME_INFO.text = myUserData.m_UserNickName.ToString();
			m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL.enabled = true;
			m_NKM_UI_GAME_OPTION_MEMBER_WITHDRAWAL.UnLock();
			m_NKM_UI_GAME_OPTION_ACCOUNT_CONNECTION.Lock();
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_INFO.text = "";
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.text = DISCONNECTED_STRING;
			m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_CONNECTED.color = cDisonnectedColor;
			if (NKCDefineManager.DEFINE_NXTOY())
			{
				SetContentForNxToy(myUserData);
			}
			else
			{
				if (NKCPublisherModule.IsNexonPCBuild())
				{
					SetContentForNexonPC(myUserData);
				}
				if (NKCPublisherModule.IsGamebasePublished())
				{
					SetContentForGamebase(myUserData);
				}
				if (NKCPublisherModule.IsSteamPC())
				{
					SetContentForSteamPC(myUserData);
				}
			}
		}
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.STEAM_ACCOUNT_LINK))
		{
			bool flag = NKCAccountLinkMgr.m_requestUserProfile != null;
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_INPUT, bValue: true);
			NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_INPUT, OnClickAccountLinkCodeInput);
			if (NKCPublisherModule.Auth.IsGuest())
			{
				NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK, delegate
				{
					NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_STEAM_LINK_GUEST_ACCOUNT);
				});
			}
			else
			{
				NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK, OnClickAccountLink);
			}
			Log.Debug("[SteamLink][Option] HasLinkRequest[" + flag + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/GameOption/NKCUIGameOptionAccount.cs", 482);
			if (m_NKM_UI_GAME_OPTION_ACCOUNT_LINK != null && m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_INPUT != null)
			{
				if (flag)
				{
					m_NKM_UI_GAME_OPTION_ACCOUNT_LINK.Lock();
					m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_INPUT.UnLock();
				}
				else
				{
					m_NKM_UI_GAME_OPTION_ACCOUNT_LINK.UnLock();
					m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_INPUT.Lock();
				}
				NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_TEXT, NKCPublisherModule.Auth.GetAccountLinkText());
				NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_TEXT_LOCK, NKCPublisherModule.Auth.GetAccountLinkText());
				NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_INPUT_TEXT, NKCStringTable.GetString("SI_PF_STEAMLINK_OPEN_CODE_INPUT"));
				NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_INPUT_TEXT_LOCK, NKCStringTable.GetString("SI_PF_STEAMLINK_OPEN_CODE_INPUT"));
			}
			Log.Debug($"[SteamLink][Option] UserData - enableAccountLink[{myUserData.m_enableAccountLink}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/GameOption/NKCUIGameOptionAccount.cs", 502);
			if (myUserData.m_enableAccountLink)
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_REWARD, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_INPUT, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_REWARD, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ACCOUNT_LINK_REWARD, bValue: true);
			}
		}
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.SERVICE_TRANSFER_REGIST))
		{
			NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_IMPORTANT_NOTICE_TEXT, NKCStringTable.GetString("SI_PF_SERVICE_TRANSFER_REGIST_NOTICE_TITLE"));
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_IMPORTANT_NOTICE, bValue: true);
			NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_IMPORTANT_NOTICE, OnClickServiceTransferRegist);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_IMPORTANT_NOTICE_REFUSE, bValue: true);
			NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_IMPORTANT_NOTICE_REFUSE, OnClickServiceCenter);
		}
		else if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.SERVICE_TRANSFER))
		{
			NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_IMPORTANT_NOTICE_TEXT, NKCStringTable.GetString("SI_PF_SERVICE_TRANSFER"));
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_IMPORTANT_NOTICE, bValue: true);
			NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_IMPORTANT_NOTICE, OnClickServiceTransfer);
		}
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			m_ctglOptionAllowPush?.Select(gameOptionData.GetAllowPush(NKC_GAME_OPTION_PUSH_GROUP.PUSH), bForce: true);
		}
	}

	private void OnClickLogoutButton()
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCPopupOKCancel.OpenOKBox(LOGOUT_WARNING_TITLE_STRING, LOGOUT_UNUSABLE_CONTENT_STRING);
		}
		else
		{
			NKCPopupOKCancel.OpenOKCancelBox(LOGOUT_WARNING_TITLE_STRING, LOGOUT_USABLE_CONTENT_STRING, OnClickLogoutOKButton);
		}
	}

	private void OnClickGuestAccountTransferButton()
	{
	}

	private void OnClickAccountCodeCopy()
	{
		if (m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_INFO != null)
		{
			GUIUtility.systemCopyBuffer = m_NKM_UI_GAME_OPTION_ACCOUNT_ACCOUNTID_INFO.text;
		}
	}

	private void OnClickAnnouncement()
	{
		NKCUIGameOption.Instance.Close();
		NKCUINews.Instance.SetDataAndOpen(bForceRefresh: true);
	}

	private void OnClickBillingRestore()
	{
		NKCPublisherModule.InAppPurchase.BillingRestore(NKCShopManager.OnBillingRestoreManual);
	}

	private void OnClickServiceCenter()
	{
		NKCPublisherModule.Notice.OpenCustomerCenter(OnCustomerCenter);
	}

	private void OnCustomerCenter(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		switch (resultCode)
		{
		case NKC_PUBLISHER_RESULT_CODE.NPRC_OK:
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_ACCOUNT_DATA_RESTORE_SUCCESS:
		case NKC_PUBLISHER_RESULT_CODE.NPRC_ACCOUNT_USER_QUIT_SUCCESS:
			NKCPacketHandlersLobby.MoveToLogin();
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_ACCOUNT_DATA_RESTORE_FAIL:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TOY_GUEST_ACCOUT_RESTORE_FAIL + "\n" + NKCPublisherModule.LastError);
			break;
		default:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(resultCode.ToString()));
			break;
		}
	}

	private void OnClickQnA()
	{
		NKCPublisherModule.Notice.OpenQnA(null);
	}

	private void OnClickWithdrawal()
	{
		if (NKCDefineManager.DEFINE_SB_GB())
		{
			if (NKCPublisherModule.Auth.IsGuest())
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_OPTION_DROPOUT_WARNING_INSTANT, delegate
				{
					NKCUIAccountWithdrawCheckPopup.Instance.OpenUI();
				});
			}
			else
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_OPTION_DROPOUT_WARNING, delegate
				{
					NKCUIAccountWithdrawCheckPopup.Instance.OpenUI();
				});
			}
		}
		else
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_OPTION_DROPOUT_WARNING, delegate
			{
				NKCUIAccountWithdrawCheckPopup.Instance.OpenUI();
			}, delegate
			{
			});
		}
	}

	private void OnClickWithdrawalNexon()
	{
	}

	private void OnClickSyncAccount()
	{
		NKCPublisherModule.Auth.ChangeAccount(AfterChangeAccount, syncAccount: true);
	}

	private void AfterChangeAccount(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		switch (resultCode)
		{
		case NKC_PUBLISHER_RESULT_CODE.NPRC_OK:
		case NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_CHANGEACCOUNT_SUCCESS_GUEST_SYNC:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_CHANGEACCOUNT_SUCCESS_GUEST_SYNC);
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_CHANGEACCOUNT_SUCCESS_ACCOUNT_CHANGED:
			NKCPacketHandlersLobby.MoveToLogin();
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_CHANGEACCOUNT_FAIL_NO_CHANGEABLE:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TOY_SYNC_ACCOUNT_FAIL);
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_CHANGEACCOUNT_FAIL_GUEST_ALREADY_MAPPED:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_CHANGEACCOUNT_FAIL_GUEST_ALREADY_MAPPED);
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_CHANGEACCOUNT_SUCCESS_GUEST_SYNC_RELOGIN:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_DP_CHANGEACCOUNT_SUCCESS_GUEST_SYNC_LOGOUT"), delegate
			{
				OnClickLogoutOKButton();
			});
			break;
		}
	}

	private void OnClickLogoutOKButton()
	{
		NKMPopUpBox.OpenWaitBox();
		NKCPublisherModule.Auth.Logout(OnLogoutComplete);
	}

	private void OnLogoutComplete(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (NKCPublisherModule.CheckError(resultCode, additionalError))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TOY_LOGOUT_SUCCESS, NKCPacketHandlersLobby.MoveToLogin);
		}
	}

	private void OnClickGuide()
	{
		NKCUIPopUpGuide.Instance.Open();
	}

	private void OnClickGameGradeCheck()
	{
		NKCPopupGameGradeCheck.Instance.Open();
	}

	private void OnClickCommunity()
	{
		NKCPublisherModule.Notice.OpenCommunity(null);
	}

	private void OnClickResourceDivision()
	{
		NKCPopupResourceDivision.Instance.Open();
	}

	private void OnClickAccountLink()
	{
		NKCAccountLinkMgr.StartLinkProcess();
	}

	private void OnClickAccountLinkCodeInput()
	{
		NKCAccountLinkMgr.OpenPrivateLinkCodeInput();
	}

	private void OnClickServiceTransfer()
	{
		NKCServiceTransferMgr.StartServiceTransferProcess();
	}

	private void OnClickServiceTransferRegist()
	{
		NKCServiceTransferMgr.StartServiceTransferRegistProcess();
	}

	private void OnClickInitServer()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCStringTable.GetString("SI_PF_POPUP_SERVER_INITIALIZATION_WARNING"), delegate
		{
			NKCUIAccountWithdrawCheckPopup.Instance.OpenUI(bInitAccount: true);
		});
	}

	private void OnClickSelectServer()
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCStringTable.GetString("SI_DP_OPTION_CANNOT_CHANGE_SERVER_WHEN_IN_GAME_BATTLE"));
		}
		else
		{
			NKCPopupSelectServer.Instance.Open(bShowCloseMenu: true, bMoveToPatcher: true);
		}
	}

	private void OnClickAdmobShowPrivacy()
	{
	}

	private void OnClickTogglePush(bool bSet)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.SetAllowPush(NKC_GAME_OPTION_PUSH_GROUP.PUSH, bSet);
			NKCPublisherModule.Push.ReRegisterPush();
		}
	}
}
