using NKM.Templet.Base;

namespace NKM.Templet.Office;

public sealed class NKMOfficeRoomTemplet : INKMTemplet
{
	public enum RoomType
	{
		Dorm,
		Lab,
		Forge,
		Hangar,
		CEO,
		Terrabrain
	}

	private int priceItemId;

	public int ID { get; private set; }

	public string Name { get; private set; }

	public int CellX { get; private set; }

	public int CellY { get; private set; }

	public int CellZ { get; private set; }

	public RoomType Type { get; private set; }

	public string FacilityPrefab { get; private set; }

	public string DefaultBGM { get; private set; }

	public int SectionId { get; private set; }

	public int UnitLimitCount { get; private set; }

	public NKMOfficeSectionTemplet SectionTemplet { get; private set; }

	public NKMItemMiscTemplet PriceItem { get; private set; }

	public int Price { get; private set; }

	public bool IsFacility => Type != RoomType.Dorm;

	public int Key => ID;

	public int LeftWallX => CellX;

	public int LeftWallY => CellZ;

	public int RightWallX => CellY;

	public int RightWallY => CellZ;

	public int FloorX => CellX;

	public int FloorY => CellY;

	public STAGE_UNLOCK_REQ_TYPE UnlockReqType { get; private set; }

	public int UnlockReqValue { get; private set; }

	public bool HasUnlockType
	{
		get
		{
			if (UnlockReqType != STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE)
			{
				return UnlockReqValue != 0;
			}
			return false;
		}
	}

	internal string DebugName => $"[OfficeRoom:{ID}/{Name}]";

	public static NKMOfficeRoomTemplet Find(int key)
	{
		return NKMTempletContainer<NKMOfficeRoomTemplet>.Find(key);
	}

	public static NKMOfficeRoomTemplet Find(RoomType key)
	{
		foreach (NKMOfficeRoomTemplet value in NKMTempletContainer<NKMOfficeRoomTemplet>.Values)
		{
			if (value.Type == key)
			{
				return value;
			}
		}
		return null;
	}

	public static NKMOfficeRoomTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeRoomTemplet.cs", 58))
		{
			return null;
		}
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = new NKMOfficeRoomTemplet
		{
			ID = lua.GetInt32("ID"),
			Name = lua.GetString("Name"),
			CellX = lua.GetInt32("CellX"),
			CellY = lua.GetInt32("CellY"),
			CellZ = lua.GetInt32("CellZ"),
			SectionId = lua.GetInt32("SectionID"),
			priceItemId = lua.GetInt32("PriceItemID", 0),
			Price = lua.GetInt32("Price", 0),
			UnitLimitCount = lua.GetInt32("UnitLimit"),
			Type = lua.GetEnum("Type", RoomType.Dorm),
			UnlockReqType = lua.GetEnum("UnlockReqType", STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE),
			UnlockReqValue = lua.GetInt32("UnlockReqValue", 0)
		};
		if (lua.GetData("FacilityPrefab", out var rValue, string.Empty))
		{
			nKMOfficeRoomTemplet.FacilityPrefab = rValue;
		}
		if (lua.GetData("DefaultBGM", out var rValue2, string.Empty))
		{
			nKMOfficeRoomTemplet.DefaultBGM = rValue2;
		}
		return nKMOfficeRoomTemplet;
	}

	public void Join()
	{
		if (priceItemId > 0)
		{
			PriceItem = NKMItemMiscTemplet.Find(priceItemId);
			if (PriceItem == null)
			{
				NKMTempletError.Add($"{DebugName} invalid priceItemId:{priceItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeRoomTemplet.cs", 99);
			}
		}
		SectionTemplet = NKMTempletContainer<NKMOfficeSectionTemplet>.Find(SectionId);
		if (SectionTemplet == null)
		{
			NKMTempletError.Add($"{DebugName} invalid sectionId:{SectionId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeRoomTemplet.cs", 106);
		}
		else
		{
			SectionTemplet.AddRoom(this);
		}
	}

	public void Validate()
	{
		if (IsFacility && string.IsNullOrEmpty(FacilityPrefab))
		{
			NKMTempletError.Add($"{DebugName} facility has no prefab data. roomType:{Type}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeRoomTemplet.cs", 118);
		}
		if (PriceItem != null && Price <= 0)
		{
			NKMTempletError.Add($"{DebugName} invalid price:{Price}.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeRoomTemplet.cs", 123);
		}
	}
}
