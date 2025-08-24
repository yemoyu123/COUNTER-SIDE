using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityLayerMask;

[TaskCategory("Unity/LayerMask")]
[TaskDescription("Gets the layer of a GameObject.")]
public class GetLayer : Action
{
	[Tooltip("The GameObject to set the layer of")]
	public SharedGameObject targetGameObject;

	[Tooltip("The name of the layer to get")]
	[RequiredField]
	public SharedString storeResult;

	public override TaskStatus OnUpdate()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		storeResult.Value = LayerMask.LayerToName(defaultGameObject.layer);
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		storeResult = "";
	}
}
