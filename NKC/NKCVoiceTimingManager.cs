using System.Collections.Generic;
using NKM;

namespace NKC;

public static class NKCVoiceTimingManager
{
	private static Dictionary<int, Dictionary<VOICE_TYPE, List<NKCVoiceTimingTemplet>>> unitVoiceTimingData = new Dictionary<int, Dictionary<VOICE_TYPE, List<NKCVoiceTimingTemplet>>>();

	public static void LoadFromLua(string fileName)
	{
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath("AB_SCRIPT", fileName) && nKMLua.OpenTable("UNIT_VOICE_TEMPLET"))
		{
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				NKCVoiceTimingTemplet nKCVoiceTimingTemplet = new NKCVoiceTimingTemplet();
				nKCVoiceTimingTemplet.LoadLUA(nKMLua);
				if (!unitVoiceTimingData.ContainsKey(nKCVoiceTimingTemplet.UnitId))
				{
					unitVoiceTimingData.Add(nKCVoiceTimingTemplet.UnitId, new Dictionary<VOICE_TYPE, List<NKCVoiceTimingTemplet>>());
				}
				if (!unitVoiceTimingData[nKCVoiceTimingTemplet.UnitId].ContainsKey(nKCVoiceTimingTemplet.VoiceType))
				{
					unitVoiceTimingData[nKCVoiceTimingTemplet.UnitId].Add(nKCVoiceTimingTemplet.VoiceType, new List<NKCVoiceTimingTemplet>());
				}
				unitVoiceTimingData[nKCVoiceTimingTemplet.UnitId][nKCVoiceTimingTemplet.VoiceType].Add(nKCVoiceTimingTemplet);
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
	}

	public static float GetDelayTime(int unitId, int skinId, NKCVoiceTemplet voiceTemplet)
	{
		if (!unitVoiceTimingData.ContainsKey(unitId))
		{
			return 0f;
		}
		if (!unitVoiceTimingData[unitId].ContainsKey(voiceTemplet.Type))
		{
			return 0f;
		}
		return unitVoiceTimingData[unitId][voiceTemplet.Type].Find((NKCVoiceTimingTemplet e) => e.FileName == voiceTemplet.FileName && e.SkinId == skinId)?.VoiceStartTime ?? 0f;
	}
}
