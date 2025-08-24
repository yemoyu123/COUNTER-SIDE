using System;
using UnityEngine;

namespace NKC.FX;

[Serializable]
public class NKC_FX_EVENT_RANDOM_EXECUTER : MonoBehaviour
{
	[Serializable]
	public class EventProbability
	{
		public bool Enable;

		public float Probability;

		public NKC_FX_EVENT Event;
	}

	public bool AutoStart;

	public bool AutoDisable;

	public EventProbability[] EventProbs;

	private NKC_FX_EVENT target;

	private bool executed;

	private void OnDestroy()
	{
		if (EventProbs != null)
		{
			EventProbs = null;
		}
		if (target != null)
		{
			target = null;
		}
	}

	private void Awake()
	{
		executed = false;
	}

	private void Update()
	{
		if (!executed && AutoStart)
		{
			Execute();
		}
	}

	public void Execute()
	{
		if (EventProbs != null && EventProbs.Length != 0)
		{
			target = GetRandomData(EventProbs);
			if (target != null)
			{
				target.Execute();
				executed = true;
			}
			else
			{
				Debug.LogWarning("Target Event is Null.", base.gameObject);
			}
		}
	}

	private void LateUpdate()
	{
		if (executed && AutoDisable)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		executed = false;
	}

	private NKC_FX_EVENT GetRandomData(EventProbability[] _randomGroup)
	{
		float[] array = new float[_randomGroup.Length];
		for (int i = 0; i < _randomGroup.Length; i++)
		{
			if (_randomGroup[i].Enable && _randomGroup[i].Probability > 0f)
			{
				array[i] = _randomGroup[i].Probability;
			}
			else
			{
				array[i] = 0f;
			}
		}
		return _randomGroup[Choose(array)].Event;
	}

	public float GetTotalProbability()
	{
		float[] array = new float[EventProbs.Length];
		for (int i = 0; i < EventProbs.Length; i++)
		{
			if (EventProbs[i].Enable && EventProbs[i].Probability > 0f)
			{
				array[i] = EventProbs[i].Probability;
			}
			else
			{
				array[i] = 0f;
			}
		}
		float num = 0f;
		float[] array2 = array;
		foreach (float num2 in array2)
		{
			num += num2;
		}
		return num;
	}

	private int Choose(float[] _probs)
	{
		float num = 0f;
		foreach (float num2 in _probs)
		{
			num += num2;
		}
		float num3 = UnityEngine.Random.value * num;
		for (int j = 0; j < _probs.Length; j++)
		{
			if (num3 < _probs[j])
			{
				return j;
			}
			num3 -= _probs[j];
		}
		return _probs.Length - 1;
	}
}
