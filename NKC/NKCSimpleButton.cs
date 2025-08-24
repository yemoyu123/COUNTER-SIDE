using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC;

public class NKCSimpleButton : MonoBehaviour
{
	public NKCUIComStateButton m_button;

	public GameObject m_objBtnBGNormal;

	public GameObject m_objBtnBGSelected;

	public GameObject m_objBtnBGLocked;

	public Image m_img;

	public Text m_txt;

	public Color m_colOff;

	public Color m_colOn;

	public void Init(UnityAction OnTouch)
	{
		m_button.PointerClick.RemoveAllListeners();
		m_button.PointerClick.AddListener(OnTouch);
		m_button.m_ButtonBG_Normal = m_objBtnBGNormal;
		m_button.m_ButtonBG_Selected = m_objBtnBGSelected;
		m_button.m_ButtonBG_Locked = m_objBtnBGLocked;
	}

	public void On()
	{
		NKCUtil.SetImageColor(m_img, m_colOn);
		NKCUtil.SetLabelTextColor(m_txt, m_colOn);
		m_button.Select(bSelect: true);
	}

	public void Off()
	{
		NKCUtil.SetImageColor(m_img, m_colOff);
		NKCUtil.SetLabelTextColor(m_txt, m_colOff);
		m_button.Select(bSelect: false);
	}
}
