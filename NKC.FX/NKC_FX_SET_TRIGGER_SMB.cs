using UnityEngine;

namespace NKC.FX;

public class NKC_FX_SET_TRIGGER_SMB : StateMachineBehaviour
{
	public string TriggerName;

	private int triggerHash;

	private void Awake()
	{
		triggerHash = Animator.StringToHash(TriggerName);
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetTrigger(triggerHash);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger(triggerHash);
	}
}
