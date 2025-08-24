using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationSubMainStreamEPSlot : MonoBehaviour
{
	public delegate void OnEPSlotSelect(int episodeID);

	public NKCUIComToggle m_tgl;

	public Animator m_Ani;

	public Text m_lbOn;

	public Text m_lbOff;

	public NKCComTMPUIText m_lbOnTMP;

	public NKCComTMPUIText m_lbOffTMP;

	public GameObject m_objRedDot;

	public GameObject m_objEventDrop;

	private OnEPSlotSelect m_dOnEPSlotSelect;

	private int m_EpisodeID;

	private int m_UIIndex = -1;

	private bool m_bSelected;

	public void InitUI(OnEPSlotSelect onSelected, NKCUIComToggleGroup toggleGroup)
	{
		m_tgl.OnValueChanged.RemoveAllListeners();
		m_tgl.OnValueChanged.AddListener(OnTgl);
		m_tgl.m_bGetCallbackWhileLocked = true;
		m_tgl.SetToggleGroup(toggleGroup);
		m_dOnEPSlotSelect = onSelected;
	}

	public int GetEpisodeID()
	{
		return m_EpisodeID;
	}

	public int GetUIIndex()
	{
		return m_UIIndex;
	}

	public void SetData(int episodeID, string epName, int uIIndex)
	{
		m_EpisodeID = episodeID;
		m_UIIndex = uIIndex;
		NKCUtil.SetLabelText(m_lbOn, epName);
		NKCUtil.SetLabelText(m_lbOnTMP, epName);
		NKCUtil.SetLabelText(m_lbOff, epName);
		NKCUtil.SetLabelText(m_lbOffTMP, epName);
		NKMEpisodeTempletV2 episodeTemplet = NKMEpisodeTempletV2.Find(episodeID, EPISODE_DIFFICULTY.NORMAL);
		NKCUtil.SetGameobjectActive(m_objEventDrop, NKMEpisodeMgr.CheckEpisodeHasEventDrop(episodeTemplet) || NKMEpisodeMgr.CheckEpisodeHasBuffDrop(episodeTemplet));
		RefreshRedDot();
	}

	public void SetSelected(bool bValue)
	{
		if (m_bSelected || bValue)
		{
			if (bValue)
			{
				m_Ani.Play("ON_IDLE");
			}
			else
			{
				m_Ani.Play("OFF_IDLE");
			}
		}
		m_tgl.Select(bValue, bForce: true, bImmediate: true);
		m_bSelected = bValue;
	}

	public void ChangeSelected(bool bValue)
	{
		if (m_bSelected || bValue)
		{
			if (bValue)
			{
				m_Ani.SetTrigger("ON");
			}
			else
			{
				m_Ani.SetTrigger("OFF");
			}
		}
		m_tgl.Select(bValue, bForce: true, bImmediate: true);
		m_bSelected = bValue;
	}

	private void OnTgl(bool bValue)
	{
		if (bValue)
		{
			m_dOnEPSlotSelect?.Invoke(m_EpisodeID);
		}
	}

	public void RefreshRedDot()
	{
		NKCUtil.SetGameobjectActive(m_objRedDot, NKMEpisodeMgr.HasReddot(m_EpisodeID));
	}
}
