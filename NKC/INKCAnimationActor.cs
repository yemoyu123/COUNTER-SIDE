using UnityEngine;

namespace NKC;

public interface INKCAnimationActor
{
	Animator Animator { get; }

	Transform SDParent { get; }

	Transform Transform { get; }

	void PlaySpineAnimation(string name, bool loop, float timeScale);

	void PlaySpineAnimation(NKCASUIUnitIllust.eAnimation eAnim, bool loop, float timeScale, bool bDefaultAnim);

	bool IsSpineAnimationFinished(NKCASUIUnitIllust.eAnimation eAnim);

	bool IsSpineAnimationFinished(string name);

	Vector3 GetBonePosition(string name);

	void PlayEmotion(string animName, float speed);

	bool CanPlaySpineAnimation(string name);

	bool CanPlaySpineAnimation(NKCASUIUnitIllust.eAnimation eAnim);
}
