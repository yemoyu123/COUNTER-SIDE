using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperatorCardSlot : NKCUIUnitSelectListSlotBase
{
	[Serializable]
	public struct SkillInfo
	{
		public GameObject m_Object;

		public Text m_Lv;

		public GameObject m_Max;
	}

	private NKCAssetInstanceData m_Instance;

	public NKCUIComButton m_NKM_UI_OPERATOR_CARD_SLOT;

	public Image m_NKM_UI_OPERATOR_CARD_SLOT_BG;

	public Image m_NKM_UI_OPERATOR_CARD_ICON;

	public Text m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_NUMBER_TEXT;

	public Image m_NKM_UI_OPERATOR_CARD_LEVEL_GAUGE;

	public Text m_NKM_UI_OPERATOR_CARD_LEVEL_TEXT1;

	public List<SkillInfo> m_lstSkill;

	public Text m_NKM_UI_OPERATOR_CARD_TITLE_TEXT;

	public Text m_NKM_UI_OPERATOR_CARD_NAME_TEXT;

	public GameObject m_NKM_UI_OPERATOR_CARD_USED_BG;

	public Text m_NKM_UI_OPERATOR_CARD_USED_TEXT;

	public GameObject m_NKM_UI_OPERATOR_CARD_LOCK_SELECT;

	public static NKCUIOperatorCardSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_unit_slot_card", "NKM_UI_OPERATOR_CARD_SLOT");
		NKCUIOperatorCardSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIOperatorCardSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIOperatorCardSlot Prefab null!");
			return null;
		}
		if (parent != null)
		{
			component.transform.SetParent(parent);
			component.GetComponent<RectTransform>().localScale = Vector3.one;
			component.Init();
		}
		component.m_Instance = nKCAssetInstanceData;
		component.gameObject.SetActive(value: false);
		return component;
	}

	private void Init()
	{
		if (m_NKM_UI_OPERATOR_CARD_SLOT != null)
		{
			m_NKM_UI_OPERATOR_CARD_SLOT.PointerClick.RemoveAllListeners();
			m_NKM_UI_OPERATOR_CARD_SLOT.PointerClick.AddListener(OnClick);
		}
	}

	public void Clear()
	{
		if (m_Instance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_Instance);
		}
	}

	protected override void OnClick()
	{
		if (dOnSelectThisOperatorSlot != null)
		{
			dOnSelectThisOperatorSlot(m_OperatorData, m_NKMUnitTempletBase, m_DeckIndex, m_eUnitSlotState, m_eUnitSelectState);
		}
	}
}
