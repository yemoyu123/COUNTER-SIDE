using System;
using System.Collections.Generic;
using DG.Tweening;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIComUnitFilterOptionList : MonoBehaviour
{
	[Serializable]
	public class ButtonWithLabel
	{
		public NKCUIComStateButton m_StateButton;

		public Text m_Label;
	}

	public delegate void OnFilterOptionChange(NKM_UNIT_STYLE_TYPE type);

	public List<ButtonWithLabel> m_lstFilterButtons;

	public NKCUIComToggle m_tglFilterOption;

	public List<Text> m_lstlbFilterOption;

	public RectTransform m_rtFilterOption;

	public float MENU_ANIM_TIME = 0.3f;

	private OnFilterOptionChange dOnFilterOptionChange;

	private readonly List<NKM_UNIT_STYLE_TYPE> lstUnitFilter = new List<NKM_UNIT_STYLE_TYPE>
	{
		NKM_UNIT_STYLE_TYPE.NUST_INVALID,
		NKM_UNIT_STYLE_TYPE.NUST_COUNTER,
		NKM_UNIT_STYLE_TYPE.NUST_SOLDIER,
		NKM_UNIT_STYLE_TYPE.NUST_MECHANIC
	};

	private readonly List<NKM_UNIT_STYLE_TYPE> lstShipFilter = new List<NKM_UNIT_STYLE_TYPE>
	{
		NKM_UNIT_STYLE_TYPE.NUST_INVALID,
		NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT,
		NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER,
		NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY,
		NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL,
		NKM_UNIT_STYLE_TYPE.NUST_SHIP_PATROL
	};

	public void Init(OnFilterOptionChange onFilterOptionChange)
	{
		m_tglFilterOption.OnValueChanged.RemoveAllListeners();
		m_tglFilterOption.OnValueChanged.AddListener(OnTglFilter);
		dOnFilterOptionChange = onFilterOptionChange;
	}

	private void OnTglFilter(bool bChecked)
	{
		SetOpenFilterOption(bChecked);
	}

	private void SetOpenFilterOption(bool bOpen, bool bAnimate = true)
	{
		m_tglFilterOption.Select(bOpen, bForce: true);
		m_rtFilterOption.DOKill();
		Vector3 one = Vector3.one;
		one.y = (bOpen ? 1 : 0);
		if (bAnimate && m_rtFilterOption.gameObject.activeInHierarchy)
		{
			m_rtFilterOption.DOScale(one, MENU_ANIM_TIME).SetEase(Ease.OutCubic);
		}
		else
		{
			m_rtFilterOption.localScale = one;
		}
	}

	public void SetFilterState(NKM_UNIT_STYLE_TYPE type, bool bAnimate)
	{
		SetOpenFilterOption(bOpen: false, bAnimate);
		SetFilterUI(type);
	}

	public void SetFilterButtonList(NKM_UNIT_TYPE eType)
	{
		List<NKM_UNIT_STYLE_TYPE> list = ((eType != NKM_UNIT_TYPE.NUT_NORMAL && eType == NKM_UNIT_TYPE.NUT_SHIP) ? lstShipFilter : lstUnitFilter);
		for (int i = 0; i < list.Count; i++)
		{
			NKM_UNIT_STYLE_TYPE targetFilterType = list[i];
			if (i >= m_lstFilterButtons.Count)
			{
				continue;
			}
			NKCUtil.SetGameobjectActive(m_lstFilterButtons[i].m_StateButton, bValue: true);
			NKCUtil.SetLabelText(m_lstFilterButtons[i].m_Label, NKCUnitSortSystem.GetFilterName(targetFilterType));
			if (m_lstFilterButtons[i].m_StateButton != null)
			{
				m_lstFilterButtons[i].m_StateButton.PointerClick.RemoveAllListeners();
				m_lstFilterButtons[i].m_StateButton.PointerClick.AddListener(delegate
				{
					OnSelectFilter(targetFilterType);
				});
			}
		}
		if (list.Count < m_lstFilterButtons.Count)
		{
			for (int num = list.Count; num < m_lstFilterButtons.Count; num++)
			{
				NKCUtil.SetGameobjectActive(m_lstFilterButtons[num].m_StateButton, bValue: false);
			}
		}
		SetFilterUI(NKM_UNIT_STYLE_TYPE.NUST_INVALID);
		SetOpenFilterOption(bOpen: false);
	}

	private void SetFilterUI(NKM_UNIT_STYLE_TYPE filterOption)
	{
		foreach (Text item in m_lstlbFilterOption)
		{
			NKCUtil.SetLabelText(item, NKCUnitSortSystem.GetFilterName(filterOption));
		}
	}

	private void OnSelectFilter(NKM_UNIT_STYLE_TYPE type)
	{
		SetOpenFilterOption(bOpen: false);
		SetFilterUI(type);
		dOnFilterOptionChange?.Invoke(type);
	}
}
