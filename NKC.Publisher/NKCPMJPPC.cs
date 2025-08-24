using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using ClientPacket.Account;
using Cs.Logging;
using Cs.Protocol;
using NKC.UI;
using NKM;
using NKM.Shop;
using UnityEngine;

namespace NKC.Publisher;

public class NKCPMJPPC : NKCPublisherModule
{
	public class AuthJPPC : NKCPMAuthentication
	{
		private bool m_bLoginSuccessFromPubAuth;

		private string m_strNexonPassport = "";

		private string m_strNxHWID = "";

		private DateTime m_UserInfoAcquisitionTime;

		public static bool s_bNGS_Start;

		public static bool s_bNGS_Start_Success;

		public override bool LoginToPublisherCompleted => m_bLoginSuccessFromPubAuth;

		public override bool Init()
		{
			m_strNexonPassport = "";
			m_strNxHWID = "";
			m_bLoginSuccessFromPubAuth = false;
			return true;
		}

		public override bool OnLoginSuccessToCS()
		{
			if (s_bNGS_Start_Success)
			{
				return true;
			}
			if (s_bNGS_Start)
			{
				return false;
			}
			s_bNGS_Start = true;
			if (0 == 0)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, "NGS Start Failed", delegate
				{
					Application.Quit();
				});
				return false;
			}
			s_bNGS_Start_Success = true;
			return false;
		}

		public override string GetPublisherAccountCode()
		{
			return NKCPMNexonNGS.GetNpaCode();
		}

		public override void LoginToPublisher(OnComplete dOnComplete)
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			string text = "";
			if (commandLineArgs != null && commandLineArgs.Length > 1)
			{
				text = commandLineArgs[1];
			}
			Log.Debug("passPort : " + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMJPPC.cs", 250);
			text = text.Replace("/passport:", "");
			Log.Debug("/passport: 제거 후, passPort : " + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMJPPC.cs", 254);
			int num = 0;
			if (num != 0)
			{
				m_bLoginSuccessFromPubAuth = false;
				string additionalError = "";
				switch (num)
				{
				case 20000:
					additionalError = "SSOサーバーアクセス失敗、またはSSOサーバー内部エラーです。";
					break;
				case 20002:
					additionalError = "メンテナンス中です。";
					break;
				case 20013:
					additionalError = "プレイヤー認証情報がありません。";
					break;
				case 20014:
					additionalError = "ログインIPが不一致です。";
					break;
				case 20015:
					additionalError = "Nexon Passport整合性にエラーが発生しています。";
					break;
				case 20018:
					additionalError = "他のIPで接続されています。";
					break;
				case 20048:
					additionalError = "2次認証に失敗しました。";
					break;
				case 20049:
					additionalError = "誤ったゲーム情報です。";
					break;
				case 20056:
					additionalError = "誤ったチャネリング情報です。";
					break;
				}
				dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_LOGIN_FAIL, additionalError);
			}
		}

		public override void PrepareCSLogin(OnComplete dOnComplete)
		{
			if (m_bLoginSuccessFromPubAuth)
			{
				dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
			}
			else
			{
				dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_LOGIN_FAIL_NOT_READY);
			}
		}

		public override void ChangeAccount(OnComplete onComplete, bool syncAccount)
		{
			onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_FAIL_NO_SUPPORT);
		}

		public override ISerializable MakeLoginServerLoginReqPacket()
		{
			if (string.IsNullOrWhiteSpace(NKCScenManager.GetScenManager().GetConnectGame().GetReconnectKey()))
			{
				NKMPacket_NXPC_LOGIN_REQ obj = new NKMPacket_NXPC_LOGIN_REQ
				{
					deviceUid = m_strNxHWID
				};
				Debug.Log("nexonPassport : " + m_strNexonPassport);
				obj.nexonPassport = m_strNexonPassport;
				obj.protocolVersion = 960;
				obj.ssoLoginDate = m_UserInfoAcquisitionTime;
				obj.userMobileData = new NKMUserMobileData
				{
					m_MarketId = "0",
					m_Country = "JP",
					m_Language = "JA_JP",
					m_AuthPlatform = "SSO",
					m_Platform = Application.platform.ToString(),
					m_OsVersion = SystemInfo.operatingSystem.ToString(),
					m_AdId = "",
					m_ClientVersion = Application.version
				};
				return obj;
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
	}

	public class InAppJPPC : NKCPMInAppPurchase
	{
		public override bool CheckReceivedBillingProductList => true;

		public override void RequestBillingProductList(OnComplete dOnComplete)
		{
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public override void BillingRestore(OnComplete dOnComplete)
		{
			RunFakeProcess(dOnComplete, "Billing Restored", showPopup: false);
		}

		public override void InappPurchase(ShopItemTemplet shopTemplet, OnComplete dOnComplete, string metadata = "", List<int> lstSelection = null)
		{
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_FAIL_NOT_SUPPORTED);
		}

		public override bool IsRegisteredProduct(string marketID, int productID)
		{
			return false;
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
			NKCPublisherModule.Notice.OpenURL("http://m.nexon.com/terms/60", dOnClose);
		}

		public override string GetCurrencyMark(int productID)
		{
			return "円";
		}

		public override void OpenPaymentLaw(OnComplete dOnClose)
		{
			NKCPublisherModule.Notice.OpenURL("https://m.nexon.com/terms/625", dOnClose);
		}

		public override void OpenCommercialLaw(OnComplete dOnClose)
		{
			NKCPublisherModule.Notice.OpenURL("https://m.nexon.com/terms/629", dOnClose);
		}
	}

	public class NoticeJPPC : NKCPMNotice
	{
		public override void OpenCustomerCenter(OnComplete dOnComplete)
		{
			Log.Debug("[OpenCustomerCenter] START", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMJPPC.cs", 464);
			string text = "";
			string text2 = "https://m-page.nexon.com/cc/jppc/auth/sso?clientId=MjY5OQ&npp=" + text;
			Log.Debug("[OpenCustomerCenter] " + text2, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMJPPC.cs", 471);
			string text3 = "{GameID:STAGE,CharacterName:" + NKCScenManager.CurrentUserData().m_UserNickName + ",ClientVersion:" + Application.version + ",UID:" + NKCScenManager.CurrentUserData().m_UserUID + ",NPA_Code:" + NKCPublisherModule.Auth.GetPublisherAccountCode() + ",BusinessLicenseCode:" + NKCScenManager.CurrentUserData().m_FriendCode + "}";
			Log.Debug("[OpenCustomerCenter] RequestHeader [" + text3 + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMJPPC.cs", 483);
			if (false)
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text2.Trim());
				httpWebRequest.Method = "GET";
				httpWebRequest.Headers.Add("x-toy-locale", "ja-JP");
				httpWebRequest.Headers.Add("x-toy-game-meta", text3);
				try
				{
					using HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
					if (httpWebResponse.StatusCode == HttpStatusCode.OK)
					{
						Log.Debug("[OpenCustomerCenter] WebResponse - OK", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMJPPC.cs", 499);
					}
					else
					{
						Log.Debug("[OpenCustomerCenter] response StatusCode[" + httpWebResponse.StatusDescription + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMJPPC.cs", 503);
					}
				}
				catch (Exception ex)
				{
					Log.Error("HTTPWebRequest Exception : " + ex.Message, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMJPPC.cs", 509);
				}
			}
			NKCPublisherModule.Notice.OpenURL(text2.Trim(), dOnComplete);
		}

		public override bool IsActiveCustomerCenter()
		{
			return true;
		}

		public override void OpenQnA(OnComplete dOnComplete)
		{
		}

		public override bool CheckOpenNoticeWhenFirstLobbyVisit()
		{
			return false;
		}

		public override void OpenNotice(OnComplete onComplete)
		{
			NKCPublisherModule.Notice.OpenURL("https://counterside.nexon.co.jp/news", onComplete);
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

	public class ServerInfoJPPC : NKCPMServerInfo
	{
		public override bool GetUseLocalSaveLastServerInfoToGetTags()
		{
			return false;
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

	public const uint GAMECODE_TEST = 16818287u;

	public const uint GAMECODE_STAGE = 16818288u;

	public const uint GAMECODE_LIVE = 16818286u;

	public const string URL_PAYMENT_LAW = "https://m.nexon.com/terms/625";

	public const string URL_POLICY = "http://m.nexon.com/terms/60";

	public const string URL_WEB_BANNER = "https://counterside.nexon.co.jp/news";

	public const string URL_COMMERCIAL_LAW_PC = "https://m.nexon.com/terms/629";

	public const string GAME_ID = "STAGE";

	public const string SERVICE_ID = "MjY5OQ";

	public const string URL_CUSTOMER_CENTER = "https://m-page.nexon.com/cc/jppc/auth/sso?clientId={0}&npp={1}";

	protected override ePublisherType _PublisherType => ePublisherType.JPPC;

	protected override bool _Busy => false;

	protected override void OnTimeOut()
	{
	}

	private void OnDestroy()
	{
	}

	private static void NgsSendCallback(IntPtr bytes, uint size, uint sessionID)
	{
		byte[] array = new byte[size];
		Marshal.Copy(bytes, array, 0, (int)size);
		Log.Debug($"[NgsSendCallback] SessionID[{sessionID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMJPPC.cs", 73);
		NKCPacketSender.Send_NKMPacket_NEXON_NGS_DATA_NOT(array);
	}

	protected override NKCPMAuthentication MakeAuthInstance()
	{
		return new AuthJPPC();
	}

	protected override NKCPMInAppPurchase MakeInappInstance()
	{
		return new InAppJPPC();
	}

	protected override NKCPMNotice MakeNoticeInstance()
	{
		return new NoticeJPPC();
	}

	protected override NKCPMServerInfo MakeServerInfoInstance()
	{
		return new ServerInfoJPPC();
	}

	protected override NKCPMStatistics MakeStatisticsInstance()
	{
		return new StatisticsNone();
	}

	private static void OnAuthClosedCallback(uint nType)
	{
		switch (nType)
		{
		case 1u:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, "인증 서비스가 중지 되었습니다.", delegate
			{
				Application.Quit();
			});
			break;
		case 2u:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, "네트워크 연결에 실패했습니다.", delegate
			{
				Application.Quit();
			});
			break;
		case 20014u:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, "IP가 변경 되었습니다.", delegate
			{
				Application.Quit();
			});
			break;
		case 20015u:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, "넥슨 패스포트가 유효하지 않습니다.", delegate
			{
				Application.Quit();
			});
			break;
		case 20018u:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, "다른 세션에 의해 로그아웃 되었습니다.", delegate
			{
				Application.Quit();
			});
			break;
		default:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, "인증 서버 접속이 끊겼습니다.", delegate
			{
				Application.Quit();
			});
			break;
		}
	}

	protected override void _Init(OnComplete dOnComplete)
	{
		if (!NKCDefineManager.DEFINE_NX_PC_TEST() && !NKCDefineManager.DEFINE_NX_PC_STAGE() && !NKCDefineManager.DEFINE_NX_PC_LIVE())
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, "Stage Build인지 Live Build인지 알 수 없습니다.", delegate
			{
				Application.Quit();
			});
		}
		else
		{
			NKCPublisherModule.InitState = ePublisherInitState.Initialized;
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}
	}

	private static void RunFakeProcess(OnComplete dOnComplete, string fakeMessage, bool showPopup)
	{
		Debug.Log(fakeMessage);
		dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
	}
}
