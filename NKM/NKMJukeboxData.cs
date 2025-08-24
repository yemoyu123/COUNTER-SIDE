using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public class NKMJukeboxData : ISerializable
{
	private Dictionary<int, int> dicBgmId = new Dictionary<int, int>();

	public int GetJukeboxBgmId(NKM_BGM_TYPE bgmType)
	{
		if (!dicBgmId.ContainsKey((int)bgmType))
		{
			return 0;
		}
		return dicBgmId[(int)bgmType];
	}

	public void SetJukeboxBgm(int bgmType, int bgmId)
	{
		if (!dicBgmId.ContainsKey(bgmType))
		{
			dicBgmId.Add(bgmType, bgmId);
		}
		else
		{
			dicBgmId[bgmType] = bgmId;
		}
	}

	public bool IsBgmChanged(NKM_BGM_TYPE bgmType, int bgmId)
	{
		if (!dicBgmId.ContainsKey((int)bgmType))
		{
			return true;
		}
		if (dicBgmId[(int)bgmType] != bgmId)
		{
			return true;
		}
		return false;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref dicBgmId);
	}
}
