using System.Collections.Generic;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMCommandModulePassiveTemplet : INKMTemplet
{
	private int id;

	private int cmdPassiveGroupId;

	private int ratio;

	private HashSet<NKM_UNIT_STYLE_TYPE> allowStyleTypes;

	private HashSet<NKM_UNIT_ROLE_TYPE> allowRoleTypes;

	private int statGroupId;

	public int Key => id;

	public int PassiveGroupId => cmdPassiveGroupId;

	public int StatGroupId => statGroupId;

	public int Ratio => ratio;

	public HashSet<NKM_UNIT_STYLE_TYPE> StyleTypes => allowStyleTypes;

	public HashSet<NKM_UNIT_ROLE_TYPE> RoleTypes => allowRoleTypes;

	public static NKMCommandModulePassiveTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 674))
		{
			return null;
		}
		NKMCommandModulePassiveTemplet nKMCommandModulePassiveTemplet = new NKMCommandModulePassiveTemplet
		{
			id = lua.GetInt32("ID"),
			cmdPassiveGroupId = lua.GetInt32("CMDPassiveGroupID"),
			ratio = lua.GetInt32("Ratio"),
			statGroupId = lua.GetInt32("StatGroupID")
		};
		nKMCommandModulePassiveTemplet.allowStyleTypes = new HashSet<NKM_UNIT_STYLE_TYPE>();
		if (lua.OpenTable("ListRangeSonAllowStyleType"))
		{
			NKM_UNIT_STYLE_TYPE result;
			for (int i = 1; lua.GetDataEnum<NKM_UNIT_STYLE_TYPE>(i, out result); i++)
			{
				nKMCommandModulePassiveTemplet.allowStyleTypes.Add(result);
			}
			lua.CloseTable();
		}
		nKMCommandModulePassiveTemplet.allowRoleTypes = new HashSet<NKM_UNIT_ROLE_TYPE>();
		if (lua.OpenTable("ListRangeSonAllowRoleType"))
		{
			int j = 1;
			for (NKM_UNIT_ROLE_TYPE result2 = NKM_UNIT_ROLE_TYPE.NURT_INVALID; lua.GetData(j, ref result2); j++)
			{
				nKMCommandModulePassiveTemplet.allowRoleTypes.Add(result2);
			}
			lua.CloseTable();
		}
		return nKMCommandModulePassiveTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (ratio < 0)
		{
			NKMTempletError.Add($"[NKMCommandModulePassive:{id}] ratio\ufffd\ufffd\ufffd\ufffd \ufffd\u033b\ufffd\ufffd\ufffd. ratio:{ratio}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 727);
		}
		if (NKMShipModuleGroupTemplet.GetStatListsByGroupId(statGroupId) == null)
		{
			NKMTempletError.Add($"[NKMCommandModulePassive:{id}] statGroupId\ufffd\ufffd\ufffd\ufffd CommandModuleRandomStatTemplet\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd. statGroupId:{statGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 732);
		}
	}
}
