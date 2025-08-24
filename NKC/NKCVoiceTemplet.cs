using NKM;

namespace NKC;

public class NKCVoiceTemplet
{
	public int Index;

	public VOICE_TYPE Type;

	public VOICE_CONDITION Condition;

	public int ConditionValue;

	public int Rate = 100;

	public int Volume = 100;

	public string FileName = "";

	public string Npc = "";

	public int Priority = int.MaxValue;

	public bool LoadLUA(NKMLua lua)
	{
		lua.GetData("Index", ref Index);
		lua.GetData("Type", ref Type);
		lua.GetData("Condition", ref Condition);
		lua.GetData("ConditionValue", ref ConditionValue);
		lua.GetData("Rate", ref Rate);
		lua.GetData("Volume", ref Volume);
		lua.GetData("FileName", ref FileName);
		lua.GetData("NPC", ref Npc);
		lua.GetData("Priority", ref Priority);
		return true;
	}
}
