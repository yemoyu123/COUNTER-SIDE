using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Allows multiple action tasks to be added to a single node.")]
[TaskIcon("{SkinColor}StackedActionIcon.png")]
public class StackedAction : Action
{
	public enum ComparisonType
	{
		Sequence,
		Selector
	}

	[InspectTask]
	public Action[] actions;

	[Tooltip("Specifies if the tasks should be traversed with an AND (Sequence) or an OR (Selector).")]
	public ComparisonType comparisonType;

	[Tooltip("Should the tasks be labeled within the graph?")]
	public bool graphLabel;

	public override void OnAwake()
	{
		if (actions == null)
		{
			return;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				actions[i].GameObject = gameObject;
				actions[i].Transform = transform;
				actions[i].Owner = base.Owner;
				actions[i].OnAwake();
			}
		}
	}

	public override void OnStart()
	{
		if (actions == null)
		{
			return;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				actions[i].OnStart();
			}
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (actions == null)
		{
			return TaskStatus.Failure;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				TaskStatus taskStatus = actions[i].OnUpdate();
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
		if (actions == null)
		{
			return;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				actions[i].OnFixedUpdate();
			}
		}
	}

	public override void OnLateUpdate()
	{
		if (actions == null)
		{
			return;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				actions[i].OnLateUpdate();
			}
		}
	}

	public override void OnEnd()
	{
		if (actions == null)
		{
			return;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				actions[i].OnEnd();
			}
		}
	}

	public override void OnTriggerEnter(Collider other)
	{
		if (actions == null)
		{
			return;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				actions[i].OnTriggerEnter(other);
			}
		}
	}

	public override void OnTriggerEnter2D(Collider2D other)
	{
		if (actions == null)
		{
			return;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				actions[i].OnTriggerEnter2D(other);
			}
		}
	}

	public override void OnTriggerExit(Collider other)
	{
		if (actions == null)
		{
			return;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				actions[i].OnTriggerExit(other);
			}
		}
	}

	public override void OnTriggerExit2D(Collider2D other)
	{
		if (actions == null)
		{
			return;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				actions[i].OnTriggerExit2D(other);
			}
		}
	}

	public override void OnCollisionEnter(Collision collision)
	{
		if (actions == null)
		{
			return;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				actions[i].OnCollisionEnter(collision);
			}
		}
	}

	public override void OnCollisionEnter2D(Collision2D collision)
	{
		if (actions == null)
		{
			return;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				actions[i].OnCollisionEnter2D(collision);
			}
		}
	}

	public override void OnCollisionExit(Collision collision)
	{
		if (actions == null)
		{
			return;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				actions[i].OnCollisionExit(collision);
			}
		}
	}

	public override void OnCollisionExit2D(Collision2D collision)
	{
		if (actions == null)
		{
			return;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				actions[i].OnCollisionExit2D(collision);
			}
		}
	}

	public override string OnDrawNodeText()
	{
		if (actions == null || !graphLabel)
		{
			return string.Empty;
		}
		string text = string.Empty;
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "\n";
				}
				text += actions[i].GetType().Name;
			}
		}
		return text;
	}

	public override void OnReset()
	{
		if (actions == null)
		{
			return;
		}
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
			{
				actions[i].OnReset();
			}
		}
	}
}
