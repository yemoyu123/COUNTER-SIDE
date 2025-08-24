using System.Collections.Generic;
using Cs.Logging;
using NKC.UI;
using NKM;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.Audio;

namespace NKC;

public class NKCSoundManager
{
	private struct BackgroundContinueData
	{
		public bool bPlayBackgroundBGM;

		public float fPlayBackgroundBGMTime;

		public string strBackgroundBGMAssetID;

		public BackgroundContinueData(bool bPlayingBGM, float fStartTime)
		{
			bPlayBackgroundBGM = bPlayingBGM;
			fPlayBackgroundBGMTime = fStartTime;
			strBackgroundBGMAssetID = "";
		}
	}

	private static int m_ReserveSoundDataCount = 20;

	private static int m_UIDIndex = 1;

	private static GameObject m_NKM_SOUND = null;

	private static Dictionary<NKM_SCEN_ID, NKCScenMusicData> m_dicScenMusicData = new Dictionary<NKM_SCEN_ID, NKCScenMusicData>();

	private static LinkedList<SoundData> m_linklistSoundData = new LinkedList<SoundData>();

	private static LinkedList<SoundData> m_linklistSoundDataReserve = new LinkedList<SoundData>();

	private static Queue<SoundData> m_qSoundDataPool = new Queue<SoundData>();

	private static float m_fCamPosXBefore = 0f;

	private static float m_fCamPosX = 0f;

	private static float m_fAllVol = 1f;

	private static float m_fSoundVol = 1f;

	private static float m_fMusicVol = 1f;

	private static float m_fMusicVolFactor = 1f;

	private static float m_fVoiceVol = 1f;

	private static string m_MusicName = "";

	private static SoundData m_MusicData1 = null;

	private static SoundData m_MusicData2 = null;

	private static int m_MusicDataNow = 2;

	private static NKMTrackingFloat m_MusicData1Fade = new NKMTrackingFloat();

	private static NKMTrackingFloat m_MusicData2Fade = new NKMTrackingFloat();

	private static BackgroundContinueData m_BackgroundBGMContinueData = new BackgroundContinueData(bPlayingBGM: false, 0f);

	private static bool m_bCurrentSceneInGameMode = false;

	private const string AUDIO_MIXER_BUNDLE_NAME = "ab_audio_mixer";

	private const string AUDIO_MIXER_ASSET_NAME = "AB_SOUND_AUDIO_MIXER_01";

	private static bool m_bSoundEffect = false;

	private static AudioMixerGroup m_AudioMixerGroupInBgm = null;

	private static AudioMixerGroup m_AudioMixerGroupInSoundNormal = null;

	private static AudioMixerGroup m_AudioMixerGroupInSoundVoice = null;

	private static AudioMixerGroup m_AudioMixerGroupOutBgm = null;

	private static AudioMixerGroup m_AudioMixerGroupOutSoundNormal = null;

	private static AudioMixerGroup m_AudioMixerGroupOutSoundVoice = null;

	private const string AUDIO_MIXER_NAME_IN_BGM = "InBgm";

	private const string AUDIO_MIXER_NAME_IN_SOUND_NORMAL = "InSoundNormal";

	private const string AUDIO_MIXER_NAME_IN_SOUND_VOICE = "InSoundVoice";

	private const string AUDIO_MIXER_NAME_OUT_BGM = "OutBgm";

	private const string AUDIO_MIXER_NAME_OUT_SOUND_NORMAL = "OutSoundNormal";

	private const string AUDIO_MIXER_NAME_OUT_SOUND_VOICE = "OutSoundVoice";

	private static Dictionary<string, AudioMixerGroup> m_dicCustomAudioMixerGroup = new Dictionary<string, AudioMixerGroup>();

	public static string CurrentMusicName => m_MusicName;

	public static bool IsInit()
	{
		return m_NKM_SOUND != null;
	}

	public static void Init()
	{
		Log.Debug("[SoundManager] Init]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCSoundManager.cs", 194);
		m_NKM_SOUND = GameObject.Find("NKM_SOUND");
		ReserveSoundData(m_ReserveSoundDataCount);
		m_MusicData1 = OpenSoundData();
		m_MusicData2 = OpenSoundData();
		AudioSettings.OnAudioConfigurationChanged -= OnAudioSettingsChanged;
		AudioSettings.OnAudioConfigurationChanged += OnAudioSettingsChanged;
	}

	public static void OnAudioSettingsChanged(bool deviceWasChanged)
	{
		Log.Debug($"[SoundManager] OnAudioSettingsChanged deviceChanged[{deviceWasChanged}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCSoundManager.cs", 208);
	}

	public static bool LoadFromLUA(string fileName)
	{
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath("AB_SCRIPT", fileName) && nKMLua.OpenTable("m_dicScenMusicData"))
		{
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				NKCScenMusicData nKCScenMusicData = new NKCScenMusicData();
				nKCScenMusicData.LoadFromLUA(nKMLua);
				if (!m_dicScenMusicData.ContainsKey(nKCScenMusicData.m_NKM_SCEN_ID))
				{
					m_dicScenMusicData.Add(nKCScenMusicData.m_NKM_SCEN_ID, nKCScenMusicData);
				}
				else
				{
					m_dicScenMusicData[nKCScenMusicData.m_NKM_SCEN_ID].DeepCopyFromSource(nKCScenMusicData);
				}
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
		return true;
	}

	public static void Unload()
	{
		while (m_qSoundDataPool.Count > m_ReserveSoundDataCount)
		{
			m_qSoundDataPool.Dequeue().Unload();
		}
	}

	public static void Update(float deltaTime)
	{
		m_fCamPosXBefore = m_fCamPosX;
		m_fCamPosX = NKCCamera.GetPosNowX();
		LinkedListNode<SoundData> linkedListNode = m_linklistSoundDataReserve.First;
		while (linkedListNode != null)
		{
			SoundData value = linkedListNode.Value;
			if (value != null)
			{
				value.m_fReserveTime -= deltaTime;
				if (value.m_fReserveTime <= 0f)
				{
					PlaySound(value);
					LinkedListNode<SoundData> next = linkedListNode.Next;
					m_linklistSoundDataReserve.Remove(linkedListNode);
					linkedListNode = next;
					continue;
				}
			}
			linkedListNode = linkedListNode.Next;
		}
		int num = m_linklistSoundData.Count - 30;
		linkedListNode = m_linklistSoundData.First;
		while (linkedListNode != null)
		{
			bool flag = false;
			SoundData value2 = linkedListNode.Value;
			if (value2 != null)
			{
				if (num > 0)
				{
					num--;
					value2.m_bPlayed = true;
					flag = true;
				}
				if (value2.m_AudioSource != null)
				{
					if (value2.m_AudioSource.isPlaying)
					{
						value2.m_bPlayed = true;
					}
					if (value2.m_bPlayed && !value2.m_AudioSource.isPlaying)
					{
						flag = true;
					}
					else if (m_fCamPosXBefore != m_fCamPosX && value2.m_fRange > 0f)
					{
						if (value2.m_Track == SOUND_TRACK.NORMAL)
						{
							value2.m_AudioSource.volume = GetFinalVol(value2.m_fSoundPosX, value2.m_fRange, value2.m_fLocalVol);
						}
						else
						{
							value2.m_AudioSource.volume = GetFinalVoiceVolume(value2.m_fSoundPosX, value2.m_fRange, value2.m_fLocalVol);
						}
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					if (value2.m_AudioSource != null)
					{
						value2.m_AudioSource.Stop();
					}
					CloseSoundData(value2);
					LinkedListNode<SoundData> next2 = linkedListNode.Next;
					m_linklistSoundData.Remove(linkedListNode);
					linkedListNode = next2;
					continue;
				}
			}
			linkedListNode = linkedListNode.Next;
		}
		m_MusicData1Fade.Update(deltaTime);
		m_MusicData2Fade.Update(deltaTime);
		if (m_MusicData1Fade.IsTracking())
		{
			if (m_MusicData1.m_AudioSource != null)
			{
				m_MusicData1.m_AudioSource.volume = GetFinalMusicVolume(m_MusicData1.m_fLocalVol) * m_MusicData1Fade.GetNowValue();
			}
		}
		else if (m_MusicDataNow != 1 && m_MusicData1.m_AudioSource != null && m_MusicData1.m_AudioSource.isPlaying)
		{
			m_MusicData1.m_AudioSource.Stop();
		}
		if (m_MusicData2Fade.IsTracking())
		{
			if (m_MusicData2.m_AudioSource != null)
			{
				m_MusicData2.m_AudioSource.volume = GetFinalMusicVolume(m_MusicData2.m_fLocalVol) * m_MusicData2Fade.GetNowValue();
			}
		}
		else if (m_MusicDataNow == 1 && m_MusicData2.m_AudioSource != null && m_MusicData2.m_AudioSource.isPlaying)
		{
			m_MusicData2.m_AudioSource.Stop();
		}
		if (m_MusicData1Fade.IsTracking() || m_MusicData2Fade.IsTracking())
		{
			return;
		}
		if (m_MusicDataNow == 1)
		{
			if (m_MusicData1.m_AudioSource != null && m_MusicData1.m_AudioSource.isPlaying)
			{
				m_MusicData1.m_AudioSource.volume = GetFinalMusicVolume(m_MusicData1.m_fLocalVol);
			}
		}
		else if (m_MusicData2.m_AudioSource != null && m_MusicData2.m_AudioSource.isPlaying)
		{
			m_MusicData2.m_AudioSource.volume = GetFinalMusicVolume(m_MusicData2.m_fLocalVol);
		}
	}

	private static int GetNewSoundUID()
	{
		return m_UIDIndex++;
	}

	private static void ReserveSoundData(int count)
	{
		for (int i = 0; i < count; i++)
		{
			SoundData soundData = new SoundData();
			soundData.m_SoundGameObj.transform.SetParent(m_NKM_SOUND.transform);
			if (soundData.m_SoundGameObj.activeSelf)
			{
				soundData.m_SoundGameObj.SetActive(value: false);
			}
			m_qSoundDataPool.Enqueue(soundData);
		}
	}

	private static SoundData OpenSoundData()
	{
		SoundData soundData = null;
		if (m_qSoundDataPool.Count > 0)
		{
			soundData = m_qSoundDataPool.Dequeue();
		}
		if (soundData != null && soundData.m_SoundGameObj == null)
		{
			Debug.LogWarning("invalid SoundData detected. try recovering...");
			soundData.Unload();
			soundData = null;
		}
		if (soundData == null)
		{
			soundData = new SoundData();
			soundData.m_SoundGameObj.transform.SetParent(m_NKM_SOUND.transform);
		}
		if (!soundData.m_SoundGameObj.activeSelf)
		{
			soundData.m_SoundGameObj.SetActive(value: true);
		}
		soundData.m_bPlayed = false;
		return soundData;
	}

	private static void CloseSoundData(SoundData cSoundData)
	{
		if (cSoundData.m_SoundGameObj != null)
		{
			if (cSoundData.m_SoundGameObj.activeSelf)
			{
				cSoundData.m_SoundGameObj.SetActive(value: false);
			}
			m_qSoundDataPool.Enqueue(cSoundData);
		}
		else
		{
			cSoundData.Unload();
		}
	}

	public static int PlaySound(string audioClipName, float fLocalVol, float fSoundPosX, float fRange, bool bLoop = false, float reserveTime = 0f, bool bShowCaption = false, float fStartTime = 0f)
	{
		return PlaySound(SOUND_TRACK.NORMAL, audioClipName, 0, bClearVoice: false, bIgnoreSameVoice: false, fLocalVol, fSoundPosX, fRange, bLoop, reserveTime, bShowCaption, fStartTime);
	}

	public static int PlaySound(NKMAssetName assetName, float fLocalVol, float fSoundPosX, float fRange, bool bLoop = false, float reserveTime = 0f, bool bShowCaption = false, float fStartTime = 0f)
	{
		return PlaySound(SOUND_TRACK.NORMAL, assetName.m_BundleName, assetName.m_AssetName, 0, bClearVoice: false, bIgnoreSameVoice: false, fLocalVol, fSoundPosX, fRange, bLoop, reserveTime, bShowCaption, fStartTime);
	}

	public static int PlayVoice(string audioClipName, short gameUnitUID, bool bClearVoice, bool bIgnoreSameVoice, float fLocalVol, float fSoundPosX, float fRange, bool bLoop = false, float reserveTime = 0f, bool bShowCaption = false)
	{
		return PlaySound(SOUND_TRACK.VOICE, audioClipName, gameUnitUID, bClearVoice, bIgnoreSameVoice, fLocalVol, fSoundPosX, fRange, bLoop, reserveTime, bShowCaption);
	}

	public static int PlayVoice(NKMAssetName assetName, short gameUnitUID, bool bClearVoice, bool bIgnoreSameVoice, float fLocalVol, float fSoundPosX, float fRange, bool bLoop = false, float reserveTime = 0f, bool bShowCaption = false, float startTime = 0f, float delay = 0f)
	{
		return PlaySound(SOUND_TRACK.VOICE, assetName.m_BundleName, assetName.m_AssetName, gameUnitUID, bClearVoice, bIgnoreSameVoice, fLocalVol, fSoundPosX, fRange, bLoop, reserveTime, bShowCaption, startTime, delay);
	}

	public static int PlayVoice(string bundleName, string audioClipName, short gameUnitUID, bool bClearVoice, bool bIgnoreSameVoice, float fLocalVol, float fSoundPosX, float fRange, bool bLoop = false, float reserveTime = 0f, bool bShowCaption = false, float startTime = 0f, float delay = 0f)
	{
		return PlaySound(SOUND_TRACK.VOICE, bundleName, audioClipName, gameUnitUID, bClearVoice, bIgnoreSameVoice, fLocalVol, fSoundPosX, fRange, bLoop, reserveTime, bShowCaption, startTime, delay);
	}

	private static int PlaySound(SOUND_TRACK eTrack, string audioClipName, short gameUnitUID, bool bClearVoice, bool bIgnoreSameVoice, float fLocalVol, float fSoundPosX, float fRange, bool bLoop = false, float reserveTime = 0f, bool bShowCaption = false, float fStartTime = 0f)
	{
		string bundleName = NKCAssetResourceManager.GetBundleName(audioClipName, bIgnoreNotFoundError: true);
		if (string.IsNullOrEmpty(bundleName))
		{
			return 0;
		}
		return PlaySound(eTrack, bundleName, audioClipName, gameUnitUID, bClearVoice, bIgnoreSameVoice, fLocalVol, fSoundPosX, fRange, bLoop, reserveTime, bShowCaption, fStartTime);
	}

	private static int PlaySound(SOUND_TRACK eTrack, string bundleName, string audioClipName, short gameUnitUID, bool bClearVoice, bool bIgnoreSameVoice, float fLocalVol, float fSoundPosX, float fRange, bool bLoop = false, float reserveTime = 0f, bool bShowCaption = false, float fStartTime = 0f, float delay = 0f, string targetAudioMixer = "")
	{
		if (NKCScenManager.GetScenManager() != null && NKCScenManager.GetScenManager().GetNKCPowerSaveMode().GetEnable() && !NKCScenManager.GetScenManager().GetNKCPowerSaveMode().IsJukeBoxMode)
		{
			return 0;
		}
		if (m_NKM_SOUND == null)
		{
			return 0;
		}
		if (!NKCAssetResourceManager.IsBundleExists(bundleName, audioClipName))
		{
			Debug.LogWarning("voice bundle " + bundleName + " for audioClip " + audioClipName + " not exists!");
			if (bShowCaption)
			{
				NKCUIManager.NKCUIOverlayCaption.OpenCaption(NKCUtilString.GetVoiceCaption(bundleName, audioClipName), 0, delay);
			}
			return 0;
		}
		if (bIgnoreSameVoice)
		{
			foreach (SoundData item in m_linklistSoundDataReserve)
			{
				if (item.m_NKCAssetResourceData.m_NKMAssetName.m_AssetName.Equals(audioClipName))
				{
					return 0;
				}
			}
			foreach (SoundData linklistSoundDatum in m_linklistSoundData)
			{
				if (linklistSoundDatum.m_NKCAssetResourceData.m_NKMAssetName.m_AssetName.Equals(audioClipName))
				{
					return 0;
				}
			}
		}
		SoundData soundData = OpenSoundData();
		soundData.m_SoundUID = GetNewSoundUID();
		soundData.m_fLocalVol = fLocalVol;
		soundData.m_fReserveTime = reserveTime;
		soundData.m_fSoundPosX = fSoundPosX;
		soundData.m_fRange = fRange;
		soundData.m_Track = eTrack;
		soundData.m_GameUnitUID = gameUnitUID;
		soundData.m_fDelay = delay;
		if (bClearVoice)
		{
			ClearVoice(soundData.m_GameUnitUID);
		}
		NKCAssetResourceData nKCAssetResourceData = NKCAssetResourceManager.OpenResource<AudioClip>(bundleName, audioClipName);
		if (soundData.m_NKCAssetResourceData != null)
		{
			NKCAssetResourceManager.CloseResource(soundData.m_NKCAssetResourceData);
		}
		soundData.m_NKCAssetResourceData = nKCAssetResourceData;
		if (soundData.m_NKCAssetResourceData != null && soundData.m_NKCAssetResourceData.GetAsset<AudioClip>() != null)
		{
			AudioClip asset = soundData.m_NKCAssetResourceData.GetAsset<AudioClip>();
			soundData.m_AudioSource.clip = asset;
		}
		soundData.m_AudioSource.loop = bLoop;
		soundData.m_AudioSource.time = fStartTime;
		if (soundData.m_fReserveTime > 0f)
		{
			m_linklistSoundDataReserve.AddLast(soundData);
		}
		else
		{
			PlaySound(soundData, bShowCaption, targetAudioMixer);
		}
		return soundData.m_SoundUID;
	}

	private static void ClearVoice(short gameUnitUID)
	{
		LinkedListNode<SoundData> linkedListNode;
		for (linkedListNode = m_linklistSoundDataReserve.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			SoundData value = linkedListNode.Value;
			if (value != null && value.m_Track == SOUND_TRACK.VOICE && value.m_GameUnitUID == gameUnitUID)
			{
				CloseSoundData(value);
				_ = linkedListNode.Next;
				m_linklistSoundDataReserve.Remove(linkedListNode);
			}
		}
		linkedListNode = m_linklistSoundData.First;
		while (linkedListNode != null)
		{
			SoundData value2 = linkedListNode.Value;
			if (value2 != null && value2.m_Track == SOUND_TRACK.VOICE && value2.m_GameUnitUID == gameUnitUID)
			{
				value2.m_bPlayed = true;
				value2.m_AudioSource.Stop();
				CloseSoundData(value2);
				LinkedListNode<SoundData> next = linkedListNode.Next;
				m_linklistSoundData.Remove(linkedListNode);
				linkedListNode = next;
			}
			else
			{
				linkedListNode = linkedListNode.Next;
			}
		}
	}

	private static bool PlaySound(SoundData cSoundData, bool bShowCaption = false, string audioMixerGroup = "")
	{
		if (NKCScenManager.GetScenManager().GetNKCPowerSaveMode().GetEnable() && !NKCScenManager.GetScenManager().GetNKCPowerSaveMode().IsJukeBoxMode)
		{
			return false;
		}
		m_linklistSoundData.AddLast(cSoundData);
		float volume = 0f;
		switch (cSoundData.m_Track)
		{
		case SOUND_TRACK.NORMAL:
			volume = GetFinalVol(cSoundData.m_fSoundPosX, cSoundData.m_fRange, cSoundData.m_fLocalVol);
			break;
		case SOUND_TRACK.VOICE:
			volume = GetFinalVoiceVolume(cSoundData.m_fSoundPosX, cSoundData.m_fRange, cSoundData.m_fLocalVol);
			break;
		}
		cSoundData.m_AudioSource.volume = volume;
		cSoundData.m_AudioSource.outputAudioMixerGroup = GetAudioMixerGroupSound(cSoundData.m_Track, audioMixerGroup);
		if (cSoundData.m_fDelay <= 0f)
		{
			cSoundData.m_AudioSource.Play();
		}
		else
		{
			cSoundData.m_AudioSource.PlayDelayed(cSoundData.m_fDelay);
		}
		if (bShowCaption)
		{
			NKCUIManager.NKCUIOverlayCaption.OpenCaption(NKCUtilString.GetVoiceCaption(cSoundData.m_NKCAssetResourceData.m_NKMAssetName), cSoundData.m_SoundUID);
		}
		return true;
	}

	public static void SetSoundVolume(float fVol)
	{
		m_fSoundVol = fVol;
		SetMixerVolume("InSoundNormal", fVol);
		SetMixerVolume("OutSoundNormal", fVol);
	}

	public static void StopSound(int soundUID)
	{
		for (LinkedListNode<SoundData> linkedListNode = m_linklistSoundData.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			SoundData value = linkedListNode.Value;
			if (value.m_SoundUID == soundUID)
			{
				value.m_AudioSource.Stop();
				m_linklistSoundData.Remove(linkedListNode);
				CloseSoundData(value);
				return;
			}
		}
		for (LinkedListNode<SoundData> linkedListNode = m_linklistSoundDataReserve.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			SoundData value2 = linkedListNode.Value;
			if (value2.m_SoundUID == soundUID)
			{
				value2.m_AudioSource.Stop();
				m_linklistSoundDataReserve.Remove(linkedListNode);
				CloseSoundData(value2);
				break;
			}
		}
	}

	public static void StopAllSound()
	{
		foreach (SoundData linklistSoundDatum in m_linklistSoundData)
		{
			linklistSoundDatum.m_AudioSource.Stop();
			CloseSoundData(linklistSoundDatum);
		}
		m_linklistSoundData.Clear();
		foreach (SoundData item in m_linklistSoundDataReserve)
		{
			item.m_AudioSource.Stop();
			CloseSoundData(item);
		}
		m_linklistSoundDataReserve.Clear();
	}

	public static void StopAllSound(SOUND_TRACK trackType)
	{
		foreach (SoundData linklistSoundDatum in m_linklistSoundData)
		{
			if (linklistSoundDatum.m_Track == trackType)
			{
				linklistSoundDatum.m_AudioSource.Stop();
				CloseSoundData(linklistSoundDatum);
			}
		}
		foreach (SoundData item in m_linklistSoundDataReserve)
		{
			if (item.m_Track == trackType)
			{
				item.m_AudioSource.Stop();
				CloseSoundData(item);
			}
		}
	}

	public static float GetFinalVol(float fSoundPosX, float fRange, float fLocalVol)
	{
		if (NKCScenManager.GetScenManager().GetNKCPowerSaveMode().GetEnable() && !NKCScenManager.GetScenManager().GetNKCPowerSaveMode().IsJukeBoxMode)
		{
			return 0f;
		}
		if (NKCScenManager.GetScenManager().GetGameOptionData().SoundMute)
		{
			return 0f;
		}
		if (fRange <= 0f)
		{
			return m_fSoundVol * fLocalVol * m_fAllVol;
		}
		float num = Mathf.Abs(m_fCamPosX - fSoundPosX);
		if (num <= fRange)
		{
			return m_fSoundVol * fLocalVol * m_fAllVol;
		}
		float num2 = 1f - num * 0.01f * 0.1f;
		if (num2 < 0.3f)
		{
			num2 = 0.3f;
		}
		return m_fSoundVol * fLocalVol * num2 * m_fAllVol;
	}

	private static SoundData GetNowMusicData()
	{
		if (m_MusicDataNow == 1)
		{
			return m_MusicData1;
		}
		return m_MusicData2;
	}

	private static SoundData GetNewMusicData()
	{
		if (m_MusicDataNow == 1)
		{
			m_MusicDataNow = 2;
			m_MusicData2Fade.SetNowValue(0f);
			m_MusicData2Fade.SetTracking(1f, 1f, TRACKING_DATA_TYPE.TDT_NORMAL);
			m_MusicData1Fade.SetTracking(0f, 1f, TRACKING_DATA_TYPE.TDT_NORMAL);
			return m_MusicData2;
		}
		m_MusicDataNow = 1;
		m_MusicData1Fade.SetNowValue(0f);
		m_MusicData1Fade.SetTracking(1f, 1f, TRACKING_DATA_TYPE.TDT_NORMAL);
		m_MusicData2Fade.SetTracking(0f, 1f, TRACKING_DATA_TYPE.TDT_NORMAL);
		return m_MusicData1;
	}

	public static void PlayMusic(string audioClipName, bool bLoop = false, float fLocalVol = 1f, bool bForce = false, float fStartTime = 0f, float fDelay = 0f)
	{
		if (!bForce && m_MusicName.CompareTo(audioClipName) == 0)
		{
			return;
		}
		NKCAssetResourceData nKCAssetResourceData = NKCAssetResourceManager.OpenResource<AudioClip>("ab_music/" + audioClipName, audioClipName);
		if (nKCAssetResourceData != null && !(nKCAssetResourceData.GetAsset<AudioClip>() == null))
		{
			SaveBackgroundBGMTime(audioClipName);
			m_MusicName = audioClipName;
			SoundData newMusicData = GetNewMusicData();
			if (newMusicData.m_NKCAssetResourceData != null)
			{
				NKCAssetResourceManager.CloseResource(newMusicData.m_NKCAssetResourceData);
			}
			newMusicData.m_NKCAssetResourceData = nKCAssetResourceData;
			AudioClip asset = newMusicData.m_NKCAssetResourceData.GetAsset<AudioClip>();
			newMusicData.m_AudioSource.clip = asset;
			newMusicData.m_AudioSource.loop = bLoop;
			newMusicData.m_fLocalVol = fLocalVol;
			newMusicData.m_AudioSource.time = fStartTime;
			newMusicData.m_AudioSource.outputAudioMixerGroup = GetAudioMixerGroupMusic();
			if (fDelay <= 0f)
			{
				newMusicData.m_AudioSource.Play();
			}
			else
			{
				newMusicData.m_AudioSource.PlayDelayed(fDelay);
			}
		}
	}

	public static void PlayScenMusic()
	{
		PlayScenMusic(NKCScenManager.GetScenManager().GetNowScenID());
	}

	public static void PlayScenMusic(NKM_SCEN_ID eNKM_SCEN_ID, bool bForce = false)
	{
		m_bCurrentSceneInGameMode = eNKM_SCEN_ID == NKM_SCEN_ID.NSI_GAME;
		if (!m_dicScenMusicData.ContainsKey(eNKM_SCEN_ID))
		{
			return;
		}
		NKCScenMusicData nKCScenMusicData = m_dicScenMusicData[eNKM_SCEN_ID];
		string text = nKCScenMusicData.m_MusicName;
		float fStartTime = 0f;
		float fLocalVol = 1f;
		if (nKCScenMusicData.m_MusicName == "FOLLOW_LOBBY")
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				bool flag = false;
				NKCBackgroundTemplet nKCBackgroundTemplet = NKCBackgroundTemplet.Find(nKMUserData.BackgroundID);
				if (nKCBackgroundTemplet != null)
				{
					flag = true;
					text = nKCBackgroundTemplet.m_Background_Music;
				}
				NKCBGMInfoTemplet nKCBGMInfoTemplet = NKMTempletContainer<NKCBGMInfoTemplet>.Find(nKMUserData.BackgroundBGMID);
				if (nKCBGMInfoTemplet != null)
				{
					flag = true;
					text = nKCBGMInfoTemplet.m_BgmAssetID;
					fLocalVol = nKCBGMInfoTemplet.BGMVolume;
				}
				if (nKMUserData.BackgroundBGMContinue && flag)
				{
					m_BackgroundBGMContinueData.strBackgroundBGMAssetID = text;
					if (!m_BackgroundBGMContinueData.bPlayBackgroundBGM)
					{
						m_BackgroundBGMContinueData.bPlayBackgroundBGM = true;
						fStartTime = m_BackgroundBGMContinueData.fPlayBackgroundBGMTime;
					}
				}
			}
		}
		PlayMusic(text, bLoop: true, fLocalVol, bForce, fStartTime);
	}

	public static void SaveBackgroundBGMTime(string strBgmAssetID)
	{
		if (!string.Equals(strBgmAssetID, m_MusicName) && !string.IsNullOrEmpty(m_BackgroundBGMContinueData.strBackgroundBGMAssetID) && !string.Equals(m_BackgroundBGMContinueData.strBackgroundBGMAssetID, strBgmAssetID))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null && nKMUserData.BackgroundBGMContinue && m_BackgroundBGMContinueData.bPlayBackgroundBGM)
			{
				m_BackgroundBGMContinueData.fPlayBackgroundBGMTime = GetMusicTime();
				m_BackgroundBGMContinueData.bPlayBackgroundBGM = false;
			}
		}
	}

	public static bool IsSameMusic(string audioClipName)
	{
		if (m_MusicName.CompareTo(audioClipName) == 0)
		{
			return true;
		}
		return false;
	}

	public static void ChangeAllVolume(float delta)
	{
		SetAllVolume(m_fAllVol + delta);
	}

	public static void SetAllVolume(float fVol)
	{
		m_fAllVol = fVol;
		NKCUIComVideoPlayer.OnUpdateVolume();
		SetMusicVolume(m_fMusicVol);
		SetVoiceVolume(m_fVoiceVol);
		SetSoundVolume(m_fSoundVol);
	}

	public static void SetMute(bool bMute, bool bIgnoreApplicationFocus = false)
	{
		AudioListener.volume = ((!bMute && (Application.isFocused || bIgnoreApplicationFocus)) ? 1 : 0);
		NKCUIComVideoPlayer.OnUpdateVolume();
	}

	public static void SetMusicVolume(float fVol)
	{
		m_fMusicVol = fVol;
		if (m_MusicData1 != null && m_MusicData1.m_AudioSource != null)
		{
			m_MusicData1.m_AudioSource.volume = GetFinalMusicVolume(m_MusicData1.m_fLocalVol);
		}
		if (m_MusicData2 != null && m_MusicData2.m_AudioSource != null)
		{
			m_MusicData2.m_AudioSource.volume = GetFinalMusicVolume(m_MusicData2.m_fLocalVol);
		}
		SetMixerVolume("InBgm", fVol);
		SetMixerVolume("OutBgm", fVol);
	}

	public static void StopMusic()
	{
		if (m_MusicData1.m_AudioSource != null)
		{
			m_MusicData1.m_AudioSource.Stop();
		}
		if (m_MusicData2.m_AudioSource != null)
		{
			m_MusicData2.m_AudioSource.Stop();
		}
		m_MusicName = "";
	}

	public static void FadeOutMusic()
	{
		m_MusicData2Fade.SetTracking(0f, 1f, TRACKING_DATA_TYPE.TDT_NORMAL);
		m_MusicData1Fade.SetTracking(0f, 1f, TRACKING_DATA_TYPE.TDT_NORMAL);
	}

	public static void SetMusicVolumeFactor(float fVolFactor)
	{
		m_fMusicVolFactor = fVolFactor;
		if (m_MusicData1.m_AudioSource != null)
		{
			m_MusicData1.m_AudioSource.volume = GetFinalMusicVolume(m_MusicData1.m_fLocalVol);
		}
		if (m_MusicData2.m_AudioSource != null)
		{
			m_MusicData2.m_AudioSource.volume = GetFinalMusicVolume(m_MusicData2.m_fLocalVol);
		}
	}

	private static float GetFinalMusicVolume(float fLocalVol)
	{
		if (NKCScenManager.GetScenManager().GetNKCPowerSaveMode().GetEnable() && !NKCScenManager.GetScenManager().GetNKCPowerSaveMode().IsJukeBoxMode)
		{
			return 0f;
		}
		return m_fMusicVol * fLocalVol * m_fMusicVolFactor * m_fAllVol;
	}

	public static void SetVoiceVolume(float fVolume)
	{
		m_fVoiceVol = fVolume;
		SetMixerVolume("InSoundVoice", fVolume);
		SetMixerVolume("OutSoundVoice", fVolume);
	}

	public static float GetFinalVoiceVolume(float fSoundPosX, float fRange, float fLocalVol)
	{
		if (NKCScenManager.GetScenManager().GetNKCPowerSaveMode().GetEnable())
		{
			return 0f;
		}
		if (fRange <= 0f)
		{
			return m_fVoiceVol * fLocalVol * m_fAllVol;
		}
		float num = Mathf.Abs(m_fCamPosX - fSoundPosX);
		if (num <= fRange)
		{
			return m_fVoiceVol * fLocalVol * m_fAllVol;
		}
		float num2 = 1f - num * 0.01f * 0.1f;
		if (num2 < 0.3f)
		{
			num2 = 0.3f;
		}
		return m_fVoiceVol * fLocalVol * num2 * m_fAllVol;
	}

	public static bool IsPlayingVoice(int soundUID = -1)
	{
		LinkedListNode<SoundData> linkedListNode = m_linklistSoundData.First;
		while (linkedListNode != null)
		{
			SoundData value = linkedListNode.Value;
			if (value != null && value.m_Track == SOUND_TRACK.VOICE && value.m_AudioSource.isPlaying)
			{
				if (soundUID == -1)
				{
					return true;
				}
				if (value.m_SoundUID == soundUID)
				{
					return true;
				}
			}
			if (linkedListNode != null)
			{
				linkedListNode = linkedListNode.Next;
			}
		}
		return false;
	}

	public static SoundData GetPlayingVoiceData(int soundUID)
	{
		LinkedListNode<SoundData> linkedListNode = m_linklistSoundData.First;
		while (linkedListNode != null)
		{
			SoundData value = linkedListNode.Value;
			if (value.m_SoundUID == soundUID)
			{
				return value;
			}
			if (linkedListNode != null)
			{
				linkedListNode = linkedListNode.Next;
			}
		}
		return null;
	}

	public static void SetChangeMusicTime(float fValue)
	{
		SoundData nowMusicData = GetNowMusicData();
		if (nowMusicData != null && null != nowMusicData.m_AudioSource)
		{
			nowMusicData.m_AudioSource.time = nowMusicData.m_AudioSource.clip.length * Mathf.Min(0.99f, fValue);
			nowMusicData.m_AudioSource.Play();
		}
	}

	public static float GetMusicTime()
	{
		SoundData nowMusicData = GetNowMusicData();
		if (nowMusicData != null && null != nowMusicData.m_AudioSource)
		{
			return nowMusicData.m_AudioSource.time;
		}
		return 0f;
	}

	public static AudioSource GetAudioSource()
	{
		return GetNowMusicData()?.m_AudioSource;
	}

	public static float GetNowMusicLocalVolume()
	{
		return GetNowMusicData()?.m_fLocalVol ?? 0f;
	}

	public static void SetSoundEffect(bool bActive)
	{
		m_bSoundEffect = bActive;
		if (bActive)
		{
			NKCAssetResourceData nKCAssetResourceData = NKCAssetResourceManager.OpenResource<AudioMixer>("ab_audio_mixer", "AB_SOUND_AUDIO_MIXER_01");
			if (nKCAssetResourceData == null || nKCAssetResourceData.GetAsset<AudioMixer>() == null)
			{
				return;
			}
			AudioMixer asset = nKCAssetResourceData.GetAsset<AudioMixer>();
			if (null == asset)
			{
				return;
			}
			AudioMixerGroup[] array = asset.FindMatchingGroups("Master");
			if (array == null)
			{
				return;
			}
			AudioMixerGroup[] array2 = array;
			foreach (AudioMixerGroup audioMixerGroup in array2)
			{
				switch (audioMixerGroup.name)
				{
				case "InBgm":
					m_AudioMixerGroupInBgm = audioMixerGroup;
					continue;
				case "InSoundNormal":
					m_AudioMixerGroupInSoundNormal = audioMixerGroup;
					continue;
				case "InSoundVoice":
					m_AudioMixerGroupInSoundVoice = audioMixerGroup;
					continue;
				case "OutBgm":
					m_AudioMixerGroupOutBgm = audioMixerGroup;
					continue;
				case "OutSoundNormal":
					m_AudioMixerGroupOutSoundNormal = audioMixerGroup;
					continue;
				case "OutSoundVoice":
					m_AudioMixerGroupOutSoundVoice = audioMixerGroup;
					continue;
				}
				if (m_dicCustomAudioMixerGroup.ContainsKey(audioMixerGroup.name))
				{
					m_dicCustomAudioMixerGroup[audioMixerGroup.name] = audioMixerGroup;
				}
			}
			UpdateAudioMixerToGameData();
		}
		else
		{
			m_AudioMixerGroupInBgm = null;
			m_AudioMixerGroupInSoundNormal = null;
			m_AudioMixerGroupInSoundVoice = null;
			m_AudioMixerGroupOutBgm = null;
			m_AudioMixerGroupOutSoundNormal = null;
			m_AudioMixerGroupOutSoundVoice = null;
			NKCAssetResourceManager.CloseResource("ab_audio_mixer", "AB_SOUND_AUDIO_MIXER_01");
		}
		ResetBGMEffect(bActive);
	}

	private static void UpdateAudioMixerToGameData()
	{
		NKCScenManager scenManager = NKCScenManager.GetScenManager();
		if (!(null == scenManager))
		{
			NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
			if (gameOptionData != null)
			{
				float soundVolumeAsFloat = gameOptionData.GetSoundVolumeAsFloat(NKC_GAME_OPTION_SOUND_GROUP.VOICE);
				SetMixerVolume("InSoundVoice", soundVolumeAsFloat);
				SetMixerVolume("OutSoundVoice", soundVolumeAsFloat);
				soundVolumeAsFloat = gameOptionData.GetSoundVolumeAsFloat(NKC_GAME_OPTION_SOUND_GROUP.BGM);
				SetMixerVolume("InBgm", soundVolumeAsFloat);
				SetMixerVolume("OutBgm", soundVolumeAsFloat);
				soundVolumeAsFloat = gameOptionData.GetSoundVolumeAsFloat(NKC_GAME_OPTION_SOUND_GROUP.SE);
				SetMixerVolume("InSoundNormal", soundVolumeAsFloat);
				SetMixerVolume("OutSoundNormal", soundVolumeAsFloat);
			}
		}
	}

	private static void ResetBGMEffect(bool bActiveEffect)
	{
		SoundData nowMusicData = GetNowMusicData();
		if (nowMusicData != null && null != nowMusicData.m_AudioSource)
		{
			if (bActiveEffect)
			{
				nowMusicData.m_AudioSource.outputAudioMixerGroup = GetAudioMixerGroupMusic();
			}
			else
			{
				nowMusicData.m_AudioSource.outputAudioMixerGroup = null;
			}
		}
	}

	private static AudioMixerGroup GetAudioMixerGroupMusic()
	{
		if (!m_bSoundEffect)
		{
			return null;
		}
		if (m_bCurrentSceneInGameMode)
		{
			return GetAudioMixerGroup("InBgm");
		}
		return GetAudioMixerGroup("OutBgm");
	}

	private static AudioMixerGroup GetAudioMixerGroupSound(SOUND_TRACK soundType, string audioMixerGroupName)
	{
		if (!m_bSoundEffect)
		{
			return null;
		}
		if (!string.IsNullOrEmpty(audioMixerGroupName) && m_dicCustomAudioMixerGroup.ContainsKey(audioMixerGroupName))
		{
			return m_dicCustomAudioMixerGroup[audioMixerGroupName];
		}
		if (m_bCurrentSceneInGameMode)
		{
			if (soundType != SOUND_TRACK.VOICE)
			{
				return GetAudioMixerGroup("InSoundNormal");
			}
			return GetAudioMixerGroup("InSoundVoice");
		}
		if (soundType != SOUND_TRACK.VOICE)
		{
			return GetAudioMixerGroup("OutSoundNormal");
		}
		return GetAudioMixerGroup("OutSoundVoice");
	}

	private static AudioMixerGroup GetAudioMixerGroup(string mixerName)
	{
		switch (mixerName)
		{
		case "InBgm":
			return m_AudioMixerGroupInBgm;
		case "InSoundNormal":
			return m_AudioMixerGroupInSoundNormal;
		case "InSoundVoice":
			return m_AudioMixerGroupInSoundVoice;
		case "OutBgm":
			return m_AudioMixerGroupOutBgm;
		case "OutSoundNormal":
			return m_AudioMixerGroupOutSoundNormal;
		case "OutSoundVoice":
			return m_AudioMixerGroupOutSoundVoice;
		default:
			if (m_dicCustomAudioMixerGroup.ContainsKey(mixerName))
			{
				return m_dicCustomAudioMixerGroup[mixerName];
			}
			return null;
		}
	}

	public static void SetMixerVolume(string mixerName, float volume)
	{
		if (m_bSoundEffect)
		{
			AudioMixerGroup audioMixerGroup = GetAudioMixerGroup(mixerName);
			if (!(null == audioMixerGroup))
			{
				GetMixerVolumeValue(mixerName, out var fMax, out var fMin);
				float value = (fMax - fMin) * volume + fMin;
				audioMixerGroup.audioMixer.SetFloat(mixerName, value);
			}
		}
	}

	private static void GetMixerVolumeValue(string mixerName, out float fMax, out float fMin)
	{
		float num = 0f;
		float num2 = 0f;
		switch (mixerName)
		{
		case "InBgm":
		case "OutBgm":
			num2 = 6f;
			num = 70f;
			break;
		case "InSoundNormal":
		case "OutSoundNormal":
			num2 = 8f;
			num = 60f;
			break;
		case "InSoundVoice":
		case "OutSoundVoice":
			num2 = 4f;
			num = 80f;
			break;
		default:
			num2 = 0f;
			num = 60f;
			break;
		}
		fMax = num2;
		fMin = num2 / (100f - num) * num * -1f;
	}

	public static void AddAudioMixerGroupKey(string newAudioMixerGroupName)
	{
		if (!string.IsNullOrEmpty(newAudioMixerGroupName))
		{
			if (m_dicCustomAudioMixerGroup.ContainsKey(newAudioMixerGroupName))
			{
				Debug.Log("<color=red>Fail - AddAudioMixerGroupKey, duplicated group name : " + newAudioMixerGroupName + "  </color>");
			}
			else
			{
				m_dicCustomAudioMixerGroup.Add(newAudioMixerGroupName, null);
			}
		}
	}
}
