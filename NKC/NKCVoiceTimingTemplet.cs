using NKM;

namespace NKC;

public class NKCVoiceTimingTemplet
{
	public int Index;

	public int UnitId;

	public int SkinId;

	public VOICE_TYPE VoiceType;

	public VOICE_CONDITION VoiceCondition;

	public string FileName = "";

	public float VoiceStartTime;

	public bool LoadLUA(NKMLua lua)
	{
		lua.GetData("Index", ref Index);
		lua.GetData("Type", ref VoiceType);
		lua.GetData("Condition", ref VoiceCondition);
		lua.GetData("FileName", ref FileName);
		lua.GetData("VoiceStartTime", ref VoiceStartTime);
		lua.GetData("Unit_ID", ref UnitId);
		lua.GetData("Skin_ID", ref SkinId);
		return true;
	}
}
