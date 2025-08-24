using System.Collections.Generic;
using UnityEngine;

namespace NKC.UI;

public class NKCUIComRandomVoicePlayer : MonoBehaviour
{
	public int unitID;

	public VOICE_TYPE[] m_voice;

	public bool m_voicePlay = true;

	public bool m_playOnEnable;

	private int m_prevVoiceIndex = -1;

	private void OnEnable()
	{
		if (m_voicePlay && m_playOnEnable)
		{
			PlayRandomVoice();
		}
	}

	public int PlayRandomVoice()
	{
		int result = 0;
		if (m_voicePlay && m_voice != null && m_voice.Length != 0)
		{
			int num = m_voice.Length;
			List<int> list = new List<int>();
			for (int i = 0; i < num; i++)
			{
				if (i != m_prevVoiceIndex)
				{
					list.Add(i);
				}
			}
			int num2 = Random.Range(0, list.Count);
			int num3 = -1;
			if (num2 < list.Count)
			{
				num3 = list[num2];
			}
			if (num == 1)
			{
				num3 = 0;
			}
			if (num3 >= 0 && num3 < m_voice.Length)
			{
				m_prevVoiceIndex = num3;
				result = NKCUIVoiceManager.PlayVoice(m_voice[num3], unitID);
			}
		}
		return result;
	}
}
