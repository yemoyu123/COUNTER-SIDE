using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM;

public class NKMDETempletManager
{
	public static Dictionary<string, NKMDamageEffectTemplet> m_dicNKMDamageEffectTemplet = new Dictionary<string, NKMDamageEffectTemplet>();

	private NKMDETempletManager()
	{
	}

	public static bool LoadFromLUA(List<string> listFileName, bool bReload)
	{
		bool result = true;
		foreach (string item in listFileName)
		{
			LoadFromLUA(item, bReload);
		}
		return result;
	}

	private static void LoadFromLUA(string fileName, bool bReload)
	{
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath("AB_SCRIPT_EFFECT", fileName) || !nKMLua.OpenTable("m_dicNKMDamageEffectTemplet"))
		{
			return;
		}
		int num = 1;
		while (nKMLua.OpenTable(num))
		{
			NKMDamageEffectTemplet nKMDamageEffectTemplet = new NKMDamageEffectTemplet();
			string rValue = "";
			if (nKMLua.GetData("m_BASE_ID", ref rValue) && rValue.Length > 1)
			{
				if (m_dicNKMDamageEffectTemplet.ContainsKey(rValue))
				{
					nKMDamageEffectTemplet.DeepCopyFromSource(m_dicNKMDamageEffectTemplet[rValue]);
					nKMDamageEffectTemplet.LoadFromLUA(nKMLua);
				}
				else
				{
					NKMTempletError.Add("NKMDETempletManager LoadFromLUA m_BASE_ID dont exist m_DamageEffectID: " + nKMDamageEffectTemplet.m_DamageEffectID + ", m_BASE_ID: " + rValue, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffectTemplet.cs", 507);
				}
			}
			else
			{
				nKMDamageEffectTemplet.LoadFromLUA(nKMLua);
			}
			if (!m_dicNKMDamageEffectTemplet.ContainsKey(nKMDamageEffectTemplet.m_DamageEffectID))
			{
				m_dicNKMDamageEffectTemplet.Add(nKMDamageEffectTemplet.m_DamageEffectID, nKMDamageEffectTemplet);
			}
			else if (bReload)
			{
				m_dicNKMDamageEffectTemplet[nKMDamageEffectTemplet.m_DamageEffectID].DeepCopyFromSource(nKMDamageEffectTemplet);
			}
			else
			{
				NKMTempletError.Add("NKMDETempletManager LoadFromLUA duplicate TempletID m_DamageEffectID: " + nKMDamageEffectTemplet.m_DamageEffectID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffectTemplet.cs", 526);
			}
			num++;
			nKMLua.CloseTable();
		}
	}

	public static NKMDamageEffectTemplet GetDETemplet(string deID)
	{
		if (m_dicNKMDamageEffectTemplet.ContainsKey(deID))
		{
			return m_dicNKMDamageEffectTemplet[deID];
		}
		NKMTempletError.Add("GetDETemplet null: " + deID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffectTemplet.cs", 543);
		return null;
	}
}
