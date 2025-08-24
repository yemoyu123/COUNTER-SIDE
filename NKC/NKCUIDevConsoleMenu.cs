using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleMenu : MonoBehaviour
{
	public delegate void ChangeMainMenu(DEV_CONSOLE_MENU_TYPE newMain);

	public delegate void ChangeSubBtn(DEV_CONSOLE_SUB_MENU newMain);

	public delegate void ChangeSubToggle(DEV_CONSOLE_SUB_MENU newMain, bool bSet);

	[Header("Dummy Object")]
	public GameObject m_pfbButton;

	public GameObject m_pfbCheckBox;

	[Header("Menu Parent")]
	public RectTransform m_rectMainMenu;

	public RectTransform m_rectSubMenu;

	private Dictionary<DEV_CONSOLE_MENU_TYPE, List<GameObject>> m_dicSubMenus = new Dictionary<DEV_CONSOLE_MENU_TYPE, List<GameObject>>();

	public void Init(List<ConsoleMainMenu> mainMenus, ChangeMainMenu callMain)
	{
		foreach (ConsoleMainMenu main in mainMenus)
		{
			GameObject buttonObj = GetButtonObj(m_rectMainMenu);
			if (!(null != buttonObj))
			{
				continue;
			}
			SetUIStringKey(buttonObj, main.strKey, Color.black);
			NKCUIComStateButton component = buttonObj.GetComponent<NKCUIComStateButton>();
			if (null != component)
			{
				NKCUtil.SetBindFunction(component, delegate
				{
					OnClickMainMenu(main.type);
					callMain(main.type);
				});
			}
		}
	}

	public void OnClickMainMenu(DEV_CONSOLE_MENU_TYPE newMain)
	{
		foreach (KeyValuePair<DEV_CONSOLE_MENU_TYPE, List<GameObject>> dicSubMenu in m_dicSubMenus)
		{
			bool bValue = dicSubMenu.Key == newMain;
			foreach (GameObject item in dicSubMenu.Value)
			{
				NKCUtil.SetGameobjectActive(item, bValue);
			}
		}
	}

	public void Init(DEV_CONSOLE_MENU_TYPE mainType, List<ConsoleSubMenu> subMenus, ChangeSubBtn callSub, ChangeSubToggle callToggle)
	{
		foreach (ConsoleSubMenu slot in subMenus)
		{
			if (slot.stype == SUB_MENU_TYPE.BUTTON)
			{
				GameObject buttonObj = GetButtonObj(m_rectSubMenu);
				SetUIStringKey(buttonObj, slot.strKey, slot.bWarning ? Color.red : Color.black);
				AddSubMenuObject(mainType, buttonObj);
				NKCUIComStateButton component = buttonObj.GetComponent<NKCUIComStateButton>();
				if (null != component)
				{
					NKCUtil.SetBindFunction(component, delegate
					{
						Debug.Log($"[DevConsoleMenu]Select Sub Menu : {slot.type}");
						callSub(slot.type);
					});
				}
				NKCUtil.SetGameobjectActive(buttonObj, bValue: false);
			}
			if (slot.stype != SUB_MENU_TYPE.CHECK_BOX)
			{
				continue;
			}
			GameObject toggleObj = GetToggleObj(m_rectSubMenu);
			SetUIStringKey(toggleObj, slot.strKey, Color.white);
			AddSubMenuObject(mainType, toggleObj);
			NKCUIComToggle toggle = toggleObj.GetComponent<NKCUIComToggle>();
			if (null != toggle)
			{
				NKCUtil.SetToggleValueChangedDelegate(toggle, delegate(bool x)
				{
					Debug.Log($"[DevConsoleMenu]ChangeSubMenuToggle {slot.type} - {x}");
					callToggle(slot.type, x);
					if (IsForceSelectToggle(slot.type, out var val))
					{
						toggle.Select(val, bForce: true, bImmediate: true);
					}
				});
			}
			NKCUtil.SetGameobjectActive(toggleObj, bValue: false);
		}
	}

	private void AddSubMenuObject(DEV_CONSOLE_MENU_TYPE mainType, GameObject obj)
	{
		if (!m_dicSubMenus.ContainsKey(mainType))
		{
			m_dicSubMenus.Add(mainType, new List<GameObject> { obj });
		}
		else
		{
			m_dicSubMenus[mainType].Add(obj);
		}
	}

	private bool IsForceSelectToggle(DEV_CONSOLE_SUB_MENU subMenu, out bool val)
	{
		val = false;
		if (subMenu == DEV_CONSOLE_SUB_MENU.SHOW_STRING_ID)
		{
			NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
			if (gameOptionData != null)
			{
				val = gameOptionData.UseKeyStringView;
			}
			return true;
		}
		return false;
	}

	public GameObject GetButtonObj(RectTransform rtParent)
	{
		GameObject gameObject = Object.Instantiate(m_pfbButton);
		if (null != gameObject)
		{
			NKCUtil.SetGameobjectActive(gameObject, bValue: true);
			gameObject.transform.SetParent(rtParent, worldPositionStays: false);
		}
		return gameObject;
	}

	public GameObject GetToggleObj(RectTransform rtParent)
	{
		GameObject gameObject = Object.Instantiate(m_pfbCheckBox);
		if (null != gameObject)
		{
			NKCUtil.SetGameobjectActive(gameObject, bValue: true);
			gameObject.transform.SetParent(rtParent, worldPositionStays: false);
		}
		return gameObject;
	}

	public void SetUIStringKey(GameObject targetObj, string strKey, Color _txtColor)
	{
		if (null == targetObj || string.IsNullOrEmpty(strKey))
		{
			return;
		}
		bool num = strKey.Contains("SI_");
		Text componentInChildren = targetObj.GetComponentInChildren<Text>();
		if (!num)
		{
			NKCUtil.SetLabelText(componentInChildren, strKey);
		}
		else
		{
			NKCUIComStringChanger componentInChildren2 = targetObj.GetComponentInChildren<NKCUIComStringChanger>();
			Text componentInChildren3 = targetObj.GetComponentInChildren<Text>();
			if (null != componentInChildren2 && (bool)componentInChildren3)
			{
				TargetStringInfoToChange item = new TargetStringInfoToChange
				{
					m_Key = strKey,
					m_lbText = componentInChildren3
				};
				componentInChildren2.m_lstTargetStringInfoToChange.Add(item);
				componentInChildren2.Translate();
			}
		}
		NKCUtil.SetLabelTextColor(componentInChildren, _txtColor);
	}
}
