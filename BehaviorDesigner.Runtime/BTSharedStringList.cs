using System;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime;

[Serializable]
public class BTSharedStringList : BTSharedNKCValue<List<string>>
{
	private readonly char[] defaultSeperator = new char[4] { '\n', ' ', '\t', ',' };

	public static implicit operator BTSharedStringList(List<string> value)
	{
		return new BTSharedStringList
		{
			Value = value
		};
	}

	public override bool TryParse(string parameters)
	{
		return TryParse(parameters, defaultSeperator);
	}

	public override string ToString()
	{
		return BTSharedNKCValue.ToDebugString(base.Value);
	}

	public bool TryParse(string parameters, char[] seperator)
	{
		string[] collection = parameters.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
		base.Value = new List<string>(collection);
		return true;
	}
}
