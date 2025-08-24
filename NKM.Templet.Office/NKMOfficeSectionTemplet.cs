using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet.Office;

public sealed class NKMOfficeSectionTemplet : INKMTemplet
{
	private readonly Dictionary<int, NKMOfficeRoomTemplet> rooms = new Dictionary<int, NKMOfficeRoomTemplet>();

	private int priceItemId;

	public int Key => SectionId;

	public int Order { get; private set; }

	public string SectionName { get; private set; }

	public int SectionId { get; private set; }

	public NKMItemMiscTemplet PriceItem { get; private set; }

	public bool IsFacility { get; private set; }

	public int Price { get; private set; }

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

	public IReadOnlyDictionary<int, NKMOfficeRoomTemplet> Rooms => rooms;

	private string DebugName => $"[OfficeSection:{SectionId}/{SectionName}]";

	public static NKMOfficeSectionTemplet Find(int key)
	{
		return NKMTempletContainer<NKMOfficeSectionTemplet>.Find(key);
	}

	public static NKMOfficeSectionTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeSectionTemplet.cs", 31))
		{
			return null;
		}
		return new NKMOfficeSectionTemplet
		{
			SectionId = lua.GetInt32("SectionID"),
			Order = lua.GetInt32("Order"),
			SectionName = lua.GetString("SectionName"),
			priceItemId = lua.GetInt32("PriceItemID", 0),
			Price = lua.GetInt32("Price", 0),
			UnlockReqType = lua.GetEnum("UnlockReqType", STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE),
			UnlockReqValue = lua.GetInt32("UnlockReqValue", 0),
			IsFacility = lua.GetBoolean("IsFacility", defaultValue: false)
		};
	}

	public void Join()
	{
		if (priceItemId > 0)
		{
			PriceItem = NKMItemMiscTemplet.Find(priceItemId);
			if (PriceItem == null)
			{
				NKMTempletError.Add($"{DebugName} invalid priceItemId:{priceItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeSectionTemplet.cs", 56);
			}
		}
	}

	public void Validate()
	{
		if (PriceItem != null && Price <= 0)
		{
			NKMTempletError.Add($"{DebugName} invalid price:{Price}.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeSectionTemplet.cs", 65);
		}
	}

	internal void AddRoom(NKMOfficeRoomTemplet roomTemplet)
	{
		if (rooms.ContainsKey(roomTemplet.ID))
		{
			NKMTempletError.Add($"{DebugName} duplicated roomId:{roomTemplet.ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeSectionTemplet.cs", 73);
		}
		else if (roomTemplet.IsFacility && !IsFacility)
		{
			NKMTempletError.Add(DebugName + " 시설인 방을 시설이 아닌 섹션에 연결. room:" + roomTemplet.DebugName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeSectionTemplet.cs", 79);
		}
		else
		{
			rooms.Add(roomTemplet.ID, roomTemplet);
		}
	}
}
