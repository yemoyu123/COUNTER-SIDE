using System.Collections.Generic;
using NKM;

namespace NKC;

public class CollectionStoryData
{
	private bool m_bActive;

	public readonly string m_BGGroupTitle;

	public readonly string m_BGGroupText;

	public STAGE_UNLOCK_REQ_TYPE m_UnlockReqType;

	public readonly int m_UnlockReqValue;

	public List<NKCIllustFileData> m_FileData = new List<NKCIllustFileData>();

	public int m_ActID;

	public int m_MissionIndex;

	public string m_StageName;

	public CollectionStoryData(STAGE_UNLOCK_REQ_TYPE type, int ReqVal)
	{
		m_UnlockReqType = type;
		m_UnlockReqValue = ReqVal;
	}

	public void SetClearState(bool active)
	{
		m_bActive = active;
	}

	public bool IsClearMission()
	{
		return m_bActive;
	}
}
