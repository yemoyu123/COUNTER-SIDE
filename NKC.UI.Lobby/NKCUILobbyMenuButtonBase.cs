using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public abstract class NKCUILobbyMenuButtonBase : MonoBehaviour
{
	public delegate void OnNotify();

	public GameObject m_objLock;

	public Text m_lbLock;

	private OnNotify dOnNotify;

	private bool m_bHasNotify;

	protected ContentsType m_ContentsType = ContentsType.None;

	protected bool m_bLocked = true;

	public bool Locked => m_bLocked;

	protected abstract void ContentsUpdate(NKMUserData userData);

	public virtual void PlayAnimation(bool bActive)
	{
	}

	public virtual void CleanUp()
	{
	}

	public void UpdateData(NKMUserData userData)
	{
		UpdateLock();
		if (!m_bLocked)
		{
			ContentsUpdate(userData);
		}
		else
		{
			SetNotify(value: false);
		}
	}

	protected virtual void UpdateLock()
	{
		m_bLocked = !NKCContentManager.IsContentsUnlocked(m_ContentsType);
		NKCUtil.SetLabelText(m_lbLock, NKCContentManager.GetLockedMessage(m_ContentsType));
		NKCUtil.SetGameobjectActive(m_objLock, m_bLocked);
	}

	public void SetOnNotify(OnNotify onNotify)
	{
		dOnNotify = onNotify;
	}

	protected virtual void SetNotify(bool value)
	{
		m_bHasNotify = value;
		if (dOnNotify != null)
		{
			dOnNotify();
		}
	}

	public bool HasNotify()
	{
		return m_bHasNotify;
	}
}
