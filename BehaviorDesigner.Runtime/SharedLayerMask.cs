using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime;

[Serializable]
public class SharedLayerMask : SharedVariable<LayerMask>
{
	public static implicit operator SharedLayerMask(LayerMask value)
	{
		return new SharedLayerMask
		{
			Value = value
		};
	}
}
