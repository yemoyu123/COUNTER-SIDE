using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComHotkeyDisplay : MonoBehaviour
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_HOTKEY";

	private const string ASSET_NAME = "NKM_UI_HOTKEY";

	public Text m_lbText;

	private NKCAssetInstanceData m_NKCAssetInstanceData;

	public static void OpenInstance(Transform parent, string text)
	{
		if (!(parent == null) && parent.gameObject.activeInHierarchy)
		{
			NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_HOTKEY", "NKM_UI_HOTKEY");
			NKCUIComHotkeyDisplay component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIComHotkeyDisplay>();
			component.SetPosition(parent);
			component.m_NKCAssetInstanceData = nKCAssetInstanceData;
			component.SetText(text);
		}
	}

	public static void OpenInstance(Transform parent, InGamehotkeyEventType type)
	{
		OpenInstance(parent, NKCInputManager.GetInputManager().GetHotkeyString(type));
	}

	public static void OpenInstance(Transform parent, HotkeyEventType type)
	{
		OpenInstance(parent, NKCInputManager.GetInputManager().GetHotkeyString(type));
	}

	public static void OpenArrowInstance(Transform parent)
	{
		OpenInstance(parent, HotkeyEventType.Left, HotkeyEventType.Down, HotkeyEventType.Up, HotkeyEventType.Right);
	}

	public static void OpenInstance(Transform parent, params HotkeyEventType[] types)
	{
		OpenInstance(parent, MakeHotkeyString(types));
	}

	public static void OpenInstance(ScrollRect sr, Transform trans = null)
	{
		if (!(sr == null) && sr.gameObject.activeInHierarchy)
		{
			if (trans == null)
			{
				trans = ((!(sr.viewport != null)) ? sr.transform : sr.viewport);
			}
			if (sr.vertical && sr.horizontal)
			{
				OpenInstance(trans, HotkeyEventType.Left, HotkeyEventType.Down, HotkeyEventType.Up, HotkeyEventType.Right);
			}
			else if (sr.vertical)
			{
				OpenInstance(trans, HotkeyEventType.Down, HotkeyEventType.Up);
			}
			else if (sr.horizontal)
			{
				OpenInstance(trans, HotkeyEventType.Left, HotkeyEventType.Right);
			}
		}
	}

	public static void OpenInstance(LoopScrollRect sr, Transform trans = null)
	{
		if (!(sr == null) && sr.gameObject.activeInHierarchy)
		{
			if (trans == null)
			{
				trans = ((!(sr.viewport != null)) ? sr.transform : sr.viewport);
			}
			if (sr.vertical && sr.horizontal)
			{
				OpenInstance(trans, HotkeyEventType.Left, HotkeyEventType.Down, HotkeyEventType.Up, HotkeyEventType.Right);
			}
			else if (sr.vertical)
			{
				OpenInstance(trans, HotkeyEventType.Down, HotkeyEventType.Up);
			}
			else if (sr.horizontal)
			{
				OpenInstance(trans, HotkeyEventType.Left, HotkeyEventType.Right);
			}
		}
	}

	private static string MakeHotkeyString(HotkeyEventType[] types)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		for (int i = 0; i < types.Length; i++)
		{
			string hotkeyString = NKCInputManager.GetInputManager().GetHotkeyString(types[i]);
			if (!string.IsNullOrEmpty(hotkeyString))
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(hotkeyString);
			}
		}
		return stringBuilder.ToString();
	}

	private void SetPosition(Transform parent)
	{
		RectTransform uIBaseRect = NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIOverlay);
		base.transform.SetParent(uIBaseRect);
		RectTransform rectTransform = parent as RectTransform;
		if (rectTransform != null)
		{
			base.transform.position = rectTransform.GetCenterWorldPos();
		}
		else
		{
			base.transform.position = parent.position;
		}
	}

	public void SetText(string str)
	{
		NKCUtil.SetLabelText(m_lbText, str);
	}

	private void Update()
	{
		if (!NKCInputManager.IsHotkeyPressed(HotkeyEventType.ShowHotkey))
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		CloseInstance();
	}

	private void CloseInstance()
	{
		if (m_NKCAssetInstanceData != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceData);
			m_NKCAssetInstanceData = null;
		}
	}
}
