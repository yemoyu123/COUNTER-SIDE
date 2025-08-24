using System;
using System.Collections;
using System.Collections.Generic;
using ClientPacket.Account;
using ClientPacket.WorldMap;
using Cs.Logging;
using Cs.Protocol;
using NKC.UI;
using NKM;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;

namespace NKC.Publisher;

public abstract class NKCPublisherModule : MonoBehaviour
{
	public abstract class NKCPMPush
	{
		protected struct LocalPushData
		{
			public NKC_GAME_OPTION_ALARM_GROUP eventType;

			public DateTime reserveTime;

			public LocalPushData(NKC_GAME_OPTION_ALARM_GROUP type, long timeTick)
			{
				eventType = type;
				reserveTime = new DateTime(timeTick);
			}

			public LocalPushData(NKC_GAME_OPTION_ALARM_GROUP type, DateTime time)
			{
				eventType = type;
				reserveTime = time;
			}
		}

		protected const string LOCAL_PUSH = "LOCAL_PUSH";

		public abstract void Init();

		public virtual void ReRegisterPush()
		{
		}

		protected void UpdateLocalPush(NKC_GAME_OPTION_ALARM_GROUP type, long timeTick)
		{
			if (ReserveLocalPush(timeTick, type))
			{
				RegisterNotifiyTime(type, timeTick);
			}
		}

		protected abstract bool ReserveLocalPush(DateTime newUtcTime, NKC_GAME_OPTION_ALARM_GROUP evtType);

		protected abstract void CancelLocalPush(NKC_GAME_OPTION_ALARM_GROUP evtType);

		protected virtual void ClearAllLocalPush()
		{
			foreach (NKC_GAME_OPTION_ALARM_GROUP value in Enum.GetValues(typeof(NKC_GAME_OPTION_ALARM_GROUP)))
			{
				if (IsValidType(value))
				{
					CancelLocalPush(value);
					ClearRegistedNotifyTime(value);
				}
			}
		}

		protected bool ReserveLocalPush(long newTimeTick, NKC_GAME_OPTION_ALARM_GROUP evtType)
		{
			return ReserveLocalPush(new DateTime(newTimeTick), evtType);
		}

		public void SetAlarm(NKC_GAME_OPTION_ALARM_GROUP alarmGroup, bool allow)
		{
			if (IsValidType(alarmGroup))
			{
				SetLocalPush(alarmGroup, allow);
				if (alarmGroup != NKC_GAME_OPTION_ALARM_GROUP.ALLOW_ALL_ALARM)
				{
					UpdateAllAlarmButton();
				}
			}
			if (alarmGroup == NKC_GAME_OPTION_ALARM_GROUP.ALLOW_ALL_ALARM)
			{
				if (allow)
				{
					UpdateLocalPush(NKC_GAME_OPTION_ALARM_GROUP.LONG_TERM_NOT_CONNECTED_NOTIFY, bForce: true);
				}
				else
				{
					ClearLocalPush(NKC_GAME_OPTION_ALARM_GROUP.LONG_TERM_NOT_CONNECTED_NOTIFY);
				}
			}
		}

		private void UpdateAllAlarmButton()
		{
			NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
			if (gameOptionData == null)
			{
				return;
			}
			bool flag = gameOptionData.GetAllowAlarm(NKC_GAME_OPTION_ALARM_GROUP.ALLOW_ALL_ALARM);
			bool flag2 = true;
			foreach (NKC_GAME_OPTION_ALARM_GROUP value in Enum.GetValues(typeof(NKC_GAME_OPTION_ALARM_GROUP)))
			{
				if (!IsValidType(value))
				{
					continue;
				}
				if (flag)
				{
					flag2 = false;
					if (!gameOptionData.GetAllowAlarm(value))
					{
						flag = false;
						break;
					}
				}
				else
				{
					flag2 &= gameOptionData.GetAllowAlarm(value);
				}
			}
			if (flag2)
			{
				flag = true;
			}
			gameOptionData.SetAllowAlarm(NKC_GAME_OPTION_ALARM_GROUP.ALLOW_ALL_ALARM, flag);
		}

		private void SetLocalPush(NKC_GAME_OPTION_ALARM_GROUP type, bool bActive)
		{
			if (IsValidType(type))
			{
				if (bActive)
				{
					UpdateLocalPush(type);
					return;
				}
				ClearRegistedNotifyTime(type);
				CancelLocalPush(type);
			}
		}

		private long GetCompleteTime(NKC_GAME_OPTION_ALARM_GROUP type)
		{
			switch (type)
			{
			case NKC_GAME_OPTION_ALARM_GROUP.RESOURCE_SUPPLY_COMPLETE:
				return GetCompleteTimeAutoSupply();
			case NKC_GAME_OPTION_ALARM_GROUP.WORLD_MAP_MISSION_COMPLETE:
				return GetCompleteTimeWorldMission();
			case NKC_GAME_OPTION_ALARM_GROUP.CRAFT_COMPLETE:
				return GetCompleteTimeCraft();
			case NKC_GAME_OPTION_ALARM_GROUP.PVP_POINT_COMPLETE:
				return GetCompleteTimePVPPoint();
			case NKC_GAME_OPTION_ALARM_GROUP.LONG_TERM_NOT_CONNECTED_NOTIFY:
				return GetNotifyTimeLongTermNotConnected();
			case NKC_GAME_OPTION_ALARM_GROUP.GUILD_DUNGEON_NOTIFY:
				if (NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_DUNGEON))
				{
					return GetNextGuildDungeonStateChangeTime();
				}
				break;
			}
			return 0L;
		}

		private long GetCompleteTimeAutoSupply(bool bForce = false)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			NKMUserExpTemplet userExpTemplet = NKCExpManager.GetUserExpTemplet(nKMUserData);
			if (userExpTemplet == null)
			{
				return 0L;
			}
			long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(2);
			if (userExpTemplet.m_EterniumCap <= countMiscItem || NKCScenManager.CurrentUserData().lastEterniumUpdateDate == DateTime.MinValue)
			{
				return 0L;
			}
			int num = (int)((float)(userExpTemplet.m_EterniumCap - countMiscItem) / (float)userExpTemplet.m_RechargeEternium + 0.5f);
			return NKCSynchronizedTime.ToUtcTime(NKCScenManager.CurrentUserData().lastEterniumUpdateDate.AddTicks(NKMCommonConst.RECHARGE_TIME.Ticks * num)).Ticks;
		}

		private long GetCompleteTimeWorldMission()
		{
			long num = 0L;
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				foreach (KeyValuePair<int, NKMWorldMapCityData> item in nKMUserData.m_WorldmapData.worldMapCityDataMap)
				{
					NKMWorldMapCityData value = item.Value;
					if (value != null && value.HasMission() && !value.IsMissionFinished(NKCSynchronizedTime.GetServerUTCTime()) && num < value.worldMapMission.completeTime)
					{
						num = value.worldMapMission.completeTime;
					}
				}
			}
			return num;
		}

		private long GetCompleteTimeCraft()
		{
			long num = 0L;
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				foreach (KeyValuePair<byte, NKMCraftSlotData> slot in nKMUserData.m_CraftData.SlotList)
				{
					if (slot.Value.CompleteDate > num)
					{
						num = slot.Value.CompleteDate;
					}
				}
			}
			return num;
		}

		private long GetCompleteTimePVPPoint()
		{
			long result = 0L;
			long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(6);
			int cHARGE_POINT_MAX_COUNT = NKMPvpCommonConst.Instance.CHARGE_POINT_MAX_COUNT;
			if (countMiscItem < cHARGE_POINT_MAX_COUNT)
			{
				int num = cHARGE_POINT_MAX_COUNT - (int)countMiscItem;
				int num2 = num / NKMPvpCommonConst.Instance.CHARGE_POINT_ONE_STEP;
				if (num - num2 * NKMPvpCommonConst.Instance.CHARGE_POINT_ONE_STEP > 0)
				{
					num2++;
				}
				result = new DateTime(new DateTime(NKCPVPManager.GetLastUpdateChargePointTicks()).Ticks + NKMPvpCommonConst.Instance.CHARGE_POINT_REFRESH_INTERVAL_TICKS * num2).Ticks;
			}
			return result;
		}

		private long GetNotifyTimeLongTermNotConnected()
		{
			return NKCSynchronizedTime.GetServerUTCTime().AddDays(10.0).Ticks;
		}

		private long GetNextGuildDungeonStateChangeTime()
		{
			return NKCGuildCoopManager.m_NextSessionStartDateUTC.Ticks;
		}

		protected bool GetRegistedNotifyTime(NKC_GAME_OPTION_ALARM_GROUP type, out LocalPushData pushData)
		{
			pushData = default(LocalPushData);
			string key = string.Format("{0}_{1}_{2}", "LOCAL_PUSH", type, NKCScenManager.CurrentUserData().m_UserUID);
			if (PlayerPrefs.HasKey(key))
			{
				string s = PlayerPrefs.GetString(key);
				long result = 0L;
				if (!long.TryParse(s, out result))
				{
					pushData.reserveTime = new DateTime(0L);
					return false;
				}
				pushData.reserveTime = new DateTime(result);
				pushData.eventType = type;
				return true;
			}
			pushData.reserveTime = new DateTime(0L);
			return false;
		}

		protected void RegisterNotifiyTime(NKC_GAME_OPTION_ALARM_GROUP type, long timeTick)
		{
			PlayerPrefs.SetString(string.Format("{0}_{1}_{2}", "LOCAL_PUSH", type, NKCScenManager.CurrentUserData().m_UserUID), timeTick.ToString());
		}

		protected void ClearRegistedNotifyTime(NKC_GAME_OPTION_ALARM_GROUP type)
		{
			PlayerPrefs.DeleteKey(string.Format("{0}_{1}_{2}", "LOCAL_PUSH", type, NKCScenManager.CurrentUserData().m_UserUID));
		}

		protected List<LocalPushData> GetAllRegisteredPush()
		{
			List<LocalPushData> list = new List<LocalPushData>();
			foreach (NKC_GAME_OPTION_ALARM_GROUP value in Enum.GetValues(typeof(NKC_GAME_OPTION_ALARM_GROUP)))
			{
				if (GetRegistedNotifyTime(value, out var pushData) && !NKCSynchronizedTime.IsFinished(pushData.reserveTime))
				{
					list.Add(pushData);
				}
			}
			return list;
		}

		public void UpdateAllLocalPush()
		{
			NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
			if (gameOptionData == null)
			{
				return;
			}
			foreach (NKC_GAME_OPTION_ALARM_GROUP value in Enum.GetValues(typeof(NKC_GAME_OPTION_ALARM_GROUP)))
			{
				if (IsValidType(value))
				{
					SetLocalPush(value, gameOptionData.GetAllowAlarm(value));
				}
			}
		}

		public void UpdateAllLocalPush(bool bVal)
		{
			NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
			if (gameOptionData == null)
			{
				return;
			}
			foreach (NKC_GAME_OPTION_ALARM_GROUP value in Enum.GetValues(typeof(NKC_GAME_OPTION_ALARM_GROUP)))
			{
				if (IsValidType(value))
				{
					gameOptionData.SetAllowAlarm(value, bVal);
				}
			}
		}

		private bool IsPossiblePush(NKC_GAME_OPTION_ALARM_GROUP type)
		{
			if (type == NKC_GAME_OPTION_ALARM_GROUP.LONG_TERM_NOT_CONNECTED_NOTIFY)
			{
				return true;
			}
			return NKCScenManager.GetScenManager().GetGameOptionData()?.GetAllowAlarm(type) ?? false;
		}

		private bool IsValidType(NKC_GAME_OPTION_ALARM_GROUP type)
		{
			if (type == NKC_GAME_OPTION_ALARM_GROUP.CRAFT_COMPLETE || type == NKC_GAME_OPTION_ALARM_GROUP.PVP_POINT_COMPLETE || type == NKC_GAME_OPTION_ALARM_GROUP.WORLD_MAP_MISSION_COMPLETE || type == NKC_GAME_OPTION_ALARM_GROUP.LONG_TERM_NOT_CONNECTED_NOTIFY || type == NKC_GAME_OPTION_ALARM_GROUP.RESOURCE_SUPPLY_COMPLETE)
			{
				return true;
			}
			return false;
		}

		public void UpdateLocalPush(NKC_GAME_OPTION_ALARM_GROUP type, bool bForce = false)
		{
			if (IsValidType(type) && IsPossiblePush(type))
			{
				long num = 0L;
				if (GetRegistedNotifyTime(type, out var pushData))
				{
					num = pushData.reserveTime.Ticks;
				}
				long completeTime = GetCompleteTime(type);
				if (completeTime == 0L)
				{
					ClearRegistedNotifyTime(type);
					CancelLocalPush(type);
				}
				else if (bForce || num < completeTime)
				{
					Debug.Log($"[로컬푸시 등록] Type : {type} Time : {new DateTime(num)}, CurcompleteTime : {new DateTime(completeTime)}");
					UpdateLocalPush(type, completeTime);
				}
			}
		}

		protected string GetLocalPushText(NKC_GAME_OPTION_ALARM_GROUP evtType)
		{
			return evtType switch
			{
				NKC_GAME_OPTION_ALARM_GROUP.CRAFT_COMPLETE => NKCUtilString.GET_STRING_TOY_LOCAL_PUSH_GEAR_CRAFT_DESCRIPTION, 
				NKC_GAME_OPTION_ALARM_GROUP.RESOURCE_SUPPLY_COMPLETE => NKCUtilString.GET_STRING_TOY_LOCAL_PUSH_AUTO_SUPPLY_DESCRIPTION, 
				NKC_GAME_OPTION_ALARM_GROUP.WORLD_MAP_MISSION_COMPLETE => NKCUtilString.GET_STRING_TOY_LOCAL_PUSH_WORLD_MAP_DESCRIPTION, 
				NKC_GAME_OPTION_ALARM_GROUP.PVP_POINT_COMPLETE => NKCUtilString.GET_STRING_TOY_LOCAL_PUSH_PVP_POINT_DESCRIPTION, 
				NKC_GAME_OPTION_ALARM_GROUP.LONG_TERM_NOT_CONNECTED_NOTIFY => NKCUtilString.GET_STRING_TOY_LOCAL_PUSH_NOT_CONNECTED_DESCRIPTION, 
				_ => "", 
			};
		}

		protected string GetLocalPushTitle(NKC_GAME_OPTION_ALARM_GROUP evtType)
		{
			return evtType switch
			{
				NKC_GAME_OPTION_ALARM_GROUP.CRAFT_COMPLETE => NKCUtilString.GET_STRING_TOY_LOCAL_PUSH_GEAR_CRAFT_TITLE, 
				NKC_GAME_OPTION_ALARM_GROUP.RESOURCE_SUPPLY_COMPLETE => NKCUtilString.GET_STRING_TOY_LOCAL_PUSH_AUTO_SUPPLY_TITLE, 
				NKC_GAME_OPTION_ALARM_GROUP.WORLD_MAP_MISSION_COMPLETE => NKCUtilString.GET_STRING_TOY_LOCAL_PUSH_WORLD_MAP_TITLE, 
				NKC_GAME_OPTION_ALARM_GROUP.PVP_POINT_COMPLETE => NKCUtilString.GET_STRING_TOY_LOCAL_PUSH_PVP_POINT_TITLE, 
				NKC_GAME_OPTION_ALARM_GROUP.LONG_TERM_NOT_CONNECTED_NOTIFY => NKCUtilString.GET_STRING_TOY_LOCAL_PUSH_NOT_CONNECTED_TITLE, 
				_ => "", 
			};
		}

		private void ClearLocalPush(NKC_GAME_OPTION_ALARM_GROUP type)
		{
			if (IsValidType(type))
			{
				CancelLocalPush(type);
				ClearRegistedNotifyTime(type);
			}
		}
	}

	public enum SNS_SHARE_TYPE
	{
		SST_NONE = -1,
		SST_WECHAT = 1,
		SST_WECHAT_MOMENTS = 2,
		SST_WEIBO = 3,
		SST_FACEBOOK = 4,
		SST_QQ = 5
	}

	public delegate void OnComplete(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError = null);

	public enum ePublisherType
	{
		None,
		NexonToy,
		Zlong,
		NexonPC,
		SB_Gamebase,
		NexonToyJP,
		JPPC,
		STEAM
	}

	public enum ePublisherInitState
	{
		NotInitialized,
		Maintanance,
		Initialized
	}

	public abstract class NKCPMAuthentication
	{
		public enum LOGIN_IDP_TYPE
		{
			none,
			guest,
			google,
			facebook,
			twitter,
			appleid
		}

		public virtual bool LogoutReservedAfterGame { get; set; }

		public abstract bool LoginToPublisherCompleted { get; }

		public virtual bool Init()
		{
			return true;
		}

		public abstract string GetPublisherAccountCode();

		public virtual void LogoutReserved()
		{
			LogoutReservedAfterGame = false;
			Logout(null);
		}

		public virtual LOGIN_IDP_TYPE GetLoginIdpType()
		{
			return LOGIN_IDP_TYPE.guest;
		}

		public abstract void LoginToPublisher(OnComplete dOnComplete);

		public abstract void PrepareCSLogin(OnComplete dOnComplete);

		public abstract ISerializable MakeLoginServerLoginReqPacket();

		public abstract ISerializable MakeGameServerLoginReqPacket(string accesstoken);

		public abstract void ChangeAccount(OnComplete dOnComplete, bool syncAccount);

		public abstract void Logout(OnComplete dOnComplete);

		public virtual bool IsTryAuthWhenSessionExpired()
		{
			return false;
		}

		public virtual void TryRestoreQuitUser(OnComplete dOnComplete)
		{
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_LOGIN_FAIL_QUIT_USER_RESTORE);
		}

		public virtual void TryResolveUser(OnComplete onComplete)
		{
			onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_LOGIN_FAIL_USER_RESOLVE);
		}

		public virtual void ResetConnection()
		{
		}

		public virtual void OnReconnectFail(OnComplete dOnComplete)
		{
		}

		public virtual bool CheckExitCallFirst()
		{
			return false;
		}

		public virtual void Exit(OnComplete onComplete)
		{
			onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public virtual bool CheckNeedToCheckEnableQR_AfterPubLogin()
		{
			return false;
		}

		public virtual bool CheckEnableQR_Login()
		{
			return false;
		}

		public virtual void QR_Login(OnComplete onComplete)
		{
			onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public virtual void LoginToPublisherBy(string providerName, OnComplete onComplete)
		{
			onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public virtual void AddMapping(string providerName, OnComplete onComplete)
		{
			Debug.Log("AddMapping : " + providerName);
			onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public virtual void RemoveMapping(string providerName)
		{
			Debug.Log("RemoveMapping : " + providerName);
		}

		public virtual void Withdraw(OnComplete onComplete)
		{
			Debug.Log("Withdraw");
			onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public virtual void TemporaryWithdrawal(OnComplete onComplete)
		{
			Debug.Log("TemporaryWithdrawal");
			onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public virtual bool IsGuest()
		{
			return false;
		}

		public virtual bool OnLoginSuccessToCS()
		{
			return true;
		}

		public virtual void OpenCertification()
		{
		}

		public virtual string GetAccountLinkText()
		{
			return "";
		}

		public virtual ISerializable MakeWithdrawReqPacket()
		{
			return null;
		}
	}

	public abstract class NKCPMInAppPurchase
	{
		public class BillingRestoreInfo
		{
			public int ProductId;

			public readonly string MarketId;

			public readonly string Token;

			public readonly string State;

			private NKM_ERROR_CODE _errorCode;

			public BillingRestoreInfo(string marketId, string token, string state)
			{
				MarketId = marketId;
				Token = token;
				State = state;
			}

			public void SetServerError(NKM_ERROR_CODE errorCode)
			{
				_errorCode = errorCode;
			}

			public void SetError(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
			{
				if (resultCode != NKC_PUBLISHER_RESULT_CODE.NPRC_OK)
				{
					CheckError(resultCode, additionalError, bCloseWaitBox: false, null, popupMessage: true);
				}
			}
		}

		private readonly Dictionary<string, BillingRestoreInfo> m_billingRestoredDic = new Dictionary<string, BillingRestoreInfo>();

		public abstract bool CheckReceivedBillingProductList { get; }

		public virtual void Init()
		{
		}

		public abstract void RequestBillingProductList(OnComplete dOnComplete);

		public abstract bool IsRegisteredProduct(string marketID, int productID);

		public abstract string GetLocalPriceString(string marketID, int productID);

		public abstract decimal GetLocalPrice(string marketID, int productID);

		public virtual string GetPriceCurrency(string marketID, int productID)
		{
			return "";
		}

		public abstract void InappPurchase(ShopItemTemplet shopTemplet, OnComplete dOnComplete, string metadata = "", List<int> lstSelection = null);

		public abstract void BillingRestore(OnComplete dOnComplete);

		public virtual bool IsBillingRestoreActive()
		{
			return false;
		}

		public abstract List<int> GetInappProductIDs();

		public virtual void OpenPolicy(OnComplete dOnClose)
		{
			dOnClose?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public virtual string GetCurrencyMark(int productID)
		{
			return "￦";
		}

		public virtual bool ShowJPNPaymentPolicy()
		{
			NKCDefineManager.DEFINE_NX_PC();
			return false;
		}

		public virtual bool IsJPNPaymentPolicy()
		{
			return false;
		}

		public virtual bool ShowCashResourceState()
		{
			return false;
		}

		public virtual void OpenPaymentLaw(OnComplete dOnClose)
		{
			dOnClose?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public virtual void OpenCommercialLaw(OnComplete dOnClose)
		{
			dOnClose?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public virtual bool ShowPurchasePolicy()
		{
			return false;
		}

		public virtual bool ShowPurchasePolicyBtn()
		{
			return false;
		}

		protected bool AddBillingRestoreInfo(string stampId, string marketID, string token, string state)
		{
			if (m_billingRestoredDic.ContainsKey(stampId))
			{
				Log.Warn("[BillingRestoreInfo] Already Exist", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPublisherModule.cs", 750);
				return false;
			}
			BillingRestoreInfo billingRestoreInfo = new BillingRestoreInfo(marketID, token, state);
			ShopItemTemplet shopTempletByMarketID = NKCShopManager.GetShopTempletByMarketID(marketID);
			if (shopTempletByMarketID == null)
			{
				billingRestoreInfo.SetError(NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_FAIL_INVALID_SHOPTEMPLET, marketID);
				Log.Warn("[BillingRestoreInfo] 템플릿에 해당 상품 찾지 못함 : " + marketID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPublisherModule.cs", 760);
				m_billingRestoredDic.Add(stampId, billingRestoreInfo);
				return false;
			}
			billingRestoreInfo.ProductId = shopTempletByMarketID.m_ProductID;
			if (!IsRegisteredProduct(marketID, billingRestoreInfo.ProductId))
			{
				billingRestoreInfo.SetError(NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_FAIL_INVALID_TOYPRODUCT, marketID);
				Log.Warn($"[BillingRestoreInfo] 토이 상품리스트에서 상품 찾지 못함 : {marketID}, {shopTempletByMarketID.m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPublisherModule.cs", 770);
				m_billingRestoredDic.Add(stampId, billingRestoreInfo);
				return false;
			}
			m_billingRestoredDic.Add(stampId, billingRestoreInfo);
			return true;
		}
	}

	public struct FunnelLogData
	{
		public string funnelName;
	}

	public abstract class NKCPMStatistics
	{
		public enum eClientAction
		{
			AppStart,
			Patch_TagGetFailed,
			Patch_TagProvided,
			Patch_VersionCheckComplete,
			Patch_DownloadAvailable,
			Patch_DownloadStart,
			Patch_DownloadComplete,
			Patch_MoveToMainScene,
			TryLoginToGameServer,
			Login_ShowNotice,
			DungeonGameClear,
			WarfareGameClear,
			PvPGameFinished,
			Lobby_ShowNotice,
			PlayerNameChanged,
			AfterPublisherLogin,
			AfterSyncAccountComplete,
			MissionClear
		}

		public string GetDeviceID()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}

		public virtual void LogClientAction(eClientAction funnelPosition, int key = 0, string data = null)
		{
			switch (funnelPosition)
			{
			case eClientAction.AfterSyncAccountComplete:
				NKCMMPManager.OnCustomEvent("05_loading_complete");
				break;
			case eClientAction.DungeonGameClear:
				NKCMMPManager.OnClearDungeon(key);
				break;
			case eClientAction.WarfareGameClear:
				NKCMMPManager.OnWarfareResult(key);
				break;
			case eClientAction.MissionClear:
				NKCMMPManager.OnCustomEvent(data);
				break;
			}
			LogClientActionForPublisher(funnelPosition, key, data);
		}

		public abstract void LogClientActionForPublisher(eClientAction funnelPosition, int key, string data);

		public virtual void TrackPurchase(int itemID)
		{
		}

		public virtual void OnLoginSuccessToCS(NKMPacket_JOIN_LOBBY_ACK res)
		{
		}

		public virtual void OnFirstEpisodeClear()
		{
			Log.Debug("OnFirstEpisodeClear!!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Publisher/NKCPublisherModule.cs", 886);
		}

		public virtual void OnUserLevelUp(int newUserLevel)
		{
		}

		public virtual void OnPatchStart()
		{
		}

		public virtual void OnPatchEnd()
		{
		}

		public virtual void OnResetAgreement()
		{
		}
	}

	public abstract class NKCPMNotice
	{
		public enum eOptionalBannerPlaces
		{
			AppLaunch,
			EnterLobby,
			EP1Start,
			EP2Act2Clear,
			EP2Clear,
			maintenance,
			ep1act4clear
		}

		public virtual string NoticeUrl(bool bPatcher = false)
		{
			return "";
		}

		public virtual bool CheckOpenNoticeWhenFirstLoginSuccess()
		{
			return false;
		}

		public virtual bool CheckOpenNoticeWhenFirstLobbyVisit()
		{
			return true;
		}

		public abstract void OpenNotice(OnComplete dOnComplete);

		public abstract void NotifyMainenance(OnComplete dOnComplete);

		public virtual void OpenCustomerCenter(OnComplete dOnComplete)
		{
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public virtual bool IsActiveCustomerCenter()
		{
			return true;
		}

		public virtual void OpenQnA(OnComplete dOnComplete)
		{
		}

		public virtual void OpenCommunity(OnComplete dOnComplete)
		{
			if (NKMContentsVersionManager.HasCountryTag(CountryTagType.TWN))
			{
				Application.OpenURL("https://www.facebook.com/gamebeansFW");
			}
			else if (NKMContentsVersionManager.HasCountryTag(CountryTagType.SEA))
			{
				Application.OpenURL("https://www.facebook.com/CounterSide-113060560534111");
			}
			else if (IsNexonPCBuild())
			{
				if (NKMContentsVersionManager.HasCountryTag(CountryTagType.KOR))
				{
					Application.OpenURL("https://forum.nexon.com/counterside/main");
				}
				else
				{
					Notice.OpenNotice(null);
				}
			}
			else
			{
				Notice.OpenNotice(null);
			}
		}

		public virtual void OpenSurvey(long surveyID, OnComplete onComplete)
		{
		}

		public virtual void OpenURL(string url, OnComplete dOnComplete)
		{
			Debug.Log("OpenURL : " + url);
			Application.OpenURL(url);
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public virtual void OpenPromotionalBanner(eOptionalBannerPlaces placeType, OnComplete dOnComplete)
		{
			Debug.Log("OpenPromotionalBanner " + placeType);
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public virtual void OpenInfoWindow(OnComplete dOnComplete)
		{
			Debug.Log("OpenInfoWindow");
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public virtual void OpenAgreement(OnComplete dOnComplete)
		{
			Debug.Log("OpenAgreement");
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}

		public virtual void OpenPrivacyPolicy(OnComplete dOnComplete)
		{
			Debug.Log("OpenPrivacyPolicy");
			dOnComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}
	}

	public abstract class NKCPMMarketing
	{
		protected bool marketReviewCompleted;

		protected string m_reviewReason;

		private UnityAction dOnCloseMarketReview;

		public virtual bool IsOfferwallAvailable => false;

		public virtual bool MarketReviewEnabled
		{
			get
			{
				if (marketReviewCompleted)
				{
					return false;
				}
				if (NKCDefineManager.DEFINE_UNITY_STANDALONE())
				{
					return false;
				}
				return true;
			}
		}

		public virtual void Init()
		{
		}

		public virtual bool IsEnableWechatFollowEvent()
		{
			return false;
		}

		public virtual string MakeWechatFollowCode(int activityInstanceId)
		{
			return "";
		}

		public virtual bool IsCouponEnabled()
		{
			if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.INTERNAL_COUPON_SYSTEM))
			{
				return true;
			}
			return false;
		}

		public virtual bool IsUseSelfCouponPopup()
		{
			if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.INTERNAL_COUPON_SYSTEM))
			{
				return false;
			}
			return true;
		}

		public virtual void OpenCoupon()
		{
			Debug.Log("OpenCoupon");
		}

		public virtual void SendUseCouponReqToCSServer(string code)
		{
			if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.INTERNAL_COUPON_SYSTEM))
			{
				NKCPacketSender.Send_NKMPacket_BSIDE_COUPON_USE_REQ(code);
			}
		}

		public virtual void ShowOfferWall()
		{
		}

		public void SetMarketReviewCompleted()
		{
			marketReviewCompleted = true;
		}

		public virtual void OpenMarketReviewPopup(string reviewLog, UnityAction onClose)
		{
			Debug.Log("MarketReviewPopup");
			m_reviewReason = reviewLog;
			dOnCloseMarketReview = onClose;
			if (NKCDefineManager.DEFINE_IOS())
			{
				MoveToReview();
				dOnCloseMarketReview?.Invoke();
			}
			else if (NKCDefineManager.DEFINE_ANDROID())
			{
				NKCPopupReviewInduce.Instance.OpenOKCancel(delegate
				{
					MoveToReview();
					dOnCloseMarketReview?.Invoke();
				}, delegate
				{
					dOnCloseMarketReview?.Invoke();
				});
			}
			else
			{
				onClose?.Invoke();
			}
		}

		private void MoveToReview()
		{
			Debug.Log("MoveToMarket");
			if (NKCDefineManager.DEFINE_ANDROID())
			{
				Application.OpenURL("market://details?id=" + Application.identifier);
			}
			else
			{
				NKCDefineManager.DEFINE_IOS();
			}
			NKCPacketSender.Send_NKMPacket_UPDATE_MARKET_REVIEW_REQ(m_reviewReason);
		}

		public virtual bool SnsShareEnabled(NKMUnitData unitData)
		{
			return false;
		}

		public virtual bool IsUseSnsSharePopup()
		{
			return false;
		}

		public virtual bool IsOnlyUnitShare()
		{
			return true;
		}

		public virtual bool IsUseSnsShareOn10SeqContract()
		{
			return false;
		}

		public virtual bool SnsQRImageEnabled()
		{
			return true;
		}

		public virtual void TrySnsShare(SNS_SHARE_TYPE sst, string capturePath, string thumbnailPath, OnComplete onComplete)
		{
			Debug.Log("TrySnsShare : " + capturePath);
			onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		}
	}

	public abstract class NKCPMPermission
	{
		public virtual void Init()
		{
		}

		public abstract void RequestCameraPermission(OnComplete dOnComplete);

		public virtual void CheckCameraPermission()
		{
		}

		public virtual void RequestAppTrackingPermission()
		{
		}

		public virtual void RequestTerm(OnComplete dOnComplete)
		{
		}
	}

	public abstract class NKCPMServerInfo
	{
		public virtual bool IsUsePatchConnectionInfo()
		{
			return true;
		}

		public virtual string GetServerConfigPath()
		{
			return "";
		}

		public virtual bool GetUseLocalSaveLastServerInfoToGetTags()
		{
			return true;
		}
	}

	public abstract class NKCPMLocalization
	{
		public delegate void TranslateCallback(NKC_PUBLISHER_RESULT_CODE resultCode, string translatedString, long chatUID, string additionalError);

		private Dictionary<long, TranslateCallback> m_requestedTranslationList = new Dictionary<long, TranslateCallback>();

		public virtual bool UseDefaultLanguageOnFirstRun => false;

		public virtual bool UseTranslation => NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.USE_CHAT_TRANSLATION);

		public virtual void Init()
		{
		}

		public abstract NKM_NATIONAL_CODE GetDefaultLanguage();

		public virtual NKC_VOICE_CODE GetDefaultVoice()
		{
			return NKC_VOICE_CODE.NVC_KOR;
		}

		public virtual void SetPublisherModuleLanguage(NKM_NATIONAL_CODE code)
		{
		}

		public virtual bool IsPossibleJson(string inputStr)
		{
			return false;
		}

		public virtual string GetTranslationIfJson(string origin)
		{
			return origin;
		}

		public virtual void Translate(long chatUID, string str, NKM_NATIONAL_CODE userLanguage, TranslateCallback callback)
		{
			if (callback != null && m_requestedTranslationList != null)
			{
				if (!m_requestedTranslationList.ContainsKey(chatUID))
				{
					m_requestedTranslationList.Add(chatUID, callback);
				}
				NKCPacketSender.Send_NKMPacket_GUILD_CHAT_TRANSLATE_REQ(NKCGuildManager.MyGuildData.guildUid, chatUID, NKCStringTable.GetLanguageCode(userLanguage, bForTranslation: true));
			}
		}

		public virtual void OnTranslateCompleteFromCS_Server(long chatUID, string textTranslated)
		{
			if (m_requestedTranslationList != null)
			{
				if (m_requestedTranslationList.TryGetValue(chatUID, out var value))
				{
					value?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK, textTranslated, chatUID, null);
				}
				m_requestedTranslationList.Remove(chatUID);
			}
		}

		public virtual bool IsForceSelectPrevLangWhenNoServerTags()
		{
			return false;
		}

		public virtual void SetDefaultLanage(NKM_NATIONAL_CODE eNKM_NATIONAL_CODE)
		{
		}
	}

	public delegate void Func(OnComplete onComplete);

	private static GameObject s_objInstance;

	protected static NKCPublisherModule s_Instance;

	public const float API_TIMEOUT = 10f;

	private Coroutine crWaitForCall;

	private const float DEFAULT_TIME_OUT = 10f;

	private bool m_bWaitCall;

	public static NKCPublisherModule Instance
	{
		get
		{
			if (s_Instance == null)
			{
				MakeInstance();
			}
			return s_Instance;
		}
	}

	protected abstract ePublisherType _PublisherType { get; }

	public static ePublisherType PublisherType => Instance._PublisherType;

	public static ePublisherInitState InitState { get; protected set; }

	protected abstract bool _Busy { get; }

	public static bool Busy => Instance._Busy;

	public static string LastError { get; protected set; }

	public static NKCPMAuthentication Auth { get; private set; }

	public static NKCPMInAppPurchase InAppPurchase { get; private set; }

	public static NKCPMStatistics Statistics { get; private set; }

	public static NKCPMNotice Notice { get; private set; }

	public static NKCPMPush Push { get; private set; }

	public static NKCPMPermission Permission { get; private set; }

	public static NKCPMServerInfo ServerInfo { get; private set; }

	public static NKCPMLocalization Localization { get; private set; }

	public static NKCPMMarketing Marketing { get; private set; }

	public static NKCPublisherModule MakeInstance()
	{
		if (s_objInstance == null)
		{
			s_objInstance = new GameObject("NKCPublisherModule");
			UnityEngine.Object.DontDestroyOnLoad(s_objInstance);
		}
		if (NKCDefineManager.DEFINE_STEAM())
		{
			s_Instance = s_objInstance.AddComponent<NKCPMSteamPC>();
		}
		else if (NKCDefineManager.DEFINE_JPPC())
		{
			s_Instance = s_objInstance.AddComponent<NKCPMJPPC>();
		}
		else
		{
			s_Instance = s_objInstance.AddComponent<NKCPMNone>();
		}
		return s_Instance;
	}

	public static bool IsPublisherNoneType()
	{
		return Instance._PublisherType == ePublisherType.None;
	}

	public static bool ApplyCultureInfo()
	{
		ePublisherType publisherType = PublisherType;
		if (publisherType == ePublisherType.SB_Gamebase || publisherType == ePublisherType.STEAM)
		{
			return true;
		}
		return false;
	}

	public static bool IsPCBuild()
	{
		if (IsNexonPCBuild())
		{
			return true;
		}
		if (IsSteamPC())
		{
			return true;
		}
		return false;
	}

	public static bool IsNexonPCBuild()
	{
		ePublisherType publisherType = PublisherType;
		if (publisherType == ePublisherType.NexonPC || publisherType == ePublisherType.JPPC)
		{
			return true;
		}
		return false;
	}

	public static bool IsSteamPC()
	{
		if (PublisherType == ePublisherType.STEAM)
		{
			return true;
		}
		return false;
	}

	public static bool ShowGameOptionGradeCheck()
	{
		if (PublisherType == ePublisherType.NexonPC)
		{
			return true;
		}
		return false;
	}

	public static bool IsNexonPublished()
	{
		switch (PublisherType)
		{
		case ePublisherType.NexonToy:
		case ePublisherType.NexonPC:
		case ePublisherType.NexonToyJP:
		case ePublisherType.JPPC:
			return true;
		default:
			return false;
		}
	}

	public static bool IsZlongPublished()
	{
		if (PublisherType == ePublisherType.Zlong)
		{
			return true;
		}
		return false;
	}

	public static bool IsGamebasePublished()
	{
		if (PublisherType == ePublisherType.SB_Gamebase)
		{
			return true;
		}
		return false;
	}

	public static bool IsBusy()
	{
		return Instance._Busy;
	}

	protected abstract void OnTimeOut();

	public static void InitInstance(OnComplete onComplete)
	{
		Debug.Log("[NKCPM] InitInstance");
		Auth = Instance.MakeAuthInstance();
		InAppPurchase = Instance.MakeInappInstance();
		Statistics = Instance.MakeStatisticsInstance();
		Notice = Instance.MakeNoticeInstance();
		Push = Instance.MakePushInstance();
		if (Push != null)
		{
			Push.Init();
		}
		Permission = Instance.MakePermissionInstance();
		ServerInfo = Instance.MakeServerInfoInstance();
		Localization = Instance.MakeLocalizationInstance();
		Marketing = Instance.MakeMarketingInstance();
		Instance._Init(onComplete);
	}

	public static void DoAfterLogout()
	{
		if (InitState == ePublisherInitState.Initialized && Auth.Init())
		{
			InAppPurchase.Init();
			Permission.Init();
			Marketing.Init();
			Localization.Init();
		}
	}

	protected abstract void _Init(OnComplete onComplete);

	protected abstract NKCPMAuthentication MakeAuthInstance();

	protected abstract NKCPMInAppPurchase MakeInappInstance();

	protected abstract NKCPMStatistics MakeStatisticsInstance();

	protected abstract NKCPMNotice MakeNoticeInstance();

	protected virtual NKCPMPush MakePushInstance()
	{
		return new NKCPMNone.PushNone();
	}

	protected virtual NKCPMPermission MakePermissionInstance()
	{
		return new NKCPMNone.PermissionNone();
	}

	protected virtual NKCPMServerInfo MakeServerInfoInstance()
	{
		return new NKCPMNone.ServerInfoDefault();
	}

	protected virtual NKCPMLocalization MakeLocalizationInstance()
	{
		return new NKCPMNone.LocalizationNone();
	}

	protected virtual NKCPMMarketing MakeMarketingInstance()
	{
		return new NKCPMNone.MarketingNone();
	}

	public virtual bool IsReviewServer()
	{
		return false;
	}

	public static bool CheckReviewServerSkipVariant(string assetVariant)
	{
		if (!Instance.IsReviewServer())
		{
			return false;
		}
		return assetVariant switch
		{
			"vkor" => true, 
			"vjpn" => true, 
			"vchn" => true, 
			_ => false, 
		};
	}

	public static bool CheckError(NKC_PUBLISHER_RESULT_CODE errorCode, string additionalError, bool bCloseWaitBox = true, NKCPopupOKCancel.OnButton onOkButton = null, bool popupMessage = false)
	{
		if (bCloseWaitBox)
		{
			NKMPopUpBox.CloseWaitBox();
		}
		if (errorCode == NKC_PUBLISHER_RESULT_CODE.NPRC_OK)
		{
			return true;
		}
		Debug.LogWarning("Publisher Error Code : " + errorCode);
		if (PassErrorPopup(errorCode))
		{
			onOkButton?.Invoke();
			return false;
		}
		string errorMessage = GetErrorMessage(errorCode, additionalError);
		if (popupMessage)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(errorMessage, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, errorMessage, onOkButton);
		}
		return false;
	}

	public static bool CheckBusy(OnComplete onComplete)
	{
		if (Busy)
		{
			Debug.Log("Busy!!");
			onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_BUSY);
			return false;
		}
		return true;
	}

	public static string GetErrorMessage(NKC_PUBLISHER_RESULT_CODE errorCode, string additionalError = null)
	{
		string text = null;
		if (NKCStringTable.CheckExistString(errorCode.ToString()))
		{
			string text2 = NKCStringTable.GetString(errorCode.ToString());
			if (!string.IsNullOrEmpty(text2))
			{
				text = text2;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			text = NKCStringTable.GetString("SI_ERROR_DEFAULT_MESSAGE");
		}
		text = ((NKCScenManager.CurrentUserData() == null || (int)NKCScenManager.CurrentUserData().m_eAuthLevel <= 1) ? $"{text}\n(P{(int)errorCode})" : $"{text}\n(P{(int)errorCode} {errorCode})");
		if (!string.IsNullOrEmpty(additionalError))
		{
			text += $"({additionalError})";
		}
		return text;
	}

	private static bool PassErrorPopup(NKC_PUBLISHER_RESULT_CODE resultCode)
	{
		if ((uint)(resultCode - 300) <= 1u || resultCode == NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_CHANGEACCOUNT_SUCCESS_QUIT || resultCode == NKC_PUBLISHER_RESULT_CODE.NPRC_NOTICE_NO_SHOW)
		{
			return true;
		}
		return false;
	}

	public void WaitForCall(Func targetFunc, OnComplete onComplete, float timeout = 10f)
	{
		crWaitForCall = StartCoroutine(Instance._WaitForCall(targetFunc, onComplete, timeout));
	}

	private IEnumerator _WaitForCall(Func targetFunc, OnComplete onComplete, float timeout)
	{
		m_bWaitCall = true;
		targetFunc(delegate(NKC_PUBLISHER_RESULT_CODE resultCode, string addError)
		{
			m_bWaitCall = false;
			onComplete?.Invoke(resultCode, addError);
			if (crWaitForCall != null)
			{
				StopCoroutine(crWaitForCall);
				crWaitForCall = null;
			}
		});
		float waitTime = 0f;
		while (m_bWaitCall)
		{
			waitTime += Time.unscaledDeltaTime;
			if (waitTime >= timeout && m_bWaitCall)
			{
				Debug.Log("Timeout!!");
				OnTimeOut();
				onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_TIMEOUT);
				crWaitForCall = null;
				break;
			}
			yield return null;
		}
	}
}
