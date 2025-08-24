using System;
using System.Collections.Generic;
using System.Linq;
using NKM;
using UnityEngine;

namespace NKC.UI.Event;

public class NKCUIEventSubUIMissionTab : MonoBehaviour
{
	public NKCUIComToggle m_tglButton;

	public GameObject m_objOn;

	public GameObject m_objOnLock;

	public GameObject m_objOff;

	public GameObject m_objOffLock;

	public GameObject m_objReddot;

	public GameObject m_objOffComplete;

	private Action<int, bool> dOnSelectTab;

	private int m_tabID;

	private bool m_bLocked;

	private bool m_bCompleted;

	public bool Locked => m_bLocked;

	public bool Completed => m_bCompleted;

	internal void SetData(NKMMissionTabTemplet tabTemplet, NKCUIComToggleGroup toggleGroup, Action<int, bool> onSelectTab)
	{
		m_tglButton.OnValueChanged.RemoveAllListeners();
		m_tglButton.OnValueChanged.AddListener(OnSelected);
		m_tglButton.SetToggleGroup(toggleGroup);
		dOnSelectTab = onSelectTab;
		m_tabID = tabTemplet.m_tabID;
		m_tglButton.SetTitleText(tabTemplet.GetDesc());
		CheckTabState(tabTemplet, out m_bLocked, out m_bCompleted);
		UpdateReddot();
	}

	private void UpdateReddot()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		bool bValue = !m_bLocked && !m_bCompleted && nKMUserData.m_MissionData.CheckCompletableMission(nKMUserData, m_tabID);
		NKCUtil.SetGameobjectActive(m_objReddot, bValue);
	}

	private void CheckTabState(NKMMissionTabTemplet tabTemplet, out bool bLocked, out bool bCompleted)
	{
		bCompleted = false;
		bLocked = false;
		if (NKCScenManager.CurrentUserData() != null)
		{
			List<NKMMissionTemplet> missionTempletListByType = NKMMissionManager.GetMissionTempletListByType(tabTemplet.m_tabID);
			bCompleted = missionTempletListByType.All((NKMMissionTemplet x) => NKCScenManager.CurrentUserData().m_MissionData.IsMissionCompleted(x));
			if (!NKMMissionManager.CheckMissionTabUnlocked(tabTemplet.m_tabID, NKCScenManager.CurrentUserData()))
			{
				bLocked = true;
			}
			NKCUtil.SetGameobjectActive(m_objOn, !bLocked);
			NKCUtil.SetGameobjectActive(m_objOff, !bLocked);
			NKCUtil.SetGameobjectActive(m_objOnLock, bLocked);
			NKCUtil.SetGameobjectActive(m_objOffLock, bLocked);
			if (bLocked && !tabTemplet.m_VisibleWhenLocked)
			{
				m_tglButton.Lock();
			}
			else
			{
				m_tglButton.UnLock();
			}
			NKCUtil.SetGameobjectActive(m_objOffComplete, bCompleted);
		}
	}

	private void OnSelected(bool bChecked)
	{
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(m_tabID);
		if (m_bLocked)
		{
			string missionTabUnlockCondition = NKMMissionManager.GetMissionTabUnlockCondition(m_tabID, NKCScenManager.CurrentUserData());
			if (!string.IsNullOrEmpty(missionTabUnlockCondition))
			{
				NKCPopupMessageManager.AddPopupMessage(missionTabUnlockCondition);
			}
		}
		if (!m_bLocked || missionTabTemplet.m_VisibleWhenLocked)
		{
			dOnSelectTab?.Invoke(m_tabID, bChecked);
		}
	}
}
