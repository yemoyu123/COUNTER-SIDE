using NKM.Templet.Base;
using NKM.Templet.Office;

namespace NKM;

public sealed class NKMOfficeConst
{
	public sealed class NameCardConst
	{
		private int itemId;

		public NKMItemMiscTemplet ItemTemplet { get; private set; }

		public int ItemValue { get; private set; }

		public int DailyLimit { get; private set; }

		public void Load(NKMLua lua)
		{
			using (lua.OpenTable("OfficeHostNameCard", "[OfficeConst] loading OfficeHostNameCard table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOfficeConst.cs", 96))
			{
				itemId = lua.GetInt32("ItemId");
				ItemValue = lua.GetInt32("ItemValue");
				DailyLimit = lua.GetInt32("DayLimit");
			}
		}

		public void Join()
		{
			ItemTemplet = NKMItemMiscTemplet.Find(itemId);
			if (ItemTemplet == null)
			{
				NKMTempletError.Add($"[Office] 명함 수령시 교환 아이템 아이디가 유효하지 않음: {itemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOfficeConst.cs", 109);
			}
		}

		public void Validate()
		{
			if (ItemValue <= 0)
			{
				NKMTempletError.Add($"[Office] 명함 수령시 교환 아이템 개수가 유효하지 않음: {ItemValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOfficeConst.cs", 117);
			}
			if (DailyLimit <= 0 || DailyLimit > 100)
			{
				NKMTempletError.Add($"[Office] 수령 가능 명함 일일 제한 수량이 유효하지 않음: {DailyLimit}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOfficeConst.cs", 122);
			}
		}
	}

	public sealed class OfficeInteractionConst
	{
		public int ActInteriorRatePercent { get; private set; }

		public float ActInteriorCoolTime { get; private set; }

		public int ActUnitRatePercent { get; private set; }

		public float ActUnitCoolTime { get; private set; }

		public int ActSoloRatePercent { get; private set; }

		public float ActSoloCoolTime { get; private set; }

		public int RoomEnterActRatePercent { get; private set; }

		public void Load(NKMLua lua)
		{
			using (lua.OpenTable("OfficeInteraction", "[OfficeConst] loading OfficeInteraction table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOfficeConst.cs", 139))
			{
				ActInteriorRatePercent = lua.GetInt32("ActInteriorRatePercent");
				ActInteriorCoolTime = lua.GetFloat("ActInteriorCoolTime");
				ActUnitRatePercent = lua.GetInt32("ActUnitRatePercent");
				ActUnitCoolTime = lua.GetFloat("ActUnitCoolTime");
				ActSoloRatePercent = lua.GetInt32("ActSoloUnitRatePercent");
				ActSoloCoolTime = lua.GetFloat("ActSoloUnitCoolTime");
				RoomEnterActRatePercent = lua.GetInt32("RoomEnterActRatePercent");
			}
		}
	}

	public sealed class OfficePresetConst
	{
		private int baseCount;

		private int maxCount;

		private int expandCostValue;

		private int expandCostItemId = 101;

		public int BaseCount => baseCount;

		public int MaxNameLength => 20;

		public NKMItemMiscTemplet ExpandCostItem { get; private set; }

		public int MaxCount => maxCount;

		public int ExpandCostValue => expandCostValue;

		public void Load(NKMLua lua)
		{
			using (lua.OpenTable("OfficeUserPreset", "[OfficeConst] open subTable 'OfficeUserPreset' failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOfficeConst.cs", 167))
			{
				baseCount = lua.GetInt32("FREE_PRESET");
				maxCount = lua.GetInt32("MAX_PRESET");
				expandCostValue = lua.GetInt32("PRESET_PRICE_QUARTZ");
			}
		}

		public void Join()
		{
			ExpandCostItem = NKMItemMiscTemplet.Find(expandCostItemId);
			if (ExpandCostItem == null)
			{
				NKMTempletError.Add($"[Office] 기숙사 확장 아이템 아이디가 유효하지 않음: {expandCostItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOfficeConst.cs", 180);
			}
		}

		public void Validate()
		{
			if (expandCostValue <= 0)
			{
				NKMTempletError.Add($"[Office] 기숙사 확장 아이템 개수가 유효하지 않음: {expandCostValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOfficeConst.cs", 188);
			}
		}
	}

	private int backgroundItemId;

	private int wallItemId;

	private int floorItemId;

	private int officeNamingLimit;

	private int partyUseResourceId;

	private int partyMaxRefillCount;

	public OfficeInteractionConst OfficeInteraction = new OfficeInteractionConst();

	public const int NAMECARD_SEND_DAILY_LIMIT = 5;

	public const float OFFICE_INTERACTION_CHECK_TIME = 1f;

	public const int OFFICE_START_INTERACTION_PROBABILITY = 100;

	public NKMOfficeInteriorTemplet DefaultBackgroundItem { get; private set; } = NKMOfficeInteriorTemplet.Invalid;

	public NKMOfficeInteriorTemplet DefaultWallItem { get; private set; } = NKMOfficeInteriorTemplet.Invalid;

	public NKMOfficeInteriorTemplet DefaultFloorItem { get; private set; } = NKMOfficeInteriorTemplet.Invalid;

	public NKMItemMiscTemplet PartyUseItem { get; private set; }

	public int PartyUseItemMaxRefillCount => partyMaxRefillCount;

	public int OfficeNamingLimit => officeNamingLimit;

	public int MaxPostCountPerPage => 50;

	public NameCardConst NameCard { get; } = new NameCardConst();

	public OfficePresetConst PresetConst { get; } = new OfficePresetConst();

	public void Load(NKMLua lua)
	{
		using (lua.OpenTable("Office", "[OfficeConst] loading Office table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOfficeConst.cs", 32))
		{
			backgroundItemId = lua.GetInt32("OfficeDefaultBackground");
			wallItemId = lua.GetInt32("OfficeDefaultWall");
			floorItemId = lua.GetInt32("OfficeDefaultFloor");
			officeNamingLimit = lua.GetInt32("OfficeNamingLimit");
			NameCard.Load(lua);
			OfficeInteraction.Load(lua);
			PresetConst.Load(lua);
			using (lua.OpenTable("OfficeParty", "[OfficeConst] open subTable 'OfficeParty' failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOfficeConst.cs", 42))
			{
				partyUseResourceId = lua.GetInt32("UseResourceId");
				partyMaxRefillCount = lua.GetInt32("MaxCount");
			}
		}
	}

	public void Join()
	{
		DefaultBackgroundItem = NKMOfficeInteriorTemplet.Find(backgroundItemId);
		if (DefaultBackgroundItem == null)
		{
			NKMTempletError.Add($"[Office] 기본 배경 아이템 아이디가 유효하지 않음: {backgroundItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOfficeConst.cs", 55);
		}
		DefaultWallItem = NKMOfficeInteriorTemplet.Find(wallItemId);
		if (DefaultWallItem == null)
		{
			NKMTempletError.Add($"[Office] 기본 벽지 아이템 아이디가 유효하지 않음: {wallItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOfficeConst.cs", 61);
		}
		DefaultFloorItem = NKMOfficeInteriorTemplet.Find(floorItemId);
		if (DefaultFloorItem == null)
		{
			NKMTempletError.Add($"[Office] 기본 바닥 아이템 아이디가 유효하지 않음: {floorItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOfficeConst.cs", 67);
		}
		NameCard.Join();
		PartyUseItem = NKMItemMiscTemplet.Find(partyUseResourceId);
		if (PartyUseItem == null)
		{
			NKMTempletError.Add($"[Office] 회식 사용 아이템 아이디가 유효하지 않음: {partyUseResourceId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOfficeConst.cs", 75);
		}
		PresetConst.Join();
	}

	public void Validate()
	{
		NameCard.Validate();
		PresetConst.Validate();
	}
}
