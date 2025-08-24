using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUISkillSlot : MonoBehaviour
{
	public delegate void OnClickSkillSlot(NKMUnitSkillTemplet selectedSkillTemplet);

	public NKCUIComButton m_cbtnSlot;

	public GameObject m_objEmpty;

	public GameObject m_objHyperEmpty;

	public GameObject m_objBorder;

	public GameObject m_objHighlighted;

	public GameObject m_objHyperHighlighted;

	public GameObject m_objNormalSkillLocked;

	public GameObject m_objHyperSkillLocked;

	public Image m_imgSkillIcon;

	public Text m_lbSkillLevel;

	public Image m_imgNew;

	private OnClickSkillSlot dOnClickSkillSlot;

	private NKMUnitSkillTemplet m_CurrentSkillTemplet;

	private bool m_bIsHyper;

	public int CurrentSkillID
	{
		get
		{
			if (m_CurrentSkillTemplet == null)
			{
				return 0;
			}
			return m_CurrentSkillTemplet.m_ID;
		}
	}

	public void Init(OnClickSkillSlot dOnClick)
	{
		if (m_cbtnSlot != null)
		{
			m_cbtnSlot.PointerClick.RemoveAllListeners();
			m_cbtnSlot.PointerClick.AddListener(OnClick);
		}
		dOnClickSkillSlot = dOnClick;
		NKCUtil.SetGameobjectActive(m_objEmpty, bValue: true);
		NKCUtil.SetGameobjectActive(m_objHyperEmpty, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBorder, bValue: false);
		NKCUtil.SetGameobjectActive(m_objHighlighted, bValue: false);
		NKCUtil.SetGameobjectActive(m_objHyperHighlighted, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNormalSkillLocked, bValue: false);
		NKCUtil.SetGameobjectActive(m_objHyperSkillLocked, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgSkillIcon, bValue: false);
		NKCUtil.SetGameobjectActive(m_lbSkillLevel, bValue: false);
	}

	public void Cleanup()
	{
		SetData(null, bIsHyper: false);
	}

	public void SetData(NKMUnitSkillTemplet unitSkillTemplet, bool bIsHyper)
	{
		m_CurrentSkillTemplet = unitSkillTemplet;
		m_bIsHyper = bIsHyper;
		if (unitSkillTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
			NKCUtil.SetGameobjectActive(m_objHyperEmpty, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNormalSkillLocked, bValue: false);
			NKCUtil.SetGameobjectActive(m_objHyperSkillLocked, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgSkillIcon, bValue: true);
			NKCUtil.SetGameobjectActive(m_lbSkillLevel, bValue: true);
			NKCUtil.SetImageSprite(m_imgSkillIcon, NKCUtil.GetSkillIconSprite(unitSkillTemplet));
			NKCUtil.SetLabelText(m_lbSkillLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, unitSkillTemplet.m_Level));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objEmpty, !m_bIsHyper);
			NKCUtil.SetGameobjectActive(m_objHyperEmpty, m_bIsHyper);
			NKCUtil.SetGameobjectActive(m_objHighlighted, bValue: false);
			NKCUtil.SetGameobjectActive(m_objHyperHighlighted, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNormalSkillLocked, bValue: false);
			NKCUtil.SetGameobjectActive(m_objHyperSkillLocked, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgSkillIcon, bValue: false);
			NKCUtil.SetGameobjectActive(m_lbSkillLevel, bValue: false);
		}
	}

	public void LockSkill(bool value)
	{
		if (m_bIsHyper)
		{
			NKCUtil.SetGameobjectActive(m_objHyperSkillLocked, value);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objNormalSkillLocked, value);
		}
	}

	public void SetHighlight(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objHyperHighlighted, m_bIsHyper && value);
		NKCUtil.SetGameobjectActive(m_objHighlighted, !m_bIsHyper && value);
	}

	public void OnClick()
	{
		if (dOnClickSkillSlot != null)
		{
			dOnClickSkillSlot(m_CurrentSkillTemplet);
		}
	}

	public void SetShipData(NKMShipSkillTemplet shipSkillTemplet, bool bLock = false)
	{
		m_imgSkillIcon.sprite = NKCUtil.GetSkillIconSprite(shipSkillTemplet);
		NKCUtil.SetGameobjectActive(m_objHyperEmpty, bValue: false);
		NKCUtil.SetGameobjectActive(m_objHighlighted, bValue: false);
		NKCUtil.SetGameobjectActive(m_objHyperHighlighted, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNormalSkillLocked, bLock);
		NKCUtil.SetGameobjectActive(m_objHyperSkillLocked, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgSkillIcon, bValue: true);
		NKCUtil.SetGameobjectActive(m_lbSkillLevel, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgNew, bValue: false);
	}
}
