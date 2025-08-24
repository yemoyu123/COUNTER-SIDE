using UnityEngine;
using UnityEngine.Events;

namespace NKC.FX;

public class NKC_FXM_EVENT : NKC_FXM_EVALUATER
{
	public UnityEvent Evt;

	[Range(0f, 1f)]
	public float ProbFxEvent = 1f;

	private bool executed;

	private void OnDestroy()
	{
		if (Evt != null)
		{
			Evt = null;
		}
	}

	public override void Init()
	{
		if (!init && Evt != null)
		{
			Duration = 0f;
			executed = false;
			init = true;
		}
	}

	protected override void OnStart()
	{
		if (base.isActiveAndEnabled && !executed)
		{
			ExecuteEvent();
		}
	}

	protected override void OnComplete()
	{
		playbackTime = 0f;
		if (executed)
		{
			CancelInvoke();
			executed = false;
		}
	}

	public void ExecuteEvent()
	{
		HandleEvent(Evt, ProbFxEvent);
	}

	private void HandleEvent(UnityEvent _evt, float _prob)
	{
		float value = Random.value;
		if (_evt.GetPersistentEventCount() > 0 && _prob > value)
		{
			_evt.Invoke();
			executed = true;
		}
	}
}
