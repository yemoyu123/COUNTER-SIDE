using System.Collections.Generic;
using System.Linq;
using NKM.Contract2;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMShipLimitBreakTemplet : INKMTemplet
{
	private int id;

	private NKM_UNIT_TYPE unitType;

	private int shipId;

	private int shipLimitBreakGrade;

	private int shipLimitBreakMaxLevel;

	private List<int> listMaterialShipId;

	private List<MiscItemUnit> shipLimitBreakItems = new List<MiscItemUnit>(NKMCommonConst.ShipLimitBreakItemCount);

	private string openTag;

	public NKMUnitTempletBase BaseShipTemplet { get; private set; }

	public int Key => id;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(openTag);

	public int ShipId => shipId;

	public NKM_UNIT_TYPE UnitType => unitType;

	public int ShipLimitBreakMaxLevel => shipLimitBreakMaxLevel;

	public List<int> ListMaterialShipId => listMaterialShipId;

	public int ShipLimitBreakGrade => shipLimitBreakGrade;

	public List<MiscItemUnit> ShipLimitBreakItems => shipLimitBreakItems;

	public static NKMShipLimitBreakTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 419))
		{
			return null;
		}
		NKMShipLimitBreakTemplet nKMShipLimitBreakTemplet = new NKMShipLimitBreakTemplet
		{
			id = lua.GetInt32("ID"),
			openTag = lua.GetString("OpenTag"),
			shipId = lua.GetInt32("ShipID"),
			shipLimitBreakGrade = lua.GetInt32("ShipLimitBreakGrade"),
			shipLimitBreakMaxLevel = lua.GetInt32("ShipLimitBreakMaxLevel")
		};
		lua.GetDataEnum<NKM_UNIT_TYPE>("UnitType", out nKMShipLimitBreakTemplet.unitType);
		for (int i = 0; i < NKMCommonConst.ShipLimitBreakItemCount; i++)
		{
			int rValue = -1;
			int rValue2 = -1;
			if ((1u & (lua.GetData($"ShipLimitBreakItemID{i + 1}", ref rValue) ? 1u : 0u) & (lua.GetData($"ShipLimitBreakItemValue{i + 1}", ref rValue2) ? 1u : 0u)) != 0)
			{
				nKMShipLimitBreakTemplet.shipLimitBreakItems.Add(new MiscItemUnit(rValue, rValue2));
			}
		}
		if (lua.OpenTable("ListMaterialShipID"))
		{
			nKMShipLimitBreakTemplet.listMaterialShipId = new List<int>();
			int j = 1;
			for (int rValue3 = 0; lua.GetData(j, ref rValue3); j++)
			{
				nKMShipLimitBreakTemplet.listMaterialShipId.Add(rValue3);
			}
			lua.CloseTable();
		}
		return nKMShipLimitBreakTemplet;
	}

	public void Join()
	{
		BaseShipTemplet = NKMTempletContainer<NKMUnitTempletBase>.Find(shipId);
		if (BaseShipTemplet == null)
		{
			NKMTempletError.Add($"[ShipLimitBreak:{shipId}] \ufffd߸\ufffd\ufffd\ufffd \ufffdԼ\ufffd Id. shipId:{shipId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 475);
		}
	}

	public void Validate()
	{
		if (unitType != NKM_UNIT_TYPE.NUT_SHIP)
		{
			NKMTempletError.Add($"[ShipLimitBreak:{shipId}] \ufffd߸\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd Ÿ\ufffd\ufffd. unitType:{unitType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 483);
		}
		if (shipLimitBreakGrade < 1)
		{
			NKMTempletError.Add($"[ShipLimitBreak:{shipId}] \ufffdԼ\ufffd \ufffd\u033d\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\u0730谡 \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd. shipLimitBreakGrade:{shipLimitBreakGrade}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 487);
		}
		if (shipLimitBreakMaxLevel < 0)
		{
			NKMTempletError.Add($"[ShipLimitBreak:{shipId}] \ufffdԼ\ufffd \ufffd\u033d\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\u05b4\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd. shipLimitBreakMaxLevel:{shipLimitBreakMaxLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 492);
		}
		if (!shipLimitBreakItems.Any())
		{
			NKMTempletError.Add($"[ShipLimitBreak:{shipId}] \ufffdԼ\ufffd \ufffd\u033d\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 497);
		}
		shipLimitBreakItems.ForEach(delegate(MiscItemUnit s)
		{
			s.Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 500);
		});
		int[] array = (from e in shipLimitBreakItems
			group e by e.ItemId into e
			where e.Count() > 1
			select e.Key).ToArray();
		if (array.Any())
		{
			string arg = string.Join(", ", array);
			NKMTempletError.Add($"[ShipLimitBreak:{shipId}] \ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd \ufffdߺ\ufffd\ufffdǴ\ufffd \ufffd\u05f8\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd. duplicateItemId:{arg}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 510);
		}
		foreach (int item in listMaterialShipId)
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMTempletContainer<NKMUnitTempletBase>.Find(item);
			if (nKMUnitTempletBase == null)
			{
				NKMTempletError.Add($"[ShipLimitBreak:{shipId}] \ufffd߸\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd \ufffdԼ\ufffd Id. materialShipId:{item}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 518);
			}
			if (nKMUnitTempletBase.m_ShipGroupID != BaseShipTemplet.m_ShipGroupID)
			{
				NKMTempletError.Add($"[ShipLimitBreak:{shipId}] \ufffd\ufffd\ufffd \ufffdԼ\ufffd\ufffd\ufffd \ufffd\u05f7\ufffd Id\ufffd\ufffd \ufffdٸ\ufffd. shipId:{shipId} baseShipGruipId:{BaseShipTemplet.m_ShipGroupID} materialShipId:{item} materialShipGroupID:{nKMUnitTempletBase.m_ShipGroupID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 523);
			}
		}
	}

	public bool ValidateConsumeShipId(int shipId)
	{
		return listMaterialShipId.Contains(shipId);
	}
}
