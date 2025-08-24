using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class NKCUIRectMoveSet : MonoBehaviour
{
	[Serializable]
	public class MoveEvent
	{
		public NKCUIRectMove targetRectMove;

		public string Name;

		public float StartTime;

		public bool bAnimate = true;
	}

	[Serializable]
	public class MoveSet
	{
		public string Name;

		public List<MoveEvent> m_lstMoveEvent;
	}

	public List<MoveSet> m_lstMoveSet;

	public void PlayMoveSet(string Name, bool bAnimate = true, NKCUIRectMove.OnTrackingComplete onComplete = null)
	{
		if (m_lstMoveSet != null)
		{
			MoveSet moveset = m_lstMoveSet.Find((MoveSet x) => x.Name == Name);
			PlayMoveSet(moveset, bAnimate, onComplete);
		}
	}

	private void PlayMoveSet(MoveSet moveset, bool bAnimate, NKCUIRectMove.OnTrackingComplete onComplete)
	{
		if (moveset == null)
		{
			return;
		}
		StopAllCoroutines();
		if (bAnimate && base.gameObject.activeInHierarchy)
		{
			StartCoroutine(_PlayMoveSet(moveset, onComplete));
			return;
		}
		foreach (MoveEvent item in moveset.m_lstMoveEvent)
		{
			if (item.targetRectMove != null)
			{
				item.targetRectMove.Set(item.Name);
			}
		}
		onComplete?.Invoke();
	}

	private IEnumerator _PlayMoveSet(MoveSet moveset, NKCUIRectMove.OnTrackingComplete onComplete)
	{
		if (moveset.m_lstMoveEvent.Count < 0)
		{
			yield break;
		}
		float ProcessedTime = 0f;
		moveset.m_lstMoveEvent.Sort((MoveEvent a, MoveEvent b) => a.StartTime.CompareTo(b.StartTime));
		int currentIndex = 0;
		int lastMoveIndex = 0;
		float moveEventFinishTime = GetMoveEventFinishTime(moveset.m_lstMoveEvent[0]);
		for (int num = 1; num < moveset.m_lstMoveEvent.Count; num++)
		{
			MoveEvent move = moveset.m_lstMoveEvent[num];
			if (GetMoveEventFinishTime(move) > moveEventFinishTime)
			{
				lastMoveIndex = num;
			}
		}
		while (currentIndex < moveset.m_lstMoveEvent.Count)
		{
			MoveEvent moveEvent = moveset.m_lstMoveEvent[currentIndex];
			if (moveEvent.StartTime <= ProcessedTime)
			{
				if (moveEvent.bAnimate)
				{
					if (moveEvent.targetRectMove != null)
					{
						if (lastMoveIndex == currentIndex)
						{
							moveEvent.targetRectMove.Transit(moveEvent.Name, onComplete);
						}
						else
						{
							moveEvent.targetRectMove.Transit(moveEvent.Name);
						}
					}
				}
				else
				{
					if (moveEvent.targetRectMove != null)
					{
						moveEvent.targetRectMove.Set(moveEvent.Name);
					}
					if (lastMoveIndex == currentIndex)
					{
						onComplete?.Invoke();
					}
				}
				currentIndex++;
			}
			else
			{
				yield return null;
				ProcessedTime += Time.deltaTime;
			}
		}
		yield return null;
	}

	private float GetMoveEventFinishTime(MoveEvent move)
	{
		if (move.bAnimate && move.targetRectMove != null)
		{
			NKCUIRectMove.MoveInfo moveInfo = move.targetRectMove.GetMoveInfo(move.Name);
			return move.StartTime + moveInfo.TrackTime;
		}
		return move.StartTime;
	}
}
