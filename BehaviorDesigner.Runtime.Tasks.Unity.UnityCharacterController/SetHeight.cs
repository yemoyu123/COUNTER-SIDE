using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController;

[TaskCategory("Unity/CharacterController")]
[TaskDescription("Sets the height of the CharacterController. Returns Success.")]
public class SetHeight : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The height of the CharacterController")]
	public SharedFloat height;

	private CharacterController characterController;

	private GameObject prevGameObject;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			characterController = defaultGameObject.GetComponent<CharacterController>();
			prevGameObject = defaultGameObject;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (characterController == null)
		{
			Debug.LogWarning("CharacterController is null");
			return TaskStatus.Failure;
		}
		characterController.height = height.Value;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		height = 0f;
	}
}
