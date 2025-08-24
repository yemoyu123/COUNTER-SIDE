using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.HUD;

public class NKCGameHudAlertCommon : MonoBehaviour
{
	public Animator m_Animator;

	public Image m_Image;

	public Text m_Title;

	public Text m_Desc;

	public void SetData(string title, string desc, string iconName)
	{
		Sprite sp = (string.IsNullOrEmpty(iconName) ? null : NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_BC", iconName));
		NKCUtil.SetLabelText(m_Title, title);
		NKCUtil.SetLabelText(m_Desc, desc);
		NKCUtil.SetImageSprite(m_Image, sp, bDisableIfSpriteNull: true);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_Animator.SetTrigger("PLAY");
		m_Animator.GetCurrentAnimatorStateInfo(0);
	}

	public bool IsFinished()
	{
		return m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
	}
}
