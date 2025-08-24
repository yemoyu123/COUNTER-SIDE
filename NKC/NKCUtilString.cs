using System;
using System.Collections.Generic;
using System.Text;
using ClientPacket.Negotiation;
using ClientPacket.Warfare;
using Cs.Core.Util;
using Cs.Logging;
using NKC.Publisher;
using NKC.UI;
using NKC.UI.Collection;
using NKM;
using NKM.Guild;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Templet.Office;
using UnityEngine;

namespace NKC;

public class NKCUtilString
{
	public class CompNKMDiveArtifactTemplet : IComparer<NKMDiveArtifactTemplet>
	{
		public int Compare(NKMDiveArtifactTemplet x, NKMDiveArtifactTemplet y)
		{
			if (x == null)
			{
				return 1;
			}
			if (y == null)
			{
				return -1;
			}
			if (x.Category < y.Category)
			{
				return 1;
			}
			if (x.Category > y.Category)
			{
				return -1;
			}
			return x.ArtifactID.CompareTo(y.ArtifactID);
		}
	}

	public class CompGuildDungeonArtifactTemplet : IComparer<GuildDungeonArtifactTemplet>
	{
		public int Compare(GuildDungeonArtifactTemplet x, GuildDungeonArtifactTemplet y)
		{
			if (x == null)
			{
				return 1;
			}
			if (y == null)
			{
				return -1;
			}
			if (x.GetCategory() < y.GetCategory())
			{
				return 1;
			}
			if (x.GetCategory() > y.GetCategory())
			{
				return -1;
			}
			return x.GetArtifactId().CompareTo(y.GetArtifactId());
		}
	}

	private delegate string ConditionToString(int input);

	private static StringBuilder m_sStringBuilder = new StringBuilder();

	public static string GET_STRING_CONTENTS_VERSION_CHANGE => NKCStringTable.GetString("SI_DP_CONTENTS_VERSION_CHANGE");

	public static string GET_STRING_CUTSCENE_MOVIE_SKIP_TITLE => NKCStringTable.GetString("SI_PF_CUTSCENE_MOVIE_SKIP_TITLE");

	public static string GET_STRING_CUTSCENE_MOVIE_SKIP_DESC => NKCStringTable.GetString("SI_PF_CUTSCENE_MOVIE_SKIP_DESC");

	public static string GET_STRING_COPY_COMPLETE => NKCStringTable.GetString("SI_PF_COPY_COMPLETE");

	public static string GET_STRING_SHUTDOWN_ALARM => NKCStringTable.GetString("SI_DP_SHUTDOWN_ALARM");

	public static string GET_STRING_COMMON_GRADE_ONE_PARAM => NKCStringTable.GetString("SI_DP_COMMON_GRADE_ONE_PARAM");

	public static string GET_STRING_MAIN_SHIP => NKCStringTable.GetString("SI_DP_MAIN_SHIP");

	public static string GET_STRING_MANAGEMENT => NKCStringTable.GetString("SI_DP_MANAGEMENT");

	public static string GET_STRING_EDITOR_NO_SUPPORT_THIS_FUNC => NKCStringTable.GetString("SI_DP_EDITOR_NO_SUPPORT_THIS_FUNC");

	public static string GET_STRING_QUIT => NKCStringTable.GetString("SI_DP_QUIT");

	public static string GET_STRING_NO_EXIST => NKCStringTable.GetString("SI_DP_NO_EXIST");

	public static string GET_STRING_WEAK => NKCStringTable.GetString("SI_DP_WEAK");

	public static string GET_STRING_NORMAL => NKCStringTable.GetString("SI_DP_NORMAL");

	public static string GET_STRING_CUSTOM => NKCStringTable.GetString("SI_DP_CUSTOM");

	public static string GET_STRING_WORST => NKCStringTable.GetString("SI_DP_WORST");

	public static string GET_STRING_LOW => NKCStringTable.GetString("SI_DP_LOW");

	public static string GET_STRING_GOOD => NKCStringTable.GetString("SI_DP_GOOD");

	public static string GET_STRING_BEST => NKCStringTable.GetString("SI_DP_BEST");

	public static string GET_STRING_LOW2 => NKCStringTable.GetString("SI_DP_LOW2");

	public static string GET_STRING_HIGH => NKCStringTable.GetString("SI_DP_HIGH");

	public static string GET_STRING_ATTACK_READY => NKCStringTable.GetString("SI_DP_ATTACK_READY");

	public static string GET_STRING_ATTACK_PREPARING => NKCStringTable.GetString("SI_DP_ATTACK_PREPARING");

	public static string GET_STRING_ATTACK_WAITING_OPPONENT => NKCStringTable.GetString("SI_DP_ATTACK_WAITING_OPPONENT");

	public static string GET_STRING_ATTACK_COST_IS_NOT_ENOUGH => NKCStringTable.GetString("SI_DP_ATTACK_COST_IS_NOT_ENOUGH");

	public static string GET_STRING_ORGANIZATION => NKCStringTable.GetString("SI_DP_ORGANIZATION");

	public static string GET_STRING_SAVE => NKCStringTable.GetString("SI_DP_SAVE");

	public static string GET_STRING_RANDOM => NKCStringTable.GetString("SI_PF_SLOT_CARD_RANDOM");

	public static string GET_STRING_NICKNAME_CHANGE_RECHECK_ONE_PARAM => NKCStringTable.GetString("SI_DP_PROFILE_NICKNAME_CHANGE_RECHECK_ONE_PARAM");

	public static string GET_STRING_ERROR => NKCStringTable.GetString("SI_DP_ERROR");

	public static string GET_STRING_NOTICE => NKCStringTable.GetString("SI_DP_NOTICE");

	public static string GET_STRING_WARNING => NKCStringTable.GetString("SI_DP_WARNING");

	public static string GET_STRING_UNLOCK => NKCStringTable.GetString("SI_DP_UNLOCK");

	public static string GET_STRING_INFORMATION => NKCStringTable.GetString("SI_DP_INFORMATION");

	public static string GET_STRING_CONFIRM => NKCStringTable.GetString("SI_DP_CONFIRM");

	public static string GET_STRING_CANCEL => NKCStringTable.GetString("SI_DP_CANCEL");

	public static string GET_STRING_SQUAD_ONE_PARAM => NKCStringTable.GetString("SI_DP_SQUAD_ONE_PARAM");

	public static string GET_STRING_SQUAD_TWO_PARAM => NKCStringTable.GetString("SI_DP_SQUAD_TWO_PARAM");

	public static string GET_STRING_CHOICE_ONE_PARAM => NKCStringTable.GetString("SI_DP_CHOICE_ONE_PARAM");

	public static string GET_STRING_NO_EXIST_EQUIP_TO_CHANGE => NKCStringTable.GetString("SI_DP_NO_EXIST_EQUIP_TO_CHANGE");

	public static string GET_STRING_COMING_SOON_SYSTEM => NKCStringTable.GetString("SI_DP_COMING_SOON_SYSTEM");

	public static string GET_STRING_COUNTING_ONE_PARAM => NKCStringTable.GetString("SI_DP_COUNTING_ONE_PARAM");

	public static string GET_STRING_TOTAL_RANK_ONE_PARAM => NKCStringTable.GetString("SI_DP_TOTAL_RANK_ONE_PARAM");

	public static string GET_STRING_RANK_ONE_PARAM => NKCStringTable.GetString("SI_DP_RANK_ONE_PARAM");

	public static string GET_STRING_NO_EXIST_TARGET_TO_SELECT => NKCStringTable.GetString("SI_DP_NO_EXIST_TARGET_TO_SELECT");

	public static string GET_STRING_PROFILE => NKCStringTable.GetString("SI_DP_PROFILE");

	public static string GET_STRING_REWARD_LIST_POPUP_TITLE => NKCStringTable.GetString("SI_DP_REWARD_LIST_POPUP_TITLE");

	public static string GET_STRING_REWARD_LIST_POPUP_DESC => NKCStringTable.GetString("SI_DP_REWARD_LIST_POPUP_DESC");

	public static string GET_STRING_POPUP_RESOURCE_CONFIRM_REWARD_DESC_02 => NKCStringTable.GetString("SI_DP_POPUP_RESOURCE_CONFIRM_REWARD_DESC_02");

	public static string GET_STRING_WIN => NKCStringTable.GetString("SI_DP_WIN");

	public static string GET_STRING_LOSE => NKCStringTable.GetString("SI_DP_LOSE");

	public static string GET_STRING_DRAW => NKCStringTable.GetString("SI_DP_DRAW");

	public static string GET_STRING_LEVEL_UP => NKCStringTable.GetString("SI_DP_LEVEL_UP");

	public static string GET_STRING_LEVEL_ONE_PARAM => NKCStringTable.GetString("SI_DP_LEVEL_ONE_PARAM");

	public static string GET_STRING_EXP_PLUS_ONE_PARAM => NKCStringTable.GetString("SI_DP_EXP_PLUS_ONE_PARAM");

	public static string GET_STRING_PLUS_EXP_ONE_PARAM => NKCStringTable.GetString("SI_DP_PLUS_EXP_ONE_PARAM");

	public static string GET_STRING_SHIP_INFO => NKCStringTable.GetString("SI_DP_SHIP_INFO");

	public static string GET_STRING_ITEM_GAIN => NKCStringTable.GetString("SI_DP_ITEM_GAIN");

	public static string GET_STRING_CONGRATULATION => NKCStringTable.GetString("SI_DP_CONGRATULATION");

	public static string GET_STRING_UNIT_INFO => NKCStringTable.GetString("SI_DP_UNIT_INFO");

	public static string GET_STRING_VOTE => NKCStringTable.GetString("SI_DP_VOTE");

	public static string GET_STRING_I_D => NKCStringTable.GetString("SI_DP_I_D");

	public static string GET_STRING_SLOT_FIRST_REWARD => NKCStringTable.GetString("SI_DP_SLOT_FIRST_REWARD");

	public static string GET_STRING_KILL_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_KILL_COUNT_ONE_PARAM");

	public static string GET_STRING_ATTACK_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_ATTACK_COUNT_ONE_PARAM");

	public static string GET_STRING_REMAIN_COUNT_TWO_PARAM => NKCStringTable.GetString("SI_DP_REMAIN_COUNT_TWO_PARAM");

	public static string GET_STRING_ITEM_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_ITEM_COUNT_ONE_PARAM");

	public static string GET_STRING_ERROR_SERVER_GAME_DATA => NKCStringTable.GetString("SI_DP_ERROR_SERVER_GAME_DATA");

	public static string GET_STRING_ERROR_SERVER_GAME_DATA_AND_GO_LOBBY => NKCStringTable.GetString("SI_DP_ERROR_SERVER_GAME_DATA_AND_GO_LOBBY");

	public static string GET_STRING_ERROR_RECONNECT => NKCStringTable.GetString("SI_DP_ERROR_RECONNECT");

	public static string GET_STRING_ERROR_DECONNECT => NKCStringTable.GetString("SI_DP_ERROR_DECONNECT");

	public static string GET_STRING_ERROR_FAIL_CONNECT => NKCStringTable.GetString("SI_DP_ERROR_FAIL_CONNECT");

	public static string GET_STRING_TRY_AGAIN => NKCStringTable.GetString("SI_DP_TRY_AGAIN");

	public static string GET_STRING_FILTER_ALL => NKCStringTable.GetString("SI_DP_FILTER_ALL");

	public static string GET_STRING_SORT_ENHANCE => NKCStringTable.GetString("SI_DP_SORT_ENHANCE");

	public static string GET_STRING_SORT_TIER => NKCStringTable.GetString("SI_DP_SORT_TIER");

	public static string GET_STRING_SORT_RARITY => NKCStringTable.GetString("SI_DP_SORT_RARITY");

	public static string GET_STRING_SORT_UID => NKCStringTable.GetString("SI_DP_SORT_UID");

	public static string GET_STRING_SORT_LEVEL => NKCStringTable.GetString("SI_DP_SORT_LEVEL");

	public static string GET_STRING_SORT_IDX => NKCStringTable.GetString("SI_DP_SORT_IDX");

	public static string GET_STRING_SORT_ATTACK => NKCStringTable.GetString("SI_DP_SORT_ATTACK");

	public static string GET_STRING_SORT_CRIT => NKCStringTable.GetString("SI_DP_SORT_CRIT");

	public static string GET_STRING_SORT_DEFENSE => NKCStringTable.GetString("SI_DP_SORT_DEFENSE");

	public static string GET_STRING_SORT_EVADE => NKCStringTable.GetString("SI_DP_SORT_EVADE");

	public static string GET_STRING_SORT_HEALTH => NKCStringTable.GetString("SI_DP_SORT_HEALTH");

	public static string GET_STRING_SORT_HIT => NKCStringTable.GetString("SI_DP_SORT_HIT");

	public static string GET_STRING_SORT_POPWER => NKCStringTable.GetString("SI_DP_SORT_POWER");

	public static string GET_STRING_SORT_COST => NKCStringTable.GetString("SI_DP_SORT_COST");

	public static string GET_STRING_SORT_PLAYER_LEVEL => NKCStringTable.GetString("SI_DP_SORT_PLAYER_LEVEL");

	public static string GET_STRING_SORT_LOGIN_TIME => NKCStringTable.GetString("SI_DP_SORT_LOGIN_TIME");

	public static string GET_STRING_SORT_ENHANCE_PROGRESS => NKCStringTable.GetString("SI_DP_SORT_ENHANCE_PROGRESS");

	public static string GET_STRING_SORT_SCOUT_PROGRESS => NKCStringTable.GetString("SI_PF_PERSONNEL_SCOUT_PIECE_HAVE");

	public static string GET_STRING_SORT_INTERIOR_POINT => NKCStringTable.GetString("SI_PF_POPUP_SORT_LIST_INTERIOR");

	public static string GET_STRING_SORT_PLACE_TYPE => NKCStringTable.GetString("SI_PF_POPUP_SORT_LIST_PLACE_TYPE");

	public static string GET_STRING_SORT_LIMIT_BREAK_PROGRESS => NKCStringTable.GetString("SI_PF_BASE_MENU_LAB_TRANSCENDENCE_TEXT");

	public static string GET_STRING_SORT_TRANSCENDENCE => NKCStringTable.GetString("SI_DP_SORT_TRANSCENDENCE");

	public static string GET_STRING_SORT_LOYALTY => NKCStringTable.GetString("SI_DP_SORT_LOYALTY");

	public static string GET_STRING_SORT_SQUAD_DUNGEON => NKCStringTable.GetString("SI_DP_SORT_DUNGEON_SQUAD");

	public static string GET_STRING_SORT_SQUAD_WARFARE => NKCStringTable.GetString("SI_DP_SORT_WARFARE_SQUAD");

	public static string GET_STRING_SORT_SQUAD_PVP => NKCStringTable.GetString("SI_DP_SORT_GAUNTLET");

	public static string GET_STRING_SORT_FAVORITE => NKCStringTable.GetString("SI_DP_SORT_FAVOURITE");

	public static string GET_STRING_SORT_TIME => NKCStringTable.GetString("SI_DP_SORT_TIME");

	public static string GET_STRING_FAVORITES_NO_ENTRY => NKCStringTable.GetString("SI_PF_FAVORITES_NO_ENTRY");

	public static string GET_STRING_SORT_DEPLOY_STATUS => NKCStringTable.GetString("SI_DP_SORT_DEPLOY");

	public static string GET_STRING_FAIL_NET => NKCStringTable.GetString("SI_DP_FAIL_NET");

	public static string GET_STRING_PVP => NKCStringTable.GetString("SI_DP_PVP");

	public static string GET_STRING_SKILL_COOLTIME_INC => NKCStringTable.GetString("SI_DP_SKILL_COOLTIME_INC");

	public static string GET_STRING_SKILL_COOLTIME_DEC => NKCStringTable.GetString("SI_DP_SKILL_COOLTIME_DEC");

	public static string GET_STRING_UPSIDE_MENU_WAIT_ITEM => NKCStringTable.GetString("SI_DP_UPSIDE_MENU_WAIT_ITEM");

	public static string GET_STRING_UI_LOADING_ERROR => NKCStringTable.GetString("SI_DP_UI_LOADING_ERROR");

	public static string GET_STRING_BUSINESS_MOTO_INPUT_PLEASE => NKCStringTable.GetString("SI_DP_BUSINESS_MOTO_INPUT_PLEASE");

	public static string GET_STRING_REGISTERTIME_DATE => NKCStringTable.GetString("SI_DP_REGISTERTIME_DATE");

	public static string GET_STRING_NO_RANK => NKCStringTable.GetString("SI_DP_NO_RANK");

	public static string GET_STRING_PROFILE_BIRTHDAY_INFO => NKCStringTable.GetString("SI_PF_PROFILE_BIRTHDAY_INFO");

	public static string GET_STRING_PROFILE_BIRTHDAY_POPUP_DESC => NKCStringTable.GetString("SI_PF_PROFILE_BIRTHDAY_POPUP_DESC");

	public static string GET_STRING_UNIT_BAN_COST => NKCStringTable.GetString("SI_DP_UNIT_BAN_COST");

	public static string GET_STRING_UNIT_UP_COST => NKCStringTable.GetString("SI_DP_UNIT_UP_COST");

	public static string GET_STRING_EMBLEM_EQUIPPED_EMBLEM_UNEQUIP => NKCStringTable.GetString("SI_DP_EMBLEM_EQUIPPED_EMBLEM_UNEQUIP");

	public static string GET_STRING_EMBLEM_EQUIPPED_EMBLEM_UNEQUIP_CONFIRM => NKCStringTable.GetString("SI_DP_EMBLEM_EQUIPPED_EMBLEM_UNEQUIP_CONFIRM");

	public static string GET_STRING_EMBLEM_EQUIPPED_EMBLEM_CHANGE_CONFIRM => NKCStringTable.GetString("SI_DP_EMBLEM_EQUIPPED_EMBLEM_CHANGE_CONFIRM");

	public static string GET_STRING_EMBLEM_EQUIP_CONFIRM => NKCStringTable.GetString("SI_DP_EMBLEM_EQUIP_CONFIRM");

	public static string GET_STRING_EMOTICON_ENEMY_GAME_OUT => NKCStringTable.GetString("SI_DP_EMOTICON_ENEMY_GAME_OUT");

	public static string GET_STRING_CONTRACT_RESULT_NONE => NKCStringTable.GetString("SI_DP_CONTRACT_RESULT_NONE");

	public static string GET_STRING_CONTRACT_PROGRESS_NONE => NKCStringTable.GetString("SI_DP_CONTRACT_PROGRESS_NONE");

	public static string GET_STRING_CONTRACT_NOT_ENOUGH_QUICK_ITEM => NKCStringTable.GetString("SI_DP_CONTRACT_NOT_ENOUGH_QUICK_ITEM");

	public static string GET_STRING_CONTRACT_CHARGE_UNIT_TEXT => NKCStringTable.GetString("SI_DP_CONTRACT_CHARGE_UNIT_TEXT");

	public static string GET_STRING_CONTRACT_CHARGE_SHIP_TEXT => NKCStringTable.GetString("SI_DP_CONTRACT_CHARGE_SHIP_TEXT");

	public static string GET_STRING_CONTRACT_MILEAGE_POINT => NKCStringTable.GetString("SI_DP_CONTRACT_MILEAGE_POINT");

	public static string GET_STRING_CONTRACT_MILEAGE_EVENT => NKCStringTable.GetString("SI_DP_CONTRACT_MILEAGE_EVENT");

	public static string GET_STRING_CONTRACT_USE_ALL_QUICK_ITEM => NKCStringTable.GetString("SI_DP_CONTRACT_USE_ALL_QUICK_ITEM");

	public static string GET_STRING_CONTRACT_USE_ALL_QUICK_ITEM_DISCRIPTION => NKCStringTable.GetString("SI_DP_CONTRACT_USE_ALL_QUICK_ITEM_DISCRIPTION");

	public static string GET_STRING_CONTRACT_NOT_AVAILABLE_TIME => NKCStringTable.GetString("SI_DP_CONTRACT_NOT_AVAILABLE_TIME");

	public static string GET_STRING_CONTRACT_HAVE_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_CONTRACT_HAVE_COUNT_ONE_PARAM");

	public static string GET_STRING_CONTRACT_INSTANT_UNIT_CONTRACT_USE => NKCStringTable.GetString("SI_DP_CONTRACT_INSTANT_UNIT_CONTRACT_USE");

	public static string GET_STRING_CONTRACT_UNIT_EMERGENCY_COUPON_USE => NKCStringTable.GetString("SI_DP_CONTRACT_UNIT_EMERGENCY_COUPON_USE");

	public static string GET_STRING_CONTRACT_INSTANT_SHIP_CONTRACT_USE => NKCStringTable.GetString("SI_DP_CONTRACT_INSTANT_SHIP_CONTRACT_USE");

	public static string GET_STRING_CONTRACT_SHIP_NUMBER => NKCStringTable.GetString("SI_DP_CONTRACT_SHIP_NUMBER");

	public static string GET_STRING_CONTRACT_SHIP_EMERGENCY_COUPON_USE => NKCStringTable.GetString("SI_DP_CONTRACT_SHIP_EMERGENCY_COUPON_USE");

	public static string GET_STRING_CONTRACT_SHIP_EMERGENCY_CONTRACT_COUPON_USE_REQ => NKCStringTable.GetString("SI_DP_CONTRACT_SHIP_EMERGENCY_CONTRACT_COUPON_USE_REQ");

	public static string GET_STRING_CONTRACT_SHIP_NEW_CONTRACT_SLOT_OPEN_REQ => NKCStringTable.GetString("SI_DP_CONTRACT_SHIP_NEW_CONTRACT_SLOT_OPEN_REQ");

	public static string GET_STRING_CONTRACT_SHIP_COMPLETE_CONTRACT_EXIST => NKCStringTable.GetString("SI_DP_CONTRACT_SHIP_COMPLETE_CONTRACT_EXIST");

	public static string GET_STRING_CONTRACT_SHIP_ON_GOING_CONTRACT_NUMBER_ONE_PARAM => NKCStringTable.GetString("SI_DP_CONTRACT_SHIP_ON_GOING_CONTRACT_NUMBER_ONE_PARAM");

	public static string GET_STRING_CONTRACT_SHIP_ON_GOING_CONTRACT_NO_EXIST => NKCStringTable.GetString("SI_DP_CONTRACT_SHIP_ON_GOING_CONTRACT_NO_EXIST");

	public static string GET_STRING_CONTRACT_UNIT_FAIL => NKCStringTable.GetString("SI_DP_CONTRACT_UNIT_FAIL");

	public static string GET_STRING_CONTRACT_SHIP_FAIL => NKCStringTable.GetString("SI_DP_CONTRACT_SHIP_FAIL");

	public static string GET_STRING_CONTRACT_FAIL_PROGRESS => NKCStringTable.GetString("SI_DP_CONTRACT_FAIL_PROGRESS");

	public static string GET_STRING_CONTRACT_FAIL_COMPLETE => NKCStringTable.GetString("SI_DP_CONTRACT_FAIL_COMPLETE");

	public static string GET_STRING_CONTRACT_FAIL_EMPTY => NKCStringTable.GetString("SI_DP_CONTRACT_FAIL_EMPTY");

	public static string GET_STRING_CONTRACT_FAIL_QUIICK => NKCStringTable.GetString("SI_DP_CONTRACT_FAIL_QUIICK");

	public static string GET_STRING_CONTRACT_FAIL_QUIICK_DESC => NKCStringTable.GetString("SI_DP_CONTRACT_FAIL_QUIICK_DESC");

	public static string GET_STRING_CONTRACT_SLOT_UNLOCK_FAIL => NKCStringTable.GetString("SI_DP_CONTRACT_SLOT_UNLOCK_FAIL");

	public static string GET_STRING_CONTRACT_SLOT_UNLOCK_FAIL_MAX => NKCStringTable.GetString("SI_DP_CONTRACT_SLOT_UNLOCK_FAIL_MAX");

	public static string GET_STRING_CONTRACT_SLOT_UNLOCK_FAIL_COST => NKCStringTable.GetString("SI_DP_CONTRACT_SLOT_UNLOCK_FAIL_COST");

	public static string GET_STRING_CONTRACT_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_CONTRACT_COUNT_ONE_PARAM");

	public static string GET_STRING_CONTRACT_FREE_TRY_DESC => NKCStringTable.GetString("SI_DP_CONTRACT_FREE_TRY_DESC");

	public static string GET_STRING_CONTRACT_FREE_02_TRY_DESC => NKCStringTable.GetString("SI_DP_CONTRACT_MULTI_FREE_TRY_DESC");

	public static string GET_STRING_CONTRACT_FREE_02_TRY_DESC_01 => NKCStringTable.GetString("SI_DP_CONTRACT_FREE_MULTI_TRY_POPUP_DESC");

	public static string GET_STRING_CONTRACT_CONFIRMATION_DESC_01 => NKCStringTable.GetString("SI_DP_CONTRACT_CONFIRMATION_DESC_01");

	public static string GET_STRING_CONTRACT_CONFIRMATION_DESC => NKCStringTable.GetString("SI_DP_CONTRACT_CONFIRMATION_DESC");

	public static string GET_STRING_CONTRACT_CONFIRMATION_ONE_LEFT_DESC => NKCStringTable.GetString("SI_PF_CONTRACT_RECRUIT_CONTRACT_BONUS_SPECIAL_TEXT");

	public static string GET_STRING_CONTRACT_CONFIRM_TOOLTIP_TITLE => NKCStringTable.GetString("SI_PF_CONTRACT_CONFIRM_TOOLTIP_TITLE");

	public static string GET_STRING_CONTRACT_CONFIRM_TOOLTIP_DESC => NKCStringTable.GetString("SI_PF_CONTRACT_CONFIRM_TOOLTIP_DESC");

	public static string GET_STRING_CONTRACT_CONFIRM_BOTTOM_DESC => NKCStringTable.GetString("SI_DP_CONTRACT_CONFIRM_BOTTOM_DESC");

	public static string GET_STRING_CONTRACT_CONFIRM_BOTTOM_DESC_OPERATOR => NKCStringTable.GetString("SI_PF_CONTRACT_CONFIRM_BOTTOM_DESC_OPR");

	public static string GET_STRING_CONTRACT_CONFIRMATION_POPUP_TITLE_01 => NKCStringTable.GetString("SI_DP_CONTRACT_CONFIRMATION_POPUP_TITLE_01");

	public static string GET_STRING_CONTRACT_CONFIRMATION_POPUP_TITLE_01_OPERATOR => NKCStringTable.GetString("SI_PF_CONTRACT_CONFIRM_BOTTOM_TITLE_OPR");

	public static string GET_STRING_CONTRACT_POPUP_RATE_EVENT_TIME_OVER_01 => NKCStringTable.GetString("SI_DP_CONTRACT_POPUP_RATE_EVENT_TIME_OVER_01");

	public static string GET_STRING_CONTRACT_POPUP_RATE_DETAIL_PERCENT_01 => NKCStringTable.GetString("SI_DP_CONTRACT_POPUP_RATE_DETAIL_PERCENT_01");

	public static string GET_STRING_CONTRACT_POPUP_RATE_DETAIL_PICKUP_ONLY_TITLE => NKCStringTable.GetString("SI_PF_CUSTOM_PACKAGE_INFO_TOP_TEXT");

	public static string GET_STRING_CONTRACT_MISC_NOT_ENOUGH_01 => NKCStringTable.GetString("SI_DP_CONTRACT_MISC_NOT_ENOUGH_01");

	public static string GET_STRING_CONTRACT_REQ_DESC_03 => NKCStringTable.GetString("SI_DP_CONTRACT_REQ_DESC_03");

	public static string GET_STRING_SELECTABLE_CONTRACT_DESC => NKCStringTable.GetString("SI_DP_SELECTABLE_CONTRACT_DESC");

	public static string GET_STRING_SELECTABLE_CONTRACT_UNIT_POOL_CHANGE_CONFIRM => NKCStringTable.GetString("SI_DP_SELECTABLE_CONTRACT_UNIT_POOL_CHANGE_CONFIRM");

	public static string GET_STRING_SELECTABLE_CONTRACT_CONFIRM => NKCStringTable.GetString("SI_DP_SELECTABLE_CONTRACT_CONFIRM");

	public static string GET_STRING_SELECTABLE_CONTRACT_NOT_ENOUGH => NKCStringTable.GetString("SI_DP_SELECTABLE_CONTRACT_NOT_ENOUGH");

	public static string GET_STRING_SELECTABLE_CONTRACT_USE_ITEM => NKCStringTable.GetString("SI_DP_SELECTABLE_CONTRACT_USE_ITEM");

	public static string GET_STRING_CONTRACT_FREE_EVENT_DESC => NKCStringTable.GetString("SI_DP_CONTRACT_FREE_EVENT_DESC");

	public static string GET_STRING_CONTRACT_REMAIN_COUNT_DESC => NKCStringTable.GetString("SI_DP_CONTRACT_COUNT_CLOSE_DESC");

	public static string GET_STRING_CONTRACT_CLOSE_TOOLTIP_TITLE => NKCStringTable.GetString("SI_PF_CONTRACT_COUNT_CLOSE_TOOLTIP_TITLE");

	public static string GET_STRING_CONTRACT_CLOSE_TOOLTIP_DESC => NKCStringTable.GetString("SI_PF_CONTRACT_COUNT_CLOSE_TOOLTIP_DESC");

	public static string GET_STRING_CONTRACT_FREE_BUTTON_DESC => NKCStringTable.GetString("SI_PF_CONTRACT_FREE_BUTTON_DESC");

	public static string GET_STRING_CONTRACT_FREE_BUTTON_DESC_01 => NKCStringTable.GetString("SI_DP_CONTRACT_FREE_BUTTON_DESC_01");

	public static string GET_STRING_CONTRACT_BUTTON_DESC => NKCStringTable.GetString("SI_DP_CONTRACT_BUTTON_DESC");

	public static string GET_STRING_CONTRACT_NOT_ENOUGH_FREE_TRY => NKCStringTable.GetString("SI_PF_CONTRACT_NOT_ENOUGH_FREE_TRY");

	public static string GET_STRING_CONTRACT_NOT_ENOUGH_ITEM_TRY => NKCStringTable.GetString("SI_DP_CONTRACT_NOT_ENOUGH_ITEM_TRY");

	public static string GET_STRING_CONTRACT_LIMIT_TIME => NKCStringTable.GetString("SI_DP_CONTRACT_LIMIT_TIME");

	public static string GET_STRING_CONTRACT_LIMIT_TIME_ADD_DAY => NKCStringTable.GetString("SI_DP_CONTRACT_LIMIT_TIME_ADD_DAY");

	public static string GET_STRING_CONTRACT_LIMIT_TIME_DAY => NKCStringTable.GetString("SI_DP_CONTRACT_LIMIT_TIME_DAY");

	public static string GET_STRING_CONTRACT_LIMIT_TIME_HOUR => NKCStringTable.GetString("SI_DP_CONTRACT_LIMIT_TIME_HOUR");

	public static string GET_STRING_CONTRACT_LIMIT_TIME_SECOND => NKCStringTable.GetString("SI_DP_CONTRACT_LIMIT_TIME_SECOND");

	public static string GET_STRING_CONTRACT_END_RECRUIT_TIME => NKCStringTable.GetString("SI_DP_CONTRACT_END_RECRUIT_TIME");

	public static string GET_STRING_CONTRACT_SELECTION_TITLE => NKCStringTable.GetString("SI_PF_CONTRACT_SELECTION_TITLE");

	public static string GET_STRING_CONTRACT_DAILY_RETRY_ENOUGH_TIMER_DESC => NKCStringTable.GetString("SI_DP_CONTRACT_DAILY_RETRY_ENOUGH_TIMER_DESC");

	public static string GET_STRING_CONTRACT_ALL_COMPLETE_CONFIRM => NKCStringTable.GetString("SI_DP_CONTRACT_ALL_COMPLETE_CONFIRM");

	public static string GET_STRING_CONTRACT_MISC_REWARD_DESC_01 => NKCStringTable.GetString("SI_PF_RESULT_CONTRACT_POINT_BONUS");

	public static string GET_STRING_CONTRACT_FREE_TRY_EXIT_DESC => NKCStringTable.GetString("SI_PF_CONTRACT_FREE_TRY_EXIT_DESC");

	public static string GET_STRING_CONTRACT_CUSTOM_CONTRACT_PICK_UP_WARNING_DESC => NKCStringTable.GetString("SI_DP_CUSTOM_CONTRACT_PICK_UP_WARNING_DESC");

	public static string GET_STRING_CONTRACT_CUSTOM_CONTRACT_RESET_WARNING_DESC => NKCStringTable.GetString("SI_DP_CUSTOM_CONTRACT_RESET_WARNING_DESC");

	public static string GET_STRING_CONTRACT_CUSTOM_CONTRACT_SELECT_BLOCK_DESC => NKCStringTable.GetString("SI_PF_CUSTOM_CONTRACT_SELECT_BLOCK_DESC");

	public static string GET_STRING_CONTRACT_CUSTOM_CONTRACT_CONFIRM_TOOLTIP_TITLE => NKCStringTable.GetString("SI_PF_CUSTOM_CONTRACT_CONFIRM_TOOLTIP_TITLE");

	public static string GET_STRING_CONTRACT_CUSTOM_CONTRACT_CONFIRM_TOOLTIP_DESC => NKCStringTable.GetString("SI_PF_CUSTOM_CONTRACT_CONFIRM_TOOLTIP_DESC");

	public static string GET_STRING_CONTRACT_SUBTITLE_PERCENTAGE_TITLE_01 => NKCStringTable.GetString("SI_PF_CONTRACT_V2_SUBTITLE_PERCENTAGE_TEXT");

	public static string GET_STRING_CONTRACT_CUSTOM_CONTRACT_NON_SELECT_TARGET_UNIT_DESC => NKCStringTable.GetString("SI_PF_CONTRACT_V2_SUBTITLE_03_TEXT");

	public static string GET_STRING_CONTRACT_CUSTOM_CONTRACT_DUPLICATE_WARNING_DESC => NKCStringTable.GetString("SI_DP_CUSTOM_CONTRACT_DUPLICATE_WARNING_DESC");

	public static string GET_STRING_CONTRACT_CUSTOM_CONTRACT_SELECT_WARNING_DESC => NKCStringTable.GetString("SI_DP_CUSTOM_CONTRACT_SELECT_WARNING_DESC");

	public static string GET_STRING_CUSTOM_CONTRACT_CONFIRM_TOOLTIP_TITLE => NKCStringTable.GetString("SI_PF_CUSTOM_CONTRACT_CONFIRM_TOOLTIP_TITLE");

	public static string GET_STRING_CUSTOM_CONTRACT_AWAKEN_CONFIRM_TOOLTIP_DESC => NKCStringTable.GetString("SI_PF_CUSTOM_CONTRACT_AWAKEN_CONFIRM_TOOLTIP_DESC");

	public static string GET_STRING_UNIT_SELECT => NKCStringTable.GetString("SI_DP_UNIT_SELECT");

	public static string GET_STRING_UNIT_SELECT_UNIT_NO_EXIST => NKCStringTable.GetString("SI_DP_UNIT_SELECT_UNIT_NO_EXIST");

	public static string GET_STRING_UNIT_SELECT_SHIP_NO_EXIST => NKCStringTable.GetString("SI_DP_UNIT_SELECT_SHIP_NO_EXIST");

	public static string GET_STRING_UNIT_SELECT_TROPHY_NO_EXIST => NKCStringTable.GetString("SI_DP_UNIT_SELECT_TROPHY_NO_EXIST");

	public static string GET_STRING_UNIT_SELECT_OPERATOR_NO_EXIST => NKCStringTable.GetString("SI_PF_OPERATOR_UNIT_SELECT_OPERATOR_NO_EXIST");

	public static string GET_STRING_UNIT_SELECT_HAVE_COUNT => NKCStringTable.GetString("SI_DP_CHOICE_ALREADY_HAVE");

	public static string GET_STRING_UNIT_SELECT_UNIT_COUNT => NKCStringTable.GetString("SI_DP_UNIT_SELECT_UNIT_COUNT");

	public static string GET_STRING_UNIT_SELECT_SHIP_COUNT => NKCStringTable.GetString("SI_DP_UNIT_SELECT_SHIP_COUNT");

	public static string GET_STRING_UNIT_SELECT_MAIN => NKCStringTable.GetString("SI_DP_UNIT_SELECT_MAIN");

	public static string GET_STRING_UNIT_SELECT_SUB_MAIN => NKCStringTable.GetString("SI_DP_UNIT_SELECT_SUB_MAIN");

	public static string GET_STRING_UNIT_SELECT_IMPOSSIBLE_DUPLICATE_ORGANIZE => NKCStringTable.GetString("SI_DP_UNIT_SELECT_IMPOSSIBLE_DUPLICATE_ORGANIZE");

	public static string GET_STRING_COLLECTION_UNIT => NKCStringTable.GetString("SI_PF_COLLECTION_EMPLOYEE");

	public static string GET_STRING_COLLECTION_SHIP => NKCStringTable.GetString("SI_PF_COLLECTION_SHIP");

	public static string GET_STRING_COLLECTION_ALLOWED_RANGE_VOTE_COMPLETE => NKCStringTable.GetString("SI_DP_COLLECTION_ALLOWED_RANGE_VOTE_COMPLETE");

	public static string GET_STRING_COLLECTION_TRAINING_MODE_CHANGE_REQ => NKCStringTable.GetString("SI_DP_COLLECTION_TRAINING_MODE_CHANGE_REQ");

	public static string GET_STRING_COLLECTION_STORY_SUB_TITLE_ONE_PARAM => NKCStringTable.GetString("SI_DP_COLLECTION_STORY_SUB_TITLE_ONE_PARAM");

	public static string GET_STRING_COLLECTION_STORY_EXTRA_TITLE_ONE_PARAM => "Extra. {0}";

	public static string GET_STRING_COLLECTION_STORY_ETC_TITLE => NKCStringTable.GetString("SI_PF_FILTER_MOLD_TYPE_ETC");

	public static string GET_STRING_REVIEW_DELETE => NKCStringTable.GetString("SI_DP_REVIEW_DELETE");

	public static string GET_STRING_POPUP_UNIT_REVIEW_DELETE => NKCStringTable.GetString("SI_DP_POPUP_UNIT_REVIEW_DELETE");

	public static string GET_STRING_POPUP_UNIT_REVIEW_SCORE => NKCStringTable.GetString("SI_DP_POPUP_UNIT_REVIEW_SCORE");

	public static string GET_STRING_UNIT_REVIEW_SCORE_VOTE_ONE_PARAM => NKCStringTable.GetString("SI_DP_UNIT_REVIEW_SCORE_VOTE_ONE_PARAM");

	public static string GET_STRING_UNIT_REVIEW_SCORE_VOTE_PLUS_ONE_PARAM => NKCStringTable.GetString("SI_DP_UNIT_REVIEW_SCORE_VOTE_PLUS_ONE_PARAM");

	public static string GET_STRING_UNIT_REVIEW => NKCStringTable.GetString("SI_DP_UNIT_REVIEW");

	public static string GET_STRING_REVIEW_DELETE_AND_WRITE => NKCStringTable.GetString("SI_DP_REVIEW_DELETE_AND_WRITE");

	public static string GET_STRING_REVIEW_IS_ALREADY_DELETE => NKCStringTable.GetString("SI_DP_REVIEW_IS_ALREADY_DELETE");

	public static string GET_STRING_COLLECTION => NKCStringTable.GetString("SI_DP_COLLECTION");

	public static string GET_STRING_COLLECTION_SKIN_SLOT_NAME => NKCStringTable.GetString("SI_DP_COLLECTION_SKIN_SLOT_NAME");

	public static string GET_STRING_SKIN_STORY_REPLAY_CONFIRM => NKCStringTable.GetString("SI_DP_SKIN_STORY_REPLAY_CONFIRM");

	public static string GET_STRING_MAIL => NKCStringTable.GetString("SI_DP_MAIL");

	public static string GET_STRING_MAIL_HAVE_COUNT => NKCStringTable.GetString("SI_DP_MAIL_HAVE_COUNT");

	public static string GET_STRING_INVEN => NKCStringTable.GetString("SI_DP_INVEN");

	public static string GET_STRING_INVEN_EQUIP_SELECT => NKCStringTable.GetString("SI_DP_INVEN_EQUIP_SELECT");

	public static string GET_STRING_INVEN_EQUIP_CHANGE_WARNING => NKCStringTable.GetString("SI_DP_INVEN_EQUIP_CHANGE_WARNING");

	public static string GET_STRING_INVEN_MISC_NO_EXIST => NKCStringTable.GetString("SI_DP_INVEN_MISC_NO_EXIST");

	public static string GET_STRING_INVEN_EQUIP_NO_EXIST => NKCStringTable.GetString("SI_DP_INVEN_EQUIP_NO_EXIST");

	public static string GET_STRING_INVEN_GIVE_RANDOM_OPTION => NKCStringTable.GetString("SI_DP_INVEN_GIVE_RANDOM_OPTION");

	public static string GET_STRING_INVEN_RANDOM_OPTION => NKCStringTable.GetString("SI_DP_STAT_SHORT_NAME_FOR_INVEN_EQUIP_RANDOM_OPTION");

	public static string GET_STRING_HAVE_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_HAVE_COUNT_ONE_PARAM");

	public static string GET_STRING_FACTORY_UPGRADE_OPTION_SUCCESSION => NKCStringTable.GetString("SI_PF_FACTORY_UPGRADE_OPTION_SUCCESSION");

	public static string GET_STRING_FILTER_EQUIP_OPTION_SEARCH => NKCStringTable.GetString("SI_PF_FILTER_EQUIP_OPTION_SEARCH");

	public static string GET_STRING_MENU_UNEQUIP => NKCStringTable.GetString("SI_DP_MENU_UNEQUIP");

	public static string GET_STRING_MENU_EQUIP => NKCStringTable.GetString("SI_DP_MENU_EQUIP");

	public static string GET_STRING_INVEN_THERE_IS_NO_UNIT_TO_EQUIP => NKCStringTable.GetString("SI_DP_INVEN_THERE_IS_NO_UNIT_TO_EQUIP");

	public static string GET_STRING_TOOLTIP_QUANTITY_ONE_PARAM => NKCStringTable.GetString("SI_DP_TOOLTIP_QUANTITY_ONE_PARAM");

	public static string GET_STRING_ITEM_LACK_DESC_ONE_PARAM => NKCStringTable.GetString("SI_DP_ITEM_LACK_DESC_ONE_PARAM");

	public static string GET_STRING_USE_ONE_PARAM => NKCStringTable.GetString("SI_DP_USE_ONE_PARAM");

	public static string GET_STRING_USE_PACKAGE => NKCStringTable.GetString("SI_DP_USE_PACKAGE");

	public static string GET_STRING_USE_CHOICE => NKCStringTable.GetString("SI_DP_USE_CHOICE");

	public static string GET_STRING_CHOICE_UNIT => NKCStringTable.GetString("SI_DP_CHOICE_TITLE_UNIT");

	public static string GET_STRING_CHOICE_SHIP => NKCStringTable.GetString("SI_DP_CHOICE_TITLE_SHIP");

	public static string GET_STRING_CHOICE_EQUIP => NKCStringTable.GetString("SI_DP_CHOICE_TITLE_EQUIP");

	public static string GET_STRING_CHOICE_MISC => NKCStringTable.GetString("SI_DP_CHOICE_TITLE_MISC");

	public static string GET_STRING_CHOICE_RECHECK_DECISION => NKCStringTable.GetString("SI_DP_CHOICE_RECHECK_DECISION");

	public static string GET_STRING_CHOICE_UNIT_CONFIRM => NKCStringTable.GetString("SI_DP_CHOICE_UNIT_CONFIRM");

	public static string GET_STRING_CHOICE_SHIP_CONFIRM => NKCStringTable.GetString("SI_DP_CHOICE_SHIP_CONFIRM");

	public static string GET_STRING_CHOICE_EQUIP_CONFIRM => NKCStringTable.GetString("SI_DP_CHOICE_EQUIP_CONFIRM");

	public static string GET_STRING_CHOICE_MISC_CONFIRM => NKCStringTable.GetString("SI_DP_CHOICE_MISC_CONFIRM");

	public static string GET_STRING_CHOICE_SKIN_CONFIRM => NKCStringTable.GetString("SI_DP_CHOICE_SKIN_CONFIRM");

	public static string GET_STRING_CHOICE_UNIT_RECHECK => NKCStringTable.GetString("SI_DP_CHOICE_UNIT_CONFIRM_RECHECK");

	public static string GET_STRING_CHOICE_SHIP_RECHECK => NKCStringTable.GetString("SI_DP_CHOICE_SHIP_CONFIRM_RECHECK");

	public static string GET_STRING_CHOICE_EQUIP_RECHECK => NKCStringTable.GetString("SI_DP_CHOICE_EQUIP_CONFIRM_RECHECK");

	public static string GET_STRING_CHOICE_MISC_RECHECK => NKCStringTable.GetString("SI_DP_CHOICE_MISC_CONFIRM_RECHECK");

	public static string GET_STRING_CHOICE_SKIN_RECHECK => NKCStringTable.GetString("SI_DP_CHOICE_SKIN_CONFIRM_RECHECK");

	public static string GET_STRING_CANNOT_EQUIP_ITEM_PRIVATE => NKCStringTable.GetString("SI_DP_EQUIP_POPUP_EQUIP_ITEM_FAIL_PRIVATE");

	public static string GET_STRING_ALREADY_FULL_EQUIPMENT => NKCStringTable.GetString("SI_PF_EQUIP_ALREADY_FULL_EQUIPMENT");

	public static string GET_STRING_INVENTORY_UNIT => NKCStringTable.GetString("SI_DP_INVENTORY_UNIT");

	public static string GET_STRING_INVENTORY_SHIP => NKCStringTable.GetString("SI_DP_INVENTORY_SHIP");

	public static string GET_STRING_INVENTORY_EQUIP => NKCStringTable.GetString("SI_DP_INVENTORY_EQUIP");

	public static string GET_STRING_TROPHY_UNIT => NKCStringTable.GetString("SI_DP_INVENTORY_TROPHY");

	public static string GET_STRING_FILTER_EQUIP_TYPE_STAT_MAIN => NKCStringTable.GetString("SI_PF_FILTER_EQUIP_TYPE_STAT_MAIN");

	public static string GET_STRING_FILTER_EQUIP_TYPE_STAT_SUB1 => NKCStringTable.GetString("SI_PF_FILTER_EQUIP_TYPE_STAT_SUB1");

	public static string GET_STRING_FILTER_EQUIP_TYPE_STAT_SUB2 => NKCStringTable.GetString("SI_PF_FILTER_EQUIP_TYPE_STAT_SUB2");

	public static string GET_STRING_EQUIP_FILTER_HIDDEN_OPTION => NKCStringTable.GetString("SI_PF_EQUIP_FILTER_HIDDEN_OPTION");

	public static string GET_STRING_FILTER_EQUIP_TYPE_STAT_SET => NKCStringTable.GetString("SI_PF_FILTER_EQUIP_TYPE_STAT_SET");

	public static string GET_STRING_EQUIP_FILTER_SELECT_ENCHANT => NKCStringTable.GetString("SI_PF_EQUIP_FILTER_SELECT_ENCHANT");

	public static string GET_STRING_EQUIP_FILTER_SELECT_ENCHANT_COUNT => NKCStringTable.GetString("SI_PF_EQUIP_FILTER_SELECT_ENCHANT_COUNT");

	public static string GET_STRING_MISC_UNIT_SELECTION_UNIT_INFO => NKCStringTable.GetString("SI_SELECTION_UNIT_INFO_TEXT");

	public static string GET_STRING_MISC_UNIT_SELECTION_UNIT_INFO_100 => NKCStringTable.GetString("SI_SELECTION_UNIT_LEVEL_100_INFO_TEXT");

	public static string GET_STRING_MISC_UNIT_SELECTION_UNIT_INFO_110 => NKCStringTable.GetString("SI_SELECTION_UNIT_LEVEL_110_INFO_TEXT");

	public static string GET_STRING_MISC_UNIT_SELECTION_UNIT_INFO_120 => NKCStringTable.GetString("SI_SELECTION_UNIT_LEVEL_120_INFO_TEXT");

	public static string GET_STRING_SELECTION_UNIT_LEVEL_INFO_TEXT => NKCStringTable.GetString("SI_SELECTION_UNIT_LEVEL_INFO_TEXT");

	public static string GET_STRING_SELECTION_SHIP_INFO_TEXT => NKCStringTable.GetString("SI_SELECTION_SHIP_INFO_TEXT");

	public static string GET_STRING_SELECTION_SHIP_INFO_TEXT_LEVEL => NKCStringTable.GetString("SI_SELECTION_SHIP_INFO_TEXT_LEVEL");

	public static string GET_STRING_OPERATOR_SELECTION_DESC => NKCStringTable.GetString("SI_PF_OPERATOR_SELECTION_DESC");

	public static string GET_STRING_OPERATOR_SELECTION_DESC_LEVEL => NKCStringTable.GetString("SI_PF_OPERATOR_SELECTION_DESC_LEVEL");

	public static string GET_STRING_ID => NKCStringTable.GetString("SI_DP_ID");

	public static string GET_STRING_DECK_BATCH_FAIL_STATE_WARFARE => NKCStringTable.GetString("SI_DP_DECK_BATCH_FAIL_STATE_WARFARE");

	public static string GET_STRING_DECK_BATCH_FAIL_STATE_DIVE => NKCStringTable.GetString("SI_DP_DECK_BATCH_FAIL_STATE_DIVE");

	public static string GET_STRING_EVENT_DECK_FIXED_TWO_PARAM => NKCStringTable.GetString("SI_DP_EVENT_DECK_FIXED_TWO_PARAM");

	public static string GET_STRING_NO_EXIST_SELECT_UNIT => NKCStringTable.GetString("SI_DP_NO_EXIST_SELECT_UNIT");

	public static string GET_STRING_SKIN_ONE_PARAM => NKCStringTable.GetString("SI_DP_SKIN_ONE_PARAM");

	public static string GET_STRING_UNIT_ROLE_INFO => NKCStringTable.GetString("SI_DP_UNIT_ROLE_INFO");

	public static string GET_STRING_DECK_CHANGE_UNIT_WARNING => NKCStringTable.GetString("SI_DP_DECK_CHANGE_UNIT_WARNING");

	public static string GET_STRING_DECK_BUTTON_ACTION => NKCStringTable.GetString("SI_DP_DECK_BUTTON_ACTION");

	public static string GET_STRING_DECK_CANNOT_START => NKCStringTable.GetString("SI_DP_DECK_CANNOT_START");

	public static string GET_STRING_DECK_STATE_DOING_MISSION => NKCStringTable.GetString("SI_DP_DECK_STATE_DOING_MISSION");

	public static string GET_STRING_DECK_STATE_DOING_WARFARE => NKCStringTable.GetString("SI_DP_DECK_STATE_DOING_WARFARE");

	public static string GET_STRING_DECK_STATE_DOING_DIVE => NKCStringTable.GetString("SI_DP_DECK_STATE_DOING_DIVE");

	public static string GET_STRING_DECK_STATE_FREE => NKCStringTable.GetString("SI_DP_DECK_STATE_FREE");

	public static string GET_STRING_DECK_UNIT_STATE_DOING => NKCStringTable.GetString("SI_DP_DECK_UNIT_STATE_DOING");

	public static string GET_STRING_DECK_UNIT_STATE_DOING_MISSION => NKCStringTable.GetString("SI_DP_DECK_UNIT_STATE_DOING_MISSION");

	public static string GET_STRING_DECK_UNIT_STATE_MISSION => NKCStringTable.GetString("SI_DP_DECK_UNIT_STATE_MISSION");

	public static string GET_STRING_DECK_UNIT_STATE_DUPLICATE => NKCStringTable.GetString("SI_DP_DECK_UNIT_STATE_DUPLICATE");

	public static string GET_STRING_DECK_UNIT_STATE_DECKED => NKCStringTable.GetString("SI_DP_DECK_UNIT_STATE_DECKED");

	public static string GET_STRING_DECK_UNIT_STATE_LOCKED => NKCStringTable.GetString("SI_DP_DECK_UNIT_STATE_LOCKED");

	public static string GET_STRING_DECK_UNIT_STATE_MAINUNIT => NKCStringTable.GetString("SI_DP_DECK_UNIT_STATE_MAINUNIT");

	public static string GET_STRING_DECK_BUTTON_START => NKCStringTable.GetString("SI_DP_DECK_BUTTON_START");

	public static string GET_STRING_DECK_BUTTON_OK => NKCStringTable.GetString("SI_DP_DECK_BUTTON_OK");

	public static string GET_STRING_DECK_BUTTON_SELECT => NKCStringTable.GetString("SI_DP_DECK_BUTTON_SELECT");

	public static string GET_STRING_DECK_BUTTON_BATCH => NKCStringTable.GetString("SI_DP_DECK_BUTTON_BATCH");

	public static string GET_STRING_DECK_BUTTON_PVP => NKCStringTable.GetString("SI_DP_DECK_BUTTON_PVP");

	public static string GET_STRING_DECK_SELECT_SHIP => NKCStringTable.GetString("SI_DP_DECK_SELECT_SHIP");

	public static string GET_STRING_COOLTIME_ONE_PARAM => NKCStringTable.GetString("SI_DP_COOLTIME_ONE_PARAM");

	public static string GET_STRING_SKIN_LOCK => NKCStringTable.GetString("SI_DP_SKIN_LOCK");

	public static string GET_STRING_SKIN_GRADE_PREMIUM => NKCStringTable.GetString("SI_DP_SKIN_GRADE_PREMIUM");

	public static string GET_STRING_UNIT_SKILL_INFO_ONE_PARAM => NKCStringTable.GetString("SI_DP_UNIT_SKILL_INFO_ONE_PARAM");

	public static string GET_STRING_DECK_SUCCESS_RATE_ONE_PARAM => NKCStringTable.GetString("SI_DP_DECK_SUCCESS_RATE_ONE_PARAM");

	public static string GET_STRING_DECK_AVG_SUMMON_COST => NKCStringTable.GetString("SI_DP_DECK_AVG_SUMMON_COST");

	public static string GET_STRING_DECK_SLOT_UNLOCK => NKCStringTable.GetString("SI_DP_DECK_SLOT_UNLOCK");

	public static string GET_STRING_EVENT_DECK => NKCStringTable.GetString("SI_DP_EVENT_DECK");

	public static string GET_STRING_MOVE_TO_TEST_MODE => NKCStringTable.GetString("SI_DP_MOVE_TO_TEST_MODE");

	public static string GET_STRING_MOVE_TO_COLLECTION => NKCStringTable.GetString("SI_DP_MOVE_TO_COLLECTION");

	public static string GET_STRING_MOVE_TO_LAB => NKCStringTable.GetString("SI_DP_MOVE_TO_LAB");

	public static string GET_STRING_MOVE_TO_SHIPYARD => NKCStringTable.GetString("SI_DP_MOVE_TO_SHIPYARD");

	public static string GET_STRING_UNKNOWN => NKCStringTable.GetString("SI_DP_UNKNOWN");

	public static string GET_STRING_BASE => NKCStringTable.GetString("SI_DP_BASE");

	public static string GET_STRING_BASE_SKIN => NKCStringTable.GetString("SI_DP_BASE_SKIN");

	public static string GET_STRING_SKIN_GRADE_NORMAL => NKCStringTable.GetString("SI_DP_SKIN_GRADE_NORMAL");

	public static string GET_STRING_SKIN_GRADE_RARE => NKCStringTable.GetString("SI_DP_SKIN_GRADE_RARE");

	public static string GET_STRING_SKIN_GRADE_SPECIAL => NKCStringTable.GetString("SI_DP_SKIN_GRADE_SPECIAL");

	public static string GET_STRING_REMAIN_UNIT_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_REMAIN_UNIT_COUNT_ONE_PARAM");

	public static string GET_STRING_SKILL_TRAINING_COMPLETE => NKCStringTable.GetString("SI_DP_SKILL_TRAINING_COMPLETE");

	public static string GET_STRING_LOYALTY => NKCStringTable.GetString("SI_DP_LOYALTY");

	public static string GET_STRING_LOYALTY_LIFETIME => NKCStringTable.GetString("SI_DP_LOYALTY_LIFETIME");

	public static string GET_STRING_DECK_VIEW_EMPTY_SLOT_COST => NKCStringTable.GetString("SI_PF_DECKVIEW_BASIC_COST");

	public static string GET_STRING_MOVE_TO_NEGOTIATE => NKCStringTable.GetString("SI_DP_MOVE_TO_NEGOTIATE");

	public static string GET_STRING_REVIEW_BAN => NKCStringTable.GetString("SI_DP_REVIEW_BAN");

	public static string GET_STRING_REVIEW_UNBAN => NKCStringTable.GetString("SI_DP_REVIEW_UNBAN");

	public static string GET_STRING_REVIEW_BAN_DESC => NKCStringTable.GetString("SI_DP_REVIEW_BAN_DESC");

	public static string GET_STRING_REVIEW_BAN_CANCEL_DESC => NKCStringTable.GetString("SI_DP_REVIEW_BAN_CANCEL_DESC");

	public static string GET_STRING_REVIEW_BANNED_CONTENT => NKCStringTable.GetString("SI_DP_REVIEW_BANNED_CONTENT");

	public static string GET_STRING_DECK_UNIT_STATE_SEIZURE => NKCStringTable.GetString("SI_DP_DECK_UNIT_STATE_SEIZURE");

	public static string GET_STRING_EVENT_DECK_ENEMY_LEVEL => NKCStringTable.GetString("SI_DP_SHADOW_PALACE_LEVEL");

	private static string GET_DECK_NUMBER_STRING_WARFARE => NKCStringTable.GetString("SI_DP_DECK_NUMBER_STRING_WARFARE");

	private static string GET_DECK_NUMBER_STRING_DAILY => NKCStringTable.GetString("SI_DP_DECK_NUMBER_STRING_DAILY");

	private static string GET_DECK_NUMBER_STRING_FRIEND => NKCStringTable.GetString("SI_DP_DECK_NUMBER_STRING_FRIEND");

	private static string GET_DECK_NUMBER_STRING_RAID => NKCStringTable.GetString("SI_DP_DECK_NUMBER_STRING_RAID");

	private static string GET_DECK_NUMBER_STRING_PVP_DEFENCE => NKCStringTable.GetString("SI_DP_DECK_NUMBER_STRING_PVP_DEFENCE");

	private static string GET_DECK_NUMBER_STRING_DIVE => NKCStringTable.GetString("SI_DP_DECK_NUMBER_STRING_DIVE");

	private static string GET_DECK_NUMBER_STRING_TRIM => NKCStringTable.GetString("SI_DP_DECK_NUMBER_STRING_TRIM");

	public static string GET_STRING_REMOVE_SHIP_REFUSE_TEXT => NKCStringTable.GetString("SI_DP_REMOVE_SHIP_REFUSE_TEXT");

	public static string GET_STRING_EP_TRAINING_NUMBER => "TR.{0}";

	public static string GET_STRING_EP_CUTSCEN_NUMBER => "#{0}";

	public static string GET_STRING_EPISODE_GIVE_UP_WARFARE => NKCStringTable.GetString("SI_DP_EPISODE_GIVE_UP_WARFARE");

	public static string GET_STRING_SIDE_STORY => NKCStringTable.GetString("SI_DP_SIDE_STORY");

	public static string GET_STRING_MAIN_STREAM => NKCStringTable.GetString("SI_DP_MAIN_STREAM");

	public static string GET_STRING_MENU_NAME_OPERATION_VIEWER => NKCStringTable.GetString("SI_DP_MENU_NAME_OPERATION_VIEWER");

	public static string GET_STRING_MENU_NAME_CC => NKCStringTable.GetString("SI_DP_MENU_NAME_CC");

	public static string GET_STRING_MENU_NAME_CCS => NKCStringTable.GetString("SI_DP_MENU_NAME_CCS");

	public static string GET_STRING_COUNTER_CASE_SLOT_BUTTON_LOCK => NKCStringTable.GetString("SI_DP_COUNTER_CASE_SLOT_BUTTON_LOCK");

	public static string GET_STRING_COUNTER_CASE_SLOT_BUTTON_UNLOCK => NKCStringTable.GetString("SI_DP_COUNTER_CASE_SLOT_BUTTON_UNLOCK");

	public static string GET_STRING_COUNTER_CASE_UNLOCK_BUTTON => NKCStringTable.GetString("SI_DP_COUNTER_CASE_UNLOCK_BUTTON");

	public static string GET_STRING_COUNTER_CASE_LOCK_BUTTON => NKCStringTable.GetString("SI_DP_COUNTER_CASE_LOCK_BUTTON");

	public static string GET_STRING_DAILY_CHECK_DAY => NKCStringTable.GetString("SI_DP_DAILY_CHECK_DAY");

	public static string GET_STRING_CONTENTS_UNLOCK_CLEAR_STAGE => NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_STAGE");

	public static string GET_STRING_NO_EVENT => NKCStringTable.GetString("SI_TOAST_EP_CATEGORY_HAVE_NO_EVENT");

	public static string GET_STRING_FREE_ORDER => NKCStringTable.GetString("SI_DP_FREE_ORDER");

	public static string GET_STRING_EVENT => NKCStringTable.GetString("SI_DP_EVENT");

	public static string GET_STRING_EPISODE_CATEGORY_EC_EVENT => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_EVENT");

	public static string GET_STRING_EPISODE_PROGRESS => NKCStringTable.GetString("SI_DP_EPISODE_PROGRESS");

	public static string GET_STRING_EPISODE_SUBSTREAM_DATA_EXPUNGED => NKCStringTable.GetString("SI_DP_EPISODE_SUBSTREAM_DATA_EXPUNGED");

	public static string GET_STRING_OPERATION_SUBSTREAM_SHORTCUT_TITLE => NKCStringTable.GetString("SI_DP_OPERATION_SUBSTREAM_SHORTCUT_TITLE");

	public static string GET_STRING_EPISODE_CATEGORY_EC_SIDESTORY => NKCStringTable.GetString("SI_EPISODE_CATEGORY_EC_SIDESTORY");

	public static string GET_STRING_EPISODE_SUPPLEMENT => NKCStringTable.GetString("SI_DP_EPISODE_SUPPLEMENT");

	public static string GET_STRING_WARFARE_FIRST_ALL_CLEAR => NKCStringTable.GetString("SI_DP_WARFARE_FIRST_ALL_CLEAR");

	public static string GET_STRING_MENU_NAME_WARFARE => NKCStringTable.GetString("SI_DP_MENU_NAME_WARFARE");

	public static string GET_STRING_WARFARE_REPAIR_TITLE => NKCStringTable.GetString("SI_DP_WARFARE_REPAIR_TITLE");

	public static string GET_STRING_WARFARE_REPAIR_DESC => NKCStringTable.GetString("SI_DP_WARFARE_REPAIR_DESC");

	public static string GET_STRING_WARFARE_SUPPLY_TITLE => NKCStringTable.GetString("SI_DP_WARFARE_SUPPLY_TITLE");

	public static string GET_STRING_WARFARE_SUPPLY_DESC => NKCStringTable.GetString("SI_DP_WARFARE_SUPPLY_DESC");

	public static string GET_STRING_WARFARE_WARNING_GAME_START => NKCStringTable.GetString("SI_DP_WARFARE_WARNING_GAME_START");

	public static string GET_STRING_WARFARE_WARNING_FINISH_TURN => NKCStringTable.GetString("SI_DP_WARFARE_WARNING_FINISH_TURN");

	public static string GET_STRING_WARFARE_WARNING_GIVE_UP => NKCStringTable.GetString("SI_DP_WARFARE_WARNING_GIVE_UP");

	public static string GET_STRING_WARFARE_WARNING_SUPPLY => NKCStringTable.GetString("SI_DP_WARFARE_WARNING_SUPPLY");

	public static string GET_STRING_WARFARE_WARNING_NO_EXIST_SUPPLY => NKCStringTable.GetString("SI_DP_WARFARE_WARNING_NO_EXIST_SUPPLY");

	public static string GET_STRING_WARFARE_PHASE_FINISH => NKCStringTable.GetString("SI_DP_WARFARE_PHASE_FINISH");

	public static string GET_STRING_WARFARE_WAVE_ONE_PARAM => NKCStringTable.GetString("SI_DP_WARFARE_WAVE_ONE_PARAM");

	public static string GET_STRING_WARFARE_RESULT_GAME_TIP => NKCStringTable.GetString("SI_DP_WARFARE_RESULT_GAME_TIP1") + NKCStringTable.GetString("SI_DP_WARFARE_RESULT_GAME_TIP2") + NKCStringTable.GetString("SI_DP_WARFARE_RESULT_GAME_TIP3") + NKCStringTable.GetString("SI_DP_WARFARE_RESULT_GAME_TIP4");

	public static string GET_STRING_DUNGEON_RESULT_GAME_TIP => NKCStringTable.GetString("SI_DP_DUNGEON_RESULT_GAME_TIP");

	public static string GET_STRING_WARFARE_POPUP_ENEMY_INFO_KILL => NKCStringTable.GetString("SI_DP_WARFARE_POPUP_ENEMY_INFO_KILL");

	public static string GET_STRING_WARFARE_POPUP_ENEMY_INFO_WAVE => NKCStringTable.GetString("SI_DP_WARFARE_POPUP_ENEMY_INFO_WAVE");

	public static string GET_STRING_WARFARE_POPUP_ENEMY_INFO_WAVE_ONE_PARAM => NKCStringTable.GetString("SI_DP_WARFARE_POPUP_ENEMY_INFO_WAVE_ONE_PARAM");

	public static string GET_STRING_WARFARE_SUPPORTER_SUPPLY => NKCStringTable.GetString("SI_DP_WARFARE_SUPPORTER_SUPPLY");

	public static string GET_STRING_WARFARE_SUPPORTER_REPAIR => NKCStringTable.GetString("SI_DP_WARFARE_SUPPORTER_REPAIR");

	public static string GET_STRING_WARFARE_CANNOT_START_BECAUSE_NO_USER_UNIT => NKCStringTable.GetString("SI_DP_WARFARE_CANNOT_START_BECAUSE_NO_USER_UNIT");

	public static string GET_STRING_WARFARE_CANNOT_FIND_RETRY_DATA => NKCStringTable.GetString("SI_TOAST_REPEAT_UNABLE_BY_RECONNECT");

	public static string GET_STRING_WARFARE_CANNOT_PAUSE => NKCStringTable.GetString("SI_DP_WARFARE_CANNOT_PAUSE");

	public static string GET_STRING_WARFARE_RECOVERY => NKCStringTable.GetString("SI_DP_WARFARE_RECOVERY");

	public static string GET_STRING_WARFARE_RECOVERY_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_WARFARE_RECOVERY_COUNT_ONE_PARAM");

	public static string GET_STRING_WARFARE_RECOVERY_NO_UNIT => NKCStringTable.GetString("SI_DP_WARFARE_RECOVERY_NO_UNIT");

	public static string GET_STRING_WARFARE_RECOVERY_NO_TILE => NKCStringTable.GetString("SI_DP_WARFARE_RECOVERY_NO_TILE");

	public static string GET_STRING_WARFARE_RECOVERABLE => NKCStringTable.GetString("SI_DP_WARFARE_RECOVERABLE");

	public static string GET_STRING_WARFARE_RECOVERY_CONFIRM => NKCStringTable.GetString("SI_DP_WARFARE_RECOVERY_CONFIRM");

	public static string GET_STRING_WARFARE_GAME_HUD_RESTORE_LIMIT_DESC_DAY => NKCStringTable.GetString("SI_DP_WARFARE_GAME_HUD_RESTORE_LIMIT_DESC_DAY_03");

	public static string GET_STRING_WARFARE_GAME_HUD_RESTORE_LIMIT_DESC_WEEK => NKCStringTable.GetString("SI_DP_WARFARE_GAME_HUD_RESTORE_LIMIT_DESC_WEEK_03");

	public static string GET_STRING_WARFARE_GAME_HUD_RESTORE_LIMIT_DESC_MONTH => NKCStringTable.GetString("SI_DP_WARFARE_GAME_HUD_RESTORE_LIMIT_DESC_MONTH_03");

	public static string GET_STRING_WARFARE_GAEM_HUD_RESTORE_LIMIT_OVER_DESC => NKCStringTable.GetString("SI_DP_WARFARE_GAEM_HUD_RESTORE_LIMIT_OVER_DESC");

	public static string GET_STRING_WARFARE_GAME_HUD_OPERATION_START => NKCStringTable.GetString("SI_DP_WARFARE_GAME_HUD_OPERATION_START");

	public static string GET_STRING_WARFARE_GAME_HUD_OPERATION_RESTORE => NKCStringTable.GetString("SI_DP_WARFARE_GAME_HUD_OPERATION_RESTORE");

	public static string GET_STRING_WARFARE_SUPPLY_DESC_MULTIPLY => NKCStringTable.GetString("SI_DP_MULTIPLY_OPERATION_SUPPLY_POPUP_DESC");

	public static string GET_STRING_WARFARE_WARNING_GIVE_UP_MULTIPLY => NKCStringTable.GetString("SI_DP_MULTIPLY_OPERATION_WARFARE_OUT_POPUP_DESC");

	public static string GET_STRING_DIVE_RESET => NKCStringTable.GetString("SI_DP_DIVE_RESET");

	public static string GET_STRING_DIVE_RESET_CONFIRM => NKCStringTable.GetString("SI_DP_DIVE_RESET_CONFIRM");

	public static string GET_STRING_DIVE_READY_FIRST_REWARD => NKCStringTable.GetString("SI_DP_DIVE_READY_FIRST_REWARD");

	public static string GET_STRING_DIVE_READY_EXPLORE_REWARD => NKCStringTable.GetString("SI_DP_DIVE_READY_EXPLORE_REWARD");

	public static string GET_STRING_DIVE_READY_SAFE_MINING_REWARD => NKCStringTable.GetString("SI_PF_DIVE_SAFE_MINING_REWARD");

	public static string GET_STRING_DIVE_ARTIFACT_EXCHANGE_TOTAL_GET_ITEM => NKCStringTable.GetString("SI_DP_DIVE_ARTIFACT_EXCHANGE_TOTAL_GET_ITEM");

	public static string GET_STRING_DIVE_REMAIN_TIME_TO_RESET => NKCStringTable.GetString("SI_DP_DIVE_REMAIN_TIME_TO_RESET");

	public static string GET_STRING_DIVE_RESULT_GAME_TIP => NKCStringTable.GetString("SI_DP_DIVE_RESULT_GAME_TIP1") + NKCStringTable.GetString("SI_DP_DIVE_RESULT_GAME_TIP2") + NKCStringTable.GetString("SI_DP_DIVE_RESULT_GAME_TIP3");

	public static string GET_STRING_DIVE => NKCStringTable.GetString("SI_DP_DIVE");

	public static string GET_STRING_DIVE_READY => NKCStringTable.GetString("SI_DP_DIVE_READY");

	public static string GET_STRING_DIVE_GIVE_UP => NKCStringTable.GetString("SI_DP_DIVE_GIVE_UP");

	public static string GET_STRING_DIVE_WARNING_SUPPLY => NKCStringTable.GetString("SI_DP_DIVE_NONE_BULLET_POPUP_DESC");

	public static string GET_STRING_DIVE_FLOOR_LEVEL_ONE_PARAM => NKCStringTable.GetString("SI_DP_DIVE_FLOOR_LEVEL_ONE_PARAM");

	public static string GET_STRING_DIVE_LEFT_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_DIVE_LEFT_COUNT_ONE_PARAM");

	public static string GET_STRING_DIVE_SQUAD_NO_EXIST_HP => NKCStringTable.GetString("SI_DP_DIVE_SQUAD_NO_EXIST_HP");

	public static string GET_STRING_DIVE_SQUAD_NO_EXIST_SUPPLY => NKCStringTable.GetString("SI_DP_DIVE_SQUAD_NO_EXIST_SUPPLY");

	public static string GET_STRING_DIVE_EVENT_POPUP => NKCStringTable.GetString("SI_DP_DIVE_EVENT_POPUP");

	public static string GET_STRING_DIVE_NO_SELECT_DECK => NKCStringTable.GetString("SI_DP_DIVE_NO_SELECT_DECK");

	public static string GET_STRING_DIVE_NO_EXIST_COST => NKCStringTable.GetString("SI_DP_DIVE_NO_EXIST_COST");

	public static string GET_STRING_DIVE_NO_ENOUGH_DECK => NKCStringTable.GetString("SI_DP_DIVE_NO_ENOUGH_DECK");

	public static string GET_STRING_SELECT_SQUAD => NKCStringTable.GetString("SI_DP_SELECT_SQUAD");

	public static string GET_STRING_DIVE_GIVE_UP_AND_START => NKCStringTable.GetString("SI_DP_DIVE_GIVE_UP_AND_START");

	public static string GET_STRING_DIVE_NO_EXIST_SQUAD => NKCStringTable.GetString("SI_DP_DIVE_NO_EXIST_SQUAD");

	public static string GET_STRING_DIVE_GO => NKCStringTable.GetString("SI_DP_DIVE_GO");

	public static string GET_STRING_DIVE_ARTIFACT_GET_SKIP_CHECK_REQ => NKCStringTable.GetString("SI_DP_DIVE_ARTIFACT_GET_SKIP_CHECK_REQ");

	public static string GET_STRING_DIVE_ARTIFACT_ALREADY_FULL => NKCStringTable.GetString("SI_DP_DIVE_ARTIFACT_ALREADY_FULL");

	public static string GET_STRING_DIVE_GIVE_UP_RECOMMEND => NKCStringTable.GetString("SI_DP_DIVE_RETREAT_POPUP_DESC");

	public static string GET_STRING_DIVE_SAFE_MINING => NKCStringTable.GetString("SI_PF_DIVE_SAFE_MINING");

	public static string GET_STRING_DIVE_SAFE_MINING_START => NKCStringTable.GetString("SI_PF_DIVE_SAFE_MINING_START");

	public static string GET_STRING_DIVE_SAFE_MINING_ON => NKCStringTable.GetString("SI_PF_DIVE_SAFE_MINING_ON");

	public static string GET_STRING_DIVE_SAFE_MINING_RESULT => NKCStringTable.GetString("SI_PF_DIVE_SAFE_MINING_RESULT");

	public static string GET_STRING_MENU_NAME_DUNGEON_POPUP => "DungeonPopup";

	public static string GET_STRING_ENEMY_LIST_TITLE => NKCStringTable.GetString("SI_DP_ENEMY_LIST_TITLE");

	public static string GET_STRING_ENEMY_LIST_DESC => NKCStringTable.GetString("SI_DP_ENEMY_LIST_DESC");

	public static string GET_STRING_OPERATION_POPUP_BUTTON => NKCStringTable.GetString("SI_DP_OPERATION_POPUP_BUTTON");

	public static string GET_STRING_OPERATION_POPUP_BUTTON_PLAYING => NKCStringTable.GetString("SI_DP_OPERATION_POPUP_BUTTON_PLAYING");

	public static string GET_STRING_DUNGEON_LEVEL_ONE_PARAM => NKCStringTable.GetString("SI_DP_DUNGEON_LEVEL_ONE_PARAM");

	public static string GET_STRING_DUNGEON_MISSION_COST_FAIL_ONE_PARAM => NKCStringTable.GetString("SI_DP_DUNGEON_MISSION_COST_FAIL_ONE_PARAM");

	public static string GET_STRING_DUNGEON_MISSION_COST_WARNING_THREE_PARAM => NKCStringTable.GetString("SI_DP_DUNGEON_MISSION_COST_WARNING_THREE_PARAM");

	public static string GET_STRING_DUNGEON_MISSION_COST_THREE_PARAM => NKCStringTable.GetString("SI_DP_DUNGEON_MISSION_COST_THREE_PARAM");

	public static string GET_STRING_DUNGEON_MISSION_TIME_FAIL_ONE_PARAM => NKCStringTable.GetString("SI_DP_DUNGEON_MISSION_TIME_FAIL_ONE_PARAM");

	public static string GET_STRING_DUNGEON_MISSION_TIME_WARNING_THREE_PARAM => NKCStringTable.GetString("SI_DP_DUNGEON_MISSION_TIME_WARNING_THREE_PARAM");

	public static string GET_STRING_DUNGEON_MISSION_TIME_THREE_PARAM => NKCStringTable.GetString("SI_DP_DUNGEON_MISSION_TIME_THREE_PARAM");

	public static string GET_STRING_DUNGEON_MISSION_HP_FAIL_ONE_PARAM => NKCStringTable.GetString("SI_DP_DUNGEON_MISSION_HP_FAIL_ONE_PARAM");

	public static string GET_STRING_DUNGEON_MISSION_HP_WARNING_TWO_PARAM => NKCStringTable.GetString("SI_DP_DUNGEON_MISSION_HP_WARNING_TWO_PARAM");

	public static string GET_STRING_DUNGEON_MISSION_HP_TWO_PARAM => NKCStringTable.GetString("SI_DP_DUNGEON_MISSION_HP_TWO_PARAM");

	public static string GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_DAY_02 => NKCStringTable.GetString("SI_DP_POPUP_DUNGEON_ENTER_LIMIT_DESC_ACTIVE_02");

	public static string GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_WEEK_02 => NKCStringTable.GetString("SI_DP_POPUP_DUNGEON_ENTER_LIMIT_DESC_WEEK_02");

	public static string GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_MONTH_02 => NKCStringTable.GetString("SI_DP_POPUP_DUNGEON_ENTER_LIMIT_DESC_MONTH_02");

	public static string GET_STRING_ENTER_LIMIT_OVER => NKCStringTable.GetString("SI_DP_ENTER_LIMIT_OVER");

	public static string GET_STRING_ACT_DUNGEON_SLOT_FIGHT_POWER_DESC => NKCStringTable.GetString("SI_DP_ACT_DUNGEON_SLOT_FIGHT_POWER_DESC");

	public static string GET_STRING_POPUP_DUNGEON_GET_MAIN_REWARD => NKCStringTable.GetString("SI_DP_POPUP_DUNGEON_GET_MAIN_REWARD");

	public static string GET_STRING_INGAME_NUSE_ONLY_NO_DISPEL_CODE => "◇";

	public static string GET_STRING_INGAME_UNIT_COUNT_MAX_SAME_TIME => NKCStringTable.GetString("SI_DP_INGAME_UNIT_COUNT_MAX_SAME_TIME");

	public static string GET_STRING_INGAME_USER_A_NAME_TWO_PARAM => NKCStringTable.GetString("SI_DP_INGAME_USER_A_NAME_TWO_PARAM");

	public static string GET_STRING_INGAME_USER_B_NAME_TWO_PARAM => NKCStringTable.GetString("SI_DP_INGAME_USER_B_NAME_TWO_PARAM");

	public static string GET_STRING_INGAME_USER_B_LEVEL_ONE_PARAM => GET_STRING_LEVEL_ONE_PARAM;

	public static string GET_STRING_INGAME_TEAM_A_NAME_TWO_PARAM => NKCStringTable.GetString("SI_DP_INGAME_TEAM_A_NAME_TWO_PARAM");

	public static string GET_STRING_INGAME_TEAM_B_NAME_TWO_PARAM => NKCStringTable.GetString("SI_DP_INGAME_TEAM_B_NAME_TWO_PARAM");

	public static string GET_STRING_INGAME_RESPAWN_FAIL_STATE => NKCStringTable.GetString("SI_DP_INGAME_RESPAWN_FAIL_STATE");

	public static string GET_STRING_INGAME_RESPAWN_FAIL_ALREADY_SPAWN => NKCStringTable.GetString("SI_DP_INGAME_RESPAWN_FAIL_ALREADY_SPAWN");

	public static string GET_STRING_INGAME_RESPAWN_FAIL_UNIT_DATA => NKCStringTable.GetString("SI_DP_INGAME_RESPAWN_FAIL_UNIT_DATA");

	public static string GET_STRING_INGAME_RESPAWN_FAIL_COST => NKCStringTable.GetString("SI_DP_INGAME_RESPAWN_FAIL_COST");

	public static string GET_STRING_INGAME_RESPAWN_FAIL_MAP => NKCStringTable.GetString("SI_DP_INGAME_RESPAWN_FAIL_MAP");

	public static string GET_STRING_INGAME_SHIP_SKILL_FAIL_STATE => NKCStringTable.GetString("SI_DP_INGAME_SHIP_SKILL_FAIL_STATE");

	public static string GET_STRING_INGAME_SHIP_SKILL_FAIL_DIE => NKCStringTable.GetString("SI_DP_INGAME_SHIP_SKILL_FAIL_DIE");

	public static string GET_STRING_INGAME_SHIP_SKILL_FAIL_USE_OTHER_SKILL => NKCStringTable.GetString("SI_DP_INGAME_SHIP_SKILL_FAIL_USE_OTHER_SKILL");

	public static string GET_STRING_INGAME_SHIP_SKILL_FAIL_COOLTIME => NKCStringTable.GetString("SI_DP_INGAME_SHIP_SKILL_FAIL_COOLTIME");

	public static string GET_STRING_INGAME_SHIP_SKILL_FAIL_SILENCE => NKCStringTable.GetString("SI_DP_INGAME_SHIP_SKILL_FAIL_SILENCE");

	public static string GET_STRING_INGAME_SHIP_SKILL_FAIL_SLEEP => NKCStringTable.GetString("SI_DP_INGAME_SHIP_SKILL_FAIL_SLEEP");

	public static string GET_STRING_DANGER_MSG_UNIT_RESPAWN => NKCStringTable.GetString("SI_DP_DUNGEON_NO_UNIT_WARNING");

	public static string GET_STRING_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_COUNT_ONE_PARAM");

	public static string GET_STRING_TIME => NKCStringTable.GetString("SI_DP_TIME");

	public static string GET_STRING_TIME_PERIOD => NKCStringTable.GetString("SI_DP_TIME_PERIOD");

	public static string GET_STRING_TIME_DAY => NKCStringTable.GetString("SI_DP_TIME_DAY");

	public static string GET_STRING_TIME_HOUR => NKCStringTable.GetString("SI_DP_TIME_HOUR");

	public static string GET_STRING_TIME_MINUTE => NKCStringTable.GetString("SI_DP_TIME_MINUTE");

	public static string GET_STRING_TIME_SECOND => NKCStringTable.GetString("SI_DP_TIME_SECOND");

	public static string GET_STRING_TIME_DAY_ONE_PARAM => NKCStringTable.GetString("SI_DP_TIME_DAY_ONE_PARAM");

	public static string GET_STRING_TIME_HOUR_ONE_PARAM => NKCStringTable.GetString("SI_DP_TIME_HOUR_ONE_PARAM");

	public static string GET_STRING_TIME_MINUTE_ONE_PARAM => NKCStringTable.GetString("SI_DP_TIME_MINUTE_ONE_PARAM");

	public static string GET_STRING_TIME_SECOND_ONE_PARAM => NKCStringTable.GetString("SI_DP_TIME_SECOND_ONE_PARAM");

	public static string GET_STRING_TIME_REMAIN_DAY_ONE_PARAM => NKCStringTable.GetString("SI_DP_TIME_REMAIN_DAY_ONE_PARAM");

	public static string GET_STRING_TIME_REMAIN_HOUR_ONE_PARAM => NKCStringTable.GetString("SI_DP_TIME_REMAIN_HOUR_ONE_PARAM");

	public static string GET_STRING_TIME_REMAIN_MINUTE_ONE_PARAM => NKCStringTable.GetString("SI_DP_TIME_REMAIN_MINUTE_ONE_PARAM");

	public static string GET_STRING_TIME_REMAIN_SECOND_ONE_PARAM => NKCStringTable.GetString("SI_DP_TIME_REMAIN_SECOND_ONE_PARAM");

	public static string GET_STRING_TIME_REMAIN_SHOP_DAY_OVER => NKCStringTable.GetString("SI_DP_TIME_REMAIN_SHOP_DAY_OVER");

	public static string GET_STRING_TIME_REMAIN_SHOP_EXPIRE_TODAY => NKCStringTable.GetString("SI_DP_TIME_REMAIN_SHOP_EXPIRE_TODAY");

	public static string GET_STRING_TIME_CLOSING => NKCStringTable.GetString("SI_DP_TIME_CLOSING");

	public static string GET_STRING_TIME_DAY_AGO_ONE_PARAM => NKCStringTable.GetString("SI_DP_TIME_DAY_AGO_ONE_PARAM");

	public static string GET_STRING_TIME_HOUR_AGO_ONE_PARAM => NKCStringTable.GetString("SI_DP_TIME_HOUR_AGO_ONE_PARAM");

	public static string GET_STRING_TIME_MINUTE_AGO_ONE_PARAM => NKCStringTable.GetString("SI_DP_TIME_MINUTE_AGO_ONE_PARAM");

	public static string GET_STRING_TIME_SECOND_AGO_ONE_PARAM => NKCStringTable.GetString("SI_DP_TIME_SECOND_AGO_ONE_PARAM");

	public static string GET_STRING_TIME_A_SECOND_AGO => NKCStringTable.GetString("SI_DP_TIME_A_SECOND_AGO");

	public static string GET_STRING_DATE_FOUR_PARAM => NKCStringTable.GetString("SI_DP_DATE_FOUR_PARAM");

	public static string GET_STRING_TIME_NO_LIMIT => NKCStringTable.GetString("SI_DP_TIME_NO_LIMIT");

	public static string GET_STRING_TIME_DAY_HOUR_TWO_PARAM => NKCStringTable.GetString("SI_DP_TIME_DAY_HOUR_TWO_PARAM");

	public static string GET_STRING_TIME_HOUR_MINUTE_TWO_PARAM => NKCStringTable.GetString("SI_DP_TIME_HOUR_MINUTE_TWO_PARAM");

	public static string GET_STRING_SEASON_TIME_UP_TO_END_ONE_PARAM => NKCStringTable.GetString("SI_DP_SEASON_TIME_UP_TO_END_ONE_PARAM");

	public static string GET_STRING_REMAIN_TIME_LEFT_ONE_PARAM => NKCStringTable.GetString("SI_PF_REMAIN_TIME_LEFT_ONE_PARAM");

	public static string GET_STRING_EVENT_DATE_UNLIMITED_TEXT => NKCStringTable.GetString("SI_PF_EVENT_DATE_LIMITED_TEXT");

	public static string GET_STRING_SHOP => NKCStringTable.GetString("SI_DP_SHOP");

	public static string GET_STRING_SHOP_NEXT_REFRESH_ONE_PARAM => NKCStringTable.GetString("SI_DP_SHOP_NEXT_REFRESH_ONE_PARAM");

	public static string GET_STRING_SHOP_REMAIN_NUMBER_TWO_PARAM => NKCStringTable.GetString("SI_DP_SHOP_REMAIN_NUMBER_TWO_PARAM");

	public static string GET_STRING_SHOP_SUPPLY_LIST_INSTANTLY_REFRESH_REQ => NKCStringTable.GetString("SI_DP_SHOP_SUPPLY_LIST_INSTANTLY_REFRESH_REQ");

	public static string GET_STRING_SHOP_SKIN_INFO => NKCStringTable.GetString("SI_DP_SHOP_SKIN_INFO");

	public static string GET_STRING_SHOP_PACKAGE_INFO => NKCStringTable.GetString("SI_DP_SHOP_PACKAGE_INFO");

	public static string GET_STRING_SHOP_PURCHASE => NKCStringTable.GetString("SI_DP_SHOP_PURCHASE");

	public static string GET_STRING_SHOP_ACCOUNT_PURCHASE_COUNT_TWO_PARAM => NKCStringTable.GetString("SI_DP_SHOP_ACCOUNT_PURCHASE_COUNT_TWO_PARAM");

	public static string GET_STRING_SHOP_DAY_PURCHASE_COUNT_TWO_PARAM => NKCStringTable.GetString("SI_DP_SHOP_DAY_PURCHASE_COUNT_TWO_PARAM");

	public static string GET_STRING_SHOP_MONTH_PURCHASE_COUNT_TWO_PARAM => NKCStringTable.GetString("SI_DP_SHOP_MONTH_PURCHASE_COUNT_TWO_PARAM");

	public static string GET_STRING_SHOP_WEEK_PURCHASE_COUNT_TWO_PARAM => NKCStringTable.GetString("SI_DP_SHOP_WEEK_PURCHASE_COUNT_TWO_PARAM");

	public static string GET_STRING_SHOP_FIRST_PURCHASE => NKCStringTable.GetString("SI_DP_SHOP_FIRST_PURCHASE");

	public static string GET_STRING_SHOP_LIMIT => NKCStringTable.GetString("SI_DP_SHOP_LIMIT");

	public static string GET_STRING_SHOP_WAS_NOT_ABLE_TO_GET_PRODUCT_LIST_FROM_SERVER => NKCStringTable.GetString("SI_DP_SHOP_WAS_NOT_ABLE_TO_GET_PRODUCT_LIST_FROM_SERVER");

	public static string GET_STRING_SHOP_SUPPLY_LIST_GET_FAIL => NKCStringTable.GetString("SI_DP_SHOP_SUPPLY_LIST_GET_FAIL");

	public static string GET_STRING_PURCHASE_POPUP_TITLE => NKCStringTable.GetString("SI_DP_PURCHASE_POPUP_TITLE");

	public static string GET_STRING_PURCHASE_POPUP_DESC => NKCStringTable.GetString("SI_DP_PURCHASE_POPUP_DESC");

	public static string GET_STRING_SHOP_COMPLETE_PURCHASE => NKCStringTable.GetString("SI_DP_SHOP_FIRST_PURCHASE");

	public static string GET_STRING_SHOP_TIME_LIMIT => NKCStringTable.GetString("SI_DP_SHOP_SALE_PERIOD");

	public static string GET_STRING_SHOP_POPULAR => NKCStringTable.GetString("SI_DP_SHOP_POPULAR");

	public static string GET_STRING_SHOP_NEW => NKCStringTable.GetString("SI_DP_SHOP_NEW");

	public static string GET_STRING_SHOP_BEST => NKCStringTable.GetString("SI_DP_SHOP_BEST");

	public static string GET_STRING_SHOP_NOT_REGISTED_PRODUCT => NKCStringTable.GetString("SI_DP_SHOP_NOT_REGISTED_PRODUCT");

	public static string GET_STRING_SHOP_FREE => NKCStringTable.GetString("SI_DP_SHOP_FREE");

	public static string GET_STRING_SHOP_NOT_ENOUGH_REQUIREMENT => NKCStringTable.GetString("SI_DP_SHOP_NOT_ENOUGH_REQUIREMENT");

	public static string GET_STRING_SHOP_CHAIN_NEXT_RESET_ONE_PARAM => NKCStringTable.GetString("SI_DP_SHOP_CHAIN_NEXT_RESET_ONE_PARAM");

	public static string GET_STRING_SHOP_CHAIN_NEXT_RESET_ONE_PARAM_CLOSE => NKCStringTable.GetString("SI_DP_SHOP_CHAIN_NEXT_RESET_ONE_PARAM_CLOSE");

	public static string GET_STRING_SHOP_PURCHASE_COUNT_TWO_PARAM => NKCStringTable.GetString("SI_DP_SHOP_PURCHASE_COUNT_TWO_PARAM");

	public static string GET_STRING_SHOP_BUY_ALL_TITLE => NKCStringTable.GetString("SI_DP_SHOP_BUY_ALL_TITLE");

	public static string GET_STRING_SHOP_BUY_ALL_DESC => NKCStringTable.GetString("SI_DP_SHOP_BUY_ALL_DESC");

	public static string GET_STRING_SHOP_CHAIN_LOCKED => NKCStringTable.GetString("SI_DP_SHOP_CHAIN_LOCKED");

	public static string GET_STRING_SHOP_SPECIAL => NKCStringTable.GetString("SI_DP_SHOP_SPECIAL");

	public static string GET_STRING_SHOP_SUBSCRIBE_DAY_ENOUGH_DESC => NKCStringTable.GetString("SI_DP_SHOP_SUBSCRIBE_DAY_ENOUGH_DESC");

	public static string GET_STRING_SHOP_SKIN_STORY_MSG => NKCStringTable.GetString("SI_DP_SHOP_SKIN_STORY_MSG");

	public static string GET_STRING_SHOP_SKIN_LOGIN_CUTIN_MSG => NKCStringTable.GetString("SI_DP_SHOP_SKIN_LOGIN_CUTIN_MSG");

	public static string GET_STRING_SHOP_BUY_SHORTCUT_TITLE => NKCStringTable.GetString("SI_DP_ITEM_NOT_ENOUGH_PRODUCT_POPUP_DESC");

	public static string GET_STRING_LOGIN_NOT_READY => NKCStringTable.GetString("SI_DP_LOGIN_NOT_READY");

	public static string GET_STRING_INITIALIZE_FAILED => NKCStringTable.GetString("SI_DP_INITIALIZE_FAILED");

	public static string GET_STRING_DECONNECT_AND_GO_TITLE => NKCStringTable.GetString("SI_DP_DECONNECT_AND_GO_TITLE");

	public static string GET_STRING_TOY_LOGGED_IN_GUEST => NKCStringTable.GetString("SI_DP_TOY_LOGGED_IN_GUEST");

	public static string GET_STRING_TOY_LOGGED_IN_NEXON => NKCStringTable.GetString("SI_DP_TOY_LOGGED_IN_NEXON");

	public static string GET_STRING_TOY_LOGGED_IN_FACEBOOK => NKCStringTable.GetString("SI_DP_TOY_LOGGED_IN_FACEBOOK");

	public static string GET_STRING_TOY_LOGGED_IN_GOOGLE => NKCStringTable.GetString("SI_DP_TOY_LOGGED_IN_GOOGLE");

	public static string GET_STRING_TOY_LOGGED_IN_APPLE => NKCStringTable.GetString("SI_DP_TOY_LOGGED_IN_APPLE");

	public static string GET_STRING_TOY_GUEST_ACCOUNT_TRANSFER_BACKUP => NKCStringTable.GetString("SI_DP_TOY_GUEST_ACCOUNT_TRANSFER_BACKUP");

	public static string GET_STRING_TOY_GUEST_ACCOUNT_TRANSFER_RESTORE => NKCStringTable.GetString("SI_DP_TOY_GUEST_ACCOUNT_TRANSFER_RESTORE");

	public static string GET_STRING_ERROR_MULTIPLE_CONNECT => NKCStringTable.GetString("SI_DP_ERROR_MULTIPLE_CONNECT");

	public static string GET_STRING_ALREADY_LOGGED_IN => NKCStringTable.GetString("SI_DP_ALREADY_LOGGED_IN");

	public static string GET_STRING_FAIL_TO_PROCESS_TERMS => NKCStringTable.GetString("SI_DP_FAIL_TO_PROCESS_TERMS");

	public static string GET_STRING_TOY_IS_BEING_DISABLED => NKCStringTable.GetString("SI_DP_TOY_IS_BEING_DISABLED");

	public static string GET_STRING_TOY_LOCAL_PUSH_CONTRACT_TITLE => NKCStringTable.GetString("SI_DP_TOY_LOCAL_PUSH_CONTRACT_TITLE");

	public static string GET_STRING_TOY_LOCAL_PUSH_CONTRACT_DESCRIPTION => NKCStringTable.GetString("SI_DP_TOY_LOCAL_PUSH_CONTRACT_DESCRIPTION");

	public static string GET_STRING_TOY_LOCAL_PUSH_WORLD_MAP_TITLE => NKCStringTable.GetString("SI_DP_TOY_LOCAL_PUSH_WORLD_MAP_TITLE");

	public static string GET_STRING_TOY_LOCAL_PUSH_WORLD_MAP_DESCRIPTION => NKCStringTable.GetString("SI_DP_TOY_LOCAL_PUSH_WORLD_MAP_DESCRIPTION");

	public static string GET_STRING_TOY_LOCAL_PUSH_AUTO_SUPPLY_TITLE => NKCStringTable.GetString("SI_DP_TOY_LOCAL_PUSH_AUTO_SUPPLY_TITLE");

	public static string GET_STRING_TOY_LOCAL_PUSH_AUTO_SUPPLY_DESCRIPTION => NKCStringTable.GetString("SI_DP_TOY_LOCAL_PUSH_AUTO_SUPPLY_DESCRIPTION");

	public static string GET_STRING_TOY_LOCAL_PUSH_GEAR_CRAFT_TITLE => NKCStringTable.GetString("SI_DP_TOY_LOCAL_PUSH_GEAR_CRAFT_TITLE");

	public static string GET_STRING_TOY_LOCAL_PUSH_GEAR_CRAFT_DESCRIPTION => NKCStringTable.GetString("SI_DP_TOY_LOCAL_PUSH_GEAR_CRAFT_DESCRIPTION");

	public static string GET_STRING_TOY_LOCAL_PUSH_PVP_POINT_TITLE => NKCStringTable.GetString("SI_DP_TOY_LOCAL_PUSH_PVP_POINT_TITLE");

	public static string GET_STRING_TOY_LOCAL_PUSH_PVP_POINT_DESCRIPTION => NKCStringTable.GetString("SI_DP_TOY_LOCAL_PUSH_PVP_POINT_DESCRIPTION");

	public static string GET_STRING_ERROR_TOY_SHOP_LIST => NKCStringTable.GetString("SI_DP_ERROR_TOY_SHOP_LIST");

	public static string GET_STRING_TOY_LOCAL_PUSH_NOT_CONNECTED_TITLE => NKCStringTable.GetString("SI_DP_TOY_LOCAL_PUSH_NOT_CONNECTED_TITLE");

	public static string GET_STRING_TOY_LOCAL_PUSH_NOT_CONNECTED_DESCRIPTION => NKCStringTable.GetString("SI_DP_TOY_LOCAL_PUSH_NOT_CONNECTED_DESCRIPTION");

	public static string GET_STRING_TOY_LOGOUT_SUCCESS => NKCStringTable.GetString("SI_DP_TOY_LOGOUT_SUCCESS");

	public static string GET_STRING_TOY_COMMUNITY_CONNECT_FAIL => NKCStringTable.GetString("SI_DP_TOY_COMMUNITY_CONNECT_FAIL");

	public static string GET_STRING_TOY_SYNC_ACCOUNT_CANCEL => NKCStringTable.GetString("SI_DP_TOY_SYNC_ACCOUNT_CANCEL");

	public static string GET_STRING_TOY_SYNC_ACCOUNT_FAIL => NKCStringTable.GetString("SI_DP_TOY_SYNC_ACCOUNT_FAIL");

	public static string GET_STRING_TOY_SERVICE_WITHDRAWAL_REQ_FAIL => NKCStringTable.GetString("SI_DP_TOY_SERVICE_WITHDRAWAL_REQ_FAIL");

	public static string GET_STRING_CHANGEACCOUNT_FAIL_GUEST_ALREADY_MAPPED => NKCStringTable.GetString("SI_DP_CHANGEACCOUNT_FAIL_GUEST_ALREADY_MAPPED");

	public static string GET_STRING_CHANGEACCOUNT_SUCCESS_GUEST_SYNC => NKCStringTable.GetString("SI_DP_CHANGEACCOUNT_SUCCESS_GUEST_SYNC");

	public static string GET_STRING_AUTH_LOGIN_QUIT_USER => NKCStringTable.GetString("SI_DP_AUTH_LOGIN_QUIT_USER");

	public static string GET_STRING_TOY_BILLING_PAYMENT_FAIL => NKCStringTable.GetString("SI_DP_TOY_BILLING_PAYMENT_FAIL");

	public static string GET_STRING_TOY_BILLING_PAYMENT_FAIL_DESC_1 => NKCStringTable.GetString("SI_DP_TOY_BILLING_PAYMENT_FAIL_DESC_1");

	public static string GET_STRING_TOY_BILLING_PAYMENT_FAIL_DESC_2 => NKCStringTable.GetString("SI_DP_TOY_BILLING_PAYMENT_FAIL_DESC_2");

	public static string GET_STRING_TOY_BILLING_RESTORE_FAIL => NKCStringTable.GetString("SI_DP_TOY_BILLING_RESTORE_FAIL");

	public static string GET_STRING_TOY_BILLING_RESTORE_REQ_FAIL => NKCStringTable.GetString("SI_DP_TOY_BILLING_RESTORE_REQ_FAIL");

	public static string GET_STRING_TOY_NOT_EXIST_RESTORE_ITEM => NKCStringTable.GetString("SI_DP_TOY_NOT_EXIST_RESTORE_ITEM");

	public static string GET_STRING_TOY_BILLING_RESTORE_INVALID_WORK_DESC_1 => NKCStringTable.GetString("SI_DP_TOY_BILLING_RESTORE_INVALID_WORK_DESC_1");

	public static string GET_STRING_TOY_BILLING_RESTORE_NETWORK_ISSUE_DESC_1 => NKCStringTable.GetString("SI_DP_TOY_BILLING_RESTORE_NETWORK_ISSUE_DESC_1");

	public static string GET_STRING_TOY_BILLING_RESTORE_INVALID_WORK_DESC_2 => NKCStringTable.GetString("SI_DP_TOY_BILLING_RESTORE_INVALID_WORK_DESC_2");

	public static string GET_STRING_TOY_BILLING_RESTORE_NETWORK_ISSUE_DESC_2 => NKCStringTable.GetString("SI_DP_TOY_BILLING_RESTORE_NETWORK_ISSUE_DESC_2");

	public static string GET_STRING_TOY_BACKUP_CODE_REQ_CANCEL => NKCStringTable.GetString("SI_DP_TOY_BACKUP_CODE_REQ_CANCEL");

	public static string GET_STRING_TOY_GUEST_ACCOUT_RESTORE_FAIL => NKCStringTable.GetString("SI_DP_TOY_GUEST_ACCOUT_RESTORE_FAIL");

	public static string GET_STRING_TOY_NEXON_UNREGISTER_FAIL => NKCStringTable.GetString("SI_DP_TOY_NEXON_UNREGISTER_FAIL");

	public static string GET_STRING_TOY_USER_INFO_UPDATE_FAIL => NKCStringTable.GetString("SI_DP_TOY_USER_INFO_UPDATE_FAIL");

	public static string GET_STRING_TOY_RE_TRY_TITLE => NKCStringTable.GetString("SI_DP_TOY_RE_TRY_TITLE");

	public static string GET_STRING_TOY_RE_TRY_DESC => NKCStringTable.GetString("SI_DP_TOY_RE_TRY_DESC");

	public static string GET_STRING_TOY_LOGGED_IN_GUEST_KOR => NKCStringTable.GetString("SI_DP_TOY_LOGGED_IN_GUEST_KOR");

	public static string GET_STRING_TOY_CONNECTED_ACCOUNT => NKCStringTable.GetString("SI_DP_TOY_CONNECTED_ACCOUNT");

	public static string GET_STRING_TOY_CUSTOMER_CENTER_RESPOND => NKCStringTable.GetString("SI_DP_TOY_CUSTOMER_CENTER_RESPOND");

	public static string GET_STRING_TOY_LOGIN_CHANGE_ACCOUNT => NKCStringTable.GetString("SI_DP_TOY_LOGIN_CHANGE_ACCOUNT");

	public static string GET_STRING_TOY_LOGIN_NO_AUTH => NKCStringTable.GetString("SI_DP_TOY_LOGIN_NO_AUTH");

	public static string GET_STRING_LOBBY_CHECK_QUIT_GAME => NKCStringTable.GetString("SI_DP_LOBBY_CHECK_QUIT_GAME");

	public static string GET_STRING_LOBBY_CONTRACT_COMPLETED => NKCStringTable.GetString("SI_DP_LOBBY_CONTRACT_COMPLETED");

	public static string GET_STRING_LOBBY_CONTRACT_PROGRESSING => NKCStringTable.GetString("SI_DP_LOBBY_CONTRACT_PROGRESSING");

	public static string GET_STRING_LOBBY_USER_BUFF_NONE => NKCStringTable.GetString("SI_DP_LOBBY_USER_BUFF_NONE");

	public static string GET_STRING_LOBBY_UNIT_CAPTAIN => NKCStringTable.GetString("SI_PF_SLOT_CARD_LOBBY_CAPTAIN");

	public static string GET_STRING_LOBBY_BG_SELECT_CAPTAIN => NKCStringTable.GetString("SI_PF_LOBBY_BACKGROUND_CAPTAIN_TEXT");

	public static string GET_STRING_LOBBY_CITY_MISSION_ONGOING => NKCStringTable.GetString("SI_LOBBY_RIGHT_MENU_1_WORLDMAP_PROGRESS_TEXT");

	public static string GET_STRING_LOBBY_CITY_MISSION_COMPLETE => NKCStringTable.GetString("SI_LOBBY_RIGHT_MENU_1_WORLDMAP_PROGRESS_COMPLETE_TEXT");

	public static string GET_STRING_LOBBY_FIERCEBATTLE_TIME_REMAIN => NKCStringTable.GetString("SI_LOBBY_FIERCEBATTLE_TIME_REMAIN");

	public static string GET_STRING_MISSION_SHORTCUT_FAIL => NKCStringTable.GetString("SI_DP_MISSION_SHORTCUT_FAIL");

	public static string GET_STRING_MISSION_EXPIRED => NKCStringTable.GetString("SI_DP_MISSION_EXPIRED");

	public static string GET_STRING_MISSION_COMPLETE => NKCStringTable.GetString("SI_DP_MISSION_COMPLETE");

	public static string GET_STRING_MISSION_COMPLETE_ONE_PARAM => NKCStringTable.GetString("SI_DP_MISSION_COMPLETE_ONE_PARAM");

	public static string GET_STRING_MISSION => NKCStringTable.GetString("SI_DP_MISSION");

	public static string GET_STRING_MISSION_RESET_INTERVAL_DAILY => NKCStringTable.GetString("SI_DP_MISSION_RESET_INTERVAL_DAILY");

	public static string GET_STRING_MISSION_RESET_INTERVAL_WEEKLY => NKCStringTable.GetString("SI_DP_MISSION_RESET_INTERVAL_WEEKLY");

	public static string GET_STRING_MISSION_RESET_INTERVAL_MONTHLY => NKCStringTable.GetString("SI_DP_MISSION_RESET_INTERVAL_MONTHLY");

	public static string GET_STRING_MISSION_REMAIN_TWO_PARAM => NKCStringTable.GetString("SI_DP_MISSION_REMAIN_TWO_PARAM");

	public static string GET_STRING_MISSION_COMPLETE_GROWTH_TAB => NKCStringTable.GetString("SI_DP_MISSION_COMPLETE_GROWTH_TAB");

	public static string GET_STRING_MISSION_LOCK_GROWTH_TAB => NKCStringTable.GetString("SI_DP_MISSION_LOCK_GROWTH_TAB");

	public static string GET_STRING_MISSION_TAB_GROWTH_01 => NKCStringTable.GetString("SI_DP_MISSION_TAB_GROWTH_01");

	public static string GET_STRING_MISSION_TAB_GROWTH_02 => NKCStringTable.GetString("SI_DP_MISSION_TAB_GROWTH_02");

	public static string GET_STRING_MISSION_TAB_GROWTH_03 => NKCStringTable.GetString("SI_DP_MISSION_TAB_GROWTH_03");

	public static string GET_STRING_MISSION_UNAVAILABLE => NKCStringTable.GetString("SI_DP_MISSION_UNAVAILABLE");

	public static string GET_STRING_MISSION_NEED_GROWTH_ALL_COMPLETE => NKCStringTable.GetString("SI_DP_MISSION_NEED_GROWTH_ALL_COMPLETE");

	public static string GET_STRING_MISSION_ONE_PARAM => NKCStringTable.GetString("SI_DP_MISSION_ONE_PARAM");

	public static string GET_STRING_GAUNTLET_OPEN_RANK_MODE => NKCStringTable.GetString("SI_DP_GAUNTLET_OPEN_RANK_MODE");

	public static string GET_STRING_GAUNTLET_NOT_OPEN_RANK_MODE => NKCStringTable.GetString("SI_DP_GAUNTLET_NOT_OPEN_RANK_MODE");

	public static string GET_STRING_GAUNTLET_LEAGUE_TAG_CLOSE => NKCStringTable.GetString("SI_PF_GAUNTLET_LEAGUE_TAG_CLOSE");

	public static string GET_STRING_GAUNTLET_LEAGUE_TAG_CLOSE_MESSAGE => NKCStringTable.GetString("SI_PF_GAUNTLET_LEAGUE_TAG_CLOSE_MESSAGE");

	public static string GET_STRING_GAUNTLET_ASYNC_TICKET_USE_POPUP_TEXT => NKCStringTable.GetString("SI_GAUNTLET_ASYNC_TICKET_USE_POPUP_TEXT");

	public static string GET_STRING_GAUNTLET_MATCHING_FAIL_ALARM => NKCStringTable.GetString("SI_DP_GAUNTLET_MATCHING_FAIL_ALARM");

	public static string GET_STRING_GAUNTLET_BAN_LEVEL_ONE_PARAM => NKCStringTable.GetString("SI_DP_GAUNTLET_BAN_LEVEL");

	public static string GET_STRING_GAUNTLET_UP_LEVEL_ONE_PARAM => NKCStringTable.GetString("SI_DP_GAUNTLET_UP_LEVEL");

	public static string GET_STRING_GAUNTLET_BAN_APPLY_DESC_ONE_PARAM => NKCStringTable.GetString("SI_DP_GAUNTLET_BAN_DEBUFF_SHIP");

	public static string GET_STRING_GAUNTLET => NKCStringTable.GetString("SI_DP_GAUNTLET");

	public static string GET_STRING_GAUNTLET_DEMOTE_WARNING => NKCStringTable.GetString("SI_DP_GAUNTLET_DEMOTE_WARNING");

	public static string GET_STRING_GAUNTLET_THIS_WEEK_LEAGUE_ONE_PARAM => NKCStringTable.GetString("SI_DP_GAUNTLET_THIS_WEEK_LEAGUE_ONE_PARAM");

	public static string GET_STRING_GAUNTLET_LEAUGE_GUIDE => NKCStringTable.GetString("SI_DP_GAUNTLET_LEAUGE_GUIDE");

	public static string GET_STRING_GAUNTLET_SEASON_TITLE_ONE_PARAM => NKCStringTable.GetString("SI_DP_GAUNTLET_SEASON_TITLE_ONE_PARAM");

	public static string GET_STRING_GAUNTLET_SEASON_LEAUGE_DESC => NKCStringTable.GetString("SI_DP_GAUNTLET_SEASON_LEAUGE_DESC");

	public static string GET_STRING_GAUNTLET_WEEK_LEAGUE => NKCStringTable.GetString("SI_DP_GAUNTLET_WEEK_LEAGUE");

	public static string GET_STRING_GAUNTLET_WEEK_LEAGUE_DESC => NKCStringTable.GetString("SI_DP_GAUNTLET_WEEK_LEAGUE_DESC");

	public static string GET_STRING_GAUNTLET_BATTLE_RECORD => NKCStringTable.GetString("SI_DP_GAUNTLET_BATTLE_RECORD");

	public static string GET_STRING_GAUNTLET_WIN_STREAK_ONE_PARAM => NKCStringTable.GetString("SI_DP_GAUNTLET_WIN_STREAK_ONE_PARAM");

	public static string GET_STRING_GAUNTLET_SEASON_NUMBERING_NAME => NKCStringTable.GetString("SI_DP_GAUNTLET_SEASON_NUMBERING_NAME");

	public static string GET_STRING_GAUNTLET_WIN_COUNT => NKCStringTable.GetString("SI_DP_GAUNTLET_WIN_COUNT");

	public static string GET_STRING_GAUNTLET_RANK_GAME => NKCStringTable.GetString("SI_DP_GAUNTLET_RANK_GAME");

	public static string GET_STRING_GAUNTLET_NORMAL_GAME => NKCStringTable.GetString("SI_DP_GAUNTLET_NORMAL_GAME");

	public static string GET_STRING_GAUNTLET_SEASON_REWARD => NKCStringTable.GetString("SI_POPUP_SEASON_RESULT_TITLE");

	public static string GET_STRING_GAUNTLET_WEEKLY_REWARD => NKCStringTable.GetString("SI_POPUP_WEEKLY_RESULT_TITLE");

	public static string GET_STRING_GAUNTLET_RANK_HELP_DESC => NKCStringTable.GetString("SI_DP_GAUNTLET_INFO_DESC");

	public static string GET_STRING_GAUNTLET_ASYNC_SEASON_LEAGUE_DESC => NKCStringTable.GetString("SI_DP_GAUNTLET_ASYNC_SEASON_LEAUGE_DESC");

	public static string GET_STRING_GAUNTLET_ASYNC_WEEK_LEAGUE_DESC => NKCStringTable.GetString("SI_DP_GAUNTLET_ASYNC_WEEK_LEAGUE_DESC");

	public static string GET_STRING_GAUNTLET_ASYNC_HELP_DESC => NKCStringTable.GetString("SI_DP_GAUNTLET_ASYNC_NEW_INFO_LEAGUE_DESC");

	public static string GET_STRING_GAUNTLET_LEAGUE_SEASON_LEAGUE_DESC => NKCStringTable.GetString("SI_DP_GAUNTLET_SEASON_LEAUGE_MODE_DESC");

	public static string GET_STRING_GAUNTLET_LEAGUE_HELP_DESC => NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_INFO_DESC");

	public static string GET_STRING_FIREND_NO_EXIST_ASYNC_LOG => NKCStringTable.GetString("SI_PF_GAUNTLET_HAVE_NO_DEFENSE_DECK");

	public static string GET_STRING_GAUNTLET_RANK_NO_JOIN => NKCStringTable.GetString("SI_DP_GAUNTLET_RANK_NO_JOIN");

	public static string GET_STRING_GAUNTLET_THIS_SEASON_LEAGUE_BEING_EVALUATED => NKCStringTable.GetString("SI_DP_GAUNTLET_THIS_SEASON_LEAGUE_BEING_EVALUATED");

	public static string GET_STRING_GAUNTLET_THIS_SEASON_LEAGUE_REMAIN_TIME_ONE_PARAM => NKCStringTable.GetString("SI_DP_GAUNTLET_THIS_SEASON_LEAGUE_REMAIN_TIME_ONE_PARAM");

	public static string GET_STRING_GAUNTLET_ASYNC_GAME_READY => NKCStringTable.GetString("SI_DP_GAUNTLET_ASYNC_GAME_READY");

	public static string GET_STRING_GAUNTLET_ASYNC_GAME_START => NKCStringTable.GetString("SI_DP_GAUNTLET_ASYNC_GAME_START");

	public static string GET_STRING_GAUNTLET_ASYNC_GAME => NKCStringTable.GetString("SI_DP_GAUNTLET_ASYNC_GAME");

	public static string GET_STRING_GAUNTLET_ASYNC_LOCK_DEFENSE_DECK => NKCStringTable.GetString("SI_PF_GAUNTLET_NEED_DEFENSE_DECK_DESC");

	public static string GET_STRING_GAUNTLET_ASYNC_LOCK_CLOSING => NKCStringTable.GetString("SI_PF_GAUNTLET_ASYNC_TIME_CLOSING");

	public static string GET_STRING_GAUNTLET_LEAGUE_MATCH_OTHER_PLAYER_CANCEL => NKCStringTable.GetString("SI_PF_PVP_SURRENDER_SELECT_NOTICE");

	public static string GET_STRING_GAUNTLET_SELECT_IMPOSSIBLE => NKCStringTable.GetString("SI_GAUNTLET_LOBBY_CASTING_BAN_SELECT_IMPOSSIBLE");

	public static string GET_STRING_GAUNTLET_CASTING_BAN_SELECT_UNIT => NKCStringTable.GetString("SI_PVP_CASTING_BAN_SELECT_UNIT_START");

	public static string GET_STRING_GAUNTLET_CASTING_BAN_SELECT_SHIP => NKCStringTable.GetString("SI_PVP_CASTING_BAN_SELECT_SHIP_START");

	public static string GET_STRING_GAUNTLET_CASTING_BAN_SELECT_OPER => NKCStringTable.GetString("SI_PVP_CASTING_BAN_SELECT_OPR_START");

	public static string GET_STRING_GAUNTLET_CASTING_BAN_SELECT_LIST_TITLE => NKCStringTable.GetString("SI_PVP_CASTING_BAN_SELECT");

	public static string GET_STRING_GAUNTLET_THIS_WEEK_LEAGUE_CASTING_BEN_ONE_PARAM => NKCStringTable.GetString("SI_PVP_CASTING_BAN_SELECT_FINISH_DATE");

	public static string GET_STRING_GAUNTLET_BAN_POPUP_DESC_UNIT => NKCStringTable.GetString("SI_DP_GAUNTLET_BAN_POPUP_DESC_UNIT");

	public static string GET_STRING_GAUNTLET_BAN_POPUP_DESC_SHIP => NKCStringTable.GetString("SI_DP_GAUNTLET_BAN_POPUP_DESC_SHIP");

	public static string GET_STRING_GAUNTLET_BAN_POPUP_DESC_OPER => NKCStringTable.GetString("SI_DP_GAUNTLET_BAN_POPUP_DESC_OPR");

	public static string GET_STRING_GAUNTLET_BAN_POPUP_DESC_UP => NKCStringTable.GetString("SI_DP_GAUNTLET_UP_POPUP_DESC_UNIT");

	public static string GET_STRING_GAUNTLET_CASTING_BAN_SELECT_COMPLET => NKCStringTable.GetString("SI_PVP_CASTING_BAN_SELECT_COMPLETE");

	public static string GET_STRING_PVP_CASTING_BAN_SELECT_STATUS_SUB_TEXT => NKCStringTable.GetString("SI_PVP_CASTING_BAN_SELECT_STATUS_SUB_TEXT");

	public static string GET_STRING_GAUNTLET_ASYNC_NPC_BLOCK_DESC => NKCStringTable.GetString("SI_PF_ASYNC_NEW_NPC_NOTICE_DESC");

	public static string GET_STRING_GAUNTLET_DECK_UNIT_NOT_ALL_EQUIPED_GEAR_DESC => NKCStringTable.GetString("SI_PVP_POPUP_UNIT_NON_EQUIP");

	public static string GET_STRING_GAUNTLET_EVENTMATCH_CANNOT_ENTER => NKCStringTable.GetString("SI_DP_EVENTMATCH_POPUP_NOT_ENTER");

	public static string GET_STRING_GAUNTLET_EVENTMATCH_REWARD_NONE => NKCStringTable.GetString("SI_DP_EVENTMATCH_POPUP_REWARD_NOT_COUNT");

	public static string GET_STRING_GAUNTLET_EVENTMATCH_EVENTDECK_DESC => NKCStringTable.GetString("SI_DP_EVENTMATCH_EVENTDECK_LIMIT_COUNT");

	public static string GET_STRING_GAUNTLET_EVENT_GAME => NKCStringTable.GetString("SI_PF_EVENTMATCH");

	public static string GET_STRING_GAUNTLET_EVENT_UNIT_DETAIL_INFO_NOT_POSSIBLE => NKCStringTable.GetString("SI_PF_EVENTMATCH_POPUP_NO_ENTER_INFO_DESC");

	public static string GET_STRING_GAUNTLET_EVENT_SLOT_CLOSED => NKCStringTable.GetString("SI_DP_EVENTMATCH_EVENTDECK_SLOT_CLOSED");

	public static string GET_STRING_GAUNTLET_EVENT_SLOT_UNIT_DETERMINED => NKCStringTable.GetString("SI_DP_EVENTMATCH_EVENTDECK");

	public static string GET_STRING_GAUNTLET_SURRENDER_WARNING => NKCStringTable.GetString("SI_DP_OPTION_PVP_SURRENDER_WARNING");

	public static string GET_STRING_GAUNTLET_LEAGUE_GIVEUP_POPUP => NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_GIVEUP_POPUP");

	public static string GET_STRING_GAUNTLET_SURRENDER_IMPOSSIBILITY => NKCStringTable.GetString("NEC_FAIL_PVP_SURRENDER_IMPOSSIBILITY");

	public static string GET_STRING_GAUNTLET_GLOBAL_BAN_SELECT_UNIT_DESC => NKCStringTable.GetString("SI_PF_DRAFT_BAN_GLOBAL_SUB_TEXT");

	public static string GET_STRING_GAUNTLET_GLOBAL_BAN_NOT_SELECT_UNIT => NKCStringTable.GetString("SI_DP_DRAFT_BAN_GLOBAL_NOT_ENOUGH_TEXT");

	public static string GET_STRING_GAUNTLET_GLOBAL_BAN_POPUP_SUB_TITLE => NKCStringTable.GetString("SI_PF_DRAFT_BAN_GLOBAL_TEXT");

	public static string GET_STRING_GAUNTLET_GLOBAL_BAN_NO_MAX_SELECT => NKCStringTable.GetString("SI_PVP_GLOBAL_BAN_SELECT_COMPLETE");

	public static string GET_STRING_POPUP_LEAGUE_REWARD_INFO_DESC => NKCStringTable.GetString("SI_PF_POPUP_LEAGUE_REWARD_INFO_DESC");

	public static string GET_STRING_POPUP_UNLIMITED_REWARD_INFO_DESC => NKCStringTable.GetString("SI_PF_POPUP_UNLIMITED_REWARD_INFO_DESC");

	public static string GET_STRING_POPUP_LEAGUE_CALCULATE => NKCStringTable.GetString("SI_PF_POPUP_LEAGUE_CALCULATE");

	public static string GET_STRING_LEAGUE_DRAFT_UNKNOWN => NKCStringTable.GetString("SI_PF_LEAGUE_DRAFT_UNKNOWN");

	public static string GET_STRING_FRIEND => NKCStringTable.GetString("SI_DP_FRIEND");

	public static string GET_STRING_FRIEND_SHOP_COMING_SOON => NKCStringTable.GetString("SI_DP_FRIEND_SHOP_COMING_SOON");

	public static string GET_STRING_FRIEND_CHANGE_IMAGE => NKCStringTable.GetString("SI_DP_FRIEND_CHANGE_IMAGE");

	public static string GET_STRING_FRIEND_MAIN_DECK => NKCStringTable.GetString("SI_DP_FRIEND_MAIN_DECK");

	public static string GET_STRING_FRIEND_LAST_CONNECT => NKCStringTable.GetString("SI_DP_FRIEND_LAST_CONNECT");

	public static string GET_STRING_FRIEND_LEVEL => NKCStringTable.GetString("SI_DP_FRIEND_LEVEL");

	public static string GET_STRING_FRIEND_SEARCH => NKCStringTable.GetString("SI_DP_FRIEND_SEARCH");

	public static string GET_STRING_FRIEND_INFO_LEVEL_ONE_PARAM => NKCStringTable.GetString("SI_DP_FRIEND_INFO_LEVEL_ONE_PARAM");

	public static string GET_STRING_FIREND_NO_EXIST_PVP_LOG => NKCStringTable.GetString("SI_DP_FIREND_NO_EXIST_PVP_LOG");

	public static string GET_STRING_FRIEND_LIST_IS_EMPTY => NKCStringTable.GetString("SI_DP_FRIEND_LIST_IS_EMPTY");

	public static string GET_STRING_FRIEND_LIST_BLOCK_IS_EMPTY => NKCStringTable.GetString("SI_DP_FRIEND_LIST_BLOCK_IS_EMPTY");

	public static string GET_STRING_FRIEND_LIST_RECV_IS_EMPTY => NKCStringTable.GetString("SI_DP_FRIEND_LIST_RECV_IS_EMPTY");

	public static string GET_STRING_FRIEND_LIST_REQ_IS_EMPTY => NKCStringTable.GetString("SI_DP_FRIEND_LIST_REQ_IS_EMPTY");

	public static string GET_STRING_FRIEND_COUNT_TWO_PARAM => NKCStringTable.GetString("SI_DP_FRIEND_COUNT_TWO_PARAM");

	public static string GET_STRING_FRIEND_BLOCK_COUNT_TWO_PARAM => NKCStringTable.GetString("SI_DP_FRIEND_BLOCK_COUNT_TWO_PARAM");

	public static string GET_STRING_FRIEND_RECV_COUNT_TWO_PARAM => NKCStringTable.GetString("SI_DP_FRIEND_RECV_COUNT_TWO_PARAM");

	public static string GET_STRING_FRIEND_REQ_COUNT_TWO_PARAM => NKCStringTable.GetString("SI_DP_FRIEND_REQ_COUNT_TWO_PARAM");

	public static string GET_STRING_FRIEND_ADD_REQ_COMPLETE => NKCStringTable.GetString("SI_DP_FRIEND_ADD_REQ_COMPLETE");

	public static string GET_STRING_FRIEND_EMBLEM => NKCStringTable.GetString("SI_DP_FRIEND_EMBLEM");

	public static string GET_STRING_FRIEND_DELETE_REQ => NKCStringTable.GetString("SI_DP_FRIEND_DELETE_REQ");

	public static string GET_STRING_FRIEND_BLOCK_REQ_ONE_PARAM => NKCStringTable.GetString("SI_DP_FRIEND_BLOCK_REQ_ONE_PARAM");

	public static string GET_STRING_FRIEND_BLOCK_CANCEL_NOTICE => NKCStringTable.GetString("SI_DP_FRIEND_BLOCK_CANCEL_NOTICE");

	public static string GET_STRING_FRIEND_BLOCK => NKCStringTable.GetString("SI_DP_FRIEND_BLOCK");

	public static string GET_STRING_FRIEND_UNBLOCK => NKCStringTable.GetString("SI_DP_FRIEND_UNBLOCK");

	public static string GET_STRING_FRIEND_REQ => NKCStringTable.GetString("SI_DP_FRIEND_REQ");

	public static string GET_STRING_FRIEND_REQ_CANCEL => NKCStringTable.GetString("SI_DP_FRIEND_REQ_CANCEL");

	public static string GET_STRING_FRIEND_REMOVE => NKCStringTable.GetString("SI_DP_FRIEND_REMOVE");

	public static string GET_STRING_FRIEND_ACCEPT => NKCStringTable.GetString("SI_DP_FRIEND_ACCEPT");

	public static string GET_STRING_FRIEND_REFUSE => NKCStringTable.GetString("SI_DP_FRIEND_REFUSE");

	public static string GET_STRING_FRIEND_TALK => NKCStringTable.GetString("SI_DP_FRIEND_TALK");

	public static string GET_STRING_FRIEND_ROOM => NKCStringTable.GetString("SI_DP_FRIEND_ROOM");

	public static string GET_STRING_FRIEND_PVP => NKCStringTable.GetString("SI_DP_FRIEND_PVP");

	public static string GET_STRING_ACCEPT => NKCStringTable.GetString("SI_DP_ACCEPT");

	public static string GET_STRING_REFUSE => NKCStringTable.GetString("SI_DP_REFUSE");

	public static string GET_STRING_WORLDMAP_EVENT_STATE_FAIL => NKCStringTable.GetString("SI_DP_WORLDMAP_EVENT_STATE_FAIL");

	public static string GET_STRING_WORLDMAP_EVENT_STATE_TIME_EXPIRED => NKCStringTable.GetString("SI_DP_WORLDMAP_EVENT_STATE_TIME_EXPIRED");

	public static string GET_STRING_WORLDMAP => NKCStringTable.GetString("SI_DP_WORLDMAP");

	public static string GET_STRING_WORLDMAP_EVENT_WARNING => NKCStringTable.GetString("SI_DP_WORLDMAP_EVENT_WARNING");

	public static string GET_STRING_MENU_NAME_WORLDMAP_BUILDING => NKCStringTable.GetString("SI_DP_MENU_NAME_WORLDMAP_BUILDING");

	public static string GET_STRING_WORLDMAP_BUILDING_BUILD => NKCStringTable.GetString("SI_DP_WORLDMAP_BUILDING_BUILD");

	public static string GET_STRING_WORLDMAP_BUILDING_MAX_LEVEL_ONE_PARAM => NKCStringTable.GetString("SI_DP_WORLDMAP_BUILDING_MAX_LEVEL_ONE_PARAM");

	public static string GET_STRING_WORLDMAP_BUILDING_SLOT_TWO_PARAM => NKCStringTable.GetString("SI_DP_WORLDMAP_BUILDING_SLOT_TWO_PARAM");

	public static string GET_STRING_WORLDMAP_BUILDING_REQ_CITY_LEVEL_ONE_PARAM => NKCStringTable.GetString("SI_DP_WORLDMAP_BUILDING_REQ_CITY_LEVEL_ONE_PARAM");

	public static string GET_STRING_WORLDMAP_BUILDING_REQ_BUILD_TWO_PARAM => NKCStringTable.GetString("SI_DP_WORLDMAP_BUILDING_REQ_BUILD_TWO_PARAM");

	public static string GET_STRING_WORLDMAP_BUILDING_CITY_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_WORLDMAP_BUILDING_CITY_COUNT_ONE_PARAM");

	public static string GET_STRING_WORLDMAP_BUILDING_CREDIT_REQ_LEVEL_ONE_PARAM => NKCStringTable.GetString("SI_DP_WORLDMAP_BUILDING_CREDIT_REQ_LEVEL_ONE_PARAM");

	public static string GET_STRING_WORLDMAP_BUILDING_DIVE_CLEAR_ONE_PARAM => NKCStringTable.GetString("SI_DP_WORLDMAP_BUILDING_REQ_BUILD_DIVE_CLEAR_ONE_PARAM");

	public static string GET_STRING_WORLDMAP_NO_EXIST_EVENT => NKCStringTable.GetString("SI_DP_WORLDMAP_NO_EXIST_EVENT");

	public static string GET_STRING_WORLDMAP_NO_EXIST_JOIN => NKCStringTable.GetString("SI_DP_WORLDMAP_NO_EXIST_JOIN");

	public static string GET_STRING_WORLDMAP_NO_EXIST_COOP => NKCStringTable.GetString("SI_DP_WORLDMAP_NO_EXIST_COOP");

	public static string GET_STRING_WORLDMAP_GO_BUTTON => NKCStringTable.GetString("SI_DP_WORLDMAP_GO_BUTTON");

	public static string GET_STRING_WORLDMAP_BUILDING_ALREADY_BUILD => NKCStringTable.GetString("SI_DP_WORLDMAP_BUILDING_ALREADY_BUILD");

	public static string GET_STRING_WORLDMAP_BUILDING_REQ_BUILDING_ONE_PARAM => NKCStringTable.GetString("SI_DP_WORLDMAP_BUILDING_REQ_BUILDING_ONE_PARAM");

	public static string GET_STRING_WORLDMAP_CITY_LEADER => NKCStringTable.GetString("SI_DP_WORLDMAP_CITY_LEADER");

	public static string GET_STRING_WORLDMAP_ANOTHER_CITY => NKCStringTable.GetString("SI_DP_WORLDMAP_ANOTHER_CITY");

	public static string GET_STRING_WORLDMAP_CITY_SET_LEADER => NKCStringTable.GetString("SI_DP_WORLDMAP_CITY_SET_LEADER");

	public static string GET_STRING_WORLDMAP_CITY_CHANGE_LEADER => NKCStringTable.GetString("SI_DP_WORLDMAP_CITY_CHANGE_LEADER");

	public static string GET_STRING_WORLDMAP_CITY_SELECT_LEADER => NKCStringTable.GetString("SI_DP_WORLDMAP_CITY_SELECT_LEADER");

	public static string GET_STRING_WORLDMAP_CITY_MISSION_CONFIRM_TWO_PARAM => NKCStringTable.GetString("SI_DP_WORLDMAP_CITY_MISSION_CONFIRM_TWO_PARAM");

	public static string GET_STRING_WORLDMAP_CITY_MISSION_SELECT_SQUAD => NKCStringTable.GetString("SI_DP_WORLDMAP_CITY_MISSION_SELECT_SQUAD");

	public static string GET_STRING_WORLDMAP_CITY_MISSION_REFRESH => NKCStringTable.GetString("SI_DP_WORLDMAP_CITY_MISSION_REFRESH");

	public static string GET_STRING_WORLDMAP_CITY_MISSION_CANCEL => NKCStringTable.GetString("SI_DP_WORLDMAP_CITY_MISSION_CANCEL");

	public static string GET_STRING_WORLDMAP_CITY_MISSION_REQ_LEVEL_ONE_PARAM => NKCStringTable.GetString("SI_DP_WORLDMAP_CITY_MISSION_REQ_LEVEL_ONE_PARAM");

	public static string GET_STRING_WORLDMAP_CITY_MISSION_REWARD_ADD_TEXT => NKCStringTable.GetString("SI_DP_WORLDMAP_CITY_MISSION_REWARD_ADD_TEXT");

	public static string GET_STRING_WORLDMAP_CITY_MISSION_DOING => NKCStringTable.GetString("SI_DP_WORLDMAP_CITY_MISSION_DOING");

	public static string GET_STRING_WORLDMAP_BUILDING_REMOVE_DESC_TWO_PARAM => NKCStringTable.GetString("SI_DP_WORLDMAP_BUILDING_REMOVE_DESC_TWO_PARAM");

	public static string GET_STRING_WORLDMAP_BUILDING_REMOVE_POINT => NKCStringTable.GetString("SI_DP_WORLDMAP_BUILDING_REMOVE_POINT");

	public static string GET_STRING_WORLDMAP_HELP_BUTTON => NKCStringTable.GetString("SI_DP_WORLDMAP_HELP_BUTTON");

	public static string GET_STRING_WORLDMAP_PROGRESS_BUTTON_POPUP => NKCStringTable.GetString("SI_DP_WORLDMAP_PROGRESS_BUTTON_POPUP");

	public static string GET_STRING_MENU_NAME_WORLDMAP_NEW_BUILDING => NKCStringTable.GetString("SI_DP_MENU_NAME_WORLDMAP_NEW_BUILDING");

	public static string GET_STRING_WORLDMAP_CITY_NO_EXIST_LEADER => NKCStringTable.GetString("SI_DP_WORLDMAP_CITY_NO_EXIST_LEADER");

	public static string GET_STRING_WORLDMAP_CITY_DATA_IS_NULL => NKCStringTable.GetString("SI_DP_WORLDMAP_CITY_DATA_IS_NULL");

	public static string GET_STRING_POPUP_RESOURCE_WITHDRAW => NKCStringTable.GetString("SI_DP_POPUP_RESOURCE_WITHDRAW");

	public static string GET_STRING_WORLDMAP_BUILDING_REMOVE => NKCStringTable.GetString("SI_DP_WORLDMAP_BUILDING_REMOVE");

	public static string GET_STRING_WORLDMAP_CITY_MAKE_COMPLETE => NKCStringTable.GetString("SI_DP_WORLDMAP_CITY_MAKE_COMPLETE");

	public static string GET_STRING_WORLDMAP_EVENT_POPUP_OK_CANCEL_RAID_LEVEL => NKCStringTable.GetString("SI_DP_WORLDMAP_EVENT_POPUP_OK_CANCEL_RAID_LEVEL");

	public static string GET_STRING_WORLDMAP_EVENT_POPUP_OK_CANCEL_DIVE_LEVEL => NKCStringTable.GetString("SI_DP_WORLDMAP_EVENT_POPUP_OK_CANCEL_DIVE_LEVEL");

	public static string GET_STRING_WORLDMAP_EVENT_POPUP_OK_CANCEL_NEW_RAID_DELETE_WARN => NKCStringTable.GetString("SI_DP_WORLDMAP_EVENT_POPUP_OK_CANCEL_NEW_RAID_DELETE_WARN");

	public static string GET_STRING_WORLDMAP_EVENT_POPUP_OK_CANCEL_NEW_DIVE_DELETE_WARN => NKCStringTable.GetString("SI_DP_WORLDMAP_EVENT_POPUP_OK_CANCEL_NEW_DIVE_DELETE_WARN");

	public static string GET_STRING_WORLDMAP_EVENT_POPUP_OK_CANCEL_ON_GOING_RAID_DELETE_WARN => NKCStringTable.GetString("SI_DP_WORLDMAP_EVENT_POPUP_OK_CANCEL_ON_GOING_RAID_DELETE_WARN");

	public static string GET_STRING_WORLDMAP_EVENT_POPUP_OK_CANCEL_ON_GOING_DIVE_DELETE_WARN => NKCStringTable.GetString("SI_DP_WORLDMAP_EVENT_POPUP_OK_CANCEL_ON_GOING_DIVE_DELETE_WARN");

	public static string GET_STRING_WORLDMAP_CITY_UNLOCK_DESC => NKCStringTable.GetString("SI_DP_WORLD_MAP_CITY_UNLOCK_DESC");

	public static string GET_STRING_WORLDMAP_RAID_COOP_REQUEST_ALL => NKCStringTable.GetString("SI_DP_RAID_COOP_REQ_WARNING");

	public static string GET_STRING_ATTENDANCE => NKCStringTable.GetString("SI_DP_ATTENDANCE");

	public static string GET_STRING_NEWS_DOES_NOT_HAVE_NOTICE => NKCStringTable.GetString("SI_DP_NEWS_DOES_NOT_HAVE_NOTICE");

	public static string GET_STRING_NEWS_DOES_NOT_HAVE_NEWS => NKCStringTable.GetString("SI_DP_NEWS_DOES_NOT_HAVE_NEWS");

	public static string GET_STRING_RAID => NKCStringTable.GetString("SI_DP_RAID");

	public static string GET_STRING_RAID_REQ_SUPPORT => NKCStringTable.GetString("SI_DP_RAID_REQ_SUPPORT");

	public static string GET_STRING_RAID_REMAIN_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_RAID_REMAIN_COUNT_ONE_PARAM");

	public static string GET_STRING_RAID_COOP_REQ_WARNING => NKCStringTable.GetString("SI_DP_RAID_COOP_REQ_WARNING");

	public static string GET_STRING_RAID_SUPPORT_LIST => NKCStringTable.GetString("SI_DP_RAID_SUPPORT_LIST");

	public static string GET_STRING_WORLD_MAP_RAID_REMAIN_TIME => NKCStringTable.GetString("SI_DP_WORLD_MAP_RAID_REMAIN_TIME");

	public static string GET_STRING_WORLD_MAP_RAID_RIGHT_SUPPORT => NKCStringTable.GetString("SI_PF_WORLD_MAP_RAID_RIGHT_SUPPORT");

	public static string GET_STRING_WORLD_MAP_RAID_NOT_ASSIST_CHANGE => NKCStringTable.GetString("SI_PF_WORLD_MAP_RAID_NOT_ASSIST_CHANGE");

	public static string GET_STRING_WORLD_MAP_RAID_NOT_SCORE => NKCStringTable.GetString("SI_PF_WORLD_MAP_RAID_NOT_SCORE");

	public static string GET_STRING_WORLD_MAP_RAID_NO_ASSIST_COUNT => NKCStringTable.GetString("SI_PF_WORLD_MAP_RAID_NO_ASSIST_COUNT");

	public static string GET_STRING_ICON_SLOT_RAID_01 => NKCStringTable.GetString("SI_PF_ICON_SLOT_RAID_01");

	public static string GET_STRING_ICON_SLOT_RAID_02 => NKCStringTable.GetString("SI_PF_ICON_SLOT_RAID_02");

	public static string GET_STRING_WORLD_MAP_RAID_SEASON_END => NKCStringTable.GetString("SI_PF_WORLD_MAP_RAID_SEASON_END");

	public static string GET_STRING_WORLD_MAP_RAID_SWEEP_TITLE => NKCStringTable.GetString("SI_PF_WORLD_MAP_EXPLORE_POPUP_SWEEP_TITLE");

	public static string GET_STRING_WORLD_MAP_RAID_SWEEP_DESC => NKCStringTable.GetString("SI_PF_WORLD_MAP_EXPLORE_POPUP_SWEEP_DESC");

	public static string GET_STRING_CONTRACT => NKCStringTable.GetString("SI_DP_CONTRACT");

	public static string GET_STRING_CONTRACT_EMERGENCY_CONTRACT_COUPON_USE_REQ => NKCStringTable.GetString("SI_DP_CONTRACT_EMERGENCY_CONTRACT_COUPON_USE_REQ");

	public static string GET_STRING_CONTRACT_NEW_CONTRACT_SLOT_OPEN_REQ => NKCStringTable.GetString("SI_DP_CONTRACT_NEW_CONTRACT_SLOT_OPEN_REQ");

	public static string GET_STRING_CONTRACT_COMPLETE_CONTRACT_EXIST => NKCStringTable.GetString("SI_DP_CONTRACT_COMPLETE_CONTRACT_EXIST");

	public static string GET_STRING_CONTRACT_ON_GOING_CONTRACT_NUMBER_ONE_PARAM => NKCStringTable.GetString("SI_DP_CONTRACT_ON_GOING_CONTRACT_NUMBER_ONE_PARAM");

	public static string GET_STRING_CONTRACT_ON_GOING_CONTRACT_NO_EXIST => NKCStringTable.GetString("SI_DP_CONTRACT_ON_GOING_CONTRACT_NO_EXIST");

	public static string GET_STRING_CONTRACT_AVAILABLE_SLOT_NO_EXIST => NKCStringTable.GetString("SI_DP_CONTRACT_AVAILABLE_SLOT_NO_EXIST");

	public static string GET_STRING_CONTRACT_UNIT_NUMBER => NKCStringTable.GetString("SI_DP_CONTRACT_UNIT_NUMBER");

	public static string GET_STRING_CONTRACT_COUNT_CLOSE_TOOLTIP_TITLE => NKCStringTable.GetString("SI_PF_CONTRACT_COUNT_CLOSE_TOOLTIP_TITLE");

	public static string GET_STRING_CONTRACT_COUNT_CLOSE_TOOLTIP_DESC => NKCStringTable.GetString("SI_PF_CONTRACT_COUNT_CLOSE_TOOLTIP_DESC");

	public static string GET_STRING_CONTRACT_SLOT_UNIT => NKCStringTable.GetString("SI_DP_CONTRACT_SLOT_UNIT");

	public static string GET_STRING_CONTRACT_NOT_ENOUGH_LIMIT_COUNT => NKCStringTable.GetString("SI_PF_CONTRACT_NOT_ENOUGH_TRY_COUNT");

	public static string GET_STRING_CONTRACT_NOT_ENOUGH_LIMIT_REQ_ITEM_COUNT => NKCStringTable.GetString("SI_PF_CONTRACT_NOT_ENOUGH_LIMIT_REQ_ITEM_COUNT");

	public static string GET_STRING_ALREADY_ENHANCE_MAX => NKCStringTable.GetString("SI_DP_ALREADY_ENHANCE_MAX");

	public static string GET_STRING_LIMITBREAK_GROWTH_INFO_ONE_PARAM => NKCStringTable.GetString("SI_DP_LIMITBREAK_GROWTH_INFO_ONE_PARAM");

	public static string GET_STRING_LIMITBREAK_NO_EXIST_CONSUME_UNIT => NKCStringTable.GetString("SI_DP_LIMITBREAK_NO_EXIST_CONSUME_UNIT");

	public static string GET_STRING_LIMITBTEAK_INFO => NKCStringTable.GetString("SI_DP_LIMITBTEAK_INFO");

	public static string GET_STRING_LIMITBREAK_WARNING_SELECT_UNIT => NKCStringTable.GetString("SI_DP_LIMITBREAK_SELECT_HIGH_GRADE");

	public static string GET_STRING_LIMITBREAK_WARNING_RUN => NKCStringTable.GetString("SI_DP_LIMITBREAK_INCLUDE_HIGH_GRADE");

	public static string GET_STRING_LIMITBREAK_WARNING_CANCEL => NKCStringTable.GetString("SI_DP_LIMITBREAK_CANCELED");

	public static string GET_STRING_LIMITBREAK_CONFIRM => NKCStringTable.GetString("SI_DP_LIMITBREAK_CONFIRM");

	public static string GET_STRING_LIMITBREAK_CONFIRM_AWAKEN => NKCStringTable.GetString("SI_DP_LIMITBREAK_CONFIRM_AWAKEN");

	public static string GET_STRING_LIMITBREAK_TRANSCENDENCE_LEVEL_ONE_PARAM => NKCStringTable.GetString("SI_DP_LIMITBREAK_TRANSCENDENCE_LEVEL_ONE_PARAM");

	public static string GET_STRING_SKILL_ATTACK_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_SKILL_ATTACK_COUNT_ONE_PARAM");

	public static string GET_STRING_ENHANCE_NEED_SET_TARGET_UNIT => NKCStringTable.GetString("SI_DP_ENHANCE_NEED_SET_TARGET_UNIT");

	public static string GET_STRING_ENHANCE_NEED_SET_CONSUME_UNIT => NKCStringTable.GetString("SI_DP_ENHANCE_NEED_SET_CONSUME_UNIT");

	public static string GET_STRING_ENHANCE_SELECT_CONSUM_UNIT => NKCStringTable.GetString("SI_DP_ENHANCE_SELECT_CONSUM_UNIT");

	public static string GET_STRING_ENHANCE_NO_EXIST_CONSUME_UNIT => NKCStringTable.GetString("SI_DP_ENHANCE_NO_EXIST_CONSUME_UNIT");

	public static string GET_STRING_LIMITBREAK_RESULT => NKCStringTable.GetString("SI_DP_LIMITBREAK_RESULT");

	public static string GET_STRING_ALREADY_LIMITBREAK_MAX => NKCStringTable.GetString("SI_DP_ALREADY_LIMITBREAK_MAX");

	public static string GET_STRING_LIMITBREAK_RESULT_GROWTH_INFO_ONE_PARAM => NKCStringTable.GetString("SI_DP_LIMITBREAK_RESULT_GROWTH_INFO_ONE_PARAM");

	public static string GET_STRING_OPTION_CUTSCEN_NEXT_TALK_SPEED_WHEN_AUTO_FAST => NKCStringTable.GetString("SI_DP_OPTION_CUTSCENE_FAST");

	public static string GET_STRING_OPTION_CUTSCEN_NEXT_TALK_SPEED_WHEN_AUTO_NORMAL => NKCStringTable.GetString("SI_DP_OPTION_CUTSCENE_NORMAL");

	public static string GET_STRING_OPTION_CUTSCEN_NEXT_TALK_SPEED_WHEN_AUTO_SLOW => NKCStringTable.GetString("SI_DP_OPTION_CUTSCENE_SLOW");

	public static string GET_STRING_OPTION_RESET_WARNING => NKCStringTable.GetString("SI_DP_OPTION_RESET_WARNING");

	public static string GET_STRING_OPTION_DROPOUT_WARNING => NKCStringTable.GetString("SI_DP_OPTION_DROPOUT_WARNING");

	public static string GET_STRING_OPTION_DROPOUT_WARNING_INSTANT => NKCStringTable.GetString("SI_DP_OPTION_DROPOUT_WARNING_INSTANT");

	public static string GET_STRING_OPTION_CONNECTED => NKCStringTable.GetString("SI_DP_OPTION_CONNECTED");

	public static string GET_STRING_OPTION_DISCONNECTED => NKCStringTable.GetString("SI_DP_OPTION_DISCONNECTED");

	public static string GET_STRING_OPTION_LOGOUT_REQ => NKCStringTable.GetString("SI_DP_OPTION_LOGOUT_REQ");

	public static string GET_STRING_OPTION_CANNOT_LOG_OUT_WHEN_IN_GAME_BATTLE => NKCStringTable.GetString("SI_DP_OPTION_CANNOT_LOG_OUT_WHEN_IN_GAME_BATTLE");

	public static string GET_STRING_OPTION_HIGH_QUALITY => NKCStringTable.GetString("SI_DP_OPTION_HIGH_QUALITY");

	public static string GET_STRING_OPTION_CHANGE_WARNING => NKCStringTable.GetString("SI_DP_OPTION_CHANGE_WARNING");

	public static string GET_STRING_OPTION_30_FPS => NKCStringTable.GetString("SI_DP_OPTION_30_FPS");

	public static string GET_STRING_OPTION_60_FPS => NKCStringTable.GetString("SI_DP_OPTION_60_FPS");

	public static string GET_STRING_OPTION_MISSION_GIVE_UP_WARNING => NKCStringTable.GetString("SI_DP_OPTION_MISSION_GIVE_UP_WARNING");

	public static string GET_STRING_OPTION_GAME_LANG_CHANGE_REQ => NKCStringTable.GetString("SI_DP_OPTION_GAME_LANG_CHANGE_REQ");

	public static string GET_STRING_OPTION_MEDAL_COND => NKCStringTable.GetString("SI_DP_OPTION_MEDAL_COND");

	public static string GET_STRING_OPTION_RANK_COND => NKCStringTable.GetString("SI_DP_OPTION_RANK_COND");

	public static string GET_STRING_EXIST => NKCStringTable.GetString("SI_DP_EXIST");

	public static string GET_STRING_LIMITED => NKCStringTable.GetString("SI_DP_LIMITED");

	public static string GET_STRING_OPTION_LANGUAGE_CHANGE => NKCStringTable.GetString("SI_DP_OPTION_LANGUAGE_CHANGE");

	public static string GET_STRING_GAME_OPTION_ACCOUNT_NEXON_WITHDRAWAL => NKCStringTable.GetString("SI_DP_GAME_OPTION_ACCOUNT_NEXON_WITHDRAWAL");

	public static string GET_STRING_OPTION_DROPOUT_WARNING_NEXON => NKCStringTable.GetString("SI_DP_OPTION_DROPOUT_WARNING_NEXON");

	public static string GET_STRING_OPTION_MISSION_GIVE_UP_WARNING_MULTIPLY => NKCStringTable.GetString("SI_DP_MULTIPLY_OPERATION_DUNGEON_OUT_POPUP_DESC");

	public static string GET_STRING_OPTION_SIGN_OUT_MESSAGE_MISS_MATCHED => NKCStringTable.GetString("SI_PF_POPUP_SIGN_OUT_ERROR_TEXT");

	public static string GET_STRING_OPTION_BILLING_RESTORE_TITLE => NKCStringTable.GetString("SI_PF_OPTION_ACCOUNT_RESTORE_PURCHASE");

	public static string GET_STRING_OPTION_BILLING_RESTORE_SUCCESS_DESC => NKCStringTable.GetString("SI_DP_OPTION_RESTORE_PURCHASE_SUCCESS");

	public static string GET_STRING_OPTION_BILLING_RESTORE_FAIL_DESC => NKCStringTable.GetString("SI_DP_OPTION_RESTORE_PURCHASE_FAIL");

	public static string GET_STRING_OPTION_BILLING_RESTORE_EMPTY_LIST_DESC => NKCStringTable.GetString("SI_DP_OPTION_RESTORE_PURCHASE_NO_LIST");

	public static string GET_STRING_FORGE_CRAFT_COUNT_INFINITE_SYMBOL => NKCStringTable.GetString("SI_DP_FORGE_CRAFT_COUNT_INFINITE_SYMBOL");

	public static string GET_STRING_FORGE_CRAFT_POPUP_TITLE => NKCStringTable.GetString("SI_DP_FORGE_CRAFT_POPUP_TITLE");

	public static string GET_STRING_FORGE => NKCStringTable.GetString("SI_DP_FORGE");

	public static string GET_STRING_NO_EXIST_ENCHANT_EQUIP => NKCStringTable.GetString("SI_DP_NO_EXIST_ENCHANT_EQUIP");

	public static string GET_STRING_NO_EXIST_TUNING_EQUIP => NKCStringTable.GetString("SI_DP_NO_EXIST_TUNING_EQUIP");

	public static string GET_STRING_NO_EXIST_HIDDEN_OPTION_EQUIP => NKCStringTable.GetString("SI_DP_NO_EXIST_HIDDEN_OPTION_EQUIP");

	public static string GET_STRING_FORGE_CRAFT => NKCStringTable.GetString("SI_DP_FORGE_CRAFT");

	public static string GET_STRING_FORGE_CRAFT_USE_MISC_TWO_PARAM => NKCStringTable.GetString("SI_DP_FORGE_CRAFT_USE_MISC_TWO_PARAM");

	public static string GET_STRING_FORGE_CRAFT_MOLD => NKCStringTable.GetString("SI_DP_FORGE_CRAFT_MOLD");

	public static string GET_STRING_FORGE_CRAFT_SLOT_ADD => NKCStringTable.GetString("SI_DP_FORGE_CRAFT_SLOT_ADD");

	public static string GET_STRING_FORGE_CRAFT_WAIT_NAME => NKCStringTable.GetString("SI_DP_FORGE_CRAFT_WAIT_NAME");

	public static string GET_STRING_FORGE_CRAFT_WAIT_TEXT => NKCStringTable.GetString("SI_DP_FORGE_CRAFT_WAIT_TEXT");

	public static string GET_STRING_FORGE_CRAFT_ING_TEXT => NKCStringTable.GetString("SI_DP_FORGE_CRAFT_ING_TEXT");

	public static string GET_STRING_FORGE_CRAFT_COMPLETED_TEXT => NKCStringTable.GetString("SI_DP_FORGE_CRAFT_COMPLETED_TEXT");

	public static string GET_STRING_FORGE_ENCHANT_LEVEL_ONE_PARAM => NKCStringTable.GetString("SI_DP_FORGE_ENCHANT_LEVEL_ONE_PARAM");

	public static string GET_STRING_FORGE_ENCHANT_ALREADY_MAX => NKCStringTable.GetString("SI_DP_FORGE_ENCHANT_ALREADY_MAX");

	public static string GET_STRING_FORGE_ENCHANT_NEED_CONSUME => NKCStringTable.GetString("SI_DP_FORGE_ENCHANT_NEED_CONSUME");

	public static string GET_STRING_FORGE_ENCHANT_NO_EXIST_CONSUME => NKCStringTable.GetString("SI_DP_FORGE_ENCHANT_NO_EXIST_CONSUME");

	public static string GET_STRING_FORGE_TUNING_CONFIRM_TITLE => NKCStringTable.GetString("SI_DP_FORGE_TUNING_CONFIRM_TITLE");

	public static string GET_STRING_FORGE_TUNING_CONFIRM_DESC_TWO_PARAM => NKCStringTable.GetString("SI_DP_FORGE_TUNING_CONFIRM_DESC_TWO_PARAM");

	public static string GET_STRING_FORGE_TUNING_CONFIRM_DESC_TWO_PARAM_FULL => NKCStringTable.GetString("SI_DP_FORGE_TUNING_CONFIRM_DESC_TWO_PARAM_BONUS_FULL");

	public static string GET_STRING_FORGE_TUNING_HAS_RESERVED_EQUIP_TUNING => NKCStringTable.GetString("SI_PF_FORGE_TUNING_HAS_RESERVED_EQUIP_TUNING");

	public static string GET_STRING_FORGE_TUNNING => NKCStringTable.GetString("SI_DP_FORGE_TUNNING");

	public static string GET_STRING_FORGE_TUNING_STAT_CURRENT => NKCStringTable.GetString("SI_DP_FORGE_TUNING_STAT_CURRENT");

	public static string GET_STRING_FORGE_TUNING_STAT_CHANGE => NKCStringTable.GetString("SI_DP_FORGE_TUNING_STAT_CHANGE");

	public static string GET_STRING_EQUIP_BREAK_UP_NO_EXIST_EQUIP => NKCStringTable.GetString("SI_DP_EQUIP_BREAK_UP_NO_EXIST_EQUIP");

	public static string GET_STRING_NO_EXIST_SELECTED_EQUIP => NKCStringTable.GetString("SI_DP_NO_EXIST_SELECTED_EQUIP");

	public static string GET_STRING_EQUIP_BREAK_UP_WARNING => NKCStringTable.GetString("SI_DP_EQUIP_BREAK_UP_WARNING");

	public static string GET_STRING_TUNING_OPTIN_NONE => NKCStringTable.GetString("SI_DP_TUNING_OPTIN_NONE");

	public static string GET_STRING_TUNING_OPTIN_CAN_NOT_CHANGE => NKCStringTable.GetString("SI_DP_TUNING_OPTIN_CAN_NOT_CHANGE");

	public static string GET_STRING_TUNING_OPTION_SLOT_EXCLUSIVE => NKCStringTable.GetString("SI_DP_TUNING_OPTION_SLOT_EXCLUSIVE");

	public static string GET_STRING_TUNING_OPTION_SLOT_OPTION => NKCStringTable.GetString("SI_DP_TUNING_OPTION_SLOT_OPTION");

	public static string GET_STRING_FORGE_CRAFT_ITEM_NO_FOUND => NKCStringTable.GetString("SI_DP_FORGE_CRAFT_ITEM_NO_FOUND");

	public static string GET_STRING_FORGE_CRAFT_POPUP => NKCStringTable.GetString("SI_DP_FORGE_CRAFT_POPUP");

	public static string GET_STRING_FORGE_TUNING_FAIL => NKCStringTable.GetString("SI_DP_FORGE_TUNING_FAIL");

	public static string GET_STRING_FORGE_TUNING_PRECISION_ONE_PARAM => NKCStringTable.GetString("SI_DP_FORGE_TUNING_PRECISION_ONE_PARAM");

	public static string GET_STRING_FORGE_TUNING_STAT_BASE => NKCStringTable.GetString("SI_DP_FORGE_TUNING_STAT_BASE");

	public static string GET_STRING_FORGE_TUNING_STAT_RESULT => NKCStringTable.GetString("SI_DP_FORGE_TUNING_STAT_RESULT");

	public static string GET_STRING_FORGE_TUNING_COMPLETE => NKCStringTable.GetString("SI_DP_FORGE_TUNING_COMPLETE");

	public static string GET_STRING_EQUIP_BREAK_UP => NKCStringTable.GetString("SI_DP_EQUIP_BREAK_UP");

	public static string GET_STRING_FORGE_SET_OPTION_CHANGE_POPUP_CONFIRM_TITLE => NKCStringTable.GetString("SI_DP_FORGE_SET_POPUP_CONFIRM_TITLE");

	public static string GET_STRING_FORGE_SET_OPTION_CHANGE_POPUP_CONFIRM_DESC => NKCStringTable.GetString("SI_DP_FORGE_SET_POPUP_CONFIRM_DESC");

	public static string GET_STRING_FORGE_SET_OPTION_CHANGE_POPUP_CONFIRM_DESC_FULL => NKCStringTable.GetString("SI_DP_FORGE_SET_POPUP_CONFIRM_DESC_BONUS_FULL");

	public static string GET_STRING_EQUIP_SELECT_ACC_1 => NKCStringTable.GetString("SI_DP_EQUIP_WHERE_ACC_SLOT_1");

	public static string GET_STRING_EQUIP_SELECT_ACC_2 => NKCStringTable.GetString("SI_DP_EQUIP_WHERE_ACC_SLOT_2");

	public static string GET_STRING_EQUIP_ACC_2_LOCKED_DESC => NKCStringTable.GetString("SI_TOAST_EQUIP_SLOT_ACC2_IS_LOCKED");

	public static string GET_STRING_FORGE_TUNING_SET_NO_OPTION => NKCStringTable.GetString("SI_DP_FORGE_SET_NO_OPTION");

	public static string GET_STRING_FORGE_TUNING_SET_OPTION_CANNOT_CHANGE => NKCStringTable.GetString("SI_DP_FORGE_SET_CANNOT_CHANGE");

	public static string GET_STRING_FORGE_CRAFT_USE_MISC_ONE_PARAM => NKCStringTable.GetString("SI_DP_FORGE_CRAFT_USE_MISC_ONE_PARAM");

	public static string GET_STRING_FORGE_CRAFT_MOLD_DESC => NKCStringTable.GetString("SI_DP_FORGE_CRAFT_MOLD_DESC");

	public static string GET_STRING_FORGE_TUNING_EXIT_CONFIRM => NKCStringTable.GetString("SI_DP_FORGE_TUNING_EXIT_CONFIRM");

	public static string GET_STRING_FORGE_SET_OPTION_TUNING_EXIT_CONFIRM => NKCStringTable.GetString("SI_DP_FORGE_SET_EXIT_CONFIRM");

	public static string GET_STRING_SORT_CRAFTABLE => NKCStringTable.GetString("SI_DP_SORT_CRAFTABLE");

	public static string GET_STRING_SORT_SETOPTION => NKCStringTable.GetString("SI_PF_SORT_EQUIP_SET_OPTION");

	public static string GET_STRING_IMPOSSIBLE_TUNING_BY_WARFARE => NKCStringTable.GetString("SI_PF_IMPOSSIBLE_TUNING_BY_WARFARE");

	public static string GET_STRING_IMPOSSIBLE_TUNING_BY_DIVE => NKCStringTable.GetString("SI_PF_IMPOSSIBLE_TUNING_BY_DIVE");

	public static string GET_STRING_SETOPTION_CHANGE_NOTICE => NKCStringTable.GetString("SI_PF_SETOPTION_CHANGE_NOTICE");

	public static string GET_STRING_STAT_SHORT_NAME_FOR_INVEN_EQUIP_RANDOM_SET => NKCStringTable.GetString("SI_DP_STAT_SHORT_NAME_FOR_INVEN_EQUIP_RANDOM_SET");

	public static string GET_STRING_EQUIP_ENCHANT_NOT_ENOUGH_MODULE => NKCStringTable.GetString("SI_DP_EQUIP_ENCHANT_NOT_ENOUGH_MODULE");

	public static string GET_STRING_EQUIP_SET_RATE_INFO => NKCStringTable.GetString("SI_DP_EQUIP_SET_RATE_INFO");

	public static string GET_STRING_EQUIP_REROLL_RATE_TEXT => NKCStringTable.GetString("SI_DP_POPUP_RATE_TEXT");

	public static string GET_STRING_TUNING_OPTION_CHANGE_BONUS_TITLE => NKCStringTable.GetString("SI_PF_FACTORY_TUNING_OPTION_CHANGE_BONUS_TITLE");

	public static string GET_STRING_TUNING_OPTION_CHANGE_BONUS_ACTIVE => NKCStringTable.GetString("SI_PF_FACTORY_TUNING_OPTION_CHANGE_BONUS_ACTIVE");

	public static string GET_STRING_TUNING_SET_OPTION_CHANGE_BONUS_TITLE => NKCStringTable.GetString("SI_PF_FACTORY_TUNING_SET_CHANGE_BONUS_TITLE");

	public static string GET_STRING_TUNING_SET_OPTION_CHANGE_BONUS_ACTIVE => NKCStringTable.GetString("SI_PF_FACTORY_TUNING_SET_CHANGE_BONUS_ACTIVE");

	public static string GET_STRING_HANGAR_SHIPYARD_SKILL_NEW => NKCStringTable.GetString("SI_DP_HANGAR_SHIPYARD_SKILL_NEW");

	public static string GET_STRING_HANGAR_SHIPYARD_SKILL_UPGRADE => NKCStringTable.GetString("SI_DP_HANGAR_SHIPYARD_SKILL_UPGRADE");

	public static string GET_STRING_HANGAR_BUILD => NKCStringTable.GetString("SI_DP_HANGAR_BUILD");

	public static string GET_STRING_HANGAR_BUILD_FAIL => NKCStringTable.GetString("SI_DP_HANGAR_BUILD_FAIL");

	public static string GET_STRING_HANGAR_CONFIRM => NKCStringTable.GetString("SI_DP_HANGAR_CONFIRM");

	public static string GET_STRING_HANGAR_CONFIRM_FAIL => NKCStringTable.GetString("SI_DP_HANGAR_CONFIRM_FAIL");

	public static string GET_STRING_HANGAR_SHIPYARD => NKCStringTable.GetString("SI_DP_HANGAR_SHIPYARD");

	public static string GET_STRING_SELECT_SHIP => NKCStringTable.GetString("SI_DP_SELECT_SHIP");

	public static string GET_STRING_NO_EXIST_SELECT_SHIP => NKCStringTable.GetString("SI_DP_NO_EXIST_SELECT_SHIP");

	public static string GET_STRING_HANGAR_SHIP_LEVEL_TWO_PARAM => NKCStringTable.GetString("SI_DP_HANGAR_SHIP_LEVEL_TWO_PARAM");

	public static string GET_STRING_HANGAR_LVUP => NKCStringTable.GetString("SI_PF_HANGAR_LVUP");

	public static string GET_STRING_HANGAR_SHIPYARD_POPUP_LEVEL_UP_TEXT => NKCStringTable.GetString("SI_DP_HANGAR_SHIPYARD_POPUP_LEVEL_UP_TEXT");

	public static string GET_STRING_HANGAR_SHIPYARD_POPUP_UPGRADE_TITLE => NKCStringTable.GetString("SI_DP_HANGAR_SHIPYARD_POPUP_UPGRADE_TITLE");

	public static string GET_STRING_HANGAR_SHIPYARD_POPUP_UPGRADE_TEXT => NKCStringTable.GetString("SI_DP_HANGAR_SHIPYARD_POPUP_UPGRADE_TEXT");

	public static string GET_STRING_HANGAR_SHIPYARD_CANNOT_CHANGE_SHIP => NKCStringTable.GetString("SI_DP_HANGAR_SHIPYARD_CANNOT_CHANGE_SHIP");

	public static string GET_STRING_HANGAR_SHIPYARD_CANNOT_FIND_INFORMATION => NKCStringTable.GetString("SI_DP_HANGAR_SHIPYARD_CANNOT_FIND_INFORMATION");

	public static string GET_STRING_HANGAR_SHIP_LEVEL_ONE_PARAM => NKCStringTable.GetString("SI_DP_HANGAR_SHIP_LEVEL_ONE_PARAM");

	public static string GET_STRING_REMOVE_SHIP_NO_EXIST_SHIP => NKCStringTable.GetString("SI_DP_REMOVE_SHIP_NO_EXIST_SHIP");

	public static string GET_STRING_REMOVE_SHIP_WARNING_MSG => NKCStringTable.GetString("SI_DP_REMOVE_SHIP_WARNING_MSG");

	public static string GET_STRING_REMOVE_SHIP_SELECT => NKCStringTable.GetString("SI_DP_REMOVE_SHIP_SELECT");

	public static string GET_STRING_REMOVE_SHIP => NKCStringTable.GetString("SI_DP_REMOVE_SHIP");

	public static string GET_STRING_ATTACK_SPEED_ONE_PARAM => NKCStringTable.GetString("SI_DP_ATTACK_SPEED_ONE_PARAM");

	public static string GET_STRING_HANGAR_UPGRADE_RESULT => NKCStringTable.GetString("SI_DP_HANGAR_UPGRADE_RESULT");

	public static string GET_STRING_SHIP_BUILD_FAIL => NKCStringTable.GetString("SI_DP_SHIP_BUILD_FAIL");

	public static string GET_STRING_SHIP_LEVEL_UP_FAIL => NKCStringTable.GetString("SI_DP_SHIP_LEVEL_UP_FAIL");

	public static string GET_STRING_SHIP_DIVISION_FAIL => NKCStringTable.GetString("SI_DP_SHIP_DIVISION_FAIL");

	public static string GET_STRING_SHIP_UPGRADE_FAIL => NKCStringTable.GetString("SI_DP_SHIP_UPGRADE_FAIL");

	public static string GET_STRING_SHIP_BUILD_CONDITION_FAIL_PLAYER_LEVEL => NKCStringTable.GetString("SI_DP_SHIP_BUILD_CONDITION_FAIL_PLAYER_LEVEL");

	public static string GET_STRING_SHIP_BUILD_CONDITION_FAIL_DUNGEON_CLEAR => NKCStringTable.GetString("SI_DP_SHIP_BUILD_CONDITION_FAIL_DUNGEON_CLEAR");

	public static string GET_STRING_SHIP_BUILD_CONDITION_FAIL_DUNGEON_CLEAR_VER2 => NKCStringTable.GetString("SI_DP_SHIP_BUILD_CONDITION_FAIL_DUNGEON_CLEAR_VER2");

	public static string GET_STRING_SHIP_BUILD_CONDITION_FAIL_SHIP_COLLECT => NKCStringTable.GetString("SI_DP_SHIP_BUILD_CONDITION_FAIL_SHIP_COLLECT");

	public static string GET_STRING_SHIP_BUILD_CONDITION_FAIL_WARFARE_CLEAR => NKCStringTable.GetString("SI_DP_SHIP_BUILD_CONDITION_FAIL_WARFARE_CLEAR");

	public static string GET_STRING_SHIP_BUILD_CONDITION_FAIL_SHIP_LEVEL => NKCStringTable.GetString("SI_DP_SHIP_BUILD_CONDITION_FAIL_SHIP_LEVEL");

	public static string GET_STRING_SHIP_BUILD_CONDITION_FAIL_SHADOW_CLEAR => NKCStringTable.GetString("SI_DP_SHIP_BUILD_CONDITION_FAIL_SHADOW_CLEAR");

	public static string GET_STRING_POPUP_MENU_NAME_BUILD_CONFIRM => NKCStringTable.GetString("SI_DP_POPUP_MENU_NAME_BUILD_CONFIRM");

	public static string GET_STRING_POPUP_MENU_NAME_SHIPYARD_CONFIRM => NKCStringTable.GetString("SI_DP_POPUP_MENU_NAME_SHIPYARD_CONFIRM");

	public static string GET_STRING_HANGAR_SHIPYARD_POPUP_LEVEL_UP_MISC_TEXT => NKCStringTable.GetString("SI_DP_HANGAR_SHIPYARD_POPUP_LEVEL_UP_MISC_TEXT");

	public static string GET_STRING_HANGAR_SHIPYARD_POPUP_UPGRADE_MISC_TEXT => NKCStringTable.GetString("SI_DP_HANGAR_SHIPYARD_POPUP_UPGRADE_MISC_TEXT");

	public static string GET_STRING_HANGAR_SHIPYARD_POPUP_DESC_LEVEL_UP => NKCStringTable.GetString("SI_DP_HANGAR_SHIPYARD_POPUP_DESC_LEVEL_UP");

	public static string GET_STRING_HANGAR_SHIPYARD_POPUP_DESC_UPGRADE => NKCStringTable.GetString("SI_DP_HANGAR_SHIPYARD_POPUP_DESC_UPGRADE");

	public static string GET_STRING_HANGAR_UPGRADE_COST => NKCStringTable.GetString("SI_PF_HANGAR_UPGRADE_COST");

	public static string GET_STRING_HANGAR_UPGRADE_COST_2 => NKCStringTable.GetString("SI_PF_HANGAR_UPGRADE_COST_2");

	public static string GET_STRING_SHIP_INFO_01_SHIPYARD_MODULE_STEP_INFO => NKCStringTable.GetString("SI_PF_SHIP_INFO_01_SHIPYARD_MODULE_STEP_INFO");

	public static string GET_STRING_SHIP_LIMITBREAK_NOT_CHOICE_SHIP => NKCStringTable.GetString("SI_PF_SHIP_LIMITBREAK_NOT_CHOICE_SHIP");

	public static string GET_STRING_SHIP_COMMAND_MODULE_SLOT_OPTION => NKCStringTable.GetString("SI_PF_SHIP_COMMAND_MODULE_SLOT_OPTION");

	public static string GET_STRING_SHIP_COMMANDMODULE_SLOT_LOCK => NKCStringTable.GetString("SI_DP_SHIP_COMMANDMODULE_SLOT_LOCK");

	public static string GET_STRING_SHIP_COMMANDMODULE_SLOT_UNLOCK => NKCStringTable.GetString("SI_DP_SHIP_COMMANDMODULE_SLOT_UNLOCK");

	public static string GET_STRING_SHIP_COMMAND_MODULE => NKCStringTable.GetString("SI_PF_SHIP_COMMAND_MODULE");

	public static string GET_STRING_SHIP_COMMAND_MODULE_SLOT_LOCK_COST_NOT_RECOVERED => NKCStringTable.GetString("SI_PF_SHIP_COMMAND_MODULE_SLOT_LOCK_COST_NOT_RECOVERED");

	public static string GET_STRING_NEC_FAIL_SHIP_COMMAND_MODULE_LOCK_NO_CONFIRM => NKCStringTable.GetString("NEC_FAIL_SHIP_COMMAND_MODULE_LOCK_NO_CONFIRM");

	public static string GET_STRING_SHIP_COMMANDMODULE_EXIT_CONFIRM => NKCStringTable.GetString("SI_DP_SHIP_COMMANDMODULE_EXIT_CONFIRM");

	public static string GET_STRING_SHIP_COMMAND_MODULE_SLOT_ALL_LOCK => NKCStringTable.GetString("SI_PF_SHIP_COMMAND_MODULE_SLOT_ALL_LOCK");

	public static string GET_STRING_SHIP_LIMITBREAK => NKCStringTable.GetString("SI_PF_SHIP_LIMITBREAK");

	public static string GET_STRING_SHIP_LIMITBREAK_POPUP_DESC => NKCStringTable.GetString("SI_DP_SHIP_LIMITBREAK_POPUP_DESC");

	public static string GET_STRING_SHIP_COMMANDMODULE_OPEN => NKCStringTable.GetString("SI_DP_SHIP_COMMANDMODULE_OPEN");

	public static string GET_STRING_SHIP_LIMITBREAK_GRADE => NKCStringTable.GetString("SI_DP_SHIP_LIMITBREAK_GRADE");

	public static string GET_STRING_SHIP_LIMITBREAK_GRADE_COMMANDMODULE_UNLOCK => NKCStringTable.GetString("SI_DP_SHIP_LIMITBREAK_GRADE_COMMANDMODULE_UNLOCK");

	public static string GET_STRING_SHIP_COMMAND_MODULE_NOT_LIMITBREAK => NKCStringTable.GetString("SI_PF_SHIP_COMMAND_MODULE_NOT_LIMITBREAK");

	public static string GET_STRING_SHIP_INFO_COMMAND_MODULE_NO_LIMITBREAK => NKCStringTable.GetString("SI_PF_SHIP_INFO_COMMAND_MODULE_NO_LIMITBREAK");

	public static string GET_STRING_SHIP_LIMITBREAK_WARNING => NKCStringTable.GetString("SI_PF_SHIP_LIMITBREAK_WARNING");

	public static string GET_STRING_SHIP_INFO_MODULE_STEP_TEXT => NKCStringTable.GetString("SI_PF_SHIP_INFO_MODULE_STEP_TEXT");

	public static string GET_STRING_SHIP_COMMAND_MODULE_SLOT_NOT_OPEN => NKCStringTable.GetString("SI_PF_SHIP_COMMAND_MODULE_SLOT_NOT_OPEN");

	public static string GET_STRING_SHIP_COMMAND_MODULE_SLOT_HAS_RESERVED => NKCStringTable.GetString("SI_PF_SHIP_COMMAND_MODULE_SLOT_HAS_RESERVED");

	public static string GET_STRING_REMOVE_UNIT_NO_EXIST_UNIT => NKCStringTable.GetString("SI_DP_REMOVE_UNIT_NO_EXIST_UNIT");

	public static string GET_STRING_REMOVE_UNIT_NO_EXIST_TROPHY => NKCStringTable.GetString("SI_DP_REMOVE_UNIT_NO_EXIST_TROPHY");

	public static string GET_STRING_REMOVE_UNIT_WARNING => NKCStringTable.GetString("SI_DP_REMOVE_UNIT_WARNING");

	public static string GET_STRING_NO_EXIST_SELECTED_UNIT => NKCStringTable.GetString("SI_DP_NO_EXIST_SELECTED_UNIT");

	public static string GET_STRING_REMOVE_UNIT => NKCStringTable.GetString("SI_DP_REMOVE_UNIT");

	public static string GET_STRING_NO_EXIST_UNIT => NKCStringTable.GetString("SI_DP_NO_EXIST_UNIT");

	public static string GET_STRING_REMOVE_UNIT_FAIL => NKCStringTable.GetString("SI_DP_REMOVE_UNIT_FAIL");

	public static string GET_STRING_REMOVE_UNIT_FAIL_LOCKED => NKCStringTable.GetString("SI_DP_REMOVE_UNIT_FAIL_LOCKED");

	public static string GET_STRING_REMOVE_UNIT_FAIL_IN_DECK => NKCStringTable.GetString("SI_DP_REMOVE_UNIT_FAIL_IN_DECK");

	public static string GET_STRING_REMOVE_UNIT_FAIL_MAINUNIT => NKCStringTable.GetString("SI_DP_REMOVE_UNIT_FAIL_MAINUNIT");

	public static string GET_STRING_REMOVE_UNIT_FAIL_WORLDMAP_LEADER => NKCStringTable.GetString("SI_DP_REMOVE_UNIT_FAIL_WORLDMAP_LEADER");

	public static string GET_STRING_REMOVE_UNIT_SELECT => NKCStringTable.GetString("SI_DP_REMOVE_UNIT_SELECT");

	public static string GET_STRING_EXTRACT_OPEATOR_SELECT => NKCStringTable.GetString("SI_DP_EXTRACT_OPERATOR_SELECT");

	public static string GET_STRING_EXTRACT_UNIT_NO_EXIST_OPERATOR => NKCStringTable.GetString("SI_PF_EXTRACT_UNIT_NO_EXIST_OPERATOR");

	public static string GET_STRING_CEO => NKCStringTable.GetString("SI_DP_CEO");

	public static string GET_STRING_NEGOTIATE_OFFER_SELECTION_CANCEL => NKCStringTable.GetString("SI_DP_NEGOTIATE_OFFER_SELECTION_CANCEL");

	public static string GET_STRING_LIFETIME_CONTRACT_DATE_THREE_PARAM => NKCStringTable.GetString("SI_DP_LIFETIME_CONTRACT_DATE_THREE_PARAM");

	public static string GET_STRING_NEGOTIATE => NKCStringTable.GetString("SI_DP_NEGOTIATE");

	public static string GET_STRING_NEGOTIATE_LEVEL_MAX => NKCStringTable.GetString("SI_DP_NEGOTIATE_LEVEL_MAX");

	public static string GET_STRING_NEGOTIATE_NO_EXIST_UNIT => NKCStringTable.GetString("SI_DP_NEGOTIATE_NO_EXIST_UNIT");

	public static string GET_STRING_NEGOTIATE_OFFER_UNIT_FIRST => NKCStringTable.GetString("SI_DP_NEGOTIATE_OFFER_UNIT_FIRST");

	public static string GET_STRING_ROUND_ONE_PARAM => NKCStringTable.GetString("SI_DP_ROUND_ONE_PARAM");

	public static string GET_STRING_FINAL_ROUND => NKCStringTable.GetString("SI_DP_FINAL_ROUND");

	public static string GET_STRING_NEGOTIATE_OFFER_SELECTION_RAISE_ALL => NKCStringTable.GetString("SI_DP_NEGOTIATE_OFFER_SELECTION_RAISE_ALL");

	public static string GET_STRING_NEGOTIATE_OFFER_SELECTION_RAISE => NKCStringTable.GetString("SI_DP_NEGOTIATE_OFFER_SELECTION_RAISE");

	public static string GET_STRING_NEGOTIATE_OFFER_SELECTION_OK => NKCStringTable.GetString("SI_DP_NEGOTIATE_OFFER_SELECTION_OK");

	public static string GET_STRING_NEGOTIATE_OFFER_SELECTION_PASSION => NKCStringTable.GetString("SI_DP_NEGOTIATE_OFFER_SELECTION_PASSION");

	public static string GET_STRING_NOT_ENOUGH_NEGOTIATE_MATERIALS => NKCStringTable.GetString("SI_DP_NOT_ENOUGH_NEGOTIATE_MATERIALS");

	public static string GET_STRING_NEGOTIATE_RAISE_DESC => NKCStringTable.GetString("SI_DP_NEGOTIATE_RAISE_DESC");

	public static string GET_STRING_NEGOTIATE_OK_DESC => NKCStringTable.GetString("SI_DP_NEGOTIATE_OK_DESC");

	public static string GET_STRING_NEGOTIATE_PASSION_DESC => NKCStringTable.GetString("SI_DP_NEGOTIATE_PASSION_DESC");

	public static string GET_NEGOTIATE_OFFER_SELECTION_BONUS => NKCStringTable.GetString("SI_DP_NEGOTIATE_OFFER_SELECTION_BONUS");

	public static string GET_STRING_MAX_LEVEL_LOYALTY => NKCStringTable.GetString("SI_DP_MAX_LEVEL_LOYALTY");

	public static string GET_STRING_NEGOTIATE_OVER_MAX_LEVEL_ONE_PARAM => NKCStringTable.GetString("SI_DP_NEGOTIATE_OVER_MAX_LEVEL_ONE_PARAM");

	public static string GET_STRING_LIFETIME => NKCStringTable.GetString("SI_DP_LIFETIME");

	public static string GET_STRING_LIFETIME_CONTRACT_TITLE => NKCStringTable.GetString("SI_DP_LIFETIME_CONTRACT_TITLE");

	public static string GET_STRING_LIFETIME_CONTRACT_TITLE_MECHANIC => NKCStringTable.GetString("SI_DP_LIFETIME_CONTRACT_TITLE_MECHANIC");

	public static string GET_STRING_LIFETIME_CONTRACT_UNIT_SIGN_TWO_PARAM => NKCStringTable.GetString("SI_DP_LIFETIME_CONTRACT_UNIT_SIGN_TWO_PARAM");

	public static string GET_STRING_LIFETIME_CONTRACT_PLAYER_SIGN_ONE_PARAM => NKCStringTable.GetString("SI_DP_LIFETIME_CONTRACT_PLAYER_SIGN_ONE_PARAM");

	public static string GET_STRING_LIFETIME_REWARD_INFO => NKCStringTable.GetString("SI_DP_LIFETIME_REWARD_INFO");

	public static string GET_STRING_LIFETIME_NO_EXIST_UNIT => NKCStringTable.GetString("SI_DP_LIFETIME_NO_EXIST_UNIT");

	public static string GET_STRING_LIFETIME_REPLAY => NKCStringTable.GetString("SI_DP_LOYALTY_LIFETIME_RECALL");

	public static string GET_STRING_LIFETIME_CONTRACT_POPUP => NKCStringTable.GetString("SI_POPUP_UNIT_INFO_LOYALTY_100");

	public static string GET_STRING_LIFETIME_LOYALTY_INFO => NKCStringTable.GetString("SI_POPUP_UNIT_INFO_LOYALTY");

	public static string GET_STRING_TUTORIAL => NKCStringTable.GetString("SI_DP_TUTORIAL");

	public static string GET_STRING_TUTORIAL_GUIDE => NKCStringTable.GetString("SI_DP_TUTORIAL_GUIDE");

	public static string GET_STRING_TUTORIAL_IMAGE => NKCStringTable.GetString("SI_DP_TUTORIAL_IMAGE");

	public static string GET_STRING_TUTORIAL_FIRST => NKCStringTable.GetString("SI_DP_TUTORIAL_FIRST");

	public static string GET_STRING_TUTORIAL_NEXT => NKCStringTable.GetString("SI_DP_TUTORIAL_NEXT");

	public static string GET_STRING_REWARD_FIRST_CLEAR => NKCStringTable.GetString("SI_DP_REWARD_FIRST_CLEAR");

	public static string GET_STRING_REWARD_FIRST_STAR_ALL_CLEAR => NKCStringTable.GetString("SI_DP_WARFARE_FIRST_ALL_CLEAR");

	public static string GET_STRING_REWARD_CHANCE_UP => NKCStringTable.GetString("SI_DP_REWARD_CHANCE_UP");

	public static string GET_STRING_SLOT_VIEWR_DESC => NKCStringTable.GetString("SI_DP_SLOT_VIEWR_DESC");

	public static string GET_STRING_FIRST_GET_SHIP => NKCStringTable.GetString("SI_DP_FIRST_GET_SHIP");

	public static string GET_STRING_FIRST_GET_UNIT => NKCStringTable.GetString("SI_DP_FIRST_GET_UNIT");

	public static string GET_STRING_RESULT_CITY_MISSION => NKCStringTable.GetString("SI_DP_RESULT_CITY_MISSION");

	public static string GET_STRING_RESULT_MISSION => NKCStringTable.GetString("SI_DP_RESULT_MISSION");

	public static string GET_STRING_GET_UNIT => NKCStringTable.GetString("SI_DP_GET_UNIT");

	public static string GET_STRING_GET_SHIP => NKCStringTable.GetString("SI_DP_GET_SHIP");

	public static string GET_STRING_RESULT_LIMIT_BREAK_UNIT_ONE_PARAM => NKCStringTable.GetString("SI_DP_RESULT_LIMIT_BREAK_UNIT_ONE_PARAM");

	public static string GET_STRING_RESULT_LIMIT_BREAK_UNIT_ONE_PARAM_UNLOCK_HYPER_SKILL => NKCStringTable.GetString("SI_DP_RESULT_LIMIT_BREAK_UNIT_ONE_PARAM_UNLOCK_HYPER_SKILL");

	public static string GET_STRING_RESULT_LIMIT_BREAK_UNIT_MAX_LEVEL_TWO_PARAM => NKCStringTable.GetString("SI_DP_RESULT_LIMIT_BREAK_UNIT_MAX_LEVEL_TWO_PARAM");

	public static string GET_STRING_RESULT_BONUS_EXP => NKCStringTable.GetString("SI_DP_RESULT_BONUS_EXP");

	public static string GET_STRING_RESULT_BONUS_RESOURCE => NKCStringTable.GetString("SI_DP_RESULT_BONUS_RESOURCE");

	public static string GET_EVENT_BUFF_TYPE_RWDBOUNS_CREDIT => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_RWDBOUNS_CREDIT");

	public static string GET_EVENT_BUFF_TYPE_RWDBOUNS_ETERNIUM => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_RWDBOUNS_ETERNIUM");

	public static string GET_EVENT_BUFF_TYPE_RWDBOUNS_INFORMATION => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_RWDBOUNS_INFORMATION");

	public static string GET_EVENT_BUFF_TYPE_RWDBOUNS_EXP_PLAYER => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_RWDBOUNS_EXP_PLAYER");

	public static string GET_EVENT_BUFF_TYPE_RWDBOUNS_EXP_UNIT => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_RWDBOUNS_EXP_UNIT");

	public static string GET_EVENT_BUFF_TYPE_WARFARE_ETNM_DISCOUNT => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_WARFARE_ETNM_DISCOUNT");

	public static string GET_EVENT_BUFF_TYPE_WARFARE_DUNGEON_ETNM_DISCOUNT => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_WARFARE_DUNGEON_ETNM_DISCOUNT");

	public static string GET_EVENT_BUFF_TYPE_PVP_POINT_CHARGE => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_PVP_POINT_CHARGE");

	public static string GET_EVENT_BUFF_TYPE_CITY_MISSION_TIMEKEEP => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_CITY_MISSION_TIMEKEEP");

	public static string GET_EVENT_BUFF_TYPE_CITY_MISSION_WMMR_S_UP => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_CITY_MISSION_WMMR_S_UP");

	public static string GET_EVENT_BUFF_TYPE_NEGOTIATION_CREDIT_DISCOUNT => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_BASE_PERSONNAL_NEGOTIATION_CREDIT_DISCOUNT");

	public static string GET_EVENT_BUFF_TYPE_FACTORY_CRAFT_CREDIT_DISCOUNT => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_BASE_FACTORY_CRAFT_CREDIT_DISCOUNT");

	public static string GET_EVENT_BUFF_TYPE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT");

	public static string GET_EVENT_BUFF_SCOPE_MAINSTREAM => NKCStringTable.GetString("SI_DP_EVENT_BUFF_SCOPE_MAINSTREAM");

	public static string GET_EVENT_BUFF_SCOPE_SIDESTORY => NKCStringTable.GetString("SI_DP_EVENT_BUFF_SCOPE_SIDESTORY");

	public static string GET_EVENT_BUFF_SCOPE_WORLDMAP => NKCStringTable.GetString("SI_DP_EVENT_BUFF_SCOPE_WORLDMAP");

	public static string GET_EVENT_BUFF_SCOPE_PVP => NKCStringTable.GetString("SI_DP_EVENT_BUFF_SCOPE_PVP");

	public static string GET_EVENT_BUFF_SCOPE_ALL_WARFARE_DUNGEON => NKCStringTable.GetString("SI_DP_EVENT_BUFF_SCOPE_ALL_WARFARE_DUNGEON");

	public static string GET_EVENT_BUFF_SCOPE_ALL_WARFARE => NKCStringTable.GetString("SI_DP_EVENT_BUFF_SCOPE_ALL_WARFARE");

	public static string GET_EVENT_BUFF_SCOPE_ALL_DUNGEON => NKCStringTable.GetString("SI_DP_EVENT_BUFF_SCOPE_ALL_DUNGEON");

	public static string GET_EVENT_BUFF_TYPE_PVP_POINT_INVENTORY => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_PVP_POINT_INVENTORY");

	public static string GET_EVENT_BUFF_TYPE_PVP_POINT_REWARD => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_PVP_POINT_REWARD");

	public static string GET_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_DESC => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_OPR_SKILL_ENHANCE_COST_DISCOUNT");

	public static string GET_EVENT_BUFF_TYPE_OPERATOR_SKILL_INCRESE_RATIO_DESC => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_OPR_SKILL_ENHANCE_SUCCESS_RATE_BONUS");

	public static string GET_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_COST_DISCOUNT => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_COST_DISCOUNT");

	public static string GET_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SSR => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SSR");

	public static string GET_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SR => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SR");

	public static string GET_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_R => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_R");

	public static string GET_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_N => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_N");

	public static string GET_EVENT_BUFF_TYPE_BASE_FACTORY_POTENTIAL_SOCKET_CREDIT_DISCOUNT => NKCStringTable.GetString("SI_DP_EVENT_BUFF_TYPE_BASE_FACTORY_POTENTIAL_SOCKET_CREDIT_DISCOUNT");

	public static string GE_STRING_COMPANY_BUFFTIME_ACCOUNTLEVEL => NKCStringTable.GetString("SI_DP_COMPANY_BUFFTIME_ACCOUNTLEVEL");

	public static string GET_STRING_AUTO_RESOURCE_SUPPLY_FAIL => NKCStringTable.GetString("SI_DP_AUTO_RESOURCE_SUPPLY_FAIL");

	public static string GET_STRING_AUTO_RESOURCE_SUPPLY_FAIL_ETER_FULL => NKCStringTable.GetString("SI_DP_AUTO_RESOURCE_SUPPLY_FAIL_ETER_FULL");

	public static string GET_STRING_AUTO_RESOURCE_SUPPLY_CREDIT_DESC_01 => NKCStringTable.GetString("SI_PF_AUTO_RESOURCE_SUPPLY_CREDIT_DESC_01");

	public static string GET_STRING_AUTO_RESOURCE_SUPPLY_ETERNIUM_DESC_01 => NKCStringTable.GetString("SI_PF_AUTO_RESOURCE_SUPPLY_ETERNIUM_DESC_01");

	public static string GET_STRING_AUTO_RESOURCE_SUPPLY_MAX_CREDIT => NKCStringTable.GetString("SI_LOBBY_RESOURCE_RECHARGE_FUND_MAX_CREDIT");

	public static string GET_STRING_AUTO_RESOURCE_SUPPLY_MAX_ETERNIUM => NKCStringTable.GetString("SI_LOBBY_RESOURCE_RECHARGE_FUND_MAX_ETERNIUM");

	public static string GET_STRING_TOOLTIP_ETC_NETWORK_TITLE => NKCStringTable.GetString("SI_DP_TOOLTIP_ETC_NETWORK_TITLE");

	public static string GET_STRING_TOOLTIP_ETC_NETWORK_DESC => NKCStringTable.GetString("SI_DP_TOOLTIP_ETC_NETWORK_DESC");

	public static string GET_STRING_REPEAT_OPERATION_COST_COUNT_UNTIL_NOW => NKCStringTable.GetString("SI_DP_REPEAT_OPERATION_COST_COUNT_UNTIL_NOW");

	public static string GET_STRING_REPEAT_OPERATION_COST_COUNT => NKCStringTable.GetString("SI_DP_REPEAT_OPERATION_COST_COUNT");

	public static string GET_STRING_REPEAT_OPERATION_REPEAT_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_REPEAT_OPERATION_REPEAT_COUNT_ONE_PARAM");

	public static string GET_STRING_REPEAT_OPERATION_RED_COLOR_REPEAT_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_REPEAT_OPERATION_RED_COLOR_REPEAT_COUNT_ONE_PARAM");

	public static string GET_STRING_REPEAT_OPERATION_COMPLETE_REPEAT_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_REPEAT_OPERATION_COMPLETE_REPEAT_COUNT_ONE_PARAM");

	public static string GET_STRING_REPEAT_OPERATION_REMAIN_REPEAT_COUNT_ONE_PARAM => NKCStringTable.GetString("SI_DP_REPEAT_OPERATION_REMAIN_REPEAT_COUNT_ONE_PARAM");

	public static string GET_STRING_REPEAT_OPERATION_IS_ON_GOING => NKCStringTable.GetString("SI_DP_REPEAT_OPERATION_IS_ON_GOING");

	public static string GET_STRING_REPEAT_OPERATION_IS_TERMINATED => NKCStringTable.GetString("SI_DP_REPEAT_OPERATION_IS_TERMINATED");

	public static string GET_STRING_REPEAT_OPERATION_RESULT_TOTAL_TIME => NKCStringTable.GetString("SI_DP_REPEAT_OPERATION_RESULT_TOTAL_TIME");

	public static string GET_STRING_REPEAT_OPERATION_NEED_MORE_RESOURCE => NKCStringTable.GetString("SI_DP_REPEAT_OPERATION_NEED_MORE_RESOURCE");

	public static string GET_STRING_REPEAT_OPERATION_COST_MORE_REQUIRED => NKCStringTable.GetString("SI_DP_REPEAT_OPERATION_COST_MORE_REQUIRED");

	public static string GET_STRING_REPEAT_OPERATION_FAIL => NKCStringTable.GetString("SI_DP_REPEAT_OPERATION_FAIL");

	public static string GET_STRING_PATCHER_PC_NEW_APP_AVAILABLE_ZLONG => NKCStringTable.GetString("SI_DP_PATCHER_PC_NEW_APP_AVAILABLE_ZLONG");

	public static string GET_STRING_REFRESH_DATA => NKCStringTable.GetString("SI_DP_PATCHER_REFRESH_DATA");

	public static string GET_STRING_PATCHER_NEED_UPDATE => NKCStringTable.GetString("SI_DP_PATCHER_NEED_UPDATE");

	public static string GET_STRING_PATCHER_CAN_UPDATE => NKCStringTable.GetString("SI_DP_PATCHER_CAN_UPDATE");

	public static string GET_STRING_PATCHER_MOVE_TO_MARKET => NKCStringTable.GetString("SI_DP_PATCHER_MOVE_TO_MARKET");

	public static string GET_STRING_PATCHER_CONTINUE => NKCStringTable.GetString("SI_DP_PATCHER_CONTINUE");

	public static string GET_STRING_NOTICE_DOWNLOAD_ONE_PARAM => NKCStringTable.GetString("SI_DP_PATCHER_NOTICE_DOWNLOAD_ONE_PARAM");

	public static string GET_STRING_NOTICE_ASK_DOWNLOAD_WITH_PROLOGUE_ONE_PARAM => NKCStringTable.GetString("SI_DP_PATCHER_ASK_DOWNLOAD_WITH_PROLOGUE_ONE_PARAM");

	public static string GET_STRING_NOTICE_PLAY_WITHOUT_DOWNLOAD => NKCStringTable.GetString("SI_DP_PATCHER_PLAY_WITHOUT_DOWNLOAD");

	public static string GET_STRING_NOTICE_ASK_DOWNLOAD_IMMEDIATELY_OR_WITH_PROLOGUE => NKCStringTable.GetString("SI_DP_PATCHER_ASK_DOWNLOAD_IMMEDIATELY_OR_WITH_PROLOGUE");

	public static string GET_STRING_DECONNECT_INTERNET => NKCStringTable.GetString("SI_DP_PATCHER_DECONNECT_INTERNET");

	public static string GET_STRING_RETRY => NKCStringTable.GetString("SI_DP_PATCHER_RETRY");

	public static string GET_STRING_FAIL_VERSION => NKCStringTable.GetString("SI_DP_PATCHER_FAIL_VERSION");

	public static string GET_STRING_FAIL_PATCHDATA => NKCStringTable.GetString("SI_DP_PATCHER_FAIL_PATCHDATA");

	public static string GET_STRING_ERROR_DOWNLOAD_ONE_PARAM => NKCStringTable.GetString("SI_DP_PATCHER_ERROR_DOWNLOAD_ONE_PARAM");

	public static string GET_STRING_PATCHER_CHECKING_VERSION_INFORMATION => NKCStringTable.GetString("SI_DP_PATCHER_CHECKING_VERSION_INFORMATION");

	public static string GET_STRING_PATCHER_DOWNLOADING => NKCStringTable.GetString("SI_DP_PATCHER_DOWNLOADING");

	public static string GET_STRING_PATCHER_FINISHING_PATCHPROCESS => NKCStringTable.GetString("SI_DP_PATCHER_FINISHING_PATCHPROCESS");

	public static string GET_STRING_PATCHER_INITIALIZING => NKCStringTable.GetString("SI_DP_PATCHER_INITIALIZING");

	public static string GET_STRING_PATCHER_ERROR_DOWNLOADING_THREE_PARMA => NKCStringTable.GetString("SI_DP_PATCHER_ERROR_DOWNLOADING_THREE_PARMA");

	public static string GET_STRING_PATCHER_CAN_BACKGROUND_DOWNLOAD => NKCStringTable.GetString("SI_DP_PATCHER_CAN_BACKGROUND_DOWNLOAD");

	public static string GET_NXPATCHER_DOWNLOAD_COMPLETE => NKCStringTable.GetString("SI_DP_PATCHER_NXPATCHER_DOWNLOAD_COMPLETE");

	public static string GET_NXPATCHER_DOWNLOAD_CONTINUE => NKCStringTable.GetString("SI_DP_PATCHER_NXPATCHER_DOWNLOAD_CONTINUE");

	public static string GET_NXPATCHER_DOWNLOAD_PROGRESS => NKCStringTable.GetString("SI_DP_PATCHER_NXPATCHER_DOWNLOAD_PROGRESS");

	public static string GET_STRING_PATCHER_WARNING => NKCStringTable.GetString("SI_DP_PATCHER_WARNING");

	public static string GET_STRING_PATCHER_CONFIRM => NKCStringTable.GetString("SI_DP_PATCHER_CONFIRM");

	public static string GET_STRING_PATCHER_CANCEL => NKCStringTable.GetString("SI_DP_PATCHER_CANCEL");

	public static string GET_STRING_PATCHER_ERROR => NKCStringTable.GetString("SI_DP_PATCHER_ERROR");

	public static string GET_STRING_PATCHER_NOTICE => NKCStringTable.GetString("SI_DP_PATCHER_NOTICE");

	public static string GET_DEV_CONSOLE_CHEAT_EMOTICON_CHEAT => NKCStringTable.GetString("SI_PF_DEV_CONSOLE_CHEAT_EMOTICON_CHEAT");

	public static string GET_DEV_CONSOLE_CHEAT_MOOJUCK_MODE_DESC_01 => NKCStringTable.GetString("SI_PF_DEV_CONSOLE_CHEAT_moojuck_MODE_DESC_01");

	public static string GET_DEV_CONSOLE_CHEAT_WAREFARE_UNBREAKABLE_MODE_DESC_01 => NKCStringTable.GetString("SI_PF_DEV_CONSOLE_CHEAT_WAREFARE_UNBREAKABLE_MODE_DESC_01");

	public static string GET_DEV_CONSOLE_CHEAT_ACCOUNT_RESET_DESC => NKCStringTable.GetString("SI_PF_DEV_CONSOLE_CHEAT_ACCOUNT_RESET_DESC");

	public static string GET_DEV_CONSOLE_CHEAT_MANAGEMENT_RESET_DESC => NKCStringTable.GetString("SI_PF_DEV_CONSOLE_CHEAT_MANAGEMENT_RESET_DESC");

	public static string GET_DEV_CONSOLE_TUTORIAL_NECESSARY_DESC_01 => NKCStringTable.GetString("SI_PF_DEV_CONSOLE_CHEAT_TUTORIAL_NECESSARY_DESC_01");

	public static string GET_DEV_CONSOLE_TUTORIAL_COMPLETE_DESC_01 => NKCStringTable.GetString("SI_PF_DEV_CONSOLE_CHEAT_TUTORIAL_COMPLETE_DESC_01");

	public static string GET_DEV_CONSOLE_WORLDMAP_NO_LEADER => NKCStringTable.GetString("SI_PF_DEV_CONSOLE_CHEAT_LEADER_ERROR");

	public static string GET_DEV_CONSOLE_WORLDMAP_MISSION_NOT_GOING => NKCStringTable.GetString("SI_PF_DEV_CONSOLE_CHEAT_MISSION_ERROR");

	public static string GET_DEV_CONSOLE_WORLDMAP_MISSION_LEFT_TIME_TWO_PARAM => NKCStringTable.GetString("SI_PF_DEV_CONSOLE_CHEAT_MISSION_TIME");

	public static string GET_DEV_CONSOLE_WORLDMAP_NO_EVENT => NKCStringTable.GetString("SI_PF_DEV_CONSOLE_CHEAT_EVENT_ERROR");

	public static string GET_DEV_CONSOLE_WORLDMAP_EVENT_LEFT_TIME_TWO_PARAM => NKCStringTable.GetString("SI_PF_DEV_CONSOLE_CHEAT_EVENT_TIME");

	public static string GET_DEV_CONSOLE_WORLDMAP_NO_EXIST_EVENT_ID => NKCStringTable.GetString("SI_PF_DEV_CONSOLE_CHEAT_EVENT_NOTFOUND");

	public static string GET_STRING_REPLAY => NKCStringTable.GetString("SI_PF_REPLAY");

	public static string GET_STRING_REPLAY_OPTION_LEAVE_TITLE => NKCStringTable.GetString("SI_PF_OPTION_REPLAY_LEAVE_TITLE");

	public static string GET_STRING_REPLAY_OPTION_LEAVE_DESC => NKCStringTable.GetString("SI_PF_OPTION_REPLAY_LEAVE_DESC");

	public static string GET_STRING_EVENT_TOTAL_PAY => NKCStringTable.GetString("SI_PF_EVENT_S_MEDAL_CASH_INPUT_01");

	public static string GET_STRING_EVENT_TOTAL_PAY_RETURN => NKCStringTable.GetString("SI_PF_EVENT_S_MEDAL_CASH_INPUT_02");

	public static string GET_STRING_EVENT_MENU => NKCStringTable.GetString("SI_DP_MENU_EVENT");

	public static string GET_STRING_EXCEPTION_EVENT_EXPIRED_POPUP => NKCStringTable.GetString("SI_MENU_EXCEPTION_EVENT_EXPIRED_POPUP");

	public static string GET_STRING_EVENT_BINGO_MILEAGE => NKCStringTable.GetString("SI_DP_EVENT_BINGO_SELECT_MILLAGE_INFORMATION");

	public static string GET_STRING_EVENT_BINGO_SPECIAL => NKCStringTable.GetString("SI_DP_EVENT_BINGO_SELECT_BUTTON_OFF_TO_ON");

	public static string GET_STRING_EVENT_BINGO_SPECIAL_CANCEL => NKCStringTable.GetString("SI_DP_EVENT_BINGO_SELECT_BUTTON_ON_TO_OFF");

	public static string GET_STRING_EVENT_BINGO_REWARD_TITLE => NKCStringTable.GetString("SI_DP_EVENT_BINGO_REWARD_BINGO_LINE_TITLE");

	public static string GET_STRING_EVENT_BINGO_REWARD_SLOT_PROGRESS => NKCStringTable.GetString("SI_DP_EVENT_BINGO_REWARD_GET");

	public static string GET_STRING_EVENT_BINGO_REWARD_SLOT_COMPLETE => NKCStringTable.GetString("SI_DP_EVENT_BINGO_REWARD_GET_COMPLET");

	public static string GET_STRING_EVENT_BINGO_TRY_CONFIRM => NKCStringTable.GetString("SI_DP_EVENT_BINGO_GET_QUESTION_NUMBER_NORMAL_POPUP");

	public static string GET_STRING_EVENT_BINGO_USE_MILEAGE => NKCStringTable.GetString("SI_DP_EVENT_BINGO_GET_QUESTION_NUMBER_SELECT_POPUP");

	public static string GET_STRING_EVENT_BINGO_REMAIN_MILEAGE => NKCStringTable.GetString("SI_DP_EVENT_BINGO_GET_INFOMATION_HAVE_POINT");

	public static string GET_STRING_EVENT_BINGO_COMPLETE => NKCStringTable.GetString("SI_PF_EVENT_BINGO_ALL_CLEAR_TOAST_MESSAGE");

	public static string GET_STRING_EVENT_RACE_RESULT_TEAM_RED => NKCStringTable.GetString("SI_PF_EVENT_RACE_RESULT_TEAM_RED");

	public static string GET_STRING_EVENT_RACE_RESULT_TEAM_BLUE => NKCStringTable.GetString("SI_PF_EVENT_RACE_RESULT_TEAM_BLUE");

	public static string GET_STRING_EVENT_RACE_BET_MAX_DESC => NKCStringTable.GetString("SI_DP_RACE_BETTING_MAX_ERROR");

	public static string GET_STRING_EVENT_RACE_BET_ITEM_NOT_ENOUGH => NKCStringTable.GetString("SI_DP_RACE_BETTING_ITEM_NOT_ENOUGH_ERROR");

	public static string GET_STRING_EVENT_RACE_BET_HISTORY_DAY_01 => NKCStringTable.GetString("SI_PF_POPUP_EVENT_RACE_DAY");

	public static string GET_STRING_EVENT_RACE_BET_HISTORY_REWARD_DESC_04 => NKCStringTable.GetString("SI_PF_POPUP_EVENT_RACE_BETTING_HISTORY");

	public static string GET_STRING_EVENT_RACE_BET_TEAM_RED => NKCStringTable.GetString("SI_PF_POPUP_EVENT_RACE_RED_TEAM");

	public static string GET_STRING_EVENT_RACE_BET_TEAM_BLUE => NKCStringTable.GetString("SI_PF_POPUP_EVENT_RACE_BLUE_TEAM");

	public static string GET_STRING_EVENT_RACE_NONE_BETTING_HISTORY => NKCStringTable.GetString("SI_PF_POPUP_EVENT_RACE_NONE_BETTING_HISTORY");

	public static string GET_STRING_EVENT_RACE_BETTING_TUTORIAL => NKCStringTable.GetString("SI_PF_POPUP_EVENT_RACE_BETTING_TUTORIAL");

	public static string GET_STRING_EVENT_RACE_BETTING_LAST_DAY => NKCStringTable.GetString("SI_DP_RACE_BETTING_LAST_DAY");

	public static string GET_STRING_EVENT_RACE_BETTING_MAINTENANCE_TIME => NKCStringTable.GetString("SI_PF_POPUP_EVENT_RACE_CALCULATE");

	public static string GET_STRING_EVENT_RACE_BETTING_RATIO => NKCStringTable.GetString("SI_PF_POPUP_EVENT_RACE_RATIO");

	public static string GET_STRING_EVENT_RACE_BETTING_COUNT_NOT_ENOUGH => NKCStringTable.GetString("SI_DP_RACE_BETTING_ITEM_ERROR");

	public static string GET_STRING_EVENT_RACE_BETTING_NOT_SELECT_TEAM => NKCStringTable.GetString("SI_DP_RACE_BETTING_NOT_SELECT_TEAM");

	public static string GET_STRING_CONSORTIUM_INTRO => NKCStringTable.GetString("SI_LOBBY_RIGHT_MENU_3_CONSORTIUM");

	public static string GET_STRING_CONSORTIUM_JOIN_COOLTIME_ONE_PARAM => NKCStringTable.GetString("SI_DP_CONSORTIUM_JOIN_COOLTIME_DESC");

	public static string GET_STRING_CONSORTIUM_INTRO_JOIN_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_INTRO_JOIN_TEXT");

	public static string GET_STRING_CONSORTIUM_INTRO_FOUNDATION => NKCStringTable.GetString("SI_PF_CONSORTIUM_INTRO_FOUNDATION_TEXT");

	public static string GET_STRING_CONSORTIUM_CREATE_NAME_SUB_GUIDE_USEFUL => NKCStringTable.GetString("SI_PF_CONSORTIUM_CREATE_NAME_SUB_GUIDE_USEFUL_DESC");

	public static string GET_STRING_CONSORTIUM_CREATE_NAME_SUB_GUIDE_BADWORD => NKCStringTable.GetString("SI_PF_CONSORTIUM_CREATE_NAME_SUB_GUIDE_BADWORD_DESC");

	public static string GET_STRING_CONSORTIUM_CREATE_NAME_SUB_GUIDE_BASIC_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_CREATE_NAME_SUB_GUIDE_BASIC_DESC");

	public static string GET_STRING_CONSORTIUM_CREATE_NAME_SUB_GUIDE_BASIC_DESC_GLOBAL => NKCStringTable.GetString("SI_PF_CONSORTIUM_CREATE_NAME_SUB_GUIDE_BASIC_DESC_GLOBAL");

	public static string GET_STRING_CONSORTIUM_CREATE_CONFIRM_POPUP_TITLE => NKCStringTable.GetString("SI_PF_CONSORTIUM_CREATE_CONFIRM_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_CREATE_CONFIRM_POPUP_BODY => NKCStringTable.GetString("SI_DP_CONSORTIUM_CREATE_CONFIRM_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_JOIN_CONFIRM_SITUATION => NKCStringTable.GetString("SI_PF_CONSORTIUM_JOIN_CONFIRM_SITUATION_DESC");

	public static string GET_STRING_CONSORTIUM_ATTENDANCE_REWARD_CONDITION => NKCStringTable.GetString("SI_DP_CONSORTIUM_ATTENDANCE_REWARD_CONDITION");

	public static string GET_STRING_CONSORTIUM_POPUP_ATTENDANCE_REWARD_TITLE => NKCStringTable.GetString("SI_PF_CONSORTIUM_POPUP_ATTENDANCE_REWARD_TITLE");

	public static string GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_RIGHTOFF_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_CREATE_JOIN_METHOD_RIGHTOFF_DESC");

	public static string GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_CONFIRM_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_CREATE_JOIN_METHOD_CONFIRM_DESC");

	public static string GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_BLIND_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_CREATE_JOIN_METHOD_BLIND_DESC");

	public static string GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_GUIDE_RIGHTOFF_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_CREATE_JOIN_METHOD_GUIDE_RIGHTOFF_DESC");

	public static string GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_GUIDE_CONFIRM_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_CREATE_JOIN_METHOD_GUIDE_CONFIRM_DESC");

	public static string GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_GUIDE_BLIND_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_CREATE_JOIN_METHOD_GUIDE_BLIND_DESC");

	public static string GET_STRING_CONSORTIUM_POPUP_ATTENDANCE_REWARD_BASIC => NKCStringTable.GetString("SI_PF_CONSORTIUM_POPUP_ATTENDANCE_REWARD_BASIC");

	public static string GET_STRING_CONSORTIUM_JOIN_CONFIRM_JOIN_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_JOIN_CONFIRM_JOIN_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_JOIN_RIGHTOFF_JOIN_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_JOIN_RIGHTOFF_JOIN_POPUP_BODY_DESC");

	public static string GET_CONSORTIUM_JOIN_RIGHTOFF_JOIN_POPUP_APPROVE_BTN_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_JOIN_RIGHTOFF_JOIN_POPUP_APPROVE_BTN_DESC");

	public static string GET_CONSORTIUM_JOIN_CONFIRM_JOIN_POPUP_APPROVE_BTN_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_JOIN_CONFIRM_JOIN_POPUP_APPROVE_BTN_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_CONFIRM_JOIN_CONFIRM_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_CONFIRM_JOIN_CONFIRM_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_CONFIRM_JOIN_CONFIRM_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_MEMBER_CONFIRM_JOIN_CONFIRM_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_CONFIRM_JOIN_REFUSE_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_CONFIRM_JOIN_REFUSE_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_CONFIRM_JOIN_REFUSE_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_MEMBER_CONFIRM_JOIN_REFUSE_POPUP_BODY_DESC");

	public static string GET_CONSORTIUM_JOIN_CONFIRM_JOIN_CANCEL_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_JOIN_CONFIRM_JOIN_CANCEL_POPUP_TITLE_DESC");

	public static string GET_CONSORTIUM_JOIN_CONFIRM_JOIN_CANCEL_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_JOIN_CONFIRM_JOIN_CANCEL_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_JOIN_CONFIRM_JOIN_SUCCESS_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_JOIN_CONFIRM_JOIN_SUCCESS_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_JOIN_CONFIRM_JOIN_SUCCESS_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_JOIN_CONFIRM_JOIN_SUCCESS_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_OPTION_DATA_SAVE_CANCEL_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DATA_SAVE_CANCEL_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_OPTION_DATA_SAVE_CANCEL_POPUP_BODY_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DATA_SAVE_CANCEL_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_OPTION_DATA_SAVE_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DATA_SAVE_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_OPTION_DATA_SAVE_POPUP_BODY_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DATA_SAVE_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_LOBBY_INFORMATION_CHANGE_OVERLAY_TITLE_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_INFORMATION_CHANGE_OVERLAY_TITLE_TEXT");

	public static string GET_STRING_CONSORTIUM_LOBBY_INFORMATION_CHANGE_OVERLAY_BODY_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_INFORMATION_CHANGE_OVERLAY_BODY_TEXT");

	public static string GET_STRING_CONSORTIUM_MEMBER_GRADE_UP => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_GRADE_UP");

	public static string GET_STRING_CONSORTIUM_MEMBER_GRADE_UP_CONFIRM_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_MEMBER_GRADE_UP_CONFIRM_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_GRADE_DOWN => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_GRADE_DOWN");

	public static string GET_STRING_CONSORTIUM_MEMBER_GRADE_DOWN_CONFIRM_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_MEMBER_GRADE_DOWN_CONFIRM_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_OPTION_DISMANTLE_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DISMANTLE_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_OPTION_DISMANTLE_POPUP_BODY_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DISMANTLE_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_OPTION_DISMANTLE_POPUP_CONFIRM_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DISMANTLE_POPUP_CONFIRM_DESC");

	public static string GET_STRING_CONSORTIUM_OPTION_DISMANTLE_CANCEL_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DISMANTLE_CANCEL_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_OPTION_DISMANTLE_CANCEL_POPUP_BODY_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DISMANTLE_CANCEL_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_OPTION_DISMANTLE_CANCEL_POPUP_CONFIRM_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DISMANTLE_CANCEL_POPUP_CONFIRM_DESC");

	public static string GET_STRING_CONSORTIUM_OPTION_DISMANTLE_INFORMATION_BTN_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DISMANTLE_INFORMATION_BTN_TEXT");

	public static string GET_STRING_CONSORTIUM_OPTION_DISMANTLE_HANDOVER_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DISMANTLE_HANDOVER_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_OPTION_DISMANTLE_HANDOVER_POPUP_BODY_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DISMANTLE_HANDOVER_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_OPTION_DISMANTLE_HANDOVER_CONFIRM_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DISMANTLE_HANDOVER_CONFIRM_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_OPTION_DISMANTLE_HANDOVER_CONFIRM_POPUP_BODY_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OPTION_DISMANTLE_HANDOVER_CONFIRM_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_GRADE_HANDOVER_CONFIRM_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_GRADE_HANDOVER_CONFIRM_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_GRADE_HANDOVER_CONFIRM_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_MEMBER_GRADE_HANDOVER_CONFIRM_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_EXIT_CONFIRM_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_EXIT_CONFIRM_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_EXIT_CONFIRM_POPUP_BODY_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_EXIT_CONFIRM_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_FORCE_EXIT_CONFIRM_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_FORCE_EXIT_CONFIRM_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_FORCE_EXIT_CONFIRM_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_MEMBER_FORCE_EXIT_CONFIRM_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_POPUP_INVITE_TITLE => NKCStringTable.GetString("SI_PF_CONSORTIUM_POPUP_INVITE_TITLE");

	public static string GET_STRING_CONSORTIUM_INVITE_SEND_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_INVITE_SEND_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_INVITE_SEND_SUCCESS_BODY_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_INVITE_SEND_SUCCESS_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_INVITE => NKCStringTable.GetString("SI_PF_CONSORTIUM_INVITE");

	public static string GET_STRING_CONSORTIUM_JOIN_INVITE_JOIN_AGREE_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_JOIN_INVITE_JOIN_AGREE_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_INVITE_JOIN_REJECT_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_INVITE_JOIN_REJECT_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_INVITE_JOIN_REJECT_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_INVITE_JOIN_REJECT_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_INTRODUCE_WRITE_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_INTRODUCE_WRITE_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_INTRODUCE_WRITE_POPUP_BODY_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_INTRODUCE_WRITE_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_INTRODUCE_WRITE_POPUP_GUIDE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_INTRODUCE_WRITE_POPUP_GUIDE_DESC");

	public static string GET_STRING_CONSORTIUM_OVERLAY_MESSAGE_HEAD_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_OVERLAY_MESSAGE_HEAD_DESC");

	public static string GET_STRING_CONSORTIUM_OVERLAY_MESSAGE_BODY_JOIN => NKCStringTable.GetString("SI_DP_CONSORTIUM_OVERLAY_MESSAGE_BODY_JOIN");

	public static string GET_STRING_CONSORTIUM_OVERLAY_MESSAGE_BODY_LEVEL_UP => NKCStringTable.GetString("SI_DP_CONSORTIUM_OVERLAY_MESSAGE_BODY_LEVEL_UP");

	public static string GET_STRING_CONSORTIUM_ATTENDANCE_SUCCESS_TOAST_MESSAGE_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_ATTENDANCE_SUCCESS_TOAST_MESSAGE_TEXT");

	public static string GET_STRING_CONSORTIUM_MEMBER_INTRODUCE_WRITE_SUCCESS_TOAST_MESSAGE_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_INTRODUCE_WRITE_SUCCESS_TOAST_MESSAGE_TEXT");

	public static string GET_STRING_CONSORTIUM_MEMBER_CHANGE_PERMISSION_TOAST_MESSAGE_TITLE_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_MEMBER_CHANGE_PERMISSION_TOAST_MESSAGE_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_GRADE_UP_INFORMATION_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_MEMBER_GRADE_UP_INFORMATION_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_FRADE_DOWN_INFORMATION_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_MEMBER_FRADE_DOWN_INFORMATION_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_CHANGE_MASTER_TOAST_MESSAGE_TITLE_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_MEMBER_CHANGE_MASTER_TOAST_MESSAGE_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_GRADE_HANDOVER_INFORMATION_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_MEMBER_GRADE_HANDOVER_INFORMATION_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_FORCE_EXIT_TOAST_MESSAGE_TITLE_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_MEMBER_FORCE_EXIT_TOAST_MESSAGE_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_MEMBER_FORCE_EXIT_INFORMATION_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_MEMBER_FORCE_EXIT_INFORMATION_POPUP_BODY_DESC");

	public static string GET_STRING_CONFORTIUM_FAIL_GUILD_NOT_BELONG_AT_PRESENT_POPUP_TEXT => NKCStringTable.GetString("SI_PF_CONFORTIUM_FAIL_GUILD_NOT_BELONG_AT_PRESENT_POPUP_TEXT");

	public static string GET_STRING_CONSORTIUM_MEMBER_SORT_LIST_GRADE => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_SORT_LIST_GRADE");

	public static string GET_STRING_CONSORTIUM_MEMBER_SORT_LIST_SCORE => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_SORT_LIST_SCORE");

	public static string GET_STRING_CONSORTIUM_MEMBER_SORT_LIST_SCORE_ALL => NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_SORT_LIST_SCORE_ALL");

	public static string GET_STRING_CONSORTIUM_POPUP_DONATION_COMFIRM_TITLE => NKCStringTable.GetString("SI_PF_CONSORTIUM_POPUP_DONATION_COMFIRM_TITLE");

	public static string GET_STRING_CONSORTIUM_POPUP_DONATION_COMFIRM_BODY => NKCStringTable.GetString("SI_PF_CONSORTIUM_POPUP_DONATION_COMFIRM_BODY");

	public static string GET_STRING_CONSORTIUM_DONATION_SUCCESS_TOAST_TEXT => NKCStringTable.GetString("SI_DP_CONSORTIUM_DONATION_SUCCESS_TOAST_TEXT");

	public static string GET_STRING_CONSORTIUM_WELFARE_PERSONAL_CONFIRM_TITLE => NKCStringTable.GetString("SI_PF_CONSORTIUM_WELFARE_PERSONAL_CONFIRM_TITLE");

	public static string GET_STRING_CONSORTIUM_WELFARE_SUBTAB_PERSONAL_CONFIRM_BODY => NKCStringTable.GetString("SI_DP_CONSORTIUM_WELFARE_SUBTAB_PERSONAL_CONFIRM_BODY");

	public static string GET_STRING_CONSORTIUM_WELFARE_GUILD_CONFIRM_TITLE => NKCStringTable.GetString("SI_PF_CONSORTIUM_WELFARE_GUILD_CONFIRM_TITLE");

	public static string GET_STRING_CONSORTIUM_WELFARE_SUBTAB_GUILD_CONFIRM_BODY => NKCStringTable.GetString("SI_DP_CONSORTIUM_WELFARE_SUBTAB_GUILD_CONFIRM_BODY");

	public static string GET_STRING_CONSORTIUM_WELFARE_BUY_POINT_TITLE => NKCStringTable.GetString("SI_PF_CONSORTIUM_WELFARE_BUY_POINT_TITLE");

	public static string GET_STRING_CONSORTIUM_WELFARE_BUY_POINT_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_WELFARE_BUY_POINT_DESC");

	public static string GET_STRING_CONSORTIUM_POPUP_MISSION_REFRESH_CONFIRM_TITLE => NKCStringTable.GetString("SI_PF_CONSORTIUM_POPUP_MISSION_REFRESH_CONFIRM_TITLE");

	public static string GET_STRING_CONSORTIUM_POPUP_MISSION_REFRESH_CONFIRM_BODY => NKCStringTable.GetString("SI_PF_CONSORTIUM_POPUP_MISSION_REFRESH_CONFIRM_BODY");

	public static string GET_STRING_CONSORTIUM_POPUP_DONATION_REFRESH_FREE_TEXT => NKCStringTable.GetString("SI_DP_CONSORTIUM_POPUP_DONATION_REFRESH_FREE_TEXT");

	public static string GET_STRING_CONSORTIUM_MISSION_REFRESH_PROGRESS_TITLE_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_MISSION_REFRESH_PROGRESS_TITLE_TEXT");

	public static string GET_STRING_CONSORTIUM_MISSION_REFRESH_PROGRESS_DESC_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_MISSION_REFRESH_PROGRESS_DESC_TEXT");

	public static string GET_STRING_CONSORTIUM_RANKING_TOP_INFO_EXP => NKCStringTable.GetString("SI_PF_CONSORTIUM_RANKING_TOP_INFO_EXP");

	public static string GET_STRING_CONSORTIUM_RANKING_TOP_INFO_DAMAGE => NKCStringTable.GetString("SI_PF_CONSORTIUM_RANKING_TOP_INFO_DAMAGE");

	public static string GET_STRING_CONSORTIUM_NAME_CHANGE => NKCStringTable.GetString("SI_PF_CONSORTIUM_NAME_CHANGE_POPUP_TITLE");

	public static string GET_STRING_CONSORTIUM_NAME_CHANGE_CONFIRM => NKCStringTable.GetString("SI_PF_CONSORTIUM_NAME_CHANGE_POPUP_DESC");

	public static string GET_STRING_CONSORTIUM_NAME_CHANGE_FREE_COUNT => NKCStringTable.GetString("SI_PF_CONSORTIUM_NAME_CHANGE_FREE_DESC");

	public static string GET_STRING_CONSORTIUM_NAME_CHANGE_NOTICE => NKCStringTable.GetString("SI_PF_CONSORTIUM_FOUNDATION_CHAT_NAME_CHANGE_MAIN");

	public static string GET_STRING_CONSORTIUM_LOBBY_MESSAGE_IN_PROGRESS => NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_MESSAGE_IN_PROGRESS");

	public static string GET_STRING_CONSORTIUM_LOBBY_MESSAGE_RESULT => NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_MESSAGE_RESULT");

	public static string GET_STRING_CONSORTIUM_LOBBY_MESSAGE_CALCULATE => NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_MESSAGE_CALCULATE");

	public static string GET_STRING_CONSORTIUM_LOBBY_MESSAGE_NOT_IN_PROGRESS => NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_MESSAGE_NOT_IN_PROGRESS");

	public static string GET_STRING_CONSORTIUM_LOBBY_MESSAGE_UNABLE => NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_MESSAGE_UNABLE");

	public static string GET_STRING_CONSORTIUM_SEASON_OPEN_BEFORE_TOAST_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_SEASON_OPEN_BEFORE_TOAST_TEXT");

	public static string GET_STRING_CONSORTIUM_TIME_OUT_ERROR_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_TIME_OUT_ERROR_TEXT");

	public static string GET_STRING_CONSORTIUM_SESSION_ATTACK_ING_USER_INFO => NKCStringTable.GetString("SI_DP_CONSORTIUM_SESSION_ATTACK_ING_USER_INFO");

	public static string GET_STRING_CONSORTIUM_SESSION_PROGRESS_OF_ROUND_INFO => NKCStringTable.GetString("SI_DP_CONSORTIUM_SESSION_PROGRESS_OF_ROUND_INFO");

	public static string GET_STRING_CONSORTIUM_SESSION_PROGRESS_OF_ROUND_TASK_INFO => NKCStringTable.GetString("SI_DP_CONSORTIUM_SESSION_PROGRESS_OF_ROUND_TASK_INFO");

	public static string GET_STRING_CONSORTIUM_COOP_FRONT_COUNT_ARENA => NKCStringTable.GetString("SI_PF_CONSORTIUM_COOP_FRONT_COUNT_ARENA");

	public static string GET_STRING_CONSORTIUM_COOP_FRONT_COUNT_RAID => NKCStringTable.GetString("SI_PF_CONSORTIUM_COOP_FRONT_COUNT_RAID");

	public static string GET_STRING_CONSORTIUM_DUNGEON_DUNGEON_UI_ARENA_INFO => NKCStringTable.GetString("SI_DP_CONSORTIUM_DUNGEON_DUNGEON_UI_ARENA_INFO");

	public static string GET_STRING_CONSORTIUM_DUNGEON_ARTIFACT_DUNGEON_CHALLENGE_INFO => NKCStringTable.GetString("SI_DP_CONSORTIUM_DUNGEON_ARTIFACT_DUNGEON_CHALLENGE_INFO");

	public static string GET_STRING_CONSORTIUM_DUNGEON_PLAY_COUNT_BUY_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_DUNGEON_PLAY_COUNT_BUY_TEXT");

	public static string GET_STRING_CONSORTIUM_DUNGEON_PLAY_COUNT_BUY_SUCCESS_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_DUNGEON_PLAY_COUNT_BUY_SUCCESS_TEXT");

	public static string GET_STRING_CONSORTIUM_DUNGEON_ARTIFACT_DUNGEON_POPUP_TITLE => NKCStringTable.GetString("SI_DP_CONSORTIUM_DUNGEON_ARTIFACT_DUNGEON_POPUP_TITLE");

	public static string GET_STRING_CONSORTIUM_DUNGEON_RAID_UI_LEVEL_INFO => NKCStringTable.GetString("SI_DP_CONSORTIUM_DUNGEON_RAID_UI_LEVEL_INFO");

	public static string GET_STRING_CONSORTIUM_DUNGEON_RESULT_BOSS_LEVEL_INFO => NKCStringTable.GetString("SI_DP_CONSORTIUM_DUNGEON_RESULT_BOSS_LEVEL_INFO");

	public static string GET_STRING_CONSORTIUM_DUNGEON_RAID_EXTRA_BOSS => NKCStringTable.GetString("SI_PF_GUILD_DUNGEON_EXTRA");

	public static string GET_STRING_MOVE_TO_BOSS_ELIMINATE => NKCStringTable.GetString("SI_DP_MOVE_TO_BOSS_ELIMINATE");

	public static string GET_STRING_POPUP_CONSORTIUM_COOP_SESSION_END_SUB_01_TEXT => NKCStringTable.GetString("SI_PF_POPUP_CONSORTIUM_COOP_SESSION_END_SUB_01_TEXT");

	public static string GET_STRING_POPUP_CONSORTIUM_COOP_SESSION_END_SUB_02_TEXT => NKCStringTable.GetString("SI_PF_POPUP_CONSORTIUM_COOP_SESSION_END_SUB_02_TEXT");

	public static string GET_STRING_CONSORTIUM_SESSION_END_INFORMATION_TEXT => NKCStringTable.GetString("SI_DP_CONSORTIUM_SESSION_END_INFORMATION_TEXT");

	public static string GET_STRING_POPUP_CONSORTIUM_COOP_END_SUB_01_TEXT => NKCStringTable.GetString("SI_PF_POPUP_CONSORTIUM_COOP_END_SUB_01_TEXT");

	public static string GET_STRING_POPUP_CONSORTIUM_COOP_END_SUB_02_TEXT => NKCStringTable.GetString("SI_PF_POPUP_CONSORTIUM_COOP_END_SUB_02_TEXT");

	public static string GET_STRING_CONSORTIUM_SEASION_END_INFORMATION_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_SEASION_END_INFORMATION_TEXT");

	public static string GET_STRING_POPUP_CONSORTIUM_COOP_RESULT_TITLE01 => NKCStringTable.GetString("SI_PF_POPUP_CONSORTIUM_COOP_RESULT_TITLE01");

	public static string GET_STRING_POPUP_CONSORTIUM_COOP_RESULT_TITLE02 => NKCStringTable.GetString("SI_PF_POPUP_CONSORTIUM_COOP_RESULT_TITLE02");

	public static string GET_STRING_CONSORTIUM_DUNGEON_RESULT_BOSS_SUMMARY_INFO => NKCStringTable.GetString("SI_PF_CONSORTIUM_DUNGEON_RESULT_BOSS_SUMMARY_INFO");

	public static string GET_STRING_CONSORTIUM_DUNGEON_RESULT_BOSS_SUMMARY_INFO_FAIL => NKCStringTable.GetString("SI_PF_CONSORTIUM_DUNGEON_RESULT_BOSS_SUMMARY_INFO_FAIL");

	public static string GET_STRING_CONSORTIUM_DUNGEON_RESULT_BOSS_SUMMARY_INFO_FAIL_2 => NKCStringTable.GetString("SI_DP_CONSORTIUM_DUNGEON_RESULT_BOSS_SUMMARY_INFO_FAIL");

	public static string GET_STRING_CONSORTIUM_DUNGEON_RESULT_BOSS_SUMMARY_INFO_2 => NKCStringTable.GetString("SI_PF_CONSORTIUM_DUNGEON_RESULT_BOSS_SUMMARY_INFO_2");

	public static string GET_STRING_CONSORTIUM_DUNGEON_MENU_SEASON_REWARD_KILL_SCORE_STATUS => NKCStringTable.GetString("SI_PF_CONSORTIUM_DUNGEON_MENU_SEASON_REWARD_KILL_SCORE_STATUS");

	public static string GET_STRING_CONSORTIUM_DUNGEON_MENU_SEASON_REWARD_PARTICIPATION_SCORE_STATUS => NKCStringTable.GetString("SI_PF_CONSORTIUM_DUNGEON_MENU_SEASON_REWARD_PARTICIPATION_SCORE_STATUS");

	public static string GET_STRING_CONSORTIUM_DUNGEON_INFORMATION_CHANGE_OVERLAY_BODY_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_DUNGEON_INFORMATION_CHANGE_OVERLAY_BODY_TEXT");

	public static string GET_STRING_CONSORTIUM_CHAT_ACCUMELATED_RECEIPT_REPORT_TEXT => NKCStringTable.GetString("SI_PF_CONSORTIUM_CHAT_ACCUMELATED_RECEIPT_REPORT_TEXT");

	public static string GET_STRING_CONSORTIUM_CHAT_REPORT_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_CHAT_REPORT_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_CHAT_REPORT_POPUP_BODY_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_CHAT_REPORT_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_CHAT_REPORT_CONFIRM_POPUP_TITLE_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_CHAT_REPORT_CONFIRM_POPUP_TITLE_DESC");

	public static string GET_STRING_CONSORTIUM_CHAT_REPORT_CONFIRM_POPUP_BODY_DESC => NKCStringTable.GetString("SI_PF_CONSORTIUM_CHAT_REPORT_CONFIRM_POPUP_BODY_DESC");

	public static string GET_STRING_CONSORTIUM_CHAT_INFORMATION_SANCTION_DESC => NKCStringTable.GetString("SI_DP_CONSORTIUM_CHAT_INFORMATION_SANCTION_DESC");

	public static string GET_STRING_CHAT_FRIEND_COUNT_TEXT => NKCStringTable.GetString("SI_PF_CHAT_FRIEND_COUNT_TEXT");

	public static string GET_STRING_CHAT_CONSORTIUM_MEMBER_COUNT_TEXT => NKCStringTable.GetString("SI_PF_CHAT_CONSORTIUM_MEMBER_COUNT_TEXT");

	public static string GET_STRING_CHAT_CONSORTIUM_JOIN_REQ_DESC => NKCStringTable.GetString("SI_DP_CHAT_CONSORTIUM_JOIN_REQ_DESC");

	public static string GET_STRING_CHAT_BLOCKED => NKCStringTable.GetString("SI_PF_CHAT_BLOCKED");

	public static string GET_STRING_OPTION_GAME_CHAT_NOTICE => NKCStringTable.GetString("SI_PF_OPTION_GAME_CHAT_NOTICE");

	public static string GET_STRING_CHAT_COPY_COMPLETE => NKCStringTable.GetString("SI_DP_CHAT_COPY_TEXT");

	public static string GET_SHADOW_PALACE => NKCStringTable.GetString("SI_DP_SHADOW_PALACE_TITLE");

	public static string GET_SHADOW_PALACE_NUMBER => NKCStringTable.GetString("SI_DP_SHADOW_PALACE_NAME");

	public static string GET_SHADOW_PALACE_START_CONFIRM => NKCStringTable.GetString("SI_DP_SHADOW_PALACE_ENTER_POPUP_DESC");

	public static string GET_SHADOW_BATTLE_ENEMY_LEVEL => NKCStringTable.GetString("SI_DP_SHADOW_PALACE_ENERMY_LEVEL");

	public static string GET_SHADOW_BATTLE_CLEAR_NUM => NKCStringTable.GetString("SI_DP_SHADOW_PALACE_ENERMY_CLEAR_TIME");

	public static string GET_SHADOW_RECORD_POPUP_TITLE => NKCStringTable.GetString("SI_DP_SHADOW_PALACE_CLEAR_REPORT");

	public static string GET_SHADOW_RECORD_POPUP_SLOT_NORMAL => NKCStringTable.GetString("SI_DP_SHADOW_PALACE_CLEAR_ROOM_NUM");

	public static string GET_SHADOW_RECORD_POPUP_SLOT_BOSS => NKCStringTable.GetString("SI_DP_SHADOW_PALACE_CLEAR_ROOM_BOSS");

	public static string GET_SHADOW_PALACE_REMAIN_TICKET => NKCStringTable.GetString("SI_DP_SHADOW_BUTTON_TICKET_VIEW");

	public static string GET_SHADOW_PALACE_RESULT_GAME_TIP => NKCStringTable.GetString("SI_DP_WARFARE_RESULT_SHADOW_TIP1") + NKCStringTable.GetString("SI_DP_WARFARE_RESULT_SHADOW_TIP2");

	public static string GET_STRING_SHADOW_SKIP_ERROR => NKCStringTable.GetString("SI_DP_SHADOW_SKIP_ERROR");

	public static string GET_MULTIPLY_REWARD_ONE_PARAM => NKCStringTable.GetString("SI_DP_MULTIPLY_OPERATION_VALUE_1");

	public static string GET_MULTIPLY_REWARD_RESULT_ONE_PARAM => NKCStringTable.GetString("SI_DP_MULTIPLY_OPERATION_VALUE_2");

	public static string GET_MULTIPLY_OPERATION_MEDAL_COND => NKCStringTable.GetString("SI_DP_MULTIPLY_OPERATION_MEDAL_COND");

	public static string GET_MULTIPLY_REWARD_COUNT_PARAM_02 => NKCStringTable.GetString("SI_DP_MULTIPLY_OPERATION_REWARD_COUNT");

	public static string GET_STRING_POPUP_HAMBER_MENU_MISSION_TITLE_DESC_01 => NKCStringTable.GetString("SI_DP_POPUP_HAMBER_MENU_MISSION_TITLE_DESC_01");

	public static string GET_FRIEND_MENTORING_MENTEE_LIMIT_LEVEL => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_MENTOR_UNLOCK_LEVEL");

	public static string GET_FRIEND_MENTORING_MENTEE_NOT_CLEAR_MENTEE_MESSION => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_MENTOR_UNLOCK_MISSION");

	public static string GET_FRIEND_MENTORING_MENTEE_PROCESSING_MISSION => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_MENTEE_PROCESSING_MISSION");

	public static string GET_FRIEND_MENTORING_INVITE_REWARD_TITLE => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_MENTOR_INVITE_REWARD_TITLE");

	public static string GET_FRIEND_MENTORING_MENTEE_COUNT_DESC_02 => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_MENTEE_COUNT");

	public static string GET_FRIEND_MENTORING_MENTEE_MISSION_NOT_COMPLETE => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_MENTEE_MISSION_REQ");

	public static string GET_FRIEND_MENTORING_CAN_NOT_DELETE_MENTEE_DESC_01 => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_CAN_NOT_DELETE_MENTEE");

	public static string GET_FRIEND_MENTORING_MENTEE_DELETE_TITLE => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_POPUP_DELETE_MENTEE_TITLE");

	public static string GET_FRIEND_MENTORING_MENTEE_DELETE_DESC_02 => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_POPUP_DELETE_MENTEE");

	public static string GET_FRIEND_MENTORING_REGISTER_MENTOR_ACCEPT_TITLE => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_REGISTER_MENTOR_ACCEPT_TITLE");

	public static string GET_FRIEND_MENTORING_REGISTER_MENTOR_DISACCEPT_TITLE => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_REGISTER_MENTOR_DISACCEPT_TITLE");

	public static string GET_FRIEND_MENTORING_REGISTER_MENTOR_ACCEPT_DESC_01 => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_REGISTER_MENTOR_ACCEPT_DESC_01");

	public static string GET_FRIEND_MENTORING_REGISTER_MENTOR_DISACCEPT_DESC_01 => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_REGISTER_MENTOR_DISACCEPT_DESC_01");

	public static string GET_FRIEND_MENTORING_LIMIT_COUNT_DESC => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_REGISTER_MENTEE_LIMIT_COUNT_DESC_01");

	public static string GET_FRIEND_MENTORING_SEASON_END_CHECK => NKCStringTable.GetString("SI_DP_FRIEND_MENTORING_SEASON_CALCULATE");

	public static string GET_FIERCE_ACTIVATE_SEASON_END => NKCStringTable.GetString("SI_PF_FIERCE_ACTIVATE_SEASON_END");

	public static string GET_FIERCE_ENTER_WAIT_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_ENTER_WAIT_DESC_01");

	public static string GET_FIERCE_CAN_NOT_ENTER_FIERCE_BATTLE_SUPPORT => NKCStringTable.GetString("SI_PF_FIERCE_CAN_NOT_ENTER_FIERCE_BATTLE_SUPPORT");

	public static string GET_FIERCE_CAN_NOT_ACCESS => NKCStringTable.GetString("SI_PF_FIERCE_CAN_NOT_ACCESS");

	public static string GET_FIERCE_WAIT_ACTIVATE_DAY_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_WAIT_ACTIVATE_DAY_DESC_01");

	public static string GET_FIERCE_WAIT_ACTIVATE_HOUR_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_WAIT_ACTIVATE_HOUR_DESC_01");

	public static string GET_FIERCE_WAIT_ACTIVATE_MINUTE_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_WAIT_ACTIVATE_MINUTE_DESC_01");

	public static string GET_FIERCE_WAIT_ACTIVATE_SECOND_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_WAIT_ACTIVATE_SECOND_DESC_01");

	public static string GET_FIERCE_WAIT_REWARD_DAY_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_WAIT_REWARD_DAY_DESC_01");

	public static string GET_FIERCE_WAIT_REWARD_HOUR_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_WAIT_REWARD_HOUR_DESC_01");

	public static string GET_FIERCE_WAIT_REWARD_MINUTE_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_WAIT_REWARD_MINUTE_DESC_01");

	public static string GET_FIERCE_WAIT_REWARD_SECOND_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_WAIT_REWARD_SECOND_DESC_01");

	public static string GET_FIERCE_WAIT_END_DAY_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_WAIT_END_DAY_DESC_01");

	public static string GET_FIERCE_WAIT_END_HOUR_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_WAIT_END_HOUR_DESC_01");

	public static string GET_FIERCE_WAIT_END_MINUTE_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_WAIT_END_MINUTE_DESC_01");

	public static string GET_FIERCE_WAIT_END_SECOND_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_WAIT_END_SECOND_DESC_01");

	public static string GET_FIERCE_TIME_END_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_TIME_END_DESC_01");

	public static string GET_FIERCE_RANK_DESC_01 => NKCStringTable.GetString("SI_PF_S2_FIERCE_RANK_DESC_01");

	public static string GET_FIERCE_RANK_IN_TOP_100_DESC_01 => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_RANK_IN_DESC");

	public static string GET_FIERCE_ENTER_LIMIT => NKCStringTable.GetString("SI_PF_S2_FIERCE_ENTER_LIMIT");

	public static string GET_FIERCE_BATTLE_ENTER_LIMIT => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_ENTER_LIMIT");

	public static string GET_FIERCE_BATTLE_RESET_FAIL => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_RESET_FAIL");

	public static string GET_FIERCE_BATTLE_POINT_REWARD_TITLE => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_POINT_REWARD_TITLE");

	public static string GET_FIERCE_BATTLE_GIVE_UP_DESC => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_GIVE_UP_POPUP");

	public static string GET_FIERCE_BATTLE_NOT_ACCESS => NKCStringTable.GetString("SI_CONTENTS_UNLOCK_FIERCE");

	public static string GET_FIERCE_BATTLE_ENTER_SEASON_END => NKCStringTable.GetString("SI_PF_FIERCE_WAITING");

	public static string GET_FIERCE_BATTLE_END_TEXT_NEW_RECORD => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_END_TEXT_NEW_RECORD");

	public static string GET_FIERCE_BATTLE_END_TEXT => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_END_TEXT");

	public static string GET_STRING_FIERCE => NKCStringTable.GetString("SI_PF_FIERCE");

	public static string GET_STRING_FIERCE_POPUP_SELF_PENALTY_BLOCK_TEXT => NKCStringTable.GetString("SI_DP_S2_FIERCE_NO_PENALTY_TEXT");

	public static string GET_STRING_FIERCE_POPUP_SELF_PENALTY_NONE => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_PENALTY_SELECT_INFO_TEXT");

	public static string GET_STRING_FIERCE_POPUP_SELF_PENALTY_BUFF => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_PENALTY_SELECT_BUFF");

	public static string GET_STRING_FIERCE_POPUP_SELF_PENALTY_DEBUFF => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_PENALTY_SELECT_DEBUFF");

	public static string GET_STRING_FIERCE_PENALTY_POINT => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_PENALTY_INFO_TEXT");

	public static string GET_STRING_FIERCE_PENALTY_SCORE_PLUS => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_PENALTY_SCORE_PLUS");

	public static string GET_STRING_FIERCE_PENALTY_SCORE_MINUS => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_PENALTY_SCORE_MINUS");

	public static string GET_STRING_FIERCE_PENALTY_SCORE_PLUS_DESC => NKCStringTable.GetString("SI_PF_S2_FIERCE_BATTLE_PENALTY_INFO_PLUS_TEXT");

	public static string GET_STRING_FIERCE_PENALTY_SCORE_MINUS_DESC => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_PENALTY_INFO_MINUS_TEXT");

	public static string GET_STRING_FIERCE_PENALTY_TITLE_DEBUFF => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_PENALTY_SUBTITLE_2");

	public static string GET_STRING_FIERCE_PENALTY_BUFF_POINT => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_PENALTY_SELECT_BUFF_POINT");

	public static string GET_STRING_FIERCE_PENALTY_DEBUFF_POINT => NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_PENALTY_SELECT_DEBUFF_POINT");

	public static string GET_STRING_FIERCE_RECODE_TITLE => NKCStringTable.GetString("SI_PF_FIERCE_RECODE_TITLE");

	public static string GET_STRING_FIERCE_BOSS_LEVEL_EXPERT => NKCStringTable.GetString("SI_DP_LEVEL_UNKNOWN");

	public static string GET_STRING_OPERATOR_CONTRACT_STYLE => NKCStringTable.GetString("SI_PF_OPERATOR_CONTRACT_STYLE");

	public static string GET_STRING_OPERATOR_SKILL_TRANSFER => NKCStringTable.GetString("SI_PF_OPERATOR_SKILL_TRANSPORT");

	public static string GET_STRING_OPERATOR_INFO_MENU_NAME => NKCStringTable.GetString("SI_PF_OPERATOR_INFO_MENU_NAME");

	public static string GET_STRING_INVEITORY_OPERATOR_TITLE => NKCStringTable.GetString("SI_PF_INVENTORY_OPERATOR_TITLE");

	public static string GET_STRING_INVENTORY_OPERATOR_ADD_DESC => NKCStringTable.GetString("SI_PF_INVENTORY_OPERATOR_ADD_DESC");

	public static string GET_STRING_INVENTORY_OPERATOR_ADD_FULL => NKCStringTable.GetString("SI_PF_INVENTORY_OPERATOR_ADD_FULL");

	public static string GET_STRING_OPERATOR_PASSIVE_SKILL_TRANSFER_BLOCK => NKCStringTable.GetString("SI_PF_OPERATOR_PASSIVE_SKILL_TRANSFER_BLOCK");

	public static string GET_STRING_OPERATOR_SKILL_RESULT_TITLE => NKCStringTable.GetString("SI_PF_OPERATOR_SKILL_RESULT_TITLE");

	public static string GET_STRING_OPERATOR_SKILL_POPUP => NKCStringTable.GetString("SI_PF_OPERATOR_SKILL_POPUP");

	public static string GET_STRING_OPERATOR_MAIN_SKILL_SUCCESS => NKCStringTable.GetString("SI_PF_OPERATOR_MAIN_SKILL_SUCCESS");

	public static string GET_STRING_OPERATOR_MAIN_SKILL_FAIL => NKCStringTable.GetString("SI_PF_OPERATOR_MAIN_SKILL_FAIL");

	public static string GET_STRING_OPERATOR_PASSIVE_SKILL_SUCCESS => NKCStringTable.GetString("SI_PF_OPERATOR_PASSIVE_SKILL_SUCCESS");

	public static string GET_STRING_OPERATOR_PASSIVE_SKILL_FAIL => NKCStringTable.GetString("SI_PF_OPERATOR_PASSIVE_SKILL_FAIL");

	public static string GET_STRING_OPERATOR_PASSIVE_SKILL_IMPLANT_SUCCESS => NKCStringTable.GetString("SI_PF_OPERATOR_PASSIVE_SKILL_IMPLANT_SUCCESS");

	public static string GET_STRING_OPERATOR_PASSIVE_SKILL_IMPLANT_FAIL => NKCStringTable.GetString("SI_PF_OPERATOR_PASSIVE_SKILL_IMPLANT_FAIL");

	public static string GET_STRING_OPERATOR_SKILL_INFO_POPUP_TITLE => NKCStringTable.GetString("SI_PF_OPERATOR_SKILL_INFO_POPUP_TITLE");

	public static string GET_STRING_OPERATOR_TOOLTIP_ACTIVE_SKILL_TITLE => NKCStringTable.GetString("SI_PF_OPERATOR_TOOLTIP_ACTIVE_SKILL_TITLE");

	public static string GET_STRING_OPERATOR_TOOLTIP_PASSIVE_SKILL_TITLE => NKCStringTable.GetString("SI_PF_OPERATOR_TOOLTIP_PASSIVE_SKILL_TITLE");

	public static string GET_STRING_NO_EXIST_SELECTED_OPERATOR => NKCStringTable.GetString("SI_PF_NO_EXIST_SELECTED_OPERATOR");

	public static string GET_STRING_REMOVE_OPERATOR_WARNING => NKCStringTable.GetString("SI_PF_REMOVE_OPERATOR_WARNING");

	public static string GET_STRING_EXTRACT_OPERATOR_WARNING => NKCStringTable.GetString("SI_PF_EXTRACT_OPERATOR_WARNING");

	public static string GET_STRING_NO_EXIST_OPERATOR => NKCStringTable.GetString("SI_PF_NO_EXIST_OPERATOR");

	public static string GET_STRING_OPERATOR_REMOVE_CONFIRM_ONE_PARAM => NKCStringTable.GetString("SI_PF_OPERATOR_REMOVE_CONFIRM_ONE_PARAM");

	public static string GET_STRING_OPERATOR_IMPLANT_BLOCK_LOCK_UNIT => NKCStringTable.GetString("SI_PF_OPERATOR_IMPLANT_BLOCK_LOCK_UNIT");

	public static string GET_STRING_OPERATOR_IMPLANT_BLOCK_NOT_POSSIBLE => NKCStringTable.GetString("SI_PF_OPERATOR_IMPLANT_BLOCK_NOT_POSSIBLE");

	public static string GET_STRING_OPERATOR_IMPLANT_TRY_NOTHING => NKCStringTable.GetString("SI_PF_OPERATOR_IMPLANT_TRY_NOTHING");

	public static string GET_STRING_OPERATOR_NOT_ENOUGH_ITEM => NKCStringTable.GetString("SI_PF_OPERATOR_NOT_ENOUGH_ITEM");

	public static string GET_STRING_REMOVE_UNIT_NO_EXIST_OPERATOR => NKCStringTable.GetString("SI_PF_REMOVE_UNIT_NO_EXIST_OPERATOR");

	public static string GET_STRING_OPERATOR_POPUP_CHANGE_STAT_PLUS_DESC_01 => NKCStringTable.GetString("SI_PF_OPERATOR_POPUP_CHANGE_STAT_PLUS_DESC_01");

	public static string GET_STRING_OPERATOR_POPUP_CHANGE_STAT_MINUS_DESC_01 => NKCStringTable.GetString("SI_PF_OPERATOR_POPUP_CHANGE_STAT_MINUS_DESC_01");

	public static string GET_STRING_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_MAIN_SKILL_MAX_LEVEL => NKCStringTable.GetString("SI_PF_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_MAIN_SKILL_MAX_LEVEL");

	public static string GET_STRING_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_MAIN_SKILL_NOT_MATCH => NKCStringTable.GetString("SI_PF_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_MAIN_SKILL_NOT_MATCH");

	public static string GET_STRING_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_SUB_SKILL_MAX_LEVEL => NKCStringTable.GetString("SI_PF_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_SUB_SKILL_MAX_LEVEL");

	public static string GET_STRING_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_SUB_SKILL_NOT_MATCH => NKCStringTable.GetString("SI_PF_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_SUB_SKILL_NOT_MATCH");

	public static string GET_STRING_OPERATOR_POPUP_SKILL_CONFIRM_DESC => NKCStringTable.GetString("SI_PF_OPERATOR_POPUP_SKILL_CONFIRM_DESC");

	public static string GET_STRING_OPERATOR_SKILL_COOL_REDUCE => NKCStringTable.GetString("SI_PF_OPERATOR_INFO_STAT_NAME_SKILL");

	public static string GET_STRING_OPERATOR_CONFIRM_POPUP_TITLE_TRANSFER => NKCStringTable.GetString("SI_PF_OPERATOR_SKILL_CONFIRM_POPUP_TITLE");

	public static string GET_STRING_OPERATOR_CONFIRM_POPUP_TITLE_SELECT => NKCStringTable.GetString("SI_PF_OPERATOR_CONFIRM_POPUP_TITLE_SELECT");

	public static string GET_STRING_OPERATOR_CONFIRM_POPUP_CONFIRM_WARNING => NKCStringTable.GetString("SI_PF_OPERATOR_CONFIRM_POPUP_CONFIRM_WARNING");

	public static string GET_STRING_OPERATOR_CONFIRM_POPUP_CONFIRM_WARNING_UNIT_LEVEL => NKCStringTable.GetString("SI_PF_OPERATOR_CONFIRM_POPUP_CONFIRM_WARNING_UNIT_LEVEL");

	public static string GET_STRING_OPERATOR_CONFIRM_POPUP_CONFIRM_WARNING_UNIT_ACTIVE_SKILL_LEVEL => NKCStringTable.GetString("SI_PF_OPERATOR_CONFIRM_POPUP_CONFIRM_WARNING_ACTIVE_SKILL_LEVEL");

	public static string GET_STRING_OPERATOR_CONFIRM_POPUP_CONFIRM_WARNING_UNIT_PASSIVE_SKILL_LEVEL => NKCStringTable.GetString("SI_PF_OPERATOR_CONFIRM_POPUP_CONFIRM_WARNING_PASSIVE_SKILL_LEVEL");

	public static string GET_STRING_OPERATOR_CONFIRM_POPUP_WARNING => NKCStringTable.GetString("SI_PF_OPERATOR_CONFIRM_POPUP_CONFIRM_WARNING");

	public static string GET_STRING_OPERATOR_CONFIRM_FUSION_TEXT => NKCStringTable.GetString("SI_PF_OPERATOR_CONFIRM_POPUP_FUSION_CONFIRM_TEXT");

	public static string GET_STRING_OPERATOR_SKILL_TRANSFER_NONE_TITLE => NKCStringTable.GetString("SI_PF_OPERATOR_POPUP_SKILL_NONE_TEXT_TITLE");

	public static string GET_STRING_OPERATOR_SKILL_TRANSFER_NONE_DESC => NKCStringTable.GetString("SI_PF_OPERATOR_POPUP_SKILL_NONE_TEXT_DESC");

	public static string GET_STRING_OPERATOR_TOKEN_TRANSFER_NONE_TITLE => NKCStringTable.GetString("SI_PF_OPERATOR_TOKEN_POPUP_SKILL_NONE_TEXT_TITLE");

	public static string GET_STRING_OPERATOR_TOKEN_TRANSFER_NONE_DESC => NKCStringTable.GetString("SI_PF_OPERATOR_TOKEN_POPUP_SKILL_NONE_TEXT_DESC");

	public static string GET_STRING_OPERATOR_TOKEN_TRANSFER_MAIN_SKILL_NOT_TOKEN => NKCStringTable.GetString("SI_PF_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_MAIN_SKILL_NOT_TOKEN");

	public static string GET_STRING_OPERATOR_TOKEN_PASSIVE_SKILL_TRANSFER_BLOCK => NKCStringTable.GetString("SI_PF_OPERATOR_TOKEN_PASSIVE_SKILL_TRANSFER_BLOCK");

	public static string GET_STRING_EVENTPASS_EVENT_PASS_MENU_TITLE => NKCStringTable.GetString("SI_LOBBY_SUB_BTNS_EVENT_PASS");

	public static string GET_STRING_EVENTPASS_END_TIME_REMAIN => NKCStringTable.GetString("SI_DP_EVENT_PASS_END_TIME_REMAIN");

	public static string GET_STRING_EVENTPASS_END_TIME_ALMOST_END => NKCStringTable.GetString("SI_DP_EVENT_PASS_END_TIME_ALMOST_END");

	public static string GET_STRING_EVENTPASS_EXP => NKCStringTable.GetString("SI_DP_EVENT_PASS_EXP");

	public static string GET_STRING_EVENTPASS_MAX_PASS_LEVEL_FINAL_REWARD => NKCStringTable.GetString("SI_DP_EVENT_PASS_LEVEL_FINAL_REWARD");

	public static string GET_STRING_EVENTPASS_ELAPSED_WEEK => NKCStringTable.GetString("SI_DP_EVENT_PASS_ELAPSED_WEEK");

	public static string GET_STRING_EVENTPASS_UPDATE_TIME_LEFT => NKCStringTable.GetString("SI_DP_EVENT_PASS_UPDATE_TIME_LEFT");

	public static string GET_STRING_EVENTPASS_DAILY_MISSION_ACHIEVE => NKCStringTable.GetString("SI_DP_EVENT_PASS_DAILY_MISSION_ACHIEVE");

	public static string GET_STRING_EVENTPASS_WEEKLY_MISSION_ACHIEVE => NKCStringTable.GetString("SI_DP_EVENT_PASS_WEEKLY_MISSION_ACHIEVE");

	public static string GET_STRING_EVENTPASS_PASS_LEVEL_UP_NOTICE => NKCStringTable.GetString("SI_DP_EVENT_PASS_LEVEL_UP_NOTICE");

	public static string GET_STRING_EVENTPASS_PASS_LEVEL_UP_DESC => NKCStringTable.GetString("SI_DP_EVENT_PASS_LEVEL_UP_DESC");

	public static string GET_STRING_EVENTPASS_CORE_PASS_PLUS_PURCHASE_EXP_LOSS => NKCStringTable.GetString("SI_DP_EVENT_PASS_CORE_PASS_PLUS_PURCHASE_EXP_LOSS");

	public static string GET_STRING_PURCHASE_REFUND_IMPOSSIBLE => NKCStringTable.GetString("SI_DP_PURCHASE_REFUND_IMPOSSIBLE");

	public static string GET_STRING_EVENTPASS_MISSION_REFRESH => NKCStringTable.GetString("SI_DP_EVENT_PASS_MISSION_REFRESH");

	public static string GET_STRING_EVENTPASS_MISSION_REFRESH_DESC => NKCStringTable.GetString("SI_DP_EVENT_PASS_MISSION_REFRESH_DESC");

	public static string GET_STRING_EVENTPASS_MISSION_REFRESH_WARNING_DESC => NKCStringTable.GetString("SI_DP_EVENT_PASS_MISSION_REFRESH_WARNING_DESC");

	public static string GET_STRING_EVENTPASS_MISSION_REFRESH_FREECOUNT => NKCStringTable.GetString("SI_DP_EVENT_PASS_MISSION_REFRESH_FREECOUNT");

	public static string GET_STRING_EVENTPASS_MISSION_REFRESH_COUNT => NKCStringTable.GetString("SI_DP_EVENT_PASS_MISSION_REFRESH_COUNT");

	public static string GET_STRING_EVENTPASS_MISSION_REFRESH_MAX_DESC => NKCStringTable.GetString("SI_DP_EVENT_PASS_MISSION_REFRESH_MAX_DESC");

	public static string GET_STRING_EVENTPASS_MISSION_COMPLETE_DAILY_ALL => NKCStringTable.GetString("SI_DP_EVENT_PASS_MISSION_COMPLETE_DAILY_ALL");

	public static string GET_STRING_EVENTPASS_MISSION_COMPLETE_WEEKLY_ALL => NKCStringTable.GetString("SI_DP_EVENT_PASS_MISSION_COMPLETE_WEEKLY_ALL");

	public static string GET_STRING_EVENTPASS_REWARD_POSSIBLE => NKCStringTable.GetString("SI_DP_EVENT_PASS_REWARD_POSSIBLE");

	public static string GET_STRING_EVENTPASS_END => NKCStringTable.GetString("SI_MENU_EXCEPTION_EVENT_EXPIRED_POPUP");

	public static string GET_STRING_EVENTPASS_MISSION_COMPLETE_DAILY_ALL_EX => NKCStringTable.GetString("SI_DP_EVENTPASS_MISSION_COMPLETE_DAILY_ALL_EX");

	public static string GET_STRING_EVENTPASS_MISSION_COMPLETE_WEEKLY_ALL_EX => NKCStringTable.GetString("SI_DP_EVENTPASS_MISSION_COMPLETE_WEEKLY_ALL_EX");

	public static string GET_STRING_EVENTPASS_COREPASS_DISCOUNT_RATE => NKCStringTable.GetString("SI_DP_EVENTPASS_DISCOUNT_PRICE");

	public static string GET_STRING_EVENTPASS_MISSION_UPDATING_DAILY => NKCStringTable.GetString("SI_DP_EVENT_PASS_MISSION_UPDATING_DAILY");

	public static string GET_STRING_EVENTPASS_MISSION_UPDATING_WEEKLY => NKCStringTable.GetString("SI_DP_EVENT_PASS_MISSION_UPDATING_WEEKLY");

	public static string GET_STRING_EVENTPASS_LOBBY_MENU_LEFT_TIME_DAYS_01 => NKCStringTable.GetString("SI_PF_EVENTPASS_LOBBY_MENU_LEFT_TIME_DAYS_01");

	public static string GET_STRING_EVENTPASS_LOBBY_MENU_LEFT_TIME_HOUR_01 => NKCStringTable.GetString("SI_PF_EVENTPASS_LOBBY_MENU_LEFT_TIME_HOUR_01");

	public static string GET_STRING_EVENTPASS_LOBBY_MENU_LEFT_TIME_MIN_01 => NKCStringTable.GetString("SI_PF_EVENTPASS_LOBBY_MENU_LEFT_TIME_MIN_01");

	public static string GET_STRING_EVENTPASS_LOBBY_MENU_LEFT_TIME_SEC_01 => NKCStringTable.GetString("SI_PF_EVENTPASS_LOBBY_MENU_LEFT_TIME_SEC_01");

	public static string GET_STRING_EQUIP_PRESET_NAME => NKCStringTable.GetString("SI_DP_EQUIP_PRESET_NAME");

	public static string GET_STRING_EQUIP_PRESET_ADD_TITLE => NKCStringTable.GetString("SI_DP_EQUIP_PRESET_ADD");

	public static string GET_STRING_EQUIP_PRESET_ADD_CONTENT => NKCStringTable.GetString("SI_DP_EQUIP_PRESET_ADD_DESC");

	public static string GET_STRING_EQUIP_PRESET_SAVE_CONTENT => NKCStringTable.GetString("SI_DP_EQUIP_PRESET_SAVE_DESC");

	public static string GET_STRING_EQUIP_PRESET_NONE => NKCStringTable.GetString("SI_DP_EQUIP_PRESET_NONE");

	public static string GET_STRING_EQUIP_PRESET_DIFFERENT_TYPE => NKCStringTable.GetString("SI_DP_EQUIP_PRESET_DIFFERENT_TYPE");

	public static string GET_STRING_EQUIP_PRESET_APPLY_SLOT_LOCKED => NKCStringTable.GetString("SI_DP_EQUIP_PRESET_APPLY_SLOT_LOCKED");

	public static string GET_STRING_EQUIP_PRESET_DIFFERENT_POSITION => NKCStringTable.GetString("SI_DP_EQUIP_PRESET_DIFFERENT_POSITION");

	public static string GET_STRING_EQUIP_PRESET_PRIVATE_EQUIP => NKCStringTable.GetString("SI_DP_EQUIP_PRESET_PRIVATE_EQUIP");

	public static string GET_STRING_EQUIP_PRESET_SLOT_FULL => NKCStringTable.GetString("SI_DP_EQUIP_PRESET_SLOT_FULL");

	public static string GET_STRING_EQUIP_PRESET_UNIT_EQUIP_EMPTY => NKCStringTable.GetString("SI_DP_EQUIP_PRESET_UNIT_EQUIP_EMPTY");

	public static string GET_STRING_EQUIP_PRESET_ORDER_SAVE_WARNING => NKCStringTable.GetString("SI_DP_EQUIP_PRESET_LIST_SAVE_DESC");

	public static string GET_STRING_EQUIP_PRESET_CLEAR_DESC => NKCStringTable.GetString("SI_DP_EQUIP_PRESET_CLEAR_DESC");

	public static string GET_STRING_OFFICE_ALREADY_ASSIGNED_UNIT => NKCStringTable.GetString("SI_DP_OFFICE_ALREADY_ASSIGNED_UNIT");

	public static string GET_STRING_OFFICE_FULL_ASSIGNED => NKCStringTable.GetString("SI_DP_OFFICE_FULL_ASSIGNED");

	public static string GET_STRING_OFFICE_ASSIGN_COMPLETE => NKCStringTable.GetString("SI_DP_OFFICE_ASSIGN_COMPLETE");

	public static string GET_STRING_OFFICE_ASSIGN_CANCEL => NKCStringTable.GetString("SI_DP_OFFICE_ASSIGN_CANCEL");

	public static string GET_STRING_OFFICE_PURCHASE_SECTION => NKCStringTable.GetString("SI_DP_OFFICE_PURCHASE_SECTION");

	public static string GET_STRING_OFFICE_PURCHASE_ROOM => NKCStringTable.GetString("SI_DP_OFFICE_PURCHASE_ROOM");

	public static string GET_STRING_OFFICE_ROOM_IN_LOCKED_SECTION => NKCStringTable.GetString("SI_DP_OFFICE_ROOM_IN_LOCKED_SECTION");

	public static string GET_STRING_OFFICE_OPENED_DORMS_COUNT => NKCStringTable.GetString("SI_DP_OFFICE_OPENED_DORMS_COUNT");

	public static string GET_STRING_OFFICE_MINIMAP => NKCStringTable.GetString("SI_DP_OFFICE_MINIMAP");

	public static string GET_STRING_OFFICE_DORMITORY => NKCStringTable.GetString("SI_DP_OFFICE_DOMITORY");

	public static string GET_STRING_OFFICE_ROOM_IN => NKCStringTable.GetString("SI_DP_OFFICE_ROOM_IN");

	public static string GET_STRING_OFFICE_DECORATE => NKCStringTable.GetString("SI_DP_OFFICE_DECORATE");

	public static string GET_STRING_OFFICE_FRIEND_CANNOT_VISIT => NKCStringTable.GetString("SI_DP_OFFICE_FRIEND_CANNOT_VISIT");

	public static string GET_STRING_OFFICE_FRIEND_NICKNAME => NKCStringTable.GetString("SI_DP_OFFICE_FRIEND_NICKNAME");

	public static string GET_STRING_OFFICE_REQUEST_FRIEND => NKCStringTable.GetString("SI_DP_OFFICE_REQUEST_FRIEND");

	public static string GET_STRING_OFFICE_BIZCARD_SENDED_ALL => NKCStringTable.GetString("SI_DP_OFFICE_BIZCARD_SENDED_ALL");

	public static string GET_STRING_TOOLTIP => "툴팁";

	public static string GET_STRING_POPUP_ITEM_LACK => "ItemLack";

	public static string GET_STRING_POPUP_ITEM_BOX => "ItemBox";

	public static string GET_STRING_POPUP_ITEM_EQUIP_BOX => "ItemEquipBox";

	public static string GET_STRING_UNIT_INFO_DETAIL => "UnitInfoDetail";

	public static string GET_STRING_SKIN => NKCStringTable.GetString("SI_PF_GET_STRING_SKIN");

	public static string GET_STRING_SKIN_GRADE_VARIATION => NKCStringTable.GetString("SI_PF_GET_STRING_SKIN_GRADE_VARIATION");

	public static string GET_STRING_POPUP_SKILL_FULL_INFO => "SkillInfo";

	public static string GET_STRING_MENU_NAME_OPERATION_INTRO => "작전인트로";

	public static string GET_STRING_WARFARE_RESULT => "전역 결과";

	public static string GET_STRING_WARFARE_SELECT_SHIP_POPUP => "WarfareSelectShipPopup";

	public static string GET_STRING_LOGIN => "로그인";

	public static string GET_STRING_FRIEND_INFO => "FriendInfo";

	public static string GET_STRING_MENU_NAME_WORLDMAP_EVENT => "탐색현황";

	public static string GET_STRING_NEWS => "뉴스";

	public static string GET_STRING_CONTRACT_BUNDLE => "일괄 채용";

	public static string GET_STRING_SKILL_TRAIN_POPUP => "스킬 훈련 메뉴";

	public static string GET_STRING_SLOT_VIEWR => NKCStringTable.GetString("SI_PF_GET_STRING_SLOT_VIEWR");

	public static string GET_STRING_RESULT => "결과창";

	public static string GET_STRING_PRIVATE_PVP => NKCStringTable.GetString("SI_DP_FRIEND_PVP");

	public static string GET_STRING_PRIVATE_PVP_INVITE_REQ => NKCStringTable.GetString("SI_DP_POPUP_NOTICE_FRIENDLY_MATCH_WAIT_ACCEPT");

	public static string GET_STRING_PRIVATE_PVP_INVITE_NOT => NKCStringTable.GetString("SI_DP_POPUP_NOTICE_FRIENDLY_MATCH_INVITE_ARRIVE");

	public static string GET_STRING_PRIVATE_PVP_AUTO_CANCEL_ID => "SI_DP_POPUP_NOTICE_FRIENDLY_MATCH_INVITE_REJECT";

	public static string GET_STRING_PRIVATE_PVP_READY_CANCEL_TITLE => NKCStringTable.GetString("SI_PF_GAUNTLET_FRIENDLY_BATTLE_CANCEL");

	public static string GET_STRING_PRIVATE_PVP_READY_CANCEL => NKCStringTable.GetString("SI_PF_GAUNTLET_FRIENDLY_BATTLE_CANCEL_CHECK");

	public static string GET_STRING_PRIVATE_PVP_GUILD_MEMBER_EMPTY => NKCStringTable.GetString("SI_DP_CONSORTIUM_LIST_IS_EMPTY");

	public static string GET_STRING_PRIVATE_PVP_USER_KICK => NKCStringTable.GetString("SI_PF_OBSERVE_SELECT_USER_SENTOFF");

	public static string GET_STRING_PRIVATE_PVP_OPTION_SETUP_VIEW => NKCStringTable.GetString("SI_PF_OBSERVE_POPUP_OPTION_INFO");

	public static string GET_STRING_PRIVATE_PVP_OPTION_SETUP_ENABLE => NKCStringTable.GetString("SI_PF_OBSERVE_POPUP_OPTION_CHANGE_TEXT");

	public static string GET_STRING_PRIVATE_PVP_OPTION_SETUP_CONFIRM => NKCStringTable.GetString("SI_PF_OBSERVE_POPUP_OPTION_CHANGE_CONFIRM");

	public static string GET_STRING_PRIVATE_PVP_OBSERVE_CODE_NOT_EXIST => NKCStringTable.GetString("SI_PF_OBSERVE_ENTER_FAIL_1");

	public static string GET_STRING_PRIVATE_PVP_OBSERVE_CODE_CANNOT_ENTER => NKCStringTable.GetString("SI_PF_OBSERVE_ENTER_FAIL_2");

	public static string GET_STRING_PRIVATE_PVP_KICKED_BY_HOST => NKCStringTable.GetString("SI_PF_OBSERVE_HOST_SENTOFF");

	public static string GET_STRING_PRIVATE_PVP_OBSERVE_COUNT_MAX => NKCStringTable.GetString("SI_PF_OBSERVE_INVITE_FAIL_USER_COUNT_MAX");

	public static string GET_STRING_PRIVATE_PVP_OBSERVE_EXIT => NKCStringTable.GetString("SI_PF_OBSERVE_EXIT_POPUP_TEXT");

	public static string GET_STRING_PRIVATE_PVP_OBSERVE_LEAVE_ROOM => NKCStringTable.GetString("SI_PF_OBSERVE_ROOM_EXIT_NOT_HOST_1");

	public static string GET_STRING_PRIVATE_PVP_HOST_LEFT_ROOM => NKCStringTable.GetString("SI_PF_OBSERVE_ROOM_EXIT_NOT_HOST_2");

	public static string GET_STRING_PRIVATE_PVP_HOST_TERMINATE_ROOM => NKCStringTable.GetString("SI_PF_OBSERVE_ROOM_EXIT_HOST");

	public static string GET_STRING_PRIVATE_PVP_UNUSABLE_FUNCTION => NKCStringTable.GetString("SI_PF_FRIENDLY_MATCH_NO_FUNCTION");

	public static string GET_STRING_POPUP_NOTICE_FRIENDLY_MATCH_CHANGE_OPTION => NKCStringTable.GetString("SI_DP_POPUP_NOTICE_FRIENDLY_MATCH_CHANGE_OPTION");

	public static string GET_STRING_FRIENDLY_MATCH_READY_DECK => NKCStringTable.GetString("SI_PF_FRIENDLY_MATCH_READY_DECK");

	public static string GET_STRING_POPUP_NOTICE_FRIENDLY_MATCH_AFTER_LOADING => NKCStringTable.GetString("SI_DP_POPUP_NOTICE_FRIENDLY_MATCH_AFTER_LOADING");

	public static string GET_STRING_READYING => NKCStringTable.GetString("SI_PF_READYING");

	public static string GET_STRING_GAUNTLET_OPEN_LEAGUE_MODE => NKCStringTable.GetString("SI_DP_GAUNTLET_OPEN_LEAGUE_MODE");

	public static string GET_STRING_GAUNTLET_NOT_OPEN_LEAGUE_MODE => NKCStringTable.GetString("SI_DP_GAUNTLET_NOT_OPEN_LEAGUE_MODE");

	public static string GET_STRING_GAUNTLET_LEAGUE_GAME => NKCStringTable.GetString("SI_PF_LEAGUEMATCH");

	public static string GET_STRING_GAUNTLET_LEAGUE_TITLE => NKCStringTable.GetString("SI_PF_LEAGUE_TITLE");

	public static string GET_STRING_GAUNTLET_UNLIMITED_TITLE => NKCStringTable.GetString("SI_PF_UNLIMITED_TITLE");

	public static string GET_STRING_GAUNTLET_LEAGUE_START_REQ_POPUP_DESC => NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_START_REQ_POPUP_DESC");

	public static string GET_STRING_GAUNTLET_LEAGUE_START_REQ_SHIP_POPUP_DESC => NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_START_REQ_SHIP_POPUP_DESC");

	public static string GET_STRING_GAUNTLET_LEAGUE_START_REQ_UNIT_POPUP_DESC => NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_START_REQ_UNIT_POPUP_DESC");

	public static string GET_STRING_TOOLTIP_DEADLINE_BUFF_TITLE => NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_TOOLTIP_DEADLINE_BUFF_TITLE");

	public static string GET_STRING_POPUP_LEAGUE_NO_MAG => NKCStringTable.GetString("SI_PF_POPUP_LEAGUE_NO_MAG");

	public static string GET_STRING_RECALL_FINAL_CHECK_POPUP_DESC => NKCStringTable.GetString("SI_PF_RECALL_FINAL_CHECK_POPUP_DESC");

	public static string GET_STRING_RECALL_FINAL_CHECK_POPUP_DESC2 => NKCStringTable.GetString("SI_PF_RECALL_POPUP_SUBTITLE_2");

	public static string GET_STRING_RECALL_FINAL_CHECK_POPUP_DESC3 => NKCStringTable.GetString("SI_PF_RECALL_POPUP_SUBTITLE_3");

	public static string GET_STRING_RECALL_FINAL_CHECK_POPUP_DATE => NKCStringTable.GetString("SI_PF_RECALL_FINAL_CHECK_POPUP_DATE");

	public static string GET_STRING_RECALL_ERROR_ALT_USING_UNIT => NKCStringTable.GetString("SI_PF_RECALL_ERROR_ALT_USING_UNIT");

	public static string GET_STRING_RECALL_ERROR_ALT_UNIT_SELECT => NKCStringTable.GetString("SI_PF_RECALL_ERROR_ALT_UNIT_SELECT");

	public static string GET_STRING_RECALL_COMPLETE => NKCStringTable.GetString("SI_PF_RECALL_COMPLETE");

	public static string GET_STRING_RECALL_DESC_END_DATE => NKCStringTable.GetString("SI_PF_RECALL_DESC_END_DATE");

	public static string GET_STRING_KILLCOUNT_SERVER_REWARD_CURRENT_STEP => NKCStringTable.GetString("SI_DP_KILLCOUNT_SERVER_REWARD_CURRENT_STEP");

	public static string GET_STRING_KILLCOUNT_SERVER_REWARD_STEP => NKCStringTable.GetString("SI_DP_KILLCOUNT_SERVER_REWARD_STEP");

	public static string GET_STRING_KILLCOUNT_SERVER_REWARD_DESC => NKCStringTable.GetString("SI_DP_KILLCOUNT_SERVER_REWARD_DESC");

	public static string GET_STRING_KILLCOUNT_SERVER_REWARD_GET => NKCStringTable.GetString("SI_DP_KILLCOUNT_SERVER_REWARD_GET");

	public static string GET_STRING_REARM_EXTRACT_NOT_TARGET_UNIT => NKCStringTable.GetString("NEC_FAIL_EXTRACT_UNIT_TARGET_LACK");

	public static string GET_STRING_REARM_EXTRACT_TITLE => NKCStringTable.GetString("SI_PF_REARM_SHORTCUT_MENU_RECORD");

	public static string GET_STRING_REARM_PROCESS_TITLE => NKCStringTable.GetString("SI_PF_REARM_SHORTCUT_MENU_REARM");

	public static string GET_STRING_REARM_EXTRACT_RESULT_TITLE => NKCStringTable.GetString("SI_PF_REARM_REARM_EXTRACT_RESULT_TITLE");

	public static string GET_STRING_REARM_CONFIRM_POPUP_TITLE => NKCStringTable.GetString("SI_PF_REARM_REARM_CONFIRM_POPUP_TITLE");

	public static string GET_STRING_REARM_CONFIRM_POPUP_FINAL_BOX_DESC => NKCStringTable.GetString("SI_PF_REARM_REARM_CONFIRM_POPUP_FINAL_BOX_DESC");

	public static string GET_STRING_REARM_CONFIRM_POPIP_BOX_TITLE => NKCStringTable.GetString("SI_PF_REARM_REARM_CONFIRM_POPIP_BOX_TITLE");

	public static string GET_STRING_REARM_EXTRACT_UNIT_LIMIT_UNDER_8 => NKCStringTable.GetString("NEC_FAIL_UNIT_REMOVE");

	public static string GET_STRING_REARM_EXTRACT_LACK_TARGET_UNIT_COUNT => NKCStringTable.GetString("NEC_FAIL_EXTRACT_UNIT_LACK");

	public static string GET_STRING_REARM_PROCESS_UNIT_SELECT_LIST_TITLE => NKCStringTable.GetString("SI_PF_REARM_PROCESS_UNIT_SELECT_LIST_TITLE");

	public static string GET_STRING_REARM_EXTRACT_CONFIRM_POPUP_DESC => NKCStringTable.GetString("SI_PF_REARM_RECORD_CONFIRM_RESULT_TEXT_V2");

	public static string GET_STRING_REARM_EXTRACT_CONFIRM_POPUP_SYNERGY_BONUS => NKCStringTable.GetString("SI_PF_REARM_RECORD_CONFIRM_SYNERGY_TEXT");

	public static string GET_STRING_REARM_EXTRACT_NOT_ACTIVE_SYNERGY_BOUNS => NKCStringTable.GetString("SI_PF_REARM_RECORD_CONFIRM_SYNERGY_DISABLE_TEXT");

	public static string GET_STRING_REARM_RESULT_POPUP_SKILL_LEVEL_BEFORE => NKCStringTable.GetString("SI_PF_REARM_RESULT_SKILL_BEFORE_LV_TEXT");

	public static string GET_STRING_REARM_RESULT_POPUP_SKILL_LEVEL_AFTER => NKCStringTable.GetString("SI_PF_REARM_RESULT_SKILL_AFTER_LV_TEXT");

	public static string GET_STRING_REARM_PROCESS_BLOCK_MESSAGE_EMPTY_TARGET_UNIT => NKCStringTable.GetString("NEC_FAIL_REARM_UNIT_NOT");

	public static string GET_STRING_REARM_PROCESS_BLOCK_MESSAGE_LACK_COND => NKCStringTable.GetString("NEC_FAIL_REARM_LACK_ITEM_LV");

	public static string GET_STRING_REARM_PROCESS_BLOCK_MESSAGE_EQUIPED => NKCStringTable.GetString("NEC_FAIL_REARM_UNIT_EQUIP");

	public static string GET_STRING_FACTORY_UPGRADE_TITLE => NKCStringTable.GetString("SI_PF_FACTORY_UPGRADE_TITLE");

	public static string GET_STRING_EQUIP_OPTION_MAIN => NKCStringTable.GetString("SI_PF_EQUIP_OPTION_MAIN");

	public static string GET_STRING_EQUIP_OPTION_1 => NKCStringTable.GetString("SI_PF_FQUIP_OPTION_1");

	public static string GET_STRING_EQUIP_OPTION_2 => NKCStringTable.GetString("SI_PF_EQUIP_OPTION_2");

	public static string GET_STRING_EQUIP_OPTION_SET => NKCStringTable.GetString("SI_PF_EQUIP_OPTION_SET");

	public static string GET_STRING_FACTORY_UPGRADE_MAIN_OPTION_TOOLTIP => NKCStringTable.GetString("SI_PF_FACTORY_UPGRADE_MAIN_OPTION_TOOLTIP");

	public static string GET_STRING_FACTORY_UPGRADE_MAIN_SUB_TOOLTIP => NKCStringTable.GetString("SI_PF_FACTORY_UPGRADE_MAIN_SUB_TOOLTIP");

	public static string GET_STRING_FACTORY_UPGRADE_CONFIRM_POPUP => NKCStringTable.GetString("SI_PF_FACTORY_UPGRADE_CONFIRM_POPUP");

	public static string GET_STRING_FACTORY_UPGRADE_COMPLETE => NKCStringTable.GetString("SI_PF_FACTORY_UPGRADE_COMPLETE");

	public static string GET_STRING_FACTORY_HIDDEN_OPTION_TITLE => NKCStringTable.GetString("SI_PF_FACTORY_HIDDEN_OPTION_TITLE");

	public static string GET_STRING_EQUIP_POTENTIAL_OPEN_REQUIRED => NKCStringTable.GetString("SI_DP_EQUIP_POTENTIAL_OPEN_REQUIRED");

	public static string GET_STRING_EQUIP_POTENTIAL_REQUIRED_ENCHANT_LV => NKCStringTable.GetString("SI_DP_EQUIP_POTENTIAL_REQUIRED_ENCHANT_LV");

	public static string GET_STRING_EQUIP_POTENTIAL_OPEN_ENABLE => NKCStringTable.GetString("SI_DP_EQUIP_POTENTIAL_OPEN_ENABLE");

	public static string GET_STRING_EQUIP_POTENTIAL_CANNOT_OPEN => NKCStringTable.GetString("SI_DP_EQUIP_POTENTIAL_CANNOT_OPEN");

	public static string GET_STRING_FORGE_RELIC_REROLL_HAS_RESERVED_EQUIP_TUNING => NKCStringTable.GetString("SI_PF_FORGE_RELIC_REROLL_HAS_RESERVED_EQUIP_TUNING");

	public static string GET_STRING_FORGE_RELIC_REROLL_EXIT_CONFIRM => NKCStringTable.GetString("SI_DP_FORGE_RELIC_REROLL_EXIT_CONFIRM");

	public static string GET_STRING_FORGE_RELIC_REROLL_CONFIRM_DESC_TWO_PARAM => NKCStringTable.GetString("SI_DP_FORGE_RELIC_REROLL_CONFIRM_DESC_TWO_PARAM");

	public static string GET_STRING_FACTORY_HIDDEN_OPTION_DISABLE => NKCStringTable.GetString("SI_PF_FACTORY_HIDDEN_OPTION_DISABLE");

	public static string GET_STRING_FACTORY_HIDDEN_OPTION_EMPTY => NKCStringTable.GetString("SI_PF_FACTORY_HIDDEN_OPTION_EMPTY");

	public static string GET_STRING_FACTORY_HIDDEN_OPTION_REROLL_DISABLE => NKCStringTable.GetString("SI_PF_FACTORY_HIDDEN_OPTION_REROLL_DISABLE");

	public static string GET_STRING_FACTORY_HIDDEN_OPTION_REROLL_EMPTY => NKCStringTable.GetString("SI_PF_FACTORY_HIDDEN_OPTION_REROLL_EMPTY");

	public static string GET_STRING_FACTORY_HIDDEN_OPTION_COST => NKCStringTable.GetString("SI_PF_FACTORY_HIDDEN_OPTION_COST");

	public static string GET_STRING_FACTORY_HIDDEN_OPTION_REROLL_COST => NKCStringTable.GetString("SI_PF_FACTORY_HIDDEN_OPTION_REROLL_COST");

	public static string GET_STRING_EQUIP_RELIC_REROLL_SLOT_OPEN => NKCStringTable.GetString("SI_DP_EQUIP_RELIC_REROLL_SLOT_OPEN");

	public static string GET_STRING_EQUIP_RELIC_REROLL_COUNT_LIMIT => NKCStringTable.GetString("SI_DP_EQUIP_RELIC_REROLL_COUNT_LIMIT");

	public static string GET_STRING_FACTORY_HIDDEN_OPTION_SOCKET => NKCStringTable.GetString("SI_PF_FACTORY_HIDDEN_OPTION_SOCKET");

	public static string GET_STRING_GREMORY_REQUEST => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_GREMORY_REQUEST");

	public static string GET_STRING_GREMORY_GIVE_DESC => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_GREMORY_GIVE_DESC");

	public static string GET_STRING_GREMORY_GIVE_END => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_GREMORY_GIVE_END");

	public static string GET_STRING_GREMORY_GIVE_CANCEL => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_GREMORY_GIVE_CANCEL_DESC");

	public static string GET_STRING_GREMORY_CREATE_RESULT => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_REWARD_TEXT");

	public static string GET_STRING_GREMORY_BARTENDER_HELLO => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_BARTENDER_MAKING_HELLO");

	public static string GET_STRING_GREMORY_MATERIAL_COUNT => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_MENU_COUNT_TEXT");

	public static string GET_STRING_GREMORY_BARTENDER_SHAKE => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_BARTENDER_MAKING_SHAKE");

	public static string GET_STRING_GREMORY_BARTENDER_STIR => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_BARTENDER_MAKING_STIR");

	public static string GET_STRING_GREMORY_BARTENDER_REJECT => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_BARTENDER_MAKING_REJECT");

	public static string GET_STRING_GREMORY_SELECT_COCKTAIL => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_GREMORY_GIVE_SELECT_DESC");

	public static string GET_STRING_GREMORY_THAT_IS_WRONG_COCKTAIL => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_GREMORY_GIVE_REJECT_DESC");

	public static string GET_STRING_GREMORY_NEED_MORE_COCKTAIL => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_GREMORY_GIVE_REJECT_AMOUNT_DESC");

	public static string GET_STRING_GREMORY_DAILY_REWARD => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_DAILYREWARD_TEXT");

	public static string GET_STRING_GREMORY_MOMO_HELLO_1 => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_MOMO_HELLO1");

	public static string GET_STRING_GREMORY_MOMO_HELLO_2 => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_MOMO_HELLO2");

	public static string GET_STRING_GREMORY_MOMO_IGNORE_MISSION => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_MOMO_IGNORE_MISSION");

	public static string GET_STRING_GREMORY_MOMO_IGNORE_ERRAND => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_MOMO_IGNORE_ERRAND");

	public static string GET_STRING_GREMORY_MOMO_COMPLETE_ERRAND => NKCStringTable.GetString("SI_PF_EVENT_GREMORY_BAR_MOMO_COMPLETE_ERRAND");

	public static string GET_STRING_JUKEBOX_TITLE => NKCStringTable.GetString("SI_DP_MUSIC");

	public static string GET_STRING_JUKEBOX_CONTENTS_UNLOCK => NKCStringTable.GetString("SI_CONTENTS_UNLOCK_BASE_PERSONNAL");

	public static string GET_STRING_JUKEBOX_BLOCK_SLEEP_MODE => NKCStringTable.GetString("SI_PF_BGM_SLEEP_MODE_NONE");

	public static string GET_STRING_JUKEBOX_FINISH_SLEEP_MODE => NKCStringTable.GetString("SI_PF_BGM_SLEEP_MODE_MUSIC_DONE");

	public static string GET_STRING_JUKEBOX_BLOCK_SLOT_MSG => NKCStringTable.GetString("SI_PF_BGM_BLIND_MINI_POPUP_TEXT");

	public static string GET_STRING_JUKEBOX_SLOT_UNLCOK_DESC_01 => NKCStringTable.GetString("SI_PF_BGM_UNLOCK");

	public static string GET_STRING_JUKEBOX_SAVE_BTN_TITLE_APPLY_LOBBY => NKCStringTable.GetString("SI_PF_USER_INFO_PRESET_SAVE");

	public static string GET_STRING_JUKEBOX_SAVE_BTN_TITLE_APPLY => NKCStringTable.GetString("SI_PF_APPLY");

	public static string GET_STRING_JUKEBOX_MUSIC_DEFAULT => NKCStringTable.GetString("SI_DP_LOBBY_MUSIC_DEFAULT");

	public static string GET_STRING_AD_FAILED_TO_LOAD => NKCStringTable.GetString("SI_DP_AD_FAILED_TO_LOAD");

	public static string GET_STRING_AD_FAILED_TO_SHOW => NKCStringTable.GetString("SI_DP_AD_FAILED_TO_SHOW");

	public static string GET_STRING_AD_NOT_INITIALIZED => NKCStringTable.GetString("SI_DP_AD_NOT_INITIALIZED");

	public static string GET_STRING_AD_NOT_READY_STATUS => NKCStringTable.GetString("SI_DP_AD_NOT_READY_STATUS");

	public static string GET_STRING_TRIM_NOT_INTERVAL_TIME => NKCStringTable.GetString("SI_PF_TRIM_DUNGEON_START_POPUP_INTERVAL_TEXT");

	public static string GET_STRING_TRIM_NOT_ENOUGH_SQUAD => NKCStringTable.GetString("SI_PF_TRIM_DUNGEON_START_POPUP_MADE_SQUAD_TEXT");

	public static string GET_STRING_TRIM_NOT_ENOUGH_POWER => NKCStringTable.GetString("SI_PF_TRIM_DUNGEON_START_POPUP_SQUAD_COMBAT_TEXT");

	public static string GET_STRING_TRIM_NOT_ENOUGH_TRY_COUNT => NKCStringTable.GetString("SI_PF_TRIM_DUNGEON_START_POPUP_COUNT_OVER_TEXT");

	public static string GET_STRING_TRIM_NOT_ENOUGH_TRY_COUNT_RESTORE => NKCStringTable.GetString("SI_PF_TRIM_DUNGEON_START_RESTORE_TEXT");

	public static string GET_STRING_TRIM_SQUAD_COMBAT_PENALTY => NKCStringTable.GetString("SI_PF_TRIM_DUNGEON_SQUAD_COMBAT_PENALTY_TEXT");

	public static string GET_STRING_TRIM_EXIST_TRIM_COMBAT_DATA => NKCStringTable.GetString("SI_DP_TRIM_MAIN_PLAYING_TEXT");

	public static string GET_STRING_TRIM_INTERVAL_END => NKCStringTable.GetString("SI_DP_TRIM_MAIN_END_INTERVAL_TEXT");

	public static string GET_STRING_TRIM_STAGE_INDEX => NKCStringTable.GetString("SI_DP_TRIM_DUNGEON_STAGE_TEXT");

	public static string GET_STRING_TRIM_ENTER_REMAIN_COUNT => NKCStringTable.GetString("SI_PF_WORLD_MAP_TRIM_BUTTON_COUNT");

	public static string GET_STRING_MOUDLE_MERGE_TARGET_EMPTY => NKCStringTable.GetString("SI_EVENT_MERGE_TARGET_EMPTY");

	public static string GET_STRING_MOUDLE_MERGE_NO_ENOUGH_COUNT => NKCStringTable.GetString("SI_EVENT_MERGE_NO_ENOUGH_COUNT");

	public static string GET_STRING_MODULE_MERGE_INPUT_COUNT => NKCStringTable.GetString("SI_DP_MERGE_AMOUNT");

	public static string GET_STRING_MODULE_MERGE_INPUT_UNIT_HAVE_COUNT => NKCStringTable.GetString("SI_DP_MERGE_HAVE_AMOUNT");

	public static string GET_STRING_MODULE_CONTRACT_MILEAGE_POINT_DESC_02 => NKCStringTable.GetString("SI_DP_CONTRACT_MILEAGE_POINT_EVENT_CLB_003");

	public static string GET_STRING_TACTIC_UPDATE => NKCStringTable.GetString("SI_DP_CONTRACT");

	public static string GET_STRING_TACTIC_UPDATE_UNIT_SELECT_LIST_EMPTY_MESSAGE => NKCStringTable.GetString("SI_DP_TACTIC_UPDATE_TEXT_UNIT_NOTICE");

	public static string GET_STRING_TACTIC_UPDATE_UNIT_SELECT_NONE => NKCStringTable.GetString("SI_DP_TACTIC_UPDATE_TEXT_UNIT_SELECT");

	public static string GET_STRING_TACTIC_UPDATE_MAX_UNIT => NKCStringTable.GetString("SI_DP_TACTIC_UPDATE_TEXT_UNIT_MAX");

	public static string GET_STRING_TACTIC_UPDATE_DESC_REARM => NKCStringTable.GetString("SI_PF_UNIT_TACTIC_UPDATE_DESC_REARM");

	public static string GET_STRING_TACTIC_UPDATE_DESC => NKCStringTable.GetString("SI_PF_UNIT_TACTIC_UPDATE_DESC");

	public static string GET_STRING_TACTIC_UPDATE_UNIT_WARNING => NKCStringTable.GetString("SI_DP_TACTIC_UPDATE_TEXT_UNIT_WARNING");

	public static string GET_STRING_TACTIC_UPDATE_REFOUND_POPUP_TITLE => NKCStringTable.GetString("SI_PF_UNIT_TACTIC_UPDATE_TEXT_RETURN");

	public static string GET_STRING_TACTIC_UPDATE_REFOUND_POPUP_DESC => NKCStringTable.GetString("SI_PF_UNIT_TACTIC_UPDATE_DESC_RETURN");

	public static string GET_STRING_TACTIC_UPDATE_BLOCK_MESSAGE_EQUIPED => NKCStringTable.GetString("SI_DP_TACTIC_UPDATE_TEXT_UNIT_EQUIP");

	public static string GET_STRING_TACTIC_UPDATE_SORT_TITLE => NKCStringTable.GetString("SI_PF_UNIT_TACTIC_UPDATE_TEXT_GRADE");

	public static string GET_STRING_TACTIC_UPDATE_RETURN_COUNT => NKCStringTable.GetString("SI_PF_UNIT_TACTIC_UPDATE_TEXT_RETURN_COUNT");

	public static string GET_STRING_TACTIC_CAN_NOT_TRY_LEVEL_UP => NKCStringTable.GetString("SI_DP_UNIT_REACTOR_NOT_MAX_LEVEL");

	public static string GET_STRING_TACTIC_UPDATE_DESC_TACTIC => NKCStringTable.GetString("SI_PF_UNIT_TACTIC_UPDATE_DESC_2");

	public static string GET_STRING_DEFENCE_BANNER_BEST_SCORE => NKCStringTable.GetString("SI_PF_DEFENCE_BANNER_BEST_SCORE");

	public static string GET_STRING_DEFENCE_SCORE_DESC => NKCStringTable.GetString("SI_PF_DEFENCE_SCORE_DESC");

	public static string GET_STRING_DEF_TOAST_EVENT_END => NKCStringTable.GetString("SI_PF_DEF_TOAST_EVENT_END");

	public static string GET_STRING_POPUP_FIERCE_BATTLE_REWARD_INFO_TOP_TEXT => NKCStringTable.GetString("SI_PF_POPUP_FIERCE_BATTLE_REWARD_INFO_TOP_TEXT");

	public static string GET_STRING_POPUP_FIERCE_BATTLE_REWARD_INFO_DESC => NKCStringTable.GetString("SI_PF_POPUP_FIERCE_BATTLE_REWARD_INFO_DESC");

	public static string GET_STRING_POPUP_DEF_REWARD_INFO_DESC => NKCStringTable.GetString("SI_PF_POPUP_DEF_REWARD_INFO_DESC");

	public static string GET_STRING_DEF_RECORD_TITLE => NKCStringTable.GetString("SI_PF_DEF_RECORD_TITLE");

	public static string GET_STRING_UNIT_REACTOR_SKILL_TITLE_SKILL => NKCStringTable.GetString("SI_DP_UNIT_REACTOR_SKILL_TITLE_SKILL");

	public static string GET_STRING_UNIT_REACTOR_SKILL_TITLE_STAT => NKCStringTable.GetString("SI_PF_UNIT_REACTOR_LEVEL_UP_STAT");

	public static string GET_STRING_UNIT_REACTOR_NOT_MATCH_TARGET_LEVEL => NKCStringTable.GetString("SI_DP_UNIT_REACTOR_NOT_MATCH_TARGET_LEVEL");

	public static string GET_STRING_UNIT_REACTOR_NOT_FIND_SKILL_DATA => NKCStringTable.GetString("SI_DP_UNIT_REACTOR_NOT_FIND_SKILL_DATA");

	public static string GET_STRING_UNIT_REACTOR_BASE_SKILL_NOT_MAX_LEVEL_01 => NKCStringTable.GetString("SI_DP_UNIT_REACTOR_BASE_SKILL_NOT_MAX_LEVEL_01");

	public static string GET_STRING_UNIT_REACTOR_UPGRADE_COST_ITEM_NOT_ENOUGH => NKCStringTable.GetString("SI_DP_UNIT_REACTOR_UPGRADE_COST_ITEM_NOT_ENOUGH");

	public static string GET_STRING_UNIT_REACTOR_INFO_NOT_OPEN => NKCStringTable.GetString("SI_DP_UNIT_REACTOR_NO_INFO");

	public static string GET_STRING_UNIT_REACTOR_EQUIP_TYPE => NKCStringTable.GetString("SI_DP_EQUIP_POSITION_REACTOR");

	public static string GET_STRING_UNIT_REACTOR_EQUIP_UNIT_STYLE_PRIVATE_01 => NKCStringTable.GetString("SI_DP_EQUIP_POSITION_STRING_BY_UNIT_STYLE_PRIVATE");

	public static string GET_STRING_UNIT_REACTOR_EQUIP_LEVEL_01 => NKCStringTable.GetString("SI_PF_UNIT_REACTOR_EQUIP_LEVEL");

	public static string GET_STRING_UNIT_REACTOR_REMOVE_WARNING_DESC => NKCStringTable.GetString("SI_DP_REACTOR_UNIT_WARNING_POPUP");

	public static string GET_STRING_UNIT_REACTOR_LOCK_MSG => NKCStringTable.GetString("SI_DP_UNIT_REACTOR_NO_INFO");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL_DECK_ENTER => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL_DECK_ENTER");

	public static string GET_STRING_TOURNAMENT_DECK_ENTER_POPUP_TIMEOUT => NKCStringTable.GetString("SI_DP_TOURNAMENT_DECK_ENTER_POPUP_TIMEOUT");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_BAN => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL_INFO_BAN");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_DECK => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL_INFO_DECK");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_READY_QUALIFY => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL_INFO_READY_QUALIFY");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_QUALIFY => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL_INFO_QUALIFY");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_READY_GROUP => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL_INFO_READY_GROUP");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_GROUP_A => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL_INFO_GROUP_A");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_GROUP_B => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL_INFO_GROUP_B");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_GROUP_C => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL_INFO_GROUP_C");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_GROUP_D => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL_INFO_GROUP_D");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_READY_FINAL => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL_INFO_READY_FINAL");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_FINAL => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL_INFO_FINAL");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_READY_END => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL_INFO_READY_END");

	public static string GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_END => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_INTERVAL_INFO_END");

	public static string GET_STRING_TOURNAMENT_REWARD_RANK_DESC_QUALIFY => NKCStringTable.GetString("SI_PF_TOURNAMENT_REWARD_RANK_DESC_QUALIFY");

	public static string GET_STRING_TOURNAMENT_REWARD_RANK_DESC_GROUP_32 => NKCStringTable.GetString("SI_PF_TOURNAMENT_REWARD_RANK_DESC_GROUP_32");

	public static string GET_STRING_TOURNAMENT_REWARD_RANK_DESC_GROUP_16 => NKCStringTable.GetString("SI_PF_TOURNAMENT_REWARD_RANK_DESC_GROUP_16");

	public static string GET_STRING_TOURNAMENT_REWARD_RANK_DESC_GROUP_8 => NKCStringTable.GetString("SI_PF_TOURNAMENT_REWARD_RANK_DESC_GROUP_8");

	public static string GET_STRING_TOURNAMENT_REWARD_RANK_DESC_FINAL => NKCStringTable.GetString("SI_PF_TOURNAMENT_REWARD_RANK_DESC_FINAL");

	public static string GET_STRING_TOURNAMENT_REWARD_PREDICT_DESC => NKCStringTable.GetString("SI_PF_TOURNAMENT_REWARD_PREDICT_DESC");

	public static string GET_STRING_TOURNAMENT_LOBBY_NO_REWARD => NKCStringTable.GetString("SI_DP_TOURNAMENT_LOBBY_NO_REWARD");

	public static string GE_STRING_TOURNAMENT_HOF_NO_RECORD => NKCStringTable.GetString("SI_DP_TOURNAMENT_HOF_NO_RECORD");

	public static string GET_STRING_TOURNAMENT_LOBBY_GUIDE => NKCStringTable.GetString("SI_PF_TOURNAMENT_LOBBY_GUIDE");

	public static string GET_STRING_TOURNAMENT_DECK_ENTER_POPUP_BAN_TIME => NKCStringTable.GetString("SI_DP_TOURNAMENT_DECK_ENTER_POPUP_BAN_TIME");

	public static string GET_STRING_TOURNAMENT_BAN_NO_LIST => NKCStringTable.GetString("SI_PF_TOURNAMENT_BAN_NO_LIST");

	public static string GET_STRING_TOURNAMENT_BAN_NO_SELECT => NKCStringTable.GetString("SI_PF_TOURNAMENT_BAN_NO_SELECT");

	public static string GET_STRING_TOURNAMENT_BAN_SUBTITLE => NKCStringTable.GetString("SI_PF_TOURNAMENT_BAN_SUBTITLE");

	public static string GET_STRING_TOURNAMENT_BAN_INFO_UNIT => NKCStringTable.GetString("SI_PF_TOURNAMENT_BAN_INFO_UNIT");

	public static string GET_STRING_TOURNAMENT_BAN_INFO_SHIP => NKCStringTable.GetString("SI_PF_TOURNAMENT_BAN_INFO_SHIP");

	public static string GET_STRING_TOURNAMENT_BAN_TIMEOUT => NKCStringTable.GetString("SI_PF_TOURNAMENT_BAN_TIMEOUT");

	public static string GET_STRING_TOURNAMENT_DECK_ENTER_TITLE => NKCStringTable.GetString("SI_PF_TOURNAMENT_DECK_ENTER_TITLE");

	public static string GET_STRING_TOURNAMENT_DECK_ENTER_POPUP => NKCStringTable.GetString("SI_DP_TOURNAMENT_DECK_ENTER_POPUP");

	public static string GET_STRING_TOURNAMENT_DECK_ENTER_POPUP_EMPTY => NKCStringTable.GetString("SI_DP_TOURNAMENT_DECK_ENTER_POPUP_EMPTY");

	public static string GET_STRING_TOURNAMENT_DECK_ENTER_POPUP_COMPLETE => NKCStringTable.GetString("SI_DP_TOURNAMENT_DECK_ENTER_POPUP_COMPLETE");

	public static string GET_STRING_TOURNAMENT_DECK_ENTER_POPUP_CHANGE => NKCStringTable.GetString("SI_DP_TOURNAMENT_DECK_ENTER_POPUP_CHANGE");

	public static string GET_STRING_TOURNAMENT_DECK_ENTER_POPUP_MINIMUM_CP => NKCStringTable.GetString("SI_DP_TOURNAMENT_DECK_ENTER_POPUP_MINIMUM_CP");

	public static string GET_STRING_TOURNAMENT_QUALIFY_COUNTDOWN => NKCStringTable.GetString("SI_PF_TOURNAMENT_QUALIFY_COUNTDOWN");

	public static string GET_STRING_TOURNAMENT_QUALIFY_NO_ENTER => NKCStringTable.GetString("SI_PF_TOURNAMENT_QUALIFY_NO_ENTER");

	public static string GET_STRING_TOURNAMENT_QUALIFY_EMPTY => NKCStringTable.GetString("SI_PF_TOURNAMENT_QUALIFY_EMPTY");

	public static string GET_STRING_TOURNAMENT_GROUP_ENTER_FAIL => NKCStringTable.GetString("SI_PF_TOURNAMENT_GROUP_ENTER_FAIL");

	public static string GET_STRING_TOURNAMENT_CHEERING_NOT_COMPLETED => NKCStringTable.GetString("SI_DP_TOURNAMENT_PREDICT_NO_SELECT");

	public static string GET_STRING_TOURNAMENT_CHEERING_REGISTERED => NKCStringTable.GetString("SI_DP_TOURNAMENT_PREDICT_COMPLETE");

	public static string GET_STRING_TOURNAMENT_CHEERING_CORRECT_COUNT => NKCStringTable.GetString("SI_PF_TOURNAMENT_PREDICT_COUNT");

	public static string GET_STRING_MATCHTEN_INGAME_RESTART_BUTTON => NKCStringTable.GetString("SI_PF_MATCHTEN_INGAME_RESTART_BUTTON");

	public static string GET_STRING_POPUP_MATCHTEN_HOME_DESC => NKCStringTable.GetString("SI_PF_POPUP_MATCHTEN_HOME_DESC_1") + "\n" + NKCStringTable.GetString("SI_PF_POPUP_MATCHTEN_HOME_DESC_2");

	public static string GET_STRING_POPUP_MATCHTEN_REFRESH_DESC => NKCStringTable.GetString("SI_PF_POPUP_MATCHTEN_REFRESH_DESC_1") + "\n" + NKCStringTable.GetString("SI_PF_POPUP_MATCHTEN_REFRESH_DESC_2");

	public static string GET_STRING_NEC_FAIL_MATCHTEN_INVALID_DATA => NKCStringTable.GetString("NEC_FAIL_MATCHTEN_INVALID_DATA");

	public static string GET_STRING_RATE_WEB_NOTICE_URL_KOR => NKCStringTable.GetString("SI_DP_RATE_WEB_NOTICE_URL_KOR");

	public static string GET_STRING_RATE_WEB_NOTICE_URL_JPN => NKCStringTable.GetString("SI_DP_RATE_WEB_NOTICE_URL_JPN");

	public static string GET_STRING_RATE_WEB_NOTICE_URL_GLOBAL => NKCStringTable.GetString("SI_DP_RATE_WEB_NOTICE_URL_GLOBAL");

	public static string GetPlayTimeWarning(int hour)
	{
		return string.Format(NKCStringTable.GetString("SI_DP_PC_PLAYTIME_WARNING"), hour);
	}

	public static string GetRankNumber(int rank, bool bUpper = false)
	{
		int num = rank % 100;
		if ((uint)(num - 11) <= 2u)
		{
			if (!bUpper)
			{
				return "th";
			}
			return "TH";
		}
		switch (rank % 10)
		{
		case 1:
			if (!bUpper)
			{
				return "st";
			}
			return "ST";
		case 2:
			if (!bUpper)
			{
				return "nd";
			}
			return "ND";
		case 3:
			if (!bUpper)
			{
				return "rd";
			}
			return "RD";
		default:
			if (!bUpper)
			{
				return "th";
			}
			return "TH";
		}
	}

	public static string GetDeckNumberString(NKMDeckIndex deckIndex)
	{
		return deckIndex.m_eDeckType switch
		{
			NKM_DECK_TYPE.NDT_NORMAL => GET_DECK_NUMBER_STRING_WARFARE + (deckIndex.m_iIndex + 1), 
			NKM_DECK_TYPE.NDT_PVP => GET_STRING_PVP + (deckIndex.m_iIndex + 1), 
			NKM_DECK_TYPE.NDT_DAILY => GET_DECK_NUMBER_STRING_DAILY + (deckIndex.m_iIndex + 1), 
			NKM_DECK_TYPE.NDT_FRIEND => GET_DECK_NUMBER_STRING_FRIEND + (deckIndex.m_iIndex + 1), 
			NKM_DECK_TYPE.NDT_RAID => GET_DECK_NUMBER_STRING_RAID + (deckIndex.m_iIndex + 1), 
			NKM_DECK_TYPE.NDT_PVP_DEFENCE => GET_DECK_NUMBER_STRING_PVP_DEFENCE, 
			NKM_DECK_TYPE.NDT_DIVE => GET_DECK_NUMBER_STRING_DIVE + (deckIndex.m_iIndex + 1), 
			NKM_DECK_TYPE.NDT_TRIM => GET_DECK_NUMBER_STRING_TRIM + (deckIndex.m_iIndex + 1), 
			NKM_DECK_TYPE.NDT_TOURNAMENT => (deckIndex.m_iIndex + 1).ToString(), 
			_ => "", 
		};
	}

	public static string GetBuffStatValueShortString(NKM_STAT_TYPE statType, int Value, bool TTRate = false)
	{
		int num = Value / 100;
		int num2 = Value % 100;
		if (TTRate)
		{
			num = Value / 10000;
			num2 = Value % 10000;
		}
		bool flag = false;
		if (num < 0 || num2 < 0)
		{
			flag = true;
		}
		num = Math.Abs(num);
		num2 = Math.Abs(num2);
		if (IsNameReversedIfNegative(statType))
		{
			if (num2 == 0)
			{
				return $"{GetStatShortName(statType, Value)} +{num}";
			}
			return $"{GetStatShortName(statType, Value)} +{num}.{num2:0#}";
		}
		if (num2 == 0)
		{
			if (!flag)
			{
				return $"{GetStatShortName(statType)} +{num}";
			}
			return $"{GetStatShortName(statType)} -{num}";
		}
		if (!flag)
		{
			return $"{GetStatShortName(statType)} +{num}.{num2:0#}";
		}
		return $"{GetStatShortName(statType)} -{num}.{num2:0#}";
	}

	public static string GetBuffStatFactorShortString(NKM_STAT_TYPE statType, int Factor, bool TTRate = false)
	{
		int num = Factor / 1;
		int num2 = Factor % 1;
		if (TTRate)
		{
			num = Factor / 100;
			num2 = Factor % 100;
		}
		bool flag = false;
		if (num < 0 || num2 < 0)
		{
			flag = true;
		}
		num = Math.Abs(num);
		num2 = Math.Abs(num2);
		if (IsNameReversedIfNegative(statType))
		{
			if (num2 == 0)
			{
				return $"{GetStatShortName(statType, Factor)} +{num}%";
			}
			return $"{GetStatShortName(statType, Factor)} +{num}.{num2:0#}%";
		}
		if (num2 == 0)
		{
			if (!flag)
			{
				return $"{GetStatShortName(statType, Factor)} +{num}%";
			}
			return $"{GetStatShortName(statType, Factor)} -{num}%";
		}
		if (!flag)
		{
			return $"{GetStatShortName(statType, Factor)} +{num}.{num2:0#}%";
		}
		return $"{GetStatShortName(statType, Factor)} -{num}.{num2:0#}%";
	}

	public static string GetDetailStatShortNameForInvenEquip(NKM_STAT_TYPE statType, float minValue, float maxValue, bool bPercent)
	{
		if (bPercent)
		{
			decimal num = new decimal(minValue);
			num = Math.Round(num * 10000m) / 10000m;
			decimal num2 = new decimal(maxValue);
			num2 = Math.Round(num2 * 10000m) / 10000m;
			if (IsNameReversedIfNegative(statType) && num2 < 0m)
			{
				return GetStatShortName(statType, bNegative: true) + " " + $"{-num2:P2}~{-num:P2}";
			}
			return GetStatShortName(statType) + " " + $"{num:P2}~{num2:P2}";
		}
		if (IsNameReversedIfNegative(statType) && maxValue < 0f)
		{
			return GetStatShortName(statType, bNegative: true) + " " + $"{Mathf.Abs(maxValue)}~{Mathf.Abs(minValue)}";
		}
		return GetStatShortName(statType) + " " + $"{minValue}~{maxValue}";
	}

	public static string GetStatRangeString(NKM_STAT_TYPE statType, float minValue, float maxValue, bool bPercent)
	{
		if (bPercent)
		{
			decimal num = new decimal(minValue);
			num = Math.Round(num * 1000m) / 1000m;
			decimal num2 = new decimal(maxValue);
			num2 = Math.Round(num2 * 1000m) / 1000m;
			if (IsNameReversedIfNegative(statType) && num2 < 0m)
			{
				return $"{-num2:P1}~{-num:P1}" ?? "";
			}
			return $"{num:P1}~{num2:P1}" ?? "";
		}
		if (IsNameReversedIfNegative(statType) && maxValue < 0f)
		{
			return $"{Mathf.Abs(maxValue)}~{Mathf.Abs(minValue)}" ?? "";
		}
		return $"{minValue}~{maxValue}" ?? "";
	}

	public static string GetStatShortName(NKM_STAT_TYPE statType, bool bNegative)
	{
		if (statType == NKM_STAT_TYPE.NST_RANDOM || statType == NKM_STAT_TYPE.NST_END)
		{
			return "";
		}
		NKCStatInfoTemplet nKCStatInfoTemplet = NKMTempletContainer<NKCStatInfoTemplet>.Find((NKCStatInfoTemplet x) => x.StatType == statType);
		if (nKCStatInfoTemplet != null)
		{
			if (bNegative && !string.IsNullOrEmpty(nKCStatInfoTemplet.Stat_Negative_Name))
			{
				return NKCStringTable.GetString(nKCStatInfoTemplet.Stat_Negative_Name);
			}
			return NKCStringTable.GetString(nKCStatInfoTemplet.Stat_Name);
		}
		Log.Error($"NKCStatInfoTemplet is null - StatType : {statType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCUtilString.cs", 356);
		return GetStatShortName(statType);
	}

	public static string GetStatShortName(NKM_STAT_TYPE statType, decimal value)
	{
		return GetStatShortName(statType, value < 0m);
	}

	public static string GetStatShortName(NKM_STAT_TYPE statType, int value)
	{
		return GetStatShortName(statType, value < 0);
	}

	public static string GetStatShortName(NKM_STAT_TYPE statType, float value)
	{
		return GetStatShortName(statType, value < 0f);
	}

	public static string GetStatShortString(NKM_STAT_TYPE statType, float value, bool bPercent)
	{
		if (bPercent)
		{
			return GetStatFactorShortString(statType, value);
		}
		return GetStatValueShortString(statType, value);
	}

	public static string GetStatShortString(NKM_STAT_TYPE statType, float factor, float value)
	{
		if (factor != 0f)
		{
			return GetStatFactorShortString(statType, factor);
		}
		return GetStatValueShortString(statType, value);
	}

	public static string GetStatShortString(string format, NKM_STAT_TYPE statType, float value)
	{
		if (IsNameReversedIfNegative(statType))
		{
			return string.Format(format, GetStatShortName(statType, value), Mathf.Abs(value));
		}
		return string.Format(format, GetStatShortName(statType), value);
	}

	public static string GetStatShortString(string format, NKM_STAT_TYPE statType, decimal value)
	{
		if (IsNameReversedIfNegative(statType) && value < 0m)
		{
			return string.Format(format, GetStatShortName(statType, value), -value);
		}
		return string.Format(format, GetStatShortName(statType), value);
	}

	public static string GetStatValueShortString(NKM_STAT_TYPE statType, float value, string format = "{0} {1:+#;-#;''}")
	{
		return GetStatShortString(format, statType, value);
	}

	public static string GetStatFactorShortString(NKM_STAT_TYPE statType, float factor, string format = "{0} {1:+#.0%;-#.0%;0%}")
	{
		return GetStatShortString(format, statType, factor);
	}

	public static string GetSlotOptionString(NKMShipCmdSlot slotData, string format = "[{0}] {1}")
	{
		if (slotData == null)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (slotData.targetStyleType.Count > 0)
		{
			stringBuilder.Append("[");
			bool flag = false;
			foreach (NKM_UNIT_STYLE_TYPE item in slotData.targetStyleType)
			{
				if (flag)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(GetUnitStyleName(item) ?? "");
				flag = true;
			}
			stringBuilder.Append("] ");
		}
		if (slotData.targetRoleType.Count > 0)
		{
			stringBuilder.Append("[");
			bool flag2 = false;
			foreach (NKM_UNIT_ROLE_TYPE item2 in slotData.targetRoleType)
			{
				if (flag2)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(GetRoleText(item2, bAwaken: false) ?? "");
				flag2 = true;
			}
			stringBuilder.Append("] ");
		}
		if (IsNameReversedIfNegative(slotData.statType) && slotData.statValue < 0f)
		{
			stringBuilder.Append(string.Format(format, GetStatShortName(slotData.statType, bNegative: true), GetShipModuleStatValue(slotData.statType, slotData.statValue)));
		}
		else
		{
			stringBuilder.Append(string.Format(format, GetStatShortName(slotData.statType), GetShipModuleStatValue(slotData.statType, slotData.statValue)));
		}
		return stringBuilder.ToString();
	}

	public static string GetShipModuleStatValue(NKM_STAT_TYPE statType, float statValue)
	{
		decimal num = new decimal(statValue);
		num = Math.Round(num * 1000m) / 1000m;
		if (num < 0m && IsNameReversedIfNegative(statType))
		{
			num = -num;
		}
		if (num > 0m)
		{
			if (NKMShipManager.IsPercentStat(statType, statValue))
			{
				return $"{num:P1}";
			}
			return $"{num}";
		}
		return string.Empty;
	}

	public static bool IsNameReversedIfNegative(NKM_STAT_TYPE statType)
	{
		NKCStatInfoTemplet nKCStatInfoTemplet = NKMTempletContainer<NKCStatInfoTemplet>.Find((NKCStatInfoTemplet x) => x.StatType == statType);
		if (nKCStatInfoTemplet != null)
		{
			return !string.IsNullOrEmpty(nKCStatInfoTemplet.Stat_Negative_Name);
		}
		return false;
	}

	public static string GetStatShortName(NKM_STAT_TYPE statType)
	{
		if (statType == NKM_STAT_TYPE.NST_RANDOM || statType == NKM_STAT_TYPE.NST_END)
		{
			return "";
		}
		NKCStatInfoTemplet nKCStatInfoTemplet = NKMTempletContainer<NKCStatInfoTemplet>.Find((NKCStatInfoTemplet x) => x.StatType == statType);
		if (nKCStatInfoTemplet != null)
		{
			return NKCStringTable.GetString(nKCStatInfoTemplet.Stat_Name);
		}
		Log.Error($"NKCStatInfoTemplet is null - StatType : {statType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCUtilString.cs", 525);
		return "";
	}

	public static string GetUnitStyleName(NKM_UNIT_STYLE_TYPE unitType)
	{
		return unitType switch
		{
			NKM_UNIT_STYLE_TYPE.NUST_COUNTER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_NAME_NUST_COUNTER"), 
			NKM_UNIT_STYLE_TYPE.NUST_MECHANIC => NKCStringTable.GetString("SI_DP_UNIT_STYLE_NAME_NUST_MECHANIC"), 
			NKM_UNIT_STYLE_TYPE.NUST_SOLDIER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_NAME_NUST_SOLDIER"), 
			NKM_UNIT_STYLE_TYPE.NUST_TRAINER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_NAME_NUST_TRAINER"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT => NKCStringTable.GetString("SI_DP_UNIT_STYLE_NAME_NUST_SHIP_ASSAULT"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_NAME_NUST_SHIP_CRUISER"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY => NKCStringTable.GetString("SI_DP_UNIT_STYLE_NAME_NUST_SHIP_HEAVY"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL => NKCStringTable.GetString("SI_DP_UNIT_STYLE_NAME_NUST_SHIP_SPECIAL"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_PATROL => NKCStringTable.GetString("SI_DP_UNIT_STYLE_NAME_NUST_SHIP_PATROL"), 
			NKM_UNIT_STYLE_TYPE.NUST_ENCHANT => NKCStringTable.GetString("SI_DP_UNIT_STYLE_NAME_NUST_ENCHANT"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_ETC => NKCStringTable.GetString("SI_DP_UNIT_STYLE_NAME_NUST_SHIP_ETC"), 
			NKM_UNIT_STYLE_TYPE.NUST_OPERATOR => NKCStringTable.GetString("SI_DP_UNIT_STYLE_NAME_NUST_OPERATOR"), 
			NKM_UNIT_STYLE_TYPE.NUST_REPLACER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_NAME_NUST_REPLACER"), 
			NKM_UNIT_STYLE_TYPE.NUST_CORRUPTED => NKCStringTable.GetString("SI_DP_UNIT_STYLE_NAME_NUST_CORRUPTED"), 
			_ => "", 
		};
	}

	public static string GetUnitStyleEngName(NKM_UNIT_STYLE_TYPE unitType)
	{
		return unitType switch
		{
			NKM_UNIT_STYLE_TYPE.NUST_COUNTER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_ENG_NAME_NUST_COUNTER"), 
			NKM_UNIT_STYLE_TYPE.NUST_MECHANIC => NKCStringTable.GetString("SI_DP_UNIT_STYLE_ENG_NAME_NUST_MECHANIC"), 
			NKM_UNIT_STYLE_TYPE.NUST_SOLDIER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_ENG_NAME_NUST_SOLDIER"), 
			NKM_UNIT_STYLE_TYPE.NUST_TRAINER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_ENG_NAME_NUST_TRAINER"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT => NKCStringTable.GetString("SI_DP_UNIT_STYLE_ENG_NAME_NUST_SHIP_ASSAULT"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_ENG_NAME_NUST_SHIP_CRUISER"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY => NKCStringTable.GetString("SI_DP_UNIT_STYLE_ENG_NAME_NUST_SHIP_HEAVY"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL => NKCStringTable.GetString("SI_DP_UNIT_STYLE_ENG_NAME_NUST_SHIP_SPECIAL"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_PATROL => NKCStringTable.GetString("SI_DP_UNIT_STYLE_ENG_NAME_NUST_SHIP_PATROL"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_ETC => NKCStringTable.GetString("SI_DP_UNIT_STYLE_ENG_NAME_NUST_SHIP_ETC"), 
			NKM_UNIT_STYLE_TYPE.NUST_REPLACER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_ENG_NAME_NUST_REPLACER"), 
			NKM_UNIT_STYLE_TYPE.NUST_CORRUPTED => NKCStringTable.GetString("SI_DP_UNIT_STYLE_ENG_NAME_NUST_CORRUPTED"), 
			_ => "", 
		};
	}

	public static string GetUnitStyleDesc(NKM_UNIT_STYLE_TYPE unitType)
	{
		return unitType switch
		{
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT => NKCStringTable.GetString("SI_DP_UNIT_STYLE_DESC_NUST_SHIP_ASSAULT"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_DESC_NUST_SHIP_CRUISER"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY => NKCStringTable.GetString("SI_DP_UNIT_STYLE_DESC_NUST_SHIP_HEAVY"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL => NKCStringTable.GetString("SI_DP_UNIT_STYLE_DESC_NUST_SHIP_SPECIAL"), 
			NKM_UNIT_STYLE_TYPE.NUST_COUNTER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_DESC_NUST_COUNTER"), 
			NKM_UNIT_STYLE_TYPE.NUST_MECHANIC => NKCStringTable.GetString("SI_DP_UNIT_STYLE_DESC_NUST_MECHANIC"), 
			NKM_UNIT_STYLE_TYPE.NUST_SOLDIER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_DESC_NUST_SOLDIER"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_PATROL => NKCStringTable.GetString("SI_DP_UNIT_STYLE_DESC_NUST_SHIP_PATROL"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_ETC => NKCStringTable.GetString("SI_DP_UNIT_STYLE_DESC_NUST_SHIP_ETC"), 
			_ => "", 
		};
	}

	public static string GetUnitStyleString(NKMUnitTempletBase unitTemplet)
	{
		if (unitTemplet == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GetUnitStyleName(unitTemplet.m_NKM_UNIT_STYLE_TYPE));
		if (unitTemplet.m_NKM_UNIT_STYLE_TYPE_SUB != NKM_UNIT_STYLE_TYPE.NUST_INVALID)
		{
			stringBuilder.Append(" ");
			stringBuilder.Append(GetUnitStyleName(unitTemplet.m_NKM_UNIT_STYLE_TYPE_SUB));
		}
		return stringBuilder.ToString();
	}

	public static string GetUnitStyleMarkString(NKMUnitTempletBase unitTemplet, bool bUseColor = true)
	{
		if (unitTemplet == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GetUnitStyleEngName(unitTemplet.m_NKM_UNIT_STYLE_TYPE));
		if (unitTemplet.m_NKM_UNIT_STYLE_TYPE_SUB != NKM_UNIT_STYLE_TYPE.NUST_INVALID)
		{
			stringBuilder.Append(" ");
			string unitSubTypeColorCode = GetUnitSubTypeColorCode(unitTemplet.m_NKM_UNIT_STYLE_TYPE_SUB);
			if (bUseColor && !string.IsNullOrEmpty(unitSubTypeColorCode))
			{
				stringBuilder.AppendFormat("<color=#{0}>", unitSubTypeColorCode);
			}
			stringBuilder.Append(GetUnitStyleEngName(unitTemplet.m_NKM_UNIT_STYLE_TYPE_SUB));
			if (bUseColor && !string.IsNullOrEmpty(unitSubTypeColorCode))
			{
				stringBuilder.Append("</color>");
			}
		}
		return stringBuilder.ToString();
	}

	public static string GetUnitSubTypeColorCode(NKM_UNIT_STYLE_TYPE unitType)
	{
		return unitType switch
		{
			NKM_UNIT_STYLE_TYPE.NUST_REPLACER => "FF0081", 
			NKM_UNIT_STYLE_TYPE.NUST_CORRUPTED => "EC2200", 
			_ => "", 
		};
	}

	public static string GetEquipPosSimpleStrByUnitStyle(NKM_UNIT_STYLE_TYPE unitType, ITEM_EQUIP_POSITION equipPosition)
	{
		return $"{GetUnitStyleName(unitType)} {GetEquipPositionString(equipPosition)}";
	}

	public static string GetSkillTypeName(NKM_SKILL_TYPE type)
	{
		switch (type)
		{
		case NKM_SKILL_TYPE.NST_PASSIVE:
			return NKCStringTable.GetString("SI_DP_SKILL_TYPE_NAME_NST_PASSIVE");
		case NKM_SKILL_TYPE.NST_ATTACK:
			return NKCStringTable.GetString("SI_DP_SKILL_TYPE_NAME_NST_ATTACK");
		case NKM_SKILL_TYPE.NST_SKILL:
			return NKCStringTable.GetString("SI_DP_SKILL_TYPE_NAME_NST_SKILL");
		case NKM_SKILL_TYPE.NST_HYPER:
			return NKCStringTable.GetString("SI_DP_SKILL_TYPE_NAME_NST_HYPER");
		case NKM_SKILL_TYPE.NST_SHIP_ACTIVE:
			return NKCStringTable.GetString("SI_DP_SKILL_TYPE_NAME_NST_SHIP_ACTIVE");
		case NKM_SKILL_TYPE.NST_LEADER:
			return NKCStringTable.GetString("SI_PF_REARM_LEADER_SKILL");
		default:
			Debug.LogError("Unknown skill type");
			return "";
		}
	}

	public static string GetRoleText(NKMUnitTempletBase unitTempletBase)
	{
		if (unitTempletBase == null)
		{
			return "";
		}
		return GetRoleText(unitTempletBase.m_NKM_UNIT_ROLE_TYPE, unitTempletBase.m_bAwaken);
	}

	public static string GetRoleText(NKM_UNIT_ROLE_TYPE roleType, bool bAwaken)
	{
		if (bAwaken)
		{
			switch (roleType)
			{
			case NKM_UNIT_ROLE_TYPE.NURT_STRIKER:
				return NKCStringTable.GetString("SI_DP_ROLE_TEXT_NURT_STRIKER_AWAKEN");
			case NKM_UNIT_ROLE_TYPE.NURT_RANGER:
				return NKCStringTable.GetString("SI_DP_ROLE_TEXT_NURT_RANGER_AWAKEN");
			case NKM_UNIT_ROLE_TYPE.NURT_DEFENDER:
				return NKCStringTable.GetString("SI_DP_ROLE_TEXT_NURT_DEFENDER_AWAKEN");
			case NKM_UNIT_ROLE_TYPE.NURT_SNIPER:
				return NKCStringTable.GetString("SI_DP_ROLE_TEXT_NURT_SNIPER_AWAKEN");
			case NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER:
				return NKCStringTable.GetString("SI_DP_ROLE_TEXT_NURT_SUPPORTER_AWAKEN");
			case NKM_UNIT_ROLE_TYPE.NURT_SIEGE:
				return NKCStringTable.GetString("SI_DP_ROLE_TEXT_NURT_SIEGE_AWAKEN");
			case NKM_UNIT_ROLE_TYPE.NURT_TOWER:
				return NKCStringTable.GetString("SI_DP_ROLE_TEXT_NURT_TOWER_AWAKEN");
			}
			Debug.LogError("not implemented type : " + roleType);
		}
		else
		{
			switch (roleType)
			{
			case NKM_UNIT_ROLE_TYPE.NURT_STRIKER:
				return NKCStringTable.GetString("SI_DP_ROLE_TEXT_NURT_STRIKER");
			case NKM_UNIT_ROLE_TYPE.NURT_RANGER:
				return NKCStringTable.GetString("SI_DP_ROLE_TEXT_NURT_RANGER");
			case NKM_UNIT_ROLE_TYPE.NURT_DEFENDER:
				return NKCStringTable.GetString("SI_DP_ROLE_TEXT_NURT_DEFENDER");
			case NKM_UNIT_ROLE_TYPE.NURT_SNIPER:
				return NKCStringTable.GetString("SI_DP_ROLE_TEXT_NURT_SNIPER");
			case NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER:
				return NKCStringTable.GetString("SI_DP_ROLE_TEXT_NURT_SUPPORTER");
			case NKM_UNIT_ROLE_TYPE.NURT_SIEGE:
				return NKCStringTable.GetString("SI_DP_ROLE_TEXT_NURT_SIEGE");
			case NKM_UNIT_ROLE_TYPE.NURT_TOWER:
				return NKCStringTable.GetString("SI_DP_ROLE_TEXT_NURT_TOWER");
			}
			Debug.LogError("not implemented type : " + roleType);
		}
		return "";
	}

	public static string GetMoveTypeText(bool bAirUnit)
	{
		string text = "";
		if (bAirUnit)
		{
			return NKCStringTable.GetString("SI_DP_MOVE_TYPE_TEXT_MT_AIR");
		}
		return NKCStringTable.GetString("SI_DP_MOVE_TYPE_TEXT_MT_LAND");
	}

	public static string GetAtkTypeText(NKMUnitTempletBase unitTempletBase)
	{
		if (unitTempletBase == null)
		{
			return "";
		}
		if (unitTempletBase.m_NKM_FIND_TARGET_TYPE_Desc == NKM_FIND_TARGET_TYPE.NFTT_INVALID)
		{
			return GetAtkTypeText(unitTempletBase.m_NKM_FIND_TARGET_TYPE);
		}
		return GetAtkTypeText(unitTempletBase.m_NKM_FIND_TARGET_TYPE_Desc);
	}

	public static string GetAtkTypeText(NKM_FIND_TARGET_TYPE targetType)
	{
		switch (targetType)
		{
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR_BOSS_LAST:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_FAR_ENEMY_AIR");
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND_RANGER_SUPPORTER_SNIPER_FIRST:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND_BOSS_LAST:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_FAR_ENEMY_LAND");
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR_FIRST:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_BOSS_LAST:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_FAR_ENEMY");
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_AIR:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_ENEMY_BOSS_AIR");
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_LAND:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_ENEMY_BOSS_LAND");
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_ONLY:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_ENEMY_BOSS");
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_NEAR_MY_TEAM");
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LAND:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_NEAR_MY_TEAM_LAND");
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_AIR:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_NEAR_MY_TEAM_AIR");
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_FAR_MY_TEAM");
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_LAND:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_FAR_MY_TEAM_LAND");
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_AIR:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_FAR_MY_TEAM_AIR");
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_NEAR_MY_TEAM_LOW_HP");
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_LAND:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_NEAR_MY_TEAM_LOW_HP_LAND");
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_AIR:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_NEAR_MY_TEAM_LOW_HP_AIR");
		case NKM_FIND_TARGET_TYPE.NFTT_NO:
			return NKCStringTable.GetString("SI_DP_ATK_TYPE_TEXT_NFTT_NO");
		default:
			return "";
		}
	}

	public static string GetSourceTypeName(NKM_UNIT_SOURCE_TYPE sourceType)
	{
		return sourceType switch
		{
			NKM_UNIT_SOURCE_TYPE.NUST_CONFLICT => NKCStringTable.GetString("SI_WEAK_TAG_SYMBOL_CONFLICT"), 
			NKM_UNIT_SOURCE_TYPE.NUST_STABLE => NKCStringTable.GetString("SI_WEAK_TAG_SYMBOL_STABLE"), 
			NKM_UNIT_SOURCE_TYPE.NUST_LIBERAL => NKCStringTable.GetString("SI_WEAK_TAG_SYMBOL_LIBERAL"), 
			_ => "", 
		};
	}

	public static string GetGradeString(NKM_UNIT_GRADE grade)
	{
		return grade switch
		{
			NKM_UNIT_GRADE.NUG_R => NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_R"), 
			NKM_UNIT_GRADE.NUG_SR => NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_SR"), 
			NKM_UNIT_GRADE.NUG_SSR => NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_SSR"), 
			_ => NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_N"), 
		};
	}

	public static string GetItemEquipTier(int tier)
	{
		return string.Format($"T{tier}");
	}

	public static string GetItemEquipNameWithTier(NKMEquipTemplet equipTemplet)
	{
		if (equipTemplet != null)
		{
			return string.Format(equipTemplet.GetItemName() + " " + GetItemEquipTier(equipTemplet.m_NKM_ITEM_TIER));
		}
		return "";
	}

	public static string GetSlotModeTypeString(NKCUISlot.eSlotMode type, int ID)
	{
		switch (type)
		{
		case NKCUISlot.eSlotMode.Unit:
		case NKCUISlot.eSlotMode.UnitCount:
			return NKCStringTable.GetString("SI_DP_SLOT_MODE_TYPE_STRING_UNIT");
		case NKCUISlot.eSlotMode.Mold:
			return NKCStringTable.GetString("SI_DP_SLOT_MODE_TYPE_STRING_MOLD");
		case NKCUISlot.eSlotMode.ItemMisc:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(ID);
			if (itemMiscTempletByID != null)
			{
				return GetMiscTypeString(itemMiscTempletByID.m_ItemMiscType);
			}
			break;
		}
		case NKCUISlot.eSlotMode.Equip:
		case NKCUISlot.eSlotMode.EquipCount:
			return NKCStringTable.GetString("SI_DP_SLOT_MODE_TYPE_STRING_EQUIP");
		case NKCUISlot.eSlotMode.Skin:
			return NKCStringTable.GetString("SI_DP_SLOT_MODE_TYPE_STRING_SKIN");
		}
		return "";
	}

	public static string GetUnitAbilityName(int unitID, string seperator = "   ")
	{
		StringBuilder stringBuilder = new StringBuilder();
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		bool flag = false;
		if (unitTempletBase != null)
		{
			foreach (NKM_UNIT_TAG item in unitTempletBase.m_hsUnitTag)
			{
				if (flag)
				{
					stringBuilder.Append(seperator);
				}
				stringBuilder.Append(GetUnitTagString(item));
				flag = true;
			}
		}
		return stringBuilder.ToString();
	}

	public static string GetUnitTagString(NKM_UNIT_TAG tag)
	{
		return tag switch
		{
			NKM_UNIT_TAG.NUT_PATROL => NKCStringTable.GetString("SI_DP_UNIT_ABILITY_NAME_TAG_PATROL"), 
			NKM_UNIT_TAG.NUT_SWINGBY => NKCStringTable.GetString("SI_DP_UNIT_ABILITY_NAME_TAG_SWINGBY"), 
			NKM_UNIT_TAG.NUT_RESPAWN_FREE_POS => NKCStringTable.GetString("SI_DP_UNIT_ABILITY_NAME_TAG_RESPAWN_FREE_POS"), 
			NKM_UNIT_TAG.NUT_REVENGE => NKCStringTable.GetString("SI_DP_UNIT_ABILITY_NAME_TAG_REVENGE"), 
			NKM_UNIT_TAG.NUT_FURY => NKCStringTable.GetString("SI_PF_POPUP_UNIT_INFOPOPUP_FURY"), 
			NKM_UNIT_TAG.NUT_MULTI_UNIT => NKCStringTable.GetString("SI_DP_UNIT_ABILITY_NAME_TAG_MULTI_UNIT"), 
			NKM_UNIT_TAG.NUT_LIMIT_1 => NKCStringTable.GetString("SI_DP_UNIT_ABILITY_NAME_TAG_LIMIT", 1), 
			NKM_UNIT_TAG.NUT_LIMIT_2 => NKCStringTable.GetString("SI_DP_UNIT_ABILITY_NAME_TAG_LIMIT", 2), 
			NKM_UNIT_TAG.NUT_LIMIT_3 => NKCStringTable.GetString("SI_DP_UNIT_ABILITY_NAME_TAG_LIMIT", 3), 
			_ => string.Empty, 
		};
	}

	public static string GetEquipTypeString(NKMEquipTemplet equipTemplet)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(equipTemplet.GetPrivateUnitID());
		if (unitTempletBase != null)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_EQUIP_TYPE_STRING_PRIVATE"), unitTempletBase.GetUnitName(), GetEquipPositionString(equipTemplet.m_ItemEquipPosition));
		}
		return string.Format(NKCStringTable.GetString("SI_DP_EQUIP_TYPE_STRING_ELSE"), GetUnitStyleName(equipTemplet.m_EquipUnitStyleType), GetEquipPositionString(equipTemplet.m_ItemEquipPosition));
	}

	public static string GetEquipPositionStringByUnitStyle(NKMEquipTemplet equipTemplet, bool skipPrivateUnit = false)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(equipTemplet.GetPrivateUnitID());
		if (unitTempletBase != null && !skipPrivateUnit)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_EQUIP_POSITION_STRING_BY_UNIT_STYLE_PRIVATE"), unitTempletBase.GetUnitName());
		}
		ITEM_EQUIP_POSITION itemEquipPosition = equipTemplet.m_ItemEquipPosition;
		return GetEquipPosSimpleStrByUnitStyle(equipTemplet.m_EquipUnitStyleType, itemEquipPosition);
	}

	public static string GetRespawnCountText(int unitID)
	{
		string result = "";
		int num = NKMUnitManager.GetUnitStatTemplet(unitID)?.m_RespawnCount ?? 1;
		if (num > 1)
		{
			result = string.Format("x" + num);
		}
		return result;
	}

	public static string GetFriendCode(long friendCode, bool bOpponent)
	{
		if (friendCode <= 0)
		{
			return "";
		}
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			if (bOpponent)
			{
				if (gameOptionData.StreamingHideOpponentInfo)
				{
					return "";
				}
			}
			else if (gameOptionData.StreamingHideMyInfo)
			{
				return "";
			}
		}
		return string.Format($"#{friendCode}");
	}

	public static string GetFriendCode(long friendCode)
	{
		if (friendCode <= 0)
		{
			return "";
		}
		return string.Format($"#{friendCode}");
	}

	public static string GET_STRING_CONFIRM_BY_ALL_SEARCH()
	{
		if (NKCStringTable.CheckExistString("SI_DP_CONFIRM"))
		{
			return NKCStringTable.GetString("SI_DP_CONFIRM");
		}
		if (NKCStringTable.CheckExistString("SI_DP_PATCHER_CONFIRM"))
		{
			return NKCStringTable.GetString("SI_DP_PATCHER_CONFIRM");
		}
		return "";
	}

	public static string GET_STRING_CANCEL_BY_ALL_SEARCH()
	{
		if (NKCStringTable.CheckExistString("SI_DP_CANCEL"))
		{
			return NKCStringTable.GetString("SI_DP_CANCEL");
		}
		if (NKCStringTable.CheckExistString("SI_DP_PATCHER_CANCEL"))
		{
			return NKCStringTable.GetString("SI_DP_PATCHER_CANCEL");
		}
		return "";
	}

	public static string GetEnhanceProgressString(float progressPercent)
	{
		if (progressPercent >= 1f)
		{
			return "MAX";
		}
		return $"{Mathf.FloorToInt(progressPercent * 100f)}%";
	}

	public static string GetMonthString(int month)
	{
		return NKCStringTable.GetString($"SI_DATE_MONTH_{month}");
	}

	public static string GetCollectionRateStrByType(NKCUICollectionGeneral.CollectionType type)
	{
		string result = NKCStringTable.GetString("SI_DP_COLLECTION_RATE_STRING_BY_TYPE_CT_TEAM_UP");
		switch (type)
		{
		case NKCUICollectionGeneral.CollectionType.CT_TEAM_UP:
			result = NKCStringTable.GetString("SI_DP_COLLECTION_RATE_STRING_BY_TYPE_CT_TEAM_UP");
			break;
		case NKCUICollectionGeneral.CollectionType.CT_SHIP:
			result = NKCStringTable.GetString("SI_DP_COLLECTION_RATE_STRING_BY_TYPE_CT_SHIP");
			break;
		case NKCUICollectionGeneral.CollectionType.CT_UNIT:
			result = NKCStringTable.GetString("SI_DP_COLLECTION_RATE_STRING_BY_TYPE_CT_UNIT");
			break;
		case NKCUICollectionGeneral.CollectionType.CT_ILLUST:
			result = NKCStringTable.GetString("SI_DP_COLLECTION_RATE_STRING_BY_TYPE_CT_ILLUST");
			break;
		case NKCUICollectionGeneral.CollectionType.CT_STORY:
			result = NKCStringTable.GetString("SI_DP_COLLECTION_RATE_STRING_BY_TYPE_CT_STORY");
			break;
		case NKCUICollectionGeneral.CollectionType.CT_OPERATOR:
			result = NKCStringTable.GetString("SI_DP_COLLECTION_RATE_STRING_BY_TYPE_CT_OPERATOR");
			break;
		case NKCUICollectionGeneral.CollectionType.CT_EMBLEM:
			result = NKCStringTable.GetString("SI_DP_COLLECTION_RATE_STRING_BY_TYPE_CT_EMBLEM");
			break;
		case NKCUICollectionGeneral.CollectionType.CT_FRAME:
			result = NKCStringTable.GetString("SI_DP_COLLECTION_RATE_STRING_BY_TYPE_CT_FRAME");
			break;
		case NKCUICollectionGeneral.CollectionType.CT_BACKGROUND:
			result = NKCStringTable.GetString("SI_DP_COLLECTION_RATE_STRING_BY_TYPE_CT_BACKGROUND");
			break;
		}
		return result;
	}

	public static string GetCollectionStoryCategory(NKCCollectionManager.COLLECTION_STORY_CATEGORY category)
	{
		return category switch
		{
			NKCCollectionManager.COLLECTION_STORY_CATEGORY.MAINSTREAM => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_MAINSTREAM"), 
			NKCCollectionManager.COLLECTION_STORY_CATEGORY.SIDESTORY => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_SUBSTREAM"), 
			NKCCollectionManager.COLLECTION_STORY_CATEGORY.EVENT => GET_STRING_EPISODE_CATEGORY_EC_EVENT, 
			NKCCollectionManager.COLLECTION_STORY_CATEGORY.WORLDMAP => GET_STRING_WORLDMAP, 
			NKCCollectionManager.COLLECTION_STORY_CATEGORY.SEASONAL => NKCStringTable.GetString("SI_OPERATION_MENU_TEXT_SEASONAL_BUTTON"), 
			NKCCollectionManager.COLLECTION_STORY_CATEGORY.ETC => GET_STRING_COLLECTION_STORY_ETC_TITLE, 
			_ => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_DEFAULT"), 
		};
	}

	public static string GetFinalMailContents(string originContents)
	{
		if (NKCPublisherModule.Localization.IsPossibleJson(originContents))
		{
			return NKCPublisherModule.Localization.GetTranslationIfJson(originContents);
		}
		return NKCServerStringFormatter.TranslateServerFormattedString(originContents);
	}

	public static string GetExpandDesc(NKM_INVENTORY_EXPAND_TYPE type, bool isFullMsg = false)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		switch (type)
		{
		case NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP:
			text = NKCStringTable.GetString("SI_DP_EXPAND_DESC_NIET_EQUIP_DESC").Split('\n')[0];
			text2 = NKCStringTable.GetString("SI_DP_EXPAND_DESC_NIET_EQUIP_FULL");
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT:
			text = NKCStringTable.GetString("SI_DP_EXPAND_DESC_NIET_UNIT_DESC").Split('\n')[0];
			text2 = NKCStringTable.GetString("SI_DP_EXPAND_DESC_NIET_UNIT_FULL");
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP:
			text = NKCStringTable.GetString("SI_DP_EXPAND_DESC_NIET_SHIP_DESC").Split('\n')[0];
			text2 = NKCStringTable.GetString("SI_DP_EXPAND_DESC_NIET_SHIP_FULL");
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_OPERATOR:
			text = GET_STRING_INVENTORY_OPERATOR_ADD_DESC.Split('\n')[0];
			text2 = GET_STRING_INVENTORY_OPERATOR_ADD_FULL;
			break;
		}
		if (isFullMsg)
		{
			if (NKCAdManager.IsAdRewardInventory(type))
			{
				return text2.Split('\n')[0] + " " + text + "\n" + NKCStringTable.GetString("SI_DP_AD_EXPAND_DESC");
			}
			return text2 + text;
		}
		if (NKCAdManager.IsAdRewardInventory(type))
		{
			text = text + "\n" + NKCStringTable.GetString("SI_DP_AD_EXPAND_DESC");
		}
		return text;
	}

	public static string GetEpisodeCategory(EPISODE_CATEGORY episode_category)
	{
		return episode_category switch
		{
			EPISODE_CATEGORY.EC_MAINSTREAM => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_MAINSTREAM"), 
			EPISODE_CATEGORY.EC_SUPPLY => NKCStringTable.GetString("SI_OPERATION_MENU_TEXT_SUPPLYMISSION"), 
			EPISODE_CATEGORY.EC_DAILY => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_DAILY"), 
			EPISODE_CATEGORY.EC_COUNTERCASE => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_COUNTERCASE"), 
			EPISODE_CATEGORY.EC_SIDESTORY => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_SUBSTREAM"), 
			EPISODE_CATEGORY.EC_FIELD => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_FIELD"), 
			EPISODE_CATEGORY.EC_EVENT => GET_STRING_EPISODE_CATEGORY_EC_EVENT, 
			EPISODE_CATEGORY.EC_CHALLENGE => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_CHALLENGE"), 
			EPISODE_CATEGORY.EC_TIMEATTACK => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_TIMEATTACK"), 
			EPISODE_CATEGORY.EC_TRIM => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_TRIM"), 
			EPISODE_CATEGORY.EC_FIERCE => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_FIERCE"), 
			EPISODE_CATEGORY.EC_SHADOW => NKCStringTable.GetString("SI_PF_WORLD_MAP_SHADOW_BUTTON_TEXT"), 
			EPISODE_CATEGORY.EC_SEASONAL => NKCStringTable.GetString("SI_OPERATION_MENU_TEXT_SEASONAL"), 
			_ => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_DEFAULT"), 
		};
	}

	public static string GetEpisodeCategoryEx1(EPISODE_CATEGORY episode_category)
	{
		return episode_category switch
		{
			EPISODE_CATEGORY.EC_MAINSTREAM => NKCStringTable.GetString("SI_OPERATION_MENU_TEXT_MAINSTREAM"), 
			EPISODE_CATEGORY.EC_SUPPLY => NKCStringTable.GetString("SI_OPERATION_MENU_TEXT_SUPPLYMISSION"), 
			EPISODE_CATEGORY.EC_DAILY => NKCStringTable.GetString("SI_OPERATION_MENU_TEXT_DAILYMISSION"), 
			EPISODE_CATEGORY.EC_COUNTERCASE => NKCStringTable.GetString("SI_OPERATION_MENU_TEXT_COUNTERCASE"), 
			EPISODE_CATEGORY.EC_SIDESTORY => NKCStringTable.GetString("SI_OPERATION_MENU_TEXT_ANOTHERSTORY3"), 
			EPISODE_CATEGORY.EC_FIELD => NKCStringTable.GetString("SI_OPERATION_MENU_TEXT_FREECONTRACT"), 
			EPISODE_CATEGORY.EC_EVENT => NKCStringTable.GetString("SI_OPERATION_MENU_TEXT_EVENT"), 
			EPISODE_CATEGORY.EC_SEASONAL => NKCStringTable.GetString("SI_OPERATION_MENU_TEXT_SEASONAL"), 
			EPISODE_CATEGORY.EC_CHALLENGE => NKCStringTable.GetString("SI_OPERATION_MENU_TEXT_CHALLENGE"), 
			_ => NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_DEFAULT"), 
		};
	}

	public static string GetEpisodeName(EPISODE_CATEGORY episode_category, string episode_name, string episode_title)
	{
		return episode_category switch
		{
			EPISODE_CATEGORY.EC_DAILY => string.Format(NKCStringTable.GetString("SI_DP_EPISODE_NAME_EC_DAILY"), episode_name, ""), 
			EPISODE_CATEGORY.EC_CHALLENGE => string.Format(NKCStringTable.GetString("SI_DP_EPISODE_NAME_DEFAULT"), episode_title, ""), 
			_ => string.Format(NKCStringTable.GetString("SI_DP_EPISODE_NAME_DEFAULT"), episode_title, episode_name), 
		};
	}

	public static string GetSidestoryUnlockRequireDesc(NKMStageTempletV2 stageTemplet)
	{
		NKMStageTempletV2 nKMStageTempletV = null;
		switch (stageTemplet.m_UnlockInfo.eReqType)
		{
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE:
			nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(NKMWarfareTemplet.Find(stageTemplet.m_UnlockInfo.reqValue).m_WarfareStrID);
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON:
			nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(NKMDungeonManager.GetDungeonStrID(stageTemplet.m_UnlockInfo.reqValue));
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE:
			nKMStageTempletV = NKMPhaseTemplet.Find(stageTemplet.m_UnlockInfo.reqValue).StageTemplet;
			break;
		}
		if (nKMStageTempletV != null)
		{
			NKMDungeonTempletBase dungeonTempletBase = nKMStageTempletV.DungeonTempletBase;
			if (dungeonTempletBase != null && dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_CUTSCENE_AND_BUY_EC_SIDESTORY"), nKMStageTempletV.EpisodeTemplet.GetEpisodeTitle(), nKMStageTempletV.GetDungeonName());
			}
			return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_DUNGEON_AND_BUY_EC_SIDESTORY"), nKMStageTempletV.EpisodeTemplet.GetEpisodeTitle(), nKMStageTempletV.ActId, nKMStageTempletV.m_StageUINum);
		}
		Debug.LogError($"StageTemplet is null - {stageTemplet.m_UnlockInfo.reqValue}");
		return "";
	}

	public static string GetUnlockConditionRequireDesc(NKMStageTempletV2 stageTemplet, bool bSimple = false)
	{
		if (stageTemplet == null)
		{
			return "";
		}
		return GetUnlockConditionRequireDesc(stageTemplet.m_UnlockInfo, bSimple);
	}

	public static string GetUnlockConditionRequireDesc(UnlockInfo unlockInfo, bool bSimple = false)
	{
		return GetUnlockConditionRequireDesc(unlockInfo.eReqType, unlockInfo.reqValue, unlockInfo.reqValueStr, unlockInfo.reqDateTime, bSimple);
	}

	public static string GetUnlockConditionRequireDesc(List<UnlockInfo> lstUnlockInfo, bool bLockedOnly, bool bSimple = false)
	{
		if (lstUnlockInfo == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		NKMUserData cNKMUserData = NKCScenManager.CurrentUserData();
		foreach (UnlockInfo item in lstUnlockInfo)
		{
			UnlockInfo unlockInfo = item;
			if (!bLockedOnly || !NKMContentUnlockManager.IsContentUnlocked(cNKMUserData, in unlockInfo))
			{
				stringBuilder.AppendLine(GetUnlockConditionRequireDesc(unlockInfo, bSimple));
			}
		}
		return stringBuilder.ToString().TrimEnd();
	}

	public static string GetStageUnlockConditionRequireDesc(NKMStageTempletV2 stageTempletRequire, bool bSimple)
	{
		if (stageTempletRequire != null)
		{
			switch (stageTempletRequire.EpisodeCategory)
			{
			case EPISODE_CATEGORY.EC_MAINSTREAM:
			{
				NKMEpisodeTempletV2 episodeTemplet3 = stageTempletRequire.EpisodeTemplet;
				if (episodeTemplet3 != null)
				{
					string text = ((stageTempletRequire.m_Difficulty == EPISODE_DIFFICULTY.NORMAL) ? "" : string.Format("[{0}]", NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_ADDON_HARD")));
					if (bSimple)
					{
						return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_WARFARE_SIMPLE"), episodeTemplet3.GetEpisodeTitle(), stageTempletRequire.ActId, stageTempletRequire.m_StageUINum, text);
					}
					return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_WARFARE_DEFAULT"), episodeTemplet3.GetEpisodeTitle(), stageTempletRequire.ActId, stageTempletRequire.m_StageUINum, text);
				}
				break;
			}
			case EPISODE_CATEGORY.EC_DAILY:
			{
				NKMEpisodeTempletV2 episodeTemplet4 = stageTempletRequire.EpisodeTemplet;
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_DUNGEON_EC_DAILY"), episodeTemplet4.GetEpisodeName(), stageTempletRequire.m_StageUINum);
			}
			case EPISODE_CATEGORY.EC_COUNTERCASE:
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(stageTempletRequire.ActId);
				if (unitTempletBase != null)
				{
					return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_DUNGEON_EC_COUNTERCASE"), unitTempletBase.GetUnitName(), stageTempletRequire.m_StageUINum);
				}
				break;
			}
			case EPISODE_CATEGORY.EC_SIDESTORY:
			{
				NKMEpisodeTempletV2 episodeTemplet5 = stageTempletRequire.EpisodeTemplet;
				NKMDungeonTempletBase dungeonTempletBase = stageTempletRequire.DungeonTempletBase;
				if (dungeonTempletBase != null && dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
				{
					return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_CUTSCENE_AND_BUY_EC_SIDESTORY"), episodeTemplet5.GetEpisodeName(), stageTempletRequire.GetDungeonName());
				}
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_DUNGEON_EC_SIDESTORY"), episodeTemplet5.GetEpisodeName(), stageTempletRequire.ActId, stageTempletRequire.m_StageUINum);
			}
			case EPISODE_CATEGORY.EC_FIELD:
			{
				NKMEpisodeTempletV2 episodeTemplet2 = stageTempletRequire.EpisodeTemplet;
				if (episodeTemplet2 != null)
				{
					if (bSimple)
					{
						return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_FIELD_SIMPLE"), episodeTemplet2.GetEpisodeTitle(), stageTempletRequire.ActId, stageTempletRequire.m_StageUINum);
					}
					return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_FIELD_DEFAULT"), episodeTemplet2.GetEpisodeTitle(), stageTempletRequire.ActId, stageTempletRequire.m_StageUINum);
				}
				break;
			}
			case EPISODE_CATEGORY.EC_EVENT:
			case EPISODE_CATEGORY.EC_SEASONAL:
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_START_DATETIME"), stageTempletRequire.ActId);
			case EPISODE_CATEGORY.EC_CHALLENGE:
			{
				NKMEpisodeTempletV2 episodeTemplet = stageTempletRequire.EpisodeTemplet;
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_DUNGEON_EC_CHALLENGE"), episodeTemplet.GetEpisodeTitle(), stageTempletRequire.ActId, stageTempletRequire.m_StageUINum);
			}
			}
		}
		return "";
	}

	public static string GetUnlockConditionRequireDesc(STAGE_UNLOCK_REQ_TYPE eReqType, int reqValue, string reqValueStr, DateTime reqDateTime, bool bSimple = false)
	{
		switch (eReqType)
		{
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE:
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(reqValue);
			if (nKMWarfareTemplet != null)
			{
				return GetStageUnlockConditionRequireDesc(NKMEpisodeMgr.FindStageTempletByBattleStrID(nKMWarfareTemplet.m_WarfareStrID), bSimple);
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE:
		{
			NKMPhaseTemplet nKMPhaseTemplet = NKMPhaseTemplet.Find(reqValue);
			if (nKMPhaseTemplet != null)
			{
				return GetStageUnlockConditionRequireDesc(nKMPhaseTemplet.StageTemplet, bSimple);
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_CITY_COUNT:
			if (bSimple)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CITY_COUNT_SIMPLE"), reqValue);
			}
			return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CITY_COUNT_DEFAULT"), reqValue);
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_GET:
		{
			NKMUnitTempletBase unitTempletBase5 = NKMUnitManager.GetUnitTempletBase(reqValue);
			if (unitTempletBase5 != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_UNIT_GET"), unitTempletBase5.GetUnitName());
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_20:
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(reqValue);
			if (unitTempletBase2 != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_UNIT_LEVEL_20"), unitTempletBase2.GetUnitName());
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_25:
		{
			NKMUnitTempletBase unitTempletBase9 = NKMUnitManager.GetUnitTempletBase(reqValue);
			if (unitTempletBase9 != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_UNIT_LEVEL_25"), unitTempletBase9.GetUnitName());
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_50:
		{
			NKMUnitTempletBase unitTempletBase6 = NKMUnitManager.GetUnitTempletBase(reqValue);
			if (unitTempletBase6 != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_UNIT_LEVEL_50"), unitTempletBase6.GetUnitName());
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_80:
		{
			NKMUnitTempletBase unitTempletBase4 = NKMUnitManager.GetUnitTempletBase(reqValue);
			if (unitTempletBase4 != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_UNIT_LEVEL_80"), unitTempletBase4.GetUnitName());
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LIMIT_GARDE_1:
		{
			NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase(reqValue);
			if (unitTempletBase3 != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_UNIT_LIMIT_GARDE_1"), unitTempletBase3.GetUnitName());
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LIMIT_GARDE_2:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(reqValue);
			if (unitTempletBase != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_UNIT_LIMIT_GARDE_2"), unitTempletBase.GetUnitName());
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LIMIT_GARDE_3:
		{
			NKMUnitTempletBase unitTempletBase10 = NKMUnitManager.GetUnitTempletBase(reqValue);
			if (unitTempletBase10 != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_UNIT_LIMIT_GARDE_3"), unitTempletBase10.GetUnitName());
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_DEVOTION:
		{
			NKMUnitTempletBase unitTempletBase8 = NKMUnitManager.GetUnitTempletBase(reqValue);
			if (unitTempletBase8 != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_UNIT_DEVOTION"), unitTempletBase8.GetUnitName());
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_100:
		{
			NKMUnitTempletBase unitTempletBase7 = NKMUnitManager.GetUnitTempletBase(reqValue);
			if (unitTempletBase7 != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_UNIT_LEVEL_100"), unitTempletBase7.GetUnitName());
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_PLAYER_LEVEL:
			if (bSimple)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_PLAYER_LEVEL_SIMPLE"), reqValue);
			}
			return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_PLAYER_LEVEL_DEFAULT"), reqValue);
		case STAGE_UNLOCK_REQ_TYPE.SURT_GUILD_LEVEL:
			if (bSimple)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_GUILD_LEVEL_SIMPLE"), reqValue);
			}
			return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_GUILD_LEVEL_DEFAULT"), reqValue);
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON:
		{
			string stageUnlockConditionRequireDesc = GetStageUnlockConditionRequireDesc(NKMEpisodeMgr.FindStageTempletByBattleStrID(NKMDungeonManager.GetDungeonStrID(reqValue)), bSimple);
			if (!string.IsNullOrEmpty(stageUnlockConditionRequireDesc))
			{
				return stageUnlockConditionRequireDesc;
			}
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(reqValue);
			if (dungeonTempletBase != null)
			{
				if (dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
				{
					return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_DUNGEON_NDT_CUTSCENE"), dungeonTempletBase.GetDungeonName());
				}
				if (bSimple)
				{
					return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_DUNGEON_ELSE_SIMPLE"), dungeonTempletBase.GetDungeonName());
				}
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_DUNGEON_ELSE_DEFAULT"), dungeonTempletBase.GetDungeonName());
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DIVE:
			Debug.LogError("UnExpected!");
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_STAGE:
			return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_STAGE"));
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_TRIM:
		{
			NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(reqValue);
			if (!int.TryParse(reqValueStr, out var result2))
			{
				result2 = 1;
			}
			return NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_TRIM", NKCStringTable.GetString(nKMTrimTemplet.TirmGroupName), result2);
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_START_DATETIME:
			if (reqDateTime <= DateTime.MinValue)
			{
				Debug.LogError("STAGE_UNLOCK_REQ_TYPE.SURT_START_DATETIME 사용 시 reqValueStr 에 시간이 들어가야함 (2020-07-01 04:00)");
				break;
			}
			if (reqDateTime <= ServiceTime.Recent)
			{
				Debug.LogError("잠겨있지 않으므로 여기 들어오면 안됨");
				break;
			}
			return NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_ALWAYS_LOCKED");
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE_START_DATETIME:
			if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_START_DATETIME, reqValue, reqDateTime)))
			{
				return GetUnlockConditionRequireDesc(STAGE_UNLOCK_REQ_TYPE.SURT_START_DATETIME, reqValue, reqValueStr, reqDateTime, bSimple);
			}
			if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE, reqValue)))
			{
				return GetUnlockConditionRequireDesc(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE, reqValue, reqValueStr, reqDateTime, bSimple);
			}
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON_START_DATETIME:
			if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_START_DATETIME, reqValue, reqDateTime)))
			{
				return GetUnlockConditionRequireDesc(STAGE_UNLOCK_REQ_TYPE.SURT_START_DATETIME, reqValue, reqValueStr, reqDateTime, bSimple);
			}
			if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON, reqValue)))
			{
				return GetUnlockConditionRequireDesc(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON, reqValue, reqValueStr, reqDateTime, bSimple);
			}
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_INTERVAL:
			if (NKCSynchronizedTime.IsEventTime(reqValueStr))
			{
				Debug.LogError("잠겨있지 않으므로 여기 들어오면 안됨");
				break;
			}
			return NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_ALWAYS_LOCKED");
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON_INTERVAL:
			if (!NKCSynchronizedTime.IsEventTime(reqValueStr))
			{
				return GetUnlockConditionRequireDesc(STAGE_UNLOCK_REQ_TYPE.SURT_INTERVAL, reqValue, reqValueStr, reqDateTime, bSimple);
			}
			if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE, reqValue)))
			{
				return GetUnlockConditionRequireDesc(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON, reqValue, reqValueStr, reqDateTime, bSimple);
			}
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE_INTERVAL:
			if (!NKCSynchronizedTime.IsEventTime(reqValueStr))
			{
				return GetUnlockConditionRequireDesc(STAGE_UNLOCK_REQ_TYPE.SURT_INTERVAL, reqValue, reqValueStr, reqDateTime, bSimple);
			}
			if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON, reqValue)))
			{
				return GetUnlockConditionRequireDesc(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE, reqValue, reqValueStr, reqDateTime, bSimple);
			}
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE_INTERVAL:
			if (!NKCSynchronizedTime.IsEventTime(reqValueStr))
			{
				return GetUnlockConditionRequireDesc(STAGE_UNLOCK_REQ_TYPE.SURT_INTERVAL, reqValue, reqValueStr, reqDateTime, bSimple);
			}
			if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE, reqValue)))
			{
				return GetUnlockConditionRequireDesc(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE, reqValue, reqValueStr, reqDateTime, bSimple);
			}
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_NEWBIE_USER:
		{
			if (int.TryParse(reqValueStr, out var result))
			{
				DateTime registerTime = NKCScenManager.CurrentUserData().m_NKMUserDateData.m_RegisterTime;
				DateTime dateTime = registerTime.AddDays(result);
				if (dateTime > NKCSynchronizedTime.GetServerUTCTime())
				{
					return NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_NEWBIE_USER_BEFORE", GetRemainTimeString(dateTime, 1));
				}
			}
			if (NKCScenManager.CurrentUserData().m_NKMUserDateData.m_RegisterTime.AddDays(reqValue) > NKCSynchronizedTime.GetServerUTCTime())
			{
				return NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_NEWBIE_USER_AFTER", reqValue);
			}
			Debug.LogError("잠겨있지 않으므로 여기 들어오면 안됨");
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_LAST_STAGE:
		{
			NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(reqValue);
			StringBuilder stringBuilder = new StringBuilder();
			switch (nKMStageTempletV.EpisodeCategory)
			{
			case EPISODE_CATEGORY.EC_MAINSTREAM:
				stringBuilder.Append(string.Format("{0} {1} {2}{3}", NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_MAINSTREAM"), nKMStageTempletV.EpisodeTemplet.GetEpisodeTitle(), NKCStringTable.GetString("SI_DP_CONTENTS_UNLOCK_ACT_DISPLAY"), nKMStageTempletV.ActId));
				break;
			case EPISODE_CATEGORY.EC_DAILY:
				stringBuilder.Append(NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_DAILY") + " " + nKMStageTempletV.EpisodeTemplet.GetEpisodeName());
				break;
			case EPISODE_CATEGORY.EC_SUPPLY:
				stringBuilder.Append(NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_SUPPLY") + " " + nKMStageTempletV.EpisodeTemplet.GetEpisodeName());
				break;
			case EPISODE_CATEGORY.EC_FIELD:
				stringBuilder.Append(string.Format("{0} {1} {2}{3}", NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_FIELD"), nKMStageTempletV.EpisodeTemplet.GetEpisodeTitle(), NKCStringTable.GetString("SI_DP_CONTENTS_UNLOCK_ACT_DISPLAY"), nKMStageTempletV.ActId));
				break;
			case EPISODE_CATEGORY.EC_SIDESTORY:
				stringBuilder.Append(string.Format("{0} {1} {2}{3}", NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_SIDESTORY"), nKMStageTempletV.EpisodeTemplet.GetEpisodeTitle(), NKCStringTable.GetString("SI_DP_CONTENTS_UNLOCK_ACT_DISPLAY"), nKMStageTempletV.ActId));
				break;
			case EPISODE_CATEGORY.EC_EVENT:
				stringBuilder.Append(string.Format("{0} {1} {2}{3}", NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_EVENT"), nKMStageTempletV.EpisodeTemplet.GetEpisodeTitle(), NKCStringTable.GetString("SI_DP_CONTENTS_UNLOCK_ACT_DISPLAY"), nKMStageTempletV.ActId));
				break;
			case EPISODE_CATEGORY.EC_SEASONAL:
				stringBuilder.Append(string.Format("{0} {1} {2}{3}", NKCStringTable.GetString("SI_OPERATION_MENU_TEXT_SEASONAL"), nKMStageTempletV.EpisodeTemplet.GetEpisodeTitle(), NKCStringTable.GetString("SI_DP_CONTENTS_UNLOCK_ACT_DISPLAY"), nKMStageTempletV.ActId));
				break;
			case EPISODE_CATEGORY.EC_COUNTERCASE:
				stringBuilder.Append(NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_COUNTERCASE") + " " + nKMStageTempletV.EpisodeTemplet.GetEpisodeName());
				break;
			case EPISODE_CATEGORY.EC_CHALLENGE:
				stringBuilder.Append(string.Format("{0} {1} {2}{3}", NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_CHALLENGE"), nKMStageTempletV.EpisodeTemplet.GetEpisodeTitle(), NKCStringTable.GetString("SI_DP_CONTENTS_UNLOCK_ACT_DISPLAY"), nKMStageTempletV.ActId));
				break;
			default:
				return NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_ALWAYS_LOCKED");
			}
			if (nKMStageTempletV.m_Difficulty == EPISODE_DIFFICULTY.HARD)
			{
				stringBuilder.Append(" [" + NKCStringTable.GetString("SI_OPERATION_DIFF_HARD1") + "]");
			}
			return string.Format(NKCStringTable.GetString("SI_DP_CONTENTS_UNLOCK_ALL_CLEAR"), stringBuilder.ToString());
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_MISSION_TAB_UNLOCKED:
		{
			NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(reqValue);
			if (missionTabTemplet != null)
			{
				return GetUnlockConditionRequireDesc(missionTabTemplet.m_UnlockInfo, bLockedOnly: true, bSimple);
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_DIVE_HISTORY_CLEARED:
		{
			NKMDiveTemplet nKMDiveTemplet = NKMDiveTemplet.Find(reqValue);
			if (nKMDiveTemplet != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_DIVE_HISTORY_CLEARED"), nKMDiveTemplet.IndexID);
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_LOCKED:
		case STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_HIDDEN:
			return NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_ALWAYS_LOCKED");
		case STAGE_UNLOCK_REQ_TYPE.SURT_OPEN_ROOM:
		{
			NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(reqValue);
			string arg = NKCStringTable.GetString(nKMOfficeRoomTemplet.SectionTemplet.SectionName) + " " + NKCStringTable.GetString(nKMOfficeRoomTemplet.Name);
			return string.Format(NKCStringTable.GetString("SI_DP_OFFICE_ROOM_UNLOCK_WARNING"), arg);
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_MISSION_CLEAR:
		{
			NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(reqValue);
			if (missionTemplet != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_SURT_MISSION_CLEAR_DEFAULT"), missionTemplet.GetTitle());
			}
			break;
		}
		}
		Debug.LogError($"UnlockCondition string case not found for :{eReqType.ToString()}. using default string.");
		return NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_ALWAYS_LOCKED");
	}

	public static string GetEpisodeTitle(NKMEpisodeTempletV2 cNKMEpisodeTemplet, NKMStageTempletV2 stageTemplet)
	{
		if (cNKMEpisodeTemplet != null && stageTemplet != null)
		{
			return cNKMEpisodeTemplet.GetEpisodeTitle();
		}
		return "";
	}

	public static string GetEpisodeNumber(NKMEpisodeTempletV2 cNKMEpisodeTemplet, NKMStageTempletV2 stageTemplet)
	{
		if (cNKMEpisodeTemplet != null && stageTemplet != null)
		{
			if (stageTemplet.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE)
			{
				return string.Format(GET_STRING_EP_TRAINING_NUMBER, stageTemplet.m_StageUINum);
			}
			bool flag = false;
			if (stageTemplet.m_STAGE_TYPE == STAGE_TYPE.ST_DUNGEON)
			{
				NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(stageTemplet.m_StageBattleStrID);
				if (dungeonTempletBase != null && dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
				{
					flag = true;
				}
			}
			if (flag)
			{
				return string.Format(GET_STRING_EP_CUTSCEN_NUMBER, stageTemplet.m_StageUINum);
			}
			return $"{stageTemplet.ActId}-{stageTemplet.m_StageUINum}";
		}
		return "";
	}

	public static string GetWFMissionText(WARFARE_GAME_MISSION_TYPE missionType, int missionValue)
	{
		StringBuilder stringBuilder = new StringBuilder();
		switch (missionType)
		{
		case WARFARE_GAME_MISSION_TYPE.WFMT_CLEAR:
			stringBuilder.Append(NKCStringTable.GetString("SI_DP_WF_MISSION_TEXT_WFMT_CLEAR"));
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_ALLKILL:
			stringBuilder.Append(NKCStringTable.GetString("SI_DP_WF_MISSION_TEXT_WFMT_ALLKILL"));
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_PHASE:
			stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_DP_WF_MISSION_TEXT_WFMT_PHASE"), missionValue));
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_NO_SHIPWRECK:
			stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_DP_WF_MISSION_TEXT_WFMT_NO_SHIPWRECK")));
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_KILL:
			stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_DP_WF_MISSION_TEXT_WFMT_KILL"), missionValue));
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_FIRST_ATTACK:
			stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_DP_WF_MISSION_TEXT_WFMT_FIRST_ATTACK"), missionValue));
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_ASSIST:
			stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_DP_WF_MISSION_TEXT_WFMT_ASSIST"), missionValue));
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_CONTAINER:
			stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_DP_WF_MISSION_TEXT_WFMT_CONTAINER"), missionValue));
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_NOSUPPLY_WIN:
			stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_DP_WF_MISSION_TEXT_WFMT_NOSUPPLY_WIN"), missionValue));
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_NOSUPPLY_ALLKILL:
			stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_DP_WF_MISSION_TEXT_WFMT_NOSUPPLY_ALLKILL"), missionValue));
			break;
		default:
			return "";
		}
		return stringBuilder.ToString();
	}

	public static string GetWFWinContionText(WARFARE_GAME_CONDITION winCondition)
	{
		return winCondition switch
		{
			WARFARE_GAME_CONDITION.WFC_KILL_BOSS => NKCStringTable.GetString("SI_DP_WF_WIN_CONDITION_TEXT_WFC_KILL_BOSS"), 
			WARFARE_GAME_CONDITION.WFC_KILL_ALL => NKCStringTable.GetString("SI_DP_WF_WIN_CONDITION_TEXT_WFC_KILL_ALL"), 
			WARFARE_GAME_CONDITION.WFC_KILL_TARGET => NKCStringTable.GetString("SI_DP_WF_WIN_CONDITION_TEXT_WFC_KILL_TARGET"), 
			WARFARE_GAME_CONDITION.WFC_TILE_ENTER => NKCStringTable.GetString("SI_DP_WF_WIN_CONDITION_TEXT_WFC_TILE_ENTER"), 
			WARFARE_GAME_CONDITION.WFC_PHASE_TILE_HOLD => NKCStringTable.GetString("SI_DP_WF_WIN_CONDITION_TEXT_WFC_PHASE_TILE_HOLD"), 
			_ => "", 
		};
	}

	public static string GetWFLoseConditionText(WARFARE_GAME_CONDITION loseCondition)
	{
		return loseCondition switch
		{
			WARFARE_GAME_CONDITION.WFC_KILL_ALL => NKCStringTable.GetString("SI_DP_WF_LOSE_CONDITION_TEXT_WFC_KILL_ALL"), 
			WARFARE_GAME_CONDITION.WFC_PHASE => NKCStringTable.GetString("SI_DP_WF_LOSE_CONDITION_TEXT_WFC_PHASE"), 
			WARFARE_GAME_CONDITION.WFC_KILL_COUNT => NKCStringTable.GetString("SI_DP_WF_LOSE_CONDITION_TEXT_WFC_KILL_COUNT"), 
			WARFARE_GAME_CONDITION.WFC_KILL_BOSS => NKCStringTable.GetString("SI_DP_WF_LOSE_CONDITION_TEXT_WFC_KILL_BOSS"), 
			WARFARE_GAME_CONDITION.WFC_TILE_ENTER => NKCStringTable.GetString("SI_DP_WF_LOSE_CONDITION_TEXT_WFC_TILE_ENTER"), 
			_ => "", 
		};
	}

	public static string GetWFEpisodeNumber(NKMWarfareTemplet cNKMWarfareTemplet)
	{
		if (cNKMWarfareTemplet != null)
		{
			NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(cNKMWarfareTemplet.m_WarfareStrID);
			if (nKMStageTempletV != null)
			{
				NKMEpisodeTempletV2 episodeTemplet = nKMStageTempletV.EpisodeTemplet;
				if (episodeTemplet != null)
				{
					string episodeNumber = GetEpisodeNumber(episodeTemplet, nKMStageTempletV);
					if (!string.IsNullOrEmpty(episodeNumber))
					{
						m_sStringBuilder.Clear();
						m_sStringBuilder.Append(episodeTemplet.GetEpisodeTitle());
						m_sStringBuilder.Append(" ");
						m_sStringBuilder.Append(episodeNumber);
						return m_sStringBuilder.ToString();
					}
				}
			}
		}
		return "";
	}

	public static string GetWFMissionTextWithProgress(WarfareGameData warfareGameData, WARFARE_GAME_MISSION_TYPE missionType, int missionValue)
	{
		if (missionType == WARFARE_GAME_MISSION_TYPE.WFMT_NONE)
		{
			return string.Empty;
		}
		string wFMissionText = GetWFMissionText(missionType, missionValue);
		int currentMissionValue = NKCWarfareManager.GetCurrentMissionValue(warfareGameData, missionType);
		return $"{wFMissionText} ({currentMissionValue}/{missionValue})";
	}

	public static string GetCurrentProgress(int turnCount, int value)
	{
		return $" ({turnCount}/{value})";
	}

	public static string GetPlayingWarfare(string episodeTitle, int stageActID, int _StageUINum)
	{
		return string.Format(NKCStringTable.GetString("SI_DP_PLAYING_WARFARE"), episodeTitle, stageActID, _StageUINum);
	}

	public static string GetDiveArtifactTotalViewDesc(List<int> lstArtifact)
	{
		m_sStringBuilder.Clear();
		if (lstArtifact.Count <= 0)
		{
			return m_sStringBuilder.ToString();
		}
		List<NKMDiveArtifactTemplet> list = new List<NKMDiveArtifactTemplet>();
		for (int i = 0; i < lstArtifact.Count; i++)
		{
			NKMDiveArtifactTemplet nKMDiveArtifactTemplet = NKMDiveArtifactTemplet.Find(lstArtifact[i]);
			if (nKMDiveArtifactTemplet != null)
			{
				list.Add(nKMDiveArtifactTemplet);
			}
		}
		list.Sort(new CompNKMDiveArtifactTemplet());
		int num = Enum.GetNames(typeof(NKM_DIVE_ARTIFACT_CATEGORY)).Length;
		List<bool> list2 = new List<bool>();
		for (int j = 0; j < num; j++)
		{
			list2.Add(item: false);
		}
		int num2 = -1;
		for (int k = 0; k < list.Count; k++)
		{
			NKMDiveArtifactTemplet nKMDiveArtifactTemplet2 = list[k];
			if (nKMDiveArtifactTemplet2 == null || nKMDiveArtifactTemplet2.BattleConditionID <= 0)
			{
				continue;
			}
			int category = (int)nKMDiveArtifactTemplet2.Category;
			if (num2 != -1 && num2 != category)
			{
				m_sStringBuilder.AppendLine();
			}
			num2 = category;
			if (!list2[category])
			{
				list2[category] = true;
				switch (category)
				{
				case 0:
					m_sStringBuilder.AppendLine(NKCStringTable.GetString("SI_DP_DIVE_ARTIFACT_TOTAL_VIEW_DESC_CATEGORY_CHECK_NDAC_ALL"));
					break;
				case 1:
					m_sStringBuilder.AppendLine(NKCStringTable.GetString("SI_DP_DIVE_ARTIFACT_TOTAL_VIEW_DESC_CATEGORY_CHECK_NDAC_COUNTER"));
					break;
				case 2:
					m_sStringBuilder.AppendLine(NKCStringTable.GetString("SI_DP_DIVE_ARTIFACT_TOTAL_VIEW_DESC_CATEGORY_CHECK_NDAC_SOLDIER"));
					break;
				case 3:
					m_sStringBuilder.AppendLine(NKCStringTable.GetString("SI_DP_DIVE_ARTIFACT_TOTAL_VIEW_DESC_CATEGORY_CHECK_NDAC_MECHANIC"));
					break;
				case 4:
					m_sStringBuilder.AppendLine(NKCStringTable.GetString("SI_DP_DIVE_ARTIFACT_TOTAL_VIEW_DESC_CATEGORY_CHECK_NDAC_ETC"));
					break;
				}
			}
			m_sStringBuilder.AppendLine("·" + nKMDiveArtifactTemplet2.ArtifactMiscDesc_2_Translated);
		}
		return m_sStringBuilder.ToString();
	}

	public static string GetGuildArtifactTotalViewDesc(List<int> lstArtifact)
	{
		m_sStringBuilder.Clear();
		if (lstArtifact.Count <= 0)
		{
			return m_sStringBuilder.ToString();
		}
		List<GuildDungeonArtifactTemplet> list = new List<GuildDungeonArtifactTemplet>();
		for (int i = 0; i < lstArtifact.Count; i++)
		{
			GuildDungeonArtifactTemplet artifactTemplet = GuildDungeonTempletManager.GetArtifactTemplet(lstArtifact[i]);
			if (artifactTemplet != null)
			{
				list.Add(artifactTemplet);
			}
		}
		list.Sort(new CompGuildDungeonArtifactTemplet());
		int num = Enum.GetNames(typeof(NKM_DIVE_ARTIFACT_CATEGORY)).Length;
		List<bool> list2 = new List<bool>();
		for (int j = 0; j < num; j++)
		{
			list2.Add(item: false);
		}
		int num2 = -1;
		for (int k = 0; k < list.Count; k++)
		{
			GuildDungeonArtifactTemplet guildDungeonArtifactTemplet = list[k];
			if (guildDungeonArtifactTemplet == null)
			{
				continue;
			}
			int category = (int)guildDungeonArtifactTemplet.GetCategory();
			if (num2 != -1 && num2 != category)
			{
				m_sStringBuilder.AppendLine();
			}
			num2 = category;
			if (!list2[category])
			{
				list2[category] = true;
				switch (category)
				{
				case 0:
					m_sStringBuilder.AppendLine(NKCStringTable.GetString("SI_DP_DIVE_ARTIFACT_TOTAL_VIEW_DESC_CATEGORY_CHECK_NDAC_ALL"));
					break;
				case 1:
					m_sStringBuilder.AppendLine(NKCStringTable.GetString("SI_DP_DIVE_ARTIFACT_TOTAL_VIEW_DESC_CATEGORY_CHECK_NDAC_COUNTER"));
					break;
				case 2:
					m_sStringBuilder.AppendLine(NKCStringTable.GetString("SI_DP_DIVE_ARTIFACT_TOTAL_VIEW_DESC_CATEGORY_CHECK_NDAC_SOLDIER"));
					break;
				case 3:
					m_sStringBuilder.AppendLine(NKCStringTable.GetString("SI_DP_DIVE_ARTIFACT_TOTAL_VIEW_DESC_CATEGORY_CHECK_NDAC_MECHANIC"));
					break;
				case 4:
					m_sStringBuilder.AppendLine(NKCStringTable.GetString("SI_DP_DIVE_ARTIFACT_TOTAL_VIEW_DESC_CATEGORY_CHECK_NDAC_ETC"));
					break;
				}
			}
			m_sStringBuilder.AppendLine("·" + guildDungeonArtifactTemplet.GetDescShort());
		}
		return m_sStringBuilder.ToString();
	}

	public static void GetDiveEventText(NKM_DIVE_EVENT_TYPE type, out string title, out string subTitle)
	{
		title = "";
		subTitle = "";
		switch (type)
		{
		case NKM_DIVE_EVENT_TYPE.NDET_ITEM:
			title = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_ITEM_TITLE");
			subTitle = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_ITEM_SUBTITLE");
			break;
		case NKM_DIVE_EVENT_TYPE.NDET_UNIT:
			title = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_UNIT_TITLE");
			subTitle = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_UNIT_SUBTITLE");
			break;
		case NKM_DIVE_EVENT_TYPE.NDET_REPAIR:
			title = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_REPAIR_TITLE");
			subTitle = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_REPAIR_SUBTITLE");
			break;
		case NKM_DIVE_EVENT_TYPE.NDET_SUPPLY:
			title = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_SUPPLY_TITLE");
			subTitle = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_SUPPLY_SUBTITLE");
			break;
		case NKM_DIVE_EVENT_TYPE.NDET_LOSTSHIP_ITEM:
			title = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_LOSTSHIP_ITEM_TITLE");
			subTitle = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_LOSTSHIP_ITEM_SUBTITLE");
			break;
		case NKM_DIVE_EVENT_TYPE.NDET_LOSTSHIP_UNIT:
			title = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_LOSTSHIP_UNIT_TITLE");
			subTitle = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_LOSTSHIP_UNIT_SUBTITLE");
			break;
		case NKM_DIVE_EVENT_TYPE.NDET_LOSTSHIP_REPAIR:
			title = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_LOSTSHIP_REPAIR_TITLE");
			subTitle = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_LOSTSHIP_REPAIR_SUBTITLE");
			break;
		case NKM_DIVE_EVENT_TYPE.NDET_LOSTSHIP_SUPPLY:
			title = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_LOSTSHIP_SUPPLY_TITLE");
			subTitle = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_LOSTSHIP_SUPPLY_SUBTITLE");
			break;
		case NKM_DIVE_EVENT_TYPE.NDET_BEACON_DUNGEON:
			title = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_BEACON_DUNGEON_TITLE");
			subTitle = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_BEACON_DUNGEON_SUBTITLE");
			break;
		case NKM_DIVE_EVENT_TYPE.NDET_BEACON_BLANK:
			title = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_BEACON_BLANK_TITLE");
			subTitle = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_BEACON_BLANK_SUBTITLE");
			break;
		case NKM_DIVE_EVENT_TYPE.NDET_BEACON_ITEM:
			title = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_BEACON_ITEM_TITLE");
			subTitle = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_BEACON_ITEM_SUBTITLE");
			break;
		case NKM_DIVE_EVENT_TYPE.NDET_BEACON_UNIT:
			title = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_BEACON_UNIT_TITLE");
			subTitle = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_BEACON_UNIT_SUBTITLE");
			break;
		case NKM_DIVE_EVENT_TYPE.NDET_BEACON_STORM:
			title = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_BEACON_STORM_TITLE");
			subTitle = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_BEACON_STORM_SUBTITLE");
			break;
		case NKM_DIVE_EVENT_TYPE.NDET_ARTIFACT:
			title = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_BLANK_TITLE");
			subTitle = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_BLANK_SUBTITLE");
			break;
		case NKM_DIVE_EVENT_TYPE.NDET_DUNGEON:
		case NKM_DIVE_EVENT_TYPE.NDET_DUNGEON_BOSS:
		case NKM_DIVE_EVENT_TYPE.NDET_BLANK:
		case NKM_DIVE_EVENT_TYPE.NDET_LOSTSHIP_RANDOM:
		case NKM_DIVE_EVENT_TYPE.NDET_BEACON_RANDOM:
			break;
		}
	}

	internal static string GetStatusName(NKM_UNIT_STATUS_EFFECT status, bool addMark = false)
	{
		NKMUnitStatusTemplet nKMUnitStatusTemplet = NKMUnitStatusTemplet.Find(status);
		if (nKMUnitStatusTemplet != null)
		{
			if (addMark)
			{
				return NKCStringTable.GetString(nKMUnitStatusTemplet.m_StatusStrID) + GET_STRING_INGAME_NUSE_ONLY_NO_DISPEL_CODE;
			}
			return NKCStringTable.GetString(nKMUnitStatusTemplet.m_StatusStrID);
		}
		return "";
	}

	internal static string GetStatusImmuneName(NKM_UNIT_STATUS_EFFECT status)
	{
		string statusName = GetStatusName(status);
		if (string.IsNullOrEmpty(statusName))
		{
			return "";
		}
		return NKCStringTable.GetString("SI_BATTLE_IMMUNE_COMMON", statusName);
	}

	public static string GetDGMissionText(DUNGEON_GAME_MISSION_TYPE missionType, int missionValue)
	{
		switch (missionType)
		{
		case DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR:
			return NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_CLEAR");
		case DUNGEON_GAME_MISSION_TYPE.DGMT_TIME:
			if (missionValue % 60 > 0)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_TIME_HAVE_SEC"), missionValue / 60, missionValue % 60);
			}
			return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_TIME_ELSE"), missionValue / 60);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_COST:
			return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_COST"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_RESPAWN:
			return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_RESPAWN"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_SHIP_HP_DAMAGE:
			return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_SHIP_HP_DAMAGE"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_SOLDIER:
			return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_DECKCOUNT_SOLDIER"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_MECHANIC:
			return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_DECKCOUNT_MECHANIC"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_COUNTER:
			return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_DECKCOUNT_COUNTER"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_DEFENDER:
			return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_DECKCOUNT_DEFENDER"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_STRIKER:
			return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_DECKCOUNT_STRIKER"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_RANGER:
			return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_DECKCOUNT_RANGER"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_SNIPER:
			return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_DECKCOUNT_SNIPER"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_TOWER:
			return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_DECKCOUNT_TOWER"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_SIEGE:
			return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_DECKCOUNT_SIEGE"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_SUPPORTER:
			return string.Format(NKCStringTable.GetString("SI_DP_DG_MISSION_TEXT_GAME_MISSION_TYPE_DGMT_DECKCOUNT_SUPPORTER"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_WAVE_CLEAR_LEVEL:
			return string.Format(NKCStringTable.GetString("SI_PF_DEFENCE_BANNER_MEDAL"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_TEAM_A_KILL_COUNT:
			return string.Format(NKCStringTable.GetString("SI_PF_DEFENCE_BANNER_MEDAL"), missionValue);
		case DUNGEON_GAME_MISSION_TYPE.DGMT_TEAM_B_KILL_COUNT:
			return string.Format(NKCStringTable.GetString("SI_PF_DEFENCE_BANNER_MEDAL"), missionValue);
		default:
			return "";
		}
	}

	public static string GetDailyDungeonLVDesc(int stageIndex)
	{
		return string.Format(NKCStringTable.GetString("SI_DP_DAILY_DUNGEON_LV_DESC"), stageIndex);
	}

	public static string GetDGMissionTextWithProgress(NKMGame game, DUNGEON_GAME_MISSION_TYPE missionType, int missionValue)
	{
		if (missionType == DUNGEON_GAME_MISSION_TYPE.DGMT_NONE)
		{
			return string.Empty;
		}
		string dGMissionText = GetDGMissionText(missionType, missionValue);
		int currentMissionValue = NKMDungeonManager.GetCurrentMissionValue(game, missionType);
		if (missionType == DUNGEON_GAME_MISSION_TYPE.DGMT_SHIP_HP_DAMAGE)
		{
			return $"{dGMissionText} ({currentMissionValue}/{missionValue} %)";
		}
		return $"{dGMissionText} ({currentMissionValue}/{missionValue})";
	}

	public static string GetRemainTimeStringExWithoutEnd(DateTime endTime)
	{
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(endTime);
		if (timeLeft.TotalDays >= 1.0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_DAYS_AND_HOURS"), timeLeft.Days, timeLeft.Hours);
		}
		if (timeLeft.TotalHours >= 1.0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_HOURS_AND_MINUTES"), timeLeft.Hours, timeLeft.Minutes);
		}
		if (timeLeft.TotalMinutes >= 1.0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_MINUTES"), timeLeft.Minutes);
		}
		if (timeLeft.TotalSeconds >= 1.0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_SECONDS"), timeLeft.Seconds);
		}
		return NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_END_SOON");
	}

	public static string GetDayString(DayOfWeek dayOfWeek)
	{
		return dayOfWeek switch
		{
			DayOfWeek.Monday => NKCStringTable.GetString("SI_DP_DAY_STRING_MONDAY"), 
			DayOfWeek.Tuesday => NKCStringTable.GetString("SI_DP_DAY_STRING_TUESDAY"), 
			DayOfWeek.Wednesday => NKCStringTable.GetString("SI_DP_DAY_STRING_WEDNESDAY"), 
			DayOfWeek.Thursday => NKCStringTable.GetString("SI_DP_DAY_STRING_THURSDAY"), 
			DayOfWeek.Friday => NKCStringTable.GetString("SI_DP_DAY_STRING_FRIDAY"), 
			DayOfWeek.Saturday => NKCStringTable.GetString("SI_DP_DAY_STRING_SATURDAY"), 
			DayOfWeek.Sunday => NKCStringTable.GetString("SI_DP_DAY_STRING_SUNDAY"), 
			_ => "", 
		};
	}

	public static string GetTimeString(DateTime endUTCTime, bool bSeconds = true)
	{
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(endUTCTime);
		if (timeLeft.TotalSeconds <= 0.0)
		{
			return " - ";
		}
		if (timeLeft.Days > 0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_DAYS"), timeLeft.Days);
		}
		if (timeLeft.Hours > 0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_HOURS"), timeLeft.Hours);
		}
		if (timeLeft.Minutes > 0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_MINUTES"), timeLeft.Minutes);
		}
		if (bSeconds)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_SECONDS"), timeLeft.Seconds);
		}
		return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_MINUTES"), 1);
	}

	public static string GetRemainTimeStringOneParam(DateTime endUTCTime)
	{
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(endUTCTime);
		if (timeLeft.TotalDays >= 1.0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_DAYS"), timeLeft.Days);
		}
		if (timeLeft.TotalHours >= 1.0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_HOURS"), timeLeft.Hours);
		}
		if (timeLeft.TotalMinutes >= 1.0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_MINUTES"), timeLeft.Minutes);
		}
		if (timeLeft.TotalSeconds >= 1.0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_SECONDS"), timeLeft.Seconds);
		}
		if (timeLeft.TotalSeconds > 0.0)
		{
			return NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_END_SOON");
		}
		return NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_END");
	}

	public static string GetRemainTimeStringEx(DateTime endUTCTime)
	{
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(endUTCTime);
		if (timeLeft.TotalDays >= 1.0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_DAYS_AND_HOURS"), timeLeft.Days, timeLeft.Hours);
		}
		if (timeLeft.TotalHours >= 1.0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_HOURS_AND_MINUTES"), timeLeft.Hours, timeLeft.Minutes);
		}
		if (timeLeft.TotalMinutes >= 1.0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_MINUTES"), timeLeft.Minutes);
		}
		if (timeLeft.TotalSeconds >= 1.0)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_SECONDS"), timeLeft.Seconds);
		}
		if (timeLeft.TotalSeconds > 0.0)
		{
			return NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_END_SOON");
		}
		return NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_END");
	}

	public static string GetRemainTimeString(DateTime endUTCTime, int maxWordCount)
	{
		return GetRemainTimeString(NKCSynchronizedTime.GetTimeLeft(endUTCTime), maxWordCount);
	}

	public static string GetRemainTimeString(TimeSpan timeSpan, int maxWordCount, bool bShowSecond = true)
	{
		if (timeSpan.TotalSeconds <= 0.0)
		{
			return GET_STRING_QUIT;
		}
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		if (timeSpan.TotalDays >= 1.0 && num < maxWordCount)
		{
			stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_DAYS"), timeSpan.Days));
			num++;
		}
		if (timeSpan.TotalHours >= 1.0 && num < maxWordCount)
		{
			if (num > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_HOURS"), timeSpan.Hours));
			num++;
		}
		if (timeSpan.TotalMinutes >= 1.0 && num < maxWordCount)
		{
			if (num > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_MINUTES"), timeSpan.Minutes));
			num++;
		}
		if (timeSpan.TotalSeconds >= 1.0 && bShowSecond && num < maxWordCount)
		{
			if (num > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_SECONDS"), timeSpan.Seconds));
		}
		return stringBuilder.ToString();
	}

	public static string GetTimeStringFromSeconds(int second)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(second);
		return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
	}

	public static string GetTimeStringFromMinutes(int minutes)
	{
		TimeSpan timeSpan = TimeSpan.FromMinutes(minutes);
		return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:00";
	}

	public static string GetRemainTimeStringForGauntletWeekly()
	{
		if (!NKCSynchronizedTime.IsFinished(NKCPVPManager.WeekCalcStartDateUtc))
		{
			return GetRemainTimeStringEx(NKCPVPManager.WeekCalcStartDateUtc);
		}
		return GET_STRING_TIME_CLOSING;
	}

	public static string GetTimeSpanString(TimeSpan timeSpan)
	{
		return $"{timeSpan.Hours + timeSpan.Days * 24:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
	}

	public static string GetTimeSpanStringMS(TimeSpan timeSpan)
	{
		return $"{timeSpan.Hours * 60 + timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
	}

	public static string GetTimeSpanStringDay(TimeSpan timeSpan)
	{
		if (timeSpan.Days > 0)
		{
			return $"{string.Format(GET_STRING_TIME_DAY_ONE_PARAM, timeSpan.Days)} {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
		}
		return GetTimeSpanString(timeSpan);
	}

	public static string GetTimeSpanStringDHM(TimeSpan timeSpan)
	{
		if (timeSpan.Days > 0)
		{
			return string.Format(GET_STRING_TIME_DAY_ONE_PARAM, timeSpan.Days);
		}
		if (timeSpan.Hours > 0)
		{
			return string.Format(GET_STRING_TIME_HOUR_ONE_PARAM, timeSpan.Hours);
		}
		return string.Format(GET_STRING_TIME_MINUTE_ONE_PARAM, timeSpan.Minutes);
	}

	public static TimeSpan GetLastTimeSpan(DateTime lastTime)
	{
		return NKCSynchronizedTime.GetServerUTCTime() - lastTime;
	}

	public static string GetLastTimeString(DateTime lastTime)
	{
		TimeSpan timeSpan = NKCSynchronizedTime.GetServerUTCTime() - lastTime;
		if (timeSpan.TotalDays >= 1.0)
		{
			return string.Format(GET_STRING_TIME_DAY_AGO_ONE_PARAM, (int)timeSpan.TotalDays);
		}
		if (timeSpan.TotalHours >= 1.0)
		{
			return string.Format(GET_STRING_TIME_HOUR_AGO_ONE_PARAM, (int)timeSpan.TotalHours);
		}
		if (timeSpan.TotalMinutes >= 1.0)
		{
			return string.Format(GET_STRING_TIME_MINUTE_AGO_ONE_PARAM, (int)timeSpan.TotalMinutes);
		}
		if (timeSpan.TotalSeconds >= 1.0)
		{
			return string.Format(GET_STRING_TIME_SECOND_AGO_ONE_PARAM, (int)timeSpan.TotalSeconds);
		}
		return GET_STRING_TIME_A_SECOND_AGO;
	}

	public static string GetResetTimeString(DateTime baseTimeUTC, NKMTime.TimePeriod timePeriod, int maxWordCount)
	{
		DateTime nextResetTime = NKMTime.GetNextResetTime(baseTimeUTC, timePeriod);
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(nextResetTime);
		string text = "FFDF5D";
		switch (timePeriod)
		{
		case NKMTime.TimePeriod.Day:
			if (timeLeft.TotalHours < 6.0)
			{
				text = "cd2121";
			}
			break;
		case NKMTime.TimePeriod.Week:
			if (timeLeft.TotalDays < 1.0)
			{
				text = "cd2121";
			}
			break;
		}
		string arg = "<color=#" + text + ">" + GetRemainTimeString(nextResetTime, maxWordCount) + "</color>";
		return string.Format(GET_STRING_SHOP_CHAIN_NEXT_RESET_ONE_PARAM, arg);
	}

	public static string GetTimeIntervalString(DateTime startTimeLocal, DateTime endTimeLocal, int IntervalFromUTC, bool bDateOnly = false)
	{
		if (bDateOnly)
		{
			return string.Format("{0} ~ {1}", startTimeLocal.ToString("yyyy-MM-dd"), endTimeLocal.ToString("yyyy-MM-dd"));
		}
		return string.Format("{0} ~ {1} (UTC{2:+#;-#;''})", startTimeLocal.ToString("yyyy-MM-dd"), endTimeLocal.ToString("yyyy-MM-dd HH:mm"), IntervalFromUTC);
	}

	public static string GetInAppPurchasePriceString(object price, int productID)
	{
		return NKCPublisherModule.InAppPurchase.GetCurrencyMark(productID) + " " + price.ToString();
	}

	public static string GetShopDescriptionText(string desc, bool bFirstBuy)
	{
		if (string.IsNullOrEmpty(desc))
		{
			return "";
		}
		string[] array = desc.Split('|');
		if (array.Length >= 2)
		{
			if (array.Length > 2)
			{
				Debug.LogWarning("Too many tokens in shopstring : " + desc);
			}
			return array[bFirstBuy ? 1 : 0];
		}
		return desc;
	}

	public static string GetShopItemBuyMessage(ShopItemTemplet productTemplet, bool bItemToMail = false)
	{
		if (productTemplet == null)
		{
			return "";
		}
		if (bItemToMail)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_SHOP_ITEM_BUY_MESSAGE_ITEM_TO_MAIL"), productTemplet.GetItemName());
		}
		return string.Format(NKCStringTable.GetString("SI_DP_SHOP_ITEM_BUY_MESSAGE_ELSE"), productTemplet.GetItemName());
	}

	public static string GetShopItemBuyMessage(NKMShopRandomListData randomListData)
	{
		if (randomListData == null)
		{
			return "";
		}
		switch (randomListData.itemType)
		{
		case NKM_REWARD_TYPE.RT_USER_EXP:
		{
			NKMItemMiscTemplet itemMiscTempletByRewardType = NKMItemManager.GetItemMiscTempletByRewardType(randomListData.itemType);
			if (itemMiscTempletByRewardType != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_SHOP_ITEM_BUY_MESSAGE_RT_USER_EXP"), itemMiscTempletByRewardType.GetItemName(), randomListData.itemCount);
			}
			return "";
		}
		case NKM_REWARD_TYPE.RT_MISC:
		case NKM_REWARD_TYPE.RT_MISSION_POINT:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(randomListData.itemId);
			if (itemMiscTempletByID != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_SHOP_ITEM_BUY_MESSAGE_RT_MISC"), itemMiscTempletByID.GetItemName(), randomListData.itemCount);
			}
			return "";
		}
		case NKM_REWARD_TYPE.RT_EQUIP:
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(randomListData.itemId);
			return string.Format(NKCStringTable.GetString("SI_DP_SHOP_ITEM_BUY_MESSAGE_RT_EQUIP"), GetItemEquipNameWithTier(equipTemplet));
		}
		case NKM_REWARD_TYPE.RT_SHIP:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(randomListData.itemId);
			if (unitTempletBase != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_SHOP_ITEM_BUY_MESSAGE_RT_SHIP"), unitTempletBase.GetUnitName());
			}
			return "";
		}
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_OPERATOR:
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(randomListData.itemId);
			if (unitTempletBase2 != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_SHOP_ITEM_BUY_MESSAGE_RT_UNIT"), unitTempletBase2.GetUnitName());
			}
			return "";
		}
		case NKM_REWARD_TYPE.RT_SKIN:
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(randomListData.itemId);
			if (skinTemplet != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_SHOP_ITEM_BUY_MESSAGE_RT_SKIN"), skinTemplet.GetTitle());
			}
			return "";
		}
		case NKM_REWARD_TYPE.RT_MOLD:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(randomListData.itemId);
			if (itemMoldTempletByID != null)
			{
				return string.Format(NKCStringTable.GetString("SI_DP_SHOP_ITEM_BUY_MESSAGE_RT_MOLD"), itemMoldTempletByID.GetItemName());
			}
			return "";
		}
		default:
			return "";
		}
	}

	public static string GetAppVersionText()
	{
		string text = ((NKCStringTable.GetNationalCode() == NKM_NATIONAL_CODE.NNC_CENSORED_CHINESE) ? "" : "App Version : ");
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
			return text + Application.version + "A";
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXPlayer:
		case RuntimePlatform.IPhonePlayer:
		case RuntimePlatform.tvOS:
			return text + Application.version + "I";
		default:
			return text + Application.version + "U";
		}
	}

	public static string GetProtocolVersionText()
	{
		if (NKCStringTable.GetNationalCode() != NKM_NATIONAL_CODE.NNC_CENSORED_CHINESE)
		{
			return "Protocol Version : " + 960 + " / Data Version : " + NKMDataVersion.DataVersion + " / StreamID " + (sbyte)(-1);
		}
		return 960 + " / " + NKMDataVersion.DataVersion + " / " + (sbyte)(-1);
	}

	public static string GetBuffString()
	{
		return string.Format("");
	}

	public static string GetWorldMapMissionType(NKMWorldMapMissionTemplet.WorldMapMissionType type)
	{
		return type switch
		{
			NKMWorldMapMissionTemplet.WorldMapMissionType.WMT_DEFENCE => NKCStringTable.GetString("SI_DP_WORLD_MAP_MISSION_TYPE_WMT_DEFENCE"), 
			NKMWorldMapMissionTemplet.WorldMapMissionType.WMT_EXPLORE => NKCStringTable.GetString("SI_DP_WORLD_MAP_MISSION_TYPE_WMT_EXPLORE"), 
			NKMWorldMapMissionTemplet.WorldMapMissionType.WMT_MINING => NKCStringTable.GetString("SI_DP_WORLD_MAP_MISSION_TYPE_WMT_MINING"), 
			NKMWorldMapMissionTemplet.WorldMapMissionType.WMT_OFFICE => NKCStringTable.GetString("SI_DP_WORLD_MAP_MISSION_TYPE_WMT_OFFICE"), 
			_ => NKCStringTable.GetString("SI_DP_WORLD_MAP_MISSION_TYPE_WMT_DEFENCE"), 
		};
	}

	public static string GetUnitStyleType(NKM_UNIT_STYLE_TYPE type)
	{
		return type switch
		{
			NKM_UNIT_STYLE_TYPE.NUST_INVALID => NKCStringTable.GetString("SI_DP_UNIT_STYLE_TYPE_NUST_INVALID"), 
			NKM_UNIT_STYLE_TYPE.NUST_COUNTER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_TYPE_NUST_COUNTER"), 
			NKM_UNIT_STYLE_TYPE.NUST_SOLDIER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_TYPE_NUST_SOLDIER"), 
			NKM_UNIT_STYLE_TYPE.NUST_MECHANIC => NKCStringTable.GetString("SI_DP_UNIT_STYLE_TYPE_NUST_MECHANIC"), 
			NKM_UNIT_STYLE_TYPE.NUST_CORRUPTED => NKCStringTable.GetString("SI_DP_UNIT_STYLE_TYPE_NUST_CORRUPTED"), 
			NKM_UNIT_STYLE_TYPE.NUST_REPLACER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_TYPE_NUST_REPLACER"), 
			NKM_UNIT_STYLE_TYPE.NUST_TRAINER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_TYPE_NUST_TRAINER"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT => NKCStringTable.GetString("SI_DP_UNIT_STYLE_TYPE_NUST_SHIP_ASSAULT"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY => NKCStringTable.GetString("SI_DP_UNIT_STYLE_TYPE_NUST_SHIP_HEAVY"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER => NKCStringTable.GetString("SI_DP_UNIT_STYLE_TYPE_NUST_SHIP_CRUISER"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL => NKCStringTable.GetString("SI_DP_UNIT_STYLE_TYPE_NUST_SHIP_SPECIAL"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_PATROL => NKCStringTable.GetString("SI_DP_UNIT_STYLE_TYPE_NUST_SHIP_PATROL"), 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_ETC => NKCStringTable.GetString("SI_DP_UNIT_STYLE_TYPE_NUST_SHIP_ETC"), 
			_ => NKCStringTable.GetString("SI_DP_UNIT_STYLE_TYPE_UNKNOWN"), 
		};
	}

	public static string GetUnitRoleType(NKM_UNIT_ROLE_TYPE type, bool bAwaken)
	{
		if (bAwaken)
		{
			switch (type)
			{
			case NKM_UNIT_ROLE_TYPE.NURT_INVALID:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_INVALID");
			case NKM_UNIT_ROLE_TYPE.NURT_STRIKER:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_STRIKER_AWAKEN");
			case NKM_UNIT_ROLE_TYPE.NURT_RANGER:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_RANGER_AWAKEN");
			case NKM_UNIT_ROLE_TYPE.NURT_DEFENDER:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_DEFENDER_AWAKEN");
			case NKM_UNIT_ROLE_TYPE.NURT_SNIPER:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_SNIPER_AWAKEN");
			case NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_SUPPORTER_AWAKEN");
			case NKM_UNIT_ROLE_TYPE.NURT_SIEGE:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_SIEGE_AWAKEN");
			case NKM_UNIT_ROLE_TYPE.NURT_TOWER:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_TOWER_AWAKEN");
			}
		}
		else
		{
			switch (type)
			{
			case NKM_UNIT_ROLE_TYPE.NURT_INVALID:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_INVALID");
			case NKM_UNIT_ROLE_TYPE.NURT_STRIKER:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_STRIKER");
			case NKM_UNIT_ROLE_TYPE.NURT_RANGER:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_RANGER");
			case NKM_UNIT_ROLE_TYPE.NURT_DEFENDER:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_DEFENDER");
			case NKM_UNIT_ROLE_TYPE.NURT_SNIPER:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_SNIPER");
			case NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_SUPPORTER");
			case NKM_UNIT_ROLE_TYPE.NURT_SIEGE:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_SIEGE");
			case NKM_UNIT_ROLE_TYPE.NURT_TOWER:
				return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_NURT_TOWER");
			}
		}
		return NKCStringTable.GetString("SI_DP_UNIT_ROLE_TYPE_UNKNOWN");
	}

	public static string GetMenuName(NKCUIUnitInfo.UNIT_INFO_TAB_STATE state)
	{
		return state switch
		{
			NKCUIUnitInfo.UNIT_INFO_TAB_STATE.NEGOTIATION => GET_STRING_NEGOTIATE, 
			NKCUIUnitInfo.UNIT_INFO_TAB_STATE.LIMIT_BREAK => NKCStringTable.GetString("SI_DP_LAB_SELECT_UNIT_MENU_NAME_LDS_UNIT_LIMITBREAK"), 
			NKCUIUnitInfo.UNIT_INFO_TAB_STATE.SKILL_TRAIN => NKCStringTable.GetString("SI_DP_LAB_SELECT_UNIT_MENU_NAME_LDS_UNIT_SKILL_TRAIN"), 
			_ => "", 
		};
	}

	public static string GetEmptyMessage(NKCUIUnitInfo.UNIT_INFO_TAB_STATE state)
	{
		return state switch
		{
			NKCUIUnitInfo.UNIT_INFO_TAB_STATE.NEGOTIATION => GET_STRING_NEGOTIATE_NO_EXIST_UNIT, 
			NKCUIUnitInfo.UNIT_INFO_TAB_STATE.LIMIT_BREAK => NKCStringTable.GetString("SI_DP_LAB_SELECT_UNIT_EMPTY_MSG_LIMITBREAK"), 
			NKCUIUnitInfo.UNIT_INFO_TAB_STATE.SKILL_TRAIN => NKCStringTable.GetString("SI_DP_LAB_SELECT_UNIT_EMPTY_MSG_SKILL_TRAIN"), 
			_ => "", 
		};
	}

	public static string GetLabMenuName(NKCUILab.LAB_DETAIL_STATE state)
	{
		return state switch
		{
			NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_ENHANCE => NKCStringTable.GetString("SI_DP_LAB_MENU_NAME_LDS_UNIT_ENHANCE"), 
			NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK => NKCStringTable.GetString("SI_DP_LAB_MENU_NAME_LDS_UNIT_LIMITBREAK"), 
			NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN => NKCStringTable.GetString("SI_DP_LAB_MENU_NAME_LDS_UNIT_SKILL_TRAIN"), 
			_ => NKCStringTable.GetString("SI_DP_LAB_MENU_NAME_LDS_MENU"), 
		};
	}

	public static string GetLabSelectUnitMenuName(NKCUILab.LAB_DETAIL_STATE state)
	{
		return state switch
		{
			NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_ENHANCE => NKCStringTable.GetString("SI_DP_LAB_SELECT_UNIT_MENU_NAME_LDS_UNIT_ENHANCE"), 
			NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK => NKCStringTable.GetString("SI_DP_LAB_SELECT_UNIT_MENU_NAME_LDS_UNIT_LIMITBREAK"), 
			NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN => NKCStringTable.GetString("SI_DP_LAB_SELECT_UNIT_MENU_NAME_LDS_UNIT_SKILL_TRAIN"), 
			_ => "", 
		};
	}

	public static string GetLabSelectUnitEmptyMsg(NKCUILab.LAB_DETAIL_STATE state)
	{
		return state switch
		{
			NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_ENHANCE => NKCStringTable.GetString("SI_DP_LAB_SELECT_UNIT_EMPTY_MSG_ENHANCE"), 
			NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK => NKCStringTable.GetString("SI_DP_LAB_SELECT_UNIT_EMPTY_MSG_LIMITBREAK"), 
			NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN => NKCStringTable.GetString("SI_DP_LAB_SELECT_UNIT_EMPTY_MSG_SKILL_TRAIN"), 
			_ => "", 
		};
	}

	public static string GetRefineResultMsg(NKM_EQUIP_REFINE_RESULT eNKM_EQUIP_REFINE_RESULT)
	{
		return eNKM_EQUIP_REFINE_RESULT switch
		{
			NKM_EQUIP_REFINE_RESULT.NERR_SUCCESS => NKCStringTable.GetString("SI_DP_REFINE_RESULT_MSG_NERR_SUCCESS"), 
			NKM_EQUIP_REFINE_RESULT.NERR_GREAT_SUCCESS => NKCStringTable.GetString("SI_DP_REFINE_RESULT_MSG_NERR_GREAT_SUCCESS"), 
			NKM_EQUIP_REFINE_RESULT.NERR_FAIL => NKCStringTable.GetString("SI_DP_REFINE_RESULT_MSG_NERR_FAIL"), 
			NKM_EQUIP_REFINE_RESULT.NERR_GREAT_FAIL => NKCStringTable.GetString("SI_DP_REFINE_RESULT_MSG_NERR_GREAT_FAIL"), 
			_ => NKCStringTable.GetString("SI_DP_REFINE_RESULT_MSG_UNKNOWN"), 
		};
	}

	public static string GetMoldResetCount(NKMItemMoldTemplet moldTemplet)
	{
		if (moldTemplet.m_ResetGroupId > 0)
		{
			NKMResetCounterGroupTemplet nKMResetCounterGroupTemplet = NKMResetCounterGroupTemplet.Find(moldTemplet.m_ResetGroupId);
			if (nKMResetCounterGroupTemplet != null)
			{
				int remainResetCount = NKMItemManager.GetRemainResetCount(nKMResetCounterGroupTemplet.GroupId);
				if (remainResetCount >= 0)
				{
					switch (nKMResetCounterGroupTemplet.Type)
					{
					case COUNT_RESET_TYPE.MONTH:
						return string.Format(NKCStringTable.GetString("SI_DP_MOLD_MONTH_PRODUCT_COUNT_TWO_PARAM") ?? "", remainResetCount, nKMResetCounterGroupTemplet.MaxCount);
					case COUNT_RESET_TYPE.WEEK:
						return string.Format(NKCStringTable.GetString("SI_DP_MOLD_WEEK_PRODUCT_COUNT_TWO_PARAM") ?? "", remainResetCount, nKMResetCounterGroupTemplet.MaxCount);
					case COUNT_RESET_TYPE.DAY:
						return string.Format(NKCStringTable.GetString("SI_DP_MOLD_DAY_PRODUCT_COUNT_TWO_PARAM") ?? "", remainResetCount, nKMResetCounterGroupTemplet.MaxCount);
					case COUNT_RESET_TYPE.FIXED:
						return string.Format(NKCStringTable.GetString("SI_DP_MOLD_ACCOUNT_PRODUCT_COUNT_TWO_PARAM") ?? "", remainResetCount, nKMResetCounterGroupTemplet.MaxCount);
					}
				}
			}
		}
		return "";
	}

	public static void GetNegotiateResult(NEGOTIATE_RESULT result, out string title, out string info)
	{
		title = "";
		info = "";
		switch (result)
		{
		case NEGOTIATE_RESULT.SUCCESS:
			title = NKCStringTable.GetString("SI_DP_NEGOTIATE_RESULT_BIG_SUCCESS_TITLE");
			info = NKCStringTable.GetString("SI_DP_NEGOTIATE_RESULT_BIG_SUCCESS_INFO");
			break;
		case NEGOTIATE_RESULT.COMPLETE:
			title = NKCStringTable.GetString("SI_DP_NEGOTIATE_RESULT_SUCCESS_TITLE");
			info = NKCStringTable.GetString("SI_DP_NEGOTIATE_RESULT_SUCCESS_INFO");
			break;
		}
	}

	public static string GetNegotiateDesc(NEGOTIATE_BOSS_SELECTION selection)
	{
		return selection switch
		{
			NEGOTIATE_BOSS_SELECTION.RAISE => string.Format(GET_STRING_NEGOTIATE_RAISE_DESC, NKMCommonConst.Negotiation.Bonus_CreditIncreasePercent, NKMCommonConst.Negotiation.Bonus_LoyaltyIncreasePercent, NKMCommonConst.Negotiation.Bonus_ResultSuccessPercent / NKMCommonConst.Negotiation.Normal_ResultSuccessPercent), 
			NEGOTIATE_BOSS_SELECTION.OK => GET_STRING_NEGOTIATE_OK_DESC, 
			NEGOTIATE_BOSS_SELECTION.PASSION => string.Format(GET_STRING_NEGOTIATE_PASSION_DESC, NKMCommonConst.Negotiation.Passion_CreditDecreasePercent), 
			_ => string.Empty, 
		};
	}

	public static string GetLifetimeContractDesc(NKMUnitTempletBase unitTempletBase, string playerName)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(NKCStringTable.GetString("SI_DP_LIFETIME_CONTRACT_DESC_UNIT_TITLE"), unitTempletBase.GetUnitTitle());
		stringBuilder.AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat(NKCStringTable.GetString("SI_DP_LIFETIME_CONTRACT_DESC_UNIT_NAME"), playerName, unitTempletBase.GetUnitName());
		return stringBuilder.ToString();
	}

	public static string GetNegotiateSpeechBySelection(NEGOTIATE_BOSS_SELECTION selection)
	{
		return selection switch
		{
			NEGOTIATE_BOSS_SELECTION.OK => NKCStringTable.GetString("SI_DP_NEGOTIATE_SPEECH_BY_SELECTION_OK"), 
			NEGOTIATE_BOSS_SELECTION.PASSION => NKCStringTable.GetString("SI_DP_NEGOTIATE_SPEECH_BY_SELECTION_PASSION"), 
			_ => NKCStringTable.GetString("SI_DP_NEGOTIATE_SPEECH_BY_SELECTION_RAISE"), 
		};
	}

	public static string GetNegotiateResultTalk(NEGOTIATE_RESULT result)
	{
		return result switch
		{
			NEGOTIATE_RESULT.SUCCESS => NKCStringTable.GetString("SI_DP_NEGOTIATE_RESULT_TALK_BIG_SUCCESS"), 
			NEGOTIATE_RESULT.COMPLETE => NKCStringTable.GetString("SI_DP_NEGOTIATE_RESULT_TALK_SUCCESS"), 
			_ => "", 
		};
	}

	public static string GetBaseMenuName(NKCUIBaseSceneMenu.BaseSceneMenuType type)
	{
		return type switch
		{
			NKCUIBaseSceneMenu.BaseSceneMenuType.Factory => NKCStringTable.GetString("SI_DP_BASE_MENU_NAME_FACTORY"), 
			NKCUIBaseSceneMenu.BaseSceneMenuType.Hangar => NKCStringTable.GetString("SI_DP_BASE_MENU_NAME_HANGAR"), 
			NKCUIBaseSceneMenu.BaseSceneMenuType.Lab => NKCStringTable.GetString("SI_DP_BASE_MENU_NAME_LAB"), 
			NKCUIBaseSceneMenu.BaseSceneMenuType.Personnel => NKCStringTable.GetString("SI_DP_BASE_MENU_NAME_PERSONNEL"), 
			_ => NKCStringTable.GetString("SI_DP_BASE_MENU_NAME_BASE"), 
		};
	}

	public static string GetBaseMenuNameEng(NKCUIBaseSceneMenu.BaseSceneMenuType type)
	{
		return type switch
		{
			NKCUIBaseSceneMenu.BaseSceneMenuType.Hangar => NKCStringTable.GetString("SI_DP_BASE_MENU_NAME_ENG_HANGAR"), 
			NKCUIBaseSceneMenu.BaseSceneMenuType.Lab => NKCStringTable.GetString("SI_DP_BASE_MENU_NAME_ENG_LAB"), 
			NKCUIBaseSceneMenu.BaseSceneMenuType.Personnel => NKCStringTable.GetString("SI_DP_BASE_MENU_NAME_ENG_PERSONNEL"), 
			_ => NKCStringTable.GetString("SI_DP_BASE_MENU_NAME_ENG_FACTORY"), 
		};
	}

	public static string GetBaseSubMenuDetail(NKCUIBaseSceneMenu.BaseSceneMenuType type)
	{
		return type switch
		{
			NKCUIBaseSceneMenu.BaseSceneMenuType.Hangar => NKCStringTable.GetString("SI_DP_BASE_SUB_MENU_DETAIL_HANGAR"), 
			NKCUIBaseSceneMenu.BaseSceneMenuType.Lab => NKCStringTable.GetString("SI_DP_BASE_SUB_MENU_DETAIL_LAB"), 
			NKCUIBaseSceneMenu.BaseSceneMenuType.Personnel => NKCStringTable.GetString("SI_DP_BASE_SUB_MENU_DETAIL_PERSONNEL"), 
			_ => NKCStringTable.GetString("SI_DP_BASE_SUB_MENU_DETAIL_FACTORY"), 
		};
	}

	public static string GetCompanyBuffTitle(string buffName, bool bAddBuff)
	{
		if (bAddBuff)
		{
			return string.Format(NKCStringTable.GetString("SI_DP_EVENT_BUFF_TITLE_ADD"), buffName);
		}
		return string.Format(NKCStringTable.GetString("SI_DP_EVENT_BUFF_TITLE_REMOVE"), buffName);
	}

	public static string GetEquipPositionString(ITEM_EQUIP_POSITION equipPosition)
	{
		switch (equipPosition)
		{
		case ITEM_EQUIP_POSITION.IEP_ACC:
		case ITEM_EQUIP_POSITION.IEP_ACC2:
			return NKCStringTable.GetString("SI_DP_EQUIP_POSITION_STRING_IEP_ACC");
		case ITEM_EQUIP_POSITION.IEP_DEFENCE:
			return NKCStringTable.GetString("SI_DP_EQUIP_POSITION_STRING_IEP_DEFENCE");
		case ITEM_EQUIP_POSITION.IEP_WEAPON:
			return NKCStringTable.GetString("SI_DP_EQUIP_POSITION_STRING_IEP_WEAPON");
		default:
			return "";
		}
	}

	private static string GetMiscTypeString(NKM_ITEM_MISC_TYPE miscType)
	{
		switch (miscType)
		{
		case NKM_ITEM_MISC_TYPE.IMT_MISC:
		case NKM_ITEM_MISC_TYPE.IMT_PACKAGE:
		case NKM_ITEM_MISC_TYPE.IMT_RANDOMBOX:
			return NKCStringTable.GetString("SI_DP_MISC_TYPE_STRING_IMT_RANDOMBOX");
		case NKM_ITEM_MISC_TYPE.IMT_RESOURCE:
			return NKCStringTable.GetString("SI_DP_MISC_TYPE_STRING_IMT_RESOURCE");
		case NKM_ITEM_MISC_TYPE.IMT_INTERIOR:
			return NKCStringTable.GetString("SI_DP_MISC_TYPE_STRING_IMT_INTERIOR");
		default:
			return "";
		}
	}

	public static string ApplyBuffValueToString(string str, IEnumerable<string> targetBuffTempletStrIDs, int buffLevel, int timeLevel)
	{
		string[] separator = new string[1] { "##" };
		string[] array = str.Split(separator, StringSplitOptions.None);
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string targetBuffTempletStrID in targetBuffTempletStrIDs)
		{
			if (num < array.Length)
			{
				break;
			}
			NKMBuffTemplet buffTempletByStrID = NKMBuffManager.GetBuffTempletByStrID(targetBuffTempletStrID);
			stringBuilder.Append(ApplyBuffValueToString(array[num], buffTempletByStrID, buffLevel, timeLevel));
			num++;
		}
		return stringBuilder.ToString();
	}

	public static string ApplyBuffValueToString(string str, IEnumerable<NKMBuffTemplet> targetBuffTemplets, int buffLevel, int timeLevel)
	{
		string[] separator = new string[1] { "##" };
		string[] array = str.Split(separator, StringSplitOptions.None);
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (NKMBuffTemplet targetBuffTemplet in targetBuffTemplets)
		{
			if (num < array.Length)
			{
				break;
			}
			stringBuilder.Append(ApplyBuffValueToString(array[num], targetBuffTemplet, buffLevel, timeLevel));
			num++;
		}
		return stringBuilder.ToString();
	}

	public static string ApplyBuffValueToString(string str, NKMBuffTemplet targetBuffTemplet, int buffLevel, int buffTimeLevel)
	{
		if (targetBuffTemplet == null)
		{
			return str;
		}
		int num = buffLevel - 1;
		int num2 = buffTimeLevel - 1;
		float num3 = ((targetBuffTemplet.m_fLifeTime > 0f) ? (targetBuffTemplet.m_fLifeTime + targetBuffTemplet.m_fLifeTimePerLevel * (float)num2) : targetBuffTemplet.m_fLifeTime);
		float num4 = ((targetBuffTemplet.m_StatValue1 != 0) ? (targetBuffTemplet.m_StatValue1 + targetBuffTemplet.m_StatAddPerLevel1 * num) : 0);
		float num5 = ((targetBuffTemplet.m_StatValue2 != 0) ? (targetBuffTemplet.m_StatValue2 + targetBuffTemplet.m_StatAddPerLevel2 * num) : 0);
		float num6 = ((targetBuffTemplet.m_StatValue3 != 0) ? (targetBuffTemplet.m_StatValue3 + targetBuffTemplet.m_StatAddPerLevel3 * num) : 0);
		num4 *= (NKMUnitStatManager.IsPercentStat(targetBuffTemplet.m_StatType1) ? 0.01f : 0.0001f);
		num5 *= (NKMUnitStatManager.IsPercentStat(targetBuffTemplet.m_StatType2) ? 0.01f : 0.0001f);
		num6 *= (NKMUnitStatManager.IsPercentStat(targetBuffTemplet.m_StatType3) ? 0.01f : 0.0001f);
		num4 = Mathf.Abs(num4);
		num5 = Mathf.Abs(num5);
		num6 = Mathf.Abs(num6);
		bool flag = num4 < 0f;
		bool flag2 = num5 < 0f;
		bool flag3 = num6 < 0f;
		string statShortName = GetStatShortName(targetBuffTemplet.m_StatType1, flag);
		string statShortName2 = GetStatShortName(targetBuffTemplet.m_StatType2, flag2);
		string statShortName3 = GetStatShortName(targetBuffTemplet.m_StatType3, flag3);
		float f = GetBuffStatPerLevelStringValue(targetBuffTemplet.m_StatAddPerLevel1, targetBuffTemplet.m_StatType1, targetBuffTemplet.m_StatValue1);
		float f2 = GetBuffStatPerLevelStringValue(targetBuffTemplet.m_StatAddPerLevel2, targetBuffTemplet.m_StatType2, targetBuffTemplet.m_StatValue2);
		float f3 = GetBuffStatPerLevelStringValue(targetBuffTemplet.m_StatAddPerLevel3, targetBuffTemplet.m_StatType3, targetBuffTemplet.m_StatValue3);
		if (flag && IsNameReversedIfNegative(targetBuffTemplet.m_StatType1))
		{
			num4 = Mathf.Abs(num4);
			f = Mathf.Abs(f);
		}
		if (flag2 && IsNameReversedIfNegative(targetBuffTemplet.m_StatType2))
		{
			num5 = Mathf.Abs(num5);
			f2 = Mathf.Abs(f2);
		}
		if (flag3 && IsNameReversedIfNegative(targetBuffTemplet.m_StatType3))
		{
			num6 = Mathf.Abs(num6);
			f3 = Mathf.Abs(f3);
		}
		return str.Replace("{time}", num3.ToString("0.##")).Replace("{value1}", num4.ToString("0.##")).Replace("{value2}", num5.ToString("0.##"))
			.Replace("{value3}", num6.ToString("0.##"))
			.Replace("{factor1}", num4.ToString("0.##"))
			.Replace("{factor2}", num5.ToString("0.##"))
			.Replace("{factor3}", num6.ToString("0.##"))
			.Replace("{stat1}", statShortName)
			.Replace("{stat2}", statShortName2)
			.Replace("{stat3}", statShortName3)
			.Replace("{statperlevel1}", f.ToString("0.##"))
			.Replace("{statperlevel2}", f2.ToString("0.##"))
			.Replace("{statperlevel3}", f3.ToString("0.##"))
			.Replace("{barrierhp}", ((targetBuffTemplet.m_fBarrierHP != 0f) ? (targetBuffTemplet.m_fBarrierHP * 100f + 100f * targetBuffTemplet.m_fBarrierHPPerLevel * (float)num) : 0f).ToString("0.##"))
			.Replace("{barrierhpperlevel}", (targetBuffTemplet.m_fBarrierHPPerLevel * 100f).ToString("0.##"))
			.Replace("{maxoverlapcount}", targetBuffTemplet.m_MaxOverlapCount.ToString("N0"));
	}

	public static string ApplyBuffValueToString(NKMTacticalCommandTemplet tacticalTemplet, int level)
	{
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder("");
		for (int i = 0; i < tacticalTemplet.m_lstBuffStrID_MyTeam.Count; i++)
		{
			NKMBuffTemplet buffTempletByStrID = NKMBuffManager.GetBuffTempletByStrID(tacticalTemplet.m_lstBuffStrID_MyTeam[i]);
			if (flag)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.Append(ApplyBuffValueToString(tacticalTemplet.GetTCDescMyTeam(i), buffTempletByStrID, level, level));
			flag = true;
		}
		for (int j = 0; j < tacticalTemplet.m_lstBuffStrID_Enemy.Count; j++)
		{
			NKMBuffTemplet buffTempletByStrID = NKMBuffManager.GetBuffTempletByStrID(tacticalTemplet.m_lstBuffStrID_Enemy[j]);
			if (flag && stringBuilder.Length != 0)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.Append(ApplyBuffValueToString(tacticalTemplet.GetTCDescEnemy(j), buffTempletByStrID, level, level));
			flag = true;
		}
		stringBuilder.Replace("{costpump}", tacticalTemplet.m_fCostPump.ToString("0.##"));
		stringBuilder.Replace("{costpumpperlevel}", tacticalTemplet.m_fCostPumpPerLevel.ToString("0.##"));
		stringBuilder.Replace("{costpumpresult}", (tacticalTemplet.m_fCostPump + tacticalTemplet.m_fCostPumpPerLevel * (float)(level - 1)).ToString("0.##"));
		return stringBuilder.ToString();
	}

	private static float GetBuffStatPerLevelStringValue(float statPerLevel, NKM_STAT_TYPE statType, float statValue)
	{
		if (statValue != 0f)
		{
			statPerLevel *= (NKMUnitStatManager.IsPercentStat(statType) ? 0.01f : 0.0001f);
			return Mathf.Abs(statPerLevel);
		}
		return 0f;
	}

	public static string GetUserNickname(string nickname, bool bOpponent)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData == null)
		{
			return nickname;
		}
		if (bOpponent)
		{
			if (gameOptionData.StreamingHideOpponentInfo)
			{
				return "???";
			}
			return NKCStringTable.GetString(nickname, bSkipErrorCheck: true);
		}
		if (gameOptionData.StreamingHideMyInfo)
		{
			return "???";
		}
		return nickname;
	}

	public static string GetUserGuildName(string guildName, bool bOpponent)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData == null)
		{
			return guildName;
		}
		if (bOpponent)
		{
			if (gameOptionData.StreamingHideOpponentInfo)
			{
				return "???";
			}
		}
		else if (gameOptionData.StreamingHideMyInfo)
		{
			return "???";
		}
		return guildName;
	}

	public static string GetVoiceCaption(NKMAssetName cNKMAssetName)
	{
		if (cNKMAssetName == null)
		{
			return string.Empty;
		}
		string strID = cNKMAssetName.m_BundleName + "@" + cNKMAssetName.m_AssetName;
		if (NKCDefineManager.DEFINE_SERVICE() && !NKCStringTable.CheckExistString(strID))
		{
			return string.Empty;
		}
		return NKCStringTable.GetString(strID);
	}

	public static string GetVoiceCaption(string bundleName, string assetName)
	{
		string strID = bundleName + "@" + assetName;
		if (NKCDefineManager.DEFINE_SERVICE() && !NKCStringTable.CheckExistString(strID))
		{
			return string.Empty;
		}
		return NKCStringTable.GetString(strID);
	}

	public static string GetVoiceKey(string bundleName, string assetName)
	{
		return bundleName + "@" + assetName;
	}

	public static string GetStringVoiceCategory(bool lifetime)
	{
		if (lifetime)
		{
			return NKCStringTable.GetString("SI_DP_STRING_VOICE_CATEGORY_LIFETIME");
		}
		return NKCStringTable.GetString("SI_DP_STRING_VOICE_CATEGORY_NORMAL");
	}

	public static string GetDeckConditionString(NKMDeckCondition.EventDeckCondition condition)
	{
		if (condition is NKMDeckCondition.AllDeckCondition)
		{
			switch ((condition as NKMDeckCondition.AllDeckCondition).eCondition)
			{
			case NKMDeckCondition.ALL_DECK_CONDITION.UNIT_COST_TOTAL:
				return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_UNIT_COST_TOTAL_TWO_PARAM"), GetValueListString(condition), GetMoreLessString(condition.eMoreLess));
			case NKMDeckCondition.ALL_DECK_CONDITION.REARM_COUNT:
				if (condition.IsProhibited())
				{
					return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_REARM_ONE_PARAM"), GetMoreLessString(NKMDeckCondition.MORE_LESS.NOT));
				}
				return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_REARM_COUNT_TWO_PARAM"), GetValueListString(condition), GetMoreLessString(condition.eMoreLess));
			case NKMDeckCondition.ALL_DECK_CONDITION.AWAKEN_COUNT:
				if (condition.IsProhibited())
				{
					return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_AWAKEN_ONE_PARAM"), GetMoreLessString(NKMDeckCondition.MORE_LESS.NOT));
				}
				return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_AWAKEN_COUNTL_TWO_PARAM"), GetValueListString(condition), GetMoreLessString(condition.eMoreLess));
			case NKMDeckCondition.ALL_DECK_CONDITION.UNIT_GROUND_COUNT:
				if (condition.IsProhibited())
				{
					return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_UNIT_GROUND_NOT"));
				}
				return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_UNIT_GROUND_COUNT_TWO_PARAM"), GetValueListString(condition), GetMoreLessString(condition.eMoreLess));
			case NKMDeckCondition.ALL_DECK_CONDITION.UNIT_AIR_COUNT:
				if (condition.IsProhibited())
				{
					return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_UNIT_AIR_NOT"));
				}
				return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_UNIT_AIR_COUNT_TWO_PARAM"), GetValueListString(condition), GetMoreLessString(condition.eMoreLess));
			case NKMDeckCondition.ALL_DECK_CONDITION.UNIT_GRADE_COUNT:
				if (condition.IsProhibited())
				{
					return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_UNIT_GRADE_COUNT_NOT", GetValueListString(condition, GetEventdeckGradeString)));
				}
				return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_UNIT_GRADE_COUNT_THREE_PARAM"), GetValueListString(condition, GetEventdeckGradeString), condition.Value, GetMoreLessString(condition.eMoreLess));
			}
		}
		else if (condition is NKMDeckCondition.SingleCondition)
		{
			switch ((condition as NKMDeckCondition.SingleCondition).eCondition)
			{
			case NKMDeckCondition.DECK_CONDITION.UNIT_COST:
				return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_UNIT_COST_TWO_PARAM"), GetValueListString(condition), GetMoreLessString(condition.eMoreLess));
			case NKMDeckCondition.DECK_CONDITION.UNIT_GRADE:
				return $"{GetValueListString(condition, GetEventdeckGradeString)} {GetMoreLessString(condition.eMoreLess)}";
			case NKMDeckCondition.DECK_CONDITION.UNIT_LEVEL:
				return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_UNIT_LEVEL_TWO_PARAM"), GetValueListString(condition), GetMoreLessString(condition.eMoreLess));
			case NKMDeckCondition.DECK_CONDITION.UNIT_ROLE:
				return $"{GetValueListString(condition, (int x) => GetUnitRoleType((NKM_UNIT_ROLE_TYPE)x, bAwaken: false))} {GetMoreLessString(condition.eMoreLess)}";
			case NKMDeckCondition.DECK_CONDITION.UNIT_STYLE:
				return $"{GetValueListString(condition, (int x) => GetUnitStyleName((NKM_UNIT_STYLE_TYPE)x))} {GetMoreLessString(condition.eMoreLess)}";
			case NKMDeckCondition.DECK_CONDITION.SHIP_LEVEL:
				return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_SHIP_LEVEL_TWO_PARAM"), GetValueListString(condition), GetMoreLessString(condition.eMoreLess));
			case NKMDeckCondition.DECK_CONDITION.SHIP_STYLE:
				return $"{GetValueListString(condition, (int x) => GetUnitStyleType((NKM_UNIT_STYLE_TYPE)x))} {GetMoreLessString(condition.eMoreLess)}";
			case NKMDeckCondition.DECK_CONDITION.UNIT_ID_NOT:
			{
				string valueListString = GetValueListString(condition, delegate(int x)
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(x);
					if (unitTempletBase == null)
					{
						Debug.LogError($"UnitID {x} from deckCondition not found!");
						return "";
					}
					string unitTitle = unitTempletBase.GetUnitTitle();
					return string.IsNullOrEmpty(unitTitle) ? unitTempletBase.GetUnitName() : (unitTitle + " " + unitTempletBase.GetUnitName());
				});
				return string.Format(NKCStringTable.GetString("SI_DP_DECKCONDITION_UNIT_ID_NOT_ONE_PARAM"), valueListString);
			}
			}
		}
		return "";
	}

	public static string GetGameConditionString(NKMDeckCondition.GameCondition condition)
	{
		switch (condition.eCondition)
		{
		case NKMDeckCondition.GAME_CONDITION.LEVEL_CAP:
			return NKCStringTable.GetString("SI_DP_DECKCONDITION_LEVEL_CAP_ONE_PARAM", condition.Value);
		case NKMDeckCondition.GAME_CONDITION.FORCE_REARM_TO_BASIC:
			return NKCStringTable.GetString("SI_DP_DECKCONDITION_FORCE_REARM_TO_BASIC", condition.Value);
		case NKMDeckCondition.GAME_CONDITION.MODIFY_START_COST:
			if (condition.Value > 0)
			{
				return NKCStringTable.GetString("SI_DP_GAMECONDITION_START_COST_TWO_PARAM", condition.Value, NKCStringTable.GetString("SI_DP_GAMECONDITION_INCREASE"));
			}
			if (condition.Value < 0)
			{
				return NKCStringTable.GetString("SI_DP_GAMECONDITION_START_COST_TWO_PARAM", Mathf.Abs(condition.Value), NKCStringTable.GetString("SI_DP_GAMECONDITION_DECREASE"));
			}
			break;
		case NKMDeckCondition.GAME_CONDITION.SET_START_COST:
			return NKCStringTable.GetString("SI_DP_GAMECONDITION_SET_COST", condition.Value);
		}
		return "";
	}

	private static string GetValueListString(NKMDeckCondition.EventDeckCondition condition, ConditionToString converter)
	{
		if (condition.lstValue != null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < condition.lstValue.Count; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(converter(condition.lstValue[i]));
			}
			return stringBuilder.ToString();
		}
		return converter(condition.Value);
	}

	private static string GetValueListString(NKMDeckCondition.EventDeckCondition condition)
	{
		return GetValueListString(condition, (int x) => x.ToString());
	}

	private static string GetEventdeckGradeString(int grade)
	{
		return (NKM_UNIT_GRADE)(short)grade switch
		{
			NKM_UNIT_GRADE.NUG_R => NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_R_FOR_EVENTDECK"), 
			NKM_UNIT_GRADE.NUG_SR => NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_SR_FOR_EVENTDECK"), 
			NKM_UNIT_GRADE.NUG_SSR => NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_SSR_FOR_EVENTDECK"), 
			_ => NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_N_FOR_EVENTDECK"), 
		};
	}

	public static string GetMoreLessString(NKMDeckCondition.MORE_LESS MoreLess)
	{
		return MoreLess switch
		{
			NKMDeckCondition.MORE_LESS.LESS => NKCStringTable.GetString("SI_DP_EVENTDECK_DECKCONDITION_LESS"), 
			NKMDeckCondition.MORE_LESS.MORE => NKCStringTable.GetString("SI_DP_EVENTDECK_DECKCONDITION_MORE"), 
			NKMDeckCondition.MORE_LESS.NOT => NKCStringTable.GetString("SI_DP_EVENTDECK_DECKCONDITION_NOT"), 
			_ => NKCStringTable.GetString("SI_DP_EVENTDECK_DECKCONDITION_EQUAL"), 
		};
	}

	public static string GET_SHADOW_PALACE_GIVE_UP(params object[] param)
	{
		return NKCStringTable.GetString("SI_DP_SHADOW_PALACE_SURRENDER_POPUP_DESC", bSkipErrorCheck: false, param);
	}

	public static string GetEquipUpgradeStateString(NKC_EQUIP_UPGRADE_STATE state)
	{
		return state switch
		{
			NKC_EQUIP_UPGRADE_STATE.NEED_PRECISION => NKCStringTable.GetString("SI_PF_FACTORY_UPGRADE_STATE_TUNING"), 
			NKC_EQUIP_UPGRADE_STATE.NEED_ENHANCE => NKCStringTable.GetString("SI_PF_FACTORY_UPGRADE_STATE_ENHANCE"), 
			NKC_EQUIP_UPGRADE_STATE.NOT_HAVE => NKCStringTable.GetString("SI_PF_FACTORY_UPGRADE_STATE_NOHAVE"), 
			_ => "", 
		};
	}
}
