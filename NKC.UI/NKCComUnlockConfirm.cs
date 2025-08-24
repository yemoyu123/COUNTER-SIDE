using System;
using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC.UI;

public class NKCComUnlockConfirm : MonoBehaviour
{
	[Serializable]
	public struct UnlockCondition
	{
		public STAGE_UNLOCK_REQ_TYPE reqType;

		public int reqValue;

		public string reqValueStr;
	}

	public UnlockCondition[] m_unlockCondition;

	public bool m_applyAsDeactivator;

	public bool m_setTargetObject;

	public GameObject[] m_objActiveWhenConfirmed;

	public GameObject[] m_objActiveWhenDenied;

	public bool m_ignoreSuperUser;

	private void OnEnable()
	{
		bool flag = IsActivated();
		if (m_setTargetObject)
		{
			if (m_objActiveWhenConfirmed != null)
			{
				int num = m_objActiveWhenConfirmed.Length;
				for (int i = 0; i < num; i++)
				{
					NKCUtil.SetGameobjectActive(m_objActiveWhenConfirmed[i], flag);
				}
			}
			if (m_objActiveWhenDenied != null)
			{
				int num2 = m_objActiveWhenDenied.Length;
				for (int j = 0; j < num2; j++)
				{
					NKCUtil.SetGameobjectActive(m_objActiveWhenDenied[j], !flag);
				}
			}
		}
		else
		{
			base.gameObject.SetActive(flag);
		}
	}

	private bool IsActivated()
	{
		if (m_unlockCondition == null)
		{
			return base.gameObject.activeSelf;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return base.gameObject.activeSelf;
		}
		List<UnlockInfo> lstUnlockInfo = new List<UnlockInfo>();
		int num = m_unlockCondition.Length;
		for (int i = 0; i < num; i++)
		{
			if (UnlockInfo.IsDateTimeData(m_unlockCondition[i].reqType))
			{
				DateTime.TryParse(m_unlockCondition[i].reqValueStr, out var result);
				UnlockInfo item = new UnlockInfo(m_unlockCondition[i].reqType, m_unlockCondition[i].reqValue, result);
				lstUnlockInfo.Add(item);
			}
			else
			{
				UnlockInfo item2 = new UnlockInfo(m_unlockCondition[i].reqType, m_unlockCondition[i].reqValue, m_unlockCondition[i].reqValueStr);
				lstUnlockInfo.Add(item2);
			}
		}
		bool flag = NKMContentUnlockManager.IsContentUnlocked(nKMUserData, in lstUnlockInfo, m_ignoreSuperUser);
		if (!m_setTargetObject && m_applyAsDeactivator)
		{
			flag = !flag;
		}
		return flag;
	}
}
