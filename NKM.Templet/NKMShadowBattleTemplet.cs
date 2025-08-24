using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMShadowBattleTemplet : INKMTemplet
{
	public int BATTLE_GROUP;

	public int BATTLE_ORDER;

	public PALACE_BATTLE_TYPE PALACE_BATTLE_TYPE;

	public int DUNGEON_ID;

	public string PALACE_BATTLE_IMG;

	public int Key => BATTLE_GROUP;

	public static NKMShadowBattleTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMShadowBattleTemplet.cs", 24))
		{
			return null;
		}
		NKMShadowBattleTemplet nKMShadowBattleTemplet = new NKMShadowBattleTemplet();
		if ((1u & (cNKMLua.GetData("BATTLE_GROUP", ref nKMShadowBattleTemplet.BATTLE_GROUP) ? 1u : 0u) & (cNKMLua.GetData("BATTLE_ORDER", ref nKMShadowBattleTemplet.BATTLE_ORDER) ? 1u : 0u) & (cNKMLua.GetData("PALACE_BATTLE_TYPE", ref nKMShadowBattleTemplet.PALACE_BATTLE_TYPE) ? 1u : 0u) & (cNKMLua.GetData("DUNGEON_ID", ref nKMShadowBattleTemplet.DUNGEON_ID) ? 1u : 0u) & (cNKMLua.GetData("PALACE_BATTLE_IMG", ref nKMShadowBattleTemplet.PALACE_BATTLE_IMG) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKMShadowBattleTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
