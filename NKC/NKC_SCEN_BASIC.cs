using NKC.Loading;
using NKC.UI;
using NKM;
using UnityEngine;

namespace NKC;

public abstract class NKC_SCEN_BASIC
{
	protected NKM_SCEN_ID m_NKM_SCEN_ID;

	protected NKC_SCEN_STATE m_NKC_SCEN_STATE = NKC_SCEN_STATE.NSS_END;

	protected int m_LoadUICompleteWaitCount;

	protected int m_LoadCompleteWaitCount;

	protected bool m_bLoadedUI;

	protected float m_fLoadingProgress;

	public NKM_SCEN_ID Get_NKM_SCEN_ID()
	{
		return m_NKM_SCEN_ID;
	}

	public NKC_SCEN_STATE Get_NKC_SCEN_STATE()
	{
		return m_NKC_SCEN_STATE;
	}

	public void Set_NKC_SCEN_STATE(NKC_SCEN_STATE eNKC_SCEN_STATE)
	{
		m_NKC_SCEN_STATE = eNKC_SCEN_STATE;
	}

	public NKC_SCEN_BASIC()
	{
	}

	public virtual void ScenChangeStart()
	{
		Debug.LogFormat("{0}.ScenChangeStart", m_NKM_SCEN_ID.ToString());
		m_NKC_SCEN_STATE = NKC_SCEN_STATE.NSS_CHANGE_START;
		m_fLoadingProgress = 0f;
		NKCLoadingScreenManager.SetLoadingProgress(m_fLoadingProgress);
		ScenDataReq();
	}

	public virtual void ScenDataReq()
	{
		Debug.LogFormat("{0}.ScenDataReq", m_NKM_SCEN_ID.ToString());
		m_NKC_SCEN_STATE = NKC_SCEN_STATE.NSS_DATA_REQ_WAIT;
	}

	public virtual void ScenDataReqWaitUpdate()
	{
		Debug.LogFormat("{0}.ScenDataReqWaitUpdate", m_NKM_SCEN_ID.ToString());
		ScenLoadUIStart();
	}

	public virtual void ScenLoadUIStart()
	{
		Debug.LogFormat("{0}.ScenLoadUIStart", m_NKM_SCEN_ID.ToString());
		m_NKC_SCEN_STATE = NKC_SCEN_STATE.NSS_LOADING_UI;
		m_fLoadingProgress = 0.1f;
		NKCLoadingScreenManager.SetLoadingProgress(m_fLoadingProgress);
	}

	public virtual void ScenLoadUIUpdate()
	{
		if (NKCAssetResourceManager.IsLoadEnd())
		{
			m_LoadUICompleteWaitCount = 0;
			ScenLoadUICompleteWait();
		}
		m_fLoadingProgress = 0.1f;
		m_fLoadingProgress += NKCAssetResourceManager.GetLoadProgress() * 0.1f;
		NKCLoadingScreenManager.SetLoadingProgress(m_fLoadingProgress);
	}

	public virtual void ScenLoadUICompleteWait()
	{
		if (m_LoadUICompleteWaitCount > 0)
		{
			ScenLoadUIComplete();
			return;
		}
		m_NKC_SCEN_STATE = NKC_SCEN_STATE.NSS_LOADING_UI_COMPLETE_WAIT;
		m_LoadUICompleteWaitCount++;
		m_fLoadingProgress = 0.4f;
		NKCLoadingScreenManager.SetLoadingProgress(m_fLoadingProgress);
	}

	public virtual void ScenLoadUIComplete()
	{
		Debug.LogFormat("{0}.ScenLoadUIComplete", m_NKM_SCEN_ID.ToString());
		m_NKC_SCEN_STATE = NKC_SCEN_STATE.NSS_LOADING;
	}

	public virtual void ScenLoadUpdate()
	{
		if (NKCAssetResourceManager.IsLoadEnd())
		{
			ScenLoadLastStart();
		}
		m_fLoadingProgress = 0.4f;
		m_fLoadingProgress += NKCAssetResourceManager.GetLoadProgress() * 0.1f;
		NKCLoadingScreenManager.SetLoadingProgress(m_fLoadingProgress);
	}

	public virtual void ScenLoadLastStart()
	{
		Debug.LogFormat("{0}.ScenLoadLastStart", m_NKM_SCEN_ID.ToString());
		m_NKC_SCEN_STATE = NKC_SCEN_STATE.NSS_LOADING_LAST;
	}

	public virtual void ScenLoadLastUpdate()
	{
		if (NKCScenManager.GetScenManager().GetObjectPool().IsLoadComplete())
		{
			m_LoadCompleteWaitCount = 0;
			ScenLoadCompleteWait();
		}
		m_fLoadingProgress = 0.7f;
		m_fLoadingProgress += NKCScenManager.GetScenManager().GetObjectPool().GetLoadProgress() * 0.2f;
		NKCLoadingScreenManager.SetLoadingProgress(m_fLoadingProgress);
	}

	public virtual void ScenLoadCompleteWait()
	{
		if (m_LoadCompleteWaitCount > 0)
		{
			ScenLoadComplete();
			return;
		}
		m_NKC_SCEN_STATE = NKC_SCEN_STATE.NSS_LOADING_COMPLETE_WAIT;
		m_LoadCompleteWaitCount++;
		m_fLoadingProgress = 1f;
		NKCLoadingScreenManager.SetLoadingProgress(m_fLoadingProgress);
	}

	public virtual void ScenLoadComplete()
	{
		Debug.LogFormat("{0}.ScenLoadComplete", m_NKM_SCEN_ID.ToString());
		m_NKC_SCEN_STATE = NKC_SCEN_STATE.NSS_LOADING_COMPLETE;
		NKCUIVoiceManager.CleanUp();
		NKCResourceUtility.SwapResource();
		m_bLoadedUI = true;
	}

	public virtual void ScenStart()
	{
		Debug.LogFormat("{0}.ScenStart", m_NKM_SCEN_ID.ToString());
		m_NKC_SCEN_STATE = NKC_SCEN_STATE.NSS_START;
		PlayScenMusic();
		NKCUIManager.OnSceneOpenComplete();
		if (m_NKM_SCEN_ID != NKM_SCEN_ID.NSI_HOME)
		{
			NKCUIFadeInOut.FadeIn(0.1f);
		}
		else
		{
			NKCUIFadeInOut.FadeIn(0.01f);
		}
	}

	public virtual void PlayScenMusic()
	{
		NKCSoundManager.PlayScenMusic(m_NKM_SCEN_ID);
	}

	public virtual void ScenEnd()
	{
		m_NKC_SCEN_STATE = NKC_SCEN_STATE.NSS_END;
	}

	public virtual void ScenUpdate()
	{
	}

	public virtual bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public virtual bool GoBack()
	{
		return false;
	}

	public virtual void UnloadUI()
	{
		m_bLoadedUI = false;
	}
}
