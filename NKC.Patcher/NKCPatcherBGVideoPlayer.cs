using System;
using System.Collections;
using AssetBundles;
using Cs.Logging;
using UnityEngine;
using UnityEngine.Video;

namespace NKC.Patcher;

public class NKCPatcherBGVideoPlayer : MonoBehaviour
{
	private VideoPlayer m_videoPlayer;

	private Coroutine _coroutine;

	private Action<bool> _onAction;

	public void Awake()
	{
		m_videoPlayer = GetComponent<VideoPlayer>();
		if (m_videoPlayer != null)
		{
			m_videoPlayer.targetCamera = GameObject.Find("SCEN_UI_Camera")?.GetComponent<Camera>();
		}
	}

	public void SetAction(Action<bool> onAction)
	{
		_onAction = onAction;
	}

	public void PlayVideo()
	{
		if (_coroutine != null)
		{
			StopCoroutine(_coroutine);
		}
		_coroutine = StartCoroutine(Play());
	}

	private string GetRawPatchMoviePath(string movieAssetPath)
	{
		string text = AssetBundleManager.GetLocalDownloadPath() ?? "";
		if (NKCDefineManager.DEFINE_UNITY_EDITOR())
		{
			text += "/";
		}
		string text2 = text + "ASSET_RAW/";
		Debug.Log("[PatchVideoPlayer] localRawPath : " + text2);
		foreach (string item in AssetBundleManager.GetMergedVariantString(movieAssetPath))
		{
			string text3 = text2 + item;
			if (NKCPatchUtility.IsFileExists(text3))
			{
				string text4 = text3.Replace(text, "");
				NKCPatchInfo defaultDownloadHistoryPatchInfo = PatchManifestManager.BasePatchInfoController.GetDefaultDownloadHistoryPatchInfo();
				if (defaultDownloadHistoryPatchInfo != null && defaultDownloadHistoryPatchInfo.PatchInfoExists(text4))
				{
					Debug.Log("[PatchVideoPlayer] Exist in downloadedHistoryPatchInfo : " + text2);
					return text3;
				}
				NKCPatchInfo curPatchInfo = PatchManifestManager.BasePatchInfoController.GetCurPatchInfo();
				if (curPatchInfo != null && curPatchInfo.PatchInfoExists(text4))
				{
					Debug.Log("[PatchVideoPlayer] Exist in currentPatchInfo : " + text2);
					return text3;
				}
			}
		}
		return string.Empty;
	}

	private IEnumerator Play()
	{
		Log.Debug("[BG Video Play]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchChecker/NKCPatcherBGVideoPlayer.cs", 85);
		if (m_videoPlayer == null)
		{
			_onAction?.Invoke(obj: true);
			yield break;
		}
		string text = AssetBundleManager.GetRawFilePath("Movie/PatchMovieHiRes.mp4");
		bool flag = NKCPatchDownloader.Instance.IsFileWillDownloaded("ASSET_RAW/Movie/PatchMovieHiRes.mp4");
		Debug.Log($"Hi-res movie path[{text}] moviePatch[{flag}]");
		string rawPatchMoviePath = GetRawPatchMoviePath("Movie/PatchMovieHiRes.mp4");
		if (!string.IsNullOrEmpty(rawPatchMoviePath))
		{
			text = rawPatchMoviePath;
		}
		if (string.IsNullOrEmpty(text) || flag)
		{
			if (string.IsNullOrEmpty(text))
			{
				Debug.Log("Hi-res movie not found. playing default movie");
			}
			else
			{
				Debug.Log("Hi-res movie found but will updated. playing default movie");
			}
			m_videoPlayer.clip = Resources.Load<VideoClip>("PatchMovie");
			if (m_videoPlayer.clip == null)
			{
				_onAction?.Invoke(obj: true);
				yield break;
			}
		}
		else
		{
			Debug.Log("playing hi-res movie");
			m_videoPlayer.url = text;
		}
		bool flag2 = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_SOUND_MUTE", 0) == 1;
		float num = 100f;
		float num2 = 60f;
		string text2 = PlayerPrefs.GetString("NKM_LOCAL_SAVE_GAME_OPTION_SOUND_VOLUMES", "");
		if (text2 != "")
		{
			string[] array = text2.Split(':');
			int num3 = 3;
			if (array.Length > num3 && int.TryParse(array[num3], out var result))
			{
				num = result;
			}
			num3 = 1;
			if (array.Length > num3 && int.TryParse(array[num3], out var result2))
			{
				num2 = result2;
			}
		}
		num /= 100f;
		num2 /= 100f;
		float volume = (flag2 ? 0f : (num * num2));
		m_videoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
		m_videoPlayer.enabled = true;
		m_videoPlayer.isLooping = true;
		m_videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
		m_videoPlayer.EnableAudioTrack(0, enabled: true);
		m_videoPlayer.controlledAudioTrackCount = 1;
		m_videoPlayer.SetDirectAudioVolume(0, volume);
		m_videoPlayer.SetDirectAudioMute(0, !Application.isFocused);
		m_videoPlayer.playbackSpeed = 1f;
		m_videoPlayer.Prepare();
		while (!m_videoPlayer.isPrepared)
		{
			yield return null;
		}
		m_videoPlayer.Play();
		Debug.Log("[VideoPlayer] Play");
		_onAction?.Invoke(obj: false);
		yield return null;
	}

	public void StopBG()
	{
		Log.Debug("[BG Video Stop]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchChecker/NKCPatcherBGVideoPlayer.cs", 183);
		_ = _coroutine;
		if (m_videoPlayer != null)
		{
			m_videoPlayer.Stop();
		}
	}

	public void OnFocus(bool focus)
	{
		if (m_videoPlayer != null)
		{
			m_videoPlayer.SetDirectAudioMute(0, !focus);
		}
	}
}
