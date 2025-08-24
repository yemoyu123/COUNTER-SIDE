using Spine.Unity;
using UnityEngine;

namespace NKC.FX;

public class NKC_FX_ROOT_STATIC : MonoBehaviour
{
	public SkeletonAnimation skeletonAnimation;

	public Animator AnimatorStatic;

	private void OnDestroy()
	{
		skeletonAnimation = null;
		AnimatorStatic = null;
	}

	private void Start()
	{
		if (skeletonAnimation != null && AnimatorStatic != null)
		{
			skeletonAnimation.UpdateComplete += delegate
			{
				AnimatorStatic.Update(Time.deltaTime);
				AnimatorStatic.speed = skeletonAnimation.timeScale;
			};
		}
	}
}
