using NKC.Templet;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupFilterThemeSlot : MonoBehaviour
{
	public delegate void OnClick(int id);

	public Image m_ImgIcon;

	public Text m_lbName;

	public NKCUIComToggle m_Toggle;

	private OnClick dOnClick;

	private int m_ID;

	public void Init(NKCUIComToggleGroup toggleGroup)
	{
		if (m_Toggle != null)
		{
			m_Toggle.SetToggleGroup(toggleGroup);
		}
		NKCUtil.SetToggleValueChangedDelegate(m_Toggle, OnToggle);
	}

	public void SetData(NKCThemeGroupTemplet templet, OnClick onClick)
	{
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("ab_inven_icon_fnc_theme", templet.GroupIconName));
		NKCUtil.SetImageSprite(m_ImgIcon, orLoadAssetResource);
		NKCUtil.SetLabelText(m_lbName, NKCStringTable.GetString(templet.GroupStringKey));
		m_ID = templet.Key;
		dOnClick = onClick;
	}

	public void SetSelected(bool value)
	{
		m_Toggle.Select(value, bForce: true);
	}

	private void OnToggle(bool value)
	{
		if (value)
		{
			dOnClick?.Invoke(m_ID);
		}
	}
}
