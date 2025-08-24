using System;
using System.Collections;
using NKC.UI;
using UnityEngine;

namespace NKC;

public class NKCMemoryCleaner : MonoBehaviour
{
	public delegate void OnComplete();

	public bool m_bWaitingMemCleaning;

	private NKCUIManager.eUIUnloadFlag m_eUIUnloadFlag;

	private bool m_bDontCloseOpenedUI = true;

	private OnComplete m_OnComplete;

	public void Clean(OnComplete _OnComplete = null, NKCUIManager.eUIUnloadFlag eUIUnloadFlag = NKCUIManager.eUIUnloadFlag.DEFAULT, bool bDontCloseOpenedUI = true)
	{
		if (m_bWaitingMemCleaning)
		{
			Debug.LogWarning("Waiting NKC Memory Cleaning");
			return;
		}
		m_OnComplete = _OnComplete;
		m_eUIUnloadFlag = eUIUnloadFlag;
		m_bDontCloseOpenedUI = bDontCloseOpenedUI;
		StartCoroutine(Clean_());
	}

	public void UnloadObjectPool()
	{
		if (m_bWaitingMemCleaning)
		{
			Debug.LogWarning("Waiting NKC Memory Cleaning");
		}
		else if (NKCScenManager.GetScenManager() != null && NKCScenManager.GetScenManager().GetObjectPool() != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().Unload();
		}
	}

	public void DoUnloadUnusedAssetsAndGC()
	{
		if (!m_bWaitingMemCleaning)
		{
			StartCoroutine(DoUnloadUnusedAssetsAndGC_());
		}
	}

	private IEnumerator DoUnloadUnusedAssetsAndGC_()
	{
		m_bWaitingMemCleaning = true;
		yield return new WaitForEndOfFrame();
		AsyncOperation async = Resources.UnloadUnusedAssets();
		while (!async.isDone)
		{
			yield return null;
		}
		GC.Collect();
		GC.WaitForPendingFinalizers();
		m_bWaitingMemCleaning = false;
	}

	private IEnumerator Clean_()
	{
		m_bWaitingMemCleaning = true;
		Debug.Log("NKCMemoryCleaner Clean Start");
		NKCUIManager.UnloadAllUI(m_eUIUnloadFlag, m_bDontCloseOpenedUI);
		NKCSoundManager.Unload();
		if (NKCScenManager.GetScenManager() != null && NKCScenManager.GetScenManager().GetObjectPool() != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().Unload();
		}
		yield return new WaitForEndOfFrame();
		AsyncOperation async = Resources.UnloadUnusedAssets();
		while (!async.isDone)
		{
			yield return null;
		}
		GC.Collect();
		GC.WaitForPendingFinalizers();
		Debug.Log("NKCMemoryCleaner Clean Finish");
		m_bWaitingMemCleaning = false;
		if (m_OnComplete != null)
		{
			m_OnComplete();
			m_OnComplete = null;
		}
	}

	public void WaitForClean(OnComplete onComplete)
	{
		StartCoroutine(_WaitForClean(onComplete));
	}

	public IEnumerator _WaitForClean(OnComplete onComplete)
	{
		while (m_bWaitingMemCleaning)
		{
			yield return null;
		}
		onComplete?.Invoke();
	}
}
