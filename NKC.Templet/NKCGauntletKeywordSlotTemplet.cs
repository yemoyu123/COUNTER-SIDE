using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCGauntletKeywordSlotTemplet : INKMTemplet
{
	private NKM_GAME_TYPE slotId;

	private int[] slotKeywordId = new int[6];

	private const int slotCount = 6;

	public int Key => (int)slotId;

	public int[] SlotKeywordId => slotKeywordId;

	public static NKCGauntletKeywordSlotTemplet Find(NKM_GAME_TYPE keywordId)
	{
		return NKMTempletContainer<NKCGauntletKeywordSlotTemplet>.Find((int)keywordId);
	}

	public static NKCGauntletKeywordSlotTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCGauntletKeywordTemplet.cs", 59))
		{
			return null;
		}
		NKCGauntletKeywordSlotTemplet nKCGauntletKeywordSlotTemplet = new NKCGauntletKeywordSlotTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("SlotID", ref nKCGauntletKeywordSlotTemplet.slotId);
		for (int i = 0; i < 6; i++)
		{
			string pszName = $"SLOT_{i + 1}";
			cNKMLua.GetData(pszName, out var rValue, 0);
			nKCGauntletKeywordSlotTemplet.slotKeywordId[i] = rValue;
		}
		if (!flag)
		{
			return null;
		}
		return nKCGauntletKeywordSlotTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
