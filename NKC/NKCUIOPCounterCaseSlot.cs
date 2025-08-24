using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIOPCounterCaseSlot : MonoBehaviour
{
	public delegate void OnClick();

	public NKCUIComButton m_NKCUIOPCounterCaseSlot;

	public Slider m_NKM_UI_COUNTER_CASE_SLOT_GAUGE;

	public Text m_NKM_UI_COUNTER_CASE_SLOT_GAUGE_TEXT_NUMBER;

	public Text m_NKM_UI_COUNTER_CASE_SLOT_TITLE;

	public GameObject m_objLock;

	public Text m_lbLockText;

	public GameObject m_objRedDot;

	private OnClick dOnClick;

	private bool m_bInitComplete;

	private void InitUI()
	{
		if (m_NKCUIOPCounterCaseSlot != null)
		{
			m_NKCUIOPCounterCaseSlot.PointerClick.RemoveAllListeners();
			m_NKCUIOPCounterCaseSlot.PointerClick.AddListener(OnClickBtn);
		}
		m_bInitComplete = true;
	}

	public void SetData(NKMEpisodeTempletV2 episodeTemplet, ContentsType contentsType, OnClick onClick)
	{
		if (episodeTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_COUNTER_CASE_SLOT_GAUGE.gameObject, bValue: false);
			m_NKM_UI_COUNTER_CASE_SLOT_GAUGE_TEXT_NUMBER.text = "";
			return;
		}
		if (!m_bInitComplete)
		{
			InitUI();
		}
		dOnClick = onClick;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		bool flag = false;
		flag = NKMEpisodeMgr.IsPossibleEpisode(myUserData, episodeTemplet);
		NKCUtil.SetGameobjectActive(m_objLock, !flag);
		NKCUtil.SetGameobjectActive(m_lbLockText, !NKCContentManager.IsContentsUnlocked(contentsType));
		NKCUtil.SetLabelText(m_lbLockText, NKCContentManager.GetLockedMessage(contentsType));
		NKCUtil.SetLabelText(m_NKM_UI_COUNTER_CASE_SLOT_TITLE, episodeTemplet.GetEpisodeName());
		NKCUtil.SetGameobjectActive(m_objRedDot, NKCContentManager.CheckNewCounterCase(episodeTemplet));
		NKCUtil.SetGameobjectActive(m_NKM_UI_COUNTER_CASE_SLOT_GAUGE.gameObject, bValue: false);
		m_NKM_UI_COUNTER_CASE_SLOT_GAUGE_TEXT_NUMBER.text = "";
	}

	private void OnClickBtn()
	{
		dOnClick?.Invoke();
	}
}
