using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCCollectionVoiceTemplet : INKMTemplet
{
	public int IDX;

	public VOICE_TYPE m_VoiceType;

	public bool m_bVoiceCondLifetime;

	public string m_VoicePostID;

	private string m_VoiceButtonName;

	public NKC_VOICE_TYPE m_VoiceCategory = NKC_VOICE_TYPE.ETC;

	public int Key => IDX;

	public string ButtonName => NKCStringTable.GetString(m_VoiceButtonName);

	public static NKCCollectionVoiceTemplet Find(int idx)
	{
		return NKMTempletContainer<NKCCollectionVoiceTemplet>.Find(idx);
	}

	public static NKCCollectionVoiceTemplet LoadLua(NKMLua lua)
	{
		NKCCollectionVoiceTemplet nKCCollectionVoiceTemplet = new NKCCollectionVoiceTemplet();
		int num = 1 & (lua.GetData("IDX", ref nKCCollectionVoiceTemplet.IDX) ? 1 : 0);
		lua.GetData("m_VoiceType", ref nKCCollectionVoiceTemplet.m_VoiceType);
		lua.GetData("m_bVoiceCondLifetime", ref nKCCollectionVoiceTemplet.m_bVoiceCondLifetime);
		lua.GetData("m_VoicePostID", ref nKCCollectionVoiceTemplet.m_VoicePostID);
		lua.GetData("m_VoiceButtonName", ref nKCCollectionVoiceTemplet.m_VoiceButtonName);
		lua.GetData("m_VoiceCategory", ref nKCCollectionVoiceTemplet.m_VoiceCategory);
		if (num == 0)
		{
			return null;
		}
		return nKCCollectionVoiceTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
