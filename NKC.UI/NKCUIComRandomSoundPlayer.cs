using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC.UI;

public class NKCUIComRandomSoundPlayer : MonoBehaviour
{
	public bool m_bPlayOnEnable;

	public bool Voice;

	public bool ShowCaption;

	public List<string> m_lstRandomSound = new List<string>();

	private int m_iCurPlayedSoundUId;

	private void OnEnable()
	{
		if (m_bPlayOnEnable)
		{
			OnRandomSoundPlay();
		}
	}

	public void OnRandomSoundPlay()
	{
		if (m_lstRandomSound.Count > 0)
		{
			if (m_iCurPlayedSoundUId != 0)
			{
				NKCSoundManager.StopSound(m_iCurPlayedSoundUId);
			}
			int index = Random.Range(0, m_lstRandomSound.Count);
			NKMAssetName assetName = NKMAssetName.ParseBundleName(m_lstRandomSound[index], m_lstRandomSound[index]);
			if (Voice)
			{
				m_iCurPlayedSoundUId = NKCSoundManager.PlayVoice(assetName, 0, bClearVoice: false, bIgnoreSameVoice: false, 1f, 0f, 0f, bLoop: false, 0f, ShowCaption);
			}
			else
			{
				m_iCurPlayedSoundUId = NKCSoundManager.PlaySound(assetName, 1f, 0f, 0f, bLoop: false, 0f, ShowCaption);
			}
		}
	}
}
