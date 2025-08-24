using System;
using UnityEngine;

namespace NKC.FX;

[Serializable]
[ExecuteAlways]
public class NKC_FXM_EVALUATER : MonoBehaviour
{
	[HideInInspector]
	public float StartTime;

	[HideInInspector]
	public float Duration = 1f;

	[HideInInspector]
	public bool RandomValue;

	[HideInInspector]
	public bool Resimulate;

	[HideInInspector]
	public ResetAction ResetMode;

	private bool isStarted;

	private bool isCompleted;

	protected float playbackTime;

	protected float deltaTime;

	protected bool init;

	private System.Random _random = new System.Random();

	public bool IsStarted
	{
		get
		{
			return isStarted;
		}
		set
		{
			isStarted = value;
		}
	}

	public bool IsCompleted
	{
		get
		{
			return isCompleted;
		}
		set
		{
			isCompleted = value;
		}
	}

	private void OnEnable()
	{
		isStarted = false;
		playbackTime = 0f;
	}

	public virtual void Init()
	{
	}

	public void UpdateInternal(float _playbackTime, float _deltaTime = 0f)
	{
		playbackTime = Mathf.Clamp(_playbackTime - StartTime, 0f, Duration);
		deltaTime = _deltaTime;
		if (init && base.enabled)
		{
			if (!isStarted)
			{
				isStarted = true;
				OnStart();
			}
			OnExecute();
			if (Duration <= playbackTime)
			{
				isCompleted = true;
				OnComplete();
			}
		}
	}

	public void ResetEvaluater()
	{
		if (init)
		{
			isStarted = false;
			isCompleted = false;
			switch (ResetMode)
			{
			case ResetAction.SyncSelf:
				playbackTime = 0f;
				OnExecute(_render: false);
				break;
			case ResetAction.SyncPlayer:
				playbackTime = 0f;
				OnExecute(_render: false);
				break;
			case ResetAction.NoReset:
				break;
			}
		}
	}

	public virtual void SetRandomValue(bool _resimulate)
	{
	}

	protected virtual void OnStart()
	{
	}

	protected virtual void OnExecute(bool _render = true)
	{
	}

	protected virtual void OnComplete()
	{
		if (ResetMode == ResetAction.SyncSelf)
		{
			playbackTime = 0f;
			OnExecute(_render: false);
		}
	}

	public float GetPlaybackTime()
	{
		return playbackTime;
	}

	protected AnimationCurve InitCurve(float _start, float _end)
	{
		Keyframe[] array = new Keyframe[2];
		array[0].time = 0f;
		array[0].value = _start;
		array[1].time = 1f;
		array[1].value = _end;
		return new AnimationCurve(array);
	}

	protected AnimationCurve InitCurveLinear(float _start, float _end)
	{
		return AnimationCurve.Linear(0f, _start, 1f, _end);
	}

	protected float Evaluate(AnimationCurve _value)
	{
		if (Mathf.Approximately(0f, Duration))
		{
			return _value.Evaluate(0f);
		}
		return _value.Evaluate(playbackTime / Duration);
	}

	protected Color Evaluate(Gradient _value)
	{
		if (Mathf.Approximately(0f, Duration))
		{
			return _value.Evaluate(0f);
		}
		return _value.Evaluate(playbackTime / Duration);
	}

	protected void Shuffle(int[] array)
	{
		for (int num = array.Length - 1; num > 0; num--)
		{
			int num2 = _random.Next(0, num);
			int num3 = array[num2];
			array[num2] = array[num];
			array[num] = num3;
		}
	}

	protected void Shuffle(Sprite[] array)
	{
		for (int num = array.Length - 1; num > 0; num--)
		{
			int num2 = _random.Next(0, num);
			Sprite sprite = array[num2];
			array[num2] = array[num];
			array[num] = sprite;
		}
	}

	protected void Shuffle(Mesh[] array)
	{
		for (int num = array.Length - 1; num > 0; num--)
		{
			int num2 = _random.Next(0, num);
			Mesh mesh = array[num2];
			array[num2] = array[num];
			array[num] = mesh;
		}
	}
}
