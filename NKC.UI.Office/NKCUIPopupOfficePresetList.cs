using ClientPacket.Office;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIPopupOfficePresetList : NKCUIBase
{
	public enum ActionType
	{
		Save,
		Load,
		Clear,
		Rename,
		Add
	}

	public delegate void OnAction(ActionType type, int id, string name);

	private const string ASSET_BUNDLE_NAME = "ab_ui_office";

	private const string UI_ASSET_NAME = "AB_UI_POPUP_OFFICE_PRESET_LIST";

	private static NKCUIPopupOfficePresetList m_Instance;

	private OnAction dOnAction;

	public NKCUIPopupOfficePresetSlot m_SlotPrefab;

	public LoopScrollRect m_ScrollRect;

	public NKCUIComStateButton m_csbtnClose;

	public Text m_lbPresetCount;

	public string m_strCountFormat = "{0}/{1}";

	private int m_currentRoomID;

	public static NKCUIPopupOfficePresetList Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupOfficePresetList>("ab_ui_office", "AB_UI_POPUP_OFFICE_PRESET_LIST", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupOfficePresetList>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private void InitUI()
	{
		if (m_ScrollRect != null)
		{
			m_ScrollRect.dOnGetObject += GetSlot;
			m_ScrollRect.dOnReturnObject += ReturnSlot;
			m_ScrollRect.dOnProvideData += ProvideSlotData;
			m_ScrollRect.SetAutoResize(2);
			NKCUtil.SetScrollHotKey(m_ScrollRect);
			m_ScrollRect.PrepareCells();
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
	}

	public void Open(int currentRoomID, OnAction onAction)
	{
		base.gameObject.SetActive(value: true);
		dOnAction = onAction;
		m_currentRoomID = currentRoomID;
		if (m_ScrollRect != null)
		{
			m_ScrollRect.TotalCount = Mathf.Min(NKCScenManager.CurrentUserData().OfficeData.GetPresetCount() + 1, NKMCommonConst.Office.PresetConst.MaxCount);
			m_ScrollRect.SetIndexPosition(0);
		}
		SetSlotCountText();
		UIOpened();
	}

	private RectTransform GetSlot(int index)
	{
		NKCUIPopupOfficePresetSlot nKCUIPopupOfficePresetSlot = Object.Instantiate(m_SlotPrefab);
		nKCUIPopupOfficePresetSlot.Init();
		nKCUIPopupOfficePresetSlot.SetLoopScroll(m_ScrollRect);
		return nKCUIPopupOfficePresetSlot.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		Object.Destroy(go.gameObject);
	}

	private void ProvideSlotData(Transform tr, int idx)
	{
		NKCUIPopupOfficePresetSlot component = tr.GetComponent<NKCUIPopupOfficePresetSlot>();
		int presetCount = NKCScenManager.CurrentUserData().OfficeData.GetPresetCount();
		if (idx < presetCount)
		{
			NKMOfficePreset preset = NKCScenManager.CurrentUserData().OfficeData.GetPreset(idx);
			component.SetData(m_currentRoomID, preset, dOnAction);
		}
		else
		{
			component.SetPlus(dOnAction);
		}
	}

	public void Refresh(int index = -1)
	{
		if (index < 0)
		{
			m_ScrollRect.TotalCount = Mathf.Min(NKCScenManager.CurrentUserData().OfficeData.GetPresetCount() + 1, NKMCommonConst.Office.PresetConst.MaxCount);
			m_ScrollRect.RefreshCells();
		}
		else
		{
			Transform child = m_ScrollRect.GetChild(index);
			if (child != null)
			{
				ProvideSlotData(child, index);
			}
		}
		SetSlotCountText();
	}

	public void PlayUnlockEffect(int unlockCount)
	{
		for (int i = 0; i < unlockCount; i++)
		{
			int index = NKCScenManager.CurrentUserData().OfficeData.GetPresetCount() - 1 - i;
			Transform child = m_ScrollRect.GetChild(index);
			if (child != null)
			{
				NKCUIPopupOfficePresetSlot component = child.GetComponent<NKCUIPopupOfficePresetSlot>();
				if (component != null)
				{
					component.PlayUnlockEffect();
				}
			}
		}
	}

	private void SetSlotCountText()
	{
		int presetCount = NKCScenManager.CurrentUserData().OfficeData.GetPresetCount();
		NKCUtil.SetLabelText(m_lbPresetCount, string.Format(m_strCountFormat, presetCount, NKMCommonConst.Office.PresetConst.MaxCount));
	}
}
