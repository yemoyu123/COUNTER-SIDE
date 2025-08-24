using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIComMapSectionButton : MonoBehaviour
{
	public NKCUIComStateButton m_csbtnButton;

	public Text m_lbNormalText;

	public Text m_lbLockText;

	public GameObject m_objNormal;

	public GameObject m_objLock;

	public GameObject m_objRedDot;

	public int m_iSectionId { get; private set; }

	public void SetLock(int sectionId, bool value)
	{
		m_iSectionId = sectionId;
		NKCUtil.SetGameobjectActive(m_objNormal, !value);
		NKCUtil.SetGameobjectActive(m_objLock, value);
	}

	public bool IsLocked()
	{
		return m_objLock.activeSelf;
	}

	public void SetRedDot(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objRedDot, value);
	}

	public bool IsRedDotOn()
	{
		if (m_objRedDot == null)
		{
			return false;
		}
		return m_objRedDot.activeSelf;
	}
}
