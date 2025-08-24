using NKM;
using UnityEngine;

namespace NKC.UI;

public class NKCUIComShortcut : MonoBehaviour
{
	public NKM_SHORTCUT_TYPE m_ShortcutType;

	public string m_ShortcutParam = "";

	public bool m_bForce;

	private void Awake()
	{
		if (m_ShortcutType == NKM_SHORTCUT_TYPE.SHORTCUT_NONE)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUIComStateButton component = GetComponent<NKCUIComStateButton>();
		if (component == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		component.PointerClick.RemoveAllListeners();
		component.PointerClick.AddListener(OnClickBtn);
	}

	private void OnClickBtn()
	{
		NKCContentManager.MoveToShortCut(m_ShortcutType, m_ShortcutParam, m_bForce);
	}
}
