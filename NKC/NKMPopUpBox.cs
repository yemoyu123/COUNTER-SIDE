using System.Collections;
using NKC.Publisher;
using NKC.UI;
using UnityEngine;

namespace NKC;

public class NKMPopUpBox
{
	public delegate bool WaitFlagGetter();

	private static GameObject m_NUF_POPUP_WAIT_BOX_Panel;

	private static string m_WaitBoxTimeOutMsg = "";

	private static float m_WaitBoxTimeMax = 0f;

	private static GameObject m_Cog_3;

	private static GameObject m_NKM_UI_WAIT;

	private static GameObject m_Cog_3_Small;

	private static WaitFlagGetter dWaitFlagGetter;

	public static void Init()
	{
		m_NUF_POPUP_WAIT_BOX_Panel = NKCUIManager.OpenUI("NUF_POPUP_WAIT_BOX_Panel");
		m_Cog_3 = m_NUF_POPUP_WAIT_BOX_Panel.transform.Find("Cog 3").gameObject;
		m_NKM_UI_WAIT = m_NUF_POPUP_WAIT_BOX_Panel.transform.Find("NKM_UI_WAIT").gameObject;
		m_Cog_3_Small = m_NUF_POPUP_WAIT_BOX_Panel.transform.Find("Cog 3_Small").gameObject;
		NKCUtil.SetGameobjectActive(m_NUF_POPUP_WAIT_BOX_Panel, bValue: false);
	}

	public static void Update()
	{
		if (!m_NUF_POPUP_WAIT_BOX_Panel.activeSelf)
		{
			return;
		}
		if (dWaitFlagGetter != null && !dWaitFlagGetter())
		{
			m_WaitBoxTimeMax = 0f;
			CloseWaitBox();
		}
		else
		{
			if (!(m_WaitBoxTimeMax > 0f))
			{
				return;
			}
			m_WaitBoxTimeMax -= Time.deltaTime;
			if (m_WaitBoxTimeMax <= 0f)
			{
				CloseWaitBox();
				if (!string.IsNullOrEmpty(m_WaitBoxTimeOutMsg))
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_FAIL_NET, m_WaitBoxTimeOutMsg);
				}
				m_WaitBoxTimeMax = 0f;
			}
		}
	}

	public static void OpenWaitBox(NKC_OPEN_WAIT_BOX_TYPE eNKC_OPEN_WAIT_BOX_TYPE, float fWaitTimeMax = 0f, string timeOutMsg = "", WaitFlagGetter waitFlagGetter = null)
	{
		if (waitFlagGetter == null || waitFlagGetter())
		{
			dWaitFlagGetter = waitFlagGetter;
			switch (eNKC_OPEN_WAIT_BOX_TYPE)
			{
			case NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL:
				OpenWaitBox(fWaitTimeMax, timeOutMsg);
				break;
			case NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL:
				OpenSmallWaitBox(fWaitTimeMax, timeOutMsg);
				break;
			case NKC_OPEN_WAIT_BOX_TYPE.NOWBT_CLOSE:
				CloseWaitBox();
				break;
			}
		}
	}

	public static void OpenPublisherAPIWaitBox(NKC_OPEN_WAIT_BOX_TYPE eNKC_OPEN_WAIT_BOX_TYPE)
	{
		if (NKCPublisherModule.Busy)
		{
			OpenWaitBox(eNKC_OPEN_WAIT_BOX_TYPE, 10f, NKCPublisherModule.GetErrorMessage(NKC_PUBLISHER_RESULT_CODE.NPRC_TIMEOUT), NKCPublisherModule.IsBusy);
		}
	}

	public static IEnumerator Wait(NKC_OPEN_WAIT_BOX_TYPE eNKC_OPEN_WAIT_BOX_TYPE, float fWaitTimeMax = 0f, string timeOutMsg = "", WaitFlagGetter waitFlagGetter = null)
	{
		OpenWaitBox(eNKC_OPEN_WAIT_BOX_TYPE, fWaitTimeMax, timeOutMsg, waitFlagGetter);
		while (IsOpenedWaitBox())
		{
			yield return null;
		}
	}

	public static IEnumerator WaitPublisherAPI(NKC_OPEN_WAIT_BOX_TYPE eNKC_OPEN_WAIT_BOX_TYPE)
	{
		OpenPublisherAPIWaitBox(eNKC_OPEN_WAIT_BOX_TYPE);
		while (IsOpenedWaitBox())
		{
			yield return null;
		}
	}

	public static void OpenSmallWaitBox(float fWaitTimeMax = 0f, string timeOutMsg = "")
	{
		m_WaitBoxTimeMax = fWaitTimeMax;
		m_WaitBoxTimeOutMsg = timeOutMsg;
		NKCUtil.SetGameobjectActive(m_NUF_POPUP_WAIT_BOX_Panel, bValue: true);
		NKCUtil.SetGameobjectActive(m_Cog_3, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_WAIT, bValue: false);
		NKCUtil.SetGameobjectActive(m_Cog_3_Small, bValue: true);
	}

	public static void OpenWaitBox(float fWaitTimeMax = 0f, string timeOutMsg = "")
	{
		m_WaitBoxTimeMax = fWaitTimeMax;
		m_WaitBoxTimeOutMsg = timeOutMsg;
		NKCUtil.SetGameobjectActive(m_NUF_POPUP_WAIT_BOX_Panel, bValue: true);
		NKCUtil.SetGameobjectActive(m_Cog_3, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_WAIT, bValue: true);
		NKCUtil.SetGameobjectActive(m_Cog_3_Small, bValue: false);
	}

	public static void CloseWaitBox()
	{
		dWaitFlagGetter = null;
		NKCUtil.SetGameobjectActive(m_NUF_POPUP_WAIT_BOX_Panel, bValue: false);
	}

	public static bool IsOpenedWaitBox()
	{
		return m_NUF_POPUP_WAIT_BOX_Panel.activeSelf;
	}
}
