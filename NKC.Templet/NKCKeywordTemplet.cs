using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCKeywordTemplet : INKMTemplet
{
	private static int count;

	public int index;

	public string Key;

	public string Name;

	public string Desc;

	int INKMTemplet.Key => index;

	public static NKCKeywordTemplet Find(string key)
	{
		return NKMTempletContainer<NKCKeywordTemplet>.Find(key);
	}

	public static void Load()
	{
		NKMTempletContainer<NKCKeywordTemplet>.Load("AB_SCRIPT", "LUA_KEYWORD_TEMPLET", "KEYWORD_TEMPLET", LoadFromLua, (NKCKeywordTemplet t) => t.Key);
	}

	public static NKCKeywordTemplet LoadFromLua(NKMLua cNKMLua)
	{
		NKCKeywordTemplet nKCKeywordTemplet = new NKCKeywordTemplet();
		nKCKeywordTemplet.index = count;
		count++;
		if ((1u & (cNKMLua.GetData("Keyword", ref nKCKeywordTemplet.Key) ? 1u : 0u) & (cNKMLua.GetData("m_KeywordName", ref nKCKeywordTemplet.Name) ? 1u : 0u) & (cNKMLua.GetData("m_KeywordDesc", ref nKCKeywordTemplet.Desc) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKCKeywordTemplet;
	}

	public string GetTMPLinkString()
	{
		return $"<link=\"{Key}\"><SPRITE=0><u>{NKCStringTable.GetString(Name, null)}</u></link>";
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
