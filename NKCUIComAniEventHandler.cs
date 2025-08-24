using UnityEngine;
using UnityEngine.Events;

public class NKCUIComAniEventHandler : MonoBehaviour
{
	private delegate void OnAniEventHandler();

	public UnityEvent m_NKCUIComAniEvent;

	public void NKCUIComAniEvent()
	{
		if (m_NKCUIComAniEvent != null)
		{
			m_NKCUIComAniEvent.Invoke();
		}
	}
}
