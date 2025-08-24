using System.Collections;
using System.Collections.Generic;
using Cs.Logging;
using UnityEngine;

namespace NKC.Patcher;

public class NKCPatchChecker : MonoBehaviour, IPatcher
{
	public static NKCPatchChecker m_instance;

	public AudioSource m_audioSource;

	public AudioClip m_ambientBGM;

	[SerializeField]
	public NKCPatcherBGVideoPlayer m_patcherVideoPlayer;

	[SerializeField]
	public NKCPatcherUI m_patcherUI;

	private readonly List<IPatchProcessStrategy> _patchProcessModules = new List<IPatchProcessStrategy>();

	public static NKCPatcherUI PatcherUI
	{
		get
		{
			if (m_instance == null)
			{
				return null;
			}
			return m_instance.m_patcherUI;
		}
	}

	public static NKCPatcherBGVideoPlayer PatcherVideoPlayer
	{
		get
		{
			if (m_instance == null)
			{
				return null;
			}
			return m_instance.m_patcherVideoPlayer;
		}
	}

	public bool PatchSuccess => _patchProcessModules.Find((IPatchProcessStrategy x) => x.Status == IPatchProcessStrategy.ExecutionStatus.Fail) == null;

	public string ReasonOfFailure => _patchProcessModules.Find((IPatchProcessStrategy x) => x.Status == IPatchProcessStrategy.ExecutionStatus.Fail)?.ReasonOfFailure;

	private void Awake()
	{
		m_instance = this;
		if (PatcherVideoPlayer == null)
		{
			Log.Error("PatcherVideoPlayer is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchChecker/NKCPatchChecker.cs", 54);
			return;
		}
		if (PatcherUI == null)
		{
			Log.Error("PatcherUI is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchChecker/NKCPatchChecker.cs", 60);
			return;
		}
		PatcherVideoPlayer.SetAction(PatcherUI.OnInvalidPatcherVideoPlayer);
		RegisterPatchProcess();
	}

	public void RegisterPatchProcess()
	{
		_patchProcessModules.Clear();
		_patchProcessModules.Add(new WaitForUnityEditorPatchSkip());
		_patchProcessModules.Add(new WaitForInternetConnection());
		_patchProcessModules.Add(new WaitForDownLoaderInitialization());
		_patchProcessModules.Add(new WaitForAppVersionCheckStatus());
		_patchProcessModules.Add(new WaitForAssetBundleVersionCheckStatus());
		_patchProcessModules.Add(new WaitForDownloadStatus());
		_patchProcessModules.Add(new WaitForTouch());
		_patchProcessModules.Add(new WaitForAssetBundleInitialize());
		_patchProcessModules.Add(new WaitForCheckVersion());
	}

	private void OnApplicationFocus(bool focus)
	{
		if (PatcherVideoPlayer != null)
		{
			PatcherVideoPlayer?.OnFocus(focus);
		}
	}

	public void SetActive(bool active)
	{
		NKCUtil.SetGameobjectActive(this, active);
		PatcherUI.SetActive(active);
	}

	public IEnumerator ProcessPatch()
	{
		foreach (IPatchProcessStrategy patchProcessModule in _patchProcessModules)
		{
			Log.Debug(string.Format("[{0}][{1}] Start", "ProcessPatch", patchProcessModule.GetType()), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchChecker/NKCPatchChecker.cs", 123);
			yield return patchProcessModule.GetEnumerator();
			if (patchProcessModule.ErrorOccurred() || patchProcessModule.SkipNextProcess())
			{
				break;
			}
		}
	}

	public static void StopBGM()
	{
		if (!(m_instance == null) && !(m_instance.m_audioSource == null) && !(m_instance.m_ambientBGM == null))
		{
			m_instance.m_audioSource.Stop();
		}
	}

	public void PlayBGM()
	{
		if (!(m_audioSource == null) && !(m_ambientBGM == null))
		{
			Log.Debug("[PatcherManager] Play BGM", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchChecker/NKCPatchChecker.cs", 159);
			m_audioSource.loop = true;
			m_audioSource.clip = m_ambientBGM;
			m_audioSource.volume = GetVolume();
			m_audioSource.enabled = true;
			m_audioSource.Play();
		}
	}

	public static float GetVolume()
	{
		bool flag = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_SOUND_MUTE", 0) == 1;
		float result = 1f;
		string text = PlayerPrefs.GetString("NKM_LOCAL_SAVE_GAME_OPTION_SOUND_VOLUMES", "");
		if (text != "")
		{
			int num = 3;
			string[] array = text.Split(':');
			if (array.Length > num && int.TryParse(array[num], out var result2))
			{
				result = (float)result2 / 100f;
			}
		}
		if (!flag)
		{
			return result;
		}
		return 0f;
	}
}
