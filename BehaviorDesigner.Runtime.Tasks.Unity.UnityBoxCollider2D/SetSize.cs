using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityBoxCollider2D;

[TaskCategory("Unity/BoxCollider2D")]
[TaskDescription("Sets the size of the BoxCollider2D. Returns Success.")]
public class SetSize : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The size of the BoxCollider2D")]
	public SharedVector2 size;

	private BoxCollider2D boxCollider2D;

	private GameObject prevGameObject;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			boxCollider2D = defaultGameObject.GetComponent<BoxCollider2D>();
			prevGameObject = defaultGameObject;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (boxCollider2D == null)
		{
			Debug.LogWarning("BoxCollider2D is null");
			return TaskStatus.Failure;
		}
		boxCollider2D.size = size.Value;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		size = Vector2.zero;
	}
}
