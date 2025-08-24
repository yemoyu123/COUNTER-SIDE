using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;

[TaskCategory("Unity/GameObject")]
[TaskDescription("Finds a GameObject by tag. Returns success if an object is found.")]
public class FindWithTag : Action
{
	[Tooltip("The tag of the GameObject to find")]
	public SharedString tag;

	[Tooltip("Should a random GameObject be found?")]
	public SharedBool random;

	[Tooltip("The object found by name")]
	[RequiredField]
	public SharedGameObject storeValue;

	public override TaskStatus OnUpdate()
	{
		if (random.Value)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(tag.Value);
			if (array == null || array.Length == 0)
			{
				return TaskStatus.Failure;
			}
			storeValue.Value = array[Random.Range(0, array.Length)];
		}
		else
		{
			storeValue.Value = GameObject.FindWithTag(tag.Value);
		}
		if (!(storeValue.Value != null))
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		tag.Value = null;
		storeValue.Value = null;
	}
}
