using NKC.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIPopupVoiceSlot : MonoBehaviour
{
	public delegate void OnChangeToggle(bool bValue, int index);

	public NKCUIComToggle m_toggle;

	[Header("ON")]
	public Text m_txtType_On;

	public Text m_txtName_On;

	public GameObject m_objSkin_On;

	[Header("OFF")]
	public Text m_txtType_Off;

	public Text m_txtName_Off;

	public GameObject m_objSkin_Off;

	private NKCAssetInstanceData m_instance;

	private int m_index;

	private OnChangeToggle dOnChangeToggle;

	public static NKCUIPopupVoiceSlot newInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_VOICE_LIST_SLOT");
		if (nKCAssetInstanceData.m_Instant == null)
		{
			Debug.LogError("NKCUIPopupVoiceSlot Prefab null!");
			return null;
		}
		NKCUIPopupVoiceSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIPopupVoiceSlot>();
		component.m_instance = nKCAssetInstanceData;
		component.transform.SetParent(parent);
		component.transform.localPosition = Vector3.zero;
		component.transform.localScale = Vector3.one;
		component.Init();
		return component;
	}

	private void Init()
	{
		m_toggle.OnValueChanged.RemoveAllListeners();
		m_toggle.OnValueChanged.AddListener(ChangeToggle);
		m_toggle.Select(bSelect: false, bForce: true);
	}

	public void SetUI(int index, NKCCollectionVoiceTemplet templet, bool bSkin, OnChangeToggle onChangeToggle)
	{
		m_index = index;
		NKCUtil.SetLabelText(m_txtType_On, NKCUtilString.GetStringVoiceCategory(templet.m_bVoiceCondLifetime));
		NKCUtil.SetLabelText(m_txtType_Off, NKCUtilString.GetStringVoiceCategory(templet.m_bVoiceCondLifetime));
		NKCUtil.SetLabelText(m_txtName_On, templet.ButtonName);
		NKCUtil.SetLabelText(m_txtName_Off, templet.ButtonName);
		NKCUtil.SetGameobjectActive(m_objSkin_On, bSkin);
		NKCUtil.SetGameobjectActive(m_objSkin_Off, bSkin);
		dOnChangeToggle = onChangeToggle;
	}

	public void Clear()
	{
		if (m_instance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_instance);
		}
	}

	public void SetToggle(int curIndex)
	{
		m_toggle.Select(curIndex == m_index, bForce: true);
	}

	private void ChangeToggle(bool bSet)
	{
		dOnChangeToggle?.Invoke(bSet, m_index);
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_instance);
		m_instance = null;
		Object.Destroy(base.gameObject);
	}
}
