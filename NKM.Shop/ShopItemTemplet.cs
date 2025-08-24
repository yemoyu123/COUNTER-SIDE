using System;
using System.Collections.Generic;
using System.Text;
using ClientPacket.Common;
using Cs.Shared.Time;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Templet.Office;

namespace NKM.Shop;

public sealed class ShopItemTemplet : INKMTemplet, INKMTempletEx
{
	public enum eBannerImgType
	{
		NONE,
		BIG,
		SMALL
	}

	public const string MarketIdNexonJpn = "com.nexon.cosjp_";

	public const string MarketIDNexonKor = "com.nexon.counterside_";

	public static readonly TimeSpan ShopResetHour = TimeSpan.FromHours(0.0);

	internal string eventIntervalId;

	internal string discountIntervalId;

	public int m_ProductID;

	public string m_MarketID = string.Empty;

	public string m_TabID;

	public bool m_bEnabled;

	public bool m_bVisible = true;

	public bool m_HideWhenSoldOut;

	public NKM_REWARD_TYPE m_ItemType;

	public int m_ItemID;

	public int m_FreeValue;

	public int m_PaidValue;

	public int m_BuffRewardID;

	public int m_PriceItemID;

	public int m_Price;

	public int m_PriceSteam;

	public int m_PriceSteamKRW;

	public PURCHASE_EVENT_REWARD_TYPE m_PurchaseEventType = PURCHASE_EVENT_REWARD_TYPE.NONE;

	public int m_PurchaseEventID;

	public int m_PurchaseEventValue;

	public SHOP_RESET_TYPE resetType;

	public int m_QuantityLimit;

	public int m_LimitShowIndex;

	public bool m_bUnlockBanner;

	public UnlockInfo m_UnlockInfo;

	public int m_EventTime;

	public string m_MailTitle;

	public string m_MailDesc;

	public float m_DiscountRate;

	public int m_ProfitRate;

	public int m_ChainIndex;

	public int m_NewbieDate;

	public double m_paidAmountRequired;

	public ReturningUserType m_ReturningUserType;

	public int m_refundMedalCount;

	public int m_InstantProductLimit;

	public string m_OpenTag;

	public HashSet<string> m_hsReddotAllow = new HashSet<string>();

	public bool m_bSpoiler;

	public string m_ItemName;

	public string m_Item_Desc;

	public string m_Item_Desc_Popup;

	public string m_CardImage;

	public string m_CardPrefab;

	public ShopItemRibbon m_TagImage;

	public int m_TabSubIndex;

	public int m_OrderList;

	public string m_UnlockReqStrID;

	public bool m_PointExchangeSpecial;

	public ShopReddotType m_Reddot;

	private ShopTabTemplet m_shopTabTemplet;

	public int TotalValue => m_FreeValue + m_PaidValue;

	public NKMIntervalTemplet EventIntervalTemplet { get; private set; } = NKMIntervalTemplet.Invalid;

	public NKMIntervalTemplet DiscountIntervalTemplet { get; private set; } = NKMIntervalTemplet.Invalid;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public bool ItemEnableByTag => NKMRewardTemplet.IsOpenedReward(m_ItemType, m_ItemID, useRandomContract: true);

	public bool IsLimitedItem => m_LimitShowIndex > 0;

	public bool HasDiscountDuration => DiscountIntervalTemplet.IsValid;

	public bool IsReturningProduct => m_ReturningUserType != ReturningUserType.None;

	public bool IsNewbieProduct => m_NewbieDate > 0;

	public bool IsInstantProduct => m_InstantProductLimit > 0;

	public int Key => m_ProductID;

	public ShopTabTemplet TabTemplet
	{
		get
		{
			if (m_shopTabTemplet != null)
			{
				return m_shopTabTemplet;
			}
			m_shopTabTemplet = ShopTabTemplet.Find(m_TabID, m_TabSubIndex);
			return m_shopTabTemplet;
		}
	}

	public NKMItemMiscTemplet MiscProductTemplet { get; private set; }

	public NKMEmoticonTemplet EmoticonProductTemplet { get; private set; }

	public NKMItemMiscTemplet PriceTemplet { get; private set; }

	public TimeSpan LimitSaleTime { get; private set; }

	public bool IsInAppProduct => m_PriceItemID == 0;

	public bool HasDateLimit
	{
		get
		{
			if (!IsReturningProduct && EventIntervalTemplet != null)
			{
				return EventIntervalTemplet.IsValid;
			}
			return false;
		}
	}

	public DateTime EventDateStartUtc => NKCSynchronizedTime.GetIntervalUtc(EventIntervalTemplet, bStart: true);

	public DateTime EventDateEndUtc => NKCSynchronizedTime.GetIntervalUtc(EventIntervalTemplet, bStart: false);

	public DateTime DiscountStartDateUtc => NKCSynchronizedTime.GetIntervalUtc(DiscountIntervalTemplet, bStart: true);

	public DateTime DiscountEndDateUtc => NKCSynchronizedTime.GetIntervalUtc(DiscountIntervalTemplet, bStart: false);

	public bool NeedHistory
	{
		get
		{
			if (m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.NONE)
			{
				return resetType != SHOP_RESET_TYPE.Unlimited;
			}
			return true;
		}
	}

	public bool HasDiscountDateLimit => DiscountIntervalTemplet.IsValid;

	public static ShopItemTemplet LoadFromLUA(NKMLua lua)
	{
		ShopItemTemplet shopItemTemplet = new ShopItemTemplet();
		bool flag = true;
		flag &= lua.GetData("m_ProductID", ref shopItemTemplet.m_ProductID);
		flag &= lua.GetData("m_TabID", ref shopItemTemplet.m_TabID);
		flag &= lua.GetData("m_bEnabled", ref shopItemTemplet.m_bEnabled);
		lua.GetData("m_bVisible", ref shopItemTemplet.m_bVisible);
		lua.GetData("m_HideWhenSoldOut", ref shopItemTemplet.m_HideWhenSoldOut);
		flag &= lua.GetData("m_ItemType", ref shopItemTemplet.m_ItemType);
		flag &= lua.GetData("m_ItemID", ref shopItemTemplet.m_ItemID);
		lua.GetData("m_FreeValue", ref shopItemTemplet.m_FreeValue);
		lua.GetData("m_PaidValue", ref shopItemTemplet.m_PaidValue);
		lua.GetData("m_BuffRewardID", ref shopItemTemplet.m_BuffRewardID);
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 135))
		{
			return null;
		}
		flag &= lua.GetData("m_PriceItemID", ref shopItemTemplet.m_PriceItemID);
		flag &= lua.GetData("m_Price", ref shopItemTemplet.m_Price);
		lua.GetData("m_PriceSteam", ref shopItemTemplet.m_PriceSteam);
		lua.GetData("m_PriceSteamKRW", ref shopItemTemplet.m_PriceSteamKRW);
		lua.GetData("m_PurchaseEventType", ref shopItemTemplet.m_PurchaseEventType);
		lua.GetData("m_PurchaseEventID", ref shopItemTemplet.m_PurchaseEventID);
		lua.GetData("m_PurchaseEventValue", ref shopItemTemplet.m_PurchaseEventValue);
		lua.GetData("m_QuantityLimit", ref shopItemTemplet.m_QuantityLimit);
		lua.GetData("m_LimitShowIndex", ref shopItemTemplet.m_LimitShowIndex);
		lua.GetData("m_bUnlockBanner", ref shopItemTemplet.m_bUnlockBanner);
		shopItemTemplet.m_UnlockInfo = UnlockInfo.LoadFromLua(lua);
		lua.GetData("m_EventDateStrID", ref shopItemTemplet.eventIntervalId);
		lua.GetData("m_EventTime", ref shopItemTemplet.m_EventTime);
		lua.GetData("m_MailTitle", ref shopItemTemplet.m_MailTitle);
		lua.GetData("m_MailDesc", ref shopItemTemplet.m_MailDesc);
		lua.GetData("m_ChainIndex", ref shopItemTemplet.m_ChainIndex);
		lua.GetData("m_NewbieDate", ref shopItemTemplet.m_NewbieDate);
		lua.GetData("m_paidAmountRequired", ref shopItemTemplet.m_paidAmountRequired);
		flag &= lua.GetData("m_ItemName", ref shopItemTemplet.m_ItemName);
		lua.GetData("m_Item_Desc", ref shopItemTemplet.m_Item_Desc);
		lua.GetData("m_Item_Desc_Popup", ref shopItemTemplet.m_Item_Desc_Popup);
		lua.GetData("m_CardImage", ref shopItemTemplet.m_CardImage);
		lua.GetData("m_CardPrefab", ref shopItemTemplet.m_CardPrefab);
		lua.GetData("m_TagImage", ref shopItemTemplet.m_TagImage);
		lua.GetData("m_UnlockReqStrID", ref shopItemTemplet.m_UnlockReqStrID);
		lua.GetData("m_DiscountRate", ref shopItemTemplet.m_DiscountRate);
		lua.GetData("m_ProfitRate", ref shopItemTemplet.m_ProfitRate);
		lua.GetData("m_DiscountDateStrID", ref shopItemTemplet.discountIntervalId);
		lua.GetData("m_TabSubIndex", ref shopItemTemplet.m_TabSubIndex);
		lua.GetData("m_OrderList", ref shopItemTemplet.m_OrderList);
		lua.GetData("m_refundMedalCount", ref shopItemTemplet.m_refundMedalCount);
		lua.GetData("m_QuantityLimitCond", ref shopItemTemplet.resetType);
		lua.GetData("m_ReturningUserType", ref shopItemTemplet.m_ReturningUserType);
		lua.GetData("m_InstantProductLimit", ref shopItemTemplet.m_InstantProductLimit);
		lua.GetData("m_OpenTag", ref shopItemTemplet.m_OpenTag);
		lua.GetData("m_Reddot", ref shopItemTemplet.m_Reddot);
		lua.GetData("bPointExchangeSpecial", ref shopItemTemplet.m_PointExchangeSpecial);
		lua.GetData("m_Reddot_Allow", shopItemTemplet.m_hsReddotAllow);
		lua.GetData("m_Spoiler", ref shopItemTemplet.m_bSpoiler);
		shopItemTemplet.LimitSaleTime = TimeSpan.FromDays(shopItemTemplet.m_NewbieDate);
		if (!flag)
		{
			return null;
		}
		return shopItemTemplet;
	}

	public static ShopItemTemplet Find(int productId)
	{
		return NKMTempletContainer<ShopItemTemplet>.Find(productId);
	}

	public void Join()
	{
		if (TabTemplet == null)
		{
			if (m_TabID != "TAB_HOME_LEVEL" && m_TabID != "TAB_HOME_LIMIT")
			{
				NKMTempletError.Add($"[ShopItemTemplet] invalid tab id:{m_TabID} subIndex:{m_TabSubIndex} productId:{m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 202);
			}
		}
		else
		{
			TabTemplet.Add(this);
		}
		if (m_ItemType == NKM_REWARD_TYPE.RT_MISC)
		{
			MiscProductTemplet = NKMItemManager.GetItemMiscTempletByID(m_ItemID);
			if (MiscProductTemplet == null)
			{
				NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] m_ItemID 유효하지 않습니다. itemType:{m_ItemType} itemId:{m_ItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 215);
			}
		}
		if (m_ItemType == NKM_REWARD_TYPE.RT_EMOTICON)
		{
			EmoticonProductTemplet = NKMEmoticonTemplet.Find(m_ItemID);
			if (EmoticonProductTemplet == null)
			{
				NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] m_ItemID 유효하지 않습니다. itemType:{m_ItemType} itemId:{m_ItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 224);
			}
		}
		PriceTemplet = NKMItemManager.GetItemMiscTempletByID(m_PriceItemID);
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
		if (IsInAppProduct)
		{
			m_MarketID = $"{m_ProductID}";
		}
	}

	public void JoinIntervalTemplet()
	{
		if (!string.IsNullOrEmpty(eventIntervalId))
		{
			EventIntervalTemplet = NKMIntervalTemplet.Find(eventIntervalId);
			if (EventIntervalTemplet == null)
			{
				EventIntervalTemplet = NKMIntervalTemplet.Unuseable;
				NKMTempletError.Add("잘못된 interval id :" + eventIntervalId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 249);
			}
		}
		if (!string.IsNullOrEmpty(discountIntervalId))
		{
			DiscountIntervalTemplet = NKMIntervalTemplet.Find(discountIntervalId);
			if (DiscountIntervalTemplet == null)
			{
				DiscountIntervalTemplet = NKMIntervalTemplet.Unuseable;
				NKMTempletError.Add("잘못된 interval id:" + discountIntervalId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 259);
			}
			else if (DiscountIntervalTemplet.IsRepeatDate)
			{
				NKMTempletError.Add($"[ShopItemTemplet:{Key}] 할인기간으로 반복 기간설정 사용 불가. id:{discountIntervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 263);
			}
		}
	}

	public void Validate()
	{
		if (!EnableByTag)
		{
			return;
		}
		if (IsReturningProduct && !EventIntervalTemplet.IsValid)
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 복귀상품은 시작가능 기간을 지정해야 함. eventIntervalId:{eventIntervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 280);
		}
		if (m_ItemType != NKM_REWARD_TYPE.RT_NONE && m_ItemID > 0)
		{
			if (!NKMRewardTemplet.IsValidReward(m_ItemType, m_ItemID) || (m_FreeValue <= 0 && m_PaidValue <= 0))
			{
				NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 상점 보상 정보가 존재하지 않음 m_ItemType:{m_ItemType} m_ItemID:{m_ItemID} m_FreeValue:{m_FreeValue} m_PaidValue:{m_PaidValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 288);
			}
			if (m_ItemType == NKM_REWARD_TYPE.RT_MISC && m_ItemID != 101 && m_ItemID != 102 && m_PaidValue != 0)
			{
				NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 유료재화 설정 오류. m_ItemType:{m_ItemType} m_ItemID:{m_ItemID} m_PaidValue:{m_PaidValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 296);
			}
		}
		if (m_DiscountRate < 0f || m_DiscountRate >= 100f)
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 할인율은 0~100 사이의 값만 입력 가능함 m_DiscountRate:{m_DiscountRate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 302);
		}
		if (m_ProfitRate != 0 && m_ProfitRate < 100)
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 이득률은 100 이상의 값만 입력 가능함 m_ProfitRate:{m_ProfitRate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 307);
		}
		if (m_BuffRewardID > 0 && NKMCompanyBuffManager.GetCompanyBuffTemplet(m_BuffRewardID) == null)
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 회사 버프가 존재하지 않음 m_BuffRewardID:{m_BuffRewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 312);
		}
		if (m_PriceItemID > 0)
		{
			if (PriceTemplet == null)
			{
				NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] m_PriceItemID가 유효하지 않습니다. priceItemId:{m_PriceItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 319);
			}
			else if (PriceTemplet.m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_RESOURCE)
			{
				NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] miscType은 재화타입만 허용합니다. priceItemId:{m_PriceItemID} itemType:{PriceTemplet.m_ItemMiscType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 323);
			}
		}
		if (IsInAppProduct && (m_MailTitle == null || m_MailDesc == null))
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 인앱결제 상품은 우편 정보가 반드시 필요.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 331);
		}
		if (m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.LEVELUP_PACKAGE && m_QuantityLimit > 1)
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 레벨업 패키지 상품의 구매 가능 횟수가 1을 초과.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 337);
		}
		if (m_Price == 0 && !HasLimit())
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 무료상품은 구매제한이 반드시 존재해야 함.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 342);
		}
		if (HasLimit() && m_QuantityLimit <= 0)
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 구매제한 아이템에 제한횟수 미설정. cond:{resetType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 347);
		}
		if (m_NewbieDate > 0 && IsReturningProduct)
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 가입일 체크상품은 복귀상품 설정 불가. newbieData:{m_NewbieDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 366);
		}
		if (m_paidAmountRequired < 0.0)
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 첫충전 지급상품 - 필요 결제금액이 올바르지 않음:{m_paidAmountRequired}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 372);
		}
		if (IsInAppProduct && m_refundMedalCount <= 0)
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 유료상품은 환불주화가치 설정 필요", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 377);
			m_refundMedalCount = 1;
		}
		if (!IsInAppProduct && m_refundMedalCount > 0)
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 유료상품 외에는 환불주화가치 설정할 필요 없음. 혼선 방지를 위해 오류 처리", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 383);
		}
		if (m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE && (IsInstantProduct || m_UnlockInfo.reqValue > 0))
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] m_UnlockReqType 값이 없는데 m_InstantProductLimit 또는 m_UnlockReqValue 값이 존재함", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 389);
		}
		if (m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE && m_UnlockInfo.reqValue > 0)
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] m_UnlockReqType 값이 있는데 m_UnlockReqValue 값이 없음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 395);
		}
		if (IsInstantProduct)
		{
			if (m_UnlockInfo.eReqType != STAGE_UNLOCK_REQ_TYPE.SURT_PLAYER_LEVEL && m_UnlockInfo.eReqType != STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_GET && m_UnlockInfo.eReqType != STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_STAGE)
			{
				NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 사용할 수 없는 UnlockReqType를 지정함 m_UnlockReqType:{m_UnlockInfo.eReqType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 404);
			}
			if (m_NewbieDate > 0 || m_ReturningUserType > ReturningUserType.None)
			{
				NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] m_InstantProductLimit 값이 있는데 m_NewbieDate:{m_NewbieDate} 혹은 m_ReturningUserType:{m_ReturningUserType} 값이 존재함.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 409);
			}
		}
		if (m_bUnlockBanner)
		{
			string.IsNullOrEmpty(m_CardPrefab);
		}
		if (MiscProductTemplet != null && MiscProductTemplet.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CUSTOM_PACKAGE && m_PurchaseEventType != PURCHASE_EVENT_REWARD_TYPE.NONE)
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 커스텀패키지 아이템에는 m_PurchaseEventType 설정 불가. m_PurchaseEventType:{m_PurchaseEventType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 426);
		}
		if (m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_SHOP_BUY_ITEM_ALL)
		{
			ShopItemTemplet shopItemTemplet = Find(m_UnlockInfo.reqValue);
			if (shopItemTemplet == null)
			{
				NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 타겟상품 매진 후 상품 구매설정. 타겟 상품 아이디가 유효하지 않음:{m_UnlockInfo.reqValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 435);
			}
			else if (!shopItemTemplet.m_bEnabled)
			{
				NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 타겟상품 매진 후 상품 구매설정. 타겟 상품이 비활성 상태. productId:{shopItemTemplet.m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 439);
			}
		}
		if (m_Reddot == ShopReddotType.REDDOT_PURCHASED && resetType == SHOP_RESET_TYPE.Unlimited)
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 레드닷 타입 오류. 리셋 타입이 없는 상품은 레드닷 설정 불가. m_Reddot:{m_Reddot}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 446);
		}
		if (TabTemplet != null && !TabTemplet.EnableByTag)
		{
			NKMTempletError.Add($"[ShopTemplet:{m_ProductID}] 상품이 열려있지만, 탭이 닫혀있습니다. tabId:{m_TabID} subIndex:{m_TabSubIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopItemTemplet.cs", 453);
		}
	}

	public bool IsDiscountTime(DateTime current)
	{
		return DiscountIntervalTemplet.IsValidTime(current);
	}

	public bool IsCountResetType()
	{
		if (TabTemplet != null && TabTemplet.IsCountResetType)
		{
			if (m_ChainIndex < 3)
			{
				return true;
			}
			return m_QuantityLimit > 0;
		}
		if (resetType != SHOP_RESET_TYPE.Unlimited)
		{
			return resetType != SHOP_RESET_TYPE.FIXED;
		}
		return false;
	}

	public bool HasLimit()
	{
		if (TabTemplet != null && TabTemplet.IsCountResetType)
		{
			if (m_ChainIndex < 3)
			{
				return true;
			}
			return m_QuantityLimit > 0;
		}
		return resetType != SHOP_RESET_TYPE.Unlimited;
	}

	public bool IsSubscribeItem()
	{
		return m_PurchaseEventType.ToSucscriptionDays() > 0;
	}

	public bool CanInstantProductPurchase(DateTime current, DateTime regDate)
	{
		if (m_InstantProductLimit <= 0)
		{
			return true;
		}
		TimeSpan timeSpan = current - regDate;
		TimeSpan timeSpan2 = TimeSpan.FromMinutes(m_InstantProductLimit);
		if (timeSpan > timeSpan2)
		{
			return false;
		}
		return true;
	}

	public bool CalcNextResetDayOfWeek(DateTime current, TimeSpan resetHourSpan, out DateTime result)
	{
		if (!resetType.ToDayOfWeek(out var result2))
		{
			result = default(DateTime);
			return false;
		}
		result = WeeklyReset.CalcNextReset(current, result2, resetHourSpan);
		return true;
	}

	public string GetItemName()
	{
		return NKCStringTable.GetString(m_ItemName);
	}

	public string GetItemDesc()
	{
		return NKCStringTable.GetString(m_Item_Desc);
	}

	public string GetItemDescPopup()
	{
		if (MiscProductTemplet != null && MiscProductTemplet.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_INTERIOR)
		{
			NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMOfficeInteriorTemplet.Find(MiscProductTemplet.Key);
			if (nKMOfficeInteriorTemplet != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(NKCStringTable.GetString("SI_DP_INTERIOR_SCORE_ONE_PARAM", nKMOfficeInteriorTemplet.InteriorScore));
				if (nKMOfficeInteriorTemplet.InteriorCategory == InteriorCategory.FURNITURE)
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(NKCStringTable.GetString("SI_DP_INTERIOR_SIZE_TWO_PARAM", nKMOfficeInteriorTemplet.CellX, nKMOfficeInteriorTemplet.CellY));
				}
				stringBuilder.AppendLine();
				stringBuilder.Append(NKCStringTable.GetString(m_Item_Desc_Popup));
				return stringBuilder.ToString();
			}
		}
		return NKCStringTable.GetString(m_Item_Desc_Popup);
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}
}
