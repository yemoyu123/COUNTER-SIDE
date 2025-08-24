using NKC;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMEmoticonTemplet : INKMTemplet
{
	public static readonly NKMEmoticonTemplet Invalid = new NKMEmoticonTemplet
	{
		m_EmoticonID = 0
	};

	public int m_EmoticonID;

	public string m_EmoticonStrID;

	public NKM_EMOTICON_TYPE m_EmoticonType;

	public NKM_EMOTICON_GRADE m_EmoticonGrade;

	public string m_EmoticonName;

	public string m_EmoticonDesc;

	public string m_EmoticonAssetName;

	public string m_EmoticonaAnimationName;

	public string m_EmoticonaIconName;

	public string m_EmoticonSound;

	public int Key => m_EmoticonID;

	public static NKMEmoticonTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMEmoticonTemplet nKMEmoticonTemplet = new NKMEmoticonTemplet();
		cNKMLua.GetData("m_EmoticonID", ref nKMEmoticonTemplet.m_EmoticonID);
		cNKMLua.GetData("m_EmoticonStrID", ref nKMEmoticonTemplet.m_EmoticonStrID);
		cNKMLua.GetData("m_EmoticonType", ref nKMEmoticonTemplet.m_EmoticonType);
		cNKMLua.GetData("m_EmoticonGrade", ref nKMEmoticonTemplet.m_EmoticonGrade);
		cNKMLua.GetData("m_EmoticonName", ref nKMEmoticonTemplet.m_EmoticonName);
		cNKMLua.GetData("m_EmoticonDesc", ref nKMEmoticonTemplet.m_EmoticonDesc);
		cNKMLua.GetData("m_EmoticonAssetName", ref nKMEmoticonTemplet.m_EmoticonAssetName);
		cNKMLua.GetData("m_EmoticonaAnimationName", ref nKMEmoticonTemplet.m_EmoticonaAnimationName);
		cNKMLua.GetData("m_EmoticonaIconName", ref nKMEmoticonTemplet.m_EmoticonaIconName);
		cNKMLua.GetData("m_EmoticonSound", ref nKMEmoticonTemplet.m_EmoticonSound);
		return nKMEmoticonTemplet;
	}

	public static NKMEmoticonTemplet Find(int key)
	{
		return NKMTempletContainer<NKMEmoticonTemplet>.Find(key);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	public string GetEmoticonName()
	{
		return NKCStringTable.GetString(m_EmoticonName);
	}

	public string GetEmoticonDesc()
	{
		return NKCStringTable.GetString(m_EmoticonDesc);
	}
}
