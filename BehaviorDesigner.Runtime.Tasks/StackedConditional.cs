using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Allows multiple conditional tasks to be added to a single node.")]
[TaskIcon("{SkinColor}StackedConditionalIcon.png")]
public class StackedConditional : Conditional
{
	public enum ComparisonType
	{
		Sequence,
		Selector
	}

	[InspectTask]
	public Conditional[] conditionals;

	[Tooltip("Specifies if the tasks should be traversed with an AND (Sequence) or an OR (Selector).")]
	public ComparisonType comparisonType;

	[Tooltip("Should the tasks be labeled within the graph?")]
	public bool graphLabel;

	public override void OnAwake()
	{
		if (conditionals == null)
		{
			return;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				conditionals[i].GameObject = gameObject;
				conditionals[i].Transform = transform;
				conditionals[i].Owner = base.Owner;
				conditionals[i].OnAwake();
			}
		}
	}

	public override void OnStart()
	{
		if (conditionals == null)
		{
			return;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				conditionals[i].OnStart();
			}
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (conditionals == null)
		{
			return TaskStatus.Failure;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				TaskStatus taskStatus = conditionals[i].OnUpdate();
				if (comparisonType == ComparisonType.Sequence && taskStatus == TaskStatus.Failure)
				{
					return TaskStatus.Failure;
				}
				if (comparisonType == ComparisonType.Selector && taskStatus == TaskStatus.Success)
				{
					return TaskStatus.Success;
				}
			}
		}
		if (comparisonType != ComparisonType.Sequence)
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnFixedUpdate()
	{
		if (conditionals == null)
		{
			return;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				conditionals[i].OnFixedUpdate();
			}
		}
	}

	public override void OnLateUpdate()
	{
		if (conditionals == null)
		{
			return;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				conditionals[i].OnLateUpdate();
			}
		}
	}

	public override void OnEnd()
	{
		if (conditionals == null)
		{
			return;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				conditionals[i].OnEnd();
			}
		}
	}

	public override void OnTriggerEnter(Collider other)
	{
		if (conditionals == null)
		{
			return;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				conditionals[i].OnTriggerEnter(other);
			}
		}
	}

	public override void OnTriggerEnter2D(Collider2D other)
	{
		if (conditionals == null)
		{
			return;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				conditionals[i].OnTriggerEnter2D(other);
			}
		}
	}

	public override void OnTriggerExit(Collider other)
	{
		if (conditionals == null)
		{
			return;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				conditionals[i].OnTriggerExit(other);
			}
		}
	}

	public override void OnTriggerExit2D(Collider2D other)
	{
		if (conditionals == null)
		{
			return;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				conditionals[i].OnTriggerExit2D(other);
			}
		}
	}

	public override void OnCollisionEnter(Collision collision)
	{
		if (conditionals == null)
		{
			return;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				conditionals[i].OnCollisionEnter(collision);
			}
		}
	}

	public override void OnCollisionEnter2D(Collision2D collision)
	{
		if (conditionals == null)
		{
			return;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				conditionals[i].OnCollisionEnter2D(collision);
			}
		}
	}

	public override void OnCollisionExit(Collision collision)
	{
		if (conditionals == null)
		{
			return;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				conditionals[i].OnCollisionExit(collision);
			}
		}
	}

	public override void OnCollisionExit2D(Collision2D collision)
	{
		if (conditionals == null)
		{
			return;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				conditionals[i].OnCollisionExit2D(collision);
			}
		}
	}

	public override string OnDrawNodeText()
	{
		if (conditionals == null || !graphLabel)
		{
			return string.Empty;
		}
		string text = string.Empty;
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "\n";
				}
				text += conditionals[i].GetType().Name;
			}
		}
		return text;
	}

	public override void OnReset()
	{
		if (conditionals == null)
		{
			return;
		}
		for (int i = 0; i < conditionals.Length; i++)
		{
			if (conditionals[i] != null)
			{
				conditionals[i].OnReset();
			}
		}
	}
}
