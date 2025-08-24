using System;
using System.Collections.Generic;
using Cs.GameLog.CountryDescription;
using Cs.Logging;
using NKM.Templet;

namespace NKM;

public static class NKMConst
{
	public static class Deck
	{
		public const int MaxUnitEnter = 8;
	}

	public static class Post
	{
		public static readonly DateTime MaxExpirationUtcDate = new DateTime(3000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		public static readonly DateTime UnlimitedExpirationUtcDate = new DateTime(2100, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
	}

	public static class Negotiation
	{
		public const int MaxMaterialCount = 3;
	}

	public static class Profile
	{
		public const int MaxEmblemCount = 3;
	}

	public static class Equip
	{
		public const int MaxPotentialOptionSocketCount = 3;

		public const int MaxMaterialCount = 4;
	}

	public static class Warfare
	{
		public const int DEFAULT_SUPPLY_COUNT = 2;

		public const int MAX_SUPPLY_COUNT = 2;

		public const int SERVICE_COST_ITEM_ID = 2;

		public const int EXPIRE_TIME = 12;

		public const int FriendshipPointItemId = 8;

		public const int RECOVERY_COST_MULTIPLE = 2;

		public const string MyFriendhipPointMailTitle = "SI_MAIL_ASSIST_ME_TITLE_V2";

		public const string MyFriendhipPointMailText = "SI_MAIL_ASSIST_ME_DESC_V2";

		public const string FriendFriendhipPointMailTitle = "SI_MAIL_ASSIST_OTHER_TITLE_V2";

		public const string FriendFriendhipPointMailText = "SI_MAIL_ASSIST_OTHER_DESC_V2";

		public const string MyGuestUsageMailTitle = "SI_MAIL_AST_GUEST_ME_TITLE";

		public const string MyGuestUsageMailText = "SI_MAIL_AST_GUEST_ME_DESC";

		public const string GuestUsageMailTitle = "SI_MAIL_AST_GUEST_OTHER_TITLE";

		public const string GuestUsageMailText = "SI_MAIL_AST_GUEST_OTHER_DESC";
	}

	public static class Episode
	{
		public const int DAILYMISSION_ATTACK = 101;

		public const int DAILYMISSION_SEARCH = 102;

		public const int DAILYMISSION_DEFENCE = 103;

		public const int DAILYMISSION_TACTICAL = 104;

		public const int MAX_ONE_TIME_REWARD_COUNT = 3;
	}

	public static class Raid
	{
		public const int MAX_RAID_RESULT_COUNT = 99;

		public const int MAX_RAID_BUFF_LEVEL = 5;

		public const int MAX_RAID_REWARD_GROUP_COUNT = 3;
	}

	public static class Craft
	{
		public const int CASH_COST_UNLOCK_SLOT = 300;

		public const int MAX_CRAFT_START_COUNT = 10;

		public const int MAX_CRAFT_MISC_START_COUNT = 999;
	}

	public static class Dungeon
	{
		public const int DEFAULT_SUPPLY = 2;
	}

	public static class Worldmap
	{
		public static readonly List<int> CITY_OPEN_CASH_COST = new List<int> { 0, 800, 2400, 4500, 8000, 12500 };

		public static readonly List<int> CITY_OPEN_CREDIT_COST = new List<int> { 0, 100000, 200000, 400000, 800000, 1600000 };

		public static readonly int RaidBuildingId = 21;

		public static readonly int DiveBuildingId = 22;
	}

	public static class Unit
	{
		public const int LoyaltyMax = 10000;

		public const int PermanentContractDocumentId = 1024;

		public const int ContractDocumentId = 1001;

		public const int ContractInstantCouponId = 1002;

		public const int ContractMilesageItemId = 401;

		public const int ShipContractDoc = 1015;

		public const float BonusExpRateOfPermanentContract = 0.2f;

		public const int UnitMaxLevelTier0 = 100;

		public const int UnitMaxLevelTier1 = 110;

		public const int UnitMaxLevelTier2 = 120;

		public const int UnitLevelCap = 120;

		public const short LimitBreakMaxLevelTier0 = 3;

		public const short LimitBreakMaxLevelTier1 = 8;

		public const short LimitBreakMaxLevelTier2 = 13;

		public const short LimitBreakCap = 13;
	}

	public static class Contract
	{
		public const int SelectableContractSlotCount = 10;
	}

	public static class Ship
	{
		public const int MaxUpgradeLevel = 6;

		public static readonly List<int> CoffinIds = new List<int> { 21001, 22001, 23001, 24001, 25001, 26001 };

		public static readonly List<int> BaseShipGroupIds = new List<int> { 20001, 20022 };
	}

	public static class OperationPowerFactor
	{
		public const int ClassValueOfSniper = 900;

		public const int ClassValueOfDefender = 900;

		public const int ClassValueOfStriker = 850;

		public const int ClassValueOfRanger = 850;

		public const int ClassValueOfSuppoter = 800;

		public const int ClassValueOfTower = 700;

		public const int ClasValueOfSiege = 700;

		public const int GradeValueOfAwakenSSR = 10075;

		public const int GradeValueOfRearmSSR = 9675;

		public const int GradeValueOfSSR = 9500;

		public const int GradeValueOfRearmSR = 9275;

		public const int GradeValueOfSR = 9125;

		public const int GradeValueOfR = 8850;

		public const int GradeValueOfN = 8600;

		public const float AdjustmentFactorOfUnit = 0.6f;

		public const float AdjustmentFactorOfEquip = 0.5f;

		public const int TacticValueOfAwakenSSR = 800;

		public const int TacticValueOfRearmSSR = 550;

		public const int TacticValueOfSSR = 500;

		public const int TacticValueOfRearmSR = 400;

		public const int TacticValueOfSR = 350;

		public const int TacticValueOfR = 300;

		public const int TacticValueOfN = 250;

		public const int ReactorValueOfAwakenSSR = 800;

		public const int ReactorValueOfRearmSSR = 750;

		public const int ReactorValueOfSSR = 700;

		public const int ReactorValueOfRearmSR = 650;

		public const int ReactorValueOfSR = 600;

		public const int ReactorValueOfR = 550;

		public const int ReactorValueOfN = 500;

		public static int GetClassValue(NKM_UNIT_ROLE_TYPE unitRoleType)
		{
			switch (unitRoleType)
			{
			case NKM_UNIT_ROLE_TYPE.NURT_SNIPER:
				return 900;
			case NKM_UNIT_ROLE_TYPE.NURT_DEFENDER:
				return 900;
			case NKM_UNIT_ROLE_TYPE.NURT_STRIKER:
				return 850;
			case NKM_UNIT_ROLE_TYPE.NURT_RANGER:
				return 850;
			case NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER:
				return 800;
			case NKM_UNIT_ROLE_TYPE.NURT_TOWER:
				return 700;
			case NKM_UNIT_ROLE_TYPE.NURT_SIEGE:
				return 700;
			default:
				Log.Error($"invalid unitRoleType - {unitRoleType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMConst.cs", 190);
				return 0;
			}
		}

		public static int GetGradeValue(NKM_UNIT_GRADE unitGrade, bool bAwaken, bool bRearm)
		{
			if (bAwaken)
			{
				if (unitGrade == NKM_UNIT_GRADE.NUG_SSR)
				{
					return 10075;
				}
			}
			else if (bRearm)
			{
				switch (unitGrade)
				{
				case NKM_UNIT_GRADE.NUG_SSR:
					return 9675;
				case NKM_UNIT_GRADE.NUG_SR:
					return 9275;
				}
			}
			else
			{
				switch (unitGrade)
				{
				case NKM_UNIT_GRADE.NUG_SSR:
					return 9500;
				case NKM_UNIT_GRADE.NUG_SR:
					return 9125;
				case NKM_UNIT_GRADE.NUG_R:
					return 8850;
				case NKM_UNIT_GRADE.NUG_N:
					return 8600;
				}
			}
			throw new InvalidOperationException("invalid unitGrade");
		}

		public static int GetTacticValue(NKM_UNIT_GRADE unitGrade, bool bAwaken, bool bRearm)
		{
			if (bAwaken)
			{
				if (unitGrade == NKM_UNIT_GRADE.NUG_SSR)
				{
					return 800;
				}
			}
			else if (bRearm)
			{
				switch (unitGrade)
				{
				case NKM_UNIT_GRADE.NUG_SSR:
					return 550;
				case NKM_UNIT_GRADE.NUG_SR:
					return 400;
				}
			}
			else
			{
				switch (unitGrade)
				{
				case NKM_UNIT_GRADE.NUG_SSR:
					return 500;
				case NKM_UNIT_GRADE.NUG_SR:
					return 350;
				case NKM_UNIT_GRADE.NUG_R:
					return 300;
				case NKM_UNIT_GRADE.NUG_N:
					return 250;
				}
			}
			throw new InvalidOperationException("invalid unitGrade");
		}

		public static int GetReactorValue(NKM_UNIT_GRADE unitGrade, bool bAwaken, bool bRearm)
		{
			if (bAwaken)
			{
				if (unitGrade == NKM_UNIT_GRADE.NUG_SSR)
				{
					return 800;
				}
			}
			else if (bRearm)
			{
				switch (unitGrade)
				{
				case NKM_UNIT_GRADE.NUG_SSR:
					return 750;
				case NKM_UNIT_GRADE.NUG_SR:
					return 650;
				}
			}
			else
			{
				switch (unitGrade)
				{
				case NKM_UNIT_GRADE.NUG_SSR:
					return 700;
				case NKM_UNIT_GRADE.NUG_SR:
					return 600;
				case NKM_UNIT_GRADE.NUG_R:
					return 550;
				case NKM_UNIT_GRADE.NUG_N:
					return 500;
				}
			}
			throw new InvalidOperationException("invalid unitGrade");
		}
	}

	public static class RandomShop
	{
		public const int MaxSlotCount = 9;

		public const int RefreshCost = 15;

		public const int RefreshMaxCountPerDay = 5;
	}

	public static class Account
	{
		public const int NicknameChangeItemId = 510;
	}

	public static class ServerString
	{
		public const string Seperator = "@@";

		public const string GuildBanId = "<GuildBanId>";

		public const string MiscId = "<MiscId>";

		public const string EquipId = "<EquipId>";

		public const string MoldId = "<MoldId>";

		public static string BuildMiscId(int itemId)
		{
			return string.Format("{0}{1}", "<MiscId>", itemId);
		}

		public static string BuildMoldId(int itemId)
		{
			return string.Format("{0}{1}", "<MoldId>", itemId);
		}
	}

	public static class UserLevelUp
	{
		public const string UserLevelUpPostTitle = "SI_MAIL_ACCOUNT_LEVEL_UP_REWARD_TITLE";

		public const string UserLevelUpPostDesc = "SI_MAIL_ACCOUNT_LEVEL_UP_REWARD_DESC";
	}

	public static class Buff
	{
		public enum BuffType
		{
			[CountryDescription("", CountryCode.KOR)]
			NONE,
			[CountryDescription("전역, 던전 크레딧 보상 증가", CountryCode.KOR)]
			WARFARE_DUNGEON_REWARD_CREDIT,
			[CountryDescription("전역, 던전 유닛 경험치 증가", CountryCode.KOR)]
			WARFARE_DUNGEON_REWARD_EXP_UNIT,
			[CountryDescription("전역, 던전 회사 경험치 증가", CountryCode.KOR)]
			WARFARE_DUNGEON_REWARD_EXP_COMPANY,
			[CountryDescription("전역 입장 이터니움 비용 감소", CountryCode.KOR)]
			WARFARE_ETERNIUM_DISCOUNT,
			[CountryDescription("전역, 던전 입장 이터니움 비용 감소", CountryCode.KOR)]
			WARFARE_DUNGEON_ETERNIUM_DISCOUNT,
			[CountryDescription("랭크전 건틀렛 시간당 포인트 획득량 증가", CountryCode.KOR)]
			PVP_POINT_CHARGE,
			[CountryDescription("모든 건틀렛 포인트 보상 증가", CountryCode.KOR)]
			ALL_PVP_POINT_REWARD,
			[CountryDescription("월드맵 미션 성공률 증가", CountryCode.KOR)]
			WORLDMAP_MISSION_COMPLETE_RATIO_BONUS,
			[CountryDescription("연봉협상 시 크레딧 재화의 소모량 감소", CountryCode.KOR)]
			BASE_PERSONNAL_NEGOTIATION_CREDIT_DISCOUNT,
			[CountryDescription("장비 제작 시 크레딧 재화의 소모량 감소", CountryCode.KOR)]
			BASE_FACTORY_CRAFT_CREDIT_DISCOUNT,
			[CountryDescription("장비 강화, 튜닝 시 크레딧 재화의 소모량 감소", CountryCode.KOR)]
			BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT,
			[CountryDescription("오퍼레이터 스킬전수 비용 감소", CountryCode.KOR)]
			OPERATOR_SKILL_ENHANCE_COST_DISCOUNT,
			[CountryDescription("오퍼레이터 스킬전수 성공확률 증가 SSR", CountryCode.KOR)]
			OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SSR,
			[CountryDescription("오퍼레이터 스킬전수 성공확률 증가 SR", CountryCode.KOR)]
			OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SR,
			[CountryDescription("오퍼레이터 스킬전수 성공확률 증가 R", CountryCode.KOR)]
			OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_R,
			[CountryDescription("오퍼레이터 스킬전수 성공확률 증가 N", CountryCode.KOR)]
			OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_N,
			[CountryDescription("잠재 옵션 크레딧 비용 할인", CountryCode.KOR)]
			BASE_FACTORY_POTENTIAL_SOCKET_CREDIT_DISCOUNT,
			[CountryDescription("전역, 던전 클리어 추가 드롭", CountryCode.KOR)]
			WARFARE_DUNGEON_REWARD_DROP_BONUS
		}
	}

	public static class Background
	{
		public const int defaultID = 9001;

		public const string defaultPrefab = "AB_UI_BG_SPRITE_CITY_NIGHT";

		public const string FollowLobby = "FOLLOW_LOBBY";

		public const int MaxBackgroundUnitCount = 8;
	}

	public static class ShadowPalace
	{
		public const int dummyUnitID = 999;

		public const int dummyShipID = 26001;

		public const string ShadowPlaceTag = "UNLOCK_SHADOW_PALACE_ON";
	}

	public static class Operator
	{
		public const string OperatorTag = "OPERATOR";
	}

	public static class Jukebox
	{
		public const float JukeboxBgmChangeCoolTime = 1f;
	}

	public static class Subscription
	{
		public const int ExpirationNoticeMailPostId = 101;

		public const int ExpirationNoticeMailCheckDays = 3;

		public const string ExpirationNoticeMailTitle = "SI_SHOP_MAIL_TITLE_SUBSCRIPTION";

		public const string ExpirationNoticeMailDesc = "SI_SHOP_MAIL_DESC_SUBSCRIPTION";

		public static readonly HashSet<int> ExpirationNoticeMailProducts = new HashSet<int> { 160492, 170000, 180000 };
	}

	public const int MAX_AUTO_SUPPLY_ACCUMULATE_MINUTES = 480;

	public static void Validate()
	{
		if (NKMItemManager.GetItemMiscTempletByID(1001) == null)
		{
			Log.ErrorAndExit($"채용계약서 id가 올바르지 않음. id:{1001}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMConst.cs", 364);
		}
		if (NKMItemManager.GetItemMiscTempletByID(1024) == null)
		{
			Log.ErrorAndExit($"종신고용서 id가 올바르지 않음. id:{1024}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMConst.cs", 369);
		}
		if (NKMItemManager.GetItemMiscTempletByID(1002) == null)
		{
			Log.ErrorAndExit($"긴급채용쿠폰 id가 올바르지 않음. id:{1002}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMConst.cs", 374);
		}
		if (NKMItemManager.GetItemMiscTempletByID(401) == null)
		{
			Log.ErrorAndExit($"특별채용권 id가 올바르지 않음. id:{401}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMConst.cs", 379);
		}
		if (NKMItemManager.GetItemMiscTempletByID(1015) == null)
		{
			Log.ErrorAndExit($"함선건조권 id가 올바르지 않음. id:{1015}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMConst.cs", 384);
		}
		if (NKMItemManager.GetItemMiscTempletByID(510) == null)
		{
			Log.ErrorAndExit($"닉네임변경권 id가 올바르지 않음. id:{510}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMConst.cs", 389);
		}
		if (NKMItemManager.GetItemMiscTempletByID(13) == null)
		{
			Log.ErrorAndExit($"전략전 티켓 id가 올바르지 않음. id:{13}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMConst.cs", 394);
		}
	}
}
