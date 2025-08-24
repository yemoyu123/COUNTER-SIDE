using UnityEngine;

namespace NKC;

[DisallowMultipleComponent]
public class NKCComSpineAnimControl : MonoBehaviour
{
	private GameObject m_SPINE_SkeletonAnimation;

	private NKCAnimSpine m_NKCAnimSpine = new NKCAnimSpine();

	public string animationName;

	public bool loop;

	public float timeScale = 1f;

	private void Awake()
	{
		Transform transform = base.gameObject.transform.Find("SPINE_SkeletonAnimation");
		if (!(transform == null))
		{
			m_SPINE_SkeletonAnimation = transform.gameObject;
			m_NKCAnimSpine.SetAnimObj(m_SPINE_SkeletonAnimation, base.gameObject);
		}
	}

	private void Update()
	{
		if (!(m_SPINE_SkeletonAnimation == null))
		{
			m_NKCAnimSpine.Update(Time.deltaTime);
		}
	}

	public void Play(string animName)
	{
		if (!(m_SPINE_SkeletonAnimation == null))
		{
			m_NKCAnimSpine.SetPlaySpeed(timeScale);
			m_NKCAnimSpine.Play(animName, loop);
		}
	}

	public void Stop()
	{
	}
}
