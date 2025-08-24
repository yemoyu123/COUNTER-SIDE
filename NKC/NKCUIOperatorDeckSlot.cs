using System;
using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIOperatorDeckSlot : MonoBehaviour
{
	[Serializable]
	public struct Skill
	{
		public GameObject m_Obj;

		public Text m_LV;

		public GameObject m_MAX;

		public GameObject m_ENHANCE;

		public GameObject m_IMPLANT;
	}

	public delegate void OnSelectOperator(long unitUID);

	private enum STAT
	{
		NONE,
		HIDE,
		LOCK,
		EMPTY,
		ACTIVE,
		RANDOM
	}

	private NKCAssetInstanceData m_Instance;

	[Header("오퍼레이터 슬롯")]
	public NKCUIComStateButton m_NKM_UI_OPERATOR_DECK_SLOT;

	public GameObject m_NKM_UI_OPERATOR_DECK_SLOT_MAIN;

	public Image m_NKM_UI_OPERATOR_DECK_SLOT_BG_Panel;

	public Image m_NKM_UI_OPERATOR_DECK_SLOT_IMG_Panel;

	public GameObject m_NKM_UI_OPERATOR_DECK_SLOT_BORDER;

	public Text m_NKM_UI_OPERATOR_DECK_SLOT_LEVEL_TEXT1;

	public GameObject m_objMaxLevel;

	public List<Skill> m_lstSkill;

	private OnSelectOperator m_OnClick;

	[Header("밴 표시")]
	public GameObject m_objBan;

	public Text m_lbBanLevel;

	private long m_curOperatorUID;

	private STAT m_curStat;

	public static NKCUIOperatorDeckSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_nkm_ui_operator_deck", "NKM_UI_OPERATOR_DECK_SLOT");
		NKCUIOperatorDeckSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIOperatorDeckSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIOperatorDeckSlot Prefab null!");
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

	public void Init(OnSelectOperator callBack = null)
	{
		if (m_NKM_UI_OPERATOR_DECK_SLOT != null)
		{
			m_NKM_UI_OPERATOR_DECK_SLOT.PointerClick.RemoveAllListeners();
			m_NKM_UI_OPERATOR_DECK_SLOT.PointerClick.AddListener(OnClick);
		}
		m_OnClick = callBack;
		m_curOperatorUID = 0L;
	}

	public void Clear()
	{
		if (m_Instance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_Instance);
		}
	}

	public void SetSelectEffect(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_DECK_SLOT_BORDER, bActive);
	}

	private void UpdateCommonUI(NKMOperator operatorData)
	{
		if (operatorData != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorData.id);
			if (unitTempletBase != null)
			{
				NKCUtil.SetImageSprite(m_NKM_UI_OPERATOR_DECK_SLOT_BG_Panel, NKCUtil.GetSpriteOperatorBG(unitTempletBase.m_NKM_UNIT_GRADE));
			}
			Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, operatorData);
			if (sprite != null)
			{
				NKCUtil.SetImageSprite(m_NKM_UI_OPERATOR_DECK_SLOT_IMG_Panel, sprite);
			}
			NKCUtil.SetLabelText(m_NKM_UI_OPERATOR_DECK_SLOT_LEVEL_TEXT1, operatorData.level.ToString());
			NKCUtil.SetGameobjectActive(m_objMaxLevel, NKCOperatorUtil.IsMaximumLevel(operatorData.level));
		}
	}

	private void HideSkillLevelUI()
	{
		foreach (Skill item in m_lstSkill)
		{
			NKCUtil.SetGameobjectActive(item.m_Obj, bValue: false);
		}
	}

	public void SetData(NKMOperator operatorData, bool bEnableShowBan = false)
	{
		NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
		if (operatorData == null)
		{
			SetState(STAT.EMPTY);
			m_curOperatorUID = 0L;
			return;
		}
		SetState(STAT.ACTIVE);
		m_curOperatorUID = operatorData.uid;
		UpdateCommonUI(operatorData);
		HideSkillLevelUI();
		if (bEnableShowBan)
		{
			ProcessBanUI();
		}
	}

	public void UpdateData(NKMOperator operatorData)
	{
		if (m_curOperatorUID != 0L && m_curOperatorUID == operatorData.uid)
		{
			UpdateCommonUI(operatorData);
		}
	}

	public void SetData(NKMUnitTempletBase unitTempletBase, int Level)
	{
		if (unitTempletBase == null)
		{
			SetState(STAT.EMPTY);
			m_curOperatorUID = 0L;
			HideSkillLevelUI();
			return;
		}
		NKCUtil.SetImageSprite(m_NKM_UI_OPERATOR_DECK_SLOT_BG_Panel, NKCUtil.GetSpriteOperatorBG(unitTempletBase.m_NKM_UNIT_GRADE));
		NKMUnitData nKMUnitData = new NKMUnitData();
		nKMUnitData.m_UnitID = unitTempletBase.m_UnitID;
		nKMUnitData.m_SkinID = 0;
		Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, nKMUnitData);
		if (sprite != null)
		{
			NKCUtil.SetImageSprite(m_NKM_UI_OPERATOR_DECK_SLOT_IMG_Panel, sprite);
		}
		NKCUtil.SetLabelText(m_NKM_UI_OPERATOR_DECK_SLOT_LEVEL_TEXT1, Level.ToString());
		NKCUtil.SetGameobjectActive(m_objMaxLevel, NKCOperatorUtil.IsMaximumLevel(Level));
		HideSkillLevelUI();
		SetState(STAT.ACTIVE);
	}

	private void OnClick()
	{
		m_OnClick?.Invoke(m_curOperatorUID);
	}

	public void SetHide()
	{
		SetState(STAT.HIDE);
	}

	public void SetLock()
	{
		SetState(STAT.LOCK);
	}

	public void SetEmpty()
	{
		SetState(STAT.EMPTY);
	}

	public void SetRandom()
	{
		SetState(STAT.RANDOM);
	}

	private void SetState(STAT newStat)
	{
		switch (newStat)
		{
		case STAT.LOCK:
			NKCUtil.SetImageSprite(m_NKM_UI_OPERATOR_DECK_SLOT_BG_Panel, NKCOperatorUtil.GetSpriteLockSlot());
			break;
		case STAT.EMPTY:
			NKCUtil.SetImageSprite(m_NKM_UI_OPERATOR_DECK_SLOT_BG_Panel, NKCOperatorUtil.GetSpriteEmptySlot());
			break;
		case STAT.RANDOM:
			NKCUtil.SetImageSprite(m_NKM_UI_OPERATOR_DECK_SLOT_BG_Panel, NKCOperatorUtil.GetSpriteRandomSlot());
			break;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_DECK_SLOT_MAIN, newStat == STAT.ACTIVE);
		NKCUtil.SetGameobjectActive(base.gameObject, newStat != STAT.HIDE);
		m_curStat = newStat;
	}

	private void ProcessBanUI()
	{
		NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(m_curOperatorUID);
		if (operatorData != null)
		{
			if (!NKCBanManager.IsBanOperator(operatorData.id))
			{
				NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
			int operBanLevel = NKCBanManager.GetOperBanLevel(operatorData.id);
			NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_BAN_LEVEL_ONE_PARAM, operBanLevel));
		}
	}
}
