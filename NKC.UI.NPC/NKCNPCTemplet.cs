using NKM;

namespace NKC.UI.NPC;

public class NKCNPCTemplet
{
	public NPC_ACTION_TYPE m_ActionType;

	public float m_ActionCoolTime;

	public string m_AnimationName = "";

	public string m_VoiceFileName = "";

	public NPC_CONDITION m_ConditionType;

	public int m_ConditionValue;

	public int m_Volume = 1;

	public string m_Text = "";

	public bool LoadLUA(NKMLua lua)
	{
		bool data = lua.GetData("m_ActionType", ref m_ActionType);
		lua.GetData("m_ActionCoolTime", ref m_ActionCoolTime);
		lua.GetData("m_AnimationName", ref m_AnimationName);
		lua.GetData("m_VoiceFileName", ref m_VoiceFileName);
		lua.GetData("m_ConditionType", ref m_ConditionType);
		lua.GetData("m_ConditionValue", ref m_ConditionValue);
		lua.GetData("m_Volume", ref m_Volume);
		lua.GetData("m_Text", ref m_Text);
		return data;
	}
}
