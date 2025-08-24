using NKM;
using NKM.Templet.Base;

namespace NKC;

public class NKCCurrencyTemplet : INKMTemplet
{
	public int m_Type;

	public string m_Code;

	public int Key => m_Type;

	public static NKCCurrencyTemplet LoadFromLua(NKMLua cNKMLua)
	{
		NKCCurrencyTemplet nKCCurrencyTemplet = new NKCCurrencyTemplet();
		if ((1u & (cNKMLua.GetData("m_Type", ref nKCCurrencyTemplet.m_Type) ? 1u : 0u) & (cNKMLua.GetData("m_Code", ref nKCCurrencyTemplet.m_Code) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKCCurrencyTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
