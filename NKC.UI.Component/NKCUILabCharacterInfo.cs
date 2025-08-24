using NKM;
using UnityEngine;

namespace NKC.UI.Component;

public class NKCUILabCharacterInfo : MonoBehaviour
{
	public delegate void OnClickChangeCharacterButton();

	public NKCUICharInfoSummary m_UICharInfo;

	public NKCUIComButton m_cbtnChangeCharacter;

	private OnClickChangeCharacterButton dOnClickChangeCharacterButton;

	public void Init(OnClickChangeCharacterButton _dOnClickChangeCharacterButton)
	{
		dOnClickChangeCharacterButton = _dOnClickChangeCharacterButton;
		m_UICharInfo?.SetUnitClassRootActive(value: false);
		if (m_cbtnChangeCharacter != null)
		{
			m_cbtnChangeCharacter.PointerClick.RemoveAllListeners();
			m_cbtnChangeCharacter.PointerClick.AddListener(OnChangeCharacterButton);
		}
		if (m_UICharInfo != null)
		{
			m_UICharInfo.Init();
		}
	}

	public void SetData(NKMUnitData unitData)
	{
		m_UICharInfo.SetData(unitData);
		Debug.Log("unit LB status : " + NKMUnitLimitBreakManager.GetUnitLimitbreakStatus(unitData));
	}

	public void OnChangeCharacterButton()
	{
		if (dOnClickChangeCharacterButton != null)
		{
			dOnClickChangeCharacterButton();
		}
	}
}
