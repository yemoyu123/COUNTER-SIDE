using System.Collections;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIResultSubUIWorldmap : NKCUIResultSubUIBase
{
	public Text m_lbText;

	private Transform m_trSD;

	private NKCASUIUnitIllust m_spineSD;

	private bool m_bInit;

	private bool m_bFinished;

	public override void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
		if (!m_bInit)
		{
			m_trSD = base.transform.Find("SD_CHAR_AREA");
			m_bInit = true;
		}
	}

	public void SetData(bool bBigSuccess, NKMUnitData leaderUnit)
	{
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_spineSD);
		NKCUtil.SetGameobjectActive(m_lbText, bValue: true);
		if (!bBigSuccess || leaderUnit == null)
		{
			base.ProcessRequired = false;
			m_spineSD = null;
			return;
		}
		Init();
		m_spineSD = NKCResourceUtility.OpenSpineSD(leaderUnit);
		if (m_spineSD == null)
		{
			base.ProcessRequired = false;
			return;
		}
		m_spineSD.SetParent(m_trSD, worldPositionStays: false);
		RectTransform rectTransform = m_spineSD.GetRectTransform();
		if (rectTransform != null)
		{
			rectTransform.localPosition = Vector2.zero;
			rectTransform.localScale = Vector3.one;
			rectTransform.localRotation = Quaternion.identity;
		}
		base.ProcessRequired = true;
	}

	public void SetData(bool bBigSuccess, int unitID, int skinID)
	{
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_spineSD);
		NKCUtil.SetGameobjectActive(m_lbText, bValue: false);
		if (!bBigSuccess || unitID <= 0)
		{
			base.ProcessRequired = false;
			m_spineSD = null;
			return;
		}
		Init();
		m_spineSD = NKCResourceUtility.OpenSpineSD(unitID, skinID);
		if (m_spineSD == null)
		{
			base.ProcessRequired = false;
			return;
		}
		m_spineSD.SetParent(m_trSD, worldPositionStays: false);
		RectTransform rectTransform = m_spineSD.GetRectTransform();
		if (rectTransform != null)
		{
			rectTransform.localPosition = Vector2.zero;
			rectTransform.localScale = Vector3.one;
			rectTransform.localRotation = Quaternion.identity;
		}
		base.ProcessRequired = true;
	}

	public override void FinishProcess()
	{
		if (base.gameObject.activeInHierarchy)
		{
			m_bFinished = true;
			StopAllCoroutines();
		}
	}

	public override bool IsProcessFinished()
	{
		return m_bFinished;
	}

	protected override IEnumerator InnerProcess(bool bAutoSkip)
	{
		m_bFinished = false;
		yield return null;
		m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_WIN, loop: true);
		yield return null;
		float aniTime = m_spineSD.GetAnimationTime(NKCASUIUnitIllust.eAnimation.SD_WIN);
		float deltaTime = 0f;
		while (deltaTime < aniTime)
		{
			deltaTime += Time.deltaTime;
			yield return null;
		}
		yield return null;
		FinishProcess();
	}
}
