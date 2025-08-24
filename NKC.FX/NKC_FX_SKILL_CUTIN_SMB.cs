using UnityEngine;

namespace NKC.FX;

public class NKC_FX_SKILL_CUTIN_SMB : StateMachineBehaviour
{
	public bool StartPrepare;

	public bool ExitRelease;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (StartPrepare)
		{
			animator.gameObject.GetComponent<NKC_FX_SKILL_CUTIN_RENDER>().PrepareRender();
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (ExitRelease)
		{
			animator.gameObject.GetComponent<NKC_FX_SKILL_CUTIN_RENDER>().ReleaseRender();
		}
	}
}
