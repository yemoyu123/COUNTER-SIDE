using ClientPacket.Mode;
using NKC.UI;
using NKC.UI.Trim;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_TRIM : NKC_SCEN_BASIC
{
	private NKCUITrimMain m_uiTrimMain;

	private NKCUIManager.LoadedUIData m_loadUIDataTrimMain;

	private int m_currentTrimIntervalId;

	private int m_reservedTrimId;

	public int TrimIntervalId => m_currentTrimIntervalId;

	public NKC_SCEN_TRIM()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_TRIM;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		NKCUITrimUtility.TrimIntervalJoin();
		if (!NKCUIManager.IsValid(m_loadUIDataTrimMain))
		{
			m_loadUIDataTrimMain = NKCUITrimMain.OpenNewInstanceAsync();
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (m_uiTrimMain == null)
		{
			if (m_loadUIDataTrimMain != null && m_loadUIDataTrimMain.CheckLoadAndGetInstance<NKCUITrimMain>(out m_uiTrimMain))
			{
				m_uiTrimMain.Init();
				NKCUtil.SetGameobjectActive(m_uiTrimMain.gameObject, bValue: false);
			}
			else
			{
				Debug.LogError("NKC_SCEN_TRIM.ScenLoadUIComplete - ui load AB_UI_TRIM failed");
			}
		}
		_ = NKCUIDeckViewer.Instance;
	}

	public override void ScenStart()
	{
		base.ScenStart();
		if (m_uiTrimMain == null)
		{
			Debug.LogError("TrimMain ui not found");
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_GAME_LOAD_FAILED, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
			return;
		}
		NKMTrimIntervalTemplet nKMTrimIntervalTemplet = NKMTrimIntervalTemplet.Find(NKCSynchronizedTime.ServiceTime);
		m_currentTrimIntervalId = 0;
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.DIMENSION_TRIM))
		{
			string lockedMessage = NKCContentManager.GetLockedMessage(ContentsType.DIMENSION_TRIM);
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, lockedMessage, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
		}
		else if (nKMTrimIntervalTemplet != null && NKCUITrimUtility.OpenTagEnabled)
		{
			m_currentTrimIntervalId = nKMTrimIntervalTemplet.TrimIntervalID;
			m_uiTrimMain.Open(nKMTrimIntervalTemplet, m_reservedTrimId);
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TRIM_NOT_INTERVAL_TIME, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
		}
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		m_reservedTrimId = 0;
		m_uiTrimMain?.Close();
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_uiTrimMain = null;
		m_loadUIDataTrimMain?.CloseInstance();
		m_loadUIDataTrimMain = null;
	}

	public void SetReservedTrim(TrimModeState TrimModeState)
	{
		if (TrimModeState != null)
		{
			m_reservedTrimId = TrimModeState.trimId;
		}
	}
}
