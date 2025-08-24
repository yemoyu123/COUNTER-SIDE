namespace NKM;

public static class NKMMain
{
	public const int RESET_TIME = 4;

	public const byte DECK_UNIT_COUNT = 8;

	public const byte RAID_DECK_UNIT_COUNT = 16;

	public const byte LEAGUE_PVP_DECK_UNIT_COUNT = 9;

	public const byte LEAGUE_PVP_GLOBAL_BAN_UNIT_COUNT = 2;

	public const int MAXIMUM_CONTRACT_SLOT = 10;

	public const int CASH_COST_UNLOCK_CONTRACT_SLOT = 300;

	public const int CASH_COST_UNLOCK_SHIP_BUILDING_SLOT = 400;

	public const int CASH_COST_UNLOCK_ARMY_SLOT = 600;

	public const int MAX_SKILL_COUNT_PER_UNIT = 5;

	public const int MAX_SKILL_COUNT_PER_SHIP = 3;

	public const int MAX_COUNT_UNIT_SKILL_STAT = 5;

	public const string TOUCH_UI_RES_NAME = "AB_fx_ui_touch";

	public const int IMI_CREDIT = 1;

	public const int IMI_ETERNIUM = 2;

	public const int IMI_INFORMATION = 3;

	public const int IMI_DAILY_TICKET = 4;

	public const int IMI_PVP_POINT = 5;

	public const int IMI_PVP_CHARGE_POINT = 6;

	public const int IMI_CLOTHES_COUPON = 7;

	public const int IMI_ITEM_MISC_PARTNERS_BUSINESS_CARD = 8;

	public const int IMI_PVP_CHARGE_POINT_FOR_PRACTICE = 9;

	public const int IMI_DIVE_POINT = 11;

	public const int IMI_PVP_SEASON_POINT = 12;

	public const int IMI_ASYNC_PVP_TICKET = 13;

	public const int IMI_DAILY_TICKET_A = 15;

	public const int IMI_DAILY_TICKET_B = 16;

	public const int IMI_DAILY_TICKET_C = 17;

	public const int IMI_CONTRIBUTION_MEDAL = 18;

	public const int IMI_SHADOW_TICKET = 19;

	public const int IMI_WAR_CLOUD_SHADOW_PIECE = 20;

	public const int IMI_GUILD_POINT = 21;

	public const int IMI_GUILD_WELFARE_POINT = 23;

	public const int IMI_GUILD_UNION_POINT = 24;

	public const int IMI_FIERCE_POINT = 25;

	public const int IMI_CHALLENGE_TICKET = 26;

	public const int IMI_QUARTZ = 101;

	public const int IMI_PACKAGE_MEDAL = 102;

	public const int IMI_REPEAT_POINT = 201;

	public const int IMI_ACHIEVE_POINT = 202;

	public const int IMI_DAILY_REPEAT_POINT = 203;

	public const int IMI_WEEKLY_REPEAT_POINT = 204;

	public const int IMI_PVP_CHARGE_POINT_X2 = 301;

	public const int IMI_SPECIAL_CONTRACT_POINT = 401;

	public const int IMI_USER_EXP = 501;

	public const int IMI_UNIT_EXP = 502;

	public const int IMI_GUILD_EXP = 503;

	public const int IMI_EVENT_PASS_EXP = 504;

	public const int IMI_EVENT_POINT_01 = 601;

	public const int IMI_2020_VALENTINE_TICKET = 602;

	public const int IMI_2020_VALENTINE_CHOCOBALL = 603;

	public const int IMI_RUSTY_DOG_TAG = 613;

	public const int IMI_2021_HORIZON = 636;

	public const int IMI_ITEM_MISC_RANDOM_UNIT = 901;

	public const int IMI_ITEM_MISC_RANDOM_WEAPON = 902;

	public const int IMI_ITEM_MISC_RANDOM_MISC = 903;

	public const int IMI_ITEM_MISC_RANDOM_MOLD = 904;

	public const int IMI_UNIT_LEVEL = 910;

	public const int IMI_BUILD_POINT = 911;

	public const int IMI_LIMITBREAK = 912;

	public const int IMI_ITEM_MISC_UNIT_SKILL_TRAING_1 = 1018;

	public const int IMI_ITEM_MISC_UNIT_SKILL_TRAING_2 = 1019;

	public const int IMI_ITEM_MISC_UNIT_NEGOTIATE_DOC_1 = 1031;

	public const int IMI_ITEM_MISC_UNIT_NEGOTIATE_DOC_2 = 1032;

	public const int IMI_ITEM_MISC_UNIT_NEGOTIATE_DOC_3 = 1033;

	public const int IMI_ITEM_MISC_CONTRACT_DOC_1 = 1001;

	public const int IMI_ITEM_MISC_CONTRACT_SPECIAL = 1021;

	public const int IMI_ITEM_MISC_UNIT_REMOVE_CARD = 1022;

	public const int IMI_ITEM_MISC_SHIP_REMOVE_CARD = 1023;

	public const int IMI_ITEM_MISC_CONTRACT_DOC_CLASSIFIED = 1034;

	public const int IMI_ITEM_MISC_DIVE_COORD_INITIALIZE = 1041;

	public const int IMI_ITEM_MISC_DIVE_COORD_INITIALIZE_FORCE = 1042;

	public const int INAPP_PURCHASE_ITEM_ID = 0;

	public const int IMI_ITEM_MISC_MAKE_INSTANT = 1012;

	public const int IMI_ITEM_MISC_MAKE_INSTANT_COST_CNT = 1;

	public const int IMI_ITEM_MISC_TUNING_MATERIAL = 1013;

	public const int IMI_ITEM_MISC_SET_MATERIAL = 1035;

	public const int IMI_ITEM_MISC_RELIC_MATERIAL = 1073;

	public const int IMI_ITEM_MISC_EXPLOR_PERMIT = 1065;

	public const int MAX_COUNT_NEW_DAILY_TICKET_FREE = 2;

	public const int DAILY_TICKET_PRODUCT_ID = 40103;

	public const int DAILY_TICKET_A_PRODUCT_ID = 40121;

	public const int DAILY_TICKET_B_PRODUCT_ID = 40122;

	public const int DAILY_TICKET_C_PRODUCT_ID = 40123;

	public const int MAX_FRIEND_ADD_COUNT = 60;

	public const int MAX_FRIEND_REQ_COUNT = 50;

	public const int MAX_FRIEND_RES_COUNT = 50;

	public const int MAX_FRIEND_BAN_COUNT = 30;

	public const int MAX_FRIEND_DEL_COUNT_PER_DAY = 10;

	public const int MAX_FRIEND_INTRO_LENGTH = 20;

	public const int FRIEND_BOX_PRODUCT_ID = 6011;

	public const int NKM_COUNTER_CASE_NORMAL_EP_ID = 50;

	public const int NKM_COUNTER_CASE_SECRET_EP_ID = 51;

	public const int MAX_EPISODE_DIFFICULTY_COUNT = 2;

	public const int MAX_EPISODE_COMPLETE_REWARD = 3;

	public const int MAX_EPISODE_MISSION_MEDAL_COUNT = 3;

	public const int DID_TUTORIAL_1 = 1004;

	public const int DID_TUTORIAL_2 = 1005;

	public const int DID_TUTORIAL_3 = 1006;

	public const int DID_TUTORIAL_4 = 1007;

	public const int TUTORIAL_1_STAGE_ID = 11211;

	public const int TUTORIAL_2_STAGE_ID = 11212;

	public const int TUTORIAL_3_STAGE_ID = 11213;

	public const int TUTORIAL_4_STAGE_ID = 11214;

	public const int DID_TRAINING_1 = 20001;

	public const int DID_TRAINING_2 = 20002;

	public const int DID_TRAINING_3 = 20003;

	public const int DID_TRAINING_4 = 20004;

	public const int DID_TRAINING_5 = 20005;

	public const int MISSION_GROWTH01 = 6;

	public const int MISSION_GROWTH02 = 7;

	public const int MISSION_GROWTH03 = 8;

	public const int SHADOW_PALACE_TICKET = 19;

	public const int SHADOW_PALACE_TICKET_MAX_COUNT = 3;

	public const int SHADOW_PALACE_COIN = 20;

	public const int SHADOW_UNLOCK_DUNGEON_ID = 10514;

	public static readonly int[] excludeUnitID = new int[12]
	{
		21001, 22001, 23001, 24001, 25001, 26001, 21022, 22022, 23022, 24022,
		25022, 26022
	};

	public static NKM_ERROR_CODE IsValidDeck(NKMArmyData cNKMArmyData, NKM_DECK_TYPE selectDeckType, byte selectDeckIndex)
	{
		return IsValidDeck(cNKMArmyData, new NKMDeckIndex(selectDeckType, selectDeckIndex));
	}

	public static NKM_ERROR_CODE IsValidDeck(NKMArmyData cNKMArmyData, NKM_DECK_TYPE selectDeckType, byte selectDeckIndex, NKM_GAME_TYPE gameType)
	{
		return IsValidDeck(cNKMArmyData, new NKMDeckIndex(selectDeckType, selectDeckIndex), gameType);
	}

	public static NKM_ERROR_CODE IsValidDeck(NKMArmyData cNKMArmyData, NKMDeckIndex selectDeckIndex)
	{
		NKMDeckData deckData = cNKMArmyData.GetDeckData(selectDeckIndex);
		if (deckData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DECK_DATA_INVALID;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = deckData.IsValidState();
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		nKM_ERROR_CODE = IsValidDeckCommon(cNKMArmyData, deckData, selectDeckIndex, NKM_GAME_TYPE.NGT_INVALID);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE IsValidDeck(NKMArmyData cNKMArmyData, NKMDeckIndex selectDeckIndex, NKM_GAME_TYPE gameType)
	{
		NKMDeckData deckData = cNKMArmyData.GetDeckData(selectDeckIndex);
		if (deckData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DECK_DATA_INVALID;
		}
		if (!deckData.IsValidGame(gameType))
		{
			return NKM_ERROR_CODE.NEC_FAIL_DECK_INVALID_GAME_TYPE;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = IsValidDeckCommon(cNKMArmyData, deckData, selectDeckIndex, gameType);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE IsValidDeckCommon(NKMArmyData cNKMArmyData, NKMDeckData cNKMDeckData, NKMDeckIndex selectDeckIndex, NKM_GAME_TYPE gameType)
	{
		if (!cNKMArmyData.IsValidDeckIndex(selectDeckIndex))
		{
			return NKM_ERROR_CODE.NEC_FAIL_SELECT_DECK_INDEX_INVALID;
		}
		if (cNKMDeckData.CheckHasDuplicateUnit(cNKMArmyData))
		{
			return NKM_ERROR_CODE.NEC_FAIL_DECK_DUPLICATE_UNIT;
		}
		NKMUnitData shipFromUID = cNKMArmyData.GetShipFromUID(cNKMDeckData.m_ShipUID);
		if (shipFromUID == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DECK_NO_SHIP;
		}
		if (shipFromUID.IsSeized)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SEIZED_SHIP_IN_DECK;
		}
		int num = 0;
		foreach (long item in cNKMDeckData.m_listDeckUnitUID)
		{
			NKMUnitData unitFromUID = cNKMArmyData.GetUnitFromUID(item);
			if (unitFromUID != null)
			{
				if (unitFromUID.IsSeized)
				{
					return NKM_ERROR_CODE.NEC_FAIL_SEIZED_UNIT_IN_DECK;
				}
				num++;
			}
		}
		if (num == 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DECK_NOT_ENOUGH_UNIT_COUNT;
		}
		if (NKMGame.IsPVP(gameType) && num != 8)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DECK_NOT_ENOUGH_UNIT_COUNT;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static float GetAttackCostRateByLimitBreakLevel(short limitBreakLevel)
	{
		if (limitBreakLevel <= 0)
		{
			return 1f;
		}
		return limitBreakLevel switch
		{
			1 => 1.6f, 
			2 => 2f, 
			_ => 2.5f, 
		};
	}

	public static bool IsBusyDeck(NKMDeckData deckData)
	{
		NKM_DECK_STATE state = deckData.GetState();
		if ((uint)(state - 1) <= 2u)
		{
			return true;
		}
		return false;
	}

	public static string IsTutorialDungeon(int dungeonId)
	{
		return dungeonId switch
		{
			1004 => "stage_1", 
			1005 => "stage_2", 
			1006 => "stage_3", 
			1007 => "stage_4", 
			20001 => "stage_5", 
			20002 => "stage_6", 
			20003 => "stage_7", 
			20004 => "stage_8", 
			20005 => "stage_9", 
			_ => string.Empty, 
		};
	}
}
