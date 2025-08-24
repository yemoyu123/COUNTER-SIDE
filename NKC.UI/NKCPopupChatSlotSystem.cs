using ClientPacket.Common;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupChatSlotSystem : MonoBehaviour
{
	public Text m_lbMessage;

	public Image m_bg;

	public Color m_cNormal;

	public Color m_cNotice;

	public Color m_cNoticeDungeon;

	public Color m_cGuildRename;

	public void SetData(NKMChatMessageData data)
	{
		NKCUtil.SetLabelText(m_lbMessage, NKCServerStringFormatter.TranslateServerFormattedString(data.message));
		if (data.messageType == ChatMessageType.SystemNotice)
		{
			NKCUtil.SetImageColor(m_bg, m_cNotice);
		}
		else if (data.messageType == ChatMessageType.SystemNoticeDungeon)
		{
			NKCUtil.SetImageColor(m_bg, m_cNoticeDungeon);
		}
		else if (data.messageType == ChatMessageType.SystemRename)
		{
			NKCUtil.SetImageColor(m_bg, m_cGuildRename);
		}
		else
		{
			NKCUtil.SetImageColor(m_bg, m_cNormal);
		}
	}
}
