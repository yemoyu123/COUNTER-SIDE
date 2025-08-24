using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector3;

[TaskCategory("Unity/Vector3")]
[TaskDescription("Rotate the current rotation to the target rotation.")]
public class RotateTowards : Action
{
	[Tooltip("The current rotation in euler angles")]
	public SharedVector3 currentRotation;

	[Tooltip("The target rotation in euler angles")]
	public SharedVector3 targetRotation;

	[Tooltip("The maximum delta of the degrees")]
	public SharedFloat maxDegreesDelta;

	[Tooltip("The maximum delta of the magnitude")]
	public SharedFloat maxMagnitudeDelta;

	[Tooltip("The rotation resut")]
	[RequiredField]
	public SharedVector3 storeResult;

	public override TaskStatus OnUpdate()
	{
		storeResult.Value = Vector3.RotateTowards(currentRotation.Value, targetRotation.Value, maxDegreesDelta.Value * ((float)System.Math.PI / 180f) * Time.deltaTime, maxMagnitudeDelta.Value);
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		currentRotation = Vector3.zero;
		targetRotation = Vector3.zero;
		storeResult = Vector3.zero;
		maxDegreesDelta = 0f;
		maxMagnitudeDelta = 0f;
	}
}
