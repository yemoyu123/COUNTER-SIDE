using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.Game;

public class NKCASUnitRespawnTimer : MonoBehaviour
{
	public Vector3 Offset = new Vector3(0f, -75f, 0f);

	public Image m_imgTimer;

	public Color m_Color;

	public void SetPosition(Vector3 pos)
	{
		base.transform.localPosition = pos + Offset;
	}

	public void Play(float time)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (m_imgTimer != null)
		{
			m_imgTimer.fillAmount = 0f;
			m_imgTimer.color = m_Color;
			m_imgTimer.DOFillAmount(1f, time).SetEase(Ease.Linear).OnComplete(Delay);
		}
	}

	private void Delay()
	{
		m_imgTimer.DOFillAmount(1f, 0.1f).OnComplete(Overtime);
	}

	private void Overtime()
	{
		m_imgTimer.color = Color.red;
		m_imgTimer.fillAmount = 0f;
		m_imgTimer.DOFillAmount(1f, 1f).SetEase(Ease.Linear);
	}

	public void Stop()
	{
		if (m_imgTimer != null)
		{
			m_imgTimer.DOKill();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
