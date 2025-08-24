using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCOfficeCharacterTemplet : INKMTemplet
{
	public enum eType
	{
		Unit,
		Skin
	}

	public eType Type;

	public int ID;

	public string PrefabAsset;

	public string BTAsset;

	public float WalkSpeed;

	public float RunSpeed;

	public bool IgnoreObstacles;

	public string Variables;

	public int Key => (int)(ID * 10 + Type);

	public static NKCOfficeCharacterTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCOfficeCharacterTemplet.cs", 29))
		{
			return null;
		}
		NKCOfficeCharacterTemplet nKCOfficeCharacterTemplet = new NKCOfficeCharacterTemplet();
		int num = (int)(1u & (cNKMLua.GetDataEnum<eType>("Type", out nKCOfficeCharacterTemplet.Type) ? 1u : 0u)) & (cNKMLua.GetData("ID", ref nKCOfficeCharacterTemplet.ID) ? 1 : 0);
		cNKMLua.GetData("PrefabAsset", ref nKCOfficeCharacterTemplet.PrefabAsset);
		cNKMLua.GetData("BTAsset", ref nKCOfficeCharacterTemplet.BTAsset);
		cNKMLua.GetData("WalkSpeed", ref nKCOfficeCharacterTemplet.WalkSpeed);
		cNKMLua.GetData("RunSpeed", ref nKCOfficeCharacterTemplet.RunSpeed);
		cNKMLua.GetData("IgnoreObstacles", ref nKCOfficeCharacterTemplet.IgnoreObstacles);
		cNKMLua.GetData("Variables", ref nKCOfficeCharacterTemplet.Variables);
		if (num == 0)
		{
			return null;
		}
		return nKCOfficeCharacterTemplet;
	}

	public static NKCOfficeCharacterTemplet Find(eType type, int ID)
	{
		return NKMTempletContainer<NKCOfficeCharacterTemplet>.Find((int)(ID * 10 + type));
	}

	public static NKCOfficeCharacterTemplet Find(int unitID, int skinID)
	{
		if (skinID != 0)
		{
			NKCOfficeCharacterTemplet nKCOfficeCharacterTemplet = Find(eType.Skin, skinID);
			if (nKCOfficeCharacterTemplet != null)
			{
				return nKCOfficeCharacterTemplet;
			}
		}
		return Find(eType.Unit, unitID);
	}

	public static NKCOfficeCharacterTemplet Find(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return null;
		}
		if (unitData.m_SkinID != 0)
		{
			NKCOfficeCharacterTemplet nKCOfficeCharacterTemplet = Find(eType.Skin, unitData.m_SkinID);
			if (nKCOfficeCharacterTemplet != null)
			{
				return nKCOfficeCharacterTemplet;
			}
		}
		return Find(eType.Unit, unitData.m_UnitID);
	}

	public void Join()
	{
	}

	public void Validate()
	{
		switch (Type)
		{
		case eType.Unit:
			if (NKMUnitManager.GetUnitTempletBase(ID) == null)
			{
				NKMTempletError.Add($"[NKCOfficeCharacterTemplet] invalid CharID:{ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCOfficeCharacterTemplet.cs", 93);
			}
			break;
		case eType.Skin:
			if (NKMSkinManager.GetSkinTemplet(ID) == null)
			{
				NKMTempletError.Add($"[NKCOfficeCharacterTemplet] invalid SkinID:{ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCOfficeCharacterTemplet.cs", 100);
			}
			break;
		}
	}
}
