using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCPopupEventMissionSlot : MonoBehaviour
{
	public delegate void OnTouchProgress(NKMMissionTemplet templet, NKMMissionData data);

	public delegate void OnTouchComplete(NKMMissionTemplet templet, NKMMissionData data);

	public Text m_txtTitle;

	public Text m_txtExplain;

	public GameObject m_objClear;

	public GameObject m_objComplete;

	[Header("진행도")]
	public Text m_txtGauge;

	public Slider m_slider;

	public Image m_imgProgress;

	[Header("버튼")]
	public NKCUIComButton m_btnComplete;

	public NKCUIComButton m_btnProgress;

	public NKCUIComButton m_btnDisable;

	private OnTouchProgress dOnTouchProgress;

	private OnTouchComplete dOnTouchComplete;

	private NKMMissionTemplet m_missionTemplet;

	private NKMMissionData m_missionData;

	public void Init(OnTouchProgress onTouchProgress, OnTouchComplete onTouchComplete)
	{
		if (m_btnComplete != null)
		{
			m_btnComplete.PointerClick.RemoveAllListeners();
			m_btnComplete.PointerClick.AddListener(OnTouchCompleteBtn);
		}
		if (m_btnProgress != null)
		{
			m_btnProgress.PointerClick.RemoveAllListeners();
			m_btnProgress.PointerClick.AddListener(OnTouchProgressBtn);
		}
		if (m_btnDisable != null)
		{
			m_btnDisable.PointerClick.RemoveAllListeners();
			m_btnDisable.PointerClick.AddListener(OnTouchDisableBtn);
		}
		dOnTouchProgress = onTouchProgress;
		dOnTouchComplete = onTouchComplete;
	}

	public void SetData(NKMMissionTemplet missionTemplet, NKMMissionData missionData)
	{
		m_missionTemplet = missionTemplet;
		m_missionData = missionData;
		NKCUtil.SetLabelText(m_txtTitle, missionTemplet.GetTitle());
		NKCUtil.SetLabelText(m_txtExplain, missionTemplet.GetDesc());
		bool flag = missionData != null && NKMMissionManager.CanComplete(missionTemplet, NKCScenManager.CurrentUserData(), missionData) == NKM_ERROR_CODE.NEC_OK;
		bool flag2 = missionData?.IsComplete ?? false;
		NKCUtil.SetGameobjectActive(m_objClear, flag);
		NKCUtil.SetGameobjectActive(m_objComplete, flag2);
		long num = 0L;
		if (missionData != null)
		{
			num = ((!(flag || flag2)) ? missionData.times : missionTemplet.m_Times);
		}
		if (m_slider != null)
		{
			if (missionTemplet.m_Times > 0)
			{
				m_slider.value = (float)num / (float)missionTemplet.m_Times;
			}
			else
			{
				m_slider.value = 0f;
			}
		}
		if (m_imgProgress != null)
		{
			if (missionTemplet.m_Times > 0)
			{
				m_imgProgress.fillAmount = (float)num / (float)missionTemplet.m_Times;
			}
			else
			{
				m_imgProgress.fillAmount = 0f;
			}
		}
		NKCUtil.SetLabelText(m_txtGauge, flag ? "" : $"{num}/{missionTemplet.m_Times}");
		NKCUtil.SetGameobjectActive(m_btnProgress, !flag && !flag2);
		NKCUtil.SetGameobjectActive(m_btnComplete, flag);
		NKCUtil.SetGameobjectActive(m_btnDisable, flag2);
	}

	private void OnTouchCompleteBtn()
	{
		dOnTouchComplete?.Invoke(m_missionTemplet, m_missionData);
	}

	private void OnTouchProgressBtn()
	{
		dOnTouchProgress?.Invoke(m_missionTemplet, m_missionData);
	}

	private void OnTouchDisableBtn()
	{
	}
}
