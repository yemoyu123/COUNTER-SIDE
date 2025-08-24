using System;
using System.Collections.Generic;

namespace NKC.UI;

public class NKCUICutState
{
	public bool m_bTitle;

	public bool m_bFading;

	public bool m_bWaitClick;

	public float m_fWaitTime;

	public float m_fAddWaitTimeForAuto;

	public float m_fElapsedTimeWithoutAutoCalc;

	public bool m_bTalk;

	public string m_EndBGMFileName = "";

	public NKC_CUTSCEN_SOUND_CONTROL m_EndFXSoundControl = NKC_CUTSCEN_SOUND_CONTROL.NCSC_ONE_TIME_PLAY;

	public string m_EndFXSoundName = "";

	public int m_VoiceUID = -1;

	public bool m_bPlayVideo;

	public bool m_bWaitSelection;

	public List<Tuple<string, string>> m_lstSelectionMark = new List<Tuple<string, string>>();

	public bool m_bMovieSkipEnable = true;

	public void InitPerCut()
	{
		m_bTitle = false;
		m_bWaitClick = false;
		m_bFading = false;
		m_fWaitTime = 0f;
		m_fAddWaitTimeForAuto = 0f;
		m_fElapsedTimeWithoutAutoCalc = 0f;
		m_bTalk = false;
		m_EndBGMFileName = "";
		m_EndFXSoundControl = NKC_CUTSCEN_SOUND_CONTROL.NCSC_ONE_TIME_PLAY;
		m_EndFXSoundName = "";
		if (m_VoiceUID >= 0)
		{
			NKCSoundManager.StopSound(m_VoiceUID);
		}
		m_VoiceUID = -1;
		m_bPlayVideo = false;
		m_bWaitSelection = false;
		m_bMovieSkipEnable = true;
	}
}
