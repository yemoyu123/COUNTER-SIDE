using System.Collections.Generic;
using NKM;

namespace NKC;

public class NKCCutScenTemplet
{
	public int m_CutScenID;

	public string m_CutScenStrID = "";

	public List<NKCCutTemplet> m_listCutTemplet = new List<NKCCutTemplet>();

	public bool LoadFromLUA(NKMLua cNKMLua, int index)
	{
		cNKMLua.GetData("m_CutScenID", ref m_CutScenID);
		cNKMLua.GetData("m_CutScenStrID", ref m_CutScenStrID);
		NKCCutTemplet nKCCutTemplet = new NKCCutTemplet();
		nKCCutTemplet.LoadFromLUA(cNKMLua, m_CutScenStrID, index);
		m_listCutTemplet.Add(nKCCutTemplet);
		return true;
	}

	public void AddCutTemplet(NKCCutScenTemplet cNKCCutScenTemplet)
	{
		if (cNKCCutScenTemplet != null && cNKCCutScenTemplet.m_listCutTemplet.Count > 0)
		{
			m_listCutTemplet.Add(cNKCCutScenTemplet.m_listCutTemplet[0]);
		}
	}

	public string GetLastMusicAssetName()
	{
		for (int num = m_listCutTemplet.Count - 1; num >= 0; num--)
		{
			NKCCutTemplet nKCCutTemplet = m_listCutTemplet[num];
			if (nKCCutTemplet != null)
			{
				if (nKCCutTemplet.m_EndBGMFileName.Length > 0)
				{
					return nKCCutTemplet.m_EndBGMFileName;
				}
				if (nKCCutTemplet.m_StartBGMFileName.Length > 0)
				{
					return nKCCutTemplet.m_StartBGMFileName;
				}
				if (nKCCutTemplet.m_Action == NKCCutTemplet.eCutsceneAction.PLAY_MUSIC)
				{
					string actionFirstToken = nKCCutTemplet.GetActionFirstToken();
					if (!string.IsNullOrEmpty(actionFirstToken))
					{
						return actionFirstToken;
					}
				}
			}
		}
		return null;
	}
}
