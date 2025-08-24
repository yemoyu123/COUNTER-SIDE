using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.NPC;

public class NKCUIComNPCVoicePlayer : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public NPC_TYPE m_npcType;

	public string[] m_voiceFileName;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (m_voiceFileName != null)
		{
			int num = m_voiceFileName.Length;
			int num2 = Random.Range(0, m_voiceFileName.Length);
			if (num2 >= 0 && num2 < num && !string.IsNullOrEmpty(m_voiceFileName[num2]))
			{
				NKCUINPCBase.PlayVoice(m_npcType, m_voiceFileName[num2], bStopCurrentSound: true, bShowCaption: true);
			}
		}
	}
}
