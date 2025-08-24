using System;
using UnityEngine;
using UnityEngine.Events;

public class WaitForSecondsWithCancel : CustomYieldInstruction
{
	public delegate bool CancelWait();

	private CancelWait dCancelWait;

	private float waitTime;

	private UnityAction dOnCancel;

	public override bool keepWaiting
	{
		get
		{
			if (dCancelWait())
			{
				Debug.Log("CancelWait");
				dOnCancel?.Invoke();
				return false;
			}
			return Time.realtimeSinceStartup < waitTime;
		}
	}

	public WaitForSecondsWithCancel(float time, CancelWait waitCondition, UnityAction onCancel)
	{
		if (waitCondition == null)
		{
			throw new Exception("waitCondition delegate null!");
		}
		waitTime = Time.realtimeSinceStartup + time;
		dCancelWait = waitCondition;
		dOnCancel = onCancel;
	}
}
