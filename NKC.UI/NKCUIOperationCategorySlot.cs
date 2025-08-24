using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCUIOperationCategorySlot : MonoBehaviour
{
	public delegate void OnSlotSelect(EPISODE_GROUP catgory, bool bShowFade);

	public NKCUIComToggle m_tgl;

	public Animator m_Ani;

	private OnSlotSelect m_dOnSlotSelect;

	private EPISODE_GROUP m_EpisodeGroup;

	public EPISODE_GROUP GetEpisodeGroup()
	{
		return m_EpisodeGroup;
	}

	public void InitUI(EPISODE_GROUP category, OnSlotSelect onSlotSelect)
	{
		m_tgl.OnValueChanged.RemoveAllListeners();
		m_tgl.OnValueChanged.AddListener(OnTgl);
		m_tgl.m_bGetCallbackWhileLocked = true;
		m_dOnSlotSelect = onSlotSelect;
		m_EpisodeGroup = category;
	}

	public void UpdateTglState()
	{
		if (NKCContentManager.IsContentsUnlocked(GetContentsType(m_EpisodeGroup)))
		{
			m_tgl.UnLock();
		}
		else
		{
			m_tgl.Lock();
		}
	}

	private ContentsType GetContentsType(EPISODE_GROUP group)
	{
		return group switch
		{
			EPISODE_GROUP.EG_MAINSTREAM => ContentsType.EPISODE, 
			EPISODE_GROUP.EG_SUMMARY => ContentsType.OPERATION_SUMMARY, 
			EPISODE_GROUP.EG_SUBSTREAM => ContentsType.OPERATION_SUBSTREAM, 
			EPISODE_GROUP.EG_GROWTH => ContentsType.OPERATION_GROWTH, 
			EPISODE_GROUP.EG_CHALLENGE => ContentsType.OPERATION_CHALLENGE, 
			_ => ContentsType.None, 
		};
	}

	public void SetSelected(bool bValue)
	{
		m_tgl.Select(bValue, bForce: true, bImmediate: true);
		if (bValue)
		{
			m_Ani.SetTrigger("ON");
		}
		else
		{
			m_Ani.SetTrigger("OFF");
		}
	}

	public void ChangeSelected(bool bValue)
	{
		m_tgl.Select(bValue, bForce: true, bImmediate: true);
		if (bValue)
		{
			m_Ani.SetTrigger("OFF_TO_ON");
		}
		else
		{
			m_Ani.SetTrigger("ON_TO_OFF");
		}
	}

	public void OnTgl(bool bValue)
	{
		if (m_tgl.m_bLock)
		{
			NKCContentManager.ShowLockedMessagePopup(GetContentsType(m_EpisodeGroup));
		}
		else
		{
			m_dOnSlotSelect?.Invoke(m_EpisodeGroup, bShowFade: true);
		}
	}
}
