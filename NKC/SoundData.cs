using UnityEngine;

namespace NKC;

public class SoundData
{
	public int m_SoundUID;

	public GameObject m_SoundGameObj;

	public AudioSource m_AudioSource;

	public float m_fReserveTime;

	public bool m_bPlayed;

	public float m_fLocalVol = 1f;

	public float m_fSoundPosX;

	public float m_fRange;

	public SOUND_TRACK m_Track;

	public short m_GameUnitUID;

	public float m_fDelay;

	public NKCAssetResourceData m_NKCAssetResourceData;

	public SoundData()
	{
		m_SoundGameObj = new GameObject("NKM_SOUND_OBJ");
		m_AudioSource = m_SoundGameObj.AddComponent<AudioSource>();
		m_AudioSource.mute = false;
		m_AudioSource.bypassEffects = false;
		m_AudioSource.bypassListenerEffects = false;
		m_AudioSource.bypassReverbZones = false;
		m_AudioSource.playOnAwake = false;
		m_AudioSource.loop = false;
		m_AudioSource.priority = 128;
		m_AudioSource.volume = 1f;
		m_AudioSource.pitch = 1f;
		m_AudioSource.panStereo = 0f;
		m_AudioSource.spatialBlend = 0f;
		m_AudioSource.reverbZoneMix = 1f;
		m_AudioSource.dopplerLevel = 0f;
		m_AudioSource.spread = 0f;
		m_AudioSource.rolloffMode = AudioRolloffMode.Custom;
		m_AudioSource.maxDistance = 2000f;
	}

	public void Unload()
	{
		if (m_NKCAssetResourceData != null)
		{
			NKCAssetResourceManager.CloseResource(m_NKCAssetResourceData);
			m_NKCAssetResourceData = null;
		}
		if (m_SoundGameObj != null)
		{
			Object.Destroy(m_SoundGameObj);
		}
		m_SoundGameObj = null;
		m_AudioSource = null;
		m_Track = SOUND_TRACK.NORMAL;
		m_GameUnitUID = 0;
		m_fDelay = 0f;
	}
}
