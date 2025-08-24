using UnityEngine.EventSystems;

namespace NKC.UI;

public class NKCUIComStateButtonPopup : NKCUIComStateButton
{
	public string BUNDLE_NAME = "";

	public string ASSET_NAME = "";

	public string POPUP_TITLE_STR_KEY = "";

	public string POPUP_DESC_STR_KEY = "";

	private NKCPopupImage m_PopupImage;

	protected override void OnPointerClickEvent(PointerEventData eventData)
	{
		if (!string.IsNullOrEmpty(BUNDLE_NAME) && !string.IsNullOrEmpty(ASSET_NAME))
		{
			m_PopupImage = NKCPopupImage.OpenInstance(BUNDLE_NAME, ASSET_NAME);
			m_PopupImage.Open(POPUP_TITLE_STR_KEY, POPUP_DESC_STR_KEY);
		}
		else
		{
			base.OnPointerClickEvent(eventData);
		}
	}
}
