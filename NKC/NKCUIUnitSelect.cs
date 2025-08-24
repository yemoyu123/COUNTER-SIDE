using UnityEngine;
using UnityEngine.Events;

namespace NKC;

public class NKCUIUnitSelect : MonoBehaviour
{
	private Animator m_Anim;

	private NKCUIComStateButton m_UNIT_add;

	public void Init(UnityAction addListener = null)
	{
		m_Anim = GetComponent<Animator>();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_UNIT_add = GetComponentInChildren<NKCUIComStateButton>();
		if (addListener != null)
		{
			m_UNIT_add.PointerClick.RemoveAllListeners();
			m_UNIT_add.PointerClick.AddListener(addListener);
		}
	}

	public void Prepare()
	{
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	public void SetListener(UnityAction addListener)
	{
		m_UNIT_add.PointerClick.RemoveAllListeners();
		m_UNIT_add.PointerClick.AddListener(addListener);
	}

	public void Outro()
	{
		m_Anim.SetTrigger("Exit");
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
