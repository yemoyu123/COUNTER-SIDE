using ClientPacket.Office;
using NKC.Office;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIPopupOfficePresetSlot : MonoBehaviour
{
	public GameObject m_objNormal;

	public InputField m_ifPresetName;

	public NKCUIComDragScrollInputField m_comDragScrollInputField;

	public NKCUIComStateButton m_csbtnRename;

	public NKCUIComStateButton m_csbtnClear;

	public NKCUIComStateButton m_csbtnLoad;

	public NKCUIComStateButton m_csbtnSave;

	public NKCUIComStateButton m_csbtnAdd;

	public GameObject m_objUnlockFx;

	public Text m_lbDefaultName;

	private int m_PresetID;

	private NKCUIPopupOfficePresetList.OnAction dOnAction;

	public void Init()
	{
		if (m_ifPresetName != null)
		{
			m_ifPresetName.onEndEdit.RemoveAllListeners();
			m_ifPresetName.onEndEdit.AddListener(OnTextInputDoneEdit);
			m_ifPresetName.onValueChanged.RemoveAllListeners();
			m_ifPresetName.onValueChanged.AddListener(OnInputNameChanged);
			m_ifPresetName.onValidateInput = NKCFilterManager.FilterEmojiInput;
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnRename, OnBtnRename);
		NKCUtil.SetButtonClickDelegate(m_csbtnClear, OnBtnClear);
		NKCUtil.SetButtonClickDelegate(m_csbtnLoad, OnBtnLoad);
		NKCUtil.SetButtonClickDelegate(m_csbtnSave, OnBtnSave);
		NKCUtil.SetButtonClickDelegate(m_csbtnAdd, OnBtnAdd);
		NKCUtil.SetGameobjectActive(m_objUnlockFx, bValue: false);
	}

	public void SetData(int currentRoomID, NKMOfficePreset preset, NKCUIPopupOfficePresetList.OnAction onAction)
	{
		NKCUtil.SetGameobjectActive(m_objNormal, bValue: true);
		NKCUtil.SetGameobjectActive(m_csbtnAdd, bValue: false);
		NKCUtil.SetGameobjectActive(m_objUnlockFx, bValue: false);
		m_PresetID = preset.presetId;
		if (m_ifPresetName != null)
		{
			m_ifPresetName.text = preset.name;
		}
		bool value = NKCOfficeManager.IsEmpryPreset(preset);
		if (m_csbtnLoad != null)
		{
			m_csbtnLoad.SetLock(value);
		}
		if (m_csbtnClear != null)
		{
			m_csbtnClear.SetLock(value);
		}
		if (m_csbtnSave != null)
		{
			bool value2 = NKCOfficeManager.IsEmpryRoom(NKCScenManager.CurrentUserData().OfficeData.GetOfficeRoom(currentRoomID));
			m_csbtnSave.SetLock(value2);
		}
		NKCUtil.SetLabelText(m_lbDefaultName, NKCOfficeManager.GetDefaultPresetName(m_PresetID));
		dOnAction = onAction;
	}

	public void SetLoopScroll(LoopScrollRect sr)
	{
		if (!(m_comDragScrollInputField == null))
		{
			m_comDragScrollInputField.ScrollRect = sr;
		}
	}

	public void SetPlus(NKCUIPopupOfficePresetList.OnAction onAction)
	{
		NKCUtil.SetGameobjectActive(m_objNormal, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnAdd, bValue: true);
		NKCUtil.SetGameobjectActive(m_objUnlockFx, bValue: false);
		dOnAction = onAction;
	}

	public void PlayUnlockEffect()
	{
		NKCUtil.SetGameobjectActive(m_objUnlockFx, bValue: true);
	}

	private void OnTextInputDoneEdit(string str)
	{
		NKMOfficePreset preset = NKCScenManager.CurrentUserData().OfficeData.GetPreset(m_PresetID);
		if (preset != null && preset.name != str)
		{
			dOnAction?.Invoke(NKCUIPopupOfficePresetList.ActionType.Rename, m_PresetID, str);
		}
		if (m_comDragScrollInputField != null)
		{
			m_comDragScrollInputField.ActiveInput = false;
		}
		if (m_ifPresetName != null)
		{
			m_ifPresetName.enabled = false;
		}
	}

	private void OnInputNameChanged(string _string)
	{
		m_ifPresetName.text = NKCFilterManager.CheckBadChat(m_ifPresetName.text);
		if (m_ifPresetName.text.Length >= NKMCommonConst.Office.PresetConst.MaxNameLength)
		{
			m_ifPresetName.text = m_ifPresetName.text.Substring(0, NKMCommonConst.Office.PresetConst.MaxNameLength);
		}
	}

	private void OnBtnRename()
	{
		if (!m_ifPresetName.isFocused)
		{
			m_ifPresetName.enabled = true;
			m_ifPresetName.Select();
			m_ifPresetName.ActivateInputField();
			if (m_comDragScrollInputField != null)
			{
				m_comDragScrollInputField.ActiveInput = true;
			}
		}
	}

	private void OnBtnLoad()
	{
		dOnAction?.Invoke(NKCUIPopupOfficePresetList.ActionType.Load, m_PresetID, null);
	}

	private void OnBtnSave()
	{
		dOnAction?.Invoke(NKCUIPopupOfficePresetList.ActionType.Save, m_PresetID, null);
	}

	private void OnBtnClear()
	{
		dOnAction?.Invoke(NKCUIPopupOfficePresetList.ActionType.Clear, m_PresetID, null);
	}

	private void OnBtnAdd()
	{
		dOnAction?.Invoke(NKCUIPopupOfficePresetList.ActionType.Add, 1, null);
	}
}
