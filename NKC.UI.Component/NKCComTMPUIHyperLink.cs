using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.Component;

public class NKCComTMPUIHyperLink : NKCComTMPUIText, IPointerDownHandler, IEventSystemHandler
{
	public override void OnPointerDown(PointerEventData eventData)
	{
		int num = TMP_TextUtilities.FindIntersectingLink(this, Input.mousePosition, NKCCamera.GetSubUICamera());
		if (num != -1)
		{
			TMP_LinkInfo tMP_LinkInfo = base.textInfo.linkInfo[num];
			Application.OpenURL(tMP_LinkInfo.GetLinkID());
			return;
		}
		string text = UITextUtilities.FindIntersectingWordWrappedAroundSpace(this, Input.mousePosition, NKCCamera.GetSubUICamera(), visibleOnly: true);
		if (!string.IsNullOrEmpty(text) && UITextUtilities.hasLinkText(text))
		{
			Application.OpenURL(UITextUtilities.RemoveHtmlLikeTags(text));
		}
	}
}
