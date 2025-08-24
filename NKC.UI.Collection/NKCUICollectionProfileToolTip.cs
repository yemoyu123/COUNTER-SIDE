using NKC.Templet;
using NKC.UI.Tooltip;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.Collection;

public class NKCUICollectionProfileToolTip : MonoBehaviour
{
	public NKCUIComStateButton m_button;

	private string m_profileType;

	public void Init()
	{
		if (m_button != null)
		{
			m_button.PointerDown.RemoveAllListeners();
			m_button.PointerDown.AddListener(OnButtonDown);
		}
	}

	public void SetDescData(string profileType)
	{
		m_profileType = profileType;
		if (m_button != null)
		{
			m_button.enabled = !string.IsNullOrEmpty(m_profileType) && NKCCollectionManager.GetProfileToolTipTemplet(m_profileType) != null;
		}
	}

	private void OnButtonDown(PointerEventData e)
	{
		if (!string.IsNullOrEmpty(m_profileType))
		{
			NKCCollectionProfileToolTipTemplet profileToolTipTemplet = NKCCollectionManager.GetProfileToolTipTemplet(m_profileType);
			if (profileToolTipTemplet != null)
			{
				string title = NKCStringTable.GetString(m_profileType);
				string desc = NKCStringTable.GetString(profileToolTipTemplet.ProfileTypeDesc);
				NKCUITooltip.Instance.Open(NKCUISlot.eSlotMode.Etc, title, desc, e.position);
			}
		}
	}
}
