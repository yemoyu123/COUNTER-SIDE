using UnityEngine;

namespace NKC.FX;

public class NKC_FX_SET_BOOL_SMB : StateMachineBehaviour
{
	public string ParameterName;

	[Space]
	public bool EnableOnEnter;

	public bool OnEnterBool;

	[Space]
	public bool EnableOnExit;

	public bool OnExitBool;

	private int hash;

	private void Awake()
	{
		hash = Animator.StringToHash(ParameterName);
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (EnableOnEnter)
		{
			animator.SetBool(ParameterName, OnEnterBool);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (EnableOnExit)
		{
			animator.SetBool(ParameterName, OnExitBool);
		}
	}
}
