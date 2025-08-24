using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.LeaderBoard;
using NKM.Contract2;
using NKM.Templet.Base;

namespace NKM.Guild;

public sealed class GuildTemplet
{
	public readonly struct CreationType
	{
		public int UserMinLevel { get; }

		public MiscItemUnit[] ReqMiscItems { get; }

		public CreationType(NKMLua lua)
		{
			using (lua.OpenTable("Creation", "Creation table open failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 321))
			{
				UserMinLevel = lua.GetInt32("UserMinLevel");
				List<MiscItemUnit> list = new List<MiscItemUnit>(5);
				using (lua.OpenTable("ReqMiscItems", "reqMiscItems table open failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 325))
				{
					int num = 1;
					while (lua.OpenTable(num++))
					{
						int @int = lua.GetInt32("ItemId");
						int int2 = lua.GetInt32("ItemCount");
						list.Add(new MiscItemUnit(@int, int2));
						lua.CloseTable();
					}
					ReqMiscItems = list.ToArray();
				}
			}
		}
	}

	public readonly struct MasterMigrationType
	{
		public TimeSpan ReceiverLoginPrecondition { get; }

		public TimeSpan AutoMigrationDuration { get; }

		public MasterMigrationType(NKMLua lua)
		{
			using (lua.OpenTable("MasterMigration", "Creation table open failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 350))
			{
				int @int = lua.GetInt32("ReceiverPrecondition_LoginDays");
				ReceiverLoginPrecondition = TimeSpan.FromDays(@int);
				@int = lua.GetInt32("AutoMigrationDuration_LoginDays");
				AutoMigrationDuration = TimeSpan.FromDays(@int);
			}
		}

		internal void Validate()
		{
			if (ReceiverLoginPrecondition < TimeSpan.Zero || ReceiverLoginPrecondition > TimeSpan.FromDays(30.0))
			{
				NKMTempletError.Add($"[Guild] ReceiverLoginPrecondition - invalid value:{ReceiverLoginPrecondition}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 367);
			}
			if (AutoMigrationDuration < TimeSpan.Zero || AutoMigrationDuration > TimeSpan.FromDays(30.0))
			{
				NKMTempletError.Add($"[Guild] AutoMigrationDuration - invalid value:{AutoMigrationDuration}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 372);
			}
		}
	}

	public readonly struct MailText
	{
		public string Title { get; }

		public string Message { get; }

		public MailText(NKMLua lua, string prefix)
		{
			Title = lua.GetString(prefix + "Title");
			Message = lua.GetString(prefix + "Message");
			if (string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Message))
			{
				NKMTempletError.Add("[Guild] mail text is empty. prefix:" + prefix, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 386);
			}
		}
	}

	public const int GuildRankRefreshMinutes = 3;

	public const int MaxGuildRankCount = 100;

	public const int MaxJoinReceiveCount = 100;

	public const int MinGUildNameByte = 2;

	public const int MaxGUildNameByte = 16;

	public const int MaxChatMessageCount = 128;

	public const int MaxLogMessageCount = 128;

	public const int MaxChatTextCount = 70;

	public const int GuildExpMiscId = 503;

	public const int WelfarePointMiscId = 23;

	public const int UnionPointMiscId = 24;

	public static readonly TimeSpan NoticeCooltime = TimeSpan.FromMinutes(1.0);

	private int closingDelayHour;

	private int exitPenaltyHour;

	public CreationType Creation { get; private set; }

	public TimeSpan ClosingDelaySpan { get; private set; }

	public int AttendanceExp { get; private set; }

	public TimeSpan ExitPenaltyDuration { get; private set; }

	public int MaxJoinRequestCount { get; private set; }

	public int MaxInviteCount { get; private set; }

	public int MaxStaffCount { get; private set; }

	public int DailyDonationCount { get; private set; }

	public int WelfarePointBuyAmount { get; private set; }

	public long WelfarePointBuyLimit { get; private set; }

	public long WelfarePointPrice { get; private set; }

	public int ChatComplainCountToBlock { get; private set; }

	public int ChatAutoBlockHour { get; private set; }

	public MasterMigrationType MasterMigration { get; private set; }

	public MailText PromotionMailText { get; private set; }

	public MailText DemotionMailText { get; private set; }

	public MailText MasterMigrationMailText { get; private set; }

	public MailText BanMailText { get; private set; }

	public MailText AutoMigrationOldMasterMailText { get; private set; }

	public MailText AutoMigrationNewMasterMailText { get; private set; }

	public string SystemChatJoin { get; private set; }

	public string SystemChatExit { get; private set; }

	public string SystemChatBan { get; private set; }

	public string SystemChatPromotion { get; private set; }

	public string SystemChatLevelUp { get; private set; }

	public string SystemChatMasterMigration { get; private set; }

	public string SystemChatNotice { get; private set; }

	public string SystemChatNoticeDungeon { get; private set; }

	public string SystemChatRename { get; private set; }

	public int FirstJoinMiscItemId { get; private set; }

	public int FirstJoinMiscItemCount { get; private set; }

	public string FirstJoinPostTitle { get; private set; } = string.Empty;

	public string FirstJoinPostContent { get; private set; } = string.Empty;

	public int FirstJoinPostExpireDay { get; private set; } = 1;

	public int ConsortiumNameChangeFree { get; private set; } = 1;

	public int ConsortiumNameChangeLimitDay { get; private set; } = 7;

	public int ConsortiumNameChangeResourceItemId { get; private set; } = 24;

	public long ConsortiumNameChangeResourceValue { get; private set; } = 400000L;

	public static int GetMaxRankingCount(LeaderBoardRangeType rangeType)
	{
		return rangeType switch
		{
			LeaderBoardRangeType.ALL => 100, 
			LeaderBoardRangeType.TOP10 => 10, 
			_ => 0, 
		};
	}

	public void LoadFromLua(NKMLua lua)
	{
		Creation = new CreationType(lua);
		closingDelayHour = lua.GetInt32("ClosingDelayHour");
		ClosingDelaySpan = TimeSpan.FromHours(closingDelayHour);
		AttendanceExp = lua.GetInt32("AttendanceExp");
		exitPenaltyHour = lua.GetInt32("ExitPenaltyHour");
		ExitPenaltyDuration = TimeSpan.FromHours(exitPenaltyHour);
		MaxJoinRequestCount = lua.GetInt32("MaxJoinRequestCount");
		MaxInviteCount = lua.GetInt32("MaxInviteCount");
		MaxStaffCount = lua.GetInt32("MaxStaffCount");
		DailyDonationCount = lua.GetInt32("DailyDonationCount");
		WelfarePointBuyAmount = lua.GetInt32("WelfarePointBuyAmount");
		WelfarePointBuyLimit = lua.GetInt32("WelfarePointBuyLimit");
		WelfarePointPrice = lua.GetInt32("WelfarePointPrice");
		ChatComplainCountToBlock = lua.GetInt32("ChatComplainCountToBlock");
		ChatAutoBlockHour = lua.GetInt32("ChatAutoBlockHour");
		ConsortiumNameChangeFree = lua.GetInt32("ConsortiumNameChangeFree");
		ConsortiumNameChangeLimitDay = lua.GetInt32("ConsortiumNameChangeLimitDay");
		ConsortiumNameChangeResourceItemId = lua.GetInt32("ConsortiumNameChangeResourceItemID");
		ConsortiumNameChangeResourceValue = lua.GetInt64("ConsortiumNameChangeResourceValue");
		MasterMigration = new MasterMigrationType(lua);
		using (lua.OpenTable("NotifyMailText", "NotifyMailText table open failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 113))
		{
			PromotionMailText = new MailText(lua, "Promotion");
			DemotionMailText = new MailText(lua, "Demotion");
			MasterMigrationMailText = new MailText(lua, "MasterMigration");
			BanMailText = new MailText(lua, "Ban");
			AutoMigrationNewMasterMailText = new MailText(lua, "AutoMigrationNewMaster");
			AutoMigrationOldMasterMailText = new MailText(lua, "AutoMigrationOldMaster");
		}
		using (lua.OpenTable("NotifyChatText", "NotifyChatText table open failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 123))
		{
			SystemChatJoin = lua.GetString("MessageJoin");
			SystemChatExit = lua.GetString("MessageExit");
			SystemChatBan = lua.GetString("MessageBan");
			SystemChatPromotion = lua.GetString("MessagePromotion");
			SystemChatLevelUp = lua.GetString("MessageLevelUp");
			SystemChatMasterMigration = lua.GetString("MessageMasterMigration");
			SystemChatNotice = lua.GetString("MessageModifyNotificationMain");
			SystemChatNoticeDungeon = lua.GetString("MessageModifyNotificationDungeon");
			SystemChatRename = lua.GetString("MessageModifyNotificationRename");
		}
		using (lua.OpenTable("FirstJoinReward", "FirstJoinReward table opend failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 136))
		{
			FirstJoinMiscItemId = lua.GetInt32("MiscItemId");
			FirstJoinMiscItemCount = lua.GetInt32("MiscItemCount");
			FirstJoinPostTitle = lua.GetString("PostTitle");
			FirstJoinPostContent = lua.GetString("PostContent");
			FirstJoinPostExpireDay = lua.GetInt32("PostExpireDay");
		}
	}

	public void Join()
	{
		MiscItemUnit[] reqMiscItems = Creation.ReqMiscItems;
		for (int i = 0; i < reqMiscItems.Length; i++)
		{
			reqMiscItems[i].Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 150);
		}
	}

	public void Validate()
	{
		if (ExitPenaltyDuration < TimeSpan.Zero || ExitPenaltyDuration > TimeSpan.FromDays(10.0))
		{
			NKMTempletError.Add($"[Guild] ExitPenaltyHour - invalid value:{ExitPenaltyDuration}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 158);
		}
		if (Creation.UserMinLevel < 1)
		{
			NKMTempletError.Add($"[Guild] Creation.UserMinLevel - invalid value:{Creation.UserMinLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 163);
		}
		if (Creation.ReqMiscItems.Length == 0)
		{
			NKMTempletError.Add("[Guild] Creation.ReqMiscItems - empty", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 168);
		}
		if ((from e in Creation.ReqMiscItems
			group e by e.ItemId).Any((IGrouping<int, MiscItemUnit> e) => e.Count() > 1))
		{
			NKMTempletError.Add("[Guild] Creation.ReqMiscItems - duplicated itemId", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 173);
		}
		if (ClosingDelaySpan <= TimeSpan.Zero || ClosingDelaySpan > TimeSpan.FromDays(10.0))
		{
			NKMTempletError.Add($"[Guild] ClosingDelayHour - invalid value:{ClosingDelaySpan}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 178);
		}
		if (AttendanceExp <= 0 || AttendanceExp > 100000)
		{
			NKMTempletError.Add($"[Guild] AttendanceExp - not in valid range:{AttendanceExp}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 183);
		}
		string[] array = new string[6] { SystemChatJoin, SystemChatExit, SystemChatBan, SystemChatPromotion, SystemChatLevelUp, SystemChatMasterMigration };
		for (int num = 0; num < array.Length; num++)
		{
			if (string.IsNullOrEmpty(array[num]))
			{
				NKMTempletError.Add("[Guild] NotifyChatText - invalid string key", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 200);
			}
		}
		if (MaxJoinRequestCount <= 0 || MaxJoinRequestCount > 10)
		{
			NKMTempletError.Add($"[Guild] MaxJoinRequestCount - invalid value:{MaxJoinRequestCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 206);
		}
		if (MaxInviteCount <= 0 || MaxInviteCount > 50)
		{
			NKMTempletError.Add($"[Guild] MaxInviteCount - invalid value:{MaxInviteCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 211);
		}
		if (MaxStaffCount <= 0 || MaxStaffCount > 10)
		{
			NKMTempletError.Add($"[Guild] MaxStaffCount - invalid value:{MaxStaffCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 216);
		}
		if (DailyDonationCount <= 0 || DailyDonationCount > 10)
		{
			NKMTempletError.Add($"[Guild] DailyDonationCount - invalid value:{DailyDonationCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 221);
		}
		if (WelfarePointBuyAmount <= 0 || WelfarePointBuyAmount > 100)
		{
			NKMTempletError.Add($"[Guild] WelfarePointBuyAmount - invalid value:{WelfarePointBuyAmount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 226);
		}
		if (WelfarePointBuyLimit <= 0 || WelfarePointBuyLimit > 10000000)
		{
			NKMTempletError.Add($"[Guild] WelfarePointBuyLimit - invalid value:{WelfarePointBuyLimit}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 231);
		}
		if (WelfarePointPrice <= 0 || WelfarePointPrice > 100)
		{
			NKMTempletError.Add($"[Guild] WelfarePointPrice - invalid value:{WelfarePointPrice}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 236);
		}
		if (ChatComplainCountToBlock <= 0 || ChatComplainCountToBlock > 100)
		{
			NKMTempletError.Add($"[Guild] ChatComplainCountToBlock - invalid value:{ChatComplainCountToBlock}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 241);
		}
		if (ChatAutoBlockHour <= 0 || ChatAutoBlockHour > 100)
		{
			NKMTempletError.Add($"[Guild] ChatAutoBlockHour - invalid value:{ChatAutoBlockHour}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 246);
		}
		MasterMigration.Validate();
		if (NKMItemManager.GetItemMiscTempletByID(503) == null)
		{
			NKMTempletError.Add($"[Guild] 길드경험치 데이터 확인 불가. itemId:{503}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 253);
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(23);
		if (itemMiscTempletByID == null)
		{
			NKMTempletError.Add($"[Guild] 복지포인트 데이터 확인 불가. itemId:{23}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 259);
		}
		else if (itemMiscTempletByID.m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_MISC)
		{
			NKMTempletError.Add($"[Guild] 복지포인트 MiscType 오류. itemId:{23} miscType:{itemMiscTempletByID.m_ItemMiscType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 263);
		}
		itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(24);
		if (itemMiscTempletByID == null)
		{
			NKMTempletError.Add($"[Guild] 연합포인트 데이터 확인 불가. itemId:{24}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 269);
		}
		else if (itemMiscTempletByID.m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_VIEW)
		{
			NKMTempletError.Add($"[Guild] 연합포인트 MiscType 오류. itemId:{24} miscType:{itemMiscTempletByID.m_ItemMiscType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 273);
		}
		if (ConsortiumNameChangeFree != 1)
		{
			NKMTempletError.Add($"[Guild] 컨소시엄 이름 변경 가능 횟수가 1회가 아닌 경우. ConsortiumNameChangeFree:{ConsortiumNameChangeFree}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 278);
		}
		if (ConsortiumNameChangeLimitDay <= 0)
		{
			NKMTempletError.Add($"[Guild] 컨소시엄 이름 변경에 필요한 제한 기한이 비정상적인 경우. ConsortiumNameChangeLimitDay:{ConsortiumNameChangeLimitDay}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 283);
		}
		if (ConsortiumNameChangeResourceItemId != 24)
		{
			NKMTempletError.Add($"[Guild] 컨소시엄 이름 변경에 필요한 재화가 연합공동기금 이외의 값일 경우 확인이 필요. ConsortiumNameChangeResourceItemId:{ConsortiumNameChangeResourceItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 288);
		}
		if (ConsortiumNameChangeResourceValue <= 0)
		{
			NKMTempletError.Add($"[Guild] 컨소시엄 이름 변경에 필요한 재화 개수가 0 이하로 내려갈 수 없음. ConsortiumNameChangeResourceValue:{ConsortiumNameChangeResourceValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildTemplet.cs", 293);
		}
	}

	public void CheatClosingDelaySpan(TimeSpan closingDelaySpan)
	{
		ClosingDelaySpan = closingDelaySpan;
	}

	public void ResetClosingDelaySpan()
	{
		ClosingDelaySpan = TimeSpan.FromHours(closingDelayHour);
	}

	public void CheatExitPenaltyDuration(TimeSpan exitPenaltySpan)
	{
		ExitPenaltyDuration = exitPenaltySpan;
	}

	public void ResetExitPenaltyDuration()
	{
		ExitPenaltyDuration = TimeSpan.FromHours(exitPenaltyHour);
	}
}
