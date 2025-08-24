using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace NKC.BT;

public class BTSetBoolVariable : Action
{
	public string name;

	public bool value;

	public override void OnStart()
	{
		base.OnAwake();
		if (base.Owner.GetVariable(name) is SharedBool sharedBool)
		{
			sharedBool.Value = value;
		}
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}
