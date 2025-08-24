using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIMessageToastSimpleSlot : MonoBehaviour
{
	public float m_fStayTime;

	public Animator m_animator;

	public Image m_imgIcon;

	public Text m_lbName;

	public Text m_lbCount;

	public RectTransform m_rectIcon;

	public RectTransform m_rectTextX;

	public RectTransform m_rectLayoutGroup;

	private float m_fIdleTimer;

	public void SetData(NKCPopupMessageToastSimple.MessageData msgData)
	{
		m_fIdleTimer = 0f;
		NKCUtil.SetImageSprite(m_imgIcon, msgData.sprite);
		NKCUtil.SetLabelText(m_lbName, msgData.name);
		NKCUtil.SetLabelText(m_lbCount, msgData.count.ToString());
		if (string.IsNullOrEmpty(msgData.name))
		{
			return;
		}
		float num = m_lbCount.preferredWidth + m_rectIcon.GetWidth() + m_rectTextX.GetWidth();
		float num2 = m_rectLayoutGroup.GetWidth() - num;
		if (!(m_lbName.preferredWidth > num2))
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int length = msgData.name.Length;
		for (int i = 0; i < length; i++)
		{
			stringBuilder.Append(msgData.name[i]);
			NKCUtil.SetLabelText(m_lbName, stringBuilder.ToString());
			if (m_lbName.preferredWidth > num2)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				stringBuilder.Append("...");
				NKCUtil.SetLabelText(m_lbName, stringBuilder.ToString());
				break;
			}
		}
	}

	public void ResetSlot()
	{
		NKCUtil.SetImageSprite(m_imgIcon, null);
		NKCUtil.SetLabelText(m_lbName, "");
		NKCUtil.SetLabelText(m_lbCount, "");
	}

	public void PlayIdleAni()
	{
		m_fIdleTimer = m_fStayTime;
		m_animator.Play("NKM_UI_POPUP_MESSAGE_TOAST_IDLE");
	}

	private void Update()
	{
		if (m_animator.GetCurrentAnimatorStateInfo(0).IsName("NKM_UI_POPUP_MESSAGE_TOAST_IDLE"))
		{
			m_fIdleTimer += Time.deltaTime;
			if (m_fIdleTimer >= m_fStayTime && base.transform.GetSiblingIndex() == 0)
			{
				m_animator.SetTrigger("OUT");
			}
		}
	}
}
