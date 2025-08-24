using NKC.Templet;
using NKC.UI.Tooltip;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.Component;

public class NKCComTMPUIText : TextMeshProUGUI, IPointerDownHandler, IEventSystemHandler
{
	public string m_StringKey;

	public bool m_IgnoreNewlineChar;

	public override string text
	{
		get
		{
			return base.text;
		}
		set
		{
			if (m_IgnoreNewlineChar)
			{
				base.text = NKCUtil.RemoveLabelCharText(text, "\n");
			}
			else
			{
				base.text = value;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (!string.IsNullOrWhiteSpace(m_StringKey))
		{
			NKCUtil.SetLabelText(this, NKCStringTable.GetString(m_StringKey));
		}
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		int num = TMP_TextUtilities.FindIntersectingLink(this, Input.mousePosition, NKCCamera.GetSubUICamera());
		if (num != -1)
		{
			TMP_LinkInfo tMP_LinkInfo = base.textInfo.linkInfo[num];
			NKCKeywordTemplet nKCKeywordTemplet = NKCKeywordTemplet.Find(tMP_LinkInfo.GetLinkID());
			if (nKCKeywordTemplet != null)
			{
				NKCUITooltip.TextData textData = new NKCUITooltip.TextData(NKCStringTable.GetString(nKCKeywordTemplet.Desc));
				NKCUITooltip.Instance.Open(textData, eventData.position);
			}
		}
	}

	public override void SetAllDirty()
	{
		base.SetAllDirty();
		m_havePropertiesChanged = true;
	}

	public override void SetMaterialDirty()
	{
		m_havePropertiesChanged = true;
		base.SetMaterialDirty();
	}
}
