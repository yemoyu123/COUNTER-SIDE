using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ClientPacket.Account;
using ClientPacket.Shop;
using Cs.Logging;
using Cs.Protocol;
using NKC.Localization;
using NKC.PacketHandler;
using NKC.UI;
using NKM;
using NKM.Shop;
using Steamworks;
using UnityEngine;

namespace NKC.Publisher;

public class NKCPMSteamPC : NKCPublisherModule
{
	public class AuthSteam : NKCPMAuthentication
	{
		private bool m_bLoginSuccessFromPubAuth;

		protected Callback<GetAuthSessionTicketResponse_t> m_AuthSessionTicketResponse;

		private HAuthTicket m_AuthSessionTicket;

		private byte[] m_Ticket = new byte[1024];

		private uint m_pcbTicket;

		private string m_strTicket;

		private ulong m_SteamUserID;

		private string m_strUserID;

		public string m_appID;

		private OnComplete m_onLoginToPublisherComplete;

		public override bool LoginToPublisherCompleted => m_bLoginSuccessFromPubAuth;

		public override bool Init()
		{
			m_bLoginSuccessFromPubAuth = false;
			m_pcbTicket = 0u;
			m_strTicket = string.Empty;
			Log.Debug($"[SteamLogin] AuthSteam - Init[{SteamManager.Initialized}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 130);
			if (!SteamManager.Initialized)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_STEAM_INITIALIZE), delegate
				{
					NKCMain.QuitGame();
				});
				return false;
			}
			m_AuthSessionTicket = HAuthTicket.Invalid;
			if (m_AuthSessionTicketResponse == null)
			{
				m_AuthSessionTicketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(OnAuthSessionTicketResponse);
			}
			return true;
		}

		public override string GetPublisherAccountCode()
		{
			return m_strUserID;
		}

		public override void LoginToPublisher(OnComplete dOnComplete)
		{
			try
			{
				Log.Debug("[SteamLogin] LoginBySteam - LoginToPublisher", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 157);
				m_SteamUserID = SteamUser.GetSteamID().m_SteamID;
				m_strUserID = m_SteamUserID.ToString();
				m_appID = SteamUtils.GetAppID().m_AppId.ToString();
				Log.Debug("[SteamLogin] AppID[" + m_appID + "] Language[" + ((LocalizationSteam)NKCPublisherModule.Localization).m_strGameLanguage + "] IPCountry[" + ((LocalizationSteam)NKCPublisherModule.Localization).m_strCountry + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 165);
				m_onLoginToPublisherComplete = dOnComplete;
				m_AuthSessionTicket = HAuthTicket.Invalid;
				if (m_AuthSessionTicketResponse == null)
				{
					m_AuthSessionTicketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(OnAuthSessionTicketResponse);
				}
				Log.Debug("[SteamLogin] LoginBySteam - GetAuthSessionTicket", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 175);
				m_AuthSessionTicket = SteamUser.GetAuthSessionTicket(m_Ticket, m_Ticket.Length, out m_pcbTicket);
				if (m_AuthSessionTicket == HAuthTicket.Invalid || m_pcbTicket == 0)
				{
					Log.Error("[SteamLogin] GetAuthSessionTicket Failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 180);
					dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_LOGIN_FAIL, "GetAuthSessionTicket");
				}
			}
			catch (Exception ex)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_STEAM_INITIALIZE), delegate
				{
					NKCMain.QuitGame();
				});
				Log.Error("[SteamLogin] Exception [" + ex.Message + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 189);
			}
		}

		private void OnAuthSessionTicketResponse(GetAuthSessionTicketResponse_t pCallback)
		{
			Log.Debug("[SteamLogin] OnAuthSessionTicketResponse", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 199);
			if (m_AuthSessionTicket == HAuthTicket.Invalid)
			{
				return;
			}
			EResult eResult = pCallback.m_eResult;
			if (eResult == EResult.k_EResultOK || eResult == EResult.k_EResultAdministratorOK)
			{
				Array.Resize(ref m_Ticket, (int)m_pcbTicket);
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < m_pcbTicket; i++)
				{
					stringBuilder.AppendFormat("{0:x2}", m_Ticket[i]);
				}
				m_strTicket = stringBuilder.ToString();
				m_bLoginSuccessFromPubAuth = true;
				if (m_onLoginToPublisherComplete != null)
				{
					m_onLoginToPublisherComplete(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
				}
			}
			else if (m_onLoginToPublisherComplete != null)
			{
				m_onLoginToPublisherComplete(NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_LOGIN_FAIL, pCallback.m_eResult.ToString());
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
				NKMPacket_STEAM_LOGIN_REQ obj = new NKMPacket_STEAM_LOGIN_REQ
				{
					accessToken = m_strTicket,
					accountId = m_strUserID,
					protocolVersion = 960,
					deviceUid = SystemInfo.deviceUniqueIdentifier
				};
				NKMUserMobileData userMobileData = new NKMUserMobileData
				{
					m_AdId = "",
					m_MarketId = "Steam",
					m_AuthPlatform = "Steam",
					m_Country = "Unknown",
					m_Language = NKCStringTable.GetCurrLanguageCode(),
					m_Platform = Application.platform.ToString(),
					m_OsVersion = SystemInfo.operatingSystem.ToString(),
					m_ClientVersion = Application.version
				};
				obj.userMobileData = userMobileData;
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

		public override string GetAccountLinkText()
		{
			return NKCStringTable.GetString("SI_PF_STEAMLINK_TO_MOBILE");
		}
	}

	public class LocalizationSteam : NKCPMLocalization
	{
		public class SteamLanguageCodeData
		{
			public string webApiLanguageCode;

			public string name;

			public string nativeName;

			public string csLanguageCode;

			public SteamLanguageCodeData(string _name, string _nativeName, string _webApiLanguageCode, string _csLanguageCode)
			{
				name = _name;
				nativeName = _nativeName;
				webApiLanguageCode = _webApiLanguageCode;
				csLanguageCode = _csLanguageCode;
			}
		}

		private string _steamCurrentCountry;

		private string _steamCurrentGameLanguage;

		private Dictionary<string, SteamLanguageCodeData> m_steamLanguageCodeData = new Dictionary<string, SteamLanguageCodeData>();

		public override bool UseDefaultLanguageOnFirstRun => true;

		public string m_strCountry
		{
			get
			{
				if (_steamCurrentCountry == null)
				{
					_steamCurrentCountry = SteamUtils.GetIPCountry();
					Log.Debug("[LocalizationSteam] m_strCountry[" + _steamCurrentCountry + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 330);
				}
				return _steamCurrentCountry;
			}
		}

		public string m_strGameLanguage
		{
			get
			{
				if (_steamCurrentGameLanguage == null)
				{
					Log.Debug("[LocalizationSteam] m_strGameLanguage is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 344);
					if (SteamManager.Initialized)
					{
						string steamUILanguage = SteamUtils.GetSteamUILanguage();
						_steamCurrentGameLanguage = SteamApps.GetCurrentGameLanguage();
						Log.Debug("[LocalizationSteam] GetCurrentGameLanguage[" + _steamCurrentGameLanguage + "] GetSteamUILanguage[" + steamUILanguage + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 350);
						string availableGameLanguages = SteamApps.GetAvailableGameLanguages();
						Log.Debug("[LocalizationSteam] GetAvailableGameLanguages[" + availableGameLanguages + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 353);
					}
					else
					{
						_steamCurrentGameLanguage = "english";
					}
					CreateSteamLanguageCodeData();
				}
				return _steamCurrentGameLanguage;
			}
		}

		public void CreateSteamLanguageCodeData()
		{
			Log.Debug("[LocalizationSteam] CreateSteamLanguageCodeData", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 389);
			m_steamLanguageCodeData.Clear();
			m_steamLanguageCodeData.Add("arabic", new SteamLanguageCodeData("Arabic", "العربية", "ar", null));
			m_steamLanguageCodeData.Add("bulgarian", new SteamLanguageCodeData("Bulgarian", "български език", "bg", null));
			m_steamLanguageCodeData.Add("finnish", new SteamLanguageCodeData("Finnish", "Suomi", "fi", null));
			m_steamLanguageCodeData.Add("greek", new SteamLanguageCodeData("Greek", "Ελληνικά", "el", null));
			m_steamLanguageCodeData.Add("hungarian", new SteamLanguageCodeData("Hungarian", "Magyar", "hu", null));
			m_steamLanguageCodeData.Add("italian", new SteamLanguageCodeData("Italian", "Italiano", "it", null));
			m_steamLanguageCodeData.Add("norwegian", new SteamLanguageCodeData("Norwegian", "Norsk", "no", null));
			m_steamLanguageCodeData.Add("polish", new SteamLanguageCodeData("Polish", "Polski", "pl", null));
			m_steamLanguageCodeData.Add("portuguese", new SteamLanguageCodeData("Portuguese", "Português", "pt", null));
			m_steamLanguageCodeData.Add("brazilian", new SteamLanguageCodeData("Portuguese-Brazil", "Português-Brasil", "pt-BR", null));
			m_steamLanguageCodeData.Add("romanian", new SteamLanguageCodeData("Romanian", "Română", "ro", null));
			m_steamLanguageCodeData.Add("russian", new SteamLanguageCodeData("Russian", "Русский", "ru", null));
			m_steamLanguageCodeData.Add("spanish", new SteamLanguageCodeData("Spanish-Spain", "Español-España", "es", null));
			m_steamLanguageCodeData.Add("latam", new SteamLanguageCodeData("Spanish-Latin America", "Español-Latinoamérica", "es-419", null));
			m_steamLanguageCodeData.Add("swedish", new SteamLanguageCodeData("Swedish", "Svenska", "sv", null));
			m_steamLanguageCodeData.Add("turkish", new SteamLanguageCodeData("Turkish", "Türkçe", "tr", null));
			m_steamLanguageCodeData.Add("ukrainian", new SteamLanguageCodeData("Ukrainian", "Українська", "uk", null));
			m_steamLanguageCodeData.Add("schinese", new SteamLanguageCodeData("Chinese (Simplified)", "简体中文", "zh-CN", "zh-hans"));
			m_steamLanguageCodeData.Add("tchinese", new SteamLanguageCodeData("Chinese (Traditional)", "繁體中文", "zh-TW", "zh-hant"));
			m_steamLanguageCodeData.Add("czech", new SteamLanguageCodeData("Czech", "čeština", "cs", "de"));
			m_steamLanguageCodeData.Add("danish", new SteamLanguageCodeData("Danish", "Dansk", "da", "de"));
			m_steamLanguageCodeData.Add("dutch", new SteamLanguageCodeData("Dutch", "Nederlands", "nl", "de"));
			m_steamLanguageCodeData.Add("german", new SteamLanguageCodeData("German", "Deutsch", "de", "de"));
			m_steamLanguageCodeData.Add("english", new SteamLanguageCodeData("English", "English", "en", "en"));
			m_steamLanguageCodeData.Add("french", new SteamLanguageCodeData("French", "Français", "fr", "fr"));
			m_steamLanguageCodeData.Add("japanese", new SteamLanguageCodeData("Japanese", "日本語", "ja", "ja"));
			m_steamLanguageCodeData.Add("koreana", new SteamLanguageCodeData("Korean", "한국어", "ko", "ko"));
			m_steamLanguageCodeData.Add("thai", new SteamLanguageCodeData("Thai", "ไทย", "th", "th"));
			m_steamLanguageCodeData.Add("vietnamese", new SteamLanguageCodeData("Vietnamese", "Tiếng Việt", "vn", "vi"));
		}

		public string GetWebApiLanguageCode(string apiLanguageCode)
		{
			if (m_steamLanguageCodeData == null)
			{
				return "";
			}
			if (m_steamLanguageCodeData.TryGetValue(apiLanguageCode, out var value))
			{
				return value.webApiLanguageCode;
			}
			return "";
		}

		public string GetCurrentWebApiLanguageCode()
		{
			return GetWebApiLanguageCode(m_strGameLanguage);
		}

		public string GetCurrentCSLanguageCode()
		{
			if (m_steamLanguageCodeData == null)
			{
				return "";
			}
			if (m_steamLanguageCodeData.TryGetValue(m_strGameLanguage, out var value) && !string.IsNullOrEmpty(value.csLanguageCode))
			{
				return value.csLanguageCode;
			}
			return "";
		}

		public override NKM_NATIONAL_CODE GetDefaultLanguage()
		{
			Debug.Log("[LocalizationSteam] GetDefaultLanguage");
			NKM_NATIONAL_CODE result = NKM_NATIONAL_CODE.NNC_ENG;
			if (!SteamManager.Initialized)
			{
				NKM_NATIONAL_CODE nKM_NATIONAL_CODE = NKCGameOptionData.LoadLanguageCode(NKM_NATIONAL_CODE.NNC_END);
				if (nKM_NATIONAL_CODE != NKM_NATIONAL_CODE.NNC_END)
				{
					Debug.Log("LocalizationSteam - SteamManager not initialized. using saved lang code [" + nKM_NATIONAL_CODE.ToString() + "]");
					return nKM_NATIONAL_CODE;
				}
			}
			string currentCSLanguageCode = GetCurrentCSLanguageCode();
			Debug.Log("[LocalizationSteam] - SystemLanguageCode [" + currentCSLanguageCode + "]");
			if (string.IsNullOrWhiteSpace(currentCSLanguageCode))
			{
				Log.Debug("[LocalizationSteam] Can't Find GetDeviceLanguageCode : Set to Default Language [" + result.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 486);
				return result;
			}
			string langTagByLangCode = NKCLocalization.GetLangTagByLangCode(currentCSLanguageCode);
			Debug.Log("[LocalizationSteam] - LanguageTag [" + langTagByLangCode + "]");
			if (string.IsNullOrWhiteSpace(langTagByLangCode))
			{
				Log.Debug("[LocalizationSteam] Can't Find LangCode By [" + currentCSLanguageCode + "] : Set to Default Language [" + result.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 494);
				return result;
			}
			bool flag = false;
			TextAsset textAsset = Resources.Load<TextAsset>("LUA_DEFAULT_CONTENTS_TAG");
			if (textAsset != null)
			{
				Debug.Log("[LocalizationSteam] - patcherStringLua");
				string str = textAsset.ToString();
				using (NKMLua nKMLua = new NKMLua())
				{
					if (!nKMLua.DoString(str))
					{
						Log.Debug("[LocalizationSteam] Can't load DEFAULT_CONTENTS_TAG : Set to Default Language [" + result.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 509);
						return result;
					}
					List<string> list = new List<string>();
					if (!nKMLua.GetData(CountryTagType.GLOBAL.ToString(), list))
					{
						Log.Debug("[LocalizationSteam] Can't load Global taglist : Set to Default Language [" + result.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 516);
						return result;
					}
					foreach (string item in list)
					{
						if (item == langTagByLangCode)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					Debug.Log("[LocalizationSteam] - FoundLangTagInDefaultTags");
					if (NKCLocalization.s_dicLanguageTag.TryGetValue(langTagByLangCode, out var value))
					{
						Log.Debug("[LocalizationSteam] Selected Default Language : Set Language [" + value.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 535);
						return value;
					}
				}
			}
			Log.Debug("[LocalizationSteam] Couldn't set default language : Set to Default Language [" + result.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 541);
			return result;
		}
	}

	public class InAppSteam : NKCPMInAppPurchase
	{
		private CultureInfo m_cultureInfoForUSD;

		private CultureInfo m_cultureInfoForKRW;

		protected Callback<MicroTxnAuthorizationResponse_t> m_MicroTxnAuthorizationResponse;

		protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

		private string m_payload = "";

		private OnComplete m_onInappPurchaseComplete;

		private ShopItemTemplet m_requestedShopItemTemplet;

		private Coroutine m_initWaitCoroutine;

		private bool m_bOverlayActivated;

		private const float INAPP_INIT_TIME = 5f;

		private CultureInfo m_CultureInfoByOpenTag
		{
			get
			{
				if (!NKMOpenTagManager.IsOpened("STEAM_CURRENCY_KRW"))
				{
					return m_cultureInfoForUSD;
				}
				return m_cultureInfoForKRW;
			}
		}

		public override bool CheckReceivedBillingProductList => true;

		public override void Init()
		{
			if (m_MicroTxnAuthorizationResponse == null)
			{
				m_MicroTxnAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(OnMicroTxnAuthorizationResponse);
			}
			if (m_GameOverlayActivated == null)
			{
				m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
			}
			Log.Debug("[InAppSteam] Init", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 573);
			m_cultureInfoForUSD = GetCultureInfosByCurrencySymbol("USD");
			if (m_cultureInfoForUSD != null)
			{
				Debug.Log($"[InAppSteam] Name: {m_cultureInfoForUSD.Name}, LCID: {m_cultureInfoForUSD.LCID}, DisplayName: {m_cultureInfoForUSD.DisplayName}, NativeName: {m_cultureInfoForUSD.NativeName}");
			}
			else
			{
				Debug.LogError("[InAppSteam] Not found USD curtureInfo");
			}
			m_cultureInfoForKRW = GetCultureInfosByCurrencySymbol("KRW");
			if (m_cultureInfoForKRW != null)
			{
				Debug.Log($"[InAppSteam] Name: {m_cultureInfoForKRW.Name}, LCID: {m_cultureInfoForKRW.LCID}, DisplayName: {m_cultureInfoForKRW.DisplayName}, NativeName: {m_cultureInfoForKRW.NativeName}");
			}
			else
			{
				Debug.LogError("[InAppSteam] Not found KRW curtureInfo");
			}
			base.Init();
		}

		public override void RequestBillingProductList(OnComplete dOnComplete)
		{
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public override void BillingRestore(OnComplete dOnComplete)
		{
			RunFakeProcess(dOnComplete, "Billing Restored Fake", showPopup: false);
		}

		public override void InappPurchase(ShopItemTemplet shopTemplet, OnComplete dOnComplete, string metadata = "", List<int> lstSelection = null)
		{
			m_requestedShopItemTemplet = null;
			m_onInappPurchaseComplete = dOnComplete;
			if (shopTemplet == null)
			{
				dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_FAIL_BAD_ITEM);
				return;
			}
			m_requestedShopItemTemplet = shopTemplet;
			m_payload = null;
			if (lstSelection != null)
			{
				m_payload = NKCShopManager.EncodeCustomPackageSelectList(lstSelection);
			}
			NKMPopUpBox.OpenWaitBox();
			Send_NKMPacket_STEAM_BUY_INIT_REQ(shopTemplet);
		}

		public void Send_NKMPacket_STEAM_BUY_INIT_REQ(ShopItemTemplet shopItemTemplet)
		{
			NKMPacket_STEAM_BUY_INIT_REQ nKMPacket_STEAM_BUY_INIT_REQ = new NKMPacket_STEAM_BUY_INIT_REQ();
			nKMPacket_STEAM_BUY_INIT_REQ.steamId = NKCPublisherModule.Auth.GetPublisherAccountCode();
			nKMPacket_STEAM_BUY_INIT_REQ.productId = shopItemTemplet.m_ProductID;
			nKMPacket_STEAM_BUY_INIT_REQ.itemShopDesc = shopItemTemplet.GetItemName();
			nKMPacket_STEAM_BUY_INIT_REQ.language = ((LocalizationSteam)NKCPublisherModule.Localization).GetCurrentWebApiLanguageCode();
			nKMPacket_STEAM_BUY_INIT_REQ.country = ((LocalizationSteam)NKCPublisherModule.Localization).m_strCountry;
			Log.Debug($"[Steam][Inapp] NKMPacket_STEAM_BUY_INIT_REQ steamID[{nKMPacket_STEAM_BUY_INIT_REQ.steamId}] productID[{nKMPacket_STEAM_BUY_INIT_REQ.productId}] lang[{nKMPacket_STEAM_BUY_INIT_REQ.language}] country[{nKMPacket_STEAM_BUY_INIT_REQ.country}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 655);
			m_bOverlayActivated = false;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_STEAM_BUY_INIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public void OnRecv(NKMPacket_STEAM_BUY_INIT_ACK sPacket)
		{
			if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
			{
				m_requestedShopItemTemplet = null;
				m_onInappPurchaseComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_FAIL, sPacket.errorCode.ToString());
			}
			else
			{
				Debug.Log($"[Steam][Inapp] NKMPacket_STEAM_BUY_INIT_ACK product[{sPacket.productId}] orderID[{sPacket.orderId}]");
				NKMPopUpBox.OpenWaitBox();
				m_initWaitCoroutine = NKCPublisherModule.Instance.StartCoroutine(WaitForOverlay(5f));
			}
		}

		public IEnumerator WaitForOverlay(float fTime)
		{
			yield return new WaitForSeconds(fTime);
			NKMPopUpBox.CloseWaitBox();
			if (!m_bOverlayActivated)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_STEAM_ENABLE_OVERLAY));
			}
		}

		private void OnMicroTxnAuthorizationResponse(MicroTxnAuthorizationResponse_t pCallback)
		{
			Debug.Log("[Steam][Inapp][MicroTxn] OnMicroTxnAuthorizationResponse");
			NKMPopUpBox.CloseWaitBox();
			m_bOverlayActivated = true;
			if (pCallback.m_bAuthorized == 1)
			{
				Debug.Log($"[Steam][Inapp][MicroTxn] Authorized Payment - orderID[{pCallback.m_ulOrderID}]");
				Send_NKMPacket_STEAM_BUY_REQ(pCallback.m_ulOrderID);
			}
			else
			{
				Debug.Log($"[Steam][Inapp][MicroTxn] Failed to authorize payment - orderID[{pCallback.m_ulOrderID}]");
				m_onInappPurchaseComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_FAIL_USER_CANCEL);
			}
		}

		private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
		{
			if (m_initWaitCoroutine != null)
			{
				NKCPublisherModule.Instance.StopCoroutine(m_initWaitCoroutine);
			}
			NKMPopUpBox.CloseWaitBox();
			m_bOverlayActivated = true;
			if (pCallback.m_bActive != 0)
			{
				Log.Debug("[Steam][Overlay] Steam Overlay has been activated", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 726);
			}
			else
			{
				Log.Debug("[Steam][Overlay] Steam Overlay has been closed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 730);
			}
		}

		public void Send_NKMPacket_STEAM_BUY_REQ(ulong orderID)
		{
			Debug.Log($"[Steam][Inapp][MicroTxn] NKMPacket_STEAM_BUY_REQ - orderID[{orderID}]");
			List<int> selectIndices = NKCShopManager.DecodeCustomPackageSelectList(m_payload);
			NKMPacket_STEAM_BUY_REQ nKMPacket_STEAM_BUY_REQ = new NKMPacket_STEAM_BUY_REQ();
			nKMPacket_STEAM_BUY_REQ.steamId = NKCPublisherModule.Auth.GetPublisherAccountCode();
			nKMPacket_STEAM_BUY_REQ.orderId = orderID.ToString();
			nKMPacket_STEAM_BUY_REQ.productId = m_requestedShopItemTemplet.m_ProductID;
			nKMPacket_STEAM_BUY_REQ.country = ((LocalizationSteam)NKCPublisherModule.Localization).m_strCountry;
			nKMPacket_STEAM_BUY_REQ.selectIndices = selectIndices;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_STEAM_BUY_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			m_onInappPurchaseComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public override bool IsRegisteredProduct(string marketID, int productID)
		{
			if (NKCShopManager.GetShopTempletByMarketID(marketID) != null)
			{
				return true;
			}
			return false;
		}

		public override decimal GetLocalPrice(string marketID, int productID)
		{
			ShopItemTemplet shopTempletByMarketID = NKCShopManager.GetShopTempletByMarketID(marketID);
			if (shopTempletByMarketID != null)
			{
				return NKCScenManager.CurrentUserData().m_ShopData.GetRealPrice(shopTempletByMarketID, 1, useSteamPrice: true);
			}
			return 0m;
		}

		private CultureInfo GetCultureInfosByCurrencySymbol(string currencySymbol)
		{
			if (currencySymbol == null)
			{
				throw new ArgumentNullException("currencySymbol");
			}
			return CultureInfo.GetCultures(CultureTypes.SpecificCultures).FirstOrDefault((CultureInfo x) => new RegionInfo(x.LCID).ISOCurrencySymbol == currencySymbol);
		}

		public override string GetLocalPriceString(string marketID, int productID)
		{
			ShopItemTemplet shopTempletByMarketID = NKCShopManager.GetShopTempletByMarketID(marketID);
			if (shopTempletByMarketID == null)
			{
				Log.Debug("[InAppSteam] shopTemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 791);
				return "-";
			}
			if (m_CultureInfoByOpenTag == null)
			{
				Log.Debug("[InAppSteam] curtureInfo is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 797);
				return "-";
			}
			return ((float)NKCScenManager.CurrentUserData().m_ShopData.GetRealPrice(shopTempletByMarketID, 1, useSteamPrice: true) / 100f).ToString("c", m_CultureInfoByOpenTag);
		}

		public override List<int> GetInappProductIDs()
		{
			return new List<int>(NKCShopManager.GetMarketProductList().Keys);
		}

		public override void OpenPolicy(OnComplete dOnClose)
		{
			string text = NKCStringTable.GetString("SI_STEAM_URL_POLICY", bSkipErrorCheck: true);
			if (string.IsNullOrEmpty(text))
			{
				dOnClose?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
			}
			else
			{
				NKCPublisherModule.Notice.OpenURL(text, dOnClose);
			}
		}

		public override string GetCurrencyMark(int productID)
		{
			return m_CultureInfoByOpenTag.NumberFormat.CurrencySymbol;
		}

		public override void OpenPaymentLaw(OnComplete dOnClose)
		{
			string text = NKCStringTable.GetString("SI_STEAM_URL_PAYMENT_LAW", bSkipErrorCheck: true);
			if (string.IsNullOrEmpty(text))
			{
				dOnClose?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
			}
			else
			{
				NKCPublisherModule.Notice.OpenURL(text, dOnClose);
			}
		}

		public override void OpenCommercialLaw(OnComplete dOnClose)
		{
			string text = NKCStringTable.GetString("SI_STEAM_URL_COMMERCIAL_LAW", bSkipErrorCheck: true);
			if (string.IsNullOrEmpty(text))
			{
				dOnClose?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
			}
			else
			{
				NKCPublisherModule.Notice.OpenURL(text, dOnClose);
			}
		}

		public override bool ShowPurchasePolicy()
		{
			return false;
		}

		public override bool ShowPurchasePolicyBtn()
		{
			return false;
		}

		public override bool ShowCashResourceState()
		{
			if (NKCDefineManager.DEFINE_SELECT_SERVER())
			{
				return NKCGameOptionData.LoadLanguageCode(NKM_NATIONAL_CODE.NNC_JAPAN) == NKM_NATIONAL_CODE.NNC_JAPAN;
			}
			return false;
		}
	}

	public class NoticeSteam : NKCPMNotice
	{
		public override string NoticeUrl(bool bPatcher = false)
		{
			if (NKCDefineManager.DEFINE_SELECT_SERVER())
			{
				return NKCConnectionInfo.GetCurrentLoginServerString("SI_SYSTEM_WEB_NOTICE_URL", bSkipErrorCheck: true);
			}
			return NKCStringTable.GetString("SI_SYSTEM_WEB_NOTICE_URL", bSkipErrorCheck: true);
		}

		public override bool CheckOpenNoticeWhenFirstLoginSuccess()
		{
			if (NKCMain.m_ranAsSafeMode)
			{
				return true;
			}
			return false;
		}

		public override void OpenCommunity(OnComplete dOnComplete)
		{
			string text = "https://discord.gg/countersideglobal";
			if (NKCStringTable.CheckExistString("SI_STEAM_URL_COMMUNITY"))
			{
				text = NKCStringTable.GetString("SI_STEAM_URL_COMMUNITY", bSkipErrorCheck: true);
			}
			Debug.Log("[STEAM] OpenCommunity : " + text);
			NKMPopUpBox.CloseWaitBox();
			Application.OpenURL(text);
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public override void OpenCustomerCenter(OnComplete dOnComplete)
		{
			string text = "https://counterside.com/?page_id=1278";
			if (NKCStringTable.CheckExistString("SI_STEAM_URL_CUSTOMER_CENTER"))
			{
				text = NKCStringTable.GetString("SI_STEAM_URL_CUSTOMER_CENTER", bSkipErrorCheck: true);
			}
			Debug.Log("[STEAM] OpenCustomerCenter : " + text);
			NKMPopUpBox.CloseWaitBox();
			Application.OpenURL(text);
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
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
			return true;
		}

		public override void OpenNotice(OnComplete onComplete)
		{
			if (string.IsNullOrEmpty(NoticeUrl()) || NoticeUrl().Equals("SI_SYSTEM_WEB_NOTICE_URL"))
			{
				onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
			}
			else
			{
				NKCPublisherModule.Notice.OpenURL(NoticeUrl(), onComplete);
			}
		}

		public override void OpenPromotionalBanner(eOptionalBannerPlaces placeType, OnComplete dOnComplete)
		{
			RunFakeProcess(dOnComplete, "OptionalBanner : " + placeType, showPopup: true);
		}

		public override void NotifyMainenance(OnComplete dOnComplete)
		{
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public override void OpenURL(string url, OnComplete dOnComplete)
		{
			Debug.Log("[STEAM] OpenURL : " + url);
			NKMPopUpBox.CloseWaitBox();
			NKCPopupNoticeWeb.Instance.Open(url, dOnComplete);
		}
	}

	public class ServerInfoSteam : NKCPMServerInfo
	{
		public override bool GetUseLocalSaveLastServerInfoToGetTags()
		{
			return false;
		}

		public override string GetServerConfigPath()
		{
			string text = UnityEngine.Random.Range(1000000, 8000000).ToString();
			text += UnityEngine.Random.Range(1000000, 8000000);
			string text2 = "?p=" + text;
			string text3 = "https://ctsglobal-cdndown.sbside.com/server_config/live/";
			if (NKCDefineManager.DEFINE_GLOBALQA())
			{
				text3 = "http://FileServer.bside.com/server_config/Dev/";
				string customServerInfoAddress = NKCConnectionInfo.CustomServerInfoAddress;
				if (!string.IsNullOrEmpty(customServerInfoAddress))
				{
					text3 = customServerInfoAddress;
				}
			}
			string serverInfoFileName = NKCConnectionInfo.ServerInfoFileName;
			return text3 + serverInfoFileName + text2;
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

	protected override ePublisherType _PublisherType => ePublisherType.STEAM;

	protected override bool _Busy => false;

	protected override void OnTimeOut()
	{
	}

	private void OnDestroy()
	{
	}

	protected override NKCPMAuthentication MakeAuthInstance()
	{
		return new AuthSteam();
	}

	protected override NKCPMInAppPurchase MakeInappInstance()
	{
		return new InAppSteam();
	}

	protected override NKCPMNotice MakeNoticeInstance()
	{
		return new NoticeSteam();
	}

	protected override NKCPMServerInfo MakeServerInfoInstance()
	{
		return new ServerInfoSteam();
	}

	protected override NKCPMStatistics MakeStatisticsInstance()
	{
		return new StatisticsNone();
	}

	protected override NKCPMLocalization MakeLocalizationInstance()
	{
		return new LocalizationSteam();
	}

	private void Start()
	{
		if ((UnityEngine.Object)null == (UnityEngine.Object)null)
		{
			Log.Debug("[SteamLogin] NKCPMSteamPC - Add SteamManager", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 81);
			new GameObject("SteamManager").AddComponent<SteamManager>();
		}
	}

	protected override void _Init(OnComplete dOnComplete)
	{
		NKCPublisherModule.InitState = ePublisherInitState.Initialized;
		Log.Debug($"[SteamPC][_Init] SteamManager[{SteamManager.Initialized}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPMSteamPC.cs", 95);
		if (!SteamManager.Initialized)
		{
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_STEAM_INITIALIZE_FAIL);
		}
		else
		{
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}
	}

	private static void RunFakeProcess(OnComplete dOnComplete, string fakeMessage, bool showPopup)
	{
		Debug.Log(fakeMessage);
		dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
	}
}
