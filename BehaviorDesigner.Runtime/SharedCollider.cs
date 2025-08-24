using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime;

[Serializable]
public class SharedCollider : SharedVariable<Collider>
{
	public static implicit operator SharedCollider(Collider value)
	{
		return new SharedCollider
		{
			mValue = value
		};
	}
}
