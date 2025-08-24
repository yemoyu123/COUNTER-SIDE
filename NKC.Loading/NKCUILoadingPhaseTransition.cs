using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.Loading;

public class NKCUILoadingPhaseTransition : MonoBehaviour
{
	private const string INTRO_ANIM_NAME = "INTRO";

	private const string IDLE_ANIM_NAME = "IDLE";

	private const string OUTRO_ANIM_NAME = "OUTRO";

	public Animator m_Animator;

	public GameObject m_objProgress;

	public Text m_lbProgress;

	public Text m_lbProgressCount;

	public Image m_imgProgress;

	public void PlayIntro()
	{
		PlayAnimation("INTRO");
		NKCUtil.SetGameobjectActive(m_objProgress, bValue: false);
	}

	public void PlayIdle()
	{
		PlayAnimation("IDLE");
	}

	public void PlayOutro()
	{
		PlayAnimation("OUTRO");
		NKCUtil.SetGameobjectActive(m_objProgress, bValue: false);
	}

	private void PlayAnimation(string name)
	{
		if (m_Animator != null)
		{
			m_Animator.Play(name, -1, 0f);
			m_Animator.Update(0.001f);
		}
	}

	public bool IsAnimFinished()
	{
		if (m_Animator == null)
		{
			return true;
		}
		AnimatorStateInfo currentAnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
		if (!currentAnimatorStateInfo.loop)
		{
			return currentAnimatorStateInfo.normalizedTime > 1f;
		}
		return false;
	}

	public void SetLoadingProgress(float fProgress)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_GAME)
		{
			NKCUtil.SetGameobjectActive(m_objProgress, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objProgress, bValue: true);
		NKCUtil.SetLabelText(m_lbProgress, NKCUtilString.GET_STRING_ATTACK_PREPARING);
		NKCUtil.SetLabelText(m_lbProgressCount, $"{(int)(fProgress * 100f)}%");
		m_imgProgress.fillAmount = fProgress;
	}
}
