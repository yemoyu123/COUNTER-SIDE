using System.Collections.Generic;
using NKM;

namespace NKC;

public class NKCCollectionIllustData
{
	private bool m_bActive;

	public readonly string m_BGGroupTitle;

	public readonly string m_BGGroupText;

	public STAGE_UNLOCK_REQ_TYPE m_UnlockReqType;

	public readonly int m_UnlockReqValue;

	public List<NKCIllustFileData> m_FileData = new List<NKCIllustFileData>();

	public NKCCollectionIllustData(string BGGroupTitle, string BGGroupText, string BGThumbnailFileName, string BGFileName, string BGAniName, STAGE_UNLOCK_REQ_TYPE UnlockReqType, int UnlockReqValue)
	{
		m_BGGroupTitle = BGGroupTitle;
		m_BGGroupText = BGGroupText;
		m_UnlockReqType = UnlockReqType;
		m_UnlockReqValue = UnlockReqValue;
		m_FileData.Add(new NKCIllustFileData(BGThumbnailFileName, BGFileName, BGAniName));
		m_bActive = false;
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
