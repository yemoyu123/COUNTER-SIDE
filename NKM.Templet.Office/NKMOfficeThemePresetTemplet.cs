using System.Collections.Generic;
using System.Linq;
using ClientPacket.Office;
using Cs.Logging;
using Cs.Protocol;
using NKM.Templet.Base;

namespace NKM.Templet.Office;

public class NKMOfficeThemePresetTemplet : INKMTemplet
{
	private NKMOfficePreset officePreset;

	public int IDX;

	public string ThemaPresetID;

	public string ThemaPresetStringID;

	public string ThemaPresetDescID;

	public string ThemaPresetIMG;

	public string OpenTag;

	public bool AlwaysAppearOnList = true;

	public const string THEMA_PRESET_BUNDLE = "AB_INVEN_ICON_FNC_THEME";

	public bool EnableByTag => NKMOpenTagManager.IsOpened(OpenTag);

	public NKMOfficePreset OfficePreset => officePreset;

	public int Key => IDX;

	public NKMOfficeInteriorTemplet FloorInterior { get; private set; } = NKMCommonConst.Office.DefaultFloorItem;

	public NKMOfficeInteriorTemplet WallInterior { get; private set; } = NKMCommonConst.Office.DefaultWallItem;

	public NKMOfficeInteriorTemplet BackgroundInterior { get; private set; } = NKMCommonConst.Office.DefaultBackgroundItem;

	public IEnumerable<NKMOfficeInteriorTemplet> AllInteriors
	{
		get
		{
			if (FloorInterior != null)
			{
				yield return FloorInterior;
			}
			if (WallInterior != null)
			{
				yield return WallInterior;
			}
			if (BackgroundInterior != null)
			{
				yield return BackgroundInterior;
			}
			HashSet<int> hsVisitedFurniture = new HashSet<int>();
			foreach (NKMOfficeFurniture furniture in officePreset.furnitures)
			{
				if (furniture != null && !hsVisitedFurniture.Contains(furniture.itemId))
				{
					hsVisitedFurniture.Add(furniture.itemId);
					NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMOfficeInteriorTemplet.Find(furniture.itemId);
					if (nKMOfficeInteriorTemplet != null)
					{
						yield return nKMOfficeInteriorTemplet;
					}
				}
			}
		}
	}

	public IReadOnlyList<NKMOfficeFurniture> Furnitures => officePreset.furnitures;

	public static NKMOfficeThemePresetTemplet Find(int idx)
	{
		return NKMTempletContainer<NKMOfficeThemePresetTemplet>.Find(idx);
	}

	public static NKMOfficeThemePresetTemplet Find(string strID)
	{
		return NKMTempletContainer<NKMOfficeThemePresetTemplet>.Find(strID);
	}

	public static NKMOfficeThemePresetTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeThemePresetTemplet.cs", 68))
		{
			return null;
		}
		NKMOfficeThemePresetTemplet nKMOfficeThemePresetTemplet = new NKMOfficeThemePresetTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("IDX", ref nKMOfficeThemePresetTemplet.IDX);
		flag &= cNKMLua.GetData("ThemaPresetID", ref nKMOfficeThemePresetTemplet.ThemaPresetID);
		flag &= cNKMLua.GetData("ThemaPresetStringID", ref nKMOfficeThemePresetTemplet.ThemaPresetStringID);
		flag &= cNKMLua.GetData("ThemaPresetDescID", ref nKMOfficeThemePresetTemplet.ThemaPresetDescID);
		flag &= cNKMLua.GetData("ThemaPresetIMG", ref nKMOfficeThemePresetTemplet.ThemaPresetIMG);
		string rValue = "";
		flag &= cNKMLua.GetData("ThemaPresetExportID", ref rValue);
		cNKMLua.GetData("AlwaysAppearOnList", ref nKMOfficeThemePresetTemplet.AlwaysAppearOnList);
		cNKMLua.GetData("OpenTag", ref nKMOfficeThemePresetTemplet.OpenTag);
		if (flag)
		{
			try
			{
				nKMOfficeThemePresetTemplet.officePreset = new NKMOfficePreset();
				nKMOfficeThemePresetTemplet.officePreset.FromBase64(rValue);
				nKMOfficeThemePresetTemplet.officePreset.presetId = nKMOfficeThemePresetTemplet.Key;
			}
			catch
			{
				Log.Error("[NKMOfficeThemePresetTemplet] OfficePreset Decoding failed. preset = " + rValue, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeThemePresetTemplet.cs", 97);
				nKMOfficeThemePresetTemplet.officePreset = null;
			}
		}
		if (!flag)
		{
			return null;
		}
		return nKMOfficeThemePresetTemplet;
	}

	public void Join()
	{
		if (officePreset != null)
		{
			FloorInterior = NKMOfficeInteriorTemplet.Find(officePreset.floorInteriorId);
			if (FloorInterior == null)
			{
				Log.Error($"[OfficeThema.{IDX}] 바닥 가구 Id 값이 잘못됨. backgroundId:{officePreset.floorInteriorId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeThemePresetTemplet.cs", 113);
				FloorInterior = NKMCommonConst.Office.DefaultFloorItem;
			}
			WallInterior = NKMOfficeInteriorTemplet.Find(officePreset.wallInteriorId);
			if (WallInterior == null)
			{
				Log.Error($"[OfficeThema.{IDX}] 벽 가구 Id 값이 잘못됨. backgroundId:{officePreset.wallInteriorId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeThemePresetTemplet.cs", 120);
				WallInterior = NKMCommonConst.Office.DefaultWallItem;
			}
			BackgroundInterior = NKMOfficeInteriorTemplet.Find(officePreset.backgroundId);
			if (BackgroundInterior == null)
			{
				Log.Error($"[OfficeThema.{IDX}] 배경 가구 Id 값이 잘못됨. backgroundId:{officePreset.backgroundId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeThemePresetTemplet.cs", 127);
				BackgroundInterior = NKMCommonConst.Office.DefaultBackgroundItem;
			}
		}
	}

	public void Validate()
	{
		if (officePreset == null)
		{
			return;
		}
		foreach (IGrouping<int, NKMOfficeFurniture> item in (from e in officePreset.furnitures
			group e by e.itemId).ToList())
		{
			if (NKMOfficeInteriorTemplet.Find(item.Key) == null)
			{
				Log.Error($"[OfficeThema.{IDX}] 존재하지 않는 가구 ID. backgroundId:{item.Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeThemePresetTemplet.cs", 143);
			}
		}
	}
}
