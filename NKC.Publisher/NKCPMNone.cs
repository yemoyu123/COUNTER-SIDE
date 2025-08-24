using System;
using System.Collections.Generic;
using ClientPacket.Account;
using Cs.Logging;
using Cs.Protocol;
using NKC.UI;
using NKM;
using NKM.Shop;
using UnityEngine;

namespace NKC.Publisher;

public class NKCPMNone : NKCPublisherModule
{
	public class AuthNone : NKCPMAuthentication
	{
		public override bool LoginToPublisherCompleted => true;

		public override string GetPublisherAccountCode()
		{
			if (NKCScenManager.CurrentUserData() != null)
			{
				return NKCScenManager.CurrentUserData().m_UserUID.ToString();
			}
			return "";
		}

		public override void LoginToPublisher(OnComplete dOnComplete)
		{
			RunFakeProcess(dOnComplete, "SyncAccount", showPopup: false);
		}

		public override void PrepareCSLogin(OnComplete dOnComplete)
		{
			RunFakeProcess(dOnComplete, "Login", showPopup: false);
		}

		public override void ChangeAccount(OnComplete onComplete, bool syncAccount)
		{
			onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_FAIL_NO_SUPPORT);
		}

		public override ISerializable MakeLoginServerLoginReqPacket()
		{
			if (string.IsNullOrWhiteSpace(NKCScenManager.GetScenManager().GetConnectGame().GetReconnectKey()))
			{
				NKMPacket_LOGIN_REQ nKMPacket_LOGIN_REQ = new NKMPacket_LOGIN_REQ();
				nKMPacket_LOGIN_REQ.protocolVersion = 960L;
				nKMPacket_LOGIN_REQ.accountID = PlayerPrefs.GetString("NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING");
				nKMPacket_LOGIN_REQ.password = PlayerPrefs.GetString("NKM_LOCAL_SAVE_KEY_LOGIN_PASSWORD_STRING");
				Log.Debug($"[Login] LoginReq ProtocolVersion[{nKMPacket_LOGIN_REQ.protocolVersion}] ID[{nKMPacket_LOGIN_REQ.accountID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMNone.cs", 168);
				NKM_USER_AUTH_LEVEL userAuthLevel = NKM_USER_AUTH_LEVEL.NORMAL_USER;
				nKMPacket_LOGIN_REQ.userAuthLevel = userAuthLevel;
				nKMPacket_LOGIN_REQ.deviceUid = SystemInfo.deviceUniqueIdentifier;
				return nKMPacket_LOGIN_REQ;
			}
			return new NKMPacket_RECONNECT_REQ
			{
				reconnectKey = NKCScenManager.GetScenManager().GetConnectGame().GetReconnectKey()
			};
		}

		public override ISerializable MakeGameServerLoginReqPacket(string accessToken)
		{
			return new NKMPacket_JOIN_LOBBY_REQ
			{
				protocolVersion = 960,
				accessToken = accessToken
			};
		}

		public override void Logout(OnComplete dOnComplete)
		{
			RunFakeProcess(dOnComplete, "Logout", showPopup: false);
		}

		public override bool IsTryAuthWhenSessionExpired()
		{
			return true;
		}
	}

	public class InAppNone : NKCPMInAppPurchase
	{
		public override bool CheckReceivedBillingProductList => true;

		public override void RequestBillingProductList(OnComplete dOnComplete)
		{
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public override void BillingRestore(OnComplete dOnComplete)
		{
			RunFakeProcess(dOnComplete, "Billing Restored", showPopup: true);
		}

		public override void InappPurchase(ShopItemTemplet shopTemplet, OnComplete dOnComplete, string metadata = "", List<int> lstSelection = null)
		{
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_FAIL_NOT_SUPPORTED);
		}

		public override bool IsRegisteredProduct(string marketID, int productID)
		{
			return NKCShopManager.GetShopTempletByMarketID(marketID) != null;
		}

		public override decimal GetLocalPrice(string marketID, int productID)
		{
			ShopItemTemplet shopTempletByMarketID = NKCShopManager.GetShopTempletByMarketID(marketID);
			return NKCScenManager.CurrentUserData().m_ShopData.GetRealPrice(shopTempletByMarketID);
		}

		public override string GetLocalPriceString(string marketID, int productID)
		{
			ShopItemTemplet shopTempletByMarketID = NKCShopManager.GetShopTempletByMarketID(marketID);
			return NKCUtilString.GetInAppPurchasePriceString(NKCScenManager.CurrentUserData().m_ShopData.GetRealPrice(shopTempletByMarketID).ToString("N0"), productID);
		}

		public override List<int> GetInappProductIDs()
		{
			return new List<int>(NKCShopManager.GetMarketProductList().Keys);
		}

		public override void OpenPolicy(OnComplete dOnClose)
		{
			dOnClose?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}
	}

	public class NoticeNone : NKCPMNotice
	{
		public override void OpenCustomerCenter(OnComplete dOnComplete)
		{
			Application.OpenURL("https://forum.nexon.com/counterside");
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public override bool IsActiveCustomerCenter()
		{
			return true;
		}

		public override void OpenQnA(OnComplete dOnComplete)
		{
		}

		public override void OpenNotice(OnComplete onComplete)
		{
			if (NKCDefineManager.DEFINE_WEBVIEW_TEST())
			{
				NKMPopUpBox.CloseWaitBox();
				NKCPopupNoticeWeb.Instance.Open("https://counterside.nexon.com/Banner/Index/", onComplete);
			}
			else if (NKCNewsManager.CheckNeedNewsPopup(NKCSynchronizedTime.GetServerUTCTime()))
			{
				NKCUINews.Instance.SetDataAndOpen(bForceRefresh: true);
				NKCUINews.Instance.SetCloseCallback(delegate
				{
					onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
				});
			}
			else
			{
				onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
			}
		}

		public override void OpenPromotionalBanner(eOptionalBannerPlaces placeType, OnComplete dOnComplete)
		{
			RunFakeProcess(dOnComplete, "OptionalBanner : " + placeType, showPopup: true);
		}

		public override void NotifyMainenance(OnComplete dOnComplete)
		{
			RunFakeProcess(dOnComplete, "Maintanance", showPopup: true);
		}
	}

	public class StatisticsNone : NKCPMStatistics
	{
		public override void LogClientActionForPublisher(eClientAction funnelPosition, int key = 0, string data = null)
		{
			RunFakeProcess(null, $"SendFunnel : {funnelPosition} {key}", showPopup: false);
		}

		public override void TrackPurchase(int itemID)
		{
			RunFakeProcess(null, $"TrackPurchase : {itemID}", showPopup: false);
		}
	}

	public class PushNone : NKCPMPush
	{
		public override void Init()
		{
		}

		protected override void CancelLocalPush(NKC_GAME_OPTION_ALARM_GROUP evtType)
		{
		}

		protected override void ClearAllLocalPush()
		{
			base.ClearAllLocalPush();
		}

		protected override bool ReserveLocalPush(DateTime newUtcTime, NKC_GAME_OPTION_ALARM_GROUP evtType)
		{
			CancelLocalPush(evtType);
			TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(newUtcTime.Ticks);
			Debug.Log($"로컬푸시 등록 - 타입 : {evtType}, 등록 시간 : {newUtcTime} : 남은 시간 : {timeLeft}");
			return true;
		}
	}

	public class PermissionNone : NKCPMPermission
	{
		public override void RequestCameraPermission(OnComplete dOnComplete)
		{
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}
	}

	public class ServerInfoDefault : NKCPMServerInfo
	{
		public override bool GetUseLocalSaveLastServerInfoToGetTags()
		{
			if (NKCDefineManager.DEFINE_SELECT_SERVER())
			{
				return false;
			}
			if (NKCDefineManager.DEFINE_DOWNLOAD_CONFIG())
			{
				return false;
			}
			return true;
		}

		public override string GetServerConfigPath()
		{
			string text = UnityEngine.Random.Range(1000000, 8000000).ToString();
			text += UnityEngine.Random.Range(1000000, 8000000);
			string text2 = "?p=" + text;
			string text3 = "http://FileServer.bside.com/ConnectionInfo/";
			string text4 = "ConnectionInfo.json";
			if (NKCDefineManager.DEFINE_SELECT_SERVER())
			{
				text3 = "http://FileServer.bside.com/server_config/Dev/";
				string customServerInfoAddress = NKCConnectionInfo.CustomServerInfoAddress;
				if (!string.IsNullOrEmpty(customServerInfoAddress))
				{
					text3 = customServerInfoAddress;
				}
				text4 = NKCConnectionInfo.ServerInfoFileName;
			}
			return text3 + text4 + text2;
		}
	}

	public class LocalizationNone : NKCPMLocalization
	{
		public override NKM_NATIONAL_CODE GetDefaultLanguage()
		{
			return NKM_NATIONAL_CODE.NNC_KOREA;
		}
	}

	public class MarketingNone : NKCPMMarketing
	{
		private OnComplete dOnSnsShareComplete;

		public override bool SnsShareEnabled(NKMUnitData unitData)
		{
			return false;
		}

		public override void TrySnsShare(SNS_SHARE_TYPE sst, string capturePath, string thumbnailPath, OnComplete onComplete)
		{
			Debug.Log("TrySnsShare : " + capturePath + " / " + thumbnailPath);
			dOnSnsShareComplete = onComplete;
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, "이걸로 공유함?", OnShareOK, OnShareFinish);
		}

		private void OnShareFinish()
		{
			dOnSnsShareComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		private void OnShareOK()
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, "공유했소", OnShareFinish);
		}
	}

	private static bool s_bWait;

	protected override ePublisherType _PublisherType => ePublisherType.None;

	protected override bool _Busy => s_bWait;

	protected override NKCPMAuthentication MakeAuthInstance()
	{
		return new AuthNone();
	}

	protected override NKCPMInAppPurchase MakeInappInstance()
	{
		return new InAppNone();
	}

	protected override NKCPMNotice MakeNoticeInstance()
	{
		return new NoticeNone();
	}

	protected override NKCPMStatistics MakeStatisticsInstance()
	{
		return new StatisticsNone();
	}

	public override bool IsReviewServer()
	{
		if (NKCConnectionInfo.s_ServerType == "REVIEW")
		{
			return true;
		}
		return false;
	}

	protected override void OnTimeOut()
	{
		s_bWait = false;
	}

	protected override void _Init(OnComplete dOnComplete)
	{
		NKCPublisherModule.InitState = ePublisherInitState.Initialized;
		RunFakeProcess(dOnComplete, "Init", showPopup: false);
	}

	private static void RunFakeProcess(OnComplete dOnComplete, string fakeMessage, bool showPopup)
	{
		Debug.Log(fakeMessage);
		dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
	}
}
