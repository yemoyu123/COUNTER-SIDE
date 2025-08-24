using UnityEngine;

namespace NKC.FX;

public class NKC_FX_CONTRACT_HELI_SMB : StateMachineBehaviour
{
	public int Phase;

	public bool DecideSR;

	public bool DecideSSR;

	public bool DecideAwaken;

	public bool DefaultSound;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetInteger("Phase", Phase);
		if (DecideSR)
		{
			animator.SetBool("DecideSR", value: true);
		}
		if (DecideSSR)
		{
			animator.SetBool("DecideSSR", value: true);
		}
		if (DecideAwaken)
		{
			animator.SetBool("DecideAwaken", value: true);
		}
		if (DefaultSound)
		{
			NKCSoundManager.PlaySound("FX_UI_RECRUITMNET_GET", 1f, 0f, 0f);
		}
	}
}
