using System.Linq;
using ClientPacket.Office;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet.Office;

public sealed class NKMOfficeInteriorTemplet : NKMItemMiscTemplet
{
	private int refundItemId;

	public string InteractionGroupID;

	public float TargetRange;

	public bool ExclusiveBool;

	public bool Animation;

	public bool Effect;

	public string TouchSound;

	public float SoundDelay;

	public string RoamingUnitPrefab;

	public string UnitInteractionMotion;

	public string PlayBGM;

	public int BGMVolume;

	public static NKMOfficeInteriorTemplet Invalid { get; } = new NKMOfficeInteriorTemplet();

	public int Id => m_ItemMiscID;

	public InteriorCategory InteriorCategory { get; private set; }

	public InteriorTarget Target { get; private set; }

	public int CellX { get; private set; }

	public int CellY { get; private set; }

	public string PrefabName { get; private set; }

	public bool IsTexture { get; private set; }

	public int InteriorScore { get; private set; }

	public int MaxStack { get; private set; }

	public NKMItemMiscTemplet RefundItem { get; private set; }

	public long RefundItemPrice { get; private set; }

	public string GroupID { get; private set; }

	public bool HasInteraction => !string.IsNullOrEmpty(InteractionGroupID);

	public float GetBGMVolume
	{
		get
		{
			if (BGMVolume == 0)
			{
				return 1f;
			}
			return (float)BGMVolume * 0.01f;
		}
	}

	public bool HasSound => !string.IsNullOrEmpty(TouchSound);

	public bool HasBGM => !string.IsNullOrEmpty(PlayBGM);

	public new static NKMOfficeInteriorTemplet Find(int key)
	{
		return NKMTempletContainer<NKMOfficeInteriorTemplet>.Find(key);
	}

	public static NKMOfficeInteriorTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeInteriorTemplet.cs", 73))
		{
			return null;
		}
		NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = new NKMOfficeInteriorTemplet();
		if (!nKMOfficeInteriorTemplet.Load(lua))
		{
			return null;
		}
		return nKMOfficeInteriorTemplet;
	}

	public static void MergeContainer()
	{
		int num = NKMItemMiscTemplet.Values.Count();
		int num2 = NKMItemMiscTemplet.InteriorValues.Count();
		NKMTempletContainer<NKMItemMiscTemplet>.AddRange(NKMItemMiscTemplet.InteriorValues, (NKMItemMiscTemplet e) => e.m_ItemMiscStrID);
		int num3 = NKMItemMiscTemplet.Values.Count();
		Log.Debug($"[InteriorTemplet] miscCount {num} + interiorCount {num2} = {num3}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeInteriorTemplet.cs", 90);
	}

	public override void Join()
	{
		base.Join();
		RefundItem = NKMItemMiscTemplet.Find(refundItemId);
		if (RefundItem == null)
		{
			NKMTempletError.Add($"[Interior] invalid refundItemId:{refundItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeInteriorTemplet.cs", 100);
		}
	}

	public override void Validate()
	{
		base.Validate();
		if (m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_INTERIOR)
		{
			NKMTempletError.Add($"[Interior:{m_ItemMiscID}] invalid miscType:{m_ItemMiscType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeInteriorTemplet.cs", 110);
		}
		if (m_ItemMiscSubType != NKM_ITEM_MISC_SUBTYPE.IMST_INTERIOR_DECO && m_ItemMiscSubType != NKM_ITEM_MISC_SUBTYPE.IMST_INTERIOR_FURNITURE)
		{
			NKMTempletError.Add($"[Interior:{m_ItemMiscID}] invalid miscSubType:{m_ItemMiscSubType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeInteriorTemplet.cs", 116);
		}
		if (InteriorCategory == InteriorCategory.DECO && Target == InteriorTarget.Tile)
		{
			NKMTempletError.Add($"[Interior:{m_ItemMiscID}] Tile decoration not allowed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeInteriorTemplet.cs", 121);
		}
		if (InteriorCategory == InteriorCategory.FURNITURE)
		{
			if (Target == InteriorTarget.Background)
			{
				NKMTempletError.Add($"[Interior:{m_ItemMiscID}] Background Furniture not allowed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeInteriorTemplet.cs", 128);
			}
			if (CellX <= 0 || CellY <= 0)
			{
				NKMTempletError.Add($"[Interior:{m_ItemMiscID}] nonpositive cell size. x:{CellX} y:{CellY}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeInteriorTemplet.cs", 133);
			}
		}
		if (InteriorCategory == InteriorCategory.DECO && MaxStack != 1)
		{
			NKMTempletError.Add($"[Interior:{m_ItemMiscID}] DECO 타입은 하나 이상 stack 불가. maxStack:{MaxStack}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeInteriorTemplet.cs", 139);
		}
		else if (MaxStack <= 0)
		{
			NKMTempletError.Add($"[Interior:{m_ItemMiscID}] invalid maxStack:{MaxStack}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeInteriorTemplet.cs", 143);
		}
		if (HasInteraction && TargetRange <= 0f)
		{
			NKMTempletError.Add($"[Interior:{m_ItemMiscID}] invalid TargetRange:{TargetRange}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeInteriorTemplet.cs", 148);
		}
	}

	public bool PlacementEnable(OfficePlaneType planeType)
	{
		if (InteriorCategory != InteriorCategory.FURNITURE)
		{
			return false;
		}
		switch (Target)
		{
		case InteriorTarget.Floor:
			return planeType == OfficePlaneType.Floor;
		case InteriorTarget.Tile:
			return planeType == OfficePlaneType.Tile;
		case InteriorTarget.Wall:
			if (planeType != OfficePlaneType.LeftWall)
			{
				return planeType == OfficePlaneType.RightWall;
			}
			return true;
		default:
			Log.Error($"[Interior] invalid target:{Target}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeInteriorTemplet.cs", 172);
			return false;
		}
	}

	protected override bool Load(NKMLua lua)
	{
		if (!base.Load(lua))
		{
			return false;
		}
		Target = lua.GetEnum<InteriorTarget>("Target");
		CellX = lua.GetInt32("CellX");
		CellY = lua.GetInt32("CellY");
		PrefabName = lua.GetString("PrefabName");
		IsTexture = lua.GetBoolean("IsTexture", defaultValue: false);
		InteriorScore = lua.GetInt32("InteriorScore");
		MaxStack = lua.GetInt32("MaxStack");
		refundItemId = lua.GetInt32("RefundItemID");
		RefundItemPrice = lua.GetInt32("RefundItemPrice");
		InteractionGroupID = lua.GetString("InteractionGroupID", "");
		TargetRange = lua.GetFloat("TargetRange", 0f);
		ExclusiveBool = lua.GetBoolean("ExclusiveBool", defaultValue: false);
		Animation = lua.GetBoolean("Animation", defaultValue: false);
		Effect = lua.GetBoolean("Effect", defaultValue: false);
		TouchSound = lua.GetString("TouchSound", "");
		SoundDelay = lua.GetFloat("SoundDelay", 0f);
		RoamingUnitPrefab = lua.GetString("RoamingUnitPrefab", "");
		UnitInteractionMotion = lua.GetString("UnitInteractionMotion", "");
		GroupID = lua.GetString("GroupID", "");
		PlayBGM = lua.GetString("PlayBGM", "");
		BGMVolume = lua.GetInt32("BGMVolume", 100);
		switch (m_ItemMiscSubType)
		{
		case NKM_ITEM_MISC_SUBTYPE.IMST_INTERIOR_DECO:
			InteriorCategory = InteriorCategory.DECO;
			break;
		case NKM_ITEM_MISC_SUBTYPE.IMST_INTERIOR_FURNITURE:
			InteriorCategory = InteriorCategory.FURNITURE;
			break;
		}
		return true;
	}
}
