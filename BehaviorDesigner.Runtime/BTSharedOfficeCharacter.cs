using NKC.Office;

namespace BehaviorDesigner.Runtime;

public class BTSharedOfficeCharacter : SharedVariable<NKCOfficeCharacter>
{
	public static implicit operator BTSharedOfficeCharacter(NKCOfficeCharacter value)
	{
		return new BTSharedOfficeCharacter
		{
			Value = value
		};
	}
}
