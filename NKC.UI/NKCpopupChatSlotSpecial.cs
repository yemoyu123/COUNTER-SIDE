using ClientPacket.Common;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCpopupChatSlotSpecial : MonoBehaviour
{
	public Text m_lbTitle;

	public Text m_lbDesc;

	public void SetData(NKMChatMessageData data)
	{
		if (data.messageType == ChatMessageType.SystemLevelUp)
		{
			NKCUtil.SetLabelText(m_lbTitle, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, data.typeParam));
		}
		NKCUtil.SetLabelText(m_lbDesc, NKCServerStringFormatter.TranslateServerFormattedString(data.message));
	}
}
