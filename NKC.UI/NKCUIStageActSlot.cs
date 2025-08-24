using System;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIStageActSlot : MonoBehaviour
{
	public delegate void OnSelectActSlot(int actID);

	[Header("폰트")]
	public Font m_fMain;

	public int m_MainSize;

	public Font m_fRajdhani;

	public int m_RajdhaniSize;

	[Space]
	public NKCUIComToggle m_tgl;

	public Text m_lbActNameOn;

	public Text m_lbActNameOff;

	public GameObject m_objNew;

	public GameObject m_objLocked;

	public GameObject m_objRemainTime;

	public Text m_lbRemainTime;

	private OnSelectActSlot m_dOnSelectActSlot;

	private int m_EpisodeID;

	private int m_ActID;

	private EPISODE_DIFFICULTY m_Difficulty;

	private int m_StageViewerID;

	private int m_SortIndex;

	private bool m_bIsLocked;

	private bool m_bUseLockEndTime;

	private DateTime m_lockEndTimeUtc;

	private float m_deltaTime;

	public int GetStageViewerID()
	{
		return m_StageViewerID;
	}

	public int GetSortIndex()
	{
		return m_SortIndex;
	}

	public bool IsLocked()
	{
		return m_bIsLocked;
	}

	public void SetData(int episodeID, EPISODE_DIFFICULTY difficulty, int actID, string actName, NKCUIComToggleGroup tglGroup, OnSelectActSlot onSelectActSlot)
	{
		m_tgl.OnValueChanged.RemoveAllListeners();
		m_tgl.OnValueChanged.AddListener(OnValueChanged);
		m_tgl.m_bGetCallbackWhileLocked = true;
		m_tgl.SetToggleGroup(tglGroup);
		m_EpisodeID = episodeID;
		m_Difficulty = difficulty;
		m_ActID = actID;
		m_dOnSelectActSlot = onSelectActSlot;
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(episodeID, difficulty);
		if (nKMEpisodeTempletV != null)
		{
			m_SortIndex = nKMEpisodeTempletV.m_SortIndex;
			if (nKMEpisodeTempletV.UseEpSlot())
			{
				m_StageViewerID = m_EpisodeID;
			}
			else
			{
				m_StageViewerID = m_ActID;
			}
			NKMStageTempletV2 firstStage = nKMEpisodeTempletV.GetFirstStage(nKMEpisodeTempletV.UseEpSlot() ? 1 : actID);
			if (firstStage != null)
			{
				if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in firstStage.m_UnlockInfo))
				{
					m_tgl.Lock();
					NKCUtil.SetGameobjectActive(m_objLocked, bValue: true);
					m_bIsLocked = true;
					if (!NKMContentUnlockManager.IsStarted(firstStage.m_UnlockInfo))
					{
						m_lockEndTimeUtc = NKMContentUnlockManager.GetConditionStartTime(firstStage.m_UnlockInfo);
						NKCUtil.SetGameobjectActive(m_objLocked, bValue: true);
						NKCUtil.SetGameobjectActive(m_objRemainTime, bValue: true);
						m_bIsLocked = true;
						m_bUseLockEndTime = true;
						SetLockText(m_lockEndTimeUtc);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_objLocked, bValue: true);
						NKCUtil.SetGameobjectActive(m_objRemainTime, bValue: false);
						m_bIsLocked = true;
						m_bUseLockEndTime = false;
					}
				}
				else
				{
					m_tgl.UnLock();
					NKCUtil.SetGameobjectActive(m_objLocked, bValue: false);
					NKCUtil.SetGameobjectActive(m_objRemainTime, bValue: false);
					m_bIsLocked = false;
				}
			}
			if (m_fRajdhani != null && m_fMain != null)
			{
				switch (nKMEpisodeTempletV.m_EPCategory)
				{
				default:
					m_lbActNameOn.font = m_fRajdhani;
					m_lbActNameOn.fontSize = m_RajdhaniSize;
					m_lbActNameOff.font = m_fRajdhani;
					m_lbActNameOff.fontSize = m_RajdhaniSize;
					break;
				case EPISODE_CATEGORY.EC_DAILY:
				case EPISODE_CATEGORY.EC_SUPPLY:
					m_lbActNameOn.font = m_fMain;
					m_lbActNameOn.fontSize = m_MainSize;
					m_lbActNameOff.font = m_fMain;
					m_lbActNameOff.fontSize = m_MainSize;
					break;
				}
			}
		}
		if (!string.IsNullOrEmpty(actName))
		{
			NKCUtil.SetLabelText(m_lbActNameOn, actName);
			NKCUtil.SetLabelText(m_lbActNameOff, actName);
		}
		RefreshReddot();
	}

	public void SetLockText(DateTime lockEndTimeUtc)
	{
		if (lockEndTimeUtc < NKCSynchronizedTime.GetServerUTCTime())
		{
			SetData(m_EpisodeID, m_Difficulty, m_ActID, "", m_tgl.m_ToggleGroup, m_dOnSelectActSlot);
			return;
		}
		if (NKCSynchronizedTime.GetTimeLeft(lockEndTimeUtc).TotalSeconds < 1.0)
		{
			NKCUtil.SetLabelText(m_lbRemainTime, NKCUtilString.GET_STRING_QUIT);
			return;
		}
		string remainTimeString = NKCUtilString.GetRemainTimeString(lockEndTimeUtc, 2);
		NKCUtil.SetLabelText(m_lbRemainTime, string.Format(NKCUtilString.GET_STRING_SHOP_CHAIN_NEXT_RESET_ONE_PARAM_CLOSE, remainTimeString));
	}

	public void ResetData()
	{
		m_EpisodeID = 0;
		m_Difficulty = EPISODE_DIFFICULTY.NORMAL;
		m_ActID = 0;
		m_StageViewerID = 0;
		m_SortIndex = 0;
	}

	public void SetSelected(bool bValue)
	{
		m_tgl.Select(bValue, bForce: true, bImmediate: true);
	}

	private void OnValueChanged(bool bValue)
	{
		if (m_tgl.m_bLock)
		{
			NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(m_EpisodeID, m_Difficulty);
			if (nKMEpisodeTempletV != null)
			{
				if (nKMEpisodeTempletV.m_DicStage.ContainsKey(m_ActID))
				{
					NKMStageTempletV2 firstStage = nKMEpisodeTempletV.GetFirstStage(m_ActID);
					if (firstStage != null)
					{
						string unlockConditionRequireDesc = NKCUtilString.GetUnlockConditionRequireDesc(firstStage);
						NKCUIManager.NKCPopupMessage.Open(new PopupMessage(unlockConditionRequireDesc, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
					}
				}
				return;
			}
		}
		if (bValue)
		{
			m_dOnSelectActSlot?.Invoke(m_ActID);
		}
	}

	public void RefreshReddot()
	{
		NKCUtil.SetGameobjectActive(m_objNew, NKMEpisodeMgr.HasReddot(m_EpisodeID, m_Difficulty, m_ActID));
	}

	private void Update()
	{
		if (m_bUseLockEndTime)
		{
			m_deltaTime += Time.deltaTime;
			if (m_deltaTime > 1f)
			{
				m_deltaTime = 0f;
				SetLockText(m_lockEndTimeUtc);
			}
		}
	}
}
