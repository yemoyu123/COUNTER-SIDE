using System;
using NKM;

namespace BehaviorDesigner.Runtime;

[Serializable]
public class BTSharedEnum<T> : BTSharedNKCValue<T> where T : Enum
{
	public static implicit operator BTSharedEnum<T>(T value)
	{
		return new BTSharedEnum<T>
		{
			Value = value
		};
	}

	public override bool TryParse(string parameter)
	{
		if (parameter.TryParse<T>(out var @enum))
		{
			base.Value = @enum;
			return true;
		}
		return false;
	}
}
