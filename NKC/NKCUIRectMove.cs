using System;
using System.Collections;
using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC;

[RequireComponent(typeof(RectTransform))]
public class NKCUIRectMove : MonoBehaviour
{
	[Serializable]
	public class MoveInfo
	{
		public string Name;

		public bool bNoApplyAnchoredPosition;

		public Vector2 AnchoredPosition;

		public Vector2 SizeDelta;

		public Vector2 anchorMin;

		public Vector2 anchorMax;

		public Vector2 Pivot;

		public Vector3 Scale;

		public float TrackTime;

		public TRACKING_DATA_TYPE TrackType;
	}

	public delegate void OnTrackingComplete();

	public List<MoveInfo> m_lstMoveInfo;

	private RectTransform m_RectTransform;

	public NKCUIComSafeArea m_comSafeArea;

	private bool m_bRunning;

	private Coroutine TransitCoroutine;

	private RectTransform RectTransform
	{
		get
		{
			if (m_RectTransform == null)
			{
				m_RectTransform = GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	public bool IsRunning()
	{
		return m_bRunning;
	}

	private void Awake()
	{
		m_RectTransform = GetComponent<RectTransform>();
	}

	public void CopyFromRect(int index)
	{
		MoveInfo moveInfo = m_lstMoveInfo[index];
		MoveInfo value = new MoveInfo
		{
			AnchoredPosition = RectTransform.anchoredPosition,
			Pivot = RectTransform.pivot,
			SizeDelta = RectTransform.sizeDelta,
			anchorMax = RectTransform.anchorMax,
			anchorMin = RectTransform.anchorMin,
			Scale = RectTransform.localScale,
			Name = moveInfo.Name,
			TrackTime = moveInfo.TrackTime,
			TrackType = moveInfo.TrackType
		};
		m_lstMoveInfo[index] = value;
	}

	public void CopyFromRect()
	{
		MoveInfo item = new MoveInfo
		{
			AnchoredPosition = RectTransform.anchoredPosition,
			Pivot = RectTransform.pivot,
			SizeDelta = RectTransform.sizeDelta,
			anchorMax = RectTransform.anchorMax,
			anchorMin = RectTransform.anchorMin,
			Scale = RectTransform.localScale,
			Name = "New Moveinfo",
			TrackTime = 0.4f,
			TrackType = TRACKING_DATA_TYPE.TDT_SLOWER
		};
		if (m_lstMoveInfo == null)
		{
			m_lstMoveInfo = new List<MoveInfo>();
		}
		m_lstMoveInfo.Add(item);
	}

	public void StopTracking()
	{
		if (TransitCoroutine != null)
		{
			StopCoroutine(TransitCoroutine);
		}
		TransitCoroutine = null;
		m_bRunning = false;
	}

	public MoveInfo GetMoveInfo(string name)
	{
		return m_lstMoveInfo.Find((MoveInfo x) => x.Name == name);
	}

	public void AddMoveInfo(MoveInfo info)
	{
		if (m_lstMoveInfo.Exists((MoveInfo x) => x.Name == info.Name))
		{
			Debug.LogError("Moveinfo of same name exists!");
		}
		else
		{
			m_lstMoveInfo.Add(info);
		}
	}

	public void Set(int index)
	{
		if (index < m_lstMoveInfo.Count)
		{
			MoveInfo info = m_lstMoveInfo[index];
			Set(info);
		}
	}

	public void Set(string name)
	{
		MoveInfo info = m_lstMoveInfo.Find((MoveInfo x) => x.Name == name);
		Set(info);
	}

	public void Set(string startName, string endName, float fPercent)
	{
		MoveInfo moveInfo = m_lstMoveInfo.Find((MoveInfo x) => x.Name == startName);
		MoveInfo moveInfo2 = m_lstMoveInfo.Find((MoveInfo x) => x.Name == endName);
		if (moveInfo != null && moveInfo2 != null)
		{
			RectTransform.anchoredPosition = moveInfo.AnchoredPosition + (moveInfo2.AnchoredPosition - moveInfo.AnchoredPosition) * fPercent;
			RectTransform.localScale = moveInfo.Scale * (1f - fPercent) + moveInfo2.Scale * fPercent;
		}
	}

	private void Set(MoveInfo info)
	{
		if (info == null)
		{
			return;
		}
		StopTracking();
		if (!info.bNoApplyAnchoredPosition)
		{
			Vector2 vector = info.AnchoredPosition;
			if (m_comSafeArea != null)
			{
				vector = m_comSafeArea.GetSafeAreaPos(vector);
				m_comSafeArea.SetInit();
			}
			RectTransform.anchoredPosition = vector;
		}
		Vector3 localScale = info.Scale;
		if (m_comSafeArea != null && m_comSafeArea.m_bUseScale)
		{
			localScale = m_comSafeArea.GetSafeAreaScale();
			m_comSafeArea.SetInit();
		}
		RectTransform.localScale = localScale;
		RectTransform.anchorMax = info.anchorMax;
		RectTransform.anchorMin = info.anchorMin;
		RectTransform.pivot = info.Pivot;
		Vector2 sizeDelta = info.SizeDelta;
		if (m_comSafeArea != null && m_comSafeArea.m_bUseRectSide)
		{
			float safeAreaWidth = m_comSafeArea.GetSafeAreaWidth(sizeDelta.x);
			sizeDelta.x = safeAreaWidth;
			m_comSafeArea.SetInit();
		}
		if (m_comSafeArea != null && m_comSafeArea.m_bUseRectHeight)
		{
			float safeAreaHeight = m_comSafeArea.GetSafeAreaHeight(sizeDelta.y);
			sizeDelta.y = safeAreaHeight;
			m_comSafeArea.SetInit();
		}
		RectTransform.sizeDelta = sizeDelta;
	}

	public void Move(string Name, bool bAnimate)
	{
		if (bAnimate)
		{
			Transit(Name);
		}
		else
		{
			Set(Name);
		}
	}

	public void Transit(string name, OnTrackingComplete onComplete = null)
	{
		MoveInfo info = m_lstMoveInfo.Find((MoveInfo x) => x.Name == name);
		Transit(info, onComplete);
	}

	public void Transit(int index, OnTrackingComplete onComplete = null)
	{
		if (index >= 0 && index < m_lstMoveInfo.Count)
		{
			Transit(m_lstMoveInfo[index], onComplete);
		}
	}

	private void Transit(MoveInfo info, OnTrackingComplete onComplete)
	{
		if (info == null)
		{
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			Set(info);
			return;
		}
		StopTracking();
		if (info.TrackTime == 0f)
		{
			Set(info);
			onComplete?.Invoke();
		}
		else
		{
			TransitCoroutine = StartCoroutine(_Transit(info, onComplete));
		}
	}

	private IEnumerator _Transit(MoveInfo info, OnTrackingComplete onComplete)
	{
		m_bRunning = true;
		Vector2 PositionBegin = RectTransform.anchoredPosition;
		Vector2 vector = info.AnchoredPosition;
		if (m_comSafeArea != null)
		{
			vector = m_comSafeArea.GetSafeAreaPos(vector);
			m_comSafeArea.SetInit();
		}
		Vector2 PositionEnd = vector;
		Vector3 scaleBegin = RectTransform.localScale;
		Vector3 vector2 = info.Scale;
		if (m_comSafeArea != null && m_comSafeArea.m_bUseScale)
		{
			vector2 = m_comSafeArea.GetSafeAreaScale();
			m_comSafeArea.SetInit();
		}
		Vector3 scaleEnd = vector2;
		Vector2 anchorMaxBegin = RectTransform.anchorMax;
		Vector2 anchorMaxEnd = info.anchorMax;
		Vector2 anchorMinBegin = RectTransform.anchorMin;
		Vector2 anchorMinEnd = info.anchorMin;
		Vector2 pivotBegin = RectTransform.pivot;
		Vector2 pivotEnd = info.Pivot;
		Vector2 sizeDelta = RectTransform.sizeDelta;
		if (m_comSafeArea != null && m_comSafeArea.m_bUseRectSide)
		{
			float safeAreaWidth = m_comSafeArea.GetSafeAreaWidth(sizeDelta.x);
			sizeDelta.x = safeAreaWidth;
			m_comSafeArea.SetInit();
		}
		if (m_comSafeArea != null && m_comSafeArea.m_bUseRectHeight)
		{
			float safeAreaHeight = m_comSafeArea.GetSafeAreaHeight(sizeDelta.y);
			sizeDelta.y = safeAreaHeight;
			m_comSafeArea.SetInit();
		}
		Vector2 sizeDeltaBegin = sizeDelta;
		sizeDelta = info.SizeDelta;
		if (m_comSafeArea != null && m_comSafeArea.m_bUseRectSide)
		{
			float safeAreaWidth2 = m_comSafeArea.GetSafeAreaWidth(sizeDelta.x);
			sizeDelta.x = safeAreaWidth2;
			m_comSafeArea.SetInit();
		}
		if (m_comSafeArea != null && m_comSafeArea.m_bUseRectHeight)
		{
			float safeAreaHeight2 = m_comSafeArea.GetSafeAreaHeight(sizeDelta.y);
			sizeDelta.y = safeAreaHeight2;
			m_comSafeArea.SetInit();
		}
		Vector2 sizeDeltaEnd = sizeDelta;
		float fDeltaTime = 0f;
		yield return null;
		for (fDeltaTime += Time.deltaTime; fDeltaTime < info.TrackTime; fDeltaTime += Time.deltaTime)
		{
			float progress = NKMTrackingFloat.TrackRatio(info.TrackType, fDeltaTime, info.TrackTime);
			if (!info.bNoApplyAnchoredPosition)
			{
				RectTransform.anchoredPosition = NKCUtil.Lerp(PositionBegin, PositionEnd, progress);
			}
			RectTransform.localScale = NKCUtil.Lerp(scaleBegin, scaleEnd, progress);
			RectTransform.anchorMax = NKCUtil.Lerp(anchorMaxBegin, anchorMaxEnd, progress);
			RectTransform.anchorMin = NKCUtil.Lerp(anchorMinBegin, anchorMinEnd, progress);
			RectTransform.pivot = NKCUtil.Lerp(pivotBegin, pivotEnd, progress);
			RectTransform.sizeDelta = NKCUtil.Lerp(sizeDeltaBegin, sizeDeltaEnd, progress);
			yield return null;
		}
		if (!info.bNoApplyAnchoredPosition)
		{
			RectTransform.anchoredPosition = PositionEnd;
		}
		RectTransform.localScale = scaleEnd;
		RectTransform.anchorMax = anchorMaxEnd;
		RectTransform.anchorMin = anchorMinEnd;
		RectTransform.pivot = pivotEnd;
		RectTransform.sizeDelta = sizeDeltaEnd;
		onComplete?.Invoke();
		m_bRunning = false;
	}
}
