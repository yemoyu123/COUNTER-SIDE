using NKC.UI.Tooltip;
using NKM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIShipSkillSlot : MonoBehaviour
{
	public enum eShipSkillSlotStatus
	{
		NONE,
		LOCK,
		UNLOCK,
		NEW,
		ENHANCE,
		NEW_GET_POPUP
	}

	public NKCUIComButton m_cbtnSlot;

	public GameObject m_objHighlighted;

	public Image m_imgSkillIcon;

	public GameObject SKILL_ICON_LOCK;

	public GameObject SKILL_ICON_UNLOCK;

	public GameObject SKILL_ICON_NEW;

	public GameObject SKILL_ICON_ENHANCE;

	public GameObject SKILL_ICON_NEW_GET;

	public Text SKILL_ICON_NEW_GET_TEXT;

	public Text m_lbSkillType;

	private UnityAction dOnClickSkillSlot;

	private NKMShipSkillTemplet m_CurrentSkillTemplet;

	private bool m_bToolTip;

	public int CurrentSkillID
	{
		get
		{
			if (m_CurrentSkillTemplet == null)
			{
				return 0;
			}
			return m_CurrentSkillTemplet.m_ShipSkillID;
		}
	}

	public void Init(UnityAction dOnClick, bool bCallToolTip = false)
	{
		if (m_cbtnSlot != null)
		{
			m_cbtnSlot.PointerClick.RemoveAllListeners();
			m_cbtnSlot.PointerClick.AddListener(OnClick);
		}
		if (m_cbtnSlot != null)
		{
			m_cbtnSlot.PointerDown.RemoveAllListeners();
			m_cbtnSlot.PointerDown.AddListener(OnPointerDown);
		}
		dOnClickSkillSlot = dOnClick;
		NKCUtil.SetGameobjectActive(m_objHighlighted, bValue: false);
		m_bToolTip = bCallToolTip;
	}

	public void Cleanup()
	{
		SetData(null);
	}

	public void SetData(NKMShipSkillTemplet unitSkillTemplet, eShipSkillSlotStatus status = eShipSkillSlotStatus.NONE)
	{
		m_CurrentSkillTemplet = unitSkillTemplet;
		if (unitSkillTemplet != null)
		{
			m_imgSkillIcon.sprite = NKCUtil.GetSkillIconSprite(unitSkillTemplet);
			NKCUtil.SetLabelText(m_lbSkillType, NKCUtilString.GetSkillTypeName(unitSkillTemplet.m_NKM_SKILL_TYPE));
		}
		else
		{
			m_imgSkillIcon.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_SHIP_SKILL_ICON", "SS_NO_SKILL_ICON");
		}
		SetStatus(status);
	}

	public void SetStatus(eShipSkillSlotStatus status = eShipSkillSlotStatus.NONE)
	{
		NKCUtil.SetGameobjectActive(SKILL_ICON_ENHANCE, status == eShipSkillSlotStatus.ENHANCE);
		NKCUtil.SetGameobjectActive(SKILL_ICON_LOCK, status == eShipSkillSlotStatus.LOCK);
		NKCUtil.SetGameobjectActive(SKILL_ICON_UNLOCK, status == eShipSkillSlotStatus.UNLOCK);
		NKCUtil.SetGameobjectActive(SKILL_ICON_NEW, status == eShipSkillSlotStatus.NEW);
		NKCUtil.SetGameobjectActive(SKILL_ICON_NEW_GET, status == eShipSkillSlotStatus.NEW_GET_POPUP);
	}

	public void SetText(string text)
	{
		NKCUtil.SetLabelText(SKILL_ICON_NEW_GET_TEXT, text);
	}

	public void SetHighlight(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objHighlighted, value);
	}

	public void OnClick()
	{
		if (dOnClickSkillSlot != null)
		{
			dOnClickSkillSlot();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (m_bToolTip && m_CurrentSkillTemplet != null)
		{
			NKCUITooltip.Instance.Open(m_CurrentSkillTemplet, eventData.position);
		}
	}
}
