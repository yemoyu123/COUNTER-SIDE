using System;
using System.Collections;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIRechargeEternium : MonoBehaviour
{
	public GameObject m_ChargeEternium;

	public Text m_ChargeTimeText;

	public Text m_ChargeEterniumText;

	public GameObject m_EterniumGauge;

	public GameObject m_EterniumFullGlow;

	public Image m_imgEterniumGauge;

	public Text m_lbEterniumCap;

	private long m_lCurMaxEternium;

	private DateTime m_nextUpdateTime = DateTime.MinValue;

	public void UpdateData(NKMUserData userData)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.RECHARGE_FUND) || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		NKMUserExpTemplet userExpTemplet = NKCExpManager.GetUserExpTemplet(userData);
		if (userExpTemplet == null)
		{
			Debug.LogError($"자원정보를 얻어오지 못했습니다.{userData.m_UserLevel}");
			return;
		}
		m_lCurMaxEternium = userExpTemplet.m_EterniumCap;
		if (IsMaxEterniumCap())
		{
			NKCUtil.SetLabelText(m_ChargeTimeText, "MAX");
			StopCoroutine(OnStartRechargeCount());
		}
		else
		{
			m_nextUpdateTime = NKCScenManager.CurrentUserData().lastEterniumUpdateDate + NKMCommonConst.RECHARGE_TIME;
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(OnStartRechargeCount());
			}
		}
		NKCUtil.SetLabelText(m_ChargeEterniumText, "+" + userExpTemplet.m_RechargeEternium);
		NKCUtil.SetLabelText(m_lbEterniumCap, $"/{m_lCurMaxEternium}");
		UpdateEterniumUI(userData);
	}

	private bool IsMaxEterniumCap()
	{
		if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(2) < m_lCurMaxEternium)
		{
			return NKCScenManager.CurrentUserData().lastEterniumUpdateDate == DateTime.MinValue;
		}
		return true;
	}

	private void UpdateEterniumUI(NKMUserData userData)
	{
		if (m_imgEterniumGauge != null)
		{
			float eterniumCapProgress = userData.GetEterniumCapProgress();
			if (base.gameObject.activeInHierarchy)
			{
				m_imgEterniumGauge.DOFillAmount(eterniumCapProgress, 0.3f).SetEase(Ease.OutCubic);
			}
			else
			{
				m_imgEterniumGauge.fillAmount = eterniumCapProgress;
			}
			NKCUtil.SetGameobjectActive(m_EterniumFullGlow, eterniumCapProgress >= 1f);
		}
	}

	private IEnumerator OnStartRechargeCount()
	{
		while (!IsMaxEterniumCap())
		{
			TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(m_nextUpdateTime);
			NKCUtil.SetLabelText(m_ChargeTimeText, $"{timeLeft.Minutes:00}:{timeLeft.Seconds:00}");
			yield return new WaitForSeconds(1f);
		}
		yield return null;
	}
}
