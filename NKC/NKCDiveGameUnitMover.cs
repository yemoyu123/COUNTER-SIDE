using System.Collections;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCDiveGameUnitMover : MonoBehaviour
{
	public delegate void OnCompleteMove();

	private OnCompleteMove m_OnCompleteMove;

	private Vector3 m_BeginPos;

	private Vector3 m_EndPos;

	private bool m_bRunning;

	private bool m_bPause;

	private Coroutine m_MoveCoroutine;

	private void SetMoveCoroutine(Coroutine _Coroutine)
	{
		m_MoveCoroutine = _Coroutine;
	}

	private Coroutine GetMoveCoroutine()
	{
		return m_MoveCoroutine;
	}

	public void SetPause(bool bSet)
	{
		m_bPause = bSet;
	}

	public bool IsRunning()
	{
		return m_bRunning;
	}

	private void CommonProcess(OnCompleteMove _OnCompleteMove)
	{
		m_OnCompleteMove = _OnCompleteMove;
	}

	public void Move(Vector3 _EndPos, float _fTrackingTime, OnCompleteMove _OnCompleteMove = null)
	{
		CommonProcess(_OnCompleteMove);
		if (IsRunning())
		{
			Stop();
		}
		m_MoveCoroutine = StartCoroutine(_Move(_EndPos, _fTrackingTime));
	}

	private IEnumerator _Move(Vector3 _EndPos, float _fTrackingTime)
	{
		m_bRunning = true;
		m_bPause = false;
		float fDeltaTime = 0f;
		m_EndPos = _EndPos;
		m_BeginPos = base.transform.localPosition;
		yield return null;
		fDeltaTime += Time.deltaTime;
		while (fDeltaTime < _fTrackingTime)
		{
			float progress = NKMTrackingFloat.TrackRatio(TRACKING_DATA_TYPE.TDT_SLOWER, fDeltaTime, _fTrackingTime);
			base.transform.localPosition = NKCUtil.Lerp(m_BeginPos, m_EndPos, progress);
			yield return null;
			if (!m_bPause)
			{
				fDeltaTime += Time.deltaTime;
			}
		}
		base.gameObject.transform.localPosition = m_EndPos;
		m_bRunning = false;
		if (m_OnCompleteMove != null)
		{
			m_OnCompleteMove();
		}
	}

	public void Stop()
	{
		if (m_MoveCoroutine != null)
		{
			StopCoroutine(m_MoveCoroutine);
		}
		m_MoveCoroutine = null;
		m_bRunning = false;
		m_bPause = false;
	}
}
