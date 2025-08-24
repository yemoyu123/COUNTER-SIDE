using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIBingoSlot : MonoBehaviour
{
	public delegate void OnClick(int index);

	public NKCUIComStateButton m_btn;

	[Header("숫자")]
	public GameObject m_objNumber;

	public Text m_txtNumber;

	public List<Text> m_lstLbNumber;

	public GameObject m_objOn;

	public GameObject m_objOff;

	public Color m_colorOn;

	public Color m_colorOff;

	[Header("미션")]
	public GameObject m_objMission;

	public Text m_txtMission;

	public Text m_txtMissionTagOn;

	public Text m_txtMissionTagOff;

	public GameObject m_objMissionOn;

	public GameObject m_objMissionOff;

	public Color m_colorMissionOn;

	public Color m_colorMissionOff;

	[Header("선택")]
	public GameObject m_objSelect;

	public GameObject m_objSelectFx;

	public GameObject m_objDisable;

	public Color m_colorSelect;

	[Header("획득")]
	public GameObject m_objGetFx;

	private bool m_isMission;

	private int m_index;

	private bool m_bHave;

	private OnClick dOnClick;

	public bool IsHas => m_bHave;

	public void Init(OnClick onClick)
	{
		dOnClick = onClick;
		if (m_btn != null)
		{
			m_btn.PointerClick.RemoveAllListeners();
			m_btn.PointerClick.AddListener(OnTouch);
		}
	}

	public void SetData(int index, int num, bool bHave, bool isMission)
	{
		m_index = index;
		m_isMission = isMission;
		m_bHave = bHave;
		NKCUtil.SetGameobjectActive(m_objNumber, !m_isMission);
		NKCUtil.SetGameobjectActive(m_txtNumber, !m_isMission);
		NKCUtil.SetGameobjectActive(m_objMission, m_isMission);
		NKCUtil.SetGameobjectActive(m_txtMission, m_isMission);
		if (m_isMission)
		{
			NKCUtil.SetLabelText(m_txtMissionTagOn, num.ToString());
			NKCUtil.SetLabelText(m_txtMissionTagOff, num.ToString());
			NKCUtil.SetGameobjectActive(m_objMissionOn, bHave);
			NKCUtil.SetGameobjectActive(m_objMissionOff, !bHave);
		}
		else
		{
			string msg = num.ToString();
			NKCUtil.SetLabelText(m_txtNumber, msg);
			foreach (Text item in m_lstLbNumber)
			{
				if (!(item == null))
				{
					NKCUtil.SetLabelText(item, msg);
				}
			}
			NKCUtil.SetGameobjectActive(m_objOn, bHave);
			NKCUtil.SetGameobjectActive(m_objOff, !bHave);
		}
		SetTextColor(bSpecialMode: false);
	}

	public void SetSpecialMode(bool bSpecialMode)
	{
		NKCUtil.SetGameobjectActive(m_objSelect, bSpecialMode && !m_isMission && !m_bHave);
		NKCUtil.SetGameobjectActive(m_objDisable, bSpecialMode && (m_isMission || m_bHave));
		SetTextColor(bSpecialMode);
	}

	public void SetSelectFx(bool active)
	{
		NKCUtil.SetGameobjectActive(m_objSelectFx, active);
	}

	public void SetGetFx(bool active)
	{
		NKCUtil.SetGameobjectActive(m_objGetFx, active);
		if (active)
		{
			NKCSoundManager.PlaySound("FX_UI_ATTENDANCE_CHECK", 1f, 0f, 0f);
		}
	}

	private void SetTextColor(bool bSpecialMode)
	{
		if (m_bHave)
		{
			NKCUtil.SetLabelTextColor(m_txtNumber, m_colorOn);
			NKCUtil.SetLabelTextColor(m_txtMission, m_colorMissionOn);
		}
		else if (bSpecialMode)
		{
			NKCUtil.SetLabelTextColor(m_txtNumber, m_colorSelect);
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_txtNumber, m_colorOff);
			NKCUtil.SetLabelTextColor(m_txtMission, m_colorMissionOff);
		}
	}

	private void OnTouch()
	{
		dOnClick?.Invoke(m_index);
	}
}
