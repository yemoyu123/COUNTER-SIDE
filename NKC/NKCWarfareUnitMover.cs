using System.Collections;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCWarfareUnitMover : MonoBehaviour
{
	public delegate void OnCompleteMove(NKCWarfareGameUnit cNKCWarfareGameUnit);

	public NKCWarfareGameUnit m_NKCWarfareGameUnit;

	private OnCompleteMove m_OnCompleteMove;

	private Vector3 m_BeginPos;

	private Vector3 m_EndPos;

	private bool m_bRunning;

	private bool m_bPause;

	private bool m_bPlayMoveEndAni;

	private Coroutine m_MoveCoroutine;

	public const float WAIT_TIME_AFTER_LANDING = 0.5f;

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
		m_bPlayMoveEndAni = false;
		m_NKCWarfareGameUnit.SetState(NKCWarfareGameUnit.NKC_WARFARE_GAME_UNIT_STATE.NWGUS_MOVING);
	}

	public void Jump(Vector3 _EndPos, float _fTrackingTime, OnCompleteMove _OnCompleteMove = null)
	{
		_fTrackingTime -= 0.5f;
		if (_fTrackingTime <= 0f)
		{
			_fTrackingTime = 0f;
		}
		CommonProcess(_OnCompleteMove);
		if (IsRunning())
		{
			Stop();
		}
		m_MoveCoroutine = StartCoroutine(_Jump(_EndPos, _fTrackingTime));
	}

	public void Move(Vector3 _EndPos, float _fTrackingTime, OnCompleteMove _OnCompleteMove = null)
	{
		CommonProcess(_OnCompleteMove);
		if (IsRunning())
		{
			Stop();
		}
		m_NKCWarfareGameUnit.transform.SetAsLastSibling();
		m_MoveCoroutine = StartCoroutine(_Move(_EndPos, _fTrackingTime));
	}

	private IEnumerator _Move(Vector3 _EndPos, float _fTrackingTime)
	{
		m_bRunning = true;
		m_bPause = false;
		float fDeltaTime = 0f;
		m_EndPos = _EndPos;
		m_BeginPos = m_NKCWarfareGameUnit.transform.localPosition;
		yield return null;
		float num = Time.deltaTime;
		if (num > NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f)
		{
			num = NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f;
		}
		fDeltaTime += num;
		while (fDeltaTime < _fTrackingTime)
		{
			float progress = NKMTrackingFloat.TrackRatio(TRACKING_DATA_TYPE.TDT_SLOWER, fDeltaTime, _fTrackingTime);
			m_NKCWarfareGameUnit.transform.localPosition = NKCUtil.Lerp(m_BeginPos, m_EndPos, progress);
			yield return null;
			if (!m_bPause)
			{
				num = Time.deltaTime;
				if (num > NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f)
				{
					num = NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f;
				}
				fDeltaTime += num;
			}
			if (!m_bPlayMoveEndAni && fDeltaTime > _fTrackingTime * 0.7f)
			{
				m_bPlayMoveEndAni = true;
				m_NKCWarfareGameUnit.PlayClickAni();
			}
		}
		m_NKCWarfareGameUnit.gameObject.transform.localPosition = m_EndPos;
		m_NKCWarfareGameUnit.SetState(NKCWarfareGameUnit.NKC_WARFARE_GAME_UNIT_STATE.NWGUS_IDLE);
		if (m_OnCompleteMove != null)
		{
			m_OnCompleteMove(m_NKCWarfareGameUnit);
		}
		m_bRunning = false;
	}

	private IEnumerator _Jump(Vector3 _EndPos, float _fTrackingTime)
	{
		m_bRunning = true;
		m_bPause = false;
		float fDeltaTime = 0f;
		m_EndPos = _EndPos;
		m_BeginPos = m_NKCWarfareGameUnit.transform.localPosition;
		yield return null;
		float num = Time.deltaTime;
		if (num > NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f)
		{
			num = NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f;
		}
		fDeltaTime += num;
		while (fDeltaTime < _fTrackingTime)
		{
			float progress = NKMTrackingFloat.TrackRatio(TRACKING_DATA_TYPE.TDT_SLOWER, fDeltaTime, _fTrackingTime);
			float progress2 = NKMTrackingFloat.TrackRatio(TRACKING_DATA_TYPE.TDT_SLOWER, fDeltaTime, _fTrackingTime / 3f);
			float progress3 = NKMTrackingFloat.TrackRatio(TRACKING_DATA_TYPE.TDT_FASTER, fDeltaTime - _fTrackingTime / 3f, _fTrackingTime * 2f / 3f);
			m_NKCWarfareGameUnit.gameObject.transform.localPosition = NKCUtil.Lerp(m_BeginPos, m_EndPos, progress);
			if (fDeltaTime <= _fTrackingTime / 3f)
			{
				Vector3 localPosition = m_NKCWarfareGameUnit.gameObject.transform.localPosition;
				localPosition.z = NKCUtil.Lerp(0f, -125f, progress2);
				m_NKCWarfareGameUnit.gameObject.transform.localPosition = localPosition;
			}
			else
			{
				Vector3 localPosition2 = m_NKCWarfareGameUnit.gameObject.transform.localPosition;
				localPosition2.z = NKCUtil.Lerp(-125f, 0f, progress3);
				m_NKCWarfareGameUnit.gameObject.transform.localPosition = localPosition2;
			}
			yield return null;
			if (!m_bPause)
			{
				num = Time.deltaTime;
				if (num > NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f)
				{
					num = NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f;
				}
				fDeltaTime += num;
			}
		}
		m_NKCWarfareGameUnit.PlayClickAni();
		m_NKCWarfareGameUnit.gameObject.transform.localPosition = m_EndPos;
		while (fDeltaTime < _fTrackingTime + 0.5f)
		{
			yield return null;
			if (!m_bPause)
			{
				num = Time.deltaTime;
				if (num > NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f)
				{
					num = NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f;
				}
				fDeltaTime += num;
			}
		}
		m_NKCWarfareGameUnit.SetState(NKCWarfareGameUnit.NKC_WARFARE_GAME_UNIT_STATE.NWGUS_IDLE);
		if (m_OnCompleteMove != null)
		{
			m_OnCompleteMove(m_NKCWarfareGameUnit);
		}
		m_bRunning = false;
	}

	public void Stop()
	{
		if (m_MoveCoroutine != null)
		{
			StopCoroutine(m_MoveCoroutine);
		}
		m_MoveCoroutine = null;
		m_bRunning = false;
		m_bPlayMoveEndAni = false;
		m_bPause = false;
		m_NKCWarfareGameUnit.SetState(NKCWarfareGameUnit.NKC_WARFARE_GAME_UNIT_STATE.NWGUS_IDLE);
	}
}
