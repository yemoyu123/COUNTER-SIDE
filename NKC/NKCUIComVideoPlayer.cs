using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssetBundles;
using NKC.UI;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.Video;

namespace NKC;

[RequireComponent(typeof(VideoPlayer))]
public abstract class NKCUIComVideoPlayer : MonoBehaviour
{
	public enum eVideoMessage
	{
		PlayFailed,
		PlayBegin,
		PlayComplete
	}

	public delegate void VideoPlayMessageCallback(eVideoMessage message);

	protected enum VideoState
	{
		Stop,
		PreparingPlay,
		Playing
	}

	public struct strVideoCaption
	{
		public long startTime;

		public long endTime;

		public string caption;

		public strVideoCaption(long sTime, long eTime, string str)
		{
			startTime = sTime;
			endTime = eTime;
			caption = str;
		}
	}

	protected static HashSet<NKCUIComVideoPlayer> s_setVideoPlayers = new HashSet<NKCUIComVideoPlayer>();

	protected VideoPlayer m_VideoPlayer;

	[Header("영상 재생속도")]
	public float m_fMoviePlaySpeed = 1f;

	[Header("영상 사운드 사용하는가?")]
	public bool m_bUseSound;

	[Header("영상 파일명. ASSET_RAW/Movie/ 안에서의 경로를 입력할 것")]
	public string m_sFilename;

	private const float PREPARE_WAIT_LIMIT = 1.5f;

	private bool m_bForcePlay;

	protected VideoState m_VideoState;

	private VideoPlayMessageCallback dVideoPlayMessageCallback;

	protected VideoPlayer VideoPlayer
	{
		get
		{
			if (m_VideoPlayer == null)
			{
				m_VideoPlayer = GetComponent<VideoPlayer>();
				m_VideoPlayer.aspectRatio = VideoAspectRatio.FitOutside;
				m_VideoPlayer.enabled = false;
			}
			return m_VideoPlayer;
		}
	}

	private bool IsPrepared
	{
		get
		{
			if (!m_bForcePlay)
			{
				NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
				if (gameOptionData != null && !gameOptionData.UseVideoTexture)
				{
					return true;
				}
			}
			if (!(VideoPlayer != null))
			{
				return false;
			}
			return VideoPlayer.isPrepared;
		}
	}

	public bool IsPlaying
	{
		get
		{
			if (!(VideoPlayer != null))
			{
				return false;
			}
			return VideoPlayer.isPlaying;
		}
	}

	public bool PlayOnAwake
	{
		get
		{
			if (!(VideoPlayer != null))
			{
				return false;
			}
			return VideoPlayer.playOnAwake;
		}
	}

	public static void OnUpdateVolume()
	{
		foreach (NKCUIComVideoPlayer s_setVideoPlayer in s_setVideoPlayers)
		{
			if (s_setVideoPlayer != null)
			{
				s_setVideoPlayer.VolumeUpdated();
			}
		}
	}

	private void OnDestroy()
	{
	}

	protected void Register()
	{
		s_setVideoPlayers.Add(this);
	}

	protected void Unregister()
	{
		if (s_setVideoPlayers != null)
		{
			_ = s_setVideoPlayers.Count;
		}
		s_setVideoPlayers.Remove(this);
	}

	public bool IsPlayingOrPreparing()
	{
		if (VideoPlayer == null)
		{
			return false;
		}
		if (VideoPlayer.isPlaying)
		{
			return true;
		}
		if (m_VideoState == VideoState.PreparingPlay)
		{
			return true;
		}
		return false;
	}

	public bool IsPreparing()
	{
		return m_VideoState == VideoState.PreparingPlay;
	}

	public void SetCamera(Camera cam)
	{
		VideoPlayer.targetCamera = cam;
	}

	public void SetAlpha(float fAlpha)
	{
		if (VideoPlayer != null)
		{
			VideoPlayer.targetCameraAlpha = fAlpha;
		}
	}

	private void Start()
	{
		Register();
		if (PlayOnAwake && m_VideoState == VideoState.Stop)
		{
			Play(VideoPlayer.isLooping, m_bUseSound);
		}
	}

	public void Prepare(string rawVideoFileName)
	{
		m_sFilename = rawVideoFileName;
		Prepare();
	}

	public virtual void Prepare()
	{
		if (!m_bForcePlay)
		{
			NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
			if (gameOptionData != null && !gameOptionData.UseVideoTexture)
			{
				return;
			}
		}
		if (VideoPlayer.isPlaying)
		{
			VideoPlayer.Stop();
		}
		string rawFilePath = AssetBundleManager.GetRawFilePath("Movie/" + m_sFilename);
		if (!VideoPlayer.isPrepared || VideoPlayer.url != rawFilePath)
		{
			VideoPlayer.enabled = true;
			VideoPlayer.url = rawFilePath;
			VideoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
			VideoPlayer.EnableAudioTrack(0, m_bUseSound);
			VideoPlayer.controlledAudioTrackCount = (ushort)(m_bUseSound ? 1 : 0);
			VideoPlayer.SetDirectAudioVolume(0, NKCSoundManager.GetFinalVol(0f, 0f, 1f));
			VideoPlayer.SetDirectAudioMute(0, !Application.isFocused);
			VideoPlayer.Prepare();
		}
	}

	protected virtual void OnStateChange(VideoState state)
	{
		m_VideoState = state;
	}

	public virtual void CleanUp()
	{
		Unregister();
		SetAlpha(1f);
		if (VideoPlayer != null)
		{
			VideoPlayer.Stop();
			VideoPlayer.enabled = false;
		}
	}

	protected virtual bool CanPlayVideo()
	{
		return true;
	}

	public void Play(string rawVideoFileName, bool bLoop, bool bPlaySound = false, VideoPlayMessageCallback videoPlayMessageCallback = null, bool bForcePlay = false)
	{
		if (!IsPlaying || !(rawVideoFileName == m_sFilename))
		{
			m_sFilename = rawVideoFileName;
			Play(bLoop, bPlaySound, videoPlayMessageCallback, bForcePlay);
		}
	}

	public void Play(bool bLoop, bool bPlaySound = false, VideoPlayMessageCallback videoPlayMessageCallback = null, bool bForcePlay = false)
	{
		Register();
		PrepareMovieCaption();
		m_bForcePlay = bForcePlay;
		if (!bForcePlay)
		{
			NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
			if (gameOptionData != null && !gameOptionData.UseVideoTexture)
			{
				OnStateChange(VideoState.Stop);
				videoPlayMessageCallback?.Invoke(eVideoMessage.PlayComplete);
				return;
			}
		}
		if (!base.gameObject.activeInHierarchy || !CanPlayVideo())
		{
			OnStateChange(VideoState.Stop);
			videoPlayMessageCallback?.Invoke(eVideoMessage.PlayComplete);
			return;
		}
		if (VideoPlayer.isPlaying)
		{
			VideoPlayer.Stop();
		}
		m_bUseSound = bPlaySound;
		dVideoPlayMessageCallback = videoPlayMessageCallback;
		VideoPlayer.isLooping = bLoop;
		StopAllCoroutines();
		OnStateChange(VideoState.PreparingPlay);
		StartCoroutine(VideoPlayProcess());
	}

	private void PrepareMovieCaption()
	{
		if (string.IsNullOrEmpty(m_sFilename))
		{
			return;
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(m_sFilename);
		if (string.IsNullOrEmpty(fileNameWithoutExtension) || !AssetBundleManager.IsAssetExists("ab_script_movie_caption", "lua_" + fileNameWithoutExtension))
		{
			return;
		}
		NKMTempletContainer<NKCMovieCaptionTemplet>.Load("ab_script_movie_caption", "lua_" + fileNameWithoutExtension, "MOVIE_CAPTION", NKCMovieCaptionTemplet.LoadFromLUA);
		if (NKMTempletContainer<NKCMovieCaptionTemplet>.Values.Count() <= 0)
		{
			Debug.Log("NKCUIComVideoPlayer::PrepareMovieCaption() : lua_" + fileNameWithoutExtension + " - dont have caption");
			return;
		}
		List<NKCMovieCaptionTemplet> list = NKMTempletContainer<NKCMovieCaptionTemplet>.Values.ToList();
		list.Sort((NKCMovieCaptionTemplet x, NKCMovieCaptionTemplet y) => x.m_StartSecond.CompareTo(y.m_StartSecond));
		List<NKCUIComCaption.CaptionDataTime> list2 = new List<NKCUIComCaption.CaptionDataTime>();
		int num = 0;
		foreach (NKCMovieCaptionTemplet item in list)
		{
			num++;
			string caption = item.m_Caption;
			if (!string.IsNullOrEmpty(NKCStringTable.GetString(item.m_StringKey)))
			{
				caption = NKCStringTable.GetString(item.m_StringKey);
			}
			list2.Add(new NKCUIComCaption.CaptionDataTime(caption, num, item.m_StartSecond, item.m_ShowSecond, item.m_bHideBackground));
		}
		NKCUIManager.NKCUIOverlayCaption.OpenCaption(list2.OrderBy((NKCUIComCaption.CaptionDataTime x) => x.startTime).ToList());
	}

	public void InvalidateCallBack()
	{
		dVideoPlayMessageCallback = null;
	}

	public void Stop()
	{
		StopAllCoroutines();
		VideoPlayer.Stop();
		VideoPlayer.enabled = false;
		NKCUIManager.NKCUIOverlayCaption.CloseAllCaption();
		OnStateChange(VideoState.Stop);
	}

	private IEnumerator VideoPlayProcess()
	{
		Debug.Log("Videoplayer Play process begin");
		OnStateChange(VideoState.PreparingPlay);
		VideoPlayer.enabled = true;
		VideoPlayer.playbackSpeed = m_fMoviePlaySpeed;
		if (!VideoPlayer.isPrepared)
		{
			Debug.Log("Preparing Video");
			OnStateChange(VideoState.PreparingPlay);
			Prepare();
			float waitTime = 0f;
			float targetWaitTime = (m_bForcePlay ? 15f : 1.5f);
			if (!VideoPlayer.isPrepared)
			{
				yield return null;
			}
			while (!VideoPlayer.isPrepared)
			{
				float unscaledDeltaTime = Time.unscaledDeltaTime;
				waitTime += unscaledDeltaTime;
				if (waitTime > targetWaitTime)
				{
					Debug.LogWarning("Video Loading Took too long. cancel video play");
					OnStateChange(VideoState.Stop);
					VideoPlayer.enabled = false;
					dVideoPlayMessageCallback?.Invoke(eVideoMessage.PlayFailed);
					yield break;
				}
				yield return null;
			}
		}
		Debug.Log("Playing Video");
		OnStateChange(VideoState.Playing);
		VideoPlayer.Play();
		yield return null;
		Debug.Log("Video Begin. Waiting video to finish");
		dVideoPlayMessageCallback?.Invoke(eVideoMessage.PlayBegin);
		while (VideoPlayer.isPlaying)
		{
			yield return null;
		}
		Debug.Log("Video Complete");
		dVideoPlayMessageCallback?.Invoke(eVideoMessage.PlayComplete);
	}

	public void SetPlaybackSpeed(float speed)
	{
		m_fMoviePlaySpeed = speed;
		VideoPlayer.playbackSpeed = speed;
	}

	private void OnApplicationQuit()
	{
	}

	private void VolumeUpdated()
	{
		if (!(VideoPlayer == null))
		{
			NKCGameOptionData nKCGameOptionData = NKCScenManager.GetScenManager()?.GetGameOptionData();
			if (nKCGameOptionData != null)
			{
				VideoPlayer.SetDirectAudioMute(0, !Application.isFocused || nKCGameOptionData.SoundMute);
			}
			else
			{
				VideoPlayer.SetDirectAudioMute(0, !Application.isFocused);
			}
			VideoPlayer.SetDirectAudioVolume(0, NKCSoundManager.GetFinalVol(0f, 0f, 1f));
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		VolumeUpdated();
	}
}
