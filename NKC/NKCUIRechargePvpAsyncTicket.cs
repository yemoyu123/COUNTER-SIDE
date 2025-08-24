using System;
using System.Collections;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIRechargePvpAsyncTicket : MonoBehaviour
{
	public GameObject m_ChargeTicket;

	public Text m_ChargeTimeText;

	public Text m_lbChargeTicketCount;

	public Text m_lbTicketCap;

	private long m_lCurMaxTicketCount;

	private DateTime m_nextUpdateTime = DateTime.MinValue;

	public void UpdateData()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		m_lCurMaxTicketCount = NKMPvpCommonConst.Instance.AsyncTicketMaxCount;
		if (IsMaxTicketCap())
		{
			NKCUtil.SetGameobjectActive(m_lbChargeTicketCount, bValue: false);
			NKCUtil.SetLabelText(m_ChargeTimeText, "MAX");
			StopCoroutine(OnStartRechargeCount());
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbChargeTicketCount, bValue: true);
			NKCUtil.SetLabelText(m_lbChargeTicketCount, $"+{NKMPvpCommonConst.Instance.AsyncTicketChargeCount}");
			m_nextUpdateTime = NKCScenManager.CurrentUserData().lastAsyncTicketUpdateDate.AddSeconds(NKMPvpCommonConst.Instance.AsyncTicketChargeInterval);
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(OnStartRechargeCount());
			}
		}
		NKCUtil.SetLabelText(m_lbTicketCap, $"/{m_lCurMaxTicketCount}");
	}

	private bool IsMaxTicketCap()
	{
		if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(13) < m_lCurMaxTicketCount)
		{
			return NKCScenManager.CurrentUserData().lastAsyncTicketUpdateDate == DateTime.MinValue;
		}
		return true;
	}

	private IEnumerator OnStartRechargeCount()
	{
		while (!IsMaxTicketCap())
		{
			TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(NKCSynchronizedTime.ToUtcTime(m_nextUpdateTime));
			NKCUtil.SetLabelText(m_ChargeTimeText, $"{timeLeft.Hours:00}:{timeLeft.Minutes:00}:{timeLeft.Seconds:00}");
			yield return new WaitForSeconds(1f);
		}
		yield return null;
	}
}
