using NKC.UI;
using UnityEngine;

namespace NKC.FX;

public class NKC_FX_DIVE_ARTIFACT_POPUP_SMB : StateMachineBehaviour
{
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.gameObject.GetComponent<NKCPopupDiveArtifactGet>().Close();
	}
}
