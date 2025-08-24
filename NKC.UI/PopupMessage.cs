namespace NKC.UI;

public class PopupMessage
{
	public string m_message;

	public NKCPopupMessage.eMessagePosition m_messagePosition;

	public float m_delayTime;

	public bool m_bPreemptive;

	public bool m_bShowFX;

	public bool m_bWaitForGameEnd;

	public PopupMessage(string message, NKCPopupMessage.eMessagePosition messagePosition, float delayTime, bool bPreemptive, bool bShowFX, bool bWaitForGameEnd)
	{
		m_message = message;
		m_messagePosition = messagePosition;
		m_delayTime = delayTime;
		m_bPreemptive = bPreemptive;
		m_bShowFX = bShowFX;
		m_bWaitForGameEnd = bWaitForGameEnd;
	}
}
