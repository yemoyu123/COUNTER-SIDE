using Cs.Logging;
using UnityEngine;

namespace NKC.FX;

public class NKC_FX_STATE_LOG_SMB : StateMachineBehaviour
{
	public string StateName = string.Empty;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Log.Info("[<color=magenta><b>OnStateEnter</b></color>][<color=green><b>" + StateName + "</b></color>] : " + animator.transform.parent.gameObject.name, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/FX/SMB/NKC_FX_STATE_LOG_SMB.cs", 10);
	}
}
